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
                ////.InnerJoin<FakeRelatedRow, DependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId)
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "OtherAlias")
                ////.InnerJoin<FakeRelatedRow, DependencyRow>(
                ////    row => row.RelatedId,
                ////    "OtherAlias",
                ////    row => row.FakeComplexEntityId,
                ////    "RelatedDependency")
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "RelatedAlias")
                .Relations;

            var expected = new EntityRelation(EntityRelationType.InnerJoin);

            expected.Join<DataRow, FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId);

            var actual = relations.FirstOrDefault();
            Assert.AreEqual(expected, actual);
        }

        /////// <summary>
        /////// The inner join test.
        /////// </summary>
        ////[TestMethod]
        ////public void InnerJoin_WithMatchingRelationProperty_MatchesExpected()
        ////{
        ////    var relations =
        ////        new SqlFromClause<DataRow>()
        ////            .InnerJoin(row => row.Related, row => row.FakeDataId, row => row.FakeDataId)
        ////            ////.InnerJoin(row => row.Related, row => row.DependencyEntity, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            ////.InnerJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            .Relations;

        ////    var expected = new EntityRelation(new PetaPocoDefinitionProvider(), EntityRelationType.InnerJoin);
        ////    expected.Join<DataRow, FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId);

        ////    var actual = relations.FirstOrDefault();
        ////    Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, Evaluate.GetDifferences(expected, actual)));
        ////}

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void InnerJoin_ExtendedRelationWithoutRelationAlias_MatchesExpected()
        {
            var relations = new EntityRelationSet<DataRow>()
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId)
                .InnerJoin<FakeRelatedRow, DependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId)
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "OtherAlias")
                ////.InnerJoin<FakeRelatedRow, DependencyRow>(
                ////    row => row.RelatedId,
                ////    "OtherAlias",
                ////    row => row.FakeComplexEntityId,
                ////    "RelatedDependency")
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "RelatedAlias")
                .Relations;

            var expected = new EntityRelation(EntityRelationType.InnerJoin);
            expected.Join<FakeRelatedRow, DependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId);

            var actual = relations.FirstOrDefault();
            Assert.AreEqual(expected, actual);
        }

        /////// <summary>
        /////// The inner join test.
        /////// </summary>
        ////[TestMethod]
        ////public void InnerJoin_WithMatchingSourceAndRelationProperties_MatchesExpected()
        ////{
        ////    var relations =
        ////        new SqlFromClause<DataRow>()
        ////            ////.InnerJoin(row => row.Related, row => row.FakeDataId, row => row.FakeDataId)
        ////            .InnerJoin(row => row.Related, row => row.DependencyEntity, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            ////.InnerJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            .Relations;

        ////    var expected = new EntityRelation(new PetaPocoDefinitionProvider(), EntityRelationType.InnerJoin);
        ////    expected.Join<FakeRelatedRow, DependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId);

        ////    var actual = relations.FirstOrDefault();
        ////    Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, Evaluate.GetDifferences(expected, actual)));
        ////}

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void InnerJoin_WithRelationAlias_MatchesExpected()
        {
            var relations = new EntityRelationSet<DataRow>()
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId)
                ////.InnerJoin<FakeRelatedRow, DependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId)
                .InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "OtherAlias")
                ////.InnerJoin<FakeRelatedRow, DependencyRow>(
                ////    row => row.RelatedId,
                ////    "OtherAlias",
                ////    row => row.FakeComplexEntityId,
                ////    "RelatedDependency")
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "RelatedAlias")
                .Relations;

            var expected = new EntityRelation(EntityRelationType.InnerJoin);
            expected.Join<DataRow, FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, null, "OtherAlias");

            var actual = relations.FirstOrDefault();
            Assert.AreEqual(expected, actual);
        }

        /////// <summary>
        /////// The inner join test.
        /////// </summary>
        ////[TestMethod]
        ////public void InnerJoin_WithRelationProperty_MatchesExpected()
        ////{
        ////    var relations =
        ////        new SqlFromClause<DataRow>()
        ////            ////.InnerJoin(row => row.Related, row => row.FakeDataId, row => row.FakeDataId)
        ////            ////.InnerJoin(row => row.Related, row => row.DependencyEntity, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            .InnerJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            ////.InnerJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            .Relations;

        ////    var expected = new EntityRelation(new PetaPocoDefinitionProvider(), EntityRelationType.InnerJoin);
        ////    expected.Join<DataRow, FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, null, "OtherAlias");

        ////    var actual = relations.FirstOrDefault();
        ////    Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, Evaluate.GetDifferences(expected, actual)));
        ////}

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void InnerJoin_WithSourceAndRelationAlias_MatchesExpected()
        {
            var relations = new EntityRelationSet<DataRow>()
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId)
                ////.InnerJoin<FakeRelatedRow, DependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId)
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "OtherAlias")
                .InnerJoin<FakeRelatedRow, DependencyRow>(
                    row => row.RelatedId,
                    "OtherAlias",
                    row => row.FakeComplexEntityId,
                    "RelatedDependency")
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "RelatedAlias")
                .Relations;

            var expected = new EntityRelation(EntityRelationType.InnerJoin);
            expected.Join<FakeRelatedRow, DependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId, "OtherAlias", "RelatedDependency");

            var actual = relations.FirstOrDefault();
            Assert.AreEqual(expected, actual);
        }

        /////// <summary>
        /////// The inner join test.
        /////// </summary>
        ////[TestMethod]
        ////public void InnerJoin_WithSourceAndRelationProperty_MatchesExpected()
        ////{
        ////    var relations =
        ////        new SqlFromClause<DataRow>()
        ////            ////.InnerJoin(row => row.Related, row => row.FakeDataId, row => row.FakeDataId)
        ////            ////.InnerJoin(row => row.Related, row => row.DependencyEntity, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            .InnerJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            .Relations;

        ////    var expected = new EntityRelation(new PetaPocoDefinitionProvider(), EntityRelationType.InnerJoin);
        ////    expected.Join<FakeRelatedRow, DependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId, "OtherAlias", "RelatedDependency");

        ////    var actual = relations.FirstOrDefault();
        ////    Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, Evaluate.GetDifferences(expected, actual)));
        ////}

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void InnerJoin_InferredWithMatchingRelationProperty_MatchesExpected()
        {
            var relations =
                new EntityRelationSet<DataRow>()
                    .InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                    ////.InnerJoin(row => row.Related, row => row.DependencyEntity, row => row.RelatedId, row => row.FakeComplexEntityId)
                    ////.InnerJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
                    ////.InnerJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.FakeComplexEntityId)
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
                    .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.FakeComplexEntityId)
                    ////.InnerJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
                    ////.InnerJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.FakeComplexEntityId)
                    ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
                    .Relations;

            var expected = new EntityRelation(EntityRelationType.InnerJoin);
            expected.Join<FakeRelatedRow, DependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId);

            Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        }

        // TODO: No longer valid.
        /////// <summary>
        /////// The inner join test.
        /////// </summary>
        ////[TestMethod]
        ////public void InnerJoin_InferredWithRelationProperty_MatchesExpected()
        ////{
        ////    var relations =
        ////        new SqlFromClause<DataRow>()
        ////            ////.InnerJoin(row => row.Related, row => row.FakeDataId, row => row.FakeDataId)
        ////            ////.InnerJoin(row => row.Related, row => row.DependencyEntity, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
        ////            ////.InnerJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            .Relations;

        ////    var expected = new EntityRelation(EntityRelationType.InnerJoin);
        ////    expected.Join<DataRow, FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, null, "OtherAlias");

        ////    Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        ////}

        // TODO: No longer valid.
        /////// <summary>
        /////// The inner join test.
        /////// </summary>
        ////[TestMethod]
        ////public void InnerJoin_InferredWithSourceAndRelationProperty_MatchesExpected()
        ////{
        ////    var relations =
        ////        new SqlFromClause<DataRow>()
        ////            ////.InnerJoin(row => row.Related, row => row.FakeDataId, row => row.FakeDataId)
        ////            ////.InnerJoin(row => row.Related, row => row.DependencyEntity, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            .Relations;

        ////    var expected = new EntityRelation(EntityRelationType.InnerJoin);
        ////    expected.Join<FakeRelatedRow, DependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId, "OtherAlias", "RelatedDependency");

        ////    Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        ////}

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void LeftJoin_WithoutRelationAlias_MatchesExpected()
        {
            var relations = new EntityRelationSet<DataRow>()
                .LeftJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId)
                ////.InnerJoin<FakeRelatedRow, DependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId)
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "OtherAlias")
                ////.InnerJoin<FakeRelatedRow, DependencyRow>(
                ////    row => row.RelatedId,
                ////    "OtherAlias",
                ////    row => row.FakeComplexEntityId,
                ////    "RelatedDependency")
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "RelatedAlias")
                .Relations;

            var expected = new EntityRelation(EntityRelationType.LeftJoin);
            expected.Join<DataRow, FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId);

            var actual = relations.FirstOrDefault();
            Assert.AreEqual(expected, actual);
        }

        /////// <summary>
        /////// The inner join test.
        /////// </summary>
        ////[TestMethod]
        ////public void LeftJoin_WithMatchingRelationProperty_MatchesExpected()
        ////{
        ////    var relations =
        ////        new SqlFromClause<DataRow>()
        ////            .LeftJoin(row => row.Related, row => row.FakeDataId, row => row.FakeDataId)
        ////            ////.InnerJoin(row => row.Related, row => row.DependencyEntity, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            ////.InnerJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            .Relations;

        ////    var expected = new EntityRelation(new PetaPocoDefinitionProvider(), EntityRelationType.LeftJoin);
        ////    expected.Join<DataRow, FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId);

        ////    var actual = relations.FirstOrDefault();
        ////    Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, Evaluate.GetDifferences(expected, actual)));
        ////}

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void LeftJoin_ExtendedRelationWithoutRelationAlias_MatchesExpected()
        {
            var relations = new EntityRelationSet<DataRow>()
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId)
                .LeftJoin<FakeRelatedRow, DependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId)
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "OtherAlias")
                ////.InnerJoin<FakeRelatedRow, DependencyRow>(
                ////    row => row.RelatedId,
                ////    "OtherAlias",
                ////    row => row.FakeComplexEntityId,
                ////    "RelatedDependency")
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "RelatedAlias")
                .Relations;

            var expected = new EntityRelation(EntityRelationType.LeftJoin);
            expected.Join<FakeRelatedRow, DependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId);

            var actual = relations.FirstOrDefault();
            Assert.AreEqual(expected, actual);
        }

        /////// <summary>
        /////// The inner join test.
        /////// </summary>
        ////[TestMethod]
        ////public void LeftJoin_WithMatchingSourceAndRelationProperties_MatchesExpected()
        ////{
        ////    var relations =
        ////        new SqlFromClause<DataRow>()
        ////            ////.InnerJoin(row => row.Related, row => row.FakeDataId, row => row.FakeDataId)
        ////            .LeftJoin(row => row.Related, row => row.DependencyEntity, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            ////.InnerJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            .Relations;

        ////    var expected = new EntityRelation(new PetaPocoDefinitionProvider(), EntityRelationType.LeftJoin);
        ////    expected.Join<FakeRelatedRow, DependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId);

        ////    var actual = relations.FirstOrDefault();
        ////    Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, Evaluate.GetDifferences(expected, actual)));
        ////}

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void LeftJoin_WithRelationAlias_MatchesExpected()
        {
            var relations = new EntityRelationSet<DataRow>()
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId)
                ////.InnerJoin<FakeRelatedRow, DependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId)
                .LeftJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "OtherAlias")
                ////.InnerJoin<FakeRelatedRow, DependencyRow>(
                ////    row => row.RelatedId,
                ////    "OtherAlias",
                ////    row => row.FakeComplexEntityId,
                ////    "RelatedDependency")
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "RelatedAlias")
                .Relations;

            var expected = new EntityRelation(EntityRelationType.LeftJoin);
            expected.Join<DataRow, FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, null, "OtherAlias");

            var actual = relations.FirstOrDefault();
            Assert.AreEqual(expected, actual);
        }

        /////// <summary>
        /////// The inner join test.
        /////// </summary>
        ////[TestMethod]
        ////public void LeftJoin_WithRelationProperty_MatchesExpected()
        ////{
        ////    var relations =
        ////        new SqlFromClause<DataRow>()
        ////            ////.InnerJoin(row => row.Related, row => row.FakeDataId, row => row.FakeDataId)
        ////            ////.InnerJoin(row => row.Related, row => row.DependencyEntity, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            .LeftJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            ////.InnerJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            .Relations;

        ////    var expected = new EntityRelation(new PetaPocoDefinitionProvider(), EntityRelationType.LeftJoin);
        ////    expected.Join<DataRow, FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, null, "OtherAlias");

        ////    var actual = relations.FirstOrDefault();
        ////    Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, Evaluate.GetDifferences(expected, actual)));
        ////}

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void LeftJoin_WithSourceAndRelationAlias_MatchesExpected()
        {
            var relations = new EntityRelationSet<DataRow>()
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId)
                ////.InnerJoin<FakeRelatedRow, DependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId)
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "OtherAlias")
                .LeftJoin<FakeRelatedRow, DependencyRow>(
                    row => row.RelatedId,
                    "OtherAlias",
                    row => row.FakeComplexEntityId,
                    "RelatedDependency")
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "RelatedAlias")
                .Relations;

            var expected = new EntityRelation(EntityRelationType.LeftJoin);
            expected.Join<FakeRelatedRow, DependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId, "OtherAlias", "RelatedDependency");

            var actual = relations.FirstOrDefault();
            Assert.AreEqual(expected, actual);
        }

        /////// <summary>
        /////// The inner join test.
        /////// </summary>
        ////[TestMethod]
        ////public void LeftJoin_WithSourceAndRelationProperty_MatchesExpected()
        ////{
        ////    var relations =
        ////        new SqlFromClause<DataRow>()
        ////            ////.InnerJoin(row => row.Related, row => row.FakeDataId, row => row.FakeDataId)
        ////            ////.InnerJoin(row => row.Related, row => row.DependencyEntity, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            .LeftJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            .Relations;

        ////    var expected = new EntityRelation(new PetaPocoDefinitionProvider(), EntityRelationType.LeftJoin);
        ////    expected.Join<FakeRelatedRow, DependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId, "OtherAlias", "RelatedDependency");

        ////    var actual = relations.FirstOrDefault();
        ////    Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, Evaluate.GetDifferences(expected, actual)));
        ////}

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void LeftJoin_InferredWithMatchingRelationProperty_MatchesExpected()
        {
            var relations =
                new EntityRelationSet<DataRow>()
                    .LeftJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                    ////.InnerJoin(row => row.Related, row => row.DependencyEntity, row => row.RelatedId, row => row.FakeComplexEntityId)
                    ////.InnerJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
                    ////.InnerJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.FakeComplexEntityId)
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
                    ////.InnerJoin(row => row.Related, row => row.FakeDataId, row => row.FakeDataId)
                    .LeftJoin(row => row.Related.RelatedId, row => row.DependencyEntity.FakeComplexEntityId)
                    ////.InnerJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
                    ////.InnerJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.FakeComplexEntityId)
                    ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
                    .Relations;

            var expected = new EntityRelation(EntityRelationType.LeftJoin);
            expected.Join<FakeRelatedRow, DependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId);

            Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        }

        // TODO: No longer valid.
        /////// <summary>
        /////// The inner join test.
        /////// </summary>
        ////[TestMethod]
        ////public void LeftJoin_InferredWithRelationProperty_MatchesExpected()
        ////{
        ////    var relations =
        ////        new SqlFromClause<DataRow>()
        ////            ////.InnerJoin(row => row.Related, row => row.FakeDataId, row => row.FakeDataId)
        ////            ////.InnerJoin(row => row.Related, row => row.DependencyEntity, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            .LeftJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
        ////            ////.InnerJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            .Relations;

        ////    var expected = new EntityRelation(EntityRelationType.LeftJoin);
        ////    expected.Join<DataRow, FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, null, "OtherAlias");

        ////    Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        ////}

        // TODO: No longer valid.
        /////// <summary>
        /////// The inner join test.
        /////// </summary>
        ////[TestMethod]
        ////public void LeftJoin_InferredWithSourceAndRelationProperty_MatchesExpected()
        ////{
        ////    var relations =
        ////        new SqlFromClause<DataRow>()
        ////            ////.InnerJoin(row => row.Related, row => row.FakeDataId, row => row.FakeDataId)
        ////            ////.InnerJoin(row => row.Related, row => row.DependencyEntity, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            .LeftJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            .Relations;

        ////    var expected = new EntityRelation(EntityRelationType.LeftJoin);
        ////    expected.Join<FakeRelatedRow, DependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId, "OtherAlias", "RelatedDependency");

        ////    Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        ////}
    }
}