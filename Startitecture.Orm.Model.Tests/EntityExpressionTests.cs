// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityExpressionTests.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model.Tests
{
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Orm.Model;
    using Startitecture.Orm.Testing.Entities;

    /// <summary>
    /// Tests the <see cref="EntityExpression{T}"/> class.
    /// </summary>
    [TestClass]
    public class EntityExpressionTests
    {
        /// <summary>
        /// Tests the As method.
        /// </summary>
        [TestMethod]
        public void As_ExpressionSelectionActionWithRelation_ExpressionRelationCountMatchesExpected()
        {
            var target = new EntityExpression<DomainAggregateRow>();
            var actual = target.As(
                selection => selection.From(set => set.InnerJoin(row => row.SubContainer.SubContainerId, row => row.SubContainerId)),
                "myQuery");

            Assert.IsNotNull(actual.Expression);
            Assert.AreEqual(1, actual.Expression.Relations.Count());
        }

        /// <summary>
        /// Tests the As method.
        /// </summary>
        [TestMethod]
        public void As_WithISelection_SelectionMatchesType()
        {
            // TODO: Getting an ISelection interface from EntitySelection<T> is too annoying to be useful.
            var target = new EntityExpression<DomainAggregateRow>();
            ISelection query = new EntitySelection<DomainAggregateRow>();
            var actual = target.As(query, "myQuery");

            Assert.IsNotNull(actual.Expression);
            Assert.IsInstanceOfType(actual, typeof(EntityExpression<DomainAggregateRow>));
        }

        /// <summary>
        /// Tests the ForSelection method.
        /// </summary>
        [TestMethod]
        public void ForSelection_SelectionWithMultipleMatchAttributes_ParentExpressionRelationCountMatchesExpected()
        {
            var target = new EntityExpression<DomainAggregateRow>();
            var actual = target.ForSelection<SubContainerRow>(
                set => set.On(row => row.SubContainerId, row => row.SubContainerId).On(row => row.Description, row => row.Name));

            Assert.IsNotNull(actual.ParentExpression);
            Assert.AreEqual(2, actual.ParentExpression.Relations.Count());
        }
    }
}