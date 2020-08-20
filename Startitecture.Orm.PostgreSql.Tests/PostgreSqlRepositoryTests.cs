﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostgreSqlRepositoryTests.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Defines the PostgreSqlRepositoryTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.PostgreSql.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::AutoMapper;

    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Core;
    using Startitecture.Orm.AutoMapper;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Schema;
    using Startitecture.Orm.Testing.Entities;
    using Startitecture.Orm.Testing.Entities.TableTypes;
    using Startitecture.Orm.Testing.Model;

    /// <summary>
    /// The PostgreSQL repository tests.
    /// </summary>
    [TestClass]
    public class PostgreSqlRepositoryTests
    {
        /// <summary>
        /// The entity mapper.
        /// </summary>
        private readonly IEntityMapperFactory mapperFactory = new EntityMapperFactory(
            new MapperConfiguration(expression => { expression.AddProfile<GenericSubmissionMappingProfile>(); }));

        /// <summary>
        /// The configuration root.
        /// </summary>
        private static IConfigurationRoot ConfigurationRoot => new ConfigurationBuilder().AddJsonFile("appSettings.json", false).Build();

        /// <summary>
        /// The execute test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void Insert_ListOfFields_DoesNotThrowException()
        {
            var internalId = new Field
                                 {
                                     Name = "INS_Internal ID",
                                     Description = "Unique ID used internally"
                                 };

            var firstName = new Field
                                {
                                    Name = "INS_First Name",
                                    Description = "The person's first name"
                                };

            var lastName = new Field
                               {
                                   Name = "INS_Last Name",
                                   Description = "The person's last name"
                               };

            var yearlyWage = new Field
                                 {
                                     Name = "INS_Yearly Wage",
                                     Description = "The base wage paid year over year."
                                 };

            var hireDate = new Field
                               {
                                   Name = "INS_Hire Date",
                                   Description = "The date and time of hire for the person"
                               };

            var bonusTarget = new Field
                                  {
                                      Name = "INS_Bonus Target",
                                      Description = "The target bonus for the person"
                                  };

            var contactNumbers = new Field
                                     {
                                         Name = "INS_Contact Numbers",
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

            var mapper = this.mapperFactory.Create();
            var providerFactory = new PostgreSqlProviderFactory(new DataAnnotationsDefinitionProvider());

            using (var provider = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDbPg")))
            {
                var fieldRepository = new PostgreSqlRepository<Field, FieldRow>(provider, mapper);

                // Delete the existing rows.
                fieldRepository.Delete(Select.From<FieldRow>().WhereEqual(row => row.Name, "INS_%"));

                var fieldRows = fields.Select(
                    field => new FieldRow
                                 {
                                     Name = field.Name,
                                     Description = field.Description
                                 });

                var transaction = provider.StartTransaction();
                fieldRepository.Insert(fieldRows, transaction, null); // Nothing special here
                transaction.Commit();
            }
        }

        /// <summary>
        /// The execute test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void InsertForResults_TableValueInsertForFieldsWithReturn_MatchesExpected()
        {
            var internalId = new Field
                                 {
                                     Name = "INS_Internal ID",
                                     Description = "Unique ID used internally"
                                 };

            var firstName = new Field
                                {
                                    Name = "INS_First Name",
                                    Description = "The person's first name"
                                };

            var lastName = new Field
                               {
                                   Name = "INS_Last Name",
                                   Description = "The person's last name"
                               };

            var yearlyWage = new Field
                                 {
                                     Name = "INS_Yearly Wage",
                                     Description = "The base wage paid year over year."
                                 };

            var hireDate = new Field
                               {
                                   Name = "INS_Hire Date",
                                   Description = "The date and time of hire for the person"
                               };

            var bonusTarget = new Field
                                  {
                                      Name = "INS_Bonus Target",
                                      Description = "The target bonus for the person"
                                  };

            var contactNumbers = new Field
                                     {
                                         Name = "INS_Contact Numbers",
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
            var mapper = this.mapperFactory.Create();
            var providerFactory = new PostgreSqlProviderFactory(new DataAnnotationsDefinitionProvider());

            using (var provider = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDbPg")))
            {
                var fieldRepository = new PostgreSqlRepository<Field, FieldRow>(provider, mapper);

                // Delete the existing rows.
                fieldRepository.Delete(Select.From<FieldRow>().WhereEqual(row => row.Name, "INS_%"));

                var fieldRows = fields.Select(
                    field => new FieldRow
                                 {
                                     Name = field.Name,
                                     Description = field.Description
                                 });

                var transaction = provider.StartTransaction();
                var actual = fieldRepository.InsertForResults(fieldRows, transaction, null);

                foreach (var fieldRow in actual)
                {
                    Assert.AreNotEqual(0, fieldRow.FieldId);
                }

                transaction.Commit();
            }
        }

        /// <summary>
        /// The execute test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void InsertForResults_GenericSubmission_MatchesExpected()
        {
            var mapper = this.mapperFactory.Create();
            var providerFactory = new PostgreSqlProviderFactory(new DataAnnotationsDefinitionProvider());

            using (var provider = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDbPg")))
            {
                var identityRepository = new PostgreSqlRepository<DomainIdentity, DomainIdentityRow>(provider, mapper);

                var domainIdentity = identityRepository.FirstOrDefault(
                                         Select.From<DomainIdentity>().WhereEqual(identity => identity.UniqueIdentifier, Environment.UserName))
                                     ?? identityRepository.Save(
                                         new DomainIdentity(Environment.UserName)
                                             {
                                                 FirstName = "King",
                                                 MiddleName = "T.",
                                                 LastName = "Animal"
                                             });

                var expected = new GenericSubmission("My Submission", domainIdentity);
                var internalId = new Field
                                     {
                                         Name = "Internal ID",
                                         Description = "Unique ID used internally"
                                     };

                var firstName = new Field
                                    {
                                        Name = "First Name",
                                        Description = "The person's first name"
                                    };

                var lastName = new Field
                                   {
                                       Name = "Last Name",
                                       Description = "The person's last name"
                                   };

                var yearlyWage = new Field
                                     {
                                         Name = "Yearly Wage",
                                         Description = "The base wage paid year over year."
                                     };

                var hireDate = new Field
                                   {
                                       Name = "Hire Date",
                                       Description = "The date and time of hire for the person"
                                   };

                var bonusTarget = new Field
                                      {
                                          Name = "Bonus Target",
                                          Description = "The target bonus for the person"
                                      };

                var contactNumbers = new Field
                                         {
                                             Name = "Contact Numbers",
                                             Description = "A list of contact numbers for the person in order of preference"
                                         };

                expected.SetValue(internalId, 9234);
                expected.SetValue(firstName, "Dan");
                expected.SetValue(lastName, "The Man");
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

                var fieldRepository = new PostgreSqlRepository<Field, FieldRow>(provider, mapper);

                var fields = expected.SubmissionValues.Select(value => value.Field).Distinct().ToDictionary(field => field.Name, field => field);
                var inclusionValues = fields.Keys.ToArray();
                var existingFields = fieldRepository.SelectEntities(new EntitySelection<Field>().Include(field => field.Name, inclusionValues));

                foreach (var field in existingFields)
                {
                    var output = fields[field.Name];
                    mapper.MapTo(field, output);
                }

                foreach (var field in fields.Values.Where(field => field.FieldId.HasValue == false))
                {
                    fieldRepository.Save(field);
                }

                var submissionRepository = new PostgreSqlRepository<GenericSubmission, GenericSubmissionRow>(provider, mapper);

                var transaction = provider.StartTransaction();
                submissionRepository.Save(expected);

                var submissionId = expected.GenericSubmissionId.GetValueOrDefault();
                Assert.AreNotEqual(0, submissionId);

                var fieldValueRepository = new PostgreSqlRepository<FieldValue, FieldValueRow>(provider, mapper);

                // Do the field values
                var valuesList = from v in expected.SubmissionValues
                                 select new FieldValueRow
                                            {
                                                FieldId = v.Field.FieldId.GetValueOrDefault(),
                                                LastModifiedByDomainIdentifierId = domainIdentity.DomainIdentityId.GetValueOrDefault(),
                                                LastModifiedTime = expected.SubmittedTime
                                            };

                var insertedValues = fieldValueRepository.InsertForResults(valuesList, transaction, null)
                    .ToDictionary(row => row.FieldId, row => row);

                // Map back to the domain object.
                foreach (var value in expected.SubmissionValues)
                {
                    var input = insertedValues[value.Field.FieldId.GetValueOrDefault()];
                    mapper.MapTo(input, value);
                }

                var fieldValueElementRepository = new PostgreSqlRepository<FieldValueElement, FieldValueElementRow>(provider, mapper);

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

                var insertedElements = fieldValueElementRepository.InsertForResults(
                        elementsList,
                        transaction,
                        insert => insert.Returning(row => row.FieldValueId, row => row.Order))
                    .ToDictionary(row => new Tuple<long, int>(row.FieldValueId, row.Order));

                foreach (var element in expected.SubmissionValues.SelectMany(value => value.Elements))
                {
                    var input = insertedElements[new Tuple<long, int>(element.FieldValue.FieldValueId.GetValueOrDefault(), element.Order)];
                    mapper.MapTo(input, element);
                }

                var dateElementRepository = new PostgreSqlRepository<FieldValueElement, DateElementRow>(provider, mapper);
                dateElementRepository.Insert(
                    elementsList.Where(row => row.DateElement.HasValue),
                    transaction,
                    insert => insert.InsertInto(row => row.DateElementId, row => row.Value)
                        .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.DateElement));

                var floatElementRepository = new PostgreSqlRepository<FieldValueElement, FloatElementRow>(provider, mapper);
                floatElementRepository.Insert(
                    elementsList.Where(row => row.FloatElement.HasValue),
                    transaction,
                    insert => insert.InsertInto(row => row.FloatElementId, row => row.Value)
                        .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.FloatElement));

                var integerElementRepository = new PostgreSqlRepository<FieldValueElement, IntegerElementRow>(provider, mapper);
                integerElementRepository.Insert(
                    elementsList.Where(row => row.IntegerElement.HasValue),
                    transaction,
                    insert => insert.InsertInto(row => row.IntegerElementId, row => row.Value)
                        .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.IntegerElement));

                var moneyElementRepository = new PostgreSqlRepository<FieldValueElement, MoneyElementRow>(provider, mapper);
                moneyElementRepository.Insert(
                    elementsList.Where(row => row.MoneyElement.HasValue),
                    transaction,
                    insert => insert.InsertInto(row => row.MoneyElementId, row => row.Value)
                        .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.MoneyElement));

                var textElementRepository = new PostgreSqlRepository<FieldValueElement, TextElementRow>(provider, mapper);
                textElementRepository.Insert(
                    elementsList.Where(row => row.TextElement != null),
                    transaction,
                    insert => insert.InsertInto(row => row.TextElementId, row => row.Value)
                        .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.TextElement));

                // Attach the values to the submission
                var genericValueSubmissions = from v in insertedValues.Values
                                              select new GenericSubmissionValueRow
                                                         {
                                                             GenericSubmissionId = submissionId,
                                                             GenericSubmissionValueId = v.FieldValueId
                                                         };

                var genericSubmissionValueRepository = new PostgreSqlRepository<FieldValue, GenericSubmissionValueRow>(provider, mapper);
                genericSubmissionValueRepository.Insert(genericValueSubmissions, transaction, null);
                transaction.Commit();
            }
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

            var providerFactory = new PostgreSqlProviderFactory(new DataAnnotationsDefinitionProvider());

            GenericSubmission baselineSubmission;
            DomainIdentity domainIdentity2;

            var mapper = this.mapperFactory.Create();

            using (var provider = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDbPg")))
            {
                // Set up the domain identity, not part of our validity testing.
                var identityRepository = new PostgreSqlRepository<DomainIdentity, DomainIdentityRow>(provider, mapper);
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

                this.MergeSubmission(baselineSubmission, provider, mapper);
            }

            Assert.IsTrue(baselineSubmission.GenericSubmissionId.HasValue);

            // Reusing the key lets us test whether updates are in fact working as expected.
            var expected = new GenericSubmission(
                "My Final MERGE Submission",
                domainIdentity2,
                baselineSubmission.GenericSubmissionId.GetValueOrDefault());

            expected.SetValue(yearlyWage, 75100.35m);
            expected.SetValue(hireDate, new DateTimeOffset(DateTimeOffset.Now.Date));
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
            using (var provider = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDbPg")))
            {
                var fieldValueRepository = new PostgreSqlRepository<FieldValue, FieldValueRow>(provider, mapper);

                // Get rid of all the previous fields.
                fieldValueRepository.Delete(
                    Select.From<FieldValueRow>()
                        .Include(row => row.FieldValueId, baselineSubmission.SubmissionValues.Select(value => value.FieldValueId).ToArray()));

                this.MergeSubmission(expected, provider, mapper);

                var submissionRepository = new PostgreSqlRepository<GenericSubmission, GenericSubmissionRow>(provider, mapper);
                actual = submissionRepository.FirstOrDefault(expected.GenericSubmissionId);

                var genericSubmissionValueRepository = new PostgreSqlRepository<FieldValue, GenericSubmissionValueRow>(provider, mapper);
                var values = genericSubmissionValueRepository.SelectEntities(
                        Select.From<GenericSubmissionValueRow>()
                            .InnerJoin(row => row.GenericSubmissionValueId, row => row.FieldValue.FieldValueId)
                            .InnerJoin(row => row.FieldValue.FieldId, row => row.FieldValue.Field.FieldId)
                            .InnerJoin(row => row.FieldValue.LastModifiedByDomainIdentifierId, row => row.FieldValue.LastModifiedBy.DomainIdentityId)
                            .WhereEqual(row => row.GenericSubmissionId, expected.GenericSubmissionId.GetValueOrDefault()))
                    .ToDictionary(value => value.FieldValueId.GetValueOrDefault(), value => value);

                actual.Load(values.Values);

                var valueElementRows = provider.SelectEntities(
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

            var expectedElements = expected.SubmissionValues.SelectMany(value => value.Elements)
                .OrderBy(element => element.FieldValueElementId)
                .ToList();

            var actualElements = actual.SubmissionValues.SelectMany(value => value.Elements).OrderBy(element => element.FieldValueElementId).ToList();
            var firstExpectedElement = expectedElements.Skip(1).First();
            var firstActualElement = actualElements.Skip(1).FirstOrDefault();
            Assert.AreEqual(
                firstExpectedElement,
                firstActualElement,
                string.Join(Environment.NewLine, firstExpectedElement.GetDifferences(firstActualElement)));

            CollectionAssert.AreEqual(expectedElements, actualElements);

            Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
            CollectionAssert.AreEqual(
                expected.SubmissionValues.OrderBy(x => x.FieldValueId).ToList(),
                actual.SubmissionValues.OrderBy(x => x.FieldValueId).ToList());
        }

        /// <summary>
        /// Merges the specified <paramref name="submission" /> into the repository backed by the <paramref name="provider" />.
        /// </summary>
        /// <param name="submission">
        /// The submission.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="mapper">
        /// The mapper.
        /// </param>
        private void MergeSubmission(GenericSubmission submission, IRepositoryProvider provider, IEntityMapper mapper)
        {
            // Merge our existing fields
            var fields = submission.SubmissionValues.Select(value => value.Field).Distinct().ToList();
            var fieldItems = from f in fields
                             select new FieldRow
                                        {
                                            Name = f.Name,
                                            Description = f.Description
                                        };

            var transaction = provider.StartTransaction();

            var fieldRepository = new PostgreSqlRepository<Field, FieldRow>(provider, mapper);
            var mergedFields = fieldRepository.InsertForResults(
                    fieldItems,
                    transaction,
                    insert => insert.OnConflict(row => row.Name)
                        .Upsert(row => row.Description)
                        .Returning(row => row.FieldId, row => row.Name, row => row.Description))
                .ToDictionary(row => row.Name, row => row);

            foreach (var field in fields)
            {
                mergedFields.TryGetValue(field.Name, out var input);

                // Because we are doing a subset, and we know we will get back baseline fields. If MERGE is messed up this will error later when there
                // aren't IDs for baseline fields.
                if (input == null)
                {
                    continue;
                }

                mapper.MapTo(input, field);
            }

            var submissionRepository = new PostgreSqlRepository<GenericSubmission, GenericSubmissionRow>(provider, mapper);
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
            var fieldValueRepository = new PostgreSqlRepository<FieldValue, FieldValueRow>(provider, mapper);
            var mergedFieldValues = fieldValueRepository.InsertForResults(
                    fieldValues,
                    transaction,
                    insert => insert.OnConflict(row => row.FieldValueId)
                        .Upsert(row => row.LastModifiedByDomainIdentifierId, row => row.LastModifiedTime)
                        .Returning(
                            row => row.FieldValueId,
                            row => row.FieldId,
                            row => row.LastModifiedByDomainIdentifierId,
                            row => row.LastModifiedTime))
                .ToDictionary(row => row.FieldId, row => row);

            Assert.IsTrue(mergedFieldValues.Values.All(row => row.FieldValueId > 0));

            // Map back to the domain object. TODO: Automate?
            foreach (var value in submission.SubmissionValues)
            {
                var input = mergedFieldValues[value.Field.FieldId.GetValueOrDefault()];
                mapper.MapTo(input, value);
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

            var fieldValueElementRepository = new PostgreSqlRepository<FieldValueElement, FieldValueElementRow>(provider, mapper);
            var mergedValueElements = fieldValueElementRepository.InsertForResults(
                    valueElements,
                    transaction,
                    insert => insert.OnConflict(row => row.FieldValueElementId)
                        .Upsert(row => row.Order)
                        .Returning(row => row.FieldValueElementId, row => row.FieldValueId, row => row.Order))
                .ToDictionary(row => new Tuple<long, int>(row.FieldValueId, row.Order), row => row);

            foreach (var element in valueElements)
            {
                var input = mergedValueElements[new Tuple<long, int>(element.FieldValueId, element.Order)];
                element.FieldValueElementId = input.FieldValueElementId;
                Assert.IsTrue(element.FieldValueElementId.HasValue);
            }

            // Note that we use the value elements for insertion because mergeValueElements will only have what is in the FieldValueElement row.
            var dateElementRepository = new PostgreSqlRepository<FieldValueElement, DateElementRow>(provider, mapper);
            dateElementRepository.Insert(
                valueElements.Where(row => row.DateElement.HasValue),
                transaction,
                insert => insert.From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.DateElement)
                    .OnConflict(row => row.DateElementId)
                    .Upsert(row => row.Value));

            var floatElementRepository = new PostgreSqlRepository<FieldValueElement, FloatElementRow>(provider, mapper);
            floatElementRepository.Insert(
                valueElements.Where(row => row.FloatElement.HasValue),
                transaction,
                insert => insert.From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.FloatElement)
                    .OnConflict(row => row.FloatElementId)
                    .Upsert(row => row.Value));

            var integerElementRepository = new PostgreSqlRepository<FieldValueElement, IntegerElementRow>(provider, mapper);
            integerElementRepository.Insert(
                valueElements.Where(row => row.IntegerElement.HasValue),
                transaction,
                insert => insert.From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.IntegerElement)
                    .OnConflict(row => row.IntegerElementId)
                    .Upsert(row => row.Value));

            var moneyElementRepository = new PostgreSqlRepository<FieldValueElement, MoneyElementRow>(provider, mapper);
            moneyElementRepository.Insert(
                valueElements.Where(row => row.MoneyElement.HasValue),
                transaction,
                insert => insert.From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.MoneyElement)
                    .OnConflict(row => row.MoneyElementId)
                    .Upsert(row => row.Value));

            var textElementRepository = new PostgreSqlRepository<FieldValueElement, TextElementRow>(provider, mapper);
            textElementRepository.Insert(
                valueElements.Where(row => row.TextElement != null),
                transaction,
                insert => insert.From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.TextElement)
                    .OnConflict(row => row.TextElementId)
                    .Upsert(row => row.Value));

            // Attach the values to the submission
            var genericValueSubmissions = from v in mergedFieldValues.Values
                                          select new GenericSubmissionValueTableTypeRow
                                                     {
                                                         GenericSubmissionId = submission.GenericSubmissionId.GetValueOrDefault(),
                                                         GenericSubmissionValueId = v.FieldValueId.GetValueOrDefault()
                                                     };

            var genericSubmissionValueRepository = new PostgreSqlRepository<FieldValue, GenericSubmissionValueRow>(provider, mapper);
            genericSubmissionValueRepository.Insert(genericValueSubmissions, transaction, null);
            transaction.Commit();
        }
    }
}