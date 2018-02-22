// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;

    using AutoMapper;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Resources;

    /// <summary>
    /// Provides a basic mapping configuration for entities and data items.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of entity to map.
    /// </typeparam>
    /// <typeparam name="TDataItem">
    /// The type of data item to map.
    /// </typeparam>
    public class EntityMappingProfile<TEntity, TDataItem> : MappingProfileBase<TEntity, TDataItem>
        where TDataItem : ITransactionContext
    {
        /// <summary>
        /// The row to entity expression.
        /// </summary>
        private readonly IMappingExpression<TDataItem, TEntity> rowToEntityExpression;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityMappingProfile{TEntity,TDataItem}" /> class.
        /// </summary>
        protected EntityMappingProfile()
        {
            this.rowToEntityExpression = this.CreateMap<TDataItem, TEntity>();
        }

        /// <summary>
        /// Gets the row to entity mapping expression.
        /// </summary>
        protected IMappingExpression<TDataItem, TEntity> RowToEntityExpression => this.rowToEntityExpression;

        /// <summary>
        /// Sets a key for the data item from one of the data item properties. There can only be one key per
        /// <typeparamref name="TDataKey"/> type.
        /// </summary>
        /// <param name="entityKey">
        /// The entity Key.
        /// </param>
        /// <param name="dataItemKey">
        /// The data Item Key.
        /// </param>
        /// <typeparam name="TEntityKey">
        /// The type of the entity key property.
        /// </typeparam>
        /// <typeparam name="TDataKey">
        /// The type of the data key property.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="T:Startitecture.Orm.Repository.IEntityMappingProfile`2"/>.
        /// </returns>
        public override sealed IEntityMappingProfile<TEntity, TDataItem> SetPrimaryKey<TEntityKey, TDataKey>(
            Expression<Func<TEntity, TEntityKey>> entityKey,
            Expression<Func<TDataItem, TDataKey>> dataItemKey)
        {
            if (entityKey == null)
            {
                throw new ArgumentNullException(nameof(entityKey));
            }

            if (dataItemKey == null)
            {
                throw new ArgumentNullException(nameof(dataItemKey));
            }

            base.SetPrimaryKey(entityKey, dataItemKey);
            this.rowToEntityExpression.AfterMap((item, entity) => item.SetDependency(dataItemKey, entity));
            return this;
        }

        /*
        /// <summary>
        /// Creates a dependency mapping that automatically adds the dependency to the dependency container after being mapped.
        /// </summary>
        /// <param name="dependencyProperty">
        /// The dependency property to map.
        /// </param>
        /// <param name="dependencyKey">
        /// A function that selects the dependency key from the data item.
        /// </param>
        /// <typeparam name="TDependency">
        /// The type of dependency to map to.
        /// </typeparam>
        /// <typeparam name="TKey">
        /// The type of the dependency key.
        /// </typeparam>
        /// <returns>
        /// An <see cref="IMappingExpression"/> that maps the specified <typeparamref name="TDataItem"/> to the <typeparamref name="TDependency"/>.
        /// </returns>
        [Obsolete("Use CreateRelatedEntityProfile instead.")]
        public IMappingExpression<TDataItem, TDependency> CreateDependencyProfile<TDependency, TKey>(
            Expression<Func<TEntity, TDependency>> dependencyProperty,
            Expression<Func<TDataItem, TKey>> dependencyKey)
        {
            if (typeof(TDependency) == typeof(TEntity))
            {
                throw new BusinessException(dependencyProperty, ValidationMessages.DependencyMappingToSameTypeAsEntity);
            }

            this.rowToEntityExpression.MapDependency(dependencyProperty, dependencyKey);

            return
                this.CreateMap<TDataItem, TDependency>()
                    .AfterMap((item, dependency) => item.SetDependency(dependencyKey, dependency));
        }
*/

        /*
        /// <summary>
        /// Maps an enumeration to its corresponding primary key ID.
        /// </summary>
        /// <param name="enumerationProperty">
        /// The enumeration property.
        /// </param>
        /// <param name="enumerationId">
        /// The enumeration id.
        /// </param>
        /// <typeparam name="TEnum">
        /// The type of enumeration to map.
        /// </typeparam>
        /// <typeparam name="TId">
        /// The type of the ID to map.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="IEntityMappingProfile{TEntity, TDataItem}"/> with the enumeration mapped between the entity and the 
        /// data item.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="enumerationProperty"/> or <paramref name="enumerationId"/> is null.
        /// </exception>
        [Obsolete("Use MapProperty instead.")]
        public IEntityMappingProfile<TEntity, TDataItem> MapEnumeration<TEnum, TId>(
            Expression<Func<TEntity, TEnum>> enumerationProperty,
            Expression<Func<TDataItem, TId>> enumerationId)
        {
            this.MapProperty(enumerationProperty, enumerationId);
            this.MapRowToEntityProperty(enumerationProperty, enumerationId);
            return this;
        }
*/

        /// <summary>
        /// Maps a property to its corresponding row column.
        /// </summary>
        /// <typeparam name="TProperty">
        /// The property to map.
        /// </typeparam>
        /// <typeparam name="TColumn">
        /// The column to map the property to.
        /// </typeparam>
        /// <param name="entityProperty">
        /// The entity property to map.
        /// </param>
        /// <param name="dataItemColumn">
        /// The data column to map to.
        /// </param>
        /// <returns>
        /// The current <see cref="IEntityMappingProfile{TEntity, TDataItem}"/> with the property mapped between the entity and
        /// the
        /// data item.
        /// </returns>
        public override sealed IEntityMappingProfile<TEntity, TDataItem> MapProperty<TProperty, TColumn>(
            [NotNull] Expression<Func<TEntity, TProperty>> entityProperty,
            [NotNull] Expression<Func<TDataItem, TColumn>> dataItemColumn)
        {
            if (entityProperty == null)
            {
                throw new ArgumentNullException(nameof(entityProperty));
            }

            if (dataItemColumn == null)
            {
                throw new ArgumentNullException(nameof(dataItemColumn));
            }

            this.EntityToRowExpression.MapEntityProperty(dataItemColumn, entityProperty);
            this.MapRowToEntityProperty(entityProperty, dataItemColumn);
            return this;
        }

        /// <summary>
        /// Resolves an entity when it cannot be mapped from the data item directly.
        /// </summary>
        /// <param name="relatedEntity">
        /// The related entity property to map.
        /// </param>
        /// <param name="relatedEntityKey">
        /// A function that selects the related entity key from the data item.
        /// </param>
        /// <typeparam name="TRelation">
        /// The type of related entity to map to.
        /// </typeparam>
        /// <typeparam name="TDataKey">
        /// The type of data key.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="IEntityMappingProfile{TEntity, TDataItem}"/>.
        /// </returns>
        public override sealed IEntityMappingProfile<TEntity, TDataItem> ResolveUnmappedEntity<TRelation, TDataKey>(
            [NotNull] Expression<Func<TEntity, TRelation>> relatedEntity,
            [NotNull] Expression<Func<TDataItem, TDataKey>> relatedEntityKey)
        {
            if (relatedEntity == null)
            {
                throw new ArgumentNullException(nameof(relatedEntity));
            }

            if (relatedEntityKey == null)
            {
                throw new ArgumentNullException(nameof(relatedEntityKey));
            }

            this.rowToEntityExpression.ForMember(
                relatedEntity,
                expr => expr.ResolveUsing(item => GetFromContainer<TRelation, TDataKey>(item, relatedEntityKey)));

            return this;
        }

        /// <summary>
        /// Maps a relation
        /// </summary>
        /// <param name="entityProperty">
        /// The entity property to map.
        /// </param>
        /// <param name="entityRelation">
        /// The entity relation on the current data item.
        /// </param>
        /// <param name="relatedEntityKey">
        /// The related entity key.
        /// </param>
        /// <typeparam name="TProperty">
        /// The type of entity property to map.
        /// </typeparam>
        /// <typeparam name="TRelation">
        /// The type of entity relation on the data item.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="IEntityMappingProfile{TEntity, TDataItem}"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="entityProperty"/>, <paramref name="entityRelation"/>, or <paramref name="relatedEntityKey"/> is
        /// null.
        /// </exception>
        public override sealed IEntityMappingProfile<TEntity, TDataItem> MapRelation<TProperty, TRelation>(
            Expression<Func<TEntity, TProperty>> entityProperty,
            Expression<Func<TDataItem, TRelation>> entityRelation,
            Expression<Func<TRelation, object>> relatedEntityKey)
        {
            if (entityProperty == null)
            {
                throw new ArgumentNullException(nameof(entityProperty));
            }

            if (entityRelation == null)
            {
                throw new ArgumentNullException(nameof(entityRelation));
            }

            if (relatedEntityKey == null)
            {
                throw new ArgumentNullException(nameof(relatedEntityKey));
            }

            this.rowToEntityExpression.ForMember(entityProperty, expr => expr.Condition(item => entityRelation.Compile().Invoke(item) != null));

            this.rowToEntityExpression.ForMember(
                entityProperty,
                expr => expr.MapFrom(row => row.ResolveOrMapRelatedEntity<TDataItem, TRelation, TProperty>(entityRelation, relatedEntityKey)));

            var constructorParam = entityProperty.GetConstructorParam();

            if (constructorParam != null)
            {
                this.rowToEntityExpression.ForCtorParam(
                    constructorParam.Name,
                    expr => expr.MapFrom(row => row.ResolveOrMapRelatedEntity<TDataItem, TRelation, TProperty>(entityRelation, relatedEntityKey)));
            }

            return this;
        }

        /*
        /// <summary>
        /// Maps a related entity from the data item.
        /// </summary>
        /// <param name="relatedEntity">
        /// The related entity.
        /// </param>
        /// <param name="relatedEntityKey">
        /// The key of the related entity.
        /// </param>
        /// <param name="typeResolver">
        /// The related entity key.
        /// </param>
        /// <typeparam name="TRelation">
        /// The type of the related entity.
        /// </typeparam>
        /// <typeparam name="TDataKey">
        /// The type of data key.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="IEntityMappingProfile{TEntity, TDataItem}"/>.
        /// </returns>
        [Obsolete]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Allows fluent usage of the method.")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Allows fluent usage of the method.")]
        public IEntityMappingProfile<TEntity, TDataItem> ResolveRelatedEntity<TRelation, TDataKey>(
            [NotNull] Expression<Func<TEntity, TRelation>> relatedEntity,
            [NotNull] Expression<Func<TDataItem, TDataKey>> relatedEntityKey,
            [NotNull] Func<TDataItem, TRelation> typeResolver)
            where TRelation : class
        {
            if (relatedEntity == null)
            {
                throw new ArgumentNullException(nameof(relatedEntity));
            }

            if (relatedEntityKey == null)
            {
                throw new ArgumentNullException(nameof(relatedEntityKey));
            }

            if (typeResolver == null)
            {
                throw new ArgumentNullException(nameof(typeResolver));
            }

            this.rowToEntityExpression.ForMember(
                relatedEntity.GetPropertyName(),
                expr => expr.ResolveUsing(item => item.ConditionallyCreateRelatedEntity(relatedEntityKey, typeResolver)));

            return this;
        }
*/

        /// <summary>
        /// Ignores all the specified entity properties during mapping. This will not suppress mapping of constructor parameters.
        /// </summary>
        /// <param name="propertySelectors">
        /// The properties to ignore.
        /// </param>
        /// <returns>
        /// The current <see cref="IEntityMappingProfile{TEntity, TDataItem}"/> with the specified properties ignored.
        /// </returns>
        public override sealed IEntityMappingProfile<TEntity, TDataItem> IgnoreForEntity(
            [NotNull] params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            if (propertySelectors == null)
            {
                throw new ArgumentNullException(nameof(propertySelectors));
            }

            this.rowToEntityExpression.Ignore(propertySelectors);
            return this;
        }

        /// <summary>
        /// Maps a property between the entity and the data item.
        /// </summary>
        /// <param name="relationProperty">
        /// The relation to create a profile for.
        /// </param>
        /// <param name="relationKey">
        /// The foreign key of the relation.
        /// </param>
        /// <typeparam name="TRelation">
        /// The type of the related entity.
        /// </typeparam>
        /// <typeparam name="TDataKey">
        /// The type of data key.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="RelatedEntityMappingContainer{TDataItem, TRelation}"/> with the specified property mapped.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Allows fluent usage of the method.")]
        public RelatedEntityMappingContainer<TDataItem, TRelation> CreateRelatedEntityProfile<TRelation, TDataKey>(
            [NotNull] Expression<Func<TEntity, TRelation>> relationProperty,
            [NotNull] Expression<Func<TDataItem, TDataKey>> relationKey)
        {
            if (relationProperty == null)
            {
                throw new ArgumentNullException(nameof(relationProperty));
            }

            if (relationKey == null)
            {
                throw new ArgumentNullException(nameof(relationKey));
            }

            if (typeof(TRelation) == typeof(TEntity))
            {
                throw new OperationException(relationProperty, ValidationMessages.DependencyMappingToSameTypeAsEntity);
            }

            var mappingExpression = this.MapRelationDependency(relationProperty, relationKey);
            return new RelatedEntityMappingContainer<TDataItem, TRelation>(mappingExpression);
        }

        /// <summary>
        /// Maps a property between the entity and the data item.
        /// </summary>
        /// <param name="relationProperty">
        /// The relation to create a profile for.
        /// </param>
        /// <param name="relationKey">
        /// The foreign key of the relation.
        /// </param>
        /// <param name="entityKey">
        /// The entity key that corresponds to the relation key.
        /// </param>
        /// <typeparam name="TRelation">
        /// The type of the related entity.
        /// </typeparam>
        /// <typeparam name="TDataKey">
        /// The type of the data key.
        /// </typeparam>
        /// <typeparam name="TEntityKey">
        /// The type of the entity key.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="RelatedEntityMappingContainer{TDataItem, TRelation}"/> with the specified property mapped.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Allows fluent usage of the method.")]
        public RelatedEntityMappingContainer<TDataItem, TRelation> CreateRelatedEntityProfile<TRelation, TDataKey, TEntityKey>(
            [NotNull] Expression<Func<TEntity, TRelation>> relationProperty,
            [NotNull] Expression<Func<TDataItem, TDataKey>> relationKey,
            [NotNull] Expression<Func<TRelation, TEntityKey>> entityKey)
        {
            if (relationProperty == null)
            {
                throw new ArgumentNullException(nameof(relationProperty));
            }

            if (relationKey == null)
            {
                throw new ArgumentNullException(nameof(relationKey));
            }

            if (entityKey == null)
            {
                throw new ArgumentNullException(nameof(entityKey));
            }

            if (typeof(TRelation) == typeof(TEntity))
            {
                throw new OperationException(relationProperty, ValidationMessages.DependencyMappingToSameTypeAsEntity);
            }

            // Also map the key to preserve existing maps. Constructor validation is disabled, we will not be able to support that.
            this.CreateMap<TDataKey, TRelation>().DisableCtorValidation().ForMember(entityKey, expr => expr.MapFrom(source => source))
                .ForAllOtherMembers(expr => expr.Ignore());

            return this.CreateRelatedEntityProfile(relationProperty, relationKey).MapEntityProperty(entityKey, relationKey);
        }

        /// <summary>
        /// Keeps the target property from being mapped.
        /// </summary>
        /// <param name="targetProperty">
        /// The target property.
        /// </param>
        /// <returns>
        /// The current <see cref="IMappingExpression"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Allows fluent usage of the method.")]
        public IMappingExpression<TDataItem, TEntity> DoNotMap([NotNull] Expression<Func<TEntity, object>> targetProperty)
        {
            if (targetProperty == null)
            {
                throw new ArgumentNullException(nameof(targetProperty));
            }

            return this.rowToEntityExpression.ForMember(targetProperty, expr => expr.UseDestinationValue()).Ignore(targetProperty);
        }

        /// <summary>
        /// Gets a relation from the dependency container.
        /// </summary>
        /// <param name="item">
        /// The data item that contains the dependency container.
        /// </param>
        /// <param name="relatedEntityKey">
        /// The related entity key.
        /// </param>
        /// <typeparam name="TRelation">
        /// The type of the relation to get.
        /// </typeparam>
        /// <typeparam name="TDataKey">
        /// The type of the data key to retrieve the item with.
        /// </typeparam>
        /// <returns>
        /// The relation in the dependency container, or null if the dependency can't be found.
        /// </returns>
        private static TRelation GetFromContainer<TRelation, TDataKey>(TDataItem item, Expression<Func<TDataItem, TDataKey>> relatedEntityKey)
            where TRelation : class
        {
            var dataKey = relatedEntityKey.Compile().Invoke(item);

            if (Evaluate.IsDefaultValue(dataKey))
            {
                return default(TRelation);
            }

            return item.TransactionProvider?.DependencyContainer.GetDependency<TRelation>(dataKey);
        }

        /// <summary>
        /// Maps a row to entity property.
        /// </summary>
        /// <param name="entityProperty">
        /// The entity property to map.
        /// </param>
        /// <param name="dataItemColumn">
        /// The data item column that is the source of the value.
        /// </param>
        /// <typeparam name="TProperty">
        /// The type of the entity property.
        /// </typeparam>
        /// <typeparam name="TColumn">
        /// The type of the data item column.
        /// </typeparam>
        private void MapRowToEntityProperty<TProperty, TColumn>(
            Expression<Func<TEntity, TProperty>> entityProperty,
            Expression<Func<TDataItem, TColumn>> dataItemColumn)
        {
            // If there's a setter on the property, then also perform the reverse mapping.
            var propertyInfo = entityProperty.GetProperty();

            if (propertyInfo.SetMethod != null)
            {
                this.rowToEntityExpression.MapEntityProperty(entityProperty, dataItemColumn);
            }
        }

        /// <summary>
        /// Maps a relation dependency.
        /// </summary>
        /// <param name="relationProperty">
        /// The relation property.
        /// </param>
        /// <param name="relationKey">
        /// The relation key.
        /// </param>
        /// <typeparam name="TRelation">
        /// The type of relation to map.
        /// </typeparam>
        /// <typeparam name="TDataKey">
        /// The type of data key.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IMappingExpression"/>.
        /// </returns>
        private IMappingExpression<TDataItem, TRelation> MapRelationDependency<TRelation, TDataKey>(
            Expression<Func<TEntity, TRelation>> relationProperty,
            Expression<Func<TDataItem, TDataKey>> relationKey)
        {
            this.rowToEntityExpression.MapDependency(relationProperty, relationKey);

            // See if there's a constructor parameter for our relation.
            var constructorParam = relationProperty.GetConstructorParam();

            // Probably need to also map dependency here.
            if (constructorParam != null)
            {
                this.rowToEntityExpression.ForCtorParam(
                    constructorParam.Name,
                    expression => expression.MapFrom(item => item.ResolveOrMapRelatedEntity<TDataItem, TDataKey, TRelation>(relationKey)));
            }

            return this.CreateMap<TDataItem, TRelation>().AfterMap((item, dependency) => item.SetDependency(relationKey, dependency));
        }
    }
}