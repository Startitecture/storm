// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataAnnotationsDefinitionProviderTests.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Defines the DataAnnotationsDefinitionProviderTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Schema.Tests
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.Caching;

    using JetBrains.Annotations;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Core;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Testing.Entities;

    /// <summary>
    /// The data annotations definition provider tests.
    /// </summary>
    [TestClass]
    public class DataAnnotationsDefinitionProviderTests
    {
        /// <summary>
        /// The cache lock.
        /// </summary>
        private static readonly object CacheLock = new object();

        /// <summary>
        /// The type name policy.
        /// </summary>
        private static readonly CacheItemPolicy ItemPolicy = new CacheItemPolicy { AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration };

        /// <summary>
        /// The get entity reference test.
        /// </summary>
        [TestMethod]
        public void GetEntityReference_DirectAttributeWithoutEntityAlias_MatchesExpected()
        {
            var target = new DataAnnotationsDefinitionProvider();
            Expression<Func<ComplexRaisedRow, object>> expression = row => row.FakeOtherEnumerationId;
            var expected = new EntityReference
                               {
                                   ContainerType = null,
                                   EntityType = typeof(ComplexRaisedRow),
                                   EntityAlias = null
                               };
            var actual = target.GetEntityReference(expression);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// The get entity reference test.
        /// </summary>
        [TestMethod]
        public void GetEntityReference_RelatedAttributeWithEntityAlias_MatchesExpected()
        {
            var target = new DataAnnotationsDefinitionProvider();
            Expression<Func<ComplexRaisedRow, object>> expression = row => row.SubEntity.FakeSubSubEntityId;
            var expected = new EntityReference
                               {
                                   ContainerType = null,
                                   EntityType = typeof(SubRow),
                                   EntityAlias = nameof(ComplexRaisedRow.SubEntity)
                               };

            var actual = target.GetEntityReference(expression);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// The get entity reference test.
        /// </summary>
        [TestMethod]
        public void GetEntityReference_FlatRelatedAttribute_MatchesExpected()
        {
            var target = new DataAnnotationsDefinitionProvider();
            Expression<Func<ComplexFlatRow, object>> expression = row => row.FakeSubEntityUniqueName;
            var expected = new EntityReference
                               {
                                   ContainerType = null,
                                   EntityType = typeof(SubRow),
                                   EntityAlias = null
                               };

            var actual = target.GetEntityReference(expression);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// The get entity reference test.
        /// </summary>
        [TestMethod]
        public void GetEntityReference_FlatRelatedAttributeWithEntityAlias_MatchesExpected()
        {
            var target = new DataAnnotationsDefinitionProvider();
            Expression<Func<ComplexFlatRow, object>> expression = row => row.CreatedByUniqueName;
            var expected = new EntityReference
                               {
                                   ContainerType = null,
                                   EntityType = typeof(MultiReferenceRow),
                                   EntityAlias = "CreatedBy"
                               };

            var actual = target.GetEntityReference(expression);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// The get entity reference test.
        /// </summary>
        [TestMethod]
        public void GetEntityReference_DirectPropertyWithoutEntityAlias_MatchesExpected()
        {
            var target = new DataAnnotationsDefinitionProvider();
            var expected = new EntityReference
                               {
                                   ContainerType = null,
                                   EntityType = typeof(ComplexRaisedRow),
                                   EntityAlias = null
                               };
            var actual = target.GetEntityReference(
                typeof(ComplexRaisedRow).GetProperty(nameof(ComplexRaisedRow.FakeOtherEnumerationId)) ?? throw new InvalidOperationException());

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// The get entity reference test.
        /// </summary>
        [TestMethod]
        public void GetEntityReference_FlatRelatedProperty_MatchesExpected()
        {
            var target = new DataAnnotationsDefinitionProvider();
            var expected = new EntityReference
                               {
                                   ContainerType = null,
                                   EntityType = typeof(SubRow),
                                   EntityAlias = null
                               };

            var actual = target.GetEntityReference(
                typeof(ComplexFlatRow).GetProperty(nameof(ComplexFlatRow.FakeSubEntityUniqueName)) ?? throw new InvalidOperationException());

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// The get entity reference test.
        /// </summary>
        [TestMethod]
        public void GetEntityReference_FlatRelatedPropertyInfoWithEntityAlias_MatchesExpected()
        {
            var target = new DataAnnotationsDefinitionProvider();
            var expected = new EntityReference
                               {
                                   ContainerType = null,
                                   EntityType = typeof(MultiReferenceRow),
                                   EntityAlias = "CreatedBy"
                               };

            var actual = target.GetEntityReference(
                typeof(ComplexFlatRow).GetProperty(nameof(ComplexFlatRow.CreatedByUniqueName)) ?? throw new InvalidOperationException());

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// The get entity location reference with alias matches expected.
        /// </summary>
        [TestMethod]
        public void GetEntityLocation_Reference_MatchesExpected()
        {
            var target = new DataAnnotationsDefinitionProvider();
            var expected = new EntityLocation(
                typeof(SubRow),
                typeof(SubRow).GetCustomAttribute<TableAttribute>().Schema ?? typeof(SubRow).Namespace,
                typeof(SubRow).GetCustomAttribute<TableAttribute>()?.Name ?? nameof(SubRow),
                false,
                null);

            var actual = target.GetEntityLocation(
                new EntityReference
                    {
                        ContainerType = null,
                        EntityType = typeof(SubRow),
                        EntityAlias = null
                    });

            Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
        }

        /// <summary>
        /// The get entity location reference with alias matches expected.
        /// </summary>
        [TestMethod]
        public void GetEntityLocation_ReferenceWithAlias_MatchesExpected()
        {
            var target = new DataAnnotationsDefinitionProvider();
            var expected = new EntityLocation(
                typeof(MultiReferenceRow),
                typeof(MultiReferenceRow).GetCustomAttribute<TableAttribute>().Schema ?? typeof(MultiReferenceRow).Namespace,
                typeof(MultiReferenceRow).GetCustomAttribute<TableAttribute>()?.Name ?? nameof(MultiReferenceRow),
                false,
                "CreatedBy");

            var actual = target.GetEntityLocation(
                new EntityReference
                    {
                        ContainerType = null,
                        EntityType = typeof(MultiReferenceRow),
                        EntityAlias = "CreatedBy"
                    });

            Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
        }

        /// <summary>
        /// The resolve entity definitions for domain aggregate matches expected.
        /// </summary>
        [TestMethod]
        public void ResolveEntityDefinitions_ForDomainAggregate_MatchesExpected()
        {
            var target = new DataAnnotationsDefinitionProvider();
            var expected = GetAttributeDefinitions(typeof(DomainAggregateRow)).ToList();
            var actual = target.ResolveDefinitions(typeof(DomainAggregateRow)).ToList();
            CollectionAssert.AreEquivalent(expected, actual, string.Join(Environment.NewLine, actual.Except(expected)));
        }

        /// <summary>
        /// Gets the attribute definitions.
        /// </summary>
        /// <param name="entityType">
        /// The entity type.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="EntityAttributeDefinition"/> items.
        /// </returns>
        private static IEnumerable<EntityAttributeDefinition> GetAttributeDefinitions(Type entityType)
        {
            var entityReference = new EntityReference { EntityType = entityType };
            var entityLocation = GetEntityLocation(entityReference);

            var entityPath = new LinkedList<EntityLocation>();
            entityPath.AddLast(entityLocation);

            var attributeReferences = GetAttributeReferences(entityType);

            foreach (var attributeReference in attributeReferences)
            {
                var physicalName = GetPhysicalName(attributeReference.PropertyInfo);
                var ordinal = GetOrdinal(attributeReference.PropertyInfo);
                var attributeName = physicalName;
                var isPrimaryKey = IsKey(attributeReference.PropertyInfo);
                var isIdentity = IsIdentity(attributeReference.PropertyInfo);
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

                    var relatedLocation = GetEntityLocation(relatedEntityReference);

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
                        int.MaxValue,
                        attributeReference.PropertyInfo.Name == physicalName ? null : attributeReference.PropertyInfo.Name);

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

                    foreach (var definition in GetAttributeDefinitions(entityPath, attributeReference.PropertyInfo))
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

        /// <summary>
        /// Gets the attribute definitions.
        /// </summary>
        /// <param name="entityPath">
        /// The entity path.
        /// </param>
        /// <param name="entityProperty">
        /// The entity property.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="EntityAttributeDefinition"/> items.
        /// </returns>
        private static IEnumerable<EntityAttributeDefinition> GetAttributeDefinitions(LinkedList<EntityLocation> entityPath, PropertyInfo entityProperty)
        {
            var entityType = entityProperty.PropertyType;
            var relationReference = new EntityReference
                                        {
                                            EntityType = entityType,
                                            ContainerType = entityPath.First?.Value.EntityType,
                                            EntityAlias = entityProperty.Name
                                        };

            var relationLocation = GetEntityLocation(relationReference);

            entityPath.AddLast(relationLocation);

            var entityProperties = GetFilteredEntityProperties(entityType).ToList();

            // Do columns first.
            foreach (var propertyInfo in GetDirectPropertyInfos(entityProperties))
            {
                var propertyName = propertyInfo.Name;
                var physicalName = GetPhysicalName(propertyInfo);
                var propertyAlias = string.Concat(relationLocation.Alias ?? relationLocation.Name, '.', propertyName);
                var ordinal = GetOrdinal(propertyInfo);
                var isPrimaryKey = IsKey(propertyInfo);
                var isIdentity = IsIdentity(propertyInfo);

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
            foreach (var propertyInfo in GetRelatedColumnPropertyInfos(entityProperties))
            {
                var attributeReference = GetRelatedEntityAttributeReference(propertyInfo);

                // This is not a physical object on the POCO, so we indicate it as virtual.
                var relatedLocation = GetEntityLocation(attributeReference.EntityReference);
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
            
            foreach (var propertyInfo in GetRelationPropertyInfos(entityProperties))
            {
                var attributeName = GetPhysicalName(propertyInfo);

                // Include the relation itself for quick access to getter/setter methods.
                var relationAttribute = new EntityAttributeDefinition(
                    entityPath,
                    propertyInfo,
                    attributeName,
                    EntityAttributeTypes.Relation,
                    int.MaxValue,
                    propertyInfo.Name);

                yield return relationAttribute;

                foreach (var entityAttributeDefinition in GetAttributeDefinitions(entityPath, propertyInfo))
                {
                    yield return entityAttributeDefinition;
                }
            }

            entityPath.RemoveLast();
        }

        /// <summary>
        /// Gets the direct property info items.
        /// </summary>
        /// <param name="entityProperties">
        /// The entity properties.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="PropertyInfo"/> items.
        /// </returns>
        private static IEnumerable<PropertyInfo> GetDirectPropertyInfos(IEnumerable<PropertyInfo> entityProperties)
        {
            return entityProperties.Where(x => x.GetCustomAttribute<ColumnAttribute>() != null);
        }

        /// <summary>
        /// Gets the entity location.
        /// </summary>
        /// <param name="entityReference">
        /// The entity reference.
        /// </param>
        /// <returns>
        /// The <see cref="EntityLocation"/>.
        /// </returns>
        private static EntityLocation GetEntityLocation(EntityReference entityReference)
        {
            var cacheKey = $"{typeof(IEntityDefinition).FullName}:{entityReference}";
            var result = MemoryCache.Default.GetOrLazyAddExistingWithResult(
                CacheLock,
                cacheKey,
                entityReference,
                GetEntityLocationByReference,
                ItemPolicy);

            return result.Item;
        }

        /// <summary>
        /// Gets the filtered entity properties.
        /// </summary>
        /// <param name="entityType">
        /// The entity type.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="PropertyInfo"/> items.
        /// </returns>
        private static IEnumerable<PropertyInfo> GetFilteredEntityProperties([NotNull] Type entityType)
        {
            return entityType.GetNonIndexedProperties().Where(
                x => x.GetCustomAttribute<ColumnAttribute>() != null || x.GetCustomAttribute<RelatedEntityAttribute>() != null
                                                                     || x.GetCustomAttribute<RelationAttribute>() != null);
        }

        /// <summary>
        /// Gets the related entity attribute reference.
        /// </summary>
        /// <param name="propertyInfo">
        /// The property info.
        /// </param>
        /// <returns>
        /// The <see cref="AttributeReference"/>.
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
        /// Gets the relation property info items.
        /// </summary>
        /// <param name="entityProperties">
        /// The entity properties.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="PropertyInfo"/> items.
        /// </returns>
        private static IEnumerable<PropertyInfo> GetRelationPropertyInfos(IEnumerable<PropertyInfo> entityProperties)
        {
            return entityProperties.Where(x => x.GetCustomAttribute<RelationAttribute>() != null);
        }

        /// <summary>
        /// Gets the related column property info items.
        /// </summary>
        /// <param name="entityProperties">
        /// The entity properties.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="PropertyInfo"/> items.
        /// </returns>
        private static IEnumerable<PropertyInfo> GetRelatedColumnPropertyInfos(IEnumerable<PropertyInfo> entityProperties)
        {
            return entityProperties.Where(x => x.GetCustomAttribute<RelatedEntityAttribute>() != null);
        }

        /// <summary>
        /// Gets the attribute references.
        /// </summary>
        /// <param name="entityType">
        /// The entity type.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="AttributeReference"/> items.
        /// </returns>
        private static IEnumerable<AttributeReference> GetAttributeReferences([NotNull] Type entityType)
        {
            foreach (var propertyInfo in entityType.GetNonIndexedProperties())
            {
                if (propertyInfo.GetCustomAttribute<IgnoreAttribute>() != null)
                {
                    continue;
                }

                // Ignore collections and interfaces. 
                if (propertyInfo.PropertyType.IsInterface || propertyInfo.PropertyType.IsSubclassOf(typeof(IEnumerable<>)))
                {
                    continue;
                }

                var isIdentity = propertyInfo.GetCustomAttribute<DatabaseGeneratedAttribute>()?.DatabaseGeneratedOption
                                 == DatabaseGeneratedOption.Identity;

                yield return new AttributeReference
                                 {
                                     EntityReference = GetEntityReference(propertyInfo),
                                     IsIdentity = isIdentity,
                                     IsPrimaryKey = propertyInfo.GetCustomAttribute<KeyAttribute>() != null,
                                     Name = propertyInfo.Name,
                                     PropertyInfo = propertyInfo,
                                     PhysicalName = GetPhysicalName(propertyInfo),
                                     IsRelatedAttribute = propertyInfo.GetCustomAttribute<RelatedEntityAttribute>() != null,
                                     IsRelation = propertyInfo.GetCustomAttribute<RelationAttribute>() != null
                                 };
            }
        }

        /// <summary>
        /// Gets the entity reference.
        /// </summary>
        /// <param name="propertyInfo">
        /// The property info.
        /// </param>
        /// <returns>
        /// The <see cref="EntityReference"/>.
        /// </returns>
        private static EntityReference GetEntityReference([NotNull] PropertyInfo propertyInfo)
        {
            var relatedEntityAttribute = propertyInfo.GetCustomAttribute<RelatedEntityAttribute>();
            var relationAttribute = propertyInfo.GetCustomAttribute<RelationAttribute>();

            if (relatedEntityAttribute != null)
            {
                return new EntityReference { EntityType = relatedEntityAttribute.EntityType, EntityAlias = relatedEntityAttribute.EntityAlias };
            }

            return relationAttribute != null
                       ? new EntityReference { EntityType = propertyInfo.PropertyType, EntityAlias = propertyInfo.Name }
                       : new EntityReference { EntityType = propertyInfo.DeclaringType };
        }

        /// <summary>
        /// Gets the physical name.
        /// </summary>
        /// <param name="entityProperty">
        /// The entity property.
        /// </param>
        /// <returns>
        /// The physical name as a <see cref="string"/>.
        /// </returns>
        private static string GetPhysicalName([NotNull] PropertyInfo entityProperty)
        {
            var columnAttribute = entityProperty.GetCustomAttribute<ColumnAttribute>();
            var relatedEntityAttribute = entityProperty.GetCustomAttribute<RelatedEntityAttribute>();
            var relationAttribute = entityProperty.GetCustomAttribute<RelationAttribute>();

            var physicalName = entityProperty.Name;

            if (string.IsNullOrWhiteSpace(columnAttribute?.Name) == false)
            {
                physicalName = columnAttribute.Name;
            }
            else if (string.IsNullOrWhiteSpace(relatedEntityAttribute?.PhysicalName) == false)
            {
                physicalName = relatedEntityAttribute.PhysicalName;
            }
            else if (relationAttribute != null)
            {
                // Treat as an entity location.
                var entityReference = new EntityReference { EntityType = entityProperty.PropertyType, EntityAlias = entityProperty.Name };
                physicalName = GetEntityLocationByReference(entityReference).Name;
            }

            return physicalName;
        }

        /// <summary>
        /// Gets the entity location by reference.
        /// </summary>
        /// <param name="entityReference">
        /// The entity reference.
        /// </param>
        /// <returns>
        /// The <see cref="EntityLocation"/>.
        /// </returns>
        private static EntityLocation GetEntityLocationByReference([NotNull] EntityReference entityReference)
        {
            var entityType = entityReference.EntityType;
            var entityQualifiedName = GetEntityQualifiedName(entityType);

            // Only set the alias if it doesn't match the name.
            var entityAlias = string.Equals(entityReference.EntityAlias, entityQualifiedName.Entity, StringComparison.OrdinalIgnoreCase)
                                  ? null
                                  : entityReference.EntityAlias;

            return new EntityLocation(entityReference.EntityType, entityQualifiedName.Schema, entityQualifiedName.Entity, entityAlias);
        }

        /// <summary>
        /// Gets the entity qualified name.
        /// </summary>
        /// <param name="entityType">
        /// The entity type.
        /// </param>
        /// <returns>
        /// The <see cref="QualifiedName"/>.
        /// </returns>
        private static QualifiedName GetEntityQualifiedName([NotNull] Type entityType)
        {
            var sourceTableNameAttribute = entityType.GetCustomAttributes<TableAttribute>(true).FirstOrDefault();

            return new QualifiedName(
                null,
                sourceTableNameAttribute?.Schema ?? entityType.Namespace,
                sourceTableNameAttribute?.Name ?? entityType.Name,
                null);
        }

        /// <summary>
        /// Gets a value indicating whether the <paramref name="entityProperty"/> is a key.
        /// </summary>
        /// <param name="entityProperty">
        /// The entity property.
        /// </param>
        /// <returns>
        /// <c>true</c> if the <paramref name="entityProperty"/> is a key; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsKey(MemberInfo entityProperty)
        {
            return entityProperty.GetCustomAttribute<KeyAttribute>() != null;
        }

        /// <summary>
        /// Gets a value indicating whether the <paramref name="entityProperty"/> is an identity.
        /// </summary>
        /// <param name="entityProperty">
        /// The entity property.
        /// </param>
        /// <returns>
        /// <c>true</c> if the <paramref name="entityProperty"/> is an identity column; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsIdentity(MemberInfo entityProperty)
        {
            return entityProperty.GetCustomAttribute<DatabaseGeneratedAttribute>()?.DatabaseGeneratedOption == DatabaseGeneratedOption.Identity;
        }

        /// <summary>
        /// Gets the ordinal.
        /// </summary>
        /// <param name="entityProperty">
        /// The entity property.
        /// </param>
        /// <returns>
        /// The ordinal as an <see cref="int"/>.
        /// </returns>
        private static int GetOrdinal(MemberInfo entityProperty)
        {
            return entityProperty.GetCustomAttribute<ColumnAttribute>()?.Order ?? int.MaxValue;
        }
    }
}