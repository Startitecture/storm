// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityRepository.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Orm.Common;

    /// <summary>
    /// A base class for entity repositories.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of entity stored in the repository.
    /// </typeparam>
    /// <typeparam name="TDataItem">
    /// The type of item that represents the entity in the repository.
    /// </typeparam>
    /// <remarks>
    /// Uses the default memory cache if no cache is specified, with a save policy that does not expire items and a select policy with
    /// a sliding expiration of 30 seconds.
    /// </remarks>
    public class EntityRepository<TEntity, TDataItem> : ReadOnlyRepository<TEntity, TDataItem>, IEntityRepository<TEntity>
        where TDataItem : class, ITransactionContext, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Startitecture.Orm.Repository.EntityRepository`2"/> class.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider for this repository.
        /// </param>
        /// <param name="key">
        /// The key property for the <typeparamref name="TEntity"/>.
        /// </param>
        public EntityRepository(IRepositoryProvider repositoryProvider, Expression<Func<TEntity, object>> key)
            : this(repositoryProvider, key, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Startitecture.Orm.Repository.EntityRepository`2"/> class.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider for this repository.
        /// </param>
        /// <param name="key">
        /// The key property for the <typeparamref name="TEntity"/>.
        /// </param>
        /// <param name="selectionComparer">
        /// The selection comparer for ordering data items from the repository after being selected from the database.
        /// </param>
        public EntityRepository(IRepositoryProvider repositoryProvider, Expression<Func<TEntity, object>> key, IComparer<TDataItem> selectionComparer)
            : base(repositoryProvider, key, selectionComparer)
        {
        }

        /// <summary>
        /// Saves an item to the database.
        /// </summary>
        /// <param name="item">
        /// The entity to save.
        /// </param>
        /// <returns>
        /// The saved <typeparamref name="TEntity"/> instance.
        /// </returns>
        public TEntity Save(TEntity item)
        {
            if (Evaluate.IsNull(item))
            {
                throw new ArgumentNullException(nameof(item));
            }

            var savedEntity = this.SaveEntity(item);
            return savedEntity;
        }

        /// <summary>
        /// Saves an item to the database.
        /// </summary>
        /// <param name="item">
        /// The entity to save.
        /// </param>
        /// <returns>
        /// The saved <typeparamref name="TEntity"/> instance.
        /// </returns>
        public TEntity SaveWithChildren(TEntity item)
        {
            var savedEntity = this.Save(item);
            this.SaveChildren(savedEntity, this.RepositoryProvider);
            return savedEntity;
        }

        /// <summary>
        /// Saves an item to the database.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of item to save.
        /// </typeparam>
        /// <param name="item">
        /// The item to save.
        /// </param>
        /// <returns>
        /// The saved <typeparamref name="TEntity"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="item"/> is null.
        /// </exception>
        /// <exception cref="T:SAF.Core.BusinessException">
        /// <paramref name="item"/> failed to validate.
        /// </exception>
        /// <exception cref="T:SAF.Core.ApplicationConfigurationException">
        /// <paramref name="item"/> failed to map from a <typeparamref name="TEntity"/> to a <typeparamref name="TDataItem"/>, or 
        /// <paramref name="item"/> failed to map from a <typeparamref name="TDataItem"/> to a <typeparamref name="TEntity"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The database operation failed.
        /// </exception>
        public TEntity Save<TItem>(TItem item)
        {
            if (Evaluate.IsNull(item))
            {
                throw new ArgumentNullException(nameof(item));
            }

            var entity = this.EntityMapper.Map<TEntity>(item);
            var savedEntity = this.SaveEntity(entity);
            return savedEntity;
        }

        /// <summary>
        /// Saves an item to the database.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of item to save.
        /// </typeparam>
        /// <param name="entity">
        /// The item to save.
        /// </param>
        /// <returns>
        /// The saved <typeparamref name="TItem"/> instance.
        /// </returns>
        public TItem SaveAs<TItem>(TEntity entity)
        {
            if (Evaluate.IsNull(entity))
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var dataItem = this.SaveDataItem(entity);
            this.SaveDependents(entity, this.RepositoryProvider, dataItem);
            var savedItem = this.EntityMapper.Map<TItem>(dataItem);

            if (this.AutomaticallySetTransactionContext && savedItem is ITransactionContext)
            {
                (savedItem as ITransactionContext).SetTransactionProvider(this.RepositoryProvider);
            }

            return savedItem;
        }

        /// <summary>
        /// Deletes a single item.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of item that contains the example properties.
        /// </typeparam>
        /// <param name="example">
        /// The example entity.
        /// </param>
        /// <returns>
        /// The number of items affected as an <see cref="int"/>.
        /// </returns>
        public int Delete<TItem>(TItem example)
        {
            var dataItem = this.EntityMapper.Map<TDataItem>(example);
            var uniqueItemSelection = this.GetUniqueItemSelection(dataItem);
            return this.RepositoryProvider.DeleteItems(uniqueItemSelection);
        }

        /// <summary>
        /// Deletes a single item with its child items.
        /// </summary>
        /// <param name="entity">
        /// The entity to delete.
        /// </param>
        /// <returns>
        /// The number of non-child items affected as an <see cref="int"/>.
        /// </returns>
        public int DeleteWithChildren(TEntity entity)
        {
            this.DeleteChildren(entity, this.RepositoryProvider);
            var dataItem = this.EntityMapper.Map<TDataItem>(entity);
            var uniqueItemSelection = this.GetUniqueItemSelection(dataItem);
            return this.RepositoryProvider.DeleteItems(uniqueItemSelection);
        }

        #region Methods

        /// <summary>
        /// Saves the dependencies of the specified entity prior to saving the entity itself.
        /// </summary>
        /// <param name="entity">
        /// The entity to save.
        /// </param>
        /// <param name="provider">
        /// The repository provider for the current operation.
        /// </param>
        /// <param name="dataItem">
        /// The data item mapped from the entity.
        /// </param>
        /// <remarks>
        /// Use repositories with the entity to save dependencies and apply the results to the <paramref name="dataItem"/>.
        /// </remarks>
        protected virtual void SaveDependencies(TEntity entity, IRepositoryProvider provider, TDataItem dataItem)
        {
        }

        /// <summary>
        /// Saves the dependents of the specified entity after saving the entity itself.
        /// </summary>
        /// <param name="entity">
        /// The saved entity.
        /// </param>
        /// <param name="provider">
        /// The repository provider for the current operation.
        /// </param>
        /// <param name="dataItem">
        /// The data item mapped from the entity.
        /// </param>
        /// <remarks>
        /// Use repositories with the entity to save dependents and apply the results to the <paramref name="dataItem"/>.
        /// </remarks>
        protected virtual void SaveDependents(TEntity entity, IRepositoryProvider provider, TDataItem dataItem)
        {
        }

        /// <summary>
        /// Saves a dependent item in the specified repository.
        /// </summary>
        /// <param name="entity">
        /// The entity that contains the dependency.
        /// </param>
        /// <param name="dependencySelector">
        /// The dependency selector.
        /// </param>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="dataItem">
        /// The data item.
        /// </param>
        /// <typeparam name="TDependency">
        /// The type of dependency being saved.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="entity"/>, <paramref name="dependencySelector"/>, <paramref name="repository"/> or 
        /// <paramref name="dataItem"/> is null.
        /// </exception>
        protected void SaveDependentItem<TDependency>(
            [NotNull] TEntity entity,
            [NotNull] Func<TEntity, TDependency> dependencySelector,
            [NotNull] IEntityRepository<TDependency> repository,
            [NotNull] TDataItem dataItem)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (dependencySelector == null)
            {
                throw new ArgumentNullException(nameof(dependencySelector));
            }

            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            if (dataItem == null)
            {
                throw new ArgumentNullException(nameof(dataItem));
            }

            var dependency = dependencySelector(entity);
            var entityKey = this.RepositoryProvider.EntityDefinitionProvider.Resolve<TDataItem>()
                .PrimaryKeyAttributes.First()
                .GetValueDelegate.DynamicInvoke(dataItem);

                //// this.PrimaryKeyExpression == null ? dataItem : this.PrimaryKeyExpression.Compile().DynamicInvoke(dataItem);

            if (dependency == null)
            {
                // Get the primary key of the containing entity, or use the entity itself which requires a dependency key mapping in 
                // the dependency's mapping profile.
                repository.Delete(entityKey);
            }
            else
            {
                // Needed to apply the newly set parent ID.
                this.EntityMapper.MapTo(entityKey, dependency);
                repository.Save(dependency);

                // Now that the entity has been updated, map back to the data item.
                this.EntityMapper.MapTo(entity, dataItem);
            }
        }

        /// <summary>
        /// Saves the children of the specified entity once the entity itself has been saved.
        /// </summary>
        /// <param name="entity">
        /// The entity to save.
        /// </param>
        /// <param name="provider">
        /// The repository provider for the current operation.
        /// </param>
        protected virtual void SaveChildren(TEntity entity, IRepositoryProvider provider)
        {
        }

        /// <summary>
        /// Deletes the children of the specified entity before the entity itself is deleted.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        protected virtual void DeleteChildren(TEntity entity, IRepositoryProvider provider)
        {
        }

        #region Caching

        #endregion

        /// <summary>
        /// Saves an item to the database.
        /// </summary>
        /// <param name="entity">
        /// The item to save.
        /// </param>
        /// <returns>
        /// The saved data item.
        /// </returns>
        private TDataItem SaveDataItem(TEntity entity)
        {
            // TODO: This preserves old functionality where the data item was mapped prior to saving dependencies. 
            var dataItem = new TDataItem();
            dataItem.SetTransactionProvider(this.RepositoryProvider);
            this.SaveDependencies(entity, this.RepositoryProvider, dataItem);
            this.EntityMapper.MapTo(entity, dataItem);

            dataItem = this.RepositoryProvider.Save(dataItem);

            // In an update operation, dataItem will be a different object reference blended with the original.
            dataItem.SetTransactionProvider(this.RepositoryProvider);
            return dataItem;
        }

        /// <summary>
        /// Saves the entity.
        /// </summary>
        /// <param name="item">
        /// The item to save.
        /// </param>
        /// <returns>
        /// The saved entity.
        /// </returns>
        private TEntity SaveEntity(TEntity item)
        {
            var dataItem = this.SaveDataItem(item);

            ////// Map the data item key to the entity if one has been set.
            ////if (this.PrimaryKeyExpression != null)
            ////{
            var entityDefinition = this.RepositoryProvider.EntityDefinitionProvider.Resolve<TDataItem>();
            var key = entityDefinition.PrimaryKeyAttributes.First().GetValueDelegate.DynamicInvoke(dataItem);

                    ////.PrimaryKeyExpression.Compile().DynamicInvoke(dataItem);

                if (key is int integerKey)
                {
                    this.EntityMapper.MapTo(integerKey, item);
                }

                if (key is long longKey)
                {
                    this.EntityMapper.MapTo(longKey, item);
                }

                // The mapping must also have set the primary key.
                this.EntityMapper.MapTo(key, item);
            ////}

            this.SaveDependents(item, this.RepositoryProvider, dataItem);

            // Save the dependent elements of the entity.
            return item;
        }

        #endregion
    }
}