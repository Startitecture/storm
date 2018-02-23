// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityRepositoryTests.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Startitecture.Orm.Repository.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Rhino.Mocks;

    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Mapper;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Repository.Tests.Models;
    using Startitecture.Orm.Testing.Model;
    using Startitecture.Orm.Testing.RhinoMocks;

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
        /// The map to_ key to entity_ key is set.
        /// </summary>
        [TestMethod]
        public void MapTo_KeyToEntity_KeyIsSet()
        {
            Func<object> function = () => 23;
            object key = function.DynamicInvoke();
            var subSubEntity = new FakeSubSubEntity("MySubSubEntity") { Description = "Woo" };
            this.entityMapper.MapTo(key, subSubEntity);
            Assert.AreEqual(key, subSubEntity.FakeSubSubEntityId);
        }

        /// <summary>
        /// The save test.
        /// </summary>
        [TestMethod]
        public void Save_NewFakeComplexEntity_MatchesExpected()
        {
            var repositoryAdapterFactory = CreateInsertRepositoryAdapterFactory();

            using (var provider = new DatabaseRepositoryProvider<FakeDataContext>(repositoryAdapterFactory, this.entityMapper))
            {
                var target = new FakeComplexEntityRepository(provider);
                var fakeSubSubEntity = new FakeSubSubEntity("SubSubUniqueName");
                var fakeSubEntity = new FakeSubEntity("SubUniqueName", 234, fakeSubSubEntity);
                var fakeCreatedBy = new FakeCreatedBy("CreateUniqueName");
                var fakeModifiedBy = new FakeModifiedBy("ModifiedBy");
                var expected = new FakeComplexEntity("UniqueName", fakeSubEntity, FakeEnumeration.FirstFake, fakeCreatedBy)
                                   {
                                       ModifiedBy = fakeModifiedBy, 
                                       ModifiedTime = DateTimeOffset.Now.AddHours(1)
                                   };

                var actual = target.Save(expected);
                Assert.IsNotNull(actual.FakeSubEntity);
                Assert.IsNotNull(actual.FakeSubEntity.FakeSubSubEntity);
                Assert.IsNotNull(actual.CreatedBy);
                Assert.IsNotNull(actual.ModifiedBy);
                Assert.AreEqual(16, actual.FakeSubEntity.FakeSubEntityId);
                Assert.AreEqual(expected.FakeSubEntity, actual.FakeSubEntity);
                Assert.AreEqual(45, actual.FakeSubSubEntity.FakeSubSubEntityId);
                Assert.AreEqual(expected.FakeSubEntity.FakeSubSubEntity, actual.FakeSubEntity.FakeSubSubEntity);
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
        public void Save_NewFakeRaisedComplexEntity_MatchesExpected()
        {
            var repositoryAdapterFactory = CreateInsertRepositoryAdapterFactory();

            using (var provider = new DatabaseRepositoryProvider<FakeDataContext>(repositoryAdapterFactory, this.entityMapper))
            {
                var target = new FakeComplexEntityRepository(provider);
                var fakeSubSubEntity = new FakeSubSubEntity("SubSubUniqueName");
                var fakeSubEntity = new FakeSubEntity("SubUniqueName", 234, fakeSubSubEntity);
                var fakeCreatedBy = new FakeCreatedBy("CreateUniqueName");
                var fakeModifiedBy = new FakeModifiedBy("ModifiedBy");
                var expected = new FakeComplexEntity("UniqueName", fakeSubEntity, FakeEnumeration.FirstFake, fakeCreatedBy)
                {
                    ModifiedBy = fakeModifiedBy,
                    ModifiedTime = DateTimeOffset.Now.AddHours(1)
                };

                var actual = target.Save(expected);
                Assert.IsNotNull(actual.FakeSubEntity);
                Assert.IsNotNull(actual.FakeSubEntity.FakeSubSubEntity);
                Assert.IsNotNull(actual.CreatedBy);
                Assert.IsNotNull(actual.ModifiedBy);
                Assert.AreEqual(16, actual.FakeSubEntity.FakeSubEntityId);
                Assert.AreEqual(expected.FakeSubEntity, actual.FakeSubEntity);
                Assert.AreEqual(45, actual.FakeSubSubEntity.FakeSubSubEntityId);
                Assert.AreEqual(expected.FakeSubEntity.FakeSubSubEntity, actual.FakeSubEntity.FakeSubSubEntity);
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

            using (var provider = new DatabaseRepositoryProvider<FakeDataContext>(repositoryAdapterFactory, this.entityMapper))
            {
                var target = new FakeComplexEntityRepository(provider);
                var fakeSubSubEntity = new FakeSubSubEntity("SubSubUniqueName");
                var fakeSubEntity = new FakeSubEntity("SubUniqueName", 234, fakeSubSubEntity);
                var fakeCreatedBy = new FakeCreatedBy("CreateUniqueName");
                var fakeModifiedBy = new FakeModifiedBy("ModifiedBy");
                var expected = new FakeComplexEntity("UniqueName", fakeSubEntity, FakeEnumeration.FirstFake, fakeCreatedBy)
                {
                    ModifiedBy = fakeModifiedBy,
                    ModifiedTime = DateTimeOffset.Now.AddHours(1)
                };

                var actual = target.Save(expected);
                Assert.IsNotNull(actual.FakeSubEntity);
                Assert.IsNotNull(actual.FakeSubEntity.FakeSubSubEntity);
                Assert.IsNotNull(actual.CreatedBy);
                Assert.IsNotNull(actual.ModifiedBy);
                Assert.AreEqual(16, actual.FakeSubEntity.FakeSubEntityId);
                Assert.AreEqual(expected.FakeSubEntity, actual.FakeSubEntity);
                Assert.AreEqual(45, actual.FakeSubSubEntity.FakeSubSubEntityId);
                Assert.AreEqual(expected.FakeSubEntity.FakeSubSubEntity, actual.FakeSubEntity.FakeSubSubEntity);
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

            using (var provider = new DatabaseRepositoryProvider<FakeDataContext>(repositoryAdapterFactory, this.entityMapper))
            {
                var target = new FakeComplexEntityRepository(provider);
                var fakeSubSubEntity = new FakeSubSubEntity("SubSubUniqueName");
                var fakeSubEntity = new FakeSubEntity("SubUniqueName", 234, fakeSubSubEntity);
                var fakeCreatedBy = new FakeCreatedBy("CreateUniqueName");
                var modifiedBy = new FakeModifiedBy("ModifiedBy");
                var expected = new FakeComplexEntity("UniqueName", fakeSubEntity, FakeEnumeration.FirstFake, fakeCreatedBy)
                {
                    ModifiedBy = modifiedBy, 
                    ModifiedTime = DateTimeOffset.Now.AddHours(1)
                };

                expected.SetDependentEntity(33, DateTimeOffset.Now);

                var actual = target.Save(expected);
                Assert.IsNotNull(actual.FakeSubEntity);
                Assert.IsNotNull(actual.FakeSubEntity.FakeSubSubEntity);
                Assert.IsNotNull(actual.CreatedBy);
                Assert.IsNotNull(actual.ModifiedBy);
                Assert.IsNotNull(actual.FakeDependentEntity);
                Assert.AreEqual(16, actual.FakeSubEntity.FakeSubEntityId);
                Assert.AreEqual(expected.FakeSubEntity, actual.FakeSubEntity);
                Assert.AreEqual(45, actual.FakeSubSubEntity.FakeSubSubEntityId);
                Assert.AreEqual(expected.FakeSubEntity.FakeSubSubEntity, actual.FakeSubEntity.FakeSubSubEntity);
                Assert.AreEqual(432, actual.CreatedBy.FakeMultiReferenceEntityId);
                Assert.AreEqual(expected.CreatedBy, actual.CreatedBy);
                Assert.AreEqual(432, actual.ModifiedBy.FakeMultiReferenceEntityId);
                Assert.AreEqual(expected.ModifiedBy, actual.ModifiedBy);
                Assert.AreEqual(22, actual.FakeDependentEntityId);
                Assert.AreEqual(expected.FakeDependentEntity, actual.FakeDependentEntity);
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
            var fakeSubSubEntity = new FakeSubSubEntity("SubSubUniqueName", 45) { Description = "OriginalSubSub" };
            var fakeSubEntity = new FakeSubEntity("SubUniqueName", 234, fakeSubSubEntity, 16) { Description = "OriginalSub" };
            var originalCreatedBy = new FakeCreatedBy("CreateUniqueName", 432) { Description = "OriginalCreatedBy" };
            var modifiedBy = new FakeModifiedBy("ModifiedBy", 433) { Description = "OriginalModifiedBy" };
            var creationTime = DateTimeOffset.Now.AddDays(-1);
            var baseline = new FakeComplexEntity("UniqueName", fakeSubEntity, FakeEnumeration.FirstFake, originalCreatedBy, creationTime, 22)
            {
                Description = "OriginalComplexEntity", 
                ModifiedBy = modifiedBy, 
                ModifiedTime = DateTimeOffset.Now.AddHours(1)
            };

            var repositoryAdapter = RepositoryMockFactory.CreateAdapter();
            repositoryAdapter.StubForExistingItem<FakeSubSubRow>(fakeSubSubEntity, this.entityMapper);
            repositoryAdapter.StubForExistingItem<FakeFlatSubRow>(fakeSubEntity, this.entityMapper);
            repositoryAdapter.StubForExistingItem<FakeMultiReferenceRow, int>(
                originalCreatedBy,
                this.entityMapper,
                item => item.FakeMultiReferenceEntityId,
                originalCreatedBy.FakeMultiReferenceEntityId.GetValueOrDefault());

            repositoryAdapter.StubForExistingItem<FakeMultiReferenceRow, int>(
                modifiedBy,
                this.entityMapper,
                item => item.FakeMultiReferenceEntityId,
                modifiedBy.FakeMultiReferenceEntityId.GetValueOrDefault());

            repositoryAdapter.StubForExistingItem<FakeComplexRow>(baseline, this.entityMapper);

            using (var provider = RepositoryMockFactory.CreateConcreteProvider<FakeDataContext>(this.entityMapper, repositoryAdapter))
            {
                var newModifiedBy = new FakeModifiedBy("ModifiedBy", 433) { Description = "UpdatedModifiedBy" };

                var target = new FakeComplexEntityRepository(provider);
                var expected = target.FirstOrDefault(22);
                expected.Description = "UpdatedEntity";
                expected.ModifiedBy = newModifiedBy;
                expected.ModifiedTime = DateTimeOffset.Now.AddHours(1);
                expected.FakeSubEntity.Description = "ModifiedSub";
                expected.FakeSubSubEntity.Description = "ModifiedSubSub";

                var actual = target.Save(expected);
                Assert.IsNotNull(actual.FakeSubEntity);
                Assert.IsNotNull(actual.FakeSubEntity.FakeSubSubEntity);
                Assert.IsNotNull(actual.CreatedBy);
                Assert.IsNotNull(actual.ModifiedBy);
                Assert.AreEqual(16, actual.FakeSubEntity.FakeSubEntityId);
                Assert.AreEqual(expected.FakeSubEntity, actual.FakeSubEntity);
                Assert.AreEqual(45, actual.FakeSubSubEntity.FakeSubSubEntityId);
                Assert.AreEqual(expected.FakeSubEntity.FakeSubSubEntity, actual.FakeSubEntity.FakeSubSubEntity);
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
            var fakeSubSubEntity = new FakeSubSubEntity("SubSubUniqueName", 45) { Description = "OriginalSubSub" };
            var fakeSubEntity = new FakeSubEntity("SubUniqueName", 234, fakeSubSubEntity, 16) { Description = "OriginalSub" };
            var originalCreatedBy = new FakeCreatedBy("CreateUniqueName", 432) { Description = "OriginalCreatedBy" };
            var modifiedBy = new FakeModifiedBy("ModifiedBy", 433) { Description = "OriginalModifiedBy" };
            var creationTime = DateTimeOffset.Now.AddDays(-1);
            var baseline = new FakeComplexEntity("UniqueName", fakeSubEntity, FakeEnumeration.FirstFake, originalCreatedBy, creationTime, 22)
            {
                Description = "OriginalComplexEntity",
                ModifiedBy = modifiedBy,
                ModifiedTime = DateTimeOffset.Now.AddHours(1)
            };

            var repositoryAdapter = RepositoryMockFactory.CreateAdapter();
            repositoryAdapter.StubForExistingItem<FakeSubSubRow>(fakeSubSubEntity, this.entityMapper);
            repositoryAdapter.StubForExistingItem<FakeFlatSubRow>(fakeSubEntity, this.entityMapper);
            repositoryAdapter.StubForExistingItem<FakeMultiReferenceRow, int>(
                originalCreatedBy,
                this.entityMapper,
                item => item.FakeMultiReferenceEntityId,
                originalCreatedBy.FakeMultiReferenceEntityId.GetValueOrDefault());

            repositoryAdapter.StubForExistingItem<FakeMultiReferenceRow, int>(
                modifiedBy,
                this.entityMapper,
                item => item.FakeMultiReferenceEntityId,
                modifiedBy.FakeMultiReferenceEntityId.GetValueOrDefault());

            repositoryAdapter.StubForExistingItem<FakeComplexRow>(baseline, this.entityMapper);

            using (var provider = RepositoryMockFactory.CreateConcreteProvider<FakeDataContext>(this.entityMapper, repositoryAdapter))
            {
                var newModifiedBy = new FakeModifiedBy("ModifiedBy", 433) { Description = "UpdatedModifiedBy" };

                var target = new FakeComplexEntityRepository(provider);
                var expected = target.FirstOrDefault(22);
                expected.Description = "UpdatedEntity";
                expected.ModifiedBy = newModifiedBy;
                expected.ModifiedTime = DateTimeOffset.Now.AddHours(1);
                expected.FakeSubEntity.Description = "ModifiedSub";
                expected.FakeSubSubEntity.Description = "ModifiedSubSub";
                expected.SetDependentEntity(33);

                repositoryAdapter.StubForNewItem<FakeDependentRow>();

                var actual = target.Save(expected);
                Assert.IsNotNull(actual.FakeSubEntity);
                Assert.IsNotNull(actual.FakeSubEntity.FakeSubSubEntity);
                Assert.IsNotNull(actual.CreatedBy);
                Assert.IsNotNull(actual.ModifiedBy);
                Assert.IsNotNull(actual.FakeDependentEntity);
                Assert.AreEqual(16, actual.FakeSubEntity.FakeSubEntityId);
                Assert.AreEqual(expected.FakeSubEntity, actual.FakeSubEntity);
                Assert.AreEqual(45, actual.FakeSubSubEntity.FakeSubSubEntityId);
                Assert.AreEqual(expected.FakeSubEntity.FakeSubSubEntity, actual.FakeSubEntity.FakeSubSubEntity);
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
                Assert.AreEqual(expected.FakeDependentEntity, actual.FakeDependentEntity);
                Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
            }
        }

        /// <summary>
        /// The save test.
        /// </summary>
        [TestMethod]
        public void Save_UpdatedFakeComplexEntityWithUpdatedDependentEntity_MatchesExpected()
        {
            var fakeSubSubEntity = new FakeSubSubEntity("SubSubUniqueName", 45) { Description = "OriginalSubSub" };
            var fakeSubEntity = new FakeSubEntity("SubUniqueName", 234, fakeSubSubEntity, 16) { Description = "OriginalSub" };
            var originalCreatedBy = new FakeCreatedBy("CreateUniqueName", 432) { Description = "OriginalCreatedBy" };
            var modifiedBy = new FakeModifiedBy("ModifiedBy", 433) { Description = "OriginalModifiedBy" };
            var creationTime = DateTimeOffset.Now.AddDays(-1);
            var baseline = new FakeComplexEntity("UniqueName", fakeSubEntity, FakeEnumeration.FirstFake, originalCreatedBy, creationTime, 22)
            {
                Description = "OriginalComplexEntity",
                ModifiedBy = modifiedBy,
                ModifiedTime = DateTimeOffset.Now.AddHours(1)
            };

            var dependentEntity = baseline.SetDependentEntity(9845, DateTimeOffset.Now.AddHours(-3));

            var repositoryAdapter = RepositoryMockFactory.CreateAdapter();
            repositoryAdapter.StubForExistingItem<FakeSubSubRow>(fakeSubSubEntity, this.entityMapper);
            repositoryAdapter.StubForExistingItem<FakeFlatSubRow>(fakeSubEntity, this.entityMapper);
            repositoryAdapter.StubForExistingItem<FakeMultiReferenceRow, int>(
                originalCreatedBy,
                this.entityMapper,
                item => item.FakeMultiReferenceEntityId,
                originalCreatedBy.FakeMultiReferenceEntityId.GetValueOrDefault());

            repositoryAdapter.StubForExistingItem<FakeMultiReferenceRow, int>(
                modifiedBy,
                this.entityMapper,
                item => item.FakeMultiReferenceEntityId,
                modifiedBy.FakeMultiReferenceEntityId.GetValueOrDefault());

            repositoryAdapter.StubForExistingItem<FakeComplexRow>(baseline, this.entityMapper);
            repositoryAdapter.StubForExistingItem<FakeDependentRow>(dependentEntity, this.entityMapper);

            using (var provider = RepositoryMockFactory.CreateConcreteProvider<FakeDataContext>(this.entityMapper, repositoryAdapter))
            {
                var newModifiedBy = new FakeModifiedBy("ModifiedBy", 433) { Description = "UpdatedModifiedBy" };

                var target = new FakeComplexEntityRepository(provider);
                var expected = target.FirstOrDefault(22);
                expected.Description = "UpdatedEntity";
                expected.ModifiedBy = newModifiedBy;
                expected.ModifiedTime = DateTimeOffset.Now.AddHours(1);
                expected.FakeSubEntity.Description = "ModifiedSub";
                expected.FakeSubSubEntity.Description = "ModifiedSubSub";
                expected.SetDependentEntity(992);

                var actual = target.Save(expected);
                Assert.IsNotNull(actual.FakeSubEntity);
                Assert.IsNotNull(actual.FakeSubEntity.FakeSubSubEntity);
                Assert.IsNotNull(actual.CreatedBy);
                Assert.IsNotNull(actual.ModifiedBy);
                Assert.AreEqual(16, actual.FakeSubEntity.FakeSubEntityId);
                Assert.AreEqual(expected.FakeSubEntity, actual.FakeSubEntity);
                Assert.AreEqual(45, actual.FakeSubSubEntity.FakeSubSubEntityId);
                Assert.AreEqual(expected.FakeSubEntity.FakeSubSubEntity, actual.FakeSubEntity.FakeSubSubEntity);
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
                Assert.AreEqual(expected.FakeDependentEntity, actual.FakeDependentEntity);
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
            var fakeSubSubEntity = new FakeSubSubEntity("SubSubUniqueName", 45) { Description = "OriginalSubSub" };
            var fakeSubEntity = new FakeSubEntity("SubUniqueName", 234, fakeSubSubEntity, 16) { Description = "OriginalSub" };
            var originalCreatedBy = new FakeCreatedBy("CreateUniqueName", 432) { Description = "OriginalCreatedBy" };
            var modifiedBy = new FakeModifiedBy("ModifiedBy", 433) { Description = "OriginalModifiedBy" };
            var creationTime = DateTimeOffset.Now.AddDays(-1);
            var baseline = new FakeComplexEntity("UniqueName", fakeSubEntity, FakeEnumeration.FirstFake, originalCreatedBy, creationTime, 22)
            {
                Description = "OriginalComplexEntity",
                ModifiedBy = modifiedBy,
                ModifiedTime = DateTimeOffset.Now.AddHours(1)
            };

            var dependentEntity = baseline.SetDependentEntity(393);

            var repositoryAdapter = RepositoryMockFactory.CreateAdapter();
            repositoryAdapter.StubForExistingItem<FakeSubSubRow>(fakeSubSubEntity, this.entityMapper);
            repositoryAdapter.StubForExistingItem<FakeFlatSubRow>(fakeSubEntity, this.entityMapper);
            repositoryAdapter.StubForExistingItem<FakeMultiReferenceRow, int>(
                originalCreatedBy,
                this.entityMapper,
                item => item.FakeMultiReferenceEntityId,
                originalCreatedBy.FakeMultiReferenceEntityId.GetValueOrDefault());

            repositoryAdapter.StubForExistingItem<FakeMultiReferenceRow, int>(
                modifiedBy,
                this.entityMapper,
                item => item.FakeMultiReferenceEntityId,
                modifiedBy.FakeMultiReferenceEntityId.GetValueOrDefault());

            repositoryAdapter.StubForExistingItem<FakeComplexRow>(baseline, this.entityMapper);
            repositoryAdapter.StubForExistingItem<FakeDependentRow>(dependentEntity, this.entityMapper);

            using (var provider = RepositoryMockFactory.CreateConcreteProvider<FakeDataContext>(this.entityMapper, repositoryAdapter))
            {
                var newModifiedBy = new FakeModifiedBy("ModifiedBy", 433) { Description = "UpdatedModifiedBy" };

                var target = new FakeComplexEntityRepository(provider);
                var expected = target.FirstOrDefault(22);
                expected.Description = "UpdatedEntity";
                expected.ModifiedBy = newModifiedBy;
                expected.ModifiedTime = DateTimeOffset.Now.AddHours(1);
                expected.FakeSubEntity.Description = "ModifiedSub";
                expected.FakeSubSubEntity.Description = "ModifiedSubSub";
                expected.SetDependentEntity(0);

                var actual = target.Save(expected);
                Assert.IsNotNull(actual.FakeSubEntity);
                Assert.IsNotNull(actual.FakeSubEntity.FakeSubSubEntity);
                Assert.IsNotNull(actual.CreatedBy);
                Assert.IsNotNull(actual.ModifiedBy);
                Assert.IsNull(actual.FakeDependentEntity);
                Assert.AreEqual(16, actual.FakeSubEntity.FakeSubEntityId);
                Assert.AreEqual(expected.FakeSubEntity, actual.FakeSubEntity);
                Assert.AreEqual(45, actual.FakeSubSubEntity.FakeSubSubEntityId);
                Assert.AreEqual(expected.FakeSubEntity.FakeSubSubEntity, actual.FakeSubEntity.FakeSubSubEntity);
                Assert.AreEqual(432, actual.CreatedBy.FakeMultiReferenceEntityId);
                Assert.AreEqual(
                    baseline.CreatedBy,
                    actual.CreatedBy,
                    string.Join(Environment.NewLine, baseline.CreatedBy.GetDifferences(actual.CreatedBy)));

                Assert.AreEqual(creationTime, actual.CreationTime);
                Assert.AreEqual(433, actual.ModifiedBy.FakeMultiReferenceEntityId);
                Assert.AreEqual(expected.ModifiedBy, actual.ModifiedBy);
                Assert.AreEqual(expected.ModifiedTime, actual.ModifiedTime);
                Assert.AreEqual(expected.FakeDependentEntity, actual.FakeDependentEntity);
                Assert.AreEqual(22, actual.FakeComplexEntityId);
                Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
            }
        }

        /// <summary>
        /// The save with children_ new fake complex entity_ matches expected.
        /// </summary>
        [TestMethod]
        public void SaveWithChildren_NewFakeComplexEntity_MatchesExpected()
        {
            var repositoryAdapterFactory = CreateInsertRepositoryAdapterFactory();

            using (var provider = new DatabaseRepositoryProvider<FakeDataContext>(repositoryAdapterFactory, this.entityMapper))
            {
                var target = new FakeComplexEntityRepository(provider);
                var fakeSubSubEntity = new FakeSubSubEntity("SubSubUniqueName");
                var fakeSubEntity = new FakeSubEntity("SubUniqueName", 234, fakeSubSubEntity);
                var fakeCreatedBy = new FakeCreatedBy("CreateUniqueName");
                var modifiedBy = new FakeModifiedBy("ModifiedBy");
                var expected = new FakeComplexEntity("UniqueName", fakeSubEntity, FakeEnumeration.FirstFake, fakeCreatedBy)
                {
                    ModifiedBy = modifiedBy, 
                    ModifiedTime = DateTimeOffset.Now.AddHours(1)
                };

                var fakeChild1 = new FakeChildEntity(expected) { SomeValue = 100, Name = "Parent1" };
                var fakeChild2 = new FakeChildEntity(expected) { SomeValue = 200, Name = "Parent2" };
                var fakeChild3 = new FakeChildEntity(expected) { SomeValue = 110, Name = "Child1", Parent = fakeChild1 };
                var fakeChild4 = new FakeChildEntity(expected) { SomeValue = 120, Name = "Child2", Parent = fakeChild1 };
                var fakeChild5 = new FakeChildEntity(expected) { SomeValue = 210, Name = "Child3", Parent = fakeChild2 };

                var fakeChildren = new List<FakeChildEntity> { fakeChild1, fakeChild2, fakeChild3, fakeChild4, fakeChild5 };
                expected.Load(fakeChildren);

                var actual = target.SaveWithChildren(expected);
                Assert.IsNotNull(actual.FakeSubEntity);
                Assert.IsNotNull(actual.FakeSubEntity.FakeSubSubEntity);
                Assert.IsNotNull(actual.CreatedBy);
                Assert.IsNotNull(actual.ModifiedBy);
                Assert.AreEqual(16, actual.FakeSubEntity.FakeSubEntityId);
                Assert.AreEqual(expected.FakeSubEntity, actual.FakeSubEntity);
                Assert.AreEqual(45, actual.FakeSubSubEntity.FakeSubSubEntityId);
                Assert.AreEqual(expected.FakeSubEntity.FakeSubSubEntity, actual.FakeSubEntity.FakeSubSubEntity);
                Assert.AreEqual(432, actual.CreatedBy.FakeMultiReferenceEntityId);
                Assert.AreEqual(expected.CreatedBy, actual.CreatedBy);
                Assert.AreEqual(432, actual.ModifiedBy.FakeMultiReferenceEntityId);
                Assert.AreEqual(expected.ModifiedBy, actual.ModifiedBy);
                Assert.AreEqual(22, actual.FakeComplexEntityId);
                Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
                CollectionAssert.AreEqual(fakeChildren, actual.ChildEntities.ToList());

                foreach (var childEntity in actual.ChildEntities)
                {
                    switch (childEntity.SomeValue)
                    {
                        case 100:
                            Assert.AreEqual(1, childEntity.FakeChildEntityId);
                            break;

                        case 110:
                            Assert.AreEqual(11, childEntity.FakeChildEntityId);
                            break;

                        case 120:
                            Assert.AreEqual(12, childEntity.FakeChildEntityId);
                            break;

                        case 200:
                            Assert.AreEqual(2, childEntity.FakeChildEntityId);
                            break;

                        case 210:
                            Assert.AreEqual(21, childEntity.FakeChildEntityId);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// The save test.
        /// </summary>
        [TestMethod]
        public void Select_FirstOrDefaultFakeComplexEntity_MatchesExpected()
        {
            var fakeSubSubEntity = new FakeSubSubEntity("SubSubUniqueName1", 45) { Description = "OriginalSubSub" };
            var fakeSubEntity = new FakeSubEntity("SubUniqueName1", 234, fakeSubSubEntity, 16) { Description = "OriginalSub" };
            var originalCreatedBy = new FakeCreatedBy("CreateUniqueName", 432) { Description = "OriginalCreatedBy" };
            var modifiedBy = new FakeModifiedBy("ModifiedBy1", 433) { Description = "OriginalModifiedBy1" };
            var creationTime = DateTimeOffset.Now.AddDays(-1);
            var expected = new FakeComplexEntity("UniqueName1", fakeSubEntity, FakeEnumeration.FirstFake, originalCreatedBy, creationTime, 22)
            {
                Description = "OriginalComplexEntity1", 
                ModifiedBy = modifiedBy, 
                ModifiedTime = DateTimeOffset.Now.AddHours(1)
            };

            var existing = this.entityMapper.Map<FakeComplexRow>(expected);

            var repositoryAdapter = MockRepository.GenerateMock<IRepositoryAdapter>();
            repositoryAdapter.Stub(adapter => adapter.FirstOrDefault(Arg<ItemSelection<FakeComplexRow>>.Is.Anything)).Return(existing);

            var repositoryAdapterFactory = MockRepository.GenerateMock<IRepositoryAdapterFactory>();
            repositoryAdapterFactory.Stub(factory => factory.Create(Arg<Database>.Is.Anything)).Return(repositoryAdapter);

            using (var provider = new DatabaseRepositoryProvider<FakeDataContext>(repositoryAdapterFactory, this.entityMapper))
            {
                var target = new FakeComplexEntityRepository(provider);
                var actual = target.FirstOrDefault(existing);
                Assert.AreEqual(
                    expected.FakeSubSubEntity, 
                    actual.FakeSubSubEntity, 
                    string.Join(Environment.NewLine, expected.FakeSubSubEntity.GetDifferences(actual.FakeSubSubEntity)));

                Assert.AreEqual(
                    expected.FakeSubEntity, 
                    actual.FakeSubEntity, 
                    string.Join(Environment.NewLine, expected.FakeSubEntity.GetDifferences(actual.FakeSubEntity)));

                Assert.AreEqual(
                    expected.ModifiedBy, 
                    actual.ModifiedBy, 
                    string.Join(Environment.NewLine, expected.ModifiedBy.GetDifferences(actual.ModifiedBy)));

                Assert.AreEqual(
                    expected.CreatedBy, 
                    actual.CreatedBy, 
                    string.Join(Environment.NewLine, expected.CreatedBy.GetDifferences(actual.CreatedBy)));

                Assert.AreEqual(
                    expected, 
                    actual, 
                    string.Join(Environment.NewLine, expected.GetDifferences(actual)));
            }
        }

        /////// <summary>
        /////// The save test.
        /////// </summary>
        ////[TestMethod]
        ////public void Select_ExampleFakeComplexEntity_MatchesExpected()
        ////{
        ////    var fakeSubSubEntity = new FakeSubSubEntity("SubSubUniqueName1", 45) { Description = "OriginalSubSub" };
        ////    var fakeSubEntity = new FakeSubEntity("SubUniqueName1", 234, fakeSubSubEntity, 16) { Description = "OriginalSub" };
        ////    var originalCreatedBy = new FakeCreatedBy("CreateUniqueName", 432) { Description = "OriginalCreatedBy" };
        ////    var modifiedBy = new FakeModifiedBy("ModifiedBy1", 433) { Description = "OriginalModifiedBy1" };
        ////    var creationTime = DateTimeOffset.Now.AddDays(-1);
        ////    var match1 = new FakeComplexEntity("UniqueName1", fakeSubEntity, FakeEnumeration.FirstFake, originalCreatedBy, creationTime, 22)
        ////    {
        ////        Description = "OriginalComplexEntity1", 
        ////        ModifiedBy = modifiedBy, 
        ////        ModifiedTime = DateTimeOffset.Now.AddHours(1)
        ////    };

        ////    var updatedSubSubEntity = new FakeSubSubEntity("SubSubUniqueName2", 46) { Description = "ModifiedSubSub2" };
        ////    var updatedSubEntity = new FakeSubEntity("SubUniqueName2", 235, updatedSubSubEntity, 17) { Description = "ModifiedSub2" };
        ////    var updatedMultiReferenceEntity = new FakeCreatedBy("CreateUniqueName", 432) { Description = "OriginalCreatedBy" };
        ////    var newModifiedBy = new FakeModifiedBy("ModifiedBy2", 434) { Description = "UpdatedModifiedBy2" };
        ////    var match2 = new FakeComplexEntity("UniqueName2", updatedSubEntity, FakeEnumeration.SecondFake,  updatedMultiReferenceEntity)
        ////    {
        ////        Description = "UpdatedEntity2", 
        ////        ModifiedBy = newModifiedBy, 
        ////        ModifiedTime = DateTimeOffset.Now.AddHours(1)
        ////    };

        ////    var entities = new List<FakeComplexEntity> { match1, match2 };
        ////    var matches = entities.Select(this.entityMapper.Map<FakeComplexRow>).ToList();

        ////    var repositoryAdapter = MockRepository.GenerateMock<IRepositoryAdapter>();
        ////    repositoryAdapter.Stub(adapter => adapter.SelectItems(Arg<ItemSelection<FakeComplexRow>>.Is.Anything)).Return(matches);

        ////    var repositoryAdapterFactory = MockRepository.GenerateMock<IRepositoryAdapterFactory>();
        ////    repositoryAdapterFactory.Stub(factory => factory.Create(Arg<Database>.Is.Anything)).Return(repositoryAdapter);

        ////    using (var provider = new DatabaseRepositoryProvider<FakeDataContext>(repositoryAdapterFactory, this.entityMapper))
        ////    {
        ////        var target = new FakeComplexEntityRepository(provider);
        ////        var actual = target.SelectEntities(new ExampleQuery<FakeComplexRow>(matches.First(), row => row.CreatedByUniqueName)).ToList();
        ////        CollectionAssert.AreEqual(entities, actual);
        ////    }
        ////}

        /////// <summary>
        /////// The save test.
        /////// </summary>
        ////[TestMethod]
        ////public void Select_RangeFakeComplexEntity_MatchesExpected()
        ////{
        ////    var fakeSubSubEntity = new FakeSubSubEntity("SubSubUniqueName1", 45) { Description = "OriginalSubSub" };
        ////    var fakeSubEntity = new FakeSubEntity("SubUniqueName1", 234, fakeSubSubEntity, 16) { Description = "OriginalSub" };
        ////    var originalCreatedBy = new FakeCreatedBy("CreateUniqueName", 432) { Description = "OriginalCreatedBy" };
        ////    var modifiedBy = new FakeModifiedBy("ModifiedBy1", 433) { Description = "OriginalModifiedBy1" };
        ////    var creationTime = DateTimeOffset.Now.AddDays(-1);
        ////    var match1 = new FakeComplexEntity("UniqueName1", fakeSubEntity, FakeEnumeration.FirstFake, originalCreatedBy, creationTime, 22)
        ////    {
        ////        Description = "OriginalComplexEntity1", 
        ////        ModifiedBy = modifiedBy, 
        ////        ModifiedTime = DateTimeOffset.Now.AddHours(1)
        ////    };

        ////    var updatedSubSubEntity = new FakeSubSubEntity("SubSubUniqueName2", 46) { Description = "ModifiedSubSub2" };
        ////    var updatedSubEntity = new FakeSubEntity("SubUniqueName2", 235, updatedSubSubEntity, 17) { Description = "ModifiedSub2" };
        ////    var updatedMultiReferenceEntity = new FakeCreatedBy("CreateUniqueName", 432) { Description = "OriginalCreatedBy" };
        ////    var newModifiedBy = new FakeModifiedBy("ModifiedBy2", 434) { Description = "UpdatedModifiedBy2" };
        ////    var match2 = new FakeComplexEntity("UniqueName2", updatedSubEntity, FakeEnumeration.SecondFake, updatedMultiReferenceEntity)
        ////    {
        ////        Description = "UpdatedEntity2", 
        ////        ModifiedBy = newModifiedBy, 
        ////        ModifiedTime = DateTimeOffset.Now.AddHours(1)
        ////    };

        ////    var entities = new List<FakeComplexEntity> { match1, match2 };
        ////    var matches = entities.Select(this.entityMapper.Map<FakeComplexRow>).ToList();

        ////    var repositoryAdapter = MockRepository.GenerateMock<IRepositoryAdapter>();
        ////    repositoryAdapter.Stub(adapter => adapter.SelectItems(Arg<ItemSelection<FakeComplexRow>>.Is.Anything)).Return(matches);

        ////    var repositoryAdapterFactory = MockRepository.GenerateMock<IRepositoryAdapterFactory>();
        ////    repositoryAdapterFactory.Stub(factory => factory.Create(Arg<Database>.Is.Anything)).Return(repositoryAdapter);

        ////    using (var provider = new DatabaseRepositoryProvider<FakeDataContext>(repositoryAdapterFactory, this.entityMapper))
        ////    {
        ////        var target = new FakeComplexEntityRepository(provider);
        ////        var exampleQuery = new ExampleQuery<FakeComplexRow>(matches.First(), matches.Last(), row => row.ModifiedTime);
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

            using (var provider = new DatabaseRepositoryProvider<FakeDataContext>(repositoryAdapterFactory, this.entityMapper))
            {
                var target = new FakeChildEntityRepository(provider);
                var fakeSubSubEntity = new FakeSubSubEntity("SubSubUniqueName");
                var fakeSubEntity = new FakeSubEntity("SubUniqueName", 234, fakeSubSubEntity);
                var fakeCreatedBy = new FakeCreatedBy("CreateUniqueName");
                var modifiedBy = new FakeModifiedBy("ModifiedBy");
                var fakeComplexEntity = new FakeComplexEntity("UniqueName", fakeSubEntity, FakeEnumeration.FirstFake, fakeCreatedBy)
                {
                    ModifiedBy = modifiedBy, 
                    ModifiedTime = DateTimeOffset.Now.AddHours(1)
                };

                var expected = new FakeChildEntity(fakeComplexEntity) { Name = "Foo", SomeValue = 4492 };

                // Save this first because child doesn't save its parent.
                var fakeComplexRepo = new FakeComplexEntityRepository(provider);
                fakeComplexRepo.Save(fakeComplexEntity);

                var actual = target.Save(expected);
                Assert.IsNotNull(actual.FakeSubEntity);
                Assert.IsNotNull(actual.FakeSubEntity.FakeSubSubEntity);
                Assert.IsNotNull(actual.CreatedBy);
                Assert.IsNotNull(actual.ModifiedBy);
                Assert.AreEqual(16, actual.FakeSubEntity.FakeSubEntityId);
                Assert.AreEqual(expected.FakeSubEntity, actual.FakeSubEntity);
                Assert.AreEqual(45, actual.FakeSubSubEntity.FakeSubSubEntityId);
                Assert.AreEqual(expected.FakeSubEntity.FakeSubSubEntity, actual.FakeSubEntity.FakeSubSubEntity);
                Assert.AreEqual(432, actual.CreatedBy.FakeMultiReferenceEntityId);
                Assert.AreEqual(expected.CreatedBy, actual.CreatedBy);
                Assert.AreEqual(432, actual.ModifiedBy.FakeMultiReferenceEntityId);
                Assert.AreEqual(expected.ModifiedBy, actual.ModifiedBy);
                Assert.AreEqual(22, actual.FakeComplexEntityId);
                Assert.AreEqual(expected.FakeComplexEntity, actual.FakeComplexEntity);
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

            using (var provider = new DatabaseRepositoryProvider<FakeDataContext>(repositoryAdapterFactory, this.entityMapper))
            {
                var target = new FakeChildEntityRepository(provider);
                var fakeSubSubEntity = new FakeSubSubEntity("SubSubUniqueName") { Description = "Mah sub sub entity" };
                var fakeSubEntity = new FakeSubEntity("SubUniqueName", 234, fakeSubSubEntity) { Description = "Mah sub entity" };
                var fakeCreatedBy = new FakeCreatedBy("CreateUniqueName") { Description = "Creator" };
                var modifiedBy = new FakeModifiedBy("ModifiedBy") { Description = "Modifier" };
                var fakeComplexEntity = new FakeComplexEntity("UniqueName", fakeSubEntity, FakeEnumeration.FirstFake, fakeCreatedBy)
                {
                    ModifiedBy = modifiedBy, 
                    ModifiedTime = DateTimeOffset.Now.AddHours(1)
                };

                var expected = new FakeChildEntity(fakeComplexEntity) { Name = "Foo", SomeValue = 4492 };
                expected.FakeComplexEntity.SetDependentEntity(33, DateTimeOffset.Now);

                // Save this first because child doesn't save its parent.
                var fakeComplexRepo = new FakeComplexEntityRepository(provider);
                fakeComplexRepo.Save(fakeComplexEntity);

                var actual = target.Save(expected);
                Assert.IsNotNull(actual.FakeSubEntity);
                Assert.IsNotNull(actual.FakeSubEntity.FakeSubSubEntity);
                Assert.IsNotNull(actual.CreatedBy);
                Assert.IsNotNull(actual.ModifiedBy);
                Assert.IsNotNull(actual.FakeDependentEntity);
                Assert.AreEqual(16, actual.FakeSubEntity.FakeSubEntityId);
                Assert.AreEqual(expected.FakeSubEntity, actual.FakeSubEntity);
                Assert.AreEqual(45, actual.FakeSubSubEntity.FakeSubSubEntityId);
                Assert.AreEqual(expected.FakeSubEntity.FakeSubSubEntity, actual.FakeSubEntity.FakeSubSubEntity);
                Assert.AreEqual(432, actual.CreatedBy.FakeMultiReferenceEntityId);
                Assert.AreEqual(expected.CreatedBy, actual.CreatedBy);
                Assert.AreEqual(432, actual.ModifiedBy.FakeMultiReferenceEntityId);
                Assert.AreEqual(expected.ModifiedBy, actual.ModifiedBy);
                Assert.AreEqual(22, actual.FakeComplexEntityId);
                Assert.AreEqual(expected.FakeComplexEntity, actual.FakeComplexEntity);
                Assert.AreEqual(22, actual.FakeDependentEntityId);
                Assert.AreEqual(expected.FakeDependentEntity, actual.FakeDependentEntity);
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
            var fakeSubSubEntity = new FakeSubSubEntity("SubSubUniqueName", 45) { Description = "OriginalSubSub" };
            var fakeSubEntity = new FakeSubEntity("SubUniqueName", 234, fakeSubSubEntity, 16) { Description = "OriginalSub" };
            var originalCreatedBy = new FakeCreatedBy("CreateUniqueName", 432) { Description = "OriginalCreatedBy" };
            var modifiedBy = new FakeModifiedBy("ModifiedBy", 433) { Description = "OriginalModifiedBy" };
            var creationTime = DateTimeOffset.Now.AddDays(-1);
            var complexEntity = new FakeComplexEntity("UniqueName", fakeSubEntity, FakeEnumeration.FirstFake, originalCreatedBy, creationTime, 22)
                                    {
                                        Description = "OriginalComplexEntity",
                                        ModifiedBy = modifiedBy,
                                        ModifiedTime = DateTimeOffset.Now.AddHours(1)
                                    };

            var parent = new FakeChildEntity(complexEntity, 335) { Name = "ParentName", SomeValue = 1 };
            var baseline = new FakeChildEntity(complexEntity, 235) { Name = "OriginalName", SomeValue = 2, Parent = parent };

            var mockFactory = this.CreateComplexMockAdapterFactoryForUpdate(baseline);

            using (var provider = new DatabaseRepositoryProvider<FakeDataContext>(mockFactory.RepositoryAdapterFactory, this.entityMapper))
            {
                var newModifiedBy = new FakeModifiedBy("ModifiedBy", 433) { Description = "UpdatedModifiedBy" };

                var target = new FakeChildEntityRepository(provider);
                var expected = target.FirstOrDefault(22);
                expected.Name = "NewName";
                expected.SomeValue = 242;
                expected.FakeComplexEntity.Description = "UpdatedEntity";
                expected.FakeComplexEntity.ModifiedBy = newModifiedBy;
                expected.FakeComplexEntity.ModifiedTime = DateTimeOffset.Now.AddHours(1);
                expected.FakeSubEntity.Description = "ModifiedSub";
                expected.FakeSubSubEntity.Description = "ModifiedSubSub";
                var actual = target.Save(expected);
                Assert.IsNotNull(actual.FakeSubEntity);
                Assert.IsNotNull(actual.FakeSubEntity.FakeSubSubEntity);
                Assert.IsNotNull(actual.CreatedBy);
                Assert.IsNotNull(actual.ModifiedBy);
                Assert.AreEqual(16, actual.FakeSubEntity.FakeSubEntityId);
                Assert.AreEqual(expected.FakeSubEntity, actual.FakeSubEntity);
                Assert.AreEqual(45, actual.FakeSubSubEntity.FakeSubSubEntityId);
                Assert.AreEqual(expected.FakeSubEntity.FakeSubSubEntity, actual.FakeSubEntity.FakeSubSubEntity);
                Assert.AreEqual(432, actual.CreatedBy.FakeMultiReferenceEntityId);

                Assert.AreEqual(
                    baseline.CreatedBy, 
                    actual.CreatedBy, 
                    string.Join(Environment.NewLine, baseline.CreatedBy.GetDifferences(actual.CreatedBy)));

                Assert.AreEqual(creationTime, actual.FakeComplexEntity.CreationTime);
                Assert.AreEqual(433, actual.ModifiedBy.FakeMultiReferenceEntityId);
                Assert.AreEqual(expected.ModifiedBy, actual.ModifiedBy);
                Assert.AreEqual(expected.FakeComplexEntity.ModifiedTime, actual.FakeComplexEntity.ModifiedTime);
                Assert.AreEqual(22, actual.FakeComplexEntityId);
                Assert.AreEqual(expected.FakeComplexEntity, actual.FakeComplexEntity);
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
            var fakeSubSubEntity = new FakeSubSubEntity("SubSubUniqueName", 45) { Description = "OriginalSubSub" };
            var fakeSubEntity = new FakeSubEntity("SubUniqueName", 234, fakeSubSubEntity, 16) { Description = "OriginalSub" };
            var originalCreatedBy = new FakeCreatedBy("CreateUniqueName", 432) { Description = "OriginalCreatedBy" };
            var modifiedBy = new FakeModifiedBy("ModifiedBy", 433) { Description = "OriginalModifiedBy" };
            var creationTime = DateTimeOffset.Now.AddDays(-1);
            var complexEntity = new FakeComplexEntity("UniqueName", fakeSubEntity, FakeEnumeration.FirstFake, originalCreatedBy, creationTime, 22)
            {
                Description = "OriginalComplexEntity", 
                ModifiedBy = modifiedBy, 
                ModifiedTime = DateTimeOffset.Now.AddHours(1)
            };

            var parent = new FakeChildEntity(complexEntity, 335) { Name = "ParentName", SomeValue = 1 };
            var baseline = new FakeChildEntity(complexEntity, 235) { Name = "OriginalName", SomeValue = 2, Parent = parent };

            var mockFactory = this.CreateComplexMockAdapterFactoryForUpdate(baseline);

            using (var provider = new DatabaseRepositoryProvider<FakeDataContext>(mockFactory.RepositoryAdapterFactory, this.entityMapper))
            {
                var newModifiedBy = new FakeModifiedBy("ModifiedBy", 433) { Description = "UpdatedModifiedBy" };

                var target = new FakeChildEntityRepository(provider);
                var expected = target.FirstOrDefault(22);
                expected.Name = "NewName";
                expected.SomeValue = 242;
                expected.FakeComplexEntity.Description = "UpdatedEntity";
                expected.FakeComplexEntity.ModifiedBy = newModifiedBy;
                expected.FakeComplexEntity.ModifiedTime = DateTimeOffset.Now.AddHours(1);
                expected.FakeSubEntity.Description = "ModifiedSub";
                expected.FakeSubSubEntity.Description = "ModifiedSubSub";
                expected.FakeComplexEntity.SetDependentEntity(33, DateTimeOffset.Now);

                var fakeDependentRow = this.entityMapper.Map<FakeDependentRow>(expected.FakeDependentEntity);
                fakeDependentRow.FakeDependentEntityId = expected.FakeComplexEntityId.GetValueOrDefault();
                mockFactory.RepositoryAdapter.Stub(adapter => adapter.Insert(Arg<FakeDependentRow>.Is.Anything)).Return(fakeDependentRow);

                var actual = target.Save(expected);
                Assert.IsNotNull(actual.FakeSubEntity);
                Assert.IsNotNull(actual.FakeSubEntity.FakeSubSubEntity);
                Assert.IsNotNull(actual.CreatedBy);
                Assert.IsNotNull(actual.ModifiedBy);
                Assert.IsNotNull(actual.FakeDependentEntity);
                Assert.AreEqual(16, actual.FakeSubEntity.FakeSubEntityId);
                Assert.AreEqual(expected.FakeSubEntity, actual.FakeSubEntity);
                Assert.AreEqual(45, actual.FakeSubSubEntity.FakeSubSubEntityId);
                Assert.AreEqual(expected.FakeSubEntity.FakeSubSubEntity, actual.FakeSubEntity.FakeSubSubEntity);
                Assert.AreEqual(432, actual.CreatedBy.FakeMultiReferenceEntityId);

                Assert.AreEqual(
                    baseline.CreatedBy, 
                    actual.CreatedBy, 
                    string.Join(Environment.NewLine, baseline.CreatedBy.GetDifferences(actual.CreatedBy)));

                Assert.AreEqual(creationTime, actual.FakeComplexEntity.CreationTime);
                Assert.AreEqual(433, actual.ModifiedBy.FakeMultiReferenceEntityId);
                Assert.AreEqual(expected.ModifiedBy, actual.ModifiedBy);
                Assert.AreEqual(expected.FakeComplexEntity.ModifiedTime, actual.FakeComplexEntity.ModifiedTime);
                Assert.AreEqual(22, actual.FakeComplexEntityId);
                Assert.AreEqual(expected.FakeComplexEntity, actual.FakeComplexEntity);
                Assert.AreEqual(22, actual.FakeDependentEntityId);
                Assert.AreEqual(expected.FakeDependentEntity, actual.FakeDependentEntity);
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
            var fakeSubSubEntity = new FakeSubSubEntity("SubSubUniqueName", 45) { Description = "OriginalSubSub" };
            var fakeSubEntity = new FakeSubEntity("SubUniqueName", 234, fakeSubSubEntity, 16) { Description = "OriginalSub" };
            var originalCreatedBy = new FakeCreatedBy("CreateUniqueName", 432) { Description = "OriginalCreatedBy" };
            var modifiedBy = new FakeModifiedBy("ModifiedBy", 433) { Description = "OriginalModifiedBy" };
            var creationTime = DateTimeOffset.Now.AddDays(-1);
            var complexEntity = new FakeComplexEntity("UniqueName", fakeSubEntity, FakeEnumeration.FirstFake, originalCreatedBy, creationTime, 22)
                                    {
                                        Description = "OriginalComplexEntity", 
                                        ModifiedBy = modifiedBy, 
                                        ModifiedTime = DateTimeOffset.Now.AddHours(1)
                                    };

            complexEntity.SetDependentEntity(9843, DateTimeOffset.Now.AddHours(-3));

            var parent = new FakeChildEntity(complexEntity, 335) { Name = "ParentName", SomeValue = 1 };
            var baseline = new FakeChildEntity(complexEntity, 235) { Name = "OriginalName", SomeValue = 2, Parent = parent };

            var mockFactory = this.CreateComplexMockAdapterFactoryForUpdate(baseline);
            mockFactory.RepositoryAdapter.Stub(
                adapter =>
                adapter.Update(
                    Arg<FakeDependentRow>.Is.Anything,
                    Arg<ItemSelection<FakeDependentRow>>.Is.Anything,
                    Arg<Expression<Func<FakeDependentRow, object>>[]>.Is.Anything)).Return(1);

            using (var provider = new DatabaseRepositoryProvider<FakeDataContext>(mockFactory.RepositoryAdapterFactory, this.entityMapper))
            {
                var newModifiedBy = new FakeModifiedBy("ModifiedBy", 433) { Description = "UpdatedModifiedBy" };

                var target = new FakeChildEntityRepository(provider);
                var expected = target.FirstOrDefault(22);
                expected.Name = "NewName";
                expected.SomeValue = 242;
                expected.FakeComplexEntity.Description = "UpdatedEntity";
                expected.FakeComplexEntity.ModifiedBy = newModifiedBy;
                expected.FakeComplexEntity.ModifiedTime = DateTimeOffset.Now.AddHours(1);
                expected.FakeSubEntity.Description = "ModifiedSub";
                expected.FakeSubSubEntity.Description = "ModifiedSubSub";
                expected.FakeComplexEntity.SetDependentEntity(33, DateTimeOffset.Now);

                var fakeDependentRow = this.entityMapper.Map<FakeDependentRow>(expected.FakeDependentEntity);
                fakeDependentRow.FakeDependentEntityId = expected.FakeComplexEntityId.GetValueOrDefault();
                mockFactory.RepositoryAdapter.Stub(adapter => adapter.FirstOrDefault(Arg<ItemSelection<FakeDependentRow>>.Is.Anything))
                    .Return(fakeDependentRow);

                var actual = target.Save(expected);
                Assert.IsNotNull(actual.FakeSubEntity);
                Assert.IsNotNull(actual.FakeSubEntity.FakeSubSubEntity);
                Assert.IsNotNull(actual.CreatedBy);
                Assert.IsNotNull(actual.ModifiedBy);
                Assert.IsNotNull(actual.FakeDependentEntity);
                Assert.AreEqual(16, actual.FakeSubEntity.FakeSubEntityId);
                Assert.AreEqual(expected.FakeSubEntity, actual.FakeSubEntity);
                Assert.AreEqual(45, actual.FakeSubSubEntity.FakeSubSubEntityId);
                Assert.AreEqual(expected.FakeSubEntity.FakeSubSubEntity, actual.FakeSubEntity.FakeSubSubEntity);
                Assert.AreEqual(432, actual.CreatedBy.FakeMultiReferenceEntityId);

                Assert.AreEqual(
                    baseline.CreatedBy, 
                    actual.CreatedBy, 
                    string.Join(Environment.NewLine, baseline.CreatedBy.GetDifferences(actual.CreatedBy)));

                Assert.AreEqual(creationTime, actual.FakeComplexEntity.CreationTime);
                Assert.AreEqual(433, actual.ModifiedBy.FakeMultiReferenceEntityId);
                Assert.AreEqual(expected.ModifiedBy, actual.ModifiedBy);
                Assert.AreEqual(expected.FakeComplexEntity.ModifiedTime, actual.FakeComplexEntity.ModifiedTime);
                Assert.AreEqual(22, actual.FakeComplexEntityId);
                Assert.AreEqual(expected.FakeComplexEntity, actual.FakeComplexEntity);
                Assert.AreEqual(22, actual.FakeDependentEntityId);
                Assert.AreEqual(expected.FakeDependentEntity, actual.FakeDependentEntity);
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
            var fakeSubSubEntity = new FakeSubSubEntity("SubSubUniqueName", 45) { Description = "OriginalSubSub" };
            var fakeSubEntity = new FakeSubEntity("SubUniqueName", 234, fakeSubSubEntity, 16) { Description = "OriginalSub" };
            var originalCreatedBy = new FakeCreatedBy("CreateUniqueName", 432) { Description = "OriginalCreatedBy" };
            var modifiedBy = new FakeModifiedBy("ModifiedBy", 433) { Description = "OriginalModifiedBy" };
            var creationTime = DateTimeOffset.Now.AddDays(-1);
            var complexEntity = new FakeComplexEntity("UniqueName", fakeSubEntity, FakeEnumeration.FirstFake, originalCreatedBy, creationTime, 22)
                                    {
                                        Description = "OriginalComplexEntity",
                                        ModifiedBy = modifiedBy,
                                        ModifiedTime = DateTimeOffset.Now.AddHours(1)
                                    };

            complexEntity.SetDependentEntity(22, DateTimeOffset.Now.AddHours(-3));

            var parent = new FakeChildEntity(complexEntity, 335) { Name = "ParentName", SomeValue = 1 };
            var baseline = new FakeChildEntity(complexEntity, 235) { Name = "OriginalName", SomeValue = 2, Parent = parent };

            var mockFactory = this.CreateComplexMockAdapterFactoryForUpdate(baseline);
            mockFactory.RepositoryAdapter.Stub(adapter => adapter.DeleteSelection(Arg<ItemSelection<FakeDependentRow>>.Is.Anything)).Return(1);

            using (var provider = new DatabaseRepositoryProvider<FakeDataContext>(mockFactory.RepositoryAdapterFactory, this.entityMapper))
            {
                var newModifiedBy = new FakeModifiedBy("ModifiedBy", 433) { Description = "UpdatedModifiedBy" };

                var target = new FakeChildEntityRepository(provider);
                var expected = target.FirstOrDefault(22);
                expected.Name = "NewName";
                expected.SomeValue = 242;
                expected.FakeComplexEntity.Description = "UpdatedEntity";
                expected.FakeComplexEntity.ModifiedBy = newModifiedBy;
                expected.FakeComplexEntity.ModifiedTime = DateTimeOffset.Now.AddHours(1);
                expected.FakeSubEntity.Description = "ModifiedSub";
                expected.FakeSubSubEntity.Description = "ModifiedSubSub";
                expected.FakeComplexEntity.SetDependentEntity(0);
                var actual = target.Save(expected);
                Assert.IsNotNull(actual.FakeSubEntity);
                Assert.IsNotNull(actual.FakeSubEntity.FakeSubSubEntity);
                Assert.IsNotNull(actual.CreatedBy);
                Assert.IsNotNull(actual.ModifiedBy);
                Assert.IsNull(actual.FakeDependentEntity);
                Assert.AreEqual(16, actual.FakeSubEntity.FakeSubEntityId);
                Assert.AreEqual(expected.FakeSubEntity, actual.FakeSubEntity);
                Assert.AreEqual(45, actual.FakeSubSubEntity.FakeSubSubEntityId);
                Assert.AreEqual(expected.FakeSubEntity.FakeSubSubEntity, actual.FakeSubEntity.FakeSubSubEntity);
                Assert.AreEqual(432, actual.CreatedBy.FakeMultiReferenceEntityId);

                Assert.AreEqual(
                    baseline.CreatedBy,
                    actual.CreatedBy,
                    string.Join(Environment.NewLine, baseline.CreatedBy.GetDifferences(actual.CreatedBy)));

                Assert.AreEqual(creationTime, actual.FakeComplexEntity.CreationTime);
                Assert.AreEqual(433, actual.ModifiedBy.FakeMultiReferenceEntityId);
                Assert.AreEqual(expected.ModifiedBy, actual.ModifiedBy);
                Assert.AreEqual(expected.FakeComplexEntity.ModifiedTime, actual.FakeComplexEntity.ModifiedTime);
                Assert.AreEqual(22, actual.FakeComplexEntityId);
                Assert.AreEqual(expected.FakeComplexEntity, actual.FakeComplexEntity);
                Assert.AreEqual(expected.FakeDependentEntity, actual.FakeDependentEntity);
                Assert.AreEqual(235, actual.FakeChildEntityId);
                Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
            }
        }

        /// <summary>
        /// The save test.
        /// </summary>
        [TestMethod]
        public void Select_FirstOrDefaultFakeChildEntity_MatchesExpected()
        {
            var fakeSubSubEntity = new FakeSubSubEntity("SubSubUniqueName1", 45) { Description = "OriginalSubSub" };
            var fakeSubEntity = new FakeSubEntity("SubUniqueName1", 234, fakeSubSubEntity, 16) { Description = "OriginalSub" };
            var originalCreatedBy = new FakeCreatedBy("CreateUniqueName", 432) { Description = "OriginalCreatedBy" };
            var modifiedBy = new FakeModifiedBy("ModifiedBy1", 433) { Description = "OriginalModifiedBy1" };
            var creationTime = DateTimeOffset.Now.AddDays(-1);
            var fakeComplexEntity = new FakeComplexEntity("UniqueName1", fakeSubEntity, FakeEnumeration.FirstFake, originalCreatedBy, creationTime, 22)
            {
                Description = "OriginalComplexEntity1", 
                ModifiedBy = modifiedBy, 
                ModifiedTime = DateTimeOffset.Now.AddHours(1)
            };

            var expected = new FakeChildEntity(fakeComplexEntity, 435) { Name = "OriginalName", SomeValue = 111 };

            var existing = this.entityMapper.Map<FakeChildRow>(expected);
            var repositoryAdapter = MockRepository.GenerateMock<IRepositoryAdapter>();
            repositoryAdapter.Stub(adapter => adapter.FirstOrDefault(Arg<ItemSelection<FakeChildRow>>.Is.Anything)).Return(existing);

            var repositoryAdapterFactory = MockRepository.GenerateMock<IRepositoryAdapterFactory>();
            repositoryAdapterFactory.Stub(factory => factory.Create(Arg<Database>.Is.Anything)).Return(repositoryAdapter);

            using (var provider = new DatabaseRepositoryProvider<FakeDataContext>(repositoryAdapterFactory, this.entityMapper))
            {
                var target = new FakeChildEntityRepository(provider);
                var actual = target.FirstOrDefault(existing);
                Assert.AreEqual(
                    expected.FakeSubSubEntity, 
                    actual.FakeSubSubEntity, 
                    string.Join(Environment.NewLine, expected.FakeSubSubEntity.GetDifferences(actual.FakeSubSubEntity)));

                Assert.AreEqual(
                    expected.FakeSubEntity, 
                    actual.FakeSubEntity, 
                    string.Join(Environment.NewLine, expected.FakeSubEntity.GetDifferences(actual.FakeSubEntity)));

                Assert.AreEqual(
                    expected.ModifiedBy, 
                    actual.ModifiedBy, 
                    string.Join(Environment.NewLine, expected.ModifiedBy.GetDifferences(actual.ModifiedBy)));

                Assert.AreEqual(
                    expected.CreatedBy, 
                    actual.CreatedBy, 
                    string.Join(Environment.NewLine, expected.CreatedBy.GetDifferences(actual.CreatedBy)));

                Assert.AreEqual(
                    expected.FakeComplexEntity, 
                    actual.FakeComplexEntity, 
                    string.Join(Environment.NewLine, expected.FakeComplexEntity.GetDifferences(actual.FakeComplexEntity)));

                Assert.AreEqual(
                    expected, 
                    actual, 
                    string.Join(Environment.NewLine, expected.GetDifferences(actual)));
            }
        }

        /// <summary>
        /// The save test.
        /// </summary>
        [TestMethod]
        public void Select_FirstOrDefaultFakeChildEntityWithDependentEntity_MatchesExpected()
        {
            var fakeSubSubEntity = new FakeSubSubEntity("SubSubUniqueName1", 45) { Description = "OriginalSubSub" };
            var fakeSubEntity = new FakeSubEntity("SubUniqueName1", 234, fakeSubSubEntity, 16) { Description = "OriginalSub" };
            var originalCreatedBy = new FakeCreatedBy("CreateUniqueName", 432) { Description = "OriginalCreatedBy" };
            var modifiedBy = new FakeModifiedBy("ModifiedBy1", 433) { Description = "OriginalModifiedBy1" };
            var creationTime = DateTimeOffset.Now.AddDays(-1);
            var fakeComplexEntity = new FakeComplexEntity("UniqueName1", fakeSubEntity, FakeEnumeration.FirstFake, originalCreatedBy, creationTime, 22)
            {
                Description = "OriginalComplexEntity1",
                ModifiedBy = modifiedBy,
                ModifiedTime = DateTimeOffset.Now.AddHours(1)
            };

            fakeComplexEntity.SetDependentEntity(994);

            var expected = new FakeChildEntity(fakeComplexEntity, 435) { Name = "OriginalName", SomeValue = 111 };

            var existing = this.entityMapper.Map<FakeChildRow>(expected);
            var repositoryAdapter = MockRepository.GenerateMock<IRepositoryAdapter>();
            repositoryAdapter.Stub(adapter => adapter.FirstOrDefault(Arg<ItemSelection<FakeChildRow>>.Is.Anything)).Return(existing);

            var repositoryAdapterFactory = MockRepository.GenerateMock<IRepositoryAdapterFactory>();
            repositoryAdapterFactory.Stub(factory => factory.Create(Arg<Database>.Is.Anything)).Return(repositoryAdapter);

            using (var provider = new DatabaseRepositoryProvider<FakeDataContext>(repositoryAdapterFactory, this.entityMapper))
            {
                var target = new FakeChildEntityRepository(provider);
                var actual = target.FirstOrDefault(existing);
                Assert.IsNotNull(expected.FakeDependentEntity);
                Assert.IsNotNull(expected.FakeDependentEntityId);
                Assert.AreEqual(
                    expected.FakeSubSubEntity,
                    actual.FakeSubSubEntity,
                    string.Join(Environment.NewLine, expected.FakeSubSubEntity.GetDifferences(actual.FakeSubSubEntity)));

                Assert.AreEqual(
                    expected.FakeSubEntity,
                    actual.FakeSubEntity,
                    string.Join(Environment.NewLine, expected.FakeSubEntity.GetDifferences(actual.FakeSubEntity)));

                Assert.AreEqual(
                    expected.ModifiedBy,
                    actual.ModifiedBy,
                    string.Join(Environment.NewLine, expected.ModifiedBy.GetDifferences(actual.ModifiedBy)));

                Assert.AreEqual(
                    expected.CreatedBy,
                    actual.CreatedBy,
                    string.Join(Environment.NewLine, expected.CreatedBy.GetDifferences(actual.CreatedBy)));

                Assert.AreEqual(
                    expected.FakeDependentEntity,
                    actual.FakeDependentEntity,
                    string.Join(Environment.NewLine, expected.FakeDependentEntity.GetDifferences(actual.FakeDependentEntity)));

                Assert.AreEqual(
                    expected.FakeComplexEntity,
                    actual.FakeComplexEntity,
                    string.Join(Environment.NewLine, expected.FakeComplexEntity.GetDifferences(actual.FakeComplexEntity)));

                Assert.AreEqual(
                    expected,
                    actual,
                    string.Join(Environment.NewLine, expected.GetDifferences(actual)));
            }
        }

        /////// <summary>
        /////// The save test.
        /////// </summary>
        ////[TestMethod]
        ////public void Select_ExampleFakeChildEntity_MatchesExpected()
        ////{
        ////    var fakeSubSubEntity = new FakeSubSubEntity("SubSubUniqueName1", 45) { Description = "OriginalSubSub" };
        ////    var fakeSubEntity = new FakeSubEntity("SubUniqueName1", 234, fakeSubSubEntity, 16) { Description = "OriginalSub" };
        ////    var originalCreatedBy = new FakeCreatedBy("CreateUniqueName", 432) { Description = "OriginalCreatedBy" };
        ////    var modifiedBy = new FakeModifiedBy("ModifiedBy1", 433) { Description = "OriginalModifiedBy1" };
        ////    var creationTime = DateTimeOffset.Now.AddDays(-1);
        ////    var fakeComplexEntity = new FakeComplexEntity("UniqueName1", fakeSubEntity, FakeEnumeration.FirstFake, originalCreatedBy, creationTime, 22)
        ////    {
        ////        Description = "OriginalComplexEntity1", 
        ////        ModifiedBy = modifiedBy, 
        ////        ModifiedTime = DateTimeOffset.Now.AddHours(1)
        ////    };

        ////    var match1 = new FakeChildEntity(fakeComplexEntity, 235) { Name = "OriginalName", SomeValue = 111 };
        ////    var match2 = new FakeChildEntity(fakeComplexEntity, 236) { Name = "AnotherName", SomeValue = 112, Parent = match1 };
        ////    var match3 = new FakeChildEntity(fakeComplexEntity, 237) { Name = "YetAnotherName", SomeValue = 113, Parent = match2 };

        ////    var entities = new List<FakeChildEntity> { match1, match2, match3 };
        ////    var matches = entities.Select(this.entityMapper.Map<FakeChildRow>).ToList();

        ////    var repositoryAdapter = MockRepository.GenerateMock<IRepositoryAdapter>();
        ////    repositoryAdapter.Stub(adapter => adapter.SelectItems(Arg<ItemSelection<FakeChildRow>>.Is.Anything)).Return(matches);

        ////    var repositoryAdapterFactory = MockRepository.GenerateMock<IRepositoryAdapterFactory>();
        ////    repositoryAdapterFactory.Stub(factory => factory.Create(Arg<Database>.Is.Anything)).Return(repositoryAdapter);

        ////    using (var provider = new DatabaseRepositoryProvider<FakeDataContext>(repositoryAdapterFactory, this.entityMapper))
        ////    {
        ////        var target = new FakeChildEntityRepository(provider);
        ////        var actual = target.SelectEntities(new ExampleQuery<FakeChildRow>(matches.First(), row => row.CreatedByUniqueName)).ToList();
        ////        Assert.IsTrue(actual.Any());
        ////        Assert.AreEqual(
        ////            entities.Last(), 
        ////            actual.Last(), 
        ////            string.Join(Environment.NewLine, entities.Last().GetDifferences(actual.Last())));

        ////        CollectionAssert.AreEqual(entities, actual);

        ////        var firstComplexEntity = actual.First().FakeComplexEntity;

        ////        foreach (var childEntity in actual)
        ////        {
        ////            Assert.IsTrue(ReferenceEquals(firstComplexEntity, childEntity.FakeComplexEntity));
        ////        }
        ////    }
        ////}

        /////// <summary>
        /////// The save test.
        /////// </summary>
        ////[TestMethod]
        ////public void Select_RangeFakeChildEntity_MatchesExpected()
        ////{
        ////    var fakeSubSubEntity = new FakeSubSubEntity("SubSubUniqueName1", 45) { Description = "OriginalSubSub" };
        ////    var fakeSubEntity = new FakeSubEntity("SubUniqueName1", 234, fakeSubSubEntity, 16) { Description = "OriginalSub" };
        ////    var originalCreatedBy = new FakeCreatedBy("CreateUniqueName", 432) { Description = "OriginalCreatedBy" };
        ////    var modifiedBy = new FakeModifiedBy("ModifiedBy1", 433) { Description = "OriginalModifiedBy1" };
        ////    var creationTime = DateTimeOffset.Now.AddDays(-1);
        ////    var fakeComplexEntity = new FakeComplexEntity("UniqueName1", fakeSubEntity, FakeEnumeration.FirstFake, originalCreatedBy, creationTime, 22)
        ////    {
        ////        Description = "OriginalComplexEntity1", 
        ////        ModifiedBy = modifiedBy, 
        ////        ModifiedTime = DateTimeOffset.Now.AddHours(1)
        ////    };

        ////    var match1 = new FakeChildEntity(fakeComplexEntity, 235) { Name = "OriginalName", SomeValue = 111 };
        ////    var match2 = new FakeChildEntity(fakeComplexEntity, 236) { Name = "AnotherName", SomeValue = 112, Parent = match1 };
        ////    var match3 = new FakeChildEntity(fakeComplexEntity, 237) { Name = "YetAnotherName", SomeValue = 113, Parent = match1 };

        ////    var entities = new List<FakeChildEntity> { match1, match2, match3 };
        ////    var matches = entities.Select(this.entityMapper.Map<FakeChildRow>).ToList();

        ////    var repositoryAdapter = MockRepository.GenerateMock<IRepositoryAdapter>();
        ////    repositoryAdapter.Stub(adapter => adapter.SelectItems(Arg<ItemSelection<FakeChildRow>>.Is.Anything)).Return(matches);

        ////    var repositoryAdapterFactory = MockRepository.GenerateMock<IRepositoryAdapterFactory>();
        ////    repositoryAdapterFactory.Stub(factory => factory.Create(Arg<Database>.Is.Anything)).Return(repositoryAdapter);

        ////    using (var provider = new DatabaseRepositoryProvider<FakeDataContext>(repositoryAdapterFactory, this.entityMapper))
        ////    {
        ////        var target = new FakeChildEntityRepository(provider);
        ////        var exampleQuery = new ExampleQuery<FakeChildRow>(matches.First(), matches.Last(), row => row.FakeComplexEntityModifiedTime);
        ////        var actual = target.SelectEntities(exampleQuery).ToList();
        ////        CollectionAssert.AreEqual(entities, actual);

        ////        var firstComplexEntity = actual.First().FakeComplexEntity;

        ////        foreach (var childEntity in actual)
        ////        {
        ////            Assert.IsTrue(ReferenceEquals(firstComplexEntity, childEntity.FakeComplexEntity));
        ////        }
        ////    }
        ////}

        /// <summary>
        /// The load children_ fake child entities_ matches expected.
        /// </summary>
        [TestMethod]
        public void FirstOrDefaultWithChildren_FakeChildEntities_MatchesExpected()
        {
            var fakeSubSubEntity = new FakeSubSubEntity("foobar", 112233) { Description = "FakeSubSubEntity" };
            var fakeSubEntity = new FakeSubEntity("bar", 949, fakeSubSubEntity, 4587) { Description = "FakeSubEntity" };
            var createdBy = new FakeCreatedBy("createdBy", 49430) { Description = "The CREATOR" };
            var modifiedBy = new FakeModifiedBy("Modifier", 999291);
            var expected = new FakeComplexEntity("foo", fakeSubEntity, FakeEnumeration.ThirdFake, createdBy, DateTimeOffset.Now, 99291)
            {
                Description = "FakeComplexEntity",
                ModifiedBy = modifiedBy
            };

            var parent1 = new FakeChildEntity(expected, 4994) { Name = "Parent1", SomeValue = 993 };
            var parent2 = new FakeChildEntity(expected, 5002) { Name = "Parent2", SomeValue = 5573 };
            var children = new List<FakeChildEntity>
                                   {
                                       parent1,
                                       parent2,
                                       new FakeChildEntity(expected, 49291)
                                           {
                                               Name = "Child1",
                                               SomeValue = 24441,
                                               Parent = parent1
                                           },
                                       new FakeChildEntity(expected, 50282)
                                           {
                                               Name = "Child2",
                                               SomeValue = 244389,
                                               Parent = parent1
                                           },
                                       new FakeChildEntity(expected, 66939)
                                           {
                                               Name = "Child3",
                                               SomeValue = 48932,
                                               Parent = parent2
                                           }
                                   };

            var childRows = children.OrderByDescending(x => x.SomeValue).Select(this.entityMapper.Map<FakeChildRow>).ToList();
            var entityRow = this.entityMapper.Map<FakeComplexRow>(expected);

            var repositoryAdapter = MockRepository.GenerateMock<IRepositoryAdapter>();
            repositoryAdapter.Stub(adapter => adapter.SelectItems(Arg<ItemSelection<FakeChildRow>>.Is.Anything)).Return(childRows);
            repositoryAdapter.Stub(adapter => adapter.FirstOrDefault(Arg<ItemSelection<FakeComplexRow>>.Is.Anything)).Return(entityRow);

            var repositoryAdapterFactory = MockRepository.GenerateMock<IRepositoryAdapterFactory>();
            repositoryAdapterFactory.Stub(factory => factory.Create(Arg<Database>.Is.Anything)).Return(repositoryAdapter);

            using (var provider = new DatabaseRepositoryProvider<FakeDataContext>(repositoryAdapterFactory, this.entityMapper))
            {
                var target = new FakeComplexEntityRepository(provider);
                var actual = target.FirstOrDefaultWithChildren(99291);
                Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
                FakeChildEntity selectedParent1 = null;
                FakeChildEntity selectedParent2 = null;

                foreach (var childEntity in actual.ChildEntities)
                {
                    Assert.IsTrue(ReferenceEquals(actual, childEntity.FakeComplexEntity));

                    if (childEntity.Name == "Parent1")
                    {
                        selectedParent1 = childEntity;
                    }

                    if (childEntity.Name == "Parent2")
                    {
                        selectedParent2 = childEntity;
                    }

                    if (new[] { "Child1", "Child2" }.Contains(childEntity.Name))
                    {
                        Assert.IsTrue(ReferenceEquals(selectedParent1, childEntity.Parent));
                    }

                    if (new[] { "Child3" }.Contains(childEntity.Name))
                    {
                        Assert.IsTrue(ReferenceEquals(selectedParent2, childEntity.Parent));
                    }
                }
            }
        }
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
                        configuration.AddProfile<FakeRaisedSubEntityMappingProfile>();
                        configuration.AddProfile<FakeChildEntityMappingProfile>();
                        configuration.AddProfile<FakeComplexEntityMappingProfile>();
                        configuration.AddProfile<FakeRaisedComplexEntityMappingProfile>();
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
            CreateInsertMock<FakeComplexRow>(repositoryAdapter, row => row.FakeComplexEntityId, 22);
            CreateInsertMock<FakeSubSubRow>(repositoryAdapter, row => row.FakeSubSubEntityId, 45);
            CreateInsertMock<FakeFlatSubRow>(repositoryAdapter, row => row.FakeSubEntityId, 16);
            CreateInsertMock<FakeMultiReferenceRow>(repositoryAdapter, row => row.FakeMultiReferenceEntityId, 432);
            CreateInsertMock<FakeDependencyRow>(repositoryAdapter, row => row.FakeDependencyEntityId, 6);
            CreateInsertMock<FakeDependentRow>(repositoryAdapter, row => row.FakeDependentEntityId, 22);

            repositoryAdapter.Expect(adapter => adapter.Insert(Arg<FakeChildRow>.Is.Anything)).Return(null).WhenCalled(
                invocation =>
                    {
                        var childRow = invocation.Arguments.OfType<FakeChildRow>().First();

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
            object keyValue) where TItem : class, ITransactionContext
        {
            repositoryAdapter.Stub(adapter => adapter.Insert(Arg<TItem>.Is.Anything)).Return(null).WhenCalled(
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
        private MockedRepositoryAdapter CreateComplexMockAdapterFactoryForUpdate(FakeChildEntity entity)
        {
            var repositoryAdapter = MockRepository.GenerateMock<IRepositoryAdapter>();

            this.CreateMockAdapterForEntity(entity.FakeComplexEntity, repositoryAdapter);

            repositoryAdapter.Stub(adapter => adapter.Contains(Arg<ItemSelection<FakeChildRow>>.Is.Anything)).Return(true);

            repositoryAdapter.Stub(adapter => adapter.FirstOrDefault(Arg<ItemSelection<FakeChildRow>>.Is.Anything))
                .Return(this.entityMapper.Map<FakeChildRow>(entity));

            var repositoryAdapterFactory = MockRepository.GenerateMock<IRepositoryAdapterFactory>();
            repositoryAdapterFactory.Stub(factory => factory.Create(Arg<Database>.Is.Anything)).Return(repositoryAdapter);
            return new MockedRepositoryAdapter { RepositoryAdapterFactory = repositoryAdapterFactory, RepositoryAdapter = repositoryAdapter };
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
        private void CreateMockAdapterForEntity(FakeComplexEntity entity, IRepositoryAdapter repositoryAdapter)
        {
            repositoryAdapter.Stub(
                adapter =>
                adapter.Update(
                    Arg<FakeChildRow>.Is.Anything,
                    Arg<ItemSelection<FakeChildRow>>.Is.Anything,
                    Arg<Expression<Func<FakeChildRow, object>>[]>.Is.Anything)).Return(1);

            repositoryAdapter.Stub(
                adapter =>
                adapter.Update(
                    Arg<FakeComplexRow>.Is.Anything,
                    Arg<ItemSelection<FakeComplexRow>>.Is.Anything,
                    Arg<Expression<Func<FakeComplexRow, object>>[]>.Is.Anything)).Return(1);

            repositoryAdapter.Stub(
                adapter =>
                adapter.Update(
                    Arg<FakeSubSubRow>.Is.Anything,
                    Arg<ItemSelection<FakeSubSubRow>>.Is.Anything,
                    Arg<Expression<Func<FakeSubSubRow, object>>[]>.Is.Anything)).Return(1);

            repositoryAdapter.Stub(
                adapter =>
                adapter.Update(
                    Arg<FakeFlatSubRow>.Is.Anything,
                    Arg<ItemSelection<FakeSubRow>>.Is.Anything,
                    Arg<Expression<Func<FakeSubRow, object>>[]>.Is.Anything)).Return(1);

            repositoryAdapter.Stub(
                adapter =>
                adapter.Update(
                    Arg<FakeMultiReferenceRow>.Is.Anything,
                    Arg<ItemSelection<FakeMultiReferenceRow>>.Is.Anything,
                    Arg<Expression<Func<FakeMultiReferenceRow, object>>[]>.Is.Anything)).Return(1);

            repositoryAdapter.Stub(
                adapter =>
                adapter.Update(
                    Arg<FakeDependencyRow>.Is.Anything,
                    Arg<ItemSelection<FakeDependencyRow>>.Is.Anything,
                    Arg<Expression<Func<FakeDependencyRow, object>>[]>.Is.Anything)).Return(1);

            repositoryAdapter.Stub(
                adapter =>
                adapter.Update(
                    Arg<FakeDependentRow>.Is.Anything,
                    Arg<ItemSelection<FakeDependentRow>>.Is.Anything,
                    Arg<Expression<Func<FakeDependentRow, object>>[]>.Is.Anything)).Return(1);

            repositoryAdapter.Stub(adapter => adapter.FirstOrDefault(Arg<ItemSelection<FakeComplexRow>>.Is.Anything))
                .Return(this.entityMapper.Map<FakeComplexRow>(entity));

            repositoryAdapter.Stub(adapter => adapter.FirstOrDefault(Arg<ItemSelection<FakeSubSubRow>>.Is.Anything))
                .Return(this.entityMapper.Map<FakeSubSubRow>(entity.FakeSubSubEntity));

            repositoryAdapter.Stub(adapter => adapter.FirstOrDefault(Arg<ItemSelection<FakeFlatSubRow>>.Is.Anything))
                .Return(this.entityMapper.Map<FakeFlatSubRow>(entity.FakeSubEntityId));

            CreateInsertMock<FakeDependentRow>(repositoryAdapter, row => row.FakeDependentEntityId, entity.FakeComplexEntityId);

            repositoryAdapter.Stub(
                adapter =>
                adapter.FirstOrDefault(
                    Arg<ItemSelection<FakeMultiReferenceRow>>.Matches(selection => selection.PropertyValues.Any(x => x.Equals(432)))))
                .Return(this.entityMapper.Map<FakeMultiReferenceRow>(entity.CreatedBy));

            repositoryAdapter.Stub(
                adapter =>
                adapter.FirstOrDefault(
                    Arg<ItemSelection<FakeMultiReferenceRow>>.Matches(selection => selection.PropertyValues.Any(x => x.Equals(433)))))
                .Return(this.entityMapper.Map<FakeMultiReferenceRow>(entity.ModifiedBy));
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

        /// <summary>
        /// The constructed sub sub entity profile.
        /// </summary>
        private class ConstructedSubSubEntityMappingProfile : ConstructedEntityMappingProfile<FakeSubSubEntity, FakeSubSubRow>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ConstructedSubSubEntityMappingProfile"/> class.
            /// </summary>
            public ConstructedSubSubEntityMappingProfile()
            {
                this.SetPrimaryKey(entity => entity.FakeSubSubEntityId, row => row.FakeSubSubEntityId);
            }
        }

        /// <summary>
        /// The constructed sub entity mapping profile.
        /// </summary>
        private class ConstructedSubEntityMappingProfile : ConstructedEntityMappingProfile<FakeSubEntity, FakeSubRow>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ConstructedSubEntityMappingProfile"/> class.
            /// </summary>
            public ConstructedSubEntityMappingProfile()
            {
                this.SetPrimaryKey(entity => entity.FakeSubEntityId, row => row.FakeSubEntityId);
            }
        }

        /// <summary>
        /// The constructed dependent entity mapping profile.
        /// </summary>
        private class ConstructedDependentEntityMappingProfile : ConstructedEntityMappingProfile<FakeDependentEntity, FakeDependentRow>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ConstructedDependentEntityMappingProfile"/> class.
            /// </summary>
            public ConstructedDependentEntityMappingProfile()
            {
                this.SetPrimaryKey(entity => entity.FakeDependentEntityId, row => row.FakeDependentEntityId);
            }
        }

        /// <summary>
        /// The constructed complex entity mapping profile.
        /// </summary>
        private class ConstructedComplexEntityMappingProfile : ConstructedEntityMappingProfile<FakeComplexEntity, FakeComplexRow>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ConstructedComplexEntityMappingProfile"/> class.
            /// </summary>
            public ConstructedComplexEntityMappingProfile()
            {
                this.SetPrimaryKey(entity => entity.FakeComplexEntityId, row => row.FakeComplexEntityId)
                    .MapProperty(entity => entity.FakeEnumeration, row => row.FakeEnumerationId)
                    .MapProperty(entity => entity.FakeOtherEnumeration, row => row.FakeOtherEnumerationId);
            }
        }

        /// <summary>
        /// The constructed fake complex entity repository.
        /// </summary>
        private class ConstructedFakeComplexEntityRepository : EntityRepository<FakeComplexEntity, FakeComplexRow>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ConstructedFakeComplexEntityRepository"/> class.
            /// </summary>
            /// <param name="repositoryProvider">
            /// The repository provider.
            /// </param>
            public ConstructedFakeComplexEntityRepository(IRepositoryProvider repositoryProvider)
                : base(repositoryProvider)
            {
            }

            /// <summary>
            /// Gets a unique item selection for the specified item.
            /// </summary>
            /// <param name="item">
            /// The item to create the selection for.
            /// </param>
            /// <returns>
            /// A <see cref="T:SAF.Data.ItemSelection`1"/> for the specified item.
            /// </returns>
            protected override ItemSelection<FakeComplexRow> GetUniqueItemSelection(FakeComplexRow item)
            {
                return this.GetKeySelection(item, row => row.FakeComplexEntityId, row => row.UniqueName);
            }

            /// <summary>
            /// Constructs the entity for the specified data item.
            /// </summary>
            /// <param name="dataItem">
            /// The data item to construct an entity for.
            /// </param>
            /// <param name="repositoryProvider">
            /// The repository provider.
            /// </param>
            /// <returns>
            /// A new instance of the entity.
            /// </returns>
            protected override FakeComplexEntity ConstructEntity(FakeComplexRow dataItem, IRepositoryProvider repositoryProvider)
            {
                var fakeSubEntity = repositoryProvider.EntityMapper.Map<FakeSubEntity>(dataItem);
                var fakeCreatedBy = new FakeCreatedBy(dataItem.CreatedByUniqueName, dataItem.CreatedByFakeMultiReferenceEntityId)
                                        {
                                            Description = dataItem.CreatedByDescription
                                        };

                return new FakeComplexEntity(dataItem.UniqueName, fakeSubEntity, (FakeEnumeration)dataItem.FakeEnumerationId, fakeCreatedBy);
            }
        }
    }
}