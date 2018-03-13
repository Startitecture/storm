// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Database.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The main PetaPoco Database class.  You can either use this class directly, or derive from it.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Startitecture.Orm.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Mapper.DatabaseTypes;
    using Startitecture.Orm.Mapper.Internal;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Schema;
    using Startitecture.Orm.Sql;
    using Startitecture.Resources;

    /// <summary>
    /// The main Database class. You can either use this class directly, or derive from it.
    /// </summary>
    public class Database : IDatabaseContext
    {
        #region Static Fields

        /// <summary>
        /// The Regex for parameter prefixes.
        /// </summary>
        private static readonly Regex ParameterPrefixRegex = new Regex(@"(?<!@)@\w+", RegexOptions.Compiled);

/*
        /// <summary>
        /// The poco factories.
        /// </summary>
        private static readonly Cache<Tuple<string, string, int, int>, Delegate> PocoFactories =
            new Cache<Tuple<string, string, int, int>, Delegate>();
*/

        #endregion

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
        /// The definition provider.
        /// </summary>
        private readonly IEntityDefinitionProvider definitionProvider;

        /// <summary>
        /// The database type.
        /// </summary>
        private DatabaseType databaseType;

        /// <summary>
        /// The factory.
        /// </summary>
        private DbProviderFactory factory;

        /// <summary>
        /// The parameter prefix.
        /// </summary>
        private string paramPrefix;

        /// <summary>
        /// The transaction.
        /// </summary>
        private IDbTransaction transaction;

        /// <summary>
        /// The transaction cancelled.
        /// </summary>
        private bool transactionCancelled;

        /// <summary>
        /// The transaction depth.
        /// </summary>
        private int transactionDepth;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Database"/> class. 
        /// Construct a database using a supplied IDbConnection
        /// </summary>
        /// <param name="connection">
        /// The IDbConnection to use
        /// </param>
        /// <param name="definitionProvider">
        /// The entity definition provider.
        /// </param>
        /// <remarks>
        /// The supplied IDbConnection will not be closed/disposed - that remains the responsibility of the caller.
        /// </remarks>
        public Database([NotNull] IDbConnection connection, [NotNull] IEntityDefinitionProvider definitionProvider)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (definitionProvider == null)
            {
                throw new ArgumentNullException(nameof(definitionProvider));
            }

            this.definitionProvider = definitionProvider;
            this.pocoFactory = new RaisedPocoFactory(definitionProvider);

            // TODO: This seems to fail with SqlConnection.
            this.isConnectionUserProvided = true;
            this.Connection = connection;
            this.connectionString = connection.ConnectionString;
            this.CommonConstruct();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Database"/> class. 
        /// Construct a database using a supplied connections string and optionally a provider name
        /// </summary>
        /// <param name="connectionString">
        /// The DB connection string
        /// </param>
        /// <param name="providerName">
        /// The name of the DB provider to use
        /// </param>
        /// <param name="definitionProvider">
        /// The entity definition provider.
        /// </param>
        /// <remarks>
        /// This class will automatically close and dispose any connections it creates.
        /// </remarks>
        public Database([NotNull] string connectionString, [NotNull] string providerName, [NotNull] IEntityDefinitionProvider definitionProvider)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            if (string.IsNullOrWhiteSpace(providerName))
            {
                throw new ArgumentNullException(nameof(providerName));
            }

            if (definitionProvider == null)
            {
                throw new ArgumentNullException(nameof(definitionProvider));
            }

            this.connectionString = connectionString;
            this.providerName = providerName;
            this.definitionProvider = definitionProvider;
            this.pocoFactory = new RaisedPocoFactory(definitionProvider);
            this.CommonConstruct();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Database"/> class. 
        /// Construct a Database using a supplied connection string and a DbProviderFactory
        /// </summary>
        /// <param name="connectionString">
        /// The connection string to use
        /// </param>
        /// <param name="provider">
        /// The DbProviderFactory to use for instantiating IDbConnection's
        /// </param>
        /// <param name="definitionProvider">
        /// The entity definition provider.
        /// </param>
        public Database(
            [NotNull] string connectionString,
            [NotNull] DbProviderFactory provider,
            [NotNull] IEntityDefinitionProvider definitionProvider)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            if (definitionProvider == null)
            {
                throw new ArgumentNullException(nameof(definitionProvider));
            }

            this.connectionString = connectionString;
            this.factory = provider;
            this.definitionProvider = definitionProvider;
            this.pocoFactory = new RaisedPocoFactory(definitionProvider);
            this.CommonConstruct();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Database"/> class. 
        /// Construct a Database using a supplied connectionString Name.  The actual connection string and provider will be
        /// read from app/web.config.
        /// </summary>
        /// <param name="connectionStringName">
        /// The name of the connection
        /// </param>
        /// <param name="definitionProvider">
        /// The entity definition provider.
        /// </param>
        public Database(string connectionStringName, [NotNull] IEntityDefinitionProvider definitionProvider)
        {
            // Use first?
            if (string.IsNullOrWhiteSpace(connectionStringName))
            {
                connectionStringName = ConfigurationManager.ConnectionStrings[0].Name;
            }

            if (definitionProvider == null)
            {
                throw new ArgumentNullException(nameof(definitionProvider));
            }

            this.definitionProvider = definitionProvider;
            this.pocoFactory = new RaisedPocoFactory(definitionProvider);

            // Work out connection string and provider name
            var providerType = "System.Data.SqlClient";

            if (ConfigurationManager.ConnectionStrings[connectionStringName] != null)
            {
                if (string.IsNullOrEmpty(ConfigurationManager.ConnectionStrings[connectionStringName].ProviderName) == false)
                {
                    providerType = ConfigurationManager.ConnectionStrings[connectionStringName].ProviderName;
                }
            }
            else
            {
                throw new InvalidOperationException("Can't find a connection string with the name '" + connectionStringName + "'");
            }

            // Store factory and connection string
            this.connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            this.providerName = providerType;
            ////this.pocoDataFactory = new PocoDataFactory();
            this.CommonConstruct();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the timeout value for all SQL statements.
        /// </summary>
        public int CommandTimeout { get; set; }

        /// <summary>
        /// Gets the currently open shared connection (or null if none)
        /// </summary>
        public IDbConnection Connection { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether to automatically create the "SELECT columns" part of any query that looks like it 
        /// needs it.
        /// </summary>
        public bool EnableAutoSelect { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether parameters named <code>?myparam</code> are populated from properties of the passed
        /// in argument values.
        /// </summary>
        public bool EnableNamedParameters { get; set; }

        /// <summary>
        /// Gets or sets the timeout value for the next (and only next) SQL statement.
        /// </summary>
        public int OnetimeCommandTimeout { get; set; }

        #endregion

        // Helper to create a transaction scope
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
                    throw new InvalidOperationException(string.Format(ErrorMessages.ConnectionFactoryReturnedNullConnection, this.factory));
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
                this.OnConnectionOpened(this.Connection);
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
                this.transactionCancelled = false;
                this.OnBeginTransaction();
            }

            return this.transaction;
        }

        /// <summary>
        /// Aborts the entire outer most transaction scope
        /// </summary>
        /// <remarks>
        /// Called automatically by Transaction.Dispose() if the transaction wasn't completed.
        /// </remarks>
        public void AbortTransaction()
        {
            this.transactionCancelled = true;

            if ((--this.transactionDepth) == 0)
            {
                this.CleanupTransaction();
            }
        }

        /// <summary>
        /// Marks the current transaction scope as complete.
        /// </summary>
        public void CompleteTransaction()
        {
            if ((--this.transactionDepth) == 0)
            {
                this.CleanupTransaction();
            }
        }

        #endregion

        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

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
        public int Execute(string sql, params object[] args)
        {
            try
            {
                this.OpenSharedConnection();

                using (var cmd = this.CreateCommand(this.Connection, sql, args))
                {
                    var returnValue = cmd.ExecuteNonQuery();
                    this.OnExecutedCommand(cmd);
                    return returnValue;
                }
            }
            catch (Exception ex)
            {
                if (this.OnException(ex))
                {
                    throw;
                }

                return -1;
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
        public T ExecuteScalar<T>(string sql, params object[] args)
        {
            try
            {
                this.OpenSharedConnection();

                using (var command = this.CreateCommand(this.Connection, sql, args))
                {
                    var result = command.ExecuteScalar();
                    this.OnExecutedCommand(command);

                    // Handle nullable types
                    var underlyingType = Nullable.GetUnderlyingType(typeof(T));

                    if (underlyingType != null && result == null)
                    {
                        return default(T);
                    }

                    return (T)Convert.ChangeType(result, underlyingType ?? typeof(T));
                }
            }
            catch (Exception ex)
            {
                if (this.OnException(ex))
                {
                    throw;
                }

                return default(T);
            }
        }

        #endregion

        #region Fetch Operations

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
        public IEnumerable<T> Query<T>(string sql, params object[] args)
        {
            if (this.EnableAutoSelect)
            {
                var entityDefinition = this.definitionProvider.Resolve<T>();
                var autoSelectHelper = new AutoSelectHelper(this.databaseType, entityDefinition);
                sql = autoSelectHelper.AddSelectClause(sql);
            }

            this.OpenSharedConnection();

            using (var cmd = this.CreateCommand(this.Connection, sql, args))
            {
                IDataReader dataReader;

                try
                {
                    dataReader = cmd.ExecuteReader();
                    this.OnExecutedCommand(cmd);
                }
                catch (Exception ex)
                {
                    if (this.OnException(ex))
                    {
                        throw;
                    }

                    yield break;
                }

                using (dataReader)
                {
                    while (true)
                    {
                        T poco;

                        try
                        {
                            if (!dataReader.Read())
                            {
                                yield break;
                            }

                            var pocoDataRequest = new PocoDataRequest(dataReader, typeof(T), this.definitionProvider)
                                                      {
                                                          FieldCount = dataReader.FieldCount,
                                                          FirstColumn = 0
                                                      };

                            poco = this.pocoFactory.CreatePoco<T>(pocoDataRequest);
                        }
                        catch (Exception ex)
                        {
                            if (this.OnException(ex))
                            {
                                throw;
                            }

                            yield break;
                        }

                        yield return poco;
                    }
                }
            }
        }

        /// <summary>
        /// Runs a query and returns the result set as a typed list
        /// </summary>
        /// <typeparam name="T">
        /// The Type representing a row in the result set
        /// </typeparam>
        /// <param name="sql">
        /// The SQL query to execute
        /// </param>
        /// <param name="args">
        /// Arguments to any embedded parameters in the SQL
        /// </param>
        /// <returns>
        /// A List holding the results of the query
        /// </returns>
        public IEnumerable<T> Fetch<T>(string sql, params object[] args)
        {
            return this.Query<T>(sql, args).ToList();
        }

        /// <summary>
        /// Retrieves a page of records (without the total count)
        /// </summary>
        /// <typeparam name="T">
        /// The Type representing a row in the result set
        /// </typeparam>
        /// <param name="page">
        /// The 1 based page number to retrieve
        /// </param>
        /// <param name="itemsPerPage">
        /// The number of records per page
        /// </param>
        /// <param name="sql">
        /// The base SQL query
        /// </param>
        /// <param name="args">
        /// Arguments to any embedded parameters in the SQL statement
        /// </param>
        /// <returns>
        /// A List of results
        /// </returns>
        /// <remarks>
        /// Automatically modifies the supplied SELECT statement to only retrieve the records for the specified page.
        /// </remarks>
        public IEnumerable<T> Fetch<T>(long page, long itemsPerPage, string sql, params object[] args)
        {
            return this.SkipTake<T>((page - 1) * itemsPerPage, itemsPerPage, sql, args);
        }

        /// <summary>
        /// Retrieves a page of records and the total number of available records.
        /// </summary>
        /// <typeparam name="T">
        /// The Type representing a row in the result set.
        /// </typeparam>
        /// <param name="page">
        /// The 1 based page number to retrieve.
        /// </param>
        /// <param name="itemsPerPage">
        /// The number of records per page.
        /// </param>
        /// <param name="sqlCount">
        /// The SQL to retrieve the total number of records.
        /// </param>
        /// <param name="countArgs">
        /// Arguments to any embedded parameters in the <paramref name="sqlCount"/> statement.
        /// </param>
        /// <param name="sqlPage">
        /// The SQL To retrieve a single page of results.
        /// </param>
        /// <param name="pageArgs">
        /// Arguments to any embedded parameters in the <paramref name="sqlPage"/> statement.
        /// </param>
        /// <returns>
        /// A Page of results.
        /// </returns>
        /// <remarks>
        /// This method allows separate SQL statements to be explicitly provided for the two parts of the page query.
        /// The page and itemsPerPage parameters are not used directly and are used simply to populate the returned Page object.
        /// </remarks>
        public Page<T> FetchPage<T>(long page, long itemsPerPage, string sqlCount, object[] countArgs, string sqlPage, object[] pageArgs)
        {
            // Save the one-time command time out and use it for both queries
            var saveTimeout = this.OnetimeCommandTimeout;

            // Get the records
            var results = this.Fetch<T>(sqlPage, pageArgs);

            // Setup the paged result
            var result = new Page<T>(results)
                             {
                                 CurrentPage = page, 
                                 ItemsPerPage = itemsPerPage, 
                                 TotalItems = this.ExecuteScalar<long>(sqlCount, countArgs)
                             };

            result.TotalPages = result.TotalItems / itemsPerPage;

            if ((result.TotalItems % itemsPerPage) != 0)
            {
                result.TotalPages++;
            }

            this.OnetimeCommandTimeout = saveTimeout;

            // Done
            return result;
        }

        /// <summary>
        /// Retrieves a page of records and the total number of available records
        /// </summary>
        /// <typeparam name="T">
        /// The Type representing a row in the result set
        /// </typeparam>
        /// <param name="page">
        /// The 1 based page number to retrieve
        /// </param>
        /// <param name="itemsPerPage">
        /// The number of records per page
        /// </param>
        /// <param name="sql">
        /// The base SQL query
        /// </param>
        /// <param name="args">
        /// Arguments to any embedded parameters in the SQL statement
        /// </param>
        /// <returns>
        /// A Page of results
        /// </returns>
        /// <remarks>
        /// Automatically modifies the supplied SELECT statement to only retrieve the
        /// records for the specified page.  It will also execute a second query to retrieve the
        /// total number of records in the result set.
        /// </remarks>
        public Page<T> FetchPage<T>(long page, long itemsPerPage, string sql, params object[] args)
        {
            this.BuildPageQueries<T>((page - 1) * itemsPerPage, itemsPerPage, sql, ref args, out var sqlCount, out var sqlPage);
            return this.FetchPage<T>(page, itemsPerPage, sqlCount, args, sqlPage, args);
        }

        #endregion

        #region IEnumerable

        /// <summary>
        /// Runs a query that should always return at least one return
        /// </summary>
        /// <typeparam name="T">
        /// The Type representing a row in the result set
        /// </typeparam>
        /// <param name="sql">
        /// The SQL query
        /// </param>
        /// <param name="args">
        /// Arguments to any embedded parameters in the SQL statement
        /// </param>
        /// <returns>
        /// The first record in the result set
        /// </returns>
        public T First<T>(string sql, params object[] args)
        {
            return this.Fetch<T>(sql, args).First();
        }

        /// <summary>
        /// Runs a query and returns the first record, or the default value if no matching records
        /// </summary>
        /// <typeparam name="T">
        /// The Type representing a row in the result set
        /// </typeparam>
        /// <param name="sql">
        /// The SQL query
        /// </param>
        /// <param name="args">
        /// Arguments to any embedded parameters in the SQL statement
        /// </param>
        /// <returns>
        /// The first record in the result set, or default(T) if no matching rows
        /// </returns>
        public T FirstOrDefault<T>(string sql, params object[] args)
        {
            return this.Fetch<T>(sql, args).FirstOrDefault();
        }

        /// <summary>
        /// Runs a query that should always return a single row.
        /// </summary>
        /// <typeparam name="T">
        /// The Type representing a row in the result set
        /// </typeparam>
        /// <param name="sql">
        /// The SQL query
        /// </param>
        /// <param name="args">
        /// Arguments to any embedded parameters in the SQL statement
        /// </param>
        /// <returns>
        /// The single record matching the specified primary key value
        /// </returns>
        /// <remarks>
        /// Throws an exception if there are zero or more than one matching record
        /// </remarks>
        public T Single<T>(string sql, params object[] args)
        {
            return this.Fetch<T>(sql, args).Single();
        }

        /// <summary>
        /// Runs a query that should always return either a single row, or no rows
        /// </summary>
        /// <typeparam name="T">
        /// The Type representing a row in the result set
        /// </typeparam>
        /// <param name="sql">
        /// The SQL query
        /// </param>
        /// <param name="args">
        /// Arguments to any embedded parameters in the SQL statement
        /// </param>
        /// <returns>
        /// The single record matching the specified primary key value, or default(T) if no matching rows
        /// </returns>
        public T SingleOrDefault<T>(string sql, params object[] args)
        {
            return this.Fetch<T>(sql, args).SingleOrDefault();
        }

        /// <summary>
        /// Retrieves a range of records from result set
        /// </summary>
        /// <typeparam name="T">
        /// The Type representing a row in the result set
        /// </typeparam>
        /// <param name="skip">
        /// The number of rows at the start of the result set to skip over
        /// </param>
        /// <param name="take">
        /// The number of rows to retrieve
        /// </param>
        /// <param name="sql">
        /// The base SQL query
        /// </param>
        /// <param name="args">
        /// Arguments to any embedded parameters in the SQL statement
        /// </param>
        /// <returns>
        /// A List of results
        /// </returns>
        /// <remarks>
        /// Automatically modifies the supplied SELECT statement to only retrieve the records for the specified range.
        /// </remarks>
        public IEnumerable<T> SkipTake<T>(long skip, long take, string sql, params object[] args)
        {
            this.BuildPageQueries<T>(skip, take, sql, ref args, out _, out var sqlPage);
            return this.Fetch<T>(sqlPage, args);
        }

        #endregion

        #region Insert

        /// <summary>
        /// Performs an SQL Insert
        /// </summary>
        /// <typeparam name="T">
        /// The type of item being inserted.
        /// </typeparam>
        /// <param name="poco">
        /// The POCO object that specifies the column values to be inserted
        /// </param>
        /// <returns>
        /// The auto allocated primary key of the new record, or null for non-auto-increment tables
        /// </returns>
        /// <remarks>
        /// The name of the table, it's primary key and whether it's an auto-allocated primary key are retrieved
        /// from the POCO's attributes
        /// </remarks>
        public object Insert<T>(T poco)
        {
            if (poco == null)
            {
                throw new ArgumentNullException(nameof(poco));
            }

            var definition = this.definitionProvider.Resolve<T>();
            var tableInfo = definition.ToTableInfo();

            try
            {
                this.OpenSharedConnection();

                using (var command = this.CreateCommand(this.Connection, string.Empty))
                {
                    var names = new List<string>();
                    var values = new List<string>();
                    int index = 0;

                    // Capture the primary key type. We'll use this later to change the declared type of the returned ID.
                    var primaryKeyAttribute = definition.PrimaryKeyAttributes.FirstOrDefault();

                    var primaryKeyType = primaryKeyAttribute == EntityAttributeDefinition.Empty
                                             ? null
                                             : definition.PrimaryKeyAttributes.First().PropertyInfo.PropertyType;

                    object result = null;

                    foreach (var column in definition.DirectAttributes)
                    {
                        var columnName = column.ReferenceName;
                        var value = column.GetValueDelegate.DynamicInvoke(poco);

                        // Don't insert the primary key (except under oracle where we need bring in the next sequence value)
                        if (column.IsDirect && column.IsIdentityColumn) 
                        {
                            // Setup auto increment expression
                            string autoIncExpression = this.databaseType.GetAutoIncrementExpression(tableInfo);

                            if (autoIncExpression != null)
                            {
                                names.Add(columnName);
                                values.Add(autoIncExpression);
                            }

                            continue;
                        }

                        if (column.IsDirect && column.IsPrimaryKey)
                        {
                            // Get the primary key value that we'll later return.
                            result = value;
                        }

                        names.Add(this.databaseType.EscapeSqlIdentifier(columnName));
                        values.Add($"{this.paramPrefix}{index++}");
                        this.AddParam(command, value, column.PropertyInfo);
                    }

                    var escapeTableName = $"{tableInfo.TableName}"; //// this.databaseType.EscapeTableName(tableName);
                    var columnNames = string.Join(",", names.ToArray());
                    var columnValues = string.Join(",", values.ToArray());
                    var commandText = $"INSERT INTO {escapeTableName} ({columnNames}) VALUES ({columnValues})";

                    if (definition.AutoNumberPrimaryKey.HasValue)
                    {
                        // TODO: This is SQL-only. Move into the database type thing.
                        string type;

                        if (primaryKeyType == typeof(int))
                        {
                            type = "int";
                        }
                        else if (primaryKeyType == typeof(long))
                        {
                            type = "bigint";
                        }
                        else if (primaryKeyType == typeof(short))
                        {
                            type = "smallint";
                        }
                        else if (primaryKeyType == typeof(byte))
                        {
                            type = "tinyint";
                        }
                        else
                        {
                            type = "decimal";
                        }

                        // Declare and return the ID without using the OUTPUT statement which triggers will mess with.
                        commandText = string.Concat(
                            $"DECLARE @NewId {type}",
                            Environment.NewLine,
                            commandText,
                            Environment.NewLine,
                            "SET @NewId = SCOPE_IDENTITY()",
                            Environment.NewLine,
                            "SELECT @NewId");
                    }

                    command.CommandText = commandText;

                    if (definition.AutoNumberPrimaryKey.HasValue)
                    {
                        result = this.databaseType.ExecuteInsert(this, command, primaryKeyAttribute.ReferenceName);
                        definition.AutoNumberPrimaryKey.Value.SetValueDelegate.DynamicInvoke(poco, result);
                    }
                    else
                    {
                        this.DoPreExecute(command);
                        command.ExecuteNonQuery();
                        this.OnExecutedCommand(command);
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                if (this.OnException(ex))
                {
                    throw;
                }

                return null;
            }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// The execute non query helper.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        internal void ExecuteNonQueryHelper(IDbCommand command)
        {
            this.DoPreExecute(command);
            command.ExecuteNonQuery();
            this.OnExecutedCommand(command);
        }

        /// <summary>
        /// The execute scalar helper.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        internal object ExecuteScalarHelper(IDbCommand command)
        {
            this.DoPreExecute(command);
            var scalar = command.ExecuteScalar();
            this.OnExecutedCommand(command);
            return scalar;
        }

        #region Events

        /// <summary>
        /// Called when a transaction starts.  Overridden by the T4 template generated database
        /// classes to ensure the same DB instance is used throughout the transaction.
        /// </summary>
        protected virtual void OnBeginTransaction()
        {
        }

        /// <summary>
        /// Called when DB connection closed
        /// </summary>
        /// <param name="connection">
        /// The soon to be closed IDBConnection
        /// </param>
        protected virtual void OnConnectionClosing(IDbConnection connection)
        {
        }

        /// <summary>
        /// Occurs when a database connection is opened.
        /// </summary>
        /// <param name="connection">
        /// The newly opened <see cref="IDbConnection"/>.
        /// </param>
        protected virtual void OnConnectionOpened(IDbConnection connection)
        {
        }

        /// <summary>
        /// Called when a transaction ends.
        /// </summary>
        protected virtual void OnEndTransaction()
        {
        }

        /// <summary>
        /// Called if an exception occurs during processing of a DB operation.  Override to provide custom logging/handling.
        /// </summary>
        /// <param name="exception">
        /// The exception instance
        /// </param>
        /// <returns>
        /// True to re-throw the exception, false to suppress it
        /// </returns>
        protected virtual bool OnException(Exception exception)
        {
            return true;
        }

        /// <summary>
        /// Called on completion of command execution
        /// </summary>
        /// <param name="command">
        /// The IDbCommand that finished executing
        /// </param>
        protected virtual void OnExecutedCommand(IDbCommand command)
        {
        }

        /// <summary>
        /// Called just before an DB command is executed.
        /// </summary>
        /// <param name="command">
        /// The command to be executed.
        /// </param>
        /// <remarks>
        /// Override this method to provide custom logging of commands and/or modification of the IDbCommand before it's executed.
        /// </remarks>
        protected virtual void OnExecutingCommand(IDbCommand command)
        {
        }

        #endregion

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
        }

        /// <summary>
        /// The do pre execute.
        /// </summary>
        /// <param name="cmd">
        /// The command.
        /// </param>
        private void DoPreExecute(IDbCommand cmd)
        {
            // Setup command timeout
            if (this.OnetimeCommandTimeout != 0)
            {
                cmd.CommandTimeout = this.OnetimeCommandTimeout;
                this.OnetimeCommandTimeout = 0;
            }
            else if (this.CommandTimeout != 0)
            {
                cmd.CommandTimeout = this.CommandTimeout;
            }

            this.OnExecutingCommand(cmd);
        }

        /// <summary>
        /// Add a parameter to a DB command
        /// </summary>
        /// <param name="cmd">
        /// A reference to the IDbCommand to which the parameter is to be added
        /// </param>
        /// <param name="value">
        /// The value to assign to the parameter
        /// </param>
        /// <param name="pi">
        /// Optional, a reference to the property info of the POCO property from which the value is coming.
        /// </param>
        private void AddParam(IDbCommand cmd, object value, PropertyInfo pi)
        {
            // Convert value to from poco type to db type
            if (pi != null)
            {
                var mapper = Mappers.GetMapper(pi.DeclaringType);
                var fn = mapper.GetToDbConverter(pi);
                if (fn != null)
                {
                    value = fn(value);
                }
            }

            // Support passed in parameters
            if (value is IDbDataParameter dataParameter)
            {
                dataParameter.ParameterName = $"{this.paramPrefix}{cmd.Parameters.Count}";
                cmd.Parameters.Add(dataParameter);
                return;
            }

            // Create the parameter
            var parameter = cmd.CreateParameter();
            parameter.ParameterName = $"{this.paramPrefix}{cmd.Parameters.Count}";

            // Assign the parmeter value
            if (value == null)
            {
                parameter.Value = DBNull.Value;
            }
            else
            {
                // Give the database type first crack at converting to DB required type
                value = this.databaseType.MapParameterValue(value);

                var type = value.GetType();
                if (type.IsEnum)
                {
                    // PostgreSQL .NET driver wont cast enum to int
                    parameter.Value = (int)value;
                }
                else if (type == typeof(Guid))
                {
                    parameter.Value = value.ToString();
                    parameter.DbType = DbType.String;
                    parameter.Size = 40;
                }
                else if (type == typeof(string))
                {
                    // out of memory exception occurs if trying to save more than 4000 characters to SQL Server CE NText column. 
                    // Set before attempting to set Size, or Size will always max out at 4000
                    if ((value as string ?? Convert.ToString(value)).Length + 1 > 4000 && parameter.GetType().Name == "SqlCeParameter")
                    {
                        parameter.GetType().GetProperty("SqlDbType")?.SetValue(parameter, SqlDbType.NText, null);
                    }

                    // Help query plan caching by using common size
                    parameter.Size = Math.Max((value as string ?? Convert.ToString(value)).Length + 1, 4000);
                    parameter.Value = value;
                }
                else if (type == typeof(AnsiString))
                {
                    // Thanks @DataChomp for pointing out the SQL Server indexing performance hit of using wrong string type on varchar
                    parameter.Size = Math.Max((value as AnsiString ?? new AnsiString(string.Empty)).Value.Length + 1, 4000);
                    parameter.Value = (value as AnsiString ?? new AnsiString(string.Empty)).Value;
                    parameter.DbType = DbType.AnsiString;
                }
                else if (value.GetType().Name == "SqlGeography")
                {
                    // SqlGeography is a CLR Type
                    parameter.GetType().GetProperty("UdtTypeName")?.SetValue(parameter, "geography", null);

                    // geography is the equivalent SQL Server Type
                    parameter.Value = value;
                }
                else if (value.GetType().Name == "SqlGeometry")
                {
                    // SqlGeometry is a CLR Type
                    parameter.GetType().GetProperty("UdtTypeName")?.SetValue(parameter, "geometry", null); // geography is the equivalent SQL Server Type
                    parameter.Value = value;
                }
                else
                {
                    parameter.Value = value;
                }
            }

            // Add to the collection
            cmd.Parameters.Add(parameter);
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
            // Perform named argument replacements
            if (this.EnableNamedParameters)
            {
                var newArgs = new List<object>();
                sql = ParametersHelper.ProcessParams(sql, parameters, newArgs);
                parameters = newArgs.ToArray();
            }

            // Perform parameter prefix replacements
            if (this.paramPrefix != "@")
            {
                sql = ParameterPrefixRegex.Replace(sql, m => this.paramPrefix + m.Value.Substring(1));
            }

            sql = sql.Replace("@@", "@"); // <- double @@ escapes a single @

            // Create the command and add parameters
            var command = connection.CreateCommand();
            command.Connection = connection;
            command.CommandText = sql;
            command.Transaction = this.transaction;

            foreach (var item in parameters)
            {
                this.AddParam(command, item, null);
            }

            // Notify the DB type
            this.databaseType.PreExecute(command);

            // Call logging
            if (string.IsNullOrEmpty(sql) == false)
            {
                this.DoPreExecute(command);
            }

            return command;
        }

        /// <summary>
        /// Starting with a regular SELECT statement, derives the SQL statements required to query a
        /// DB for a page of records and the total number of records
        /// </summary>
        /// <typeparam name="T">
        /// The Type representing a row in the result set
        /// </typeparam>
        /// <param name="skip">
        /// The number of rows to skip before the start of the page
        /// </param>
        /// <param name="take">
        /// The number of rows in the page
        /// </param>
        /// <param name="sql">
        /// The original SQL select statement
        /// </param>
        /// <param name="args">
        /// Arguments to any embedded parameters in the SQL
        /// </param>
        /// <param name="sqlCount">
        /// Outputs the SQL statement to query for the total number of matching rows
        /// </param>
        /// <param name="sqlPage">
        /// Outputs the SQL statement to retrieve a single page of matching rows
        /// </param>
        private void BuildPageQueries<T>(long skip, long take, string sql, ref object[] args, out string sqlCount, out string sqlPage)
        {
            // Add auto select clause
            if (this.EnableAutoSelect)
            {
                var entityDefinition = this.definitionProvider.Resolve<T>();
                var autoSelectHelper = new AutoSelectHelper(this.databaseType, entityDefinition);
                sql = autoSelectHelper.AddSelectClause(sql);
            }

            // Split the SQL
            SqlPageStatement pageStatement;

            if (PagingHelper.TrySplitSql(sql, out pageStatement) == false)
            {
                throw new FormatException(ErrorMessages.PagedSqlCouldNotBeParsed);
            }

            sqlPage = this.databaseType.BuildPageQuery(skip, take, pageStatement, ref args);
            sqlCount = pageStatement.SqlCount;
        }

        /// <summary>
        /// Internal helper to cleanup transaction
        /// </summary>
        private void CleanupTransaction()
        {
            this.OnEndTransaction();

            if (this.transactionCancelled)
            {
                this.transaction.Rollback();
            }
            else
            {
                this.transaction.Commit();
            }

            this.transaction.Dispose();
            this.transaction = null;
        }

        /// <summary>
        /// Provides common initialization for the various constructors
        /// </summary>
        private void CommonConstruct()
        {
            // Reset
            this.transactionDepth = 0;
            this.EnableAutoSelect = true;
            this.EnableNamedParameters = true;

            // If a provider name was supplied, get the IDbProviderFactory for it
            if (this.providerName != null)
            {
                this.factory = DbProviderFactories.GetFactory(this.providerName);
            }

            // Resolve the DB Type
            var typeName = (this.factory?.GetType() ?? this.Connection.GetType()).Name;
            this.databaseType = DatabaseType.Resolve(typeName, this.providerName);

            // What character is used for delimiting parameters in SQL
            this.paramPrefix = this.databaseType.GetParameterPrefix(this.connectionString);
        }

        #endregion
    }
}