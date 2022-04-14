// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrderExpressionSetTests.cs" company="Startitecture">
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
    using Startitecture.Orm.Model;

    /// <summary>
    /// Tests the <see cref="OrderExpressionSet{T}"/> class.
    /// </summary>
    [TestClass]
    public class OrderExpressionSetTests
    {
        /// <summary>
        /// Tests the OrderBy method.
        /// </summary>
        [TestMethod]
        public void OrderBy_MultipleExpressionsOrderedAscending_ExpressionsMatchExpected()
        {
            var target = new OrderExpressionSet<SelectionTestRow>().OrderBy(row => row.SomeDate).OrderBy(row => row.SomeDecimal);

            Expression<Func<SelectionTestRow, object>> dateExpression = row => row.SomeDate;
            Expression<Func<SelectionTestRow, object>> decimalExpression = row => row.SomeDecimal;

            var expected = new List<OrderExpression>
                           {
                               new OrderExpression(dateExpression),
                               new OrderExpression(decimalExpression)
                           };

            Assert.AreEqual(
                expected.ElementAt(0).PropertyExpression.GetPropertyName(),
                target.Expressions.ElementAt(0).PropertyExpression.GetPropertyName());

            Assert.IsFalse(target.Expressions.ElementAt(0).OrderDescending);

            Assert.AreEqual(
                expected.ElementAt(1).PropertyExpression.GetPropertyName(),
                target.Expressions.ElementAt(1).PropertyExpression.GetPropertyName());

            Assert.IsFalse(target.Expressions.ElementAt(1).OrderDescending);
        }

        /// <summary>
        /// Tests the OrderByDescending method.
        /// </summary>
        [TestMethod]
        public void OrderByDescending_MultipleExpressionsOrderedDescending_ExpressionsMatchExpected()
        {
            var target = new OrderExpressionSet<SelectionTestRow>().OrderByDescending(row => row.SomeDate).OrderByDescending(row => row.SomeDecimal);

            Expression<Func<SelectionTestRow, object>> dateExpression = row => row.SomeDate;
            Expression<Func<SelectionTestRow, object>> decimalExpression = row => row.SomeDecimal;

            var expected = new List<OrderExpression>
                           {
                               new OrderExpression(dateExpression, true),
                               new OrderExpression(decimalExpression, true)
                           };

            Assert.AreEqual(
                expected.ElementAt(0).PropertyExpression.GetPropertyName(),
                target.Expressions.ElementAt(0).PropertyExpression.GetPropertyName());

            Assert.IsTrue(target.Expressions.ElementAt(0).OrderDescending);

            Assert.AreEqual(
                expected.ElementAt(1).PropertyExpression.GetPropertyName(),
                target.Expressions.ElementAt(1).PropertyExpression.GetPropertyName());

            Assert.IsTrue(target.Expressions.ElementAt(1).OrderDescending);
        }
    }
}