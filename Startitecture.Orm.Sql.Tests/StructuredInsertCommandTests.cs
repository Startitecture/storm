// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructuredInsertCommandTests.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The structured insert command tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Sql.Tests
{
    using System;
    using System.Data;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Mapper;
    using Startitecture.Orm.Repository;
    using Startitecture.Orm.Testing.Entities;
    using Startitecture.Orm.Testing.Model;

    /// <summary>
    /// The structured insert command tests
    /// </summary>
    [TestClass]
    public class StructuredInsertCommandTests
    {
        /// <summary>
        /// The entity mapper.
        /// </summary>
        private readonly IEntityMapper entityMapper = new AutoMapperEntityMapper().Initialize(expression => { });

        /// <summary>
        /// The execute test.
        /// </summary>
        [TestMethod]
        public void Execute_StructuredInsertCommand_DoesNotThrowException()
        {
            long submissionId = 0;

            var databaseFactory = new DefaultDatabaseFactory("OrmTestingContext");
            using (var provider = new DatabaseRepositoryProvider(databaseFactory, this.entityMapper))
            {
                try
                {
                    var transaction = provider.StartTransaction();
                    var expected = new GenericSubmission("My Submission");

                    submissionId = expected.GenericSubmissionId.GetValueOrDefault();

                    var valueDataTable = Singleton<DataTableLoader<GenericSubmissionRow>>.Instance.Load(
                        expected.SubmissionValues,
                        this.entityMapper);

                    ////var commandText = CreateCommandText(parameterName, false);
                    var databaseContextProvider = provider as IDatabaseContextProvider;
                    Assert.IsNotNull(databaseContextProvider);
                    var structuredCommandProvider = new StructuredSqlCommandProvider(databaseContextProvider);

                    var target =
                        new StructuredInsertCommand<GenericSubmissionRow>(structuredCommandProvider, transaction).InsertInto<FieldValueRow>();

                    target.Execute(valueDataTable);
                    provider.AbortTransaction();
                }
                finally
                {
                    ////Generate.CleanupData(provider, submissionId);
                }
            }
        }

        /// <summary>
        /// The execute test.
        /// </summary>
        [TestMethod]
        public void ExecuteReader_StructuredInsertCommandWithReturn_MatchesExpected()
        {
            long submissionId = 0;

            var databaseFactory = new DefaultDatabaseFactory("OrmTestingContext");
            using (var provider = new DatabaseRepositoryProvider(databaseFactory, this.entityMapper))
            {
                try
                {
                    var transaction = provider.StartTransaction();
                    var expected = new GenericSubmission("My Submission");

                    submissionId = expected.GenericSubmissionId.GetValueOrDefault();

                    var valueDataTable = Singleton<DataTableLoader<GenericSubmissionRow>>.Instance.Load(
                        expected.SubmissionValues,
                        this.entityMapper);

                    var databaseContextProvider = (IDatabaseContextProvider)provider;
                    Assert.IsNotNull(databaseContextProvider);
                    var structuredCommandProvider = new StructuredSqlCommandProvider(databaseContextProvider);

                    var target =
                        new StructuredInsertCommand<GenericSubmissionRow>(structuredCommandProvider, transaction).InsertInto<FieldValueRow>()
                            .SelectResults(row => row.GenericSubmissionId);

                    using (var reader = target.ExecuteReader(valueDataTable))
                    {
                        while (reader.Read())
                        {
                            var comparisonRow = new GenericSubmissionValueRow
                                                    {
                                                        GenericSubmissionValueId = reader.GetInt64(0),
                                                        GenericSubmissionId = reader.GetInt32(1),
                                                        GenericSubmission = GetGenericSubmissionFromRecord(reader, provider),
                                                        Field = GetFieldFromRecord(reader, provider)
                                                    };

                            Assert.AreNotEqual(0, comparisonRow.Field.FieldId);
                            ////Assert.IsTrue(
                            ////    expected.FormSubmissionValues.Any(
                            ////        value =>
                            ////            value.UnifiedFieldId == comparisonRow.UnifiedFieldId
                            ////            && value.LastModifiedTime == comparisonRow.LastModifiedTime
                            ////            && value.LastModifiedPersonId == comparisonRow.LastModifiedPersonId));
                        }
                    }

                    provider.AbortTransaction();
                }
                finally
                {
                    ////Generate.CleanupData(provider, submissionId);
                }
            }
        }

        /// <summary>
        /// The get field from record.
        /// </summary>
        /// <param name="record">
        /// The record.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <returns>
        /// The <see cref="FieldRow"/>.
        /// </returns>
        private static FieldRow GetFieldFromRecord(IDataRecord record, IRepositoryProvider provider)
        {
            var dependencyContainer = provider.DependencyContainer;
            var existingRow = dependencyContainer.GetDependency<FieldRow>(record.GetInt32(3));
            var row = existingRow ?? new FieldRow
                                         {
                                             FieldId = record.GetInt32(3),
                                             Name = record.GetString(6),
                                             Description = record.GetString(7)
                                         };

            row.SetTransactionProvider(provider);
            return row;
        }

        /// <summary>
        /// Gets a generic submission from a record.
        /// </summary>
        /// <param name="record">
        /// The record.
        /// </param>
        /// <param name="provider">
        /// The repository provider.
        /// </param>
        /// <returns>
        /// The <see cref="GenericSubmissionRow"/>.
        /// </returns>
        private static GenericSubmissionRow GetGenericSubmissionFromRecord(IDataRecord record, IRepositoryProvider provider)
        {
            var dependencyContainer = provider.DependencyContainer;
            var existingRow = dependencyContainer.GetDependency<GenericSubmissionRow>(record.GetInt32(2));
            var row = existingRow ?? new GenericSubmissionRow
                                         {
                                             GenericSubmissionId = record.GetInt32(2),
                                             Subject = record.GetString(4),
                                             SubmittedByDomainIdentiferId = record.GetInt32(5),
                                             SubmittedTime = (DateTimeOffset)record.GetValue(6),
                                         };

            row.SetTransactionProvider(provider);
            return row;
        }
    }
}
