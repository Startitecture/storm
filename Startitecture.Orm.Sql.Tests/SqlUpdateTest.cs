// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlUpdateTest.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// ReSharper disable StringLiteralTypo
namespace Startitecture.Orm.Sql.Tests
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Orm.Query;
    using Startitecture.Orm.Schema;
    using Startitecture.Orm.Testing.Entities;

    /// <summary>
    /// The update operation test.
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class SqlUpdateTest
    {
        #region Public Methods and Operators

        /// <summary>
        /// The update statement_ direct data_ matches expected.
        /// </summary>
        [TestMethod]
        public void UpdateStatement_DirectData_MatchesExpected()
        {
            var match = new DataRow
                            {
                                NormalColumn = "NormalColumn",
                                NullableColumn = "CouldHaveBeenNull",
                                ValueColumn = 2,
                                NullableValueColumn = null,
                                AnotherValueColumn = 12,
                                AnotherColumn = "Some Other Value"
                            };

            var baseline = new DataRow { FakeDataId = 10 };
            var boundary = new DataRow { FakeDataId = 20 };
            var selection = Select.From<DataRow>(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.NullableColumn,
                    row => row.NullableValueColumn,
                    row => row.ValueColumn)
                .InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId)
                .WhereEqual(row => row.ValueColumn, match.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                .Between(baseline, boundary, row => row.FakeDataId);

            var updateOperation = new SqlUpdate<DataRow>(new DataAnnotationsDefinitionProvider(), selection).Set(match);

            var statement = updateOperation.ExecutionStatement;
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
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
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
            var match = new DataRow
            {
                NormalColumn = "NormalColumn",
                NullableColumn = "CouldHaveBeenNull",
                ValueColumn = 2,
                NullableValueColumn = null,
                AnotherValueColumn = 12,
                AnotherColumn = "Some Other Value"
            };

            var baseline = new DataRow { FakeDataId = 10 };
            var boundary = new DataRow { FakeDataId = 20 };
            var selection = Select.From<DataRow>(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.NullableColumn,
                    row => row.NullableValueColumn,
                    row => row.ValueColumn)
                .InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId)
                .WhereEqual(row => row.ValueColumn, match.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                .Between(baseline, boundary, row => row.FakeDataId);

            var updateOperation = new SqlUpdate<DataRow>(new DataAnnotationsDefinitionProvider(), selection).Set(match);

            var statement = updateOperation.ExecutionStatement;
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
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
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
            var match = new DataRow
                            {
                                NormalColumn = "NormalColumn",
                                RelatedAlias = new FakeRelatedRow
                                                   {
                                                       RelatedProperty = "Related"
                                                   },
                                NullableColumn = "CouldHaveBeenNull",
                                ValueColumn = 2,
                                NullableValueColumn = null,
                                AnotherValueColumn = 12,
                                AnotherColumn = "Some Other Value"
                            };

            var baseline = new DataRow { FakeDataId = 10 };
            var boundary = new DataRow { FakeDataId = 20 };
            var selection = Select.From<DataRow>(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.Related.RelatedId,
                    row => row.RelatedAlias.RelatedProperty,
                    row => row.OtherAlias.RelatedProperty)
                .InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId)
                .WhereEqual(row => row.ValueColumn, match.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                .WhereEqual(row => row.RelatedAlias.RelatedProperty, match.RelatedAlias.RelatedProperty)
                .Between(baseline, boundary, row => row.FakeDataId);

            var updateOperation = new SqlUpdate<DataRow>(new DataAnnotationsDefinitionProvider(), selection).Set(match);
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
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE
[dbo].[FakeData].[ValueColumn] = @5 AND
[dbo].[FakeData].[NullableColumn] LIKE @6 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @7 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @8 AND @9";

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
        public void UpdateStatement_RaisedRelatedData_MatchesExpected()
        {
            var match = new DataRow
            {
                NormalColumn = "NormalColumn",
                RelatedAlias = new FakeRelatedRow { RelatedProperty = "Related" },
                NullableColumn = "CouldHaveBeenNull",
                ValueColumn = 2,
                NullableValueColumn = null,
                AnotherValueColumn = 12,
                AnotherColumn = "Some Other Value"
            };

            var baseline = new DataRow { FakeDataId = 10 };
            var boundary = new DataRow { FakeDataId = 20 };
            var selection = Select.From<DataRow>(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.RelatedAlias.RelatedId,
                    row => row.RelatedAlias.RelatedProperty,
                    row => row.OtherAlias.RelatedProperty)
                .InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId)
                .WhereEqual(row => row.ValueColumn, match.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                .WhereEqual(row => row.RelatedAlias.RelatedProperty, "Related")
                .Between(baseline, boundary, row => row.FakeDataId);

            var updateOperation = new SqlUpdate<DataRow>(new DataAnnotationsDefinitionProvider(), selection).Set(match);
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
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE
[dbo].[FakeData].[ValueColumn] = @5 AND
[dbo].[FakeData].[NullableColumn] LIKE @6 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @7 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @8 AND @9";

            var statement = updateOperation.ExecutionStatement;
            Assert.AreEqual(ExpectedSelection, statement);
        }

        /// <summary>
        /// The update statement_ direct data_ matches expected.
        /// </summary>
        [TestMethod]
        public void UpdateStatement_DirectDataSpecificSetValues_MatchesExpected()
        {
            var match = new DataRow
            {
                NormalColumn = "NormalColumn",
                NullableColumn = "CouldHaveBeenNull",
                ValueColumn = 2,
                NullableValueColumn = null,
            };

            var baseline = new DataRow { FakeDataId = 10 };
            var boundary = new DataRow { FakeDataId = 20 };
            var selection = Select.From<DataRow>(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.NullableColumn,
                    row => row.NullableValueColumn,
                    row => row.ValueColumn)
                .InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId)
                .WhereEqual(row => row.ValueColumn, match.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                .Between(baseline, boundary, row => row.FakeDataId);

            var target = new DataRow { NormalColumn = "UpdatedNormalColumn", NullableColumn = null };
            var updateOperation = new SqlUpdate<DataRow>(new DataAnnotationsDefinitionProvider(), selection).Set(
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
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE
[dbo].[FakeData].[ValueColumn] = @1 AND
[dbo].[FakeData].[NullableColumn] LIKE @2 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4";

            var statement = updateOperation.ExecutionStatement;
            Assert.AreEqual(ExpectedSelection, statement);
        }

        /// <summary>
        /// The update statement_ direct data_ matches expected.
        /// </summary>
        [TestMethod]
        public void UpdateStatement_RaisedDirectDataSpecificSetValues_MatchesExpected()
        {
            var match = new DataRow
            {
                NormalColumn = "NormalColumn",
                NullableColumn = "CouldHaveBeenNull",
                ValueColumn = 2,
                NullableValueColumn = null,
            };

            var baseline = new DataRow { FakeDataId = 10 };
            var boundary = new DataRow { FakeDataId = 20 };
            var selection = Select.From<DataRow>(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.NullableColumn,
                    row => row.NullableValueColumn,
                    row => row.ValueColumn)
                .InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId)
                .WhereEqual(row => row.ValueColumn, match.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                .Between(baseline, boundary, row => row.FakeDataId);

            var target = new DataRow { NormalColumn = "UpdatedNormalColumn", NullableColumn = null };
            var updateOperation = new SqlUpdate<DataRow>(new DataAnnotationsDefinitionProvider(), selection).Set(
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
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
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
            var match = new DataRow
                            {
                                NormalColumn = "NormalColumn",
                                RelatedAlias = new FakeRelatedRow
                                                   {
                                                       RelatedProperty = "Related"
                                                   },
                                NullableColumn = "CouldHaveBeenNull",
                                ValueColumn = 2,
                                NullableValueColumn = null,
                            };

            var baseline = new DataRow { FakeDataId = 10 };
            var boundary = new DataRow { FakeDataId = 20 };
            var selection = Select.From<DataRow>(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.Related.RelatedId,
                    row => row.RelatedAlias.RelatedProperty,
                    row => row.OtherAlias.RelatedProperty)
                .InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId)
                .WhereEqual(row => row.ValueColumn, match.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                .WhereEqual(row => row.RelatedAlias.RelatedProperty, match.RelatedAlias.RelatedProperty)
                .Between(baseline, boundary, row => row.FakeDataId);

            var target = new DataRow { NormalColumn = "UpdatedNormalColumn", NullableColumn = null };
            var updateOperation = new SqlUpdate<DataRow>(new DataAnnotationsDefinitionProvider(), selection).Set(
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
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE
[dbo].[FakeData].[ValueColumn] = @1 AND
[dbo].[FakeData].[NullableColumn] LIKE @2 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @3 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @4 AND @5";

            var statement = updateOperation.ExecutionStatement;
            Assert.AreEqual(ExpectedSelection, statement);
        }

        /// <summary>
        /// The update statement_ related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void UpdateStatement_RaisedRelatedDataSpecificSetValues_MatchesExpected()
        {
            var match = new DataRow
            {
                NormalColumn = "NormalColumn",
                RelatedAlias = new FakeRelatedRow { RelatedProperty = "Related" },
                NullableColumn = "CouldHaveBeenNull",
                ValueColumn = 2,
                NullableValueColumn = null,
            };

            var baseline = new DataRow { FakeDataId = 10 };
            var boundary = new DataRow { FakeDataId = 20 };
            var selection = Select.From<DataRow>(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.RelatedAlias.RelatedId,
                    row => row.RelatedAlias.RelatedProperty,
                    row => row.OtherAlias.RelatedProperty)
                .InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId)
                .WhereEqual(row => row.ValueColumn, match.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                .WhereEqual(row => row.RelatedAlias.RelatedProperty, "Related")
                .Between(baseline, boundary, row => row.FakeDataId);

            var target = new DataRow { NormalColumn = "UpdatedNormalColumn", NullableColumn = null };
            var updateOperation = new SqlUpdate<DataRow>(new DataAnnotationsDefinitionProvider(), selection).Set(
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
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE
[dbo].[FakeData].[ValueColumn] = @1 AND
[dbo].[FakeData].[NullableColumn] LIKE @2 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @3 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @4 AND @5";

            var statement = updateOperation.ExecutionStatement;
            Assert.AreEqual(ExpectedSelection, statement);
        }

        #endregion
    }
}