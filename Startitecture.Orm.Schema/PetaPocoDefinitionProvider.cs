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
        protected virtual IEnumerable<PropertyInfo> GetFilteredRelationProperties(Type entityType)
        {
            return GetPocoFilteredEntityProperties(entityType).ToList();
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
        protected override IEnumerable<AttributeReference> GetAttributes(Type entityType)
        {
            var keyAttributes = entityType.GetCustomAttributes<PrimaryKeyAttribute>().ToList();

            foreach (var entityProperty in GetPocoFilteredEntityProperties(entityType))
            {
                var physicalName = this.GetPhysicalName(entityProperty);

                var relatedEntity = entityProperty.GetCustomAttribute<RelatedEntityAttribute>(false);
                var relation = entityProperty.GetCustomAttribute<RelationAttribute>(false);
                var propertyName = entityProperty.Name;

                if (keyAttributes.Select(x => x.ColumnName).Contains(physicalName))
                {
                    var primaryKeyAttribute = keyAttributes.First(x => x.ColumnName == physicalName);

                    yield return new AttributeReference
                                     {
                                         EntityReference = new EntityReference { EntityType = entityType },
                                         Name = primaryKeyAttribute.ColumnName,
                                         PhysicalName = primaryKeyAttribute.ColumnName,
                                         IsIdentity = primaryKeyAttribute.AutoIncrement,
                                         IsPrimaryKey = true,
                                         PropertyInfo = entityProperty
                                     };
                }
                else if (entityProperty.GetCustomAttributes(typeof(IgnoreAttribute), false).Any())
                {
                    yield return new AttributeReference
                                     {
                                         EntityReference = new EntityReference { EntityType = entityType },
                                         IgnoreReference = true,
                                         Name = propertyName,
                                         PhysicalName = physicalName,
                                         PropertyInfo = entityProperty
                                     };
                }
                else if (relatedEntity != null)
                {
                    var entityReference = new EntityReference
                                              {
                                                  EntityType = relatedEntity.EntityType,
                                                  EntityAlias = relatedEntity.EntityAlias
                                              };

                    yield return new AttributeReference
                                     {
                                         EntityReference = entityReference,
                                         Name = propertyName,
                                         PhysicalName = physicalName,
                                         UseAttributeAlias = relatedEntity.UseAttributeAlias,
                                         IsRelatedAttribute = true,
                                         PropertyInfo = entityProperty
                                     };
                }
                else if (relation != null)
                {
                    var entityReference = new EntityReference
                                              {
                                                  EntityType = entityProperty.PropertyType,
                                                  EntityAlias = propertyName
                                              };

                    yield return new AttributeReference
                                     {
                                         EntityReference = entityReference,
                                         Name = propertyName,
                                         PhysicalName = physicalName,
                                         IsRelation = true,
                                         PropertyInfo = entityProperty
                                     };
                }
                else
                {
                    yield return new AttributeReference
                                     {
                                         EntityReference = new EntityReference { EntityType = entityType },
                                         Name = propertyName,
                                         PhysicalName = physicalName,
                                         PropertyInfo = entityProperty
                                     };
                }
            }
        }

        /// <inheritdoc />
        protected override AttributeReference GetAttributeReference(MemberInfo propertyInfo)
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
            return GetPocoFilteredEntityProperties(entityType, RelationPropertyAttributeTypes);
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
        private static IEnumerable<PropertyInfo> GetPocoFilteredEntityProperties(Type entityType, params Type[] attributeTypes)
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
