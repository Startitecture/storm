// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseRepositoryProvider.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
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

        /// <summary>
        /// The entity cache.
        /// </summary>
        private readonly ObjectCache entityCache;

        /// <summary>
        /// The entity lock.
        /// </summary>
        private readonly object cacheLock = new object();

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
                this.EntityDefinitionProvider = this.DatabaseContext.DefinitionProvider;
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
            this.CheckDisposed();

            try
            {
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

        /// <inheritdoc />
        public bool Contains<T>(EntitySet<T> selection)
            where T : ITransactionContext
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            this.CheckDisposed();

            var sql = this.DatabaseContext.StatementCompiler.CreateExistsStatement(selection);

            try
            {
                // Always remember to supply this method with an array of values!
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
        public T GetScalar<T>([NotNull] ISelection selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            this.CheckDisposed();
            var statement = this.DatabaseContext.StatementCompiler.CreateSelectionStatement(selection);

            try
            {
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
        public T FirstOrDefault<T>(EntitySelection<T> selection)
            where T : ITransactionContext
        {
            if (Evaluate.IsNull(selection))
            {
                throw new ArgumentNullException(nameof(selection));
            }

            this.CheckDisposed();
            T entity;

            if (this.EnableCaching)
            {
                var cacheKey = CreateCacheKey(selection);

                lock (this.cacheLock)
                {
                    entity = this.entityCache.GetOrLazyAddExisting(
                        this.cacheLock,
                        cacheKey,
                        selection,
                        this.FirstOrDefaultEntity<T>,
                        new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(2) });
                }
            }
            else
            {
                entity = this.FirstOrDefaultEntity<T>(selection);
            }

            if (entity != null)
            {
                entity.SetTransactionProvider(this);
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
        public IEnumerable<T> SelectEntities<T>(EntitySelection<T> selection)
            where T : ITransactionContext
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            this.CheckDisposed();
            var statement = this.DatabaseContext.StatementCompiler.CreateSelectionStatement(selection);

            try
            {
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
        public IEnumerable<dynamic> DynamicSelect([NotNull] ISelection selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            this.CheckDisposed();
            var statement = this.DatabaseContext.StatementCompiler.CreateSelectionStatement(selection);

            try
            {
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
        public T Insert<T>(T entity)
            where T : ITransactionContext
        {
            if (Evaluate.IsNull(entity))
            {
                throw new ArgumentNullException(nameof(entity));
            }

            this.CheckDisposed();
            var statement = this.DatabaseContext.StatementCompiler.CreateInsertionStatement<T>();
            var entityDefinition = this.EntityDefinitionProvider.Resolve<T>();
            var values = entityDefinition.InsertableAttributes.Select(definition => definition.GetValueMethod.Invoke(entity, null)).ToArray();

            try
            {
                if (entityDefinition.AutoNumberPrimaryKey.HasValue)
                {
                    var autoNumber = this.DatabaseContext.ExecuteScalar<object>(statement, values);
                    entityDefinition.AutoNumberPrimaryKey.Value.SetValueDelegate.DynamicInvoke(entity, autoNumber);
                }
                else
                {
                    this.DatabaseContext.Execute(statement, values);
                }
                ////result = this.DatabaseContext.Insert(entity);
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

            ////if (result == null)
            ////{
            ////    string message = string.Format(CultureInfo.CurrentCulture, $"Failed to insert ({typeof(T).Name}) {entity}");
            ////    throw new RepositoryException(entity, message);
            ////}

            entity.SetTransactionProvider(this);
            return entity;
        }

        /// <inheritdoc />
        public int Update<T>([NotNull] UpdateSet<T> updateSet)
            where T : ITransactionContext
        {
            if (updateSet == null)
            {
                throw new ArgumentNullException(nameof(updateSet));
            }

            var updateStatement = this.DatabaseContext.StatementCompiler.CreateUpdateStatement(updateSet);

            try
            {
                // Always use ToArray()!
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
        public void UpdateSingle<T>(T entity, params Expression<Func<T, object>>[] setExpressions)
            where T : ITransactionContext
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
            var selection = Select.Where<T>().MatchKey(entity, this.DatabaseContext.DefinitionProvider);
            var update = setExpressions.Any()
                             ? new UpdateSet<T>().Set(entity, setExpressions).Where(selection)
                             : new UpdateSet<T>().Set(entity, this.EntityDefinitionProvider).Where(selection);

            var updateStatement = this.DatabaseContext.StatementCompiler.CreateUpdateStatement(update);

            try
            {
                // Always use ToArray()!
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
        public int Delete<T>(EntitySelection<T> selection) 
            where T : ITransactionContext
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            this.CheckDisposed();
            var statement = this.DatabaseContext.StatementCompiler.CreateDeletionStatement(selection);

            try
            {
                return this.DatabaseContext.Execute(statement, selection.PropertyValues.ToArray());
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
        /// Returns a <see cref="string"/> that represents the current <see cref="object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> that represents the current <see cref="object"/>.
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
        /// <returns>
        /// A <see cref="string"/> containing a unique key for the selection.
        /// </returns>
        private static string CreateCacheKey(IEntitySet selection)
        {
            return string.Format(CultureInfo.InvariantCulture, CacheKeyFormat, selection.EntityType.ToRuntimeName(), selection);
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
        private T FirstOrDefaultEntity<T>(ISelection selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            var statement = this.DatabaseContext.StatementCompiler.CreateSelectionStatement(selection);

            try
            {
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
