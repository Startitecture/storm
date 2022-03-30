using Microsoft.VisualStudio.TestTools.UnitTesting;

using Startitecture.Orm.Model;
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntitySetTests.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
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
    /// The entity set tests.
    /// </summary>
    [TestClass]
    public class EntitySetTests
    {
        /// <summary>
        /// The skip test.
        /// </summary>
        [TestMethod]
        public void Skip_ItemSelectionPageRowOffset_MatchesExpected()
        {
            var target = new EntitySet<ComplexRaisedRow>().Seek(set => set.Skip(10));
            Assert.AreEqual(10, target.Page.RowOffset);
            Assert.AreEqual(10, target.PropertyValues.ElementAtOrDefault(0));
            Assert.AreEqual(0, target.PropertyValues.ElementAtOrDefault(1));
        }

        /// <summary>
        /// The take test.
        /// </summary>
        [TestMethod]
        public void Take_ItemSelectionPageSize_MatchesExpected()
        {
            var target = new EntitySet<ComplexRaisedRow>().Seek(set => set.Take(10));
            Assert.AreEqual(10, target.Page.Size);
            Assert.AreEqual(0, target.PropertyValues.ElementAtOrDefault(0));
            Assert.AreEqual(10, target.PropertyValues.ElementAtOrDefault(1));
        }

        /// <summary>
        /// The order by test.
        /// </summary>
        [TestMethod]
        public void OrderBy_MultipleExpressionsAdded_MatchesExpected()
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
            var selection = new EntitySet<ChildRaisedRow>();

            selection.Sort(
                set =>
                    {
                        foreach (var expression in expressions)
                        {
                            set.OrderBy(expression);
                        }
                    });

            var entityDefinition = definitionProvider.Resolve<ChildRaisedRow>();
            var expected = expressions.Select(entityDefinition.Find).ToList();

            var actual = selection.OrderByExpressions.Select(expression => expression.PropertyExpression).Select(entityDefinition.Find).ToList();
            CollectionAssert.AreEqual(expected, actual);

            Assert.IsTrue(selection.OrderByExpressions.All(expression => expression.OrderDescending == false));
        }

        /// <summary>
        /// The order by descending test.
        /// </summary>
        [TestMethod]
        public void OrderByDescending_MultipleExpressionsAdded_MatchesExpected()
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
            var selection = new EntitySet<ChildRaisedRow>();

            selection.Sort(
                set =>
                    {
                        foreach (var expression in expressions)
                        {
                            set.OrderByDescending(expression);
                        }
                    });

            var entityDefinition = definitionProvider.Resolve<ChildRaisedRow>();
            var expected = expressions.Select(entityDefinition.Find).ToList();

            var actual = selection.OrderByExpressions.Select(expression => expression.PropertyExpression).Select(entityDefinition.Find).ToList();
            CollectionAssert.AreEqual(expected, actual);

            Assert.IsTrue(selection.OrderByExpressions.All(expression => expression.OrderDescending));
        }

        /// <summary>
        /// The selection statement_ direct data_ matches expected.
        /// </summary>
        [TestMethod]
        public void PropertyValues_DirectData_MatchesExpected()
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
            var transactionSelection = new EntitySelection<DataRow>()
                .Select(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.NullableColumn,
                    row => row.NullableValueColumn,
                    row => row.ValueColumn,
                    row => row.AnotherColumn,
                    row => row.AnotherValueColumn)
                .Where(
                    set => set.Matching(match, row => row.ValueColumn, row => row.NullableColumn, row => row.NullableValueColumn)
                        .Between(baseline, boundary, row => row.FakeDataId, row => row.NormalColumn, row => row.AnotherColumn)
                        .Include(row => row.AnotherValueColumn, 5, 10, 15, 20));

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
        public void PropertyValues_DirectDataRaisedRow_MatchesExpected()
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
            var transactionSelection = Query
                .Select<DataRow>(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.NullableColumn,
                    row => row.NullableValueColumn,
                    row => row.ValueColumn,
                    row => row.AnotherColumn,
                    row => row.AnotherValueColumn)
                .Where(
                    set => set.Matching(match, row => row.ValueColumn, row => row.NullableColumn, row => row.NullableValueColumn)
                        .Between(baseline, boundary, row => row.FakeDataId, row => row.NormalColumn, row => row.AnotherColumn)
                        .Include(row => row.AnotherValueColumn, 5, 10, 15, 20));

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
        public void PropertyValues_RelatedDataRaisedRow_MatchesExpected()
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
            var transactionSelection = new EntitySelection<DataRow>().Select(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.ParentFakeDataId,
                    row => row.Related.RelatedId,
                    row => row.Related.RelatedProperty,
                    row => row.RelatedAlias.RelatedId,
                    row => row.RelatedAlias.RelatedProperty,
                    row => row.OtherAlias.RelatedId,
                    row => row.OtherAlias.RelatedProperty)
                .Where(
                    set => set.Matching(match, row => row.ValueColumn, row => row.NullableColumn, row => row.NullableValueColumn)
                        .AreEqual(row => row.RelatedAlias.RelatedProperty, "Related")
                        .Between(baseline, boundary, row => row.FakeDataId));

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
        public void PropertyValues_UnionRelatedDataRaisedRow_MatchesExpected()
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

            var transactionSelection = Query
                .Select<DataRow>(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.RelatedAlias.RelatedId,
                    row => row.RelatedAlias.RelatedProperty,
                    row => row.OtherAlias.RelatedProperty)
                .Where(
                    set => set.Matching(match1, row => row.ValueColumn, row => row.NullableColumn, row => row.NullableValueColumn)
                        .AreEqual(row => row.RelatedAlias.RelatedProperty, "Related1")
                        .Between(baseline1, boundary1, row => row.FakeDataId))
                .Union(
                    Query.From<DataRow>()
                        .Where(
                            set => set.Matching(match2, row => row.ValueColumn, row => row.NullableColumn, row => row.NullableValueColumn)
                                .AreEqual(row => row.RelatedAlias.RelatedProperty, "Related2")
                                .Between(baseline2, boundary2, row => row.FakeDataId))
                        .Union(
                            Query.From<DataRow>()
                                .Where(
                                    set => set.Matching(match3, row => row.ValueColumn, row => row.NullableColumn, row => row.NullableValueColumn)
                                        .AreEqual(row => row.RelatedAlias.RelatedProperty, "Related3")
                                        .Between(baseline3, boundary3, row => row.FakeDataId))));

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
        /// The union_ item selection with linked selections_ link type matches expected.
        /// </summary>
        [TestMethod]
        public void Union_ItemSelectionWithLinkedSelections_LinkTypeMatchesExpected()
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

            var transactionSelection = Query
                .Select<DataRow>(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.RelatedAlias.RelatedId,
                    row => row.RelatedAlias.RelatedProperty,
                    row => row.OtherAlias.RelatedProperty)
                .Where(
                    set => set.Matching(match1, row => row.ValueColumn, row => row.NullableColumn, row => row.NullableValueColumn)
                        .AreEqual(row => row.RelatedAlias.RelatedProperty, "Related1")
                        .Between(baseline1, boundary1, row => row.FakeDataId))
                .Union(
                    Query.From<DataRow>()
                        .Where(
                            set => set.Matching(match2, row => row.ValueColumn, row => row.NullableColumn, row => row.NullableValueColumn)
                                .AreEqual(row => row.RelatedAlias.RelatedProperty, "Related2")
                                .Between(baseline2, boundary2, row => row.FakeDataId))
                        .Union(
                            Query.From<DataRow>()
                                .Where(
                                    set => set.Matching(match3, row => row.ValueColumn, row => row.NullableColumn, row => row.NullableValueColumn)
                                        .AreEqual(row => row.RelatedAlias.RelatedProperty, "Related3")
                                        .Between(baseline3, boundary3, row => row.FakeDataId))));

            Assert.AreEqual(SelectionLinkType.Union, transactionSelection.LinkedSelection.LinkType);
            Assert.AreEqual(SelectionLinkType.Union, transactionSelection.LinkedSelection.Selection.LinkedSelection.LinkType);
        }

        /// <summary>
        /// The intersect_ item selection with linked selections_ link type matches expected.
        /// </summary>
        [TestMethod]
        public void Intersect_ItemSelectionWithLinkedSelections_LinkTypeMatchesExpected()
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

            var transactionSelection = Query
                .Select<DataRow>(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.RelatedAlias.RelatedId,
                    row => row.RelatedAlias.RelatedProperty,
                    row => row.OtherAlias.RelatedProperty)
                .Where(
                    set => set.Matching(match1, row => row.ValueColumn, row => row.NullableColumn, row => row.NullableValueColumn)
                        .AreEqual(row => row.RelatedAlias.RelatedProperty, "Related1")
                        .Between(baseline1, boundary1, row => row.FakeDataId))
                .Intersect(
                    Query.From<DataRow>()
                        .Where(
                            set => set.Matching(match2, row => row.ValueColumn, row => row.NullableColumn, row => row.NullableValueColumn)
                                .AreEqual(row => row.RelatedAlias.RelatedProperty, "Related2")
                                .Between(baseline2, boundary2, row => row.FakeDataId))
                        .Intersect(
                            Query.From<DataRow>()
                                .Where(
                                    set => set.Matching(match3, row => row.ValueColumn, row => row.NullableColumn, row => row.NullableValueColumn)
                                        .AreEqual(row => row.RelatedAlias.RelatedProperty, "Related3")
                                        .Between(baseline3, boundary3, row => row.FakeDataId))));

            Assert.AreEqual(SelectionLinkType.Intersection, transactionSelection.LinkedSelection.LinkType);
            Assert.AreEqual(SelectionLinkType.Intersection, transactionSelection.LinkedSelection.Selection.LinkedSelection.LinkType);
        }

        /// <summary>
        /// The except_ item selection with linked selections_ link type matches expected.
        /// </summary>
        [TestMethod]
        public void Except_ItemSelectionWithLinkedSelections_LinkTypeMatchesExpected()
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

            var transactionSelection = Query
                .Select<DataRow>(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.RelatedAlias.RelatedId,
                    row => row.RelatedAlias.RelatedProperty,
                    row => row.OtherAlias.RelatedProperty)
                .Where(
                    set => set.Matching(match1, row => row.ValueColumn, row => row.NullableColumn, row => row.NullableValueColumn)
                        .AreEqual(row => row.RelatedAlias.RelatedProperty, "Related1")
                        .Between(baseline1, boundary1, row => row.FakeDataId))
                .Except(
                    Query.From<DataRow>()
                        .Where(
                            set => set.Matching(match2, row => row.ValueColumn, row => row.NullableColumn, row => row.NullableValueColumn)
                                .AreEqual(row => row.RelatedAlias.RelatedProperty, "Related2")
                                .Between(baseline2, boundary2, row => row.FakeDataId))
                        .Except(
                            Query.From<DataRow>()
                                .Where(
                                    set => set.Matching(match3, row => row.ValueColumn, row => row.NullableColumn, row => row.NullableValueColumn)
                                        .AreEqual(row => row.RelatedAlias.RelatedProperty, "Related3")
                                        .Between(baseline3, boundary3, row => row.FakeDataId))));

            Assert.AreEqual(SelectionLinkType.Exception, transactionSelection.LinkedSelection.LinkType);
            Assert.AreEqual(SelectionLinkType.Exception, transactionSelection.LinkedSelection.Selection.LinkedSelection.LinkType);
        }

        /// <summary>
        /// The map to test.
        /// </summary>
        [TestMethod]
        public void MapSet_DtoItemSelectionToRowItemSelectionWithParentExpression_AllAttributesMapped()
        {
            var expected = Query
                .With<SelectionTestDto>(
                    selection => selection.Select(row => row.SelectionTestId).Where(set => set.AreEqual(dto => dto.ParentId, 3)),
                    "cte")
                .ForSelection<SelectionTestDto>(matches => matches.On(dto => dto.SelectionTestId, dto => dto.SelectionTestId))
                .Select(dto => dto.UniqueName, dto => dto.SomeDate, dto => dto.SomeDecimal, dto => dto.Parent.UniqueName)
                .From(set => set.InnerJoin(dto => dto.Parent.ParentId, dto => dto.ParentId))
                .Where(
                    set => set.AreEqual(dto => dto.Parent.UniqueName, "Test1")
                        .Between(dto => dto.SomeDate, DateTime.Today, DateTime.Today.AddDays(-1)))
                .Seek(subset => subset.Skip(10).Take(10));

            var actual = expected.MapSet<SelectionTestRow>();
            Assert.AreEqual(10, actual.Page.RowOffset);
            Assert.AreEqual(10, actual.Page.Size);
            AssertSetEquality(expected, actual);
        }

        [TestMethod()]
        public void SetDefaultRelationsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void FromTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void WhereTest()
        {
            Assert.Fail();
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
        private static void AssertSetEquality(IEntitySet expected, IEntitySet actual)
        {
            if (expected is ISelection expectedSelection && actual is ISelection actualSelection)
            {
                var expectedExpressions = expectedSelection.SelectExpressions.Select(expression => expression.AttributeExpression);
                var actualExpressions = actualSelection.SelectExpressions.Select(expression => expression.AttributeExpression);
                CollectionAssert.AreEqual(
                    expectedExpressions.Select(expression => $"{expression.GetProperty().PropertyType}.{expression.GetPropertyName()}").ToList(),
                    actualExpressions.Select(expression => $"{expression.GetProperty().PropertyType}.{expression.GetPropertyName()}").ToList());
            }
            else if (expected is ISelection)
            {
                Assert.Fail($"Expected type of {nameof(ISelection)}, got {actual.GetType()}");
            }

            Assert.AreEqual(expected.Page, actual.Page);

            if (expected.ParentExpression != null)
            {
                Assert.IsNotNull(actual.ParentExpression);
                Assert.AreEqual(expected.ParentExpression.Name, actual.ParentExpression.Name);
                CollectionAssert.AreEqual(expected.ParentExpression.Relations.ToList(), actual.ParentExpression.Relations.ToList());
                AssertSetEquality(expected.ParentExpression.Expression, actual.ParentExpression.Expression);
            }

            CollectionAssert.AreEqual(expected.Filters.ToList(), actual.Filters.ToList());
            CollectionAssert.AreEqual(expected.Relations.ToList(), actual.Relations.ToList());
            CollectionAssert.AreEqual(expected.OrderByExpressions.ToList(), actual.OrderByExpressions.ToList());
        }
    }
}