// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultDatabaseContextFactoryTests.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Mapper.Tests
{
    using System;
    using System.Data.Common;
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
    /// Tests the <see cref="DefaultDatabaseContextFactory"/> class.
    /// </summary>
    [TestClass]
    public class DefaultDatabaseContextFactoryTests
    {
        private const string SqlProviderName = "System.Data.SqlClient";

        /// <summary>
        /// Gets the configuration root.
        /// </summary>
        private static IConfigurationRoot ConfigurationRoot => new ConfigurationBuilder().AddJsonFile("appsettings.json", false).Build();

        /// <summary>
        /// Tests the Create method.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the result of the asynchronous operation.
        /// </returns>
        [TestMethod]
        [TestCategory("Integration")]
        public async Task Create_SqlClientConnection_OpensWithoutError()
        {
#if !NET472
            if (DbProviderFactories.GetProviderInvariantNames().Any(s => string.Equals(s, SqlProviderName, StringComparison.Ordinal)) == false)
            {
                Trace.WriteLine($"Registering {SqlProviderName} factory");
                DbProviderFactories.RegisterFactory(SqlProviderName, SqlClientFactory.Instance);
            }
#endif
            var target = new DefaultDatabaseContextFactory(
                ConfigurationRoot.GetConnectionString("MasterDatabase"),
                SqlProviderName,
                new TransactSqlAdapter(new DataAnnotationsDefinitionProvider()));

            await using (var context = target.Create())
            {
                await context.OpenSharedConnectionAsync(CancellationToken.None);
            }
        }

        /// <summary>
        /// Tests the CreateAsync method.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [TestMethod]
        [TestCategory("Integration")]
        public async Task CreateAsync_SqlClientConnection_OpensWithoutError()
        {
#if !NET472
            if (DbProviderFactories.GetProviderInvariantNames().Any(s => string.Equals(s, SqlProviderName, StringComparison.Ordinal)) == false)
            {
                Trace.WriteLine($"Registering {SqlProviderName} factory");
                DbProviderFactories.RegisterFactory(SqlProviderName, SqlClientFactory.Instance);
            }
#endif
            var target = new DefaultDatabaseContextFactory(
                ConfigurationRoot.GetConnectionString("MasterDatabase"),
                SqlProviderName,
                new TransactSqlAdapter(new DataAnnotationsDefinitionProvider()));

            await using (var context = await target.CreateAsync(CancellationToken.None))
            {
                await context.OpenSharedConnectionAsync(CancellationToken.None);
            }
        }
    }
}