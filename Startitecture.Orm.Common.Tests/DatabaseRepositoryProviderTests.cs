// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseRepositoryProviderTests.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common.Tests
{
    using System.Data;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Startitecture.Orm.Model;

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
            var mockDatabaseFactory = new Mock<IDatabaseFactory>();
            var mockDatabaseContext = new Mock<IDatabaseContext>();
            mockDatabaseFactory.Setup(factory => factory.Create()).Returns(mockDatabaseContext.Object);

            var mockConnection = new Mock<IDbConnection>();
            mockDatabaseContext.Setup(context => context.Connection).Returns(mockConnection.Object);

            mockConnection.Setup(connection => connection.ChangeDatabase(It.IsAny<string>()))
                .Callback((string s) => { mockConnection.Setup(connection => connection.Database).Returns(s); });

            var databaseFactory = mockDatabaseFactory.Object;
            var queryFactory = new Mock<IStatementFactory>().Object;

            using (var target = new DatabaseRepositoryProvider(databaseFactory, queryFactory))
            {
                target.DatabaseContext.Connection.ChangeDatabase("newDatabase");
                Assert.AreEqual("newDatabase", target.DatabaseContext.Connection.Database);
            }
        }

        /// <summary>
        /// The start transaction test.
        /// </summary>
        [TestMethod]
        public void StartTransaction_TransactionWithIsolationLevel_ReturnsTransactionWithExpectedIsolationLevel()
        {
            var mockDatabaseFactory = new Mock<IDatabaseFactory>();
            var mockDatabaseContext = new Mock<IDatabaseContext>();
            mockDatabaseFactory.Setup(factory => factory.Create()).Returns(mockDatabaseContext.Object);

            var mockConnection = new Mock<IDbConnection>();
            mockDatabaseContext.Setup(context => context.Connection).Returns(mockConnection.Object);

            mockConnection.Setup(connection => connection.BeginTransaction(It.IsAny<IsolationLevel>()))
                .Returns(
                    (IsolationLevel i) =>
                        {
                            var transaction = new Mock<IDbTransaction>();
                            transaction.Setup(dbTransaction => dbTransaction.IsolationLevel).Returns(i);
                            return transaction.Object;
                        });

            var databaseFactory = mockDatabaseFactory.Object;
            var queryFactory = new Mock<IStatementFactory>().Object;

            using (var target = new DatabaseRepositoryProvider(databaseFactory, queryFactory))
            {
                var actual = target.StartTransaction(IsolationLevel.Serializable);
                Assert.AreEqual(IsolationLevel.Serializable, actual.IsolationLevel);
            }
        }
    }
}