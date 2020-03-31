// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityRepositoryTests.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Startitecture.Orm.Repository.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Rhino.Mocks;

    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Mapper;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Schema;
    using Startitecture.Orm.Testing.Entities;
    using Startitecture.Orm.Testing.Model;
    using Startitecture.Orm.Testing.RhinoMocks;

    using MockRepository = Rhino.Mocks.MockRepository;

    /// <summary>
    /// The entity repository tests.
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class EntityRepositoryTests
    {
        /// <summary>
        /// The entity mapper.
        /// </summary>
        private readonly IEntityMapper entityMapper = CreateEntityMapper();

        /// <summary>
        /// The save test.
        /// </summary>
        [TestMethod]
        public void Save_NewFakeComplexEntity_MatchesExpected()
        {
            var repositoryAdapter = new Mock<IRepositoryAdapter>();
            repositoryAdapter.Setup(adapter => adapter.Insert(It.IsAny<ComplexRaisedRow>()))
                .Returns(
                    (ComplexRaisedRow row) =>
                        {
                            row.SetPropertyValue(raisedRow => raisedRow.FakeComplexEntityId, 43);
                            return row;
                        });

            var repositoryAdapterFactory = new Mock<IRepositoryAdapterFactory>();
            repositoryAdapterFactory
                .Setup(factory => factory.Create(It.IsAny<IDatabaseContext>())).Returns(repositoryAdapter.Object);

            var databaseContext = new Mock<IDatabaseContext>();
            ////databaseContext.Setup(context => context.)

            var databaseFactory = new Mock<IDatabaseFactory>();
            databaseFactory.Setup(factory => factory.Create()).Returns(databaseContext.Object);

            using (var provider = new DatabaseRepositoryProvider(
                GenericDatabaseFactory<FakeDataContext>.Default,
                this.entityMapper,
                repositoryAdapterFactory.Object))
            {
                var target = new EntityRepository<ComplexEntity, ComplexRaisedRow>(provider, this.entityMapper);
                var fakeSubSubEntity = new SubSubEntity("SubSubUniqueName", 6445);
                var fakeSubEntity = new SubEntity("SubUniqueName", 234, fakeSubSubEntity);
                var fakeCreatedBy = new CreatedBy("CreateUniqueName", 1122);
                var fakeModifiedBy = new ModifiedBy("ModifiedBy");
                var expected = new ComplexEntity("UniqueName", fakeSubEntity, FakeEnumeration.FirstFake, fakeCreatedBy)
                                   {
                                       ModifiedBy = fakeModifiedBy,
                                       ModifiedTime = DateTimeOffset.Now.AddHours(1)
                                   };

                var actual = target.Save(expected);
                Assert.IsNotNull(actual.SubEntity);
                Assert.IsNotNull(actual.SubEntity.SubSubEntity);
                Assert.IsNotNull(actual.CreatedBy);
                Assert.IsNotNull(actual.ModifiedBy);
                Assert.AreEqual(16, actual.SubEntity.FakeSubEntityId);
                Assert.AreEqual(expected.SubEntity, actual.SubEntity);
                Assert.AreEqual(45, actual.SubSubEntity.FakeSubSubEntityId);
                Assert.AreEqual(expected.SubEntity.SubSubEntity, actual.SubEntity.SubSubEntity);
                Assert.AreEqual(432, actual.CreatedBy.FakeMultiReferenceEntityId);
                Assert.AreEqual(expected.CreatedBy, actual.CreatedBy);
                Assert.AreEqual(432, actual.ModifiedBy.FakeMultiReferenceEntityId);
                Assert.AreEqual(expected.ModifiedBy, actual.ModifiedBy);
                Assert.AreEqual(43, actual.FakeComplexEntityId);
                Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
            }
        }

        /// <summary>
        /// The save test.
        /// </summary>
        [TestMethod]
        public void Save_NewFakeRaisedComplexEntity_MatchesExpected()
        {
            var repositoryAdapterFactory = CreateInsertRepositoryAdapterFactory();

            using (var provider = new DatabaseRepositoryProvider(
                GenericDatabaseFactory<FakeDataContext>.Default,
                this.entityMapper,
                repositoryAdapterFactory))
            {
                var target = new EntityRepository<ComplexEntity, ComplexRaisedRow>(provider, this.entityMapper);
                var fakeSubSubEntity = new SubSubEntity("SubSubUniqueName");
                var fakeSubEntity = new SubEntity("SubUniqueName", 234, fakeSubSubEntity);
                var fakeCreatedBy = new CreatedBy("CreateUniqueName");
                var fakeModifiedBy = new ModifiedBy("ModifiedBy");
                var expected = new ComplexEntity("UniqueName", fakeSubEntity, FakeEnumeration.FirstFake, fakeCreatedBy)
                                   {
                                       ModifiedBy = fakeModifiedBy,
                                       ModifiedTime = DateTimeOffset.Now.AddHours(1)
                                   };

                var actual = target.Save(expected);
                Assert.IsNotNull(actual.SubEntity);
                Assert.IsNotNull(actual.SubEntity.SubSubEntity);
                Assert.IsNotNull(actual.CreatedBy);
                Assert.IsNotNull(actual.ModifiedBy);
                Assert.AreEqual(16, actual.SubEntity.FakeSubEntityId);
                Assert.AreEqual(expected.SubEntity, actual.SubEntity);
                Assert.AreEqual(45, actual.SubSubEntity.FakeSubSubEntityId);
                Assert.AreEqual(expected.SubEntity.SubSubEntity, actual.SubEntity.SubSubEntity);
                Assert.AreEqual(432, actual.CreatedBy.FakeMultiReferenceEntityId);
                Assert.AreEqual(expected.CreatedBy, actual.CreatedBy);
                Assert.AreEqual(432, actual.ModifiedBy.FakeMultiReferenceEntityId);
                Assert.AreEqual(expected.ModifiedBy, actual.ModifiedBy);
                Assert.AreEqual(22, actual.FakeComplexEntityId);
                Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
            }
        }

        /// <summary>
        /// The save test.
        /// </summary>
        [TestMethod]
        public void Save_NewFakeConstructedComplexEntity_MatchesExpected()
        {
            var repositoryAdapterFactory = CreateInsertRepositoryAdapterFactory();

            using (var provider = new DatabaseRepositoryProvider(
                GenericDatabaseFactory<FakeDataContext>.Default,
                this.entityMapper,
                repositoryAdapterFactory))
            {
                var target = new EntityRepository<ComplexEntity, ComplexRaisedRow>(provider, this.entityMapper);
                var fakeSubSubEntity = new SubSubEntity("SubSubUniqueName");
                var fakeSubEntity = new SubEntity("SubUniqueName", 234, fakeSubSubEntity);
                var fakeCreatedBy = new CreatedBy("CreateUniqueName");
                var fakeModifiedBy = new ModifiedBy("ModifiedBy");
                var expected = new ComplexEntity("UniqueName", fakeSubEntity, FakeEnumeration.FirstFake, fakeCreatedBy)
                                   {
                                       ModifiedBy = fakeModifiedBy,
                                       ModifiedTime = DateTimeOffset.Now.AddHours(1)
                                   };

                var actual = target.Save(expected);
                Assert.IsNotNull(actual.SubEntity);
                Assert.IsNotNull(actual.SubEntity.SubSubEntity);
                Assert.IsNotNull(actual.CreatedBy);
                Assert.IsNotNull(actual.ModifiedBy);
                Assert.AreEqual(16, actual.SubEntity.FakeSubEntityId);
                Assert.AreEqual(expected.SubEntity, actual.SubEntity);
                Assert.AreEqual(45, actual.SubSubEntity.FakeSubSubEntityId);
                Assert.AreEqual(expected.SubEntity.SubSubEntity, actual.SubEntity.SubSubEntity);
                Assert.AreEqual(432, actual.CreatedBy.FakeMultiReferenceEntityId);
                Assert.AreEqual(expected.CreatedBy, actual.CreatedBy);
                Assert.AreEqual(432, actual.ModifiedBy.FakeMultiReferenceEntityId);
                Assert.AreEqual(expected.ModifiedBy, actual.ModifiedBy);
                Assert.AreEqual(22, actual.FakeComplexEntityId);
                Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
            }
        }

        /// <summary>
        /// The save test.
        /// </summary>
        [TestMethod]
        public void Save_NewFakeComplexEntityWithDependentEntity_MatchesExpected()
        {
            var repositoryAdapterFactory = CreateInsertRepositoryAdapterFactory();

            using (var provider = new DatabaseRepositoryProvider(
                GenericDatabaseFactory<FakeDataContext>.Default,
                this.entityMapper,
                repositoryAdapterFactory))
            {
                var target = new EntityRepository<ComplexEntity, ComplexRaisedRow>(provider, this.entityMapper);
                var fakeSubSubEntity = new SubSubEntity("SubSubUniqueName");
                var fakeSubEntity = new SubEntity("SubUniqueName", 234, fakeSubSubEntity);
                var fakeCreatedBy = new CreatedBy("CreateUniqueName");
                var modifiedBy = new ModifiedBy("ModifiedBy");
                var expected = new ComplexEntity("UniqueName", fakeSubEntity, FakeEnumeration.FirstFake, fakeCreatedBy)
                                   {
                                       ModifiedBy = modifiedBy,
                                       ModifiedTime = DateTimeOffset.Now.AddHours(1)
                                   };

                expected.SetDependentEntity(33, DateTimeOffset.Now);

                var actual = target.Save(expected);
                Assert.IsNotNull(actual.SubEntity);
                Assert.IsNotNull(actual.SubEntity.SubSubEntity);
                Assert.IsNotNull(actual.CreatedBy);
                Assert.IsNotNull(actual.ModifiedBy);
                Assert.IsNotNull(actual.DependentEntity);
                Assert.AreEqual(16, actual.SubEntity.FakeSubEntityId);
                Assert.AreEqual(expected.SubEntity, actual.SubEntity);
                Assert.AreEqual(45, actual.SubSubEntity.FakeSubSubEntityId);
                Assert.AreEqual(expected.SubEntity.SubSubEntity, actual.SubEntity.SubSubEntity);
                Assert.AreEqual(432, actual.CreatedBy.FakeMultiReferenceEntityId);
                Assert.AreEqual(expected.CreatedBy, actual.CreatedBy);
                Assert.AreEqual(432, actual.ModifiedBy.FakeMultiReferenceEntityId);
                Assert.AreEqual(expected.ModifiedBy, actual.ModifiedBy);
                Assert.AreEqual(22, actual.FakeDependentEntityId);
                Assert.AreEqual(expected.DependentEntity, actual.DependentEntity);
                Assert.AreEqual(22, actual.FakeComplexEntityId);
                Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
            }
        }

        /// <summary>
        /// The save test.
        /// </summary>
        [TestMethod]
        public void Save_UpdatedFakeComplexEntity_MatchesExpected()
        {
            var fakeSubSubEntity = new SubSubEntity("SubSubUniqueName", 45)
                                       {
                                           Description = "OriginalSubSub"
                                       };
            var fakeSubEntity = new SubEntity("SubUniqueName", 234, fakeSubSubEntity, 16)
                                    {
                                        Description = "OriginalSub"
                                    };
            var originalCreatedBy = new CreatedBy("CreateUniqueName", 432)
                                        {
                                            Description = "OriginalCreatedBy"
                                        };
            var modifiedBy = new ModifiedBy("ModifiedBy", 433)
                                 {
                                     Description = "OriginalModifiedBy"
                                 };
            var creationTime = DateTimeOffset.Now.AddDays(-1);
            var baseline = new ComplexEntity("UniqueName", fakeSubEntity, FakeEnumeration.FirstFake, originalCreatedBy, creationTime, 22)
                               {
                                   Description = "OriginalComplexEntity",
                                   ModifiedBy = modifiedBy,
                                   ModifiedTime = DateTimeOffset.Now.AddHours(1)
                               };

            var repositoryAdapter = RepositoryMockFactory.CreateAdapter();
            repositoryAdapter.StubForExistingItem<SubSubRow>(fakeSubSubEntity, this.entityMapper);
            repositoryAdapter.StubForExistingItem<SubRow>(fakeSubEntity, this.entityMapper);
            repositoryAdapter.StubForExistingItem<MultiReferenceRow, int>(
                originalCreatedBy,
                this.entityMapper,
                item => item.FakeMultiReferenceEntityId,
                originalCreatedBy.FakeMultiReferenceEntityId.GetValueOrDefault());

            repositoryAdapter.StubForExistingItem<MultiReferenceRow, int>(
                modifiedBy,
                this.entityMapper,
                item => item.FakeMultiReferenceEntityId,
                modifiedBy.FakeMultiReferenceEntityId.GetValueOrDefault());

            repositoryAdapter.StubForExistingItem<ComplexRaisedRow>(baseline, this.entityMapper);

            using (var provider = RepositoryMockFactory.CreateConcreteProvider<FakeDataContext>(this.entityMapper, repositoryAdapter))
            {
                var newModifiedBy = new ModifiedBy("ModifiedBy", 433)
                                        {
                                            Description = "UpdatedModifiedBy"
                                        };

                var target = new EntityRepository<ComplexEntity, ComplexRaisedRow>(provider, this.entityMapper);
                var expected = target.FirstOrDefault(22);
                expected.Description = "UpdatedEntity";
                expected.ModifiedBy = newModifiedBy;
                expected.ModifiedTime = DateTimeOffset.Now.AddHours(1);
                expected.SubEntity.Description = "ModifiedSub";
                expected.SubSubEntity.Description = "ModifiedSubSub";

                var actual = target.Save(expected);
                Assert.IsNotNull(actual.SubEntity);
                Assert.IsNotNull(actual.SubEntity.SubSubEntity);
                Assert.IsNotNull(actual.CreatedBy);
                Assert.IsNotNull(actual.ModifiedBy);
                Assert.AreEqual(16, actual.SubEntity.FakeSubEntityId);
                Assert.AreEqual(expected.SubEntity, actual.SubEntity);
                Assert.AreEqual(45, actual.SubSubEntity.FakeSubSubEntityId);
                Assert.AreEqual(expected.SubEntity.SubSubEntity, actual.SubEntity.SubSubEntity);
                Assert.AreEqual(432, actual.CreatedBy.FakeMultiReferenceEntityId);
                Assert.AreEqual(
                    baseline.CreatedBy,
                    actual.CreatedBy,
                    string.Join(Environment.NewLine, baseline.CreatedBy.GetDifferences(actual.CreatedBy)));

                Assert.AreEqual(creationTime, actual.CreationTime);
                Assert.AreEqual(433, actual.ModifiedBy.FakeMultiReferenceEntityId);
                Assert.AreEqual(expected.ModifiedBy, actual.ModifiedBy);
                Assert.AreEqual(expected.ModifiedTime, actual.ModifiedTime);
                Assert.AreEqual(22, actual.FakeComplexEntityId);
                Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
            }
        }

        /// <summary>
        /// The save test.
        /// </summary>
        [TestMethod]
        public void Save_UpdatedFakeComplexEntityWithNewDependentEntity_MatchesExpected()
        {
            var fakeSubSubEntity = new SubSubEntity("SubSubUniqueName", 45)
                                       {
                                           Description = "OriginalSubSub"
                                       };
            var fakeSubEntity = new SubEntity("SubUniqueName", 234, fakeSubSubEntity, 16)
                                    {
                                        Description = "OriginalSub"
                                    };
            var originalCreatedBy = new CreatedBy("CreateUniqueName", 432)
                                        {
                                            Description = "OriginalCreatedBy"
                                        };
            var modifiedBy = new ModifiedBy("ModifiedBy", 433)
                                 {
                                     Description = "OriginalModifiedBy"
                                 };
            var creationTime = DateTimeOffset.Now.AddDays(-1);
            var baseline = new ComplexEntity("UniqueName", fakeSubEntity, FakeEnumeration.FirstFake, originalCreatedBy, creationTime, 22)
                               {
                                   Description = "OriginalComplexEntity",
                                   ModifiedBy = modifiedBy,
                                   ModifiedTime = DateTimeOffset.Now.AddHours(1)
                               };

            var repositoryAdapter = RepositoryMockFactory.CreateAdapter();
            repositoryAdapter.StubForExistingItem<SubSubRow>(fakeSubSubEntity, this.entityMapper);
            repositoryAdapter.StubForExistingItem<SubRow>(fakeSubEntity, this.entityMapper);
            repositoryAdapter.StubForExistingItem<MultiReferenceRow, int>(
                originalCreatedBy,
                this.entityMapper,
                item => item.FakeMultiReferenceEntityId,
                originalCreatedBy.FakeMultiReferenceEntityId.GetValueOrDefault());

            repositoryAdapter.StubForExistingItem<MultiReferenceRow, int>(
                modifiedBy,
                this.entityMapper,
                item => item.FakeMultiReferenceEntityId,
                modifiedBy.FakeMultiReferenceEntityId.GetValueOrDefault());

            repositoryAdapter.StubForExistingItem<ComplexRaisedRow>(baseline, this.entityMapper);

            using (var provider = RepositoryMockFactory.CreateConcreteProvider<FakeDataContext>(this.entityMapper, repositoryAdapter))
            {
                var newModifiedBy = new ModifiedBy("ModifiedBy", 433)
                                        {
                                            Description = "UpdatedModifiedBy"
                                        };

                var target = new EntityRepository<ComplexEntity, ComplexRaisedRow>(provider, this.entityMapper);
                var expected = target.FirstOrDefault(22);
                expected.Description = "UpdatedEntity";
                expected.ModifiedBy = newModifiedBy;
                expected.ModifiedTime = DateTimeOffset.Now.AddHours(1);
                expected.SubEntity.Description = "ModifiedSub";
                expected.SubSubEntity.Description = "ModifiedSubSub";
                expected.SetDependentEntity(33);

                repositoryAdapter.StubForNewItem<DependentRow>(new DataAnnotationsDefinitionProvider());

                var actual = target.Save(expected);
                Assert.IsNotNull(actual.SubEntity);
                Assert.IsNotNull(actual.SubEntity.SubSubEntity);
                Assert.IsNotNull(actual.CreatedBy);
                Assert.IsNotNull(actual.ModifiedBy);
                Assert.IsNotNull(actual.DependentEntity);
                Assert.AreEqual(16, actual.SubEntity.FakeSubEntityId);
                Assert.AreEqual(expected.SubEntity, actual.SubEntity);
                Assert.AreEqual(45, actual.SubSubEntity.FakeSubSubEntityId);
                Assert.AreEqual(expected.SubEntity.SubSubEntity, actual.SubEntity.SubSubEntity);
                Assert.AreEqual(432, actual.CreatedBy.FakeMultiReferenceEntityId);
                Assert.AreEqual(
                    baseline.CreatedBy,
                    actual.CreatedBy,
                    string.Join(Environment.NewLine, baseline.CreatedBy.GetDifferences(actual.CreatedBy)));

                Assert.AreEqual(creationTime, actual.CreationTime);
                Assert.AreEqual(433, actual.ModifiedBy.FakeMultiReferenceEntityId);
                Assert.AreEqual(expected.ModifiedBy, actual.ModifiedBy);
                Assert.AreEqual(expected.ModifiedTime, actual.ModifiedTime);
                Assert.AreEqual(22, actual.FakeComplexEntityId);
                Assert.AreEqual(22, actual.FakeDependentEntityId);
                Assert.AreEqual(expected.DependentEntity, actual.DependentEntity);
                Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
            }
        }

        /// <summary>
        /// The save test.
        /// </summary>
        [TestMethod]
        public void Save_UpdatedFakeComplexEntityWithUpdatedDependentEntity_MatchesExpected()
        {
            var fakeSubSubEntity = new SubSubEntity("SubSubUniqueName", 45)
                                       {
                                           Description = "OriginalSubSub"
                                       };
            var fakeSubEntity = new SubEntity("SubUniqueName", 234, fakeSubSubEntity, 16)
                                    {
                                        Description = "OriginalSub"
                                    };
            var originalCreatedBy = new CreatedBy("CreateUniqueName", 432)
                                        {
                                            Description = "OriginalCreatedBy"
                                        };
            var modifiedBy = new ModifiedBy("ModifiedBy", 433)
                                 {
                                     Description = "OriginalModifiedBy"
                                 };
            var creationTime = DateTimeOffset.Now.AddDays(-1);
            var baseline = new ComplexEntity("UniqueName", fakeSubEntity, FakeEnumeration.FirstFake, originalCreatedBy, creationTime, 22)
                               {
                                   Description = "OriginalComplexEntity",
                                   ModifiedBy = modifiedBy,
                                   ModifiedTime = DateTimeOffset.Now.AddHours(1)
                               };

            var dependentEntity = baseline.SetDependentEntity(9845, DateTimeOffset.Now.AddHours(-3));

            var repositoryAdapter = RepositoryMockFactory.CreateAdapter();
            repositoryAdapter.StubForExistingItem<SubSubRow>(fakeSubSubEntity, this.entityMapper);
            repositoryAdapter.StubForExistingItem<SubRow>(fakeSubEntity, this.entityMapper);
            repositoryAdapter.StubForExistingItem<MultiReferenceRow, int>(
                originalCreatedBy,
                this.entityMapper,
                item => item.FakeMultiReferenceEntityId,
                originalCreatedBy.FakeMultiReferenceEntityId.GetValueOrDefault());

            repositoryAdapter.StubForExistingItem<MultiReferenceRow, int>(
                modifiedBy,
                this.entityMapper,
                item => item.FakeMultiReferenceEntityId,
                modifiedBy.FakeMultiReferenceEntityId.GetValueOrDefault());

            repositoryAdapter.StubForExistingItem<ComplexRaisedRow>(baseline, this.entityMapper);
            repositoryAdapter.StubForExistingItem<DependentRow>(dependentEntity, this.entityMapper);

            using (var provider = RepositoryMockFactory.CreateConcreteProvider<FakeDataContext>(this.entityMapper, repositoryAdapter))
            {
                var newModifiedBy = new ModifiedBy("ModifiedBy", 433)
                                        {
                                            Description = "UpdatedModifiedBy"
                                        };

                var target = new EntityRepository<ComplexEntity, ComplexRaisedRow>(provider, this.entityMapper);
                var expected = target.FirstOrDefault(22);
                expected.Description = "UpdatedEntity";
                expected.ModifiedBy = newModifiedBy;
                expected.ModifiedTime = DateTimeOffset.Now.AddHours(1);
                expected.SubEntity.Description = "ModifiedSub";
                expected.SubSubEntity.Description = "ModifiedSubSub";
                expected.SetDependentEntity(992);

                var actual = target.Save(expected);
                Assert.IsNotNull(actual.SubEntity);
                Assert.IsNotNull(actual.SubEntity.SubSubEntity);
                Assert.IsNotNull(actual.CreatedBy);
                Assert.IsNotNull(actual.ModifiedBy);
                Assert.AreEqual(16, actual.SubEntity.FakeSubEntityId);
                Assert.AreEqual(expected.SubEntity, actual.SubEntity);
                Assert.AreEqual(45, actual.SubSubEntity.FakeSubSubEntityId);
                Assert.AreEqual(expected.SubEntity.SubSubEntity, actual.SubEntity.SubSubEntity);
                Assert.AreEqual(432, actual.CreatedBy.FakeMultiReferenceEntityId);
                Assert.AreEqual(
                    baseline.CreatedBy,
                    actual.CreatedBy,
                    string.Join(Environment.NewLine, baseline.CreatedBy.GetDifferences(actual.CreatedBy)));

                Assert.AreEqual(creationTime, actual.CreationTime);
                Assert.AreEqual(433, actual.ModifiedBy.FakeMultiReferenceEntityId);
                Assert.AreEqual(expected.ModifiedBy, actual.ModifiedBy);
                Assert.AreEqual(expected.ModifiedTime, actual.ModifiedTime);
                Assert.AreEqual(22, actual.FakeDependentEntityId);
                Assert.AreEqual(expected.DependentEntity, actual.DependentEntity);
                Assert.AreEqual(22, actual.FakeComplexEntityId);
                Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
            }
        }

        /// <summary>
        /// The save test.
        /// </summary>
        [TestMethod]
        public void Save_UpdatedFakeComplexEntityWithRemovedDependentEntity_MatchesExpected()
        {
            var fakeSubSubEntity = new SubSubEntity("SubSubUniqueName", 45)
                                       {
                                           Description = "OriginalSubSub"
                                       };
            var fakeSubEntity = new SubEntity("SubUniqueName", 234, fakeSubSubEntity, 16)
                                    {
                                        Description = "OriginalSub"
                                    };
            var originalCreatedBy = new CreatedBy("CreateUniqueName", 432)
                                        {
                                            Description = "OriginalCreatedBy"
                                        };
            var modifiedBy = new ModifiedBy("ModifiedBy", 433)
                                 {
                                     Description = "OriginalModifiedBy"
                                 };
            var creationTime = DateTimeOffset.Now.AddDays(-1);
            var baseline = new ComplexEntity("UniqueName", fakeSubEntity, FakeEnumeration.FirstFake, originalCreatedBy, creationTime, 22)
                               {
                                   Description = "OriginalComplexEntity",
                                   ModifiedBy = modifiedBy,
                                   ModifiedTime = DateTimeOffset.Now.AddHours(1)
                               };

            var dependentEntity = baseline.SetDependentEntity(393);

            var repositoryAdapter = RepositoryMockFactory.CreateAdapter();
            repositoryAdapter.StubForExistingItem<SubSubRow>(fakeSubSubEntity, this.entityMapper);
            repositoryAdapter.StubForExistingItem<SubRow>(fakeSubEntity, this.entityMapper);
            repositoryAdapter.StubForExistingItem<MultiReferenceRow, int>(
                originalCreatedBy,
                this.entityMapper,
                item => item.FakeMultiReferenceEntityId,
                originalCreatedBy.FakeMultiReferenceEntityId.GetValueOrDefault());

            repositoryAdapter.StubForExistingItem<MultiReferenceRow, int>(
                modifiedBy,
                this.entityMapper,
                item => item.FakeMultiReferenceEntityId,
                modifiedBy.FakeMultiReferenceEntityId.GetValueOrDefault());

            repositoryAdapter.StubForExistingItem<ComplexRaisedRow>(baseline, this.entityMapper);
            repositoryAdapter.StubForExistingItem<DependentRow>(dependentEntity, this.entityMapper);

            using (var provider = RepositoryMockFactory.CreateConcreteProvider<FakeDataContext>(this.entityMapper, repositoryAdapter))
            {
                var newModifiedBy = new ModifiedBy("ModifiedBy", 433)
                                        {
                                            Description = "UpdatedModifiedBy"
                                        };

                var target = new EntityRepository<ComplexEntity, ComplexRaisedRow>(provider, this.entityMapper);
                var expected = target.FirstOrDefault(22);
                expected.Description = "UpdatedEntity";
                expected.ModifiedBy = newModifiedBy;
                expected.ModifiedTime = DateTimeOffset.Now.AddHours(1);
                expected.SubEntity.Description = "ModifiedSub";
                expected.SubSubEntity.Description = "ModifiedSubSub";
                expected.SetDependentEntity(0);

                var actual = target.Save(expected);
                Assert.IsNotNull(actual.SubEntity);
                Assert.IsNotNull(actual.SubEntity.SubSubEntity);
                Assert.IsNotNull(actual.CreatedBy);
                Assert.IsNotNull(actual.ModifiedBy);
                Assert.IsNull(actual.DependentEntity);
                Assert.AreEqual(16, actual.SubEntity.FakeSubEntityId);
                Assert.AreEqual(expected.SubEntity, actual.SubEntity);
                Assert.AreEqual(45, actual.SubSubEntity.FakeSubSubEntityId);
                Assert.AreEqual(expected.SubEntity.SubSubEntity, actual.SubEntity.SubSubEntity);
                Assert.AreEqual(432, actual.CreatedBy.FakeMultiReferenceEntityId);
                Assert.AreEqual(
                    baseline.CreatedBy,
                    actual.CreatedBy,
                    string.Join(Environment.NewLine, baseline.CreatedBy.GetDifferences(actual.CreatedBy)));

                Assert.AreEqual(creationTime, actual.CreationTime);
                Assert.AreEqual(433, actual.ModifiedBy.FakeMultiReferenceEntityId);
                Assert.AreEqual(expected.ModifiedBy, actual.ModifiedBy);
                Assert.AreEqual(expected.ModifiedTime, actual.ModifiedTime);
                Assert.AreEqual(expected.DependentEntity, actual.DependentEntity);
                Assert.AreEqual(22, actual.FakeComplexEntityId);
                Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
            }
        }

        /////// <summary>
        /////// The save with children_ new fake complex entity_ matches expected.
        /////// </summary>
        ////[TestMethod]
        ////public void SaveWithChildren_NewFakeComplexEntity_MatchesExpected()
        ////{
        ////    var repositoryAdapterFactory = CreateInsertRepositoryAdapterFactory();

        ////    using (var provider = new DatabaseRepositoryProvider(
        ////        GenericDatabaseFactory<FakeDataContext>.Default,
        ////        this.entityMapper,
        ////        repositoryAdapterFactory))
        ////    {
        ////        var target = new FakeComplexEntityRepository(provider, this.entityMapper);
        ////        var subSubEntity = new SubSubEntity("SubSubUniqueName");
        ////        var subEntity = new SubEntity("SubUniqueName", 234, subSubEntity);
        ////        var fakeCreatedBy = new CreatedBy("CreateUniqueName");
        ////        var modifiedBy = new ModifiedBy("ModifiedBy");
        ////        var expected = new ComplexEntity("UniqueName", subEntity, FakeEnumeration.FirstFake, fakeCreatedBy)
        ////                           {
        ////                               ModifiedBy = modifiedBy,
        ////                               ModifiedTime = DateTimeOffset.Now.AddHours(1)
        ////                           };

        ////        var fakeChild1 = new ChildEntity(expected)
        ////                             {
        ////                                 SomeValue = 100,
        ////                                 Name = "Parent1"
        ////                             };
        ////        var fakeChild2 = new ChildEntity(expected)
        ////                             {
        ////                                 SomeValue = 200,
        ////                                 Name = "Parent2"
        ////                             };
        ////        var fakeChild3 = new ChildEntity(expected)
        ////                             {
        ////                                 SomeValue = 110,
        ////                                 Name = "Child1",
        ////                                 Parent = fakeChild1
        ////                             };
        ////        var fakeChild4 = new ChildEntity(expected)
        ////                             {
        ////                                 SomeValue = 120,
        ////                                 Name = "Child2",
        ////                                 Parent = fakeChild1
        ////                             };
        ////        var fakeChild5 = new ChildEntity(expected)
        ////                             {
        ////                                 SomeValue = 210,
        ////                                 Name = "Child3",
        ////                                 Parent = fakeChild2
        ////                             };

        ////        var fakeChildren = new List<ChildEntity>
        ////                               {
        ////                                   fakeChild1,
        ////                                   fakeChild2,
        ////                                   fakeChild3,
        ////                                   fakeChild4,
        ////                                   fakeChild5
        ////                               };
        ////        expected.Load(fakeChildren);

        ////        var actual = target.SaveWithChildren(expected);
        ////        Assert.IsNotNull(actual.SubEntity);
        ////        Assert.IsNotNull(actual.SubEntity.SubSubEntity);
        ////        Assert.IsNotNull(actual.CreatedBy);
        ////        Assert.IsNotNull(actual.ModifiedBy);
        ////        Assert.AreEqual(16, actual.SubEntity.FakeSubEntityId);
        ////        Assert.AreEqual(expected.SubEntity, actual.SubEntity);
        ////        Assert.AreEqual(45, actual.SubSubEntity.FakeSubSubEntityId);
        ////        Assert.AreEqual(expected.SubEntity.SubSubEntity, actual.SubEntity.SubSubEntity);
        ////        Assert.AreEqual(432, actual.CreatedBy.FakeMultiReferenceEntityId);
        ////        Assert.AreEqual(expected.CreatedBy, actual.CreatedBy);
        ////        Assert.AreEqual(432, actual.ModifiedBy.FakeMultiReferenceEntityId);
        ////        Assert.AreEqual(expected.ModifiedBy, actual.ModifiedBy);
        ////        Assert.AreEqual(22, actual.FakeComplexEntityId);
        ////        Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
        ////        CollectionAssert.AreEqual(fakeChildren, actual.ChildEntities.ToList());

        ////        foreach (var childEntity in actual.ChildEntities)
        ////        {
        ////            switch (childEntity.SomeValue)
        ////            {
        ////                case 100:
        ////                    Assert.AreEqual(1, childEntity.FakeChildEntityId);
        ////                    break;

        ////                case 110:
        ////                    Assert.AreEqual(11, childEntity.FakeChildEntityId);
        ////                    break;

        ////                case 120:
        ////                    Assert.AreEqual(12, childEntity.FakeChildEntityId);
        ////                    break;

        ////                case 200:
        ////                    Assert.AreEqual(2, childEntity.FakeChildEntityId);
        ////                    break;

        ////                case 210:
        ////                    Assert.AreEqual(21, childEntity.FakeChildEntityId);
        ////                    break;
        ////            }
        ////        }
        ////    }
        ////}

        /// <summary>
        /// The save test.
        /// </summary>
        [TestMethod]
        public void Select_FirstOrDefaultFakeComplexEntity_MatchesExpected()
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
            var expected = new ComplexEntity("UniqueName1", fakeSubEntity, FakeEnumeration.FirstFake, originalCreatedBy, creationTime, 22)
                               {
                                   Description = "OriginalComplexEntity1",
                                   ModifiedBy = modifiedBy,
                                   ModifiedTime = DateTimeOffset.Now.AddHours(1)
                               };

            var existing = this.entityMapper.Map<ComplexRaisedRow>(expected);

            var repositoryAdapter = MockRepository.GenerateMock<IRepositoryAdapter>();
            repositoryAdapter.Stub(adapter => adapter.FirstOrDefault(Arg<ItemSelection<ComplexRaisedRow>>.Is.Anything)).Return(existing);

            var repositoryAdapterFactory = MockRepository.GenerateMock<IRepositoryAdapterFactory>();
            repositoryAdapterFactory.Stub(factory => factory.Create(Arg<Database>.Is.Anything)).Return(repositoryAdapter);

            using (var provider = new DatabaseRepositoryProvider(
                GenericDatabaseFactory<FakeDataContext>.Default,
                this.entityMapper,
                repositoryAdapterFactory))
            {
                var target = new EntityRepository<ComplexEntity, ComplexRaisedRow>(provider, this.entityMapper);
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

                Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
            }
        }

        /////// <summary>
        /////// The save test.
        /////// </summary>
        ////[TestMethod]
        ////public void Select_ExampleFakeComplexEntity_MatchesExpected()
        ////{
        ////    var subSubEntity = new SubSubEntity("SubSubUniqueName1", 45) { Description = "OriginalSubSub" };
        ////    var subEntity = new SubEntity("SubUniqueName1", 234, subSubEntity, 16) { Description = "OriginalSub" };
        ////    var originalCreatedBy = new CreatedBy("CreateUniqueName", 432) { Description = "OriginalCreatedBy" };
        ////    var modifiedBy = new ModifiedBy("ModifiedBy1", 433) { Description = "OriginalModifiedBy1" };
        ////    var creationTime = DateTimeOffset.Now.AddDays(-1);
        ////    var match1 = new ComplexEntity("UniqueName1", subEntity, FakeEnumeration.FirstFake, originalCreatedBy, creationTime, 22)
        ////    {
        ////        Description = "OriginalComplexEntity1", 
        ////        ModifiedBy = modifiedBy, 
        ////        ModifiedTime = DateTimeOffset.Now.AddHours(1)
        ////    };

        ////    var updatedSubSubEntity = new SubSubEntity("SubSubUniqueName2", 46) { Description = "ModifiedSubSub2" };
        ////    var updatedSubEntity = new SubEntity("SubUniqueName2", 235, updatedSubSubEntity, 17) { Description = "ModifiedSub2" };
        ////    var updatedMultiReferenceEntity = new CreatedBy("CreateUniqueName", 432) { Description = "OriginalCreatedBy" };
        ////    var newModifiedBy = new ModifiedBy("ModifiedBy2", 434) { Description = "UpdatedModifiedBy2" };
        ////    var match2 = new ComplexEntity("UniqueName2", updatedSubEntity, FakeEnumeration.SecondFake,  updatedMultiReferenceEntity)
        ////    {
        ////        Description = "UpdatedEntity2", 
        ////        ModifiedBy = newModifiedBy, 
        ////        ModifiedTime = DateTimeOffset.Now.AddHours(1)
        ////    };

        ////    var entities = new List<ComplexEntity> { match1, match2 };
        ////    var matches = entities.SqlSelect(this.entityMapper.Map<ComplexRaisedRow>).ToList();

        ////    var repositoryAdapter = MockRepository.GenerateMock<IRepositoryAdapter>();
        ////    repositoryAdapter.Stub(adapter => adapter.SelectItems(Arg<ItemSelection<ComplexRaisedRow>>.Is.Anything)).Return(matches);

        ////    var repositoryAdapterFactory = MockRepository.GenerateMock<IRepositoryAdapterFactory>();
        ////    repositoryAdapterFactory.Stub(factory => factory.Create(Arg<Database>.Is.Anything)).Return(repositoryAdapter);

        ////    using (var provider = new DatabaseRepositoryProvider<FakeDataContext>(repositoryAdapterFactory, this.entityMapper))
        ////    {
        ////        var target = new FakeComplexEntityRepository(provider);
        ////        var actual = target.SelectEntities(new ExampleQuery<ComplexRaisedRow>(matches.First(), row => row.CreatedByUniqueName)).ToList();
        ////        CollectionAssert.AreEqual(entities, actual);
        ////    }
        ////}

        /////// <summary>
        /////// The save test.
        /////// </summary>
        ////[TestMethod]
        ////public void Select_RangeFakeComplexEntity_MatchesExpected()
        ////{
        ////    var subSubEntity = new SubSubEntity("SubSubUniqueName1", 45) { Description = "OriginalSubSub" };
        ////    var subEntity = new SubEntity("SubUniqueName1", 234, subSubEntity, 16) { Description = "OriginalSub" };
        ////    var originalCreatedBy = new CreatedBy("CreateUniqueName", 432) { Description = "OriginalCreatedBy" };
        ////    var modifiedBy = new ModifiedBy("ModifiedBy1", 433) { Description = "OriginalModifiedBy1" };
        ////    var creationTime = DateTimeOffset.Now.AddDays(-1);
        ////    var match1 = new ComplexEntity("UniqueName1", subEntity, FakeEnumeration.FirstFake, originalCreatedBy, creationTime, 22)
        ////    {
        ////        Description = "OriginalComplexEntity1", 
        ////        ModifiedBy = modifiedBy, 
        ////        ModifiedTime = DateTimeOffset.Now.AddHours(1)
        ////    };

        ////    var updatedSubSubEntity = new SubSubEntity("SubSubUniqueName2", 46) { Description = "ModifiedSubSub2" };
        ////    var updatedSubEntity = new SubEntity("SubUniqueName2", 235, updatedSubSubEntity, 17) { Description = "ModifiedSub2" };
        ////    var updatedMultiReferenceEntity = new CreatedBy("CreateUniqueName", 432) { Description = "OriginalCreatedBy" };
        ////    var newModifiedBy = new ModifiedBy("ModifiedBy2", 434) { Description = "UpdatedModifiedBy2" };
        ////    var match2 = new ComplexEntity("UniqueName2", updatedSubEntity, FakeEnumeration.SecondFake, updatedMultiReferenceEntity)
        ////    {
        ////        Description = "UpdatedEntity2", 
        ////        ModifiedBy = newModifiedBy, 
        ////        ModifiedTime = DateTimeOffset.Now.AddHours(1)
        ////    };

        ////    var entities = new List<ComplexEntity> { match1, match2 };
        ////    var matches = entities.SqlSelect(this.entityMapper.Map<ComplexRaisedRow>).ToList();

        ////    var repositoryAdapter = MockRepository.GenerateMock<IRepositoryAdapter>();
        ////    repositoryAdapter.Stub(adapter => adapter.SelectItems(Arg<ItemSelection<ComplexRaisedRow>>.Is.Anything)).Return(matches);

        ////    var repositoryAdapterFactory = MockRepository.GenerateMock<IRepositoryAdapterFactory>();
        ////    repositoryAdapterFactory.Stub(factory => factory.Create(Arg<Database>.Is.Anything)).Return(repositoryAdapter);

        ////    using (var provider = new DatabaseRepositoryProvider<FakeDataContext>(repositoryAdapterFactory, this.entityMapper))
        ////    {
        ////        var target = new FakeComplexEntityRepository(provider);
        ////        var exampleQuery = new ExampleQuery<ComplexRaisedRow>(matches.First(), matches.Last(), row => row.ModifiedTime);
        ////        var actual = target.SelectEntities(exampleQuery).ToList();
        ////        var expectedFirst = entities.FirstOrDefault();
        ////        var actualFirst = actual.FirstOrDefault();
        ////        Assert.AreEqual(expectedFirst, actualFirst, string.Join(Environment.NewLine, expectedFirst.GetDifferences(actualFirst)));
        ////        CollectionAssert.AreEqual(entities, actual);
        ////    }
        ////}

        /// <summary>
        /// The save test.
        /// </summary>
        [TestMethod]
        public void Save_NewFakeChildEntity_MatchesExpected()
        {
            var repositoryAdapterFactory = CreateInsertRepositoryAdapterFactory();

            using (var provider = new DatabaseRepositoryProvider(
                GenericDatabaseFactory<FakeDataContext>.Default,
                this.entityMapper,
                repositoryAdapterFactory))
            {
                var target = new EntityRepository<ChildEntity, ChildRaisedRow>(provider, this.entityMapper);
                var fakeSubSubEntity = new SubSubEntity("SubSubUniqueName");
                var fakeSubEntity = new SubEntity("SubUniqueName", 234, fakeSubSubEntity);
                var fakeCreatedBy = new CreatedBy("CreateUniqueName");
                var modifiedBy = new ModifiedBy("ModifiedBy");
                var fakeComplexEntity = new ComplexEntity("UniqueName", fakeSubEntity, FakeEnumeration.FirstFake, fakeCreatedBy)
                                            {
                                                ModifiedBy = modifiedBy,
                                                ModifiedTime = DateTimeOffset.Now.AddHours(1)
                                            };

                var expected = new ChildEntity(fakeComplexEntity)
                                   {
                                       Name = "Foo",
                                       SomeValue = 4492
                                   };

                // Save this first because child doesn't save its parent.
                var fakeComplexRepo = new EntityRepository<ComplexEntity, ComplexRaisedRow>(provider, this.entityMapper);
                fakeComplexRepo.Save(fakeComplexEntity);

                var actual = target.Save(expected);
                Assert.IsNotNull(actual.SubEntity);
                Assert.IsNotNull(actual.SubEntity.SubSubEntity);
                Assert.IsNotNull(actual.CreatedBy);
                Assert.IsNotNull(actual.ModifiedBy);
                Assert.AreEqual(16, actual.SubEntity.FakeSubEntityId);
                Assert.AreEqual(expected.SubEntity, actual.SubEntity);
                Assert.AreEqual(45, actual.SubSubEntity.FakeSubSubEntityId);
                Assert.AreEqual(expected.SubEntity.SubSubEntity, actual.SubEntity.SubSubEntity);
                Assert.AreEqual(432, actual.CreatedBy.FakeMultiReferenceEntityId);
                Assert.AreEqual(expected.CreatedBy, actual.CreatedBy);
                Assert.AreEqual(432, actual.ModifiedBy.FakeMultiReferenceEntityId);
                Assert.AreEqual(expected.ModifiedBy, actual.ModifiedBy);
                Assert.AreEqual(22, actual.FakeComplexEntityId);
                Assert.AreEqual(expected.ComplexEntity, actual.ComplexEntity);
                Assert.AreEqual(235, actual.FakeChildEntityId);
                Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
            }
        }

        /// <summary>
        /// The save test.
        /// </summary>
        [TestMethod]
        public void Save_NewFakeChildEntityWithDependentEntity_MatchesExpected()
        {
            var repositoryAdapterFactory = CreateInsertRepositoryAdapterFactory();

            using (var provider = new DatabaseRepositoryProvider(
                GenericDatabaseFactory<FakeDataContext>.Default,
                this.entityMapper,
                repositoryAdapterFactory))
            {
                var target = new EntityRepository<ChildEntity, ChildRaisedRow>(provider, this.entityMapper);
                var fakeSubSubEntity = new SubSubEntity("SubSubUniqueName")
                                           {
                                               Description = "Mah sub sub entity"
                                           };
                var fakeSubEntity = new SubEntity("SubUniqueName", 234, fakeSubSubEntity)
                                        {
                                            Description = "Mah sub entity"
                                        };
                var fakeCreatedBy = new CreatedBy("CreateUniqueName")
                                        {
                                            Description = "Creator"
                                        };
                var modifiedBy = new ModifiedBy("ModifiedBy")
                                     {
                                         Description = "Modifier"
                                     };
                var fakeComplexEntity = new ComplexEntity("UniqueName", fakeSubEntity, FakeEnumeration.FirstFake, fakeCreatedBy)
                                            {
                                                ModifiedBy = modifiedBy,
                                                ModifiedTime = DateTimeOffset.Now.AddHours(1)
                                            };

                var expected = new ChildEntity(fakeComplexEntity)
                                   {
                                       Name = "Foo",
                                       SomeValue = 4492
                                   };
                expected.ComplexEntity.SetDependentEntity(33, DateTimeOffset.Now);

                // Save this first because child doesn't save its parent.
                var fakeComplexRepo = new EntityRepository<ComplexEntity, ComplexRaisedRow>(provider, this.entityMapper);
                fakeComplexRepo.Save(fakeComplexEntity);

                var actual = target.Save(expected);
                Assert.IsNotNull(actual.SubEntity);
                Assert.IsNotNull(actual.SubEntity.SubSubEntity);
                Assert.IsNotNull(actual.CreatedBy);
                Assert.IsNotNull(actual.ModifiedBy);
                Assert.IsNotNull(actual.DependentEntity);
                Assert.AreEqual(16, actual.SubEntity.FakeSubEntityId);
                Assert.AreEqual(expected.SubEntity, actual.SubEntity);
                Assert.AreEqual(45, actual.SubSubEntity.FakeSubSubEntityId);
                Assert.AreEqual(expected.SubEntity.SubSubEntity, actual.SubEntity.SubSubEntity);
                Assert.AreEqual(432, actual.CreatedBy.FakeMultiReferenceEntityId);
                Assert.AreEqual(expected.CreatedBy, actual.CreatedBy);
                Assert.AreEqual(432, actual.ModifiedBy.FakeMultiReferenceEntityId);
                Assert.AreEqual(expected.ModifiedBy, actual.ModifiedBy);
                Assert.AreEqual(22, actual.FakeComplexEntityId);
                Assert.AreEqual(expected.ComplexEntity, actual.ComplexEntity);
                Assert.AreEqual(22, actual.FakeDependentEntityId);
                Assert.AreEqual(expected.DependentEntity, actual.DependentEntity);
                Assert.AreEqual(235, actual.FakeChildEntityId);
                Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
            }
        }

        /// <summary>
        /// The save test.
        /// </summary>
        [TestMethod]
        public void Save_UpdatedFakeChildEntity_MatchesExpected()
        {
            var fakeSubSubEntity = new SubSubEntity("SubSubUniqueName", 45)
                                       {
                                           Description = "OriginalSubSub"
                                       };
            var fakeSubEntity = new SubEntity("SubUniqueName", 234, fakeSubSubEntity, 16)
                                    {
                                        Description = "OriginalSub"
                                    };
            var originalCreatedBy = new CreatedBy("CreateUniqueName", 432)
                                        {
                                            Description = "OriginalCreatedBy"
                                        };
            var modifiedBy = new ModifiedBy("ModifiedBy", 433)
                                 {
                                     Description = "OriginalModifiedBy"
                                 };
            var creationTime = DateTimeOffset.Now.AddDays(-1);
            var complexEntity = new ComplexEntity("UniqueName", fakeSubEntity, FakeEnumeration.FirstFake, originalCreatedBy, creationTime, 22)
                                    {
                                        Description = "OriginalComplexEntity",
                                        ModifiedBy = modifiedBy,
                                        ModifiedTime = DateTimeOffset.Now.AddHours(1)
                                    };

            var parent = new ChildEntity(complexEntity, 335)
                             {
                                 Name = "ParentName",
                                 SomeValue = 1
                             };
            var baseline = new ChildEntity(complexEntity, 235)
                               {
                                   Name = "OriginalName",
                                   SomeValue = 2,
                                   Parent = parent
                               };

            var mockFactory = this.CreateComplexMockAdapterFactoryForUpdate(baseline);

            using (var provider = new DatabaseRepositoryProvider(
                GenericDatabaseFactory<FakeDataContext>.Default,
                this.entityMapper,
                mockFactory.RepositoryAdapterFactory))
            {
                var newModifiedBy = new ModifiedBy("ModifiedBy", 433)
                                        {
                                            Description = "UpdatedModifiedBy"
                                        };

                var target = new EntityRepository<ChildEntity, ChildRaisedRow>(provider, this.entityMapper);
                var expected = target.FirstOrDefault(22);
                expected.Name = "NewName";
                expected.SomeValue = 242;
                expected.ComplexEntity.Description = "UpdatedEntity";
                expected.ComplexEntity.ModifiedBy = newModifiedBy;
                expected.ComplexEntity.ModifiedTime = DateTimeOffset.Now.AddHours(1);
                expected.SubEntity.Description = "ModifiedSub";
                expected.SubSubEntity.Description = "ModifiedSubSub";
                var actual = target.Save(expected);
                Assert.IsNotNull(actual.SubEntity);
                Assert.IsNotNull(actual.SubEntity.SubSubEntity);
                Assert.IsNotNull(actual.CreatedBy);
                Assert.IsNotNull(actual.ModifiedBy);
                Assert.AreEqual(16, actual.SubEntity.FakeSubEntityId);
                Assert.AreEqual(expected.SubEntity, actual.SubEntity);
                Assert.AreEqual(45, actual.SubSubEntity.FakeSubSubEntityId);
                Assert.AreEqual(expected.SubEntity.SubSubEntity, actual.SubEntity.SubSubEntity);
                Assert.AreEqual(432, actual.CreatedBy.FakeMultiReferenceEntityId);

                Assert.AreEqual(
                    baseline.CreatedBy,
                    actual.CreatedBy,
                    string.Join(Environment.NewLine, baseline.CreatedBy.GetDifferences(actual.CreatedBy)));

                Assert.AreEqual(creationTime, actual.ComplexEntity.CreationTime);
                Assert.AreEqual(433, actual.ModifiedBy.FakeMultiReferenceEntityId);
                Assert.AreEqual(expected.ModifiedBy, actual.ModifiedBy);
                Assert.AreEqual(expected.ComplexEntity.ModifiedTime, actual.ComplexEntity.ModifiedTime);
                Assert.AreEqual(22, actual.FakeComplexEntityId);
                Assert.AreEqual(expected.ComplexEntity, actual.ComplexEntity);
                Assert.AreEqual(235, actual.FakeChildEntityId);
                Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
            }
        }

        /// <summary>
        /// The save test.
        /// </summary>
        [TestMethod]
        public void Save_UpdatedFakeChildEntityWithNewDependentEntity_MatchesExpected()
        {
            var fakeSubSubEntity = new SubSubEntity("SubSubUniqueName", 45)
                                       {
                                           Description = "OriginalSubSub"
                                       };
            var fakeSubEntity = new SubEntity("SubUniqueName", 234, fakeSubSubEntity, 16)
                                    {
                                        Description = "OriginalSub"
                                    };
            var originalCreatedBy = new CreatedBy("CreateUniqueName", 432)
                                        {
                                            Description = "OriginalCreatedBy"
                                        };
            var modifiedBy = new ModifiedBy("ModifiedBy", 433)
                                 {
                                     Description = "OriginalModifiedBy"
                                 };
            var creationTime = DateTimeOffset.Now.AddDays(-1);
            var complexEntity = new ComplexEntity("UniqueName", fakeSubEntity, FakeEnumeration.FirstFake, originalCreatedBy, creationTime, 22)
                                    {
                                        Description = "OriginalComplexEntity",
                                        ModifiedBy = modifiedBy,
                                        ModifiedTime = DateTimeOffset.Now.AddHours(1)
                                    };

            var parent = new ChildEntity(complexEntity, 335)
                             {
                                 Name = "ParentName",
                                 SomeValue = 1
                             };
            var baseline = new ChildEntity(complexEntity, 235)
                               {
                                   Name = "OriginalName",
                                   SomeValue = 2,
                                   Parent = parent
                               };

            var mockFactory = this.CreateComplexMockAdapterFactoryForUpdate(baseline);

            using (var provider = new DatabaseRepositoryProvider(
                GenericDatabaseFactory<FakeDataContext>.Default,
                this.entityMapper,
                mockFactory.RepositoryAdapterFactory))
            {
                var newModifiedBy = new ModifiedBy("ModifiedBy", 433)
                                        {
                                            Description = "UpdatedModifiedBy"
                                        };

                var target = new EntityRepository<ChildEntity, ChildRaisedRow>(provider, this.entityMapper);
                var expected = target.FirstOrDefault(22);
                expected.Name = "NewName";
                expected.SomeValue = 242;
                expected.ComplexEntity.Description = "UpdatedEntity";
                expected.ComplexEntity.ModifiedBy = newModifiedBy;
                expected.ComplexEntity.ModifiedTime = DateTimeOffset.Now.AddHours(1);
                expected.SubEntity.Description = "ModifiedSub";
                expected.SubSubEntity.Description = "ModifiedSubSub";
                expected.ComplexEntity.SetDependentEntity(33, DateTimeOffset.Now);

                var fakeDependentRow = this.entityMapper.Map<DependentRow>(expected.DependentEntity);
                fakeDependentRow.FakeDependentEntityId = expected.FakeComplexEntityId.GetValueOrDefault();
                mockFactory.RepositoryAdapter.Stub(adapter => adapter.Insert(Arg<DependentRow>.Is.Anything)).Return(fakeDependentRow);

                var actual = target.Save(expected);
                Assert.IsNotNull(actual.SubEntity);
                Assert.IsNotNull(actual.SubEntity.SubSubEntity);
                Assert.IsNotNull(actual.CreatedBy);
                Assert.IsNotNull(actual.ModifiedBy);
                Assert.IsNotNull(actual.DependentEntity);
                Assert.AreEqual(16, actual.SubEntity.FakeSubEntityId);
                Assert.AreEqual(expected.SubEntity, actual.SubEntity);
                Assert.AreEqual(45, actual.SubSubEntity.FakeSubSubEntityId);
                Assert.AreEqual(expected.SubEntity.SubSubEntity, actual.SubEntity.SubSubEntity);
                Assert.AreEqual(432, actual.CreatedBy.FakeMultiReferenceEntityId);

                Assert.AreEqual(
                    baseline.CreatedBy,
                    actual.CreatedBy,
                    string.Join(Environment.NewLine, baseline.CreatedBy.GetDifferences(actual.CreatedBy)));

                Assert.AreEqual(creationTime, actual.ComplexEntity.CreationTime);
                Assert.AreEqual(433, actual.ModifiedBy.FakeMultiReferenceEntityId);
                Assert.AreEqual(expected.ModifiedBy, actual.ModifiedBy);
                Assert.AreEqual(expected.ComplexEntity.ModifiedTime, actual.ComplexEntity.ModifiedTime);
                Assert.AreEqual(22, actual.FakeComplexEntityId);
                Assert.AreEqual(expected.ComplexEntity, actual.ComplexEntity);
                Assert.AreEqual(22, actual.FakeDependentEntityId);
                Assert.AreEqual(expected.DependentEntity, actual.DependentEntity);
                Assert.AreEqual(235, actual.FakeChildEntityId);
                Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
            }
        }

        /// <summary>
        /// The save test.
        /// </summary>
        [TestMethod]
        public void Save_UpdatedFakeChildEntityWithUpdatedDependentEntity_MatchesExpected()
        {
            var fakeSubSubEntity = new SubSubEntity("SubSubUniqueName", 45)
                                       {
                                           Description = "OriginalSubSub"
                                       };
            var fakeSubEntity = new SubEntity("SubUniqueName", 234, fakeSubSubEntity, 16)
                                    {
                                        Description = "OriginalSub"
                                    };
            var originalCreatedBy = new CreatedBy("CreateUniqueName", 432)
                                        {
                                            Description = "OriginalCreatedBy"
                                        };
            var modifiedBy = new ModifiedBy("ModifiedBy", 433)
                                 {
                                     Description = "OriginalModifiedBy"
                                 };
            var creationTime = DateTimeOffset.Now.AddDays(-1);
            var complexEntity = new ComplexEntity("UniqueName", fakeSubEntity, FakeEnumeration.FirstFake, originalCreatedBy, creationTime, 22)
                                    {
                                        Description = "OriginalComplexEntity",
                                        ModifiedBy = modifiedBy,
                                        ModifiedTime = DateTimeOffset.Now.AddHours(1)
                                    };

            complexEntity.SetDependentEntity(9843, DateTimeOffset.Now.AddHours(-3));

            var parent = new ChildEntity(complexEntity, 335)
                             {
                                 Name = "ParentName",
                                 SomeValue = 1
                             };
            var baseline = new ChildEntity(complexEntity, 235)
                               {
                                   Name = "OriginalName",
                                   SomeValue = 2,
                                   Parent = parent
                               };

            var mockFactory = this.CreateComplexMockAdapterFactoryForUpdate(baseline);
            mockFactory.RepositoryAdapter.Stub(
                    adapter => adapter.Update(
                        Arg<DependentRow>.Is.Anything,
                        Arg<ItemSelection<DependentRow>>.Is.Anything,
                        Arg<Expression<Func<DependentRow, object>>[]>.Is.Anything))
                .Return(1);

            using (var provider = new DatabaseRepositoryProvider(
                GenericDatabaseFactory<FakeDataContext>.Default,
                this.entityMapper,
                mockFactory.RepositoryAdapterFactory))
            {
                var newModifiedBy = new ModifiedBy("ModifiedBy", 433)
                                        {
                                            Description = "UpdatedModifiedBy"
                                        };

                var target = new EntityRepository<ChildEntity, ChildRaisedRow>(provider, this.entityMapper);
                var expected = target.FirstOrDefault(22);
                expected.Name = "NewName";
                expected.SomeValue = 242;
                expected.ComplexEntity.Description = "UpdatedEntity";
                expected.ComplexEntity.ModifiedBy = newModifiedBy;
                expected.ComplexEntity.ModifiedTime = DateTimeOffset.Now.AddHours(1);
                expected.SubEntity.Description = "ModifiedSub";
                expected.SubSubEntity.Description = "ModifiedSubSub";
                expected.ComplexEntity.SetDependentEntity(33, DateTimeOffset.Now);

                var fakeDependentRow = this.entityMapper.Map<DependentRow>(expected.DependentEntity);
                fakeDependentRow.FakeDependentEntityId = expected.FakeComplexEntityId.GetValueOrDefault();
                mockFactory.RepositoryAdapter.Stub(adapter => adapter.FirstOrDefault(Arg<ItemSelection<DependentRow>>.Is.Anything))
                    .Return(fakeDependentRow);

                var actual = target.Save(expected);
                Assert.IsNotNull(actual.SubEntity);
                Assert.IsNotNull(actual.SubEntity.SubSubEntity);
                Assert.IsNotNull(actual.CreatedBy);
                Assert.IsNotNull(actual.ModifiedBy);
                Assert.IsNotNull(actual.DependentEntity);
                Assert.AreEqual(16, actual.SubEntity.FakeSubEntityId);
                Assert.AreEqual(expected.SubEntity, actual.SubEntity);
                Assert.AreEqual(45, actual.SubSubEntity.FakeSubSubEntityId);
                Assert.AreEqual(expected.SubEntity.SubSubEntity, actual.SubEntity.SubSubEntity);
                Assert.AreEqual(432, actual.CreatedBy.FakeMultiReferenceEntityId);

                Assert.AreEqual(
                    baseline.CreatedBy,
                    actual.CreatedBy,
                    string.Join(Environment.NewLine, baseline.CreatedBy.GetDifferences(actual.CreatedBy)));

                Assert.AreEqual(creationTime, actual.ComplexEntity.CreationTime);
                Assert.AreEqual(433, actual.ModifiedBy.FakeMultiReferenceEntityId);
                Assert.AreEqual(expected.ModifiedBy, actual.ModifiedBy);
                Assert.AreEqual(expected.ComplexEntity.ModifiedTime, actual.ComplexEntity.ModifiedTime);
                Assert.AreEqual(22, actual.FakeComplexEntityId);
                Assert.AreEqual(expected.ComplexEntity, actual.ComplexEntity);
                Assert.AreEqual(22, actual.FakeDependentEntityId);
                Assert.AreEqual(expected.DependentEntity, actual.DependentEntity);
                Assert.AreEqual(235, actual.FakeChildEntityId);
                Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
            }
        }

        /// <summary>
        /// The save test.
        /// </summary>
        [TestMethod]
        public void Save_UpdatedFakeChildEntityWithRemovedDependentEntity_MatchesExpected()
        {
            var fakeSubSubEntity = new SubSubEntity("SubSubUniqueName", 45)
                                       {
                                           Description = "OriginalSubSub"
                                       };
            var fakeSubEntity = new SubEntity("SubUniqueName", 234, fakeSubSubEntity, 16)
                                    {
                                        Description = "OriginalSub"
                                    };
            var originalCreatedBy = new CreatedBy("CreateUniqueName", 432)
                                        {
                                            Description = "OriginalCreatedBy"
                                        };
            var modifiedBy = new ModifiedBy("ModifiedBy", 433)
                                 {
                                     Description = "OriginalModifiedBy"
                                 };
            var creationTime = DateTimeOffset.Now.AddDays(-1);
            var complexEntity = new ComplexEntity("UniqueName", fakeSubEntity, FakeEnumeration.FirstFake, originalCreatedBy, creationTime, 22)
                                    {
                                        Description = "OriginalComplexEntity",
                                        ModifiedBy = modifiedBy,
                                        ModifiedTime = DateTimeOffset.Now.AddHours(1)
                                    };

            complexEntity.SetDependentEntity(22, DateTimeOffset.Now.AddHours(-3));

            var parent = new ChildEntity(complexEntity, 335)
                             {
                                 Name = "ParentName",
                                 SomeValue = 1
                             };
            var baseline = new ChildEntity(complexEntity, 235)
                               {
                                   Name = "OriginalName",
                                   SomeValue = 2,
                                   Parent = parent
                               };

            var mockFactory = this.CreateComplexMockAdapterFactoryForUpdate(baseline);
            mockFactory.RepositoryAdapter.Stub(adapter => adapter.DeleteSelection(Arg<ItemSelection<DependentRow>>.Is.Anything)).Return(1);

            using (var provider = new DatabaseRepositoryProvider(
                GenericDatabaseFactory<FakeDataContext>.Default,
                this.entityMapper,
                mockFactory.RepositoryAdapterFactory))
            {
                var newModifiedBy = new ModifiedBy("ModifiedBy", 433)
                                        {
                                            Description = "UpdatedModifiedBy"
                                        };

                var target = new EntityRepository<ChildEntity, ChildRaisedRow>(provider, this.entityMapper);
                var expected = target.FirstOrDefault(22);
                expected.Name = "NewName";
                expected.SomeValue = 242;
                expected.ComplexEntity.Description = "UpdatedEntity";
                expected.ComplexEntity.ModifiedBy = newModifiedBy;
                expected.ComplexEntity.ModifiedTime = DateTimeOffset.Now.AddHours(1);
                expected.SubEntity.Description = "ModifiedSub";
                expected.SubSubEntity.Description = "ModifiedSubSub";
                expected.ComplexEntity.SetDependentEntity(0);
                var actual = target.Save(expected);
                Assert.IsNotNull(actual.SubEntity);
                Assert.IsNotNull(actual.SubEntity.SubSubEntity);
                Assert.IsNotNull(actual.CreatedBy);
                Assert.IsNotNull(actual.ModifiedBy);
                Assert.IsNull(actual.DependentEntity);
                Assert.AreEqual(16, actual.SubEntity.FakeSubEntityId);
                Assert.AreEqual(expected.SubEntity, actual.SubEntity);
                Assert.AreEqual(45, actual.SubSubEntity.FakeSubSubEntityId);
                Assert.AreEqual(expected.SubEntity.SubSubEntity, actual.SubEntity.SubSubEntity);
                Assert.AreEqual(432, actual.CreatedBy.FakeMultiReferenceEntityId);

                Assert.AreEqual(
                    baseline.CreatedBy,
                    actual.CreatedBy,
                    string.Join(Environment.NewLine, baseline.CreatedBy.GetDifferences(actual.CreatedBy)));

                Assert.AreEqual(creationTime, actual.ComplexEntity.CreationTime);
                Assert.AreEqual(433, actual.ModifiedBy.FakeMultiReferenceEntityId);
                Assert.AreEqual(expected.ModifiedBy, actual.ModifiedBy);
                Assert.AreEqual(expected.ComplexEntity.ModifiedTime, actual.ComplexEntity.ModifiedTime);
                Assert.AreEqual(22, actual.FakeComplexEntityId);
                Assert.AreEqual(expected.ComplexEntity, actual.ComplexEntity);
                Assert.AreEqual(expected.DependentEntity, actual.DependentEntity);
                Assert.AreEqual(235, actual.FakeChildEntityId);
                Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
            }
        }

        /// <summary>
        /// Tests that the first or default of the repository matches the expected results.
        /// </summary>
        [TestMethod]
        public void Save_NewFakeRaisedSubEntity_MatchesExpected()
        {
            var fakeSubSubEntity = new SubSubEntity("jasdyri");
            var expected = new SubEntity("asidf", 58, fakeSubSubEntity);

            var repositoryAdapter = RepositoryMockFactory.CreateAdapter();
            using (var repositoryProvider = RepositoryMockFactory.CreateConcreteProvider<FakeDataContext>(this.entityMapper, repositoryAdapter))
            {
                repositoryAdapter.StubForNewItem<SubRow>(repositoryProvider.EntityDefinitionProvider);
                repositoryAdapter.StubForNewItem<SubSubRow>(repositoryProvider.EntityDefinitionProvider);

                var target = new EntityRepository<SubEntity, SubRow>(repositoryProvider, this.entityMapper);
                var actual = target.Save(expected);

                Assert.IsTrue(actual.FakeSubEntityId > 0);
                Assert.IsTrue(actual.FakeSubSubEntityId > 0);
                Assert.AreSame(expected, actual);
            }
        }

        /// <summary>
        /// Tests that the first or default of the repository matches the expected results.
        /// </summary>
        [TestMethod]
        public void Save_ExistingFakeRaisedSubEntity_MatchesExpected()
        {
            var fakeSubSubEntity = new SubSubEntity("jasdyri", 93475);
            var expected = new SubEntity("asidf", 58, fakeSubSubEntity, 874359);

            var repositoryAdapter = RepositoryMockFactory.CreateAdapter();
            repositoryAdapter.StubForExistingItem<SubRow>(expected, this.entityMapper);
            repositoryAdapter.StubForExistingItem<SubSubRow>(fakeSubSubEntity, this.entityMapper);

            using (var repositoryProvider = RepositoryMockFactory.CreateConcreteProvider<FakeDataContext>(this.entityMapper, repositoryAdapter))
            {
                var target = new EntityRepository<SubEntity, SubRow>(repositoryProvider, this.entityMapper);
                var actual = target.Save(expected);

                Assert.AreEqual(874359, actual.FakeSubEntityId);
                Assert.AreEqual(93475, actual.FakeSubSubEntityId);
                Assert.AreSame(expected, actual);
            }
        }

        /////// <summary>
        /////// The save test.
        /////// </summary>
        ////[TestMethod]
        ////public void Select_ExampleFakeChildEntity_MatchesExpected()
        ////{
        ////    var subSubEntity = new SubSubEntity("SubSubUniqueName1", 45) { Description = "OriginalSubSub" };
        ////    var subEntity = new SubEntity("SubUniqueName1", 234, subSubEntity, 16) { Description = "OriginalSub" };
        ////    var originalCreatedBy = new CreatedBy("CreateUniqueName", 432) { Description = "OriginalCreatedBy" };
        ////    var modifiedBy = new ModifiedBy("ModifiedBy1", 433) { Description = "OriginalModifiedBy1" };
        ////    var creationTime = DateTimeOffset.Now.AddDays(-1);
        ////    var complexEntity = new ComplexEntity("UniqueName1", subEntity, FakeEnumeration.FirstFake, originalCreatedBy, creationTime, 22)
        ////    {
        ////        Description = "OriginalComplexEntity1", 
        ////        ModifiedBy = modifiedBy, 
        ////        ModifiedTime = DateTimeOffset.Now.AddHours(1)
        ////    };

        ////    var match1 = new ChildEntity(complexEntity, 235) { Name = "OriginalName", SomeValue = 111 };
        ////    var match2 = new ChildEntity(complexEntity, 236) { Name = "AnotherName", SomeValue = 112, Parent = match1 };
        ////    var match3 = new ChildEntity(complexEntity, 237) { Name = "YetAnotherName", SomeValue = 113, Parent = match2 };

        ////    var entities = new List<ChildEntity> { match1, match2, match3 };
        ////    var matches = entities.SqlSelect(this.entityMapper.Map<ChildRaisedRow>).ToList();

        ////    var repositoryAdapter = MockRepository.GenerateMock<IRepositoryAdapter>();
        ////    repositoryAdapter.Stub(adapter => adapter.SelectItems(Arg<ItemSelection<ChildRaisedRow>>.Is.Anything)).Return(matches);

        ////    var repositoryAdapterFactory = MockRepository.GenerateMock<IRepositoryAdapterFactory>();
        ////    repositoryAdapterFactory.Stub(factory => factory.Create(Arg<Database>.Is.Anything)).Return(repositoryAdapter);

        ////    using (var provider = new DatabaseRepositoryProvider<FakeDataContext>(repositoryAdapterFactory, this.entityMapper))
        ////    {
        ////        var target = new EntityRepository<ChildEntity, ChildRaisedRow>(provider);
        ////        var actual = target.SelectEntities(new ExampleQuery<ChildRaisedRow>(matches.First(), row => row.CreatedByUniqueName)).ToList();
        ////        Assert.IsTrue(actual.Any());
        ////        Assert.AreEqual(
        ////            entities.Last(), 
        ////            actual.Last(), 
        ////            string.Join(Environment.NewLine, entities.Last().GetDifferences(actual.Last())));

        ////        CollectionAssert.AreEqual(entities, actual);

        ////        var firstComplexEntity = actual.First().ComplexEntity;

        ////        foreach (var childEntity in actual)
        ////        {
        ////            Assert.IsTrue(ReferenceEquals(firstComplexEntity, childEntity.ComplexEntity));
        ////        }
        ////    }
        ////}

        /////// <summary>
        /////// The save test.
        /////// </summary>
        ////[TestMethod]
        ////public void Select_RangeFakeChildEntity_MatchesExpected()
        ////{
        ////    var subSubEntity = new SubSubEntity("SubSubUniqueName1", 45) { Description = "OriginalSubSub" };
        ////    var subEntity = new SubEntity("SubUniqueName1", 234, subSubEntity, 16) { Description = "OriginalSub" };
        ////    var originalCreatedBy = new CreatedBy("CreateUniqueName", 432) { Description = "OriginalCreatedBy" };
        ////    var modifiedBy = new ModifiedBy("ModifiedBy1", 433) { Description = "OriginalModifiedBy1" };
        ////    var creationTime = DateTimeOffset.Now.AddDays(-1);
        ////    var complexEntity = new ComplexEntity("UniqueName1", subEntity, FakeEnumeration.FirstFake, originalCreatedBy, creationTime, 22)
        ////    {
        ////        Description = "OriginalComplexEntity1", 
        ////        ModifiedBy = modifiedBy, 
        ////        ModifiedTime = DateTimeOffset.Now.AddHours(1)
        ////    };

        ////    var match1 = new ChildEntity(complexEntity, 235) { Name = "OriginalName", SomeValue = 111 };
        ////    var match2 = new ChildEntity(complexEntity, 236) { Name = "AnotherName", SomeValue = 112, Parent = match1 };
        ////    var match3 = new ChildEntity(complexEntity, 237) { Name = "YetAnotherName", SomeValue = 113, Parent = match1 };

        ////    var entities = new List<ChildEntity> { match1, match2, match3 };
        ////    var matches = entities.SqlSelect(this.entityMapper.Map<ChildRaisedRow>).ToList();

        ////    var repositoryAdapter = MockRepository.GenerateMock<IRepositoryAdapter>();
        ////    repositoryAdapter.Stub(adapter => adapter.SelectItems(Arg<ItemSelection<ChildRaisedRow>>.Is.Anything)).Return(matches);

        ////    var repositoryAdapterFactory = MockRepository.GenerateMock<IRepositoryAdapterFactory>();
        ////    repositoryAdapterFactory.Stub(factory => factory.Create(Arg<Database>.Is.Anything)).Return(repositoryAdapter);

        ////    using (var provider = new DatabaseRepositoryProvider<FakeDataContext>(repositoryAdapterFactory, this.entityMapper))
        ////    {
        ////        var target = new EntityRepository<ChildEntity, ChildRaisedRow>(provider);
        ////        var exampleQuery = new ExampleQuery<ChildRaisedRow>(matches.First(), matches.Last(), row => row.FakeComplexEntityModifiedTime);
        ////        var actual = target.SelectEntities(exampleQuery).ToList();
        ////        CollectionAssert.AreEqual(entities, actual);

        ////        var firstComplexEntity = actual.First().ComplexEntity;

        ////        foreach (var childEntity in actual)
        ////        {
        ////            Assert.IsTrue(ReferenceEquals(firstComplexEntity, childEntity.ComplexEntity));
        ////        }
        ////    }
        ////}

        /////// <summary>
        /////// The load children_ fake child entities_ matches expected.
        /////// </summary>
        ////[TestMethod]
        ////public void FirstOrDefaultWithChildren_FakeChildEntities_MatchesExpected()
        ////{
        ////    var subSubEntity = new SubSubEntity("foobar", 112233)
        ////                               {
        ////                                   Description = "SubSubEntity"
        ////                               };
        ////    var subEntity = new SubEntity("bar", 949, subSubEntity, 4587)
        ////                            {
        ////                                Description = "SubEntity"
        ////                            };
        ////    var createdBy = new CreatedBy("createdBy", 49430)
        ////                        {
        ////                            Description = "The CREATOR"
        ////                        };
        ////    var modifiedBy = new ModifiedBy("Modifier", 999291);
        ////    var expected = new ComplexEntity("foo", subEntity, FakeEnumeration.ThirdFake, createdBy, DateTimeOffset.Now, 99291)
        ////                       {
        ////                           Description = "ComplexEntity",
        ////                           ModifiedBy = modifiedBy
        ////                       };

        ////    var parent1 = new ChildEntity(expected, 4994)
        ////                      {
        ////                          Name = "Parent1",
        ////                          SomeValue = 993
        ////                      };
        ////    var parent2 = new ChildEntity(expected, 5002)
        ////                      {
        ////                          Name = "Parent2",
        ////                          SomeValue = 5573
        ////                      };
        ////    var children = new List<ChildEntity>
        ////                       {
        ////                           parent1,
        ////                           parent2,
        ////                           new ChildEntity(expected, 49291)
        ////                               {
        ////                                   Name = "Child1",
        ////                                   SomeValue = 24441,
        ////                                   Parent = parent1
        ////                               },
        ////                           new ChildEntity(expected, 50282)
        ////                               {
        ////                                   Name = "Child2",
        ////                                   SomeValue = 244389,
        ////                                   Parent = parent1
        ////                               },
        ////                           new ChildEntity(expected, 66939)
        ////                               {
        ////                                   Name = "Child3",
        ////                                   SomeValue = 48932,
        ////                                   Parent = parent2
        ////                               }
        ////                       };

        ////    var childRows = children.OrderByDescending(x => x.SomeValue).Select(this.entityMapper.Map<ChildRaisedRow>).ToList();
        ////    var entityRow = this.entityMapper.Map<ComplexRaisedRow>(expected);

        ////    var repositoryAdapter = MockRepository.GenerateMock<IRepositoryAdapter>();
        ////    repositoryAdapter.Stub(adapter => adapter.SelectItems(Arg<ItemSelection<ChildRaisedRow>>.Is.Anything)).Return(childRows);
        ////    repositoryAdapter.Stub(adapter => adapter.FirstOrDefault(Arg<ItemSelection<ComplexRaisedRow>>.Is.Anything)).Return(entityRow);

        ////    var repositoryAdapterFactory = MockRepository.GenerateMock<IRepositoryAdapterFactory>();
        ////    repositoryAdapterFactory.Stub(factory => factory.Create(Arg<Database>.Is.Anything)).Return(repositoryAdapter);

        ////    using (var provider = new DatabaseRepositoryProvider(
        ////        GenericDatabaseFactory<FakeDataContext>.Default,
        ////        this.entityMapper,
        ////        repositoryAdapterFactory))
        ////    {
        ////        var target = new FakeComplexEntityRepository(provider, this.entityMapper);
        ////        var actual = target.FirstOrDefaultWithChildren(99291);
        ////        Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
        ////        ChildEntity selectedParent1 = null;
        ////        ChildEntity selectedParent2 = null;

        ////        foreach (var childEntity in actual.ChildEntities)
        ////        {
        ////            Assert.IsTrue(ReferenceEquals(actual, childEntity.ComplexEntity));

        ////            if (childEntity.Name == "Parent1")
        ////            {
        ////                selectedParent1 = childEntity;
        ////            }

        ////            if (childEntity.Name == "Parent2")
        ////            {
        ////                selectedParent2 = childEntity;
        ////            }

        ////            if (new[]
        ////                    {
        ////                        "Child1",
        ////                        "Child2"
        ////                    }.Contains(childEntity.Name))
        ////            {
        ////                Assert.IsTrue(ReferenceEquals(selectedParent1, childEntity.Parent));
        ////            }

        ////            if (new[]
        ////                    {
        ////                        "Child3"
        ////                    }.Contains(childEntity.Name))
        ////            {
        ////                Assert.IsTrue(ReferenceEquals(selectedParent2, childEntity.Parent));
        ////            }
        ////        }
        ////    }
        ////}

        /// <summary>
        /// The create entity mapper.
        /// </summary>
        /// <returns>
        /// The <see cref="Startitecture.Orm.Common.IEntityMapper" />.
        /// </returns>
        private static AutoMapperEntityMapper CreateEntityMapper()
        {
            var autoMapperEntityMapper = new AutoMapperEntityMapper();
            autoMapperEntityMapper.Initialize(
                configuration =>
                    {
                        configuration.AddProfile<FakeSubSubEntityMappingProfile>();
                        configuration.AddProfile<FakeMultiReferenceEntityMappingProfile>();
                        configuration.AddProfile<FakeCreatedByMappingProfile>();
                        configuration.AddProfile<FakeModifiedByMappingProfile>();
                        configuration.AddProfile<FakeSubEntityMappingProfile>();
                        configuration.AddProfile<FakeChildEntityMappingProfile>();
                        configuration.AddProfile<FakeComplexEntityMappingProfile>();
                        configuration.AddProfile<FakeDependentEntityMappingProfile>();
                    });

            return autoMapperEntityMapper;
        }

        /// <summary>
        /// The create insert repository adapter factory.
        /// </summary>
        /// <returns>
        /// The <see cref="IRepositoryAdapterFactory"/>.
        /// </returns>
        private static IRepositoryAdapterFactory CreateInsertRepositoryAdapterFactory()
        {
            var repositoryAdapter = MockRepository.GenerateMock<IRepositoryAdapter>();
            CreateInsertMock<ComplexRaisedRow>(repositoryAdapter, row => row.FakeComplexEntityId, 22);
            CreateInsertMock<SubSubRow>(repositoryAdapter, row => row.FakeSubSubEntityId, 45);
            CreateInsertMock<SubRow>(repositoryAdapter, row => row.FakeSubEntityId, 16);
            CreateInsertMock<MultiReferenceRow>(repositoryAdapter, row => row.FakeMultiReferenceEntityId, 432);
            CreateInsertMock<DependencyRow>(repositoryAdapter, row => row.FakeDependencyEntityId, 6);
            CreateInsertMock<DependentRow>(repositoryAdapter, row => row.FakeDependentEntityId, 22);

            repositoryAdapter.Expect(adapter => adapter.Insert(Arg<ChildRaisedRow>.Is.Anything))
                .Return(null)
                .WhenCalled(
                    invocation =>
                        {
                            var childRow = invocation.Arguments.OfType<ChildRaisedRow>().First();

                            switch (childRow.SomeValue)
                            {
                                case 100:
                                    childRow.FakeChildEntityId = 1;
                                    break;

                                case 110:
                                    childRow.FakeChildEntityId = 11;
                                    break;

                                case 120:
                                    childRow.FakeChildEntityId = 12;
                                    break;

                                case 200:
                                    childRow.FakeChildEntityId = 2;
                                    break;

                                case 210:
                                    childRow.FakeChildEntityId = 21;
                                    break;

                                case 4492:
                                    childRow.FakeChildEntityId = 235;
                                    break;
                            }

                            invocation.ReturnValue = childRow;
                        });

            var repositoryAdapterFactory = MockRepository.GenerateMock<IRepositoryAdapterFactory>();
            repositoryAdapterFactory.Stub(factory => factory.Create(Arg<Database>.Is.Anything)).Return(repositoryAdapter);
            return repositoryAdapterFactory;
        }

        /// <summary>
        /// Creates an insert mock for the specified adapter.
        /// </summary>
        /// <param name="repositoryAdapter">
        /// The repository adapter.
        /// </param>
        /// <param name="keyProperty">
        /// The key property.
        /// </param>
        /// <param name="keyValue">
        /// The key value.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item to set the key property on.
        /// </typeparam>
        private static void CreateInsertMock<TItem>(
            IRepositoryAdapter repositoryAdapter,
            Expression<Func<TItem, object>> keyProperty,
            object keyValue)
            where TItem : class, ITransactionContext
        {
            repositoryAdapter.Stub(adapter => adapter.Insert(Arg<TItem>.Is.Anything))
                .Return(null)
                .WhenCalled(
                    invocation =>
                        {
                            var item = invocation.Arguments.OfType<TItem>().First();
                            item.SetPropertyValue(keyProperty, keyValue);
                            invocation.ReturnValue = item;
                        });
        }

        /// <summary>
        /// Creates a complex mock adapter factory for updates to the fake child entity.
        /// </summary>
        /// <param name="entity">
        /// The entity to create the adapter for.
        /// </param>
        /// <returns>
        /// The <see cref="IRepositoryAdapterFactory"/>.
        /// </returns>
        private MockedRepositoryAdapter CreateComplexMockAdapterFactoryForUpdate(ChildEntity entity)
        {
            var repositoryAdapter = MockRepository.GenerateMock<IRepositoryAdapter>();

            this.CreateMockAdapterForEntity(entity.ComplexEntity, repositoryAdapter);

            repositoryAdapter.Stub(adapter => adapter.Contains(Arg<ItemSelection<ChildRaisedRow>>.Is.Anything)).Return(true);

            repositoryAdapter.Stub(adapter => adapter.FirstOrDefault(Arg<ItemSelection<ChildRaisedRow>>.Is.Anything))
                .Return(this.entityMapper.Map<ChildRaisedRow>(entity));

            var repositoryAdapterFactory = MockRepository.GenerateMock<IRepositoryAdapterFactory>();
            repositoryAdapterFactory.Stub(factory => factory.Create(Arg<Database>.Is.Anything)).Return(repositoryAdapter);
            return new MockedRepositoryAdapter
                       {
                           RepositoryAdapterFactory = repositoryAdapterFactory,
                           RepositoryAdapter = repositoryAdapter
                       };
        }

        /// <summary>
        /// The create mock adapter for entity.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="repositoryAdapter">
        /// The repository adapter.
        /// </param>
        private void CreateMockAdapterForEntity(ComplexEntity entity, IRepositoryAdapter repositoryAdapter)
        {
            repositoryAdapter.Stub(
                    adapter => adapter.Update(
                        Arg<ChildRaisedRow>.Is.Anything,
                        Arg<ItemSelection<ChildRaisedRow>>.Is.Anything,
                        Arg<Expression<Func<ChildRaisedRow, object>>[]>.Is.Anything))
                .Return(1);

            repositoryAdapter.Stub(
                    adapter => adapter.Update(
                        Arg<ComplexRaisedRow>.Is.Anything,
                        Arg<ItemSelection<ComplexRaisedRow>>.Is.Anything,
                        Arg<Expression<Func<ComplexRaisedRow, object>>[]>.Is.Anything))
                .Return(1);

            repositoryAdapter.Stub(
                    adapter => adapter.Update(
                        Arg<SubSubRow>.Is.Anything,
                        Arg<ItemSelection<SubSubRow>>.Is.Anything,
                        Arg<Expression<Func<SubSubRow, object>>[]>.Is.Anything))
                .Return(1);

            repositoryAdapter.Stub(
                    adapter => adapter.Update(
                        Arg<SubRow>.Is.Anything,
                        Arg<ItemSelection<SubRow>>.Is.Anything,
                        Arg<Expression<Func<SubRow, object>>[]>.Is.Anything))
                .Return(1);

            repositoryAdapter.Stub(
                    adapter => adapter.Update(
                        Arg<MultiReferenceRow>.Is.Anything,
                        Arg<ItemSelection<MultiReferenceRow>>.Is.Anything,
                        Arg<Expression<Func<MultiReferenceRow, object>>[]>.Is.Anything))
                .Return(1);

            repositoryAdapter.Stub(
                    adapter => adapter.Update(
                        Arg<DependencyRow>.Is.Anything,
                        Arg<ItemSelection<DependencyRow>>.Is.Anything,
                        Arg<Expression<Func<DependencyRow, object>>[]>.Is.Anything))
                .Return(1);

            repositoryAdapter.Stub(
                    adapter => adapter.Update(
                        Arg<DependentRow>.Is.Anything,
                        Arg<ItemSelection<DependentRow>>.Is.Anything,
                        Arg<Expression<Func<DependentRow, object>>[]>.Is.Anything))
                .Return(1);

            repositoryAdapter.Stub(adapter => adapter.FirstOrDefault(Arg<ItemSelection<ComplexRaisedRow>>.Is.Anything))
                .Return(this.entityMapper.Map<ComplexRaisedRow>(entity));

            repositoryAdapter.Stub(adapter => adapter.FirstOrDefault(Arg<ItemSelection<SubSubRow>>.Is.Anything))
                .Return(this.entityMapper.Map<SubSubRow>(entity.SubSubEntity));

            repositoryAdapter.Stub(adapter => adapter.FirstOrDefault(Arg<ItemSelection<SubRow>>.Is.Anything))
                .Return(this.entityMapper.Map<SubRow>(entity.FakeSubEntityId));

            CreateInsertMock<DependentRow>(repositoryAdapter, row => row.FakeDependentEntityId, entity.FakeComplexEntityId);

            repositoryAdapter.Stub(
                    adapter => adapter.FirstOrDefault(
                        Arg<ItemSelection<MultiReferenceRow>>.Matches(selection => selection.PropertyValues.Any(x => x.Equals(432)))))
                .Return(this.entityMapper.Map<MultiReferenceRow>(entity.CreatedBy));

            repositoryAdapter.Stub(
                    adapter => adapter.FirstOrDefault(
                        Arg<ItemSelection<MultiReferenceRow>>.Matches(selection => selection.PropertyValues.Any(x => x.Equals(433)))))
                .Return(this.entityMapper.Map<MultiReferenceRow>(entity.ModifiedBy));
        }

        /// <summary>
        /// The mocked repository adapter.
        /// </summary>
        private class MockedRepositoryAdapter
        {
            /// <summary>
            /// Gets or sets the repository adapter.
            /// </summary>
            public IRepositoryAdapter RepositoryAdapter { get; set; }

            /// <summary>
            /// Gets or sets the repository adapter factory.
            /// </summary>
            public IRepositoryAdapterFactory RepositoryAdapterFactory { get; set; }
        }
    }
}