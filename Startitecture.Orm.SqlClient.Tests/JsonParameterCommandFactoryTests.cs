// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonParameterCommandFactoryTests.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.SqlClient.Tests
{
    using System.Collections.Generic;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Orm.Mapper;
    using Startitecture.Orm.Schema;
    using Startitecture.Orm.SqlClient;
    using Startitecture.Orm.Testing.Entities;

    /// <summary>
    /// Tests the <see cref="JsonParameterCommandFactory"/> class.
    /// </summary>
    [TestClass]
    public class JsonParameterCommandFactoryTests
    {
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
        public async Task Create_JsonParameterCommand_CommandPropertiesMatchExpected()
        {
            var adapter = new TransactSqlAdapter(new DataAnnotationsDefinitionProvider());

            using (var context = new DatabaseContext(ConfigurationRoot.GetConnectionString("OrmTestDb"), SqlClientFactory.Instance, adapter))
            {
                await context.OpenSharedConnectionAsync(CancellationToken.None);

                var target = new JsonParameterCommandFactory();
                var actual = target.Create(context, "Select * FROM Table", "DependentRowTable", new List<DependentRow>());
                Assert.IsInstanceOfType(actual, typeof(SqlCommand));
                Assert.AreEqual("Select * FROM Table", actual.CommandText);
                var actualParameter = actual.Parameters["@DependentRowTable"] as SqlParameter;
                Assert.IsNotNull(actualParameter);
                Assert.AreEqual(SqlDbType.NVarChar, actualParameter.SqlDbType);
            }
        }
    }
}