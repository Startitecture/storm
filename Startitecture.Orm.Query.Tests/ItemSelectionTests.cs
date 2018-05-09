
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemSelectionTests.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Startitecture.Orm.Query.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Core;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Schema;
    using Startitecture.Orm.Sql;
    using Startitecture.Orm.Testing.Model;

    /// <summary>
    /// The item selection tests.
    /// </summary>
    [TestClass]
    public class ItemSelectionTests
    {
        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void InnerJoin_WithoutRelationAlias_MatchesExpected()
        {
            var relations = new SqlSelection<FakeFlatDataRow>()
                .InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId)

                ////.InnerJoin<FakeRelatedRow, FakeDependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId)
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "OtherAlias")
                ////.InnerJoin<FakeRelatedRow, FakeDependencyRow>(
                ////    row => row.RelatedId,
                ////    "OtherAlias",
                ////    row => row.FakeComplexEntityId,
                ////    "RelatedDependency")
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "RelatedAlias")
                .Relations;

            var expected = new EntityRelation(EntityRelationType.InnerJoin);

            expected.Join<FakeFlatDataRow, FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId);

            Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        }

        /////// <summary>
        /////// The inner join test.
        /////// </summary>
        ////[TestMethod]
        ////public void InnerJoin_WithMatchingRelationProperty_MatchesExpected()
        ////{
        ////    var relations =
        ////        new SqlSelection<FakeRaisedDataRow>()
        ////            .InnerJoin(row => row.FakeRelated, row => row.FakeDataId, row => row.FakeDataId)
        ////            ////.InnerJoin(row => row.FakeRelated, row => row.FakeDependencyEntity, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            ////.InnerJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            .Relations;

        ////    var expected = new EntityRelation(new PetaPocoDefinitionProvider(), EntityRelationType.InnerJoin);
        ////    expected.Join<FakeRaisedDataRow, FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId);

        ////    Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        ////}

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void InnerJoin_ExtendedRelationWithoutRelationAlias_MatchesExpected()
        {
            var relations = new SqlSelection<FakeFlatDataRow>()

                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId)
                .InnerJoin<FakeRelatedRow, FakeDependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId)

                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "OtherAlias")
                ////.InnerJoin<FakeRelatedRow, FakeDependencyRow>(
                ////    row => row.RelatedId,
                ////    "OtherAlias",
                ////    row => row.FakeComplexEntityId,
                ////    "RelatedDependency")
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "RelatedAlias")
                .Relations;

            var expected = new EntityRelation(EntityRelationType.InnerJoin);
            expected.Join<FakeRelatedRow, FakeDependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId);

            Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        }

        /////// <summary>
        /////// The inner join test.
        /////// </summary>
        ////[TestMethod]
        ////public void InnerJoin_WithMatchingSourceAndRelationProperties_MatchesExpected()
        ////{
        ////    var relations =
        ////        new SqlSelection<FakeRaisedDataRow>()
        ////            ////.InnerJoin(row => row.FakeRelated, row => row.FakeDataId, row => row.FakeDataId)
        ////            .InnerJoin(row => row.FakeRelated, row => row.FakeDependencyEntity, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            ////.InnerJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            .Relations;

        ////    var expected = new EntityRelation(new PetaPocoDefinitionProvider(), EntityRelationType.InnerJoin);
        ////    expected.Join<FakeRelatedRow, FakeDependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId);

        ////    Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        ////}

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void InnerJoin_WithRelationAlias_MatchesExpected()
        {
            var relations = new SqlSelection<FakeFlatDataRow>()

                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId)
                ////.InnerJoin<FakeRelatedRow, FakeDependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId)
                .InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "OtherAlias")

                ////.InnerJoin<FakeRelatedRow, FakeDependencyRow>(
                ////    row => row.RelatedId,
                ////    "OtherAlias",
                ////    row => row.FakeComplexEntityId,
                ////    "RelatedDependency")
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "RelatedAlias")
                .Relations;

            var expected = new EntityRelation(EntityRelationType.InnerJoin);
            expected.Join<FakeFlatDataRow, FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, null, "OtherAlias");

            Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        }

        /////// <summary>
        /////// The inner join test.
        /////// </summary>
        ////[TestMethod]
        ////public void InnerJoin_WithRelationProperty_MatchesExpected()
        ////{
        ////    var relations =
        ////        new SqlSelection<FakeRaisedDataRow>()
        ////            ////.InnerJoin(row => row.FakeRelated, row => row.FakeDataId, row => row.FakeDataId)
        ////            ////.InnerJoin(row => row.FakeRelated, row => row.FakeDependencyEntity, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            .InnerJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            ////.InnerJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            .Relations;

        ////    var expected = new EntityRelation(new PetaPocoDefinitionProvider(), EntityRelationType.InnerJoin);
        ////    expected.Join<FakeRaisedDataRow, FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, null, "OtherAlias");

        ////    Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        ////}

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void InnerJoin_WithSourceAndRelationAlias_MatchesExpected()
        {
            var relations = new SqlSelection<FakeFlatDataRow>()

                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId)
                ////.InnerJoin<FakeRelatedRow, FakeDependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId)
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "OtherAlias")
                .InnerJoin<FakeRelatedRow, FakeDependencyRow>(row => row.RelatedId, "OtherAlias", row => row.FakeComplexEntityId, "RelatedDependency")

                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "RelatedAlias")
                .Relations;

            var expected = new EntityRelation(EntityRelationType.InnerJoin);
            expected.Join<FakeRelatedRow, FakeDependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId, "OtherAlias", "RelatedDependency");

            Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        }

        /////// <summary>
        /////// The inner join test.
        /////// </summary>
        ////[TestMethod]
        ////public void InnerJoin_WithSourceAndRelationProperty_MatchesExpected()
        ////{
        ////    var relations =
        ////        new SqlSelection<FakeRaisedDataRow>()
        ////            ////.InnerJoin(row => row.FakeRelated, row => row.FakeDataId, row => row.FakeDataId)
        ////            ////.InnerJoin(row => row.FakeRelated, row => row.FakeDependencyEntity, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            .InnerJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            .Relations;

        ////    var expected = new EntityRelation(new PetaPocoDefinitionProvider(), EntityRelationType.InnerJoin);
        ////    expected.Join<FakeRelatedRow, FakeDependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId, "OtherAlias", "RelatedDependency");

        ////    Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        ////}

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void InnerJoin_InferredWithMatchingRelationProperty_MatchesExpected()
        {
            var relations = new SqlSelection<FakeRaisedDataRow>()
                .InnerJoin(row => row.FakeDataId, row => row.FakeRelated.FakeDataId)

                ////.InnerJoin(row => row.FakeRelated, row => row.FakeDependencyEntity, row => row.RelatedId, row => row.FakeComplexEntityId)
                ////.InnerJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
                ////.InnerJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.FakeComplexEntityId)
                ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
                .Relations;

            var expected = new EntityRelation(EntityRelationType.InnerJoin);
            expected.Join<FakeRaisedDataRow, FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId);

            Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        }

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void InnerJoin_InferredWithMatchingSourceAndRelationProperties_MatchesExpected()
        {
            var relations = new SqlSelection<FakeRaisedDataRow>()

                ////.InnerJoin(row => row.FakeRelated, row => row.FakeDataId, row => row.FakeDataId)
                .InnerJoin(row => row.FakeRelated.RelatedId, row => row.FakeDependencyEntity.FakeComplexEntityId)

                ////.InnerJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
                ////.InnerJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.FakeComplexEntityId)
                ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
                .Relations;

            var expected = new EntityRelation(EntityRelationType.InnerJoin);
            expected.Join<FakeRelatedRow, FakeDependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId);

            Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        }

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void InnerJoin_InferredWithRelationProperty_MatchesExpected()
        {
            var relations = new SqlSelection<FakeRaisedDataRow>()

                ////.InnerJoin(row => row.FakeRelated, row => row.FakeDataId, row => row.FakeDataId)
                ////.InnerJoin(row => row.FakeRelated, row => row.FakeDependencyEntity, row => row.RelatedId, row => row.FakeComplexEntityId)
                .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)

                ////.InnerJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.FakeComplexEntityId)
                ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
                .Relations;

            var expected = new EntityRelation(EntityRelationType.InnerJoin);
            expected.Join<FakeRaisedDataRow, FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, null, "OtherAlias");

            Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        }

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void InnerJoin_InferredWithSourceAndRelationProperty_MatchesExpected()
        {
            var relations = new SqlSelection<FakeRaisedDataRow>()

                ////.InnerJoin(row => row.FakeRelated, row => row.FakeDataId, row => row.FakeDataId)
                ////.InnerJoin(row => row.FakeRelated, row => row.FakeDependencyEntity, row => row.RelatedId, row => row.FakeComplexEntityId)
                ////.InnerJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
                .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.FakeComplexEntityId)

                ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
                .Relations;

            var expected = new EntityRelation(EntityRelationType.InnerJoin);
            expected.Join<FakeRelatedRow, FakeDependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId, "OtherAlias", "RelatedDependency");

            Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        }

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void LeftJoin_WithoutRelationAlias_MatchesExpected()
        {
            var relations = new SqlSelection<FakeFlatDataRow>()
                .LeftJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId)

                ////.InnerJoin<FakeRelatedRow, FakeDependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId)
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "OtherAlias")
                ////.InnerJoin<FakeRelatedRow, FakeDependencyRow>(
                ////    row => row.RelatedId,
                ////    "OtherAlias",
                ////    row => row.FakeComplexEntityId,
                ////    "RelatedDependency")
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "RelatedAlias")
                .Relations;

            var expected = new EntityRelation(EntityRelationType.LeftJoin);
            expected.Join<FakeFlatDataRow, FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId);

            Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        }

        /////// <summary>
        /////// The inner join test.
        /////// </summary>
        ////[TestMethod]
        ////public void LeftJoin_WithMatchingRelationProperty_MatchesExpected()
        ////{
        ////    var relations =
        ////        new SqlSelection<FakeRaisedDataRow>()
        ////            .LeftJoin(row => row.FakeRelated, row => row.FakeDataId, row => row.FakeDataId)
        ////            ////.InnerJoin(row => row.FakeRelated, row => row.FakeDependencyEntity, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            ////.InnerJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            .Relations;

        ////    var expected = new EntityRelation(new PetaPocoDefinitionProvider(), EntityRelationType.LeftJoin);
        ////    expected.Join<FakeRaisedDataRow, FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId);

        ////    Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        ////}

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void LeftJoin_ExtendedRelationWithoutRelationAlias_MatchesExpected()
        {
            var relations = new SqlSelection<FakeFlatDataRow>()

                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId)
                .LeftJoin<FakeRelatedRow, FakeDependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId)

                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "OtherAlias")
                ////.InnerJoin<FakeRelatedRow, FakeDependencyRow>(
                ////    row => row.RelatedId,
                ////    "OtherAlias",
                ////    row => row.FakeComplexEntityId,
                ////    "RelatedDependency")
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "RelatedAlias")
                .Relations;

            var expected = new EntityRelation(EntityRelationType.LeftJoin);
            expected.Join<FakeRelatedRow, FakeDependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId);

            Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        }

        /////// <summary>
        /////// The inner join test.
        /////// </summary>
        ////[TestMethod]
        ////public void LeftJoin_WithMatchingSourceAndRelationProperties_MatchesExpected()
        ////{
        ////    var relations =
        ////        new SqlSelection<FakeRaisedDataRow>()
        ////            ////.InnerJoin(row => row.FakeRelated, row => row.FakeDataId, row => row.FakeDataId)
        ////            .LeftJoin(row => row.FakeRelated, row => row.FakeDependencyEntity, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            ////.InnerJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            .Relations;

        ////    var expected = new EntityRelation(new PetaPocoDefinitionProvider(), EntityRelationType.LeftJoin);
        ////    expected.Join<FakeRelatedRow, FakeDependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId);

        ////    Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        ////}

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void LeftJoin_WithRelationAlias_MatchesExpected()
        {
            var relations = new SqlSelection<FakeFlatDataRow>()

                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId)
                ////.InnerJoin<FakeRelatedRow, FakeDependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId)
                .LeftJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "OtherAlias")

                ////.InnerJoin<FakeRelatedRow, FakeDependencyRow>(
                ////    row => row.RelatedId,
                ////    "OtherAlias",
                ////    row => row.FakeComplexEntityId,
                ////    "RelatedDependency")
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "RelatedAlias")
                .Relations;

            var expected = new EntityRelation(EntityRelationType.LeftJoin);
            expected.Join<FakeFlatDataRow, FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, null, "OtherAlias");

            Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        }

        /////// <summary>
        /////// The inner join test.
        /////// </summary>
        ////[TestMethod]
        ////public void LeftJoin_WithRelationProperty_MatchesExpected()
        ////{
        ////    var relations =
        ////        new SqlSelection<FakeRaisedDataRow>()
        ////            ////.InnerJoin(row => row.FakeRelated, row => row.FakeDataId, row => row.FakeDataId)
        ////            ////.InnerJoin(row => row.FakeRelated, row => row.FakeDependencyEntity, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            .LeftJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            ////.InnerJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            .Relations;

        ////    var expected = new EntityRelation(new PetaPocoDefinitionProvider(), EntityRelationType.LeftJoin);
        ////    expected.Join<FakeRaisedDataRow, FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, null, "OtherAlias");

        ////    Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        ////}

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void LeftJoin_WithSourceAndRelationAlias_MatchesExpected()
        {
            var relations = new SqlSelection<FakeFlatDataRow>()

                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId)
                ////.InnerJoin<FakeRelatedRow, FakeDependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId)
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "OtherAlias")
                .LeftJoin<FakeRelatedRow, FakeDependencyRow>(row => row.RelatedId, "OtherAlias", row => row.FakeComplexEntityId, "RelatedDependency")

                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "RelatedAlias")
                .Relations;

            var expected = new EntityRelation(EntityRelationType.LeftJoin);
            expected.Join<FakeRelatedRow, FakeDependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId, "OtherAlias", "RelatedDependency");

            Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        }

        /////// <summary>
        /////// The inner join test.
        /////// </summary>
        ////[TestMethod]
        ////public void LeftJoin_WithSourceAndRelationProperty_MatchesExpected()
        ////{
        ////    var relations =
        ////        new SqlSelection<FakeRaisedDataRow>()
        ////            ////.InnerJoin(row => row.FakeRelated, row => row.FakeDataId, row => row.FakeDataId)
        ////            ////.InnerJoin(row => row.FakeRelated, row => row.FakeDependencyEntity, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            .LeftJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            .Relations;

        ////    var expected = new EntityRelation(new PetaPocoDefinitionProvider(), EntityRelationType.LeftJoin);
        ////    expected.Join<FakeRelatedRow, FakeDependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId, "OtherAlias", "RelatedDependency");

        ////    Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        ////}

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void LeftJoin_InferredWithMatchingRelationProperty_MatchesExpected()
        {
            var relations = new SqlSelection<FakeRaisedDataRow>()
                .LeftJoin(row => row.FakeDataId, row => row.FakeRelated.FakeDataId)

                ////.InnerJoin(row => row.FakeRelated, row => row.FakeDependencyEntity, row => row.RelatedId, row => row.FakeComplexEntityId)
                ////.InnerJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
                ////.InnerJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.FakeComplexEntityId)
                ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
                .Relations;

            var expected = new EntityRelation(EntityRelationType.LeftJoin);
            expected.Join<FakeRaisedDataRow, FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId);

            Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        }

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void LeftJoin_InferredWithMatchingSourceAndRelationProperties_MatchesExpected()
        {
            var relations = new SqlSelection<FakeRaisedDataRow>()

                ////.InnerJoin(row => row.FakeRelated, row => row.FakeDataId, row => row.FakeDataId)
                .LeftJoin(row => row.FakeRelated.RelatedId, row => row.FakeDependencyEntity.FakeComplexEntityId)

                ////.InnerJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
                ////.InnerJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.FakeComplexEntityId)
                ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
                .Relations;

            var expected = new EntityRelation(EntityRelationType.LeftJoin);
            expected.Join<FakeRelatedRow, FakeDependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId);

            Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        }

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void LeftJoin_InferredWithRelationProperty_MatchesExpected()
        {
            var relations = new SqlSelection<FakeRaisedDataRow>()

                ////.InnerJoin(row => row.FakeRelated, row => row.FakeDataId, row => row.FakeDataId)
                ////.InnerJoin(row => row.FakeRelated, row => row.FakeDependencyEntity, row => row.RelatedId, row => row.FakeComplexEntityId)
                .LeftJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)

                ////.InnerJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.FakeComplexEntityId)
                ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
                .Relations;

            var expected = new EntityRelation(EntityRelationType.LeftJoin);
            expected.Join<FakeRaisedDataRow, FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, null, "OtherAlias");

            Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        }

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void LeftJoin_InferredWithSourceAndRelationProperty_MatchesExpected()
        {
            var relations = new SqlSelection<FakeRaisedDataRow>()

                ////.InnerJoin(row => row.FakeRelated, row => row.FakeDataId, row => row.FakeDataId)
                ////.InnerJoin(row => row.FakeRelated, row => row.FakeDependencyEntity, row => row.RelatedId, row => row.FakeComplexEntityId)
                ////.InnerJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
                .LeftJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.FakeComplexEntityId)

                ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
                .Relations;

            var expected = new EntityRelation(EntityRelationType.LeftJoin);
            expected.Join<FakeRelatedRow, FakeDependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId, "OtherAlias", "RelatedDependency");

            Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        }

        /// <summary>
        /// The clear relations test.
        /// </summary>
        [TestMethod]
        public void ClearRelations_ItemSelectionWithRelations_RelationsAreCleared()
        {
            var selection = new SqlSelection<FakeRaisedDataRow>();
            Assert.IsNotNull(selection.Relations.FirstOrDefault());
            selection.ClearRelations();
            Assert.IsNull(selection.Relations.FirstOrDefault());
        }

        /// <summary>
        /// The select test.
        /// </summary>
        [TestMethod]
        public void Select_DataItemWithDistinctAttributeReferences_MatchesExpected()
        {
            var selection = new SqlSelection<FakeRaisedChildRow>();
            var entityDefinition = Singleton<PetaPocoDefinitionProvider>.Instance.Resolve<FakeRaisedChildRow>();
            var expected = entityDefinition.ReturnableAttributes.ToList();
            var actual = selection.SelectExpressions.Select(entityDefinition.Find).ToList();
            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// The select test.
        /// </summary>
        [TestMethod]
        public void Select_DataItemExplicitSelectionsWithDistinctAttributeReferences_MatchesExpected()
        {
            Expression<Func<FakeRaisedChildRow, object>> expr1 = row => row.FakeChildEntityId;
            Expression<Func<FakeRaisedChildRow, object>> expr2 = row => row.Name;
            Expression<Func<FakeRaisedChildRow, object>> expr3 = row => row.SomeValue;
            Expression<Func<FakeRaisedChildRow, object>> expr4 = row => row.FakeComplexEntity.Description;
            Expression<Func<FakeRaisedChildRow, object>> expr5 = row => row.FakeComplexEntity.FakeSubEntity.Description;
            Expression<Func<FakeRaisedChildRow, object>> expr6 = row => row.FakeComplexEntity.FakeSubEntity.FakeSubSubEntity.UniqueName;

            var expressions = new List<Expression<Func<FakeRaisedChildRow, object>>> { expr1, expr2, expr3, expr4, expr5, expr6 };

            var definitionProvider = Singleton<PetaPocoDefinitionProvider>.Instance;
            var selection = new SqlSelection<FakeRaisedChildRow>().Select(expressions.ToArray());

            var entityDefinition = definitionProvider.Resolve<FakeRaisedChildRow>();
            var expected = expressions.Select(entityDefinition.Find).ToList();

            var actual = selection.SelectExpressions.Select(entityDefinition.Find).ToList();
            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// The select test.
        /// </summary>
        [TestMethod]
        public void Select_DataItemWithDuplicateAttributeReferences_MatchesExpected()
        {
            var definitionProvider = Singleton<PetaPocoDefinitionProvider>.Instance;
            var selection = new SqlSelection<InstanceSection>();
            var entityDefinition = definitionProvider.Resolve<InstanceSection>();
            var expected = entityDefinition
                .ReturnableAttributes.Distinct(new DistinctAttributeEqualityComparer())
                .ToList();

            var actual = selection.SelectExpressions.Select(entityDefinition.Find).ToList();
            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// The select test.
        /// </summary>
        [TestMethod]
        public void Select_DataItemExplicitSelectionsWithDuplicateAttributeReferences_MatchesExpected()
        {
            Expression<Func<InstanceSection, object>> expr1 = row => row.InstanceId;
            Expression<Func<InstanceSection, object>> expr2 = row => row.OwnerId;
            Expression<Func<InstanceSection, object>> expr3 = row => row.Instance.TemplateVersion.Revision;

            // A duplicate of our previous expression.
            Expression<Func<InstanceSection, object>> expr4 = row => row.TemplateSection.TemplateVersion.Revision;

            Expression<Func<InstanceSection, object>> expr5 = row => row.InstanceExtension.Enabled;
            Expression<Func<InstanceSection, object>> expr6 = row => row.Instance.TemplateVersion.Template.Name;

            var expressions = new List<Expression<Func<InstanceSection, object>>> { expr1, expr2, expr3, expr4, expr5, expr6 };

            var selection = new SqlSelection<InstanceSection>().Select(expressions.ToArray());

            var entityDefinition = Singleton<PetaPocoDefinitionProvider>.Instance.Resolve<InstanceSection>();
            var expected = expressions.Select(entityDefinition.Find).Distinct(new DistinctAttributeEqualityComparer()).ToList();

            var actual = selection.SelectExpressions.Select(entityDefinition.Find).ToList();

            // Just to be sure, we check that our duplicate is eliminated.
            Assert.AreEqual(expressions.Count - 1, expected.Count);
            CollectionAssert.AreEqual(expected, actual);
        }


        /// <summary>
        /// The selection statement_ direct data_ matches expected.
        /// </summary>
        [TestMethod]
        public void SelectionStatement_DirectData_MatchesExpected()
        {
            var match = new FakeFlatDataRow { ValueColumn = 2, NullableColumn = "CouldHaveBeenNull", NullableValueColumn = null };
            var baseline = new FakeFlatDataRow { FakeDataId = 10, NormalColumn = "Greater" };
            var boundary = new FakeFlatDataRow { FakeDataId = 20, AnotherColumn = "Less" };
            var definitionProvider = new PetaPocoDefinitionProvider();
            var transactionSelection = new ItemSelection<FakeFlatDataRow>()
                .Matching(match, row => row.ValueColumn, row => row.NullableColumn, row => row.NullableValueColumn)
                .Select(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.NullableColumn,
                    row => row.NullableValueColumn,
                    row => row.ValueColumn,
                    row => row.AnotherColumn,
                    row => row.AnotherValueColumn)
                .Between(baseline, boundary, row => row.FakeDataId, row => row.NormalColumn, row => row.AnotherColumn)
                .Include(row => row.AnotherValueColumn, 5, 10, 15, 20);

            var expected = new object[]
                               {
                                   2,
                                   "CouldHaveBeenNull",
                                   10,
                                   20,
                                   baseline.NormalColumn,
                                   boundary.AnotherColumn,
                                   5,
                                   10,
                                   15,
                                   20
                               };
            var actual = transactionSelection.PropertyValues.ToArray();
            CollectionAssert.AreEqual(
                expected,
                actual,
                "Expected: {0}{1}Actual: {2}",
                string.Join(",", expected),
                Environment.NewLine,
                string.Join(",", actual));
        }

        /// <summary>
        /// The selection statement_ direct data_ matches expected.
        /// </summary>
        [TestMethod]
        public void SelectionStatement_DirectDataRaisedRow_MatchesExpected()
        {
            var match = new FakeRaisedDataRow { ValueColumn = 2, NullableColumn = "CouldHaveBeenNull", NullableValueColumn = null };
            var baseline = new FakeRaisedDataRow { FakeDataId = 10, NormalColumn = "Greater" };
            var boundary = new FakeRaisedDataRow { FakeDataId = 20, AnotherColumn = "Less" };
            var transactionSelection =
                match.ToExampleSelection(
                        row => row.ValueColumn,
                        row => row.NullableColumn,
                        row => row.NullableValueColumn)
                    .Select(
                        row => row.FakeDataId,
                        row => row.NormalColumn,
                        row => row.NullableColumn,
                        row => row.NullableValueColumn,
                        row => row.ValueColumn,
                        row => row.AnotherColumn,
                        row => row.AnotherValueColumn)
                    .Between(baseline, boundary, row => row.FakeDataId, row => row.NormalColumn, row => row.AnotherColumn)
                    .Include(row => row.AnotherValueColumn, 5, 10, 15, 20);

            var expected = new object[]
                               {
                                   2,
                                   "CouldHaveBeenNull",
                                   10,
                                   20,
                                   baseline.NormalColumn,
                                   boundary.AnotherColumn,
                                   5,
                                   10,
                                   15,
                                   20
                               };
            var actual = transactionSelection.PropertyValues.ToArray();
            CollectionAssert.AreEqual(
                expected,
                actual,
                "Expected: {0}{1}Actual: {2}",
                string.Join(",", expected),
                Environment.NewLine,
                string.Join(",", actual));
        }

        /// <summary>
        /// The selection statement_ related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void SelectionStatement_RelatedData_MatchesExpected()
        {
            var match = new FakeFlatDataRow
            {
                NullableValueColumn = null,
                RelatedAliasRelatedProperty = "Related",
                NullableColumn = "CouldHaveBeenNull",
                ValueColumn = 2
            };

            var baseline = new FakeFlatDataRow { FakeDataId = 10 };
            var boundary = new FakeFlatDataRow { FakeDataId = 20 };
            var definitionProvider = new PetaPocoDefinitionProvider();
            var transactionSelection =
                new ItemSelection<FakeFlatDataRow>().Matching(
                    match,
                    row => row.ValueColumn,
                    row => row.NullableColumn,
                    row => row.NullableValueColumn,
                    row => row.RelatedAliasRelatedProperty)
                    .Select(
                        row => row.FakeDataId,
                        row => row.NormalColumn,
                        row => row.FakeRelatedRelatedId,
                        row => row.FakeRelatedRelatedProperty,
                        row => row.RelatedId,
                        row => row.RelatedAliasRelatedProperty,
                        row => row.OtherAliasRelatedId,
                        row => row.OtherAliasRelatedProperty,
                        row => row.ParentFakeDataId)
                    .Between(baseline, boundary, row => row.FakeDataId);

            CollectionAssert.AreEqual(
                new object[] { 2, "CouldHaveBeenNull", "Related", 10, 20 },
                transactionSelection.PropertyValues.ToArray());
        }

        /// <summary>
        /// The selection statement_ related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void SelectionStatement_RelatedDataRaisedRow_MatchesExpected()
        {
            var match = new FakeRaisedDataRow
                            {
                                NullableValueColumn = null,
                                RelatedAlias = new FakeRelatedRow { RelatedProperty = "Related" },
                                NullableColumn = "CouldHaveBeenNull",
                                ValueColumn = 2
                            };

            var baseline = new FakeRaisedDataRow { FakeDataId = 10 };
            var boundary = new FakeRaisedDataRow { FakeDataId = 20 };
            var definitionProvider = new PetaPocoDefinitionProvider();
            var transactionSelection = new ItemSelection<FakeRaisedDataRow>()
                .Matching(match, row => row.ValueColumn, row => row.NullableColumn, row => row.NullableValueColumn)
                .WhereEqual(row => row.RelatedAlias.RelatedProperty, "Related").Select(
                    row => row.FakeDataId,
                    row => row.NormalColumn,
                    row => row.ParentFakeDataId,
                    row => row.FakeRelated.RelatedId,
                    row => row.FakeRelated.RelatedProperty,
                    row => row.RelatedAlias.RelatedId,
                    row => row.RelatedAlias.RelatedProperty,
                    row => row.OtherAlias.RelatedId,
                    row => row.OtherAlias.RelatedProperty).Between(baseline, boundary, row => row.FakeDataId);

            CollectionAssert.AreEqual(
                new object[] { 2, "CouldHaveBeenNull", "Related", 10, 20 },
                transactionSelection.PropertyValues.ToArray());
        }

        /// <summary>
        /// The selection statement_ union related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void SelectionStatement_UnionRelatedData_MatchesExpected()
        {
            var match1 = new FakeFlatDataRow
            {
                NullableValueColumn = null,
                RelatedAliasRelatedProperty = "Related1",
                NullableColumn = "CouldHaveBeenNull1",
                ValueColumn = 2
            };

            var baseline1 = new FakeFlatDataRow { FakeDataId = 10 };
            var boundary1 = new FakeFlatDataRow { FakeDataId = 20 };

            var match2 = new FakeFlatDataRow
            {
                NullableValueColumn = null,
                RelatedAliasRelatedProperty = "Related2",
                NullableColumn = "CouldHaveBeenNull2",
                ValueColumn = 3
            };

            var baseline2 = new FakeFlatDataRow { FakeDataId = 50 };
            var boundary2 = new FakeFlatDataRow { FakeDataId = 40 };

            var match3 = new FakeFlatDataRow
            {
                NullableValueColumn = null,
                RelatedAliasRelatedProperty = "Related3",
                NullableColumn = "CouldHaveBeenNull3",
                ValueColumn = 4
            };

            var baseline3 = new FakeFlatDataRow { FakeDataId = 60 };
            var boundary3 = new FakeFlatDataRow { FakeDataId = 70 };

            var definitionProvider = new PetaPocoDefinitionProvider();
            var transactionSelection =
                new ItemSelection<FakeFlatDataRow>()
                    .Matching(
                        match1,
                        row => row.ValueColumn,
                        row => row.NullableColumn,
                        row => row.NullableValueColumn,
                        row => row.RelatedAliasRelatedProperty)
                    .Select(
                        row => row.FakeDataId,
                        row => row.NormalColumn,
                        row => row.RelatedId,
                        row => row.RelatedAliasRelatedProperty,
                        row => row.OtherAliasRelatedProperty)
                    .Between(baseline1, boundary1, row => row.FakeDataId)
                    .Union(match2.ToExampleSelection(
                        row => row.ValueColumn,
                        row => row.NullableColumn,
                        row => row.NullableValueColumn,
                        row => row.RelatedAliasRelatedProperty)
                    .Select(
                        row => row.FakeDataId,
                        row => row.NormalColumn,
                        row => row.RelatedId,
                        row => row.RelatedAliasRelatedProperty,
                        row => row.OtherAliasRelatedProperty)
                    .Between(baseline2, boundary2, row => row.FakeDataId)
                    .Union(match3.ToExampleSelection(
                        row => row.ValueColumn,
                        row => row.NullableColumn,
                        row => row.NullableValueColumn,
                        row => row.RelatedAliasRelatedProperty)
                    .Select(
                        row => row.FakeDataId,
                        row => row.NormalColumn,
                        row => row.RelatedId,
                        row => row.RelatedAliasRelatedProperty,
                        row => row.OtherAliasRelatedProperty)
                    .Between(baseline3, boundary3, row => row.FakeDataId)));

            var expected = new object[]
                               {
                                   2, "CouldHaveBeenNull1", "Related1", 10, 20,
                                   3, "CouldHaveBeenNull2", "Related2", 40, 50,
                                   4, "CouldHaveBeenNull3", "Related3", 60, 70
                               };

            var actual = transactionSelection.PropertyValues.ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// The selection statement_ union related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void SelectionStatement_UnionRelatedDataRaisedRow_MatchesExpected()
        {
            var match1 = new FakeRaisedDataRow
            {
                NullableValueColumn = null,
                RelatedAlias = new FakeRelatedRow { RelatedProperty = "Related1" },
                NullableColumn = "CouldHaveBeenNull1",
                ValueColumn = 2
            };

            var baseline1 = new FakeRaisedDataRow { FakeDataId = 10 };
            var boundary1 = new FakeRaisedDataRow { FakeDataId = 20 };

            var match2 = new FakeRaisedDataRow
            {
                NullableValueColumn = null,
                RelatedAlias = new FakeRelatedRow { RelatedProperty = "Related2" },
                NullableColumn = "CouldHaveBeenNull2",
                ValueColumn = 3
            };

            var baseline2 = new FakeFlatDataRow { FakeDataId = 50 };
            var boundary2 = new FakeFlatDataRow { FakeDataId = 40 };

            var match3 = new FakeRaisedDataRow
            {
                NullableValueColumn = null,
                RelatedAlias = new FakeRelatedRow { RelatedProperty = "Related3" },
                NullableColumn = "CouldHaveBeenNull3",
                ValueColumn = 4
            };

            var baseline3 = new FakeRaisedDataRow { FakeDataId = 60 };
            var boundary3 = new FakeRaisedDataRow { FakeDataId = 70 };

            var definitionProvider = new PetaPocoDefinitionProvider();
            var transactionSelection =
                new ItemSelection<FakeRaisedDataRow>()
                    .Matching(match1, row => row.ValueColumn, row => row.NullableColumn, row => row.NullableValueColumn)
                    .WhereEqual(row => row.RelatedAlias.RelatedProperty, "Related1")
                    .Select(
                        row => row.FakeDataId,
                        row => row.NormalColumn,
                        row => row.RelatedAlias.RelatedId,
                        row => row.RelatedAlias.RelatedProperty,
                        row => row.OtherAlias.RelatedProperty)
                    .Between(baseline1, boundary1, row => row.FakeDataId)
                    .Union(
                        match2.ToExampleSelection(row => row.ValueColumn, row => row.NullableColumn, row => row.NullableValueColumn)
                            .WhereEqual(row => row.RelatedAlias.RelatedProperty, "Related2")
                            .Select(
                                row => row.FakeDataId,
                                row => row.NormalColumn,
                                row => row.RelatedAlias.RelatedId,
                                row => row.RelatedAlias.RelatedProperty,
                                row => row.OtherAlias.RelatedProperty)
                            .Between(baseline2, boundary2, row => row.FakeDataId)
                            .Union(
                                match3.ToExampleSelection(row => row.ValueColumn, row => row.NullableColumn, row => row.NullableValueColumn)
                                    .WhereEqual(row => row.RelatedAlias.RelatedProperty, "Related3")
                                    .Select(
                                        row => row.FakeDataId,
                                        row => row.NormalColumn,
                                        row => row.RelatedAlias.RelatedId,
                                        row => row.RelatedAlias.RelatedProperty,
                                        row => row.OtherAlias.RelatedProperty)
                                    .Between(baseline3, boundary3, row => row.FakeDataId)));

            var expected = new object[]
                               {
                                   2, "CouldHaveBeenNull1", "Related1", 10, 20,
                                   3, "CouldHaveBeenNull2", "Related2", 40, 50,
                                   4, "CouldHaveBeenNull3", "Related3", 60, 70
                               };

            var actual = transactionSelection.PropertyValues.ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}