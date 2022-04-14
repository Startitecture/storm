// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseContextTests.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Mapper.Tests
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlTypes;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Orm.Schema;
    using Startitecture.Orm.SqlClient;

    /// <summary>
    /// The database tests.
    /// </summary>
    [TestClass]
    public class DatabaseContextTests
    {
        /// <summary>
        /// Gets the configuration root.
        /// </summary>
        private static IConfigurationRoot ConfigurationRoot => new ConfigurationBuilder().AddJsonFile("appsettings.json", false).Build();

        /// <summary>
        /// The begin transaction test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void ExecuteNonQuery_SqlCommand_ExecutesNonQuery()
        {
            var providerName = "System.Data.SqlClient";
#if !NET472
            if (DbProviderFactories.GetProviderInvariantNames().Any(s => string.Equals(s, providerName, StringComparison.Ordinal)) == false)
            {
                Trace.WriteLine($"Registering {providerName} factory");
                DbProviderFactories.RegisterFactory(providerName, SqlClientFactory.Instance);
            }
#endif
            var connectionString = ConfigurationRoot.GetConnectionString("MasterDatabase");
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var statementCompiler = new TransactSqlAdapter(definitionProvider);

            using (var target = new DatabaseContext(connectionString, providerName, statementCompiler))
            {
                target.OpenSharedConnection();
                var sqlConnection = target.Connection as SqlConnection;

                Assert.IsNotNull(sqlConnection);

                using (var transaction = sqlConnection.BeginTransaction())
                {
                    using (var command = new SqlCommand("SELECT TOP 1 * FROM sys.tables", sqlConnection, transaction))
                    {
                        command.ExecuteNonQuery();
                    }

                    transaction.Rollback();
                }
            }
        }

        /// <summary>
        /// The begin transaction test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void ExecuteQuery_SqlQuery_ExecutesQueryWithExpectedResults()
        {
            var providerName = "System.Data.SqlClient";

#if !NET472
            if (DbProviderFactories.GetProviderInvariantNames().Any(s => string.Equals(s, providerName, StringComparison.Ordinal)) == false)
            {
                Trace.WriteLine($"Registering {providerName} factory");
                DbProviderFactories.RegisterFactory(providerName, SqlClientFactory.Instance);
            }
#endif
            var connectionString = ConfigurationRoot.GetConnectionString("MasterDatabase");
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var repositoryAdapter = new TransactSqlAdapter(definitionProvider);

            using (var target = new DatabaseContext(connectionString, providerName, repositoryAdapter))
            {
                var tables = target.Query<dynamic>("SELECT * FROM sys.tables WHERE [type] = @0", 'U').ToList();
                Assert.IsTrue(tables.Any());
                Assert.IsNotNull(tables.FirstOrDefault()?.name);
                Assert.IsTrue(tables.FirstOrDefault()?.object_id > 0);

                var tableCount = target.ExecuteScalar<int>("SELECT COUNT(1) FROM sys.tables WHERE [type] = @0", 'U');
                Assert.AreNotEqual(0, tableCount);
            }
        }

        /// <summary>
        /// The database SQL connection changes database.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void Database_ChangeDatabase_DatabaseChanged()
        {
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var statementCompiler = new TransactSqlAdapter(definitionProvider);

            using (var connection = new SqlConnection(ConfigurationRoot.GetConnectionString("MasterDatabase")))
            using (var database = new DatabaseContext(connection, statementCompiler))
            {
                database.ChangeDatabase("master");
                Assert.AreEqual("master", database.Connection.Database);
            }
        }

        /// <summary>
        /// The begin transaction test.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [TestMethod]
        [TestCategory("Integration")]
        public async Task QueryAsync_SqlQuery_ExecutesQueryWithExpectedResults()
        {
            const string ProviderName = "System.Data.SqlClient";

#if !NET472
            if (DbProviderFactories.GetProviderInvariantNames().Any(s => string.Equals(s, ProviderName, StringComparison.Ordinal)) == false)
            {
                Trace.WriteLine($"Registering {ProviderName} factory");
                DbProviderFactories.RegisterFactory(ProviderName, SqlClientFactory.Instance);
            }
#endif
            var connectionString = ConfigurationRoot.GetConnectionString("MasterDatabase");
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var repositoryAdapter = new TransactSqlAdapter(definitionProvider);

            await using (var target = new DatabaseContext(connectionString, ProviderName, repositoryAdapter))
            {
                var tables = target.QueryAsync<dynamic>("SELECT * FROM sys.tables WHERE [type] = @0", CancellationToken.None, 'U');
                var count = 0;

                await foreach (var table in tables)
                {
                    Assert.IsNotNull(table.name);
                    Assert.IsTrue(table.object_id > 0);
                    count++;
                }

                Assert.AreNotEqual(0, count);

                var tableCount = await target.ExecuteScalarAsync<int>(
                                         "SELECT COUNT(1) FROM sys.tables WHERE [type] = @0",
                                         CancellationToken.None,
                                         'U');

                Assert.AreNotEqual(0, tableCount);
            }
        }

        /// <summary>
        /// The database SQL connection changes database.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> that is running the test.
        /// </returns>
        [TestMethod]
        [TestCategory("Integration")]
        public async Task OpenSharedConnectionAsync_SqlConnection_ConnectionOpened()
        {
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var statementCompiler = new TransactSqlAdapter(definitionProvider);

            await using (var connection = new SqlConnection(ConfigurationRoot.GetConnectionString("MasterDatabase")))
            await using (var database = new DatabaseContext(connection, statementCompiler))
            {
                var cancellationToken = CancellationToken.None;
                await database.OpenSharedConnectionAsync(cancellationToken);
                await database.ChangeDatabaseAsync("master", cancellationToken);
                Assert.AreEqual("master", database.Connection.Database);
            }
        }

        /// <summary>
        /// Tests the BeginTransaction method.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void BeginTransaction_DefaultIsolationLevel_TransactionIsReadCommitted()
        {
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var statementCompiler = new TransactSqlAdapter(definitionProvider);

            using (var connection = new SqlConnection(ConfigurationRoot.GetConnectionString("MasterDatabase")))
            using (var database = new DatabaseContext(connection, statementCompiler))
            {
                database.OpenSharedConnection();

                using (var transaction = database.BeginTransaction())
                {
                    Assert.IsFalse(transaction.IsDisposed);
                    Assert.AreEqual(IsolationLevel.ReadCommitted, transaction.IsolationLevel);
                }
            }
        }

        /// <summary>
        /// Tests the BeginTransactionAsync method.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the result of the asynchronous operation.
        /// </returns>
        [TestMethod]
        [TestCategory("Integration")]
        public async Task BeginTransactionAsync_DefaultIsolationLevel_TransactionIsReadCommitted()
        {
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var statementCompiler = new TransactSqlAdapter(definitionProvider);

            await using (var connection = new SqlConnection(ConfigurationRoot.GetConnectionString("MasterDatabase")))
            await using (var database = new DatabaseContext(connection, statementCompiler))
            {
                var cancellationToken = CancellationToken.None;
                await database.OpenSharedConnectionAsync(cancellationToken);
                await using (var transaction = await database.BeginTransactionAsync(CancellationToken.None))
                {
                    Assert.IsFalse(transaction.IsDisposed);
                    Assert.AreEqual(IsolationLevel.ReadCommitted, transaction.IsolationLevel);
                }
            }
        }

        /// <summary>
        /// Tests the BeginTransaction method.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void BeginTransaction_ExplicitIsolationLevel_IsolationLevelMatchesExpected()
        {
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var statementCompiler = new TransactSqlAdapter(definitionProvider);

            using (var connection = new SqlConnection(ConfigurationRoot.GetConnectionString("MasterDatabase")))
            using (var database = new DatabaseContext(connection, statementCompiler))
            {
                database.OpenSharedConnection();

                using (var transaction = database.BeginTransaction(IsolationLevel.RepeatableRead))
                {
                    Assert.IsFalse(transaction.IsDisposed);
                    Assert.AreEqual(IsolationLevel.RepeatableRead, transaction.IsolationLevel);
                }
            }
        }

        /// <summary>
        /// Tests the BeginTransactionAsync method.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the result of the asynchronous operation.
        /// </returns>
        [TestMethod]
        [TestCategory("Integration")]
        public async Task BeginTransactionAsync_ExplicitIsolationLevel_IsolationLevelMatchesExpected()
        {
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var statementCompiler = new TransactSqlAdapter(definitionProvider);

            await using (var connection = new SqlConnection(ConfigurationRoot.GetConnectionString("MasterDatabase")))
            await using (var database = new DatabaseContext(connection, statementCompiler))
            {
                var cancellationToken = CancellationToken.None;
                await database.OpenSharedConnectionAsync(cancellationToken);
                await using (var transaction = await database.BeginTransactionAsync(IsolationLevel.RepeatableRead, CancellationToken.None))
                {
                    Assert.IsFalse(transaction.IsDisposed);
                    Assert.AreEqual(IsolationLevel.RepeatableRead, transaction.IsolationLevel);
                }
            }
        }

        /// <summary>
        /// Tests the AssociateTransaction test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void AssociateTransaction_SqlCommand_CommandTransactionIsSameAsExpected()
        {
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var statementCompiler = new TransactSqlAdapter(definitionProvider);

            using (var connection = new SqlConnection(ConfigurationRoot.GetConnectionString("MasterDatabase")))
            using (var database = new DatabaseContext(connection, statementCompiler))
            {
                database.OpenSharedConnection();
                var command = new SqlCommand("SELECT * FROM sys.tables WHERE [type] = @0");

                using (var transaction = database.BeginTransaction(IsolationLevel.RepeatableRead))
                {
                    database.AssociateTransaction(command);
                    Assert.IsNotNull(command.Transaction);
                }
            }
        }

        /// <summary>
        /// Tests the GetValueMapper method.
        /// </summary>
        [TestMethod]
        public void GetValueMapper_TransactSqlAdapterForDecimalMoney_ReturnsIValueMapper()
        {
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var statementCompiler = new TransactSqlAdapter(definitionProvider);

            using (var connection = new SqlConnection(ConfigurationRoot.GetConnectionString("MasterDatabase")))
            using (var database = new DatabaseContext(connection, statementCompiler))
            {
                var actual = database.GetValueMapper(typeof(decimal), typeof(SqlMoney));
                Assert.IsNotNull(actual);
            }
        }
    }
}