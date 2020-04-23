// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemSelectionTests.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Core;
    using Startitecture.Orm.Schema;
    using Startitecture.Orm.Testing.Entities;

    /// <summary>
    /// The item selection tests.
    /// </summary>
    [TestClass]
    public class ItemSelectionTests
    {
        /// <summary>
        /// The between test.
        /// </summary>
        [TestMethod]
        public void Between_ItemSelectionWithOrderedValues_ValuesMatchExpected()
        {
            var target = new ItemSelection<SelectionTestRow>().Between(row => row.SomeDate, DateTime.Today, DateTime.Today.AddDays(1));
            Assert.AreEqual(DateTime.Today, target.PropertyValues.First());
        }

        /// <summary>
        /// The between test.
        /// </summary>
        [TestMethod]
        public void Between_ItemSelectionWithUnorderedValues_ValuesMatchExpected()
        {
            var target = new ItemSelection<SelectionTestRow>().Between(row => row.SomeDate, DateTime.Today.AddDays(1), DateTime.Today);
            Assert.AreEqual(DateTime.Today, target.PropertyValues.First());
        }

        /// <summary>
        /// The between test.
        /// </summary>
        [TestMethod]
        public void Between_ItemSelectionWithUnorderedValues_FilterMatchesExpected()
        {
            Expression<Func<SelectionTestRow, DateTime>> selector = row => row.SomeDate;
            var maxValue = DateTime.Today;  
            var minValue = maxValue.AddDays(-1);
            var expected = new ValueFilter(new AttributeLocation(selector), FilterType.Between, minValue, maxValue);
            var target = new ItemSelection<SelectionTestRow>().Between(selector, minValue, maxValue);
            var actual = target.Filters.First();

            Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
        }

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void InnerJoin_WithoutRelationAlias_MatchesExpected()
        {
            var relations = new ItemSelection<DataRow>().InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId).Relations;
            var expected = new EntityRelation(EntityRelationType.InnerJoin);
            expected.Join<DataRow, FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId);
            Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        }

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void InnerJoin_ExtendedRelationWithoutRelationAlias_MatchesExpected()
        {
            var relations = new ItemSelection<DataRow>().InnerJoin<FakeRelatedRow, DependencyRow>(row => row.RelatedId, row => row.ComplexEntityId)
                .Relations;

            var expected = new EntityRelation(EntityRelationType.InnerJoin);
            expected.Join<FakeRelatedRow, DependencyRow>(row => row.RelatedId, row => row.ComplexEntityId);

            Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        }

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void InnerJoin_WithRelationAlias_MatchesExpected()
        {
            var relations = new ItemSelection<DataRow>().InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "OtherAlias")
                .Relations;

            var expected = new EntityRelation(EntityRelationType.InnerJoin);
            expected.Join<DataRow, FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, null, "OtherAlias");

            Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        }

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void InnerJoin_WithSourceAndRelationAlias_MatchesExpected()
        {
            var relations = new ItemSelection<DataRow>().InnerJoin<FakeRelatedRow, DependencyRow>(
                    row => row.RelatedId,
                    "OtherAlias",
                    row => row.ComplexEntityId,
                    "RelatedDependency")
                .Relations;

            var expected = new EntityRelation(EntityRelationType.InnerJoin);
            expected.Join<FakeRelatedRow, DependencyRow>(row => row.RelatedId, row => row.ComplexEntityId, "OtherAlias", "RelatedDependency");

            Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        }

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void InnerJoin_InferredWithMatchingRelationProperty_MatchesExpected()
        {
            var relations = new ItemSelection<DataRow>().InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId).Relations;

            var expected = new EntityRelation(EntityRelationType.InnerJoin);
            expected.Join<DataRow, FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId);

            Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        }

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void InnerJoin_InferredWithMatchingSourceAndRelationProperties_MatchesExpected()
        {
            var relations = new ItemSelection<DataRow>().InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                .Relations;

            var expected = new EntityRelation(EntityRelationType.InnerJoin);
            expected.Join<FakeRelatedRow, DependencyRow>(row => row.RelatedId, row => row.ComplexEntityId);

            Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        }

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void LeftJoin_WithoutRelationAlias_MatchesExpected()
        {
            var relations = new ItemSelection<DataRow>().LeftJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId).Relations;

            var expected = new EntityRelation(EntityRelationType.LeftJoin);
            expected.Join<DataRow, FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId);

            Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        }

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void LeftJoin_ExtendedRelationWithoutRelationAlias_MatchesExpected()
        {
            var relations = new ItemSelection<DataRow>().LeftJoin<FakeRelatedRow, DependencyRow>(row => row.RelatedId, row => row.ComplexEntityId)
                .Relations;

            var expected = new EntityRelation(EntityRelationType.LeftJoin);
            expected.Join<FakeRelatedRow, DependencyRow>(row => row.RelatedId, row => row.ComplexEntityId);

            Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        }

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void LeftJoin_WithRelationAlias_MatchesExpected()
        {
            var relations = new ItemSelection<DataRow>().LeftJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "OtherAlias")
                .Relations;

            var expected = new EntityRelation(EntityRelationType.LeftJoin);
            expected.Join<DataRow, FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, null, "OtherAlias");

            Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        }

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void LeftJoin_WithSourceAndRelationAlias_MatchesExpected()
        {
            var relations = new ItemSelection<DataRow>().LeftJoin<FakeRelatedRow, DependencyRow>(
                    row => row.RelatedId,
                    "OtherAlias",
                    row => row.ComplexEntityId,
                    "RelatedDependency")
                .Relations;

            var expected = new EntityRelation(EntityRelationType.LeftJoin);
            expected.Join<FakeRelatedRow, DependencyRow>(row => row.RelatedId, row => row.ComplexEntityId, "OtherAlias", "RelatedDependency");

            Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        }

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void LeftJoin_InferredWithMatchingRelationProperty_MatchesExpected()
        {
            var relations = new ItemSelection<DataRow>().LeftJoin(row => row.FakeDataId, row => row.Related.FakeDataId).Relations;

            var expected = new EntityRelation(EntityRelationType.LeftJoin);
            expected.Join<DataRow, FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId);

            Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        }

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void LeftJoin_InferredWithMatchingSourceAndRelationProperties_MatchesExpected()
        {
            var relations = new ItemSelection<DataRow>().LeftJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                .Relations;

            var expected = new EntityRelation(EntityRelationType.LeftJoin);
            expected.Join<FakeRelatedRow, DependencyRow>(row => row.RelatedId, row => row.ComplexEntityId);

            Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        }

        /// <summary>
        /// The clear relations test.
        /// </summary>
        [TestMethod]
        public void ClearRelations_ItemSelectionWithRelations_RelationsAreCleared()
        {
            var selection = Select.From<DataRow>()
                .InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId);

            Assert.IsNotNull(selection.Relations.FirstOrDefault());
            selection.ClearRelations();
            Assert.IsNull(selection.Relations.FirstOrDefault());
        }

        /// <summary>
        /// The select test.
        /// </summary>
        [TestMethod]
        public void Select_DataItemExplicitSelectionsWithDistinctAttributeReferences_MatchesExpected()
        {
            Expression<Func<ChildRaisedRow, object>> expr1 = row => row.FakeChildEntityId;
            Expression<Func<ChildRaisedRow, object>> expr2 = row => row.Name;
            Expression<Func<ChildRaisedRow, object>> expr3 = row => row.SomeValue;
            Expression<Func<ChildRaisedRow, object>> expr4 = row => row.ComplexEntity.Description;
            Expression<Func<ChildRaisedRow, object>> expr5 = row => row.ComplexEntity.SubEntity.Description;
            Expression<Func<ChildRaisedRow, object>> expr6 = row => row.ComplexEntity.SubEntity.SubSubEntity.UniqueName;

            var expressions = new List<Expression<Func<ChildRaisedRow, object>>>
                                  {
                                      expr1,
                                      expr2,
                                      expr3,
                                      expr4,
                                      expr5,
                                      expr6
                                  };

            var definitionProvider = Singleton<DataAnnotationsDefinitionProvider>.Instance;
            var selection = new ItemSelection<ChildRaisedRow>().Select(expressions.ToArray());

            var entityDefinition = definitionProvider.Resolve<ChildRaisedRow>();
            var expected = expressions.Select(entityDefinition.Find).ToList();

            var actual = selection.SelectExpressions.Select(entityDefinition.Find).ToList();
            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// The selection statement_ direct data_ matches expected.
        /// </summary>
        [TestMethod]
        public void SelectionStatement_DirectData_MatchesExpected()
        {
            var match = new DataRow
                            {
                                ValueColumn = 2,
                                NullableColumn = "CouldHaveBeenNull",
                                NullableValueColumn = null
                            };
            var baseline = new DataRow
                               {
                                   FakeDataId = 10,
                                   NormalColumn = "Greater"
                               };
            var boundary = new DataRow
                               {
                                   FakeDataId = 20,
                                   AnotherColumn = "Less"
                               };
            var transactionSelection = new ItemSelection<DataRow>()
                .Matching(match, row => row.ValueColumn, row => row.NullableColumn, row => row.NullableValueColumn)
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
            var actual = transactionSelection.PropertyValues.ToArray();
            CollectionAssert.AreEqual(
                expected,
                actual,
                "Expected: {0}{1}Actual: {2}",
                string.Join(",", expected),
                Environment.NewLine,
                string.Join(",", actual));
        }

        /// <summary>
        /// The selection statement_ direct data_ matches expected.
        /// </summary>
        [TestMethod]
        public void SelectionStatement_DirectDataRaisedRow_MatchesExpected()
        {
            var match = new DataRow
                            {
                                ValueColumn = 2,
                                NullableColumn = "CouldHaveBeenNull",
                                NullableValueColumn = null
                            };
            var baseline = new DataRow
                               {
                                   FakeDataId = 10,
                                   NormalColumn = "Greater"
                               };
            var boundary = new DataRow
                               {
                                   FakeDataId = 20,
                                   AnotherColumn = "Less"
                               };
            var transactionSelection = Select
                .From<DataRow>(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.NullableColumn,
                    row => row.NullableValueColumn,
                    row => row.ValueColumn,
                    row => row.AnotherColumn,
                    row => row.AnotherValueColumn)
                .Matching(match, row => row.ValueColumn, row => row.NullableColumn, row => row.NullableValueColumn)
                .Between(baseline, boundary, row => row.FakeDataId, row => row.NormalColumn, row => row.AnotherColumn)
                .Include(row => row.AnotherValueColumn, 5, 10, 15, 20);

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
            var actual = transactionSelection.PropertyValues.ToArray();
            CollectionAssert.AreEqual(
                expected,
                actual,
                "Expected: {0}{1}Actual: {2}",
                string.Join(",", expected),
                Environment.NewLine,
                string.Join(",", actual));
        }

        /// <summary>
        /// The selection statement_ related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void SelectionStatement_RelatedDataRaisedRow_MatchesExpected()
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

            var baseline = new DataRow
                               {
                                   FakeDataId = 10
                               };
            var boundary = new DataRow
                               {
                                   FakeDataId = 20
                               };
            var transactionSelection = new ItemSelection<DataRow>()
                .Matching(match, row => row.ValueColumn, row => row.NullableColumn, row => row.NullableValueColumn)
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

            CollectionAssert.AreEqual(
                new object[]
                    {
                        2,
                        "CouldHaveBeenNull",
                        "Related",
                        10,
                        20
                    },
                transactionSelection.PropertyValues.ToArray());
        }

        /// <summary>
        /// The selection statement_ union related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void SelectionStatement_UnionRelatedDataRaisedRow_MatchesExpected()
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

            var baseline1 = new DataRow
                                {
                                    FakeDataId = 10
                                };
            var boundary1 = new DataRow
                                {
                                    FakeDataId = 20
                                };

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

            var baseline2 = new DataRow
                                {
                                    FakeDataId = 50
                                };
            var boundary2 = new DataRow
                                {
                                    FakeDataId = 40
                                };

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

            var baseline3 = new DataRow
                                {
                                    FakeDataId = 60
                                };
            var boundary3 = new DataRow
                                {
                                    FakeDataId = 70
                                };

            var transactionSelection = Select
                .From<DataRow>(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.RelatedAlias.RelatedId,
                    row => row.RelatedAlias.RelatedProperty,
                    row => row.OtherAlias.RelatedProperty)
                .Matching(match1, row => row.ValueColumn, row => row.NullableColumn, row => row.NullableValueColumn)
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
                        .Matching(match2, row => row.ValueColumn, row => row.NullableColumn, row => row.NullableValueColumn)
                        .WhereEqual(row => row.RelatedAlias.RelatedProperty, "Related2")
                        .Between(baseline2, boundary2, row => row.FakeDataId)
                        .Union(
                            Select.From<DataRow>(
                                    row => row.FakeDataId,
                                    row => row.NormalColumn,
                                    row => row.RelatedAlias.RelatedId,
                                    row => row.RelatedAlias.RelatedProperty,
                                    row => row.OtherAlias.RelatedProperty)
                                .Matching(match3, row => row.ValueColumn, row => row.NullableColumn, row => row.NullableValueColumn)
                                .WhereEqual(row => row.RelatedAlias.RelatedProperty, "Related3")
                                .Between(baseline3, boundary3, row => row.FakeDataId)));

            var expected = new object[]
                               {
                                   2,
                                   "CouldHaveBeenNull1",
                                   "Related1",
                                   10,
                                   20,
                                   3,
                                   "CouldHaveBeenNull2",
                                   "Related2",
                                   40,
                                   50,
                                   4,
                                   "CouldHaveBeenNull3",
                                   "Related3",
                                   60,
                                   70
                               };

            var actual = transactionSelection.PropertyValues.ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// The map to test.
        /// </summary>
        [TestMethod]
        public void MapTo_DtoItemSelectionToRowItemSelection_AllAttributesMapped()
        {
            var expected = new ItemSelection<SelectionTestDto>();
            var selection2 = new ItemSelection<SelectionTestDto>();
            var selection3 = new ItemSelection<SelectionTestDto>();
            expected.Select(dto => dto.UniqueName, dto => dto.SomeDate, dto => dto.SomeDecimal, dto => dto.Parent.UniqueName)
                .WhereEqual(dto => dto.Parent.UniqueName, "Test1")
                .Between(dto => dto.SomeDate, DateTime.Today, DateTime.Today.AddDays(-1))
                .InnerJoin(dto => dto.Parent.ParentId, dto => dto.ParentId)
                .Union(
                    selection2.Select(dto => dto.UniqueName, dto => dto.SomeDate, dto => dto.SomeDecimal, dto => dto.Parent.UniqueName)
                        .WhereEqual(dto => dto.Parent.UniqueName, "Test2")
                        .Between(dto => dto.SomeDecimal, 10.2m, 11.4m)
                        .InnerJoin(dto => dto.Parent.ParentId, dto => dto.ParentId)
                        .Union(
                            selection3.Select(dto => dto.UniqueName, dto => dto.SomeDate, dto => dto.SomeDecimal, dto => dto.Parent.UniqueName)
                                .WhereEqual(dto => dto.Parent.UniqueName, "Test3")
                                .Between(dto => dto.StringValue, "AAA", "BBB")
                                .InnerJoin(dto => dto.Parent.ParentId, dto => dto.ParentId)));

            expected.Limit = 10;
            selection2.Limit = 10;
            selection3.Limit = 20;

            var actual = expected.MapTo<SelectionTestRow>();

            AssertSelectionEquality(expected, actual);
            AssertSelectionEquality(expected.LinkedSelection.Selection, actual.LinkedSelection.Selection);
            AssertSelectionEquality(
                expected.LinkedSelection.Selection.LinkedSelection.Selection,
                actual.LinkedSelection.Selection.LinkedSelection.Selection);
        }

        /// <summary>
        /// The assert selection equality.
        /// </summary>
        /// <param name="expected">
        /// The expected.
        /// </param>
        /// <param name="actual">
        /// The actual.
        /// </param>
        private static void AssertSelectionEquality(ItemSelection<SelectionTestDto> expected, ItemSelection<SelectionTestRow> actual)
        {
            Assert.AreEqual(expected.Limit, actual.Limit);
            CollectionAssert.AreEqual(expected.Filters.ToList(), actual.Filters.ToList());
            CollectionAssert.AreEqual(expected.Relations.ToList(), actual.Relations.ToList());
            CollectionAssert.AreEqual(
                expected.SelectExpressions.Select(expression => $"{expression.GetProperty().PropertyType}.{expression.GetPropertyName()}").ToList(),
                actual.SelectExpressions.Select(expression => $"{expression.GetProperty().PropertyType}.{expression.GetPropertyName()}").ToList());
        }

        /// <summary>
        /// The parent DTO.
        /// </summary>
        private class ParentDto
        {
            /// <summary>
            /// Gets or sets the parent id.
            /// </summary>
            public int ParentId { get; set; }

            /// <summary>
            /// Gets or sets the unique name.
            /// </summary>
            public string UniqueName { get; set; }
        }

        /// <summary>
        /// The selection test DTO.
        /// </summary>
        private class SelectionTestDto
        {
            /// <summary>
            /// Gets or sets the selection test id.
            /// </summary>
            public int SelectionTestId { get; set; }

            /// <summary>
            /// Gets or sets the unique name.
            /// </summary>
            public string UniqueName { get; set; }

            /// <summary>
            /// Gets or sets the string value.
            /// </summary>
            public string StringValue { get; set; }

            /// <summary>
            /// Gets or sets the some decimal.
            /// </summary>
            public decimal SomeDecimal { get; set; }

            /// <summary>
            /// Gets or sets the some date.
            /// </summary>
            public DateTime SomeDate { get; set; }

            /// <summary>
            /// Gets or sets the parent.
            /// </summary>
            public ParentDto Parent { get; set; }

            /// <summary>
            /// The parent id.
            /// </summary>
            public int? ParentId => this.Parent?.ParentId;
        }

        /// <summary>
        /// The parent row.
        /// </summary>
        private class ParentRow
        {
            /// <summary>
            /// Gets or sets the parent id.
            /// </summary>
            public int ParentId { get; set; }

            /// <summary>
            /// Gets or sets the unique name.
            /// </summary>
            public string UniqueName { get; set; }
        }

        /// <summary>
        /// The selection test row.
        /// </summary>
        private class SelectionTestRow
        {
            /// <summary>
            /// Gets or sets the selection test id.
            /// </summary>
            public int SelectionTestId { get; set; }

            /// <summary>
            /// Gets or sets the unique name.
            /// </summary>
            public string UniqueName { get; set; }

            /// <summary>
            /// Gets or sets the string value.
            /// </summary>
            public string StringValue { get; set; }

            /// <summary>
            /// Gets or sets the some decimal.
            /// </summary>
            public decimal SomeDecimal { get; set; }

            /// <summary>
            /// Gets or sets the some date.
            /// </summary>
            public DateTime SomeDate { get; set; }

            /// <summary>
            /// Gets or sets the parent.
            /// </summary>
            public ParentRow Parent { get; set; }
        }
    }
}