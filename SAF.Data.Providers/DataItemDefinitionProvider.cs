// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataItemDefinitionProvider.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Contains information about the structure of a data item.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.Data.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Caching;

    using SAF.Core;

    /// <summary>
    /// Contains information about the structure of a data item.
    /// </summary>
    public class DataItemDefinitionProvider : IEntityDefinitionProvider
    {
        /// <summary>
        /// The cache key format.
        /// </summary>
        private const string CacheKeyFormat = "{0}:{1}:{2}";

        /// <summary>
        /// The default schema.
        /// </summary>
        private const string DefaultSchema = "dbo";

        #region Static Fields

        /// <summary>
        /// The cache lock.
        /// </summary>
        private static readonly object CacheLock = new object();

        /// <summary>
        /// The type name policy.
        /// </summary>
        private static readonly CacheItemPolicy ItemPolicy = new CacheItemPolicy { AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration };

        /// <summary>
        /// The key name selector.
        /// </summary>
        private static readonly Func<PrimaryKeyAttribute, string> KeyNameSelector = x => x.ColumnName;

        /// <summary>
        /// The property name selector.
        /// </summary>
        private static readonly Func<PropertyInfo, string> PropertyNameSelector = x => x.Name;

        /// <summary>
        /// The relation property attribute types.
        /// </summary>
        private static readonly Type[] RelationPropertyAttributeTypes =
            {
                typeof(ColumnAttribute),
                typeof(RelationAttribute),
                typeof(RelatedEntityAttribute)
            };

        #endregion

        /// <summary>
        /// Resolves the entity definition for the specified type.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of the item to resolve.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEntityDefinition"/> for the specified type.
        /// </returns>
        public IEntityDefinition Resolve<TItem>()
        {
            return this.Resolve(typeof(TItem));
        }

        /// <summary>
        /// Resolves the entity definition for the specified type.
        /// </summary>
        /// <param name="type">
        /// The type to resolve.
        /// </param>
        /// <returns>
        /// The <see cref="IEntityDefinition"/> for the specified type.
        /// </returns>
        public IEntityDefinition Resolve(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var cacheKey = $"{typeof(IEntityDefinition).FullName}:{type.FullName}";
            var result = MemoryCache.Default.GetOrLazyAddExistingWithResult(CacheLock, cacheKey, type, this.CreateEntityDefinition, ItemPolicy);
            return result.Item;
        }

        /// <summary>
        /// Gets the entity location for the specified type.
        /// </summary>
        /// <param name="entityReference">
        /// The entity reference to retrieve the location for.
        /// </param>
        /// <returns>
        /// An <see cref="EntityLocation"/> instance with the location of the entity.
        /// </returns>
        public EntityLocation GetEntityLocation(EntityReference entityReference)
        {
            if (entityReference == null)
            {
                throw new ArgumentNullException(nameof(entityReference));
            }

            var cacheKey = $"{typeof(IEntityDefinition).FullName}:{entityReference}";
            var result = MemoryCache.Default.GetOrLazyAddExistingWithResult(CacheLock, cacheKey, entityReference, GetEntityNameByType, ItemPolicy);
            return result.Item;
        }

        /// <summary>
        /// Returns a collection of <see cref="EntityAttributeDefinition"/> elements for the specified entity type.
        /// </summary>
        /// <param name="entityType">
        /// The type of the entity to resolve definitions for.
        /// </param>
        /// <returns>
        /// A collection of <see cref="EntityAttributeDefinition"/> based on the specified <paramref name="entityType"/>.
        /// </returns>
        public IEnumerable<EntityAttributeDefinition> ResolveDefinitions(Type entityType)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException(nameof(entityType));
            }

            var listName = typeof(List<EntityAttributeDefinition>).ToRuntimeName();
            var cacheKey = string.Format(CacheKeyFormat, typeof(IEntityDefinition).FullName, entityType.FullName, listName);
            var result = MemoryCache.Default.GetOrLazyAddExistingWithResult(CacheLock, cacheKey, entityType, this.GetRelationAttributes, ItemPolicy);
            return result.Item;
        }

        /// <summary>
        /// Gets the physical name of the property.
        /// </summary>
        /// <param name="entityProperty">
        /// The entity property to evaluate.
        /// </param>
        /// <returns>
        /// The name of the property as a <see cref="string"/>.
        /// </returns>
        private static string GetPhysicalName(PropertyInfo entityProperty)
        {
            var columnAttribute = entityProperty.GetCustomAttribute<ColumnAttribute>();
            var relationAttribute = entityProperty.GetCustomAttribute<RelationAttribute>();

            var physicalName = entityProperty.Name;

            if (string.IsNullOrWhiteSpace(columnAttribute?.Name) == false)
            {
                physicalName = columnAttribute.Name;
            }
            else if (relationAttribute != null)
            {
                // Treat as an entity location.
                var entityReference = new EntityReference { EntityType = entityProperty.PropertyType, EntityAlias = entityProperty.Name };
                physicalName = GetEntityNameByType(entityReference).Name;
            }

            return physicalName;
        }

        /// <summary>
        /// Gets filtered entity properties.
        /// </summary>
        /// <param name="entityType">
        /// The entity type to get the properties for.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of property info objects for the type.
        /// </returns>
        private static IEnumerable<PropertyInfo> GetFilteredEntityProperties(Type entityType)
        {
            return GetFilteredEntityProperties(entityType, new Type[] { });
        }

        /// <summary>
        /// Gets filtered entity properties.
        /// </summary>
        /// <param name="entityType">
        /// The entity type to get the properties for.
        /// </param>
        /// <param name="attributeTypes">
        /// The attribute Types.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of property info objects for the type.
        /// </returns>
        private static IEnumerable<PropertyInfo> GetFilteredEntityProperties(Type entityType, params Type[] attributeTypes)
        {
            var properties = entityType.GetNonIndexedProperties();

            var excludedProperties = new List<string>();

            if (typeof(ITransactionContext).IsAssignableFrom(entityType))
            {
                excludedProperties.AddRange(typeof(ITransactionContext).GetNonIndexedProperties().Select(PropertyNameSelector));
            }

            if (typeof(ICompositeEntity).IsAssignableFrom(entityType))
            {
                excludedProperties.AddRange(typeof(ICompositeEntity).GetNonIndexedProperties().Select(PropertyNameSelector));
            }

            foreach (var propertyInfo in properties)
            {
                if (excludedProperties.Contains(propertyInfo.Name))
                {
                    continue;
                }

                if (attributeTypes.Length == 0)
                {
                    yield return propertyInfo;
                }
                else
                {
                    var customAttributes = propertyInfo.GetCustomAttributes();

                    var matches = from at in attributeTypes
                                  join cat in customAttributes.Select(x => x.GetType()) on at equals cat
                                  select cat;

                    if (matches.Any())
                    {
                        yield return propertyInfo;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the entity name for the specified type.
        /// </summary>
        /// <param name="entityReference">
        /// The related entity to retrieve the location for.
        /// </param>
        /// <returns>
        /// The entity name as a <see cref="string"/>.
        /// </returns>
        private static EntityLocation GetEntityNameByType(EntityReference entityReference)
        {
            var entityType = entityReference.EntityType;
            var sourceTableNameAttribute = entityType.GetCustomAttributes<TableNameAttribute>(true).FirstOrDefault();
            var entityQualifiedName = sourceTableNameAttribute == null
                                          ? entityType.Name
                                          : sourceTableNameAttribute.TableName ?? entityType.Name;

            // Here we assume no servers are included.
            var locationTokens = entityQualifiedName.Split(new[] { '.' }, 2, StringSplitOptions.RemoveEmptyEntries);

            // Remove delimiters so we can expect consistent names.
            var nameToken = locationTokens.Last().Trim('[', ']');

            // Only set the alias if it doesn't match the name.
            var entityAlias = entityReference.EntityAlias == nameToken ? null : entityReference.EntityAlias;

            switch (locationTokens.Length)
            {
                case 2:
                    return new EntityLocation(entityReference.EntityType, locationTokens.First().Trim('[', ']'), nameToken, entityAlias);

                default:
                    return new EntityLocation(entityReference.EntityType, DefaultSchema, nameToken, entityAlias);
            }
        }

        /// <summary>
        /// Builds entity definitions for the specified type.
        /// </summary>
        /// <param name="entityType">
        /// The entity type to build the definitions for.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="EntityAttributeDefinition"/> items for the specified type.
        /// </returns>
        private IEnumerable<EntityAttributeDefinition> GetRelationAttributes(Type entityType)
        {
            var includedProperties = GetFilteredEntityProperties(entityType).ToList();
            var entityReference = new EntityReference { EntityType = entityType };
            var entityLocation = this.GetEntityLocation(entityReference);

            var entityPath = new LinkedList<EntityLocation>();
            entityPath.AddLast(entityLocation);

            var keyAttributes = entityType.GetCustomAttributes<PrimaryKeyAttribute>().ToList();

            var primaryKeys = new List<string>(keyAttributes.Where(x => x.AutoIncrement == false).Select(KeyNameSelector));
            var autonumberKeys = new List<string>(keyAttributes.Where(x => x.AutoIncrement).Select(KeyNameSelector));

            foreach (var entityProperty in includedProperties)
            {
                var physicalName = GetPhysicalName(entityProperty);
                var attributeName = physicalName;
                var relatedEntity = entityProperty.GetCustomAttribute<RelatedEntityAttribute>(false);
                var relation = entityProperty.GetCustomAttribute<RelationAttribute>(false);

                if (primaryKeys.Contains(physicalName))
                {
                    var entityAttributeDefinition = new EntityAttributeDefinition(
                                                        entityPath,
                                                        entityProperty,
                                                        attributeName,
                                                        EntityAttributeTypes.DirectPrimaryKey);

                    yield return entityAttributeDefinition;
                }
                else if (autonumberKeys.Contains(physicalName))
                {
                    var entityAttributeDefinition = new EntityAttributeDefinition(
                                                        entityPath,
                                                        entityProperty,
                                                        attributeName,
                                                        EntityAttributeTypes.DirectAutoNumberKey);

                    yield return entityAttributeDefinition;
                }
                else if (entityProperty.GetCustomAttributes(typeof(IgnoreAttribute), false).Any())
                {
                    var entityAttributeDefinition = new EntityAttributeDefinition(
                                                        entityPath,
                                                        entityProperty,
                                                        attributeName,
                                                        EntityAttributeTypes.MappedAttribute);

                    yield return entityAttributeDefinition;
                }
                else if (relatedEntity != null)
                {
                    var relatedEntityReference = new EntityReference
                    {
                        EntityType = relatedEntity.EntityType,
                        ContainerType = entityType,
                        EntityAlias = relatedEntity.EntityAlias
                    };

                    var relatedLocation = this.GetEntityLocation(relatedEntityReference);

                    // This is not a physical object on the POCO, so we indicate it as virtual.
                    relatedLocation.IsVirtual = true;

                    entityPath.AddLast(relatedLocation);

                    var isEntityAlias = string.IsNullOrWhiteSpace(relatedLocation.Alias) == false;
                    var entityIdentifier = isEntityAlias ? relatedLocation.Alias : relatedLocation.Name;

                    // Use the physical name if overridden.
                    physicalName = relatedEntity.PhysicalName ?? physicalName;

                    attributeName = relatedEntity.UseAttributeAlias && physicalName.StartsWith(entityIdentifier)
                                        ? physicalName.Substring(entityIdentifier.Length)
                                        : physicalName;

                    var attributeAlias = relatedEntity.UseAttributeAlias ? entityProperty.Name : null;

                    var entityAttributeDefinition = new EntityAttributeDefinition(
                                                        entityPath,
                                                        entityProperty,
                                                        attributeName,
                                                        EntityAttributeTypes.ExplicitRelatedAttribute,
                                                        attributeAlias);

                    entityPath.RemoveLast();

                    yield return entityAttributeDefinition;
                }
                else if (relation != null)
                {
                    // Include the relation itself for quick access to getter/setter methods.
                    var entityAttributeDefinition = new EntityAttributeDefinition(
                                                        entityPath,
                                                        entityProperty,
                                                        attributeName,
                                                        EntityAttributeTypes.Relation,
                                                        entityProperty.Name);

                    yield return entityAttributeDefinition;

                    foreach (var definition in this.GetEntityDefinitions(entityPath, entityProperty))
                    {
                        yield return definition;
                    }
                }
                else
                {
                    var entityAttributeDefinition = new EntityAttributeDefinition(
                                                        entityPath,
                                                        entityProperty,
                                                        attributeName,
                                                        EntityAttributeTypes.DirectAttribute);

                    yield return entityAttributeDefinition;
                }
            }
        }

        /// <summary>
        /// Gets the entity definitions for the specified entity property.
        /// </summary>
        /// <param name="entityPath">
        /// The entity path.
        /// </param>
        /// <param name="entityProperty">
        /// The entity property to evaluate.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="EntityAttributeDefinition"/> items.
        /// </returns>
        private IEnumerable<EntityAttributeDefinition> GetEntityDefinitions(LinkedList<EntityLocation> entityPath, PropertyInfo entityProperty)
        {
            var entityType = entityProperty.PropertyType;
            var keyAttributes = entityType.GetCustomAttributes<PrimaryKeyAttribute>().ToList();

            var primaryKeys = new List<string>(keyAttributes.Where(x => x.AutoIncrement == false).Select(KeyNameSelector));
            var autonumberKeys = new List<string>(keyAttributes.Where(x => x.AutoIncrement).Select(KeyNameSelector));

            var relationReference = new EntityReference
                                        {
                                            EntityType = entityType,
                                            ContainerType = entityPath.First?.Value.EntityType,
                                            EntityAlias = entityProperty.Name
                                        };

            var relationLocation = this.GetEntityLocation(relationReference);

            entityPath.AddLast(relationLocation);

            var entityProperties = GetFilteredEntityProperties(entityType, RelationPropertyAttributeTypes).ToList();

            // Do columns first.
            foreach (var propertyInfo in entityProperties.Where(x => x.GetCustomAttribute<ColumnAttribute>() != null))
            {
                var propertyName = propertyInfo.Name;
                var physicalName = GetPhysicalName(propertyInfo);
                var attributeName = physicalName;
                var propertyAlias = string.Concat(relationLocation.Alias ?? relationLocation.Name, '.', propertyName);
                var relatedPhysicalName = GetPhysicalName(propertyInfo);

                if (primaryKeys.Contains(physicalName))
                {
                    var entityAttributeDefinition = new EntityAttributeDefinition(
                                                        entityPath,
                                                        propertyInfo,
                                                        attributeName,
                                                        EntityAttributeTypes.RelatedPrimaryKey,
                                                        propertyName == propertyAlias ? null : propertyAlias);

                    yield return entityAttributeDefinition;
                }
                else if (autonumberKeys.Contains(physicalName))
                {
                    var entityAttributeDefinition = new EntityAttributeDefinition(
                                                        entityPath,
                                                        propertyInfo,
                                                        attributeName,
                                                        EntityAttributeTypes.RelatedAutoNumberKey,
                                                        propertyName == propertyAlias ? null : propertyAlias);

                    yield return entityAttributeDefinition;
                }
                else
                {
                    // Adding the dot avoids collisions with FKs.
                    var entityAttributeDefinition = new EntityAttributeDefinition(
                                                        entityPath,
                                                        propertyInfo,
                                                        relatedPhysicalName,
                                                        EntityAttributeTypes.RelatedAttribute,
                                                        propertyName == propertyAlias ? null : propertyAlias);

                    yield return entityAttributeDefinition;
                }
            }

            // Next, handle direct related entity attributes.
            foreach (var propertyInfo in entityProperties.Where(x => x.GetCustomAttribute<RelatedEntityAttribute>() != null))
            {
                var relatedEntity = propertyInfo.GetCustomAttribute<RelatedEntityAttribute>();

                var relatedEntityReference = new EntityReference
                {
                    EntityType = relatedEntity.EntityType,
                    EntityAlias = relatedEntity.EntityAlias
                };

                // This is not a physical object on the POCO, so we indicate it as virtual.
                var relatedLocation = this.GetEntityLocation(relatedEntityReference);
                relatedLocation.IsVirtual = true;

                entityPath.AddLast(relatedLocation);

                var propertyName = propertyInfo.Name;

                // Adding the dot avoids collisions with FKs.
                var propertyAlias = string.Concat(relationLocation.Alias ?? relationLocation.Name, '.', propertyName);
                var relatedPhysicalName = GetPhysicalName(propertyInfo);

                var entityIdentifier = relatedLocation.Alias ?? relatedLocation.Name;
                var attributeName = relatedEntity.UseAttributeAlias && relatedPhysicalName.StartsWith(entityIdentifier)
                    ? relatedPhysicalName.Substring(entityIdentifier.Length)
                    : relatedPhysicalName;

                var entityAttributeDefinition = new EntityAttributeDefinition(
                                                    entityPath,
                                                    propertyInfo,
                                                    attributeName,
                                                    EntityAttributeTypes.RelatedAttribute,
                                                    propertyName == propertyAlias ? null : propertyAlias);

                entityPath.RemoveLast();

                yield return entityAttributeDefinition;
            }
            
            foreach (var propertyInfo in entityProperties.Where(x => x.GetCustomAttribute<RelationAttribute>() != null))
            {
                var attributeName = GetPhysicalName(propertyInfo);

                // Include the relation itself for quick access to getter/setter methods.
                var relationAttribute = new EntityAttributeDefinition(
                                                    entityPath,
                                                    propertyInfo,
                                                    attributeName,
                                                    EntityAttributeTypes.Relation,
                                                    propertyInfo.Name);

                yield return relationAttribute;

                foreach (var entityAttributeDefinition in this.GetEntityDefinitions(entityPath, propertyInfo))
                {
                    yield return entityAttributeDefinition;
                }
            }

            entityPath.RemoveLast();
        }

        /// <summary>
        /// Creates an entity definition for the specified type.
        /// </summary>
        /// <param name="type">
        /// The type to create the definition for.
        /// </param>
        /// <returns>
        /// The <see cref="EntityDefinition"/> for the specified type.
        /// </returns>
        private EntityDefinition CreateEntityDefinition(Type type)
        {
            return new EntityDefinition(this, new EntityReference { EntityType = type });
        }
    }
}
