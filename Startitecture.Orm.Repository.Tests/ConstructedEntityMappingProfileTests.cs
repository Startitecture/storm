// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConstructedEntityMappingProfileTests.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Orm.Repository.Tests.Models;

    /// <summary>
    /// The constructed entity mapping profile tests.
    /// </summary>
    [TestClass]
    public class ConstructedEntityMappingProfileTests
    {
        /// <summary>
        /// The set primary key_ constructed entity mapping profile_ key is mapped.
        /// </summary>
        [TestMethod]
        public void SetPrimaryKey_ConstructedEntityMappingProfile_KeyIsMappedForRow()
        {
            var mapper = new AutoMapperEntityMapper();
            mapper.Initialize(configuration => configuration.AddProfile<KeyMappingConstructedEntityProfile>());

            var fakeSubSubEntity = new FakeSubSubEntity("UniquestName", 994) { Description = "Mah Sub Sub Entity" };
            var fakeSubEntity = new FakeSubEntity("SubUniquestName", 94, fakeSubSubEntity, 943) { Description = "Mah Sub Entity" };
            var expectedRow = new FakeFlatSubRow
                               {
                                   Description = "Mah Sub Entity",
                                   FakeSubEntityId = 943,
                                   FakeSubSubEntityDescription = "Mah Sub Sub Entity",
                                   FakeSubSubEntityId = 994,
                                   FakeSubSubEntityUniqueName = "UniquestName",
                                   UniqueName = "SubUniquestName",
                                   UniqueOtherId = 94
                               };

            var actual = mapper.Map<FakeFlatSubRow>(fakeSubEntity);
            Assert.AreEqual(expectedRow, actual);
            Assert.AreEqual<int>(943, actual.FakeSubEntityId);
        }

        /// <summary>
        /// The set primary key_ constructed entity mapping profile_ key is mapped.
        /// </summary>
        [TestMethod]
        public void SetPrimaryKey_ConstructedEntityMappingProfile_KeyIsMappedForEntity()
        {
            var mapper = new AutoMapperEntityMapper();
            mapper.Initialize(configuration => configuration.AddProfile<KeyMappingConstructedEntityProfile>());

            var fakeSubSubEntity = new FakeSubSubEntity("UniquestName", 994) { Description = "Mah Sub Sub Entity" };
            var expected = new FakeSubEntity("SubUniquestName", 94, fakeSubSubEntity) { Description = "Mah Sub Entity" };

            var actual = mapper.MapTo(943, expected);
            Assert.AreEqual<int?>(943, actual.FakeSubEntityId);
        }

        /// <summary>
        /// The key mapping constructed entity profile.
        /// </summary>
        private class KeyMappingConstructedEntityProfile : ConstructedEntityMappingProfile<FakeSubEntity, FakeFlatSubRow>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="KeyMappingConstructedEntityProfile"/> class.
            /// </summary>
            public KeyMappingConstructedEntityProfile()
            {
                this.SetPrimaryKey(entity => entity.FakeSubEntityId, row => row.FakeSubEntityId);
            }
        }
    }
}