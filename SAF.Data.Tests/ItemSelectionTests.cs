// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemSelectionTests.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SAF.Data.Providers;
    using SAF.Testing.Common;

    using Startitecture.Core;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Query;

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
            var relations = new TransactSqlSelection<FakeFlatDataRow>()
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

            var expected = new EntityRelation(new DataItemDefinitionProvider(), EntityRelationType.InnerJoin);

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
        ////        new TransactSqlSelection<FakeRaisedDataRow>()
        ////            .InnerJoin(row => row.FakeRelated, row => row.FakeDataId, row => row.FakeDataId)
        ////            ////.InnerJoin(row => row.FakeRelated, row => row.FakeDependencyEntity, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            ////.InnerJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            .Relations;

        ////    var expected = new EntityRelation(new DataItemDefinitionProvider(), EntityRelationType.InnerJoin);
        ////    expected.Join<FakeRaisedDataRow, FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId);

        ////    Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        ////}

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void InnerJoin_ExtendedRelationWithoutRelationAlias_MatchesExpected()
        {
            var relations = new TransactSqlSelection<FakeFlatDataRow>()
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

            var expected = new EntityRelation(new DataItemDefinitionProvider(), EntityRelationType.InnerJoin);
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
        ////        new TransactSqlSelection<FakeRaisedDataRow>()
        ////            ////.InnerJoin(row => row.FakeRelated, row => row.FakeDataId, row => row.FakeDataId)
        ////            .InnerJoin(row => row.FakeRelated, row => row.FakeDependencyEntity, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            ////.InnerJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            .Relations;

        ////    var expected = new EntityRelation(new DataItemDefinitionProvider(), EntityRelationType.InnerJoin);
        ////    expected.Join<FakeRelatedRow, FakeDependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId);

        ////    Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        ////}

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void InnerJoin_WithRelationAlias_MatchesExpected()
        {
            var relations = new TransactSqlSelection<FakeFlatDataRow>()
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

            var expected = new EntityRelation(new DataItemDefinitionProvider(), EntityRelationType.InnerJoin);
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
        ////        new TransactSqlSelection<FakeRaisedDataRow>()
        ////            ////.InnerJoin(row => row.FakeRelated, row => row.FakeDataId, row => row.FakeDataId)
        ////            ////.InnerJoin(row => row.FakeRelated, row => row.FakeDependencyEntity, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            .InnerJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            ////.InnerJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            .Relations;

        ////    var expected = new EntityRelation(new DataItemDefinitionProvider(), EntityRelationType.InnerJoin);
        ////    expected.Join<FakeRaisedDataRow, FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, null, "OtherAlias");

        ////    Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        ////}

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void InnerJoin_WithSourceAndRelationAlias_MatchesExpected()
        {
            var relations = new TransactSqlSelection<FakeFlatDataRow>()
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId)
                ////.InnerJoin<FakeRelatedRow, FakeDependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId)
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "OtherAlias")
                .InnerJoin<FakeRelatedRow, FakeDependencyRow>(
                    row => row.RelatedId,
                    "OtherAlias",
                    row => row.FakeComplexEntityId,
                    "RelatedDependency")
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "RelatedAlias")
                .Relations;

            var expected = new EntityRelation(new DataItemDefinitionProvider(), EntityRelationType.InnerJoin);
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
        ////        new TransactSqlSelection<FakeRaisedDataRow>()
        ////            ////.InnerJoin(row => row.FakeRelated, row => row.FakeDataId, row => row.FakeDataId)
        ////            ////.InnerJoin(row => row.FakeRelated, row => row.FakeDependencyEntity, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            .InnerJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            .Relations;

        ////    var expected = new EntityRelation(new DataItemDefinitionProvider(), EntityRelationType.InnerJoin);
        ////    expected.Join<FakeRelatedRow, FakeDependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId, "OtherAlias", "RelatedDependency");

        ////    Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        ////}

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void InnerJoin_InferredWithMatchingRelationProperty_MatchesExpected()
        {
            var relations =
                new TransactSqlSelection<FakeRaisedDataRow>()
                    .InnerJoin(row => row.FakeDataId, row => row.FakeRelated.FakeDataId)
                    ////.InnerJoin(row => row.FakeRelated, row => row.FakeDependencyEntity, row => row.RelatedId, row => row.FakeComplexEntityId)
                    ////.InnerJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
                    ////.InnerJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.FakeComplexEntityId)
                    ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
                    .Relations;

            var expected = new EntityRelation(new DataItemDefinitionProvider(), EntityRelationType.InnerJoin);
            expected.Join<FakeRaisedDataRow, FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId);

            Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        }

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void InnerJoin_InferredWithMatchingSourceAndRelationProperties_MatchesExpected()
        {
            var relations =
                new TransactSqlSelection<FakeRaisedDataRow>()
                    ////.InnerJoin(row => row.FakeRelated, row => row.FakeDataId, row => row.FakeDataId)
                    .InnerJoin(row => row.FakeRelated.RelatedId, row => row.FakeDependencyEntity.FakeComplexEntityId)
                    ////.InnerJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
                    ////.InnerJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.FakeComplexEntityId)
                    ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
                    .Relations;

            var expected = new EntityRelation(new DataItemDefinitionProvider(), EntityRelationType.InnerJoin);
            expected.Join<FakeRelatedRow, FakeDependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId);

            Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        }

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void InnerJoin_InferredWithRelationProperty_MatchesExpected()
        {
            var relations =
                new TransactSqlSelection<FakeRaisedDataRow>()
                    ////.InnerJoin(row => row.FakeRelated, row => row.FakeDataId, row => row.FakeDataId)
                    ////.InnerJoin(row => row.FakeRelated, row => row.FakeDependencyEntity, row => row.RelatedId, row => row.FakeComplexEntityId)
                    .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                    ////.InnerJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.FakeComplexEntityId)
                    ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
                    .Relations;

            var expected = new EntityRelation(new DataItemDefinitionProvider(), EntityRelationType.InnerJoin);
            expected.Join<FakeRaisedDataRow, FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, null, "OtherAlias");

            Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        }

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void InnerJoin_InferredWithSourceAndRelationProperty_MatchesExpected()
        {
            var relations =
                new TransactSqlSelection<FakeRaisedDataRow>()
                    ////.InnerJoin(row => row.FakeRelated, row => row.FakeDataId, row => row.FakeDataId)
                    ////.InnerJoin(row => row.FakeRelated, row => row.FakeDependencyEntity, row => row.RelatedId, row => row.FakeComplexEntityId)
                    ////.InnerJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
                    .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.FakeComplexEntityId)
                    ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
                    .Relations;

            var expected = new EntityRelation(new DataItemDefinitionProvider(), EntityRelationType.InnerJoin);
            expected.Join<FakeRelatedRow, FakeDependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId, "OtherAlias", "RelatedDependency");

            Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        }

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void LeftJoin_WithoutRelationAlias_MatchesExpected()
        {
            var relations = new TransactSqlSelection<FakeFlatDataRow>()
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

            var expected = new EntityRelation(new DataItemDefinitionProvider(), EntityRelationType.LeftJoin);
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
        ////        new TransactSqlSelection<FakeRaisedDataRow>()
        ////            .LeftJoin(row => row.FakeRelated, row => row.FakeDataId, row => row.FakeDataId)
        ////            ////.InnerJoin(row => row.FakeRelated, row => row.FakeDependencyEntity, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            ////.InnerJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            .Relations;

        ////    var expected = new EntityRelation(new DataItemDefinitionProvider(), EntityRelationType.LeftJoin);
        ////    expected.Join<FakeRaisedDataRow, FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId);

        ////    Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        ////}

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void LeftJoin_ExtendedRelationWithoutRelationAlias_MatchesExpected()
        {
            var relations = new TransactSqlSelection<FakeFlatDataRow>()
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

            var expected = new EntityRelation(new DataItemDefinitionProvider(), EntityRelationType.LeftJoin);
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
        ////        new TransactSqlSelection<FakeRaisedDataRow>()
        ////            ////.InnerJoin(row => row.FakeRelated, row => row.FakeDataId, row => row.FakeDataId)
        ////            .LeftJoin(row => row.FakeRelated, row => row.FakeDependencyEntity, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            ////.InnerJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            .Relations;

        ////    var expected = new EntityRelation(new DataItemDefinitionProvider(), EntityRelationType.LeftJoin);
        ////    expected.Join<FakeRelatedRow, FakeDependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId);

        ////    Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        ////}

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void LeftJoin_WithRelationAlias_MatchesExpected()
        {
            var relations = new TransactSqlSelection<FakeFlatDataRow>()
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

            var expected = new EntityRelation(new DataItemDefinitionProvider(), EntityRelationType.LeftJoin);
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
        ////        new TransactSqlSelection<FakeRaisedDataRow>()
        ////            ////.InnerJoin(row => row.FakeRelated, row => row.FakeDataId, row => row.FakeDataId)
        ////            ////.InnerJoin(row => row.FakeRelated, row => row.FakeDependencyEntity, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            .LeftJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            ////.InnerJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            .Relations;

        ////    var expected = new EntityRelation(new DataItemDefinitionProvider(), EntityRelationType.LeftJoin);
        ////    expected.Join<FakeRaisedDataRow, FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, null, "OtherAlias");

        ////    Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        ////}

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void LeftJoin_WithSourceAndRelationAlias_MatchesExpected()
        {
            var relations = new TransactSqlSelection<FakeFlatDataRow>()
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId)
                ////.InnerJoin<FakeRelatedRow, FakeDependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId)
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "OtherAlias")
                .LeftJoin<FakeRelatedRow, FakeDependencyRow>(
                    row => row.RelatedId,
                    "OtherAlias",
                    row => row.FakeComplexEntityId,
                    "RelatedDependency")
                ////.InnerJoin<FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, "RelatedAlias")
                .Relations;

            var expected = new EntityRelation(new DataItemDefinitionProvider(), EntityRelationType.LeftJoin);
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
        ////        new TransactSqlSelection<FakeRaisedDataRow>()
        ////            ////.InnerJoin(row => row.FakeRelated, row => row.FakeDataId, row => row.FakeDataId)
        ////            ////.InnerJoin(row => row.FakeRelated, row => row.FakeDependencyEntity, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            .LeftJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.FakeComplexEntityId)
        ////            ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
        ////            .Relations;

        ////    var expected = new EntityRelation(new DataItemDefinitionProvider(), EntityRelationType.LeftJoin);
        ////    expected.Join<FakeRelatedRow, FakeDependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId, "OtherAlias", "RelatedDependency");

        ////    Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        ////}

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void LeftJoin_InferredWithMatchingRelationProperty_MatchesExpected()
        {
            var relations =
                new TransactSqlSelection<FakeRaisedDataRow>()
                    .LeftJoin(row => row.FakeDataId, row => row.FakeRelated.FakeDataId)
                    ////.InnerJoin(row => row.FakeRelated, row => row.FakeDependencyEntity, row => row.RelatedId, row => row.FakeComplexEntityId)
                    ////.InnerJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
                    ////.InnerJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.FakeComplexEntityId)
                    ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
                    .Relations;

            var expected = new EntityRelation(new DataItemDefinitionProvider(), EntityRelationType.LeftJoin);
            expected.Join<FakeRaisedDataRow, FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId);

            Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        }

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void LeftJoin_InferredWithMatchingSourceAndRelationProperties_MatchesExpected()
        {
            var relations =
                new TransactSqlSelection<FakeRaisedDataRow>()
                    ////.InnerJoin(row => row.FakeRelated, row => row.FakeDataId, row => row.FakeDataId)
                    .LeftJoin(row => row.FakeRelated.RelatedId, row => row.FakeDependencyEntity.FakeComplexEntityId)
                    ////.InnerJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
                    ////.InnerJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.FakeComplexEntityId)
                    ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
                    .Relations;

            var expected = new EntityRelation(new DataItemDefinitionProvider(), EntityRelationType.LeftJoin);
            expected.Join<FakeRelatedRow, FakeDependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId);

            Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        }

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void LeftJoin_InferredWithRelationProperty_MatchesExpected()
        {
            var relations =
                new TransactSqlSelection<FakeRaisedDataRow>()
                    ////.InnerJoin(row => row.FakeRelated, row => row.FakeDataId, row => row.FakeDataId)
                    ////.InnerJoin(row => row.FakeRelated, row => row.FakeDependencyEntity, row => row.RelatedId, row => row.FakeComplexEntityId)
                    .LeftJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                    ////.InnerJoin(row => row.OtherAlias, row => row.RelatedDependency, row => row.RelatedId, row => row.FakeComplexEntityId)
                    ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
                    .Relations;

            var expected = new EntityRelation(new DataItemDefinitionProvider(), EntityRelationType.LeftJoin);
            expected.Join<FakeRaisedDataRow, FakeRelatedRow>(row => row.FakeDataId, row => row.FakeDataId, null, "OtherAlias");

            Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        }

        /// <summary>
        /// The inner join test.
        /// </summary>
        [TestMethod]
        public void LeftJoin_InferredWithSourceAndRelationProperty_MatchesExpected()
        {
            var relations =
                new TransactSqlSelection<FakeRaisedDataRow>()
                    ////.InnerJoin(row => row.FakeRelated, row => row.FakeDataId, row => row.FakeDataId)
                    ////.InnerJoin(row => row.FakeRelated, row => row.FakeDependencyEntity, row => row.RelatedId, row => row.FakeComplexEntityId)
                    ////.InnerJoin(row => row.OtherAlias, row => row.FakeDataId, row => row.FakeDataId)
                    .LeftJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.FakeComplexEntityId)
                    ////.InnerJoin(row => row.RelatedAlias, row => row.FakeDataId, row => row.FakeDataId)
                    .Relations;

            var expected = new EntityRelation(new DataItemDefinitionProvider(), EntityRelationType.LeftJoin);
            expected.Join<FakeRelatedRow, FakeDependencyRow>(row => row.RelatedId, row => row.FakeComplexEntityId, "OtherAlias", "RelatedDependency");

            Assert.IsNotNull(relations.FirstOrDefault(x => expected == (EntityRelation)x));
        }

        /// <summary>
        /// The clear relations test.
        /// </summary>
        [TestMethod]
        public void ClearRelations_ItemSelectionWithRelations_RelationsAreCleared()
        {
            var selection = new TransactSqlSelection<FakeRaisedDataRow>();
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
            var selection = new TransactSqlSelection<FakeRaisedChildRow>();
            var expected = Singleton<DataItemDefinitionProvider>.Instance.Resolve<FakeRaisedChildRow>().ReturnableAttributes.ToList();
            var actual = selection.PropertiesToReturn.ToList();
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

            var selection = new TransactSqlSelection<FakeRaisedChildRow>().Select(expressions.ToArray());

            var entityDefinition = Singleton<DataItemDefinitionProvider>.Instance.Resolve<FakeRaisedChildRow>();
            var expected = expressions.Select(entityDefinition.Find).ToList();

            var actual = selection.PropertiesToReturn.ToList();
            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// The select test.
        /// </summary>
        [TestMethod]
        public void Select_DataItemWithDuplicateAttributeReferences_MatchesExpected()
        {
            var selection = new TransactSqlSelection<InstanceSection>();
            var expected =
                Singleton<DataItemDefinitionProvider>.Instance.Resolve<InstanceSection>()
                    .ReturnableAttributes.Distinct(new DistinctAttributeEqualityComparer())
                    .ToList();

            var actual = selection.PropertiesToReturn.ToList();
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

            var selection = new TransactSqlSelection<InstanceSection>().Select(expressions.ToArray());

            var entityDefinition = Singleton<DataItemDefinitionProvider>.Instance.Resolve<InstanceSection>();
            var expected = expressions.Select(entityDefinition.Find).Distinct(new DistinctAttributeEqualityComparer()).ToList();

            var actual = selection.PropertiesToReturn.ToList();

            // Just to be sure, we check that our duplicate is eliminated.
            Assert.AreEqual(expressions.Count - 1, expected.Count);
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}