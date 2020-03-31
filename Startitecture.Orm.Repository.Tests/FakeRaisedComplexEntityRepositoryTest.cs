// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeRaisedComplexEntityRepositoryTest.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository.Tests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Orm.Common;
    using Startitecture.Orm.Testing.Entities;
    using Startitecture.Orm.Testing.Model;
    using Startitecture.Orm.Testing.RhinoMocks;

    /// <summary>
    /// The fake raised complex entity repository test.
    /// </summary>
    [TestClass]
    public class FakeRaisedComplexEntityRepositoryTest
    {
        /// <summary>
        /// The entity mapper.
        /// </summary>
        private readonly IEntityMapper entityMapper = RepositoryMockFactory.CreateEntityMapper(
            expression =>
            {
                expression.AddProfile<FakeDependentEntityMappingProfile>();
                expression.AddProfile<FakeCreatedByMappingProfile>();
                expression.AddProfile<FakeModifiedByMappingProfile>();
                expression.AddProfile<FakeSubEntityMappingProfile>();
                expression.AddProfile<FakeSubSubEntityMappingProfile>();
                expression.AddProfile<FakeComplexEntityMappingProfile>();
                expression.AddProfile<FakeChildEntityMappingProfile>();
            });

        /// <summary>
        /// The first or default_ fake raised complex entity_ matches expected.
        /// </summary>
        [TestMethod]
        public void FirstOrDefault_FakeRaisedComplexEntityWithoutDependentEntity_MatchesExpected()
        {
            var fakeSubSubEntity = new SubSubEntity("jasdyri", 5848);
            var fakeSubEntity = new SubEntity("asidf", 58, fakeSubSubEntity, 87543);
            var fakeCreatedBy = new CreatedBy("lasjdas", 4975398) { Description = "aslfdjasjkld" };
            var fakeModifiedBy = new ModifiedBy("adskljs", 58974) { Description = ",znxckas" };

            var expected = new ComplexEntity("lajsf", fakeSubEntity, FakeEnumeration.FirstFake, fakeCreatedBy, DateTimeOffset.Now, 47958)
                               {
                                   ModifiedBy = fakeModifiedBy,
                                   ModifiedTime = DateTimeOffset.Now,
                                   FakeOtherEnumeration = FakeOtherEnumeration.OtherFakeFirst,
                                   Description = "asjkla eh"
                               };

            var repositoryAdapter = RepositoryMockFactory.CreateAdapter();
            repositoryAdapter.StubForExistingItem<ComplexRaisedRow>(expected, this.entityMapper);

            using (var repositoryProvider = RepositoryMockFactory.CreateConcreteProvider<FakeDataContext>(this.entityMapper, repositoryAdapter))
            {
                var target = new EntityRepository<ComplexEntity, ComplexRaisedRow>(repositoryProvider, this.entityMapper);
                var actual = target.FirstOrDefault(expected.FakeComplexEntityId.GetValueOrDefault());

                Assert.AreEqual(expected.FakeComplexEntityId, actual.FakeComplexEntityId);
                Assert.AreEqual(expected.CreatedByFakeMultiReferenceEntityId, actual.CreatedByFakeMultiReferenceEntityId);
                Assert.AreEqual(expected.ModifiedByFakeMultiReferenceEntityId, actual.ModifiedByFakeMultiReferenceEntityId);
                Assert.AreEqual(expected.FakeSubEntityId, actual.FakeSubEntityId);
                Assert.AreEqual(expected.FakeSubSubEntityId, actual.FakeSubSubEntityId);
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        /// The first or default_ fake raised complex entity_ matches expected.
        /// </summary>
        [TestMethod]
        public void FirstOrDefault_FakeRaisedComplexEntityWithDependentEntity_MatchesExpected()
        {
            var fakeSubSubEntity = new SubSubEntity("jasdyri", 5848);
            var fakeSubEntity = new SubEntity("asidf", 58, fakeSubSubEntity, 87543);
            var fakeCreatedBy = new CreatedBy("lasjdas", 4975398) { Description = "aslfdjasjkld" };
            var fakeModifiedBy = new ModifiedBy("adskljs", 58974) { Description = ",znxckas" };

            var expected = new ComplexEntity("lajsf", fakeSubEntity, FakeEnumeration.FirstFake, fakeCreatedBy, DateTimeOffset.Now, 47958)
                               {
                                   ModifiedBy = fakeModifiedBy,
                                   ModifiedTime = DateTimeOffset.Now,
                                   FakeOtherEnumeration = FakeOtherEnumeration.OtherFakeFirst,
                                   Description = "asjkla eh"
                               };

            expected.SetDependentEntity(985, DateTimeOffset.Now);

            var repositoryAdapter = RepositoryMockFactory.CreateAdapter();
            repositoryAdapter.StubForExistingItem<ComplexRaisedRow>(expected, this.entityMapper);

            using (var repositoryProvider = RepositoryMockFactory.CreateConcreteProvider<FakeDataContext>(this.entityMapper, repositoryAdapter))
            {
                var target = new EntityRepository<ComplexEntity, ComplexRaisedRow>(repositoryProvider, this.entityMapper);
                var actual = target.FirstOrDefault(expected.FakeComplexEntityId.GetValueOrDefault());

                Assert.AreEqual(expected.FakeComplexEntityId, actual.FakeComplexEntityId);
                Assert.AreEqual(expected.CreatedByFakeMultiReferenceEntityId, actual.CreatedByFakeMultiReferenceEntityId);
                Assert.AreEqual(expected.ModifiedByFakeMultiReferenceEntityId, actual.ModifiedByFakeMultiReferenceEntityId);
                Assert.AreEqual(expected.FakeDependentEntityId, actual.FakeDependentEntityId);
                Assert.AreEqual(expected.FakeSubEntityId, actual.FakeSubEntityId);
                Assert.AreEqual(expected.FakeSubSubEntityId, actual.FakeSubSubEntityId);
                Assert.AreEqual(expected, actual);
            }
        }

        /////// <summary>
        /////// The first or default_ fake raised complex entity_ matches expected.
        /////// </summary>
        ////[TestMethod]
        ////public void FirstOrDefaultWithChildren_FakeRaisedComplexEntity_MatchesExpected()
        ////{
        ////    var subSubEntity = new SubSubEntity("jasdyri", 5848);
        ////    var subEntity = new SubEntity("asidf", 58, subSubEntity, 87543);
        ////    var fakeCreatedBy = new CreatedBy("lasjdas", 4975398) { Description = "aslfdjasjkld" };
        ////    var fakeModifiedBy = new ModifiedBy("adskljs", 58974) { Description = ",znxckas" };

        ////    var expected = new ComplexEntity("lajsf", subEntity, FakeEnumeration.FirstFake, fakeCreatedBy, DateTimeOffset.Now, 47958)
        ////                       {
        ////                           ModifiedBy = fakeModifiedBy,
        ////                           ModifiedTime = DateTimeOffset.Now,
        ////                           FakeOtherEnumeration = FakeOtherEnumeration.OtherFakeFirst,
        ////                           Description = "asjkla eh"
        ////                       };

        ////    expected.SetDependentEntity(985, DateTimeOffset.Now);

        ////    var parentChildEntity = new ChildEntity(expected, 543879) { Name = "Parent", SomeValue = 243 };
        ////    var childEntity1 = new ChildEntity(expected, 978234) { Name = "Child1", SomeValue = 48597, Parent = parentChildEntity };
        ////    var childEntity2 = new ChildEntity(expected, 3275498) { Name = "Child2", SomeValue = 345453, Parent = parentChildEntity };

        ////    var childList = new List<ChildEntity> { parentChildEntity, childEntity1, childEntity2 };
        ////    expected.Load(childList);

        ////    var repositoryAdapter = RepositoryMockFactory.CreateAdapter();
        ////    repositoryAdapter.StubForExistingItem<ComplexRaisedRow>(expected, this.entityMapper);
        ////    repositoryAdapter.StubForList<ChildEntity, ChildRaisedRow>(childList, this.entityMapper, new DataAnnotationsDefinitionProvider());

        ////    using (var repositoryProvider = RepositoryMockFactory.CreateConcreteProvider<FakeDataContext>(this.entityMapper, repositoryAdapter))
        ////    {
        ////        var target = new FakeRaisedComplexEntityRepository(repositoryProvider, this.entityMapper);
        ////        var actual = target.FirstOrDefaultWithChildren(expected.FakeComplexEntityId.GetValueOrDefault());

        ////        Assert.AreEqual(expected.FakeComplexEntityId, actual.FakeComplexEntityId);
        ////        Assert.AreEqual(expected.CreatedByFakeMultiReferenceEntityId, actual.CreatedByFakeMultiReferenceEntityId);
        ////        Assert.AreEqual(expected.ModifiedByFakeMultiReferenceEntityId, actual.ModifiedByFakeMultiReferenceEntityId);
        ////        Assert.AreEqual(expected.FakeDependentEntityId, actual.FakeDependentEntityId);
        ////        Assert.AreEqual(expected.FakeSubEntityId, actual.FakeSubEntityId);
        ////        Assert.AreEqual(expected.FakeSubSubEntityId, actual.FakeSubSubEntityId);
        ////        Assert.AreEqual(expected, actual);

        ////        string KeySelector(ChildEntity entity) => string.Concat(entity.FakeComplexEntityId, '.', entity.SomeValue);

        ////        var expectedChildren = expected.ChildEntities.OrderBy(KeySelector);
        ////        var actualChildren = actual.ChildEntities.OrderBy(KeySelector);

        ////        var firstExpected = expectedChildren.ElementAtOrDefault(1);
        ////        var firstActual = actualChildren.ElementAtOrDefault(1);

        ////        Assert.IsNotNull(firstExpected);
        ////        Assert.IsNotNull(firstActual);

        ////        Assert.AreEqual(
        ////            firstExpected,
        ////            firstActual,
        ////            string.Join(Environment.NewLine, firstExpected.GetDifferences(firstActual)));

        ////        CollectionAssert.AreEqual(expectedChildren.ToList(), actualChildren.ToList());

        ////        using (var expectedEnumerator = expectedChildren.GetEnumerator())
        ////        using (var actualEnumerator = actualChildren.GetEnumerator())
        ////        {
        ////            while (expectedEnumerator.MoveNext())
        ////            {
        ////                actualEnumerator.MoveNext();

        ////                var expectedEntity = expectedEnumerator.Current;
        ////                var actualEntity = actualEnumerator.Current;

        ////                Assert.AreEqual(
        ////                    expectedEntity,
        ////                    actualEntity,
        ////                    string.Join(Environment.NewLine, expectedEntity.GetDifferences(actualEntity)));

        ////                Assert.AreSame(actual, actualEntity.ComplexEntity);
        ////                Assert.AreEqual(expectedEntity.FakeChildEntityId, actualEntity.FakeChildEntityId);
        ////            }
        ////        }
        ////    }
        ////}

        /// <summary>
        /// Tests that the first or default of the repository matches the expected results.
        /// </summary>
        [TestMethod]
        public void Save_NewFakeRaisedComplexEntityWithoutDependentEntity_MatchesExpected()
        {
            var fakeSubSubEntity = new SubSubEntity("jasdyri");
            var fakeSubEntity = new SubEntity("asidf", 58, fakeSubSubEntity);
            var fakeCreatedBy = new CreatedBy("lasjdas") { Description = "aslfdjasjkld" };
            var fakeModifiedBy = new ModifiedBy("adskljs") { Description = ",znxckas" };

            var expected = new ComplexEntity("lajsf", fakeSubEntity, FakeEnumeration.FirstFake, fakeCreatedBy)
                               {
                                   ModifiedBy = fakeModifiedBy,
                                   ModifiedTime = DateTimeOffset.Now,
                                   FakeOtherEnumeration = FakeOtherEnumeration.OtherFakeFirst,
                                   Description = "asjkla eh"
                               };

            var repositoryAdapter = RepositoryMockFactory.CreateAdapter();
            using (var repositoryProvider = RepositoryMockFactory.CreateConcreteProvider<FakeDataContext>(this.entityMapper, repositoryAdapter))
            {
                repositoryAdapter.StubForNewItem<SubRow>(repositoryProvider.EntityDefinitionProvider);
                repositoryAdapter.StubForNewItem<SubSubRow>(repositoryProvider.EntityDefinitionProvider);
                repositoryAdapter.StubForNewItem<ComplexRaisedRow>(repositoryProvider.EntityDefinitionProvider);
                repositoryAdapter.StubForNewItem<MultiReferenceRow>(repositoryProvider.EntityDefinitionProvider);

                var target = new EntityRepository<ComplexEntity, ComplexRaisedRow>(repositoryProvider, this.entityMapper);
                var actual = target.Save(expected);

                Assert.IsTrue(actual.FakeComplexEntityId > 0);
                Assert.IsTrue(actual.CreatedByFakeMultiReferenceEntityId > 0);
                Assert.IsTrue(actual.ModifiedByFakeMultiReferenceEntityId > 0);
                Assert.IsTrue(actual.FakeSubEntityId > 0);
                Assert.IsTrue(actual.FakeSubSubEntityId > 0);
                Assert.AreSame(expected, actual);
            }
        }

        /// <summary>
        /// Tests that the first or default of the repository matches the expected results.
        /// </summary>
        [TestMethod]
        public void Save_NewFakeRaisedComplexEntityWithDependentEntity_MatchesExpected()
        {
            var fakeSubSubEntity = new SubSubEntity("jasdyri");
            var fakeSubEntity = new SubEntity("asidf", 58, fakeSubSubEntity);
            var fakeCreatedBy = new CreatedBy("lasjdas") { Description = "aslfdjasjkld" };
            var fakeModifiedBy = new ModifiedBy("adskljs") { Description = ",znxckas" };

            var expected = new ComplexEntity("lajsf", fakeSubEntity, FakeEnumeration.FirstFake, fakeCreatedBy)
                               {
                                   ModifiedBy = fakeModifiedBy,
                                   ModifiedTime = DateTimeOffset.Now,
                                   FakeOtherEnumeration = FakeOtherEnumeration.OtherFakeFirst,
                                   Description = "asjkla eh"
                               };

            expected.SetDependentEntity(985, DateTimeOffset.Now);

            var repositoryAdapter = RepositoryMockFactory.CreateAdapter();
            using (var repositoryProvider = RepositoryMockFactory.CreateConcreteProvider<FakeDataContext>(this.entityMapper, repositoryAdapter))
            {
                repositoryAdapter.StubForNewItem<SubRow>(repositoryProvider.EntityDefinitionProvider);
                repositoryAdapter.StubForNewItem<SubSubRow>(repositoryProvider.EntityDefinitionProvider);
                repositoryAdapter.StubForNewItem<ComplexRaisedRow>(repositoryProvider.EntityDefinitionProvider);
                repositoryAdapter.StubForNewItem<MultiReferenceRow>(repositoryProvider.EntityDefinitionProvider);
                repositoryAdapter.StubForNewItem<DependentRow>(repositoryProvider.EntityDefinitionProvider);

                var target = new EntityRepository<ComplexEntity, ComplexRaisedRow>(repositoryProvider, this.entityMapper);
                var actual = target.Save(expected);

                Assert.IsTrue(actual.FakeComplexEntityId > 0);
                Assert.IsTrue(actual.CreatedByFakeMultiReferenceEntityId > 0);
                Assert.IsTrue(actual.ModifiedByFakeMultiReferenceEntityId > 0);
                Assert.IsTrue(actual.FakeSubEntityId > 0);
                Assert.IsTrue(actual.FakeSubSubEntityId > 0);
                Assert.IsTrue(actual.FakeDependentEntityId > 0);
                Assert.AreSame(expected, actual);
            }
        }

        /// <summary>
        /// Tests that the first or default of the repository matches the expected results.
        /// </summary>
        [TestMethod]
        public void Save_ExistingFakeRaisedSubEntityWithoutDependentEntity_MatchesExpected()
        {
            var fakeSubSubEntity = new SubSubEntity("jasdyri", 5848);
            var fakeSubEntity = new SubEntity("asidf", 58, fakeSubSubEntity, 87543);
            var fakeCreatedBy = new CreatedBy("lasjdas", 4975398) { Description = "aslfdjasjkld" };
            var fakeModifiedBy = new ModifiedBy("adskljs", 58974) { Description = ",znxckas" };

            var expected = new ComplexEntity("lajsf", fakeSubEntity, FakeEnumeration.FirstFake, fakeCreatedBy, DateTimeOffset.Now, 47958)
                               {
                                   ModifiedBy = fakeModifiedBy,
                                   ModifiedTime = DateTimeOffset.Now,
                                   FakeOtherEnumeration = FakeOtherEnumeration.OtherFakeFirst,
                                   Description = "asjkla eh"
                               };

            var repositoryAdapter = RepositoryMockFactory.CreateAdapter();
            repositoryAdapter.StubForExistingItem<SubRow>(fakeSubEntity, this.entityMapper);
            repositoryAdapter.StubForExistingItem<SubSubRow>(fakeSubSubEntity, this.entityMapper);

            // Stub for the created by item.
            repositoryAdapter.StubForExistingItem<MultiReferenceRow, int>(
                fakeCreatedBy,
                this.entityMapper,
                row => row.FakeMultiReferenceEntityId,
                fakeCreatedBy.FakeMultiReferenceEntityId.GetValueOrDefault());

            // Stub for the modified by item.
            repositoryAdapter.StubForExistingItem<MultiReferenceRow, int>(
                fakeModifiedBy,
                this.entityMapper,
                row => row.FakeMultiReferenceEntityId,
                fakeModifiedBy.FakeMultiReferenceEntityId.GetValueOrDefault());

            repositoryAdapter.StubForExistingItem<ComplexRaisedRow>(expected, this.entityMapper);

            using (var repositoryProvider = RepositoryMockFactory.CreateConcreteProvider<FakeDataContext>(this.entityMapper, repositoryAdapter))
            {
                var target = new EntityRepository<ComplexEntity, ComplexRaisedRow>(repositoryProvider, this.entityMapper);
                var actual = target.Save(expected);

                Assert.AreEqual(47958, actual.FakeComplexEntityId);
                Assert.AreEqual(4975398, actual.CreatedByFakeMultiReferenceEntityId);
                Assert.AreEqual(58974, actual.ModifiedByFakeMultiReferenceEntityId);
                Assert.AreEqual(87543, actual.FakeSubEntityId);
                Assert.AreEqual(5848, actual.FakeSubSubEntityId);
                Assert.AreSame(expected, actual);
            }
        }

        /// <summary>
        /// Tests that the first or default of the repository matches the expected results.
        /// </summary>
        [TestMethod]
        public void Save_ExistingFakeRaisedSubEntityWithDependentEntity_MatchesExpected()
        {
            var fakeSubSubEntity = new SubSubEntity("jasdyri", 5848);
            var fakeSubEntity = new SubEntity("asidf", 58, fakeSubSubEntity, 87543);
            var fakeCreatedBy = new CreatedBy("lasjdas", 4975398) { Description = "aslfdjasjkld" };
            var fakeModifiedBy = new ModifiedBy("adskljs", 58974) { Description = ",znxckas" };

            var expected = new ComplexEntity("lajsf", fakeSubEntity, FakeEnumeration.FirstFake, fakeCreatedBy, DateTimeOffset.Now, 47958)
                               {
                                   ModifiedBy = fakeModifiedBy,
                                   ModifiedTime = DateTimeOffset.Now,
                                   FakeOtherEnumeration = FakeOtherEnumeration.OtherFakeFirst,
                                   Description = "asjkla eh"
                               };

            var fakeDependentEntity = expected.SetDependentEntity(985, DateTimeOffset.Now);

            var repositoryAdapter = RepositoryMockFactory.CreateAdapter();
            repositoryAdapter.StubForExistingItem<SubRow>(fakeSubEntity, this.entityMapper);
            repositoryAdapter.StubForExistingItem<SubSubRow>(fakeSubSubEntity, this.entityMapper);

            // Stub for the created by item.
            repositoryAdapter.StubForExistingItem<MultiReferenceRow, int>(
                fakeCreatedBy,
                this.entityMapper,
                row => row.FakeMultiReferenceEntityId,
                fakeCreatedBy.FakeMultiReferenceEntityId.GetValueOrDefault());

            // Stub for the modified by item.
            repositoryAdapter.StubForExistingItem<MultiReferenceRow, int>(
                fakeModifiedBy,
                this.entityMapper,
                row => row.FakeMultiReferenceEntityId,
                fakeModifiedBy.FakeMultiReferenceEntityId.GetValueOrDefault());

            repositoryAdapter.StubForExistingItem<DependentRow>(fakeDependentEntity, this.entityMapper);
            repositoryAdapter.StubForExistingItem<ComplexRaisedRow>(expected, this.entityMapper);

            using (var repositoryProvider = RepositoryMockFactory.CreateConcreteProvider<FakeDataContext>(this.entityMapper, repositoryAdapter))
            {
                var target = new EntityRepository<ComplexEntity, ComplexRaisedRow>(repositoryProvider, this.entityMapper);
                var actual = target.Save(expected);

                Assert.AreEqual(47958, actual.FakeComplexEntityId);
                Assert.AreEqual(4975398, actual.CreatedByFakeMultiReferenceEntityId);
                Assert.AreEqual(58974, actual.ModifiedByFakeMultiReferenceEntityId);
                Assert.AreEqual(87543, actual.FakeSubEntityId);
                Assert.AreEqual(5848, actual.FakeSubSubEntityId);
                Assert.AreEqual(47958, actual.FakeDependentEntityId);
                Assert.AreSame(expected, actual);
            }
        }

        /////// <summary>
        /////// Tests that the first or default of the repository matches the expected results.
        /////// </summary>
        ////[TestMethod]
        ////public void SaveWithChildren_ExistingFakeRaisedSubEntityWithDependentEntity_MatchesExpected()
        ////{
        ////    var subSubEntity = new SubSubEntity("jasdyri", 5848);
        ////    var subEntity = new SubEntity("asidf", 58, subSubEntity, 87543);
        ////    var fakeCreatedBy = new CreatedBy("lasjdas", 4975398) { Description = "aslfdjasjkld" };
        ////    var fakeModifiedBy = new ModifiedBy("adskljs", 58974) { Description = ",znxckas" };

        ////    var expected = new ComplexEntity("lajsf", subEntity, FakeEnumeration.FirstFake, fakeCreatedBy, DateTimeOffset.Now, 47958)
        ////                       {
        ////                           ModifiedBy = fakeModifiedBy,
        ////                           ModifiedTime = DateTimeOffset.Now,
        ////                           FakeOtherEnumeration = FakeOtherEnumeration.OtherFakeFirst,
        ////                           Description = "asjkla eh"
        ////                       };

        ////    var fakeDependentEntity = expected.SetDependentEntity(985, DateTimeOffset.Now);

        ////    var parentChildEntity = new ChildEntity(expected, 543879) { Name = "Parent", SomeValue = 243 };
        ////    var childEntity1 = new ChildEntity(expected, 978234) { Name = "Child1", SomeValue = 48597, Parent = parentChildEntity };
        ////    var childEntity2 = new ChildEntity(expected, 3275498) { Name = "Child2", SomeValue = 345453, Parent = parentChildEntity };

        ////    var childList = new List<ChildEntity> { parentChildEntity, childEntity1, childEntity2 };
        ////    expected.Load(childList);

        ////    var repositoryAdapter = RepositoryMockFactory.CreateAdapter();
        ////    repositoryAdapter.StubForExistingItem<SubRow>(subEntity, this.entityMapper);
        ////    repositoryAdapter.StubForExistingItem<SubSubRow>(subSubEntity, this.entityMapper);

        ////    // Stub for the created by item.
        ////    repositoryAdapter.StubForExistingItem<MultiReferenceRow, int>(
        ////        fakeCreatedBy,
        ////        this.entityMapper,
        ////        row => row.FakeMultiReferenceEntityId,
        ////        fakeCreatedBy.FakeMultiReferenceEntityId.GetValueOrDefault());

        ////    // Stub for the modified by item.
        ////    repositoryAdapter.StubForExistingItem<MultiReferenceRow, int>(
        ////        fakeModifiedBy,
        ////        this.entityMapper,
        ////        row => row.FakeMultiReferenceEntityId,
        ////        fakeModifiedBy.FakeMultiReferenceEntityId.GetValueOrDefault());

        ////    repositoryAdapter.StubForExistingItem<DependentRow>(fakeDependentEntity, this.entityMapper);
        ////    repositoryAdapter.StubForExistingItem<ComplexRaisedRow>(expected, this.entityMapper);
        ////    repositoryAdapter.StubForExistingItem<ChildRaisedRow, int>(
        ////        parentChildEntity,
        ////        this.entityMapper,
        ////        row => row.FakeChildEntityId,
        ////        parentChildEntity.FakeChildEntityId.GetValueOrDefault());

        ////    repositoryAdapter.StubForExistingItem<ChildRaisedRow, int>(
        ////        childEntity1,
        ////        this.entityMapper,
        ////        row => row.FakeChildEntityId,
        ////        childEntity1.FakeChildEntityId.GetValueOrDefault());

        ////    repositoryAdapter.StubForExistingItem<ChildRaisedRow, int>(
        ////        childEntity2,
        ////        this.entityMapper,
        ////        row => row.FakeChildEntityId,
        ////        childEntity2.FakeChildEntityId.GetValueOrDefault());

        ////    using (var repositoryProvider = RepositoryMockFactory.CreateConcreteProvider<FakeDataContext>(this.entityMapper, repositoryAdapter))
        ////    {
        ////        var target = new FakeRaisedComplexEntityRepository(repositoryProvider, this.entityMapper);
        ////        var actual = target.SaveWithChildren(expected);

        ////        Assert.AreEqual(47958, actual.FakeComplexEntityId);
        ////        Assert.AreEqual(4975398, actual.CreatedByFakeMultiReferenceEntityId);
        ////        Assert.AreEqual(58974, actual.ModifiedByFakeMultiReferenceEntityId);
        ////        Assert.AreEqual(87543, actual.FakeSubEntityId);
        ////        Assert.AreEqual(5848, actual.FakeSubSubEntityId);
        ////        Assert.AreEqual(47958, actual.FakeDependentEntityId);
        ////        Assert.AreSame(expected, actual);

        ////        foreach (var actualChildEntity in actual.ChildEntities)
        ////        {
        ////            Assert.IsTrue(actualChildEntity.FakeChildEntityId.HasValue);
        ////            Assert.IsTrue(actualChildEntity.FakeChildEntityId > 0);
        ////        }
        ////    }
        ////}

        /////// <summary>
        /////// Tests that the first or default of the repository matches the expected results.
        /////// </summary>
        ////[TestMethod]
        ////public void SaveWithChildren_NewFakeRaisedComplexEntityWithDependentEntity_MatchesExpected()
        ////{
        ////    var subSubEntity = new SubSubEntity("jasdyri");
        ////    var subEntity = new SubEntity("asidf", 58, subSubEntity);
        ////    var fakeCreatedBy = new CreatedBy("lasjdas") { Description = "aslfdjasjkld" };
        ////    var fakeModifiedBy = new ModifiedBy("adskljs") { Description = ",znxckas" };

        ////    var expected = new ComplexEntity("lajsf", subEntity, FakeEnumeration.FirstFake, fakeCreatedBy)
        ////                       {
        ////                           ModifiedBy = fakeModifiedBy,
        ////                           ModifiedTime = DateTimeOffset.Now,
        ////                           FakeOtherEnumeration = FakeOtherEnumeration.OtherFakeFirst,
        ////                           Description = "asjkla eh"
        ////                       };

        ////    expected.SetDependentEntity(985, DateTimeOffset.Now);

        ////    var parentChildEntity = new ChildEntity(expected) { Name = "Parent", SomeValue = 243 };
        ////    var childEntity1 = new ChildEntity(expected) { Name = "Child1", SomeValue = 48597, Parent = parentChildEntity };
        ////    var childEntity2 = new ChildEntity(expected) { Name = "Child2", SomeValue = 345453, Parent = parentChildEntity };

        ////    var childList = new List<ChildEntity> { parentChildEntity, childEntity1, childEntity2 };
        ////    expected.Load(childList);

        ////    var repositoryAdapter = RepositoryMockFactory.CreateAdapter();
        ////    using (var repositoryProvider = RepositoryMockFactory.CreateConcreteProvider<FakeDataContext>(this.entityMapper, repositoryAdapter))
        ////    {
        ////        repositoryAdapter.StubForNewItem<SubRow>(repositoryProvider.EntityDefinitionProvider);
        ////        repositoryAdapter.StubForNewItem<SubSubRow>(repositoryProvider.EntityDefinitionProvider);
        ////        repositoryAdapter.StubForNewItem<ComplexRaisedRow>(repositoryProvider.EntityDefinitionProvider);
        ////        repositoryAdapter.StubForNewItem<MultiReferenceRow>(repositoryProvider.EntityDefinitionProvider);
        ////        repositoryAdapter.StubForNewItem<DependentRow>(repositoryProvider.EntityDefinitionProvider);
        ////        repositoryAdapter.StubForNewItem<ChildRaisedRow>(repositoryProvider.EntityDefinitionProvider);

        ////        var target = new FakeRaisedComplexEntityRepository(repositoryProvider, this.entityMapper);
        ////        var actual = target.SaveWithChildren(expected);

        ////        Assert.IsTrue(actual.FakeComplexEntityId > 0);
        ////        Assert.IsTrue(actual.CreatedByFakeMultiReferenceEntityId > 0);
        ////        Assert.IsTrue(actual.ModifiedByFakeMultiReferenceEntityId > 0);
        ////        Assert.IsTrue(actual.FakeSubEntityId > 0);
        ////        Assert.IsTrue(actual.FakeSubSubEntityId > 0);
        ////        Assert.IsTrue(actual.FakeDependentEntityId > 0);
        ////        Assert.AreSame(expected, actual);

        ////        foreach (var expectedChildEntity in expected.ChildEntities)
        ////        {
        ////            var actualChildEntity = actual.ChildEntities.FirstOrDefault(x => x.SomeValue == expectedChildEntity.SomeValue);
        ////            Assert.IsNotNull(actualChildEntity);
        ////            Assert.AreEqual(expectedChildEntity, actualChildEntity);
        ////            Assert.IsTrue(actualChildEntity.FakeChildEntityId.HasValue);
        ////            Assert.IsTrue(actualChildEntity.FakeChildEntityId > 0);
        ////        }
        ////    }
        ////}
    }
}