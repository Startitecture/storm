// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlSelectionTest.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Sql.Tests
{
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Orm.Query;
    using Startitecture.Orm.Schema;
    using Startitecture.Orm.Testing.Model;

    /// <summary>
    /// The example selection test.
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class SqlSelectionTest
    {
        #region Public Methods and Operators

        /// <summary>
        /// The selection statement_ direct data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Create_SelectionStatementForDirectData_MatchesExpected()
        {
            var match = new FakeFlatDataRow { ValueColumn = 2, NullableColumn = "CouldHaveBeenNull", NullableValueColumn = null };
            var baseline = new FakeFlatDataRow { FakeDataId = 10, NormalColumn = "Greater" };
            var boundary = new FakeFlatDataRow { FakeDataId = 20, AnotherColumn = "Less" };
            var transactionSelection = Select.From<FakeFlatDataRow>()
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

            var target = new TransactSqlQueryFactory(new PetaPocoDefinitionProvider());
            var actual = target.Create(transactionSelection.AsSelect());
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement_ direct data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Create_ContainsStatementForDirectData_MatchesExpected()
        {
            var match = new FakeFlatDataRow { ValueColumn = 2, NullableColumn = "CouldHaveBeenNull", NullableValueColumn = null };
            var baseline = new FakeFlatDataRow { FakeDataId = 10, NormalColumn = "Greater" };
            var boundary = new FakeFlatDataRow { FakeDataId = 20, AnotherColumn = "Less" };

            var transactionSelection = Select.From<FakeFlatDataRow>()
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

            var target = new TransactSqlQueryFactory(new PetaPocoDefinitionProvider());
            var actual = target.Create(transactionSelection.AsContains());
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement_ direct data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Create_DeletionStatementForDirectData_MatchesExpected()
        {
            var match = new FakeFlatDataRow { ValueColumn = 2, NullableColumn = "CouldHaveBeenNull", NullableValueColumn = null };
            var baseline = new FakeFlatDataRow { FakeDataId = 10, NormalColumn = "Greater" };
            var boundary = new FakeFlatDataRow { FakeDataId = 20, AnotherColumn = "Less" };
            var transactionSelection = Select.From<FakeFlatDataRow>()
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

            var target = new TransactSqlQueryFactory(new PetaPocoDefinitionProvider());
            var actual = target.Create(transactionSelection.AsDelete());
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement_ direct data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Create_SelectionStatementForDirectDataRaisedRow_MatchesExpected()
        {
            var match = new FakeRaisedDataRow { ValueColumn = 2, NullableColumn = "CouldHaveBeenNull", NullableValueColumn = null };
            var baseline = new FakeRaisedDataRow { FakeDataId = 10, NormalColumn = "Greater" };
            var boundary = new FakeRaisedDataRow { FakeDataId = 20, AnotherColumn = "Less" };
            var transactionSelection = Select.From<FakeRaisedDataRow>()
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

            var target = new TransactSqlQueryFactory(new PetaPocoDefinitionProvider());
            var actual = target.Create(transactionSelection.AsSelect());
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement_ direct data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Create_ContainsStatementForDirectDataRaisedRow_MatchesExpected()
        {
            var match = new FakeRaisedDataRow { ValueColumn = 2, NullableColumn = "CouldHaveBeenNull", NullableValueColumn = null };
            var baseline = new FakeRaisedDataRow { FakeDataId = 10, NormalColumn = "Greater" };
            var boundary = new FakeRaisedDataRow { FakeDataId = 20, AnotherColumn = "Less" };
            var transactionSelection = Select.From<FakeRaisedDataRow>()
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

            var target = new TransactSqlQueryFactory(new PetaPocoDefinitionProvider());
            var actual = target.Create(transactionSelection.AsContains());
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement_ direct data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Create_DeletiontatementForDirectDataRaisedRow_MatchesExpected()
        {
            var match = new FakeRaisedDataRow { ValueColumn = 2, NullableColumn = "CouldHaveBeenNull", NullableValueColumn = null };
            var baseline = new FakeRaisedDataRow { FakeDataId = 10, NormalColumn = "Greater" };
            var boundary = new FakeRaisedDataRow { FakeDataId = 20, AnotherColumn = "Less" };
            var transactionSelection = Select.From<FakeRaisedDataRow>()
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

            var target = new TransactSqlQueryFactory(new PetaPocoDefinitionProvider());
            var actual = target.Create(transactionSelection.AsDelete());
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement_ related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Create_SelectionStatementForRelatedData_MatchesExpected()
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
            var transactionSelection = Select.From<FakeFlatDataRow>()
                .WhereEqual(row => row.ValueColumn, match.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                .WhereEqual(row => row.RelatedAliasRelatedProperty, match.RelatedAliasRelatedProperty)
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

            const string Expected = @"SELECT
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

            var target = new TransactSqlQueryFactory(new PetaPocoDefinitionProvider());
            var actual = target.Create(transactionSelection.AsSelect());
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement_ related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Create_ContainsStatementForRelatedData_MatchesExpected()
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
            var transactionSelection = Select.From<FakeFlatDataRow>()
                .WhereEqual(row => row.ValueColumn, match.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                .WhereEqual(row => row.RelatedAliasRelatedProperty, match.RelatedAliasRelatedProperty)
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

            const string Expected = @"IF EXISTS (
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

            var target = new TransactSqlQueryFactory(new PetaPocoDefinitionProvider());
            var actual = target.Create(transactionSelection.AsContains());
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement_ related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Create_DeletionStatementForRelatedData_MatchesExpected()
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
            var transactionSelection = Select.From<FakeFlatDataRow>()
                .WhereEqual(row => row.ValueColumn, match.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                .WhereEqual(row => row.RelatedAliasRelatedProperty, match.RelatedAliasRelatedProperty)
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

            const string Expected = @"DELETE [dbo].[FakeData]
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

            var target = new TransactSqlQueryFactory(new PetaPocoDefinitionProvider());
            var actual = target.Create(transactionSelection.AsDelete());
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement_ related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Create_SelectionStatementForRelatedDataRaisedRow_MatchesExpected()
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
            var transactionSelection = Select.From<FakeRaisedDataRow>()
                .WhereEqual(row => row.ValueColumn, match.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                .WhereEqual(row => row.RelatedAlias.RelatedProperty, "Related")
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

            const string Expected = @"SELECT
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

            var target = new TransactSqlQueryFactory(new PetaPocoDefinitionProvider());
            var actual = target.Create(transactionSelection.AsSelect());
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement_ related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Create_ContainsStatementForRelatedDataRaisedRow_MatchesExpected()
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
            var transactionSelection = Select.From<FakeRaisedDataRow>()
                .WhereEqual(row => row.ValueColumn, match.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                .WhereEqual(row => row.RelatedAlias.RelatedProperty, "Related")
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

            const string Expected = @"IF EXISTS (
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

            var target = new TransactSqlQueryFactory(new PetaPocoDefinitionProvider());
            var actual = target.Create(transactionSelection.AsContains());
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement_ related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Create_DeletionStatementForRelatedDataRaisedRow_MatchesExpected()
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
            var transactionSelection = Select.From<FakeRaisedDataRow>()
                .WhereEqual(row => row.ValueColumn, match.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                .WhereEqual(row => row.RelatedAlias.RelatedProperty, "Related")
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

            const string Expected = @"DELETE [dbo].[FakeData]
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

            var target = new TransactSqlQueryFactory(new PetaPocoDefinitionProvider());
            var actual = target.Create(transactionSelection.AsDelete());
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement_ union related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Create_SelectionStatementForUnionRelatedData_MatchesExpected()
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

            var transactionSelection = Select.From<FakeFlatDataRow>()
                .WhereEqual(row => row.ValueColumn, match1.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match1.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match1.NullableValueColumn)
                .WhereEqual(row => row.RelatedAliasRelatedProperty, match1.RelatedAliasRelatedProperty)
                .Select(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.RelatedId,
                    row => row.RelatedAliasRelatedProperty,
                    row => row.OtherAliasRelatedProperty)
                .Between(baseline1, boundary1, row => row.FakeDataId)
                .Union(
                    Select.From<FakeFlatDataRow>()
                        .WhereEqual(row => row.ValueColumn, match2.ValueColumn)
                        .WhereEqual(row => row.NullableColumn, match2.NullableColumn)
                        .WhereEqual(row => row.NullableValueColumn, match2.NullableValueColumn)
                        .WhereEqual(row => row.RelatedAliasRelatedProperty, match2.RelatedAliasRelatedProperty)
                        .Select(
                            row => row.FakeDataId,
                            row => row.NormalColumn,
                            row => row.RelatedId,
                            row => row.RelatedAliasRelatedProperty,
                            row => row.OtherAliasRelatedProperty)
                        .Between(baseline2, boundary2, row => row.FakeDataId)
                        .Union(
                            Select.From<FakeFlatDataRow>()
                                .WhereEqual(row => row.ValueColumn, match3.ValueColumn)
                                .WhereEqual(row => row.NullableColumn, match3.NullableColumn)
                                .WhereEqual(row => row.NullableValueColumn, match3.NullableValueColumn)
                                .WhereEqual(row => row.RelatedAliasRelatedProperty, match3.RelatedAliasRelatedProperty)
                                .Select(
                                    row => row.FakeDataId,
                                    row => row.NormalColumn,
                                    row => row.RelatedId,
                                    row => row.RelatedAliasRelatedProperty,
                                    row => row.OtherAliasRelatedProperty)
                                .Between(baseline3, boundary3, row => row.FakeDataId)));

            const string Expected = @"SELECT
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

            var target = new TransactSqlQueryFactory(new PetaPocoDefinitionProvider());
            var actual = target.Create(transactionSelection.AsSelect());
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement_ union related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Create_ContainsStatementForUnionRelatedData_MatchesExpected()
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

            var transactionSelection = Select.From<FakeFlatDataRow>()
                .WhereEqual(row => row.ValueColumn, match1.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match1.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match1.NullableValueColumn)
                .WhereEqual(row => row.RelatedAliasRelatedProperty, match1.RelatedAliasRelatedProperty)
                .Select(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.RelatedId,
                    row => row.RelatedAliasRelatedProperty,
                    row => row.OtherAliasRelatedProperty)
                .Between(baseline1, boundary1, row => row.FakeDataId)
                .Union(
                    Select.From<FakeFlatDataRow>()
                        .WhereEqual(row => row.ValueColumn, match2.ValueColumn)
                        .WhereEqual(row => row.NullableColumn, match2.NullableColumn)
                        .WhereEqual(row => row.NullableValueColumn, match2.NullableValueColumn)
                        .WhereEqual(row => row.RelatedAliasRelatedProperty, match2.RelatedAliasRelatedProperty)
                        .Select(
                            row => row.FakeDataId,
                            row => row.NormalColumn,
                            row => row.RelatedId,
                            row => row.RelatedAliasRelatedProperty,
                            row => row.OtherAliasRelatedProperty)
                        .Between(baseline2, boundary2, row => row.FakeDataId)
                        .Union(
                            Select.From<FakeFlatDataRow>()
                                .WhereEqual(row => row.ValueColumn, match3.ValueColumn)
                                .WhereEqual(row => row.NullableColumn, match3.NullableColumn)
                                .WhereEqual(row => row.NullableValueColumn, match3.NullableValueColumn)
                                .WhereEqual(row => row.RelatedAliasRelatedProperty, match3.RelatedAliasRelatedProperty)
                                .Select(
                                    row => row.FakeDataId,
                                    row => row.NormalColumn,
                                    row => row.RelatedId,
                                    row => row.RelatedAliasRelatedProperty,
                                    row => row.OtherAliasRelatedProperty)
                                .Between(baseline3, boundary3, row => row.FakeDataId)));

            const string Expected = @"IF EXISTS (
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

            var target = new TransactSqlQueryFactory(new PetaPocoDefinitionProvider());
            var actual = target.Create(transactionSelection.AsContains());
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement_ union related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Create_DeletionStatementForUnionRelatedData_MatchesExpected()
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

            var transactionSelection = Select.From<FakeFlatDataRow>()
                .WhereEqual(row => row.ValueColumn, match1.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match1.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match1.NullableValueColumn)
                .WhereEqual(row => row.RelatedAliasRelatedProperty, match1.RelatedAliasRelatedProperty)
                .Select(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.RelatedId,
                    row => row.RelatedAliasRelatedProperty,
                    row => row.OtherAliasRelatedProperty)
                .Between(baseline1, boundary1, row => row.FakeDataId)
                .Union(
                    Select.From<FakeFlatDataRow>()
                        .WhereEqual(row => row.ValueColumn, match2.ValueColumn)
                        .WhereEqual(row => row.NullableColumn, match2.NullableColumn)
                        .WhereEqual(row => row.NullableValueColumn, match2.NullableValueColumn)
                        .WhereEqual(row => row.RelatedAliasRelatedProperty, match2.RelatedAliasRelatedProperty)
                        .Select(
                            row => row.FakeDataId,
                            row => row.NormalColumn,
                            row => row.RelatedId,
                            row => row.RelatedAliasRelatedProperty,
                            row => row.OtherAliasRelatedProperty)
                        .Between(baseline2, boundary2, row => row.FakeDataId)
                        .Union(
                            Select.From<FakeFlatDataRow>()
                                .WhereEqual(row => row.ValueColumn, match3.ValueColumn)
                                .WhereEqual(row => row.NullableColumn, match3.NullableColumn)
                                .WhereEqual(row => row.NullableValueColumn, match3.NullableValueColumn)
                                .WhereEqual(row => row.RelatedAliasRelatedProperty, match3.RelatedAliasRelatedProperty)
                                .Select(
                                    row => row.FakeDataId,
                                    row => row.NormalColumn,
                                    row => row.RelatedId,
                                    row => row.RelatedAliasRelatedProperty,
                                    row => row.OtherAliasRelatedProperty)
                                .Between(baseline3, boundary3, row => row.FakeDataId)));

            const string Expected = @"DELETE [dbo].[FakeData]
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

            var target = new TransactSqlQueryFactory(new PetaPocoDefinitionProvider());
            var actual = target.Create(transactionSelection.AsDelete());
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement_ union related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Create_SelectionStatementForUnionRelatedDataRaisedRow_MatchesExpected()
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

            var transactionSelection = Select.From<FakeRaisedDataRow>()
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
                    Select.From<FakeRaisedDataRow>()
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
                            Select.From<FakeRaisedDataRow>()
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

            var target = new TransactSqlQueryFactory(new PetaPocoDefinitionProvider());
            var actual = target.Create(transactionSelection.AsSelect());
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement_ union related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Create_ContainsStatementForUnionRelatedDataRaisedRow_MatchesExpected()
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

            var transactionSelection = Select.From<FakeRaisedDataRow>()
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
                    Select.From<FakeRaisedDataRow>()
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
                            Select.From<FakeRaisedDataRow>()
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

            var target = new TransactSqlQueryFactory(new PetaPocoDefinitionProvider());
            var actual = target.Create(transactionSelection.AsContains());
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement_ union related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Create_DeletionStatementForUnionRelatedDataRaisedRow_MatchesExpected()
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

            var transactionSelection = Select.From<FakeRaisedDataRow>()
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
                    Select.From<FakeRaisedDataRow>()
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
                            Select.From<FakeRaisedDataRow>()
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

            var target = new TransactSqlQueryFactory(new PetaPocoDefinitionProvider());
            var actual = target.Create(transactionSelection.AsDelete());
            Assert.AreEqual(Expected, actual);
        }

        #endregion
    }
}