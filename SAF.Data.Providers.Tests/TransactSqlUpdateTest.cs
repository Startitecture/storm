// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactSqlUpdateTest.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.Data.Providers.Tests
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Orm.Repository.Tests.Models;
    using Startitecture.Orm.Sql;

    /// <summary>
    /// The update operation test.
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TransactSqlUpdateTest
    {
        #region Public Methods and Operators

        /// <summary>
        /// The update statement_ direct data_ matches expected.
        /// </summary>
        [TestMethod]
        public void UpdateStatement_DirectData_MatchesExpected()
        {
            var match = new FakeFlatDataRow
                            {
                                NormalColumn = "NormalColumn",
                                NullableColumn = "CouldHaveBeenNull",
                                ValueColumn = 2,
                                NullableValueColumn = null,
                                AnotherValueColumn = 12,
                                AnotherColumn = "Some Other Value"
                            };

            var baseline = new FakeFlatDataRow { FakeDataId = 10 };
            var boundary = new FakeFlatDataRow { FakeDataId = 20 };
            var selection =
                match.ToExampleSelection(
                        row => row.ValueColumn,
                        row => row.NullableColumn,
                        row => row.NullableValueColumn)
                    .Select(
                        row => row.FakeDataId,
                        row => row.NormalColumn,
                        row => row.NullableColumn,
                        row => row.NullableValueColumn,
                        row => row.ValueColumn)
                    .Between(baseline, boundary, row => row.FakeDataId);

            var updateOperation = new SqlUpdate<FakeFlatDataRow>(selection).Set(match);

            Stopwatch watch = Stopwatch.StartNew();
            var statement = updateOperation.ExecutionStatement;
            watch.Stop();
            Trace.TraceInformation("Statement compiled in {0}.", watch.Elapsed);

            watch.Restart();
            var statement2 = updateOperation.ExecutionStatement;
            watch.Stop();
            Trace.TraceInformation("Second compilation in {0}.", watch.Elapsed);

            var expected = new object[]
                               {
                                   "NormalColumn",
                                   "CouldHaveBeenNull",
                                   2,
                                   12, 
                                   "Some Other Value",
                                   2, 
                                   "CouldHaveBeenNull",
                                   10,
                                   20
                               };

            var actual = updateOperation.ExecutionParameters.ToArray();

            CollectionAssert.AreEqual(
                expected,
                actual,
                "Expected: {0}{1}Actual: {2}",
                string.Join(",", expected),
                Environment.NewLine,
                string.Join(",", actual));

            const string ExpectedSelection = @"UPDATE [dbo].[FakeData]
SET
[dbo].[FakeData].[NormalColumn] = @0,
[dbo].[FakeData].[NullableColumn] = @1,
[dbo].[FakeData].[ValueColumn] = @2,
[dbo].[FakeData].[AnotherValueColumn] = @3,
[dbo].[FakeData].[AnotherColumn] = @4,
[dbo].[FakeData].[NullableValueColumn] = NULL
FROM [dbo].[FakeData]
INNER JOIN [someschema].[FakeRelated] ON [dbo].[FakeData].[FakeRowId] = [someschema].[FakeRelated].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] ON [someschema].[FakeRelated].[RelatedId] = [dbo].[FakeDependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[FakeSubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[FakeSubData].[FakeSubDataId]
WHERE
[dbo].[FakeData].[ValueColumn] = @5 AND
[dbo].[FakeData].[NullableColumn] LIKE @6 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[dbo].[FakeData].[FakeRowId] BETWEEN @7 AND @8";

            Assert.AreEqual(ExpectedSelection, statement);
        }

        /// <summary>
        /// The update statement_ direct data_ matches expected.
        /// </summary>
        [TestMethod]
        public void UpdateStatement_RaisedDirectData_MatchesExpected()
        {
            var match = new FakeRaisedDataRow
            {
                NormalColumn = "NormalColumn",
                NullableColumn = "CouldHaveBeenNull",
                ValueColumn = 2,
                NullableValueColumn = null,
                AnotherValueColumn = 12,
                AnotherColumn = "Some Other Value"
            };

            var baseline = new FakeFlatDataRow { FakeDataId = 10 };
            var boundary = new FakeFlatDataRow { FakeDataId = 20 };
            var selection =
                match.ToExampleSelection(
                        row => row.ValueColumn,
                        row => row.NullableColumn,
                        row => row.NullableValueColumn)
                    .Select(
                        row => row.FakeDataId,
                        row => row.NormalColumn,
                        row => row.NullableColumn,
                        row => row.NullableValueColumn,
                        row => row.ValueColumn)
                    .Between(baseline, boundary, row => row.FakeDataId);

            var updateOperation = new SqlUpdate<FakeRaisedDataRow>(selection).Set(match);

            Stopwatch watch = Stopwatch.StartNew();
            var statement = updateOperation.ExecutionStatement;
            watch.Stop();
            Trace.TraceInformation("Statement compiled in {0}.", watch.Elapsed);

            watch.Restart();
            var statement2 = updateOperation.ExecutionStatement;
            watch.Stop();
            Trace.TraceInformation("Second compilation in {0}.", watch.Elapsed);

            var expected = new object[]
                               {
                                   "NormalColumn",
                                   "CouldHaveBeenNull",
                                   2,
                                   12,
                                   "Some Other Value",
                                   2,
                                   "CouldHaveBeenNull",
                                   10,
                                   20
                               };

            var actual = updateOperation.ExecutionParameters.ToArray();

            CollectionAssert.AreEqual(
                expected,
                actual,
                "Expected: {0}{1}Actual: {2}",
                string.Join(",", expected),
                Environment.NewLine,
                string.Join(",", actual));

            const string ExpectedSelection = @"UPDATE [dbo].[FakeData]
SET
[dbo].[FakeData].[NormalColumn] = @0,
[dbo].[FakeData].[NullableColumn] = @1,
[dbo].[FakeData].[ValueColumn] = @2,
[dbo].[FakeData].[AnotherValueColumn] = @3,
[dbo].[FakeData].[AnotherColumn] = @4,
[dbo].[FakeData].[NullableValueColumn] = NULL
FROM [dbo].[FakeData]
INNER JOIN [someschema].[FakeRelated] ON [dbo].[FakeData].[FakeRowId] = [someschema].[FakeRelated].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] ON [someschema].[FakeRelated].[RelatedId] = [dbo].[FakeDependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[FakeSubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[FakeSubData].[FakeSubDataId]
WHERE
[dbo].[FakeData].[ValueColumn] = @5 AND
[dbo].[FakeData].[NullableColumn] LIKE @6 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[dbo].[FakeData].[FakeRowId] BETWEEN @7 AND @8";

            Assert.AreEqual(ExpectedSelection, statement);
        }

        /// <summary>
        /// The update statement_ related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void UpdateStatement_RelatedData_MatchesExpected()
        {
            var match = new FakeFlatDataRow
            {
                NormalColumn = "NormalColumn",
                RelatedAliasRelatedProperty = "Related",
                NullableColumn = "CouldHaveBeenNull",
                ValueColumn = 2,
                NullableValueColumn = null,
                AnotherValueColumn = 12,
                AnotherColumn = "Some Other Value"
            };

            var baseline = new FakeFlatDataRow { FakeDataId = 10 };
            var boundary = new FakeFlatDataRow { FakeDataId = 20 };
            var selection =
                match.ToExampleSelection(
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
                    .Between(baseline, boundary, row => row.FakeDataId);

            var updateOperation = new SqlUpdate<FakeFlatDataRow>(selection).Set(match);
            var expected = new object[] { "NormalColumn", "CouldHaveBeenNull", 2, 12, "Some Other Value", 2, "CouldHaveBeenNull", "Related", 10, 20 };
            var actual = updateOperation.ExecutionParameters.ToArray();

            CollectionAssert.AreEqual(
                expected,
                actual,
                "Expected: {0}{1}Actual: {2}",
                string.Join(",", expected),
                Environment.NewLine,
                string.Join(",", actual));

            const string ExpectedSelection = @"UPDATE [dbo].[FakeData]
SET
[dbo].[FakeData].[NormalColumn] = @0,
[dbo].[FakeData].[NullableColumn] = @1,
[dbo].[FakeData].[ValueColumn] = @2,
[dbo].[FakeData].[AnotherValueColumn] = @3,
[dbo].[FakeData].[AnotherColumn] = @4,
[dbo].[FakeData].[NullableValueColumn] = NULL
FROM [dbo].[FakeData]
INNER JOIN [someschema].[FakeRelated] ON [dbo].[FakeData].[FakeRowId] = [someschema].[FakeRelated].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] ON [someschema].[FakeRelated].[RelatedId] = [dbo].[FakeDependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[FakeSubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[FakeSubData].[FakeSubDataId]
WHERE
[dbo].[FakeData].[ValueColumn] = @5 AND
[dbo].[FakeData].[NullableColumn] LIKE @6 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @7 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @8 AND @9";

            Stopwatch watch = Stopwatch.StartNew();
            var statement = updateOperation.ExecutionStatement;
            watch.Stop();
            Trace.TraceInformation("Statement compiled in {0}.", watch.Elapsed);

            watch.Restart();
            var statement2 = updateOperation.ExecutionStatement;
            watch.Stop();
            Trace.TraceInformation("Second compilation in {0}.", watch.Elapsed);
        
            Assert.AreEqual(ExpectedSelection, statement);
        }

        /// <summary>
        /// The update statement_ related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void UpdateStatement_RaisedRelatedData_MatchesExpected()
        {
            var match = new FakeRaisedDataRow
            {
                NormalColumn = "NormalColumn",
                RelatedAlias = new FakeRelatedRow { RelatedProperty = "Related" },
                NullableColumn = "CouldHaveBeenNull",
                ValueColumn = 2,
                NullableValueColumn = null,
                AnotherValueColumn = 12,
                AnotherColumn = "Some Other Value"
            };

            var baseline = new FakeRaisedDataRow { FakeDataId = 10 };
            var boundary = new FakeRaisedDataRow { FakeDataId = 20 };
            var selection =
                match.ToExampleSelection(row => row.ValueColumn, row => row.NullableColumn, row => row.NullableValueColumn)
                    .Matching(row => row.RelatedAlias.RelatedProperty, "Related")
                    .Select(
                        row => row.FakeDataId,
                        row => row.NormalColumn,
                        row => row.RelatedAlias.RelatedId,
                        row => row.RelatedAlias.RelatedProperty,
                        row => row.OtherAlias.RelatedProperty)
                    .Between(baseline, boundary, row => row.FakeDataId);

            var updateOperation = new SqlUpdate<FakeRaisedDataRow>(selection).Set(match);
            var expected = new object[] { "NormalColumn", "CouldHaveBeenNull", 2, 12, "Some Other Value", 2, "CouldHaveBeenNull", "Related", 10, 20 };
            var actual = updateOperation.ExecutionParameters.ToArray();

            CollectionAssert.AreEqual(
                expected,
                actual,
                "Expected: {0}{1}Actual: {2}",
                string.Join(",", expected),
                Environment.NewLine,
                string.Join(",", actual));

            const string ExpectedSelection = @"UPDATE [dbo].[FakeData]
SET
[dbo].[FakeData].[NormalColumn] = @0,
[dbo].[FakeData].[NullableColumn] = @1,
[dbo].[FakeData].[ValueColumn] = @2,
[dbo].[FakeData].[AnotherValueColumn] = @3,
[dbo].[FakeData].[AnotherColumn] = @4,
[dbo].[FakeData].[NullableValueColumn] = NULL
FROM [dbo].[FakeData]
INNER JOIN [someschema].[FakeRelated] ON [dbo].[FakeData].[FakeRowId] = [someschema].[FakeRelated].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] ON [someschema].[FakeRelated].[RelatedId] = [dbo].[FakeDependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[FakeSubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[FakeSubData].[FakeSubDataId]
WHERE
[dbo].[FakeData].[ValueColumn] = @5 AND
[dbo].[FakeData].[NullableColumn] LIKE @6 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @7 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @8 AND @9";

            Stopwatch watch = Stopwatch.StartNew();
            var statement = updateOperation.ExecutionStatement;
            watch.Stop();
            Trace.TraceInformation("Statement compiled in {0}.", watch.Elapsed);

            watch.Restart();
            var statement2 = updateOperation.ExecutionStatement;
            watch.Stop();
            Trace.TraceInformation("Second compilation in {0}.", watch.Elapsed);

            Assert.AreEqual(ExpectedSelection, statement);
        }

        /// <summary>
        /// The update statement_ direct data_ matches expected.
        /// </summary>
        [TestMethod]
        public void UpdateStatement_DirectDataSpecificSetValues_MatchesExpected()
        {
            var match = new FakeFlatDataRow
            {
                NormalColumn = "NormalColumn",
                NullableColumn = "CouldHaveBeenNull",
                ValueColumn = 2,
                NullableValueColumn = null,
            };

            var baseline = new FakeFlatDataRow { FakeDataId = 10 };
            var boundary = new FakeFlatDataRow { FakeDataId = 20 };
            var selection =
                match.ToExampleSelection(
                        row => row.ValueColumn,
                        row => row.NullableColumn,
                        row => row.NullableValueColumn)
                    .Select(
                        row => row.FakeDataId,
                        row => row.NormalColumn,
                        row => row.NullableColumn,
                        row => row.NullableValueColumn,
                        row => row.ValueColumn)
                    .Between(baseline, boundary, row => row.FakeDataId);

            var target = new FakeFlatDataRow { NormalColumn = "UpdatedNormalColumn", NullableColumn = null };
            var updateOperation = new SqlUpdate<FakeFlatDataRow>(selection).Set(
                target,
                row => row.NormalColumn,
                row => row.NullableColumn);

            var expected = new object[] { "UpdatedNormalColumn", 2, "CouldHaveBeenNull", 10, 20 };
            var actual = updateOperation.ExecutionParameters.ToArray();

            CollectionAssert.AreEqual(expected, actual);
            const string ExpectedSelection = @"UPDATE [dbo].[FakeData]
SET
[dbo].[FakeData].[NormalColumn] = @0,
[dbo].[FakeData].[NullableColumn] = NULL
FROM [dbo].[FakeData]
INNER JOIN [someschema].[FakeRelated] ON [dbo].[FakeData].[FakeRowId] = [someschema].[FakeRelated].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] ON [someschema].[FakeRelated].[RelatedId] = [dbo].[FakeDependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[FakeSubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[FakeSubData].[FakeSubDataId]
WHERE
[dbo].[FakeData].[ValueColumn] = @1 AND
[dbo].[FakeData].[NullableColumn] LIKE @2 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4";

            Stopwatch watch = Stopwatch.StartNew();
            var statement = updateOperation.ExecutionStatement;
            watch.Stop();
            Trace.TraceInformation("Statement compiled in {0}.", watch.Elapsed);

            watch.Restart();
            var statement2 = updateOperation.ExecutionStatement;
            watch.Stop();
            Trace.TraceInformation("Second compilation in {0}.", watch.Elapsed);

            Assert.AreEqual(ExpectedSelection, statement);
        }

        /// <summary>
        /// The update statement_ direct data_ matches expected.
        /// </summary>
        [TestMethod]
        public void UpdateStatement_RaisedDirectDataSpecificSetValues_MatchesExpected()
        {
            var match = new FakeRaisedDataRow
            {
                NormalColumn = "NormalColumn",
                NullableColumn = "CouldHaveBeenNull",
                ValueColumn = 2,
                NullableValueColumn = null,
            };

            var baseline = new FakeRaisedDataRow { FakeDataId = 10 };
            var boundary = new FakeRaisedDataRow { FakeDataId = 20 };
            var selection =
                match.ToExampleSelection(
                        row => row.ValueColumn,
                        row => row.NullableColumn,
                        row => row.NullableValueColumn)
                    .Select(
                        row => row.FakeDataId,
                        row => row.NormalColumn,
                        row => row.NullableColumn,
                        row => row.NullableValueColumn,
                        row => row.ValueColumn)
                    .Between(baseline, boundary, row => row.FakeDataId);

            var target = new FakeRaisedDataRow { NormalColumn = "UpdatedNormalColumn", NullableColumn = null };
            var updateOperation = new SqlUpdate<FakeRaisedDataRow>(selection).Set(
                target,
                row => row.NormalColumn,
                row => row.NullableColumn);

            var expected = new object[] { "UpdatedNormalColumn", 2, "CouldHaveBeenNull", 10, 20 };
            var actual = updateOperation.ExecutionParameters.ToArray();

            CollectionAssert.AreEqual(expected, actual);
            const string ExpectedSelection = @"UPDATE [dbo].[FakeData]
SET
[dbo].[FakeData].[NormalColumn] = @0,
[dbo].[FakeData].[NullableColumn] = NULL
FROM [dbo].[FakeData]
INNER JOIN [someschema].[FakeRelated] ON [dbo].[FakeData].[FakeRowId] = [someschema].[FakeRelated].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] ON [someschema].[FakeRelated].[RelatedId] = [dbo].[FakeDependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[FakeSubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[FakeSubData].[FakeSubDataId]
WHERE
[dbo].[FakeData].[ValueColumn] = @1 AND
[dbo].[FakeData].[NullableColumn] LIKE @2 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4";

            var watch = Stopwatch.StartNew();
            var statement = updateOperation.ExecutionStatement;
            watch.Stop();
            Trace.TraceInformation("Statement compiled in {0}.", watch.Elapsed);

            watch.Restart();
            var statement2 = updateOperation.ExecutionStatement;
            watch.Stop();
            Trace.TraceInformation("Second compilation in {0}.", watch.Elapsed);

            Assert.AreEqual(ExpectedSelection, statement);
        }

        /// <summary>
        /// The update statement_ related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void UpdateStatement_RelatedDataSpecificSetValues_MatchesExpected()
        {
            var match = new FakeFlatDataRow
            {
                NormalColumn = "NormalColumn",
                RelatedAliasRelatedProperty = "Related",
                NullableColumn = "CouldHaveBeenNull",
                ValueColumn = 2,
                NullableValueColumn = null,
            };

            var baseline = new FakeFlatDataRow { FakeDataId = 10 };
            var boundary = new FakeFlatDataRow { FakeDataId = 20 };
            var selection =
                match.ToExampleSelection(
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
                    .Between(baseline, boundary, row => row.FakeDataId);

            var target = new FakeFlatDataRow { NormalColumn = "UpdatedNormalColumn", NullableColumn = null };
            var updateOperation = new SqlUpdate<FakeFlatDataRow>(selection).Set(
                target,
                row => row.NormalColumn,
                row => row.NullableColumn);

            var expected = new object[] { "UpdatedNormalColumn", 2, "CouldHaveBeenNull", "Related", 10, 20 };
            var actual = updateOperation.ExecutionParameters.ToArray();

            CollectionAssert.AreEqual(expected, actual);
            const string ExpectedSelection = @"UPDATE [dbo].[FakeData]
SET
[dbo].[FakeData].[NormalColumn] = @0,
[dbo].[FakeData].[NullableColumn] = NULL
FROM [dbo].[FakeData]
INNER JOIN [someschema].[FakeRelated] ON [dbo].[FakeData].[FakeRowId] = [someschema].[FakeRelated].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] ON [someschema].[FakeRelated].[RelatedId] = [dbo].[FakeDependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[FakeSubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[FakeSubData].[FakeSubDataId]
WHERE
[dbo].[FakeData].[ValueColumn] = @1 AND
[dbo].[FakeData].[NullableColumn] LIKE @2 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @3 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @4 AND @5";

            Stopwatch watch = Stopwatch.StartNew();
            var statement = updateOperation.ExecutionStatement;
            watch.Stop();
            Trace.TraceInformation("Statement compiled in {0}.", watch.Elapsed);

            watch.Restart();
            var statement2 = updateOperation.ExecutionStatement;
            watch.Stop();
            Trace.TraceInformation("Second compilation in {0}.", watch.Elapsed);

            Assert.AreEqual(ExpectedSelection, statement);
        }

        /// <summary>
        /// The update statement_ related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void UpdateStatement_RaisedRelatedDataSpecificSetValues_MatchesExpected()
        {
            var match = new FakeRaisedDataRow
            {
                NormalColumn = "NormalColumn",
                RelatedAlias = new FakeRelatedRow { RelatedProperty = "Related" },
                NullableColumn = "CouldHaveBeenNull",
                ValueColumn = 2,
                NullableValueColumn = null,
            };

            var baseline = new FakeRaisedDataRow { FakeDataId = 10 };
            var boundary = new FakeRaisedDataRow { FakeDataId = 20 };
            var selection =
                match.ToExampleSelection(row => row.ValueColumn, row => row.NullableColumn, row => row.NullableValueColumn)
                    .Matching(row => row.RelatedAlias.RelatedProperty, "Related")
                    .Select(
                        row => row.FakeDataId,
                        row => row.NormalColumn,
                        row => row.RelatedAlias.RelatedId,
                        row => row.RelatedAlias.RelatedProperty,
                        row => row.OtherAlias.RelatedProperty)
                    .Between(baseline, boundary, row => row.FakeDataId);

            var target = new FakeRaisedDataRow { NormalColumn = "UpdatedNormalColumn", NullableColumn = null };
            var updateOperation = new SqlUpdate<FakeRaisedDataRow>(selection).Set(
                target,
                row => row.NormalColumn,
                row => row.NullableColumn);

            var expected = new object[] { "UpdatedNormalColumn", 2, "CouldHaveBeenNull", "Related", 10, 20 };
            var actual = updateOperation.ExecutionParameters.ToArray();

            CollectionAssert.AreEqual(expected, actual);
            const string ExpectedSelection = @"UPDATE [dbo].[FakeData]
SET
[dbo].[FakeData].[NormalColumn] = @0,
[dbo].[FakeData].[NullableColumn] = NULL
FROM [dbo].[FakeData]
INNER JOIN [someschema].[FakeRelated] ON [dbo].[FakeData].[FakeRowId] = [someschema].[FakeRelated].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] ON [someschema].[FakeRelated].[RelatedId] = [dbo].[FakeDependencyEntity].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[FakeDependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[FakeComplexEntityId]
INNER JOIN [someschema].[FakeRelated] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[FakeSubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[FakeSubData].[FakeSubDataId]
WHERE
[dbo].[FakeData].[ValueColumn] = @1 AND
[dbo].[FakeData].[NullableColumn] LIKE @2 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @3 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @4 AND @5";

            Stopwatch watch = Stopwatch.StartNew();
            var statement = updateOperation.ExecutionStatement;
            watch.Stop();
            Trace.TraceInformation("Statement compiled in {0}.", watch.Elapsed);

            watch.Restart();
            var statement2 = updateOperation.ExecutionStatement;
            watch.Stop();
            Trace.TraceInformation("Second compilation in {0}.", watch.Elapsed);

            Assert.AreEqual(ExpectedSelection, statement);
        }

        #endregion
    }
}