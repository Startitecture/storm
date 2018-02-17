// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadOnlyRepository.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The view repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.Caching;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Query;

    /// <summary>
    /// The view repository.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of item that is contained in the view.
    /// </typeparam>
    /// <typeparam name="TDataItem">
    /// The type of data item used to represent the domain item.
    /// </typeparam>
    public class ReadOnlyRepository<TEntity, TDataItem> : IReadOnlyRepository<TEntity>
        where TDataItem : class, ITransactionContext, new()
    {
        /// <summary>
        /// The cache key format.
        /// </summary>
        private const string CacheKeyFormat = "{0}.{1}";

        /// <summary>
        /// The entity cache.
        /// </summary>
        private readonly ObjectCache entityCache = MemoryCache.Default;

        /// <summary>
        /// The cache item policy.
        /// </summary>
        private readonly CacheItemPolicy cacheItemPolicy = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(2) };

        /// <summary>
        /// The selection comparer.
        /// </summary>
        private readonly IComparer<TDataItem> selectionComparer;

        // TODO: Caching must be enabled at a higher level. This approach makes relations get applied multiple times.
        /////// <summary>
        /////// The use primary key selection.
        /////// </summary>
        ////private ItemSelection<TDataItem> usePrimaryKeySelection;

        /////// <summary>
        /////// The use alternate key selection.
        /////// </summary>
        ////private ItemSelection<TDataItem> useAlternateKeySelection;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SAF.Data.Providers.ReadOnlyRepository`2"/> class.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider for this repository.
        /// </param>
        protected ReadOnlyRepository(IRepositoryProvider repositoryProvider)
            : this(repositoryProvider, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SAF.Data.Providers.ReadOnlyRepository`2"/> class.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider for this repository.
        /// </param>
        /// <param name="selectionComparer">
        /// The selection comparer for ordering data items from the repository after being selected from the database.
        /// </param>
        protected ReadOnlyRepository(IRepositoryProvider repositoryProvider, IComparer<TDataItem> selectionComparer)
        {
            if (repositoryProvider == null)
            {
                throw new ArgumentNullException(nameof(repositoryProvider));
            }

            repositoryProvider.ThrowOnDependencyFailure(provider => provider.EntityMapper);
            this.EntityMapper = repositoryProvider.EntityMapper;
            this.RepositoryProvider = repositoryProvider;
            this.selectionComparer = selectionComparer;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to automatically set the transaction context of items.
        /// </summary>
        public bool AutomaticallySetTransactionContext { get; set; }

        /// <summary>
        /// Gets the repository provider.
        /// </summary>
        protected IRepositoryProvider RepositoryProvider { get; }

        /// <summary>
        /// Gets the entity mapper.
        /// </summary>
        protected IEntityMapper EntityMapper { get; }

        /// <summary>
        /// Gets the primary key expression for the current repository.
        /// </summary>
        protected LambdaExpression PrimaryKeyExpression { get; private set; }

        /// <summary>
        /// Determines whether an item exists in the repository.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of item to search for.
        /// </typeparam>
        /// <param name="candidate">
        /// The candidate to search for.
        /// </param>
        /// <returns>
        /// <c>true</c> if the item exists in the repository; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="candidate"/> is null.
        /// </exception>
        /// <remarks>
        /// This method always queries the underlying provider directly rather than using a cache.
        /// </remarks>
        public bool Contains<TItem>(TItem candidate)
        {
            if (Evaluate.IsNull(candidate))
            {
                throw new ArgumentNullException(nameof(candidate));
            }

            if (this.AutomaticallySetTransactionContext && candidate is ITransactionContext)
            {
                (candidate as ITransactionContext).SetTransactionProvider(this.RepositoryProvider);
            }

            var dataItem = candidate as TDataItem ?? this.EntityMapper.Map<TDataItem>(candidate);
            dataItem.SetTransactionProvider(this.RepositoryProvider);
            var uniqueItemSelection = this.GetUniqueItemSelection(dataItem);
            return this.RepositoryProvider.Contains(uniqueItemSelection);
        }

        /// <summary>
        /// Gets an item by its identifier or unique key.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of item to search for.
        /// </typeparam>
        /// <param name="candidate">
        /// A candidate item representing the item to search for.
        /// </param>
        /// <returns>
        /// The first <typeparamref name="TEntity"/> in the repository matching the candidate item's identifier or unique key, or a 
        /// default value of the <typeparamref name="TEntity"/> type if no entity could be found using the candidate.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="candidate"/> is null.
        /// </exception>
        public TEntity FirstOrDefault<TItem>(TItem candidate)
        {
            if (Evaluate.IsNull(candidate))
            {
                throw new ArgumentNullException(nameof(candidate));
            }

            if (this.AutomaticallySetTransactionContext && candidate is ITransactionContext)
            {
                (candidate as ITransactionContext).SetTransactionProvider(this.RepositoryProvider);
            }

            TEntity entity;

            var selectionItem = candidate as TDataItem ?? this.EntityMapper.Map<TDataItem>(candidate);
            var uniqueItemSelection = this.GetUniqueItemSelection(selectionItem);
            ////var selection = this.AssociateRelatedEntities(uniqueItemSelection);
            var cacheResult = this.QueryCache(uniqueItemSelection);

            if (cacheResult.Hit)
            {
                entity = cacheResult.Item;
            }
            else
            {
                var dataItem = this.RepositoryProvider.GetFirstOrDefault(uniqueItemSelection);

                if (Evaluate.IsNull(dataItem))
                {
                    return default(TEntity);
                }

                dataItem.SetTransactionProvider(this.RepositoryProvider);

                entity = this.ConstructEntity(dataItem, this.RepositoryProvider);
                this.UpdateCache(cacheResult.Key, entity);
            }

            if (this.AutomaticallySetTransactionContext && entity is ITransactionContext)
            {
                (entity as ITransactionContext).SetTransactionProvider(this.RepositoryProvider);
            }

            return entity;
        }

        /// <summary>
        /// Loads an entity with its children.
        /// </summary>
        /// <typeparam name="TKey">
        /// The type of the key that uniquely identifies the entity.
        /// </typeparam>
        /// <param name="key">
        /// The key that uniquely identifies the entity.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is null.
        /// </exception>
        /// <returns>
        /// The entity with the specified key, with all child elements loaded, or null if the entity does not exist.
        /// </returns>
        public TEntity FirstOrDefaultWithChildren<TKey>([NotNull] TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            // Get the entity based on the provided key (ID or some such).
            var entity = this.FirstOrDefault(key);

            if (entity == null)
            {
                return default(TEntity);
            }

            this.LoadChildren(entity, this.RepositoryProvider);

            // Return the fully hydrated entity.
            return entity;
        }

        /// <summary>
        /// Gets an item by its identifier or unique key.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of item to search for.
        /// </typeparam>
        /// <param name="candidate">
        /// A candidate item representing the item to search for.
        /// </param>
        /// <returns>
        /// The first <typeparamref name="TItem"/> in the repository matching the candidate item's identifier or unique key, or a 
        /// default value of the <typeparamref name="TItem"/> type if no entity could be found using the candidate.
        /// </returns>
        public TItem FirstOrDefaultAs<TItem>(TItem candidate)
        {
            if (Evaluate.IsNull(candidate))
            {
                throw new ArgumentNullException(nameof(candidate));
            }

            if (this.AutomaticallySetTransactionContext && candidate is ITransactionContext)
            {
                (candidate as ITransactionContext).SetTransactionProvider(this.RepositoryProvider);
            }

            var selectionItem = candidate as TDataItem ?? this.EntityMapper.Map<TDataItem>(candidate);
            var uniqueItemSelection = this.GetUniqueItemSelection(selectionItem);
            ////var selection = this.AssociateRelatedEntities(uniqueItemSelection);
            var dataItem = this.RepositoryProvider.GetFirstOrDefault(uniqueItemSelection);

            if (Evaluate.IsNull(dataItem))
            {
                return default(TItem);
            }

            dataItem.SetTransactionProvider(this.RepositoryProvider);
            var item = this.EntityMapper.Map<TItem>(dataItem);

            // Propagate the transaction context.
            if (this.AutomaticallySetTransactionContext && item is ITransactionContext)
            {
                (item as ITransactionContext).SetTransactionProvider(this.RepositoryProvider);
            }

            return item;
        }

        /// <summary>
        /// Selects all items in the repository.
        /// </summary>
        /// <returns>
        /// A collection of items that match the criteria.
        /// </returns>
        public IEnumerable<TEntity> SelectAll()
        {
            var exampleSelection = new TransactSqlSelection<TDataItem>(new TDataItem());
            var dataItems = this.RepositoryProvider.GetSelection(exampleSelection);
            return this.SelectResults(dataItems);
        }

        /// <summary>
        /// Loads the children of the specified entity.
        /// </summary>
        /// <param name="entity">
        /// The entity to load the children for.
        /// </param>
        public void LoadChildren([NotNull] TEntity entity)
        {
            if (Evaluate.IsNull(entity))
            {
                throw new ArgumentNullException(nameof(entity));
            }

            this.LoadChildren(entity, this.RepositoryProvider);
        }

        /// <summary>
        /// Gets a unique item selection for the specified item.
        /// </summary>
        /// <param name="item">
        /// The item to create the selection for.
        /// </param>
        /// <returns>
        /// A <see cref="T:SAF.Data.ItemSelection`1"/> for the specified item.
        /// </returns>
        protected virtual ItemSelection<TDataItem> GetUniqueItemSelection(TDataItem item)
        {
            return item.ToExampleSelection();
        }

        /// <summary>
        /// Loads the children of the specified entity.
        /// </summary>
        /// <param name="entity">
        /// The entity to load the children for.
        /// </param>
        /// <param name="provider">
        /// The repository provider.
        /// </param>
        protected virtual void LoadChildren(TEntity entity, IRepositoryProvider provider)
        {
        }

        /// <summary>
        /// Constructs the entity for the specified data item.
        /// </summary>
        /// <param name="dataItem">
        /// The data item to construct an entity for.
        /// </param>
        /// <param name="repositoryProvider">
        /// The repository provider.
        /// </param>
        /// <returns>
        /// A new instance of the entity.
        /// </returns>
        protected virtual TEntity ConstructEntity(TDataItem dataItem, [NotNull] IRepositoryProvider repositoryProvider)
        {
            if (repositoryProvider == null)
            {
                throw new ArgumentNullException(nameof(repositoryProvider));
            }

            return repositoryProvider.EntityMapper.Map<TEntity>(dataItem);
        }

        /// <summary>
        /// Selects a list of items from the repository.
        /// </summary>
        /// <param name="selection">
        /// The selection criteria.
        /// </param>
        /// <returns>
        /// A collection of items that match the criteria.
        /// </returns>
        protected IEnumerable<TEntity> SelectEntities(ItemSelection<TDataItem> selection)
        {
            var items = this.SelectDataItems(selection);
            return this.SelectResults(items);
        }

        /// <summary>
        /// Selects a list of items from the repository.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of item to select.
        /// </typeparam>
        /// <param name="selection">
        /// The selection criteria.
        /// </param>
        /// <returns>
        /// A collection of items that match the criteria.
        /// </returns>
        protected IEnumerable<TItem> SelectEntitiesAs<TItem>(ItemSelection<TDataItem> selection)
        {
            var items = this.SelectDataItems(selection);
            return this.EntityMapper.Map<List<TItem>>(items);
        }

        /// <summary>
        /// Gets a unique selection based on a primary key and alternate keys.
        /// </summary>
        /// <param name="item">
        /// The item to create the selection for.
        /// </param>
        /// <param name="primaryKey">
        /// The primary key.
        /// </param>
        /// <typeparam name="TKey">
        /// The type of the data item's key.
        /// </typeparam>
        /// <returns>
        /// The <see cref="T:SAF.Data.ItemSelection`1"/> with the related entity associations.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Allows fluent usage of the method.")]
        protected ItemSelection<TDataItem> GetKeySelection<TKey>(TDataItem item, [NotNull] Expression<Func<TDataItem, TKey>> primaryKey)
        {
            return this.GetKeySelection(item, primaryKey, new Expression<Func<TDataItem, object>>[] { });
        }

        /// <summary>
        /// Gets a unique selection based on a primary key and alternate keys.
        /// </summary>
        /// <param name="item">
        /// The item to create the selection for.
        /// </param>
        /// <param name="primaryKey">
        /// The primary key.
        /// </param>
        /// <param name="alternateKeys">
        /// The alternate keys to use if the primary key cannot be used.
        /// </param>
        /// <typeparam name="TKey">
        /// The type of the data item's key.
        /// </typeparam>
        /// <returns>
        /// The <see cref="T:SAF.Data.ItemSelection`1"/> with the related entity associations.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Allows fluent usage of the method.")]
        protected ItemSelection<TDataItem> GetKeySelection<TKey>(
            TDataItem item,
            [NotNull] Expression<Func<TDataItem, TKey>> primaryKey,
            [NotNull] params Expression<Func<TDataItem, object>>[] alternateKeys)
        {
            if (primaryKey == null)
            {
                throw new ArgumentNullException(nameof(primaryKey));
            }

            if (alternateKeys == null)
            {
                throw new ArgumentNullException(nameof(alternateKeys));
            }

            if (this.PrimaryKeyExpression == null)
            {
                this.PrimaryKeyExpression = primaryKey;
            }

            ItemSelection<TDataItem> selection;

            var usePrimaryKey = Evaluate.Equals(default(TKey), primaryKey.Compile().Invoke(item)) == false;

            if (usePrimaryKey || alternateKeys.Length == 0)
            {
                selection = GetPrimaryKeySelection(item, primaryKey);
            }
            else
            {
                selection = item.ToExampleSelection(alternateKeys);
            }

            return selection;
        }

        /// <summary>
        /// Gets the primary key.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <param name="primaryKey">
        /// The primary key.
        /// </param>
        /// <typeparam name="TKey">
        /// The type of the key value.
        /// </typeparam>
        /// <returns>
        /// Gets the primary key selection.
        /// </returns>
        private static ItemSelection<TDataItem> GetPrimaryKeySelection<TKey>(TDataItem item, Expression<Func<TDataItem, TKey>> primaryKey)
        {
            return new TransactSqlSelection<TDataItem>(item, new[] { primaryKey.GetPropertyName() });
        }

        /// <summary>
        /// Attempts to get a cached entity based on the specified item.
        /// </summary>
        /// <param name="selection">
        /// The item that represents the unique key for the entity.
        /// </param>
        /// <returns>
        /// <c>true</c> if the entity is found; otherwise, <c>false</c>.
        /// </returns>
        private CacheResult<TEntity> QueryCache(ItemSelection<TDataItem> selection)
        {
            var key = string.Format(CacheKeyFormat, typeof(TEntity).ToRuntimeName(), selection);
            var cacheResult = new CacheResult<TEntity>(default(TEntity), false, key);

            if (this.RepositoryProvider.EnableCaching == false)
            {
                return cacheResult;
            }

            var cachedItem = this.entityCache.Get(key);

            if (cachedItem is TEntity)
            {
                ////Trace.TraceInformation("Got item '{0}' from the cache with key '{1}'.", cachedItem, key);
                cacheResult = new CacheResult<TEntity>((TEntity)cachedItem, true, key);
            }

            return cacheResult;
        }

        /// <summary>
        /// Updates the cache with the specified key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="entity">
        /// The entity to save in the cache.
        /// </param>
        private void UpdateCache(string key, TEntity entity)
        {
            this.entityCache.Set(key, entity, this.cacheItemPolicy);
        }

        /// <summary>
        /// Selects a list of items from the repository.
        /// </summary>
        /// <param name="dataSelection">
        /// The data selection.
        /// </param>
        /// <returns>
        /// A collection of items that match the criteria.
        /// </returns>
        private IEnumerable<TDataItem> SelectDataItems(ItemSelection<TDataItem> dataSelection)
        {
            var dataItems = this.RepositoryProvider.GetSelection(dataSelection);
            return dataItems;
        }

        /// <summary>
        /// Selects results and adds them to the cache.
        /// </summary>
        /// <param name="dataItems">
        /// The data items to select into the results collection.
        /// </param>
        /// <returns>
        /// A collection of entities based on the specified data items.
        /// </returns>
        private IEnumerable<TEntity> SelectResults(IEnumerable<TDataItem> dataItems)
        {
            var results = new List<TEntity>();

            var items = this.selectionComparer == null ? dataItems : dataItems.OrderBy(x => x, this.selectionComparer);

            foreach (var dataItem in items)
            {
                dataItem.SetTransactionProvider(this.RepositoryProvider);
                var entity = this.ConstructEntity(dataItem, this.RepositoryProvider);
                results.Add(entity);
            }

            return results;
        }
    }
}