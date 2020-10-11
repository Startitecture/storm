// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostgreSqlAdapterTests.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.PostgreSql.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Orm.Model;
    using Startitecture.Orm.Schema;
    using Startitecture.Orm.Testing.Entities;

    /// <summary>
    /// The postgre sql compiler tests.
    /// </summary>
    [TestClass]
    public class PostgreSqlAdapterTests
    {
        /// <summary>
        /// The create insertion statement with sequence identity matches expected.
        /// </summary>
        [TestMethod]
        public void CreateInsertionStatement_WithSequenceIdentity_MatchesExpected()
        {
            var target = new PostgreSqlAdapter(new DataAnnotationsDefinitionProvider());

            const string Expected = @"INSERT INTO ""dbo"".""FakeData""
(""NormalColumn"", ""NullableColumn"", ""ValueColumn"", ""AnotherValueColumn"", ""AnotherColumn"", ""NullableValueColumn"")
VALUES (@0, @1, @2, @3, @4, @5)
RETURNING ""FakeRowId""";

            var actual = target.CreateInsertionStatement<DataRow>();
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement direct data matches expected.
        /// </summary>
        [TestMethod]
        public void CreateExistsStatement_DirectData_MatchesExpected()
        {
            var transactionSelection = Query
                .From<DataRow>(
                    set => set.InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                        .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                        .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                        .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId))
                .Where(
                    set => set.AreEqual(row => row.ValueColumn, 2)
                        .AreEqual(row => row.NullableColumn, "CouldHaveBeenNull")
                        .AreEqual(row => row.NullableValueColumn, null)
                        .Between(row => row.FakeDataId, 10, 20)
                        .GreaterThanOrEqualTo(row => row.NormalColumn, "Greater")
                        .LessThanOrEqualTo(row => row.AnotherColumn, "Less")
                        .Include(row => row.AnotherValueColumn, 5, 10, 15, 20))
                .Sort(
                    set => set.OrderBy(row => row.Related.RelatedProperty)
                        .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                        .OrderBy(row => row.NormalColumn));

            const string Expected = @"SELECT EXISTS(SELECT
    1
FROM ""dbo"".""FakeData""
INNER JOIN ""someschema"".""Related"" ON ""dbo"".""FakeData"".""FakeRowId"" = ""someschema"".""Related"".""FakeDataId""
INNER JOIN ""dbo"".""DependencyEntity"" ON ""someschema"".""Related"".""RelatedId"" = ""dbo"".""DependencyEntity"".""ComplexEntityId""
INNER JOIN ""someschema"".""Related"" AS ""OtherAlias"" ON ""dbo"".""FakeData"".""FakeRowId"" = ""OtherAlias"".""FakeDataId""
INNER JOIN ""dbo"".""DependencyEntity"" AS ""RelatedDependency"" ON ""OtherAlias"".""RelatedId"" = ""RelatedDependency"".""ComplexEntityId""
INNER JOIN ""someschema"".""Related"" AS ""RelatedAlias"" ON ""dbo"".""FakeData"".""FakeRowId"" = ""RelatedAlias"".""FakeDataId""
LEFT JOIN ""dbo"".""SubData"" ON ""dbo"".""FakeData"".""FakeRowId"" = ""dbo"".""SubData"".""FakeSubDataId""
WHERE ""dbo"".""FakeData"".""ValueColumn"" = @0 AND
""dbo"".""FakeData"".""NullableColumn"" LIKE @1 AND
""dbo"".""FakeData"".""NullableValueColumn"" IS NULL AND
""dbo"".""FakeData"".""FakeRowId"" BETWEEN @2 AND @3 AND
""dbo"".""FakeData"".""NormalColumn"" >= @4 AND
""dbo"".""FakeData"".""AnotherColumn"" <= @5 AND
""dbo"".""FakeData"".""AnotherValueColumn"" IN (@6, @7, @8, @9)
ORDER BY ""someschema"".""Related"".""RelatedProperty"", ""OtherAlias"".""RelatedProperty"" DESC, ""dbo"".""FakeData"".""NormalColumn"")";

            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var target = new PostgreSqlAdapter(definitionProvider);
            var actual = target.CreateExistsStatement(transactionSelection);
            Assert.AreEqual(Expected, actual);
        }
    }
}