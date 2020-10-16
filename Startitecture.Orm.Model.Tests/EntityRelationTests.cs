// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityRelationTests.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Orm.Schema;
    using Startitecture.Orm.Testing.Entities;

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
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var actual = new EntityRelation(EntityRelationType.InnerJoin);
            actual.Join<ChildRaisedRow>(row => row.ComplexEntityId, row => row.ComplexEntity.ComplexEntityId);

            var childDefinition = definitionProvider.Resolve<ChildRaisedRow>();
            var childReference = new EntityReference { EntityType = typeof(ChildRaisedRow) };
            var childLocation = definitionProvider.GetEntityLocation(childReference);
            var childAttributeLocation = new AttributeLocation(typeof(ChildRaisedRow).GetProperty("ComplexEntityId"), childReference);
            var childComplexIdAttribute = childDefinition.Find(childAttributeLocation);

            Assert.AreEqual(childLocation, childDefinition.Find(actual.SourceExpression).Entity);
            Assert.AreEqual(childComplexIdAttribute, childDefinition.Find(actual.SourceExpression)); //// actual.SourceAttribute);

            var complexDefinition = definitionProvider.Resolve<ComplexRaisedRow>();
            var complexReference = new EntityReference { EntityType = typeof(ComplexRaisedRow) };
            var complexLocation = definitionProvider.GetEntityLocation(complexReference);
            var complexAttributeLocation = new AttributeLocation(typeof(ComplexRaisedRow).GetProperty("ComplexEntityId"), complexReference);
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
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var actual = new EntityRelation(EntityRelationType.InnerJoin);
            actual.Join<DataRow>(row => row.Related.RelatedId, row => row.DependencyEntity.FakeDependencyEntityId);

            var relatedDefinition = definitionProvider.Resolve<FakeRelatedRow>();
            var leftDefinition = relatedDefinition;
            var leftReference = new EntityReference { EntityType = typeof(FakeRelatedRow) };
            var leftLocation = definitionProvider.GetEntityLocation(leftReference);
            var leftAttributeLocation = new AttributeLocation(typeof(FakeRelatedRow).GetProperty(nameof(FakeRelatedRow.RelatedId)), leftReference);
            var leftAttribute = leftDefinition.Find(leftAttributeLocation);

            Assert.AreEqual(leftLocation, relatedDefinition.Find(actual.SourceExpression).Entity);
            Assert.AreEqual(leftAttribute, relatedDefinition.Find(actual.SourceExpression));

            var rightDefinition = definitionProvider.Resolve<DependencyRow>();
            var rightReference = new EntityReference { EntityType = typeof(DependencyRow) };
            var rightLocation = definitionProvider.GetEntityLocation(rightReference);
            var rightAttributeLocation = new AttributeLocation(typeof(DependencyRow).GetProperty(nameof(DependencyRow.FakeDependencyEntityId)), rightReference);
            var rightAttribute = rightDefinition.Find(rightAttributeLocation);

            Assert.AreEqual(rightLocation, rightDefinition.Find(actual.RelationExpression).Entity);
            Assert.AreEqual(rightAttribute, rightDefinition.Find(actual.RelationExpression));
        }
    }
}