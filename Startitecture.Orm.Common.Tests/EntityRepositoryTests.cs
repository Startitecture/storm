// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityRepositoryTests.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common.Tests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Startitecture.Core;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Repository;
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
        /// The entity mapper.
        /// </summary>
        private readonly IEntityMapper entityMapper = CreateEntityMapper();

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

            repositoryProvider.Setup(provider => provider.GetFirstOrDefault(It.IsAny<ItemSelection<ComplexRaisedRow>>()))
                .Returns(default(ComplexRaisedRow));

            repositoryProvider.Setup(provider => provider.Save(It.IsAny<ComplexRaisedRow>()))
                .Callback((ComplexRaisedRow row) => row.ComplexEntityId = 43)
                .Returns((ComplexRaisedRow row) => row);

            using (var provider = repositoryProvider.Object)
            {
                var target = new EntityRepository<ComplexEntity, ComplexRaisedRow>(provider, this.entityMapper);
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

            repositoryProvider.Setup(provider => provider.GetFirstOrDefault(It.IsAny<ItemSelection<ComplexRaisedRow>>()))
                .Returns(this.entityMapper.Map<ComplexRaisedRow>(baseline));

            repositoryProvider.Setup(provider => provider.Save(It.IsAny<ComplexRaisedRow>())).Returns((ComplexRaisedRow row) => row);

            using (var provider = repositoryProvider.Object)
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
            repositoryProvider.Setup(provider => provider.DeleteItems(It.IsAny<ItemSelection<ComplexRaisedRow>>())).Returns(1);

            int actual;

            using (var provider = repositoryProvider.Object)
            {
                var repository = new EntityRepository<ComplexEntity, ComplexRaisedRow>(provider, this.entityMapper);
                actual = repository.Delete(14);
            }

            Assert.AreEqual(1, actual);
        }

        /// <summary>
        /// The delete test.
        /// </summary>
        [TestMethod]
        public void Delete_ItemsBySelection_ReturnsMultipleRowsDeleted()
        {
            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);
            repositoryProvider.Setup(provider => provider.DeleteItems(It.IsAny<ItemSelection<SubSubRow>>())).Returns(5);

            int actual;

            using (var provider = repositoryProvider.Object)
            {
                var repository = new EntityRepository<SubSubEntity, SubSubRow>(provider, this.entityMapper);
                actual = repository.Delete(new ItemSelection<SubSubEntity>().WhereEqual(entity => entity.UniqueName, "bar"));
            }

            Assert.AreEqual(5, actual);
        }

        /// <summary>
        /// The create entity mapper.
        /// </summary>
        /// <returns>
        /// The <see cref="IEntityMapper" />.
        /// </returns>
        private static AutoMapperEntityMapper CreateEntityMapper()
        {
            var autoMapperEntityMapper = new AutoMapperEntityMapper();
            autoMapperEntityMapper.Initialize(
                configuration =>
                    {
                        configuration.AddProfile<SubSubEntityMappingProfile>();
                        configuration.AddProfile<MultiReferenceEntityMappingProfile>();
                        configuration.AddProfile<CreatedByMappingProfile>();
                        configuration.AddProfile<ModifiedByMappingProfile>();
                        configuration.AddProfile<SubEntityMappingProfile>();
                        configuration.AddProfile<FakeChildEntityMappingProfile>();
                        configuration.AddProfile<FakeComplexEntityMappingProfile>();
                        configuration.AddProfile<FakeDependentEntityMappingProfile>();
                    });

            return autoMapperEntityMapper;
        }
    }
}