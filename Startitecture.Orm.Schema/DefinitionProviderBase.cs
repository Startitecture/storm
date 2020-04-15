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

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Orm.Model;

    /// <summary>
    /// The definition provider base class.
    /// </summary>
    public abstract class DefinitionProviderBase : IEntityDefinitionProvider
    {
        /// <summary>
        /// The cache lock.
        /// </summary>
        private static readonly object CacheLock = new object();

        /// <summary>
        /// The type name policy.
        /// </summary>
        private static readonly CacheItemPolicy ItemPolicy = new CacheItemPolicy { AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration };

        /// <inheritdoc />
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
            var cacheKey = $"{this.GetType().FullName}:{entityType.FullName}:{listName}";
            var result = MemoryCache.Default.GetOrLazyAddExistingWithResult(CacheLock, cacheKey, entityType, this.GetRelationAttributes, ItemPolicy);

            return result.Item;
        }

        /// <inheritdoc />
        public abstract EntityReference GetEntityReference(LambdaExpression attributeExpression);

        /// <inheritdoc />
        public abstract EntityReference GetEntityReference(PropertyInfo propertyInfo);

        /// <summary>
        /// Gets the entity location by reference.
        /// </summary>
        /// <param name="entityReference">
        /// The entity reference.
        /// </param>
        /// <returns>
        /// The <see cref="EntityLocation"/>.
        /// </returns>
        protected EntityLocation GetEntityLocationByReference([NotNull] EntityReference entityReference)
        {
            if (entityReference == null)
            {
                throw new ArgumentNullException(nameof(entityReference));
            }

            var entityType = entityReference.EntityType;
            var entityQualifiedName = this.GetEntityQualifiedName(entityType);

            // Only set the alias if it doesn't match the name.
            var entityAlias = string.Equals(entityReference.EntityAlias, entityQualifiedName.Entity, StringComparison.OrdinalIgnoreCase)
                                  ? null
                                  : entityReference.EntityAlias;

            return new EntityLocation(entityReference.EntityType, entityQualifiedName.Schema, entityQualifiedName.Entity, entityAlias);
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
        protected abstract QualifiedName GetEntityQualifiedName(Type entityType);

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
        /// Gets the ordinal of the attribute.
        /// </summary>
        /// <param name="entityProperty">
        /// The entity property to evaluate.
        /// </param>
        /// <returns>
        /// The ordinal of the property as an <see cref="int"/>.
        /// </returns>
        protected abstract int GetOrdinal(PropertyInfo entityProperty);

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
        /// Determines whether the property is a key or not.
        /// </summary>
        /// <param name="entityProperty">
        /// The property info to evaluate.
        /// </param>
        /// <returns>
        /// <c>true</c> if the property is a key; otherwise, <c>false</c>.
        /// </returns>
        protected abstract bool IsKey(PropertyInfo entityProperty);

        /// <summary>
        /// Determines whether the property is an identity column or not.
        /// </summary>
        /// <param name="entityProperty">
        /// The property info to evaluate.
        /// </param>
        /// <returns>
        /// <c>true</c> if the property is an identity column; otherwise, <c>false</c>.
        /// </returns>
        protected abstract bool IsIdentity(PropertyInfo entityProperty);

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
                           PhysicalName = relatedEntity.PhysicalName ?? propertyInfo.Name
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
                var propertyAlias = string.Concat(relationLocation.Alias ?? relationLocation.Name, '.', propertyName);
                var ordinal = this.GetOrdinal(propertyInfo);
                var isPrimaryKey = this.IsKey(propertyInfo);
                var isIdentity = this.IsIdentity(propertyInfo);

                var attributeTypes = EntityAttributeTypes.RelatedAttribute;

                if (isPrimaryKey)
                {
                    attributeTypes |= EntityAttributeTypes.PrimaryKey;
                }

                if (isIdentity)
                {
                    attributeTypes |= EntityAttributeTypes.IdentityColumn;
                }

                var entityAttributeDefinition = new EntityAttributeDefinition(
                    entityPath,
                    propertyInfo,
                    physicalName,
                    attributeTypes,
                    ordinal,
                    propertyName == propertyAlias ? null : propertyAlias);

                yield return entityAttributeDefinition;
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

                var entityAttributeDefinition = new EntityAttributeDefinition(
                    entityPath,
                    propertyInfo,
                    attributeReference.PhysicalName,
                    EntityAttributeTypes.RelatedAttribute,
                    int.MaxValue,
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
                    int.MaxValue,
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
            var entityReference = new EntityReference { EntityType = entityType };
            var entityLocation = this.GetEntityLocation(entityReference);

            var entityPath = new LinkedList<EntityLocation>();
            entityPath.AddLast(entityLocation);

            var attributeReferences = this.GetAttributes(entityType);

            foreach (var attributeReference in attributeReferences)
            {
                var physicalName = this.GetPhysicalName(attributeReference.PropertyInfo);
                var ordinal = this.GetOrdinal(attributeReference.PropertyInfo);
                var attributeName = physicalName;
                var isPrimaryKey = this.IsKey(attributeReference.PropertyInfo);
                var isIdentity = this.IsIdentity(attributeReference.PropertyInfo);

                var attributeTypes = EntityAttributeTypes.None;

                if (isPrimaryKey)
                {
                    attributeTypes |= EntityAttributeTypes.PrimaryKey;
                }

                if (isIdentity)
                {
                    attributeTypes |= EntityAttributeTypes.IdentityColumn;
                }

                if (attributeReference.IgnoreReference)
                {
                    attributeTypes |= EntityAttributeTypes.MappedAttribute;
                }

                if (attributeReference.IsRelatedAttribute)
                {
                    attributeTypes |= EntityAttributeTypes.ExplicitRelatedAttribute;
                    var relatedEntityReference = new EntityReference
                                                     {
                                                         EntityType = attributeReference.EntityReference.EntityType,
                                                         ContainerType = entityType,
                                                         EntityAlias = attributeReference.EntityReference.EntityAlias
                                                     };

                    var relatedLocation = this.GetEntityLocation(relatedEntityReference);

                    // This is not a physical object on the POCO, so we indicate it as virtual.
                    relatedLocation.IsVirtual = true;

                    entityPath.AddLast(relatedLocation);

                    // Use the physical name if overridden.
                    physicalName = attributeReference.PhysicalName ?? physicalName;

                    var entityAttributeDefinition = new EntityAttributeDefinition(
                        entityPath,
                        attributeReference.PropertyInfo,
                        physicalName,
                        attributeTypes,
                        int.MaxValue);

                    entityPath.RemoveLast();

                    yield return entityAttributeDefinition;
                }
                else if (attributeReference.IsRelation)
                {
                    attributeTypes |= EntityAttributeTypes.Relation;

                    // Include the relation itself for quick access to getter/setter methods.
                    var entityAttributeDefinition = new EntityAttributeDefinition(
                        entityPath,
                        attributeReference.PropertyInfo,
                        attributeName,
                        attributeTypes,
                        ordinal,
                        attributeReference.Name);

                    yield return entityAttributeDefinition;

                    foreach (var definition in this.GetEntityDefinitions(entityPath, attributeReference.PropertyInfo))
                    {
                        yield return definition;
                    }
                }
                else
                {
                    attributeTypes |= EntityAttributeTypes.DirectAttribute;
                    var entityAttributeDefinition = new EntityAttributeDefinition(
                        entityPath,
                        attributeReference.PropertyInfo,
                        attributeName,
                        attributeTypes,
                        ordinal);

                    yield return entityAttributeDefinition;
                }
            }
        }
    }
}