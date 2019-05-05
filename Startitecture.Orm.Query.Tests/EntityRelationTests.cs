// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityRelationTests.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Query.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Orm.Model;
    using Startitecture.Orm.Schema;
    using Startitecture.Orm.Testing.Model;

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
        public void Join_LocalAttributeToRelatedAttribute_MatchesExpected()
        {
            var definitionProvider = new PetaPocoDefinitionProvider();
            var actual = new EntityRelation(EntityRelationType.InnerJoin);
            actual.Join<FakeRaisedChildRow>(row => row.FakeComplexEntityId, row => row.FakeComplexEntity.FakeComplexEntityId);

            var childDefinition = definitionProvider.Resolve<FakeRaisedChildRow>();
            var childReference = new EntityReference { EntityType = typeof(FakeRaisedChildRow) };
            var childLocation = definitionProvider.GetEntityLocation(childReference);
            var childAttributeLocation = new AttributeLocation(typeof(FakeRaisedChildRow).GetProperty("FakeComplexEntityId"), childReference);
            var childComplexIdAttribute = childDefinition.Find(childAttributeLocation);

            Assert.AreEqual(childLocation, childDefinition.Find(actual.SourceExpression).Entity);
            Assert.AreEqual(childComplexIdAttribute, childDefinition.Find(actual.SourceExpression)); //// actual.SourceAttribute);

            var complexDefinition = definitionProvider.Resolve<FakeRaisedComplexRow>();
            var complexReference = new EntityReference { EntityType = typeof(FakeRaisedComplexRow) };
            var complexLocation = definitionProvider.GetEntityLocation(complexReference);
            var complexAttributeLocation = new AttributeLocation(typeof(FakeRaisedComplexRow).GetProperty("FakeComplexEntityId"), complexReference);
            var complexIdAttribute = complexDefinition.Find(complexAttributeLocation);

            Assert.AreEqual(complexLocation, complexDefinition.Find(actual.RelationExpression).Entity); //// actual.RelationLocation);
            Assert.AreEqual(complexIdAttribute, complexDefinition.Find(actual.RelationExpression));
        }

        /// <summary>
        /// The join test.
        /// </summary>
        [TestMethod]
        public void Join_RelatedAttributeToTransitiveRelatedAttribute_MatchesExpected()
        {
            var definitionProvider = new PetaPocoDefinitionProvider();
            var actual = new EntityRelation(EntityRelationType.InnerJoin);
            actual.Join<FakeRaisedDataRow>(row => row.FakeRelated.RelatedId, row => row.FakeDependencyEntity.FakeDependencyEntityId);

            var relatedDefinition = definitionProvider.Resolve<FakeRelatedRow>();
            var leftDefinition = relatedDefinition;
            var leftReference = new EntityReference { EntityType = typeof(FakeRelatedRow) };
            var leftLocation = definitionProvider.GetEntityLocation(leftReference);
            var leftAttributeLocation = new AttributeLocation(typeof(FakeRelatedRow).GetProperty("RelatedId"), leftReference);
            var leftAttribute = leftDefinition.Find(leftAttributeLocation);

            Assert.AreEqual(leftLocation, relatedDefinition.Find(actual.SourceExpression).Entity);
            Assert.AreEqual(leftAttribute, relatedDefinition.Find(actual.SourceExpression)); //// actual.SourceAttribute);

            var rightDefinition = definitionProvider.Resolve<FakeDependencyRow>();
            var rightReference = new EntityReference { EntityType = typeof(FakeDependencyRow) };
            var rightLocation = definitionProvider.GetEntityLocation(rightReference);
            var rightAttributeLocation = new AttributeLocation(typeof(FakeDependencyRow).GetProperty("FakeDependencyEntityId"), rightReference);
            var rightAttribute = rightDefinition.Find(rightAttributeLocation);

            Assert.AreEqual(rightLocation, rightDefinition.Find(actual.RelationExpression).Entity); //// actual.RelationLocation);
            Assert.AreEqual(rightAttribute, rightDefinition.Find(actual.RelationExpression));
        }

        // TODO: No longer valid. Attributes are not evaluated at this layer.
        /////// <summary>
        /////// The join test.
        /////// </summary>
        ////[TestMethod]
        ////public void Join_RelatedAttributeToTransitiveRelatedAttributeWithAlias()
        ////{
        ////    var definitionProvider = new PetaPocoDefinitionProvider();
        ////    var actual = new EntityRelation(EntityRelationType.InnerJoin);
        ////    actual.Join<FakeRaisedChildRow>(row => row.FakeComplexEntity.FakeSubEntityId, row => row.FakeComplexEntity.FakeSubEntity.FakeSubEntityId);

        ////    var complexDefinition = definitionProvider.Resolve<FakeRaisedComplexRow>();
        ////    var complexReference = new EntityReference { EntityType = typeof(FakeRaisedComplexRow) };
        ////    var complexLocation = definitionProvider.GetEntityLocation(complexReference);
        ////    var complexSubIdAttribute = complexDefinition.Find("FakeSubEntityId");

        ////    Assert.AreEqual(complexLocation, complexDefinition.Find(actual.SourceExpression).Entity);
        ////    Assert.AreEqual(complexSubIdAttribute, complexDefinition.Find(actual.SourceExpression)); //// actual.SourceAttribute);

        ////    var subDefinition = definitionProvider.Resolve<FakeRaisedSubRow>();
        ////    var subReference = new EntityReference { EntityType = typeof(FakeRaisedSubRow), EntityAlias = "FakeSubEntity" };
        ////    var subLocation = definitionProvider.GetEntityLocation(subReference);
        ////    var subIdAttribute = subDefinition.Find("FakeSubEntityId");

        ////    Assert.AreEqual(subLocation, subDefinition.Find(actual.RelationExpression).Entity);
        ////    Assert.AreEqual(subIdAttribute, subDefinition.Find(actual.RelationExpression));
        ////}

        // TODO: No longer valid. Attributes are not evaluated at this layer.
        /////// <summary>
        /////// The join test.
        /////// </summary>
        ////[TestMethod]
        ////public void Join_RelatedAttributeWithAliasToTransitiveRelatedAttributeWithAlias()
        ////{
        ////    var definitionProvider = new PetaPocoDefinitionProvider();
        ////    var actual = new EntityRelation(EntityRelationType.InnerJoin);
        ////    actual.Join<FakeRaisedDataRow>(row => row.RelatedAlias.RelatedId, row => row.RelatedDependency.FakeDependencyEntityId);

        ////    var leftDefinition = definitionProvider.Resolve<FakeRelatedRow>();
        ////    var leftReference = new EntityReference { EntityType = typeof(FakeRelatedRow), EntityAlias = "RelatedAlias" };
        ////    var leftLocation = definitionProvider.GetEntityLocation(leftReference);
        ////    var leftAttribute = leftDefinition.Find("RelatedId");

        ////    Assert.AreEqual(leftLocation, leftDefinition.Find(actual.SourceExpression).Entity);
        ////    Assert.AreEqual(leftAttribute, leftDefinition.Find(actual.SourceExpression)); ////actual.SourceAttribute);

        ////    var rightDefinition = definitionProvider.Resolve<FakeDependencyRow>();
        ////    var rightReference = new EntityReference { EntityType = typeof(FakeDependencyRow), EntityAlias = "RelatedDependency" };
        ////    var rightLocation = definitionProvider.GetEntityLocation(rightReference);
        ////    var rightAttribute = rightDefinition.Find("FakeDependencyEntityId");

        ////    var actualRightDefinition = rightDefinition.Find(actual.RelationExpression);
        ////    Assert.AreEqual(rightLocation, actualRightDefinition.Entity);
        ////    Assert.AreEqual(rightAttribute, actualRightDefinition);
        ////}
    }
}