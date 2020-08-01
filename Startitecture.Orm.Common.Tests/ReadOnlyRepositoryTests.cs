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
        private readonly IEntityMapperFactory mapperFactory = new EntityMapperFactory(
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
                    }));

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

            var mapper = this.mapperFactory.Create();
            var existing = mapper.Map<ComplexRaisedRow>(expected);

            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            repositoryProvider.Setup(provider => provider.Contains(It.IsAny<EntitySet<ComplexRaisedRow>>()))
                .Returns(true);

            using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<ComplexEntity, ComplexRaisedRow>(provider, mapper);
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
                var target = new ReadOnlyRepository<ComplexEntity, ComplexRaisedRow>(provider, this.mapperFactory.Create());
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
                var target = new ReadOnlyRepository<ComplexEntity, ComplexRaisedRow>(provider, this.mapperFactory.Create());
                var actual = target.Contains(Select.Where<ComplexRaisedRow>().WhereEqual(row => row.FakeSubEntityId, 22));
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
                var target = new ReadOnlyRepository<ComplexEntity, ComplexRaisedRow>(provider, this.mapperFactory.Create());
                var actual = target.Contains(Select.Where<ComplexRaisedRow>().WhereEqual(row => row.FakeSubEntityId, 22));
                Assert.IsFalse(actual);
            }
        }

        /// <summary>
        /// The contains entity with string unique key specified returns true.
        /// </summary>
        [TestMethod]
        public void Contains_EntityWithStringUniqueKeySpecified_ReturnsTrue()
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
                var target = new ReadOnlyRepository<ComplexEntity, ComplexRaisedRow>(provider, this.mapperFactory.Create(), row => row.UniqueName);
                var actual = target.Contains("UniqueName");
                Assert.IsTrue(actual);
            }
        }

        /// <summary>
        /// The contains entity with value type unique key specified returns true.
        /// </summary>
        [TestMethod]
        public void Contains_EntityWithValueTypeUniqueKeySpecified_ReturnsTrue()
        {
            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            repositoryProvider.Setup(
                    provider => provider.Contains(
                        It.Is<EntitySet<DependentRow>>(
                            selection => selection.PropertyValues.Count() == 1 && selection.PropertyValues.First() as int? == 24)))
                .Returns(true);

            using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<DependentEntity, DependentRow>(provider, this.mapperFactory.Create(), row => row.DependentIntegerValue);
                var actual = target.Contains(24);
                Assert.IsTrue(actual);
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
                var target = new ReadOnlyRepository<ComplexEntity, ComplexRaisedRow>(provider, this.mapperFactory.Create(), row => row.UniqueName);
                dynamic key = new ExpandoObject();
                key.UniqueName = "UniqueName";
                key.Foo = 12;

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
                        It.Is<EntitySet<ComplexRaisedRow>>(
                            selection => selection.PropertyValues.Count() == 1 && selection.PropertyValues.First() as string == "UniqueName")))
                .Returns(true);

            using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<ComplexEntity, ComplexRaisedRow>(provider, this.mapperFactory.Create(), row => row.UniqueName);

                var actual = target.Contains(
                    new
                    {
                        UniqueName = "UniqueName",
                        Foo = 12
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

            var mapper = this.mapperFactory.Create();
            var existing = mapper.Map<ComplexRaisedRow>(expected);

            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            repositoryProvider.Setup(provider => provider.FirstOrDefault(It.IsAny<EntitySelection<ComplexRaisedRow>>()))
                .Returns(mapper.Map<ComplexRaisedRow>(existing));

            using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<ComplexEntity, ComplexRaisedRow>(provider, mapper);
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

            var mapper = this.mapperFactory.Create();
            var existing = mapper.Map<ComplexRaisedRow>(expected);

            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            repositoryProvider.Setup(provider => provider.FirstOrDefault(It.IsAny<EntitySelection<ComplexRaisedRow>>()))
                .Returns(mapper.Map<ComplexRaisedRow>(existing));

            using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<ComplexEntity, ComplexRaisedRow>(provider, mapper);
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

            var mapper = this.mapperFactory.Create();
            var existing = mapper.Map<ComplexRaisedRow>(expected);

            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            repositoryProvider.Setup(provider => provider.FirstOrDefault(It.IsAny<EntitySelection<ComplexRaisedRow>>()))
                .Returns(mapper.Map<ComplexRaisedRow>(existing));

            using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<ComplexEntity, ComplexRaisedRow>(provider, mapper);
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
        /// The contains entity with string unique key specified returns true.
        /// </summary>
        [TestMethod]
        public void FirstOrDefault_EntityWithStringUniqueKeySpecified_ReturnsEntity()
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
                        It.Is<EntitySelection<ComplexRaisedRow>>(
                            selection => selection.PropertyValues.Count() == 1 && selection.PropertyValues.First() as string == "UniqueName")))
                .Returns(expected);

            var entityMapper = this.mapperFactory.Create();

            using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<ComplexEntity, ComplexRaisedRow>(provider, entityMapper, row => row.UniqueName);
                var actual = target.FirstOrDefault("UniqueName");
                Assert.AreEqual(entityMapper.Map<ComplexEntity>(expected), actual);
            }
        }

        /// <summary>
        /// The contains entity with value type unique key specified returns true.
        /// </summary>
        [TestMethod]
        public void FirstOrDefault_EntityWithValueTypeUniqueKeySpecified_ReturnsEntity()
        {
            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            var expected = new DependentRow
            {
                FakeDependentEntityId = 46,
                DependentIntegerValue = 24,
                DependentTimeValue = DateTimeOffset.Now
            };

            repositoryProvider.Setup(
                    provider => provider.FirstOrDefault(
                        It.Is<EntitySelection<DependentRow>>(
                            selection => selection.PropertyValues.Count() == 1 && selection.PropertyValues.First() as int? == 24)))
                .Returns(expected);

            var entityMapper = this.mapperFactory.Create();

            using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<DependentEntity, DependentRow>(provider, entityMapper, row => row.DependentIntegerValue);
                var actual = target.FirstOrDefault(24);
                Assert.AreEqual(entityMapper.Map<DependentEntity>(expected), actual);
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
                        It.Is<EntitySelection<ComplexRaisedRow>>(
                            selection => selection.PropertyValues.Count() == 1 && selection.PropertyValues.First() as string == "UniqueName")))
                .Returns(expected);

            var entityMapper = this.mapperFactory.Create();

            using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<ComplexEntity, ComplexRaisedRow>(provider, entityMapper, row => row.UniqueName);
                dynamic key = new ExpandoObject();
                key.UniqueName = "UniqueName";
                key.Foo = 12;

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
                        It.Is<EntitySelection<ComplexRaisedRow>>(
                            selection => selection.PropertyValues.Count() == 1 && selection.PropertyValues.First() as string == "UniqueName")))
                .Returns(expected);

            var entityMapper = this.mapperFactory.Create();

            using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<ComplexEntity, ComplexRaisedRow>(provider, entityMapper, row => row.UniqueName);

                var actual = target.FirstOrDefault(
                    new
                    {
                        UniqueName = "UniqueName",
                        Foo = 12
                    });

                Assert.AreEqual(entityMapper.Map<ComplexEntity>(expected), actual);
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
                        It.Is<EntitySelection<DocumentRow>>(
                            selection => selection.PropertyValues.Count() == 1 && selection.PropertyValues.First() as int? == 43)))
                .Returns(result);

            var entityMapper = this.mapperFactory.Create();

            using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<Document, DocumentRow>(provider, entityMapper, row => row.Identifier);
                var actual = target.DynamicFirstOrDefault(
                    Select.From<DocumentRow>(
                            row => row.DocumentId,
                            row => row.Identifier, 
                            row => row.DocumentVersion.Name,
                            row => row.DocumentVersion.VersionNumber)
                        .WhereEqual(entity => entity.DocumentId, 43));

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

            var mapper = this.mapperFactory.Create();
            repositoryProvider.Setup(provider => provider.SelectEntities(It.IsAny<EntitySelection<SubRow>>()))
                .Returns(mapper.Map<List<SubRow>>(expected));

            using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<SubEntity, SubRow>(provider, mapper);
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

            var mapper = this.mapperFactory.Create();
            repositoryProvider.Setup(provider => provider.SelectEntities(It.IsAny<EntitySelection<SubRow>>()))
                .Returns(mapper.Map<List<SubRow>>(expected));

            using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<SubEntity, SubRow>(provider, mapper);
                var actual = target.SelectEntities(Select.From<SubRow>().WhereEqual(entity => entity.SubSubEntity.FakeSubSubEntityId, 5848)).ToList();

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
                                   new { Name = "asidf", OtherId = (short)58, SubSubEntityUniqueName = "jasdyri" },
                                   new { Name = "safd", OtherId = (short)546, SubSubEntityUniqueName = "jasdyri" },
                                   new { Name = "gjkdf", OtherId = (short)52, SubSubEntityUniqueName = "jasdyri" }
                               };

            var repositoryProvider = new Mock<IRepositoryProvider>();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryProvider.Setup(provider => provider.EntityDefinitionProvider).Returns(definitionProvider);

            var mapper = this.mapperFactory.Create();
            repositoryProvider.Setup(provider => provider.DynamicSelect(It.IsAny<EntitySelection<SubRow>>()))
                .Returns(expected);

            using (var provider = repositoryProvider.Object)
            {
                var target = new ReadOnlyRepository<SubEntity, SubRow>(provider, mapper);
                var actual = target.DynamicSelect(Select.From<SubRow>().WhereEqual(entity => entity.SubSubEntity.FakeSubSubEntityId, 5848));
                Assert.AreSame(expected, actual);
            }
        }
    }
}