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

    using Rhino.Mocks;

    using Startitecture.Orm.Schema;
    using Startitecture.Orm.Testing.Entities;

    using MockRepository = Rhino.Mocks.MockRepository;

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
                new DataAnnotationsDefinitionProvider()))
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

            using (var connection = MockRepository.GenerateMock<IDbConnection>())
            using (var target = new Database(connection, new DataAnnotationsDefinitionProvider()))
            {
                using (var command = CreateMockCommand(connection))
                {
                    command.Stub(dbCommand => dbCommand.ExecuteNonQuery()).Return(Expected);
                    command.Stub(dbCommand => dbCommand.ExecuteScalar()).Return(Expected);

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
            var expected = 423;

            using (var connection = MockRepository.GenerateMock<IDbConnection>())
            using (var target = new Database(connection, new DataAnnotationsDefinitionProvider()))
            {
                using (var command = CreateMockCommand(connection))
                {
                    command.Stub(dbCommand => dbCommand.ExecuteNonQuery()).Return(expected);
                    command.Stub(dbCommand => dbCommand.ExecuteScalar()).Return(expected);

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
        private static IDbCommand CreateMockCommand(IDbConnection connection)
        {
            var command = MockRepository.GenerateMock<IDbCommand>();
            connection.Stub(dbConnection => dbConnection.CreateCommand()).Return(command);

            var dataParameterCollection = MockRepository.GenerateMock<IDataParameterCollection>();
            command.Stub(dbCommand => dbCommand.Parameters).Return(dataParameterCollection);

            command.Stub(dbCommand => dbCommand.CreateParameter())
                .WhenCalled(invocation => invocation.ReturnValue = MockRepository.GenerateMock<IDbDataParameter>())
                .Return(null);

            return command;
        }
    }
}