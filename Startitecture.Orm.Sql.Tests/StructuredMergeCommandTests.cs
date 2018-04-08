// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructuredMergeCommandTests.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Sql.Tests
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reflection;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Mapper;
    using Startitecture.Orm.Testing.Model;
    using Startitecture.Orm.Testing.Model.DocumentEntities;
    using Startitecture.Orm.Testing.RhinoMocks;

    using UnifiedValueType = Startitecture.Orm.Testing.Model.PM.UnifiedValueType;

    /// <summary>
    /// The structured SQL command tests.
    /// </summary>
    [TestClass]
    public class StructuredMergeCommandTests
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
        public void Execute_StructuredMergeCommandDraftWithDetailMerge_DoesNotThrowException()
        {
            long submissionId = 0;

            var databaseFactory = new DefaultDatabaseFactory("OrmTestingContext");
            using (var provider = new DatabaseRepositoryProvider(databaseFactory, this.entityMapper))
            {
                provider.ChangeDatabase("DEVTEST01");

                try
                {
                    var transaction = provider.StartTransaction();
                    var expected = Generate.CreateFormSubmission(this.entityMapper, provider);

                    submissionId = expected.FormSubmissionId.GetValueOrDefault();

                    // Step #1, don't bother unless we have a database context provider.
                    var databaseContextProvider = provider as IDatabaseContextProvider;

                    Assert.IsNotNull(databaseContextProvider);
                    var structuredCommandProvider = new StructuredSqlCommandProvider(databaseContextProvider);

                    var valueTableLoader = Singleton<DataTableLoader<FormSubmissionValueRow>>.Instance;
                    var detailRowConverter = Singleton<FormSubmissionValueDetailRowConverter>.Instance;
                    var detailTableLoader = Singleton<DataTableLoader<FormSubmissionValueDetailRow>>.Instance;

                    expected.RefreshValues();

                    var valueDataTable = valueTableLoader.Load(expected.FormSubmissionValues, provider.EntityMapper);

                    // First save the unified field values and apply the output to source values. We don't delete unmatched as this 
                    // would also delete current values in ProcessWorkflowFieldValue. We are going to match our return selection on 
                    // UnifiedFieldId because that is the natural key of each submission value.
                    var mergeUnifiedValueCommand =
                        new StructuredMergeCommand<FormSubmissionValueRow>(structuredCommandProvider, transaction).MergeInto<UnifiedFieldValueRow>()
                            .SelectFromInserted(row => row.UnifiedFieldId);

                    using (var dataReader = mergeUnifiedValueCommand.ExecuteReader(valueDataTable))
                    {
                        var unifiedFieldIdOrdinal = dataReader.GetOrdinal("UnifiedFieldId");
                        var unifiedFieldValueIdOrdinal = dataReader.GetOrdinal("UnifiedFieldValueId");

                        while (dataReader.Read())
                        {
                            // We need this ID to associate the unsaved values with the saved ones.
                            var unifiedFieldId = dataReader.GetInt32(unifiedFieldIdOrdinal);

                            // We'll apply this ID to our existing values.
                            var unifiedFieldValueId = dataReader.GetInt64(unifiedFieldValueIdOrdinal);

                            // Find the entity and map to that. Mapping to the row will not work because the ID is ignored by default.
                            var formSubmissionValue = expected.FormSubmissionValues.First(x => x.UnifiedFieldId == unifiedFieldId);
                            provider.EntityMapper.MapTo(unifiedFieldValueId, formSubmissionValue);
                        }
                    }

                    // We need to refresh the data table now to merge in the FormSubmissionValues. 
                    valueTableLoader.Refresh(expected.FormSubmissionValues, valueDataTable, provider.EntityMapper);

                    // Now, we need to create another command in order to merge the form submission values. In this case we don't need
                    // to get the values back as we already have the IDs we need.
                    var mergeFormSubmissionValueCommand =
                        new StructuredMergeCommand<FormSubmissionValueRow>(structuredCommandProvider, transaction).MergeInto<FormSubmissionValueRow>();

                    mergeFormSubmissionValueCommand.Execute(valueDataTable);

                    // Having applied the IDs to each of the values, we can now save all the details.
                    var integerDetailRows =
                        detailRowConverter.Convert(expected.FormSubmissionValues.Where(x => x.UnifiedField.IsUserSourcedField == false)).ToList();

                    if (integerDetailRows.Any())
                    {
                        // Before we can insert the value instances, we must first delete them. TODO: Update UDTT to support merge here as well. 
                        var deleteIntegerQuery =
                            Select.From<UnifiedIntegerValueRow>()
                                .Matching(new FormSubmissionValueRow { FormSubmissionId = submissionId }, row => row.FormSubmissionId)
                                .InnerJoin<FormSubmissionValueRow>(row => row.UnifiedFieldValueId, row => row.FormSubmissionValueId);

                        provider.DeleteItems(deleteIntegerQuery);

                        ExecuteInsert<UnifiedIntegerValueRow, FormSubmissionValueDetailRow>(
                            detailTableLoader,
                            integerDetailRows,
                            structuredCommandProvider,
                            transaction);
                    }

                    // The other values are user-sourced and we'll add them by type.
                    var userSourcedValues = expected.FormSubmissionValues.Where(x => x.UnifiedField.IsUserSourcedField).ToList();

                    var numericDetailRows =
                        detailRowConverter.Convert(
                            userSourcedValues.Where(
                                x =>
                                    x.UnifiedValueType == UnifiedValueType.Decimal || x.UnifiedValueType == UnifiedValueType.Currency
                                    || x.UnifiedValueType == UnifiedValueType.Integer)).ToList();

                    if (numericDetailRows.Any())
                    {
                        var deleteNumericQuery =
                            Select.From<UnifiedNumericValueRow>()
                                .Matching(new FormSubmissionValueRow { FormSubmissionId = submissionId }, row => row.FormSubmissionId)
                                .InnerJoin<FormSubmissionValueRow>(row => row.UnifiedFieldValueId, row => row.FormSubmissionValueId);

                        provider.DeleteItems(deleteNumericQuery);

                        ExecuteInsert<UnifiedNumericValueRow, FormSubmissionValueDetailRow>(
                            detailTableLoader,
                            numericDetailRows,
                            structuredCommandProvider,
                            transaction);
                    }

                    var dateDetailRows =
                        detailRowConverter.Convert(userSourcedValues.Where(x => x.UnifiedValueType == UnifiedValueType.Date)).ToList();

                    if (dateDetailRows.Any())
                    {
                        var deleteDateQuery =
                            Select.From<UnifiedDateValueRow>()
                                .Matching(new FormSubmissionValueRow { FormSubmissionId = submissionId }, row => row.FormSubmissionId)
                                .InnerJoin<FormSubmissionValueRow>(row => row.UnifiedFieldValueId, row => row.FormSubmissionValueId);

                        provider.DeleteItems(deleteDateQuery);

                        ExecuteInsert<UnifiedDateValueRow, FormSubmissionValueDetailRow>(
                            detailTableLoader,
                            dateDetailRows,
                            structuredCommandProvider,
                            transaction);
                    }

                    var stringDetailRows =
                        detailRowConverter.Convert(userSourcedValues.Where(x => x.UnifiedValueType == UnifiedValueType.Text)).ToList();

                    if (stringDetailRows.Any())
                    {
                        var deleteStringQuery =
                            Select.From<UnifiedStringValueRow>()
                                .Matching(new FormSubmissionValueRow { FormSubmissionId = submissionId }, row => row.FormSubmissionId)
                                .InnerJoin<FormSubmissionValueRow>(row => row.UnifiedFieldValueId, row => row.FormSubmissionValueId);

                        provider.DeleteItems(deleteStringQuery);

                        ExecuteInsert<UnifiedStringValueRow, FormSubmissionValueDetailRow>(
                            detailTableLoader,
                            stringDetailRows,
                            structuredCommandProvider,
                            transaction);
                    }

                    provider.AbortTransaction();
                }
                finally
                {
                    Generate.CleanupData(provider, submissionId);
                }
            }
        }

        /// <summary>
        /// The execute reader_ structured sql command with mocked provider_ matches expected.
        /// </summary>
        [TestMethod]
        public void ExecuteReader_StructuredMergeCommandWithMockedProvider_MatchesExpected()
        {
            var actionPrincipal = new ActionPrincipal("someguy", 43587, 438) { FirstName = "some", LastName = "guy" };
            var formSubmission = Generate.CreateFormSubmission(actionPrincipal, true, false);

            var formSubmissionValueRows = this.entityMapper.Map<List<FormSubmissionValueRow>>(formSubmission.FormSubmissionValues);

            // Create the provider and stub out the list for the current values.
            var structuredCommandProvider = RepositoryMockFactory.CreateStructuredCommandProvider();
            structuredCommandProvider.StubForList(formSubmissionValueRows);

            var target = new StructuredMergeCommand<FormSubmissionValueRow>(structuredCommandProvider);

            var dataTableLoader = new DataTableLoader<FormSubmissionValueRow>();
            var dataTable = dataTableLoader.Load(formSubmissionValueRows);

            var properties = from p in typeof(FormSubmissionValueRow).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                             where p.CanWrite
                             orderby p.Name //// Skip stuff like TransactionProvider
                             select p;

            var propertyOrdinals = properties.Select((info, i) => new { Ordinal = i, Property = info }).ToList();

            using (var reader = target.ExecuteReader(dataTable))
            using (var enumerator = formSubmissionValueRows.GetEnumerator())
            {
                // The mocked reader will do this once per value.
                while (reader.Read())
                {
                    // Walk through the rows, assuming they are all in the same order.
                    enumerator.MoveNext();
                    var items = new object[reader.FieldCount];
                    var count = reader.GetValues(items);

                    Assert.AreEqual<int>(items.Length, count);

                    // Test each property.
                    foreach (var propertyOrdinal in propertyOrdinals)
                    {
                        var expected = items[propertyOrdinal.Ordinal];
                        var actual = propertyOrdinal.Property.GetMethod.Invoke(enumerator.Current, null);
                        Assert.AreEqual(expected, actual);
                    }
                }
            }
        }

        /// <summary>
        /// Executes an insert.
        /// </summary>
        /// <param name="detailTableLoader">
        /// The detail table loader.
        /// </param>
        /// <param name="integerDetailRows">
        /// The integer detail rows.
        /// </param>
        /// <param name="structuredCommandProvider">
        /// The database context provider.
        /// </param>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        /// <typeparam name="TDataItem">
        /// The type of data item that will be inserted into.
        /// </typeparam>
        /// <typeparam name="TStructure">
        /// The type of data structure that contains the items to insert.
        /// </typeparam>
        private static void ExecuteInsert<TDataItem, TStructure>(
            DataTableLoader<TStructure> detailTableLoader,
            IEnumerable<TStructure> integerDetailRows,
            IStructuredCommandProvider structuredCommandProvider,
            IDbTransaction transaction)
        {
            var detailDataTable = detailTableLoader.Load(integerDetailRows);
            var detailInsertCommand = new StructuredInsertCommand<TStructure>(structuredCommandProvider, transaction).InsertInto<TDataItem>();
            detailInsertCommand.Execute(detailDataTable);
        }
    }
}