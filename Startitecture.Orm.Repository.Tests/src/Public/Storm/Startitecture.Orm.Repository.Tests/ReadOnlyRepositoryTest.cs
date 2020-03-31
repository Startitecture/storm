// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadOnlyRepositoryTest.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The read only repository test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Orm.Common;
    using Startitecture.Orm.Testing.Entities;
    using Startitecture.Orm.Testing.Model;
    using Startitecture.Orm.Testing.RhinoMocks;

    /// <summary>
    /// The read only repository test.
    /// </summary>
    public partial class ReadOnlyRepositoryTest
    {
        /// <summary>
        /// Tests that the first or default of the repository matches the expected results.
        /// </summary>
        [TestMethod]
        public void Save_NewFakeRaisedSubEntity_MatchesExpected()
        {
            var fakeSubSubEntity = new SubSubEntity("jasdyri");
            var expected = new SubEntity("asidf", 58, fakeSubSubEntity);

            var repositoryAdapter = RepositoryMockFactory.CreateAdapter();
            using (var repositoryProvider = RepositoryMockFactory.CreateConcreteProvider<FakeDataContext>(this.entityMapper, repositoryAdapter))
            {
                repositoryAdapter.StubForNewItem<SubRow>(repositoryProvider.EntityDefinitionProvider);
                repositoryAdapter.StubForNewItem<SubSubRow>(repositoryProvider.EntityDefinitionProvider);

                var target = new EntityRepository<SubEntity, SubRow>(repositoryProvider, this.entityMapper);
                var actual = target.Save(expected);

                Assert.IsTrue(actual.FakeSubEntityId > 0);
                Assert.IsTrue(actual.FakeSubSubEntityId > 0);
                Assert.AreSame(expected, actual);
            }
        }

        /// <summary>
        /// Tests that the first or default of the repository matches the expected results.
        /// </summary>
        [TestMethod]
        public void Save_ExistingFakeRaisedSubEntity_MatchesExpected()
        {
            var fakeSubSubEntity = new SubSubEntity("jasdyri", 93475);
            var expected = new SubEntity("asidf", 58, fakeSubSubEntity, 874359);

            var repositoryAdapter = RepositoryMockFactory.CreateAdapter();
            repositoryAdapter.StubForExistingItem<SubRow>(expected, this.entityMapper);
            repositoryAdapter.StubForExistingItem<SubSubRow>(fakeSubSubEntity, this.entityMapper);

            using (var repositoryProvider = RepositoryMockFactory.CreateConcreteProvider<FakeDataContext>(this.entityMapper, repositoryAdapter))
            {
                var target = new EntityRepository<SubEntity, SubRow>(repositoryProvider, this.entityMapper);
                var actual = target.Save(expected);

                Assert.AreEqual(874359, actual.FakeSubEntityId);
                Assert.AreEqual(93475, actual.FakeSubSubEntityId);
                Assert.AreSame(expected, actual);
            }
        }
    }
}