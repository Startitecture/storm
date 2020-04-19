// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableValuedMergeTests.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Startitecture.Orm.SqlClient.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Core;
    using Startitecture.Orm.AutoMapper;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Schema;
    using Startitecture.Orm.SqlClient;
    using Startitecture.Orm.Testing.Entities;
    using Startitecture.Orm.Testing.Entities.TableTypes;
    using Startitecture.Orm.Testing.Model;
    using Startitecture.Orm.Testing.Moq;

    /// <summary>
    /// The structured merge command tests.
    /// </summary>
    [TestClass]
    public class TableValuedMergeTests
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
            var commandProvider = mergeItems.MockCommandProvider(definitionProvider, new TransactSqlQualifier());
            var target = new TableValuedMerge<FieldTableTypeRow>(commandProvider.Object);
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

            var commandProvider = mergeItems.MockCommandProvider(new DataAnnotationsDefinitionProvider(), new TransactSqlQualifier());

            var target = new TableValuedMerge<GenericSubmissionValueTableTypeRow>(commandProvider.Object);
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
                
            var providerFactory = new SqlClientProviderFactory(
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

                // We will add to this submission later.
                baselineSubmission = new GenericSubmission("My MERGE Submission", domainIdentity);
                baselineSubmission.SetValue(internalId, 9234);
                baselineSubmission.SetValue(firstName, "Dan");
                baselineSubmission.SetValue(lastName, "The Man");
                baselineSubmission.SetValue(yearlyWage, 72150.35m); // gonna get updated so lets check that this value got scrapped
                baselineSubmission.Submit();

                this.MergeSubmission(baselineSubmission, provider);
            }

            Assert.IsTrue(baselineSubmission.GenericSubmissionId.HasValue);

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

            GenericSubmission actual;

            // Using a new provider clears any provider-level caches
            using (var provider = providerFactory.Create())
            {
                this.MergeSubmission(expected, provider);

                var submissionRepository = new EntityRepository<GenericSubmission, GenericSubmissionRow>(provider, this.entityMapper);
                actual = submissionRepository.FirstOrDefault(expected.GenericSubmissionId);

                var fieldValueRepository = new EntityRepository<FieldValue, GenericSubmissionValueRow>(provider, this.entityMapper);
                var values = fieldValueRepository.Select(
                        Select.From<GenericSubmissionValueRow>()
                            .InnerJoin(row => row.GenericSubmissionValueId, row => row.FieldValue.FieldValueId)
                            .InnerJoin(row => row.FieldValue.FieldId, row => row.FieldValue.Field.FieldId)
                            .InnerJoin(row => row.FieldValue.LastModifiedByDomainIdentifierId, row => row.FieldValue.LastModifiedBy.DomainIdentityId)
                            .WhereEqual(row => row.GenericSubmissionId, expected.GenericSubmissionId.GetValueOrDefault()))
                    .ToDictionary(value => value.FieldValueId.GetValueOrDefault(), value => value);

                actual.Load(values.Values);

                var valueElementRows = provider.GetSelection(
                        Select.From<FieldValueElementTableTypeRow>()
                            .LeftJoin<DateElementRow>(row => row.FieldValueElementId, row => row.DateElementId)
                            .LeftJoin<FloatElementRow>(row => row.FieldValueElementId, row => row.FloatElementId)
                            .LeftJoin<IntegerElementRow>(row => row.FieldValueElementId, row => row.IntegerElementId)
                            .LeftJoin<MoneyElementRow>(row => row.FieldValueElementId, row => row.MoneyElementId)
                            .LeftJoin<TextElementRow>(row => row.FieldValueElementId, row => row.TextElementId)
                            .Include(row => row.FieldValueId, values.Keys.ToArray()))
                    .ToList();

                foreach (var key in values.Keys)
                {
                    values[key]
                        .Load(
                            from e in valueElementRows
                            where e.FieldValueId == key
                            orderby e.Order
                            select new FieldValueElement(
                                e.DateElement ?? e.FloatElement ?? e.IntegerElement ?? e.MoneyElement ?? e.TextElement as object,
                                e.FieldValueElementId.GetValueOrDefault()));
                }
            }

            Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
            CollectionAssert.AreEqual(
                expected.SubmissionValues.OrderBy(x => x.FieldValueId).ToList(),
                actual.SubmissionValues.OrderBy(x => x.FieldValueId).ToList());

            var expectedElements = expected.SubmissionValues.SelectMany(value => value.Elements).OrderBy(element => element.FieldValueElementId).ToList();
            var actualElements = actual.SubmissionValues.SelectMany(value => value.Elements).OrderBy(element => element.FieldValueElementId).ToList();
            CollectionAssert.AreEqual(expectedElements, actualElements);
        }

        /// <summary>
        /// Merges the specified <paramref name="submission"/> into the repository backed by the <paramref name="provider"/>.
        /// </summary>
        /// <param name="submission">
        /// The submission.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        private void MergeSubmission(
            GenericSubmission submission,
            IRepositoryProvider provider)
        {
            // Merge our existing fields
            var fields = submission.SubmissionValues.Select(value => value.Field).Distinct().ToList();
            var fieldItems = from f in fields
                             select new FieldTableTypeRow
                                        {
                                            Name = f.Name,
                                            Description = f.Description
                                        };

            // Merge in the field values.
            var commandProvider = new TableValuedCommandProvider((IDatabaseContextProvider)provider);
            var transaction = provider.StartTransaction();

            var fieldsCommand = new TableValuedMerge<FieldTableTypeRow>(commandProvider, transaction);
            var mergedFields = fieldsCommand
                .MergeInto<FieldRow>(fieldItems, row => row.Name)
                .SelectFromInserted(row => row.Name)
                .ExecuteForResults()
                .ToList();

            foreach (var field in fields)
            {
                var input = mergedFields.FirstOrDefault(f => string.Equals(f.Name, field.Name, StringComparison.Ordinal));

                // Because we are doing a subset, and we know we will get back baseline fields. If MERGE is messed up this will error later when there
                // aren't IDs for baseline fields.
                if (input == null)
                {
                    continue;
                }

                this.entityMapper.MapTo(input, field);
            }

            var submissionRepository = new EntityRepository<GenericSubmission, GenericSubmissionRow>(provider, this.entityMapper);
            submissionRepository.Save(submission);

            // Could be mapped as well.
            var fieldValues = from v in submission.SubmissionValues
                              select new FieldValueTableTypeRow
                                         {
                                             FieldId = v.Field.FieldId.GetValueOrDefault(),
                                             LastModifiedByDomainIdentifierId = v.LastModifiedBy.DomainIdentityId.GetValueOrDefault(),
                                             LastModifiedTime = v.LastModifiedTime
                                         };

            // We use FieldValueId to essentially ensure we're only affecting the scope of this submission. FieldId on the select brings back
            // only inserted rows matched back to their original fields.
            var fieldValueCommand = new TableValuedMerge<FieldValueTableTypeRow>(commandProvider, transaction);
            var mergedFieldValues = fieldValueCommand.MergeInto<FieldValueRow>(fieldValues, row => row.FieldValueId)
                .SelectFromInserted(row => row.FieldId)
                .ExecuteForResults()
                .ToList();

            Assert.IsTrue(mergedFieldValues.All(row => row.FieldValueId.HasValue));

            // Map back to the domain object. TODO: Automate?
            foreach (var value in submission.SubmissionValues)
            {
                var input = mergedFieldValues.First(row => row.FieldId == value.Field.FieldId);
                this.entityMapper.MapTo(input, value);
                Assert.IsTrue(value.FieldValueId.HasValue);
            }

            // Now merge in the field value elements.
            // Do the field value elements
            var valueElements = (from e in submission.SubmissionValues.SelectMany(value => value.Elements)
                                 select new FieldValueElementTableTypeRow
                                            {
                                                FieldValueElementId = e.FieldValueElementId,
                                                FieldValueId = e.FieldValue.FieldValueId.GetValueOrDefault(),
                                                Order = e.Order,
                                                DateElement = e.Element as DateTimeOffset? ?? e.Element as DateTime?,
                                                FloatElement = e.Element as double? ?? e.Element as float?,
                                                IntegerElement = e.Element as long? ?? e.Element as int? ?? e.Element as short? ?? e.Element as byte?,
                                                MoneyElement = e.Element as decimal?,
                                                TextElement = e.Element as string // here we actually want it to be null if it is not a string
                                            }).ToList();

            var elementMergeCommand = new TableValuedMerge<FieldValueElementTableTypeRow>(commandProvider, transaction);
            var mergedValueElements = elementMergeCommand.MergeInto<FieldValueElementRow>(valueElements, row => row.FieldValueId, row => row.Order)
                .DeleteUnmatchedInSource(row => row.FieldValueId) // Get rid of extraneous elements
                .SelectFromInserted(row => row.FieldValueId, row => row.Order) // Generally this is the same as the MERGE INTO 
                .ExecuteForResults()
                .ToList();

            foreach (var element in submission.SubmissionValues.SelectMany(value => value.Elements))
            {
                var input = mergedValueElements.First(row => row.FieldValueId == element.FieldValue.FieldValueId && row.Order == element.Order);
                this.entityMapper.MapTo(input, element);
                Assert.IsTrue(element.FieldValueElementId.HasValue);
            }

            var dateElementsCommand = new TableValuedMerge<FieldValueElementTableTypeRow>(commandProvider, transaction)
                .MergeInto<DateElementRow>(mergedValueElements.Where(row => row.DateElement.HasValue))
                .On<DateElementRow>(row => row.FieldValueElementId, row => row.DateElementId)
                .From(row => row.FieldValueElementId, row => row.DateElement);

            dateElementsCommand.Execute();

            var floatElementsCommand = new TableValuedMerge<FieldValueElementTableTypeRow>(commandProvider, transaction)
                .MergeInto<FloatElementRow>(mergedValueElements.Where(row => row.FloatElement.HasValue))
                .On<FloatElementRow>(row => row.FieldValueElementId, row => row.FloatElementId)
                .From(row => row.FieldValueElementId, row => row.FloatElement);

            floatElementsCommand.Execute();

            var integerElementsCommand = new TableValuedMerge<FieldValueElementTableTypeRow>(commandProvider, transaction)
                .MergeInto<IntegerElementRow>(mergedValueElements.Where(row => row.IntegerElement.HasValue))
                .On<IntegerElementRow>(row => row.FieldValueElementId, row => row.IntegerElementId)
                .From(row => row.FieldValueElementId, row => row.IntegerElement);

            integerElementsCommand.Execute();

            var moneyElementsCommand = new TableValuedMerge<FieldValueElementTableTypeRow>(commandProvider, transaction)
                .MergeInto<MoneyElementRow>(mergedValueElements.Where(row => row.MoneyElement.HasValue))
                .On<MoneyElementRow>(row => row.FieldValueElementId, row => row.MoneyElementId)
                .From(row => row.FieldValueElementId, row => row.MoneyElement);

            moneyElementsCommand.Execute();

            var textElementsCommand = new TableValuedMerge<FieldValueElementTableTypeRow>(commandProvider, transaction)
                .MergeInto<TextElementRow>(mergedValueElements.Where(row => row.TextElement != null))
                .On<TextElementRow>(row => row.FieldValueElementId, row => row.TextElementId)
                .From(row => row.FieldValueElementId, row => row.TextElement);

            textElementsCommand.Execute();

            // Attach the values to the submission
            var genericValueSubmissions = from v in mergedFieldValues
                                          select new GenericSubmissionValueTableTypeRow
                                                     {
                                                         GenericSubmissionId = submission.GenericSubmissionId.GetValueOrDefault(),
                                                         GenericSubmissionValueId = v.FieldValueId.GetValueOrDefault()
                                                     };

            var submissionCommand = new TableValuedMerge<GenericSubmissionValueTableTypeRow>(commandProvider, transaction)
                .MergeInto<GenericSubmissionValueRow>(genericValueSubmissions, row => row.GenericSubmissionValueId)
                .DeleteUnmatchedInSource(row => row.GenericSubmissionId);

            submissionCommand.Execute();
            transaction.Commit();
        }
    }
}