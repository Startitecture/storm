﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadOnlyRepositoryTests.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Defines the ReadOnlyRepositoryTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using global::AutoMapper;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Startitecture.Core;
    using Startitecture.Orm.AutoMapper;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Schema;
    using Startitecture.Orm.Testing.Entities;
    using Startitecture.Orm.Testing.Model;
    using Startitecture.Orm.Testing.Model.Contracts;
    using Startitecture.Orm.Testing.Model.Entities;

    /// <summary>
    /// The read only repository tests.
    /// </summary>
    [TestClass]
    public class ReadOnlyRepositoryTests
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
                            expression.AddProfile<DocumentMappingProfile>();
                        })));

        /// <summary>
        /// Tests the Contains method.
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

            var existing = this.mapper.Map<ComplexRaisedRow>(expected);

            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            repositoryProvider.Setup(provider => provider.Contains(It.IsAny<EntitySet<ComplexRaisedRow>>()))
                .Returns(true);

            using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<ComplexEntity, ComplexRaisedRow>(provider, this.mapper);
                var actual = target.Contains(existing);
                Assert.IsTrue(actual);
            }
        }

        /// <summary>
        /// Tests the Contains method.
        /// </summary>
        [TestMethod]
        public void Contains_ItemKey_ReturnsTrue()
        {
            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            repositoryProvider.Setup(provider => provider.Contains(It.IsAny<EntitySet<ComplexRaisedRow>>()))
                .Returns(true);

            using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<ComplexEntity, ComplexRaisedRow>(provider, this.mapper);
                var actual = target.Contains(22);
                Assert.IsTrue(actual);
            }
        }

        /// <summary>
        /// Tests the Contains method.
        /// </summary>
        [TestMethod]
        public void Contains_ItemSelectionForExistingEntity_ReturnsTrue()
        {
            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            repositoryProvider.Setup(provider => provider.Contains(It.IsAny<EntitySet<ComplexRaisedRow>>()))
                .Returns(true);

            using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<ComplexEntity, ComplexRaisedRow>(provider, this.mapper);
                var actual = target.Contains(Query.From<ComplexRaisedRow>().Where(set => set.AreEqual(row => row.FakeSubEntityId, 22)));
                Assert.IsTrue(actual);
            }
        }

        /// <summary>
        /// Tests the Contains method.
        /// </summary>
        [TestMethod]
        public void Contains_ItemSelectionForNonExistingEntity_ReturnsTrue()
        {
            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            repositoryProvider.Setup(provider => provider.Contains(It.IsAny<EntitySet<ComplexRaisedRow>>())).Returns(false);

            using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<ComplexEntity, ComplexRaisedRow>(provider, this.mapper);
                var actual = target.Contains(Query.From<ComplexRaisedRow>().Where(set => set.AreEqual(row => row.FakeSubEntityId, 22)));
                Assert.IsFalse(actual);
            }
        }

        /// <summary>
        /// Tests the Contains method.
        /// </summary>
        [TestMethod]
        public void Contains_EntityWithDynamicUniqueKeySpecified_ReturnsTrue()
        {
            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            repositoryProvider.Setup(
                    provider => provider.Contains(
                        It.Is<EntitySet<ComplexRaisedRow>>(
                            selection => selection.PropertyValues.Count() == 1 && selection.PropertyValues.First() as string == "UniqueName")))
                .Returns(true);

            using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<ComplexEntity, ComplexRaisedRow>(provider, this.mapper);
                dynamic key = new ExpandoObject();
                key.UniqueName = "UniqueName";

                var actual = target.Contains(key);

                Assert.IsTrue(actual);
            }
        }

        /// <summary>
        /// Tests the Contains method.
        /// </summary>
        [TestMethod]
        public void Contains_EntityWithAnonymousUniqueKeySpecified_ReturnsTrue()
        {
            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            repositoryProvider.Setup(
                    provider => provider.Contains(
                        It.Is<IEntitySet>(
                            selection => selection.PropertyValues.Count() == 1 && selection.PropertyValues.First() as string == "UniqueName")))
                .Returns(true);

            using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<ComplexEntity, ComplexRaisedRow>(provider, this.mapper);

                var actual = target.Contains(
                    new
                    {
                        UniqueName = "UniqueName",
                    });

                Assert.IsTrue(actual);
            }
        }

        /// <summary>
        /// Tests the ContainsAsync method.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the result of the asynchronous operation.
        /// </returns>
        [TestMethod]
        public async Task ContainsAsync_ItemExample_ReturnsTrue()
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

            var existing = this.mapper.Map<ComplexRaisedRow>(expected);

            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            repositoryProvider.Setup(provider => provider.ContainsAsync(It.IsAny<EntitySet<ComplexRaisedRow>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            await using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<ComplexEntity, ComplexRaisedRow>(provider, this.mapper);
                var actual = await target.ContainsAsync(existing, CancellationToken.None);
                Assert.IsTrue(actual);
            }
        }

        /// <summary>
        /// Tests the ContainsAsync method.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the result of the asynchronous operation.
        /// </returns>
        [TestMethod]
        public async Task ContainsAsync_ItemKey_ReturnsTrue()
        {
            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            repositoryProvider.Setup(provider => provider.ContainsAsync(It.IsAny<EntitySet<ComplexRaisedRow>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            await using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<ComplexEntity, ComplexRaisedRow>(provider, this.mapper);
                var actual = await target.ContainsAsync(22, CancellationToken.None);
                Assert.IsTrue(actual);
            }
        }

        /// <summary>
        /// Tests the ContainsAsync method.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the result of the asynchronous operation.
        /// </returns>
        [TestMethod]
        public async Task ContainsAsync_ItemSelectionForExistingEntity_ReturnsTrue()
        {
            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            repositoryProvider.Setup(provider => provider.ContainsAsync(It.IsAny<EntitySet<ComplexRaisedRow>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            await using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<ComplexEntity, ComplexRaisedRow>(provider, this.mapper);
                var actual = await target.ContainsAsync(
                    Query.From<ComplexRaisedRow>().Where(set => set.AreEqual(row => row.FakeSubEntityId, 22)),
                    CancellationToken.None);

                Assert.IsTrue(actual);
            }
        }

        /// <summary>
        /// Tests the ContainsAsync method.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the result of the asynchronous operation.
        /// </returns>
        [TestMethod]
        public async Task ContainsAsync_ItemSelectionForNonExistingEntity_ReturnsTrue()
        {
            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            repositoryProvider.Setup(provider => provider.ContainsAsync(It.IsAny<EntitySet<ComplexRaisedRow>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            await using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<ComplexEntity, ComplexRaisedRow>(provider, this.mapper);
                var actual = await target.ContainsAsync(
                    Query.From<ComplexRaisedRow>().Where(set => set.AreEqual(row => row.FakeSubEntityId, 22)),
                    CancellationToken.None);

                Assert.IsFalse(actual);
            }
        }

        /// <summary>
        /// Tests the ContainsAsync method.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the result of the asynchronous operation.
        /// </returns>
        [TestMethod]
        public async Task ContainsAsync_EntityWithDynamicUniqueKeySpecified_ReturnsTrue()
        {
            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            repositoryProvider.Setup(
                    provider => provider.ContainsAsync(
                        It.Is<EntitySet<ComplexRaisedRow>>(
                            selection => selection.PropertyValues.Count() == 1 && selection.PropertyValues.First() as string == "UniqueName"),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            await using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<ComplexEntity, ComplexRaisedRow>(provider, this.mapper);
                dynamic key = new ExpandoObject();
                key.UniqueName = "UniqueName";

                var actual = await target.ContainsAsync(key, CancellationToken.None);

                Assert.IsTrue(actual);
            }
        }

        /// <summary>
        /// Tests the ContainsAsync method.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the result of the asynchronous operation.
        /// </returns>
        [TestMethod]
        public async Task ContainsAsync_EntityWithAnonymousUniqueKeySpecified_ReturnsTrue()
        {
            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            repositoryProvider.Setup(
                    provider => provider.ContainsAsync(
                        It.Is<IEntitySet>(
                            selection => selection.PropertyValues.Count() == 1 && selection.PropertyValues.First() as string == "UniqueName"),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            await using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<ComplexEntity, ComplexRaisedRow>(provider, this.mapper);

                var actual = await target.ContainsAsync(
                                 new
                                 {
                                     UniqueName = "UniqueName",
                                 },
                                 CancellationToken.None);

                Assert.IsTrue(actual);
            }
        }

        /// <summary>
        /// Tests the FirstOrDefault method.
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

            var existing = this.mapper.Map<ComplexRaisedRow>(expected);

            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            repositoryProvider.Setup(provider => provider.FirstOrDefault(It.IsAny<EntitySet<ComplexRaisedRow>>()))
                .Returns(this.mapper.Map<ComplexRaisedRow>(existing));

            using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<ComplexEntity, ComplexRaisedRow>(provider, this.mapper);
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
        /// Tests the FirstOrDefault method.
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

            var existing = this.mapper.Map<ComplexRaisedRow>(expected);

            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            repositoryProvider.Setup(provider => provider.FirstOrDefault(It.IsAny<EntitySet<ComplexRaisedRow>>()))
                .Returns(this.mapper.Map<ComplexRaisedRow>(existing));

            using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<ComplexEntity, ComplexRaisedRow>(provider, this.mapper);
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
        /// Tests the FirstOrDefault method.
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

            var existing = this.mapper.Map<ComplexRaisedRow>(expected);

            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            repositoryProvider.Setup(provider => provider.FirstOrDefault(It.IsAny<EntitySet<ComplexRaisedRow>>()))
                .Returns(this.mapper.Map<ComplexRaisedRow>(existing));

            using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<ComplexEntity, ComplexRaisedRow>(provider, this.mapper);
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
        /// Tests the FirstOrDefault method.
        /// </summary>
        [TestMethod]
        public void FirstOrDefault_EntityWithDynamicUniqueKeySpecified_ReturnsEntity()
        {
            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            var expected = new ComplexRaisedRow
            {
                ComplexEntityId = 34,
                CreatedByFakeMultiReferenceEntityId = 3335,
                CreationTime = DateTimeOffset.Now,
                Description = "Foo",
                FakeEnumerationId = 3,
                FakeOtherEnumerationId = 6,
                ModifiedByFakeMultiReferenceEntityId = 998,
                UniqueName = "UniqueName"
            };

            repositoryProvider.Setup(
                    provider => provider.FirstOrDefault(
                        It.Is<EntitySet<ComplexRaisedRow>>(
                            selection => selection.PropertyValues.Count() == 1 && selection.PropertyValues.First() as string == "UniqueName")))
                .Returns(expected);

            var entityMapper = this.mapper;

            using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<ComplexEntity, ComplexRaisedRow>(provider, entityMapper);
                dynamic key = new ExpandoObject();
                key.UniqueName = "UniqueName";

                var actual = target.FirstOrDefault(key);

                Assert.AreEqual(entityMapper.Map<ComplexEntity>(expected), actual);
            }
        }

        /// <summary>
        /// Tests the FirstOrDefault method.
        /// </summary>
        [TestMethod]
        public void FirstOrDefault_EntityWithAnonymousUniqueKeySpecified_ReturnsEntity()
        {
            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            var expected = new ComplexRaisedRow
            {
                ComplexEntityId = 34,
                CreatedByFakeMultiReferenceEntityId = 3335,
                CreationTime = DateTimeOffset.Now,
                Description = "Foo",
                FakeEnumerationId = 3,
                FakeOtherEnumerationId = 6,
                ModifiedByFakeMultiReferenceEntityId = 998,
                UniqueName = "UniqueName"
            };

            repositoryProvider.Setup(
                    provider => provider.FirstOrDefault(
                        It.Is<EntitySet<ComplexRaisedRow>>(
                            selection => selection.PropertyValues.Count() == 1 && selection.PropertyValues.First() as string == "UniqueName")))
                .Returns(expected);

            var entityMapper = this.mapper;

            using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<ComplexEntity, ComplexRaisedRow>(provider, entityMapper);

                var actual = target.FirstOrDefault(
                    new
                    {
                        UniqueName = "UniqueName",
                    });

                Assert.AreEqual(entityMapper.Map<ComplexEntity>(expected), actual);
            }
        }

        /// <summary>
        /// Tests the FirstOrDefault method.
        /// </summary>
        [TestMethod]
        public void FirstOrDefault_EntitySet_ReturnsEntity()
        {
            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            var expected = new DocumentRow
            {
                DocumentId = 423543,
                DocumentVersionId = 1,
                DocumentVersion = new DocumentVersionRow
                {
                    DocumentVersionId = 1,
                    Name = "foo1234",
                    VersionNumber = 1
                },
                Identifier = "1234-1"
            };

            repositoryProvider.Setup(
                    provider => provider.FirstOrDefault(
                        It.Is<EntitySet<DocumentRow>>(
                            selection => selection.PropertyValues.Count() == 1 && selection.PropertyValues.First() as string == "1234-1")))
                .Returns(expected);

            var entityMapper = this.mapper;

            using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<Document, DocumentRow>(provider, entityMapper);
                var actual = target.FirstOrDefault(Query.From<DocumentDto>().Where(set => set.AreEqual(dto => dto.Identifier, "1234-1")));
                Assert.AreEqual(entityMapper.Map<Document>(expected), actual);
            }
        }

        /// <summary>
        /// Tests the FirstOrDefault method.
        /// </summary>
        [TestMethod]
        public void FirstOrDefault_EntitySetAction_ReturnsEntity()
        {
            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            var expected = new DocumentRow
            {
                DocumentId = 423543,
                DocumentVersionId = 1,
                DocumentVersion = new DocumentVersionRow
                {
                    DocumentVersionId = 1,
                    Name = "foo1234",
                    VersionNumber = 1
                },
                Identifier = "1234-1"
            };

            repositoryProvider.Setup(
                    provider => provider.FirstOrDefault(
                        It.Is<EntitySet<DocumentRow>>(
                            selection => selection.PropertyValues.Count() == 1 && selection.PropertyValues.First() as string == "1234-1")))
                .Returns(expected);

            var entityMapper = this.mapper;

            using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<Document, DocumentRow>(provider, entityMapper);
                var actual = target.FirstOrDefault(select => select.Where(set => set.AreEqual(dto => dto.Identifier, "1234-1")));
                Assert.AreEqual(entityMapper.Map<Document>(expected), actual);
            }
        }

        /// <summary>
        /// Tests the FirstOrDefaultAsync method.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the result of the asynchronous operation.
        /// </returns>
        [TestMethod]
        public async Task FirstOrDefaultAsync_ByKey_MatchesExpected()
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

            var existing = this.mapper.Map<ComplexRaisedRow>(expected);

            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            repositoryProvider.Setup(provider => provider.FirstOrDefaultAsync(It.IsAny<EntitySet<ComplexRaisedRow>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(this.mapper.Map<ComplexRaisedRow>(existing));

            await using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<ComplexEntity, ComplexRaisedRow>(provider, this.mapper);
                var actual = await target.FirstOrDefaultAsync(22, CancellationToken.None);
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
        /// Tests the FirstOrDefaultAsync method.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the result of the asynchronous operation.
        /// </returns>
        [TestMethod]
        public async Task FirstOrDefaulAsynct_FakeComplexEntityExample_MatchesExpected()
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

            var existing = this.mapper.Map<ComplexRaisedRow>(expected);

            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            repositoryProvider.Setup(provider => provider.FirstOrDefaultAsync(It.IsAny<EntitySet<ComplexRaisedRow>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(this.mapper.Map<ComplexRaisedRow>(existing));

            await using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<ComplexEntity, ComplexRaisedRow>(provider, this.mapper);
                var actual = await target.FirstOrDefaultAsync(existing, CancellationToken.None);
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
        /// Tests the FirstOrDefaultAsync method.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the result of the asynchronous operation.
        /// </returns>
        [TestMethod]
        public async Task FirstOrDefaultAsync_FakeComplexEntityWithDependentEntityExample_MatchesExpected()
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

            var existing = this.mapper.Map<ComplexRaisedRow>(expected);

            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            repositoryProvider.Setup(provider => provider.FirstOrDefaultAsync(It.IsAny<EntitySet<ComplexRaisedRow>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(this.mapper.Map<ComplexRaisedRow>(existing));

            await using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<ComplexEntity, ComplexRaisedRow>(provider, this.mapper);
                var actual = await target.FirstOrDefaultAsync(existing, CancellationToken.None);
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
        /// Tests the FirstOrDefaultAsync method.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the result of the asynchronous operation.
        /// </returns>
        [TestMethod]
        public async Task FirstOrDefaultAsync_EntityWithDynamicUniqueKeySpecified_ReturnsEntity()
        {
            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            var expected = new ComplexRaisedRow
            {
                ComplexEntityId = 34,
                CreatedByFakeMultiReferenceEntityId = 3335,
                CreationTime = DateTimeOffset.Now,
                Description = "Foo",
                FakeEnumerationId = 3,
                FakeOtherEnumerationId = 6,
                ModifiedByFakeMultiReferenceEntityId = 998,
                UniqueName = "UniqueName"
            };

            repositoryProvider.Setup(
                    provider => provider.FirstOrDefaultAsync(
                        It.Is<EntitySet<ComplexRaisedRow>>(
                            selection => selection.PropertyValues.Count() == 1 && selection.PropertyValues.First() as string == "UniqueName"),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var entityMapper = this.mapper;

            await using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<ComplexEntity, ComplexRaisedRow>(provider, entityMapper);
                dynamic key = new ExpandoObject();
                key.UniqueName = "UniqueName";

                var actual = await target.FirstOrDefaultAsync(key, CancellationToken.None);

                Assert.AreEqual(entityMapper.Map<ComplexEntity>(expected), actual);
            }
        }

        /// <summary>
        /// Tests the FirstOrDefaultAsync method.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the result of the asynchronous operation.
        /// </returns>
        [TestMethod]
        public async Task FirstOrDefaultAsync_EntityWithAnonymousUniqueKeySpecified_ReturnsEntity()
        {
            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            var expected = new ComplexRaisedRow
            {
                ComplexEntityId = 34,
                CreatedByFakeMultiReferenceEntityId = 3335,
                CreationTime = DateTimeOffset.Now,
                Description = "Foo",
                FakeEnumerationId = 3,
                FakeOtherEnumerationId = 6,
                ModifiedByFakeMultiReferenceEntityId = 998,
                UniqueName = "UniqueName"
            };

            repositoryProvider.Setup(
                    provider => provider.FirstOrDefaultAsync(
                        It.Is<EntitySet<ComplexRaisedRow>>(
                            selection => selection.PropertyValues.Count() == 1 && selection.PropertyValues.First() as string == "UniqueName"),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var entityMapper = this.mapper;

            await using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<ComplexEntity, ComplexRaisedRow>(provider, entityMapper);

                var actual = await target.FirstOrDefaultAsync(
                                 new
                                 {
                                     UniqueName = "UniqueName",
                                 },
                                 CancellationToken.None);

                Assert.AreEqual(entityMapper.Map<ComplexEntity>(expected), actual);
            }
        }

        /// <summary>
        /// Tests the FirstOrDefaultAsync method.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the result of the asynchronous operation.
        /// </returns>
        [TestMethod]
        public async Task FirstOrDefaultAsync_EntitySet_ReturnsEntity()
        {
            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            var expected = new DocumentRow
            {
                DocumentId = 423543,
                DocumentVersionId = 1,
                DocumentVersion = new DocumentVersionRow
                {
                    DocumentVersionId = 1,
                    Name = "foo1234",
                    VersionNumber = 1
                },
                Identifier = "1234-1"
            };

            repositoryProvider.Setup(
                    provider => provider.FirstOrDefaultAsync(
                        It.Is<EntitySet<DocumentRow>>(
                            selection => selection.PropertyValues.Count() == 1 && selection.PropertyValues.First() as string == "1234-1"),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var entityMapper = this.mapper;

            await using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<Document, DocumentRow>(provider, entityMapper);
                var actual = await target.FirstOrDefaultAsync(
                                 Query.From<DocumentDto>().Where(set => set.AreEqual(dto => dto.Identifier, "1234-1")),
                                 CancellationToken.None);

                Assert.AreEqual(entityMapper.Map<Document>(expected), actual);
            }
        }

        /// <summary>
        /// Tests the FirstOrDefaultAsync method.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the result of the asynchronous operation.
        /// </returns>
        [TestMethod]
        public async Task FirstOrDefaultAsync_EntitySetAction_ReturnsEntity()
        {
            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            var expected = new DocumentRow
            {
                DocumentId = 423543,
                DocumentVersionId = 1,
                DocumentVersion = new DocumentVersionRow
                {
                    DocumentVersionId = 1,
                    Name = "foo1234",
                    VersionNumber = 1
                },
                Identifier = "1234-1"
            };

            repositoryProvider.Setup(
                    provider => provider.FirstOrDefaultAsync(
                        It.Is<EntitySet<DocumentRow>>(
                            selection => selection.PropertyValues.Count() == 1 && selection.PropertyValues.First() as string == "1234-1"),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var entityMapper = this.mapper;

            await using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<Document, DocumentRow>(provider, entityMapper);
                var actual = await target.FirstOrDefaultAsync(
                                 select => select.Where(set => set.AreEqual(dto => dto.Identifier, "1234-1")),
                                 CancellationToken.None);

                Assert.AreEqual(entityMapper.Map<Document>(expected), actual);
            }
        }

        /// <summary>
        /// Tests the DynamicFirstOrDefault method.
        /// </summary>
        [TestMethod]
        public void DynamicFirstOrDefault_PartialSelect_MatchesExpected()
        {
            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            dynamic result = new ExpandoObject();
            result.DocumentId = 43;
            result.Identifier = "client.org.59432-002.pdf";
            result.DocumentVersionName = "59432-002.pdf";
            result.DocumentVersionVersionNumber = 2;

            repositoryProvider.Setup(
                    provider => provider.DynamicFirstOrDefault(
                        It.Is<ISelection>(
                            selection => selection.PropertyValues.Count() == 1 && selection.PropertyValues.First() as int? == 43)))
                .Returns(result);

            var entityMapper = this.mapper;

            using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<Document, DocumentRow>(provider, entityMapper);
                var actual = target.DynamicFirstOrDefault(
                    Query.SelectEntities<DocumentRow>(
                        select => select.Select(
                                row => row.DocumentId,
                                row => row.Identifier,
                                row => row.DocumentVersion.Name,
                                row => row.DocumentVersion.VersionNumber)
                            .Where(set => set.AreEqual(entity => entity.DocumentId, 43))));

                Assert.AreEqual(43, actual.DocumentId);
                Assert.AreEqual("client.org.59432-002.pdf", actual.Identifier);
                Assert.AreEqual("59432-002.pdf", result.DocumentVersionName);
                Assert.AreEqual(2, result.DocumentVersionVersionNumber);
            }
        }

        /// <summary>
        /// Tests the DynamicFirstOrDefaultAsync method.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [TestMethod]
        public async Task DynamicFirstOrDefaultAsync_PartialSelect_MatchesExpected()
        {
            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            dynamic result = new ExpandoObject();
            result.DocumentId = 43;
            result.Identifier = "client.org.59432-002.pdf";
            result.DocumentVersionName = "59432-002.pdf";
            result.DocumentVersionVersionNumber = 2;

            repositoryProvider.Setup(
                    provider => provider.DynamicFirstOrDefaultAsync(
                        It.Is<ISelection>(
                            selection => selection.PropertyValues.Count() == 1 && selection.PropertyValues.First() as int? == 43),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => result);

            var entityMapper = this.mapper;

            await using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<Document, DocumentRow>(provider, entityMapper);
                var actual = await target.DynamicFirstOrDefaultAsync(
                                 Query.SelectEntities<DocumentRow>(
                                     select => select.Select(
                                             row => row.DocumentId,
                                             row => row.Identifier,
                                             row => row.DocumentVersion.Name,
                                             row => row.DocumentVersion.VersionNumber)
                                         .Where(set => set.AreEqual(entity => entity.DocumentId, 43))),
                                 CancellationToken.None);

                Assert.AreEqual(43, actual.DocumentId);
                Assert.AreEqual("client.org.59432-002.pdf", actual.Identifier);
                Assert.AreEqual("59432-002.pdf", result.DocumentVersionName);
                Assert.AreEqual(2, result.DocumentVersionVersionNumber);
            }
        }

        /// <summary>
        /// Tests the SelectAll method.
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

            repositoryProvider.Setup(provider => provider.SelectEntities(It.IsAny<EntitySet<SubRow>>()))
                .Returns(this.mapper.Map<List<SubRow>>(expected));

            using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<SubEntity, SubRow>(provider, this.mapper);
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
        /// Tests the SelectAllAsync method.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the result of the asynchronous operation.
        /// </returns>
        [TestMethod]
        public async Task SelectAllAsync_FakeRaisedSubEntity_MatchesExpected()
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

            var rows = this.mapper.Map<List<SubRow>>(expected);
            repositoryProvider.Setup(provider => provider.SelectEntitiesAsync(It.IsAny<EntitySet<SubRow>>(), It.IsAny<CancellationToken>()))
                .Returns(rows.ToAsyncEnumerable());

            await using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<SubEntity, SubRow>(provider, this.mapper);
                var actual = await target.SelectAllAsync(CancellationToken.None).ToListAsync();

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
        /// Tests the SelectEntities method.
        /// </summary>
        [TestMethod]
        public void SelectEntities_ReadOnlyRepositoryWithItemSelection_MatchesExpected()
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

            repositoryProvider.Setup(provider => provider.SelectEntities(It.IsAny<EntitySet<SubRow>>()))
                .Returns(this.mapper.Map<List<SubRow>>(expected));

            using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<SubEntity, SubRow>(provider, this.mapper);
                var actual = target.SelectEntities(
                        Query.Select<SubRow>().Where(set => set.AreEqual(entity => entity.SubSubEntity.FakeSubSubEntityId, 5848)))
                    .ToList();

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
        /// Tests the SelectEntitiesAsync method.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the result of the asynchronous operation.
        /// </returns>
        [TestMethod]
        public async Task SelectEntitiesAsync_ReadOnlyRepositoryWithItemSelection_MatchesExpected()
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

            var rows = this.mapper.Map<List<SubRow>>(expected);
            repositoryProvider.Setup(provider => provider.SelectEntitiesAsync(It.IsAny<EntitySet<SubRow>>(), It.IsAny<CancellationToken>()))
                .Returns(rows.ToAsyncEnumerable());

            await using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<SubEntity, SubRow>(provider, this.mapper);
                var actual = await target.SelectEntitiesAsync(
                                     Query.Select<SubRow>().Where(set => set.AreEqual(entity => entity.SubSubEntity.FakeSubSubEntityId, 5848)),
                                     CancellationToken.None)
                                 .ToListAsync();

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
        /// Tests the DynamicSelect method.
        /// </summary>
        [TestMethod]
        public void DynamicSelect_ReadOnlyRepositoryWithItemSelection_MatchesExpected()
        {
            var expected = new List<dynamic>
                           {
                               new
                               {
                                   Name = "asidf",
                                   OtherId = (short)58,
                                   SubSubEntityUniqueName = "jasdyri"
                               },
                               new
                               {
                                   Name = "safd",
                                   OtherId = (short)546,
                                   SubSubEntityUniqueName = "jasdyri"
                               },
                               new
                               {
                                   Name = "gjkdf",
                                   OtherId = (short)52,
                                   SubSubEntityUniqueName = "jasdyri"
                               }
                           };

            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            repositoryProvider.Setup(provider => provider.DynamicSelect(It.IsAny<EntitySelection<SubRow>>()))
                .Returns(expected);

            using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<SubEntity, SubRow>(provider, this.mapper);
                var actual = target.DynamicSelect(
                    Query.SelectEntities<SubRow>(
                        select => select.Where(set => set.AreEqual(entity => entity.SubSubEntity.FakeSubSubEntityId, 5848))));

                Assert.AreSame(expected, actual);
            }
        }

        /// <summary>
        /// Tests the DynamicSelectAsync method.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the result of the asynchronous operation.
        /// </returns>
        [TestMethod]
        public async Task DynamicSelectAsync_ReadOnlyRepositoryWithItemSelection_MatchesExpected()
        {
            var expected = new List<dynamic>
                           {
                               new
                               {
                                   Name = "asidf",
                                   OtherId = (short)58,
                                   SubSubEntityUniqueName = "jasdyri"
                               },
                               new
                               {
                                   Name = "safd",
                                   OtherId = (short)546,
                                   SubSubEntityUniqueName = "jasdyri"
                               },
                               new
                               {
                                   Name = "gjkdf",
                                   OtherId = (short)52,
                                   SubSubEntityUniqueName = "jasdyri"
                               }
                           };

            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            repositoryProvider.Setup(provider => provider.DynamicSelectAsync(It.IsAny<EntitySelection<SubRow>>(), It.IsAny<CancellationToken>()))
                .Returns(expected.ToAsyncEnumerable());

            await using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<SubEntity, SubRow>(provider, this.mapper);
                var actual = await target.DynamicSelectAsync(
                                     Query.SelectEntities<SubRow>(
                                         select => select.Where(set => set.AreEqual(entity => entity.SubSubEntity.FakeSubSubEntityId, 5848))),
                                     CancellationToken.None)
                                 .ToListAsync();

                CollectionAssert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        /// Tests the GetScalar method.
        /// </summary>
        [TestMethod]
        public void GetScalar_ISelection_ResultMatchesExpected()
        {
            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            repositoryProvider.Setup(provider => provider.GetScalar<int>(It.IsAny<EntitySelection<SubRow>>()))
                .Returns(3);

            using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<SubEntity, SubRow>(provider, this.mapper);
                var entitySelection = Query.Select<SubRow>(row => row.FakeSubSubEntityId);
                entitySelection.Where(set => set.AreEqual(entity => entity.SubSubEntity.FakeSubSubEntityId, 5848));
                var actual = target.GetScalar<int>(entitySelection);

                Assert.AreEqual(3, actual);
            }
        }

        /// <summary>
        /// Tests the GetScalarAsync method.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the result of the asynchronous operation.
        /// </returns>
        [TestMethod]
        public async Task GetScalarAsync_ISelection_ResultMatchesExpected()
        {
            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            repositoryProvider.Setup(provider => provider.GetScalarAsync<int>(It.IsAny<EntitySelection<SubRow>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(3);

            await using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<SubEntity, SubRow>(provider, this.mapper);
                var entitySelection = Query.Select<SubRow>(row => row.FakeSubSubEntityId);
                entitySelection.Where(set => set.AreEqual(entity => entity.SubSubEntity.FakeSubSubEntityId, 5848));
                var actual = await target.GetScalarAsync<int>(entitySelection, CancellationToken.None);

                Assert.AreEqual(3, actual);
            }
        }
    }
}