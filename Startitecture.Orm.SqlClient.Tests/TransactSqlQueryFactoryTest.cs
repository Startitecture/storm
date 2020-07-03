﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactSqlQueryFactoryTest.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable StringLiteralTypo
namespace Startitecture.Orm.SqlClient.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Orm.Model;
    using Startitecture.Orm.Schema;
    using Startitecture.Orm.SqlClient;
    using Startitecture.Orm.Testing.Entities;

    /// <summary>
    /// The example selection test.
    /// </summary>
    [TestClass]
    public class TransactSqlQueryFactoryTest
    {
        #region Public Methods and Operators

        /// <summary>
        /// The selection statement direct data matches expected.
        /// </summary>
        [TestMethod]
        public void Create_SelectionStatementForDirectData_MatchesExpected()
        {
            var match = new DataRow { ValueColumn = 2, NullableColumn = "CouldHaveBeenNull", NullableValueColumn = null };
            var baseline = new DataRow { FakeDataId = 10, NormalColumn = "Greater" };
            var boundary = new DataRow { FakeDataId = 20, AnotherColumn = "Less" };
            var transactionSelection = Select
                .From<DataRow>(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.NullableColumn,
                    row => row.NullableValueColumn,
                    row => row.ValueColumn,
                    row => row.AnotherColumn,
                    row => row.AnotherValueColumn)
                .InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId)
                .WhereEqual(row => row.ValueColumn, match.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                .Between(baseline, boundary, row => row.FakeDataId)
                .GreaterThanOrEqualTo(row => row.NormalColumn, baseline.NormalColumn)
                .LessThanOrEqualTo(row => row.AnotherColumn, boundary.AnotherColumn)
                .Include(row => row.AnotherValueColumn, 5, 10, 15, 20)
                .OrderBy(row => row.Related.RelatedProperty)
                .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                .OrderBy(row => row.NormalColumn);

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
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[dbo].[FakeData].[FakeRowId] BETWEEN @2 AND @3 AND
[dbo].[FakeData].[NormalColumn] >= @4 AND
[dbo].[FakeData].[AnotherColumn] <= @5 AND
[dbo].[FakeData].[AnotherValueColumn] IN (@6, @7, @8, @9)
ORDER BY [someschema].[Related].[RelatedProperty], [OtherAlias].[RelatedProperty] DESC, [dbo].[FakeData].[NormalColumn]";

            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var target = new TransactSqlQueryFactory(definitionProvider);
            var actual = target.Create(transactionSelection.AsSelect(definitionProvider.Resolve<DataRow>()));
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement direct data matches expected.
        /// </summary>
        [TestMethod]
        public void Create_DefaultSelectionStatementForDirectData_AllColumnsIncluded()
        {
            var match = new DataRow { ValueColumn = 2, NullableColumn = "CouldHaveBeenNull", NullableValueColumn = null };
            var baseline = new DataRow { FakeDataId = 10, NormalColumn = "Greater" };
            var boundary = new DataRow { FakeDataId = 20, AnotherColumn = "Less" };
            var transactionSelection = Select.From<DataRow>()
                .InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId)
                .WhereEqual(row => row.ValueColumn, match.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                .Between(baseline, boundary, row => row.FakeDataId)
                .GreaterThanOrEqualTo(row => row.NormalColumn, baseline.NormalColumn)
                .LessThanOrEqualTo(row => row.AnotherColumn, boundary.AnotherColumn)
                .Include(row => row.AnotherValueColumn, 5, 10, 15, 20)
                .OrderBy(row => row.Related.RelatedProperty)
                .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                .OrderBy(row => row.NormalColumn);

            const string Expected = @"SELECT
    [someschema].[Related].[RelatedId] AS [Related.RelatedId],
    [someschema].[Related].[FakeDataId] AS [Related.FakeDataId],
    [someschema].[Related].[RelatedProperty] AS [Related.RelatedProperty],
    [OtherAlias].[RelatedId] AS [OtherAlias.RelatedId],
    [OtherAlias].[FakeDataId] AS [OtherAlias.FakeDataId],
    [OtherAlias].[RelatedProperty] AS [OtherAlias.RelatedProperty],
    [RelatedDependency].[FakeDependencyEntityId] AS [RelatedDependency.FakeDependencyEntityId],
    [RelatedDependency].[ComplexEntityId] AS [RelatedDependency.ComplexEntityId],
    [RelatedDependency].[UniqueName] AS [RelatedDependency.UniqueName],
    [RelatedDependency].[Description] AS [RelatedDependency.Description],
    [RelatedAlias].[RelatedId] AS [RelatedAlias.RelatedId],
    [RelatedAlias].[FakeDataId] AS [RelatedAlias.FakeDataId],
    [RelatedAlias].[RelatedProperty] AS [RelatedAlias.RelatedProperty],
    [dbo].[DependencyEntity].[FakeDependencyEntityId] AS [DependencyEntity.FakeDependencyEntityId],
    [dbo].[DependencyEntity].[ComplexEntityId] AS [DependencyEntity.ComplexEntityId],
    [dbo].[DependencyEntity].[UniqueName] AS [DependencyEntity.UniqueName],
    [dbo].[DependencyEntity].[Description] AS [DependencyEntity.Description],
    [dbo].[SubData].[ParentFakeDataId],
    [dbo].[FakeData].[FakeRowId],
    [dbo].[FakeData].[NormalColumn],
    [dbo].[FakeData].[NullableColumn],
    [dbo].[FakeData].[ValueColumn],
    [dbo].[FakeData].[AnotherValueColumn],
    [dbo].[FakeData].[AnotherColumn],
    [dbo].[FakeData].[NullableValueColumn]
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[dbo].[FakeData].[FakeRowId] BETWEEN @2 AND @3 AND
[dbo].[FakeData].[NormalColumn] >= @4 AND
[dbo].[FakeData].[AnotherColumn] <= @5 AND
[dbo].[FakeData].[AnotherValueColumn] IN (@6, @7, @8, @9)
ORDER BY [someschema].[Related].[RelatedProperty], [OtherAlias].[RelatedProperty] DESC, [dbo].[FakeData].[NormalColumn]";

            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var target = new TransactSqlQueryFactory(definitionProvider);
            var actual = target.Create(transactionSelection.AsSelect(definitionProvider.Resolve<DataRow>()));
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement direct data matches expected.
        /// </summary>
        [TestMethod]
        public void Create_CountStatementForDirectData_MatchesExpected()
        {
            var match = new DataRow { ValueColumn = 2, NullableColumn = "CouldHaveBeenNull", NullableValueColumn = null };
            var baseline = new DataRow { FakeDataId = 10, NormalColumn = "Greater" };
            var boundary = new DataRow { FakeDataId = 20, AnotherColumn = "Less" };
            var transactionSelection = Select
                .From<DataRow>().Count(row => row.FakeDataId)
                .InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId)
                .WhereEqual(row => row.ValueColumn, match.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                .Between(baseline, boundary, row => row.FakeDataId)
                .GreaterThanOrEqualTo(row => row.NormalColumn, baseline.NormalColumn)
                .LessThanOrEqualTo(row => row.AnotherColumn, boundary.AnotherColumn)
                .Include(row => row.AnotherValueColumn, 5, 10, 15, 20);

            const string Expected = @"SELECT
    COUNT([dbo].[FakeData].[FakeRowId])
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[dbo].[FakeData].[FakeRowId] BETWEEN @2 AND @3 AND
[dbo].[FakeData].[NormalColumn] >= @4 AND
[dbo].[FakeData].[AnotherColumn] <= @5 AND
[dbo].[FakeData].[AnotherValueColumn] IN (@6, @7, @8, @9)";

            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var target = new TransactSqlQueryFactory(definitionProvider);
            var actual = target.Create(transactionSelection.AsSelect(definitionProvider.Resolve<DataRow>()));
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement direct data matches expected.
        /// </summary>
        [TestMethod]
        public void Create_PageStatementForDirectData_MatchesExpected()
        {
            var match = new DataRow { ValueColumn = 2, NullableColumn = "CouldHaveBeenNull", NullableValueColumn = null };
            var baseline = new DataRow { FakeDataId = 10, NormalColumn = "Greater" };
            var boundary = new DataRow { FakeDataId = 20, AnotherColumn = "Less" };
            var transactionSelection = Select
                .From<DataRow>(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.NullableColumn,
                    row => row.NullableValueColumn,
                    row => row.ValueColumn,
                    row => row.AnotherColumn,
                    row => row.AnotherValueColumn)
                .InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId)
                .WhereEqual(row => row.ValueColumn, match.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                .Between(baseline, boundary, row => row.FakeDataId)
                .GreaterThanOrEqualTo(row => row.NormalColumn, baseline.NormalColumn)
                .LessThanOrEqualTo(row => row.AnotherColumn, boundary.AnotherColumn)
                .Include(row => row.AnotherValueColumn, 5, 10, 15, 20)
                .OrderBy(row => row.Related.RelatedProperty)
                .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                .OrderBy(row => row.NormalColumn)
                .Skip(5).Take(5);

            const string Expected = @";WITH pg AS
    (
        SELECT FakeRowId
        FROM [dbo].[FakeData]
        INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
        INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
        INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
        INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
        INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
        LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
        WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
        [dbo].[FakeData].[NullableColumn] LIKE @1 AND
        [dbo].[FakeData].[NullableValueColumn] IS NULL AND
        [dbo].[FakeData].[FakeRowId] BETWEEN @2 AND @3 AND
        [dbo].[FakeData].[NormalColumn] >= @4 AND
        [dbo].[FakeData].[AnotherColumn] <= @5 AND
        [dbo].[FakeData].[AnotherValueColumn] IN (@6, @7, @8, @9)
        ORDER BY [someschema].[Related].[RelatedProperty], [OtherAlias].[RelatedProperty] DESC, [dbo].[FakeData].[NormalColumn]
        OFFSET @10 ROWS
        FETCH NEXT @11 ROWS ONLY
    )
SELECT 
    [dbo].[FakeData].[FakeRowId],
    [dbo].[FakeData].[NormalColumn],
    [dbo].[FakeData].[NullableColumn],
    [dbo].[FakeData].[NullableValueColumn],
    [dbo].[FakeData].[ValueColumn],
    [dbo].[FakeData].[AnotherColumn],
    [dbo].[FakeData].[AnotherValueColumn]
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE EXISTS 
(SELECT 1 FROM pg 
WHERE pg.FakeRowId = [dbo].[FakeData].[FakeRowId]) AND
[dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[dbo].[FakeData].[FakeRowId] BETWEEN @2 AND @3 AND
[dbo].[FakeData].[NormalColumn] >= @4 AND
[dbo].[FakeData].[AnotherColumn] <= @5 AND
[dbo].[FakeData].[AnotherValueColumn] IN (@6, @7, @8, @9)
ORDER BY [someschema].[Related].[RelatedProperty], [OtherAlias].[RelatedProperty] DESC, [dbo].[FakeData].[NormalColumn] OPTION (RECOMPILE)";

            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var target = new TransactSqlQueryFactory(definitionProvider);
            var actual = target.Create(transactionSelection.AsSelect(definitionProvider.Resolve<DataRow>()));
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement direct data matches expected.
        /// </summary>
        [TestMethod]
        public void Create_ContainsStatementForDirectData_MatchesExpected()
        {
            var match = new DataRow { ValueColumn = 2, NullableColumn = "CouldHaveBeenNull", NullableValueColumn = null };
            var baseline = new DataRow { FakeDataId = 10, NormalColumn = "Greater" };
            var boundary = new DataRow { FakeDataId = 20, AnotherColumn = "Less" };

            var transactionSelection = Select
                .From<DataRow>(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.NullableColumn,
                    row => row.NullableValueColumn,
                    row => row.ValueColumn,
                    row => row.AnotherColumn,
                    row => row.AnotherValueColumn)
                .InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId)
                .WhereEqual(row => row.ValueColumn, match.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                .Between(baseline, boundary, row => row.FakeDataId)
                .GreaterThanOrEqualTo(row => row.NormalColumn, baseline.NormalColumn)
                .LessThanOrEqualTo(row => row.AnotherColumn, boundary.AnotherColumn)
                .Include(row => row.AnotherValueColumn, 5, 10, 15, 20)
                .OrderBy(row => row.Related.RelatedProperty)
                .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                .OrderBy(row => row.NormalColumn);

            const string Expected = @"IF EXISTS (
SELECT
    1
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[dbo].[FakeData].[FakeRowId] BETWEEN @2 AND @3 AND
[dbo].[FakeData].[NormalColumn] >= @4 AND
[dbo].[FakeData].[AnotherColumn] <= @5 AND
[dbo].[FakeData].[AnotherValueColumn] IN (@6, @7, @8, @9)
ORDER BY [someschema].[Related].[RelatedProperty], [OtherAlias].[RelatedProperty] DESC, [dbo].[FakeData].[NormalColumn]
) SELECT 1  ELSE SELECT 0";

            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var target = new TransactSqlQueryFactory(definitionProvider);
            var actual = target.Create(transactionSelection.AsContains(definitionProvider.Resolve<DataRow>()));
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement direct data matches expected.
        /// </summary>
        [TestMethod]
        public void Create_DeletionStatementForDirectData_MatchesExpected()
        {
            var match = new DataRow { ValueColumn = 2, NullableColumn = "CouldHaveBeenNull", NullableValueColumn = null };
            var baseline = new DataRow { FakeDataId = 10, NormalColumn = "Greater" };
            var boundary = new DataRow { FakeDataId = 20, AnotherColumn = "Less" };
            var transactionSelection = Select
                .From<DataRow>(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.NullableColumn,
                    row => row.NullableValueColumn,
                    row => row.ValueColumn,
                    row => row.AnotherColumn,
                    row => row.AnotherValueColumn)
                .InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId)
                .WhereEqual(row => row.ValueColumn, match.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                .Between(baseline, boundary, row => row.FakeDataId)
                .GreaterThanOrEqualTo(row => row.NormalColumn, baseline.NormalColumn)
                .LessThanOrEqualTo(row => row.AnotherColumn, boundary.AnotherColumn)
                .Include(row => row.AnotherValueColumn, 5, 10, 15, 20);

            const string Expected = @"DELETE [dbo].[FakeData]
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[dbo].[FakeData].[FakeRowId] BETWEEN @2 AND @3 AND
[dbo].[FakeData].[NormalColumn] >= @4 AND
[dbo].[FakeData].[AnotherColumn] <= @5 AND
[dbo].[FakeData].[AnotherValueColumn] IN (@6, @7, @8, @9)";

            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var target = new TransactSqlQueryFactory(definitionProvider);
            var actual = target.Create(transactionSelection.AsDelete(definitionProvider.Resolve<DataRow>()));
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement direct data matches expected.
        /// </summary>
        [TestMethod]
        public void Create_SelectionStatementForDirectDataRaisedRow_MatchesExpected()
        {
            var match = new DataRow { ValueColumn = 2, NullableColumn = "CouldHaveBeenNull", NullableValueColumn = null };
            var baseline = new DataRow { FakeDataId = 10, NormalColumn = "Greater" };
            var boundary = new DataRow { FakeDataId = 20, AnotherColumn = "Less" };
            var transactionSelection = Select
                .From<DataRow>(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.NullableColumn,
                    row => row.NullableValueColumn,
                    row => row.ValueColumn,
                    row => row.AnotherColumn,
                    row => row.AnotherValueColumn)
                .InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId)
                .WhereEqual(row => row.ValueColumn, match.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                .Between(baseline, boundary, row => row.FakeDataId)
                .GreaterThanOrEqualTo(row => row.NormalColumn, baseline.NormalColumn)
                .LessThanOrEqualTo(row => row.AnotherColumn, boundary.AnotherColumn)
                .Include(row => row.AnotherValueColumn, 5, 10, 15, 20)
                .OrderBy(row => row.Related.RelatedProperty)
                .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                .OrderBy(row => row.NormalColumn);

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
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[dbo].[FakeData].[FakeRowId] BETWEEN @2 AND @3 AND
[dbo].[FakeData].[NormalColumn] >= @4 AND
[dbo].[FakeData].[AnotherColumn] <= @5 AND
[dbo].[FakeData].[AnotherValueColumn] IN (@6, @7, @8, @9)
ORDER BY [someschema].[Related].[RelatedProperty], [OtherAlias].[RelatedProperty] DESC, [dbo].[FakeData].[NormalColumn]";

            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var target = new TransactSqlQueryFactory(definitionProvider);
            var actual = target.Create(transactionSelection.AsSelect(definitionProvider.Resolve<DataRow>()));
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement direct data matches expected.
        /// </summary>
        [TestMethod]
        public void Create_CountStatementForDirectDataRaisedRow_MatchesExpected()
        {
            var match = new DataRow { ValueColumn = 2, NullableColumn = "CouldHaveBeenNull", NullableValueColumn = null };
            var baseline = new DataRow { FakeDataId = 10, NormalColumn = "Greater" };
            var boundary = new DataRow { FakeDataId = 20, AnotherColumn = "Less" };
            var transactionSelection = Select
                .From<DataRow>().Count(row => row.FakeDataId)
                .InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId)
                .WhereEqual(row => row.ValueColumn, match.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                .Between(baseline, boundary, row => row.FakeDataId)
                .GreaterThanOrEqualTo(row => row.NormalColumn, baseline.NormalColumn)
                .LessThanOrEqualTo(row => row.AnotherColumn, boundary.AnotherColumn)
                .Include(row => row.AnotherValueColumn, 5, 10, 15, 20);

            const string Expected = @"SELECT
    COUNT([dbo].[FakeData].[FakeRowId])
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[dbo].[FakeData].[FakeRowId] BETWEEN @2 AND @3 AND
[dbo].[FakeData].[NormalColumn] >= @4 AND
[dbo].[FakeData].[AnotherColumn] <= @5 AND
[dbo].[FakeData].[AnotherValueColumn] IN (@6, @7, @8, @9)";

            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var target = new TransactSqlQueryFactory(definitionProvider);
            var actual = target.Create(transactionSelection.AsSelect(definitionProvider.Resolve<DataRow>()));
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement direct data matches expected.
        /// </summary>
        [TestMethod]
        public void Create_PageStatementForDirectDataRaisedRow_MatchesExpected()
        {
            var match = new DataRow { ValueColumn = 2, NullableColumn = "CouldHaveBeenNull", NullableValueColumn = null };
            var baseline = new DataRow { FakeDataId = 10, NormalColumn = "Greater" };
            var boundary = new DataRow { FakeDataId = 20, AnotherColumn = "Less" };
            var transactionSelection = Select
                .From<DataRow>(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.NullableColumn,
                    row => row.NullableValueColumn,
                    row => row.ValueColumn,
                    row => row.AnotherColumn,
                    row => row.AnotherValueColumn)
                .InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId)
                .WhereEqual(row => row.ValueColumn, match.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                .Between(baseline, boundary, row => row.FakeDataId)
                .GreaterThanOrEqualTo(row => row.NormalColumn, baseline.NormalColumn)
                .LessThanOrEqualTo(row => row.AnotherColumn, boundary.AnotherColumn)
                .Include(row => row.AnotherValueColumn, 5, 10, 15, 20)
                .OrderBy(row => row.Related.RelatedProperty)
                .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                .OrderBy(row => row.NormalColumn)
                .Skip(5).Take(5);

            const string Expected = @";WITH pg AS
    (
        SELECT FakeRowId
        FROM [dbo].[FakeData]
        INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
        INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
        INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
        INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
        INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
        LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
        WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
        [dbo].[FakeData].[NullableColumn] LIKE @1 AND
        [dbo].[FakeData].[NullableValueColumn] IS NULL AND
        [dbo].[FakeData].[FakeRowId] BETWEEN @2 AND @3 AND
        [dbo].[FakeData].[NormalColumn] >= @4 AND
        [dbo].[FakeData].[AnotherColumn] <= @5 AND
        [dbo].[FakeData].[AnotherValueColumn] IN (@6, @7, @8, @9)
        ORDER BY [someschema].[Related].[RelatedProperty], [OtherAlias].[RelatedProperty] DESC, [dbo].[FakeData].[NormalColumn]
        OFFSET @10 ROWS
        FETCH NEXT @11 ROWS ONLY
    )
SELECT 
    [dbo].[FakeData].[FakeRowId],
    [dbo].[FakeData].[NormalColumn],
    [dbo].[FakeData].[NullableColumn],
    [dbo].[FakeData].[NullableValueColumn],
    [dbo].[FakeData].[ValueColumn],
    [dbo].[FakeData].[AnotherColumn],
    [dbo].[FakeData].[AnotherValueColumn]
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE EXISTS 
(SELECT 1 FROM pg 
WHERE pg.FakeRowId = [dbo].[FakeData].[FakeRowId]) AND
[dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[dbo].[FakeData].[FakeRowId] BETWEEN @2 AND @3 AND
[dbo].[FakeData].[NormalColumn] >= @4 AND
[dbo].[FakeData].[AnotherColumn] <= @5 AND
[dbo].[FakeData].[AnotherValueColumn] IN (@6, @7, @8, @9)
ORDER BY [someschema].[Related].[RelatedProperty], [OtherAlias].[RelatedProperty] DESC, [dbo].[FakeData].[NormalColumn] OPTION (RECOMPILE)";

            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var target = new TransactSqlQueryFactory(definitionProvider);
            var actual = target.Create(transactionSelection.AsSelect(definitionProvider.Resolve<DataRow>()));
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement direct data matches expected.
        /// </summary>
        [TestMethod]
        public void Create_ContainsStatementForDirectDataRaisedRow_MatchesExpected()
        {
            var match = new DataRow { ValueColumn = 2, NullableColumn = "CouldHaveBeenNull", NullableValueColumn = null };
            var baseline = new DataRow { FakeDataId = 10, NormalColumn = "Greater" };
            var boundary = new DataRow { FakeDataId = 20, AnotherColumn = "Less" };
            var transactionSelection = Select
                .From<DataRow>(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.NullableColumn,
                    row => row.NullableValueColumn,
                    row => row.ValueColumn,
                    row => row.AnotherColumn,
                    row => row.AnotherValueColumn)
                .InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId)
                .WhereEqual(row => row.ValueColumn, match.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                .Between(baseline, boundary, row => row.FakeDataId)
                .GreaterThanOrEqualTo(row => row.NormalColumn, baseline.NormalColumn)
                .LessThanOrEqualTo(row => row.AnotherColumn, boundary.AnotherColumn)
                .Include(row => row.AnotherValueColumn, 5, 10, 15, 20)
                .OrderBy(row => row.Related.RelatedProperty)
                .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                .OrderBy(row => row.NormalColumn);

            const string Expected = @"IF EXISTS (
SELECT
    1
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[dbo].[FakeData].[FakeRowId] BETWEEN @2 AND @3 AND
[dbo].[FakeData].[NormalColumn] >= @4 AND
[dbo].[FakeData].[AnotherColumn] <= @5 AND
[dbo].[FakeData].[AnotherValueColumn] IN (@6, @7, @8, @9)
ORDER BY [someschema].[Related].[RelatedProperty], [OtherAlias].[RelatedProperty] DESC, [dbo].[FakeData].[NormalColumn]
) SELECT 1  ELSE SELECT 0";

            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var target = new TransactSqlQueryFactory(definitionProvider);
            var actual = target.Create(transactionSelection.AsContains(definitionProvider.Resolve<DataRow>()));
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement direct data matches expected. Deletion statements ignore ORDER BY.
        /// </summary>
        [TestMethod]
        public void Create_DeletionStatementForDirectDataRaisedRow_MatchesExpected()
        {
            var match = new DataRow { ValueColumn = 2, NullableColumn = "CouldHaveBeenNull", NullableValueColumn = null };
            var baseline = new DataRow { FakeDataId = 10, NormalColumn = "Greater" };
            var boundary = new DataRow { FakeDataId = 20, AnotherColumn = "Less" };
            var transactionSelection = Select
                .From<DataRow>(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.NullableColumn,
                    row => row.NullableValueColumn,
                    row => row.ValueColumn,
                    row => row.AnotherColumn,
                    row => row.AnotherValueColumn)
                .InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId)
                .WhereEqual(row => row.ValueColumn, match.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                .Between(baseline, boundary, row => row.FakeDataId)
                .GreaterThanOrEqualTo(row => row.NormalColumn, baseline.NormalColumn)
                .LessThanOrEqualTo(row => row.AnotherColumn, boundary.AnotherColumn)
                .Include(row => row.AnotherValueColumn, 5, 10, 15, 20)
                .OrderBy(row => row.Related.RelatedProperty)
                .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                .OrderBy(row => row.NormalColumn);

            const string Expected = @"DELETE [dbo].[FakeData]
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[dbo].[FakeData].[FakeRowId] BETWEEN @2 AND @3 AND
[dbo].[FakeData].[NormalColumn] >= @4 AND
[dbo].[FakeData].[AnotherColumn] <= @5 AND
[dbo].[FakeData].[AnotherValueColumn] IN (@6, @7, @8, @9)";

            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var target = new TransactSqlQueryFactory(definitionProvider);
            var actual = target.Create(transactionSelection.AsDelete(definitionProvider.Resolve<DataRow>()));
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement related data matches expected.
        /// </summary>
        [TestMethod]
        public void Create_SelectionStatementForRelatedData_MatchesExpected()
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
            var transactionSelection = Select
                .From<DataRow>(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.Related.RelatedId,
                    row => row.Related.RelatedProperty,
                    row => row.Related.RelatedId,
                    row => row.RelatedAlias.RelatedProperty,
                    row => row.OtherAlias.RelatedId,
                    row => row.OtherAlias.RelatedProperty,
                    row => row.ParentFakeDataId)
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
                .Between(baseline, boundary, row => row.FakeDataId)
                .OrderBy(row => row.Related.RelatedProperty)
                .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                .OrderBy(row => row.NormalColumn);

            const string Expected = @"SELECT
    [dbo].[FakeData].[FakeRowId],
    [dbo].[FakeData].[NormalColumn],
    [someschema].[Related].[RelatedId] AS [Related.RelatedId],
    [someschema].[Related].[RelatedProperty] AS [Related.RelatedProperty],
    [someschema].[Related].[RelatedId] AS [Related.RelatedId],
    [RelatedAlias].[RelatedProperty] AS [RelatedAlias.RelatedProperty],
    [OtherAlias].[RelatedId] AS [OtherAlias.RelatedId],
    [OtherAlias].[RelatedProperty] AS [OtherAlias.RelatedProperty],
    [dbo].[SubData].[ParentFakeDataId]
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @2 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4
ORDER BY [someschema].[Related].[RelatedProperty], [OtherAlias].[RelatedProperty] DESC, [dbo].[FakeData].[NormalColumn]";

            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var target = new TransactSqlQueryFactory(definitionProvider);
            var actual = target.Create(transactionSelection.AsSelect(definitionProvider.Resolve<DataRow>()));
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement related data matches expected.
        /// </summary>
        [TestMethod]
        public void Create_CountStatementForRelatedData_MatchesExpected()
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
            var transactionSelection = Select.From<DataRow>()
                .Count(row => row.FakeDataId)
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

            const string Expected = @"SELECT
    COUNT([dbo].[FakeData].[FakeRowId])
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @2 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4";

            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var target = new TransactSqlQueryFactory(definitionProvider);
            var actual = target.Create(transactionSelection.AsSelect(definitionProvider.Resolve<DataRow>()));
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement related data matches expected.
        /// </summary>
        [TestMethod]
        public void Create_PageStatementForRelatedData_MatchesExpected()
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
            var transactionSelection = Select
                .From<DataRow>(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.Related.RelatedId,
                    row => row.Related.RelatedProperty,
                    row => row.Related.RelatedId,
                    row => row.RelatedAlias.RelatedProperty,
                    row => row.OtherAlias.RelatedId,
                    row => row.OtherAlias.RelatedProperty,
                    row => row.ParentFakeDataId)
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
                .Between(baseline, boundary, row => row.FakeDataId)
                .OrderBy(row => row.Related.RelatedProperty)
                .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                .OrderBy(row => row.NormalColumn)
                .Skip(5).Take(5);

            const string Expected = @";WITH pg AS
    (
        SELECT FakeRowId
        FROM [dbo].[FakeData]
        INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
        INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
        INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
        INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
        INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
        LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
        WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
        [dbo].[FakeData].[NullableColumn] LIKE @1 AND
        [dbo].[FakeData].[NullableValueColumn] IS NULL AND
        [RelatedAlias].[RelatedProperty] LIKE @2 AND
        [dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4
        ORDER BY [someschema].[Related].[RelatedProperty], [OtherAlias].[RelatedProperty] DESC, [dbo].[FakeData].[NormalColumn]
        OFFSET @5 ROWS
        FETCH NEXT @6 ROWS ONLY
    )
SELECT 
    [dbo].[FakeData].[FakeRowId],
    [dbo].[FakeData].[NormalColumn],
    [someschema].[Related].[RelatedId] AS [Related.RelatedId],
    [someschema].[Related].[RelatedProperty] AS [Related.RelatedProperty],
    [someschema].[Related].[RelatedId] AS [Related.RelatedId],
    [RelatedAlias].[RelatedProperty] AS [RelatedAlias.RelatedProperty],
    [OtherAlias].[RelatedId] AS [OtherAlias.RelatedId],
    [OtherAlias].[RelatedProperty] AS [OtherAlias.RelatedProperty],
    [dbo].[SubData].[ParentFakeDataId]
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE EXISTS 
(SELECT 1 FROM pg 
WHERE pg.FakeRowId = [dbo].[FakeData].[FakeRowId]) AND
[dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @2 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4
ORDER BY [someschema].[Related].[RelatedProperty], [OtherAlias].[RelatedProperty] DESC, [dbo].[FakeData].[NormalColumn] OPTION (RECOMPILE)";

            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var target = new TransactSqlQueryFactory(definitionProvider);
            var actual = target.Create(transactionSelection.AsSelect(definitionProvider.Resolve<DataRow>()));
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement related data matches expected.
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
            var transactionSelection = Select
                .From<DataRow>(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.Related.RelatedId,
                    row => row.Related.RelatedProperty,
                    row => row.Related.RelatedId,
                    row => row.RelatedAlias.RelatedProperty,
                    row => row.OtherAlias.RelatedId,
                    row => row.OtherAlias.RelatedProperty,
                    row => row.ParentFakeDataId)
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
                .Between(baseline, boundary, row => row.FakeDataId)
                .OrderBy(row => row.Related.RelatedProperty)
                .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                .OrderBy(row => row.NormalColumn);

            const string Expected = @"IF EXISTS (
SELECT
    1
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @2 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4
ORDER BY [someschema].[Related].[RelatedProperty], [OtherAlias].[RelatedProperty] DESC, [dbo].[FakeData].[NormalColumn]
) SELECT 1  ELSE SELECT 0";

            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var target = new TransactSqlQueryFactory(definitionProvider);
            var actual = target.Create(transactionSelection.AsContains(definitionProvider.Resolve<DataRow>()));
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement related data matches expected.
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
            var transactionSelection = Select
                .From<DataRow>(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.Related.RelatedId,
                    row => row.Related.RelatedProperty,
                    row => row.Related.RelatedId,
                    row => row.RelatedAlias.RelatedProperty,
                    row => row.OtherAlias.RelatedId,
                    row => row.OtherAlias.RelatedProperty,
                    row => row.ParentFakeDataId)
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

            const string Expected = @"DELETE [dbo].[FakeData]
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @2 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4";

            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var target = new TransactSqlQueryFactory(definitionProvider);
            var actual = target.Create(transactionSelection.AsDelete(definitionProvider.Resolve<DataRow>()));
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement related data matches expected.
        /// </summary>
        [TestMethod]
        public void Create_SelectionStatementForRelatedDataRaisedRow_MatchesExpected()
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
            var transactionSelection = Select
                .From<DataRow>(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.ParentFakeDataId,
                    row => row.Related.RelatedId,
                    row => row.Related.RelatedProperty,
                    row => row.RelatedAlias.RelatedId,
                    row => row.RelatedAlias.RelatedProperty,
                    row => row.OtherAlias.RelatedId,
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
                .Between(baseline, boundary, row => row.FakeDataId)
                .OrderBy(row => row.Related.RelatedProperty)
                .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                .OrderBy(row => row.NormalColumn);

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
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @2 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4
ORDER BY [someschema].[Related].[RelatedProperty], [OtherAlias].[RelatedProperty] DESC, [dbo].[FakeData].[NormalColumn]";

            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var target = new TransactSqlQueryFactory(definitionProvider);
            var actual = target.Create(transactionSelection.AsSelect(definitionProvider.Resolve<DataRow>()));
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement related data matches expected.
        /// </summary>
        [TestMethod]
        public void Create_CountStatementForRelatedDataRaisedRow_MatchesExpected()
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
            var transactionSelection = Select
                .From<DataRow>().Count(row => row.FakeDataId)
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

            const string Expected = @"SELECT
    COUNT([dbo].[FakeData].[FakeRowId])
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @2 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4";

            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var target = new TransactSqlQueryFactory(definitionProvider);
            var actual = target.Create(transactionSelection.AsSelect(definitionProvider.Resolve<DataRow>()));
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement related data matches expected.
        /// </summary>
        [TestMethod]
        public void Create_PageStatementForRelatedDataRaisedRow_MatchesExpected()
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
            var transactionSelection = Select
                .From<DataRow>(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.ParentFakeDataId,
                    row => row.Related.RelatedId,
                    row => row.Related.RelatedProperty,
                    row => row.RelatedAlias.RelatedId,
                    row => row.RelatedAlias.RelatedProperty,
                    row => row.OtherAlias.RelatedId,
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
                .Between(baseline, boundary, row => row.FakeDataId)
                .OrderBy(row => row.Related.RelatedProperty)
                .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                .OrderBy(row => row.NormalColumn)
                .Skip(5).Take(5);

            const string Expected = @";WITH pg AS
    (
        SELECT FakeRowId
        FROM [dbo].[FakeData]
        INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
        INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
        INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
        INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
        INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
        LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
        WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
        [dbo].[FakeData].[NullableColumn] LIKE @1 AND
        [dbo].[FakeData].[NullableValueColumn] IS NULL AND
        [RelatedAlias].[RelatedProperty] LIKE @2 AND
        [dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4
        ORDER BY [someschema].[Related].[RelatedProperty], [OtherAlias].[RelatedProperty] DESC, [dbo].[FakeData].[NormalColumn]
        OFFSET @5 ROWS
        FETCH NEXT @6 ROWS ONLY
    )
SELECT 
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
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE EXISTS 
(SELECT 1 FROM pg 
WHERE pg.FakeRowId = [dbo].[FakeData].[FakeRowId]) AND
[dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @2 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4
ORDER BY [someschema].[Related].[RelatedProperty], [OtherAlias].[RelatedProperty] DESC, [dbo].[FakeData].[NormalColumn] OPTION (RECOMPILE)";

            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var target = new TransactSqlQueryFactory(definitionProvider);
            var actual = target.Create(transactionSelection.AsSelect(definitionProvider.Resolve<DataRow>()));
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement related data matches expected.
        /// </summary>
        [TestMethod]
        public void Create_ContainsStatementForRelatedDataRaisedRow_MatchesExpected()
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
            var transactionSelection = Select
                .From<DataRow>(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.ParentFakeDataId,
                    row => row.Related.RelatedId,
                    row => row.Related.RelatedProperty,
                    row => row.RelatedAlias.RelatedId,
                    row => row.RelatedAlias.RelatedProperty,
                    row => row.OtherAlias.RelatedId,
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
                .Between(baseline, boundary, row => row.FakeDataId)
                .OrderBy(row => row.Related.RelatedProperty)
                .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                .OrderBy(row => row.NormalColumn);

            const string Expected = @"IF EXISTS (
SELECT
    1
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @2 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4
ORDER BY [someschema].[Related].[RelatedProperty], [OtherAlias].[RelatedProperty] DESC, [dbo].[FakeData].[NormalColumn]
) SELECT 1  ELSE SELECT 0";

            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var target = new TransactSqlQueryFactory(definitionProvider);
            var actual = target.Create(transactionSelection.AsContains(definitionProvider.Resolve<DataRow>()));
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement related data matches expected.
        /// </summary>
        [TestMethod]
        public void Create_DeletionStatementForRelatedDataRaisedRow_MatchesExpected()
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
            var transactionSelection = Select
                .From<DataRow>(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.ParentFakeDataId,
                    row => row.Related.RelatedId,
                    row => row.Related.RelatedProperty,
                    row => row.RelatedAlias.RelatedId,
                    row => row.RelatedAlias.RelatedProperty,
                    row => row.OtherAlias.RelatedId,
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

            const string Expected = @"DELETE [dbo].[FakeData]
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @2 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4";

            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var target = new TransactSqlQueryFactory(definitionProvider);
            var actual = target.Create(transactionSelection.AsDelete(definitionProvider.Resolve<DataRow>()));
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement union related data matches expected.
        /// </summary>
        [TestMethod]
        public void Create_SelectionStatementForUnionRelatedData_MatchesExpected()
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

            var transactionSelection = Select
                .From<DataRow>(
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
                .WhereEqual(row => row.ValueColumn, match1.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match1.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match1.NullableValueColumn)
                .WhereEqual(row => row.RelatedAlias.RelatedProperty, match1.RelatedAlias.RelatedProperty)
                .Between(baseline1, boundary1, row => row.FakeDataId)
                .OrderBy(row => row.Related.RelatedProperty)
                .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                .OrderBy(row => row.NormalColumn)
                .Union(
                    Select
                        .From<DataRow>(
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
                        .WhereEqual(row => row.ValueColumn, match2.ValueColumn)
                        .WhereEqual(row => row.NullableColumn, match2.NullableColumn)
                        .WhereEqual(row => row.NullableValueColumn, match2.NullableValueColumn)
                        .WhereEqual(row => row.RelatedAlias.RelatedProperty, match2.RelatedAlias.RelatedProperty)
                        .Between(baseline2, boundary2, row => row.FakeDataId)
                        .OrderBy(row => row.Related.RelatedProperty)
                        .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                        .OrderBy(row => row.NormalColumn)
                        .Union(
                            Select.From<DataRow>(
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
                                .WhereEqual(row => row.ValueColumn, match3.ValueColumn)
                                .WhereEqual(row => row.NullableColumn, match3.NullableColumn)
                                .WhereEqual(row => row.NullableValueColumn, match3.NullableValueColumn)
                                .WhereEqual(row => row.RelatedAlias.RelatedProperty, match3.RelatedAlias.RelatedProperty)
                                .Between(baseline3, boundary3, row => row.FakeDataId)
                                .OrderBy(row => row.Related.RelatedProperty)
                                .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                                .OrderBy(row => row.NormalColumn)));

            const string Expected = @"(SELECT
    [dbo].[FakeData].[FakeRowId],
    [dbo].[FakeData].[NormalColumn],
    [someschema].[Related].[RelatedId] AS [Related.RelatedId],
    [RelatedAlias].[RelatedProperty] AS [RelatedAlias.RelatedProperty],
    [OtherAlias].[RelatedProperty] AS [OtherAlias.RelatedProperty]
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @2 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4
ORDER BY [someschema].[Related].[RelatedProperty], [OtherAlias].[RelatedProperty] DESC, [dbo].[FakeData].[NormalColumn])
UNION
(SELECT
    [dbo].[FakeData].[FakeRowId],
    [dbo].[FakeData].[NormalColumn],
    [someschema].[Related].[RelatedId] AS [Related.RelatedId],
    [RelatedAlias].[RelatedProperty] AS [RelatedAlias.RelatedProperty],
    [OtherAlias].[RelatedProperty] AS [OtherAlias.RelatedProperty]
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @5 AND
[dbo].[FakeData].[NullableColumn] LIKE @6 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @7 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @8 AND @9
ORDER BY [someschema].[Related].[RelatedProperty], [OtherAlias].[RelatedProperty] DESC, [dbo].[FakeData].[NormalColumn])
UNION
(SELECT
    [dbo].[FakeData].[FakeRowId],
    [dbo].[FakeData].[NormalColumn],
    [someschema].[Related].[RelatedId] AS [Related.RelatedId],
    [RelatedAlias].[RelatedProperty] AS [RelatedAlias.RelatedProperty],
    [OtherAlias].[RelatedProperty] AS [OtherAlias.RelatedProperty]
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @10 AND
[dbo].[FakeData].[NullableColumn] LIKE @11 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @12 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @13 AND @14
ORDER BY [someschema].[Related].[RelatedProperty], [OtherAlias].[RelatedProperty] DESC, [dbo].[FakeData].[NormalColumn])";

            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var target = new TransactSqlQueryFactory(definitionProvider);
            var actual = target.Create(transactionSelection.AsSelect(definitionProvider.Resolve<DataRow>()));
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement union related data matches expected.
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

            var transactionSelection = Select
                .From<DataRow>(
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
                .WhereEqual(row => row.ValueColumn, match1.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match1.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match1.NullableValueColumn)
                .WhereEqual(row => row.RelatedAlias.RelatedProperty, match1.RelatedAlias.RelatedProperty)
                .Between(baseline1, boundary1, row => row.FakeDataId)
                .OrderBy(row => row.Related.RelatedProperty)
                .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                .OrderBy(row => row.NormalColumn)
                .Union(
                    Select
                        .From<DataRow>(
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
                        .WhereEqual(row => row.ValueColumn, match2.ValueColumn)
                        .WhereEqual(row => row.NullableColumn, match2.NullableColumn)
                        .WhereEqual(row => row.NullableValueColumn, match2.NullableValueColumn)
                        .WhereEqual(row => row.RelatedAlias.RelatedProperty, match2.RelatedAlias.RelatedProperty)
                        .Between(baseline2, boundary2, row => row.FakeDataId)
                        .OrderBy(row => row.Related.RelatedProperty)
                        .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                        .OrderBy(row => row.NormalColumn)
                        .Union(
                            Select.From<DataRow>(
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
                                .WhereEqual(row => row.ValueColumn, match3.ValueColumn)
                                .WhereEqual(row => row.NullableColumn, match3.NullableColumn)
                                .WhereEqual(row => row.NullableValueColumn, match3.NullableValueColumn)
                                .WhereEqual(row => row.RelatedAlias.RelatedProperty, match3.RelatedAlias.RelatedProperty)
                                .Between(baseline3, boundary3, row => row.FakeDataId)
                                .OrderBy(row => row.Related.RelatedProperty)
                                .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                                .OrderBy(row => row.NormalColumn)));

            const string Expected = @"IF EXISTS (
(SELECT
    1
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @2 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4
ORDER BY [someschema].[Related].[RelatedProperty], [OtherAlias].[RelatedProperty] DESC, [dbo].[FakeData].[NormalColumn])
UNION
(SELECT
    1
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @5 AND
[dbo].[FakeData].[NullableColumn] LIKE @6 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @7 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @8 AND @9
ORDER BY [someschema].[Related].[RelatedProperty], [OtherAlias].[RelatedProperty] DESC, [dbo].[FakeData].[NormalColumn])
UNION
(SELECT
    1
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @10 AND
[dbo].[FakeData].[NullableColumn] LIKE @11 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @12 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @13 AND @14
ORDER BY [someschema].[Related].[RelatedProperty], [OtherAlias].[RelatedProperty] DESC, [dbo].[FakeData].[NormalColumn])
) SELECT 1  ELSE SELECT 0";

            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var target = new TransactSqlQueryFactory(definitionProvider);
            var actual = target.Create(transactionSelection.AsContains(definitionProvider.Resolve<DataRow>()));
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement union related data matches expected. Deletion statements ignore UNIONs.
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

            var transactionSelection = Select
                .From<DataRow>(
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
                .WhereEqual(row => row.ValueColumn, match1.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match1.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match1.NullableValueColumn)
                .WhereEqual(row => row.RelatedAlias.RelatedProperty, match1.RelatedAlias.RelatedProperty)
                .Between(baseline1, boundary1, row => row.FakeDataId)
                .Union(
                    Select
                        .From<DataRow>(
                            row => row.FakeDataId,
                            row => row.NormalColumn,
                            row => row.Related.RelatedId,
                            row => row.RelatedAlias.RelatedProperty,
                            row => row.OtherAlias.RelatedProperty)
                        .WhereEqual(row => row.ValueColumn, match2.ValueColumn)
                        .WhereEqual(row => row.NullableColumn, match2.NullableColumn)
                        .WhereEqual(row => row.NullableValueColumn, match2.NullableValueColumn)
                        .WhereEqual(row => row.RelatedAlias.RelatedProperty, match2.RelatedAlias.RelatedProperty)
                        .Between(baseline2, boundary2, row => row.FakeDataId)
                        .Union(
                            Select.From<DataRow>(
                                    row => row.FakeDataId,
                                    row => row.NormalColumn,
                                    row => row.Related.RelatedId,
                                    row => row.RelatedAlias.RelatedProperty,
                                    row => row.OtherAlias.RelatedProperty)
                                .WhereEqual(row => row.ValueColumn, match3.ValueColumn)
                                .WhereEqual(row => row.NullableColumn, match3.NullableColumn)
                                .WhereEqual(row => row.NullableValueColumn, match3.NullableValueColumn)
                                .WhereEqual(row => row.RelatedAlias.RelatedProperty, match3.RelatedAlias.RelatedProperty)
                                .Between(baseline3, boundary3, row => row.FakeDataId)));

            const string Expected = @"DELETE [dbo].[FakeData]
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @2 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4";

            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var target = new TransactSqlQueryFactory(definitionProvider);
            var actual = target.Create(transactionSelection.AsDelete(definitionProvider.Resolve<DataRow>()));
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement union related data matches expected.
        /// </summary>
        [TestMethod]
        public void Create_SelectionStatementForUnionRelatedDataRaisedRow_MatchesExpected()
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

            var transactionSelection = Select
                .From<DataRow>(
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
                .WhereEqual(row => row.ValueColumn, match1.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match1.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match1.NullableValueColumn)
                .WhereEqual(row => row.RelatedAlias.RelatedProperty, "Related1")
                .Between(baseline1, boundary1, row => row.FakeDataId)
                .OrderBy(row => row.Related.RelatedProperty)
                .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                .OrderBy(row => row.NormalColumn)
                .Union(
                    Select
                        .From<DataRow>(
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
                        .WhereEqual(row => row.ValueColumn, match2.ValueColumn)
                        .WhereEqual(row => row.NullableColumn, match2.NullableColumn)
                        .WhereEqual(row => row.NullableValueColumn, match2.NullableValueColumn)
                        .WhereEqual(row => row.RelatedAlias.RelatedProperty, "Related2")
                        .Between(baseline2, boundary2, row => row.FakeDataId)
                        .OrderBy(row => row.Related.RelatedProperty)
                        .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                        .OrderBy(row => row.NormalColumn)
                        .Union(
                            Select
                                .From<DataRow>(
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
                                .WhereEqual(row => row.ValueColumn, match3.ValueColumn)
                                .WhereEqual(row => row.NullableColumn, match3.NullableColumn)
                                .WhereEqual(row => row.NullableValueColumn, match3.NullableValueColumn)
                                .WhereEqual(row => row.RelatedAlias.RelatedProperty, "Related3")
                                .Between(baseline3, boundary3, row => row.FakeDataId)
                                .OrderBy(row => row.Related.RelatedProperty)
                                .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                                .OrderBy(row => row.NormalColumn)));

            const string Expected = @"(SELECT
    [dbo].[FakeData].[FakeRowId],
    [dbo].[FakeData].[NormalColumn],
    [RelatedAlias].[RelatedId] AS [RelatedAlias.RelatedId],
    [RelatedAlias].[RelatedProperty] AS [RelatedAlias.RelatedProperty],
    [OtherAlias].[RelatedProperty] AS [OtherAlias.RelatedProperty]
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @2 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4
ORDER BY [someschema].[Related].[RelatedProperty], [OtherAlias].[RelatedProperty] DESC, [dbo].[FakeData].[NormalColumn])
UNION
(SELECT
    [dbo].[FakeData].[FakeRowId],
    [dbo].[FakeData].[NormalColumn],
    [RelatedAlias].[RelatedId] AS [RelatedAlias.RelatedId],
    [RelatedAlias].[RelatedProperty] AS [RelatedAlias.RelatedProperty],
    [OtherAlias].[RelatedProperty] AS [OtherAlias.RelatedProperty]
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @5 AND
[dbo].[FakeData].[NullableColumn] LIKE @6 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @7 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @8 AND @9
ORDER BY [someschema].[Related].[RelatedProperty], [OtherAlias].[RelatedProperty] DESC, [dbo].[FakeData].[NormalColumn])
UNION
(SELECT
    [dbo].[FakeData].[FakeRowId],
    [dbo].[FakeData].[NormalColumn],
    [RelatedAlias].[RelatedId] AS [RelatedAlias.RelatedId],
    [RelatedAlias].[RelatedProperty] AS [RelatedAlias.RelatedProperty],
    [OtherAlias].[RelatedProperty] AS [OtherAlias.RelatedProperty]
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @10 AND
[dbo].[FakeData].[NullableColumn] LIKE @11 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @12 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @13 AND @14
ORDER BY [someschema].[Related].[RelatedProperty], [OtherAlias].[RelatedProperty] DESC, [dbo].[FakeData].[NormalColumn])";

            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var target = new TransactSqlQueryFactory(definitionProvider);
            var actual = target.Create(transactionSelection.AsSelect(definitionProvider.Resolve<DataRow>()));
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement union related data matches expected.
        /// </summary>
        [TestMethod]
        public void Create_ContainsStatementForUnionRelatedDataRaisedRow_MatchesExpected()
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

            var transactionSelection = Select
                .From<DataRow>(
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
                .WhereEqual(row => row.ValueColumn, match1.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match1.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match1.NullableValueColumn)
                .WhereEqual(row => row.RelatedAlias.RelatedProperty, "Related1")
                .Between(baseline1, boundary1, row => row.FakeDataId)
                .OrderBy(row => row.Related.RelatedProperty)
                .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                .OrderBy(row => row.NormalColumn)
                .Union(
                    Select
                        .From<DataRow>(
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
                        .WhereEqual(row => row.ValueColumn, match2.ValueColumn)
                        .WhereEqual(row => row.NullableColumn, match2.NullableColumn)
                        .WhereEqual(row => row.NullableValueColumn, match2.NullableValueColumn)
                        .WhereEqual(row => row.RelatedAlias.RelatedProperty, "Related2")
                        .Between(baseline2, boundary2, row => row.FakeDataId)
                        .OrderBy(row => row.Related.RelatedProperty)
                        .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                        .OrderBy(row => row.NormalColumn)
                        .Union(
                            Select
                                .From<DataRow>(
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
                                .WhereEqual(row => row.ValueColumn, match3.ValueColumn)
                                .WhereEqual(row => row.NullableColumn, match3.NullableColumn)
                                .WhereEqual(row => row.NullableValueColumn, match3.NullableValueColumn)
                                .WhereEqual(row => row.RelatedAlias.RelatedProperty, "Related3")
                                .Between(baseline3, boundary3, row => row.FakeDataId)
                                .OrderBy(row => row.Related.RelatedProperty)
                                .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                                .OrderBy(row => row.NormalColumn)));

            const string Expected = @"IF EXISTS (
(SELECT
    1
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @2 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4
ORDER BY [someschema].[Related].[RelatedProperty], [OtherAlias].[RelatedProperty] DESC, [dbo].[FakeData].[NormalColumn])
UNION
(SELECT
    1
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @5 AND
[dbo].[FakeData].[NullableColumn] LIKE @6 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @7 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @8 AND @9
ORDER BY [someschema].[Related].[RelatedProperty], [OtherAlias].[RelatedProperty] DESC, [dbo].[FakeData].[NormalColumn])
UNION
(SELECT
    1
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @10 AND
[dbo].[FakeData].[NullableColumn] LIKE @11 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @12 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @13 AND @14
ORDER BY [someschema].[Related].[RelatedProperty], [OtherAlias].[RelatedProperty] DESC, [dbo].[FakeData].[NormalColumn])
) SELECT 1  ELSE SELECT 0";

            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var target = new TransactSqlQueryFactory(definitionProvider);
            var actual = target.Create(transactionSelection.AsContains(definitionProvider.Resolve<DataRow>()));
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement union related data matches expected. Deletion statements ignore linked statements.
        /// </summary>
        [TestMethod]
        public void Create_DeletionStatementForUnionRelatedDataRaisedRow_MatchesExpected()
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

            var transactionSelection = Select
                .From<DataRow>(
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
                .WhereEqual(row => row.ValueColumn, match1.ValueColumn)
                .WhereEqual(row => row.NullableColumn, match1.NullableColumn)
                .WhereEqual(row => row.NullableValueColumn, match1.NullableValueColumn)
                .WhereEqual(row => row.RelatedAlias.RelatedProperty, "Related1")
                .Between(baseline1, boundary1, row => row.FakeDataId)
                .Union(
                    Select
                        .From<DataRow>(
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
                        .WhereEqual(row => row.ValueColumn, match2.ValueColumn)
                        .WhereEqual(row => row.NullableColumn, match2.NullableColumn)
                        .WhereEqual(row => row.NullableValueColumn, match2.NullableValueColumn)
                        .WhereEqual(row => row.RelatedAlias.RelatedProperty, "Related2")
                        .Between(baseline2, boundary2, row => row.FakeDataId)
                        .Union(
                            Select
                                .From<DataRow>(
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
                                .WhereEqual(row => row.ValueColumn, match3.ValueColumn)
                                .WhereEqual(row => row.NullableColumn, match3.NullableColumn)
                                .WhereEqual(row => row.NullableValueColumn, match3.NullableValueColumn)
                                .WhereEqual(row => row.RelatedAlias.RelatedProperty, "Related3")
                                .Between(baseline3, boundary3, row => row.FakeDataId)));

            const string Expected = @"DELETE [dbo].[FakeData]
FROM [dbo].[FakeData]
INNER JOIN [someschema].[Related] ON [dbo].[FakeData].[FakeRowId] = [someschema].[Related].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] ON [someschema].[Related].[RelatedId] = [dbo].[DependencyEntity].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [OtherAlias] ON [dbo].[FakeData].[FakeRowId] = [OtherAlias].[FakeDataId]
INNER JOIN [dbo].[DependencyEntity] AS [RelatedDependency] ON [OtherAlias].[RelatedId] = [RelatedDependency].[ComplexEntityId]
INNER JOIN [someschema].[Related] AS [RelatedAlias] ON [dbo].[FakeData].[FakeRowId] = [RelatedAlias].[FakeDataId]
LEFT JOIN [dbo].[SubData] ON [dbo].[FakeData].[FakeRowId] = [dbo].[SubData].[FakeSubDataId]
WHERE [dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @2 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4";

            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var target = new TransactSqlQueryFactory(definitionProvider);
            var actual = target.Create(transactionSelection.AsDelete(definitionProvider.Resolve<DataRow>()));
            Assert.AreEqual(Expected, actual);
        }

        #endregion
    }
}