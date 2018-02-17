// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityRelationTests.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SAF.Data.Providers;
    using SAF.Testing.Common;

    using Startitecture.Orm.Model;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Schema;

    /// <summary>
    /// The entity relation tests.
    /// </summary>
    [TestClass]
    public class EntityRelationTests
    {
        /// <summary>
        /// The join test.
        /// </summary>
        [TestMethod]
        public void Join_LocalAttributeToRelatedAttribute()
        {
            var definitionProvider = new DataItemDefinitionProvider();
            var actual = new EntityRelation(definitionProvider, EntityRelationType.InnerJoin);
            actual.Join<FakeRaisedChildRow>(row => row.FakeComplexEntityId, row => row.FakeComplexEntity.FakeComplexEntityId);

            var childDefinition = definitionProvider.Resolve<FakeRaisedChildRow>();
            var childReference = new EntityReference { EntityType = typeof(FakeRaisedChildRow) };
            var childLocation = definitionProvider.GetEntityLocation(childReference);
            var childComplexIdAttribute = childDefinition.Find("FakeComplexEntityId");

            Assert.AreEqual(childLocation, actual.SourceLocation);
            Assert.AreEqual(childComplexIdAttribute, actual.SourceAttribute);

            var complexDefinition = definitionProvider.Resolve<FakeRaisedComplexRow>();
            var complexReference = new EntityReference { EntityType = typeof(FakeRaisedComplexRow) };
            var complexLocation = definitionProvider.GetEntityLocation(complexReference);
            var complexIdAttribute = complexDefinition.Find("FakeComplexEntityId");

            Assert.AreEqual(complexLocation, actual.RelationLocation);
            Assert.AreEqual(complexIdAttribute, actual.RelationAttribute);
        }

        /// <summary>
        /// The join test.
        /// </summary>
        [TestMethod]
        public void Join_RelatedAttributeToTransitiveRelatedAttribute()
        {
            var definitionProvider = new DataItemDefinitionProvider();
            var actual = new EntityRelation(definitionProvider, EntityRelationType.InnerJoin);
            actual.Join<FakeRaisedDataRow>(row => row.FakeRelated.RelatedId, row => row.FakeDependencyEntity.FakeDependencyEntityId);

            var leftDefinition = definitionProvider.Resolve<FakeRelatedRow>();
            var leftReference = new EntityReference { EntityType = typeof(FakeRelatedRow) };
            var leftLocation = definitionProvider.GetEntityLocation(leftReference);
            var leftAttribute = leftDefinition.Find("RelatedId");

            Assert.AreEqual(leftLocation, actual.SourceLocation);
            Assert.AreEqual(leftAttribute, actual.SourceAttribute);

            var rightDefinition = definitionProvider.Resolve<FakeDependencyRow>();
            var rightReference = new EntityReference { EntityType = typeof(FakeDependencyRow) };
            var rightLocation = definitionProvider.GetEntityLocation(rightReference);
            var rightAttribute = rightDefinition.Find("FakeDependencyEntityId");

            Assert.AreEqual(rightLocation, actual.RelationLocation);
            Assert.AreEqual(rightAttribute, actual.RelationAttribute);
        }

        /// <summary>
        /// The join test.
        /// </summary>
        [TestMethod]
        public void Join_RelatedAttributeToTransitiveRelatedAttributeWithAlias()
        {
            var definitionProvider = new DataItemDefinitionProvider();
            var actual = new EntityRelation(definitionProvider, EntityRelationType.InnerJoin);
            actual.Join<FakeRaisedChildRow>(row => row.FakeComplexEntity.FakeSubEntityId, row => row.FakeComplexEntity.FakeSubEntity.FakeSubEntityId);

            var complexDefinition = definitionProvider.Resolve<FakeRaisedComplexRow>();
            var complexReference = new EntityReference { EntityType = typeof(FakeRaisedComplexRow) };
            var complexLocation = definitionProvider.GetEntityLocation(complexReference);
            var complexSubIdAttribute = complexDefinition.Find("FakeSubEntityId");

            Assert.AreEqual(complexLocation, actual.SourceLocation);
            Assert.AreEqual(complexSubIdAttribute, actual.SourceAttribute);

            var subDefinition = definitionProvider.Resolve<FakeRaisedSubRow>();
            var subReference = new EntityReference { EntityType = typeof(FakeRaisedSubRow), EntityAlias = "FakeSubEntity" };
            var subLocation = definitionProvider.GetEntityLocation(subReference);
            var subIdAttribute = subDefinition.Find("FakeSubEntityId");

            Assert.AreEqual(subLocation, actual.RelationLocation);
            Assert.AreEqual(subIdAttribute, actual.RelationAttribute);
        }

        /// <summary>
        /// The join test.
        /// </summary>
        [TestMethod]
        public void Join_RelatedAttributeWithAliasToTransitiveRelatedAttributeWithAlias()
        {
            var definitionProvider = new DataItemDefinitionProvider();
            var actual = new EntityRelation(definitionProvider, EntityRelationType.InnerJoin);
            actual.Join<FakeRaisedDataRow>(row => row.RelatedAlias.RelatedId, row => row.RelatedDependency.FakeDependencyEntityId);

            var leftDefinition = definitionProvider.Resolve<FakeRelatedRow>();
            var leftReference = new EntityReference { EntityType = typeof(FakeRelatedRow), EntityAlias = "RelatedAlias" };
            var leftLocation = definitionProvider.GetEntityLocation(leftReference);
            var leftAttribute = leftDefinition.Find("RelatedId");

            Assert.AreEqual(leftLocation, actual.SourceLocation);
            Assert.AreEqual(leftAttribute, actual.SourceAttribute);

            var rightDefinition = definitionProvider.Resolve<FakeDependencyRow>();
            var rightReference = new EntityReference { EntityType = typeof(FakeDependencyRow), EntityAlias = "RelatedDependency" };
            var rightLocation = definitionProvider.GetEntityLocation(rightReference);
            var rightAttribute = rightDefinition.Find("FakeDependencyEntityId");

            Assert.AreEqual(rightLocation, actual.RelationLocation);
            Assert.AreEqual(rightAttribute, actual.RelationAttribute);
        }
    }
}