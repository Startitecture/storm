using Microsoft.VisualStudio.TestTools.UnitTesting;

using Startitecture.Orm.Common;
// --------------------------------------------------------------------------------------------------------------------
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
        /// The contains test.
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
        /// The contains item selection for existing entity returns true.
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
        /// The contains item selection for non existing entity returns true.
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
        /// The contains_ entity with dynamic unique key specified_ returns true.
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
        /// The contains_ entity with dynamic unique key specified_ returns true.
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
        /// The contains_ entity with dynamic unique key specified_ returns true.
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
        /// The contains_ entity with dynamic unique key specified_ returns true.
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
        /// The contains_ entity with dynamic unique key specified_ returns true.
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
        /// The contains_ entity with dynamic unique key specified_ returns true.
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
        /// The first or default test.
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
        /// Tests that the first or default of the repository matches the expected results.
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
        /// The select test.
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
        /// The select test.
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

        [TestMethod()]
        public void ContainsAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ContainsAsyncTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void FirstOrDefaultAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void FirstOrDefaultAsyncTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void FirstOrDefaultAsyncTest2()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DynamicFirstOrDefaultAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetScalarTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetScalarAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SelectAllAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SelectEntitiesAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DynamicSelectAsyncTest()
        {
            Assert.Fail();
        }
    }
}