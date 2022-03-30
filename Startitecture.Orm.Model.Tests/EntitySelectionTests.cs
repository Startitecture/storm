// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntitySelectionTests.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The entity selection tests.
// </summary>
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
    /// The entity selection tests.
    /// </summary>
    [TestClass]
    public class EntitySelectionTests
    {
        /////// <summary>
        /////// The with as test.
        /////// </summary>
        ////[TestMethod]
        ////public void WithAs_TableExpressionForDifferentTable_ParentExpressionIsSet()
        ////{
        ////    var target = new EntitySelection<ComplexRaisedRow>().Select(row => row.ComplexEntityId, row => row.UniqueName)
        ////        .WithAs<ChildRaisedRow>(expression =>
        ////            expression.As(selection => selection.Select(row => row.ComplexEntityId), "childCte")
        ////                .For<ComplexRaisedRow>(matches => matches.On(row => row.ComplexEntityId, row => row.ComplexEntityId)));

        ////    Assert.IsNotNull(target.ParentExpression);
        ////    Assert.AreEqual("childCte", target.ParentExpression.Name);
        ////    Assert.IsTrue(target.ParentExpression.Relations.Any());
        ////}

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
            var entityDefinition = definitionProvider.Resolve<ChildRaisedRow>();

            var selection = new EntitySelection<ChildRaisedRow>().Select(expressions.ToArray());
            var expected = expressions.Select(entityDefinition.Find).ToList();

            var actual = selection.SelectExpressions.Select(expression => expression.AttributeExpression).Select(entityDefinition.Find).ToList();
            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// The count test.
        /// </summary>
        [TestMethod]
        public void CountTest_ItemSelectionWithCount_MatchesExpected()
        {
            var definitionProvider = Singleton<DataAnnotationsDefinitionProvider>.Instance;
            var entityDefinition = definitionProvider.Resolve<ComplexRaisedRow>();

            Expression<Func<ComplexRaisedRow, object>> expr1 = row => row.ComplexEntityId;
            var target = new EntitySelection<ComplexRaisedRow>().Count(row => row.ComplexEntityId);
            var expected = entityDefinition.Find(expr1);
            var attributeExpression = target.SelectExpressions.FirstOrDefault()?.AttributeExpression;
            Assert.IsNotNull(attributeExpression);
            var actual = entityDefinition.Find(attributeExpression);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(AggregateFunction.Count, target.SelectExpressions.FirstOrDefault()?.AggregateFunction);
        }

        /// <summary>
        /// The map to test.
        /// </summary>
        [TestMethod]
        public void MapSet_DtoItemSelectionToRowItemSelection_AllAttributesMapped()
        {
            var expected = new EntitySelection<SelectionTestDto>();
            var selection2 = new EntitySet<SelectionTestDto>();
            var selection3 = new EntitySet<SelectionTestDto>();
            expected.Select(dto => dto.UniqueName, dto => dto.SomeDate, dto => dto.SomeDecimal, dto => dto.Parent.UniqueName)
                .From(set => set.InnerJoin(dto => dto.Parent.ParentId, dto => dto.ParentId))
                .Where(
                    set => set.AreEqual(dto => dto.Parent.UniqueName, "Test1")
                        .Between(dto => dto.SomeDate, DateTime.Today, DateTime.Today.AddDays(-1)))
                .Seek(subset => subset.Skip(10).Take(10))
                .Union(
                    selection2.From(set => set.InnerJoin(dto => dto.Parent.ParentId, dto => dto.ParentId))
                        .Where(set => set.AreEqual(dto => dto.Parent.UniqueName, "Test2").Between(dto => dto.SomeDecimal, 10.2m, 11.4m))
                        .Seek(subset => subset.Skip(20).Take(10))
                        .Union(
                            selection3.From(set => set.InnerJoin(dto => dto.Parent.ParentId, dto => dto.ParentId))
                                .Where(set => set.AreEqual(dto => dto.Parent.UniqueName, "Test3").Between(dto => dto.StringValue, "AAA", "BBB"))
                                .Seek(subset => subset.Skip(20).Take(20))));

            var actual = expected.MapSelection<SelectionTestRow>();

            AssertSetEquality(expected, actual);
            AssertSetEquality(expected.LinkedSelection.Selection, actual.LinkedSelection.Selection);
            AssertSetEquality(
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