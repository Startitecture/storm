// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseRepositoryProvider.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Provides a concrete implementation for a database repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.Caching;

    using JetBrains.Annotations;

    using SAF.StringResources;

    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Query;

    /// <summary>
    /// Provides a concrete implementation for a database repository.
    /// </summary>
    /// <typeparam name="TContext">
    /// The type of data context to provide access to.
    /// </typeparam>
    public sealed class DatabaseRepositoryProvider<TContext> : IRepositoryProvider, IDatabaseContextProvider
        where TContext : Database
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
        /// Gets the internal identifier for this provider.
        /// </summary>
        private readonly Guid internalIdentifier = Guid.NewGuid();

        /// <summary>
        /// The data context for this repository. Required to maintain a specific Exists method signature.
        /// </summary>
        private readonly TContext dataContext;

        /// <summary>
        /// The item cache.
        /// </summary>
        private readonly ObjectCache itemCache;

        /// <summary>
        /// The item lock.
        /// </summary>
        private readonly object itemLock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SAF.Data.Providers.DatabaseRepositoryProvider`1"/> class.
        /// </summary>
        /// <param name="entityMapper">
        /// The entity mapper.
        /// </param>
        public DatabaseRepositoryProvider(IEntityMapper entityMapper)
            : this(new QueryRepositoryAdapterFactory(), entityMapper)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SAF.Data.Providers.DatabaseRepositoryProvider`1"/> class.
        /// </summary>
        /// <param name="adapterFactory">
        /// The repository adapter.
        /// </param>
        /// <param name="entityMapper">
        /// The entity mapper.
        /// </param>
        public DatabaseRepositoryProvider(IRepositoryAdapterFactory adapterFactory, IEntityMapper entityMapper)
            : this(DefaultDatabaseFactory<TContext>.Default, adapterFactory, entityMapper)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SAF.Data.Providers.DatabaseRepositoryProvider`1"/> class.
        /// </summary>
        /// <param name="databaseFactory">
        /// The database factory.
        /// </param>
        /// <param name="entityMapper">
        /// The entity mapper.
        /// </param>
        public DatabaseRepositoryProvider(IDatabaseFactory<TContext> databaseFactory, IEntityMapper entityMapper)
            : this(databaseFactory, new QueryRepositoryAdapterFactory(), entityMapper)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SAF.Data.Providers.DatabaseRepositoryProvider`1"/> class.
        /// </summary>
        /// <param name="databaseFactory">
        /// The database factory.
        /// </param>
        /// <param name="adapterFactory">
        /// The repository adapter.
        /// </param>
        /// <param name="entityMapper">
        /// The entity mapper.
        /// </param>
        public DatabaseRepositoryProvider(
            IDatabaseFactory<TContext> databaseFactory,
            IRepositoryAdapterFactory adapterFactory,
            IEntityMapper entityMapper)
        {
            if (databaseFactory == null)
            {
                throw new ArgumentNullException(nameof(databaseFactory));
            }

            if (adapterFactory == null)
            {
                throw new ArgumentNullException(nameof(adapterFactory));
            }

            this.EntityMapper = entityMapper;
            this.DependencyContainer = new DependencyContainer();
            this.itemCache = MemoryCache.Default;

            ConfigurationManager.AppSettings.ApplySetting(this, provider => provider.EnableCaching, false, bool.Parse);
            ConfigurationManager.AppSettings.ApplySetting(this, provider => provider.CacheExpiration, this.cacheTime, TimeSpan.Parse);

            try
            {
                this.dataContext = databaseFactory.Create();
                this.repositoryAdapter = adapterFactory.Create(this.dataContext);
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

        /// <summary>
        /// Occurs when the provider is disposed.
        /// </summary>
        public event EventHandler Disposed;

        /// <summary>
        /// Gets the entity mapper.
        /// </summary>
        public IEntityMapper EntityMapper { get; private set; }

        /// <summary>
        /// Gets the current database context.
        /// </summary>
        public IDatabaseContext DatabaseContext
        {
            get
            {
                return this.dataContext;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current instance is disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable caching for the current provider.
        /// </summary>
        public bool EnableCaching { get; set; }

        /// <summary>
        /// Gets or sets the amount of time after which cached items will expire.
        /// </summary>
        public TimeSpan CacheExpiration { get; set; }

        /// <summary>
        /// Gets the internal identifier for this provider.
        /// </summary>
        public Guid InstanceIdentifier
        {
            get
            {
                return this.internalIdentifier;
            }
        }

        /// <summary>
        /// Gets the dependency container.
        /// </summary>
        public IDependencyContainer DependencyContainer { get; private set; }

        /// <summary>
        /// Sets the dependency container for the item.
        /// </summary>
        /// <param name="container">
        /// The container that the item should use.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="container"/> is null.
        /// </exception>
        public void SetDependencyContainer([NotNull] IDependencyContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            this.DependencyContainer = container;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.IsDisposed = true;

            if (this.dataContext != null)
            {
                this.dataContext.Dispose();
            }

            this.OnDisposed();
        }

        /// <summary>
        /// Changes the database of the current provider.
        /// </summary>
        /// <param name="databaseName">
        /// The name of the database to switch to.
        /// </param>
        public void ChangeDatabase(string databaseName)
        {
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentNullException(nameof(databaseName));
            }

            this.CheckDisposed();
            this.dataContext.OpenSharedConnection();
            this.dataContext.Connection.ChangeDatabase(databaseName);
        }

        /// <summary>
        /// Starts a transaction in the repository.
        /// </summary>
        /// <returns>
        /// The <see cref="IDbTransaction"/> started by the provider.
        /// </returns>
        public IDbTransaction StartTransaction()
        {
            try
            {
                this.CheckDisposed();
                return this.dataContext.BeginTransaction();
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

        /// <summary>
        /// Start a transaction in the repository.
        /// </summary>
        /// <param name="isolationLevel">
        /// The isolation level for the transaction.
        /// </param>
        /// <returns>
        /// The <see cref="IDbTransaction"/> started by the provider.
        /// </returns>
        public IDbTransaction StartTransaction(IsolationLevel isolationLevel)
        {
            try
            {
                this.CheckDisposed();
                return this.dataContext.Connection.BeginTransaction(isolationLevel);
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

        /// <summary>
        /// Complete a transaction in the repository.
        /// </summary>
        public void CompleteTransaction()
        {
            try
            {
                this.dataContext.CompleteTransaction();
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

        /// <summary>
        /// Abort a transaction, rolling back changes in the repository.
        /// </summary>
        public void AbortTransaction()
        {
            try
            {
                this.dataContext.AbortTransaction();
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

        /// <summary>
        /// Determines whether an item exists given the specified unique key.
        /// </summary>
        /// <typeparam name="TDataItem">
        /// The type of data item in the repository.
        /// </typeparam>
        /// <param name="selection">
        /// A selection that contains the SQL filter and values to select the item.
        /// </param>
        /// <returns>
        /// <c>true</c> if the item exists; otherwise, <c>false</c>.
        /// </returns>
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

        /// <summary>
        /// Gets the first item matching the filter, or the default value if the item cannot be found.
        /// </summary>
        /// <typeparam name="TDataItem">
        /// The type of data item in the repository.
        /// </typeparam>
        /// <param name="selection">
        /// The data item that represents the item to retrieve.
        /// </param>
        /// <returns>
        /// The first <typeparamref name="TDataItem"/> item matching the filter, or the default value if no matching item is found.
        /// </returns>
        public TDataItem GetFirstOrDefault<TDataItem>(ItemSelection<TDataItem> selection)
            where TDataItem : ITransactionContext
        {
            if (Evaluate.IsNull(selection))
            {
                throw new ArgumentNullException(nameof(selection));
            }

            return this.FirstOrDefault(selection, this.EnableCaching);
        }

        /// <summary>
        /// Selects a matching list of items from the repository.
        /// </summary>
        /// <typeparam name="TDataItem">
        /// The type of data item in the repository.
        /// </typeparam>
        /// <param name="selection">
        /// A selection that contains the SQL filter and values to select the item.
        /// </param>
        /// <returns>
        /// A collection of items that match the filter.
        /// </returns>
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

        /// <summary>
        /// Selects a matching list of items from the repository.
        /// </summary>
        /// <typeparam name="TDataItem">
        /// The type of data item in the repository.
        /// </typeparam>
        /// <param name="selection">
        /// A selection that contains the SQL filter and values to select the item.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <param name="page">
        /// The 1-based page to return.
        /// </param>
        /// <returns>
        /// A collection of items that match the filter.
        /// </returns>
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

        /// <summary>
        /// Saves an item into the repository.
        /// </summary>
        /// <typeparam name="TDataItem">
        /// The type of data item in the repository.
        /// </typeparam>
        /// <param name="item">
        /// The item to save.
        /// </param>
        /// <param name="selection">
        /// The selection to use to uniquely select the item.
        /// </param>
        /// <returns>
        /// The saved item as a <typeparamref name="TDataItem"/>.
        /// </returns>
        /// <exception cref="RepositoryException">
        /// The item could not be saved in the repository.
        /// </exception>
        public TDataItem Save<TDataItem>(TDataItem item, ItemSelection<TDataItem> selection)
            where TDataItem : ITransactionContext
        {
            // If caching is enabled, incoming items will be compared against the cache. This will catch forward changes (A1 -> A2) but 
            // will ignore reverse changes (A2 -> A1) until the cached item expires.
            var existingItem = this.FirstOrDefault(selection, this.EnableCaching);

            var savePolicy = new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(2) };

            if (Evaluate.IsSet(existingItem))
            {
                // This needs to be done prior to the mapping so we know what is in the database.
                var existingValues = existingItem.ToValueCollection();

                // When the item already exists, we use the data item mapping to merge fields. The data item mapping should ensure that
                // primary keys are ignored so that the merged item contains the primary key from the existing entity.
                var mergedItem = this.EntityMapper.MapTo(item, existingItem);
                var mergedValues = mergedItem.ToValueCollection();

                // Do not update unless needed.
                if (existingValues.SequenceEqual(mergedValues))
                {
                    if (this.EnableCaching)
                    {
                        lock (this.itemLock)
                        {
                            this.itemCache.Set(CreateCacheKey(selection), mergedItem, savePolicy);
                        }
                    }

                    mergedItem.SetTransactionProvider(this);
                    return mergedItem;
                }

                this.Update(mergedItem, selection);

                // According to http://stackoverflow.com/questions/7477431/executenonquery-returning-a-value-of-2-when-only-1-record-was-updated,
                // if an update causes a trigger to fire then it's possible to get a 2 or higher with the rows affected, making this
                // test invalid in some scenarios.
                ////if (rowsAffected == 1)
                ////{
                if (this.EnableCaching)
                    {
                        lock (this.itemLock)
                        {
                            this.itemCache.Set(CreateCacheKey(selection), mergedItem, savePolicy);
                        }
                    }

                    mergedItem.SetTransactionProvider(this);
                    return mergedItem;
                ////}

                ////if (rowsAffected > 1)
                ////{
                ////    string message = string.Format(ErrorMessages.DataItemUpdateAffectedMultipleRows, typeof(TDataItem).Name, mergedItem);
                ////    throw new RepositoryException(mergedItem, message);
                ////}
                ////else
                ////{
                ////    string message = string.Format(ErrorMessages.DataItemUpdateNotApplied, typeof(TDataItem).Name, mergedItem);
                ////    throw new RepositoryException(mergedItem, message);
                ////}
            }

            var savedItem = this.InsertItem(item);

            if (this.EnableCaching)
            {
                lock (this.itemLock)
                {
                    this.itemCache.Set(CreateCacheKey(selection), savedItem, savePolicy);
                }
            }

            savedItem.SetTransactionProvider(this);
            return savedItem;
        }

        /// <summary>
        /// Deletes the items matching the filter.
        /// </summary>
        /// <typeparam name="TDataItem">
        /// The type of data item in the repository.
        /// </typeparam>
        /// <param name="selection">
        /// A selection that contains the SQL filter and values to select the item.
        /// </param>
        /// <returns>
        /// The number of deleted items as an <see cref="int"/>.
        /// </returns>
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

        /// <summary>
        /// Inserts a data item into the repository.
        /// </summary>
        /// <typeparam name="TDataItem">
        /// The type of data item in the repository.
        /// </typeparam>
        /// <param name="dataItem">
        /// The data item to insert.
        /// </param>
        /// <returns>
        /// The inserted <typeparamref name="TDataItem"/>.
        /// </returns>
        /// <exception cref="RepositoryException">
        /// The insert operation failed, or there was an error mapping between the model and the data item.
        /// </exception>
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

        /// <summary>
        /// Updates a selection of items in the repository.
        /// </summary>
        /// <typeparam name="TDataItem">
        /// The type of data item in the repository.
        /// </typeparam>
        /// <param name="dataItem">
        /// The item that contains the update.
        /// </param>
        /// <param name="selection">
        /// The selection to update.
        /// </param>
        /// <param name="setExpressions">
        /// A optional set of expressions that explicitly select the columns to update. If empty, all non-key columns are updated.
        /// </param>
        /// <returns>
        /// The number of updated rows.
        /// </returns>
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

        /// <summary>
        /// Executes the specified operation.
        /// </summary>
        /// <param name="executionStatement">
        /// The execution statement.
        /// </param>
        /// <param name="parameterValues">
        /// The parameter values.
        /// </param>
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
            var autoSelect = this.dataContext.EnableAutoSelect;
            this.dataContext.EnableAutoSelect = false;
            this.dataContext.Execute(executionStatement, parameterValues);
            this.dataContext.EnableAutoSelect = autoSelect;
        }

        /// <summary>
        /// Executes the specified operation for a scalar result.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the result value.
        /// </typeparam>
        /// <param name="executionStatement">
        /// The execution statement.
        /// </param>
        /// <param name="parameterValues">
        /// The parameter values.
        /// </param>
        /// <returns>
        /// The first column of the first row of the result as a type of <typeparamref name="T"/>.
        /// </returns>
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
            var autoSelect = this.dataContext.EnableAutoSelect;
            this.dataContext.EnableAutoSelect = false;
            var result = this.dataContext.ExecuteScalar<T>(executionStatement, parameterValues);
            this.dataContext.EnableAutoSelect = autoSelect;
            return result;
        }

        /// <summary>
        /// Executes the specified operation for a table result.
        /// </summary>
        /// <param name="executionStatement">
        /// The execution statement.
        /// </param>
        /// <param name="parameterValues">
        /// The parameter values.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of objects returned by the statement.
        /// </returns>
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
            var autoSelect = this.dataContext.EnableAutoSelect;
            this.dataContext.EnableAutoSelect = false;
            var result = this.dataContext.Fetch<dynamic>(executionStatement, parameterValues);
            this.dataContext.EnableAutoSelect = autoSelect;
            return result;
        }

        /// <summary>
        /// Executes the specified operation for a table result.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the result.
        /// </typeparam>
        /// <param name="executionStatement">
        /// The execution statement.
        /// </param>
        /// <param name="parameterValues">
        /// The parameter values.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of items in the type of <typeparamref name="T"/>.
        /// </returns>
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
            var autoSelect = this.dataContext.EnableAutoSelect;
            this.dataContext.EnableAutoSelect = false;
            var result = this.dataContext.Fetch<T>(executionStatement, parameterValues);
            this.dataContext.EnableAutoSelect = autoSelect;
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
            return string.Format(ToStringFormat, this.dataContext?.Connection?.Database, this.InstanceIdentifier);
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

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
