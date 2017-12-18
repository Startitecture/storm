// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateOperationTest.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The update operation test.
    /// </summary>
    [TestClass]
    public class UpdateOperationTest
    {
        #region Public Methods and Operators

        /// <summary>
        /// The update statement_ direct data_ matches expected.
        /// </summary>
        [TestMethod]
        public void UpdateStatement_DirectData_MatchesExpected()
        {
            var match = new FakeDataRow
                            {
                                NormalColumn = "NormalColumn",
                                NullableColumn = "CouldHaveBeenNull",
                                ValueColumn = 2,
                                NullableValueColumn = null,
                                AnotherValueColumn = 12,
                                AnotherColumn = "Some Other Value"
                            };

            var baseline = new FakeDataRow { Id = 10 };
            var boundary = new FakeDataRow { Id = 20 };
            var selection =
                match.ToExampleSelection(
                        row => row.ValueColumn,
                        row => row.NullableColumn,
                        row => row.NullableValueColumn)
                    .Select(
                        row => row.Id,
                        row => row.NormalColumn,
                        row => row.NullableColumn,
                        row => row.NullableValueColumn,
                        row => row.ValueColumn)
                    .Between(baseline, boundary, row => row.Id);

            var updateOperation = new UpdateOperation<FakeDataRow>(selection).Set(match);

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
                String.Join(",", expected),
                Environment.NewLine,
                String.Join(",", actual));

            const string ExpectedSelection = @"UPDATE FakeData
SET
[FakeData].[NormalColumn] = @0,
[FakeData].[NullableColumn] = @1,
[FakeData].[ValueColumn] = @2,
[FakeData].[AnotherValueColumn] = @3,
[FakeData].[AnotherColumn] = @4,
[FakeData].[NullableValueColumn] = NULL
WHERE
[FakeData].[ValueColumn] = @5 AND
[FakeData].[NullableColumn] LIKE @6 AND
[FakeData].[NullableValueColumn] IS NOT NULL AND
[FakeData].[FakeRowId] BETWEEN @7 AND @8";

            Assert.AreEqual(ExpectedSelection, statement);
        }

        /// <summary>
        /// The update statement_ related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void UpdateStatement_RelatedData_MatchesExpected()
        {
            var match = new FakeDataRow
            {
                NormalColumn = "NormalColumn",
                RelatedAliasRelatedProperty = "Related",
                NullableColumn = "CouldHaveBeenNull",
                ValueColumn = 2,
                NullableValueColumn = null,
                AnotherValueColumn = 12,
                AnotherColumn = "Some Other Value"
            };

            var baseline = new FakeDataRow { Id = 10 };
            var boundary = new FakeDataRow { Id = 20 };
            var selection =
                match.ToExampleSelection(
                        row => row.ValueColumn,
                        row => row.NullableColumn,
                        row => row.NullableValueColumn,
                        row => row.RelatedAliasRelatedProperty)
                    .Select(
                        row => row.Id,
                        row => row.NormalColumn,
                        row => row.RelatedId,
                        row => row.ResultColumn,
                        row => row.RelatedAliasRelatedProperty,
                        row => row.OtherAliasRelatedProperty)
                    .Between(baseline, boundary, row => row.Id)
                    .InnerJoin<FakeDataRow, FakeRelatedRow>(row => row.Id, row => row.FakeDataId, "OtherAlias")
                    .InnerJoin<FakeDataRow, FakeRelatedRow>(row => row.Id, row => row.FakeDataId, "RelatedAlias");

            var updateOperation = new UpdateOperation<FakeDataRow>(selection).Set(match);
            var expected = new object[] { "NormalColumn", "CouldHaveBeenNull", 2, 12, "Some Other Value", 2, "CouldHaveBeenNull", "Related", 10, 20 };
            var actual = updateOperation.ExecutionParameters.ToArray();

            CollectionAssert.AreEqual(
                expected,
                actual,
                "Expected: {0}{1}Actual: {2}",
                String.Join(",", expected),
                Environment.NewLine,
                String.Join(",", actual));

            const string ExpectedSelection = @"UPDATE FakeData
SET
[FakeData].[NormalColumn] = @0,
[FakeData].[NullableColumn] = @1,
[FakeData].[ValueColumn] = @2,
[FakeData].[AnotherValueColumn] = @3,
[FakeData].[AnotherColumn] = @4,
[FakeData].[NullableValueColumn] = NULL
FROM FakeData
INNER JOIN [FakeRelated] AS [OtherAlias] ON [FakeData].[Id] = [OtherAlias].[FakeDataId]
INNER JOIN [FakeRelated] AS [RelatedAlias] ON [FakeData].[Id] = [RelatedAlias].[FakeDataId]
WHERE
[FakeData].[ValueColumn] = @5 AND
[FakeData].[NullableColumn] LIKE @6 AND
[FakeData].[NullableValueColumn] IS NOT NULL AND
[RelatedAlias].[RelatedProperty] LIKE @7 AND
[FakeData].[FakeRowId] BETWEEN @8 AND @9";

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
            var match = new FakeDataRow
            {
                NormalColumn = "NormalColumn",
                NullableColumn = "CouldHaveBeenNull",
                ValueColumn = 2,
                NullableValueColumn = null,
            };

            var baseline = new FakeDataRow { Id = 10 };
            var boundary = new FakeDataRow { Id = 20 };
            var selection =
                match.ToExampleSelection(
                        row => row.ValueColumn,
                        row => row.NullableColumn,
                        row => row.NullableValueColumn)
                    .Select(
                        row => row.Id,
                        row => row.NormalColumn,
                        row => row.NullableColumn,
                        row => row.NullableValueColumn,
                        row => row.ValueColumn)
                    .Between(baseline, boundary, row => row.Id);

            var target = new FakeDataRow { NormalColumn = "UpdatedNormalColumn", NullableColumn = null };
            var updateOperation = new UpdateOperation<FakeDataRow>(selection).Set(
                target,
                row => row.NormalColumn,
                row => row.NullableColumn);

            var expected = new object[] { "UpdatedNormalColumn", 2, "CouldHaveBeenNull", 10, 20 };
            var actual = updateOperation.ExecutionParameters.ToArray();

            CollectionAssert.AreEqual(expected, actual);
            const string ExpectedSelection = @"UPDATE FakeData
SET
[FakeData].[NormalColumn] = @0,
[FakeData].[NullableColumn] = NULL
WHERE
[FakeData].[ValueColumn] = @1 AND
[FakeData].[NullableColumn] LIKE @2 AND
[FakeData].[NullableValueColumn] IS NOT NULL AND
[FakeData].[FakeRowId] BETWEEN @3 AND @4";

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
        public void UpdateStatement_RelatedDataSpecificSetValues_MatchesExpected()
        {
            var match = new FakeDataRow
            {
                NormalColumn = "NormalColumn",
                RelatedAliasRelatedProperty = "Related",
                NullableColumn = "CouldHaveBeenNull",
                ValueColumn = 2,
                NullableValueColumn = null,
            };

            var baseline = new FakeDataRow { Id = 10 };
            var boundary = new FakeDataRow { Id = 20 };
            var selection =
                match.ToExampleSelection(
                        row => row.ValueColumn,
                        row => row.NullableColumn,
                        row => row.NullableValueColumn,
                        row => row.RelatedAliasRelatedProperty)
                    .Select(
                        row => row.Id,
                        row => row.NormalColumn,
                        row => row.RelatedId,
                        row => row.ResultColumn,
                        row => row.RelatedAliasRelatedProperty,
                        row => row.OtherAliasRelatedProperty)
                    .Between(baseline, boundary, row => row.Id)
                    .InnerJoin<FakeDataRow, FakeRelatedRow>(row => row.Id, row => row.FakeDataId, "OtherAlias")
                    .InnerJoin<FakeDataRow, FakeRelatedRow>(row => row.Id, row => row.FakeDataId, "RelatedAlias");

            var target = new FakeDataRow { NormalColumn = "UpdatedNormalColumn", NullableColumn = null };
            var updateOperation = new UpdateOperation<FakeDataRow>(selection).Set(
                target,
                row => row.NormalColumn,
                row => row.NullableColumn);

            var expected = new object[] { "UpdatedNormalColumn", 2, "CouldHaveBeenNull", "Related", 10, 20 };
            var actual = updateOperation.ExecutionParameters.ToArray();

            CollectionAssert.AreEqual(expected, actual);
            const string ExpectedSelection = @"UPDATE FakeData
SET
[FakeData].[NormalColumn] = @0,
[FakeData].[NullableColumn] = NULL
FROM FakeData
INNER JOIN [FakeRelated] AS [OtherAlias] ON [FakeData].[Id] = [OtherAlias].[FakeDataId]
INNER JOIN [FakeRelated] AS [RelatedAlias] ON [FakeData].[Id] = [RelatedAlias].[FakeDataId]
WHERE
[FakeData].[ValueColumn] = @1 AND
[FakeData].[NullableColumn] LIKE @2 AND
[FakeData].[NullableValueColumn] IS NOT NULL AND
[RelatedAlias].[RelatedProperty] LIKE @3 AND
[FakeData].[FakeRowId] BETWEEN @4 AND @5";

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