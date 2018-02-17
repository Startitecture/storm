// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataItemDefinitionTests.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SAF.Testing.Common;

    using Startitecture.Orm.Model;

    /// <summary>
    /// The data item definition tests.
    /// </summary>
    [TestClass]
    public class DataItemDefinitionTests
    {
        /// <summary>
        /// The find test.
        /// </summary>
        [TestMethod]
        public void Find_DataItemDefinitionForExistingProperty_MatchesExpected()
        {
            var target = new DataItemDefinition<FakeRaisedDataRow>();
            var actual = target.Find("NormalColumn");
            Assert.AreNotEqual(EntityAttributeDefinition.Empty, actual);
            Assert.AreEqual("NormalColumn", actual.PropertyName);
        }

        /// <summary>
        /// The find test.
        /// </summary>
        [TestMethod]
        public void Find_DataItemDefinitionForDirectPropertyWithMatchingRelatedProperty_MatchesExpected()
        {
            var target = new DataItemDefinition<FakeRaisedDataRow>();
            var actual = target.Find("FakeDataId");
            Assert.AreNotEqual(EntityAttributeDefinition.Empty, actual);
            Assert.AreEqual("FakeDataId", actual.PropertyName);
            Assert.AreEqual(target.EntityName, actual.Entity.Name);
            Assert.AreEqual(target.EntityContainer, actual.Entity.Container);
            Assert.IsNull(actual.Entity.Alias);
        }

        /////// <summary>
        /////// The find test.
        /////// </summary>
        ////[TestMethod]
        ////public void Find_DataItemDefinitionForExistingPropertyWithType_MatchesExpected()
        ////{
        ////    var target = new DataItemDefinition<FakeRaisedDataRow>();
        ////    var actual = target.Find(
        ////        "FakeDataId",
        ////        EntityAttributeTypes.DirectAttribute,
        ////        EntityAttributeTypes.IdentityColumn,
        ////        EntityAttributeTypes.PrimaryKey);

        ////    Assert.AreNotEqual(EntityAttributeDefinition.Empty, actual);
        ////    Assert.AreEqual("FakeDataId", actual.PropertyName);
        ////    Assert.AreEqual(target.EntityName, actual.Entity.Name);
        ////    Assert.AreEqual(target.EntityContainer, actual.Entity.Container);
        ////    Assert.IsNull(actual.Entity.Alias);
        ////}
    }
}