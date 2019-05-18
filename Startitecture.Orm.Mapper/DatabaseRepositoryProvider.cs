// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseRepositoryProvider.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Provides a concrete implementation for a database repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.Caching;

    using Common;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Query;
    using Startitecture.Resources;

    /// <summary>
    /// Provides a concrete implementation for a database repository.
    /// </summary>
    public sealed class DatabaseRepositoryProvider : IRepositoryProvider, IDatabaseContextProvider
    {
        /// <summary>
        /// The to string format.
        /// </summary>
        private const string ToStringFormat = "{0} [{1}]";

        /// <summary>
        /// The cache key format.
        /// </summary>
        private const string CacheKeyFormat = "{0}=[{1}]";

        /// <summary>
        /// The cache time.
        /// </summary>
        private readonly TimeSpan cacheTime = TimeSpan.FromMinutes(2);

        /// <summary>
        /// The repository adapter.
        /// </summary>
        private readonly IRepositoryAdapter repositoryAdapter;

        /// <summary>
        /// The item cache.
        /// </summary>
        private readonly ObjectCache itemCache;

        /// <summary>
        /// The item lock.
        /// </summary>
        private readonly object itemLock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseRepositoryProvider"/> class. 
        /// </summary>
        /// <param name="databaseFactory">
        /// The database factory.
        /// </param>
        /// <param name="entityMapper">
        /// The entity mapper.
        /// </param>
        public DatabaseRepositoryProvider(
            [NotNull] IDatabaseFactory databaseFactory,
            [NotNull] IEntityMapper entityMapper)
            : this(databaseFactory, entityMapper, Singleton<SqlServerRepositoryAdapterFactory>.Instance)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseRepositoryProvider"/> class. 
        /// </summary>
        /// <param name="databaseFactory">
        /// The database factory.
        /// </param>
        /// <param name="entityMapper">
        /// The entity mapper.
        /// </param>
        /// <param name="adapterFactory">
        /// The repository adapter.
        /// </param>
        public DatabaseRepositoryProvider(
            [NotNull] IDatabaseFactory databaseFactory,
            [NotNull] IEntityMapper entityMapper,
            [NotNull] IRepositoryAdapterFactory adapterFactory)
            : this(databaseFactory, entityMapper, adapterFactory, MemoryCache.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseRepositoryProvider"/> class. 
        /// </summary>
        /// <param name="databaseFactory">
        /// The database factory.
        /// </param>
        /// <param name="entityMapper">
        /// The entity mapper.
        /// </param>
        /// <param name="adapterFactory">
        /// The repository adapter.
        /// </param>
        /// <param name="itemCache">
        /// The item cache.
        /// </param>
        public DatabaseRepositoryProvider(
            [NotNull] IDatabaseFactory databaseFactory,
            [NotNull] IEntityMapper entityMapper,
            [NotNull] IRepositoryAdapterFactory adapterFactory,
            [NotNull] ObjectCache itemCache)
        {
            if (adapterFactory == null)
            {
                throw new ArgumentNullException(nameof(adapterFactory));
            }

            if (itemCache == null)
            {
                throw new ArgumentNullException(nameof(itemCache));
            }

            if (databaseFactory == null)
            {
                throw new ArgumentNullException(nameof(databaseFactory));
            }

            if (entityMapper == null)
            {
                throw new ArgumentNullException(nameof(entityMapper));
            }

            this.EntityMapper = entityMapper;
            this.DependencyContainer = new DependencyContainer();
            this.itemCache = itemCache;

            ConfigurationManager.AppSettings.ApplySetting(this, provider => provider.EnableCaching, false, bool.Parse);
            ConfigurationManager.AppSettings.ApplySetting(this, provider => provider.CacheExpiration, this.cacheTime, TimeSpan.Parse);

            try
            {
                this.DatabaseContext = databaseFactory.Create();
                this.EntityDefinitionProvider = this.DatabaseContext.DefinitionProvider;
                this.repositoryAdapter = adapterFactory.Create(this.DatabaseContext);
            }
            catch (InvalidOperationException ex)
            {
                throw new RepositoryException(this, ex.Message, ex);
            }
            catch (DataException ex)
            {
                throw new RepositoryException(this, ex.Message, ex);
            }
            catch (SqlException ex)
            {
                throw new RepositoryException(this, ex.Message, ex);
            }
        }

        /// <inheritdoc />
        public event EventHandler Disposed;

        /// <inheritdoc />
        public IEntityMapper EntityMapper { get; }

        /// <inheritdoc />
        public IEntityDefinitionProvider EntityDefinitionProvider { get; }

        /// <inheritdoc />
        public IDatabaseContext DatabaseContext { get; }

        /// <inheritdoc />
        public bool IsDisposed { get; private set; }

        /// <inheritdoc />
        public bool EnableCaching { get; set; }

        /// <inheritdoc />
        public TimeSpan CacheExpiration { get; set; }

        /// <summary>
        /// Gets the internal identifier for this provider.
        /// </summary>
        public Guid InstanceIdentifier { get; } = Guid.NewGuid();

        /// <inheritdoc />
        public IDependencyContainer DependencyContainer { get; private set; }

        /// <inheritdoc />
        public void SetDependencyContainer([NotNull] IDependencyContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            this.DependencyContainer = container;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.IsDisposed = true;

            this.DatabaseContext?.Dispose();

            this.OnDisposed();
        }

        /// <inheritdoc />
        public void ChangeDatabase(string databaseName)
        {
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentNullException(nameof(databaseName));
            }

            this.CheckDisposed();
            this.DatabaseContext.OpenSharedConnection();
            this.DatabaseContext.Connection.ChangeDatabase(databaseName);
        }

        /// <inheritdoc />
        public IDbTransaction StartTransaction()
        {
            try
            {
                this.CheckDisposed();
                return this.DatabaseContext.BeginTransaction();
            }
            catch (InvalidOperationException ex)
            {
                throw new RepositoryException(this, ex.Message, ex);
            }
            catch (DataException ex)
            {
                throw new RepositoryException(this, ex.Message, ex);
            }
            catch (SqlException ex)
            {
                throw new RepositoryException(this, ex.Message, ex);
            }
        }

        /// <inheritdoc />
        public IDbTransaction StartTransaction(IsolationLevel isolationLevel)
        {
            try
            {
                this.CheckDisposed();
                return this.DatabaseContext.Connection.BeginTransaction(isolationLevel);
            }
            catch (InvalidOperationException ex)
            {
                throw new RepositoryException(this, ex.Message, ex);
            }
            catch (DataException ex)
            {
                throw new RepositoryException(this, ex.Message, ex);
            }
            catch (SqlException ex)
            {
                throw new RepositoryException(this, ex.Message, ex);
            }
        }

        /// <inheritdoc />
        public void CompleteTransaction()
        {
            try
            {
                this.DatabaseContext.CompleteTransaction();
            }
            catch (InvalidOperationException ex)
            {
                throw new RepositoryException(this, ex.Message, ex);
            }
            catch (DataException ex)
            {
                throw new RepositoryException(this, ex.Message, ex);
            }
            catch (SqlException ex)
            {
                throw new RepositoryException(this, ex.Message, ex);
            }
        }

        /// <inheritdoc />
        public void AbortTransaction()
        {
            try
            {
                this.DatabaseContext.AbortTransaction();
            }
            catch (InvalidOperationException ex)
            {
                throw new RepositoryException(this, ex.Message, ex);
            }
            catch (DataException ex)
            {
                throw new RepositoryException(this, ex.Message, ex);
            }
            catch (SqlException ex)
            {
                throw new RepositoryException(this, ex.Message, ex);
            }
        }

        /// <inheritdoc />
        public bool Contains<TDataItem>(ItemSelection<TDataItem> selection)
            where TDataItem : ITransactionContext
        {
            if (Evaluate.IsNull(selection))
            {
                throw new ArgumentNullException(nameof(selection));
            }

            try
            {
                this.CheckDisposed();
                return this.repositoryAdapter.Contains(selection);
            }
            catch (InvalidOperationException ex)
            {
                throw new RepositoryException(selection, ex.Message, ex);
            }
            catch (DataException ex)
            {
                throw new RepositoryException(selection, ex.Message, ex);
            }
            catch (SqlException ex)
            {
                throw new RepositoryException(selection, ex.Message, ex);
            }
        }

        /// <inheritdoc />
        public TDataItem GetFirstOrDefault<TDataItem>(ItemSelection<TDataItem> selection)
            where TDataItem : ITransactionContext
        {
            if (Evaluate.IsNull(selection))
            {
                throw new ArgumentNullException(nameof(selection));
            }

            return this.FirstOrDefault(selection, this.EnableCaching);
        }

        /// <inheritdoc />
        public IEnumerable<TDataItem> GetSelection<TDataItem>(ItemSelection<TDataItem> selection)
            where TDataItem : ITransactionContext
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            this.CheckDisposed();
            return this.repositoryAdapter.SelectItems(selection);
        }

        /// <inheritdoc />
        public Page<TDataItem> GetSelection<TDataItem>(ItemSelection<TDataItem> selection, long pageSize, long page)
            where TDataItem : ITransactionContext
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            this.CheckDisposed();
            return this.repositoryAdapter.SelectItems(selection, pageSize, page);
        }

        /// <inheritdoc />
        /// <exception cref="Startitecture.Orm.Common.RepositoryException">
        /// The item could not be saved in the repository.
        /// </exception>
        public TDataItem Save<TDataItem>(TDataItem item)
            where TDataItem : ITransactionContext
        {
            // Only review the direct attributes of the object for save comparison.
            var entityDefinition = this.EntityDefinitionProvider.Resolve<TDataItem>();
            var attributeDefinitions = entityDefinition.DirectAttributes.ToList();
            var selectExpressions = (from a in attributeDefinitions
                                     let parameter = Expression.Parameter(typeof(TDataItem))
                                     let property = Expression.Property(parameter, a.PropertyInfo)
                                     let conversion = Expression.Convert(property, typeof(object))
                                     select Expression.Lambda<Func<TDataItem, object>>(conversion, parameter)).ToArray();

            var uniqueSelection = new UniqueQuery<TDataItem>(this.DatabaseContext.DefinitionProvider, item).Select(selectExpressions);

            // If caching is enabled, incoming items will be compared against the cache. This will catch forward changes (A1 -> A2) but 
            // will ignore reverse changes (A2 -> A1) until the cached item expires.
            var existingItem = this.FirstOrDefault(uniqueSelection, this.EnableCaching);

            var savePolicy = new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(2) };

            if (Evaluate.IsSet(existingItem))
            {
                // This needs to be done prior to the mapping so we know what is in the database.
                var existingValues = from a in attributeDefinitions
                                     select a.GetValueDelegate.DynamicInvoke(existingItem);
                ////existingItem.ToValueCollection();

                // Now apply the auto-number key from the existing item to the incoming item if necessary. This is for cases when the 
                // unique key is used rather than an auto-number ID. 
                var primaryKey = entityDefinition.AutoNumberPrimaryKey;
                var keyCompare = new Lazy<bool>(
                    () => primaryKey?.GetValueDelegate.DynamicInvoke(item) == primaryKey?.GetValueDelegate.DynamicInvoke(existingItem));

                if (primaryKey != null && keyCompare.Value == false)
                {
                    var existingKey = primaryKey.Value.GetValueDelegate.DynamicInvoke(existingItem);
                    primaryKey.Value.SetValueDelegate.DynamicInvoke(item, existingKey);
                }

                ////// When the item already exists, we use the data item mapping to merge fields. The data item mapping should ensure that
                ////// primary keys are ignored so that the merged item contains the primary key from the existing entity.
                ////// TODO: To eliminate mapping at this layer, eliminate write-once and use an internal mapper.
                ////var mergedItem = this.EntityMapper.MapTo(item, existingItem);
                var mergedValues = from a in attributeDefinitions
                                   select a.GetValueDelegate.DynamicInvoke(item);

                // Do not update unless needed. // TODO: Is this wise?
                if (existingValues.SequenceEqual(mergedValues))
                {
                    if (this.EnableCaching)
                    {
                        lock (this.itemLock)
                        {
                            this.itemCache.Set(CreateCacheKey(uniqueSelection), item, savePolicy);
                        }
                    }

                    item.SetTransactionProvider(this);
                    return item;
                }

                this.Update(item, uniqueSelection);

                // According to http://stackoverflow.com/questions/7477431/executenonquery-returning-a-value-of-2-when-only-1-record-was-updated,
                // if an update causes a trigger to fire then it's possible to get a 2 or higher with the rows affected, making this
                // test invalid in some scenarios.
                ////if (rowsAffected == 1)
                ////{
                if (this.EnableCaching)
                {
                    lock (this.itemLock)
                    {
                        this.itemCache.Set(CreateCacheKey(uniqueSelection), item, savePolicy);
                    }
                }

                item.SetTransactionProvider(this);
                return item;
            }

            var savedItem = this.InsertItem(item);

            if (this.EnableCaching)
            {
                lock (this.itemLock)
                {
                    this.itemCache.Set(CreateCacheKey(uniqueSelection), savedItem, savePolicy);
                }
            }

            savedItem.SetTransactionProvider(this);
            return savedItem;
        }

        /// <inheritdoc />
        public int DeleteItems<TDataItem>(ItemSelection<TDataItem> selection) 
            where TDataItem : ITransactionContext
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            this.CheckDisposed();
            return this.repositoryAdapter.DeleteSelection(selection);
        }

        /// <inheritdoc />
        public TDataItem InsertItem<TDataItem>(TDataItem dataItem)
            where TDataItem : ITransactionContext
        {
            if (Evaluate.IsNull(dataItem))
            {
                throw new ArgumentNullException(nameof(dataItem));
            }

            this.CheckDisposed();
            var item = this.repositoryAdapter.Insert(dataItem);
            item.SetTransactionProvider(this);
            return item;
        }

        /// <inheritdoc />
        public void UpdateItem<TDataItem>(TDataItem dataItem, params Expression<Func<TDataItem, object>>[] setExpressions)
        {
            if (dataItem == null)
            {
                throw new ArgumentNullException(nameof(dataItem));
            }

            if (setExpressions == null)
            {
                throw new ArgumentNullException(nameof(setExpressions));
            }

            // PetaPoco sees only one primary key. Obviously this is a problem as we do not want to update based on primary key 
            // alone. So we generate a unique selection.
            this.CheckDisposed();
            var selection = new UniqueQuery<TDataItem>(this.DatabaseContext.DefinitionProvider, dataItem);
            this.repositoryAdapter.Update(dataItem, selection, setExpressions);
        }

        /// <inheritdoc />
        public int Update<TDataItem>(
            [NotNull] TDataItem dataItem,
            [NotNull] ItemSelection<TDataItem> selection,
            [NotNull] params Expression<Func<TDataItem, object>>[] setExpressions) 
            where TDataItem : ITransactionContext
        {
            if (dataItem == null)
            {
                throw new ArgumentNullException(nameof(dataItem));
            }

            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            if (setExpressions == null)
            {
                throw new ArgumentNullException(nameof(setExpressions));
            }

            // PetaPoco sees only one primary key. Obviously this is a problem as we do not want to update based on primary key 
            // alone. So we generate a unique selection.
            this.CheckDisposed();
            return this.repositoryAdapter.Update(dataItem, selection, setExpressions);
        }

        /// <inheritdoc />
        public void Execute(string executionStatement, params object[] parameterValues)
        {
            if (parameterValues == null)
            {
                throw new ArgumentNullException(nameof(parameterValues));
            }

            if (string.IsNullOrWhiteSpace(executionStatement))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(executionStatement));
            }

            this.CheckDisposed();
            var autoSelect = this.DatabaseContext.EnableAutoSelect;
            this.DatabaseContext.EnableAutoSelect = false;
            this.DatabaseContext.Execute(executionStatement, parameterValues);
            this.DatabaseContext.EnableAutoSelect = autoSelect;
        }

        /// <inheritdoc />
        public T ExecuteScalar<T>(string executionStatement, params object[] parameterValues)
        {
            if (parameterValues == null)
            {
                throw new ArgumentNullException(nameof(parameterValues));
            }

            if (string.IsNullOrWhiteSpace(executionStatement))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(executionStatement));
            }

            this.CheckDisposed();
            var autoSelect = this.DatabaseContext.EnableAutoSelect;
            this.DatabaseContext.EnableAutoSelect = false;
            var result = this.DatabaseContext.ExecuteScalar<T>(executionStatement, parameterValues);
            this.DatabaseContext.EnableAutoSelect = autoSelect;
            return result;
        }

        /// <inheritdoc />
        public IEnumerable<dynamic> ExecuteForResult(string executionStatement, params object[] parameterValues)
        {
            if (parameterValues == null)
            {
                throw new ArgumentNullException(nameof(parameterValues));
            }

            if (string.IsNullOrWhiteSpace(executionStatement))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(executionStatement));
            }

            this.CheckDisposed();
            var autoSelect = this.DatabaseContext.EnableAutoSelect;
            this.DatabaseContext.EnableAutoSelect = false;
            var result = this.DatabaseContext.Fetch<dynamic>(executionStatement, parameterValues);
            this.DatabaseContext.EnableAutoSelect = autoSelect;
            return result;
        }

        /// <inheritdoc />
        public IEnumerable<T> ExecuteForResult<T>(string executionStatement, params object[] parameterValues)
        {
            if (parameterValues == null)
            {
                throw new ArgumentNullException(nameof(parameterValues));
            }

            if (string.IsNullOrWhiteSpace(executionStatement))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(executionStatement));
            }

            this.CheckDisposed();
            var autoSelect = this.DatabaseContext.EnableAutoSelect;
            this.DatabaseContext.EnableAutoSelect = false;
            var result = this.DatabaseContext.Fetch<T>(executionStatement, parameterValues);
            this.DatabaseContext.EnableAutoSelect = autoSelect;
            return result;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format(ToStringFormat, this.DatabaseContext?.Connection?.Database, this.InstanceIdentifier);
        }

        /// <summary>
        /// Creates cache key based on the specified selection.
        /// </summary>
        /// <param name="selection">
        /// The selection to create a key for.
        /// </param>
        /// <typeparam name="TDataItem">
        /// The type of data item in the selection.
        /// </typeparam>
        /// <returns>
        /// A <see cref="string"/> containing a unique key for the selection.
        /// </returns>
        private static string CreateCacheKey<TDataItem>(ItemSelection<TDataItem> selection)
        {
            return string.Format(CacheKeyFormat, typeof(TDataItem).ToRuntimeName(), selection);
        }

        /// <summary>
        /// Gets the first or default item matching the selection.
        /// </summary>
        /// <param name="selection">
        /// The selection that identifies the item.
        /// </param>
        /// <param name="useCache">
        /// A value indicating whether to use the cache.
        /// </param>
        /// <typeparam name="TDataItem">
        /// The type of data item to retrieve.
        /// </typeparam>
        /// <returns>
        /// The first matching <typeparamref name="TDataItem"/>, or the default value if no item is found.
        /// </returns>
        private TDataItem FirstOrDefault<TDataItem>(ItemSelection<TDataItem> selection, bool useCache)
            where TDataItem : ITransactionContext
        {
            this.CheckDisposed();
            TDataItem dataItem;

            if (useCache)
            {
                var cacheKey = CreateCacheKey(selection);

                lock (this.itemLock)
                {
                    dataItem = this.itemCache.GetOrLazyAddExisting(
                        this.itemLock,
                        cacheKey,
                        selection,
                        this.repositoryAdapter.FirstOrDefault,
                        new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(2) });
                }
            }
            else
            {
                dataItem = this.repositoryAdapter.FirstOrDefault(selection);
            }

            if (dataItem != null)
            {
                dataItem.SetTransactionProvider(this);
            }

            return dataItem;
        }

        /// <summary>
        /// Opens a connection if one is not already opened.
        /// </summary>
        private void CheckDisposed()
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(Convert.ToString(this));
            }
        }

        /// <summary>
        /// Triggers the <see cref="Disposed"/> event.
        /// </summary>
        private void OnDisposed()
        {
            var handler = this.Disposed;

            handler?.Invoke(this, EventArgs.Empty);
        }
    }
}
