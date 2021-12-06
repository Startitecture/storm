// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseRepositoryProvider.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Provides a concrete implementation for a database repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.Caching;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Orm.Model;

    /// <summary>
    /// Provides a concrete implementation for a database repository.
    /// </summary>
    /// <remarks>
    /// This <see cref="IRepositoryProvider"/> implementation wraps all expected underlying exceptions with <see cref="Startitecture.Orm.Common.RepositoryException"/>.
    /// </remarks>
    public sealed class DatabaseRepositoryProvider : IRepositoryProvider
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
        /// The entity cache.
        /// </summary>
        private readonly ObjectCache entityCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseRepositoryProvider"/> class.
        /// </summary>
        /// <param name="databaseContextFactory">
        /// The database context factory.
        /// </param>
        public DatabaseRepositoryProvider(
            [NotNull] IDatabaseContextFactory databaseContextFactory)
            : this(databaseContextFactory, MemoryCache.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseRepositoryProvider"/> class.
        /// </summary>
        /// <param name="databaseContextFactory">
        /// The database context factory.
        /// </param>
        /// <param name="entityCache">
        /// The entity cache.
        /// </param>
        public DatabaseRepositoryProvider(
            [NotNull] IDatabaseContextFactory databaseContextFactory,
            [NotNull] ObjectCache entityCache)
        {
            if (databaseContextFactory == null)
            {
                throw new ArgumentNullException(nameof(databaseContextFactory));
            }

            this.entityCache = entityCache ?? throw new ArgumentNullException(nameof(entityCache));

            try
            {
                this.DatabaseContext = databaseContextFactory.Create();
                this.EntityDefinitionProvider = this.DatabaseContext.RepositoryAdapter.DefinitionProvider;
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
        public CacheItemPolicy CacheItemPolicy { get; set; }

        /// <summary>
        /// Gets the internal identifier for this provider.
        /// </summary>
        private Guid InstanceIdentifier { get; } = Guid.NewGuid();

        /// <inheritdoc />
        public ITransactionContext BeginTransaction()
        {
            return this.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        /// <param name="cancellationToken"></param>
        /// <inheritdoc />
        public async Task<ITransactionContext> BeginTransactionAsync(CancellationToken cancellationToken)
        {
            return await this.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public ITransactionContext BeginTransaction(IsolationLevel isolationLevel)
        {
            this.CheckDisposed();

            try
            {
                this.DatabaseContext.OpenSharedConnection();
                return this.DatabaseContext.BeginTransaction(isolationLevel);
            }
            catch (InvalidOperationException ex)
            {
                throw new RepositoryException(this, ex.Message, ex);
            }
            catch (DbException ex)
            {
                throw new RepositoryException(this, ex.Message, ex);
            }
        }

        /// <inheritdoc />
        public async Task<ITransactionContext> BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken)
        {
            this.CheckDisposed();

            try
            {
                await this.DatabaseContext.OpenSharedConnectionAsync(cancellationToken).ConfigureAwait(false);
                return await this.DatabaseContext.BeginTransactionAsync(isolationLevel, cancellationToken).ConfigureAwait(false);
            }
            catch (InvalidOperationException ex)
            {
                throw new RepositoryException(this, ex.Message, ex);
            }
            catch (DbException ex)
            {
                throw new RepositoryException(this, ex.Message, ex);
            }
        }

        /// <inheritdoc />
        public bool Contains(IEntitySet selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            this.CheckDisposed();
            var sql = this.DatabaseContext.RepositoryAdapter.CreateExistsStatement(selection);

            try
            {
                // Always remember to supply this method with an array of values!
                this.DatabaseContext.OpenSharedConnection();
                return this.DatabaseContext.ExecuteScalar<int>(sql, selection.PropertyValues.ToArray()) > 0;
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
        public async Task<bool> ContainsAsync(IEntitySet selection, CancellationToken cancellationToken)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            this.CheckDisposed();
            var sql = this.DatabaseContext.RepositoryAdapter.CreateExistsStatement(selection);

            try
            {
                // Always remember to supply this method with an array of values!
                return await this.DatabaseContext.ExecuteScalarAsync<int>(sql, cancellationToken, selection.PropertyValues.ToArray())
                           .ConfigureAwait(false)
                       > 0;
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

            this.CheckDisposed();
            var statement = this.DatabaseContext.RepositoryAdapter.CreateSelectionStatement(selection);

            try
            {
                this.DatabaseContext.OpenSharedConnection();
                return this.DatabaseContext.ExecuteScalar<T>(statement, selection.PropertyValues.ToArray());
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
        public async Task<T> GetScalarAsync<T>(ISelection selection, CancellationToken cancellationToken)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            this.CheckDisposed();
            var statement = this.DatabaseContext.RepositoryAdapter.CreateSelectionStatement(selection);

            try
            {
                await this.DatabaseContext.OpenSharedConnectionAsync(cancellationToken).ConfigureAwait(false);
                return await this.DatabaseContext.ExecuteScalarAsync<T>(statement, cancellationToken, selection.PropertyValues.ToArray())
                           .ConfigureAwait(false);
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
        public T FirstOrDefault<T>(EntitySet<T> entitySet)
        {
            if (entitySet == null)
            {
                throw new ArgumentNullException(nameof(entitySet));
            }

            this.CheckDisposed();
            T entity;

            if (this.CacheItemPolicy != null)
            {
                var cacheKey = CreateCacheKey(entitySet);
                var lazyGet = new Lazy<T>(() => this.FirstOrDefaultEntity<T>(entitySet));
                entity = ((Lazy<T>)this.entityCache.AddOrGetExisting(cacheKey, lazyGet, this.CacheItemPolicy)).Value;
            }
            else
            {
                entity = this.FirstOrDefaultEntity<T>(entitySet);
            }

            return entity;
        }

        /// <inheritdoc />
        public async Task<T> FirstOrDefaultAsync<T>(EntitySet<T> entitySet, CancellationToken cancellationToken)
        {
            if (entitySet == null)
            {
                throw new ArgumentNullException(nameof(entitySet));
            }

            this.CheckDisposed();
            T entity;

            if (this.CacheItemPolicy != null)
            {
                var cacheKey = CreateCacheKey(entitySet);
                var lazyGet = new Lazy<Task<T>>(
                    async () => await this.FirstOrDefaultEntityAsync<T>(entitySet, cancellationToken).ConfigureAwait(false));

                entity = ((Lazy<T>)this.entityCache.AddOrGetExisting(cacheKey, lazyGet, this.CacheItemPolicy)).Value;
            }
            else
            {
                entity = await this.FirstOrDefaultEntityAsync<T>(entitySet, cancellationToken).ConfigureAwait(false);
            }

            return entity;
        }

        /// <inheritdoc />
        public dynamic DynamicFirstOrDefault([NotNull] ISelection selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            this.CheckDisposed();
            var item = this.FirstOrDefaultEntity<dynamic>(selection);
            return item;
        }

        /// <inheritdoc />
        public async Task<dynamic> DynamicFirstOrDefaultAsync(ISelection selection, CancellationToken cancellationToken)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            this.CheckDisposed();
            var item = await this.FirstOrDefaultEntityAsync<dynamic>(selection, cancellationToken).ConfigureAwait(false);
            return item;
        }

        /// <inheritdoc />
        public IEnumerable<T> SelectEntities<T>(EntitySet<T> selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            this.CheckDisposed();
            var statement = this.DatabaseContext.RepositoryAdapter.CreateSelectionStatement(selection);

            try
            {
                this.DatabaseContext.OpenSharedConnection();
                return this.DatabaseContext.Query<T>(statement, selection.PropertyValues.ToArray());
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
        public async IAsyncEnumerable<T> SelectEntitiesAsync<T>(EntitySet<T> selection, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            this.CheckDisposed();
            var statement = this.DatabaseContext.RepositoryAdapter.CreateSelectionStatement(selection);

            try
            {
                await this.DatabaseContext.OpenSharedConnectionAsync(cancellationToken).ConfigureAwait(false);
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

            await foreach (var item in this.DatabaseContext.QueryAsync<T>(statement, cancellationToken, selection.PropertyValues.ToArray())
                               .ConfigureAwait(false))
            {
                yield return item;
            }
        }

        /// <inheritdoc />
        public IEnumerable<dynamic> DynamicSelect([NotNull] ISelection selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            this.CheckDisposed();
            var statement = this.DatabaseContext.RepositoryAdapter.CreateSelectionStatement(selection);

            try
            {
                this.DatabaseContext.OpenSharedConnection();
                return this.DatabaseContext.Query<dynamic>(statement, selection.PropertyValues.ToArray());
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
        public async IAsyncEnumerable<dynamic> DynamicSelectAsync(ISelection selection, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            this.CheckDisposed();
            var statement = this.DatabaseContext.RepositoryAdapter.CreateSelectionStatement(selection);

            try
            {
                await this.DatabaseContext.OpenSharedConnectionAsync(cancellationToken).ConfigureAwait(false);
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

            await foreach (var item in this.DatabaseContext.QueryAsync<dynamic>(statement, cancellationToken, selection.PropertyValues.ToArray())
                               .ConfigureAwait(false))
            {
                yield return item;
            }
        }

        /// <inheritdoc />
        public T Insert<T>(T entity)
        {
            if (Evaluate.IsNull(entity))
            {
                throw new ArgumentNullException(nameof(entity));
            }

            this.CheckDisposed();
            var statement = this.DatabaseContext.RepositoryAdapter.CreateInsertionStatement<T>();
            var entityDefinition = this.EntityDefinitionProvider.Resolve<T>();
            var values = entityDefinition.InsertableAttributes.Select(definition => definition.GetValueMethod.Invoke(entity, null)).ToArray();

            try
            {
                this.DatabaseContext.OpenSharedConnection();

                if (entityDefinition.RowIdentity.HasValue)
                {
                    var autoNumber = this.DatabaseContext.ExecuteScalar<object>(statement, values);
                    entityDefinition.RowIdentity.Value.SetValueDelegate.DynamicInvoke(entity, autoNumber);
                }
                else
                {
                    this.DatabaseContext.Execute(statement, values);
                }
            }
            catch (InvalidOperationException ex)
            {
                throw new RepositoryException(entity, ex.Message, ex);
            }
            catch (DataException ex)
            {
                throw new RepositoryException(entity, ex.Message, ex);
            }
            catch (DbException ex)
            {
                throw new RepositoryException(entity, ex.Message, ex);
            }

            return entity;
        }

        /// <inheritdoc />
        public async Task<T> InsertAsync<T>(T entity, CancellationToken cancellationToken)
        {
            if (Evaluate.IsNull(entity))
            {
                throw new ArgumentNullException(nameof(entity));
            }

            this.CheckDisposed();
            var statement = this.DatabaseContext.RepositoryAdapter.CreateInsertionStatement<T>();
            var entityDefinition = this.EntityDefinitionProvider.Resolve<T>();
            var values = entityDefinition.InsertableAttributes.Select(definition => definition.GetValueMethod.Invoke(entity, null)).ToArray();

            try
            {
                await this.DatabaseContext.OpenSharedConnectionAsync(cancellationToken).ConfigureAwait(false);

                if (entityDefinition.RowIdentity.HasValue)
                {
                    var autoNumber = await this.DatabaseContext.ExecuteScalarAsync<object>(statement, cancellationToken, values)
                                         .ConfigureAwait(false);

                    entityDefinition.RowIdentity.Value.SetValueDelegate.DynamicInvoke(entity, autoNumber);
                }
                else
                {
                    await this.DatabaseContext.ExecuteAsync(statement, cancellationToken, values).ConfigureAwait(false);
                }
            }
            catch (InvalidOperationException ex)
            {
                throw new RepositoryException(entity, ex.Message, ex);
            }
            catch (DataException ex)
            {
                throw new RepositoryException(entity, ex.Message, ex);
            }
            catch (DbException ex)
            {
                throw new RepositoryException(entity, ex.Message, ex);
            }

            return entity;
        }

        /// <inheritdoc />
        public int Update<T>([NotNull] UpdateSet<T> updateSet)
        {
            if (updateSet == null)
            {
                throw new ArgumentNullException(nameof(updateSet));
            }

            var updateStatement = this.DatabaseContext.RepositoryAdapter.CreateUpdateStatement(updateSet);

            try
            {
                // Always use ToArray()!
                this.DatabaseContext.OpenSharedConnection();
                return this.DatabaseContext.Execute(updateStatement, updateSet.PropertyValues.ToArray());
            }
            catch (InvalidOperationException ex)
            {
                throw new RepositoryException(updateSet, ex.Message, ex);
            }
            catch (DataException ex)
            {
                throw new RepositoryException(updateSet, ex.Message, ex);
            }
            catch (DbException ex)
            {
                throw new RepositoryException(updateSet, ex.Message, ex);
            }
        }

        /// <inheritdoc />
        public async Task<int> UpdateAsync<T>(UpdateSet<T> updateSet, CancellationToken cancellationToken)
        {
            if (updateSet == null)
            {
                throw new ArgumentNullException(nameof(updateSet));
            }

            var updateStatement = this.DatabaseContext.RepositoryAdapter.CreateUpdateStatement(updateSet);

            try
            {
                // Always use ToArray()!
                await this.DatabaseContext.OpenSharedConnectionAsync(cancellationToken).ConfigureAwait(false);
                return await this.DatabaseContext.ExecuteAsync(updateStatement, cancellationToken, updateSet.PropertyValues.ToArray())
                           .ConfigureAwait(false);
            }
            catch (InvalidOperationException ex)
            {
                throw new RepositoryException(updateSet, ex.Message, ex);
            }
            catch (DataException ex)
            {
                throw new RepositoryException(updateSet, ex.Message, ex);
            }
            catch (DbException ex)
            {
                throw new RepositoryException(updateSet, ex.Message, ex);
            }
        }

        /// <inheritdoc />
        public void UpdateSingle<T>(T entity, params Expression<Func<T, object>>[] setExpressions)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (setExpressions == null)
            {
                throw new ArgumentNullException(nameof(setExpressions));
            }

            this.CheckDisposed();
            var update = setExpressions.Any()
                             ? new UpdateSet<T>().Set(entity, setExpressions)
                                 .Where(set => set.MatchKey(entity, this.DatabaseContext.RepositoryAdapter.DefinitionProvider))
                             : new UpdateSet<T>().Set(entity, this.EntityDefinitionProvider)
                                 .Where(set => set.MatchKey(entity, this.DatabaseContext.RepositoryAdapter.DefinitionProvider));

            var updateStatement = this.DatabaseContext.RepositoryAdapter.CreateUpdateStatement(update);

            try
            {
                // Always use ToArray()!
                this.DatabaseContext.OpenSharedConnection();
                this.DatabaseContext.Execute(updateStatement, update.PropertyValues.ToArray());
            }
            catch (InvalidOperationException ex)
            {
                throw new RepositoryException(entity, ex.Message, ex);
            }
            catch (DataException ex)
            {
                throw new RepositoryException(entity, ex.Message, ex);
            }
            catch (DbException ex)
            {
                throw new RepositoryException(entity, ex.Message, ex);
            }
        }

        /// <inheritdoc />
        public async Task UpdateSingleAsync<T>(T entity, CancellationToken cancellationToken, params Expression<Func<T, object>>[] setExpressions)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (setExpressions == null)
            {
                throw new ArgumentNullException(nameof(setExpressions));
            }

            this.CheckDisposed();
            var update = setExpressions.Any()
                             ? new UpdateSet<T>().Set(entity, setExpressions)
                                 .Where(set => set.MatchKey(entity, this.DatabaseContext.RepositoryAdapter.DefinitionProvider))
                             : new UpdateSet<T>().Set(entity, this.EntityDefinitionProvider)
                                 .Where(set => set.MatchKey(entity, this.DatabaseContext.RepositoryAdapter.DefinitionProvider));

            var updateStatement = this.DatabaseContext.RepositoryAdapter.CreateUpdateStatement(update);

            try
            {
                // Always use ToArray()!
                await this.DatabaseContext.OpenSharedConnectionAsync(cancellationToken).ConfigureAwait(false);
                await this.DatabaseContext.ExecuteAsync(updateStatement, cancellationToken, update.PropertyValues.ToArray()).ConfigureAwait(false);
            }
            catch (InvalidOperationException ex)
            {
                throw new RepositoryException(entity, ex.Message, ex);
            }
            catch (DataException ex)
            {
                throw new RepositoryException(entity, ex.Message, ex);
            }
            catch (DbException ex)
            {
                throw new RepositoryException(entity, ex.Message, ex);
            }
        }

        /// <inheritdoc />
        public int Delete(IEntitySet entitySet)
        {
            if (entitySet == null)
            {
                throw new ArgumentNullException(nameof(entitySet));
            }

            this.CheckDisposed();
            var statement = this.DatabaseContext.RepositoryAdapter.CreateDeletionStatement(entitySet);

            try
            {
                this.DatabaseContext.OpenSharedConnection();
                return this.DatabaseContext.Execute(statement, entitySet.PropertyValues.ToArray());
            }
            catch (InvalidOperationException ex)
            {
                throw new RepositoryException(entitySet, ex.Message, ex);
            }
            catch (DataException ex)
            {
                throw new RepositoryException(entitySet, ex.Message, ex);
            }
            catch (DbException ex)
            {
                throw new RepositoryException(entitySet, ex.Message, ex);
            }
        }

        /// <inheritdoc />
        public async Task<int> DeleteAsync(IEntitySet entitySet, CancellationToken cancellationToken)
        {
            if (entitySet == null)
            {
                throw new ArgumentNullException(nameof(entitySet));
            }

            this.CheckDisposed();
            var statement = this.DatabaseContext.RepositoryAdapter.CreateDeletionStatement(entitySet);

            try
            {
                await this.DatabaseContext.OpenSharedConnectionAsync(cancellationToken).ConfigureAwait(false);
                return await this.DatabaseContext.ExecuteAsync(statement, cancellationToken, entitySet.PropertyValues.ToArray())
                           .ConfigureAwait(false);
            }
            catch (InvalidOperationException ex)
            {
                throw new RepositoryException(entitySet, ex.Message, ex);
            }
            catch (DataException ex)
            {
                throw new RepositoryException(entitySet, ex.Message, ex);
            }
            catch (DbException ex)
            {
                throw new RepositoryException(entitySet, ex.Message, ex);
            }
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents the current <see cref="object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> that represents the current <see cref="object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, ToStringFormat, this.DatabaseContext?.Connection?.Database, this.InstanceIdentifier);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.IsDisposed = true;
            this.Dispose(true);
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            this.IsDisposed = true;
            await this.DisposeAsyncCore().ConfigureAwait(false);
            this.Dispose(true);
        }

        /// <summary>
        /// Creates cache key based on the specified selection.
        /// </summary>
        /// <param name="selection">
        /// The selection to create a key for.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> containing a unique key for the selection.
        /// </returns>
        private static string CreateCacheKey(IEntitySet selection)
        {
            return string.Format(CultureInfo.InvariantCulture, CacheKeyFormat, selection.EntityType.ToRuntimeName(), selection);
        }

        /// <summary>
        /// Disposes of managed resources in the current object.
        /// </summary>
        /// <param name="disposing">
        /// A value indicating whether the method has been explicitly called.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (disposing == false)
            {
                return;
            }

            this.DatabaseContext?.Dispose();
            this.OnDisposed();
        }

        /// <summary>
        /// Asynchronously disposes of resources implementing <see cref="IAsyncDisposable"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="ValueTask"/> disposing of the resources.
        /// </returns>
        private async ValueTask DisposeAsyncCore()
        {
            if (this.DatabaseContext != null)
            {
                await this.DatabaseContext.DisposeAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Gets the first or default entity matching the selection.
        /// </summary>
        /// <param name="selection">
        /// The selection that identifies the entity.
        /// </param>
        /// <typeparam name="T">
        /// The type of entity to retrieve.
        /// </typeparam>
        /// <returns>
        /// The first matching <typeparamref name="T"/>, or the default value if no entity is found.
        /// </returns>
        private T FirstOrDefaultEntity<T>(IEntitySet selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            var statement = this.DatabaseContext.RepositoryAdapter.CreateSelectionStatement(selection);

            try
            {
                this.DatabaseContext.OpenSharedConnection();
                return this.DatabaseContext.Query<T>(statement, selection.PropertyValues.ToArray()).FirstOrDefault();
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

        /// <summary>
        /// Gets the first or default entity matching the selection.
        /// </summary>
        /// <param name="selection">
        /// The selection that identifies the entity.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token for this task.
        /// </param>
        /// <typeparam name="T">
        /// The type of entity to retrieve.
        /// </typeparam>
        /// <returns>
        /// The first matching <typeparamref name="T"/>, or the default value if no entity is found.
        /// </returns>
        private async Task<T> FirstOrDefaultEntityAsync<T>(IEntitySet selection, CancellationToken cancellationToken)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            var statement = this.DatabaseContext.RepositoryAdapter.CreateSelectionStatement(selection);

            ConfiguredCancelableAsyncEnumerable<T> results;

            try
            {
                await this.DatabaseContext.OpenSharedConnectionAsync(cancellationToken).ConfigureAwait(false);

                results = this.DatabaseContext.QueryAsync<T>(statement, cancellationToken, selection.PropertyValues.ToArray())
                    .ConfigureAwait(false);
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

            T result = default;

            await foreach (var item in results)
            {
                result = item;
                break;
            }

            return result;
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
