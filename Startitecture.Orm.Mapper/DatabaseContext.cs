// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseContext.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Exposes a minimal database context to a repository provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Model;
    using Startitecture.Resources;

    /// <summary>
    /// Exposes a minimal database context to a repository provider.
    /// </summary>
    public class DatabaseContext : IDatabaseContext
    {
        /// <summary>
        /// The connection string.
        /// </summary>
        private readonly string connectionString;

        /// <summary>
        /// The is connection user provided.
        /// </summary>
        private readonly bool isConnectionUserProvided;

        /// <summary>
        /// The POCO factory.
        /// </summary>
        private readonly RaisedPocoFactory pocoFactory;

        /// <summary>
        /// The factory.
        /// </summary>
        private readonly DbProviderFactory factory;

        /// <summary>
        /// The transaction for the current context.
        /// </summary>
        private IDbTransaction transaction;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseContext"/> class.
        /// </summary>
        /// <param name="connection">
        /// The IDbConnection to use.
        /// </param>
        /// <param name="repositoryAdapter">
        /// The name qualifier for the database.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="connection"/> or <paramref name="repositoryAdapter"/> is null.
        /// </exception>
        /// <remarks>
        /// The supplied IDbConnection will not be closed/disposed - that remains the responsibility of the caller.
        /// </remarks>
        public DatabaseContext(
            [NotNull] IDbConnection connection,
            [NotNull] IRepositoryAdapter repositoryAdapter)
        {
            this.Connection = connection ?? throw new ArgumentNullException(nameof(connection));
            this.RepositoryAdapter = repositoryAdapter ?? throw new ArgumentNullException(nameof(repositoryAdapter));
            this.isConnectionUserProvided = true;
            this.connectionString = connection.ConnectionString;
            this.pocoFactory = new RaisedPocoFactory(this.RepositoryAdapter.DefinitionProvider);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseContext"/> class.
        /// Construct a database using a supplied connections string and optionally a provider name.
        /// </summary>
        /// <param name="connectionString">
        /// The database connection string.
        /// </param>
        /// <param name="providerName">
        /// The name of the DB provider to use.
        /// </param>
        /// <param name="repositoryAdapter">
        /// The name qualifier for the database.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="connectionString"/> or <paramref name="providerName"/> is null or whitespace.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="repositoryAdapter"/> is null.
        /// </exception>
        /// <remarks>
        /// This class will automatically close and dispose any connections it creates.
        /// </remarks>
        public DatabaseContext(
            [NotNull] string connectionString,
            [NotNull] string providerName,
            [NotNull] IRepositoryAdapter repositoryAdapter)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(connectionString));
            }

            if (string.IsNullOrWhiteSpace(providerName))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(providerName));
            }

            this.RepositoryAdapter = repositoryAdapter ?? throw new ArgumentNullException(nameof(repositoryAdapter));
            this.connectionString = connectionString;
            this.factory = DbProviderFactories.GetFactory(providerName);
            this.pocoFactory = new RaisedPocoFactory(this.RepositoryAdapter.DefinitionProvider);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseContext"/> class.
        /// Construct a Database using a supplied connection string and a DbProviderFactory.
        /// </summary>
        /// <param name="connectionString">
        /// The database connection string to use.
        /// </param>
        /// <param name="providerFactory">
        /// The DbProviderFactory to use for instantiating IDbConnections.
        /// </param>
        /// <param name="repositoryAdapter">
        /// The entity definition provider.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="connectionString"/> is null or whitespace.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="providerFactory"/> or <paramref name="repositoryAdapter"/> is null.
        /// </exception>
        public DatabaseContext(
            [NotNull] string connectionString,
            [NotNull] DbProviderFactory providerFactory,
            [NotNull] IRepositoryAdapter repositoryAdapter)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(connectionString));
            }

            this.factory = providerFactory ?? throw new ArgumentNullException(nameof(providerFactory));
            this.RepositoryAdapter = repositoryAdapter ?? throw new ArgumentNullException(nameof(repositoryAdapter));
            this.connectionString = connectionString;
            this.pocoFactory = new RaisedPocoFactory(this.RepositoryAdapter.DefinitionProvider);
        }

        /// <inheritdoc />
        public int CommandTimeout { get; set; }

        /// <inheritdoc />
        public IDbConnection Connection { get; private set; }

        /// <inheritdoc />
        public int OnetimeCommandTimeout { get; set; }

        /// <inheritdoc />
        public IRepositoryAdapter RepositoryAdapter { get; }

        /// <inheritdoc />
        public void OpenSharedConnection()
        {
            if (this.Connection == null)
            {
                this.Connection = this.factory.CreateConnection();

                if (this.Connection == null)
                {
                    throw new InvalidOperationException(
                        string.Format(CultureInfo.CurrentCulture, ErrorMessages.ConnectionFactoryReturnedNullConnection, this.factory));
                }

                this.Connection.ConnectionString = this.connectionString;
            }

            if (this.Connection.State == ConnectionState.Broken)
            {
                this.Connection.Close();
            }

            if (this.Connection.State == ConnectionState.Closed)
            {
                this.Connection.Open();
            }
        }

        /// <param name="cancellationToken"></param>
        /// <inheritdoc />
        public async Task OpenSharedConnectionAsync(CancellationToken cancellationToken)
        {
            if (this.Connection is DbConnection asyncConnection)
            {
                if (this.Connection == null)
                {
                    this.Connection = this.factory.CreateConnection();

                    if (this.Connection == null)
                    {
                        throw new InvalidOperationException(
                            string.Format(CultureInfo.CurrentCulture, ErrorMessages.ConnectionFactoryReturnedNullConnection, this.factory));
                    }

                    this.Connection.ConnectionString = this.connectionString;
                }

                if (this.Connection.State == ConnectionState.Broken)
                {
#if NET472
                    asyncConnection.Close();
#else
                    await asyncConnection.CloseAsync().ConfigureAwait(false);
#endif
                }

                if (this.Connection.State == ConnectionState.Closed)
                {
                    await asyncConnection.OpenAsync(cancellationToken).ConfigureAwait(false);
                }
            }
            else
            {
                this.OpenSharedConnection();
            }
        }

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
            try
            {
                this.OpenSharedConnection();
                this.transaction = this.Connection.BeginTransaction(isolationLevel);

                // Return the transaction context, then on disposal set our copy to null.
                var transactionContext = new TransactionContext(this.transaction);
                transactionContext.Disposed += (_, _) => this.transaction = null;
                return transactionContext;
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
            try
            {
                await this.OpenSharedConnectionAsync(cancellationToken).ConfigureAwait(false);

                if (this.Connection is DbConnection asyncConnection)
                {
#if NET472
                    this.transaction = asyncConnection.BeginTransaction(isolationLevel);
#else
                    this.transaction = await asyncConnection.BeginTransactionAsync(isolationLevel, cancellationToken).ConfigureAwait(false);
#endif
                    var transactionContext = new TransactionContext(this.transaction);
                    transactionContext.Disposed += (_, _) => this.transaction = null;
                    return transactionContext;
                }

                var message = $"The underlying connection is not of the type {nameof(DbConnection)}; use {nameof(this.BeginTransaction)} instead.";
                throw new OperationException(this, message);
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

        /// <summary>
        /// Associates the <paramref name="command"/> with the current context's <see cref="IDbTransaction"/>, if any.
        /// </summary>
        /// <param name="command">
        /// The command to associate.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="command"/> is null.
        /// </exception>
        public void AssociateTransaction(IDbCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            command.Transaction = this.transaction;
        }

        /// <inheritdoc />
        public void ChangeDatabase([NotNull] string databaseName)
        {
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(databaseName));
            }

            this.OpenSharedConnection();
            this.Connection.ChangeDatabase(databaseName);
        }

        /// <inheritdoc />
        public async Task ChangeDatabaseAsync([NotNull] string databaseName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(databaseName));
            }

            await this.OpenSharedConnectionAsync(cancellationToken).ConfigureAwait(false);

            if (this.Connection is DbConnection asyncConnection)
            {
#if NET472
                this.Connection.ChangeDatabase(databaseName);
#else
                await asyncConnection.ChangeDatabaseAsync(databaseName, cancellationToken).ConfigureAwait(false);
#endif
            }
            else
            {
                this.Connection.ChangeDatabase(databaseName);
            }
        }

        /// <inheritdoc />
        public int Execute([NotNull] string sql, [NotNull] params object[] args)
        {
            if (string.IsNullOrWhiteSpace(sql))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(sql));
            }

            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            this.OpenSharedConnection();

            using (var command = this.CreateCommand(this.Connection, sql, args))
            {
                var returnValue = command.ExecuteNonQuery();
                return returnValue;
            }
        }

        /// <inheritdoc />
        public async Task<int> ExecuteAsync(string sql, CancellationToken cancellationToken, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(sql))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(sql));
            }

            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            await this.OpenSharedConnectionAsync(cancellationToken).ConfigureAwait(false);

            if (this.Connection is DbConnection asyncConnection)
            {
#if NET472
                using (var command = this.CreateAsyncCommand(asyncConnection, sql, args))
#else
                await using (var command = this.CreateAsyncCommand(asyncConnection, sql, args))
#endif
                {
                    var returnValue = await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                    return returnValue;
                }
            }

            using (var command = this.CreateCommand(this.Connection, sql, args))
            {
                // ReSharper disable once AccessToDisposedClosure
                var returnValue = command.ExecuteNonQuery();
                return returnValue;
            }
        }

        /// <inheritdoc />
        public T ExecuteScalar<T>([NotNull] string sql, [NotNull] params object[] args)
        {
            if (string.IsNullOrWhiteSpace(sql))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(sql));
            }

            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            this.OpenSharedConnection();

            using (var command = this.CreateCommand(this.Connection, sql, args))
            {
                var result = command.ExecuteScalar();
                return ConvertNullable<T>(result);
            }
        }

        /// <inheritdoc />
        public async Task<T> ExecuteScalarAsync<T>(string sql, CancellationToken cancellationToken, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(sql))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(sql));
            }

            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            await this.OpenSharedConnectionAsync(cancellationToken).ConfigureAwait(false);

            if (this.Connection is DbConnection asyncConnection)
            {
#if NET472
                using (var command = this.CreateAsyncCommand(asyncConnection, sql, args))
#else
                await using (var command = this.CreateAsyncCommand(asyncConnection, sql, args))
#endif
                {
                    var result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
                    return ConvertNullable<T>(result);
                }
            }

            using (var command = this.CreateCommand(this.Connection, sql, args))
            {
                // ReSharper disable once AccessToDisposedClosure
                var result = command.ExecuteScalar();
                return ConvertNullable<T>(result);
            }
        }

        /// <inheritdoc />
        public IEnumerable<T> Query<T>([NotNull] string sql, [NotNull] params object[] args)
        {
            if (sql == null)
            {
                throw new ArgumentNullException(nameof(sql));
            }

            if (string.IsNullOrWhiteSpace(sql))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(sql));
            }

            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            this.OpenSharedConnection();

            var entityDefinition = this.RepositoryAdapter.DefinitionProvider.Resolve<T>();

            using (var cmd = this.CreateCommand(this.Connection, sql, args))
            {
                foreach (var poco in this.ReadResults<T>(cmd, entityDefinition))
                {
                    yield return poco;
                }
            }
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<T> QueryAsync<T>(string sql, [EnumeratorCancellation] CancellationToken cancellationToken, params object[] args)
        {
            if (sql == null)
            {
                throw new ArgumentNullException(nameof(sql));
            }

            if (string.IsNullOrWhiteSpace(sql))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(sql));
            }

            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            await this.OpenSharedConnectionAsync(cancellationToken).ConfigureAwait(false);
            var entityDefinition = this.RepositoryAdapter.DefinitionProvider.Resolve<T>();

            if (this.Connection is DbConnection asyncConnection)
            {
#if NET472
                using (var command = this.CreateAsyncCommand(asyncConnection, sql, args))
#else
                await using (var command = this.CreateAsyncCommand(asyncConnection, sql, args))
#endif
                {
                    ////var results = new List<T>();
#if NET472
                    using (var dataReader = await command.ExecuteReaderAsync().ConfigureAwait(false))
#else
                    await using (var dataReader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
#endif
                    {
                        while (true)
                        {
                            if (await dataReader.ReadAsync(cancellationToken).ConfigureAwait(false) == false)
                            {
                                yield break;
                            }

                            var pocoDataRequest = new PocoDataRequest(dataReader, entityDefinition, this)
                                                  {
                                                      FirstColumn = 0
                                                  };

                            var poco = this.pocoFactory.CreatePoco<T>(pocoDataRequest);
                            yield return poco;
                        }
                    }

                    ////return results;
                }
            }
            else
            {
                using (var command = this.CreateCommand(this.Connection, sql, args))
                {
                    ////var results = new List<T>();
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (true)
                        {
                            if (dataReader.Read() == false)
                            {
                                yield break;
                            }

                            var pocoDataRequest = new PocoDataRequest(dataReader, entityDefinition, this)
                                                  {
                                                      FirstColumn = 0
                                                  };

                            var poco = this.pocoFactory.CreatePoco<T>(pocoDataRequest);
                            yield return poco;
                        }
                    }

                    ////return results;
                }
            }
        }

        /// <inheritdoc />
        public IValueMapper GetValueMapper([NotNull] Type sourceType, [NotNull] Type destinationType)
        {
            if (sourceType == null)
            {
                throw new ArgumentNullException(nameof(sourceType));
            }

            if (destinationType == null)
            {
                throw new ArgumentNullException(nameof(destinationType));
            }

            return this.RepositoryAdapter.ValueMappers.TryGetValue(new Tuple<Type, Type>(sourceType, destinationType), out var valueMapper)
                       ? valueMapper
                       : null;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            await this.DisposeAsyncCore().ConfigureAwait(false);
            this.Dispose(true);
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
            GC.SuppressFinalize(this);
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">
        /// A value indicating whether the current instance is being explicitly disposed.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing == false)
            {
                return;
            }

            // Do not dispose of user provided connections.
            if (this.Connection != null && this.isConnectionUserProvided == false)
            {
                this.Connection.Dispose();
            }

            this.transaction?.Dispose();
            this.transaction = null;
            this.pocoFactory?.Dispose();
        }

        /// <summary>
        /// Disposes of objects that implement <see cref="IAsyncDisposable"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="ValueTask"/> for the disposal operation.
        /// </returns>
        protected virtual async ValueTask DisposeAsyncCore()
        {
#if NET472
            if (this.transaction is IAsyncDisposable asyncTransaction)
            {
                await asyncTransaction.DisposeAsync().ConfigureAwait(false);
            }
#else
            if (this.transaction is DbTransaction asyncTransaction)
            {
                await asyncTransaction.DisposeAsync().ConfigureAwait(false);
            }
#endif

            this.transaction = null;
            this.pocoFactory?.Dispose();

            if (this.isConnectionUserProvided)
            {
                return;
            }

#if NET472
            if (this.Connection is IAsyncDisposable asyncConnection)
            {
                await asyncConnection.DisposeAsync().ConfigureAwait(false);
            }
#else
            if (this.Connection is DbConnection asyncConnection)
            {
                await asyncConnection.DisposeAsync().ConfigureAwait(false);
            }
#endif

            this.Connection = null;
        }

        /// <summary>
        /// Converts a nullable into the default value for the base type.
        /// </summary>
        /// <param name="value">
        /// The value to convert.
        /// </param>
        /// <typeparam name="T">
        /// The type of the value to convert.
        /// </typeparam>
        /// <returns>
        /// The value as a type of <typeparamref name="T"/>.
        /// </returns>
        private static T ConvertNullable<T>(object value)
        {
            // Handle nullable types
            var underlyingType = Nullable.GetUnderlyingType(typeof(T));

            if (underlyingType != null && value == null)
            {
                return default;
            }

            return (T)Convert.ChangeType(value, underlyingType ?? typeof(T), CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Creates a command using the specified connection.
        /// </summary>
        /// <param name="connection">
        /// The connection to use.
        /// </param>
        /// <param name="sql">
        /// The SQL statement.
        /// </param>
        /// <param name="parameters">
        /// The parameters to pass to the statement.
        /// </param>
        /// <returns>
        /// An <see cref="IDbCommand"/> for the specified command text and parameters.
        /// </returns>
        private IDbCommand CreateCommand(IDbConnection connection, string sql, params object[] parameters)
        {
            // Create the command and add parameters
            var command = connection.CreateCommand();
            command.Connection = connection;
            command.CommandText = sql;
            command.Transaction = this.transaction;

            foreach (var item in parameters)
            {
                this.AddParam(command, item);
            }

            return command;
        }

        /// <summary>
        /// Creates a command using the specified connection.
        /// </summary>
        /// <param name="connection">
        /// The connection to use.
        /// </param>
        /// <param name="sql">
        /// The SQL statement.
        /// </param>
        /// <param name="parameters">
        /// The parameters to pass to the statement.
        /// </param>
        /// <returns>
        /// An <see cref="IDbCommand"/> for the specified command text and parameters.
        /// </returns>
        private DbCommand CreateAsyncCommand(DbConnection connection, string sql, params object[] parameters)
        {
            // Create the command and add parameters
            var command = connection.CreateCommand();
            command.Connection = connection;
#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
            command.CommandText = sql;
#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities
            command.Transaction = this.transaction as DbTransaction;

            foreach (var item in parameters)
            {
                this.AddParam(command, item);
            }

            return command;
        }

        /// <summary>
        /// Add a parameter to a DB command.
        /// </summary>
        /// <param name="command">
        /// A reference to the IDbCommand to which the parameter is to be added.
        /// </param>
        /// <param name="value">
        /// The value to assign to the parameter.
        /// </param>
        private void AddParam(IDbCommand command, object value)
        {
            // Support passed in parameters
            var parameterName = command.Parameters.Count.ToString(CultureInfo.InvariantCulture);

            if (value is IDbDataParameter dataParameter)
            {
                dataParameter.ParameterName = parameterName;
                command.Parameters.Add(dataParameter);
                return;
            }

            // Create the parameter
            var parameter = this.RepositoryAdapter.CreateParameter(command, parameterName, value);

            // Add to the collection
            command.Parameters.Add(parameter);
        }

        /// <summary>
        /// Reads the results of a command.
        /// </summary>
        /// <param name="command">
        /// The command to execute as a reader.
        /// </param>
        /// <param name="entityDefinition">
        /// The entity definition.
        /// </param>
        /// <typeparam name="T">
        /// The type of results expected.
        /// </typeparam>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <typeparamref name="T"/> items.
        /// </returns>
        private IEnumerable<T> ReadResults<T>(IDbCommand command, IEntityDefinition entityDefinition)
        {
            using (var dataReader = command.ExecuteReader())
            {
                while (true)
                {
                    if (!dataReader.Read())
                    {
                        yield break;
                    }

                    var pocoDataRequest = new PocoDataRequest(dataReader, entityDefinition, this)
                                              {
                                                  FirstColumn = 0
                                              };

                    var poco = this.pocoFactory.CreatePoco<T>(pocoDataRequest);
                    yield return poco;
                }
            }
        }
    }
}