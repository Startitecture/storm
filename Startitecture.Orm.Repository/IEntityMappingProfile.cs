// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityMappingProfile.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The EntityMappingProfile interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository
{
    using System;
    using System.Linq.Expressions;

    using AutoMapper;

    using JetBrains.Annotations;

    using Startitecture.Orm.Common;

    /// <summary>
    /// The EntityMappingProfile interface.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of the entity.
    /// </typeparam>
    /// <typeparam name="TDataItem">
    /// The type of the data item.
    /// </typeparam>
    public interface IEntityMappingProfile<TEntity, TDataItem>
        where TDataItem : ITransactionContext
    {
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
        /// The current <see cref="IEntityMappingProfile{TEntity,TDataItem}"/>.
        /// </returns>
        IEntityMappingProfile<TEntity, TDataItem> SetPrimaryKey<TEntityKey, TDataKey>(
            [NotNull] Expression<Func<TEntity, TEntityKey>> entityKey,
            [NotNull] Expression<Func<TDataItem, TDataKey>> dataItemKey);

        /// <summary>
        /// Sets the data item properties that should only be written once, such as creation information.
        /// </summary>
        /// <param name="writeOnceColumns">
        /// The write once columns.
        /// </param>
        void WriteOnce(params Expression<Func<TDataItem, object>>[] writeOnceColumns);

        /// <summary>
        /// Sets a key for the data item from one of the data item properties. There can only be one key per 
        /// <typeparamref name="TKey"/> type.
        /// </summary>
        /// <param name="key">
        /// The key to set.
        /// </param>
        /// <typeparam name="TKey">
        /// The type of the key property.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="IEntityMappingProfile{TEntity,TDataItem}"/>.
        /// </returns>
        IEntityMappingProfile<TEntity, TDataItem> SetUniqueKey<TKey>([NotNull] Expression<Func<TDataItem, TKey>> key);

        /// <summary>
        /// Sets a compound key from the properties specified in the related entity.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <typeparam name="TDependency">
        /// The type that contains the key property.
        /// </typeparam>
        /// <typeparam name="TKey">
        /// The type of the key property.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="IEntityMappingProfile{TEntity,TDataItem}"/>.
        /// </returns>
        IEntityMappingProfile<TEntity, TDataItem> SetDependencyKey<TDependency, TKey>(
            [NotNull] Expression<Func<TDependency, TKey>> key);

        /// <summary>
        /// Sets a compound key from the properties specified in the related entity.
        /// </summary>
        /// <param name="sourceKey">
        /// The source key in the dependency.
        /// </param>
        /// <param name="targetKey">
        /// The target key in the current data item.
        /// </param>
        /// <typeparam name="TDependency">
        /// The type that contains the key property.
        /// </typeparam>
        /// <typeparam name="TKey">
        /// The type of the key property.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="IEntityMappingProfile{TEntity,TDataItem}"/>.
        /// </returns>
        IEntityMappingProfile<TEntity, TDataItem> SetDependencyKey<TDependency, TKey>(
            [NotNull] Expression<Func<TDependency, TKey>> sourceKey,
            [NotNull] Expression<Func<TDataItem, object>> targetKey);

        /// <summary>
        /// Maps a dependency data item that does not have a corresponding entity.
        /// </summary>
        /// <param name="primaryKey">
        /// The primary key of the dependency item.
        /// </param>
        /// <param name="writeOnceColumns">
        /// The columns of the item that should not be written more than once.
        /// </param>
        /// <typeparam name="TDependencyItem">
        /// The type of the dependency item.
        /// </typeparam>
        /// <typeparam name="TKey">
        /// The type of key to map.
        /// </typeparam>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="writeOnceColumns"/> is null.
        /// </exception>
        void CreateDependencyMap<TDependencyItem, TKey>(
            Expression<Func<TDependencyItem, TKey>> primaryKey,
            [NotNull] params Expression<Func<TDependencyItem, object>>[] writeOnceColumns) where TDependencyItem : ITransactionContext;

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
        /// The current <see cref="EntityMappingProfile{TEntity, TDataItem}"/> with the enumeration mapped between the entity and the 
        /// data item.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="enumerationProperty"/> or <paramref name="enumerationId"/> is null.
        /// </exception>
        [Obsolete("Use MapProperty instead.")]
        IEntityMappingProfile<TEntity, TDataItem> MapEnumeration<TEnum, TId>(
            [NotNull] Expression<Func<TEntity, TEnum>> enumerationProperty,
            [NotNull] Expression<Func<TDataItem, TId>> enumerationId);
*/

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
        /// An <see cref="IMappingExpression"/> that maps the specified <typeparamref name="TDataItem"/> to the <typeparamref name="TRelation"/>.
        /// </returns>
        IEntityMappingProfile<TEntity, TDataItem> ResolveUnmappedEntity<TRelation, TDataKey>(
            Expression<Func<TEntity, TRelation>> relatedEntity,
            Expression<Func<TDataItem, TDataKey>> relatedEntityKey) where TRelation : class;

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
        IEntityMappingProfile<TEntity, TDataItem> MapProperty<TProperty, TColumn>(
            Expression<Func<TEntity, TProperty>> entityProperty,
            Expression<Func<TDataItem, TColumn>> dataItemColumn);

        /// <summary>
        /// Ignores all the specified entity properties during mapping. This will not suppress mapping of constructor parameters.
        /// </summary>
        /// <param name="propertySelectors">
        /// The properties to ignore.
        /// </param>
        /// <exception cref="System.NotSupportedException">
        /// This method is not supported for the current type.
        /// </exception>
        /// <returns>
        /// The current <see cref="EntityMappingProfile{TEntity, TDataItem}"/> with the specified properties ignored.
        /// </returns>
        IEntityMappingProfile<TEntity, TDataItem> IgnoreForEntity(params Expression<Func<TEntity, object>>[] propertySelectors);

        /// <summary>
        /// Ignores all the specified data item properties during mapping.
        /// </summary>
        /// <param name="propertySelectors">
        /// The properties to ignore.
        /// </param>
        /// <returns>
        /// The current <see cref="EntityMappingProfile{TEntity, TDataItem}"/> with the specified properties ignored.
        /// </returns>
        IEntityMappingProfile<TEntity, TDataItem> IgnoreForDataItem(params Expression<Func<TDataItem, object>>[] propertySelectors);

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
        IEntityMappingProfile<TEntity, TDataItem> MapRelation<TProperty, TRelation>(
            [NotNull] Expression<Func<TEntity, TProperty>> entityProperty,
            [NotNull] Expression<Func<TDataItem, TRelation>> entityRelation,
            [NotNull] Expression<Func<TRelation, object>> relatedEntityKey)
            where TProperty : class
            where TRelation : ITransactionContext;
    }
}