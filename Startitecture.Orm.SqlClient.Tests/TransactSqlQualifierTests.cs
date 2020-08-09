// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactSqlQualifierTests.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.SqlClient.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The transact sql qualifier tests.
    /// </summary>
    [TestClass]
    public class TransactSqlQualifierTests
    {
        /// <summary>
        /// The escape test.
        /// </summary>
        [TestMethod]
        public void Escape_IdentifierWithSpaces_MatchesExpected()
        {
            var target = new TransactSqlQualifier();
            var actual = target.Escape("My Identifier");
            Assert.AreEqual("[My Identifier]", actual);
        }
    }
}