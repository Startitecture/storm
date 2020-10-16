// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonInsertTests.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.PostgreSql.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using global::AutoMapper;

    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Startitecture.Core;
    using Startitecture.Orm.AutoMapper;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Schema;
    using Startitecture.Orm.Testing.Entities;
    using Startitecture.Orm.Testing.Entities.TableTypes;
    using Startitecture.Orm.Testing.Model;

    /// <summary>
    /// The json insert tests.
    /// </summary>
    [TestClass]
    public class JsonInsertTests
    {
        /// <summary>
        /// The entity mapper.
        /// </summary>
        private readonly IEntityMapper mapper = new AutoMapperEntityMapper(
            new Mapper(
                new MapperConfiguration(
                    expression =>
                        {
                            expression.AddProfile<GenericSubmissionMappingProfile>();
                        })));

        /// <summary>
        /// Gets the configuration root.
        /// </summary>
        private static IConfigurationRoot ConfigurationRoot => new ConfigurationBuilder().AddJsonFile("appSettings.json", false).Build();

        /// <summary>
        /// The command text test.
        /// </summary>
        [TestMethod]
        public void CommandText_InsertJsonWithReturn_MatchesSelected()
        {
            var databaseContext = new Mock<IDatabaseContext>();
            var mockProvider = new Mock<IRepositoryProvider>();
            mockProvider.Setup(provider => provider.DatabaseContext).Returns(databaseContext.Object);

            using (mockProvider.Object)
            {
                var structuredCommandProvider = new Mock<IDbTableCommandFactory>();
                var repositoryAdapter = new Mock<IRepositoryAdapter>();
                repositoryAdapter.Setup(adapter => adapter.DefinitionProvider).Returns(new DataAnnotationsDefinitionProvider());
                repositoryAdapter.Setup(adapter => adapter.NameQualifier).Returns(new PostgreSqlQualifier());
                databaseContext.Setup(context => context.RepositoryAdapter).Returns(repositoryAdapter.Object);
                ////structuredCommandProvider.Setup(provider => provider.DatabaseContext).Returns(databaseContext.Object);
                structuredCommandProvider
                    .Setup(
                        provider => provider.Create(
                            It.IsAny<ITableCommand>(),
                            It.IsAny<IEnumerable<FieldValueRow>>()))
                    .Returns(new Mock<IDbCommand>().Object);

                var valuesCommand = new JsonInsert<FieldValueRow>(structuredCommandProvider.Object, databaseContext.Object)
                    .Returning(row => row.FieldId);

                valuesCommand.Execute(new List<FieldValueRow>());

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
        /// The command text test.
        /// </summary>
        [TestMethod]
        public void CommandText_InsertJsonOnConflictUpdateWithReturn_MatchesSelected()
        {
            var databaseContext = new Mock<IDatabaseContext>();
            var mockProvider = new Mock<IRepositoryProvider>();
            mockProvider.Setup(provider => provider.DatabaseContext).Returns(databaseContext.Object);

            using (mockProvider.Object)
            {
                var structuredCommandProvider = new Mock<IDbTableCommandFactory>();
                var repositoryAdapter = new Mock<IRepositoryAdapter>();
                repositoryAdapter.Setup(adapter => adapter.DefinitionProvider).Returns(new DataAnnotationsDefinitionProvider());
                repositoryAdapter.Setup(adapter => adapter.NameQualifier).Returns(new PostgreSqlQualifier());
                databaseContext.Setup(context => context.RepositoryAdapter).Returns(repositoryAdapter.Object);
                ////structuredCommandProvider.Setup(provider => provider.DatabaseContext).Returns(databaseContext.Object);
                structuredCommandProvider
                    .Setup(
                        provider => provider.Create(
                            It.IsAny<ITableCommand>(),
                            It.IsAny<IEnumerable<FieldValueRow>>()))
                    .Returns(new Mock<IDbCommand>().Object);

                var valuesCommand = new JsonInsert<FieldValueRow>(structuredCommandProvider.Object, databaseContext.Object)
                    .Upsert(row => row.LastModifiedByDomainIdentifierId, row => row.LastModifiedTime)
                    .Returning(row => row.FieldId);

                const string Expected = @"INSERT INTO ""dbo"".""FieldValue""
(""FieldId"", ""LastModifiedByDomainIdentifierId"", ""LastModifiedTime"")
SELECT t.""FieldId"", t.""LastModifiedByDomainIdentifierId"", t.""LastModifiedTime""
FROM jsonb_to_recordset(@FieldValueRows::jsonb) AS t (""FieldId"" integer, ""LastModifiedByDomainIdentifierId"" integer, ""LastModifiedTime"" timestamp with time zone)
ON CONFLICT (""FieldValueId"")
DO UPDATE SET ""LastModifiedByDomainIdentifierId"" = EXCLUDED.""LastModifiedByDomainIdentifierId"", ""LastModifiedTime"" = EXCLUDED.""LastModifiedTime""
RETURNING ""FieldId"";
";

                valuesCommand.Execute(new List<FieldValueRow>());
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
            var databaseContext = new Mock<IDatabaseContext>();
            var mockProvider = new Mock<IRepositoryProvider>();
            mockProvider.Setup(provider => provider.DatabaseContext).Returns(databaseContext.Object);

            using (mockProvider.Object)
            {
                var structuredCommandProvider = new Mock<IDbTableCommandFactory>();
                var repositoryAdapter = new Mock<IRepositoryAdapter>();
                repositoryAdapter.Setup(adapter => adapter.DefinitionProvider).Returns(new DataAnnotationsDefinitionProvider());
                repositoryAdapter.Setup(adapter => adapter.NameQualifier).Returns(new PostgreSqlQualifier());
                databaseContext.Setup(context => context.RepositoryAdapter).Returns(repositoryAdapter.Object);
                ////structuredCommandProvider.Setup(provider => provider.DatabaseContext).Returns(databaseContext.Object);
                structuredCommandProvider
                    .Setup(
                        provider => provider.Create(
                            It.IsAny<ITableCommand>(),
                            It.IsAny<IEnumerable<GenericSubmissionValueRow>>()))
                    .Returns(new Mock<IDbCommand>().Object);

                var submissionCommand = new JsonInsert<GenericSubmissionValueRow>(structuredCommandProvider.Object, databaseContext.Object);
                submissionCommand.Execute(new List<GenericSubmissionValueRow>());

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
        public void CommandText_TableValueInsertForNonIdentityKeyOnConflictDoNothing_CommandTextMatchesExpected()
        {
            var databaseContext = new Mock<IDatabaseContext>();
            var mockProvider = new Mock<IRepositoryProvider>();
            mockProvider.Setup(provider => provider.DatabaseContext).Returns(databaseContext.Object);

            using (mockProvider.Object)
            {
                var structuredCommandProvider = new Mock<IDbTableCommandFactory>();
                var repositoryAdapter = new Mock<IRepositoryAdapter>();
                repositoryAdapter.Setup(adapter => adapter.DefinitionProvider).Returns(new DataAnnotationsDefinitionProvider());
                repositoryAdapter.Setup(adapter => adapter.NameQualifier).Returns(new PostgreSqlQualifier());
                databaseContext.Setup(context => context.RepositoryAdapter).Returns(repositoryAdapter.Object);
                ////structuredCommandProvider.Setup(provider => provider.DatabaseContext).Returns(databaseContext.Object);
                structuredCommandProvider
                    .Setup(
                        provider => provider.Create(
                            It.IsAny<ITableCommand>(),
                            It.IsAny<IEnumerable<GenericSubmissionValueRow>>()))
                    .Returns(new Mock<IDbCommand>().Object);

                var submissionCommand = new JsonInsert<GenericSubmissionValueRow>(structuredCommandProvider.Object, databaseContext.Object)
                    .OnConflictDoNothing();

                submissionCommand.Execute(new List<GenericSubmissionValueRow>());

                const string Expected = @"INSERT INTO ""dbo"".""GenericSubmissionValue""
(""GenericSubmissionValueId"", ""GenericSubmissionId"")
SELECT t.""GenericSubmissionValueId"", t.""GenericSubmissionId""
FROM jsonb_to_recordset(@GenericSubmissionValueRows::jsonb) AS t (""GenericSubmissionValueId"" bigint, ""GenericSubmissionId"" integer)
ON CONFLICT DO NOTHING;
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
            var databaseContext = new Mock<IDatabaseContext>();
            var mockProvider = new Mock<IRepositoryProvider>();
            mockProvider.Setup(provider => provider.DatabaseContext).Returns(databaseContext.Object);

            using (mockProvider.Object)
            {
                var structuredCommandProvider = new Mock<IDbTableCommandFactory>();
                var repositoryAdapter = new Mock<IRepositoryAdapter>();
                repositoryAdapter.Setup(adapter => adapter.DefinitionProvider).Returns(new DataAnnotationsDefinitionProvider());
                repositoryAdapter.Setup(adapter => adapter.NameQualifier).Returns(new PostgreSqlQualifier());
                databaseContext.Setup(context => context.RepositoryAdapter).Returns(repositoryAdapter.Object);
                ////structuredCommandProvider.Setup(provider => provider.DatabaseContext).Returns(databaseContext.Object);
                structuredCommandProvider
                    .Setup(
                        provider => provider.Create(
                            It.IsAny<ITableCommand>(),
                            It.IsAny<IEnumerable<FieldValueElementTableTypeRow>>()))
                    .Returns(new Mock<IDbCommand>().Object);

                var dateElementsCommand = new JsonInsert<DateElementRow>(structuredCommandProvider.Object, databaseContext.Object)
                    .InsertInto(row => row.DateElementId, row => row.Value)
                    .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.DateElement);

                dateElementsCommand.Execute(new List<FieldValueElementTableTypeRow>());

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
        public void Execute_InsertListOfFields_DoesNotThrowException()
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

            var providerFactory = new PostgreSqlProviderFactory(new DataAnnotationsDefinitionProvider());

            using (var provider = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDbPg")))
            {
                var fieldRepository = new EntityRepository<Field, FieldRow>(provider, this.mapper);

                // Delete the existing rows.
                fieldRepository.DeleteSelection(Query.From<FieldRow>().Where(set => set.AreEqual(row => row.Name, "INS_%")));

                using (var transaction = provider.BeginTransaction())
                {
                    // Set up the structured command provider.
                    var structuredCommandProvider = new JsonCommandFactory(provider.DatabaseContext);
                    var fieldInsertCommand = new JsonInsert<FieldRow>(structuredCommandProvider, provider.DatabaseContext);

                    fieldInsertCommand.Execute(
                        fields.Select(
                            field => new FieldRow
                            {
                                Name = field.Name,
                                Description = field.Description
                            }));

                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// The execute test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void ExecuteForResults_ListOfFields_DoesNotThrowException()
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

            var providerFactory = new PostgreSqlProviderFactory(new DataAnnotationsDefinitionProvider());

            using (var provider = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDbPg")))
            {
                var fieldRepository = new EntityRepository<Field, FieldRow>(provider, this.mapper);

                // Delete the existing rows.
                fieldRepository.DeleteSelection(Query.From<FieldRow>().Where(set => set.AreEqual(row => row.Name, "INS_%")));

                var transaction = provider.BeginTransaction();

                // Set up the structured command provider.
                var structuredCommandProvider = new JsonCommandFactory(provider.DatabaseContext);
                var fieldInsertCommand =
                    new JsonInsert<FieldRow>(structuredCommandProvider, provider.DatabaseContext).Returning(
                        row => row.FieldId,
                        row => row.Description);

                var actual = fieldInsertCommand.ExecuteForResults(
                    fields.Select(
                        field => new FieldRow
                        {
                            Name = field.Name,
                            Description = field.Description
                        }));

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
        public void ExecuteForResults_GenericSubmission_DoesNotThrowException()
        {
            var providerFactory = new PostgreSqlProviderFactory(new DataAnnotationsDefinitionProvider());

            using (var provider = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDbPg")))
            {
                var identityRepository = new EntityRepository<DomainIdentity, DomainIdentityRow>(provider, this.mapper);

                var domainIdentity = identityRepository.FirstOrDefault(
                                         Query.Select<DomainIdentity>()
                                             .Where(set => set.AreEqual(identity => identity.UniqueIdentifier, Environment.UserName)))
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

                var fieldRepository = new EntityRepository<Field, FieldRow>(provider, this.mapper);

                var fields = expected.SubmissionValues.Select(value => value.Field).Distinct().ToDictionary(field => field.Name, field => field);
                var inclusionValues = fields.Keys.ToArray();
                var existingFields =
                    fieldRepository.SelectEntities(new EntitySelection<Field>().Where(set => set.Include(field => field.Name, inclusionValues)));

                foreach (var field in existingFields)
                {
                    var output = fields[field.Name];
                    this.mapper.MapTo(field, output);
                }

                foreach (var field in fields.Values.Where(field => field.FieldId.HasValue == false))
                {
                    fieldRepository.Save(field);
                }

                var submissionRepository = new EntityRepository<GenericSubmission, GenericSubmissionRow>(provider, this.mapper);

                var transaction = provider.BeginTransaction();
                submissionRepository.Save(expected);

                var submissionId = expected.GenericSubmissionId.GetValueOrDefault();
                Assert.AreNotEqual(0, submissionId);

                // Set up the structured command provider.
                var structuredCommandProvider = new JsonCommandFactory(provider.DatabaseContext);

                // Do the field values
                var valuesList = from v in expected.SubmissionValues
                                 select new FieldValueRow
                                 {
                                     FieldId = v.Field.FieldId.GetValueOrDefault(),
                                     LastModifiedByDomainIdentifierId = domainIdentity.DomainIdentityId.GetValueOrDefault(),
                                     LastModifiedTime = expected.SubmittedTime
                                 };

                var valuesCommand =
                    new JsonInsert<FieldValueRow>(structuredCommandProvider, provider.DatabaseContext)
                        .Returning(row => row.FieldValueId, row => row.FieldId);

                var insertedValues = valuesCommand.ExecuteForResults(valuesList).ToList();

                // Map back to the domain object.
                foreach (var value in expected.SubmissionValues)
                {
                    var input = insertedValues.FirstOrDefault(row => row.FieldId == value.Field.FieldId);
                    this.mapper.MapTo(input, value);
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

                var elementsCommand = new JsonInsert<FieldValueElementTableTypeRow>(structuredCommandProvider, provider.DatabaseContext)
                    .Returning(row => row.FieldValueId, row => row.Order);

                // Reassign with our added identities
                elementsList = elementsCommand.ExecuteForResults(elementsList).ToList();

                foreach (var element in expected.SubmissionValues.SelectMany(value => value.Elements))
                {
                    var input = elementsList.First(row => row.FieldValueId == element.FieldValue.FieldValueId && row.Order == element.Order);
                    this.mapper.MapTo(input, element);
                }

                var dateElementsCommand = new JsonInsert<DateElementRow>(structuredCommandProvider, provider.DatabaseContext)
                    .InsertInto(row => row.DateElementId, row => row.Value)
                    .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.DateElement);

                dateElementsCommand.Execute(elementsList.Where(row => row.DateElement.HasValue));

                var floatElementsCommand = new JsonInsert<FloatElementRow>(structuredCommandProvider, provider.DatabaseContext)
                    .InsertInto(row => row.FloatElementId, row => row.Value)
                    .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.FloatElement);

                floatElementsCommand.Execute(elementsList.Where(row => row.FloatElement.HasValue));

                var integerElementsCommand = new JsonInsert<IntegerElementRow>(structuredCommandProvider, provider.DatabaseContext)
                    .InsertInto(row => row.IntegerElementId, row => row.Value)
                    .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.IntegerElement);

                integerElementsCommand.Execute(elementsList.Where(row => row.IntegerElement.HasValue));

                var moneyElementsCommand = new JsonInsert<MoneyElementRow>(structuredCommandProvider, provider.DatabaseContext)
                    .InsertInto(row => row.MoneyElementId, row => row.Value)
                    .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.MoneyElement);

                moneyElementsCommand.Execute(elementsList.Where(row => row.MoneyElement.HasValue));

                var textElementsCommand = new JsonInsert<TextElementRow>(structuredCommandProvider, provider.DatabaseContext)
                    .InsertInto(row => row.TextElementId, row => row.Value)
                    .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.TextElement);

                textElementsCommand.Execute(elementsList.Where(row => row.TextElement != null));

                // Attach the values to the submission
                var genericValueSubmissions = from v in insertedValues
                                              select new GenericSubmissionValueRow
                                              {
                                                  GenericSubmissionId = submissionId,
                                                  GenericSubmissionValueId = v.FieldValueId
                                              };

                var submissionCommand = new JsonInsert<GenericSubmissionValueRow>(structuredCommandProvider, provider.DatabaseContext);
                submissionCommand.Execute(genericValueSubmissions);
                transaction.Commit();
            }
        }

        /// <summary>
        /// The execute for results generic submission database matches expected.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void ExecuteForResults_MultipleGenericSubmissions_MatchesExpected()
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

            using (var provider = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDbPg")))
            {
                // Set up the domain identity, not part of our validity testing.
                var identityRepository = new EntityRepository<DomainIdentity, DomainIdentityRow>(provider, this.mapper);
                var domainIdentity = identityRepository.FirstOrDefault(
                                         Query.Select<DomainIdentity>()
                                             .Where(set => set.AreEqual(identity => identity.UniqueIdentifier, Environment.UserName)))
                                     ?? identityRepository.Save(
                                         new DomainIdentity(Environment.UserName)
                                         {
                                             FirstName = "King",
                                             MiddleName = "T.",
                                             LastName = "Animal"
                                         });

                var domainIdentifier2 = $"{Environment.UserName}2";
                domainIdentity2 = identityRepository.FirstOrDefault(
                                      Query.Select<DomainIdentity>()
                                          .Where(set => set.AreEqual(identity => identity.UniqueIdentifier, domainIdentifier2)))
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
                var fieldValueRepository = new EntityRepository<FieldValue, FieldValueRow>(provider, this.mapper);

                // Get rid of all the previous fields.
                fieldValueRepository.DeleteSelection(
                    Query.From<FieldValueRow>()
                        .Where(
                            set => set.Include(
                                row => row.FieldValueId,
                                baselineSubmission.SubmissionValues.Select(value => value.FieldValueId).ToArray())));

                this.MergeSubmission(expected, provider);

                var submissionRepository = new EntityRepository<GenericSubmission, GenericSubmissionRow>(provider, this.mapper);
                actual = submissionRepository.FirstOrDefault(expected.GenericSubmissionId);

                var genericSubmissionValueRepository = new EntityRepository<FieldValue, GenericSubmissionValueRow>(provider, this.mapper);
                var values = genericSubmissionValueRepository.SelectEntities(
                        Query.Select<GenericSubmissionValueRow>()
                            .From(
                                set => set.InnerJoin(row => row.GenericSubmissionValueId, row => row.FieldValue.FieldValueId)
                                    .InnerJoin(row => row.FieldValue.FieldId, row => row.FieldValue.Field.FieldId)
                                    .InnerJoin(
                                        row => row.FieldValue.LastModifiedByDomainIdentifierId,
                                        row => row.FieldValue.LastModifiedBy.DomainIdentityId))
                            .Where(set => set.AreEqual(row => row.GenericSubmissionId, expected.GenericSubmissionId.GetValueOrDefault())))
                    .ToDictionary(value => value.FieldValueId.GetValueOrDefault(), value => value);

                actual.Load(values.Values);

                var valueElementRows = provider.SelectEntities(
                        Query.Select<FieldValueElementTableTypeRow>()
                            .From(
                                set => set.LeftJoin<DateElementRow>(row => row.FieldValueElementId, row => row.DateElementId)
                                    .LeftJoin<FloatElementRow>(row => row.FieldValueElementId, row => row.FloatElementId)
                                    .LeftJoin<IntegerElementRow>(row => row.FieldValueElementId, row => row.IntegerElementId)
                                    .LeftJoin<MoneyElementRow>(row => row.FieldValueElementId, row => row.MoneyElementId)
                                    .LeftJoin<TextElementRow>(row => row.FieldValueElementId, row => row.TextElementId))
                            .Where(set => set.Include(row => row.FieldValueId, values.Keys.ToArray())))
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
        /// Merges the specified <paramref name="submission"/> into the repository backed by the <paramref name="provider"/>.
        /// </summary>
        /// <param name="submission">
        /// The submission.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        private void MergeSubmission(GenericSubmission submission, IRepositoryProvider provider)
        {
            // Merge our existing fields
            var fields = submission.SubmissionValues.Select(value => value.Field).Distinct().ToList();
            var fieldItems = from f in fields
                             select new FieldRow
                             {
                                 Name = f.Name,
                                 Description = f.Description
                             };

            // Merge in the field values.
            var commandProvider = new JsonCommandFactory(provider.DatabaseContext);
            var transaction = provider.BeginTransaction();

            var fieldsCommand = new JsonInsert<FieldRow>(commandProvider, provider.DatabaseContext).OnConflict(row => row.Name)
                .Upsert(row => row.Description)
                .Returning(row => row.FieldId, row => row.Name, row => row.Description);

            var mergedFields = fieldsCommand.ExecuteForResults(fieldItems).ToList();

            foreach (var field in fields)
            {
                var input = mergedFields.FirstOrDefault(f => string.Equals(f.Name, field.Name, StringComparison.Ordinal));

                // Because we are doing a subset, and we know we will get back baseline fields. If MERGE is messed up this will error later when there
                // aren't IDs for baseline fields.
                if (input == null)
                {
                    continue;
                }

                this.mapper.MapTo(input, field);
            }

            var submissionRepository = new EntityRepository<GenericSubmission, GenericSubmissionRow>(provider, this.mapper);
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
            var fieldValueCommand = new JsonInsert<FieldValueRow>(commandProvider, provider.DatabaseContext);
            var mergedFieldValues = fieldValueCommand.OnConflict(row => row.FieldValueId)
                .Upsert(row => row.LastModifiedByDomainIdentifierId, row => row.LastModifiedTime)
                .Returning(row => row.FieldValueId, row => row.FieldId, row => row.LastModifiedByDomainIdentifierId, row => row.LastModifiedTime)
                .ExecuteForResults(fieldValues)
                .ToList();

            Assert.IsTrue(mergedFieldValues.All(row => row.FieldValueId > 0));

            // Map back to the domain object.
            foreach (var value in submission.SubmissionValues)
            {
                var input = mergedFieldValues.First(row => row.FieldId == value.Field.FieldId);
                this.mapper.MapTo(input, value);
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

            var elementMergeCommand = new JsonInsert<FieldValueElementRow>(commandProvider, provider.DatabaseContext);
            var mergedValueElements = elementMergeCommand.OnConflict(row => row.FieldValueElementId)
                .Upsert(row => row.Order)
                .Returning(row => row.FieldValueElementId, row => row.FieldValueId, row => row.Order)
                .ExecuteForResults(valueElements)
                .ToList();

            foreach (var element in valueElements)
            {
                var input = mergedValueElements.First(row => row.FieldValueId == element.FieldValueId && row.Order == element.Order);
                this.mapper.MapTo(input, element);
                Assert.IsTrue(element.FieldValueElementId.HasValue);
            }

            var dateElementsCommand = new JsonInsert<DateElementRow>(commandProvider, provider.DatabaseContext)
                .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.DateElement)
                .OnConflict(row => row.DateElementId)
                .Upsert(row => row.Value);

            dateElementsCommand.Execute(valueElements.Where(row => row.DateElement.HasValue));

            var floatElementsCommand = new JsonInsert<FloatElementRow>(commandProvider, provider.DatabaseContext)
                .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.FloatElement)
                .OnConflict(row => row.FloatElementId)
                .Upsert(row => row.Value);

            floatElementsCommand.Execute(valueElements.Where(row => row.FloatElement.HasValue));

            var integerElementsCommand = new JsonInsert<IntegerElementRow>(commandProvider, provider.DatabaseContext)
                .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.IntegerElement)
                .OnConflict(row => row.IntegerElementId)
                .Upsert(row => row.Value);

            integerElementsCommand.Execute(valueElements.Where(row => row.IntegerElement.HasValue));

            var moneyElementsCommand = new JsonInsert<MoneyElementRow>(commandProvider, provider.DatabaseContext)
                .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.MoneyElement)
                .OnConflict(row => row.MoneyElementId)
                .Upsert(row => row.Value);

            moneyElementsCommand.Execute(valueElements.Where(row => row.MoneyElement.HasValue));

            var textElementsCommand = new JsonInsert<TextElementRow>(commandProvider, provider.DatabaseContext)
                .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.TextElement)
                .OnConflict(row => row.TextElementId)
                .Upsert(row => row.Value);

            textElementsCommand.Execute(valueElements.Where(row => row.TextElement != null));

            // Attach the values to the submission
            var genericValueSubmissions = from v in mergedFieldValues
                                          select new GenericSubmissionValueTableTypeRow
                                          {
                                              GenericSubmissionId = submission.GenericSubmissionId.GetValueOrDefault(),
                                              GenericSubmissionValueId = v.FieldValueId
                                          };

            var submissionCommand = new JsonInsert<GenericSubmissionValueRow>(commandProvider, provider.DatabaseContext).Upsert(
                row => row.GenericSubmissionValueId,
                row => row.GenericSubmissionId);
            ////.DeleteUnmatchedInSource(row => row.GenericSubmissionId); Can't with PostgreSQL

            submissionCommand.Execute(genericValueSubmissions);
            transaction.Commit();
        }
    }
}