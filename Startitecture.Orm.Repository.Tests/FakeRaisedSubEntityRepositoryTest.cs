// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeRaisedSubEntityRepositoryTest.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository.Tests
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Orm.Common;
    using Startitecture.Orm.Repository.Tests.Models;
    using Startitecture.Orm.Testing.RhinoMocks;

    /// <summary>
    /// The fake raised sub entity repository test.
    /// </summary>
    [TestClass]
    public class FakeRaisedSubEntityRepositoryTest
    {
        /// <summary>
        /// The entity mapper.
        /// </summary>
        private readonly IEntityMapper entityMapper = RepositoryMockFactory.CreateEntityMapper(
            expression =>
                {
                    expression.AddProfile<FakeRaisedSubEntityMappingProfile>();
                    expression.AddProfile<FakeSubSubEntityMappingProfile>();
                });

        /// <summary>
        /// Tests that the first or default of the repository matches the expected results.
        /// </summary>
        [TestMethod]
        public void FirstOrDefault_FakeRaisedSubEntity_MatchesExpected()
        {
            var fakeSubSubEntity = new FakeSubSubEntity("jasdyri", 5848);
            var expected = new FakeSubEntity("asidf", 58, fakeSubSubEntity, 87543);

            var repositoryAdapter = RepositoryMockFactory.CreateAdapter();
            repositoryAdapter.StubForExistingItem<FakeRaisedSubRow>(expected, this.entityMapper);

            using (var repositoryProvider = RepositoryMockFactory.CreateConcreteProvider<FakeDataContext>(this.entityMapper, repositoryAdapter))
            {
                var target = new FakeRaisedSubEntityRepository(repositoryProvider);
                var actual = target.FirstOrDefault(expected.FakeSubEntityId.GetValueOrDefault());

                Assert.AreEqual(expected.FakeSubEntityId, actual.FakeSubEntityId);
                Assert.AreEqual(expected.FakeSubSubEntityId, actual.FakeSubSubEntityId);
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        /// Tests that the first or default of the repository matches the expected results.
        /// </summary>
        [TestMethod]
        public void Save_NewFakeRaisedSubEntity_MatchesExpected()
        {
            var fakeSubSubEntity = new FakeSubSubEntity("jasdyri");
            var expected = new FakeSubEntity("asidf", 58, fakeSubSubEntity);

            var repositoryAdapter = RepositoryMockFactory.CreateAdapter();
            repositoryAdapter.StubForNewItem<FakeRaisedSubRow>();
            repositoryAdapter.StubForNewItem<FakeSubSubRow>();

            using (var repositoryProvider = RepositoryMockFactory.CreateConcreteProvider<FakeDataContext>(this.entityMapper, repositoryAdapter))
            {
                var target = new FakeRaisedSubEntityRepository(repositoryProvider);
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
            var fakeSubSubEntity = new FakeSubSubEntity("jasdyri", 93475);
            var expected = new FakeSubEntity("asidf", 58, fakeSubSubEntity, 874359);

            var repositoryAdapter = RepositoryMockFactory.CreateAdapter();
            repositoryAdapter.StubForExistingItem<FakeRaisedSubRow>(expected, this.entityMapper);
            repositoryAdapter.StubForExistingItem<FakeSubSubRow>(fakeSubSubEntity, this.entityMapper);

            using (var repositoryProvider = RepositoryMockFactory.CreateConcreteProvider<FakeDataContext>(this.entityMapper, repositoryAdapter))
            {
                var target = new FakeRaisedSubEntityRepository(repositoryProvider);
                var actual = target.Save(expected);

                Assert.AreEqual(874359, actual.FakeSubEntityId);
                Assert.AreEqual(93475, actual.FakeSubSubEntityId);
                Assert.AreSame(expected, actual);
            }
        }

        /// <summary>
        /// Tests that the first or default of the repository matches the expected results.
        /// </summary>
        [TestMethod]
        public void SelectEntities_FakeRaisedSubEntity_MatchesExpected()
        {
            var fakeSubSubEntity = new FakeSubSubEntity("jasdyri", 5848);
            var expected = new List<FakeSubEntity>
                               {
                                   new FakeSubEntity("asidf", 58, fakeSubSubEntity, 87543),
                                   new FakeSubEntity("safd", 59, fakeSubSubEntity, 546),
                                   new FakeSubEntity("gjkdf", 52, fakeSubSubEntity, 3465)
                               };

            var repositoryAdapter = RepositoryMockFactory.CreateAdapter();
            repositoryAdapter.StubForExistingItem<FakeRaisedSubRow>(expected, this.entityMapper);
            repositoryAdapter.StubForList(this.entityMapper.Map<List<FakeRaisedSubRow>>(expected));

            using (var repositoryProvider = RepositoryMockFactory.CreateConcreteProvider<FakeDataContext>(this.entityMapper, repositoryAdapter))
            {
                var target = new FakeRaisedSubEntityRepository(repositoryProvider);
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
    }
}