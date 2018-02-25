namespace Startitecture.Orm.Sql.Tests
{
    using System;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Mapper;
    using Startitecture.Orm.Testing.Model.DocumentEntities;
    using Startitecture.Orm.Testing.RhinoMocks;

    using FormSubmissionValueRow = Startitecture.Orm.Testing.Model.DocumentEntities.FormSubmissionValueRow;
    using UnifiedFieldValueRow = Startitecture.Orm.Testing.Model.FieldEntities.UnifiedFieldValueRow;

    [TestClass]
    public class StructuredInsertCommandTests
    {
        /// <summary>
        /// The entity mapper.
        /// </summary>
        private readonly IEntityMapper entityMapper = RepositoryMockFactory.CreateEntityMapper(
            expression =>
            {
                expression.AddProfile<FormSubmissionMappingProfile>();
                expression.AddProfile<FormSubmissionValueMappingProfile>();
                expression.AddProfile<PersonMappingProfile>();
                expression.AddProfile<UnifiedFieldMappingProfile>();
            });

        /// <summary>
        /// The execute test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void Execute_StructuredInsertCommand_DoesNotThrowException()
        {
            long submissionId = 0;

            using (var provider = new DatabaseRepositoryProvider<TestDb>(this.entityMapper))
            {
                provider.ChangeDatabase("DEVTEST01");

                try
                {
                    var transaction = provider.StartTransaction();
                    var expected = Generate.CreateFormSubmission(this.entityMapper, provider);

                    submissionId = expected.FormSubmissionId.GetValueOrDefault();

                    var valueDataTable = Singleton<DataTableLoader<FormSubmissionValueRow>>.Instance.Load(
                        expected.FormSubmissionValues,
                        this.entityMapper);

                    ////var commandText = CreateCommandText(parameterName, false);
                    var databaseContextProvider = provider as IDatabaseContextProvider;
                    Assert.IsNotNull(databaseContextProvider);
                    var structuredCommandProvider = new StructuredSqlCommandProvider(databaseContextProvider);

                    var target =
                        new StructuredInsertCommand<FormSubmissionValueRow>(structuredCommandProvider, transaction).InsertInto<UnifiedFieldValueRow>();

                    target.Execute(valueDataTable);
                    provider.AbortTransaction();
                }
                finally
                {
                    Generate.CleanupData(provider, submissionId);
                }
            }
        }

        /// <summary>
        /// The execute test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void ExecuteReader_StructuredInsertCommandWithReturn_MatchesExpected()
        {
            long submissionId = 0;

            using (var provider = new DatabaseRepositoryProvider<TestDb>(this.entityMapper))
            {
                provider.ChangeDatabase("DEVTEST01");

                try
                {
                    var transaction = provider.StartTransaction();
                    var expected = Generate.CreateFormSubmission(this.entityMapper, provider);

                    submissionId = expected.FormSubmissionId.GetValueOrDefault();

                    var valueDataTable = Singleton<DataTableLoader<FormSubmissionValueRow>>.Instance.Load(
                        expected.FormSubmissionValues,
                        this.entityMapper);

                    var databaseContextProvider = provider as IDatabaseContextProvider;
                    Assert.IsNotNull(databaseContextProvider);
                    var structuredCommandProvider = new StructuredSqlCommandProvider(databaseContextProvider);

                    var target =
                        new StructuredInsertCommand<FormSubmissionValueRow>(structuredCommandProvider, transaction).InsertInto<UnifiedFieldValueRow>()
                            .SelectResults(row => row.UnifiedFieldId);

                    using (var reader = target.ExecuteReader(valueDataTable))
                    {
                        while (reader.Read())
                        {
                            var comparisonRow = new FormSubmissionValueRow
                            {
                                FormSubmissionId = reader.GetInt64(0),
                                FormSubmissionValueId = reader.GetInt64(1),
                                LastModifiedPersonId = reader.GetInt32(2),
                                LastModifiedTime = (DateTimeOffset)reader.GetValue(3),
                                UnifiedFieldCaption = reader.IsDBNull(4) ? null : reader.GetString(4),
                                UnifiedFieldId = reader.GetInt32(5),
                                UnifiedFieldLabel = reader.IsDBNull(6) ? null : reader.GetString(6),
                                UnifiedFieldName = reader.GetString(7),
                                UnifiedFieldTypeId = reader.GetInt32(8),
                                UnifiedFieldValueId = reader.GetInt64(9),
                                UnifiedValueTypeId = reader.GetInt32(10)
                            };

                            Assert.AreNotEqual<long>(0, comparisonRow.UnifiedFieldValueId);
                            Assert.IsTrue(
                                expected.FormSubmissionValues.Any(
                                    value =>
                                        value.UnifiedFieldId == comparisonRow.UnifiedFieldId
                                        && value.LastModifiedTime == comparisonRow.LastModifiedTime
                                        && value.LastModifiedPersonId == comparisonRow.LastModifiedPersonId));
                        }
                    }

                    provider.AbortTransaction();
                }
                finally
                {
                    Generate.CleanupData(provider, submissionId);
                }
            }
        }
    }
}
