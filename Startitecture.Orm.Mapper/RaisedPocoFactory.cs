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
        private static readonly Cache<Tuple<string, string, int, int>, Delegate> PocoFactories =
            new Cache<Tuple<string, string, int, int>, Delegate>();

        /// <summary>
        /// The direct factory.
        /// </summary>
        private static readonly FlatPocoFactory DirectFactory = FlatPocoFactory.DirectFactory;

        /// <summary>
        /// The relation properties cache.
        /// </summary>
        private static readonly Cache<string, PropertyInfo> RelationPropertiesCache = new Cache<string, PropertyInfo>();

        /// <summary>
        /// The POCO cache. Not static because we do not want to cache POCOs beyond the connection context.
        /// </summary>
        private readonly Cache<string, object> pocoCache = new Cache<string, object>();

        /// <summary>
        /// The definition provider.
        /// </summary>
        [NotNull]
        private readonly IEntityDefinitionProvider definitionProvider;

        /// <summary>
        /// The name qualifier.
        /// </summary>
        private readonly INameQualifier nameQualifier;

        /// <summary>
        /// Initializes a new instance of the <see cref="RaisedPocoFactory"/> class.
        /// </summary>
        /// <param name="definitionProvider">
        /// The definition provider.
        /// </param>
        /// <param name="nameQualifier">
        /// The name qualifier.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="definitionProvider"/> is not null.
        /// </exception>
        public RaisedPocoFactory([NotNull] IEntityDefinitionProvider definitionProvider, [NotNull] INameQualifier nameQualifier)
        {
            if (definitionProvider == null)
            {
                throw new ArgumentNullException(nameof(definitionProvider));
            }

            if (nameQualifier == null)
            {
                throw new ArgumentNullException(nameof(nameQualifier));
            }

            this.definitionProvider = definitionProvider;
            this.nameQualifier = nameQualifier;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.pocoCache.Dispose();
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
            var entityDefinition = this.definitionProvider.Resolve<T>();
            var qualifiedName = entityDefinition.QualifiedName;
            var typeQualifiedName = $"{typeof(T).FullName}.{qualifiedName}";

            var pocoKey = GetPocoKey(entityDefinition, reader, entityDefinition.PrimaryKeyAttributes.OrderBy(x => x.PhysicalName).ToList());
            var poco = (T)this.pocoCache.Get(pocoKey, () => GetPocoFromReader<T>(dataRequest, typeQualifiedName, entityDefinition, reader));

            var relationAttributes = entityDefinition.AllAttributes.Where(x => x.AttributeTypes == EntityAttributeTypes.Relation);

            foreach (var relationAttribute in relationAttributes)
            {
                // TODO: Change this brittle code. We need to match on aliases to get the right IDs.
                var relatedDefinition = this.definitionProvider.Resolve(relationAttribute.PropertyInfo.PropertyType);
                var relationReferenceName = string.IsNullOrWhiteSpace(relationAttribute.Alias)
                                                ? relatedDefinition.QualifiedName
                                                : $"[{relationAttribute.Alias}]";

                var keyDefinitions = entityDefinition.ReturnableAttributes
                    .Where(x => this.nameQualifier.GetReferenceName(x.Entity) == relationReferenceName && x.IsPrimaryKey).OrderBy(x => x.PhysicalName);

                var relatedPocoKey = GetPocoKey(relatedDefinition, reader, keyDefinitions.ToList());

                var entityReference = new EntityReference
                                          {
                                              ContainerType = typeof(T),
                                              EntityType = relationAttribute.PropertyInfo.PropertyType,
                                              EntityAlias = relationAttribute.Alias
                                          };

                var relatedEntity = this.pocoCache.Get(
                    relatedPocoKey,
                    () => this.GetRelatedEntity(dataRequest, typeQualifiedName, entityDefinition, reader, entityReference));

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
            // TODO: Ensure the order of keys according to the ordinality of the columns to skip re-ordering operations.
            var pocoKeyBuilder = new StringBuilder($"{entityDefinition.EntityContainer}.{entityDefinition.EntityName}");

            if (keyDefinitions.Count == 0)
            {
                throw new OperationException(
                    entityDefinition,
                    $"No keys provided for {entityDefinition.EntityContainer}.{entityDefinition.EntityName}.");
            }

            foreach (var definition in keyDefinitions)
            {
                pocoKeyBuilder.Append($".{definition.PhysicalName}.{record.GetValue(record.GetOrdinal(definition.ReferenceName))}");
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
        /// <param name="typeQualifiedName">
        /// The type qualified name.
        /// </param>
        /// <param name="entityDefinition">
        /// The entity definition.
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
        private static T GetPocoFromReader<T>(
            PocoDataRequest dataRequest,
            string typeQualifiedName,
            IEntityDefinition entityDefinition,
            IDataRecord record)
        {
            var baseKey = new Tuple<string, string, int, int>(typeQualifiedName, entityDefinition.EntityName, 0, record.FieldCount);

            var baseDirectAttributes = entityDefinition.ReturnableAttributes.Where(x => x.IsReferencedDirect).ToList();
            var basePocoDelegate = PocoFactories.Get(baseKey, () => DirectFactory.CreateDelegate(dataRequest, typeof(T), baseDirectAttributes));
            var poco = (T)basePocoDelegate.DynamicInvoke(record);
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
                var propertyInfo = RelationPropertiesCache.Get(relationKey, () => entityType.GetProperty(relationName));

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
        /// <param name="dataRequest">
        /// The data request.
        /// </param>
        /// <param name="typeQualifiedName">
        /// The type qualified name.
        /// </param>
        /// <param name="entityDefinition">
        /// The entity definition.
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
        private object GetRelatedEntity(
            PocoDataRequest dataRequest,
            string typeQualifiedName,
            IEntityDefinition entityDefinition,
            IDataRecord reader,
            EntityReference entityReference)
        {
            var relatedEntityLocation = this.definitionProvider.GetEntityLocation(entityReference);
            var relatedQualifiedName = this.nameQualifier.GetReferenceName(relatedEntityLocation);
            var relatedKey = new Tuple<string, string, int, int>(typeQualifiedName, relatedQualifiedName, 0, reader.FieldCount);

            // TODO: Cache attributes with their locations, or build explicitly.
            var relatedAttributes = entityDefinition.ReturnableAttributes.Where(x => x.ReferenceNode?.Value == relatedEntityLocation).ToList();
            var relatedType = relatedEntityLocation.EntityType;

            var relatedPocoDelegate = PocoFactories.Get(relatedKey, () => DirectFactory.CreateDelegate(dataRequest, relatedType, relatedAttributes));

            var relatedEntity = relatedPocoDelegate.DynamicInvoke(reader);
            return relatedEntity;
        }
    }
}