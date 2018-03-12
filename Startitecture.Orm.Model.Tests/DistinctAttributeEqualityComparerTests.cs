// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DistinctAttributeEqualityComparerTests.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model.Tests
{
    using System;
    using System.Linq.Expressions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Core;
    using Startitecture.Orm.Schema;
    using Startitecture.Orm.Testing.Model;

    /// <summary>
    /// The distinct attribute equality comparer tests.
    /// </summary>
    [TestClass]
    public class DistinctAttributeEqualityComparerTests
    {
        /// <summary>
        /// The equals test.
        /// </summary>
        [TestMethod]
        public void Equals_SameAttributeDifferentLocation_ReturnsTrue()
        {
            Expression<Func<InstanceSection, object>> expr1 = row => row.Instance.TemplateVersion.Revision;

            // A duplicate of our previous expression.
            Expression<Func<InstanceSection, object>> expr2 = row => row.TemplateSection.TemplateVersion.Revision;

            var entityDefinition = Singleton<PetaPocoDefinitionProvider>.Instance.Resolve<InstanceSection>();
            var firstAttribute = entityDefinition.Find(expr1);
            var secondAttribute = entityDefinition.Find(expr2);

            var target = new DistinctAttributeEqualityComparer();
            var actual = target.Equals(firstAttribute, secondAttribute);
            Assert.IsTrue(actual);
        }

        /// <summary>
        /// The equals test.
        /// </summary>
        [TestMethod]
        public void Equals_SameAttributeDifferentLocationDifferentAlias_ReturnsFalse()
        {
            Expression<Func<InstanceSection, object>> expr1 = row => row.Instance.TemplateVersion.Revision;

            // A duplicate of our previous expression.
            Expression<Func<InstanceSection, object>> expr2 = row => row.TemplateSection.TemplateVersion.Revision;

            var entityDefinition = Singleton<PetaPocoDefinitionProvider>.Instance.Resolve<InstanceSection>();
            var firstAttribute = entityDefinition.Find(expr1);
            var compareAttribute = entityDefinition.Find(expr2);
            var secondAttribute = new EntityAttributeDefinition(
                                      compareAttribute.EntityNode.List,
                                      compareAttribute.PropertyInfo,
                                      compareAttribute.PhysicalName,
                                      compareAttribute.AttributeTypes,
                                      "SomeOtherAlias");

            var target = new DistinctAttributeEqualityComparer();
            var actual = target.Equals(firstAttribute, secondAttribute);
            Assert.IsFalse(actual);
        }

        /// <summary>
        /// The equals test.
        /// </summary>
        [TestMethod]
        public void Equals_SameAttributeDifferentAlias_ReturnsFalse()
        {
            Expression<Func<InstanceSection, object>> expr1 = row => row.InstanceExtension.Enabled;

            var entityDefinition = Singleton<PetaPocoDefinitionProvider>.Instance.Resolve<InstanceSection>();
            var firstAttribute = entityDefinition.Find(expr1);
            var secondAttribute = new EntityAttributeDefinition(
                                      firstAttribute.EntityNode.List,
                                      firstAttribute.PropertyInfo,
                                      firstAttribute.PhysicalName,
                                      firstAttribute.AttributeTypes,
                                      "SomeOtherAlias");

            var target = new DistinctAttributeEqualityComparer();
            var actual = target.Equals(firstAttribute, secondAttribute);
            Assert.IsFalse(actual);
        }
    }
}