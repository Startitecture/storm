// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonInsertTests.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.PostgreSql.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::AutoMapper;

    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Startitecture.Orm.AutoMapper;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Schema;
    using Startitecture.Orm.Testing.Entities;
    using Startitecture.Orm.Testing.Entities.TableTypes;
    using Startitecture.Orm.Testing.Model;
    using Startitecture.Orm.Testing.Moq;

    /// <summary>
    /// The json insert tests.
    /// </summary>
    [TestClass]
    public class JsonInsertTests
    {
        /// <summary>
        /// The entity mapper.
        /// </summary>
        private readonly IEntityMapperFactory mapperFactory = new EntityMapperFactory(
            new MapperConfiguration(
                expression =>
                    {
                        expression.AddProfile<GenericSubmissionMappingProfile>();
                    }));

        /// <summary>
        /// The configuration root.
        /// </summary>
        private static IConfigurationRoot ConfigurationRoot => new ConfigurationBuilder().AddJsonFile("appSettings.json", false).Build();

        /// <summary>
        /// The command text test.
        /// </summary>
        [TestMethod]
        public void CommandText_InsertJsonWithReturn_MatchesSelected()
        {
            var mockProvider = new Mock<IRepositoryProvider>();

            using (mockProvider.Object)
            {
                var structuredCommandProvider = new Mock<IStructuredCommandProvider>();
                var databaseContext = new Mock<IDatabaseContext>();
                var repositoryAdapter = new Mock<IRepositoryAdapter>();
                repositoryAdapter.Setup(adapter => adapter.DefinitionProvider).Returns(new DataAnnotationsDefinitionProvider());
                repositoryAdapter.Setup(adapter => adapter.NameQualifier).Returns(new PostgreSqlQualifier());
                databaseContext.Setup(context => context.RepositoryAdapter).Returns(repositoryAdapter.Object);
                structuredCommandProvider.Setup(provider => provider.DatabaseContext).Returns(databaseContext.Object);

                var valuesCommand = new JsonInsert<FieldValueRow>(structuredCommandProvider.Object)
                    .InsertInto<FieldValueRow>(new List<FieldValueRow>())
                    .SelectResults(row => row.FieldId);

                const string Expected = @"INSERT INTO ""dbo"".""FieldValue""
(""FieldId"", ""LastModifiedByDomainIdentifierId"", ""LastModifiedTime"")
SELECT t.""FieldId"", t.""LastModifiedByDomainIdentifierId"", t.""LastModifiedTime""
FROM jsonb_to_recordset(@FieldValueRows::jsonb) AS t (""FieldId"" integer, ""LastModifiedByDomainIdentifierId"" integer, ""LastModifiedTime"" timestamp with time zone)
RETURNING ""FieldId"";
";

                var actual = valuesCommand.CommandText;
                Assert.AreEqual(Expected, actual);
            }
        }

        /// <summary>
        /// The execute test.
        /// </summary>
        [TestMethod]
        public void CommandText_TableValueInsertForNonIdentityKey_CommandTextMatchesExpected()
        {
            var mockProvider = new Mock<IRepositoryProvider>();

            using (mockProvider.Object)
            {
                // Attach the values to the submission
                var genericValueSubmissions = (from v in Enumerable.Range(1, 5)
                                               select new GenericSubmissionValueRow
                                                          {
                                                              GenericSubmissionId = 6,
                                                              GenericSubmissionValueId = v
                                                          }).ToList();

                var structuredCommandProvider = genericValueSubmissions.MockCommandProvider(
                    new DataAnnotationsDefinitionProvider(),
                    new PostgreSqlQualifier());

                var submissionCommand = new JsonInsert<GenericSubmissionValueRow>(structuredCommandProvider.Object)
                    .InsertInto<GenericSubmissionValueRow>(genericValueSubmissions);

                const string Expected = @"INSERT INTO ""dbo"".""GenericSubmissionValue""
(""GenericSubmissionValueId"", ""GenericSubmissionId"")
SELECT t.""GenericSubmissionValueId"", t.""GenericSubmissionId""
FROM jsonb_to_recordset(@GenericSubmissionValueRows::jsonb) AS t (""GenericSubmissionValueId"" bigint, ""GenericSubmissionId"" integer);
";
                var actual = submissionCommand.CommandText;
                Assert.AreEqual(Expected, actual);
            }
        }

        /// <summary>
        /// The execute test.
        /// </summary>
        [TestMethod]
        public void CommandText_TableValuedInsertForFlattenedType_MatchesExpected()
        {
            var mockProvider = new Mock<IRepositoryProvider>();

            using (mockProvider.Object)
            {
                var structuredCommandProvider = new Mock<IStructuredCommandProvider>();
                var databaseContext = new Mock<IDatabaseContext>();
                var repositoryAdapter = new Mock<IRepositoryAdapter>();
                repositoryAdapter.Setup(adapter => adapter.DefinitionProvider).Returns(new DataAnnotationsDefinitionProvider());
                repositoryAdapter.Setup(adapter => adapter.NameQualifier).Returns(new PostgreSqlQualifier());
                databaseContext.Setup(context => context.RepositoryAdapter).Returns(repositoryAdapter.Object);
                structuredCommandProvider.Setup(provider => provider.DatabaseContext).Returns(databaseContext.Object);

                var dateElementsCommand = new JsonInsert<FieldValueElementTableTypeRow>(structuredCommandProvider.Object)
                    .InsertInto<DateElementRow>(new List<FieldValueElementTableTypeRow>(), row => row.DateElementId, row => row.Value)
                    .From(row => row.FieldValueElementId, row => row.DateElement);

                const string Expected = @"INSERT INTO ""dbo"".""DateElement""
(""DateElementId"", ""Value"")
SELECT t.""FieldValueElementId"", t.""DateElement""
FROM jsonb_to_recordset(@FieldValueElementRows::jsonb) AS t (""FieldValueElementId"" bigint, ""DateElement"" timestamp with time zone);
";
                var actual = dateElementsCommand.CommandText;
                Assert.AreEqual(Expected, actual);
            }
        }

        /// <summary>
        /// The execute test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void Execute_TableValueInsertForFields_MatchesExpected()
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
            var providerFactory = new PostgreSqlProviderFactory(ConfigurationRoot.GetConnectionString("OrmTestDbPg"), new DataAnnotationsDefinitionProvider());

            using (var provider = providerFactory.Create())
            {
                var fieldRepository = new EntityRepository<Field, FieldRow>(provider, mapper);

                // Delete the existing rows.
                fieldRepository.Delete(Select.From<FieldRow>().WhereEqual(row => row.Name, "INS_%"));

                var transaction = provider.StartTransaction();

                // Set up the structured command provider.
                var databaseContextProvider = (IDatabaseContextProvider)provider;
                var structuredCommandProvider = new JsonCommandProvider(databaseContextProvider.DatabaseContext);
                var fieldInsertCommand = new JsonInsert<FieldRow>(structuredCommandProvider, transaction).InsertInto<FieldRow>(
                    fields.Select(
                        field => new FieldRow
                                     {
                                         Name = field.Name,
                                         Description = field.Description
                                     }));

                fieldInsertCommand.Execute();
                transaction.Commit();
            }
        }

        /// <summary>
        /// The execute test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void Execute_TableValueInsertForFieldsWithReturn_MatchesExpected()
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
            var providerFactory = new PostgreSqlProviderFactory(ConfigurationRoot.GetConnectionString("OrmTestDbPg"), new DataAnnotationsDefinitionProvider());

            using (var provider = providerFactory.Create())
            {
                var fieldRepository = new EntityRepository<Field, FieldRow>(provider, mapper);

                // Delete the existing rows.
                fieldRepository.Delete(Select.From<FieldRow>().WhereEqual(row => row.Name, "INS_%"));

                var transaction = provider.StartTransaction();

                // Set up the structured command provider.
                var databaseContextProvider = (IDatabaseContextProvider)provider;
                var structuredCommandProvider = new JsonCommandProvider(databaseContextProvider.DatabaseContext);
                var fieldInsertCommand = new JsonInsert<FieldRow>(structuredCommandProvider, transaction).InsertInto<FieldRow>(
                    fields.Select(
                        field => new FieldRow
                                     {
                                         Name = field.Name,
                                         Description = field.Description
                                     })).SelectResults(row => row.FieldId, row => row.Description);

                var actual = fieldInsertCommand.ExecuteForResults();

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
        public void ExecuteWithIdentityUpdate_TableValuedInsertForGenericSubmission_DoesNotThrowException()
        {
            var mapper = this.mapperFactory.Create();
            var providerFactory = new PostgreSqlProviderFactory(
                ConfigurationRoot.GetConnectionString("OrmTestDbPg"),
                new DataAnnotationsDefinitionProvider());

            using (var provider = providerFactory.Create())
            {
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
                expected.SetValue(contactNumbers, new List<string> { "423-222-2252", "615-982-0012", "+1-555-252-5521" });

                expected.Submit();

                var fieldRepository = new EntityRepository<Field, FieldRow>(provider, mapper);

                // TODO: Return names only from the repo as a dynamic
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

                var submissionRepository = new EntityRepository<GenericSubmission, GenericSubmissionRow>(provider, mapper);

                var transaction = provider.StartTransaction();
                submissionRepository.Save(expected);

                var submissionId = expected.GenericSubmissionId.GetValueOrDefault();
                Assert.AreNotEqual(0, submissionId);

                // Set up the structured command provider.
                var databaseContextProvider = (IDatabaseContextProvider)provider;
                var structuredCommandProvider = new JsonCommandProvider(databaseContextProvider.DatabaseContext);

                // Do the field values
                var valuesList = from v in expected.SubmissionValues
                                 select new FieldValueRow
                                            {
                                                FieldId = v.Field.FieldId.GetValueOrDefault(),
                                                LastModifiedByDomainIdentifierId = domainIdentity.DomainIdentityId.GetValueOrDefault(),
                                                LastModifiedTime = expected.SubmittedTime
                                            };

                var valuesCommand =
                    new JsonInsert<FieldValueRow>(structuredCommandProvider, transaction)
                        .InsertInto<FieldValueRow>(valuesList)
                        .SelectResults(row => row.FieldValueId, row => row.FieldId);

                var insertedValues = valuesCommand.ExecuteForResults().ToList();

                // Map back to the domain object.
                foreach (var value in expected.SubmissionValues)
                {
                    var input = insertedValues.FirstOrDefault(row => row.FieldId == value.Field.FieldId);
                    mapper.MapTo(input, value);
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
                                                   IntegerElement = e.Element as long? ?? e.Element as int? ?? e.Element as short? ?? e.Element as byte?,
                                                   MoneyElement = e.Element as decimal?,
                                                   TextElement = e.Element as string // here we actually want it to be null if it is not a string
                                               }).ToList();

                var elementsCommand = new JsonInsert<FieldValueElementTableTypeRow>(structuredCommandProvider, transaction)
                    .InsertInto<FieldValueElementRow>(elementsList)
                    .SelectResults(row => row.FieldValueId, row => row.Order);

                // Reassign with our added identities
                // TODO: create dictionary for seeks
                elementsList = elementsCommand.ExecuteForResults().ToList();

                foreach (var element in expected.SubmissionValues.SelectMany(value => value.Elements))
                {
                    var input = elementsList.First(row => row.FieldValueId == element.FieldValue.FieldValueId && row.Order == element.Order);
                    mapper.MapTo(input, element);
                }

                var dateElementsCommand = new JsonInsert<FieldValueElementTableTypeRow>(structuredCommandProvider, transaction)
                    .InsertInto<DateElementRow>(elementsList.Where(row => row.DateElement.HasValue), row => row.DateElementId, row => row.Value)
                    .From(row => row.FieldValueElementId, row => row.DateElement);

                dateElementsCommand.Execute();

                var floatElementsCommand = new JsonInsert<FieldValueElementTableTypeRow>(structuredCommandProvider, transaction)
                    .InsertInto<FloatElementRow>(elementsList.Where(row => row.FloatElement.HasValue), row => row.FloatElementId, row => row.Value)
                    .From(row => row.FieldValueElementId, row => row.FloatElement);

                floatElementsCommand.Execute();

                var integerElementsCommand = new JsonInsert<FieldValueElementTableTypeRow>(structuredCommandProvider, transaction)
                    .InsertInto<IntegerElementRow>(elementsList.Where(row => row.IntegerElement.HasValue), row => row.IntegerElementId, row => row.Value)
                    .From(row => row.FieldValueElementId, row => row.IntegerElement);

                integerElementsCommand.Execute();

                var moneyElementsCommand = new JsonInsert<FieldValueElementTableTypeRow>(structuredCommandProvider, transaction)
                    .InsertInto<MoneyElementRow>(elementsList.Where(row => row.MoneyElement.HasValue), row => row.MoneyElementId, row => row.Value)
                    .From(row => row.FieldValueElementId, row => row.MoneyElement);

                moneyElementsCommand.Execute();

                var textElementsCommand = new JsonInsert<FieldValueElementTableTypeRow>(structuredCommandProvider, transaction)
                    .InsertInto<TextElementRow>(elementsList.Where(row => row.TextElement != null), row => row.TextElementId, row => row.Value)
                    .From(row => row.FieldValueElementId, row => row.TextElement);

                textElementsCommand.Execute();

                // Attach the values to the submission
                var genericValueSubmissions = from v in insertedValues
                                              select new GenericSubmissionValueRow
                                                         {
                                                             GenericSubmissionId = submissionId,
                                                             GenericSubmissionValueId = v.FieldValueId
                                                         };

                var submissionCommand = new JsonInsert<GenericSubmissionValueRow>(structuredCommandProvider, transaction)
                    .InsertInto<GenericSubmissionValueRow>(genericValueSubmissions);

                submissionCommand.Execute();
                transaction.Commit();
            }
        }
    }
}