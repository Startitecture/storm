// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Database.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The main Database class.  You can either use this class directly, or derive from it.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;

    using JetBrains.Annotations;

    using Startitecture.Orm.Common;
    using Startitecture.Orm.Mapper.DatabaseTypes;
    using Startitecture.Orm.Mapper.Internal;
    using Startitecture.Orm.Model;
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
        /// <exception cref="ArgumentNullException">
        /// <paramref name="connection"/> or <paramref name="definitionProvider"/> is null.
        /// </exception>
        /// <remarks>
        /// The supplied IDbConnection will not be closed/disposed - that remains the responsibility of the caller.
        /// </remarks>
        public Database([NotNull] IDbConnection connection, [NotNull] IEntityDefinitionProvider definitionProvider)
        {
            this.DefinitionProvider = definitionProvider ?? throw new ArgumentNullException(nameof(definitionProvider));
            this.Connection = connection ?? throw new ArgumentNullException(nameof(connection));
            this.isConnectionUserProvided = true;
            this.connectionString = connection.ConnectionString;
            this.CommonConstruct();

            this.pocoFactory = new RaisedPocoFactory(definitionProvider);
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
        /// <exception cref="ArgumentException">
        /// <paramref name="connectionString"/> or <paramref name="providerName"/> is null or whitespace.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="definitionProvider"/> is null.
        /// </exception>
        /// <remarks>
        /// This class will automatically close and dispose any connections it creates.
        /// </remarks>
        public Database(
            [NotNull] string connectionString,
            [NotNull] string providerName,
            [NotNull] IEntityDefinitionProvider definitionProvider)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(connectionString));
            }

            if (string.IsNullOrWhiteSpace(providerName))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(providerName));
            }

            this.DefinitionProvider = definitionProvider ?? throw new ArgumentNullException(nameof(definitionProvider));
            this.connectionString = connectionString;
            this.providerName = providerName;
            this.CommonConstruct();

            this.pocoFactory = new RaisedPocoFactory(definitionProvider);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Database"/> class. 
        /// Construct a Database using a supplied connection string and a DbProviderFactory
        /// </summary>
        /// <param name="connectionString">
        /// The connection string to use
        /// </param>
        /// <param name="providerFactory">
        /// The DbProviderFactory to use for instantiating IDbConnections.
        /// </param>
        /// <param name="definitionProvider">
        /// The entity definition provider.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="connectionString"/> is null or whitespace.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="providerFactory"/> or <paramref name="definitionProvider"/>.
        /// </exception>
        public Database(
            [NotNull] string connectionString,
            [NotNull] DbProviderFactory providerFactory,
            [NotNull] IEntityDefinitionProvider definitionProvider)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(connectionString));
            }

            this.factory = providerFactory ?? throw new ArgumentNullException(nameof(providerFactory));
            this.DefinitionProvider = definitionProvider ?? throw new ArgumentNullException(nameof(definitionProvider));
            this.connectionString = connectionString;
            this.CommonConstruct();

            this.pocoFactory = new RaisedPocoFactory(definitionProvider);
        }

        #endregion

        #region Public Properties

        /// <inheritdoc />
        public int CommandTimeout { get; set; }

        /// <inheritdoc />
        public IDbConnection Connection { get; private set; }

        /// <inheritdoc />
        public bool EnableNamedParameters { get; set; }

        /// <inheritdoc />
        public int OnetimeCommandTimeout { get; set; }

        /// <inheritdoc />
        public IEntityDefinitionProvider DefinitionProvider { get; }

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

            var entityDefinition = this.DefinitionProvider.Resolve<T>();

            using (var cmd = this.CreateCommand(this.Connection, sql, args))
            using (var dataReader = cmd.ExecuteReader())
            {
                while (true)
                {
                    if (!dataReader.Read())
                    {
                        yield break;
                    }

                    var pocoDataRequest = new PocoDataRequest(dataReader, entityDefinition)
                                              {
                                                  FirstColumn = 0
                                              };

                    var poco = this.pocoFactory.CreatePoco<T>(pocoDataRequest);
                    yield return poco;
                }
            }
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
            return this.Query<T>(sql, args).FirstOrDefault();
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
        /// from the POCO attributes
        /// </remarks>
        public object Insert<T>(T poco)
        {
            if (poco == null)
            {
                throw new ArgumentNullException(nameof(poco));
            }

            var definition = this.DefinitionProvider.Resolve<T>();
            var tableInfo = definition.ToTableInfo();

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
                    this.AddParam(command, value);
                }

                var escapeTableName = $"{tableInfo.TableName}";
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
                    command.ExecuteNonQuery();
                }

                return result;
            }
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
        /// <param name="cmd">
        /// A reference to the IDbCommand to which the parameter is to be added
        /// </param>
        /// <param name="value">
        /// The value to assign to the parameter
        /// </param>
        private void AddParam(IDbCommand cmd, object value)
        {
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

            // Assign the parameter value
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
                    var length = (value as string ?? Convert.ToString(value, CultureInfo.CurrentCulture))?.Length;

                    if (length + 1 > 4000 && parameter.GetType().Name == "SqlCeParameter")
                    {
                        parameter.GetType().GetProperty("SqlDbType")?.SetValue(parameter, SqlDbType.NText, null);
                    }

                    // Help query plan caching by using common size
                    parameter.Size = Math.Max(length.GetValueOrDefault() + 1, 4000);
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

#if NETSTANDARD
            sql = sql.Replace("@@", "@", StringComparison.Ordinal); // <- double @@ escapes a single @
#else
            sql = sql.Replace("@@", "@"); // <- double @@ escapes a single @
#endif

            // Create the command and add parameters
            var command = connection.CreateCommand();
            command.Connection = connection;
            command.CommandText = sql;
            command.Transaction = this.transaction;

            foreach (var item in parameters)
            {
                this.AddParam(command, item);
            }

            // Notify the DB type
            this.databaseType.PreExecute(command);
            return command;
        }

        /// <summary>
        /// Provides common initialization for the various constructors
        /// </summary>
        private void CommonConstruct()
        {
            // Reset
            this.transactionDepth = 0;
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