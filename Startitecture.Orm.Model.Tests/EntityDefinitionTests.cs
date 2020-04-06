// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityDefinitionTests.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Core;
    using Startitecture.Orm.Schema;
    using Startitecture.Orm.Testing.Entities;

    /// <summary>
    /// The entity definition tests.
    /// </summary>
    [TestClass]
    public class EntityDefinitionTests
    {
        /// <summary>
        /// The find test.
        /// </summary>
        [TestMethod]
        public void Find_DirectAttribute_MatchesExpected()
        {
            var entityReference = new EntityReference { EntityType = typeof(ChildRaisedRow) };
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var target = new EntityDefinition(definitionProvider, entityReference);

            var attributeLocation = definitionProvider.GetEntityLocation(entityReference);
            var attributePath = new LinkedList<EntityLocation>();
            attributePath.AddLast(attributeLocation);
            var expected = new EntityAttributeDefinition(
                attributePath,
                entityReference.EntityType.GetProperty(nameof(ChildRaisedRow.ComplexEntityId), BindingFlags.Public | BindingFlags.Instance),
                nameof(ChildRaisedRow.ComplexEntityId),
                EntityAttributeTypes.DirectAttribute,
                2);

            var actual = target.Find(CreateExpression<ChildRaisedRow, int>(row => row.ComplexEntityId));
            Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
        }

        /// <summary>
        /// The find test.
        /// </summary>
        [TestMethod]
        public void Find_RelatedAttribute_MatchesExpected()
        {
            var entityReference = new EntityReference { EntityType = typeof(ChildRaisedRow) };
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var target = new EntityDefinition(definitionProvider, entityReference);

            var relationReference = new EntityReference { EntityType = typeof(ComplexRaisedRow) };
            var attributeLocation = definitionProvider.GetEntityLocation(relationReference);
            var attributePath = new LinkedList<EntityLocation>();
            attributePath.AddLast(attributeLocation);
            var expected = new EntityAttributeDefinition(
                attributePath,
                relationReference.EntityType.GetProperty(nameof(ComplexRaisedRow.ComplexEntityId), BindingFlags.Public | BindingFlags.Instance),
                nameof(ComplexRaisedRow.ComplexEntityId),
                EntityAttributeTypes.RelatedAutoNumberKey,
                1,
                $"ComplexEntity.{nameof(ComplexRaisedRow.ComplexEntityId)}");

            var actual = target.Find(CreateExpression<ChildRaisedRow, int>(row => row.ComplexEntity.ComplexEntityId));
            Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
        }

        /// <summary>
        /// The find test.
        /// </summary>
        [TestMethod]
        public void Find_RelatedAttributeAliasedEntity_MatchesExpected()
        {
            var entityReference = new EntityReference { EntityType = typeof(ChildRaisedRow) };
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var target = new EntityDefinition(definitionProvider, entityReference);

            var relationReference = new EntityReference { EntityType = typeof(SubRow), EntityAlias = "SubEntity" };
            var attributeLocation = definitionProvider.GetEntityLocation(relationReference);
            var attributePath = new LinkedList<EntityLocation>();
            attributePath.AddLast(attributeLocation);
            var expected = new EntityAttributeDefinition(
                               attributePath,
                               relationReference.EntityType.GetProperty(nameof(SubRow.FakeSubEntityId), BindingFlags.Public | BindingFlags.Instance),
                               nameof(SubRow.FakeSubEntityId),
                               EntityAttributeTypes.RelatedAutoNumberKey,
                               1,
                               $"SubEntity.{nameof(SubRow.FakeSubEntityId)}");

            var actual = target.Find(CreateExpression<ChildRaisedRow, int>(row => row.ComplexEntity.SubEntity.FakeSubEntityId));
            Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
        }

        /// <summary>
        /// Creates and returns a lambda expression.
        /// </summary>
        /// <param name="propertyExpression">
        /// The property expression.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item containing the property.
        /// </typeparam>
        /// <typeparam name="TProperty">
        /// The type of the property.
        /// </typeparam>
        /// <returns>
        /// The <see cref="LambdaExpression"/> created by the expression.
        /// </returns>
        private static LambdaExpression CreateExpression<TItem, TProperty>(Expression<Func<TItem, TProperty>> propertyExpression)
        {
            return propertyExpression;
        }
    }
}