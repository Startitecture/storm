// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefinitionProviderBase.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The definition provider base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Schema
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.Caching;

    using Startitecture.Core;
    using Startitecture.Orm.Model;

    /// <summary>
    /// The definition provider base class.
    /// </summary>
    public abstract class DefinitionProviderBase : IEntityDefinitionProvider
    {
        /// <summary>
        /// The cache key format.
        /// </summary>
        private const string CacheKeyFormat = "{0}:{1}:{2}";

        /// <summary>
        /// The default schema.
        /// </summary>
        private const string DefaultSchema = "dbo";

        /// <summary>
        /// The cache lock.
        /// </summary>
        private static readonly object CacheLock = new object();

        /// <summary>
        /// The type name policy.
        /// </summary>
        private static readonly CacheItemPolicy ItemPolicy = new CacheItemPolicy { AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration };

        /// <summary>
        /// Resolves the entity definition for the specified type.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of the item to resolve.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Startitecture.Orm.Model.IEntityDefinition"/> for the specified type.
        /// </returns>
        public IEntityDefinition Resolve<TItem>()
        {
            return this.Resolve(typeof(TItem));
        }

        /// <inheritdoc />
        public IEntityDefinition Resolve(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var cacheKey = $"{this.GetType().FullName}:{type.FullName}";
            var result = MemoryCache.Default.GetOrLazyAddExistingWithResult(CacheLock, cacheKey, type, this.CreateEntityDefinition, ItemPolicy);
            return result.Item;
        }

        /// <inheritdoc />
        public EntityLocation GetEntityLocation(EntityReference entityReference)
        {
            if (entityReference == null)
            {
                throw new ArgumentNullException(nameof(entityReference));
            }

            var cacheKey = $"{typeof(IEntityDefinition).FullName}:{entityReference}";
            var result = MemoryCache.Default.GetOrLazyAddExistingWithResult(
                CacheLock,
                cacheKey,
                entityReference,
                this.GetEntityLocationByReference,
                ItemPolicy);

            return result.Item;
        }

        /// <inheritdoc />
        public IEnumerable<EntityAttributeDefinition> ResolveDefinitions(Type entityType)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException(nameof(entityType));
            }

            var listName = typeof(List<EntityAttributeDefinition>).ToRuntimeName();
            var cacheKey = string.Format(CacheKeyFormat, this.GetType().FullName, entityType.FullName, listName);
            var result = MemoryCache.Default.GetOrLazyAddExistingWithResult(CacheLock, cacheKey, entityType, this.GetRelationAttributes, ItemPolicy);

            return result.Item;
        }

        /// <inheritdoc />
        public abstract EntityReference GetEntityReference(LambdaExpression attributeExpression);

        /// <summary>
        /// Gets the entity location by reference.
        /// </summary>
        /// <param name="entityReference">
        /// The entity reference.
        /// </param>
        /// <returns>
        /// The <see cref="EntityLocation"/>.
        /// </returns>
        protected EntityLocation GetEntityLocationByReference(EntityReference entityReference)
        {
            var entityType = entityReference.EntityType;
            var entityQualifiedName = this.GetEntityQualifiedName(entityType);

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
        /// Gets the entity qualified name for the specified <paramref name="entityType"/>.
        /// </summary>
        /// <param name="entityType">
        /// The entity type.
        /// </param>
        /// <returns>
        /// The qualified name as a <see cref="string"/>.
        /// </returns>
        protected abstract string GetEntityQualifiedName(Type entityType);

        /// <summary>
        /// Gets properties related to direct attributes, related attributes or related entities.
        /// </summary>
        /// <param name="entityType">
        /// The entity type.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="PropertyInfo"/> items.
        /// </returns>
        protected abstract IEnumerable<PropertyInfo> GetFilteredEntityProperties(Type entityType);

        /// <summary>
        /// Gets the relation properties of the entity. These are properties that are complete entities joined to the root entity.
        /// </summary>
        /// <param name="entityProperties">
        /// The entity properties.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="PropertyInfo"/> items.
        /// </returns>
        protected abstract IEnumerable<PropertyInfo> GetRelationPropertyInfos(IEnumerable<PropertyInfo> entityProperties);

        /// <summary>
        /// Gets the related column properties of the entity. These are columns from related entities joined to the root entity.
        /// </summary>
        /// <param name="entityProperties">
        /// The entity properties.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="PropertyInfo"/> items.
        /// </returns>
        protected abstract IEnumerable<PropertyInfo> GetRelatedColumnPropertyInfos(IEnumerable<PropertyInfo> entityProperties);

        /// <summary>
        /// Gets the direct properties of the entity. These are columns directly on the entity itself.
        /// </summary>
        /// <param name="entityProperties">
        /// The entity properties.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="PropertyInfo"/> items.
        /// </returns>
        protected abstract IEnumerable<PropertyInfo> GetDirectPropertyInfos(IEnumerable<PropertyInfo> entityProperties);

        /// <summary>
        /// Gets the physical name of the property.
        /// </summary>
        /// <param name="entityProperty">
        /// The entity property to evaluate.
        /// </param>
        /// <returns>
        /// The name of the property as a <see cref="string"/>.
        /// </returns>
        protected abstract string GetPhysicalName(PropertyInfo entityProperty);

        /// <summary>
        /// Gets the key attributes for the <paramref name="entityType"/>.
        /// </summary>
        /// <param name="entityType">
        /// The entity type.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of attribute references for the <paramref name="entityType"/>.
        /// </returns>
        protected abstract IEnumerable<AttributeReference> GetKeyAttributes(Type entityType);

        /// <summary>
        /// Gets the attribute references for the <paramref name="entityType"/>.
        /// </summary>
        /// <param name="entityType">
        /// The entity type.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of attribute references for the <paramref name="entityType"/>.
        /// </returns>
        protected abstract IEnumerable<AttributeReference> GetAttributes(Type entityType);

        /// <summary>
        /// Gets the related entity attribute reference for the specified <paramref name="propertyInfo"/>.
        /// </summary>
        /// <param name="propertyInfo">
        /// The property info.
        /// </param>
        /// <returns>
        /// The <see cref="AttributeReference"/> for the <paramref name="propertyInfo"/>.
        /// </returns>
        private static AttributeReference GetRelatedEntityAttributeReference(MemberInfo propertyInfo)
        {
            var relatedEntity = propertyInfo.GetCustomAttribute<RelatedEntityAttribute>();
            var relatedEntityReference = new EntityReference { EntityType = relatedEntity.EntityType, EntityAlias = relatedEntity.EntityAlias };

            return new AttributeReference
                       {
                           EntityReference = relatedEntityReference,
                           Name = propertyInfo.Name,
                           UseAttributeAlias = relatedEntity.UseAttributeAlias,
                           PhysicalName = relatedEntity.PhysicalName
                       };
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
            var keyAttributeReferences = this.GetKeyAttributes(entityType).ToList();
            var relationReference = new EntityReference
                                        {
                                            EntityType = entityType,
                                            ContainerType = entityPath.First?.Value.EntityType,
                                            EntityAlias = entityProperty.Name
                                        };

            var relationLocation = this.GetEntityLocation(relationReference);

            entityPath.AddLast(relationLocation);

            var entityProperties = this.GetFilteredEntityProperties(entityType).ToList();

            // Do columns first.
            foreach (var propertyInfo in this.GetDirectPropertyInfos(entityProperties))
            {
                var propertyName = propertyInfo.Name;
                var physicalName = this.GetPhysicalName(propertyInfo);
                var attributeName = physicalName;
                var propertyAlias = string.Concat(relationLocation.Alias ?? relationLocation.Name, '.', propertyName);
                var relatedPhysicalName = this.GetPhysicalName(propertyInfo);

                // TODO: refactor to use AttributeReferences
                var isPrimaryKey = keyAttributeReferences.Where(x => x.IsPrimaryKey && x.IsIdentity == false).Select(x => x.PhysicalName)
                    .Contains(physicalName);

                var isIdentity = keyAttributeReferences.Where(x => x.IsIdentity).Select(x => x.PhysicalName).Contains(physicalName);

                if (isPrimaryKey)
                {
                    var entityAttributeDefinition = new EntityAttributeDefinition(
                        entityPath,
                        propertyInfo,
                        attributeName,
                        EntityAttributeTypes.RelatedPrimaryKey,
                        propertyName == propertyAlias ? null : propertyAlias);

                    yield return entityAttributeDefinition;
                }
                else if (isIdentity)
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
            foreach (var propertyInfo in this.GetRelatedColumnPropertyInfos(entityProperties))
            {
                var attributeReference = GetRelatedEntityAttributeReference(propertyInfo);

                // This is not a physical object on the POCO, so we indicate it as virtual.
                var relatedLocation = this.GetEntityLocation(attributeReference.EntityReference);
                relatedLocation.IsVirtual = true;

                entityPath.AddLast(relatedLocation);

                // Adding the dot avoids collisions with FKs.
                var propertyAlias = string.Concat(relationLocation.Alias ?? relationLocation.Name, '.', attributeReference.Name);
                var relatedPhysicalName = this.GetPhysicalName(propertyInfo);

                var entityIdentifier = relatedLocation.Alias ?? relatedLocation.Name;
                var attributeName = attributeReference.UseAttributeAlias && relatedPhysicalName.StartsWith(entityIdentifier)
                                        ? relatedPhysicalName.Substring(entityIdentifier.Length)
                                        : relatedPhysicalName;

                var entityAttributeDefinition = new EntityAttributeDefinition(
                    entityPath,
                    propertyInfo,
                    attributeName,
                    EntityAttributeTypes.RelatedAttribute,
                    attributeReference.Name == propertyAlias ? null : propertyAlias);

                entityPath.RemoveLast();

                yield return entityAttributeDefinition;
            }
            
            foreach (var propertyInfo in this.GetRelationPropertyInfos(entityProperties))
            {
                var attributeName = this.GetPhysicalName(propertyInfo);

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
        /// The <see cref="Startitecture.Orm.Model.EntityDefinition"/> for the specified type.
        /// </returns>
        private EntityDefinition CreateEntityDefinition(Type type)
        {
            return new EntityDefinition(this, new EntityReference { EntityType = type });
        }

        /// <summary>
        /// Gets the attributes for a relation.
        /// </summary>
        /// <param name="entityType">
        /// The entity type.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="EntityAttributeDefinition"/> items for the <paramref name="entityType"/>.
        /// </returns>
        private IEnumerable<EntityAttributeDefinition> GetRelationAttributes(Type entityType)
        {
            ////var includedProperties = this.GetFilteredRelationProperties(entityType);
            var entityReference = new EntityReference { EntityType = entityType };
            var entityLocation = this.GetEntityLocation(entityReference);

            var entityPath = new LinkedList<EntityLocation>();
            entityPath.AddLast(entityLocation);

            var attributeReferences = this.GetAttributes(entityType);
            var keyAttributeReferences = this.GetKeyAttributes(entityType).ToList();

            foreach (var attributeReference in attributeReferences)
            {
                var physicalName = this.GetPhysicalName(attributeReference.PropertyInfo);
                var attributeName = physicalName;
                ////var relatedEntity = attributeReference.GetCustomAttribute<RelatedEntityAttribute>(false);
                ////var relation = attributeReference.GetCustomAttribute<RelationAttribute>(false);

                var isPrimaryKey = keyAttributeReferences.Where(x => x.IsPrimaryKey && x.IsIdentity == false).Select(x => x.PhysicalName)
                    .Contains(physicalName);

                var isIdentity = keyAttributeReferences.Where(x => x.IsIdentity).Select(x => x.PhysicalName).Contains(physicalName);

                if (isPrimaryKey)
                {
                    var entityAttributeDefinition = new EntityAttributeDefinition(
                        entityPath,
                        attributeReference.PropertyInfo,
                        attributeName,
                        EntityAttributeTypes.DirectPrimaryKey);

                    yield return entityAttributeDefinition;
                }
                else if (isIdentity)
                {
                    var entityAttributeDefinition = new EntityAttributeDefinition(
                        entityPath,
                        attributeReference.PropertyInfo,
                        attributeName,
                        EntityAttributeTypes.DirectAutoNumberKey);

                    yield return entityAttributeDefinition;
                }
                else if (attributeReference.IgnoreReference)
                {
                    var entityAttributeDefinition = new EntityAttributeDefinition(
                        entityPath,
                        attributeReference.PropertyInfo,
                        attributeName,
                        EntityAttributeTypes.MappedAttribute);

                    yield return entityAttributeDefinition;
                }
                else if (attributeReference.IsRelatedAttribute)
                {
                    var relatedEntityReference = new EntityReference
                                                     {
                                                         EntityType = attributeReference.EntityReference.EntityType, // relatedEntity.EntityType,
                                                         ContainerType = entityType,
                                                         EntityAlias = attributeReference.EntityReference.EntityAlias // relatedEntity.EntityAlias
                                                     };

                    var relatedLocation = this.GetEntityLocation(relatedEntityReference);

                    // This is not a physical object on the POCO, so we indicate it as virtual.
                    relatedLocation.IsVirtual = true;

                    entityPath.AddLast(relatedLocation);

                    var isEntityAlias = string.IsNullOrWhiteSpace(relatedLocation.Alias) == false;
                    var entityIdentifier = isEntityAlias ? relatedLocation.Alias : relatedLocation.Name;

                    // Use the physical name if overridden.
                    physicalName = attributeReference.PhysicalName ?? physicalName; // relatedEntity.PhysicalName ?? physicalName;

                    attributeName = attributeReference.UseAttributeAlias && // relatedEntity.UseAttributeAlias && 
                                    physicalName.StartsWith(entityIdentifier)
                                        ? physicalName.Substring(entityIdentifier.Length)
                                        : physicalName;

                    var attributeAlias = attributeReference.UseAttributeAlias ? // relatedEntity.UseAttributeAlias ? 
                                             attributeReference.Name : null;

                    var entityAttributeDefinition = new EntityAttributeDefinition(
                        entityPath,
                        attributeReference.PropertyInfo,
                        attributeName,
                        EntityAttributeTypes.ExplicitRelatedAttribute,
                        attributeAlias);

                    entityPath.RemoveLast();

                    yield return entityAttributeDefinition;
                }
                else if (attributeReference.IsRelation)
                {
                    // Include the relation itself for quick access to getter/setter methods.
                    var entityAttributeDefinition = new EntityAttributeDefinition(
                        entityPath,
                        attributeReference.PropertyInfo,
                        attributeName,
                        EntityAttributeTypes.Relation,
                        attributeReference.Name);

                    yield return entityAttributeDefinition;

                    foreach (var definition in this.GetEntityDefinitions(entityPath, attributeReference.PropertyInfo))
                    {
                        yield return definition;
                    }
                }
                else
                {
                    var entityAttributeDefinition = new EntityAttributeDefinition(
                        entityPath,
                        attributeReference.PropertyInfo,
                        attributeName,
                        EntityAttributeTypes.DirectAttribute);

                    yield return entityAttributeDefinition;
                }
            }
        }
    }
}