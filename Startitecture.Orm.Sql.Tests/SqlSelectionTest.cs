// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlSelectionTest.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Sql.Tests
{
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Orm.Schema;
    using Startitecture.Orm.Testing.Entities;

    /// <summary>
    /// The example selection test.
    /// </summary>
    [TestClass]
    public class SqlSelectionTest
    {
        #region Public Methods and Operators

        /// <summary>
        /// The selection statement_ direct data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Create_SelectionStatementForDirectData_MatchesExpected()
        {
            var match = new DataRow { ValueColumn = 2, NullableColumn = "CouldHaveBeenNull", NullableValueColumn = null };
            var baseline = new DataRow { FakeDataId = 10, NormalColumn = "Greater" };
            var boundary = new DataRow { FakeDataId = 20, AnotherColumn = "Less" };
            var transactionSelection = SqlSelect.From<DataRow>()
                .WhereEqual(row => row.ValueColumn, match.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                .Select(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.NullableColumn,
                    row => row.NullableValueColumn,
                    row => row.ValueColumn,
                    row => row.AnotherColumn,
                    row => row.AnotherValueColumn)
                .Between(baseline, boundary, row => row.FakeDataId)
                .GreaterThanOrEqualTo(row => row.NormalColumn, baseline.NormalColumn)
                .LessThanOrEqualTo(row => row.AnotherColumn, boundary.AnotherColumn)
                .Include(row => row.AnotherValueColumn, 5, 10, 15, 20);

            const string Expected = @"SELECT
    [dbo].[FakeData].[FakeRowId],
    [dbo].[FakeData].[NormalColumn],
    [dbo].[FakeData].[NullableColumn],
    [dbo].[FakeData].[NullableValueColumn],
    [dbo].[FakeData].[ValueColumn],
    [dbo].[FakeData].[AnotherColumn],
    [dbo].[FakeData].[AnotherValueColumn]
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[dbo].[FakeData].[FakeRowId] BETWEEN @2 AND @3 AND
[dbo].[FakeData].[NormalColumn] >= @4 AND
[dbo].[FakeData].[AnotherColumn] <= @5 AND
[dbo].[FakeData].[AnotherValueColumn] IN (@6, @7, @8, @9)";

            var expected = new object[]
                               {
                                   2, 
                                   "CouldHaveBeenNull", 
                                   10, 
                                   20, 
                                   baseline.NormalColumn,
                                   boundary.AnotherColumn, 
                                   5,
                                   10,
                                   15,
                                   20
                               };

            var target = new TransactSqlQueryFactory(new DataAnnotationsDefinitionProvider());
            var actual = target.Create(transactionSelection.AsSelect());
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement_ direct data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Create_ContainsStatementForDirectData_MatchesExpected()
        {
            var match = new DataRow { ValueColumn = 2, NullableColumn = "CouldHaveBeenNull", NullableValueColumn = null };
            var baseline = new DataRow { FakeDataId = 10, NormalColumn = "Greater" };
            var boundary = new DataRow { FakeDataId = 20, AnotherColumn = "Less" };

            var transactionSelection = SqlSelect.From<DataRow>()
                .WhereEqual(row => row.ValueColumn, match.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                .Select(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.NullableColumn,
                    row => row.NullableValueColumn,
                    row => row.ValueColumn,
                    row => row.AnotherColumn,
                    row => row.AnotherValueColumn)
                .Between(baseline, boundary, row => row.FakeDataId)
                .GreaterThanOrEqualTo(row => row.NormalColumn, baseline.NormalColumn)
                .LessThanOrEqualTo(row => row.AnotherColumn, boundary.AnotherColumn)
                .Include(row => row.AnotherValueColumn, 5, 10, 15, 20);

            const string Expected = @"IF EXISTS (
SELECT
1
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[dbo].[FakeData].[FakeRowId] BETWEEN @2 AND @3 AND
[dbo].[FakeData].[NormalColumn] >= @4 AND
[dbo].[FakeData].[AnotherColumn] <= @5 AND
[dbo].[FakeData].[AnotherValueColumn] IN (@6, @7, @8, @9)
) SELECT 1  ELSE SELECT 0";

            var target = new TransactSqlQueryFactory(new DataAnnotationsDefinitionProvider());
            var actual = target.Create(transactionSelection.AsContains());
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement_ direct data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Create_DeletionStatementForDirectData_MatchesExpected()
        {
            var match = new DataRow { ValueColumn = 2, NullableColumn = "CouldHaveBeenNull", NullableValueColumn = null };
            var baseline = new DataRow { FakeDataId = 10, NormalColumn = "Greater" };
            var boundary = new DataRow { FakeDataId = 20, AnotherColumn = "Less" };
            var transactionSelection = SqlSelect.From<DataRow>()
                .WhereEqual(row => row.ValueColumn, match.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                .Select(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.NullableColumn,
                    row => row.NullableValueColumn,
                    row => row.ValueColumn,
                    row => row.AnotherColumn,
                    row => row.AnotherValueColumn)
                .Between(baseline, boundary, row => row.FakeDataId)
                .GreaterThanOrEqualTo(row => row.NormalColumn, baseline.NormalColumn)
                .LessThanOrEqualTo(row => row.AnotherColumn, boundary.AnotherColumn)
                .Include(row => row.AnotherValueColumn, 5, 10, 15, 20);

            const string Expected = @"DELETE [dbo].[FakeData]
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[dbo].[FakeData].[FakeRowId] BETWEEN @2 AND @3 AND
[dbo].[FakeData].[NormalColumn] >= @4 AND
[dbo].[FakeData].[AnotherColumn] <= @5 AND
[dbo].[FakeData].[AnotherValueColumn] IN (@6, @7, @8, @9)";

            var target = new TransactSqlQueryFactory(new DataAnnotationsDefinitionProvider());
            var actual = target.Create(transactionSelection.AsDelete());
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement_ direct data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Create_SelectionStatementForDirectDataRaisedRow_MatchesExpected()
        {
            var match = new DataRow { ValueColumn = 2, NullableColumn = "CouldHaveBeenNull", NullableValueColumn = null };
            var baseline = new DataRow { FakeDataId = 10, NormalColumn = "Greater" };
            var boundary = new DataRow { FakeDataId = 20, AnotherColumn = "Less" };
            var transactionSelection = SqlSelect.From<DataRow>()
                .WhereEqual(row => row.ValueColumn, match.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                .Select(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.NullableColumn,
                    row => row.NullableValueColumn,
                    row => row.ValueColumn,
                    row => row.AnotherColumn,
                    row => row.AnotherValueColumn)
                .Between(baseline, boundary, row => row.FakeDataId)
                .GreaterThanOrEqualTo(row => row.NormalColumn, baseline.NormalColumn)
                .LessThanOrEqualTo(row => row.AnotherColumn, boundary.AnotherColumn)
                .Include(row => row.AnotherValueColumn, 5, 10, 15, 20);

            const string Expected = @"SELECT
    [dbo].[FakeData].[FakeRowId],
    [dbo].[FakeData].[NormalColumn],
    [dbo].[FakeData].[NullableColumn],
    [dbo].[FakeData].[NullableValueColumn],
    [dbo].[FakeData].[ValueColumn],
    [dbo].[FakeData].[AnotherColumn],
    [dbo].[FakeData].[AnotherValueColumn]
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[dbo].[FakeData].[FakeRowId] BETWEEN @2 AND @3 AND
[dbo].[FakeData].[NormalColumn] >= @4 AND
[dbo].[FakeData].[AnotherColumn] <= @5 AND
[dbo].[FakeData].[AnotherValueColumn] IN (@6, @7, @8, @9)";

            var target = new TransactSqlQueryFactory(new DataAnnotationsDefinitionProvider());
            var actual = target.Create(transactionSelection.AsSelect());
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement_ direct data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Create_ContainsStatementForDirectDataRaisedRow_MatchesExpected()
        {
            var match = new DataRow { ValueColumn = 2, NullableColumn = "CouldHaveBeenNull", NullableValueColumn = null };
            var baseline = new DataRow { FakeDataId = 10, NormalColumn = "Greater" };
            var boundary = new DataRow { FakeDataId = 20, AnotherColumn = "Less" };
            var transactionSelection = SqlSelect.From<DataRow>()
                .WhereEqual(row => row.ValueColumn, match.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                .Select(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.NullableColumn,
                    row => row.NullableValueColumn,
                    row => row.ValueColumn,
                    row => row.AnotherColumn,
                    row => row.AnotherValueColumn)
                .Between(baseline, boundary, row => row.FakeDataId)
                .GreaterThanOrEqualTo(row => row.NormalColumn, baseline.NormalColumn)
                .LessThanOrEqualTo(row => row.AnotherColumn, boundary.AnotherColumn)
                .Include(row => row.AnotherValueColumn, 5, 10, 15, 20);

            const string Expected = @"IF EXISTS (
SELECT
1
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[dbo].[FakeData].[FakeRowId] BETWEEN @2 AND @3 AND
[dbo].[FakeData].[NormalColumn] >= @4 AND
[dbo].[FakeData].[AnotherColumn] <= @5 AND
[dbo].[FakeData].[AnotherValueColumn] IN (@6, @7, @8, @9)
) SELECT 1  ELSE SELECT 0";

            var target = new TransactSqlQueryFactory(new DataAnnotationsDefinitionProvider());
            var actual = target.Create(transactionSelection.AsContains());
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement_ direct data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Create_DeletiontatementForDirectDataRaisedRow_MatchesExpected()
        {
            var match = new DataRow { ValueColumn = 2, NullableColumn = "CouldHaveBeenNull", NullableValueColumn = null };
            var baseline = new DataRow { FakeDataId = 10, NormalColumn = "Greater" };
            var boundary = new DataRow { FakeDataId = 20, AnotherColumn = "Less" };
            var transactionSelection = SqlSelect.From<DataRow>()
                .WhereEqual(row => row.ValueColumn, match.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                .Select(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.NullableColumn,
                    row => row.NullableValueColumn,
                    row => row.ValueColumn,
                    row => row.AnotherColumn,
                    row => row.AnotherValueColumn)
                .Between(baseline, boundary, row => row.FakeDataId)
                .GreaterThanOrEqualTo(row => row.NormalColumn, baseline.NormalColumn)
                .LessThanOrEqualTo(row => row.AnotherColumn, boundary.AnotherColumn)
                .Include(row => row.AnotherValueColumn, 5, 10, 15, 20);

            const string Expected = @"DELETE [dbo].[FakeData]
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[dbo].[FakeData].[FakeRowId] BETWEEN @2 AND @3 AND
[dbo].[FakeData].[NormalColumn] >= @4 AND
[dbo].[FakeData].[AnotherColumn] <= @5 AND
[dbo].[FakeData].[AnotherValueColumn] IN (@6, @7, @8, @9)";

            var target = new TransactSqlQueryFactory(new DataAnnotationsDefinitionProvider());
            var actual = target.Create(transactionSelection.AsDelete());
            Assert.AreEqual(Expected, actual);
        }

////        /// <summary>
////        /// The selection statement_ related data_ matches expected.
////        /// </summary>
////        [TestMethod]
////        public void Create_SelectionStatementForRelatedData_MatchesExpected()
////        {
////            var match = new DataRow
////                            {
////                                NullableValueColumn = null,
////                                RelatedAlias = new FakeRelatedRow
////                                                   {
////                                                       RelatedProperty = "Related"
////                                                   },
////                                NullableColumn = "CouldHaveBeenNull",
////                                ValueColumn = 2
////                            };

////            var baseline = new DataRow { FakeDataId = 10 };
////            var boundary = new DataRow { FakeDataId = 20 };
////            var transactionSelection = SqlSelect.From<DataRow>()
////                .WhereEqual(row => row.ValueColumn, match.ValueColumn)
////                .WhereEqual(row => row.NullableColumn, match.NullableColumn)
////                .WhereEqual(row => row.NullableValueColumn, match.NullableValueColumn)
////                .WhereEqual(row => row.RelatedAlias.RelatedProperty, match.RelatedAlias.RelatedProperty)
////                .Select(
////                    row => row.FakeDataId,
////                    row => row.NormalColumn,
////                    row => row.Related.RelatedId,
////                    row => row.Related.RelatedProperty,
////                    row => row.RelatedAlias.RelatedId,
////                    row => row.RelatedAlias.RelatedProperty,
////                    row => row.OtherAlias.RelatedId,
////                    row => row.OtherAlias.RelatedProperty,
////                    row => row.ParentFakeDataId)
////                .Between(baseline, boundary, row => row.FakeDataId);

////            const string Expected = @"SELECT
////    [dbo].[FakeData].[FakeRowId],
////    [dbo].[FakeData].[NormalColumn],
////    [someschema].[Related].[RelatedId] AS [FakeRelatedRelatedId],
////    [someschema].[Related].[RelatedProperty] AS [FakeRelatedRelatedProperty],
////    [RelatedAlias].[RelatedId],
////    [RelatedAlias].[RelatedProperty] AS [RelatedAliasRelatedProperty],
////    [OtherAlias].[RelatedId] AS [OtherAliasRelatedId],
////    [OtherAlias].[RelatedProperty] AS [OtherAliasRelatedProperty],
////    [dbo].[SubData].[ParentFakeDataId]
////FROM [dbo].[FakeData]
////INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
////INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[FakeComplexEntityId]
////INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
////INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
////INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
////LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
////WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
////[dbo].[FakeData].[NullableColumn] LIKE @1 AND
////[dbo].[FakeData].[NullableValueColumn] IS NULL AND
////[RelatedAlias].[RelatedProperty] LIKE @2 AND
////[dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4";

////            var target = new TransactSqlQueryFactory(new DataAnnotationsDefinitionProvider());
////            var actual = target.Create(transactionSelection.AsSelect());
////            Assert.AreEqual(Expected, actual);
////        }

        /// <summary>
        /// The selection statement_ related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Create_ContainsStatementForRelatedData_MatchesExpected()
        {
            var match = new DataRow
            {
                NullableValueColumn = null,
                RelatedAlias = new FakeRelatedRow
                                   {
                                       RelatedProperty = "Related"
                                   },
                NullableColumn = "CouldHaveBeenNull",
                ValueColumn = 2
            };

            var baseline = new DataRow { FakeDataId = 10 };
            var boundary = new DataRow { FakeDataId = 20 };
            var transactionSelection = SqlSelect.From<DataRow>()
                .WhereEqual(row => row.ValueColumn, match.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                .WhereEqual(row => row.RelatedAlias.RelatedProperty, match.RelatedAlias.RelatedProperty)
                .Select(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.Related.RelatedId,
                    row => row.Related.RelatedProperty,
                    row => row.Related.RelatedId,
                    row => row.RelatedAlias.RelatedProperty,
                    row => row.OtherAlias.RelatedId,
                    row => row.OtherAlias.RelatedProperty,
                    row => row.ParentFakeDataId)
                .Between(baseline, boundary, row => row.FakeDataId);

            const string Expected = @"IF EXISTS (
SELECT
1
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @2 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4
) SELECT 1  ELSE SELECT 0";

            var target = new TransactSqlQueryFactory(new DataAnnotationsDefinitionProvider());
            var actual = target.Create(transactionSelection.AsContains());
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement_ related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Create_DeletionStatementForRelatedData_MatchesExpected()
        {
            var match = new DataRow
            {
                NullableValueColumn = null,
                RelatedAlias = new FakeRelatedRow
                                   {
                                       RelatedProperty = "Related"
                                   },
                NullableColumn = "CouldHaveBeenNull",
                ValueColumn = 2
            };

            var baseline = new DataRow { FakeDataId = 10 };
            var boundary = new DataRow { FakeDataId = 20 };
            var transactionSelection = SqlSelect.From<DataRow>()
                .WhereEqual(row => row.ValueColumn, match.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                .WhereEqual(row => row.RelatedAlias.RelatedProperty, match.RelatedAlias.RelatedProperty)
                .Select(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.Related.RelatedId,
                    row => row.Related.RelatedProperty,
                    row => row.Related.RelatedId,
                    row => row.RelatedAlias.RelatedProperty,
                    row => row.OtherAlias.RelatedId,
                    row => row.OtherAlias.RelatedProperty,
                    row => row.ParentFakeDataId)
                .Between(baseline, boundary, row => row.FakeDataId);

            const string Expected = @"DELETE [dbo].[FakeData]
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @2 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4";

            var target = new TransactSqlQueryFactory(new DataAnnotationsDefinitionProvider());
            var actual = target.Create(transactionSelection.AsDelete());
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement_ related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Create_SelectionStatementForRelatedDataRaisedRow_MatchesExpected()
        {
            var match = new DataRow
            {
                NullableValueColumn = null,
                RelatedAlias = new FakeRelatedRow { RelatedProperty = "Related" },
                NullableColumn = "CouldHaveBeenNull",
                ValueColumn = 2
            };

            var baseline = new DataRow { FakeDataId = 10 };
            var boundary = new DataRow { FakeDataId = 20 };
            var transactionSelection = SqlSelect.From<DataRow>()
                .WhereEqual(row => row.ValueColumn, match.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                .WhereEqual(row => row.RelatedAlias.RelatedProperty, "Related")
                .Select(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.ParentFakeDataId,
                    row => row.Related.RelatedId,
                    row => row.Related.RelatedProperty,
                    row => row.RelatedAlias.RelatedId,
                    row => row.RelatedAlias.RelatedProperty,
                    row => row.OtherAlias.RelatedId,
                    row => row.OtherAlias.RelatedProperty)
                .Between(baseline, boundary, row => row.FakeDataId);

            const string Expected = @"SELECT
    [dbo].[FakeData].[FakeRowId],
    [dbo].[FakeData].[NormalColumn],
    [dbo].[SubData].[ParentFakeDataId],
    [someschema].[Related].[RelatedId] AS [Related.RelatedId],
    [someschema].[Related].[RelatedProperty] AS [Related.RelatedProperty],
    [RelatedAlias].[RelatedId] AS [RelatedAlias.RelatedId],
    [RelatedAlias].[RelatedProperty] AS [RelatedAlias.RelatedProperty],
    [OtherAlias].[RelatedId] AS [OtherAlias.RelatedId],
    [OtherAlias].[RelatedProperty] AS [OtherAlias.RelatedProperty]
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @2 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4";

            var target = new TransactSqlQueryFactory(new DataAnnotationsDefinitionProvider());
            var actual = target.Create(transactionSelection.AsSelect());
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement_ related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Create_ContainsStatementForRelatedDataRaisedRow_MatchesExpected()
        {
            var match = new DataRow
            {
                NullableValueColumn = null,
                RelatedAlias = new FakeRelatedRow { RelatedProperty = "Related" },
                NullableColumn = "CouldHaveBeenNull",
                ValueColumn = 2
            };

            var baseline = new DataRow { FakeDataId = 10 };
            var boundary = new DataRow { FakeDataId = 20 };
            var transactionSelection = SqlSelect.From<DataRow>()
                .WhereEqual(row => row.ValueColumn, match.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                .WhereEqual(row => row.RelatedAlias.RelatedProperty, "Related")
                .Select(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.ParentFakeDataId,
                    row => row.Related.RelatedId,
                    row => row.Related.RelatedProperty,
                    row => row.RelatedAlias.RelatedId,
                    row => row.RelatedAlias.RelatedProperty,
                    row => row.OtherAlias.RelatedId,
                    row => row.OtherAlias.RelatedProperty)
                .Between(baseline, boundary, row => row.FakeDataId);

            const string Expected = @"IF EXISTS (
SELECT
1
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @2 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4
) SELECT 1  ELSE SELECT 0";

            var target = new TransactSqlQueryFactory(new DataAnnotationsDefinitionProvider());
            var actual = target.Create(transactionSelection.AsContains());
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement_ related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Create_DeletionStatementForRelatedDataRaisedRow_MatchesExpected()
        {
            var match = new DataRow
            {
                NullableValueColumn = null,
                RelatedAlias = new FakeRelatedRow { RelatedProperty = "Related" },
                NullableColumn = "CouldHaveBeenNull",
                ValueColumn = 2
            };

            var baseline = new DataRow { FakeDataId = 10 };
            var boundary = new DataRow { FakeDataId = 20 };
            var transactionSelection = SqlSelect.From<DataRow>()
                .WhereEqual(row => row.ValueColumn, match.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                .WhereEqual(row => row.RelatedAlias.RelatedProperty, "Related")
                .Select(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.ParentFakeDataId,
                    row => row.Related.RelatedId,
                    row => row.Related.RelatedProperty,
                    row => row.RelatedAlias.RelatedId,
                    row => row.RelatedAlias.RelatedProperty,
                    row => row.OtherAlias.RelatedId,
                    row => row.OtherAlias.RelatedProperty)
                .Between(baseline, boundary, row => row.FakeDataId);

            const string Expected = @"DELETE [dbo].[FakeData]
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @2 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4";

            var target = new TransactSqlQueryFactory(new DataAnnotationsDefinitionProvider());
            var actual = target.Create(transactionSelection.AsDelete());
            Assert.AreEqual(Expected, actual);
        }

////        /// <summary>
////        /// The selection statement_ union related data_ matches expected.
////        /// </summary>
////        [TestMethod]
////        public void Create_SelectionStatementForUnionRelatedData_MatchesExpected()
////        {
////            var match1 = new DataRow
////            {
////                NullableValueColumn = null,
////                RelatedAlias = new FakeRelatedRow
////                                   {
////                                       RelatedProperty = "Related1"
////                                   },
////                NullableColumn = "CouldHaveBeenNull1",
////                ValueColumn = 2
////            };

////            var baseline1 = new DataRow { FakeDataId = 10 };
////            var boundary1 = new DataRow { FakeDataId = 20 };

////            var match2 = new DataRow
////            {
////                NullableValueColumn = null,
////                RelatedAlias = new FakeRelatedRow
////                                   {
////                                       RelatedProperty = "Related2"
////                                   },
////                NullableColumn = "CouldHaveBeenNull2",
////                ValueColumn = 3
////            };

////            var baseline2 = new DataRow { FakeDataId = 50 };
////            var boundary2 = new DataRow { FakeDataId = 40 };

////            var match3 = new DataRow
////            {
////                NullableValueColumn = null,
////                RelatedAlias = new FakeRelatedRow
////                                   {
////                                       RelatedProperty = "Related3"
////                                   },
////                NullableColumn = "CouldHaveBeenNull3",
////                ValueColumn = 4
////            };

////            var baseline3 = new DataRow { FakeDataId = 60 };
////            var boundary3 = new DataRow { FakeDataId = 70 };

////            var transactionSelection = SqlSelect.From<DataRow>()
////                .WhereEqual(row => row.ValueColumn, match1.ValueColumn)
////                .WhereEqual(row => row.NullableColumn, match1.NullableColumn)
////                .WhereEqual(row => row.NullableValueColumn, match1.NullableValueColumn)
////                .WhereEqual(row => row.RelatedAlias.RelatedProperty, match1.RelatedAlias.RelatedProperty)
////                .Select(
////                    row => row.FakeDataId,
////                    row => row.NormalColumn,
////                    row => row.Related.RelatedId,
////                    row => row.RelatedAlias.RelatedProperty,
////                    row => row.OtherAlias.RelatedProperty)
////                .Between(baseline1, boundary1, row => row.FakeDataId)
////                .Union(
////                    SqlSelect.From<DataRow>()
////                        .WhereEqual(row => row.ValueColumn, match2.ValueColumn)
////                        .WhereEqual(row => row.NullableColumn, match2.NullableColumn)
////                        .WhereEqual(row => row.NullableValueColumn, match2.NullableValueColumn)
////                        .WhereEqual(row => row.RelatedAlias.RelatedProperty, match2.RelatedAlias.RelatedProperty)
////                        .Select(
////                            row => row.FakeDataId,
////                            row => row.NormalColumn,
////                            row => row.Related.RelatedId,
////                            row => row.RelatedAlias.RelatedProperty,
////                            row => row.OtherAlias.RelatedProperty)
////                        .Between(baseline2, boundary2, row => row.FakeDataId)
////                        .Union(
////                            SqlSelect.From<DataRow>()
////                                .WhereEqual(row => row.ValueColumn, match3.ValueColumn)
////                                .WhereEqual(row => row.NullableColumn, match3.NullableColumn)
////                                .WhereEqual(row => row.NullableValueColumn, match3.NullableValueColumn)
////                                .WhereEqual(row => row.RelatedAlias.RelatedProperty, match3.RelatedAlias.RelatedProperty)
////                                .Select(
////                                    row => row.FakeDataId,
////                                    row => row.NormalColumn,
////                                    row => row.Related.RelatedId,
////                                    row => row.RelatedAlias.RelatedProperty,
////                                    row => row.OtherAlias.RelatedProperty)
////                                .Between(baseline3, boundary3, row => row.FakeDataId)));

////            const string Expected = @"SELECT
////    [dbo].[FakeData].[FakeRowId],
////    [dbo].[FakeData].[NormalColumn],
////    [RelatedAlias].[RelatedId],
////    [RelatedAlias].[RelatedProperty] AS [RelatedAliasRelatedProperty],
////    [OtherAlias].[RelatedProperty] AS [OtherAliasRelatedProperty]
////FROM [dbo].[FakeData]
////INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
////INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[FakeComplexEntityId]
////INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
////INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
////INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
////LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
////WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
////[dbo].[FakeData].[NullableColumn] LIKE @1 AND
////[dbo].[FakeData].[NullableValueColumn] IS NULL AND
////[RelatedAlias].[RelatedProperty] LIKE @2 AND
////[dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4
////UNION
////SELECT
////    [dbo].[FakeData].[FakeRowId],
////    [dbo].[FakeData].[NormalColumn],
////    [RelatedAlias].[RelatedId],
////    [RelatedAlias].[RelatedProperty] AS [RelatedAliasRelatedProperty],
////    [OtherAlias].[RelatedProperty] AS [OtherAliasRelatedProperty]
////FROM [dbo].[FakeData]
////INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
////INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[FakeComplexEntityId]
////INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
////INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
////INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
////LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
////WHERE [dbo].[FakeData].[ValueColumn] = @5 AND
////[dbo].[FakeData].[NullableColumn] LIKE @6 AND
////[dbo].[FakeData].[NullableValueColumn] IS NULL AND
////[RelatedAlias].[RelatedProperty] LIKE @7 AND
////[dbo].[FakeData].[FakeRowId] BETWEEN @8 AND @9
////UNION
////SELECT
////    [dbo].[FakeData].[FakeRowId],
////    [dbo].[FakeData].[NormalColumn],
////    [RelatedAlias].[RelatedId],
////    [RelatedAlias].[RelatedProperty] AS [RelatedAliasRelatedProperty],
////    [OtherAlias].[RelatedProperty] AS [OtherAliasRelatedProperty]
////FROM [dbo].[FakeData]
////INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
////INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[FakeComplexEntityId]
////INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
////INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
////INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
////LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
////WHERE [dbo].[FakeData].[ValueColumn] = @10 AND
////[dbo].[FakeData].[NullableColumn] LIKE @11 AND
////[dbo].[FakeData].[NullableValueColumn] IS NULL AND
////[RelatedAlias].[RelatedProperty] LIKE @12 AND
////[dbo].[FakeData].[FakeRowId] BETWEEN @13 AND @14";

////            var target = new TransactSqlQueryFactory(new DataAnnotationsDefinitionProvider());
////            var actual = target.Create(transactionSelection.AsSelect());
////            Assert.AreEqual(Expected, actual);
////        }

        /// <summary>
        /// The selection statement_ union related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Create_ContainsStatementForUnionRelatedData_MatchesExpected()
        {
            var match1 = new DataRow
                             {
                                 NullableValueColumn = null,
                                 RelatedAlias = new FakeRelatedRow
                                                    {
                                                        RelatedProperty = "Related1"
                                                    },
                                 NullableColumn = "CouldHaveBeenNull1",
                                 ValueColumn = 2
                             };

            var baseline1 = new DataRow { FakeDataId = 10 };
            var boundary1 = new DataRow { FakeDataId = 20 };

            var match2 = new DataRow
                             {
                                 NullableValueColumn = null,
                                 RelatedAlias = new FakeRelatedRow
                                                    {
                                                        RelatedProperty = "Related2"
                                                    },
                                 NullableColumn = "CouldHaveBeenNull2",
                                 ValueColumn = 3
                             };

            var baseline2 = new DataRow { FakeDataId = 50 };
            var boundary2 = new DataRow { FakeDataId = 40 };

            var match3 = new DataRow
                             {
                                 NullableValueColumn = null,
                                 RelatedAlias = new FakeRelatedRow
                                                    {
                                                        RelatedProperty = "Related3"
                                                    },
                                 NullableColumn = "CouldHaveBeenNull3",
                                 ValueColumn = 4
                             };

            var baseline3 = new DataRow { FakeDataId = 60 };
            var boundary3 = new DataRow { FakeDataId = 70 };

            var transactionSelection = SqlSelect.From<DataRow>()
                .WhereEqual(row => row.ValueColumn, match1.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match1.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match1.NullableValueColumn)
                .WhereEqual(row => row.RelatedAlias.RelatedProperty, match1.RelatedAlias.RelatedProperty)
                .Select(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.Related.RelatedId,
                    row => row.RelatedAlias.RelatedProperty,
                    row => row.OtherAlias.RelatedProperty)
                .Between(baseline1, boundary1, row => row.FakeDataId)
                .Union(
                    SqlSelect.From<DataRow>()
                        .WhereEqual(row => row.ValueColumn, match2.ValueColumn)
                        .WhereEqual(row => row.NullableColumn, match2.NullableColumn)
                        .WhereEqual(row => row.NullableValueColumn, match2.NullableValueColumn)
                        .WhereEqual(row => row.RelatedAlias.RelatedProperty, match2.RelatedAlias.RelatedProperty)
                        .Select(
                            row => row.FakeDataId,
                            row => row.NormalColumn,
                            row => row.Related.RelatedId,
                            row => row.RelatedAlias.RelatedProperty,
                            row => row.OtherAlias.RelatedProperty)
                        .Between(baseline2, boundary2, row => row.FakeDataId)
                        .Union(
                            SqlSelect.From<DataRow>()
                                .WhereEqual(row => row.ValueColumn, match3.ValueColumn)
                                .WhereEqual(row => row.NullableColumn, match3.NullableColumn)
                                .WhereEqual(row => row.NullableValueColumn, match3.NullableValueColumn)
                                .WhereEqual(row => row.RelatedAlias.RelatedProperty, match3.RelatedAlias.RelatedProperty)
                                .Select(
                                    row => row.FakeDataId,
                                    row => row.NormalColumn,
                                    row => row.Related.RelatedId,
                                    row => row.RelatedAlias.RelatedProperty,
                                    row => row.OtherAlias.RelatedProperty)
                                .Between(baseline3, boundary3, row => row.FakeDataId)));

            const string Expected = @"IF EXISTS (
SELECT
1
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @2 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4
UNION
SELECT
1
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @5 AND
[dbo].[FakeData].[NullableColumn] LIKE @6 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @7 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @8 AND @9
UNION
SELECT
1
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @10 AND
[dbo].[FakeData].[NullableColumn] LIKE @11 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @12 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @13 AND @14
) SELECT 1  ELSE SELECT 0";

            var target = new TransactSqlQueryFactory(new DataAnnotationsDefinitionProvider());
            var actual = target.Create(transactionSelection.AsContains());
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement_ union related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Create_DeletionStatementForUnionRelatedData_MatchesExpected()
        {
            var match1 = new DataRow
                             {
                                 NullableValueColumn = null,
                                 RelatedAlias = new FakeRelatedRow
                                                    {
                                                        RelatedProperty = "Related1"
                                                    },
                                 NullableColumn = "CouldHaveBeenNull1",
                                 ValueColumn = 2
                             };

            var baseline1 = new DataRow { FakeDataId = 10 };
            var boundary1 = new DataRow { FakeDataId = 20 };

            var match2 = new DataRow
                             {
                                 NullableValueColumn = null,
                                 RelatedAlias = new FakeRelatedRow
                                                    {
                                                        RelatedProperty = "Related2"
                                                    },
                                 NullableColumn = "CouldHaveBeenNull2",
                                 ValueColumn = 3
                             };

            var baseline2 = new DataRow { FakeDataId = 50 };
            var boundary2 = new DataRow { FakeDataId = 40 };

            var match3 = new DataRow
                             {
                                 NullableValueColumn = null,
                                 RelatedAlias = new FakeRelatedRow
                                                    {
                                                        RelatedProperty = "Related3"
                                                    },
                                 NullableColumn = "CouldHaveBeenNull3",
                                 ValueColumn = 4
                             };

            var baseline3 = new DataRow { FakeDataId = 60 };
            var boundary3 = new DataRow { FakeDataId = 70 };

            var transactionSelection = SqlSelect.From<DataRow>()
                .WhereEqual(row => row.ValueColumn, match1.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match1.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match1.NullableValueColumn)
                .WhereEqual(row => row.RelatedAlias.RelatedProperty, match1.RelatedAlias.RelatedProperty)
                .Select(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.Related.RelatedId,
                    row => row.RelatedAlias.RelatedProperty,
                    row => row.OtherAlias.RelatedProperty)
                .Between(baseline1, boundary1, row => row.FakeDataId)
                .Union(
                    SqlSelect.From<DataRow>()
                        .WhereEqual(row => row.ValueColumn, match2.ValueColumn)
                        .WhereEqual(row => row.NullableColumn, match2.NullableColumn)
                        .WhereEqual(row => row.NullableValueColumn, match2.NullableValueColumn)
                        .WhereEqual(row => row.RelatedAlias.RelatedProperty, match2.RelatedAlias.RelatedProperty)
                        .Select(
                            row => row.FakeDataId,
                            row => row.NormalColumn,
                            row => row.Related.RelatedId,
                            row => row.RelatedAlias.RelatedProperty,
                            row => row.OtherAlias.RelatedProperty)
                        .Between(baseline2, boundary2, row => row.FakeDataId)
                        .Union(
                            SqlSelect.From<DataRow>()
                                .WhereEqual(row => row.ValueColumn, match3.ValueColumn)
                                .WhereEqual(row => row.NullableColumn, match3.NullableColumn)
                                .WhereEqual(row => row.NullableValueColumn, match3.NullableValueColumn)
                                .WhereEqual(row => row.RelatedAlias.RelatedProperty, match3.RelatedAlias.RelatedProperty)
                                .Select(
                                    row => row.FakeDataId,
                                    row => row.NormalColumn,
                                    row => row.Related.RelatedId,
                                    row => row.RelatedAlias.RelatedProperty,
                                    row => row.OtherAlias.RelatedProperty)
                                .Between(baseline3, boundary3, row => row.FakeDataId)));

            const string Expected = @"DELETE [dbo].[FakeData]
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @2 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4";

            var target = new TransactSqlQueryFactory(new DataAnnotationsDefinitionProvider());
            var actual = target.Create(transactionSelection.AsDelete());
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement_ union related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Create_SelectionStatementForUnionRelatedDataRaisedRow_MatchesExpected()
        {
            var match1 = new DataRow
            {
                NullableValueColumn = null,
                RelatedAlias = new FakeRelatedRow { RelatedProperty = "Related1" },
                NullableColumn = "CouldHaveBeenNull1",
                ValueColumn = 2
            };

            var baseline1 = new DataRow { FakeDataId = 10 };
            var boundary1 = new DataRow { FakeDataId = 20 };

            var match2 = new DataRow
            {
                NullableValueColumn = null,
                RelatedAlias = new FakeRelatedRow { RelatedProperty = "Related2" },
                NullableColumn = "CouldHaveBeenNull2",
                ValueColumn = 3
            };

            var baseline2 = new DataRow { FakeDataId = 50 };
            var boundary2 = new DataRow { FakeDataId = 40 };

            var match3 = new DataRow
            {
                NullableValueColumn = null,
                RelatedAlias = new FakeRelatedRow { RelatedProperty = "Related3" },
                NullableColumn = "CouldHaveBeenNull3",
                ValueColumn = 4
            };

            var baseline3 = new DataRow { FakeDataId = 60 };
            var boundary3 = new DataRow { FakeDataId = 70 };

            var transactionSelection = SqlSelect.From<DataRow>()
                .WhereEqual(row => row.ValueColumn, match1.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match1.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match1.NullableValueColumn)
                .WhereEqual(row => row.RelatedAlias.RelatedProperty, "Related1")
                .Select(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.RelatedAlias.RelatedId,
                    row => row.RelatedAlias.RelatedProperty,
                    row => row.OtherAlias.RelatedProperty)
                .Between(baseline1, boundary1, row => row.FakeDataId)
                .Union(
                    SqlSelect.From<DataRow>()
                        .WhereEqual(row => row.ValueColumn, match2.ValueColumn)
                        .WhereEqual(row => row.NullableColumn, match2.NullableColumn)
                        .WhereEqual(row => row.NullableValueColumn, match2.NullableValueColumn)
                        .WhereEqual(row => row.RelatedAlias.RelatedProperty, "Related2")
                        .Select(
                            row => row.FakeDataId,
                            row => row.NormalColumn,
                            row => row.RelatedAlias.RelatedId,
                            row => row.RelatedAlias.RelatedProperty,
                            row => row.OtherAlias.RelatedProperty)
                        .Between(baseline2, boundary2, row => row.FakeDataId)
                        .Union(
                            SqlSelect.From<DataRow>()
                                .WhereEqual(row => row.ValueColumn, match3.ValueColumn)
                                .WhereEqual(row => row.NullableColumn, match3.NullableColumn)
                                .WhereEqual(row => row.NullableValueColumn, match3.NullableValueColumn)
                                .WhereEqual(row => row.RelatedAlias.RelatedProperty, "Related3")
                                .Select(
                                    row => row.FakeDataId,
                                    row => row.NormalColumn,
                                    row => row.RelatedAlias.RelatedId,
                                    row => row.RelatedAlias.RelatedProperty,
                                    row => row.OtherAlias.RelatedProperty)
                                .Between(baseline3, boundary3, row => row.FakeDataId)));

            const string Expected = @"SELECT
    [dbo].[FakeData].[FakeRowId],
    [dbo].[FakeData].[NormalColumn],
    [RelatedAlias].[RelatedId] AS [RelatedAlias.RelatedId],
    [RelatedAlias].[RelatedProperty] AS [RelatedAlias.RelatedProperty],
    [OtherAlias].[RelatedProperty] AS [OtherAlias.RelatedProperty]
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @2 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4
UNION
SELECT
    [dbo].[FakeData].[FakeRowId],
    [dbo].[FakeData].[NormalColumn],
    [RelatedAlias].[RelatedId] AS [RelatedAlias.RelatedId],
    [RelatedAlias].[RelatedProperty] AS [RelatedAlias.RelatedProperty],
    [OtherAlias].[RelatedProperty] AS [OtherAlias.RelatedProperty]
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @5 AND
[dbo].[FakeData].[NullableColumn] LIKE @6 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @7 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @8 AND @9
UNION
SELECT
    [dbo].[FakeData].[FakeRowId],
    [dbo].[FakeData].[NormalColumn],
    [RelatedAlias].[RelatedId] AS [RelatedAlias.RelatedId],
    [RelatedAlias].[RelatedProperty] AS [RelatedAlias.RelatedProperty],
    [OtherAlias].[RelatedProperty] AS [OtherAlias.RelatedProperty]
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @10 AND
[dbo].[FakeData].[NullableColumn] LIKE @11 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @12 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @13 AND @14";

            var target = new TransactSqlQueryFactory(new DataAnnotationsDefinitionProvider());
            var actual = target.Create(transactionSelection.AsSelect());
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement_ union related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Create_ContainsStatementForUnionRelatedDataRaisedRow_MatchesExpected()
        {
            var match1 = new DataRow
            {
                NullableValueColumn = null,
                RelatedAlias = new FakeRelatedRow { RelatedProperty = "Related1" },
                NullableColumn = "CouldHaveBeenNull1",
                ValueColumn = 2
            };

            var baseline1 = new DataRow { FakeDataId = 10 };
            var boundary1 = new DataRow { FakeDataId = 20 };

            var match2 = new DataRow
            {
                NullableValueColumn = null,
                RelatedAlias = new FakeRelatedRow { RelatedProperty = "Related2" },
                NullableColumn = "CouldHaveBeenNull2",
                ValueColumn = 3
            };

            var baseline2 = new DataRow { FakeDataId = 50 };
            var boundary2 = new DataRow { FakeDataId = 40 };

            var match3 = new DataRow
            {
                NullableValueColumn = null,
                RelatedAlias = new FakeRelatedRow { RelatedProperty = "Related3" },
                NullableColumn = "CouldHaveBeenNull3",
                ValueColumn = 4
            };

            var baseline3 = new DataRow { FakeDataId = 60 };
            var boundary3 = new DataRow { FakeDataId = 70 };

            var transactionSelection = SqlSelect.From<DataRow>()
                .WhereEqual(row => row.ValueColumn, match1.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match1.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match1.NullableValueColumn)
                .WhereEqual(row => row.RelatedAlias.RelatedProperty, "Related1")
                .Select(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.RelatedAlias.RelatedId,
                    row => row.RelatedAlias.RelatedProperty,
                    row => row.OtherAlias.RelatedProperty)
                .Between(baseline1, boundary1, row => row.FakeDataId)
                .Union(
                    SqlSelect.From<DataRow>()
                        .WhereEqual(row => row.ValueColumn, match2.ValueColumn)
                        .WhereEqual(row => row.NullableColumn, match2.NullableColumn)
                        .WhereEqual(row => row.NullableValueColumn, match2.NullableValueColumn)
                        .WhereEqual(row => row.RelatedAlias.RelatedProperty, "Related2")
                        .Select(
                            row => row.FakeDataId,
                            row => row.NormalColumn,
                            row => row.RelatedAlias.RelatedId,
                            row => row.RelatedAlias.RelatedProperty,
                            row => row.OtherAlias.RelatedProperty)
                        .Between(baseline2, boundary2, row => row.FakeDataId)
                        .Union(
                            SqlSelect.From<DataRow>()
                                .WhereEqual(row => row.ValueColumn, match3.ValueColumn)
                                .WhereEqual(row => row.NullableColumn, match3.NullableColumn)
                                .WhereEqual(row => row.NullableValueColumn, match3.NullableValueColumn)
                                .WhereEqual(row => row.RelatedAlias.RelatedProperty, "Related3")
                                .Select(
                                    row => row.FakeDataId,
                                    row => row.NormalColumn,
                                    row => row.RelatedAlias.RelatedId,
                                    row => row.RelatedAlias.RelatedProperty,
                                    row => row.OtherAlias.RelatedProperty)
                                .Between(baseline3, boundary3, row => row.FakeDataId)));

            const string Expected = @"IF EXISTS (
SELECT
1
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @2 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4
UNION
SELECT
1
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @5 AND
[dbo].[FakeData].[NullableColumn] LIKE @6 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @7 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @8 AND @9
UNION
SELECT
1
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @10 AND
[dbo].[FakeData].[NullableColumn] LIKE @11 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @12 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @13 AND @14
) SELECT 1  ELSE SELECT 0";

            var target = new TransactSqlQueryFactory(new DataAnnotationsDefinitionProvider());
            var actual = target.Create(transactionSelection.AsContains());
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement_ union related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Create_DeletionStatementForUnionRelatedDataRaisedRow_MatchesExpected()
        {
            var match1 = new DataRow
            {
                NullableValueColumn = null,
                RelatedAlias = new FakeRelatedRow { RelatedProperty = "Related1" },
                NullableColumn = "CouldHaveBeenNull1",
                ValueColumn = 2
            };

            var baseline1 = new DataRow { FakeDataId = 10 };
            var boundary1 = new DataRow { FakeDataId = 20 };

            var match2 = new DataRow
            {
                NullableValueColumn = null,
                RelatedAlias = new FakeRelatedRow { RelatedProperty = "Related2" },
                NullableColumn = "CouldHaveBeenNull2",
                ValueColumn = 3
            };

            var baseline2 = new DataRow { FakeDataId = 50 };
            var boundary2 = new DataRow { FakeDataId = 40 };

            var match3 = new DataRow
            {
                NullableValueColumn = null,
                RelatedAlias = new FakeRelatedRow { RelatedProperty = "Related3" },
                NullableColumn = "CouldHaveBeenNull3",
                ValueColumn = 4
            };

            var baseline3 = new DataRow { FakeDataId = 60 };
            var boundary3 = new DataRow { FakeDataId = 70 };

            var transactionSelection = SqlSelect.From<DataRow>()
                .WhereEqual(row => row.ValueColumn, match1.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match1.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match1.NullableValueColumn)
                .WhereEqual(row => row.RelatedAlias.RelatedProperty, "Related1")
                .Select(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.RelatedAlias.RelatedId,
                    row => row.RelatedAlias.RelatedProperty,
                    row => row.OtherAlias.RelatedProperty)
                .Between(baseline1, boundary1, row => row.FakeDataId)
                .Union(
                    SqlSelect.From<DataRow>()
                        .WhereEqual(row => row.ValueColumn, match2.ValueColumn)
                        .WhereEqual(row => row.NullableColumn, match2.NullableColumn)
                        .WhereEqual(row => row.NullableValueColumn, match2.NullableValueColumn)
                        .WhereEqual(row => row.RelatedAlias.RelatedProperty, "Related2")
                        .Select(
                            row => row.FakeDataId,
                            row => row.NormalColumn,
                            row => row.RelatedAlias.RelatedId,
                            row => row.RelatedAlias.RelatedProperty,
                            row => row.OtherAlias.RelatedProperty)
                        .Between(baseline2, boundary2, row => row.FakeDataId)
                        .Union(
                            SqlSelect.From<DataRow>()
                                .WhereEqual(row => row.ValueColumn, match3.ValueColumn)
                                .WhereEqual(row => row.NullableColumn, match3.NullableColumn)
                                .WhereEqual(row => row.NullableValueColumn, match3.NullableValueColumn)
                                .WhereEqual(row => row.RelatedAlias.RelatedProperty, "Related3")
                                .Select(
                                    row => row.FakeDataId,
                                    row => row.NormalColumn,
                                    row => row.RelatedAlias.RelatedId,
                                    row => row.RelatedAlias.RelatedProperty,
                                    row => row.OtherAlias.RelatedProperty)
                                .Between(baseline3, boundary3, row => row.FakeDataId)));

            const string Expected = @"DELETE [dbo].[FakeData]
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @2 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4";

            var target = new TransactSqlQueryFactory(new DataAnnotationsDefinitionProvider());
            var actual = target.Create(transactionSelection.AsDelete());
            Assert.AreEqual(Expected, actual);
        }

        #endregion
    }
}