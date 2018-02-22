// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConstructedEntityMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The constructed entity mapping profile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository
{
    using System;
    using System.Linq.Expressions;

    using Startitecture.Orm.Common;

    /// <summary>
    /// The constructed entity mapping profile.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of entity to map.
    /// </typeparam>
    /// <typeparam name="TDataItem">
    /// The type of data item to map.
    /// </typeparam>
    public class ConstructedEntityMappingProfile<TEntity, TDataItem> : MappingProfileBase<TEntity, TDataItem>
        where TDataItem : ITransactionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructedEntityMappingProfile{TEntity,TDataItem}"/> class.
        /// </summary>
        protected ConstructedEntityMappingProfile()
        {
        }

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
        /// The current <see cref="T:Startitecture.Orm.Repository.EntityMappingProfile`2"/>.
        /// </returns>
        public sealed override IEntityMappingProfile<TEntity, TDataItem> SetPrimaryKey<TEntityKey, TDataKey>(
            Expression<Func<TEntity, TEntityKey>> entityKey,
            Expression<Func<TDataItem, TDataKey>> dataItemKey)
        {
            return base.SetPrimaryKey(entityKey, dataItemKey);
        }

        /// <summary>
        /// Maps a property between the entity and the data item.
        /// </summary>
        /// <param name="entityProperty">
        /// The entity property.
        /// </param>
        /// <param name="dataItemColumn">
        /// The data item column.
        /// </param>
        /// <typeparam name="TProperty">
        /// The type of property to map.
        /// </typeparam>
        /// <typeparam name="TColumn">
        /// The type of column to map.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="IEntityMappingProfile{TEntity, TDataItem}"/> with the specified property mapped.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="entityProperty"/> or <paramref name="dataItemColumn"/> is <c>null</c>.
        /// </exception>
        public override IEntityMappingProfile<TEntity, TDataItem> MapProperty<TProperty, TColumn>(
            Expression<Func<TEntity, TProperty>> entityProperty,
            Expression<Func<TDataItem, TColumn>> dataItemColumn)
        {
            this.EntityToRowExpression.MapEntityProperty(dataItemColumn, entityProperty);
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
        /// <paramref name="entityProperty"/>, <paramref name="entityRelation"/>, or <paramref name="relatedEntityKey"/> is null.
        /// </exception>
        public override IEntityMappingProfile<TEntity, TDataItem> MapRelation<TProperty, TRelation>(
            Expression<Func<TEntity, TProperty>> entityProperty,
            Expression<Func<TDataItem, TRelation>> entityRelation,
            Expression<Func<TRelation, object>> relatedEntityKey)
        {
            throw new NotImplementedException();
        }
    }
}
