// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MappingProfileBase.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The mapping profile base.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository
{
    using System;
    using System.Linq.Expressions;

    using AutoMapper;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Resources;

    /// <summary>
    /// The mapping profile base.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of entity to map.
    /// </typeparam>
    /// <typeparam name="TDataItem">
    /// The type of data item to map.
    /// </typeparam>
    public abstract class MappingProfileBase<TEntity, TDataItem> : Profile, IEntityMappingProfile<TEntity, TDataItem>
        where TDataItem : ITransactionContext
    {
        /// <summary>
        /// The entity to row expression.
        /// </summary>
        private readonly IMappingExpression<TEntity, TDataItem> entityToRowExpression;

        /// <summary>
        /// The row to row expression.
        /// </summary>
        private readonly IMappingExpression<TDataItem, TDataItem> rowToRowExpression;

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingProfileBase{TEntity,TDataItem}"/> class.
        /// </summary>
        protected MappingProfileBase()
        {
            this.entityToRowExpression = this.CreateMap<TEntity, TDataItem>();
            this.entityToRowExpression.ForMember(item => item.TransactionProvider, expr => expr.Ignore());
            this.rowToRowExpression = this.CreateMap<TDataItem, TDataItem>();
        }

        /// <summary>
        /// Gets the entity to row mapping expression.
        /// </summary>
        protected IMappingExpression<TEntity, TDataItem> EntityToRowExpression
        {
            get
            {
                return this.entityToRowExpression;
            }
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
        public virtual IEntityMappingProfile<TEntity, TDataItem> SetPrimaryKey<TEntityKey, TDataKey>(
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

            // Associate a link between the entity's key type and the data item.
            this.CreateMap<TDataItem, TDataKey>().ConstructUsing(dataItemKey.Compile());

            // Create a mapping between the entity key type and the entity so that the key will be applied to the entity.
            var entityKeyMapping = this.CreateMap<TDataKey, TEntity>()
                .DisableCtorValidation()
                .ForMember(entityKey, expr => expr.MapFrom(source => source));

            entityKeyMapping.ForAllOtherMembers(expr => expr.Ignore());

            this.SetUniqueKey(dataItemKey);

            // Also map the property so in the case of mismatched key names it is not necessary to manually make this call.
            this.MapProperty(entityKey, dataItemKey);
            this.rowToRowExpression.ForMember(dataItemKey.GetPropertyName(), expr => expr.Ignore());

            return this;
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
        public abstract IEntityMappingProfile<TEntity, TDataItem> MapProperty<TProperty, TColumn>(
            Expression<Func<TEntity, TProperty>> entityProperty,
            Expression<Func<TDataItem, TColumn>> dataItemColumn);

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
        /// <exception cref="System.NotSupportedException">
        /// This method is not supported for the current type.
        /// </exception>
        /// <returns>
        /// An <see cref="IMappingExpression"/> that maps the specified <typeparamref name="TDataItem"/> to the <typeparamref name="TRelation"/>.
        /// </returns>
        public virtual IEntityMappingProfile<TEntity, TDataItem> ResolveUnmappedEntity<TRelation, TDataKey>(
            Expression<Func<TEntity, TRelation>> relatedEntity,
            Expression<Func<TDataItem, TDataKey>> relatedEntityKey)
            where TRelation : class
        {
            throw new NotSupportedException();
        }

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
        public virtual IEntityMappingProfile<TEntity, TDataItem> IgnoreForEntity(params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Ignores all the specified data item properties during mapping.
        /// </summary>
        /// <param name="propertySelectors">
        /// The properties to ignore.
        /// </param>
        /// <returns>
        /// The current <see cref="EntityMappingProfile{TEntity, TDataItem}"/> with the specified properties ignored.
        /// </returns>
        public IEntityMappingProfile<TEntity, TDataItem> IgnoreForDataItem([NotNull] params Expression<Func<TDataItem, object>>[] propertySelectors)
        {
            if (propertySelectors == null)
            {
                throw new ArgumentNullException(nameof(propertySelectors));
            }

            this.entityToRowExpression.Ignore(propertySelectors);
            return this;
        }

        /// <summary>
        /// Sets the data item properties that should only be written once, such as creation information.
        /// </summary>
        /// <param name="writeOnceColumns">
        /// The write once columns.
        /// </param>
        public void WriteOnce([NotNull] params Expression<Func<TDataItem, object>>[] writeOnceColumns)
        {
            if (writeOnceColumns == null)
            {
                throw new ArgumentNullException(nameof(writeOnceColumns));
            }

            foreach (var expression in writeOnceColumns)
            {
                this.rowToRowExpression.ForMember(expression.GetPropertyName(), expr => expr.Ignore());
            }
        }

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
        /// The current <see cref="T:Startitecture.Orm.Repository.EntityMappingProfile`2"/>.
        /// </returns>
        public IEntityMappingProfile<TEntity, TDataItem> SetUniqueKey<TKey>(Expression<Func<TDataItem, TKey>> key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (typeof(TKey) == typeof(TEntity))
            {
                throw new NotSupportedException(ErrorMessages.KeyMappingToEntityTypeNotSupported);
            }

            if (typeof(TKey) == typeof(TDataItem))
            {
                throw new NotSupportedException(ErrorMessages.KeyMappingToDataItemTypeNotSupported);
            }

            this.ConfigureKey(key);
            return this;
        }

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
        /// The current <see cref="T:Startitecture.Orm.Repository.EntityMappingProfile`2"/>.
        /// </returns>
        public IEntityMappingProfile<TEntity, TDataItem> SetDependencyKey<TDependency, TKey>(Expression<Func<TDependency, TKey>> key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (typeof(TDependency) == typeof(TEntity))
            {
                throw new NotSupportedException(ErrorMessages.KeyMappingToEntityTypeNotSupported);
            }

            if (typeof(TDependency) == typeof(TDataItem))
            {
                throw new NotSupportedException(ErrorMessages.KeyMappingToDataItemTypeNotSupported);
            }

            var mapping = this.CreateMap<TDependency, TDataItem>();
            mapping.ForMember(key.GetPropertyName(), expr => expr.MapFrom(key));
            mapping.ForAllOtherMembers(expr => expr.Ignore());
            return this;
        }

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
        /// The current <see cref="T:Startitecture.Orm.Repository.EntityMappingProfile`2"/>.
        /// </returns>
        public IEntityMappingProfile<TEntity, TDataItem> SetDependencyKey<TDependency, TKey>(
            Expression<Func<TDependency, TKey>> sourceKey,
            Expression<Func<TDataItem, object>> targetKey)
        {
            if (sourceKey == null)
            {
                throw new ArgumentNullException(nameof(sourceKey));
            }

            if (targetKey == null)
            {
                throw new ArgumentNullException(nameof(targetKey));
            }

            if (typeof(TDependency) == typeof(TEntity))
            {
                throw new NotSupportedException(ErrorMessages.KeyMappingToEntityTypeNotSupported);
            }

            if (typeof(TDependency) == typeof(TDataItem))
            {
                throw new NotSupportedException(ErrorMessages.KeyMappingToDataItemTypeNotSupported);
            }

            var mapping = this.CreateMap<TDependency, TDataItem>();
            mapping.ForMember(targetKey, expr => expr.MapFrom(sourceKey));
            mapping.ForAllOtherMembers(expr => expr.Ignore());
            return this;
        }

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
        public void CreateDependencyMap<TDependencyItem, TKey>(
            Expression<Func<TDependencyItem, TKey>> primaryKey,
            params Expression<Func<TDependencyItem, object>>[] writeOnceColumns)
            where TDependencyItem : ITransactionContext
        {
            if (writeOnceColumns == null)
            {
                throw new ArgumentNullException(nameof(writeOnceColumns));
            }

            var rowToRowMapping = this.CreateMap<TDependencyItem, TDependencyItem>();

            if (primaryKey != null)
            {
                this.CreateMap<TDependencyItem, TKey>().ConstructUsing(primaryKey.Compile());
                this.CreateMap<TKey, TDependencyItem>().MapKey(primaryKey);
                rowToRowMapping.ForMember(primaryKey.GetPropertyName(), expr => expr.Ignore());
            }

            foreach (var expression in writeOnceColumns)
            {
                rowToRowMapping.ForMember(expression.GetPropertyName(), expr => expr.Ignore());
            }
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
        public abstract IEntityMappingProfile<TEntity, TDataItem> MapRelation<TProperty, TRelation>(
            Expression<Func<TEntity, TProperty>> entityProperty,
            Expression<Func<TDataItem, TRelation>> entityRelation,
            Expression<Func<TRelation, object>> relatedEntityKey)
            where TProperty : class
            where TRelation : ITransactionContext;
    }
}