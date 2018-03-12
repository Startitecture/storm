// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PetaPocoDefinitionProvider.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Contains information about the structure of a data item.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Startitecture.Orm.Schema
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Reflection;

    using Model;

    using Startitecture.Orm.Common;
    using Startitecture.Orm.Query;

    /// <summary>
    /// Contains information about the structure of a data item.
    /// </summary>
    public class PetaPocoDefinitionProvider : DefinitionProviderBase
    {
        #region Static Fields

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

        /// <inheritdoc />
        protected override string GetEntityQualifiedName(Type entityType)
        {
            var sourceTableNameAttribute = entityType.GetCustomAttributes<TableNameAttribute>(true).FirstOrDefault();
            return sourceTableNameAttribute == null ? entityType.Name : sourceTableNameAttribute.TableName ?? entityType.Name;
        }

        /// <inheritdoc />
        /// <summary>
        /// Gets the physical name of the property.
        /// </summary>
        /// <param name="entityProperty">
        /// The entity property to evaluate.
        /// </param>
        /// <returns>
        /// The name of the property as a <see cref="T:System.String" />.
        /// </returns>
        protected override string GetPhysicalName(PropertyInfo entityProperty)
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
                physicalName = this.GetEntityLocationByReference(entityReference).Name;
            }

            return physicalName;
        }

        /// <inheritdoc />
        protected override IEnumerable<EntityAttributeDefinition> GetRelationAttributes(Type entityType)
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
                var physicalName = this.GetPhysicalName(entityProperty);
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

        /// <inheritdoc />
        protected override IEnumerable<AttributeReference> GetKeyAttributes(Type entityType)
        {
            var keyAttributes = entityType.GetCustomAttributes<PrimaryKeyAttribute>().ToList();

            foreach (var primaryKeyAttribute in keyAttributes)
            {
                yield return new AttributeReference
                {
                    EntityReference = new EntityReference { EntityType = entityType },
                    Name = primaryKeyAttribute.ColumnName,
                    PhysicalName = primaryKeyAttribute.ColumnName,
                    IsIdentity = primaryKeyAttribute.AutoIncrement,
                    IsPrimaryKey = true
                };
            }
        }

        /// <inheritdoc />
        protected override AttributeReference GetAttributeReference(MemberInfo propertyInfo)
        {
            var relatedEntity = propertyInfo.GetCustomAttribute<RelatedEntityAttribute>();

            var relatedEntityReference = new EntityReference
            {
                EntityType = relatedEntity.EntityType,
                EntityAlias = relatedEntity.EntityAlias
            };

            return new AttributeReference
            {
                EntityReference = relatedEntityReference,
                Name = propertyInfo.Name,
                UseAttributeAlias = relatedEntity.UseAttributeAlias,
                PhysicalName = relatedEntity.PhysicalName
            };
        }

        /// <inheritdoc />
        protected override IEnumerable<PropertyInfo> GetRelationPropertyInfos(IEnumerable<PropertyInfo> entityProperties)
        {
            return entityProperties.Where(x => x.GetCustomAttribute<RelationAttribute>() != null);
        }

        /// <inheritdoc />
        protected override IEnumerable<PropertyInfo> GetRelatedColumnPropertyInfos(IEnumerable<PropertyInfo> entityProperties)
        {
            return entityProperties.Where(x => x.GetCustomAttribute<RelatedEntityAttribute>() != null);
        }

        /// <inheritdoc />
        protected override IEnumerable<PropertyInfo> GetDirectPropertyInfos(IEnumerable<PropertyInfo> entityProperties)
        {
            return entityProperties.Where(x => x.GetCustomAttribute<ColumnAttribute>() != null);
        }

        /// <inheritdoc />
        protected override IEnumerable<PropertyInfo> GetFilteredEntityProperties(Type entityType)
        {
            return GetFilteredEntityProperties(entityType, RelationPropertyAttributeTypes);
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
    }
}
