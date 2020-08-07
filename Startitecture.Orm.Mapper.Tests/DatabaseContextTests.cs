// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseContextTests.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Mapper.Tests
{
    using System;
    using System.Data.Common;
    using System.Diagnostics;
    using System.Linq;

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
        /// The configuration root.
        /// </summary>
        private static IConfigurationRoot ConfigurationRoot => new ConfigurationBuilder().AddJsonFile("appSettings.json", false).Build();

        /// <summary>
        /// The begin transaction test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void BeginTransaction_SqlCommand_ExecutesNonQuery()
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
            var statementCompiler = new TransactSqlCompiler(definitionProvider);

            using (var target = new DatabaseContext(connectionString, providerName, statementCompiler))
            {
                var transaction = target.BeginTransaction() as SqlTransaction;
                Assert.IsNotNull(transaction);

                var sqlConnection = target.Connection as SqlConnection;
                Assert.IsNotNull(sqlConnection);

                using (var command = new SqlCommand("SELECT TOP 1 * FROM sys.tables", sqlConnection, transaction))
                {
                    command.ExecuteNonQuery();
                }

                transaction.Rollback();
            }
        }

        /// <summary>
        /// The database SQL connection changes database.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void Database_SqlConnectionChangeDatabase_DatabaseChanged()
        {
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var statementCompiler = new TransactSqlCompiler(definitionProvider);

            using (var connection = new SqlConnection(ConfigurationRoot.GetConnectionString("MasterDatabase")))
            using (var database = new DatabaseContext(connection, definitionProvider, statementCompiler))
            {
                connection.Open();
                database.Connection.ChangeDatabase("master");
                Assert.AreEqual("master", database.Connection.Database);
            }
        }
    }
}