// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttributeLocationTests.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model.Tests
{
    using System;
    using System.Linq.Expressions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Core;
    using Startitecture.Orm.Testing.Model;

    /// <summary>
    /// The attribute location tests.
    /// </summary>
    [TestClass]
    public class AttributeLocationTests
    {
        /// <summary>
        /// The attribute location_ construct with lambda_ matches expected.
        /// </summary>
        [TestMethod]
        public void AttributeLocation_ConstructWithLambda_MatchesExpected()
        {
            Expression<Func<Document, object>> expr = document => document.Identifier;
            var expected = new AttributeLocation(
                expr.GetProperty(),
                new EntityReference
                    {
                        EntityType = typeof(Document)
                    });
            var actual = new AttributeLocation(expr);
            Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
        }

        /// <summary>
        /// The attribute location_ construct with lambda_ matches expected.
        /// </summary>
        [TestMethod]
        public void AttributeLocation_ConstructWithNestedLambda_MatchesExpected()
        {
            Expression<Func<DocumentVersion, object>> expr = document => document.Document.Identifier;
            var expected = new AttributeLocation(
                expr.GetProperty(),
                new EntityReference
                    {
                        EntityType = typeof(Document),
                        EntityAlias = "Document"
                    });
            var actual = new AttributeLocation(expr);
            Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
        }
    }
}