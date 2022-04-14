// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttributeMatchSetTests.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model.Tests
{
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Core;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Testing.Entities;

    /// <summary>
    /// Tests the <see cref="AttributeMatchSet{TSource,TRelation}"/> class.
    /// </summary>
    [TestClass]
    public class AttributeMatchSetTests
    {
        /// <summary>
        /// Tests the On method.
        /// </summary>
        [TestMethod]
        public void On_MatchedAttributeSet_ExpressionPropertyNamesMatchExpected()
        {
            var target = new AttributeMatchSet<DomainAggregateRow, SubContainerRow>().On(row => row.SubContainerId, row => row.SubContainerId);
            Assert.AreEqual(nameof(DomainAggregateRow.SubContainerId), target.Matches.First().SourceExpression.GetPropertyName());
            Assert.AreEqual(nameof(SubContainerRow.SubContainerId), target.Matches.First().RelationExpression.GetPropertyName());
        }
    }
}