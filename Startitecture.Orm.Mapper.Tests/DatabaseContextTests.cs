// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseContextTests.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Mapper.Tests
{
    using System;
    using System.Data.Common;
    using System.Diagnostics;
    using System.Linq;
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
        private static IConfigurationRoot ConfigurationRoot => new ConfigurationBuilder().AddJsonFile("appSettings.json", false).Build();

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
                var sqlConnection = target.Connection as SqlConnection;

                Assert.IsNotNull(sqlConnection);

                using (var transaction = sqlConnection.BeginTransaction())
                {
                    using (var command = new SqlCommand("SELECT TOP 1 * FROM sys.tables", sqlConnection, transaction))
                    {
                        target.AssociateTransaction(command);
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

            await using (var target = new DatabaseContext(connectionString, providerName, repositoryAdapter))
            {
                var tables = (await target.QueryAsync<dynamic>("SELECT * FROM sys.tables WHERE [type] = @0", 'U').ConfigureAwait(false)).ToList();
                Assert.IsTrue(tables.Any());
                Assert.IsNotNull(tables.FirstOrDefault()?.name);
                Assert.IsTrue(tables.FirstOrDefault()?.object_id > 0);

                var tableCount = await target.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM sys.tables WHERE [type] = @0", 'U').ConfigureAwait(false);
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
                await database.OpenSharedConnectionAsync().ConfigureAwait(false);
                await database.ChangeDatabaseAsync("master").ConfigureAwait(false);
                Assert.AreEqual("master", database.Connection.Database);
            }
        }
    }
}