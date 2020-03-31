// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadOnlyRepositoryTest.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Rhino.Mocks;

    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Mapper;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Testing.Entities;
    using Startitecture.Orm.Testing.Model;
    using Startitecture.Orm.Testing.RhinoMocks;

    /// <summary>
    /// The fake raised sub entity repository test.
    /// </summary>
    [TestClass]
    public partial class ReadOnlyRepositoryTest
    {
        /// <summary>
        /// The entity mapper.
        /// </summary>
        private readonly IEntityMapper entityMapper = RepositoryMockFactory.CreateEntityMapper(
            expression =>
                {
                    expression.AddProfile<FakeSubEntityMappingProfile>();
                    expression.AddProfile<FakeSubSubEntityMappingProfile>();
                });

        /// <summary>
        /// Tests that the first or default of the repository matches the expected results.
        /// </summary>
        [TestMethod]
        public void FirstOrDefault_FakeRaisedSubEntity_MatchesExpected()
        {
            var fakeSubSubEntity = new SubSubEntity("jasdyri", 5848);
            var expected = new SubEntity("asidf", 58, fakeSubSubEntity, 87543);

            var repositoryAdapter = RepositoryMockFactory.CreateAdapter();
            repositoryAdapter.StubForExistingItem<SubRow>(expected, this.entityMapper);

            using (var repositoryProvider = RepositoryMockFactory.CreateConcreteProvider<FakeDataContext>(this.entityMapper, repositoryAdapter))
            {
                var target = new ReadOnlyRepository<SubEntity, SubRow>(repositoryProvider, this.entityMapper);
                var actual = target.FirstOrDefault(expected.FakeSubEntityId.GetValueOrDefault());

                Assert.AreEqual(expected.FakeSubEntityId, actual.FakeSubEntityId);
                Assert.AreEqual(expected.FakeSubSubEntityId, actual.FakeSubSubEntityId);
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        /// Tests that the first or default of the repository matches the expected results.
        /// </summary>
        [TestMethod]
        public void SelectEntities_FakeRaisedSubEntity_MatchesExpected()
        {
            var fakeSubSubEntity = new SubSubEntity("jasdyri", 5848);
            var expected = new List<SubEntity>
                               {
                                   new SubEntity("asidf", 58, fakeSubSubEntity, 87543),
                                   new SubEntity("safd", 59, fakeSubSubEntity, 546),
                                   new SubEntity("gjkdf", 52, fakeSubSubEntity, 3465)
                               };

            var repositoryAdapter = RepositoryMockFactory.CreateAdapter();

            using (var repositoryProvider = RepositoryMockFactory.CreateConcreteProvider<FakeDataContext>(this.entityMapper, repositoryAdapter))
            {
                repositoryAdapter.StubForExistingItem<SubRow>(expected, this.entityMapper);
                repositoryAdapter.StubForList(this.entityMapper.Map<List<SubRow>>(expected), repositoryProvider.EntityDefinitionProvider);

                var target = new ReadOnlyRepository<SubEntity, SubRow>(repositoryProvider, this.entityMapper);
                var actual = target.SelectAll().ToList();

                CollectionAssert.AreEqual(expected, actual);

                using (var expectedEnumerator = expected.GetEnumerator())
                using (var actualEnumerator = actual.GetEnumerator())
                {
                    while (expectedEnumerator.MoveNext())
                    {
                        actualEnumerator.MoveNext();

                        var expectedEntity = expectedEnumerator.Current;
                        var actualEntity = actualEnumerator.Current;
                        Assert.AreEqual(expectedEntity?.FakeSubEntityId, actualEntity?.FakeSubEntityId);
                        Assert.AreEqual(expectedEntity?.FakeSubSubEntityId, actualEntity?.FakeSubSubEntityId);
                    }
                }
            }
        }

        /// <summary>
        /// The save test.
        /// </summary>
        [TestMethod]
        public void Select_FirstOrDefaultFakeChildEntity_MatchesExpected()
        {
            var fakeSubSubEntity = new SubSubEntity("SubSubUniqueName1", 45)
            {
                Description = "OriginalSubSub"
            };
            var fakeSubEntity = new SubEntity("SubUniqueName1", 234, fakeSubSubEntity, 16)
            {
                Description = "OriginalSub"
            };
            var originalCreatedBy = new CreatedBy("CreateUniqueName", 432)
            {
                Description = "OriginalCreatedBy"
            };
            var modifiedBy = new ModifiedBy("ModifiedBy1", 433)
            {
                Description = "OriginalModifiedBy1"
            };
            var creationTime = DateTimeOffset.Now.AddDays(-1);
            var fakeComplexEntity = new ComplexEntity(
                                        "UniqueName1",
                                        fakeSubEntity,
                                        FakeEnumeration.FirstFake,
                                        originalCreatedBy,
                                        creationTime,
                                        22)
            {
                Description = "OriginalComplexEntity1",
                ModifiedBy = modifiedBy,
                ModifiedTime = DateTimeOffset.Now.AddHours(1)
            };

            var expected = new ChildEntity(fakeComplexEntity, 435)
            {
                Name = "OriginalName",
                SomeValue = 111
            };

            var existing = this.entityMapper.Map<ChildRaisedRow>(expected);
            var repositoryAdapter = MockRepository.GenerateMock<IRepositoryAdapter>();
            repositoryAdapter.Stub(adapter => adapter.FirstOrDefault(Arg<ItemSelection<ChildRaisedRow>>.Is.Anything)).Return(existing);

            var repositoryAdapterFactory = MockRepository.GenerateMock<IRepositoryAdapterFactory>();
            repositoryAdapterFactory.Stub(factory => factory.Create(Arg<Database>.Is.Anything)).Return(repositoryAdapter);

            using (var provider = new DatabaseRepositoryProvider(
                GenericDatabaseFactory<FakeDataContext>.Default,
                this.entityMapper,
                repositoryAdapterFactory))
            {
                var target = new EntityRepository<ChildEntity, ChildRaisedRow>(provider, this.entityMapper);
                var actual = target.FirstOrDefault(existing);
                Assert.AreEqual(
                    expected.SubSubEntity,
                    actual.SubSubEntity,
                    string.Join(Environment.NewLine, expected.SubSubEntity.GetDifferences(actual.SubSubEntity)));

                Assert.AreEqual(
                    expected.SubEntity,
                    actual.SubEntity,
                    string.Join(Environment.NewLine, expected.SubEntity.GetDifferences(actual.SubEntity)));

                Assert.AreEqual(
                    expected.ModifiedBy,
                    actual.ModifiedBy,
                    string.Join(Environment.NewLine, expected.ModifiedBy.GetDifferences(actual.ModifiedBy)));

                Assert.AreEqual(
                    expected.CreatedBy,
                    actual.CreatedBy,
                    string.Join(Environment.NewLine, expected.CreatedBy.GetDifferences(actual.CreatedBy)));

                Assert.AreEqual(
                    expected.ComplexEntity,
                    actual.ComplexEntity,
                    string.Join(Environment.NewLine, expected.ComplexEntity.GetDifferences(actual.ComplexEntity)));

                Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
            }
        }

        /// <summary>
        /// The save test.
        /// </summary>
        [TestMethod]
        public void Select_FirstOrDefaultFakeChildEntityWithDependentEntity_MatchesExpected()
        {
            var fakeSubSubEntity = new SubSubEntity("SubSubUniqueName1", 45)
            {
                Description = "OriginalSubSub"
            };
            var fakeSubEntity = new SubEntity("SubUniqueName1", 234, fakeSubSubEntity, 16)
            {
                Description = "OriginalSub"
            };
            var originalCreatedBy = new CreatedBy("CreateUniqueName", 432)
            {
                Description = "OriginalCreatedBy"
            };
            var modifiedBy = new ModifiedBy("ModifiedBy1", 433)
            {
                Description = "OriginalModifiedBy1"
            };
            var creationTime = DateTimeOffset.Now.AddDays(-1);
            var fakeComplexEntity = new ComplexEntity(
                                        "UniqueName1",
                                        fakeSubEntity,
                                        FakeEnumeration.FirstFake,
                                        originalCreatedBy,
                                        creationTime,
                                        22)
            {
                Description = "OriginalComplexEntity1",
                ModifiedBy = modifiedBy,
                ModifiedTime = DateTimeOffset.Now.AddHours(1)
            };

            fakeComplexEntity.SetDependentEntity(994);

            var expected = new ChildEntity(fakeComplexEntity, 435)
            {
                Name = "OriginalName",
                SomeValue = 111
            };

            var existing = this.entityMapper.Map<ChildRaisedRow>(expected);
            var repositoryAdapter = MockRepository.GenerateMock<IRepositoryAdapter>();
            repositoryAdapter.Stub(adapter => adapter.FirstOrDefault(Arg<ItemSelection<ChildRaisedRow>>.Is.Anything)).Return(existing);

            var repositoryAdapterFactory = MockRepository.GenerateMock<IRepositoryAdapterFactory>();
            repositoryAdapterFactory.Stub(factory => factory.Create(Arg<Database>.Is.Anything)).Return(repositoryAdapter);

            using (var provider = new DatabaseRepositoryProvider(
                GenericDatabaseFactory<FakeDataContext>.Default,
                this.entityMapper,
                repositoryAdapterFactory))
            {
                var target = new EntityRepository<ChildEntity, ChildRaisedRow>(provider, this.entityMapper);
                var actual = target.FirstOrDefault(existing);
                Assert.IsNotNull(expected.DependentEntity);
                Assert.IsNotNull(expected.FakeDependentEntityId);
                Assert.AreEqual(
                    expected.SubSubEntity,
                    actual.SubSubEntity,
                    string.Join(Environment.NewLine, expected.SubSubEntity.GetDifferences(actual.SubSubEntity)));

                Assert.AreEqual(
                    expected.SubEntity,
                    actual.SubEntity,
                    string.Join(Environment.NewLine, expected.SubEntity.GetDifferences(actual.SubEntity)));

                Assert.AreEqual(
                    expected.ModifiedBy,
                    actual.ModifiedBy,
                    string.Join(Environment.NewLine, expected.ModifiedBy.GetDifferences(actual.ModifiedBy)));

                Assert.AreEqual(
                    expected.CreatedBy,
                    actual.CreatedBy,
                    string.Join(Environment.NewLine, expected.CreatedBy.GetDifferences(actual.CreatedBy)));

                Assert.AreEqual(
                    expected.DependentEntity,
                    actual.DependentEntity,
                    string.Join(Environment.NewLine, expected.DependentEntity.GetDifferences(actual.DependentEntity)));

                Assert.AreEqual(
                    expected.ComplexEntity,
                    actual.ComplexEntity,
                    string.Join(Environment.NewLine, expected.ComplexEntity.GetDifferences(actual.ComplexEntity)));

                Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
            }
        }
    }
}