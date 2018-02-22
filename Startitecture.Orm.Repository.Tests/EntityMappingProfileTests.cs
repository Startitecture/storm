// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityMappingProfileTests.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Repository.Tests.Models;
    using Startitecture.Orm.Testing.RhinoMocks;

    /// <summary>
    /// The entity mapping profile tests.
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class EntityMappingProfileTests
    {
        /// <summary>
        /// The map to_ inherited entity from key_ matches expected.
        /// </summary>
        [TestMethod]
        public void MapTo_InheritedEntityFromKey_MatchesExpected()
        {
            var mapper = new AutoMapperEntityMapper();
            mapper.Initialize(expression => expression.AddProfile<FakeMultiReferenceEntityMappingProfile>());

            var createdBy = new FakeCreatedBy("asjdl") { Description = "Whatever" };

            mapper.MapTo(543, createdBy);

            Assert.AreEqual<int?>(543, createdBy.FakeMultiReferenceEntityId);
        }

        /// <summary>
        /// The map to_ inherited entity from key with overlapping profiles base profile last_ matches expected.
        /// </summary>
        [TestMethod]
        public void MapTo_InheritedEntityFromKeyWithOverlappingProfilesBaseProfileLast_MatchesExpected()
        {
            var mapper = new AutoMapperEntityMapper();
            mapper.Initialize(
                expression =>
                    {
                        expression.AddProfile<FakeMultiReferenceEntityMappingProfile>();
                        expression.AddProfile<FakeComplexEntityMappingProfile>();
                    });

            var createdBy = new FakeCreatedBy("asjdl") { Description = "Whatever" };

            mapper.MapTo(543, createdBy);

            Assert.AreEqual<int?>(543, createdBy.FakeMultiReferenceEntityId);
        }

        /// <summary>
        /// The map to_ inherited entity from key with overlapping profiles base profile first_ matches expected.
        /// </summary>
        [TestMethod]
        public void MapTo_InheritedEntityFromKeyWithOverlappingProfilesBaseProfileFirst_MatchesExpected()
        {
            var mapper = new AutoMapperEntityMapper();
            mapper.Initialize(
                expression =>
                    {
                        expression.AddProfile<FakeMultiReferenceEntityMappingProfile>();
                        expression.AddProfile<FakeComplexEntityMappingProfile>();
                    });

            var createdBy = new FakeCreatedBy("asjdl") { Description = "Whatever" };

            mapper.MapTo(543, createdBy);

            Assert.AreEqual<int?>(543, createdBy.FakeMultiReferenceEntityId);
        }

        /// <summary>
        /// The map to_ inherited entity from key_ matches expected.
        /// </summary>
        [TestMethod]
        public void MapTo_SuperClassFromKey_MatchesExpected()
        {
            var mapper = new AutoMapperEntityMapper();
            mapper.Initialize(
                expression =>
                {
                    expression.AddProfile<MySuperClassMappingProfile>();
                });

            var expected = new MySuperClass(45783) { Name = "MyName" };
            var actual = mapper.MapTo((long)8743, expected);
            Assert.AreEqual<long?>(8743, actual.MySuperClassId);
        }

        /// <summary>
        /// The map to_ inherited entity from key_ matches expected.
        /// </summary>
        [TestMethod]
        public void MapTo_SubClassFromKey_MatchesExpected()
        {
            var mapper = new AutoMapperEntityMapper();
            mapper.Initialize(
                expression =>
                {
                    expression.AddProfile<MySubClassMappingProfile>();
                });

            var expected = new MySubClass(45783) { Name = "MyName" };
            var actual = mapper.MapTo((long)8743, expected);
            Assert.AreEqual<long?>(8743, actual.MySuperClassId);
        }

        /// <summary>
        /// The map to_ inherited entity from key with overlapping profiles base profile last_ matches expected.
        /// </summary>
        [TestMethod]
        public void MapTo_SubClassFromKeyWithOverlappingProfilesBaseProfileLast_MatchesExpected()
        {
            var mapper = new AutoMapperEntityMapper();
            mapper.Initialize(
                expression =>
                {
                    expression.AddProfile<MySubClassMappingProfile>();
                    expression.AddProfile<MySuperClassMappingProfile>();
                });

            var expected = new MySubClass(45783) { Name = "MyName" };
            var actual = mapper.MapTo((long)8743, expected);
            Assert.AreEqual<long?>(8743, actual.MySuperClassId);
        }

        /// <summary>
        /// The map to_ inherited entity from key with overlapping profiles base profile first_ matches expected.
        /// </summary>
        [TestMethod]
        public void MapTo_SubClassFromKeyWithOverlappingProfilesBaseProfileFirst_MatchesExpected()
        {
            var mapper = new AutoMapperEntityMapper();
            mapper.Initialize(
                expression =>
                {
                    expression.AddProfile<MySuperClassMappingProfile>();
                    expression.AddProfile<MySubClassMappingProfile>();
                });

            var expected = new MySubClass(45783) { Name = "MyName" };
            var actual = mapper.MapTo((long)8743, expected);
            Assert.AreEqual<long?>(8743, actual.MySuperClassId);
        }

        /// <summary>
        /// The create dependency map test.
        /// </summary>
        [TestMethod]
        public void CreateDependencyMap_MapFakeDependencyFromKey_MatchesExpected()
        {
            var target = new FakeComplexEntityMappingProfile();
            target.CreateDependencyMap<FakeDependencyRow, long>(row => row.FakeDependencyEntityId, row => row.UniqueName);
            var entityMapper = new AutoMapperEntityMapper();
            entityMapper.Initialize(configuration => configuration.AddProfile(target));

            var expected = new FakeDependencyRow { FakeDependencyEntityId = 223 };
            var actual = entityMapper.Map<FakeDependencyRow>(expected.FakeDependencyEntityId);
            Assert.AreEqual<string>(expected.Description, actual.Description);
            Assert.AreEqual<int>(expected.FakeComplexEntityId, actual.FakeComplexEntityId);
            Assert.AreEqual<long>(expected.FakeDependencyEntityId, actual.FakeDependencyEntityId);
            Assert.AreEqual<string>(expected.UniqueName, actual.UniqueName);
        }

        /// <summary>
        /// The create dependency map test.
        /// </summary>
        [TestMethod]
        public void CreateDependencyMap_MapKeyToFakeDependency_MatchesExpected()
        {
            var target = new FakeComplexEntityMappingProfile();
            target.CreateDependencyMap<FakeDependencyRow, long>(row => row.FakeDependencyEntityId, row => row.UniqueName);
            var entityMapper = new AutoMapperEntityMapper();
            entityMapper.Initialize(configuration => configuration.AddProfile(target));

            var expected = new FakeDependencyRow { FakeDependencyEntityId = 223 };
            var actual = entityMapper.Map<FakeDependencyRow>(expected.FakeDependencyEntityId);
            Assert.AreEqual<string>(expected.Description, actual.Description);
            Assert.AreEqual<int>(expected.FakeComplexEntityId, actual.FakeComplexEntityId);
            Assert.AreEqual<long>(expected.FakeDependencyEntityId, actual.FakeDependencyEntityId);
            Assert.AreEqual<string>(expected.UniqueName, actual.UniqueName);
        }

        /// <summary>
        /// The create dependency map test.
        /// </summary>
        [TestMethod]
        public void CreateDependencyMap_MapFakeDependencyToFakeDependency_MatchesExpected()
        {
            var target = new FakeComplexEntityMappingProfile();
            target.CreateDependencyMap<FakeDependencyRow, long>(row => row.FakeDependencyEntityId, row => row.UniqueName);
            var entityMapper = new AutoMapperEntityMapper();
            entityMapper.Initialize(configuration => configuration.AddProfile(target));

            var existing = new FakeDependencyRow
                               {
                                   Description = "The old description",
                                   FakeDependencyEntityId = 223,
                                   UniqueName = "MyName",
                                   FakeComplexEntityId = 111
                               };

            var updated = new FakeDependencyRow
                              {
                                  Description = "The new description",
                                  FakeDependencyEntityId = existing.FakeDependencyEntityId,
                                  UniqueName = "Should not see this",
                                  FakeComplexEntityId = existing.FakeComplexEntityId
                              };

            var expected = new FakeDependencyRow
                               {
                                   FakeDependencyEntityId = existing.FakeDependencyEntityId,
                                   Description = updated.Description,
                                   UniqueName = existing.UniqueName,
                                   FakeComplexEntityId = updated.FakeComplexEntityId
                               };

            var actual = entityMapper.MapTo(updated, existing);
            Assert.AreEqual<string>(expected.Description, actual.Description);
            Assert.AreEqual<int>(expected.FakeComplexEntityId, actual.FakeComplexEntityId);
            Assert.AreEqual<long>(expected.FakeDependencyEntityId, actual.FakeDependencyEntityId);
            Assert.AreEqual<string>(expected.UniqueName, actual.UniqueName);
        }

        /// <summary>
        /// The create dependency map test.
        /// </summary>
        [TestMethod]
        public void CreateDependencyMap_GetKeyFromFakeDependency_MatchesExpected()
        {
            var target = new FakeComplexEntityMappingProfile();
            target.CreateDependencyMap<FakeDependencyRow, long>(row => row.FakeDependencyEntityId, row => row.UniqueName);
            var entityMapper = new AutoMapperEntityMapper();
            entityMapper.Initialize(configuration => configuration.AddProfile(target));

            var expected = new FakeDependencyRow { FakeDependencyEntityId = 223 };
            var actual = entityMapper.Map<long>(expected);
            Assert.AreEqual<long>(expected.FakeDependencyEntityId, actual);
        }

        /// <summary>
        /// The map_ complex entity to entity from row_ matches expected.
        /// </summary>
        [TestMethod]
        public void Map_ComplexEntityToEntityFromRow_MatchesExpected()
        {
            var mapper = new AutoMapperEntityMapper();
            mapper.Initialize(expression => expression.AddProfile<FakeComplexEntityMappingProfile>());

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

            var dependentTimeValue = DateTimeOffset.Now.Subtract(TimeSpan.FromDays(3));
            expected.SetDependentEntity(392, dependentTimeValue);

            var complexRow = mapper.Map<FakeComplexRow>(expected);

            ////complexRow.FakeDependentEntityId = 3392;
            var actual = mapper.Map<FakeComplexEntity>(complexRow);

            Assert.AreEqual<int?>(expected.FakeComplexEntityId, actual.FakeComplexEntityId);
            Assert.AreEqual<int?>(expected.CreatedByFakeMultiReferenceEntityId, actual.CreatedByFakeMultiReferenceEntityId);
            Assert.AreEqual<int?>(expected.FakeDependentEntityId, actual.FakeDependentEntityId);
            Assert.AreEqual<int?>(expected.FakeSubEntityId, actual.FakeSubEntityId);
            Assert.AreEqual<int?>(expected.FakeSubSubEntityId, actual.FakeSubSubEntityId);
            Assert.AreEqual<int?>(expected.ModifiedByFakeMultiReferenceEntityId, actual.ModifiedByFakeMultiReferenceEntityId);
            Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
        }

        /// <summary>
        /// The map_ missing dependent row_ matches expected.
        /// </summary>
        [TestMethod]
        public void Map_ComplexEntityToEntityFromRaisedRowWithoutDependent_MatchesExpected()
        {
            var mapper = new AutoMapperEntityMapper();
            mapper.Initialize(
                expression =>
                {
                    expression.AddProfile<FakeRaisedComplexEntityMappingProfile>();
                    expression.AddProfile<FakeCreatedByMappingProfile>();
                    expression.AddProfile<FakeDependentEntityMappingProfile>();
                    expression.AddProfile<FakeModifiedByMappingProfile>();
                    expression.AddProfile<FakeRaisedSubEntityMappingProfile>();
                    expression.AddProfile<FakeSubSubEntityMappingProfile>();
                });

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

            var complexRow = mapper.Map<FakeRaisedComplexRow>(expected);
            var actual = mapper.Map<FakeComplexEntity>(complexRow);

            Assert.AreEqual<int?>(expected.FakeComplexEntityId, actual.FakeComplexEntityId);
            Assert.AreEqual<int?>(expected.CreatedByFakeMultiReferenceEntityId, actual.CreatedByFakeMultiReferenceEntityId);
            Assert.AreEqual<int?>(expected.FakeDependentEntityId, actual.FakeDependentEntityId);
            Assert.AreEqual<int?>(expected.FakeSubEntityId, actual.FakeSubEntityId);
            Assert.AreEqual<int?>(expected.FakeSubSubEntityId, actual.FakeSubSubEntityId);
            Assert.AreEqual<int?>(expected.ModifiedByFakeMultiReferenceEntityId, actual.ModifiedByFakeMultiReferenceEntityId);
            Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
        }

        /// <summary>
        /// The map_ complex entity to entity from raised row_ matches expected.
        /// </summary>
        [TestMethod]
        public void Map_ComplexEntityToEntityFromRaisedRow_MatchesExpected()
        {
            var mapper = new AutoMapperEntityMapper();
            mapper.Initialize(
                expression =>
                    {
                        expression.AddProfile<FakeRaisedComplexEntityMappingProfile>();
                        expression.AddProfile<FakeCreatedByMappingProfile>();
                        expression.AddProfile<FakeDependentEntityMappingProfile>();
                        expression.AddProfile<FakeModifiedByMappingProfile>();
                        expression.AddProfile<FakeRaisedSubEntityMappingProfile>();
                        expression.AddProfile<FakeSubSubEntityMappingProfile>();
                    });

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

            var dependentTimeValue = DateTimeOffset.Now.Subtract(TimeSpan.FromDays(3));
            expected.SetDependentEntity(392, dependentTimeValue);

            var complexRow = mapper.Map<FakeRaisedComplexRow>(expected);
            var actual = mapper.Map<FakeComplexEntity>(complexRow);

            Assert.AreEqual<int?>(expected.FakeComplexEntityId, actual.FakeComplexEntityId);
            Assert.AreEqual<int?>(expected.CreatedByFakeMultiReferenceEntityId, actual.CreatedByFakeMultiReferenceEntityId);
            Assert.AreEqual<int?>(expected.FakeDependentEntityId, actual.FakeDependentEntityId);
            Assert.AreEqual<int?>(expected.FakeSubEntityId, actual.FakeSubEntityId);
            Assert.AreEqual<int?>(expected.FakeSubSubEntityId, actual.FakeSubSubEntityId);
            Assert.AreEqual<int?>(expected.ModifiedByFakeMultiReferenceEntityId, actual.ModifiedByFakeMultiReferenceEntityId);
            Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
        }

        /// <summary>
        /// The map_ complex entity to entity from raised row_ matches expected.
        /// </summary>
        [TestMethod]
        public void Map_ListOfComplexEntityToEntityFromRaisedRow_MatchesExpected()
        {
            var mapper = new AutoMapperEntityMapper();
            mapper.Initialize(
                expression =>
                    {
                        expression.AddProfile<FakeRaisedComplexEntityMappingProfile>();
                        expression.AddProfile<FakeCreatedByMappingProfile>();
                        expression.AddProfile<FakeDependentEntityMappingProfile>();
                        expression.AddProfile<FakeModifiedByMappingProfile>();
                        expression.AddProfile<FakeRaisedSubEntityMappingProfile>();
                        expression.AddProfile<FakeSubSubEntityMappingProfile>();
                    });

            var fakeSubSubEntity = new FakeSubSubEntity("SubSubUniqueName1", 45) { Description = "OriginalSubSub" };
            var fakeSubEntity = new FakeSubEntity("SubUniqueName1", 234, fakeSubSubEntity, 16) { Description = "OriginalSub" };
            var originalCreatedBy = new FakeCreatedBy("CreateUniqueName", 432) { Description = "OriginalCreatedBy" };
            var modifiedBy = new FakeModifiedBy("ModifiedBy1", 433) { Description = "OriginalModifiedBy1" };
            var creationTime = DateTimeOffset.Now.AddDays(-1);

            var entity1 = new FakeComplexEntity("UniqueName1", fakeSubEntity, FakeEnumeration.FirstFake, originalCreatedBy, creationTime, 22)
                              {
                                  Description = "OriginalComplexEntity1",
                                  ModifiedBy = modifiedBy,
                                  ModifiedTime = DateTimeOffset.Now.AddHours(1)
                              };

            entity1.SetDependentEntity(392, DateTimeOffset.Now.Subtract(TimeSpan.FromDays(3)));

            var entity2 = new FakeComplexEntity("UniqueName2", fakeSubEntity, FakeEnumeration.SecondFake, originalCreatedBy, creationTime, 23)
                              {
                                  Description = "OriginalComplexEntity2",
                                  ModifiedBy = modifiedBy,
                                  ModifiedTime = DateTimeOffset.Now.AddHours(1)
                              };

            entity2.SetDependentEntity(395, DateTimeOffset.Now.Subtract(TimeSpan.FromDays(5)));

            var entity3 = new FakeComplexEntity("UniqueName3", fakeSubEntity, FakeEnumeration.ThirdFake, originalCreatedBy, creationTime, 24)
                              {
                                  Description = "OriginalComplexEntity3",
                                  ModifiedBy = modifiedBy,
                                  ModifiedTime = DateTimeOffset.Now.AddHours(1)
                              };

            entity3.SetDependentEntity(398, DateTimeOffset.Now.Subtract(TimeSpan.FromDays(7)));

            var expected = new List<FakeComplexEntity> { entity1, entity2, entity3 };

            using (var repositoryProvider = RepositoryMockFactory.CreateProvider(mapper))
            {
                var rows = mapper.Map<List<FakeRaisedComplexRow>>(expected);

                foreach (var row in rows)
                {
                    row.SetTransactionProvider(repositoryProvider);
                }

                var actual = mapper.Map<List<FakeComplexEntity>>(rows);

                var actualSubEntity = actual.FirstOrDefault()?.FakeSubEntity;
                Assert.IsNotNull(actualSubEntity);

                var actualSubSubEntity = actualSubEntity.FakeSubSubEntity;
                Assert.IsNotNull(actualSubSubEntity);

                var actualCreatedBy = actual.FirstOrDefault()?.CreatedBy;
                Assert.IsNotNull(actualCreatedBy);

                var actualModifiedBy = actual.FirstOrDefault()?.ModifiedBy;
                Assert.IsNotNull(actualModifiedBy);

                foreach (var expectedEntity in expected)
                {
                    var actualEntity = actual.Find(complexEntity => complexEntity.FakeComplexEntityId == expectedEntity.FakeComplexEntityId);

                    Assert.IsNotNull(actualEntity);
                    Assert.AreEqual<int?>(expectedEntity.FakeComplexEntityId, actualEntity.FakeComplexEntityId);
                    Assert.AreEqual<int?>(expectedEntity.CreatedByFakeMultiReferenceEntityId, actualEntity.CreatedByFakeMultiReferenceEntityId);
                    Assert.AreEqual<int?>(expectedEntity.FakeDependentEntityId, actualEntity.FakeDependentEntityId);
                    Assert.AreEqual<int?>(expectedEntity.FakeSubEntityId, actualEntity.FakeSubEntityId);
                    Assert.AreEqual<int?>(expectedEntity.FakeSubSubEntityId, actualEntity.FakeSubSubEntityId);
                    Assert.AreEqual<int?>(expectedEntity.ModifiedByFakeMultiReferenceEntityId, actualEntity.ModifiedByFakeMultiReferenceEntityId);
                    Assert.AreEqual(expectedEntity, actualEntity, string.Join(Environment.NewLine, expectedEntity, actualEntity));

                    Assert.AreSame(actualSubEntity, actualEntity.FakeSubEntity);
                    Assert.AreSame(actualSubSubEntity, actualEntity.FakeSubSubEntity);
                    Assert.AreSame(actualCreatedBy, actualEntity.CreatedBy);
                    Assert.AreSame(actualModifiedBy, actualEntity.ModifiedBy);
                }
            }
        }

        /// <summary>
        /// The my super class.
        /// </summary>
        private class MySuperClass
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MySuperClass"/> class.
            /// </summary>
            public MySuperClass()
                : this(null)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="MySuperClass"/> class.
            /// </summary>
            /// <param name="mySuperClassId">
            /// The my super id.
            /// </param>
            public MySuperClass(long? mySuperClassId)
            {
                this.MySuperClassId = mySuperClassId;
            }

            /// <summary>
            /// Gets the my super id.
            /// </summary>
            public long? MySuperClassId { get; private set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            public string Name { get; set; }
        }

        /// <summary>
        /// The my sub class.
        /// </summary>
        private class MySubClass : MySuperClass
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MySubClass"/> class.
            /// </summary>
            /// <param name="myRelatedId">
            /// The my related ID.
            /// </param>
            public MySubClass(int myRelatedId)
                : this(myRelatedId, null)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="MySubClass"/> class.
            /// </summary>
            /// <param name="myRelatedId">
            /// The my related ID.
            /// </param>
            /// <param name="mySubClassId">
            /// The my sub ID.
            /// </param>
            public MySubClass(int myRelatedId, long? mySubClassId)
                : base(mySubClassId)
            {
                this.MyRelatedId = myRelatedId;
            }

            /// <summary>
            /// Gets the my sub ID.
            /// </summary>
            public long? MySubClassId
            {
                get
                {
                    return this.MySuperClassId;
                }
            }

            /// <summary>
            /// Gets the my related ID.
            /// </summary>
            public int MyRelatedId { get; private set; }
        }

        /// <summary>
        /// The my super class row.
        /// </summary>
        private class MySuperClassRow : TransactionContainer
        {
            /// <summary>
            /// Gets or sets the my super class id.
            /// </summary>
            public long MySuperClassId { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            public string Name { get; set; }
        }

        /// <summary>
        /// The my sub class row.
        /// </summary>
        private class MySubClassRow : TransactionContainer
        {
            /// <summary>
            /// Gets or sets the my sub class id.
            /// </summary>
            public long MySubClassId { get; set; }

            /// <summary>
            /// Gets or sets the my super class id.
            /// </summary>
            public long MySuperClassId { get; set; }

            /// <summary>
            /// Gets or sets the my super class name.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the my related id.
            /// </summary>
            public int MyRelatedId { get; set; }
        }

        /// <summary>
        /// The my super class mapping profile.
        /// </summary>
        private class MySuperClassMappingProfile : EntityMappingProfile<MySuperClass, MySuperClassRow>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MySuperClassMappingProfile"/> class.
            /// </summary>
            public MySuperClassMappingProfile()
            {
                this.SetPrimaryKey(superClass => superClass.MySuperClassId, row => row.MySuperClassId);
            }
        }

        /// <summary>
        /// The my sub class mapping profile.
        /// </summary>
        private class MySubClassMappingProfile : EntityMappingProfile<MySubClass, MySubClassRow>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MySubClassMappingProfile"/> class.
            /// </summary>
            public MySubClassMappingProfile()
            {
                this.SetPrimaryKey(subClass => subClass.MySuperClassId, row => row.MySubClassId);
            }
        }
    }
}