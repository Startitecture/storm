// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataMappingProfileTests.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Tests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SAF.Testing.Common;

    using Startitecture.Repository.Mapping;

    /// <summary>
    /// The data mapping profile tests.
    /// </summary>
    [TestClass]
    public class DataMappingProfileTests
    {
        /// <summary>
        /// The set dependency key test.
        /// </summary>
        [TestMethod]
        public void SetDependencyKey_FakeComplexRowWithFakeSubRowDependency_MapsId()
        {
            var entityMapper = new AutoMapperEntityMapper();
            entityMapper.Initialize(
                expression =>
                    {
                        expression.AddProfile(new DataMappingProfile<FakeComplexRow>().SetDependencyKey<FakeSubRow, int>(row => row.FakeSubEntityId));
                    });

            var fakeSubRow = new FakeSubRow { FakeSubEntityId = 24 };
            var actual = entityMapper.MapTo(fakeSubRow, new FakeComplexRow());
            Assert.AreEqual(fakeSubRow.FakeSubEntityId, actual.FakeSubEntityId);
        }

        /// <summary>
        /// The set dependency key test 1.
        /// </summary>
        [TestMethod]
        public void SetDependencyKey_FakeComplexRowWithFakeMultiReferenceRowDependency_MapsId()
        {
            var entityMapper = new AutoMapperEntityMapper();
            entityMapper.Initialize(
                expression =>
                    {
                        expression.AddProfile(
                            new DataMappingProfile<FakeComplexRow>().SetDependencyKey<FakeMultiReferenceRow, int>(
                                row => row.FakeMultiReferenceEntityId,
                                row => row.CreatedByFakeMultiReferenceEntityId));
                    });

            var fakeMultiReferenceRow = new FakeMultiReferenceRow { FakeMultiReferenceEntityId = 24 };
            var actual = entityMapper.MapTo(fakeMultiReferenceRow, new FakeComplexRow());
            Assert.AreEqual(fakeMultiReferenceRow.FakeMultiReferenceEntityId, actual.CreatedByFakeMultiReferenceEntityId);
        }

        /// <summary>
        /// The set primary key test.
        /// </summary>
        [TestMethod]
        public void SetPrimaryKey_FakeComplexRow_KeyIsMappedToRow()
        {
            var entityMapper = new AutoMapperEntityMapper();
            entityMapper.Initialize(
                expression =>
                    {
                        expression.AddProfile(new DataMappingProfile<FakeComplexRow>().SetPrimaryKey(row => row.FakeComplexEntityId));
                    });

            var actual = entityMapper.MapTo(453, new FakeComplexRow());
            Assert.AreEqual(453, actual.FakeComplexEntityId);
        }

        /// <summary>
        /// The set primary key test.
        /// </summary>
        [TestMethod]
        public void SetPrimaryKey_FakeComplexRow_KeyIsIgnoredInMerge()
        {
            var entityMapper = new AutoMapperEntityMapper();
            entityMapper.Initialize(
                expression =>
                {
                    expression.AddProfile(new DataMappingProfile<FakeComplexRow>().SetPrimaryKey(row => row.FakeComplexEntityId));
                });

            var creationTime = DateTimeOffset.Now.AddDays(-10);
            var expected = new FakeComplexRow
                               {
                                   FakeEnumerationId = 45,
                                   FakeOtherEnumerationId = 54,
                                   FakeSubEntityId = 587,
                                   Description = "new stuff",
                                   CreationTime = creationTime,
                                   CreatedByFakeMultiReferenceEntityId = 5487,
                                   ModifiedTime = DateTimeOffset.Now,
                                   ModifiedByFakeMultiReferenceEntityId = 4435,
                                   UniqueName = "UniqueName"
                               };

            var target = new FakeComplexRow
                             {
                                 FakeComplexEntityId = 648,
                                 FakeEnumerationId = 45,
                                 FakeOtherEnumerationId = 54,
                                 FakeSubEntityId = 587,
                                 Description = "old stuff",
                                 CreationTime = creationTime,
                                 CreatedByFakeMultiReferenceEntityId = 5487,
                                 ModifiedTime = creationTime,
                                 ModifiedByFakeMultiReferenceEntityId = 5487,
                                 UniqueName = "UniqueName"
                             };

            var actual = entityMapper.MapTo(expected, target);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(target.FakeComplexEntityId, actual.FakeComplexEntityId);
        }

        /// <summary>
        /// The write once test.
        /// </summary>
        [TestMethod]
        public void WriteOnce_FakeComplexRow_PropertiesIgnoredInMerge()
        {
            var entityMapper = new AutoMapperEntityMapper();
            entityMapper.Initialize(
                expression =>
                    {
                        expression.AddProfile(
                            new DataMappingProfile<FakeComplexRow>().WriteOnce(
                                row => row.FakeComplexEntityId,
                                row => row.CreatedByFakeMultiReferenceEntityId,
                                row => row.CreationTime));
                    });

            var modifiedTime = DateTimeOffset.Now;
            var creationTime = modifiedTime.AddDays(-10);

            var expected = new FakeComplexRow
                               {
                                   FakeEnumerationId = 45,
                                   FakeOtherEnumerationId = 54,
                                   FakeSubEntityId = 587,
                                   Description = "new stuff",
                                   CreationTime = modifiedTime,
                                   CreatedByFakeMultiReferenceEntityId = 4435,
                                   ModifiedTime = modifiedTime,
                                   ModifiedByFakeMultiReferenceEntityId = 4435,
                                   UniqueName = "UniqueName"
                               };

            var target = new FakeComplexRow
                             {
                                 FakeComplexEntityId = 648,
                                 FakeEnumerationId = 45,
                                 FakeOtherEnumerationId = 54,
                                 FakeSubEntityId = 587,
                                 Description = "old stuff",
                                 CreationTime = creationTime,
                                 CreatedByFakeMultiReferenceEntityId = 5487,
                                 ModifiedTime = creationTime,
                                 ModifiedByFakeMultiReferenceEntityId = 5487,
                                 UniqueName = "UniqueName"
                             };

            var actual = entityMapper.MapTo(expected, target);
            Assert.AreEqual(modifiedTime, actual.ModifiedTime);
            Assert.AreEqual(4435, actual.ModifiedByFakeMultiReferenceEntityId);
            Assert.AreEqual(creationTime, actual.CreationTime);
            Assert.AreEqual(5487, actual.CreatedByFakeMultiReferenceEntityId);
            Assert.AreEqual(expected.Description, actual.Description);
            Assert.AreEqual(expected.UniqueName, actual.UniqueName);
        }

        /// <summary>
        /// The set unique key test.
        /// </summary>
        [TestMethod]
        public void SetUniqueKey_FakeComplexRow_MapsToProperty()
        {
            var entityMapper = new AutoMapperEntityMapper();
            entityMapper.Initialize(
                expression =>
                {
                    expression.AddProfile(new DataMappingProfile<FakeComplexRow>().SetUniqueKey(row => row.UniqueName));
                });

            var actual = entityMapper.MapTo("UniqueName", new FakeComplexRow());
            Assert.AreEqual("UniqueName", actual.UniqueName);
        }

        /// <summary>
        /// The set unique key test.
        /// </summary>
        [TestMethod]
        public void SetUniqueKey_FakeComplexRow_MapsOnlyTargetProperty()
        {
            var entityMapper = new AutoMapperEntityMapper();
            entityMapper.Initialize(
                expression =>
                {
                    expression.AddProfile(new DataMappingProfile<FakeComplexRow>().SetUniqueKey(row => row.UniqueName));
                });

            var creationTime = DateTimeOffset.Now;
            var expected = new FakeComplexRow
                               {
                                   FakeComplexEntityId = 648,
                                   FakeEnumerationId = 45,
                                   FakeOtherEnumerationId = 54,
                                   FakeSubEntityId = 587,
                                   Description = "old stuff",
                                   CreationTime = creationTime,
                                   CreatedByFakeMultiReferenceEntityId = 5487,
                                   ModifiedTime = creationTime,
                                   ModifiedByFakeMultiReferenceEntityId = 5487,
                                   UniqueName = "UniqueName"
            };

            var target = new FakeComplexRow
                             {
                                 FakeComplexEntityId = 648,
                                 FakeEnumerationId = 45,
                                 FakeOtherEnumerationId = 54,
                                 FakeSubEntityId = 587,
                                 Description = "old stuff",
                                 CreationTime = creationTime,
                                 CreatedByFakeMultiReferenceEntityId = 5487,
                                 ModifiedTime = creationTime,
                                 ModifiedByFakeMultiReferenceEntityId = 5487,
                                 UniqueName = "asdljaslkj"
                             };

            var actual = entityMapper.MapTo("UniqueName", target);
            Assert.AreEqual(expected, actual);
        }
    }
}