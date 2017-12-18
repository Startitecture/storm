// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RelatedEntityMappingContainer.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;

    using AutoMapper;

    using JetBrains.Annotations;

    using SAF.Core;

    /// <summary>
    /// Acts as a mapping expression container for related entity mapping profiles.
    /// </summary>
    /// <typeparam name="TDataItem">
    /// The type of data item that is the source of the mapping.
    /// </typeparam>
    /// <typeparam name="TEntity">
    /// The type of entity that is the target of the mapping.
    /// </typeparam>
    public class RelatedEntityMappingContainer<TDataItem, TEntity>
        where TDataItem : ITransactionContext
    {
        /// <summary>
        /// The mapping expression.
        /// </summary>
        private readonly IMappingExpression<TDataItem, TEntity> mappingExpression;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelatedEntityMappingContainer{TDataItem,TEntity}"/> class.
        /// </summary>
        /// <param name="mappingExpression">
        /// The mapping expression.
        /// </param>
        internal RelatedEntityMappingContainer(IMappingExpression<TDataItem, TEntity> mappingExpression)
        {
            this.mappingExpression = mappingExpression;
        }

        /// <summary>
        /// Maps a related entity from the data item.
        /// </summary>
        /// <param name="relatedEntity">
        /// The related entity.
        /// </param>
        /// <param name="foreignKey">
        /// The foreign Key.
        /// </param>
        /// <typeparam name="TRelation">
        /// The type of the related entity.
        /// </typeparam>
        /// <typeparam name="TKey">
        /// The type of the relation's key.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="RelatedEntityMappingContainer{TDataItem, TEntity}"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Allows fluent usage of the method.")]
        public RelatedEntityMappingContainer<TDataItem, TEntity> MapRelatedEntity<TRelation, TKey>(
            [NotNull] Expression<Func<TEntity, TRelation>> relatedEntity,
            [NotNull] Expression<Func<TDataItem, TKey>> foreignKey)
        {
            if (relatedEntity == null)
            {
                throw new ArgumentNullException(nameof(relatedEntity));
            }

            if (foreignKey == null)
            {
                throw new ArgumentNullException(nameof(foreignKey));
            }

            this.mappingExpression.MapDependency(relatedEntity, foreignKey);

            var constructorParam = relatedEntity.GetConstructorParam();

            if (constructorParam != null)
            {
                this.mappingExpression.ForCtorParam(constructorParam.Name, expression => expression.MapFrom(source => source));
            }

            return this;
        }

        /// <summary>
        /// Resolves an unmapped entity when the entity cannot be mapped from the data item.
        /// </summary>
        /// <param name="relatedEntity">
        /// The related entity.
        /// </param>
        /// <param name="relatedEntityKey">
        /// The related entity key.
        /// </param>
        /// <typeparam name="TRelation">
        /// The type of the related entity.
        /// </typeparam>
        /// <typeparam name="TKey">
        /// The type of the related entity key.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="RelatedEntityMappingContainer{TDataItem, TEntity}"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Allows fluent usage of the method.")]
        public RelatedEntityMappingContainer<TDataItem, TEntity> ResolveUnmappedEntity<TRelation, TKey>(
            Expression<Func<TEntity, TRelation>> relatedEntity, 
            Func<TDataItem, TKey> relatedEntityKey) 
            where TRelation : class
        {
            this.mappingExpression.ForMember(
                relatedEntity.GetPropertyName(),
                expr => expr.MapFrom(item => ConditionallyCreateRelatedEntity<TRelation, TKey>(item, relatedEntityKey)));

            return this;
        }

        /// <summary>
        /// Maps an entity property from the specified data item property.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the source value.
        /// </typeparam>
        /// <typeparam name="TTarget">
        /// The type of the destination value.
        /// </typeparam>
        /// <param name="targetProperty">
        /// The target entity property.
        /// </param>
        /// <param name="sourceProperty">
        /// The source data item property.
        /// </param>
        /// <returns>
        /// The current <see cref="RelatedEntityMappingContainer{TDataItem, TEntity}"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Allows fluent usage of the method.")]
        public RelatedEntityMappingContainer<TDataItem, TEntity> MapEntityProperty<TSource, TTarget>(
            Expression<Func<TEntity, TTarget>> targetProperty, 
            Expression<Func<TDataItem, TSource>> sourceProperty)
        {
            this.mappingExpression.MapEntityProperty(targetProperty, sourceProperty);
            return this;
        }

        /// <summary>
        /// Ignores the specified properties for the mapping.
        /// </summary>
        /// <param name="targetProperties">
        /// The target properties to ignore.
        /// </param>
        /// <returns>
        /// The current <see cref="RelatedEntityMappingContainer{TDataItem, TEntity}"/>.
        /// </returns>
        public RelatedEntityMappingContainer<TDataItem, TEntity> Ignore(params Expression<Func<TEntity, object>>[] targetProperties)
        {
            this.mappingExpression.Ignore(targetProperties);
            return this;
        }

        /// <summary>
        /// Conditionally creates a related entity.
        /// </summary>
        /// <param name="dataItem">
        /// The data item.
        /// </param>
        /// <param name="relatedEntityKey">
        /// The related entity key.
        /// </param>
        /// <typeparam name="TRelation">
        /// The relation to create an entity for.
        /// </typeparam>
        /// <typeparam name="TKey">
        /// The type of the related entity key.
        /// </typeparam>
        /// <returns>
        /// The dependency item as an <see cref="object"/>.
        /// </returns>
        private static TRelation ConditionallyCreateRelatedEntity<TRelation, TKey>(
            [NotNull] TDataItem dataItem,
            [NotNull] Func<TDataItem, TKey> relatedEntityKey)
            where TRelation : class
        {
            var key = relatedEntityKey.Invoke(dataItem);
            return dataItem.TransactionProvider?.DependencyContainer?.GetDependency<TRelation>(key);
        }
    }
}