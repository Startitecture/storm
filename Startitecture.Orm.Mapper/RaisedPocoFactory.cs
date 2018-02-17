// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RaisedPocoFactory.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Schema;
    using Startitecture.Orm.Sql;
    using Startitecture.Resources;

    /// <summary>
    /// Creates POCOs with raised related entity POCOs for POCO data requests.
    /// </summary>
    public sealed class RaisedPocoFactory : IDisposable
    {
        /// <summary>
        /// The definition provider.
        /// </summary>
        private static readonly DataItemDefinitionProvider DefinitionProvider = Singleton<DataItemDefinitionProvider>.Instance;

        /// <summary>
        /// The poco factories.
        /// </summary>
        private static readonly Cache<Tuple<string, string, int, int>, Delegate> PocoFactories =
            new Cache<Tuple<string, string, int, int>, Delegate>();

        /// <summary>
        /// The direct factory.
        /// </summary>
        private static readonly FlatPocoFactory DirectFactory = FlatPocoFactory.DirectFactory;

        /// <summary>
        /// The get value methods cache.
        /// </summary>
        private readonly Cache<Type, MethodInfo> getValueMethodsCache = new Cache<Type, MethodInfo>();

        /// <summary>
        /// The relation properties cache.
        /// </summary>
        private readonly Cache<string, PropertyInfo> relationPropertiesCache = new Cache<string, PropertyInfo>();

        /// <summary>
        /// The set methods cache.
        /// </summary>
        private readonly Cache<string, MethodInfo> setMethodsCache = new Cache<string, MethodInfo>();

        /// <summary>
        /// The set relation methods cache.
        /// </summary>
        private readonly Cache<string, MethodInfo> setRelationMethodsCache = new Cache<string, MethodInfo>();

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.getValueMethodsCache.Dispose();
            this.relationPropertiesCache.Dispose();
            this.setMethodsCache.Dispose();
            this.setRelationMethodsCache.Dispose();
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
            var entityDefinition = DefinitionProvider.Resolve<T>();
            var qualifiedName = entityDefinition.GetQualifiedName();
            var typeQualifiedName = $"{typeof(T).FullName}.{qualifiedName}";
            var baseKey = new Tuple<string, string, int, int>(typeQualifiedName, entityDefinition.EntityName, 0, reader.FieldCount);

            var baseDirectAttributes = entityDefinition.ReturnableAttributes.Where(x => x.IsReferencedDirect).ToList();
            var basePocoDelegate = PocoFactories.Get(baseKey, () => DirectFactory.CreateDelegate(dataRequest, typeof(T), baseDirectAttributes));
            var poco = (T)basePocoDelegate.DynamicInvoke(reader);

            var relationAttributes = entityDefinition.AllAttributes.Where(x => x.AttributeTypes == EntityAttributeTypes.Relation);

            foreach (var relationAttribute in relationAttributes)
            {
                var entityReference = new EntityReference
                                          {
                                              ContainerType = typeof(T),
                                              EntityType = relationAttribute.PropertyInfo.PropertyType,
                                              EntityAlias = relationAttribute.Alias
                                          };

                var relatedEntityLocation = DefinitionProvider.GetEntityLocation(entityReference);
                var relatedQualifiedName = relatedEntityLocation.GetQualifiedName();
                var relatedKey = new Tuple<string, string, int, int>(typeQualifiedName, relatedQualifiedName, 0, reader.FieldCount);

                // TODO: Cache attributes with their locations, or build explicitly.
                var relatedAttributes = entityDefinition.ReturnableAttributes.Where(x => x.ReferenceNode?.Value == relatedEntityLocation).ToList();
                var relatedType = relatedEntityLocation.EntityType;

                var relatedPocoDelegate = PocoFactories.Get(
                    relatedKey,
                    () => DirectFactory.CreateDelegate(dataRequest, relatedType, relatedAttributes));

                var relatedEntity = relatedPocoDelegate.DynamicInvoke(reader);

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
                    var relationContainer = this.NavigateToEntity(relationAttribute.ReferenceNode, poco);
                    relationAttribute.SetValueDelegate.DynamicInvoke(relationContainer, relatedEntity);
                }
            }

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
        /// <exception cref="Startitecture.Core.OperationException">
        /// The location path is broken for the entity node.
        /// </exception>
        /// <exception cref="ApplicationConfigurationException">
        /// An attempt was made to write to a read-only property.
        /// </exception>
        private object NavigateToEntity(LinkedListNode<EntityLocation> entityNode, object basePoco)
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
                var propertyInfo = this.relationPropertiesCache.Get(relationKey, () => entityType.GetProperty(relationName));

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
                            (string)ErrorMessages.ReadOnlyPropertyCannotBeWrittenTo,
                            propertyInfo.Name,
                            entityType.Name,
                            currentEntity);

                        throw new OperationException(propertyInfo, message);
                    }

                    relation = Activator.CreateInstance((Type)propertyType);
                    propertyInfo.SetMethod.Invoke(currentEntity, new[] { relation });
                }

                currentNode = currentNode.Next;
                currentEntity = relation;
            }

            return currentEntity;
        }
    }
}