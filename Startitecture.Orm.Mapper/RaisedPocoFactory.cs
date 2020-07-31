// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RaisedPocoFactory.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Orm.Model;
    using Startitecture.Resources;

    /// <summary>
    /// Creates POCOs with raised related entity POCOs for POCO data requests.
    /// </summary>
    public sealed class RaisedPocoFactory : IDisposable
    {
        /// <summary>
        /// The POCO factories.
        /// </summary>
        private static readonly MemoryCache<Tuple<string, PocoDataRequest>, PocoDelegateInfo> PocoFactories =
            new MemoryCache<Tuple<string, PocoDataRequest>, PocoDelegateInfo>();

        /// <summary>
        /// The relation properties cache.
        /// </summary>
        private static readonly MemoryCache<string, PropertyInfo> RelationPropertiesMemoryCache = new MemoryCache<string, PropertyInfo>();

        /// <summary>
        /// The POCO cache. Not static because we do not want to cache POCOs beyond the connection context.
        /// </summary>
        private readonly MemoryCache<string, object> pocoMemoryCache = new MemoryCache<string, object>();

        /// <summary>
        /// The definition provider.
        /// </summary>
        [NotNull]
        private readonly IEntityDefinitionProvider definitionProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="RaisedPocoFactory"/> class.
        /// </summary>
        /// <param name="definitionProvider">
        /// The definition provider.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="definitionProvider"/> is not null.
        /// </exception>
        public RaisedPocoFactory([NotNull] IEntityDefinitionProvider definitionProvider)
        {
            this.definitionProvider = definitionProvider ?? throw new ArgumentNullException(nameof(definitionProvider));
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.pocoMemoryCache.Dispose();
        }

        /// <summary>
        /// Creates a POCO from the data reader.
        /// </summary>
        /// <param name="dataRequest">
        /// The data request.
        /// </param>
        /// <typeparam name="T">
        /// The type of POCO to return.
        /// </typeparam>
        /// <returns>
        /// A new POCO of the specified type.
        /// </returns>
        public T CreatePoco<T>([NotNull] PocoDataRequest dataRequest)
        {
            if (dataRequest == null)
            {
                throw new ArgumentNullException(nameof(dataRequest));
            }

            var reader = dataRequest.DataReader;

            if (typeof(T) == typeof(object))
            {
                // TODO: See if we can infer the object graph from the included attributes vs. the flatness.
                return GetPocoFromReader<dynamic>(dataRequest, typeof(T).FullName, dataRequest.DataReader);
            }
            else
            {
                var entityDefinition = this.definitionProvider.Resolve<T>();
                var pocoKey = GetPocoKey(entityDefinition, reader, entityDefinition.PrimaryKeyAttributes.OrderBy(x => x.Ordinal).ToList());
                var poco = (T)this.pocoMemoryCache.Get(pocoKey, () => GetPocoFromReader<T>(dataRequest, entityDefinition.QualifiedName, reader));

                var relationAttributes = entityDefinition.AllAttributes.Where(x => x.AttributeTypes == EntityAttributeTypes.Relation);

                foreach (var relationAttribute in relationAttributes)
                {
                    // TODO: Change this brittle code. We need to match on aliases to get the right IDs.
                    var relatedDefinition = this.definitionProvider.Resolve(relationAttribute.PropertyInfo.PropertyType);
                    var relationReferenceName = string.IsNullOrWhiteSpace(relationAttribute.Alias)
                                                    ? $"{relatedDefinition.EntityContainer}.{relatedDefinition.EntityName}"
                                                    : relationAttribute.Alias;

                    var keyDefinitions = entityDefinition.ReturnableAttributes.Where(
                            x => (string.IsNullOrWhiteSpace(x.Entity.Alias) ? $"{x.Entity.Container}.{x.Entity.Name}" : x.Entity.Alias)
                                 == relationReferenceName && x.IsPrimaryKey)
                        .OrderBy(x => x.Ordinal);

                    var relatedPocoKey = GetPocoKey(relatedDefinition, reader, keyDefinitions.ToList());

                    var entityReference = new EntityReference
                                              {
                                                  ContainerType = typeof(T),
                                                  EntityType = relationAttribute.PropertyInfo.PropertyType,
                                                  EntityAlias = relationAttribute.Alias
                                              };

                    var relatedEntity = this.pocoMemoryCache.Get(
                        relatedPocoKey,
                        () => this.GetRelatedEntity(entityDefinition.QualifiedName, dataRequest, reader, entityReference));

                    // Prevents "ghost" POCO properties when resolving second-order or higher relations.
                    if (relatedEntity == null)
                    {
                        continue;
                    }

                    // Check whether this is a first-order relation.
                    if (relationAttribute.EntityNode == relationAttribute.EntityNode.List.First)
                    {
                        relationAttribute.SetValueDelegate.DynamicInvoke(poco, relatedEntity);
                    }
                    else
                    {
                        var relationContainer = NavigateToEntity(relationAttribute.ReferenceNode, poco);
                        relationAttribute.SetValueDelegate.DynamicInvoke(relationContainer, relatedEntity);
                    }
                }

                return poco;                
            }
        }

        /// <summary>
        /// Gets a POCO key from the data record.
        /// </summary>
        /// <param name="entityDefinition">
        /// The entity definition.
        /// </param>
        /// <param name="record">
        /// The record.
        /// </param>
        /// <param name="keyDefinitions">
        /// The key definitions.
        /// </param>
        /// <returns>
        /// The POCO key as a <see cref="string"/>.
        /// </returns>
        private static string GetPocoKey(
            IEntityDefinition entityDefinition,
            IDataRecord record,
            ICollection<EntityAttributeDefinition> keyDefinitions)
        {
            var pocoKeyBuilder = new StringBuilder($"{entityDefinition.EntityContainer}.{entityDefinition.EntityName}");

            if (keyDefinitions.Count == 0)
            {
                throw new OperationException(
                    entityDefinition,
                    $"No keys provided for {entityDefinition.EntityContainer}.{entityDefinition.EntityName}.");
            }

            foreach (var definition in keyDefinitions)
            {
                try
                {
                    var ordinal = record.GetOrdinal(definition.ReferenceName);
                    var keyValue = record.GetValue(ordinal);
                    pocoKeyBuilder.Append($".{definition.PhysicalName}.{keyValue}");
#if DEBUG
                    Trace.WriteLine($"Got {definition.ReferenceName} key value '{keyValue}' at ordinal {definition.Ordinal}");
#endif
                }
                catch (IndexOutOfRangeException ex)
                {
                    var message = $"The data reader did not have a column for '{definition.ReferenceName}'. "
                                  + "Check that the query has JOINs for all relations.";

                    throw new OperationException(definition, message, ex);
                }
            }

            var pocoKey = pocoKeyBuilder.ToString();
            return pocoKey;
        }

        /// <summary>
        /// Gets a POCO from the data record.
        /// </summary>
        /// <param name="dataRequest">
        /// The data request.
        /// </param>
        /// <param name="entityName">
        /// The entity name.
        /// </param>
        /// <param name="record">
        /// The data record to read.
        /// </param>
        /// <typeparam name="T">
        /// The type of POCO to generate.
        /// </typeparam>
        /// <returns>
        /// The POCO object as a type of <typeparamref name="T"/>.
        /// </returns>
        private static T GetPocoFromReader<T>(PocoDataRequest dataRequest, string entityName, IDataRecord record)
        {
            var baseKey = new Tuple<string, PocoDataRequest>(entityName, dataRequest);
            var baseDirectAttributes = dataRequest.AttributeDefinitions.Where(x => x.IsReferencedDirect).ToList();
            var basePocoDelegate = PocoFactories.Get(baseKey, () => FlatPocoFactory.CreateDelegate(dataRequest, typeof(T), baseDirectAttributes));

            T poco;
#if DEBUG
            try
            {
                ////var recordItems = Enumerable.Range(0, record.FieldCount).Select(i => $"{record.GetName(i)}='{record.GetValue(i)}'");
                ////Trace.WriteLine($"Getting data from record {string.Join(",", recordItems)}");
#endif
                poco = (T)basePocoDelegate.MappingDelegate.DynamicInvoke(record);
#if DEBUG
            }
            catch (TargetInvocationException ex)
            {
                throw new OperationException(basePocoDelegate, $"Error creating a POCO for type {typeof(T)}: {ex.Message}", ex);
            }
#endif
            return poco;
        }

        /// <summary>
        /// Navigates to the specified entity attribute, creating the path of entities on the base POCO.
        /// </summary>
        /// <param name="entityNode">
        /// The node of the entity to navigate to.
        /// </param>
        /// <param name="basePoco">
        /// The base POCO that contains the entity.
        /// </param>
        /// <returns>
        /// The target entity as an <see cref="object"/>.
        /// </returns>
        /// <exception cref="OperationException">
        /// The location path is broken for the entity node.
        /// </exception>
        private static object NavigateToEntity(LinkedListNode<EntityLocation> entityNode, object basePoco)
        {
            // We eventually need to get to the property containing our attribute.
            var targetNode = entityNode;
            var targetLocation = targetNode?.Value;

            // Navigate to the base entity location.
            var baseNode = entityNode.List?.First;

            if (targetLocation == null || baseNode?.Next == null)
            {
                // We shouldn't have gotten a related attribute that doesn't have at least two nodes in the path.
                throw new OperationException(entityNode, ErrorMessages.BrokenPocoAttributePath);
            }

            // This attribute needs to be set on its related POCO. First we get the relation itself.
            var currentEntity = basePoco;
            var currentNode = baseNode;
            var entityLocation = baseNode.Value;

            // Starting at the base location.
            while (entityLocation != targetLocation)
            {
                if (currentNode.Next == null)
                {
                    throw new OperationException(entityLocation, ErrorMessages.BrokenPocoAttributePath);
                }

                // Move to the next location at the next node.
                entityLocation = currentNode.Next.Value;
                var relationName = entityLocation.Alias ?? entityLocation.Name;

                // The property info is specific to the raised property on our POCO.
                var entityType = currentEntity.GetType();
                var relationKey = string.Concat(entityType, '.', relationName);
                var propertyInfo = RelationPropertiesMemoryCache.Get(relationKey, () => entityType.GetProperty(relationName));

                if (propertyInfo == null)
                {
                    // They used RelatedEntityAttribute and this is direct on the POCO.
                    throw new OperationException(currentNode, ErrorMessages.NoMatchingPropertyForRelation);
                }

                var propertyType = propertyInfo.PropertyType;
                var relation = propertyInfo.GetValue(currentEntity);

                // If the relation is not set, then we set it, but only if we have got a value from the reader.
                if (relation == null)
                {
                    if (propertyInfo.CanWrite == false)
                    {
                        var message = string.Format(
                            CultureInfo.CurrentCulture,
                            ErrorMessages.ReadOnlyPropertyCannotBeWrittenTo,
                            propertyInfo.Name,
                            entityType.Name,
                            currentEntity);

                        throw new OperationException(propertyInfo, message);
                    }

                    relation = Activator.CreateInstance(propertyType);
                    propertyInfo.SetMethod.Invoke(currentEntity, new[] { relation });
                }

                currentNode = currentNode.Next;
                currentEntity = relation;
            }

            return currentEntity;
        }

        /// <summary>
        /// Gets a related entity POCO from the reader.
        /// </summary>
        /// <param name="entityAggregateName">
        /// The entity aggregate name.
        /// </param>
        /// <param name="dataRequest">
        /// The data request.
        /// </param>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <param name="entityReference">
        /// The entity reference.
        /// </param>
        /// <returns>
        /// The related POCO as an <see cref="object"/>.
        /// </returns>
        private object GetRelatedEntity(string entityAggregateName, PocoDataRequest dataRequest, IDataRecord reader, EntityReference entityReference)
        {
            var relatedEntityLocation = this.definitionProvider.GetEntityLocation(entityReference);
            var relatedQualifiedName = string.IsNullOrWhiteSpace(relatedEntityLocation.Alias)
                                           ? $"{relatedEntityLocation.Container}.{relatedEntityLocation.Name}"
                                           : relatedEntityLocation.Alias;

            var relatedKey = new Tuple<string, PocoDataRequest>($"{entityAggregateName}:{relatedQualifiedName}", dataRequest);

            // TODO: Cache attributes with their locations, or build explicitly.
            ////var relatedAttributes = entityDefinition.ReturnableAttributes.Where(x => x.ReferenceNode?.Value == relatedEntityLocation).ToList();
            var relatedAttributes = dataRequest.AttributeDefinitions.Where(x => x.ReferenceNode?.Value == relatedEntityLocation).ToList();
            var relatedType = relatedEntityLocation.EntityType;

            var relatedPocoDelegate = PocoFactories.Get(
                relatedKey,
                () => FlatPocoFactory.CreateDelegate(dataRequest, relatedType, relatedAttributes));

            object relatedEntity;
#if DEBUG
            try
            {
#endif
                relatedEntity = relatedPocoDelegate.MappingDelegate.DynamicInvoke(reader);
#if DEBUG
            }
            catch (TargetInvocationException ex)
            {
                throw new OperationException(relatedPocoDelegate, $"Error creating a POCO for type {relatedType}: {ex.Message}", ex);
            }
#endif
            return relatedEntity;
        }
    }
}