// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadOnlyRepository.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The view repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Caching;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Orm.Query;
    using Startitecture.Resources;

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
        /// Initializes a new instance of the <see cref="ReadOnlyRepository{TEntity,TDataItem}"/> class.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider for this repository.
        /// </param>
        /// <param name="entityMapper">
        /// The entity mapper.
        /// </param>
        public ReadOnlyRepository(IRepositoryProvider repositoryProvider, IEntityMapper entityMapper)
            : this(repositoryProvider, entityMapper, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyRepository{TEntity,TDataItem}"/> class.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider for this repository.
        /// </param>
        /// <param name="entityMapper">
        /// The entity mapper.
        /// </param>
        /// <param name="selectionComparer">
        /// The selection comparer for ordering data items from the repository after being selected from the database.
        /// </param>
        public ReadOnlyRepository(
            IRepositoryProvider repositoryProvider,
            [NotNull] IEntityMapper entityMapper,
            IComparer<TDataItem> selectionComparer)
        {
            this.EntityMapper = entityMapper ?? throw new ArgumentNullException(nameof(entityMapper));
            this.RepositoryProvider = repositoryProvider ?? throw new ArgumentNullException(nameof(repositoryProvider));
            this.selectionComparer = selectionComparer;
        }

        /// <summary>
        /// Gets the repository provider.
        /// </summary>
        protected IRepositoryProvider RepositoryProvider { get; }

        /// <summary>
        /// Gets the entity mapper.
        /// </summary>
        protected IEntityMapper EntityMapper { get; }

        /// <inheritdoc />
        public bool Contains<TItem>(TItem candidate)
        {
            if (Evaluate.IsNull(candidate))
            {
                throw new ArgumentNullException(nameof(candidate));
            }

            var dataItem = this.GetExampleItem(candidate);
            var uniqueItemSelection = this.GetUniqueItemSelection(dataItem);
            return this.RepositoryProvider.Contains(uniqueItemSelection);
        }

        /// <inheritdoc />
        public TEntity FirstOrDefault<TItem>(TItem candidate)
        {
            if (Evaluate.IsNull(candidate))
            {
                throw new ArgumentNullException(nameof(candidate));
            }

            if (candidate is ITransactionContext context)
            {
                context.SetTransactionProvider(this.RepositoryProvider);
            }

            TEntity entity;

            var selectionItem = this.GetExampleItem(candidate);
            var uniqueItemSelection = this.GetUniqueItemSelection(selectionItem);
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
                    return default;
                }

                dataItem.SetTransactionProvider(this.RepositoryProvider);

                entity = this.ConstructEntity(dataItem, this.RepositoryProvider);
                this.UpdateCache(cacheResult.Key, entity);
            }

            return entity;
        }

        /// <inheritdoc />
        public IEnumerable<TEntity> SelectAll()
        {
            var exampleSelection = new ItemSelection<TDataItem>();
            var dataItems = this.RepositoryProvider.GetSelection(exampleSelection);
            return this.SelectResults(dataItems);
        }

        /// <inheritdoc />
        public IEnumerable<TEntity> Select<TItem>([NotNull] ItemSelection<TItem> selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            var mappedSelection = selection.MapTo<TDataItem>();
            var dataItems = this.RepositoryProvider.GetSelection(mappedSelection);
            return this.SelectResults(dataItems);
        }

        /// <summary>
        /// Gets a unique item selection for the specified item.
        /// </summary>
        /// <param name="item">
        /// The item to create the selection for.
        /// </param>
        /// <returns>
        /// A <see cref="ItemSelection{TItem}"/> for the specified item.
        /// </returns>
        protected virtual ItemSelection<TDataItem> GetUniqueItemSelection(TDataItem item)
        {
            return new UniqueQuery<TDataItem>(this.RepositoryProvider.EntityDefinitionProvider, item);
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

            return this.EntityMapper.Map<TEntity>(dataItem);
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
            var dataItems = this.RepositoryProvider.GetSelection(selection);
            return this.SelectResults(dataItems);
        }

        /// <summary>
        /// Gets an example item from the provided <typeparamref name="TDataItem"/> key.
        /// </summary>
        /// <param name="key">
        /// The key for the example item.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item that represents the key.
        /// </typeparam>
        /// <returns>
        /// An example of the data row as a <typeparamref name="TDataItem"/>.
        /// </returns>
        /// <exception cref="OperationException">
        /// The key cannot be applied to the <typeparamref name="TDataItem"/>. The inner exception contains details as to why.
        /// </exception>
        protected TDataItem GetExampleItem<TItem>(TItem key)
        {
            TDataItem dataItem;

            if (key?.GetType().IsValueType == true)
            {
                var keyDefinition = this.RepositoryProvider.EntityDefinitionProvider.Resolve<TDataItem>().PrimaryKeyAttributes.First();

                // If the example is a value type, create a new data item as an example and set the key. Assumption is that the two are compatible.
                dataItem = new TDataItem();

                try
                {
                    keyDefinition.SetValueDelegate.DynamicInvoke(dataItem, key);
                }
                catch (ArgumentException ex)
                {
                    var message = string.Format(
                        CultureInfo.CurrentCulture,
                        ErrorMessages.CouldNotSetPrimaryKeyWithValue,
                        typeof(TDataItem),
                        keyDefinition.PropertyName,
                        key,
                        ex.Message);

                    throw new OperationException(key, message, ex);
                }
                catch (MemberAccessException ex)
                {
                    var message = string.Format(
                        CultureInfo.CurrentCulture,
                        ErrorMessages.CouldNotSetPrimaryKeyWithValue,
                        typeof(TDataItem),
                        keyDefinition.PropertyName,
                        key,
                        ex.Message);

                    throw new OperationException(key, message, ex);
                }
                catch (TargetInvocationException ex)
                {
                    var message = string.Format(
                        CultureInfo.CurrentCulture,
                        ErrorMessages.CouldNotSetPrimaryKeyWithValue,
                        typeof(TDataItem),
                        keyDefinition.PropertyName,
                        key,
                        ex.Message);

                    throw new OperationException(key, message, ex);
                }
            }
            else
            {
                dataItem = this.EntityMapper.Map<TDataItem>(key);
            }

            dataItem.SetTransactionProvider(this.RepositoryProvider);
            return dataItem;
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
            var key = string.Format(CultureInfo.InvariantCulture, CacheKeyFormat, typeof(TEntity).ToRuntimeName(), selection);
            var cacheResult = new CacheResult<TEntity>(default, false, key);

            if (this.RepositoryProvider.EnableCaching == false)
            {
                return cacheResult;
            }

            var cachedItem = this.entityCache.Get(key);

            if (cachedItem is TEntity item)
            {
                ////Trace.TraceInformation("Got item '{0}' from the cache with key '{1}'.", cachedItem, key);
                cacheResult = new CacheResult<TEntity>(item, true, key);
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

            // Order the list of data items if desired.
            var items = this.selectionComparer == null ? dataItems : dataItems.OrderBy(x => x, this.selectionComparer);

            // TODO: Put results in the container for usage by the construct entity hook.
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