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
    }
}