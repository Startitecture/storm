// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactSqlAdapterTests.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

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
    public class TransactSqlAdapterTests
    {
        #region Public Methods and Operators

        /// <summary>
        /// The create insertion statement with identity column matches expected.
        /// </summary>
        [TestMethod]
        public void CreateInsertionStatement_WithIdentityColumn_MatchesExpected()
        {
            const string Expected = @"DECLARE @NewId int
INSERT INTO [dbo].[FakeData]
([NormalColumn], [NullableColumn], [ValueColumn], [AnotherValueColumn], [AnotherColumn], [NullableValueColumn])
VALUES (@0, @1, @2, @3, @4, @5)
SET @NewId = SCOPE_IDENTITY()
SELECT @NewId";

            var target = new TransactSqlAdapter(new DataAnnotationsDefinitionProvider());
            var actual = target.CreateInsertionStatement<DataRow>();
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The create insertion statement without identity column matches expected.
        /// </summary>
        [TestMethod]
        public void CreateInsertionStatement_WithoutIdentityColumn_MatchesExpected()
        {
            const string Expected = @"INSERT INTO [dbo].[DependentEntity]
([FakeDependentEntityId], [DependentIntegerValue], [DependentTimeValue])
VALUES (@0, @1, @2)";

            var target = new TransactSqlAdapter(new DataAnnotationsDefinitionProvider());
            var actual = target.CreateInsertionStatement<DependentRow>();
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement direct data matches expected.
        /// </summary>
        [TestMethod]
        public void CreateSelectionStatement_DirectData_MatchesExpected()
        {
            var transactionSelection = Query
                .Select<DataRow>(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.NullableColumn,
                    row => row.NullableValueColumn,
                    row => row.ValueColumn,
                    row => row.AnotherColumn,
                    row => row.AnotherValueColumn)
                .From(
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
            var target = new TransactSqlAdapter(definitionProvider);
            var actual = target.CreateSelectionStatement(transactionSelection);
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement direct data matches expected.
        /// </summary>
        [TestMethod]
        public void CreateSelectionStatement_DirectDataDefaultColumns_AllColumnsIncluded()
        {
            var transactionSelection = Query.Select<DataRow>()
                .From(
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
            var target = new TransactSqlAdapter(definitionProvider);
            var actual = target.CreateSelectionStatement(transactionSelection);
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement direct data matches expected.
        /// </summary>
        [TestMethod]
        public void CreateSelectionStatement_CountDirectData_MatchesExpected()
        {
            var transactionSelection = Query.Select<DataRow>()
                .Count(row => row.FakeDataId)
                .From(
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
                        .Include(row => row.AnotherValueColumn, 5, 10, 15, 20));

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
            var target = new TransactSqlAdapter(definitionProvider);
            var actual = target.CreateSelectionStatement(transactionSelection);
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement direct data matches expected.
        /// </summary>
        [TestMethod]
        public void CreateSelectionStatement_PagedDirectData_MatchesExpected()
        {
            var tableExpression = Query.SelectEntities<DataRow>(
                select => select.Select(row => row.FakeDataId)
                    .From(
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
                            .OrderBy(row => row.NormalColumn))
                    .Seek(subset => subset.Skip(5).Take(5)));

            var transactionSelection = Query.With(tableExpression, "pgCte") 
                .ForSelection<DataRow>(matches => matches.On(row => row.FakeDataId, row => row.FakeDataId))
                .Select(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.NullableColumn,
                    row => row.NullableValueColumn,
                    row => row.ValueColumn,
                    row => row.AnotherColumn,
                    row => row.AnotherValueColumn)
                .From(
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
                ////.WithAs(tableExpression, "pgCte", relationSet => relationSet.InnerJoin()));

            const string Expected = @";WITH [pgCte] AS
    (
        SELECT
            [dbo].[FakeData].[FakeRowId]
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
        [dbo].[FakeData].[FakeRowId] BETWEEN @12 AND @13 AND
        [dbo].[FakeData].[NormalColumn] >= @14 AND
        [dbo].[FakeData].[AnotherColumn] <= @15 AND
        [dbo].[FakeData].[AnotherValueColumn] IN (@16, @17, @18, @19)
        ORDER BY [someschema].[Related].[RelatedProperty], [OtherAlias].[RelatedProperty] DESC, [dbo].[FakeData].[NormalColumn]
        OFFSET @20 ROWS
        FETCH NEXT @21 ROWS ONLY
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
(SELECT 1 FROM [pgCte] 
WHERE [pgCte].[FakeRowId] = [dbo].[FakeData].[FakeRowId]) AND
[dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[dbo].[FakeData].[FakeRowId] BETWEEN @2 AND @3 AND
[dbo].[FakeData].[NormalColumn] >= @4 AND
[dbo].[FakeData].[AnotherColumn] <= @5 AND
[dbo].[FakeData].[AnotherValueColumn] IN (@6, @7, @8, @9)
ORDER BY [someschema].[Related].[RelatedProperty], [OtherAlias].[RelatedProperty] DESC, [dbo].[FakeData].[NormalColumn] OPTION (RECOMPILE)";

            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var target = new TransactSqlAdapter(definitionProvider);
            var actual = target.CreateSelectionStatement(transactionSelection);
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The create selection statement with exists in matches expected.
        /// </summary>
        [TestMethod]
        public void CreateSelectionStatement_WithExistsIn_MatchesExpected()
        {
            var query = Query
                .Select<DomainAggregateRow>(
                    row => row.Name,
                    row => row.Description,
                    row => row.SubContainer.Name,
                    row => row.SubContainer.TopContainer.Name,
                    row => row.Template.Name)
                .From(
                    set => set.InnerJoin(row => row.SubContainer.SubContainerId, row => row.SubContainerId)
                        .InnerJoin(row => row.SubContainer.TopContainer.TopContainerId, row => row.SubContainer.TopContainerId))
                .Where(
                    set => set.AreEqual(row => row.SubContainerId, 6)
                        .ExistsIn(
                            row => row.DomainAggregateId,
                            Query.From<DomainAggregateFlagAttributeRow>(
                                    set2 => set2.InnerJoin<FlagAttributeRow>(row => row.FlagAttributeId, row => row.FlagAttributeId))
                                .Where(set2 => set2.Include((FlagAttributeRow row) => row.Name, "type1", "type2", "type3")),
                            row => row.DomainAggregateId)
                        .Include(row => row.SubContainerId, 3, 6, 7));

            const string Expected = @"SELECT
    [dbo].[DomainAggregate].[Name],
    [dbo].[DomainAggregate].[Description],
    [dbo].[SubContainer].[Name] AS [SubContainer.Name],
    [dbo].[TopContainer].[Name] AS [TopContainer.Name],
    [dbo].[Template].[Name] AS [Template.Name]
FROM [dbo].[DomainAggregate]
INNER JOIN [dbo].[DomainAggregate] ON [dbo].[SubContainer].[SubContainerId] = [dbo].[DomainAggregate].[SubContainerId]
INNER JOIN [dbo].[SubContainer] ON [dbo].[TopContainer].[TopContainerId] = [dbo].[SubContainer].[TopContainerId]
WHERE [dbo].[DomainAggregate].[SubContainerId] = @0 AND
[dbo].[DomainAggregate].[DomainAggregateId] IN
(SELECT [DomainAggregateFlagAttribute1].[DomainAggregateId]
FROM [dbo].[DomainAggregateFlagAttribute] AS [DomainAggregateFlagAttribute1]
INNER JOIN [dbo].[FlagAttribute] AS [FlagAttribute1] ON [DomainAggregateFlagAttribute1].[FlagAttributeId] = [FlagAttribute1].[FlagAttributeId]
WHERE [FlagAttribute1].[Name] IN (@1, @2, @3)) AND
[dbo].[DomainAggregate].[SubContainerId] IN (@4, @5, @6)";

            var actual = new TransactSqlAdapter(new DataAnnotationsDefinitionProvider()).CreateSelectionStatement(query);
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
            var target = new TransactSqlAdapter(definitionProvider);
            var actual = target.CreateExistsStatement(transactionSelection);
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The update statement_ direct data_ matches expected.
        /// </summary>
        [TestMethod]
        public void CreateUpdateStatement_DirectData_MatchesExpected()
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

            var updateSet = new UpdateSet<DataRow>().Set(match, new DataAnnotationsDefinitionProvider())
                .From(
                    set => set.InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                        .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                        .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                        .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId))
                .Where(
                    set => set.AreEqual(row => row.ValueColumn, match.ValueColumn)
                        .AreEqual(row => row.NullableColumn, match.NullableColumn)
                        .AreEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                        .Between(baseline, boundary, row => row.FakeDataId));

            const string Expected = @"UPDATE [dbo].[FakeData]
SET
[NormalColumn] = @0,
[NullableColumn] = @1,
[ValueColumn] = @2,
[AnotherValueColumn] = @3,
[AnotherColumn] = @4,
[NullableValueColumn] = NULL
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

            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var target = new TransactSqlAdapter(definitionProvider);
            var actual = target.CreateUpdateStatement(updateSet);
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The update statement_ direct data_ matches expected.
        /// </summary>
        [TestMethod]
        public void CreateUpdateStatement_DirectDataSpecificSetValues_MatchesExpected()
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
                
            var dataRow = new DataRow { NormalColumn = "UpdatedNormalColumn", NullableColumn = null };
            var updateSet = new UpdateSet<DataRow>().Set(dataRow, row => row.NormalColumn, row => row.NullableColumn)
                .From(
                    set => set.InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                        .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                        .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                        .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId))
                .Where(
                    set => set.AreEqual(row => row.ValueColumn, match.ValueColumn)
                        .AreEqual(row => row.NullableColumn, match.NullableColumn)
                        .AreEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                        .Between(baseline, boundary, row => row.FakeDataId));

            const string Expected = @"UPDATE [dbo].[FakeData]
SET
[NormalColumn] = @0,
[NullableColumn] = NULL
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

            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var target = new TransactSqlAdapter(definitionProvider);
            var actual = target.CreateUpdateStatement(updateSet);
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement direct data matches expected.
        /// </summary>
        [TestMethod]
        public void CreateDeletionStatement_DirectData_MatchesExpected()
        {
            var transactionSelection = Query
                .Select<DataRow>(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.NullableColumn,
                    row => row.NullableValueColumn,
                    row => row.ValueColumn,
                    row => row.AnotherColumn,
                    row => row.AnotherValueColumn)
                .From(
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
                        .Include(row => row.AnotherValueColumn, 5, 10, 15, 20));

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
            var target = new TransactSqlAdapter(definitionProvider);
            var actual = target.CreateDeletionStatement(transactionSelection);
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement direct data matches expected.
        /// </summary>
        [TestMethod]
        public void CreateSelectionStatement_DirectDataRaisedRow_MatchesExpected()
        {
            var transactionSelection = Query
                .Select<DataRow>(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.NullableColumn,
                    row => row.NullableValueColumn,
                    row => row.ValueColumn,
                    row => row.AnotherColumn,
                    row => row.AnotherValueColumn)
                .From(
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
            var target = new TransactSqlAdapter(definitionProvider);
            var actual = target.CreateSelectionStatement(transactionSelection);
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement direct data matches expected.
        /// </summary>
        [TestMethod]
        public void CreateSelectionStatement_CountDirectDataRaisedRow_MatchesExpected()
        {
            var transactionSelection = Query.Select<DataRow>()
                .Count(row => row.FakeDataId)
                .From(
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
                        .Include(row => row.AnotherValueColumn, 5, 10, 15, 20));

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
            var target = new TransactSqlAdapter(definitionProvider);
            var actual = target.CreateSelectionStatement(transactionSelection);
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement direct data matches expected.
        /// </summary>
        [TestMethod]
        public void CreateSelectionStatement_PagedDirectDataRaisedRow_MatchesExpected()
        {
            var tableExpression = Query.SelectEntities<DataRow>(
                select => select.Select(row => row.FakeDataId)
                    .From(
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
                            .OrderBy(row => row.NormalColumn))
                    .Seek(subset => subset.Skip(5).Take(5)));

            var transactionSelection = Query.With(tableExpression, "pgCte")
                .ForSelection<DataRow>(matches => matches.On(row => row.FakeDataId, row => row.FakeDataId))
                .Select(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.NullableColumn,
                    row => row.NullableValueColumn,
                    row => row.ValueColumn,
                    row => row.AnotherColumn,
                    row => row.AnotherValueColumn)
                .From(
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
                        ////.WithAs(tableExpression, "pgCte", relationSet => relationSet.InnerJoin()));

            const string Expected = @";WITH [pgCte] AS
    (
        SELECT
            [dbo].[FakeData].[FakeRowId]
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
        [dbo].[FakeData].[FakeRowId] BETWEEN @12 AND @13 AND
        [dbo].[FakeData].[NormalColumn] >= @14 AND
        [dbo].[FakeData].[AnotherColumn] <= @15 AND
        [dbo].[FakeData].[AnotherValueColumn] IN (@16, @17, @18, @19)
        ORDER BY [someschema].[Related].[RelatedProperty], [OtherAlias].[RelatedProperty] DESC, [dbo].[FakeData].[NormalColumn]
        OFFSET @20 ROWS
        FETCH NEXT @21 ROWS ONLY
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
(SELECT 1 FROM [pgCte] 
WHERE [pgCte].[FakeRowId] = [dbo].[FakeData].[FakeRowId]) AND
[dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[dbo].[FakeData].[FakeRowId] BETWEEN @2 AND @3 AND
[dbo].[FakeData].[NormalColumn] >= @4 AND
[dbo].[FakeData].[AnotherColumn] <= @5 AND
[dbo].[FakeData].[AnotherValueColumn] IN (@6, @7, @8, @9)
ORDER BY [someschema].[Related].[RelatedProperty], [OtherAlias].[RelatedProperty] DESC, [dbo].[FakeData].[NormalColumn] OPTION (RECOMPILE)";

            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var target = new TransactSqlAdapter(definitionProvider);
            var actual = target.CreateSelectionStatement(transactionSelection);
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement direct data matches expected.
        /// </summary>
        [TestMethod]
        public void CreateExistsStatement_DirectDataRaisedRow_MatchesExpected()
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
            var target = new TransactSqlAdapter(definitionProvider);
            var actual = target.CreateExistsStatement(transactionSelection);
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The update statement_ direct data_ matches expected.
        /// </summary>
        [TestMethod]
        public void CreateUpdateStatement_RaisedDirectData_MatchesExpected()
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

            var updateSet = new UpdateSet<DataRow>().Set(match, new DataAnnotationsDefinitionProvider())
                .From(
                    set => set.InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                        .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                        .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                        .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId))
                .Where(
                    set => set.AreEqual(row => row.ValueColumn, match.ValueColumn)
                        .AreEqual(row => row.NullableColumn, match.NullableColumn)
                        .AreEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                        .Between(baseline, boundary, row => row.FakeDataId));

            const string Expected = @"UPDATE [dbo].[FakeData]
SET
[NormalColumn] = @0,
[NullableColumn] = @1,
[ValueColumn] = @2,
[AnotherValueColumn] = @3,
[AnotherColumn] = @4,
[NullableValueColumn] = NULL
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

            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var target = new TransactSqlAdapter(definitionProvider);
            var actual = target.CreateUpdateStatement(updateSet);
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The update statement_ direct data_ matches expected.
        /// </summary>
        [TestMethod]
        public void CreateUpdateStatement_RaisedDirectDataSpecificSetValues_MatchesExpected()
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

            var dataRow = new DataRow { NormalColumn = "UpdatedNormalColumn", NullableColumn = null };
            var updateSet = new UpdateSet<DataRow>().Set(dataRow, row => row.NormalColumn, row => row.NullableColumn)
                .From(
                    set => set.InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                        .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                        .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                        .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId))
                .Where(
                    set => set.AreEqual(row => row.ValueColumn, match.ValueColumn)
                        .AreEqual(row => row.NullableColumn, match.NullableColumn)
                        .AreEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                        .Between(baseline, boundary, row => row.FakeDataId));

            const string Expected = @"UPDATE [dbo].[FakeData]
SET
[NormalColumn] = @0,
[NullableColumn] = NULL
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

            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var target = new TransactSqlAdapter(definitionProvider);
            var actual = target.CreateUpdateStatement(updateSet);
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement direct data matches expected. Deletion statements ignore ORDER BY.
        /// </summary>
        [TestMethod]
        public void CreateDeletionStatement_DirectDataRaisedRow_MatchesExpected()
        {
            var transactionSelection = Query
                .Select<DataRow>(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.NullableColumn,
                    row => row.NullableValueColumn,
                    row => row.ValueColumn,
                    row => row.AnotherColumn,
                    row => row.AnotherValueColumn)
                .From(
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
            var target = new TransactSqlAdapter(definitionProvider);
            var actual = target.CreateDeletionStatement(transactionSelection);
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement related data matches expected.
        /// </summary>
        [TestMethod]
        public void CreateSelectionStatement_RelatedData_MatchesExpected()
        {
            var transactionSelection = Query
                .Select<DataRow>(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.Related.RelatedId,
                    row => row.Related.RelatedProperty,
                    row => row.Related.RelatedId,
                    row => row.RelatedAlias.RelatedProperty,
                    row => row.OtherAlias.RelatedId,
                    row => row.OtherAlias.RelatedProperty,
                    row => row.ParentFakeDataId)
                .From(
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
                        .AreEqual(row => row.RelatedAlias.RelatedProperty, "Related")
                        .Between(row => row.FakeDataId, 10, 20))
                .Sort(
                    set => set.OrderBy(row => row.Related.RelatedProperty)
                        .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                        .OrderBy(row => row.NormalColumn));

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
            var target = new TransactSqlAdapter(definitionProvider);
            var actual = target.CreateSelectionStatement(transactionSelection);
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement related data matches expected.
        /// </summary>
        [TestMethod]
        public void CreateSelectionStatement_CountRelatedData_MatchesExpected()
        {
            var transactionSelection = Query.Select<DataRow>()
                .Count(row => row.FakeDataId)
                .From(
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
                        .AreEqual(row => row.RelatedAlias.RelatedProperty, "Related")
                        .Between(row => row.FakeDataId, 10, 20));

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
            var target = new TransactSqlAdapter(definitionProvider);
            var actual = target.CreateSelectionStatement(transactionSelection);
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement related data matches expected.
        /// </summary>
        [TestMethod]
        public void CreateSelectionStatement_PagedRelatedData_MatchesExpected()
        {
            var tableExpression = Query.SelectEntities<DataRow>(
                select => select.Select(row => row.FakeDataId)
                    .From(
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
                            .AreEqual(row => row.RelatedAlias.RelatedProperty, "Related")
                            .Between(row => row.FakeDataId, 10, 20))
                    .Sort(
                        set => set.OrderBy(row => row.Related.RelatedProperty)
                            .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                            .OrderBy(row => row.NormalColumn))
                    .Seek(subset => subset.Skip(5).Take(5)));

            var transactionSelection = Query.With(tableExpression, "pgCte")
                .ForSelection<DataRow>(matches => matches.On(row => row.FakeDataId, row => row.FakeDataId))
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
                .From(
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
                        .AreEqual(row => row.RelatedAlias.RelatedProperty, "Related")
                        .Between(row => row.FakeDataId, 10, 20))
                .Sort(
                    set => set.OrderBy(row => row.Related.RelatedProperty)
                        .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                        .OrderBy(row => row.NormalColumn));
                        ////.WithAs(tableExpression, "pgCte", set => set.InnerJoin()));

            const string Expected = @";WITH [pgCte] AS
    (
        SELECT
            [dbo].[FakeData].[FakeRowId]
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
        ORDER BY [someschema].[Related].[RelatedProperty], [OtherAlias].[RelatedProperty] DESC, [dbo].[FakeData].[NormalColumn]
        OFFSET @10 ROWS
        FETCH NEXT @11 ROWS ONLY
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
(SELECT 1 FROM [pgCte] 
WHERE [pgCte].[FakeRowId] = [dbo].[FakeData].[FakeRowId]) AND
[dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @2 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4
ORDER BY [someschema].[Related].[RelatedProperty], [OtherAlias].[RelatedProperty] DESC, [dbo].[FakeData].[NormalColumn] OPTION (RECOMPILE)";

            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var target = new TransactSqlAdapter(definitionProvider);
            var actual = target.CreateSelectionStatement(transactionSelection);
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement related data matches expected.
        /// </summary>
        [TestMethod]
        public void CreateExistsStatement_RelatedData_MatchesExpected()
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
                        .AreEqual(row => row.RelatedAlias.RelatedProperty, "Related")
                        .Between(row => row.FakeDataId, 10, 20))
                .Sort(
                    set => set.OrderBy(row => row.Related.RelatedProperty)
                        .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                        .OrderBy(row => row.NormalColumn));

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
            var target = new TransactSqlAdapter(definitionProvider);
            var actual = target.CreateExistsStatement(transactionSelection);
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The update statement_ related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void CreateUpdateStatement_RelatedData_MatchesExpected()
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

            var updateSet = new UpdateSet<DataRow>().Set(match, new DataAnnotationsDefinitionProvider())
                .From(
                    set => set.InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                        .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                        .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                        .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId))
                .Where(
                    set => set.AreEqual(row => row.ValueColumn, match.ValueColumn)
                        .AreEqual(row => row.NullableColumn, match.NullableColumn)
                        .AreEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                        .AreEqual(row => row.RelatedAlias.RelatedProperty, match.RelatedAlias.RelatedProperty)
                        .Between(baseline, boundary, row => row.FakeDataId));

            const string Expected = @"UPDATE [dbo].[FakeData]
SET
[NormalColumn] = @0,
[NullableColumn] = @1,
[ValueColumn] = @2,
[AnotherValueColumn] = @3,
[AnotherColumn] = @4,
[NullableValueColumn] = NULL
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

            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var target = new TransactSqlAdapter(definitionProvider);
            var actual = target.CreateUpdateStatement(updateSet);
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The update statement_ related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void CreateUpdateStatement_RelatedDataSpecificSetValues_MatchesExpected()
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

            var dataRow = new DataRow { NormalColumn = "UpdatedNormalColumn", NullableColumn = null };
            var updateSet = new UpdateSet<DataRow>().Set(dataRow, row => row.NormalColumn, row => row.NullableColumn)
                .From(
                    set => set.InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                        .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                        .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                        .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId))
                .Where(
                    set => set.AreEqual(row => row.ValueColumn, match.ValueColumn)
                        .AreEqual(row => row.NullableColumn, match.NullableColumn)
                        .AreEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                        .AreEqual(row => row.RelatedAlias.RelatedProperty, match.RelatedAlias.RelatedProperty)
                        .Between(baseline, boundary, row => row.FakeDataId));

            const string Expected = @"UPDATE [dbo].[FakeData]
SET
[NormalColumn] = @0,
[NullableColumn] = NULL
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

            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var target = new TransactSqlAdapter(definitionProvider);
            var actual = target.CreateUpdateStatement(updateSet);
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement related data matches expected.
        /// </summary>
        [TestMethod]
        public void CreateDeletionStatement_RelatedData_MatchesExpected()
        {
            var transactionSelection = Query
                .Select<DataRow>(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.Related.RelatedId,
                    row => row.Related.RelatedProperty,
                    row => row.Related.RelatedId,
                    row => row.RelatedAlias.RelatedProperty,
                    row => row.OtherAlias.RelatedId,
                    row => row.OtherAlias.RelatedProperty,
                    row => row.ParentFakeDataId)
                .From(
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
                        .AreEqual(row => row.RelatedAlias.RelatedProperty, "Related")
                        .Between(row => row.FakeDataId, 10, 20));

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
            var target = new TransactSqlAdapter(definitionProvider);
            var actual = target.CreateDeletionStatement(transactionSelection);
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement related data matches expected.
        /// </summary>
        [TestMethod]
        public void CreateSelectionStatement_RelatedDataRaisedRow_MatchesExpected()
        {
            var transactionSelection = Query
                .Select<DataRow>(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.ParentFakeDataId,
                    row => row.Related.RelatedId,
                    row => row.Related.RelatedProperty,
                    row => row.RelatedAlias.RelatedId,
                    row => row.RelatedAlias.RelatedProperty,
                    row => row.OtherAlias.RelatedId,
                    row => row.OtherAlias.RelatedProperty)
                .From(
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
                        .AreEqual(row => row.RelatedAlias.RelatedProperty, "Related")
                        .Between(row => row.FakeDataId, 10, 20))
                .Sort(
                    set => set.OrderBy(row => row.Related.RelatedProperty)
                        .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                        .OrderBy(row => row.NormalColumn));

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
            var target = new TransactSqlAdapter(definitionProvider);
            var actual = target.CreateSelectionStatement(transactionSelection);
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement related data matches expected.
        /// </summary>
        [TestMethod]
        public void CreateSelectionStatement_CountRelatedDataRaisedRow_MatchesExpected()
        {
            var transactionSelection = Query.Select<DataRow>()
                .Count(row => row.FakeDataId)
                .From(
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
                        .AreEqual(row => row.RelatedAlias.RelatedProperty, "Related")
                        .Between(row => row.FakeDataId, 10, 20));

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
            var target = new TransactSqlAdapter(definitionProvider);
            var actual = target.CreateSelectionStatement(transactionSelection);
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement related data matches expected.
        /// </summary>
        [TestMethod]
        public void CreateSelectionStatement_PagedRelatedDataRaisedRow_MatchesExpected()
        {
            var tableExpression = Query.SelectEntities<DataRow>(
                select => select.Select(row => row.FakeDataId)
                    .From(
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
                            .AreEqual(row => row.RelatedAlias.RelatedProperty, "Related")
                            .Between(row => row.FakeDataId, 10, 20))
                    .Sort(
                        set => set.OrderBy(row => row.Related.RelatedProperty)
                            .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                            .OrderBy(row => row.NormalColumn))
                    .Seek(subset => subset.Skip(5).Take(5)));

            var transactionSelection = Query.With(tableExpression, "pgCte")
                .ForSelection<DataRow>(matches => matches.On(row => row.FakeDataId, row => row.FakeDataId))
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
                .From(
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
                        .AreEqual(row => row.RelatedAlias.RelatedProperty, "Related")
                        .Between(row => row.FakeDataId, 10, 20))
                .Sort(
                    set => set.OrderBy(row => row.Related.RelatedProperty)
                        .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                        .OrderBy(row => row.NormalColumn));
                        ////.WithAs(tableExpression, "pgCte", set => set.InnerJoin()));

            const string Expected = @";WITH [pgCte] AS
    (
        SELECT
            [dbo].[FakeData].[FakeRowId]
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
        ORDER BY [someschema].[Related].[RelatedProperty], [OtherAlias].[RelatedProperty] DESC, [dbo].[FakeData].[NormalColumn]
        OFFSET @10 ROWS
        FETCH NEXT @11 ROWS ONLY
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
(SELECT 1 FROM [pgCte] 
WHERE [pgCte].[FakeRowId] = [dbo].[FakeData].[FakeRowId]) AND
[dbo].[FakeData].[ValueColumn] = @0 AND
[dbo].[FakeData].[NullableColumn] LIKE @1 AND
[dbo].[FakeData].[NullableValueColumn] IS NULL AND
[RelatedAlias].[RelatedProperty] LIKE @2 AND
[dbo].[FakeData].[FakeRowId] BETWEEN @3 AND @4
ORDER BY [someschema].[Related].[RelatedProperty], [OtherAlias].[RelatedProperty] DESC, [dbo].[FakeData].[NormalColumn] OPTION (RECOMPILE)";

            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var target = new TransactSqlAdapter(definitionProvider);
            var actual = target.CreateSelectionStatement(transactionSelection);
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement related data matches expected.
        /// </summary>
        [TestMethod]
        public void CreateExistsStatement_RelatedDataRaisedRow_MatchesExpected()
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
                        .AreEqual(row => row.RelatedAlias.RelatedProperty, "Related")
                        .Between(row => row.FakeDataId, 10, 20))
                .Sort(
                    set => set.OrderBy(row => row.Related.RelatedProperty)
                        .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                        .OrderBy(row => row.NormalColumn));

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
            var target = new TransactSqlAdapter(definitionProvider);
            var actual = target.CreateExistsStatement(transactionSelection);
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The update statement_ related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void CreateUpdateStatement_RaisedRelatedData_MatchesExpected()
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

            var updateSet = new UpdateSet<DataRow>().Set(match, new DataAnnotationsDefinitionProvider())
                .From(
                    set => set.InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                        .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                        .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                        .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId))
                .Where(
                    set => set.AreEqual(row => row.ValueColumn, match.ValueColumn)
                        .AreEqual(row => row.NullableColumn, match.NullableColumn)
                        .AreEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                        .AreEqual(row => row.RelatedAlias.RelatedProperty, "Related")
                        .Between(baseline, boundary, row => row.FakeDataId));

            const string Expected = @"UPDATE [dbo].[FakeData]
SET
[NormalColumn] = @0,
[NullableColumn] = @1,
[ValueColumn] = @2,
[AnotherValueColumn] = @3,
[AnotherColumn] = @4,
[NullableValueColumn] = NULL
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

            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var target = new TransactSqlAdapter(definitionProvider);
            var actual = target.CreateUpdateStatement(updateSet);
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The update statement_ related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void CreateUpdateStatement_RaisedRelatedDataSpecificSetValues_MatchesExpected()
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

            var dataRow = new DataRow { NormalColumn = "UpdatedNormalColumn", NullableColumn = null };
            var updateSet = new UpdateSet<DataRow>().Set(dataRow, row => row.NormalColumn, row => row.NullableColumn)
                .From(
                    set => set.InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                        .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                        .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                        .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId))
                .Where(
                    set => set.AreEqual(row => row.ValueColumn, match.ValueColumn)
                        .AreEqual(row => row.NullableColumn, match.NullableColumn)
                        .AreEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                        .AreEqual(row => row.RelatedAlias.RelatedProperty, "Related")
                        .Between(baseline, boundary, row => row.FakeDataId));

            const string Expected = @"UPDATE [dbo].[FakeData]
SET
[NormalColumn] = @0,
[NullableColumn] = NULL
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

            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var target = new TransactSqlAdapter(definitionProvider);
            var actual = target.CreateUpdateStatement(updateSet);
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement related data matches expected.
        /// </summary>
        [TestMethod]
        public void CreateDeletionStatement_RelatedDataRaisedRow_MatchesExpected()
        {
            var transactionSelection = Query
                .Select<DataRow>(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.ParentFakeDataId,
                    row => row.Related.RelatedId,
                    row => row.Related.RelatedProperty,
                    row => row.RelatedAlias.RelatedId,
                    row => row.RelatedAlias.RelatedProperty,
                    row => row.OtherAlias.RelatedId,
                    row => row.OtherAlias.RelatedProperty)
                .From(
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
                        .AreEqual(row => row.RelatedAlias.RelatedProperty, "Related")
                        .Between(row => row.FakeDataId, 10, 20));

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
            var target = new TransactSqlAdapter(definitionProvider);
            var actual = target.CreateDeletionStatement(transactionSelection);
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement union related data matches expected.
        /// </summary>
        [TestMethod]
        public void CreateSelectionStatement_UnionRelatedData_MatchesExpected()
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

            var transactionSelection = Query
                .Select<DataRow>(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.Related.RelatedId,
                    row => row.RelatedAlias.RelatedProperty,
                    row => row.OtherAlias.RelatedProperty)
                .From(
                    set => set.InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                        .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                        .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                        .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId))
                .Where(
                    set => set.AreEqual(row => row.ValueColumn, match1.ValueColumn)
                        .AreEqual(row => row.NullableColumn, match1.NullableColumn)
                        .AreEqual(row => row.NullableValueColumn, match1.NullableValueColumn)
                        .AreEqual(row => row.RelatedAlias.RelatedProperty, match1.RelatedAlias.RelatedProperty)
                        .Between(baseline1, boundary1, row => row.FakeDataId))
                .Sort(
                    set => set.OrderBy(row => row.Related.RelatedProperty)
                        .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                        .OrderBy(row => row.NormalColumn))
                .Union(
                    Query.Select<DataRow>(
                            row => row.FakeDataId,
                            row => row.NormalColumn,
                            row => row.Related.RelatedId,
                            row => row.RelatedAlias.RelatedProperty,
                            row => row.OtherAlias.RelatedProperty)
                        .From(
                            set => set.InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                                .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                                .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                                .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                                .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                                .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId))
                        .Where(
                            set => set.AreEqual(row => row.ValueColumn, match2.ValueColumn)
                                .AreEqual(row => row.NullableColumn, match2.NullableColumn)
                                .AreEqual(row => row.NullableValueColumn, match2.NullableValueColumn)
                                .AreEqual(row => row.RelatedAlias.RelatedProperty, match2.RelatedAlias.RelatedProperty)
                                .Between(baseline2, boundary2, row => row.FakeDataId))
                        .Sort(
                            set => set.OrderBy(row => row.Related.RelatedProperty)
                                .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                                .OrderBy(row => row.NormalColumn))
                        .Union(
                            Query.Select<DataRow>(
                                    row => row.FakeDataId,
                                    row => row.NormalColumn,
                                    row => row.Related.RelatedId,
                                    row => row.RelatedAlias.RelatedProperty,
                                    row => row.OtherAlias.RelatedProperty)
                                .From(
                                    set => set.InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                                        .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                                        .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                                        .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                                        .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                                        .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId))
                                .Where(
                                    set => set.AreEqual(row => row.ValueColumn, match3.ValueColumn)
                                        .AreEqual(row => row.NullableColumn, match3.NullableColumn)
                                        .AreEqual(row => row.NullableValueColumn, match3.NullableValueColumn)
                                        .AreEqual(row => row.RelatedAlias.RelatedProperty, match3.RelatedAlias.RelatedProperty)
                                        .Between(baseline3, boundary3, row => row.FakeDataId))
                                .Sort(
                                    set => set.OrderBy(row => row.Related.RelatedProperty)
                                        .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                                        .OrderBy(row => row.NormalColumn))));

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
            var target = new TransactSqlAdapter(definitionProvider);
            var actual = target.CreateSelectionStatement(transactionSelection);
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement union related data matches expected.
        /// </summary>
        [TestMethod]
        public void CreateExistsStatement_UnionRelatedData_MatchesExpected()
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

            var transactionSelection = Query
                .From<DataRow>(
                    set => set.InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                        .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                        .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                        .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId))
                .Where(
                    set => set.AreEqual(row => row.ValueColumn, match1.ValueColumn)
                        .AreEqual(row => row.NullableColumn, match1.NullableColumn)
                        .AreEqual(row => row.NullableValueColumn, match1.NullableValueColumn)
                        .AreEqual(row => row.RelatedAlias.RelatedProperty, match1.RelatedAlias.RelatedProperty)
                        .Between(baseline1, boundary1, row => row.FakeDataId))
                .Sort(
                    set => set.OrderBy(row => row.Related.RelatedProperty)
                        .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                        .OrderBy(row => row.NormalColumn))
                .Union(
                    Query.From<DataRow>(
                            set => set.InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                                .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                                .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                                .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                                .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                                .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId))
                        .Where(
                            set => set.AreEqual(row => row.ValueColumn, match2.ValueColumn)
                                .AreEqual(row => row.NullableColumn, match2.NullableColumn)
                                .AreEqual(row => row.NullableValueColumn, match2.NullableValueColumn)
                                .AreEqual(row => row.RelatedAlias.RelatedProperty, match2.RelatedAlias.RelatedProperty)
                                .Between(baseline2, boundary2, row => row.FakeDataId))
                        .Sort(
                            set => set.OrderBy(row => row.Related.RelatedProperty)
                                .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                                .OrderBy(row => row.NormalColumn))
                        .Union(
                            Query.From<DataRow>(
                                    set => set.InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                                        .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                                        .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                                        .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                                        .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                                        .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId))
                                .Where(
                                    set => set.AreEqual(row => row.ValueColumn, match3.ValueColumn)
                                        .AreEqual(row => row.NullableColumn, match3.NullableColumn)
                                        .AreEqual(row => row.NullableValueColumn, match3.NullableValueColumn)
                                        .AreEqual(row => row.RelatedAlias.RelatedProperty, match3.RelatedAlias.RelatedProperty)
                                        .Between(baseline3, boundary3, row => row.FakeDataId))
                                .Sort(
                                    set => set.OrderBy(row => row.Related.RelatedProperty)
                                        .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                                        .OrderBy(row => row.NormalColumn))));

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
            var target = new TransactSqlAdapter(definitionProvider);
            var actual = target.CreateExistsStatement(transactionSelection);
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement union related data matches expected. Deletion statements ignore UNIONs.
        /// </summary>
        [TestMethod]
        public void CreateDeletionStatement_UnionRelatedData_MatchesExpected()
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

            var transactionSelection = Query
                .Select<DataRow>(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.Related.RelatedId,
                    row => row.RelatedAlias.RelatedProperty,
                    row => row.OtherAlias.RelatedProperty)
                .From(
                    set => set.InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                        .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                        .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                        .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId))
                .Where(
                    set => set.AreEqual(row => row.ValueColumn, match1.ValueColumn)
                        .AreEqual(row => row.NullableColumn, match1.NullableColumn)
                        .AreEqual(row => row.NullableValueColumn, match1.NullableValueColumn)
                        .AreEqual(row => row.RelatedAlias.RelatedProperty, match1.RelatedAlias.RelatedProperty)
                        .Between(baseline1, boundary1, row => row.FakeDataId))
                .Union(
                    Query.From<DataRow>().Where(set => set
                        .AreEqual(row => row.ValueColumn, match2.ValueColumn)
                        .AreEqual(row => row.NullableColumn, match2.NullableColumn)
                        .AreEqual(row => row.NullableValueColumn, match2.NullableValueColumn)
                        .AreEqual(row => row.RelatedAlias.RelatedProperty, match2.RelatedAlias.RelatedProperty)
                        .Between(baseline2, boundary2, row => row.FakeDataId))
                        .Union(
                            Query.From<DataRow>().Where(set => set
                                .AreEqual(row => row.ValueColumn, match3.ValueColumn)
                                .AreEqual(row => row.NullableColumn, match3.NullableColumn)
                                .AreEqual(row => row.NullableValueColumn, match3.NullableValueColumn)
                                .AreEqual(row => row.RelatedAlias.RelatedProperty, match3.RelatedAlias.RelatedProperty)
                                .Between(baseline3, boundary3, row => row.FakeDataId))));

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
            var target = new TransactSqlAdapter(definitionProvider);
            var actual = target.CreateDeletionStatement(transactionSelection);
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement union related data matches expected.
        /// </summary>
        [TestMethod]
        public void CreateSelectionStatement_UnionRelatedDataRaisedRow_MatchesExpected()
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

            var transactionSelection = Query
                .Select<DataRow>(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.RelatedAlias.RelatedId,
                    row => row.RelatedAlias.RelatedProperty,
                    row => row.OtherAlias.RelatedProperty)
                .From(
                    set => set.InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                        .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                        .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                        .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId))
                .Where(
                    set => set.AreEqual(row => row.ValueColumn, match1.ValueColumn)
                        .AreEqual(row => row.NullableColumn, match1.NullableColumn)
                        .AreEqual(row => row.NullableValueColumn, match1.NullableValueColumn)
                        .AreEqual(row => row.RelatedAlias.RelatedProperty, "Related1")
                        .Between(baseline1, boundary1, row => row.FakeDataId))
                .Sort(
                    set => set.OrderBy(row => row.Related.RelatedProperty)
                        .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                        .OrderBy(row => row.NormalColumn))
                .Union(
                    Query.Select<DataRow>(
                            row => row.FakeDataId,
                            row => row.NormalColumn,
                            row => row.RelatedAlias.RelatedId,
                            row => row.RelatedAlias.RelatedProperty,
                            row => row.OtherAlias.RelatedProperty)
                        .From(
                            set => set.InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                                .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                                .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                                .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                                .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                                .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId))
                        .Where(
                            set => set.AreEqual(row => row.ValueColumn, match2.ValueColumn)
                                .AreEqual(row => row.NullableColumn, match2.NullableColumn)
                                .AreEqual(row => row.NullableValueColumn, match2.NullableValueColumn)
                                .AreEqual(row => row.RelatedAlias.RelatedProperty, "Related2")
                                .Between(baseline2, boundary2, row => row.FakeDataId))
                        .Sort(
                            set => set.OrderBy(row => row.Related.RelatedProperty)
                                .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                                .OrderBy(row => row.NormalColumn))
                        .Union(
                            Query.Select<DataRow>(
                                    row => row.FakeDataId,
                                    row => row.NormalColumn,
                                    row => row.RelatedAlias.RelatedId,
                                    row => row.RelatedAlias.RelatedProperty,
                                    row => row.OtherAlias.RelatedProperty)
                                .From(
                                    set => set.InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                                        .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                                        .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                                        .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                                        .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                                        .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId))
                                .Where(
                                    set => set.AreEqual(row => row.ValueColumn, match3.ValueColumn)
                                        .AreEqual(row => row.NullableColumn, match3.NullableColumn)
                                        .AreEqual(row => row.NullableValueColumn, match3.NullableValueColumn)
                                        .AreEqual(row => row.RelatedAlias.RelatedProperty, "Related3")
                                        .Between(baseline3, boundary3, row => row.FakeDataId))
                                .Sort(
                                    set => set.OrderBy(row => row.Related.RelatedProperty)
                                        .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                                        .OrderBy(row => row.NormalColumn))));

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
            var target = new TransactSqlAdapter(definitionProvider);
            var actual = target.CreateSelectionStatement(transactionSelection);
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement union related data matches expected.
        /// </summary>
        [TestMethod]
        public void CreateExistsStatement_UnionRelatedDataRaisedRow_MatchesExpected()
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

            var transactionSelection = Query
                .From<DataRow>(
                    set => set.InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                        .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                        .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                        .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId))
                .Where(
                    set => set.AreEqual(row => row.ValueColumn, match1.ValueColumn)
                        .AreEqual(row => row.NullableColumn, match1.NullableColumn)
                        .AreEqual(row => row.NullableValueColumn, match1.NullableValueColumn)
                        .AreEqual(row => row.RelatedAlias.RelatedProperty, "Related1")
                        .Between(baseline1, boundary1, row => row.FakeDataId))
                .Sort(
                    set => set.OrderBy(row => row.Related.RelatedProperty)
                        .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                        .OrderBy(row => row.NormalColumn))
                .Union(
                    Query.From<DataRow>(
                            set => set.InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                                .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                                .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                                .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                                .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                                .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId))
                        .Where(
                            set => set.AreEqual(row => row.ValueColumn, match2.ValueColumn)
                                .AreEqual(row => row.NullableColumn, match2.NullableColumn)
                                .AreEqual(row => row.NullableValueColumn, match2.NullableValueColumn)
                                .AreEqual(row => row.RelatedAlias.RelatedProperty, "Related2")
                                .Between(baseline2, boundary2, row => row.FakeDataId))
                        .Sort(
                            set => set.OrderBy(row => row.Related.RelatedProperty)
                                .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                                .OrderBy(row => row.NormalColumn))
                        .Union(
                            Query.From<DataRow>(
                                    set => set.InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                                        .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                                        .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                                        .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                                        .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                                        .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId))
                                .Where(
                                    set => set.AreEqual(row => row.ValueColumn, match3.ValueColumn)
                                        .AreEqual(row => row.NullableColumn, match3.NullableColumn)
                                        .AreEqual(row => row.NullableValueColumn, match3.NullableValueColumn)
                                        .AreEqual(row => row.RelatedAlias.RelatedProperty, "Related3")
                                        .Between(baseline3, boundary3, row => row.FakeDataId))
                                .Sort(
                                    set => set.OrderBy(row => row.Related.RelatedProperty)
                                        .OrderByDescending(row => row.OtherAlias.RelatedProperty)
                                        .OrderBy(row => row.NormalColumn))));

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
            var target = new TransactSqlAdapter(definitionProvider);
            var actual = target.CreateExistsStatement(transactionSelection);
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The selection statement union related data matches expected. Deletion statements ignore linked statements.
        /// </summary>
        [TestMethod]
        public void CreateDeletionStatement_UnionRelatedDataRaisedRow_MatchesExpected()
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

            var transactionSelection = Query
                .Select<DataRow>(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.RelatedAlias.RelatedId,
                    row => row.RelatedAlias.RelatedProperty,
                    row => row.OtherAlias.RelatedProperty)
                .From(
                    set => set.InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                        .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                        .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                        .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId))
                .Where(
                    set => set.AreEqual(row => row.ValueColumn, match1.ValueColumn)
                        .AreEqual(row => row.NullableColumn, match1.NullableColumn)
                        .AreEqual(row => row.NullableValueColumn, match1.NullableValueColumn)
                        .AreEqual(row => row.RelatedAlias.RelatedProperty, "Related1")
                        .Between(baseline1, boundary1, row => row.FakeDataId))
                .Union(
                    Query.From<DataRow>(
                            set => set.InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                                .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                                .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                                .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                                .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                                .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId))
                        .Where(
                            set => set.AreEqual(row => row.ValueColumn, match2.ValueColumn)
                                .AreEqual(row => row.NullableColumn, match2.NullableColumn)
                                .AreEqual(row => row.NullableValueColumn, match2.NullableValueColumn)
                                .AreEqual(row => row.RelatedAlias.RelatedProperty, "Related2")
                                .Between(baseline2, boundary2, row => row.FakeDataId))
                        .Union(
                            Query.From<DataRow>(
                                    set => set.InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                                        .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                                        .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                                        .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                                        .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                                        .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId))
                                .Where(
                                    set => set.AreEqual(row => row.ValueColumn, match3.ValueColumn)
                                        .AreEqual(row => row.NullableColumn, match3.NullableColumn)
                                        .AreEqual(row => row.NullableValueColumn, match3.NullableValueColumn)
                                        .AreEqual(row => row.RelatedAlias.RelatedProperty, "Related3")
                                        .Between(baseline3, boundary3, row => row.FakeDataId))));

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
            var target = new TransactSqlAdapter(definitionProvider);
            var actual = target.CreateDeletionStatement(transactionSelection);
            Assert.AreEqual(Expected, actual);
        }

        #endregion
    }
}