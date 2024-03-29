﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityRepositoryTests.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The entity repository tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common.Tests
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using global::AutoMapper;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Startitecture.Core;
    using Startitecture.Orm.AutoMapper;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Schema;
    using Startitecture.Orm.Testing.Entities;
    using Startitecture.Orm.Testing.Model;

    /// <summary>
    /// The entity repository tests.
    /// </summary>
    [TestClass]
    public class EntityRepositoryTests
    {
        /// <summary>
        /// The mapper factory.
        /// </summary>
        private readonly IEntityMapper mapper = new AutoMapperEntityMapper(
            new Mapper(
                new MapperConfiguration(
                    expression =>
                    {
                        expression.AddProfile<SubSubEntityMappingProfile>();
                        expression.AddProfile<MultiReferenceEntityMappingProfile>();
                        expression.AddProfile<CreatedByMappingProfile>();
                        expression.AddProfile<ModifiedByMappingProfile>();
                        expression.AddProfile<SubEntityMappingProfile>();
                        expression.AddProfile<FakeComplexEntityMappingProfile>();
                        expression.AddProfile<FakeDependentEntityMappingProfile>();
                    })));

        /// <summary>
        /// The save test.
        /// </summary>
        [TestMethod]
        public void Save_NewFakeComplexEntity_MatchesExpected()
        {
            var subSubEntity = new SubSubEntity("SubSubUniqueName", 6445);
            var subEntity = new SubEntity("SubUniqueName", 234, subSubEntity, 67893);
            var createdBy = new CreatedBy("CreateUniqueName", 1122);
            var modifiedBy = new ModifiedBy("ModifiedBy", 454);
            var expected = new ComplexEntity("UniqueName", subEntity, FakeEnumeration.FirstFake, createdBy)
            {
                ModifiedBy = modifiedBy,
                ModifiedTime = DateTimeOffset.Now.AddHours(1)
            };

            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            repositoryProvider.Setup(provider => provider.FirstOrDefault(It.IsAny<EntitySelection<ComplexRaisedRow>>()))
                .Returns(default(ComplexRaisedRow));

            repositoryProvider.Setup(provider => provider.Insert(It.IsAny<ComplexRaisedRow>()))
                .Callback((ComplexRaisedRow row) => row.ComplexEntityId = 43)
                .Returns((ComplexRaisedRow row) => row);

            using (var provider = repositoryProvider.Object)
            {
                var target = new EntityRepository<ComplexEntity, ComplexRaisedRow>(provider, this.mapper);
                var actual = target.Save(expected);
                Assert.IsNotNull(actual.SubEntity);
                Assert.IsNotNull(actual.SubEntity.SubSubEntity);
                Assert.IsNotNull(actual.CreatedBy);
                Assert.IsNotNull(actual.ModifiedBy);
                Assert.AreEqual(6445, actual.SubSubEntity.FakeSubSubEntityId);
                Assert.AreSame(expected.SubEntity.SubSubEntity, actual.SubEntity.SubSubEntity);
                Assert.AreEqual(67893, actual.SubEntity.FakeSubEntityId);
                Assert.AreEqual(expected.SubEntity, actual.SubEntity);
                Assert.AreEqual(1122, actual.CreatedBy.FakeMultiReferenceEntityId);
                Assert.AreEqual(expected.CreatedBy, actual.CreatedBy);
                Assert.AreEqual(454, actual.ModifiedBy.FakeMultiReferenceEntityId);
                Assert.AreEqual(expected.ModifiedBy, actual.ModifiedBy);
                Assert.AreEqual(43, actual.ComplexEntityId);
                Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
            }
        }

        /// <summary>
        /// The save test.
        /// </summary>
        [TestMethod]
        public void Save_UpdatedFakeComplexEntity_MatchesExpected()
        {
            var subSubEntity = new SubSubEntity("SubSubUniqueName", 45)
            {
                Description = "OriginalSubSub"
            };
            var subEntity = new SubEntity("SubUniqueName", 234, subSubEntity, 16)
            {
                Description = "OriginalSub"
            };
            var createdBy = new CreatedBy("CreateUniqueName", 432)
            {
                Description = "OriginalCreatedBy"
            };
            var modifiedBy = new ModifiedBy("ModifiedBy", 433)
            {
                Description = "OriginalModifiedBy"
            };
            var creationTime = DateTimeOffset.Now.AddDays(-1);
            var baseline = new ComplexEntity("UniqueName", subEntity, FakeEnumeration.FirstFake, createdBy, creationTime, 22)
            {
                Description = "OriginalComplexEntity",
                ModifiedBy = modifiedBy,
                ModifiedTime = DateTimeOffset.Now.AddHours(1)
            };

            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            repositoryProvider.Setup(provider => provider.FirstOrDefault(It.IsAny<EntitySet<ComplexRaisedRow>>()))
                .Returns(this.mapper.Map<ComplexRaisedRow>(baseline));

            repositoryProvider.Setup(
                    provider => provider.Contains(It.Is<IEntitySet>(selection => (int?)selection.PropertyValues.FirstOrDefault() == 22)))
                .Returns(true);

            repositoryProvider.Setup(provider => provider.Update(It.IsAny<UpdateSet<ComplexRaisedRow>>())).Returns(1);

            using (var provider = repositoryProvider.Object)
            {
                var newModifiedBy = new ModifiedBy("ModifiedBy", 433)
                {
                    Description = "UpdatedModifiedBy"
                };

                var target = new EntityRepository<ComplexEntity, ComplexRaisedRow>(provider, this.mapper);
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
                Assert.AreEqual(22, actual.ComplexEntityId);
                Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
            }
        }

        /// <summary>
        /// The delete test.
        /// </summary>
        [TestMethod]
        public void Delete_ItemById_ReturnsSingleRowDeleted()
        {
            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);
            repositoryProvider.Setup(provider => provider.Delete(It.IsAny<IEntitySet>())).Returns(1);

            int actual;

            using (var provider = repositoryProvider.Object)
            {
                var repository = new EntityRepository<ComplexEntity, ComplexRaisedRow>(provider, this.mapper);
                actual = repository.Delete(14);
            }

            Assert.AreEqual(1, actual);
        }

        /// <summary>
        /// Tests the DeletedAsync method.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the result of the asynchronous operation.
        /// </returns>
        [TestMethod]
        public async Task DeleteAsync_ItemById_ReturnsSingleRowDeleted()
        {
            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);
            repositoryProvider.Setup(provider => provider.DeleteAsync(It.IsAny<IEntitySet>(), It.IsAny<CancellationToken>())).ReturnsAsync(1);

            int actual;

            using (var provider = repositoryProvider.Object)
            {
                var repository = new EntityRepository<ComplexEntity, ComplexRaisedRow>(provider, this.mapper);
                actual = await repository.DeleteAsync(14, CancellationToken.None);
            }

            Assert.AreEqual(1, actual);
        }

        /// <summary>
        /// The delete test.
        /// </summary>
        [TestMethod]
        public void DeleteSelection_ItemsBySelection_ReturnsMultipleRowsDeleted()
        {
            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);
            repositoryProvider.Setup(provider => provider.Delete(It.IsAny<IEntitySet>())).Returns(5);

            int actual;

            using (var provider = repositoryProvider.Object)
            {
                var repository = new EntityRepository<SubSubEntity, SubSubRow>(provider, this.mapper);
                actual = repository.DeleteSelection(new EntitySet<SubSubEntity>().Where(set => set.AreEqual(entity => entity.UniqueName, "bar")));
            }

            Assert.AreEqual(5, actual);
        }

        /// <summary>
        /// The delete test.
        /// </summary>
        [TestMethod]
        public void DeleteEntities_ItemsBySelection_ReturnsMultipleRowsDeleted()
        {
            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);
            repositoryProvider.Setup(provider => provider.Delete(It.IsAny<IEntitySet>())).Returns(5);

            int actual;

            using (var provider = repositoryProvider.Object)
            {
                var repository = new EntityRepository<SubSubEntity, SubSubRow>(provider, this.mapper);
                actual = repository.DeleteEntities(set => set.Where(filterSet => filterSet.AreEqual(entity => entity.UniqueName, "bar")));
            }

            Assert.AreEqual(5, actual);
        }

        /// <summary>
        /// Tests the DeleteEntitiesAsync method.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the result of the asynchronous operation.
        /// </returns>
        [TestMethod]
        public async Task DeleteEntitiesAsync_ItemsBySelection_ReturnsMultipleRowsDeleted()
        {
            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);
            repositoryProvider.Setup(provider => provider.DeleteAsync(It.IsAny<IEntitySet>(), It.IsAny<CancellationToken>())).ReturnsAsync(5);

            int actual;

            using (var provider = repositoryProvider.Object)
            {
                var repository = new EntityRepository<SubSubEntity, SubSubRow>(provider, this.mapper);
                actual = await repository.DeleteEntitiesAsync(
                    set => set.Where(filterSet => filterSet.AreEqual(entity => entity.UniqueName, "bar")),
                    CancellationToken.None);
            }

            Assert.AreEqual(5, actual);
        }

        /// <summary>
        /// Tests the update method.
        /// </summary>
        [TestMethod]
        public void Update_UpdateModelSet_MappedToUpdateEntitySet()
        {
            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);
            repositoryProvider.Setup(provider => provider.Update(It.IsAny<UpdateSet<SubSubRow>>())).Returns(1);

            int actual;

            using (var provider = repositoryProvider.Object)
            {
                var repository = new EntityRepository<SubSubEntity, SubSubRow>(provider, this.mapper);
                actual = repository.Update(
                    new UpdateSet<SubSubEntity>().Set(entity => entity.UniqueName, "newName")
                        .Where(filterSet => filterSet.AreEqual(entity => entity.UniqueName, "bar")));
            }

            Assert.AreEqual(1, actual);
        }

        /// <summary>
        /// A test for the UpdateAsync method.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the result of the asynchronous operation.
        /// </returns>
        [TestMethod]
        public async Task UpdateAsync_UpdateModelSet_MappedToUpdateEntitySet()
        {
            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);
            repositoryProvider.Setup(provider => provider.UpdateAsync(It.IsAny<UpdateSet<SubSubRow>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            int actual;

            await using (var provider = repositoryProvider.Object)
            {
                var repository = new EntityRepository<SubSubEntity, SubSubRow>(provider, this.mapper);
                actual = await repository.UpdateAsync(
                                 new UpdateSet<SubSubEntity>().Set(entity => entity.UniqueName, "newName")
                                     .Where(filterSet => filterSet.AreEqual(entity => entity.UniqueName, "bar")),
                                 CancellationToken.None)
                             .ConfigureAwait(false);
            }

            Assert.AreEqual(1, actual);
        }

        /// <summary>
        /// Tests the UpdateSingle method.
        /// </summary>
        [TestMethod]
        public void UpdateSingle_ModelInput_MappedToEntity()
        {
            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);
            repositoryProvider.Setup(provider => provider.Update(It.IsAny<UpdateSet<SubSubRow>>())).Verifiable();

            using (var provider = repositoryProvider.Object)
            {
                var repository = new EntityRepository<SubSubEntity, SubSubRow>(provider, this.mapper);
                var subSubEntity = new SubSubEntity("myUniqueName", 34)
                {
                    Description = "my description"
                };

                repository.UpdateSingle(34, subSubEntity, entity => entity.Description);
            }

            repositoryProvider.Verify();
        }

        /// <summary>
        /// Tests the UpdateSingleAsync method.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the result of the asynchronous operation.
        /// </returns>
        [TestMethod]
        public async Task UpdateSingleAsync_ModelInput_MappedToEntity()
        {
            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);
            repositoryProvider.Setup(provider => provider.UpdateAsync(It.IsAny<UpdateSet<SubSubRow>>(), It.IsAny<CancellationToken>())).Verifiable();

            await using (var provider = repositoryProvider.Object)
            {
                var repository = new EntityRepository<SubSubEntity, SubSubRow>(provider, this.mapper);
                var subSubEntity = new SubSubEntity("myUniqueName", 34)
                {
                    Description = "my description"
                };

                await repository.UpdateSingleAsync(34, subSubEntity, CancellationToken.None, entity => entity.Description).ConfigureAwait(false);
            }

            repositoryProvider.Verify();
        }

        /// <summary>
        /// Tests the UpdateSingle method.
        /// </summary>
        [TestMethod]
        public void UpdateSingle_AnonymousTypeInput_MappedToEntity()
        {
            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);
            repositoryProvider.Setup(provider => provider.Update(It.IsAny<UpdateSet<SubRow>>())).Verifiable();

            using (var provider = repositoryProvider.Object)
            {
                var repository = new EntityRepository<SubEntity, SubRow>(provider, this.mapper);

                repository.UpdateSingle(
                    4444,
                    new
                    {
                        UniqueOtherId = (short)65,
                        Description = "new desc"
                    },
                    entity => entity.UniqueOtherId,
                    entity => entity.Description);
            }

            repositoryProvider.Verify();
        }

        /// <summary>
        /// Tests the UpdateSingleAsync method.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the result of the asynchronous operation.
        /// </returns>
        [TestMethod]
        public async Task UpdateSingleAsync_AnonymousTypeInput_MappedToEntity()
        {
            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);
            repositoryProvider.Setup(provider => provider.UpdateAsync(It.IsAny<UpdateSet<SubRow>>(), It.IsAny<CancellationToken>())).Verifiable();

            await using (var provider = repositoryProvider.Object)
            {
                var repository = new EntityRepository<SubEntity, SubRow>(provider, this.mapper);

                await repository.UpdateSingleAsync(
                    4444,
                    new
                    {
                        UniqueOtherId = (short)65,
                        Description = "new desc"
                    },
                    CancellationToken.None,
                    entity => entity.UniqueOtherId,
                    entity => entity.Description).ConfigureAwait(false);
            }

            repositoryProvider.Verify();
        }
    }
}