// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactSqlSelectionTest.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Sql.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Orm.Testing.Model;

    /// <summary>
    /// The example selection test.
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TransactSqlSelectionTest
    {
        #region Public Methods and Operators

        /// <summary>
        /// The selection statement_ direct data_ matches expected.
        /// </summary>
        [TestMethod]
        public void SelectionStatement_DirectData_MatchesExpected()
        {
            var match = new FakeFlatDataRow { ValueColumn = 2, NullableColumn = "CouldHaveBeenNull", NullableValueColumn = null };
            var baseline = new FakeFlatDataRow { FakeDataId = 10, NormalColumn = "Greater" };
            var boundary = new FakeFlatDataRow { FakeDataId = 20, AnotherColumn = "Less" };
            var transactionSelection =
                match.ToExampleSelection(
                        row => row.ValueColumn,
                        row => row.NullableColumn,
                        row => row.NullableValueColumn)
                    .Select(
                        row => row.FakeDataId,
                        row => row.NormalColumn,
                        row => row.NullableColumn,
                        row => row.NullableValueColumn,
                        row => row.ValueColumn,
                        row => row.AnotherColumn,
                        row => row.AnotherValueColumn)
                    .Between(baseline, boundary, row => row.FakeDataId, row => row.NormalColumn, row => row.AnotherColumn)
                    .Include(row => row.AnotherValueColumn, 5, 10, 15, 20);

            const string ExpectedSelection = @"SELECT
    [dbo].[FakeData].[FakeRowId],
    [dbo].[FakeData].[NormalColumn],
    [dbo].[FakeData].[NullableColumn],
    [dbo].[FakeData].[NullableValueColumn],
    [dbo].[FakeData].[ValueColumn],
    [dbo].[FakeData].[AnotherColumn],
    [dbo].[FakeData].[AnotherValueColumn]
FROM [dbo].[FakeData]
INNER JOIN [someschema].[FakeRelated] ON [dbo].[FakeData].[FakeRowId] = [someschema].[FakeRelated].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] ON [someschema].[FakeRelated].[RelatedId] = [dbo].[FakeDependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[FakeSubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[FakeSubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[dbo].[FakeData].[FakeRowId] BETWEEN @2 AND @3 AND
[dbo].[FakeData].[NormalColumn] >= @4 AND
[dbo].[FakeData].[AnotherColumn] <= @5 AND
[dbo].[FakeData].[AnotherValueColumn] IN (@6, @7, @8, @9)";

            const string ExpectedContains = @"IF EXISTS (
SELECT
1
FROM [dbo].[FakeData]
INNER JOIN [someschema].[FakeRelated] ON [dbo].[FakeData].[FakeRowId] = [someschema].[FakeRelated].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] ON [someschema].[FakeRelated].[RelatedId] = [dbo].[FakeDependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[FakeSubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[FakeSubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[dbo].[FakeData].[FakeRowId] BETWEEN @2 AND @3 AND
[dbo].[FakeData].[NormalColumn] >= @4 AND
[dbo].[FakeData].[AnotherColumn] <= @5 AND
[dbo].[FakeData].[AnotherValueColumn] IN (@6, @7, @8, @9)
) SELECT 1  ELSE SELECT 0";

            const string ExpectedRemoval = @"DELETE [dbo].[FakeData]
FROM [dbo].[FakeData]
INNER JOIN [someschema].[FakeRelated] ON [dbo].[FakeData].[FakeRowId] = [someschema].[FakeRelated].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] ON [someschema].[FakeRelated].[RelatedId] = [dbo].[FakeDependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[FakeSubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[FakeSubData].[FakeSubDataId]
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
            var actual = Enumerable.ToArray<object>(transactionSelection.PropertyValues);
            CollectionAssert.AreEqual(
                expected,
                actual,
                "Expected: {0}{1}Actual: {2}",
                string.Join(",", expected),
                Environment.NewLine,
                string.Join(",", actual));

            Assert.AreEqual<string>(ExpectedSelection, transactionSelection.SelectionStatement);
            Assert.AreEqual<string>(ExpectedContains, transactionSelection.ContainsStatement);
            Assert.AreEqual<string>(ExpectedRemoval, transactionSelection.RemovalStatement);
        }

        /// <summary>
        /// The selection statement_ direct data_ matches expected.
        /// </summary>
        [TestMethod]
        public void SelectionStatement_DirectDataRaisedRow_MatchesExpected()
        {
            var match = new FakeRaisedDataRow { ValueColumn = 2, NullableColumn = "CouldHaveBeenNull", NullableValueColumn = null };
            var baseline = new FakeRaisedDataRow { FakeDataId = 10, NormalColumn = "Greater" };
            var boundary = new FakeRaisedDataRow { FakeDataId = 20, AnotherColumn = "Less" };
            var transactionSelection =
                match.ToExampleSelection(
                        row => row.ValueColumn,
                        row => row.NullableColumn,
                        row => row.NullableValueColumn)
                    .Select(
                        row => row.FakeDataId,
                        row => row.NormalColumn,
                        row => row.NullableColumn,
                        row => row.NullableValueColumn,
                        row => row.ValueColumn,
                        row => row.AnotherColumn,
                        row => row.AnotherValueColumn)
                    .Between(baseline, boundary, row => row.FakeDataId, row => row.NormalColumn, row => row.AnotherColumn)
                    .Include(row => row.AnotherValueColumn, 5, 10, 15, 20);

            const string ExpectedSelection = @"SELECT
    [dbo].[FakeData].[FakeRowId],
    [dbo].[FakeData].[NormalColumn],
    [dbo].[FakeData].[NullableColumn],
    [dbo].[FakeData].[NullableValueColumn],
    [dbo].[FakeData].[ValueColumn],
    [dbo].[FakeData].[AnotherColumn],
    [dbo].[FakeData].[AnotherValueColumn]
FROM [dbo].[FakeData]
INNER JOIN [someschema].[FakeRelated] ON [dbo].[FakeData].[FakeRowId] = [someschema].[FakeRelated].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] ON [someschema].[FakeRelated].[RelatedId] = [dbo].[FakeDependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[FakeSubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[FakeSubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[dbo].[FakeData].[FakeRowId] BETWEEN @2 AND @3 AND
[dbo].[FakeData].[NormalColumn] >= @4 AND
[dbo].[FakeData].[AnotherColumn] <= @5 AND
[dbo].[FakeData].[AnotherValueColumn] IN (@6, @7, @8, @9)";

            const string ExpectedContains = @"IF EXISTS (
SELECT
1
FROM [dbo].[FakeData]
INNER JOIN [someschema].[FakeRelated] ON [dbo].[FakeData].[FakeRowId] = [someschema].[FakeRelated].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] ON [someschema].[FakeRelated].[RelatedId] = [dbo].[FakeDependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[FakeSubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[FakeSubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[dbo].[FakeData].[FakeRowId] BETWEEN @2 AND @3 AND
[dbo].[FakeData].[NormalColumn] >= @4 AND
[dbo].[FakeData].[AnotherColumn] <= @5 AND
[dbo].[FakeData].[AnotherValueColumn] IN (@6, @7, @8, @9)
) SELECT 1  ELSE SELECT 0";

            const string ExpectedRemoval = @"DELETE [dbo].[FakeData]
FROM [dbo].[FakeData]
INNER JOIN [someschema].[FakeRelated] ON [dbo].[FakeData].[FakeRowId] = [someschema].[FakeRelated].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] ON [someschema].[FakeRelated].[RelatedId] = [dbo].[FakeDependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[FakeSubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[FakeSubData].[FakeSubDataId]
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
            var actual = Enumerable.ToArray<object>(transactionSelection.PropertyValues);
            CollectionAssert.AreEqual(
                expected,
                actual,
                "Expected: {0}{1}Actual: {2}",
                string.Join(",", expected),
                Environment.NewLine,
                string.Join(",", actual));

            Assert.AreEqual<string>(ExpectedSelection, transactionSelection.SelectionStatement);
            Assert.AreEqual<string>(ExpectedContains, transactionSelection.ContainsStatement);
            Assert.AreEqual<string>(ExpectedRemoval, transactionSelection.RemovalStatement);
        }

        /// <summary>
        /// The selection statement_ related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void SelectionStatement_RelatedData_MatchesExpected()
        {
            var match = new FakeFlatDataRow
                            {
                                NullableValueColumn = null,
                                RelatedAliasRelatedProperty = "Related",
                                NullableColumn = "CouldHaveBeenNull",
                                ValueColumn = 2
                            };

            var baseline = new FakeFlatDataRow { FakeDataId = 10 };
            var boundary = new FakeFlatDataRow { FakeDataId = 20 };
            var transactionSelection =
                match.ToExampleSelection(
                    row => row.ValueColumn,
                    row => row.NullableColumn,
                    row => row.NullableValueColumn,
                    row => row.RelatedAliasRelatedProperty)
                    .Select(
                        row => row.FakeDataId,
                        row => row.NormalColumn,
                        row => row.FakeRelatedRelatedId,
                        row => row.FakeRelatedRelatedProperty,
                        row => row.RelatedId,
                        row => row.RelatedAliasRelatedProperty,
                        row => row.OtherAliasRelatedId,
                        row => row.OtherAliasRelatedProperty,
                        row => row.ParentFakeDataId)
                    .Between(baseline, boundary, row => row.FakeDataId);

            const string ExpectedSelection = @"SELECT
    [dbo].[FakeData].[FakeRowId],
    [dbo].[FakeData].[NormalColumn],
    [someschema].[FakeRelated].[RelatedId] AS [FakeRelatedRelatedId],
    [someschema].[FakeRelated].[RelatedProperty] AS [FakeRelatedRelatedProperty],
    [RelatedAlias].[RelatedId],
    [RelatedAlias].[RelatedProperty] AS [RelatedAliasRelatedProperty],
    [OtherAlias].[RelatedId] AS [OtherAliasRelatedId],
    [OtherAlias].[RelatedProperty] AS [OtherAliasRelatedProperty],
    [dbo].[FakeSubData].[ParentFakeDataId]
FROM [dbo].[FakeData]
INNER JOIN [someschema].[FakeRelated] ON [dbo].[FakeData].[FakeRowId] = [someschema].[FakeRelated].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] ON [someschema].[FakeRelated].[RelatedId] = [dbo].[FakeDependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[FakeSubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[FakeSubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @2 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4";

            const string ExpectedContains = @"IF EXISTS (
SELECT
1
FROM [dbo].[FakeData]
INNER JOIN [someschema].[FakeRelated] ON [dbo].[FakeData].[FakeRowId] = [someschema].[FakeRelated].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] ON [someschema].[FakeRelated].[RelatedId] = [dbo].[FakeDependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[FakeSubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[FakeSubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @2 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4
) SELECT 1  ELSE SELECT 0";

            const string ExpectedRemoval = @"DELETE [dbo].[FakeData]
FROM [dbo].[FakeData]
INNER JOIN [someschema].[FakeRelated] ON [dbo].[FakeData].[FakeRowId] = [someschema].[FakeRelated].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] ON [someschema].[FakeRelated].[RelatedId] = [dbo].[FakeDependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[FakeSubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[FakeSubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @2 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4";

            CollectionAssert.AreEqual(
                new object[] { 2, "CouldHaveBeenNull", "Related", 10, 20 },
                Enumerable.ToArray<object>(transactionSelection.PropertyValues));

            Assert.AreEqual<string>(ExpectedSelection, transactionSelection.SelectionStatement);
            Assert.AreEqual<string>(ExpectedContains, transactionSelection.ContainsStatement);
            Assert.AreEqual<string>(ExpectedRemoval, transactionSelection.RemovalStatement);
        }

        /// <summary>
        /// The selection statement_ related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void SelectionStatement_RelatedDataRaisedRow_MatchesExpected()
        {
            var match = new FakeRaisedDataRow
            {
                NullableValueColumn = null,
                RelatedAlias = new FakeRelatedRow { RelatedProperty = "Related" },
                NullableColumn = "CouldHaveBeenNull",
                ValueColumn = 2
            };

            var baseline = new FakeRaisedDataRow { FakeDataId = 10 };
            var boundary = new FakeRaisedDataRow { FakeDataId = 20 };
            var transactionSelection =
                match.ToExampleSelection(row => row.ValueColumn, row => row.NullableColumn, row => row.NullableValueColumn)
                    .Matching(row => row.RelatedAlias.RelatedProperty, "Related")
                    .Select(
                        row => row.FakeDataId,
                        row => row.NormalColumn,
                        row => row.ParentFakeDataId,
                        row => row.FakeRelated.RelatedId,
                        row => row.FakeRelated.RelatedProperty,
                        row => row.RelatedAlias.RelatedId,
                        row => row.RelatedAlias.RelatedProperty,
                        row => row.OtherAlias.RelatedId,
                        row => row.OtherAlias.RelatedProperty)
                    .Between(baseline, boundary, row => row.FakeDataId);

            // TODO: The query builder uses tabs. Replace with spaces and fix all tests.
            const string ExpectedSelection = @"SELECT
    [dbo].[FakeData].[FakeRowId],
    [dbo].[FakeData].[NormalColumn],
    [dbo].[FakeSubData].[ParentFakeDataId],
    [someschema].[FakeRelated].[RelatedId] AS [FakeRelated.RelatedId],
    [someschema].[FakeRelated].[RelatedProperty] AS [FakeRelated.RelatedProperty],
    [RelatedAlias].[RelatedId] AS [RelatedAlias.RelatedId],
    [RelatedAlias].[RelatedProperty] AS [RelatedAlias.RelatedProperty],
    [OtherAlias].[RelatedId] AS [OtherAlias.RelatedId],
    [OtherAlias].[RelatedProperty] AS [OtherAlias.RelatedProperty]
FROM [dbo].[FakeData]
INNER JOIN [someschema].[FakeRelated] ON [dbo].[FakeData].[FakeRowId] = [someschema].[FakeRelated].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] ON [someschema].[FakeRelated].[RelatedId] = [dbo].[FakeDependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[FakeSubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[FakeSubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @2 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4";

            const string ExpectedContains = @"IF EXISTS (
SELECT
1
FROM [dbo].[FakeData]
INNER JOIN [someschema].[FakeRelated] ON [dbo].[FakeData].[FakeRowId] = [someschema].[FakeRelated].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] ON [someschema].[FakeRelated].[RelatedId] = [dbo].[FakeDependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[FakeSubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[FakeSubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @2 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4
) SELECT 1  ELSE SELECT 0";

            const string ExpectedRemoval = @"DELETE [dbo].[FakeData]
FROM [dbo].[FakeData]
INNER JOIN [someschema].[FakeRelated] ON [dbo].[FakeData].[FakeRowId] = [someschema].[FakeRelated].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] ON [someschema].[FakeRelated].[RelatedId] = [dbo].[FakeDependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[FakeSubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[FakeSubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @2 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4";

            CollectionAssert.AreEqual(
                new object[] { 2, "CouldHaveBeenNull", "Related", 10, 20 },
                Enumerable.ToArray<object>(transactionSelection.PropertyValues));

            Assert.AreEqual<string>(ExpectedSelection, transactionSelection.SelectionStatement);
            Assert.AreEqual<string>(ExpectedContains, transactionSelection.ContainsStatement);
            Assert.AreEqual<string>(ExpectedRemoval, transactionSelection.RemovalStatement);
        }

        /// <summary>
        /// The selection statement_ union related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void SelectionStatement_UnionRelatedData_MatchesExpected()
        {
            var match1 = new FakeFlatDataRow
            {
                NullableValueColumn = null,
                RelatedAliasRelatedProperty = "Related1",
                NullableColumn = "CouldHaveBeenNull1",
                ValueColumn = 2
            };

            var baseline1 = new FakeFlatDataRow { FakeDataId = 10 };
            var boundary1 = new FakeFlatDataRow { FakeDataId = 20 };

            var match2 = new FakeFlatDataRow
            {
                NullableValueColumn = null,
                RelatedAliasRelatedProperty = "Related2",
                NullableColumn = "CouldHaveBeenNull2",
                ValueColumn = 3
            };

            var baseline2 = new FakeFlatDataRow { FakeDataId = 50 };
            var boundary2 = new FakeFlatDataRow { FakeDataId = 40 };

            var match3 = new FakeFlatDataRow
            {
                NullableValueColumn = null,
                RelatedAliasRelatedProperty = "Related3",
                NullableColumn = "CouldHaveBeenNull3",
                ValueColumn = 4
            };

            var baseline3 = new FakeFlatDataRow { FakeDataId = 60 };
            var boundary3 = new FakeFlatDataRow { FakeDataId = 70 };
            
            var transactionSelection =
                match1.ToExampleSelection(
                        row => row.ValueColumn,
                        row => row.NullableColumn,
                        row => row.NullableValueColumn,
                        row => row.RelatedAliasRelatedProperty)
                    .Select(
                        row => row.FakeDataId,
                        row => row.NormalColumn,
                        row => row.RelatedId,
                        row => row.RelatedAliasRelatedProperty,
                        row => row.OtherAliasRelatedProperty)
                    .Between(baseline1, boundary1, row => row.FakeDataId)
                    .Union(match2.ToExampleSelection(
                        row => row.ValueColumn,
                        row => row.NullableColumn,
                        row => row.NullableValueColumn,
                        row => row.RelatedAliasRelatedProperty)
                    .Select(
                        row => row.FakeDataId,
                        row => row.NormalColumn,
                        row => row.RelatedId,
                        row => row.RelatedAliasRelatedProperty,
                        row => row.OtherAliasRelatedProperty)
                    .Between(baseline2, boundary2, row => row.FakeDataId)
                    .Union(match3.ToExampleSelection(
                        row => row.ValueColumn,
                        row => row.NullableColumn,
                        row => row.NullableValueColumn,
                        row => row.RelatedAliasRelatedProperty)
                    .Select(
                        row => row.FakeDataId,
                        row => row.NormalColumn,
                        row => row.RelatedId,
                        row => row.RelatedAliasRelatedProperty,
                        row => row.OtherAliasRelatedProperty)
                    .Between(baseline3, boundary3, row => row.FakeDataId)));

            const string ExpectedSelection = @"SELECT
    [dbo].[FakeData].[FakeRowId],
    [dbo].[FakeData].[NormalColumn],
    [RelatedAlias].[RelatedId],
    [RelatedAlias].[RelatedProperty] AS [RelatedAliasRelatedProperty],
    [OtherAlias].[RelatedProperty] AS [OtherAliasRelatedProperty]
FROM [dbo].[FakeData]
INNER JOIN [someschema].[FakeRelated] ON [dbo].[FakeData].[FakeRowId] = [someschema].[FakeRelated].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] ON [someschema].[FakeRelated].[RelatedId] = [dbo].[FakeDependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[FakeSubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[FakeSubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @2 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4
UNION
SELECT
    [dbo].[FakeData].[FakeRowId],
    [dbo].[FakeData].[NormalColumn],
    [RelatedAlias].[RelatedId],
    [RelatedAlias].[RelatedProperty] AS [RelatedAliasRelatedProperty],
    [OtherAlias].[RelatedProperty] AS [OtherAliasRelatedProperty]
FROM [dbo].[FakeData]
INNER JOIN [someschema].[FakeRelated] ON [dbo].[FakeData].[FakeRowId] = [someschema].[FakeRelated].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] ON [someschema].[FakeRelated].[RelatedId] = [dbo].[FakeDependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[FakeSubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[FakeSubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @5 AND
[dbo].[FakeData].[NullableColumn] LIKE @6 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @7 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @8 AND @9
UNION
SELECT
    [dbo].[FakeData].[FakeRowId],
    [dbo].[FakeData].[NormalColumn],
    [RelatedAlias].[RelatedId],
    [RelatedAlias].[RelatedProperty] AS [RelatedAliasRelatedProperty],
    [OtherAlias].[RelatedProperty] AS [OtherAliasRelatedProperty]
FROM [dbo].[FakeData]
INNER JOIN [someschema].[FakeRelated] ON [dbo].[FakeData].[FakeRowId] = [someschema].[FakeRelated].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] ON [someschema].[FakeRelated].[RelatedId] = [dbo].[FakeDependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[FakeSubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[FakeSubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @10 AND
[dbo].[FakeData].[NullableColumn] LIKE @11 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @12 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @13 AND @14";

            const string ExpectedContains = @"IF EXISTS (
SELECT
1
FROM [dbo].[FakeData]
INNER JOIN [someschema].[FakeRelated] ON [dbo].[FakeData].[FakeRowId] = [someschema].[FakeRelated].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] ON [someschema].[FakeRelated].[RelatedId] = [dbo].[FakeDependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[FakeSubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[FakeSubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @2 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4
UNION
SELECT
1
FROM [dbo].[FakeData]
INNER JOIN [someschema].[FakeRelated] ON [dbo].[FakeData].[FakeRowId] = [someschema].[FakeRelated].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] ON [someschema].[FakeRelated].[RelatedId] = [dbo].[FakeDependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[FakeSubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[FakeSubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @5 AND
[dbo].[FakeData].[NullableColumn] LIKE @6 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @7 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @8 AND @9
UNION
SELECT
1
FROM [dbo].[FakeData]
INNER JOIN [someschema].[FakeRelated] ON [dbo].[FakeData].[FakeRowId] = [someschema].[FakeRelated].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] ON [someschema].[FakeRelated].[RelatedId] = [dbo].[FakeDependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[FakeSubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[FakeSubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @10 AND
[dbo].[FakeData].[NullableColumn] LIKE @11 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @12 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @13 AND @14
) SELECT 1  ELSE SELECT 0";

            const string ExpectedRemoval = @"DELETE [dbo].[FakeData]
FROM [dbo].[FakeData]
INNER JOIN [someschema].[FakeRelated] ON [dbo].[FakeData].[FakeRowId] = [someschema].[FakeRelated].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] ON [someschema].[FakeRelated].[RelatedId] = [dbo].[FakeDependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[FakeSubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[FakeSubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @2 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4";

            var expected = new object[]
                               {
                                   2, "CouldHaveBeenNull1", "Related1", 10, 20,
                                   3, "CouldHaveBeenNull2", "Related2", 40, 50,
                                   4, "CouldHaveBeenNull3", "Related3", 60, 70
                               };

            var actual = Enumerable.ToArray<object>(transactionSelection.PropertyValues);
            CollectionAssert.AreEqual(expected, actual);

            Assert.AreEqual<string>(ExpectedSelection, transactionSelection.SelectionStatement);
            Assert.AreEqual<string>(ExpectedContains, transactionSelection.ContainsStatement);
            Assert.AreEqual<string>(ExpectedRemoval, transactionSelection.RemovalStatement);
        }

        /// <summary>
        /// The selection statement_ union related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void SelectionStatement_UnionRelatedDataRaisedRow_MatchesExpected()
        {
            var match1 = new FakeRaisedDataRow
            {
                NullableValueColumn = null,
                RelatedAlias = new FakeRelatedRow { RelatedProperty = "Related1" },
                NullableColumn = "CouldHaveBeenNull1",
                ValueColumn = 2
            };

            var baseline1 = new FakeRaisedDataRow { FakeDataId = 10 };
            var boundary1 = new FakeRaisedDataRow { FakeDataId = 20 };

            var match2 = new FakeRaisedDataRow
            {
                NullableValueColumn = null,
                RelatedAlias = new FakeRelatedRow { RelatedProperty = "Related2" },
                NullableColumn = "CouldHaveBeenNull2",
                ValueColumn = 3
            };

            var baseline2 = new FakeFlatDataRow { FakeDataId = 50 };
            var boundary2 = new FakeFlatDataRow { FakeDataId = 40 };

            var match3 = new FakeRaisedDataRow
            {
                NullableValueColumn = null,
                RelatedAlias = new FakeRelatedRow { RelatedProperty = "Related3" },
                NullableColumn = "CouldHaveBeenNull3",
                ValueColumn = 4
            };

            var baseline3 = new FakeRaisedDataRow { FakeDataId = 60 };
            var boundary3 = new FakeRaisedDataRow { FakeDataId = 70 };

            var transactionSelection =
                match1.ToExampleSelection(row => row.ValueColumn, row => row.NullableColumn, row => row.NullableValueColumn)
                    .Matching(row => row.RelatedAlias.RelatedProperty, "Related1")
                    .Select(
                        row => row.FakeDataId,
                        row => row.NormalColumn,
                        row => row.RelatedAlias.RelatedId,
                        row => row.RelatedAlias.RelatedProperty,
                        row => row.OtherAlias.RelatedProperty)
                    .Between(baseline1, boundary1, row => row.FakeDataId)
                    .Union(
                        match2.ToExampleSelection(row => row.ValueColumn, row => row.NullableColumn, row => row.NullableValueColumn)
                            .Matching(row => row.RelatedAlias.RelatedProperty, "Related2")
                            .Select(
                                row => row.FakeDataId,
                                row => row.NormalColumn,
                                row => row.RelatedAlias.RelatedId,
                                row => row.RelatedAlias.RelatedProperty,
                                row => row.OtherAlias.RelatedProperty)
                            .Between(baseline2, boundary2, row => row.FakeDataId)
                            .Union(
                                match3.ToExampleSelection(row => row.ValueColumn, row => row.NullableColumn, row => row.NullableValueColumn)
                                    .Matching(row => row.RelatedAlias.RelatedProperty, "Related3")
                                    .Select(
                                        row => row.FakeDataId,
                                        row => row.NormalColumn,
                                        row => row.RelatedAlias.RelatedId,
                                        row => row.RelatedAlias.RelatedProperty,
                                        row => row.OtherAlias.RelatedProperty)
                                    .Between(baseline3, boundary3, row => row.FakeDataId)));

            const string ExpectedSelection = @"SELECT
    [dbo].[FakeData].[FakeRowId],
    [dbo].[FakeData].[NormalColumn],
    [RelatedAlias].[RelatedId] AS [RelatedAlias.RelatedId],
    [RelatedAlias].[RelatedProperty] AS [RelatedAlias.RelatedProperty],
    [OtherAlias].[RelatedProperty] AS [OtherAlias.RelatedProperty]
FROM [dbo].[FakeData]
INNER JOIN [someschema].[FakeRelated] ON [dbo].[FakeData].[FakeRowId] = [someschema].[FakeRelated].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] ON [someschema].[FakeRelated].[RelatedId] = [dbo].[FakeDependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[FakeSubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[FakeSubData].[FakeSubDataId]
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
INNER JOIN [someschema].[FakeRelated] ON [dbo].[FakeData].[FakeRowId] = [someschema].[FakeRelated].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] ON [someschema].[FakeRelated].[RelatedId] = [dbo].[FakeDependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[FakeSubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[FakeSubData].[FakeSubDataId]
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
INNER JOIN [someschema].[FakeRelated] ON [dbo].[FakeData].[FakeRowId] = [someschema].[FakeRelated].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] ON [someschema].[FakeRelated].[RelatedId] = [dbo].[FakeDependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[FakeSubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[FakeSubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @10 AND
[dbo].[FakeData].[NullableColumn] LIKE @11 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @12 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @13 AND @14";

            const string ExpectedContains = @"IF EXISTS (
SELECT
1
FROM [dbo].[FakeData]
INNER JOIN [someschema].[FakeRelated] ON [dbo].[FakeData].[FakeRowId] = [someschema].[FakeRelated].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] ON [someschema].[FakeRelated].[RelatedId] = [dbo].[FakeDependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[FakeSubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[FakeSubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @2 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4
UNION
SELECT
1
FROM [dbo].[FakeData]
INNER JOIN [someschema].[FakeRelated] ON [dbo].[FakeData].[FakeRowId] = [someschema].[FakeRelated].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] ON [someschema].[FakeRelated].[RelatedId] = [dbo].[FakeDependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[FakeSubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[FakeSubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @5 AND
[dbo].[FakeData].[NullableColumn] LIKE @6 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @7 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @8 AND @9
UNION
SELECT
1
FROM [dbo].[FakeData]
INNER JOIN [someschema].[FakeRelated] ON [dbo].[FakeData].[FakeRowId] = [someschema].[FakeRelated].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] ON [someschema].[FakeRelated].[RelatedId] = [dbo].[FakeDependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[FakeSubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[FakeSubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @10 AND
[dbo].[FakeData].[NullableColumn] LIKE @11 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @12 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @13 AND @14
) SELECT 1  ELSE SELECT 0";

            const string ExpectedRemoval = @"DELETE [dbo].[FakeData]
FROM [dbo].[FakeData]
INNER JOIN [someschema].[FakeRelated] ON [dbo].[FakeData].[FakeRowId] = [someschema].[FakeRelated].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] ON [someschema].[FakeRelated].[RelatedId] = [dbo].[FakeDependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[FakeSubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[FakeSubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @2 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4";

            var expected = new object[]
                               {
                                   2, "CouldHaveBeenNull1", "Related1", 10, 20,
                                   3, "CouldHaveBeenNull2", "Related2", 40, 50,
                                   4, "CouldHaveBeenNull3", "Related3", 60, 70
                               };

            var actual = Enumerable.ToArray<object>(transactionSelection.PropertyValues);
            CollectionAssert.AreEqual(expected, actual);

            Assert.AreEqual<string>(ExpectedSelection, transactionSelection.SelectionStatement);
            Assert.AreEqual<string>(ExpectedContains, transactionSelection.ContainsStatement);
            Assert.AreEqual<string>(ExpectedRemoval, transactionSelection.RemovalStatement);
        }

        #endregion
    }
}