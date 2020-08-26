// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlClientRepositoryTests.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The SQL client repository tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.SqlClient.Tests
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
    /// The SQL client repository tests.
    /// </summary>
    [TestClass]
    public class SqlClientRepositoryTests
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
        /// The insert list of fields does not throw exception.
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
            
            var providerFactory = new SqlClientProviderFactory(new DataAnnotationsDefinitionProvider());
            var entityMapper = this.mapperFactory.Create();

            using (var provider = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
            {
                var transaction = provider.StartTransaction();
                var fieldRepository = new SqlClientRepository<Field, FieldRow>(provider, entityMapper);

                var fieldSelection = Select.From<FieldRow>().WhereEqual(row => row.Name, "INS_%");
                var fieldValuesToDelete = provider.DynamicSelect(fieldSelection.Select(row => row.FieldId));
                provider.Delete(Select.From<FieldValueRow>().Include(row => row.FieldId, fieldValuesToDelete.Select(o => (int)o.FieldId).ToArray()));
                fieldRepository.Delete(fieldSelection);
                
                var fieldRows = from f in fields
                                select new FieldTableTypeRow
                                           {
                                               Name = f.Name,
                                               Description = f.Description
                                           };

                fieldRepository.Insert(fieldRows, transaction, null);
                transaction.Commit();
            }
        }

        /// <summary>
        /// The insert for results test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void InsertForResults_ListOfFields_MatchesExpected()
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

            var expected = new List<Field>
                             {
                                 internalId,
                                 firstName,
                                 lastName,
                                 yearlyWage,
                                 hireDate,
                                 bonusTarget,
                                 contactNumbers
                             };
            
            var providerFactory = new SqlClientProviderFactory(new DataAnnotationsDefinitionProvider());
            var entityMapper = this.mapperFactory.Create();

            using (var provider = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
            {
                var transaction = provider.StartTransaction();
                var fieldRepository = new SqlClientRepository<Field, FieldRow>(provider, entityMapper);

                var fieldSelection = Select.From<FieldRow>().WhereEqual(row => row.Name, "INS_%");
                var fieldValuesToDelete = provider.DynamicSelect(fieldSelection.Select(row => row.FieldId));
                provider.Delete(Select.From<FieldValueRow>().Include(row => row.FieldId, fieldValuesToDelete.Select(o => (int)o.FieldId).ToArray()));
                fieldRepository.Delete(fieldSelection);

                var fieldRows = from f in expected
                                select new FieldTableTypeRow
                                           {
                                               Name = f.Name,
                                               Description = f.Description
                                           };

                // Select results = by name because the TVP won't have the IDs
                // TODO: See if we can simplify this process to just pull inserted items without joins
                var results = fieldRepository.InsertForResults(
                        fieldRows,
                        transaction,
                        insert => insert.SelectResults(row => row.Name))
                    .ToList();
                
                transaction.Commit();

                var actual = entityMapper.Map<List<Field>>(results);
                CollectionAssert.AreEquivalent(expected, actual);
            }
        }

        /// <summary>
        /// The execute test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void Insert_InsertIntoFromFieldTranslation_DoesNotThrowException()
        {
            var mapper = this.mapperFactory.Create();
            var providerFactory = new SqlClientProviderFactory(new DataAnnotationsDefinitionProvider());

            using (var provider = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
            {
                var identityRepository = new SqlClientRepository<DomainIdentity, DomainIdentityRow>(provider, mapper);
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

                var submissionFields = expected.SubmissionValues.Select(value => value.Field)
                    .Distinct()
                    .ToDictionary(field => field.Name, field => field);
                var inclusionValues = submissionFields.Keys.ToArray();

                var fieldRepository = new SqlClientRepository<Field, FieldRow>(provider, mapper);
                var fieldSelection = new EntitySelection<Field>().Include(field => field.Name, inclusionValues);
                var existingFields = fieldRepository.SelectEntities(fieldSelection).ToList();

                foreach (var field in existingFields)
                {
                    mapper.MapTo(field, submissionFields[field.Name]);
                }

                var missingFields = submissionFields.Values.Except(existingFields).ToList();
                var fieldRows = from f in missingFields
                                select new FieldTableTypeRow
                                           {
                                               Name = f.Name,
                                               Description = f.Description
                                           };

                var insertedFields = fieldRepository.InsertForResults(fieldRows, null, insert => insert.SelectResults(row => row.Name)).ToList();

                foreach (var field in insertedFields)
                {
                    mapper.MapTo(field, submissionFields[field.Name]);
                }

                var submissionRepository = new SqlClientRepository<GenericSubmission, GenericSubmissionRow>(provider, mapper);

                var transaction = provider.StartTransaction();

                submissionRepository.Save(expected);

                var submissionId = expected.GenericSubmissionId.GetValueOrDefault();
                Assert.AreNotEqual(0, submissionId);

                // Do the field values
                var valuesList = from v in expected.SubmissionValues
                                 select new FieldValueTableTypeRow
                                            {
                                                FieldId = v.Field.FieldId.GetValueOrDefault(),
                                                LastModifiedByDomainIdentifierId = domainIdentity.DomainIdentityId.GetValueOrDefault(),
                                                LastModifiedTime = expected.SubmittedTime
                                            };

                var fieldValueRepository = new SqlClientRepository<FieldValue, FieldValueRow>(provider, mapper);

                // One field value per field.
                var fieldValues = fieldValueRepository.InsertForResults(valuesList, transaction, insert => insert.SelectResults(row => row.FieldId));

                // Get a dictionary of the values based on Field ID, which should have been updated from the previous operation.
                var submissionValues = expected.SubmissionValues.ToDictionary(value => value.Field.FieldId, value => value);

                // Map back to the domain object.
                foreach (var value in fieldValues)
                {
                    var output = submissionValues[value.FieldId];
                    mapper.MapTo(value, output);
                }

                // Attach the values to the submission
                var genericValueSubmissions = from v in submissionValues.Values
                                              select new GenericSubmissionValueTableTypeRow
                                                         {
                                                             GenericSubmissionId = submissionId,
                                                             GenericSubmissionValueId = v.FieldValueId.GetValueOrDefault()
                                                         };

                var submissionValuesRepository = new SqlClientRepository<FieldValue, GenericSubmissionValueRow>(provider, mapper);
                submissionValuesRepository.Insert(genericValueSubmissions, transaction, null);

                // Do the field value elements
                var valueElements = expected.SubmissionValues.SelectMany(value => value.Elements)
                    .ToDictionary(
                        element => new Tuple<long, int>(element.FieldValue.FieldValueId.GetValueOrDefault(), element.Order),
                        element => element);

                var elementsList = (from e in valueElements.Values
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

                var fieldValueElementRepository = new SqlClientRepository<FieldValueElement, FieldValueElementRow>(provider, mapper);
                var fieldValueElements = fieldValueElementRepository.InsertForResults(
                        elementsList,
                        transaction,
                        insert => insert.SelectResults(row => row.FieldValueId, row => row.Order))
                    .ToDictionary(row => new Tuple<long, int>(row.FieldValueId, row.Order), row => row);

                // Assign FieldValueElementId back to the elements list and map to the domain models.
                foreach (var element in elementsList)
                {
                    var input = fieldValueElements[new Tuple<long, int>(element.FieldValueId, element.Order)];
                    element.FieldValueElementId = input.FieldValueElementId;
                    mapper.MapTo(input, valueElements[new Tuple<long, int>(element.FieldValueId, element.Order)]);
                }

                Assert.IsTrue(valueElements.Values.All(element => element.FieldValueElementId > 0));

                var dateElementRepository = new SqlClientRepository<FieldValueElement, DateElementRow>(provider, mapper);
                dateElementRepository.Insert(
                    elementsList.Where(row => row.DateElement.HasValue),
                    transaction,
                    insert => insert.InsertInto(row => row.DateElementId, row => row.Value)
                        .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.DateElement));

                var floatElementRepository = new SqlClientRepository<FieldValueElement, FloatElementRow>(provider, mapper);
                floatElementRepository.Insert(
                    elementsList.Where(row => row.FloatElement.HasValue),
                    transaction,
                    insert => insert.InsertInto(row => row.FloatElementId, row => row.Value)
                        .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.FloatElement));

                var integerElementRepository = new SqlClientRepository<FieldValueElement, IntegerElementRow>(provider, mapper);
                integerElementRepository.Insert(
                    elementsList.Where(row => row.IntegerElement.HasValue),
                    transaction,
                    insert => insert.InsertInto(row => row.IntegerElementId, row => row.Value)
                        .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.IntegerElement));

                var moneyElementRepository = new SqlClientRepository<FieldValueElement, MoneyElementRow>(provider, mapper);
                moneyElementRepository.Insert(
                    elementsList.Where(row => row.MoneyElement.HasValue),
                    transaction,
                    insert => insert.InsertInto(row => row.MoneyElementId, row => row.Value)
                        .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.MoneyElement));

                var textElementRepository = new SqlClientRepository<FieldValueElement, TextElementRow>(provider, mapper);
                textElementRepository.Insert(
                    elementsList.Where(row => row.TextElement != null),
                    transaction,
                    insert => insert.InsertInto(row => row.TextElementId, row => row.Value)
                        .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.TextElement));

                transaction.Commit();
            }
        }

        /// <summary>
        /// The merge field list does not throw exception.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void Merge_FieldList_DoesNotThrowException()
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
            var providerFactory = new SqlClientProviderFactory(new DataAnnotationsDefinitionProvider());

            using (var provider = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
            {
                var fieldRepository = new SqlClientRepository<Field, FieldRow>(provider, mapper);
                fieldRepository.Merge(
                    from f in fields
                    select new FieldTableTypeRow
                               {
                                   Name = f.Name,
                                   Description = f.Description
                               },
                    null,
                    merge => merge.On<FieldTableTypeRow>(row => row.Name, row => row.Name));
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
                
            var providerFactory = new SqlClientProviderFactory(new DataAnnotationsDefinitionProvider());

            GenericSubmission baselineSubmission;
            DomainIdentity domainIdentity2;

            var mapper = this.mapperFactory.Create();

            using (var provider = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
            {
                // Set up the domain identity, not part of our validity testing.
                var identityRepository = new EntityRepository<DomainIdentity, DomainIdentityRow>(provider, mapper);
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
            using (var provider = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
            {
                this.MergeSubmission(expected, provider, mapper);

                var submissionRepository = new EntityRepository<GenericSubmission, GenericSubmissionRow>(provider, mapper);
                actual = submissionRepository.FirstOrDefault(expected.GenericSubmissionId);

                var fieldValueRepository = new EntityRepository<FieldValue, GenericSubmissionValueRow>(provider, mapper);
                var values = fieldValueRepository.SelectEntities(
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

            Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
            CollectionAssert.AreEqual(
                expected.SubmissionValues.OrderBy(x => x.FieldValueId).ToList(),
                actual.SubmissionValues.OrderBy(x => x.FieldValueId).ToList());

            var expectedElements = expected.SubmissionValues.SelectMany(value => value.Elements).ToList();
            var actualElements = actual.SubmissionValues.SelectMany(value => value.Elements).ToList();
            CollectionAssert.AreEquivalent(expectedElements, actualElements);
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
        /// <param name="mapper">
        /// The mapper.
        /// </param>
        private void MergeSubmission(
            GenericSubmission submission,
            IRepositoryProvider provider,
            IEntityMapper mapper)
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
            var fieldRepository = new SqlClientRepository<Field, FieldRow>(provider, mapper);
            var transaction = provider.StartTransaction();

            var fieldRows = fieldRepository.MergeForResults(
                    fieldItems,
                    transaction,
                    merge => merge.OnImplicit(row => row.Name).SelectFromInserted(row => row.Name))
                .ToDictionary(row => row.Name, row => row);

            foreach (var field in fields)
            {
                fieldRows.TryGetValue(field.Name, out var input);

                // Because we are doing a subset, and we know we will get back baseline fields. If MERGE is messed up this will error later when there
                // aren't IDs for baseline fields.
                if (input == null)
                {
                    continue;
                }

                mapper.MapTo(input, field);
            }

            var submissionRepository = new EntityRepository<GenericSubmission, GenericSubmissionRow>(provider, mapper);
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
            var fieldValueRepository = new SqlClientRepository<FieldValue, FieldValueRow>(provider, mapper);
            var mergedFieldValues = fieldValueRepository.MergeForResults(
                    fieldValues,
                    transaction,
                    merge => merge.OnImplicit(row => row.FieldValueId).SelectFromInserted(row => row.FieldId))
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

            var valueElementRepository = new SqlClientRepository<FieldValueElement, FieldValueElementRow>(provider, mapper);
            var mergedValueElements = valueElementRepository.MergeForResults(
                    valueElements,
                    transaction,
                    merge => merge.OnImplicit(row => row.FieldValueId, row => row.Order)
                        ////.DeleteUnmatchedInSource<FieldValueElementTableTypeRow>(row => row.FieldValueId) // Get rid of extraneous elements
                        .SelectFromInserted(row => row.FieldValueId, row => row.Order)) // Generally this is the same as the MERGE INTO 
                .ToDictionary(row => new Tuple<long, int>(row.FieldValueId, row.Order), row => row);

            foreach (var element in valueElements)
            {
                var input = mergedValueElements[new Tuple<long, int>(element.FieldValueId, element.Order)];
                element.FieldValueElementId = input.FieldValueElementId;
            }

            var dateElementRepository = new SqlClientRepository<FieldValueElement, DateElementRow>(provider, mapper);
            dateElementRepository.MergeForResults(
                valueElements.Where(row => row.DateElement.HasValue),
                transaction,
                merge => merge.From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.DateElement)
                    .On<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.DateElementId));

            var floatElementRepository = new SqlClientRepository<FieldValueElement, FloatElementRow>(provider, mapper);
            floatElementRepository.MergeForResults(
                valueElements.Where(row => row.FloatElement.HasValue),
                transaction,
                merge => merge.From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.FloatElement)
                    .On<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.FloatElementId));

            var integerElementRepository = new SqlClientRepository<FieldValueElement, IntegerElementRow>(provider, mapper);
            integerElementRepository.MergeForResults(
                valueElements.Where(row => row.IntegerElement.HasValue),
                transaction,
                merge => merge.From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.IntegerElement)
                    .On<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.IntegerElementId));

            var moneyElementRepository = new SqlClientRepository<FieldValueElement, MoneyElementRow>(provider, mapper);
            moneyElementRepository.MergeForResults(
                valueElements.Where(row => row.MoneyElement.HasValue),
                transaction,
                merge => merge.From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.MoneyElement)
                    .On<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.MoneyElementId));

            var textElementRepository = new SqlClientRepository<FieldValueElement, TextElementRow>(provider, mapper);
            textElementRepository.MergeForResults(
                valueElements.Where(row => row.TextElement != null),
                transaction,
                merge => merge.From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.TextElement)
                    .On<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.TextElementId));

            // Attach the values to the submission
            var genericValueSubmissions = from v in mergedFieldValues.Values
                                          select new GenericSubmissionValueTableTypeRow
                                                     {
                                                         GenericSubmissionId = submission.GenericSubmissionId.GetValueOrDefault(),
                                                         GenericSubmissionValueId = v.FieldValueId.GetValueOrDefault()
                                                     };

            var submissionValueRepository = new SqlClientRepository<FieldValue, GenericSubmissionValueRow>(provider, mapper);
            submissionValueRepository.Merge(
                genericValueSubmissions,
                transaction,
                merge => merge.OnImplicit(row => row.GenericSubmissionValueId)
                    .DeleteUnmatchedInSource<GenericSubmissionValueTableTypeRow>(row => row.GenericSubmissionId));

            transaction.Commit();
        }
    }
}