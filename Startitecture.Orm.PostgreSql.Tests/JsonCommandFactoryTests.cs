// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonCommandFactoryTests.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.PostgreSql.Tests
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Npgsql;
    using NpgsqlTypes;

    using Startitecture.Orm.PostgreSql;
    using Startitecture.Orm.Schema;
    using Startitecture.Orm.Testing.Entities;

    /// <summary>
    /// Tests the <see cref="JsonCommandFactory"/> class.
    /// </summary>
    [TestClass]
    public class JsonCommandFactoryTests
    {
        /// <summary>
        /// Gets the configuration root.
        /// </summary>
        private static IConfigurationRoot ConfigurationRoot =>
            new ConfigurationBuilder().AddJsonFile("appsettings.json", false)
                .AddUserSecrets<JsonCommandFactoryTests>(true)
                .Build();

        /// <summary>
        /// Tests the create method.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [TestMethod]
        [TestCategory("Integration")]
        public async Task Create_JsonCommand_CommandPropertiesMatchExpected()
        {
            var providerFactory = new PostgreSqlProviderFactory(new DataAnnotationsDefinitionProvider());

            using (var provider = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDbPg")))
            {
                var target = new JsonCommandFactory();
                await provider.DatabaseContext.OpenSharedConnectionAsync(CancellationToken.None);
                var actual = target.Create(provider.DatabaseContext, "Select * from 'Mytable'", "MyParameter", new List<DependentRow>());
                Assert.IsInstanceOfType(actual, typeof(NpgsqlCommand));
                Assert.AreEqual("Select * from 'Mytable'", actual.CommandText);
                var actualParameter = actual.Parameters["MyParameter"] as NpgsqlParameter;
                Assert.IsNotNull(actualParameter);
                Assert.AreEqual(NpgsqlDbType.Jsonb, actualParameter.NpgsqlDbType);
            }
        }
    }
}