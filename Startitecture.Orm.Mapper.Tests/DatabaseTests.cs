// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseTests.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Mapper.Tests
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Startitecture.Orm.Schema;
    using Startitecture.Orm.SqlClient;
    using Startitecture.Orm.Testing.Entities;

    /// <summary>
    /// The database tests.
    /// </summary>
    [TestClass]
    public class DatabaseTests
    {
        /// <summary>
        /// The configuration root.
        /// </summary>
        private static IConfigurationRoot ConfigurationRoot => new ConfigurationBuilder().AddJsonFile("appSettings.json", false).Build();

        /// <summary>
        /// The begin transaction test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void BeginTransaction_SqlCommand_ExecutesNonQuery()
        {
            using (var target = new Database(
                ConfigurationRoot.GetConnectionString("MasterDatabase"),
                "System.Data.SqlClient",
                new DataAnnotationsDefinitionProvider(),
                new TransactSqlQualifier()))
            {
                var transaction = target.BeginTransaction() as SqlTransaction;
                Assert.IsNotNull(transaction);

                var sqlConnection = target.Connection as SqlConnection;
                Assert.IsNotNull(sqlConnection);

                using (var command = new SqlCommand("SELECT TOP 1 * FROM sys.tables", sqlConnection, transaction))
                {
                    command.ExecuteNonQuery();
                }

                target.AbortTransaction();
            }
        }

        /// <summary>
        /// The insert test.
        /// </summary>
        [TestMethod]
        public void Insert_NewComplexRow_AutoNumberPrimaryKeyIsSet()
        {
            const int Expected = 423;

            var mockConnection = new Mock<IDbConnection>();

            using (var connection = mockConnection.Object)
            using (var target = new Database(connection, new DataAnnotationsDefinitionProvider(), new TransactSqlQualifier()))
            {
                var mockCommand = CreateMockCommand(mockConnection);
                using (mockCommand.Object)
                {
                    mockCommand.Setup(dbCommand => dbCommand.ExecuteNonQuery()).Returns(Expected);
                    mockCommand.Setup(dbCommand => dbCommand.ExecuteScalar()).Returns(Expected);

                    var row = new ComplexRaisedRow
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

                    var result = target.Insert(row);

                    Assert.AreEqual(Expected, result);
                    Assert.AreEqual(Expected, row.ComplexEntityId);
                }
            }
        }

        /// <summary>
        /// The insert test.
        /// </summary>
        [TestMethod]
        public void Insert_NewDependentRow_PrimaryKeyIsSet()
        {
            var expected = 234;
            var mockConnection = new Mock<IDbConnection>();

            using (var connection = mockConnection.Object)
            using (var target = new Database(connection, new DataAnnotationsDefinitionProvider(), new TransactSqlQualifier()))
            {
                var mockCommand = CreateMockCommand(mockConnection);

                using (mockCommand.Object)
                {
                    mockCommand.Setup(dbCommand => dbCommand.ExecuteNonQuery()).Returns(expected);
                    mockCommand.Setup(dbCommand => dbCommand.ExecuteScalar()).Returns(expected);

                    var row = new DependentRow
                                  {
                                      FakeDependentEntityId = expected,
                                      DependentIntegerValue = 4583,
                                      DependentTimeValue = DateTimeOffset.Now
                                  };

                    var result = target.Insert(row);
                    Assert.AreEqual(expected, result);
                    Assert.AreEqual(expected, row.FakeDependentEntityId);
                }
            }
        }

        /// <summary>
        /// Creates a mock command.
        /// </summary>
        /// <param name="connection">
        /// The connection to create the command for.
        /// </param>
        /// <returns>
        /// The <see cref="IDbCommand"/>.
        /// </returns>
        private static Mock<IDbCommand> CreateMockCommand(Mock<IDbConnection> connection)
        {
            var command = new Mock<IDbCommand>();
            connection.Setup(dbConnection => dbConnection.CreateCommand()).Returns(command.Object);

            var dataParameterCollection = new Mock<IDataParameterCollection>();
            command.Setup(dbCommand => dbCommand.Parameters).Returns(dataParameterCollection.Object);

            command.Setup(dbCommand => dbCommand.CreateParameter())
                .Returns(new Mock<IDbDataParameter>().Object);

            return command;
        }
    }
}