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

    using JetBrains.Annotations;

    using Startitecture.Orm.Common;
    using Startitecture.Resources;

    /// <summary>
    /// Exposes a minimal database context to a repository provider.
    /// </summary>
    public class DatabaseContext : IDatabaseContext
    {
        #region Fields

        /// <summary>
        /// The connection string.
        /// </summary>
        private readonly string connectionString;

        /// <summary>
        /// The provider name.
        /// </summary>
        private readonly string providerName;

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
        private DbProviderFactory factory;

        /// <summary>
        /// The transaction.
        /// </summary>
        private IDbTransaction transaction;

        /// <summary>
        /// The transaction depth.
        /// </summary>
        private int transactionDepth;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseContext"/> class. 
        /// Construct a database using a supplied IDbConnection
        /// </summary>
        /// <param name="connection">
        /// The IDbConnection to use
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
            this.CommonConstruct();

            this.pocoFactory = new RaisedPocoFactory(this.RepositoryAdapter.DefinitionProvider);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseContext"/> class. 
        /// Construct a database using a supplied connections string and optionally a provider name
        /// </summary>
        /// <param name="connectionString">
        /// The DB connection string
        /// </param>
        /// <param name="providerName">
        /// The name of the DB provider to use
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
            this.providerName = providerName;
            this.CommonConstruct();

            this.pocoFactory = new RaisedPocoFactory(this.RepositoryAdapter.DefinitionProvider);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseContext"/> class. 
        /// Construct a Database using a supplied connection string and a DbProviderFactory
        /// </summary>
        /// <param name="connectionString">
        /// The connection string to use
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
            this.CommonConstruct();

            this.pocoFactory = new RaisedPocoFactory(this.RepositoryAdapter.DefinitionProvider);
        }

        #endregion

        #region Public Properties

        /// <inheritdoc />
        public int CommandTimeout { get; set; }

        /// <inheritdoc />
        public IDbConnection Connection { get; private set; }

        /// <inheritdoc />
        public int OnetimeCommandTimeout { get; set; }

        /// <inheritdoc />
        public IRepositoryAdapter RepositoryAdapter { get; }

        #endregion

        #region Public Methods and Operators

        #region Connections and Transactions

        /// <summary>
        /// Open a connection that will be used for all subsequent queries.
        /// </summary>
        /// <remarks>
        /// Calls to Open/CloseSharedConnection are reference counted and should be balanced
        /// </remarks>
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

        /// <summary>
        /// Starts a transaction scope, see GetTransaction() for recommended usage
        /// </summary>
        /// <returns>
        /// A new <see cref="IDbTransaction"/>, or the current transaction if a transaction has already been started.
        /// </returns>
        public IDbTransaction BeginTransaction()
        {
            this.transactionDepth++;

            if (this.transactionDepth == 1)
            {
                this.OpenSharedConnection();
                this.transaction = this.Connection.BeginTransaction();
            }

            return this.transaction;
        }

        #endregion

        #region Execution

        /// <summary>
        /// Executes a non-query command
        /// </summary>
        /// <param name="sql">
        /// The SQL statement to execute
        /// </param>
        /// <param name="args">
        /// Arguments to any embedded parameters in the SQL
        /// </param>
        /// <returns>
        /// The number of rows affected
        /// </returns>
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

            using (var cmd = this.CreateCommand(this.Connection, sql, args))
            {
                var returnValue = cmd.ExecuteNonQuery();
                return returnValue;
            }
        }

        /// <summary>
        /// Executes a query and return the first column of the first row in the result set.
        /// </summary>
        /// <typeparam name="T">
        /// The type that the result value should be cast to
        /// </typeparam>
        /// <param name="sql">
        /// The SQL query to execute
        /// </param>
        /// <param name="args">
        /// Arguments to any embedded parameters in the SQL
        /// </param>
        /// <returns>
        /// The scalar value cast to T
        /// </returns>
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

                // Handle nullable types
                var underlyingType = Nullable.GetUnderlyingType(typeof(T));

                if (underlyingType != null && result == null)
                {
                    return default;
                }

                return (T)Convert.ChangeType(result, underlyingType ?? typeof(T), CultureInfo.CurrentCulture);
            }
        }

        #endregion

        #region Query Operations

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
            using (var dataReader = cmd.ExecuteReader())
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

        #endregion

        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Methods

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

            this.pocoFactory?.Dispose();
            this.transaction?.Dispose();
        }

        /// <summary>
        /// Add a parameter to a DB command
        /// </summary>
        /// <param name="command">
        /// A reference to the IDbCommand to which the parameter is to be added
        /// </param>
        /// <param name="value">
        /// The value to assign to the parameter
        /// </param>
        private void AddParam(IDbCommand command, object value)
        {
            // Support passed in parameters
            var parameterName = command.Parameters.Count.ToString();

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
        /// The <see cref="IDbCommand"/>.
        /// </returns>
        private IDbCommand CreateCommand(IDbConnection connection, string sql, params object[] parameters)
        {
            ////// Perform parameter prefix replacements
            ////if (this.paramPrefix != "@")
            ////{
            ////    sql = ParameterPrefixRegex.Replace(sql, m => this.paramPrefix + m.Value.Substring(1));
            ////}

////#if NETSTANDARD
////            sql = sql.Replace("@@", "@", StringComparison.Ordinal); // <- double @@ escapes a single @
////#else
////            sql = sql.Replace("@@", "@"); // <- double @@ escapes a single @
////#endif

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
        /// Provides common initialization for the various constructors
        /// </summary>
        private void CommonConstruct()
        {
            // Reset
            this.transactionDepth = 0;
            ////this.EnableNamedParameters = true;

            // If a provider name was supplied, get the IDbProviderFactory for it
            if (this.providerName != null)
            {
                this.factory = DbProviderFactories.GetFactory(this.providerName);
            }

            // Resolve the DB Type
            ////var typeName = (this.factory?.GetType() ?? this.Connection.GetType()).Name;
            ////this.databaseType = DatabaseType.Resolve(typeName, this.providerName);

            // What character is used for delimiting parameters in SQL
            ////this.paramPrefix = this.databaseType.GetParameterPrefix(this.connectionString);
        }

        #endregion
    }
}