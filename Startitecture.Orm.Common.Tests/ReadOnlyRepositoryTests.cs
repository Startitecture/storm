// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadOnlyRepositoryTests.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Defines the ReadOnlyRepositoryTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable StringLiteralTypo
namespace Startitecture.Orm.Common.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Startitecture.Core;
    using Startitecture.Orm.AutoMapper;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Schema;
    using Startitecture.Orm.Testing.Entities;
    using Startitecture.Orm.Testing.Model;

    /// <summary>
    /// The read only repository tests.
    /// </summary>
    [TestClass]
    public class ReadOnlyRepositoryTests
    {
        /// <summary>
        /// The entity mapper.
        /// </summary>
        private readonly IEntityMapper entityMapper = CreateEntityMapper();

        /// <summary>
        /// The contains test.
        /// </summary>
        [TestMethod]
        public void Contains_ItemExample_ReturnsTrue()
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

            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            repositoryProvider.Setup(provider => provider.Contains(It.IsAny<ItemSelection<ComplexRaisedRow>>()))
                .Returns(true);

            using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<ComplexEntity, ComplexRaisedRow>(provider, this.entityMapper);
                var actual = target.Contains(existing);
                Assert.IsTrue(actual);
            }
        }

        /// <summary>
        /// The contains test.
        /// </summary>
        [TestMethod]
        public void Contains_ItemKey_ReturnsTrue()
        {
            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            repositoryProvider.Setup(provider => provider.Contains(It.IsAny<ItemSelection<ComplexRaisedRow>>()))
                .Returns(true);

            using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<ComplexEntity, ComplexRaisedRow>(provider, this.entityMapper);
                var actual = target.Contains(22);
                Assert.IsTrue(actual);
            }
        }

        /// <summary>
        /// The save test.
        /// </summary>
        [TestMethod]
        public void FirstOrDefault_ByKey_MatchesExpected()
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

            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            repositoryProvider.Setup(provider => provider.GetFirstOrDefault(It.IsAny<ItemSelection<ComplexRaisedRow>>()))
                .Returns(this.entityMapper.Map<ComplexRaisedRow>(existing));

            using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<ComplexEntity, ComplexRaisedRow>(provider, this.entityMapper);
                var actual = target.FirstOrDefault(22);
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

        /// <summary>
        /// The save test.
        /// </summary>
        [TestMethod]
        public void FirstOrDefault_FakeComplexEntityExample_MatchesExpected()
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

            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            repositoryProvider.Setup(provider => provider.GetFirstOrDefault(It.IsAny<ItemSelection<ComplexRaisedRow>>()))
                .Returns(this.entityMapper.Map<ComplexRaisedRow>(existing));

            using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<ComplexEntity, ComplexRaisedRow>(provider, this.entityMapper);
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

        /// <summary>
        /// The save test.
        /// </summary>
        [TestMethod]
        public void FirstOrDefault_FakeComplexEntityWithDependentEntityExample_MatchesExpected()
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
            var expected = new ComplexEntity(
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

            expected.SetDependentEntity(994);

            var existing = this.entityMapper.Map<ComplexRaisedRow>(expected);

            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            repositoryProvider.Setup(provider => provider.GetFirstOrDefault(It.IsAny<ItemSelection<ComplexRaisedRow>>()))
                .Returns(this.entityMapper.Map<ComplexRaisedRow>(existing));

            using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<ComplexEntity, ComplexRaisedRow>(provider, this.entityMapper);
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

                Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
            }
        }

        /// <summary>
        /// Tests that the first or default of the repository matches the expected results.
        /// TODO: Ensure that SubSubEntity is the same reference
        /// </summary>
        [TestMethod]
        public void SelectAll_FakeRaisedSubEntity_MatchesExpected()
        {
            var fakeSubSubEntity = new SubSubEntity("jasdyri", 5848);
            var expected = new List<SubEntity>
                               {
                                   new SubEntity("asidf", 58, fakeSubSubEntity, 87543),
                                   new SubEntity("safd", 59, fakeSubSubEntity, 546),
                                   new SubEntity("gjkdf", 52, fakeSubSubEntity, 3465)
                               };

            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            repositoryProvider.Setup(provider => provider.GetSelection(It.IsAny<ItemSelection<SubRow>>()))
                .Returns(this.entityMapper.Map<List<SubRow>>(expected));

            using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<SubEntity, SubRow>(provider, this.entityMapper);
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
        /// The select test.
        /// TODO: Ensure that SubSubEntity is the same reference
        /// </summary>
        [TestMethod]
        public void Select_ReadOnlyRepositoryWithItemSelection_MatchesExpected()
        {
            var fakeSubSubEntity = new SubSubEntity("jasdyri", 5848);
            var expected = new List<SubEntity>
                               {
                                   new SubEntity("asidf", 58, fakeSubSubEntity, 87543),
                                   new SubEntity("safd", 59, fakeSubSubEntity, 546),
                                   new SubEntity("gjkdf", 52, fakeSubSubEntity, 3465)
                               };

            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            repositoryProvider.Setup(provider => provider.GetSelection(It.IsAny<ItemSelection<SubRow>>()))
                .Returns(this.entityMapper.Map<List<SubRow>>(expected));

            using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<SubEntity, SubRow>(provider, this.entityMapper);
                var actual = target.Select(Select.From<SubRow>().WhereEqual(entity => entity.SubSubEntity.FakeSubSubEntityId, 5848)).ToList();

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