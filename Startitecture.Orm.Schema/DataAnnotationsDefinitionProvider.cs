// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataAnnotationsDefinitionProvider.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The data annotations definition provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Schema
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Model;
    using Startitecture.Resources;

    /// <summary>
    /// The data annotations definition provider.
    /// </summary>
    public class DataAnnotationsDefinitionProvider : DefinitionProviderBase
    {
        /// <inheritdoc />
        public override EntityReference GetEntityReference(LambdaExpression attributeExpression)
        {
            if (attributeExpression == null)
            {
                throw new ArgumentNullException(nameof(attributeExpression));
            }

            var attributeMember = attributeExpression.GetMember();

            if (attributeMember == null)
            {
                throw new OperationException(attributeExpression, ValidationMessages.SelectorCannotBeEvaluated);
            }

            // Check whether there's a RelatedEntityAttribute that will override the natural type.
            // Do not use .Member.DeclaringType in place of .Expression.Type because this could be the base type of an inherited type.
            var relatedEntityAttribute = attributeMember.Member.GetCustomAttribute<RelatedEntityAttribute>();
            var declaringType = relatedEntityAttribute?.EntityType ?? attributeMember.Expression.Type;

            if (declaringType == null)
            {
                throw new OperationException(attributeExpression, ValidationMessages.PropertyMustHaveDeclaringType);
            }

            // This will be null for RelatedEntityAttributes, so we check from the attribute directly (below).
            var entityMember = attributeMember.Expression as MemberExpression;

            // The definition provider will handle identical alias/entity names.
            return new EntityReference { EntityType = declaringType, EntityAlias = relatedEntityAttribute?.EntityAlias ?? entityMember?.Member.Name };
        }

        /// <inheritdoc />
        protected override string GetEntityQualifiedName([NotNull] Type entityType)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException(nameof(entityType));
            }

            var sourceTableNameAttribute = entityType.GetCustomAttributes<TableAttribute>(true).FirstOrDefault();

            // TODO: Create database type provider to do name qualification.
            return sourceTableNameAttribute == null ? entityType.Name : $"[{sourceTableNameAttribute.Schema}].[{sourceTableNameAttribute.Name}]";
        }

        /// <inheritdoc />
        protected override IEnumerable<PropertyInfo> GetFilteredEntityProperties([NotNull] Type entityType)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException(nameof(entityType));
            }

            return entityType.GetNonIndexedProperties().Where(
                x => x.GetCustomAttribute<ColumnAttribute>() != null || x.GetCustomAttribute<RelatedEntityAttribute>() != null
                                                                     || x.GetCustomAttribute<RelationAttribute>() != null);
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
        protected override string GetPhysicalName([NotNull] PropertyInfo entityProperty)
        {
            if (entityProperty == null)
            {
                throw new ArgumentNullException(nameof(entityProperty));
            }

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
        protected override IEnumerable<AttributeReference> GetKeyAttributes([NotNull] Type entityType)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException(nameof(entityType));
            }

            return from propertyInfo in entityType.GetProperties().Where(x => x.GetCustomAttribute<KeyAttribute>() != null)
                   let isIdentity =
                       propertyInfo.GetCustomAttribute<DatabaseGeneratedAttribute>()?.DatabaseGeneratedOption == DatabaseGeneratedOption.Identity
                   select new AttributeReference
                              {
                                  EntityReference = new EntityReference { EntityType = entityType },
                                  Name = propertyInfo.Name,
                                  PropertyInfo = propertyInfo,
                                  PhysicalName = propertyInfo.GetCustomAttribute<ColumnAttribute>()?.Name,
                                  IsIdentity = isIdentity,
                                  IsPrimaryKey = true
                              };
        }

        /// <inheritdoc />
        protected override IEnumerable<AttributeReference> GetAttributes([NotNull] Type entityType)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException(nameof(entityType));
            }

            foreach (var propertyInfo in entityType.GetNonIndexedProperties())
            {
                if (propertyInfo.GetCustomAttribute<IgnoreAttribute>() != null)
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
                                     PhysicalName = this.GetPhysicalName(propertyInfo),
                                     IsRelatedAttribute = propertyInfo.GetCustomAttribute<RelatedEntityAttribute>() != null,
                                     IsRelation = propertyInfo.GetCustomAttribute<RelationAttribute>() != null
                                 };
            }
        }

        /// <summary>
        /// Gets the entity reference for the specified <paramref name="propertyInfo"/>.
        /// </summary>
        /// <param name="propertyInfo">
        /// The property info.
        /// </param>
        /// <returns>
        /// An <see cref="EntityReference"/> for the <paramref name="propertyInfo"/>.
        /// </returns>
        private static EntityReference GetEntityReference(PropertyInfo propertyInfo)
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
    }
}
