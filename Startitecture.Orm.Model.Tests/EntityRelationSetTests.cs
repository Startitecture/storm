// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityRelationSetTests.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model.Tests
{
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Orm.Testing.Entities;

    /// <summary>
    /// The entity relation set tests.
    /// </summary>
    [TestClass]
    public class EntityRelationSetTests
    {
        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void InnerJoin_WithoutRelationAlias_MatchesExpected()
        {
            var relations = new EntityRelationSet<DataRow>()
                .InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId)
                ////.InnerJoin<FakeRelatedRow, DependencyRow>(row => row.RelatedId, row => row.ComplexEntityId)
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "OtherAlias")
                ////.InnerJoin<FakeRelatedRow, DependencyRow>(
                ////    row => row.RelatedId,
                ////    "OtherAlias",
                ////    row => row.ComplexEntityId,
                ////    "RelatedDependency")
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "RelatedAlias")
                .Relations;

            var expected = new EntityRelation(EntityRelationType.InnerJoin);

            expected.Join<DataRow, FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId);

            var actual = relations.FirstOrDefault();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void InnerJoin_ExtendedRelationWithoutRelationAlias_MatchesExpected()
        {
            var relations = new EntityRelationSet<DataRow>()
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId)
                .InnerJoin<FakeRelatedRow, DependencyRow>(row => row.RelatedId, row => row.ComplexEntityId)
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "OtherAlias")
                ////.InnerJoin<FakeRelatedRow, DependencyRow>(
                ////    row => row.RelatedId,
                ////    "OtherAlias",
                ////    row => row.ComplexEntityId,
                ////    "RelatedDependency")
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "RelatedAlias")
                .Relations;

            var expected = new EntityRelation(EntityRelationType.InnerJoin);
            expected.Join<FakeRelatedRow, DependencyRow>(row => row.RelatedId, row => row.ComplexEntityId);

            var actual = relations.FirstOrDefault();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void InnerJoin_WithRelationAlias_MatchesExpected()
        {
            var relations = new EntityRelationSet<DataRow>()
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId)
                ////.InnerJoin<FakeRelatedRow, DependencyRow>(row => row.RelatedId, row => row.ComplexEntityId)
                .InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "OtherAlias")
                ////.InnerJoin<FakeRelatedRow, DependencyRow>(
                ////    row => row.RelatedId,
                ////    "OtherAlias",
                ////    row => row.ComplexEntityId,
                ////    "RelatedDependency")
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "RelatedAlias")
                .Relations;

            var expected = new EntityRelation(EntityRelationType.InnerJoin);
            expected.Join<DataRow, FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, null, "OtherAlias");

            var actual = relations.FirstOrDefault();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void InnerJoin_WithSourceAndRelationAlias_MatchesExpected()
        {
            var relations = new EntityRelationSet<DataRow>()
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId)
                ////.InnerJoin<FakeRelatedRow, DependencyRow>(row => row.RelatedId, row => row.ComplexEntityId)
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "OtherAlias")
                .InnerJoin<FakeRelatedRow, DependencyRow>(
                    row => row.RelatedId,
                    "OtherAlias",
                    row => row.ComplexEntityId,
                    "RelatedDependency")
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "RelatedAlias")
                .Relations;

            var expected = new EntityRelation(EntityRelationType.InnerJoin);
            expected.Join<FakeRelatedRow, DependencyRow>(row => row.RelatedId, row => row.ComplexEntityId, "OtherAlias", "RelatedDependency");

            var actual = relations.FirstOrDefault();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void InnerJoin_InferredWithMatchingRelationProperty_MatchesExpected()
        {
            var relations =
                new EntityRelationSet<DataRow>()
                    .InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                    ////.InnerJoin(row => row.Related, row => row.DependencyEntity, row => row.RelatedId, row => row.ComplexEntityId)
                    ////.InnerJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
                    ////.InnerJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.ComplexEntityId)
                    ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
                    .Relations;

            var expected = new EntityRelation(EntityRelationType.InnerJoin);
            expected.Join<DataRow, FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId);

            Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        }

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void InnerJoin_InferredWithMatchingSourceAndRelationProperties_MatchesExpected()
        {
            var relations =
                new EntityRelationSet<DataRow>()
                    ////.InnerJoin(row => row.Related, row => row.FakeDataId, row => row.FakeDataId)
                    .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                    ////.InnerJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
                    ////.InnerJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.ComplexEntityId)
                    ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
                    .Relations;

            var expected = new EntityRelation(EntityRelationType.InnerJoin);
            expected.Join<FakeRelatedRow, DependencyRow>(row => row.RelatedId, row => row.ComplexEntityId);

            Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        }

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void LeftJoin_WithoutRelationAlias_MatchesExpected()
        {
            var relations = new EntityRelationSet<DataRow>()
                .LeftJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId)
                ////.InnerJoin<FakeRelatedRow, DependencyRow>(row => row.RelatedId, row => row.ComplexEntityId)
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "OtherAlias")
                ////.InnerJoin<FakeRelatedRow, DependencyRow>(
                ////    row => row.RelatedId,
                ////    "OtherAlias",
                ////    row => row.ComplexEntityId,
                ////    "RelatedDependency")
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "RelatedAlias")
                .Relations;

            var expected = new EntityRelation(EntityRelationType.LeftJoin);
            expected.Join<DataRow, FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId);

            var actual = relations.FirstOrDefault();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void LeftJoin_ExtendedRelationWithoutRelationAlias_MatchesExpected()
        {
            var relations = new EntityRelationSet<DataRow>()
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId)
                .LeftJoin<FakeRelatedRow, DependencyRow>(row => row.RelatedId, row => row.ComplexEntityId)
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "OtherAlias")
                ////.InnerJoin<FakeRelatedRow, DependencyRow>(
                ////    row => row.RelatedId,
                ////    "OtherAlias",
                ////    row => row.ComplexEntityId,
                ////    "RelatedDependency")
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "RelatedAlias")
                .Relations;

            var expected = new EntityRelation(EntityRelationType.LeftJoin);
            expected.Join<FakeRelatedRow, DependencyRow>(row => row.RelatedId, row => row.ComplexEntityId);

            var actual = relations.FirstOrDefault();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void LeftJoin_WithRelationAlias_MatchesExpected()
        {
            var relations = new EntityRelationSet<DataRow>()
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId)
                ////.InnerJoin<FakeRelatedRow, DependencyRow>(row => row.RelatedId, row => row.ComplexEntityId)
                .LeftJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "OtherAlias")
                ////.InnerJoin<FakeRelatedRow, DependencyRow>(
                ////    row => row.RelatedId,
                ////    "OtherAlias",
                ////    row => row.ComplexEntityId,
                ////    "RelatedDependency")
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "RelatedAlias")
                .Relations;

            var expected = new EntityRelation(EntityRelationType.LeftJoin);
            expected.Join<DataRow, FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, null, "OtherAlias");

            var actual = relations.FirstOrDefault();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void LeftJoin_WithSourceAndRelationAlias_MatchesExpected()
        {
            var relations = new EntityRelationSet<DataRow>()
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId)
                ////.InnerJoin<FakeRelatedRow, DependencyRow>(row => row.RelatedId, row => row.ComplexEntityId)
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "OtherAlias")
                .LeftJoin<FakeRelatedRow, DependencyRow>(
                    row => row.RelatedId,
                    "OtherAlias",
                    row => row.ComplexEntityId,
                    "RelatedDependency")
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "RelatedAlias")
                .Relations;

            var expected = new EntityRelation(EntityRelationType.LeftJoin);
            expected.Join<FakeRelatedRow, DependencyRow>(row => row.RelatedId, row => row.ComplexEntityId, "OtherAlias", "RelatedDependency");

            var actual = relations.FirstOrDefault();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void LeftJoin_InferredWithMatchingRelationProperty_MatchesExpected()
        {
            var relations =
                new EntityRelationSet<DataRow>()
                    .LeftJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                    ////.InnerJoin(row => row.Related, row => row.DependencyEntity, row => row.RelatedId, row => row.ComplexEntityId)
                    ////.InnerJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
                    ////.InnerJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.ComplexEntityId)
                    ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
                    .Relations;

            var expected = new EntityRelation(EntityRelationType.LeftJoin);
            expected.Join<DataRow, FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId);

            Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        }

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void LeftJoin_InferredWithMatchingSourceAndRelationProperties_MatchesExpected()
        {
            var relations =
                new EntityRelationSet<DataRow>()
                    .LeftJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                    .Relations;

            var expected = new EntityRelation(EntityRelationType.LeftJoin);
            expected.Join<FakeRelatedRow, DependencyRow>(row => row.RelatedId, row => row.ComplexEntityId);

            Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        }

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void InnerJoin_EntityRelationSetWithTwoExternalRelations_MatchesExpected()
        {
            var relations = new EntityRelationSet<DataRow>().InnerJoin<FakeRelatedRow, DependencyRow>(
                row => row.RelatedId,
                row => row.FakeDependencyEntityId,
                "Alias").Relations;

            var expected = new EntityRelation(EntityRelationType.InnerJoin);
            expected.Join<FakeRelatedRow, DependencyRow>(row => row.RelatedId, row => row.FakeDependencyEntityId, null, "Alias");

            Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        }

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void LeftJoin_EntityRelationSetWithTwoExternalRelations_MatchesExpected()
        {
            var relations = new EntityRelationSet<DataRow>().LeftJoin<FakeRelatedRow, DependencyRow>(
                row => row.RelatedId,
                row => row.FakeDependencyEntityId,
                "Alias").Relations;

            var expected = new EntityRelation(EntityRelationType.LeftJoin);
            expected.Join<FakeRelatedRow, DependencyRow>(row => row.RelatedId, row => row.FakeDependencyEntityId, null, "Alias");

            Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        }
    }
}