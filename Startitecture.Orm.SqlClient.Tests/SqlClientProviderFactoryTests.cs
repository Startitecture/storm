// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlClientProviderFactoryTests.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.SqlClient.Tests
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Orm.Schema;

    /// <summary>
    /// The sql client provider factory tests.
    /// </summary>
    [TestClass]
    public class SqlClientProviderFactoryTests
    {
        /// <summary>
        /// The configuration root.
        /// </summary>
        private static IConfigurationRoot ConfigurationRoot => new ConfigurationBuilder().AddJsonFile("appSettings.json", false).Build();

        /// <summary>
        /// The create test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void Create_SqlClientProvider_SysTablesReturnsResultGreaterThanZero()
        {
            var target = new SqlClientProviderFactory(ConfigurationRoot.GetConnectionString("OrmTestDb"), new DataAnnotationsDefinitionProvider());

            using (var provider = target.Create())
            {
                var actual = provider.ExecuteScalar<int>("SELECT COUNT(1) FROM sys.tables");
                Assert.AreNotEqual(0, actual);
            }
        }
    }
}