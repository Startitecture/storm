// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseRepositoryProviderTests.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common.Tests
{
    using System;
    using System.Data;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Startitecture.Orm.Schema;
    using Startitecture.Orm.Testing.Entities;

    /// <summary>
    /// The database repository provider tests.
    /// </summary>
    [TestClass]
    public class DatabaseRepositoryProviderTests
    {
        /// <summary>
        /// The change database test.
        /// </summary>
        [TestMethod]
        public void ChangeDatabase_DatabaseRepositoryProvider_DatabaseChanged()
        {
            var mockDatabaseFactory = new Mock<IDatabaseContextFactory>();
            var mockDatabaseContext = new Mock<IDatabaseContext>();
            mockDatabaseFactory.Setup(factory => factory.Create()).Returns(mockDatabaseContext.Object);

            var mockConnection = new Mock<IDbConnection>();
            mockDatabaseContext.Setup(context => context.Connection).Returns(mockConnection.Object);
            var repositoryAdapter = new Mock<IRepositoryAdapter>();
            repositoryAdapter.Setup(adapter => adapter.DefinitionProvider).Returns(new DataAnnotationsDefinitionProvider());
            mockDatabaseContext.Setup(context => context.RepositoryAdapter).Returns(repositoryAdapter.Object);
            mockDatabaseContext.Setup(context => context.ChangeDatabase(It.IsAny<string>()))
                .Callback((string s) => { mockConnection.Setup(connection => connection.Database).Returns(s); });

            var databaseFactory = mockDatabaseFactory.Object;

            using (var target = new DatabaseRepositoryProvider(databaseFactory))
            {
                target.DatabaseContext.ChangeDatabase("newDatabase");
                Assert.AreEqual("newDatabase", target.DatabaseContext.Connection.Database);
            }
        }

        /// <summary>
        /// The start transaction test.
        /// </summary>
        [TestMethod]
        public void StartTransaction_TransactionWithIsolationLevel_ReturnsTransactionWithExpectedIsolationLevel()
        {
            var mockDatabaseFactory = new Mock<IDatabaseContextFactory>();
            var mockDatabaseContext = new Mock<IDatabaseContext>();
            mockDatabaseFactory.Setup(factory => factory.Create()).Returns(mockDatabaseContext.Object);

            var mockConnection = new Mock<IDbConnection>();
            mockDatabaseContext.Setup(context => context.Connection).Returns(mockConnection.Object);
            var repositoryAdapter = new Mock<IRepositoryAdapter>();
            repositoryAdapter.Setup(adapter => adapter.DefinitionProvider).Returns(new DataAnnotationsDefinitionProvider());
            mockDatabaseContext.Setup(context => context.RepositoryAdapter).Returns(repositoryAdapter.Object);
            mockDatabaseContext.Setup(context => context.BeginTransaction(It.IsAny<IsolationLevel>()))
                .Returns(
                    (IsolationLevel i) =>
                    {
                        var transaction = new Mock<ITransactionContext>();
                        transaction.Setup(dbTransaction => dbTransaction.IsolationLevel).Returns(i);
                        return transaction.Object;
                    });

            var databaseFactory = mockDatabaseFactory.Object;

            using (var target = new DatabaseRepositoryProvider(databaseFactory))
            {
                var actual = target.BeginTransaction(IsolationLevel.Serializable);
                Assert.AreEqual(IsolationLevel.Serializable, actual.IsolationLevel);
            }
        }

        /// <summary>
        /// The insert test.
        /// </summary>
        [TestMethod]
        public void Insert_IdentityPrimaryKey_PrimaryKeyIsSet()
        {
            var mockDatabaseFactory = new Mock<IDatabaseContextFactory>();
            var mockDatabaseContext = new Mock<IDatabaseContext>();
            mockDatabaseFactory.Setup(factory => factory.Create()).Returns(mockDatabaseContext.Object);

            var repositoryAdapter = new Mock<IRepositoryAdapter>();
            mockDatabaseContext.Setup(context => context.RepositoryAdapter).Returns(repositoryAdapter.Object);

            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryAdapter.Setup(compiler => compiler.CreateInsertionStatement<DependentRow>()).Returns(string.Empty);
            repositoryAdapter.Setup(adapter => adapter.DefinitionProvider).Returns(definitionProvider);
            mockDatabaseContext.Setup(context => context.ExecuteScalar<object>(It.IsAny<string>(), It.IsAny<object[]>())).Returns(234);

            var databaseFactory = mockDatabaseFactory.Object;

            using (var target = new DatabaseRepositoryProvider(databaseFactory))
            {
                var expected = new ComplexRaisedRow
                              {
                                  CreatedByFakeMultiReferenceEntityId = 87354,
                                  ModifiedByFakeMultiReferenceEntityId = 34598,
                                  FakeEnumerationId = 8,
                                  FakeOtherEnumerationId = 4,
                                  UniqueName = "MyUniqueName",
                                  Description = "Some Stuff!",
                                  FakeSubEntityId = 4598,
                                  CreationTime = DateTimeOffset.Now,
                                  ModifiedTime = DateTimeOffset.Now
                              };

                var actual = target.Insert(expected);

                Assert.AreSame(expected, actual);
                Assert.AreEqual(234, expected.ComplexEntityId);
            }
        }

        /// <summary>
        /// The insert test.
        /// </summary>
        [TestMethod]
        public void Insert_NonIdentityKey_MatchesExpected()
        {
            var mockDatabaseFactory = new Mock<IDatabaseContextFactory>();
            var mockDatabaseContext = new Mock<IDatabaseContext>();
            mockDatabaseFactory.Setup(factory => factory.Create()).Returns(mockDatabaseContext.Object);

            var repositoryAdapter = new Mock<IRepositoryAdapter>();
            mockDatabaseContext.Setup(context => context.RepositoryAdapter).Returns(repositoryAdapter.Object);

            var definitionProvider = new DataAnnotationsDefinitionProvider();
            repositoryAdapter.Setup(compiler => compiler.CreateInsertionStatement<DependentRow>()).Returns(string.Empty);
            repositoryAdapter.Setup(adapter => adapter.DefinitionProvider).Returns(definitionProvider);

            var databaseFactory = mockDatabaseFactory.Object;

            using (var target = new DatabaseRepositoryProvider(databaseFactory))
            {
                var expected = new DependentRow
                              {
                                  FakeDependentEntityId = 234,
                                  DependentIntegerValue = 4583,
                                  DependentTimeValue = DateTimeOffset.Now
                              };

                var actual = target.Insert(expected);
                Assert.AreSame(expected, actual);
            }
        }
    }
}