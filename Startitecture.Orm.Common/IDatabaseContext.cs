// <copyright file="IDatabaseContext.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>

namespace Startitecture.Orm.Common
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    /// <summary>
    /// Provides an interface for classes that contain database contexts.
    /// </summary>
    public interface IDatabaseContext : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// Gets or sets the timeout value for all SQL statements.
        /// </summary>
        int CommandTimeout { get; set; }

        /// <summary>
        /// Gets the currently open shared connection (or null if none).
        /// </summary>
        IDbConnection Connection { get; }

        /// <summary>
        /// Gets or sets the timeout value for the next (and only next) SQL statement.
        /// </summary>
        int OnetimeCommandTimeout { get; set; }

        /// <summary>
        /// Gets the statement compiler for the database context.
        /// </summary>
        IRepositoryAdapter RepositoryAdapter { get; }

        /// <summary>
        /// Opens a connection that will be used for all subsequent queries.
        /// </summary>
        void OpenSharedConnection();

        /// <summary>
        /// Opens a connection that will be used for all subsequent queries.
        /// </summary>
        /// <returns>
        /// The <see cref="Task" /> that is opening the connection.
        /// </returns>
        Task OpenSharedConnectionAsync();

        /// <summary>
        /// Opens a connection that will be used for all subsequent queries.
        /// </summary>
        /// <param name="cancellationToken">
        /// The cancellation token for the task.
        /// </param>
        /// <returns>
        /// The <see cref="Task" /> that is opening the connection.
        /// </returns>
        Task OpenSharedConnectionAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Starts a transaction in the repository.
        /// </summary>
        /// <returns>
        /// The <see cref="IDbTransaction" /> started by the provider.
        /// </returns>
        ITransactionContext BeginTransaction();

        /// <summary>
        /// Starts a transaction in the repository.
        /// </summary>
        /// <returns>
        /// The <see cref="IDbTransaction" /> started by the provider.
        /// </returns>
        Task<ITransactionContext> BeginTransactionAsync();

        /// <summary>
        /// Starts a transaction in the repository.
        /// </summary>
        /// <param name="cancellationToken">
        /// The cancellation token for this task.
        /// </param>
        /// <returns>
        /// The <see cref="IDbTransaction" /> started by the provider.
        /// </returns>
        Task<ITransactionContext> BeginTransactionAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Start a transaction in the repository.
        /// </summary>
        /// <param name="isolationLevel">
        /// The isolation level for the transaction.
        /// </param>
        /// <returns>
        /// The <see cref="IDbTransaction" /> started by the provider.
        /// </returns>
        ITransactionContext BeginTransaction(IsolationLevel isolationLevel);

        /// <summary>
        /// Start a transaction in the repository.
        /// </summary>
        /// <param name="isolationLevel">
        /// The isolation level for the transaction.
        /// </param>
        /// <returns>
        /// The <see cref="IDbTransaction" /> started by the provider.
        /// </returns>
        Task<ITransactionContext> BeginTransactionAsync(IsolationLevel isolationLevel);

        /// <summary>
        /// Start a transaction in the repository.
        /// </summary>
        /// <param name="isolationLevel">
        /// The isolation level for the transaction.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token for this task.
        /// </param>
        /// <returns>
        /// The <see cref="IDbTransaction" /> started by the provider.
        /// </returns>
        Task<ITransactionContext> BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken);

        /// <summary>
        /// Changes the database to the specified <paramref name="databaseName"/>.
        /// </summary>
        /// <param name="databaseName">
        /// The name of the database to change the connection to.
        /// </param>
        void ChangeDatabase(string databaseName);

        /// <summary>
        /// Changes the database to the specified <paramref name="databaseName"/>.
        /// </summary>
        /// <param name="databaseName">
        /// The name of the database to change the connection to.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the result of the asynchronous operation.
        /// </returns>
        Task ChangeDatabaseAsync(string databaseName);

        /// <summary>
        /// Changes the database to the specified <paramref name="databaseName"/>.
        /// </summary>
        /// <param name="databaseName">
        /// The name of the database to change the connection to.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token for this task.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the result of the asynchronous operation.
        /// </returns>
        Task ChangeDatabaseAsync(string databaseName, CancellationToken cancellationToken);

        /// <summary>
        /// Executes a non-query command.
        /// </summary>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="args">
        /// Arguments to any embedded parameters in the SQL.
        /// </param>
        /// <returns>
        /// The number of rows affected.
        /// </returns>
        int Execute(string sql, params object[] args);

        /// <summary>
        /// Executes a non-query command.
        /// </summary>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="args">
        /// Arguments to any embedded parameters in the SQL.
        /// </param>
        /// <returns>
        /// The number of rows affected.
        /// </returns>
        Task<int> ExecuteAsync(string sql, params object[] args);

        /// <summary>
        /// Executes a non-query command.
        /// </summary>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token for this task.
        /// </param>
        /// <param name="args">
        /// Arguments to any embedded parameters in the SQL.
        /// </param>
        /// <returns>
        /// The number of rows affected.
        /// </returns>
        Task<int> ExecuteAsync(string sql, CancellationToken cancellationToken, params object[] args);

        /// <summary>
        /// Executes a query and return the first column of the first row in the result set.
        /// </summary>
        /// <typeparam name="T">
        /// The type that the result value should be cast to.
        /// </typeparam>
        /// <param name="sql">
        /// The SQL query to execute.
        /// </param>
        /// <param name="args">
        /// Arguments to any embedded parameters in the SQL.
        /// </param>
        /// <returns>
        /// The scalar value cast to <typeparamref name="T" />.
        /// </returns>
        T ExecuteScalar<T>(string sql, params object[] args);

        /// <summary>
        /// Executes a query and return the first column of the first row in the result set.
        /// </summary>
        /// <typeparam name="T">
        /// The type that the result value should be cast to.
        /// </typeparam>
        /// <param name="sql">
        /// The SQL query to execute.
        /// </param>
        /// <param name="args">
        /// Arguments to any embedded parameters in the SQL.
        /// </param>
        /// <returns>
        /// The scalar value cast to <typeparamref name="T" />.
        /// </returns>
        Task<T> ExecuteScalarAsync<T>(string sql, params object[] args);

        /// <summary>
        /// Executes a query and return the first column of the first row in the result set.
        /// </summary>
        /// <typeparam name="T">
        /// The type that the result value should be cast to.
        /// </typeparam>
        /// <param name="sql">
        /// The SQL query to execute.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token for this task.
        /// </param>
        /// <param name="args">
        /// Arguments to any embedded parameters in the SQL.
        /// </param>
        /// <returns>
        /// The scalar value cast to <typeparamref name="T" />.
        /// </returns>
        Task<T> ExecuteScalarAsync<T>(string sql, CancellationToken cancellationToken, params object[] args);

        /// <summary>
        /// Runs an SQL query, returning the results as an IEnumerable collection.
        /// </summary>
        /// <typeparam name="T">
        /// The Type representing a row in the result set.
        /// </typeparam>
        /// <param name="sql">
        /// The SQL query.
        /// </param>
        /// <param name="args">
        /// Arguments to any embedded parameters in the SQL statement.
        /// </param>
        /// <returns>
        /// An enumerable collection of result records.
        /// </returns>
        /// <remarks>
        /// For some DB providers, care should be taken to not start a new Query before finishing with and disposing the previous one.
        /// In cases where this is an issue, consider using Fetch which returns the results as a List rather than an IEnumerable.
        /// </remarks>
        IEnumerable<T> Query<T>(string sql, params object[] args);

        /// <summary>
        /// Runs an SQL query, returning the results as an IEnumerable collection.
        /// </summary>
        /// <typeparam name="T">
        /// The Type representing a row in the result set.
        /// </typeparam>
        /// <param name="sql">
        /// The SQL query.
        /// </param>
        /// <param name="args">
        /// Arguments to any embedded parameters in the SQL statement.
        /// </param>
        /// <returns>
        /// An enumerable collection of result records.
        /// </returns>
        /// <remarks>
        /// For some DB providers, care should be taken to not start a new Query before finishing with and disposing the previous one.
        /// In cases where this is an issue, consider using Fetch which returns the results as a List rather than an IEnumerable.
        /// </remarks>
        IAsyncEnumerable<T> QueryAsync<T>(string sql, params object[] args);

        /// <summary>
        /// Runs an SQL query, returning the results as an IEnumerable collection.
        /// </summary>
        /// <typeparam name="T">
        /// The Type representing a row in the result set.
        /// </typeparam>
        /// <param name="sql">
        /// The SQL query.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token for this task.
        /// </param>
        /// <param name="args">
        /// Arguments to any embedded parameters in the SQL statement.
        /// </param>
        /// <returns>
        /// An enumerable collection of result records.
        /// </returns>
        /// <remarks>
        /// For some DB providers, care should be taken to not start a new Query before finishing with and disposing the previous one.
        /// In cases where this is an issue, consider using Fetch which returns the results as a List rather than an IEnumerable.
        /// </remarks>
        IAsyncEnumerable<T> QueryAsync<T>(string sql, CancellationToken cancellationToken, params object[] args);

        /// <summary>
        /// Gets a value mapper for the specified types.
        /// </summary>
        /// <param name="sourceType">
        /// The source type.
        /// </param>
        /// <param name="destinationType">
        /// The destination type.
        /// </param>
        /// <returns>
        /// An <see cref="IValueMapper" /> for the specified conversion, or <c>null</c> if no mapper has been registered.
        /// </returns>
        IValueMapper GetValueMapper(Type sourceType, Type destinationType);

        /// <summary>
        /// Associates the <paramref name="command"/> with the current context's <see cref="IDbTransaction"/>, if any.
        /// </summary>
        /// <param name="command">
        /// The command to associate.
        /// </param>
        void AssociateTransaction([NotNull] IDbCommand command);
    }
}