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
    using System.Data;
    using System.Data.Common;
    using System.Globalization;
    using System.Linq.Expressions;
    using System.Runtime.Caching;

    using Common;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Orm.Model;
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

/*
        /// <summary>
        /// The cache time.
        /// </summary>
        private readonly TimeSpan cacheTime = TimeSpan.FromMinutes(2);
*/

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
        /// <param name="adapterFactory">
        /// The repository adapter.
        /// </param>
        public DatabaseRepositoryProvider(
            [NotNull] IDatabaseFactory databaseFactory,
            [NotNull] IRepositoryAdapterFactory adapterFactory)
            : this(databaseFactory, adapterFactory, MemoryCache.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseRepositoryProvider"/> class. 
        /// </summary>
        /// <param name="databaseFactory">
        /// The database factory.
        /// </param>
        /// <param name="adapterFactory">
        /// The repository adapter.
        /// </param>
        /// <param name="itemCache">
        /// The item cache.
        /// </param>
        public DatabaseRepositoryProvider(
            [NotNull] IDatabaseFactory databaseFactory,
            [NotNull] IRepositoryAdapterFactory adapterFactory,
            [NotNull] ObjectCache itemCache)
        {
            if (adapterFactory == null)
            {
                throw new ArgumentNullException(nameof(adapterFactory));
            }

            if (databaseFactory == null)
            {
                throw new ArgumentNullException(nameof(databaseFactory));
            }

            this.itemCache = itemCache ?? throw new ArgumentNullException(nameof(itemCache));

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
            catch (DbException ex)
            {
                throw new RepositoryException(this, ex.Message, ex);
            }
        }

        /// <inheritdoc />
        public event EventHandler Disposed;

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
        private Guid InstanceIdentifier { get; } = Guid.NewGuid();

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
            catch (DbException ex)
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
            catch (DbException ex)
            {
                throw new RepositoryException(this, ex.Message, ex);
            }
        }

/*
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
            catch (DbException ex)
            {
                throw new RepositoryException(this, ex.Message, ex);
            }
        }
*/

/*
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
            catch (DbException ex)
            {
                throw new RepositoryException(this, ex.Message, ex);
            }
        }
*/

        /// <inheritdoc />
        public bool Contains<TDataItem>(EntitySelection<TDataItem> selection)
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
            catch (DbException ex)
            {
                throw new RepositoryException(selection, ex.Message, ex);
            }
        }

        /// <inheritdoc />
        public T GetScalar<T>([NotNull] ISelection selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            return this.repositoryAdapter.ExecuteScalar<T>(selection);
        }

        /// <inheritdoc />
        public TDataItem GetFirstOrDefault<TDataItem>(EntitySelection<TDataItem> selection)
            where TDataItem : ITransactionContext
        {
            if (Evaluate.IsNull(selection))
            {
                throw new ArgumentNullException(nameof(selection));
            }

            return this.FirstOrDefault(selection, this.EnableCaching);
        }

        /// <inheritdoc />
        public IEnumerable<TDataItem> GetSelection<TDataItem>(EntitySelection<TDataItem> selection)
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
        public int DeleteItems<TDataItem>(EntitySelection<TDataItem> selection) 
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

            this.CheckDisposed();
            var selection = new UniqueQuery<TDataItem>(this.DatabaseContext.DefinitionProvider, dataItem);
            this.repositoryAdapter.Update(dataItem, selection, setExpressions);
        }

        /// <inheritdoc />
        public int Update<TDataItem>(
            [NotNull] TDataItem dataItem,
            [NotNull] EntitySelection<TDataItem> selection,
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
            this.DatabaseContext.Execute(executionStatement, parameterValues);
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
            var result = this.DatabaseContext.ExecuteScalar<T>(executionStatement, parameterValues);
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
            var result = this.DatabaseContext.Query<dynamic>(executionStatement, parameterValues);
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
            var result = this.DatabaseContext.Query<T>(executionStatement, parameterValues);
            return result;
        }

        /// <summary>
        /// Returns a <see cref="String"/> that represents the current <see cref="Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="String"/> that represents the current <see cref="Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, ToStringFormat, this.DatabaseContext?.Connection?.Database, this.InstanceIdentifier);
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
        private static string CreateCacheKey<TDataItem>(EntitySelection<TDataItem> selection)
        {
            return string.Format(CultureInfo.InvariantCulture, CacheKeyFormat, typeof(TDataItem).ToRuntimeName(), selection);
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
        private TDataItem FirstOrDefault<TDataItem>(EntitySelection<TDataItem> selection, bool useCache)
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
                throw new ObjectDisposedException(Convert.ToString(this, CultureInfo.CurrentCulture));
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
