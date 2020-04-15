// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructuredMergeCommandTests.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Startitecture.Orm.Sql.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Repository;
    using Startitecture.Orm.Schema;
    using Startitecture.Orm.Testing.Entities;
    using Startitecture.Orm.Testing.Entities.TableTypes;
    using Startitecture.Orm.Testing.Model;
    using Startitecture.Orm.Testing.Moq;

    /// <summary>
    /// The structured merge command tests.
    /// </summary>
    [TestClass]
    public class StructuredMergeCommandTests
    {
        /// <summary>
        /// The entity mapper.
        /// </summary>
        private readonly IEntityMapper entityMapper =
            new AutoMapperEntityMapper().Initialize(expression => { expression.AddProfile<GenericSubmissionMappingProfile>(); });

        /// <summary>
        /// The configuration root.
        /// </summary>
        private static IConfigurationRoot ConfigurationRoot => new ConfigurationBuilder().AddJsonFile("appSettings.json", false).Build();

        /// <summary>
        /// The merge into test.d
        /// </summary>
        [TestMethod]
        public void MergeInto_FieldsTable_ReturnedItemsMatchExpected()
        {
            var internalId = new Field(421)
                                 {
                                     Name = "MERGE_Internal ID",
                                     Description = "Unique ID used internally"
                                 };

            var firstName = new Field(66)
                                {
                                    Name = "MERGE_First Name",
                                    Description = "The person's first name"
                                };

            var lastName = new Field(7887)
                               {
                                   Name = "MERGE_Last Name",
                                   Description = "The person's last name"
                               };

            var yearlyWage = new Field(82328)
                                 {
                                     Name = "MERGE_Yearly Wage",
                                     Description = "The base wage paid year over year."
                                 };

            var hireDate = new Field
                               {
                                   Name = "MERGE_Hire Date",
                                   Description = "The date and time of hire for the person"
                               };

            var bonusTarget = new Field
                                  {
                                      Name = "MERGE_Bonus Target",
                                      Description = "The target bonus for the person"
                                  };

            var contactNumbers = new Field
                                     {
                                         Name = "MERGE_Contact Numbers",
                                         Description = "A list of contact numbers for the person in order of preference"
                                     };

            var fields = new List<Field>
                             {
                                 internalId,
                                 firstName,
                                 lastName,
                                 yearlyWage,
                                 hireDate,
                                 bonusTarget,
                                 contactNumbers
                             };

            var mergeItems = (from f in fields
                             select new FieldTableTypeRow
                                        {
                                            FieldId = f.FieldId,
                                            Name = f.Name,
                                            Description = f.Description
                                        }).ToList();

            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var commandProvider = mergeItems.MockCommandProvider(definitionProvider);
            var target = new StructuredMergeCommand<FieldTableTypeRow>(commandProvider.Object);
            var typeRows = target.MergeInto<FieldRow>(mergeItems, row => row.Name).SelectFromInserted(row => row.Name).ExecuteForResults();

            Assert.IsNotNull(typeRows);
            var actual = typeRows.Select(
                row => row.FieldId.HasValue
                           ? new Field(row.FieldId.GetValueOrDefault())
                                 {
                                     Name = row.Name,
                                     Description = row.Description
                                 }
                           : new Field
                                 {
                                     Name = row.Name,
                                     Description = row.Description
                                 }).ToList();

            Assert.IsTrue(actual.All(field => field.FieldId.HasValue));
            CollectionAssert.AreEqual(fields, actual);
        }

        /// <summary>
        /// The merge into test.
        /// </summary>
        [TestMethod]
        public void DeleteUnmatchedInSource_GenericSubmissionValueTable_CommandTextMatchesExpected()
        {
            var internalId = new Field(421)
                                 {
                                     Name = "MERGE_Internal ID",
                                     Description = "Unique ID used internally"
                                 };

            var firstName = new Field(66)
                                {
                                    Name = "MERGE_First Name",
                                    Description = "The person's first name"
                                };

            var lastName = new Field(7887)
                               {
                                   Name = "MERGE_Last Name",
                                   Description = "The person's last name"
                               };

            var yearlyWage = new Field(82328)
                                 {
                                     Name = "MERGE_Yearly Wage",
                                     Description = "The base wage paid year over year."
                                 };

            var hireDate = new Field(7721123)
                               {
                                   Name = "MERGE_Hire Date",
                                   Description = "The date and time of hire for the person"
                               };

            var bonusTarget = new Field(928373)
                                  {
                                      Name = "MERGE_Bonus Target",
                                      Description = "The target bonus for the person"
                                  };

            var contactNumbers = new Field(667237)
                                     {
                                         Name = "MERGE_Contact Numbers",
                                         Description = "A list of contact numbers for the person in order of preference"
                                     };

            var domainIdentity = new DomainIdentity(Environment.UserName) { FirstName = "foo", LastName = "bar" };
            var submission = new GenericSubmission("Merge Test", domainIdentity, 678);
            submission.Load(
                new List<FieldValue>
                    {
                        new FieldValue(internalId, 239487).Set(34, domainIdentity),
                        new FieldValue(firstName, 3984).Set("Tim", domainIdentity),
                        new FieldValue(lastName, 439875).Set("bob", domainIdentity),
                        new FieldValue(yearlyWage, 98374).Set(75000.00m, domainIdentity),
                        new FieldValue(hireDate, 773839).Set(DateTimeOffset.Now.Date, domainIdentity),
                        new FieldValue(bonusTarget, 3543287).Set(1.103839, domainIdentity),
                        new FieldValue(contactNumbers, 77223).Set(new List<string> { "423-555-2212", "615.999.8888", "123-456-7890" }, domainIdentity),
                        new FieldValue(
                            new Field(777282)
                                {
                                    Name = "Deleted",
                                    Description = "Chuffed"
                                },
                            43543534).Set("DELETE_ME", domainIdentity)
                    });

            var mergeItems = (from v in submission.SubmissionValues
                              select new GenericSubmissionValueTableTypeRow
                                         {
                                             GenericSubmissionId = submission.GenericSubmissionId.GetValueOrDefault(),
                                             GenericSubmissionValueId = v.FieldValueId.GetValueOrDefault() 
                                         }).ToList();

            var commandProvider = mergeItems.MockCommandProvider(new DataAnnotationsDefinitionProvider());

            var target = new StructuredMergeCommand<GenericSubmissionValueTableTypeRow>(commandProvider.Object);
            target.MergeInto<GenericSubmissionValueRow>(mergeItems, row => row.GenericSubmissionValueId)
                .SelectFromInserted(row => row.GenericSubmissionValueId)
                .DeleteUnmatchedInSource(row => row.GenericSubmissionId);

            var expected = @"DECLARE @inserted GenericSubmissionValueTableType;
MERGE [dbo].[GenericSubmissionValue] AS [Target]
USING @GenericSubmissionValueTable AS [Source]
ON ([Target].[GenericSubmissionValueId] = [Source].[GenericSubmissionValueId])
WHEN MATCHED THEN
UPDATE SET [GenericSubmissionId] = [Source].[GenericSubmissionId]
WHEN NOT MATCHED BY TARGET THEN
INSERT ([GenericSubmissionValueId], [GenericSubmissionId])
VALUES ([Source].[GenericSubmissionValueId], [Source].[GenericSubmissionId])
WHEN NOT MATCHED BY SOURCE AND [Target].GenericSubmissionId IN (SELECT [GenericSubmissionId] FROM @GenericSubmissionValueTable) THEN DELETE
OUTPUT INSERTED.[GenericSubmissionValueId], INSERTED.[GenericSubmissionId]
INTO @inserted ([GenericSubmissionValueId], [GenericSubmissionId]);
SELECT tvp.[GenericSubmissionValueId], tvp.[GenericSubmissionId]
FROM @inserted AS i
INNER JOIN @GenericSubmissionValueTable AS tvp
ON i.[GenericSubmissionValueId] = tvp.[GenericSubmissionValueId];" + Environment.NewLine;

            Assert.AreEqual(expected, target.CommandText);
        }

        /// <summary>
        /// The execute for results generic submission database matches expected.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void ExecuteForResults_GenericSubmission_DatabaseMatchesExpected()
        {
            var internalId = new Field
                                 {
                                     Name = "MERGE_Existing_Internal ID",
                                     Description = "Unique ID used internally"
                                 };

            var firstName = new Field
                                {
                                    Name = "MERGE_Existing_First Name",
                                    Description = "The person's first name"
                                };

            var lastName = new Field
                               {
                                   Name = "MERGE_Existing_Last Name",
                                   Description = "The person's last name"
                               };

            var yearlyWage = new Field
                                 {
                                     Name = "MERGE_Existing_Yearly Wage",
                                     Description = "The base wage paid year over year."
                                 };

            var hireDate = new Field
                               {
                                   Name = "MERGE_NonExisting_Hire Date",
                                   Description = "The date and time of hire for the person"
                               };

            var bonusTarget = new Field
                                  {
                                      Name = "MERGE_NonExisting_Bonus Target",
                                      Description = "The target bonus for the person"
                                  };

            var contactNumbers = new Field
                                     {
                                         Name = "MERGE_NonExisting_Contact Numbers",
                                         Description = "A list of contact numbers for the person in order of preference"
                                     };

            var baselineFields = new List<Field>
                                     {
                                         internalId,
                                         firstName,
                                         lastName,
                                         yearlyWage,
                                         hireDate,
                                         bonusTarget,
                                         contactNumbers
                                     };
                
            var providerFactory = new SqlServerProviderFactory(
                ConfigurationRoot.GetConnectionString("OrmTestDb"),
                new DataAnnotationsDefinitionProvider());

            GenericSubmission baselineSubmission;
            DomainIdentity domainIdentity2;

            using (var provider = providerFactory.Create())
            {
                // Set up the domain identity, not part of our validity testing.
                var identityRepository = new EntityRepository<DomainIdentity, DomainIdentityRow>(provider, this.entityMapper);
                var domainIdentity = identityRepository.FirstOrDefault(
                                         Select.From<DomainIdentity>().WhereEqual(identity => identity.UniqueIdentifier, Environment.UserName))
                                     ?? identityRepository.Save(
                                         new DomainIdentity(Environment.UserName)
                                             {
                                                 FirstName = "King",
                                                 MiddleName = "T.",
                                                 LastName = "Animal"
                                             });

                var domainIdentifier2 = $"{Environment.UserName}2";
                domainIdentity2 = identityRepository.FirstOrDefault(
                                      Select.From<DomainIdentity>().WhereEqual(identity => identity.UniqueIdentifier, domainIdentifier2))
                                  ?? identityRepository.Save(
                                      new DomainIdentity(domainIdentifier2)
                                          {
                                              FirstName = "Foo",
                                              MiddleName = "J.",
                                              LastName = "Bar"
                                          });

                // Stage fields that we want to update
                var fieldRepository = new EntityRepository<Field, FieldRow>(provider, this.entityMapper);

                // Delete all "non-existing" fields.
                fieldRepository.Delete(Select.From<Field>().WhereEqual(field => field.Name, "MERGE_NonExisting_%"));
                
                // Merge in the field values.
                var commandProvider = new StructuredTransactSqlCommandProvider((IDatabaseContextProvider)provider);

                var transaction = provider.StartTransaction();

                // Merge our existing fields
                var fieldItems = from f in baselineFields.Where(field => field.Name.StartsWith("MERGE_Existing_"))
                                 select new FieldTableTypeRow
                                            {
                                                Name = f.Name,
                                                Description = f.Description
                                            };

                var fieldsCommand = new StructuredMergeCommand<FieldTableTypeRow>(commandProvider, transaction);
                var mergedFields = fieldsCommand
                    .MergeInto<FieldRow>(fieldItems, row => row.Name)
                    .SelectFromInserted(row => row.Name)
                    .ExecuteForResults()
                    .ToList();

                foreach (var field in baselineFields)
                {
                    var input = mergedFields.FirstOrDefault(f => string.Equals(f.Name, field.Name));

                    // Because we are doing a subset, and we know we will get back baseline fields. If MERGE is messed up this will error later when there
                    // aren't IDs for baseline fields.
                    if (input == null)
                    {
                        continue;
                    }

                    this.entityMapper.MapTo(input, field);
                }

                // We will add to this submission later.
                baselineSubmission = new GenericSubmission("My MERGE Submission", domainIdentity);
                baselineSubmission.SetValue(internalId, 9234);
                baselineSubmission.SetValue(firstName, "Dan");
                baselineSubmission.SetValue(lastName, "The Man");
                baselineSubmission.SetValue(yearlyWage, 72150.35m); // gonna get updated so lets check that this value got scrapped
                baselineSubmission.Submit();

                var submissionRepository = new EntityRepository<GenericSubmission, GenericSubmissionRow>(provider, this.entityMapper);
                submissionRepository.Save(baselineSubmission);

                Assert.IsTrue(baselineSubmission.GenericSubmissionId.HasValue);

                // Could be mapped as well.
                var fieldValues = from v in baselineSubmission.SubmissionValues
                                              select new FieldValueTableTypeRow
                                                         {
                                                             FieldId = v.Field.FieldId.GetValueOrDefault(),
                                                             LastModifiedByDomainIdentifierId = v.LastModifiedBy.DomainIdentityId.GetValueOrDefault(),
                                                             LastModifiedTime = v.LastModifiedTime
                                                         };

                // We use FieldValueId to essentially ensure we're only affecting the scope of this submission. FieldId on the select brings back
                // only inserted rows matched back to their original fields.
                var fieldValueCommand = new StructuredMergeCommand<FieldValueTableTypeRow>(commandProvider, transaction);
                var mergedFieldValues = fieldValueCommand
                    .MergeInto<FieldValueRow>(fieldValues, row => row.FieldValueId)
                    .SelectFromInserted(row => row.FieldId)
                    .ExecuteForResults()
                    .ToList();

                Assert.IsTrue(mergedFieldValues.All(row => row.FieldValueId.HasValue));

                // Map back to the domain object. TODO: Automate?
                foreach (var value in baselineSubmission.SubmissionValues)
                {
                    var input = mergedFieldValues.First(row => row.FieldId == value.Field.FieldId);
                    this.entityMapper.MapTo(input, value);
                    Assert.IsTrue(value.FieldValueId.HasValue);
                }

                // Now merge in the field value elements.
                // Do the field value elements
                var valueElements = (from e in baselineSubmission.SubmissionValues.SelectMany(value => value.Elements)
                                     select new FieldValueElementTableTypeRow
                                                {
                                                    FieldValueElementId = e.FieldValueElementId,
                                                    FieldValueId = e.FieldValue.FieldValueId.GetValueOrDefault(),
                                                    Order = e.Order,
                                                    DateElement = e.Element as DateTimeOffset? ?? e.Element as DateTime?,
                                                    FloatElement = e.Element as double? ?? e.Element as float?,
                                                    IntegerElement =
                                                        e.Element as long? ?? e.Element as int? ?? e.Element as short? ?? e.Element as byte?,
                                                    MoneyElement = e.Element as decimal?,
                                                    TextElement = e.Element as string // here we actually want it to be null if it is not a string
                                                }).ToList();

                var elementMergeCommand = new StructuredMergeCommand<FieldValueElementTableTypeRow>(commandProvider, transaction);
                var mergedValueElements = elementMergeCommand
                    .MergeInto<FieldValueElementRow>(valueElements, row => row.FieldValueId, row => row.Order)
                    .DeleteUnmatchedInSource(row => row.FieldValueId) // Get rid of extraneous elements
                    .SelectFromInserted(row => row.FieldValueId, row => row.Order) // Generally this is the same as the MERGE INTO 
                    .ExecuteForResults()
                    .ToList();

                foreach (var element in baselineSubmission.SubmissionValues.SelectMany(value => value.Elements))
                {
                    var input = mergedValueElements.First(row => row.FieldValueId == element.FieldValue.FieldValueId && row.Order == element.Order);
                    this.entityMapper.MapTo(input, element);
                    Assert.IsTrue(element.FieldValueElementId.HasValue);
                }

                var dateElementsCommand = new StructuredMergeCommand<FieldValueElementTableTypeRow>(commandProvider, transaction)
                    .MergeInto<DateElementRow>(mergedValueElements.Where(row => row.DateElement.HasValue), row => row.DateElementId, row => row.Value);
                    ////.From(row => row.FieldValueElementId, row => row.DateElement);

                dateElementsCommand.Execute();

                var floatElementsCommand = new StructuredMergeCommand<FieldValueElementTableTypeRow>(commandProvider, transaction)
                    .MergeInto<FloatElementRow>(mergedValueElements.Where(row => row.FloatElement.HasValue), row => row.FloatElementId, row => row.Value);
                    ////.From(row => row.FieldValueElementId, row => row.FloatElement);

                floatElementsCommand.Execute();

                var integerElementsCommand = new StructuredMergeCommand<FieldValueElementTableTypeRow>(commandProvider, transaction)
                    .MergeInto<IntegerElementRow>(
                        mergedValueElements.Where(row => row.IntegerElement.HasValue),
                        row => row.IntegerElementId,
                        row => row.Value);
                    ////.From(row => row.FieldValueElementId, row => row.IntegerElement);

                integerElementsCommand.Execute();

                var moneyElementsCommand = new StructuredMergeCommand<FieldValueElementTableTypeRow>(commandProvider, transaction)
                    .MergeInto<MoneyElementRow>(mergedValueElements.Where(row => row.MoneyElement.HasValue), row => row.MoneyElementId, row => row.Value);
                    ////.From(row => row.FieldValueElementId, row => row.MoneyElement);

                moneyElementsCommand.Execute();

                var textElementsCommand = new StructuredMergeCommand<FieldValueElementTableTypeRow>(commandProvider, transaction)
                    .MergeInto<TextElementRow>(mergedValueElements.Where(row => row.TextElement != null), row => row.TextElementId, row => row.Value);
                    ////.From(row => row.FieldValueElementId, row => row.TextElement);

                textElementsCommand.Execute();

                // Attach the values to the submission
                var genericValueSubmissions = from v in mergedFieldValues
                                              select new GenericSubmissionValueTableTypeRow
                                              {
                                                  GenericSubmissionId = baselineSubmission.GenericSubmissionId.GetValueOrDefault(),
                                                  GenericSubmissionValueId = v.FieldValueId.GetValueOrDefault()
                                              };

                var submissionCommand = new StructuredMergeCommand<GenericSubmissionValueTableTypeRow>(commandProvider, transaction)
                    .MergeInto<GenericSubmissionValueRow>(genericValueSubmissions);

                submissionCommand.Execute();
                transaction.Commit();
            }

            // Using a new provider clears any provider-level caches
            using (var provider = providerFactory.Create())
            {
                // Reusing the key lets us test whether updates are in fact working as expected.
                var expected = new GenericSubmission("My Final MERGE Submission", domainIdentity2, baselineSubmission.GenericSubmissionId.GetValueOrDefault());

                expected.SetValue(yearlyWage, 75100.35m);
                expected.SetValue(hireDate, DateTimeOffset.Now);
                expected.SetValue(bonusTarget, 1.59834578934);
                expected.SetValue(
                    contactNumbers,
                    new List<string>
                        {
                            "423-222-2252",
                            "615-982-0012",
                            "+1-555-252-5521"
                        });

                expected.Submit();

                var fields = expected.SubmissionValues.Select(value => value.Field).Distinct().ToDictionary(field => field.Name, field => field);
                var inclusionValues = fields.Keys.ToArray();

                var submissionRepository = new EntityRepository<GenericSubmission, GenericSubmissionRow>(provider, this.entityMapper);

                var transaction = provider.StartTransaction();
                submissionRepository.Save(expected);

                var submissionId = expected.GenericSubmissionId.GetValueOrDefault();
                Assert.AreNotEqual(0, submissionId);

                // Set up the structured command provider.
                var databaseContextProvider = (IDatabaseContextProvider)provider;
                var structuredCommandProvider = new StructuredTransactSqlCommandProvider(databaseContextProvider);

                // Do the field values
                var valuesList = from v in expected.SubmissionValues
                                 select new FieldValueTableTypeRow
                                            {
                                                FieldId = v.Field.FieldId.GetValueOrDefault(),
                                                LastModifiedByDomainIdentifierId = domainIdentity2.DomainIdentityId.GetValueOrDefault(),
                                                LastModifiedTime = expected.SubmittedTime
                                            };

                var valuesCommand = new StructuredInsertCommand<FieldValueTableTypeRow>(structuredCommandProvider, transaction)
                    .InsertInto<FieldValueRow>(valuesList)
                    .SelectResults(row => row.FieldId);

                var insertedValues = valuesCommand.ExecuteForResults().ToList();

                // Map back to the domain object.
                foreach (var value in expected.SubmissionValues)
                {
                    var input = insertedValues.FirstOrDefault(row => row.FieldId == value.Field.FieldId);
                    this.entityMapper.MapTo(input, value);
                }

                // Do the field value elements
                var elementsList = (from e in expected.SubmissionValues.SelectMany(value => value.Elements)
                                    select new FieldValueElementTableTypeRow
                                               {
                                                   FieldValueElementId = e.FieldValueElementId,
                                                   FieldValueId = e.FieldValue.FieldValueId.GetValueOrDefault(),
                                                   Order = e.Order,
                                                   DateElement = e.Element as DateTimeOffset? ?? e.Element as DateTime?,
                                                   FloatElement = e.Element as double? ?? e.Element as float?,
                                                   IntegerElement =
                                                       e.Element as long? ?? e.Element as int? ?? e.Element as short? ?? e.Element as byte?,
                                                   MoneyElement = e.Element as decimal?,
                                                   TextElement = e.Element as string // here we actually want it to be null if it is not a string
                                               }).ToList();

                var elementsCommand = new StructuredInsertCommand<FieldValueElementTableTypeRow>(structuredCommandProvider, transaction)
                    .InsertInto<FieldValueElementRow>(elementsList)
                    .SelectResults(row => row.FieldValueId, row => row.Order);

                // Reassign with our added identities
                // TODO: create dictionary for seeks
                elementsList = elementsCommand.ExecuteForResults().ToList();

                foreach (var element in expected.SubmissionValues.SelectMany(value => value.Elements))
                {
                    var input = elementsList.First(row => row.FieldValueId == element.FieldValue.FieldValueId && row.Order == element.Order);
                    this.entityMapper.MapTo(input, element);
                }

                var dateElementsCommand = new StructuredInsertCommand<FieldValueElementTableTypeRow>(structuredCommandProvider, transaction)
                    .InsertInto<DateElementRow>(elementsList.Where(row => row.DateElement.HasValue), row => row.DateElementId, row => row.Value)
                    .From(row => row.FieldValueElementId, row => row.DateElement);

                dateElementsCommand.Execute();

                var floatElementsCommand = new StructuredInsertCommand<FieldValueElementTableTypeRow>(structuredCommandProvider, transaction)
                    .InsertInto<FloatElementRow>(elementsList.Where(row => row.FloatElement.HasValue), row => row.FloatElementId, row => row.Value)
                    .From(row => row.FieldValueElementId, row => row.FloatElement);

                floatElementsCommand.Execute();

                var integerElementsCommand = new StructuredInsertCommand<FieldValueElementTableTypeRow>(structuredCommandProvider, transaction)
                    .InsertInto<IntegerElementRow>(
                        elementsList.Where(row => row.IntegerElement.HasValue),
                        row => row.IntegerElementId,
                        row => row.Value)
                    .From(row => row.FieldValueElementId, row => row.IntegerElement);

                integerElementsCommand.Execute();

                var moneyElementsCommand = new StructuredInsertCommand<FieldValueElementTableTypeRow>(structuredCommandProvider, transaction)
                    .InsertInto<MoneyElementRow>(elementsList.Where(row => row.MoneyElement.HasValue), row => row.MoneyElementId, row => row.Value)
                    .From(row => row.FieldValueElementId, row => row.MoneyElement);

                moneyElementsCommand.Execute();

                var textElementsCommand = new StructuredInsertCommand<FieldValueElementTableTypeRow>(structuredCommandProvider, transaction)
                    .InsertInto<TextElementRow>(elementsList.Where(row => row.TextElement != null), row => row.TextElementId, row => row.Value)
                    .From(row => row.FieldValueElementId, row => row.TextElement);

                textElementsCommand.Execute();

                // Attach the values to the submission
                var genericValueSubmissions = from v in insertedValues
                                              select new GenericSubmissionValueTableTypeRow
                                                         {
                                                             GenericSubmissionId = submissionId,
                                                             GenericSubmissionValueId = v.FieldValueId.GetValueOrDefault()
                                                         };

                var submissionCommand = new StructuredInsertCommand<GenericSubmissionValueTableTypeRow>(structuredCommandProvider, transaction)
                    .InsertInto<GenericSubmissionValueRow>(genericValueSubmissions);

                submissionCommand.Execute();

                transaction.Commit();
            }
        }
    }
}