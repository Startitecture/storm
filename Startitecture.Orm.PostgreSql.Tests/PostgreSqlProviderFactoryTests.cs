// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostgreSqlProviderFactoryTests.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.PostgreSql.Tests
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Orm.Schema;

    /// <summary>
    /// The postgre sql provider factory tests.
    /// </summary>
    [TestClass]
    public class PostgreSqlProviderFactoryTests
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
        public void Create_PostgreSqlProvider_CountOfAggregateOptionTypeGreaterThanZero()
        {
            var target = new PostgreSqlProviderFactory(ConfigurationRoot.GetConnectionString("OrmTestDbPg"), new DataAnnotationsDefinitionProvider());

            using (var provider = target.Create())
            {
                var actual = provider.ExecuteScalar<int>("SELECT COUNT(1) FROM dbo.\"AggregateOptionType\"");
                Assert.AreNotEqual(0, actual);
            }
        }
    }
}