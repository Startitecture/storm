// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostgreSqlQualifierTests.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.PostgreSql.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The postgre sql qualifier tests.
    /// </summary>
    [TestClass]
    public class PostgreSqlQualifierTests
    {
        /// <summary>
        /// The escape test.
        /// </summary>
        [TestMethod]
        public void Escape_IdentifierWithSpaces_IdentifierIsQuoted()
        {
            var target = new PostgreSqlQualifier();
            var actual = target.Escape("My Identifier [] Woo");
            Assert.AreEqual("\"My Identifier [] Woo\"", actual);
        }
    }
}