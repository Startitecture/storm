// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostgreSqlCompilerTests.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.PostgreSql.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Orm.Schema;
    using Startitecture.Orm.Testing.Entities;

    /// <summary>
    /// The postgre sql compiler tests.
    /// </summary>
    [TestClass]
    public class PostgreSqlCompilerTests
    {
        /// <summary>
        /// The create insertion statement with sequence identity matches expected.
        /// </summary>
        [TestMethod]
        public void CreateInsertionStatement_WithSequenceIdentity_MatchesExpected()
        {
            var target = new PostgreSqlCompiler(new DataAnnotationsDefinitionProvider());

            const string Expected = @"INSERT INTO ""dbo"".""FakeData""
(""NormalColumn"", ""NullableColumn"", ""ValueColumn"", ""AnotherValueColumn"", ""AnotherColumn"", ""NullableValueColumn"")
VALUES (@0, @1, @2, @3, @4, @5)
RETURNING ""FakeRowId""";

            var actual = target.CreateInsertionStatement<DataRow>();
            Assert.AreEqual(Expected, actual);
        }
    }
}