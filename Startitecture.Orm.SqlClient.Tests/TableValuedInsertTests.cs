// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableValuedInsertTests.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The structured insert command tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.SqlClient.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;

    using global::AutoMapper;

    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Startitecture.Orm.AutoMapper;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Schema;
    using Startitecture.Orm.SqlClient;
    using Startitecture.Orm.Testing.Entities;
    using Startitecture.Orm.Testing.Entities.TableTypes;
    using Startitecture.Orm.Testing.Model;
    using Startitecture.Orm.Testing.Moq;

    /// <summary>
    /// The structured insert command tests.
    /// </summary>
    [TestClass]
    public class TableValuedInsertTests
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
        private static IConfigurationRoot ConfigurationRoot => new ConfigurationBuilder().AddJsonFile("appsettings.json", false).Build();

        /// <summary>
        /// The execute test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void Execute_TableValueInsertForFields_DoesNotThrowException()
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

            using (var provider = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
            {
                var transaction = provider.BeginTransaction();

                // Set up the structured command provider.
                var structuredCommandProvider = new TableValuedParameterCommandFactory(provider.DatabaseContext);
                var fieldRepository = new EntityRepository<Field, FieldRow>(provider, this.mapper);
                var fieldValueRepository = new EntityRepository<FieldValue, FieldValueRow>(provider, this.mapper);

                // Delete the existing rows.
                var fieldSelection = Query.Select<FieldRow>().Where(set => set.AreEqual(row => row.Name, "INS_%"));
                var existingFields = fieldRepository.SelectEntities(fieldSelection);

                fieldValueRepository.DeleteSelection(
                    Query.Select<FieldValueRow>()
                        .Where(set => set.Include(row => row.FieldId, existingFields.Select(field => field.FieldId).ToArray())));

                fieldRepository.DeleteSelection(fieldSelection);
                var fieldInsertCommand = new TableValuedInsert<FieldRow>(structuredCommandProvider, provider.DatabaseContext);

                fieldInsertCommand.Execute(
                    fields.Select(
                        field => new FieldTableTypeRow
                        {
                            Name = field.Name,
                            Description = field.Description
                        }));

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
            var providerFactory = new SqlClientProviderFactory(new DataAnnotationsDefinitionProvider());

            using (var provider = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
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

                // TODO: Return names only from the repo as a dynamic
                var fields = expected.SubmissionValues.Select(value => value.Field).Distinct().ToDictionary(field => field.Name, field => field);
                var inclusionValues = fields.Keys.ToArray();
                var existingFields = fieldRepository.SelectEntities(
                    new EntitySelection<Field>().Where(set => set.Include(field => field.Name, inclusionValues)));

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
                var commandProvider = new TableValuedParameterCommandFactory(provider.DatabaseContext);

                // Do the field values
                var valuesList = from v in expected.SubmissionValues
                                 select new FieldValueTableTypeRow
                                 {
                                     FieldId = v.Field.FieldId.GetValueOrDefault(),
                                     LastModifiedByDomainIdentifierId = domainIdentity.DomainIdentityId.GetValueOrDefault(),
                                     LastModifiedTime = expected.SubmittedTime
                                 };

                var valuesCommand = new TableValuedInsert<FieldValueRow>(commandProvider, provider.DatabaseContext).SelectFromInserted();
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

                var elementsCommand = new TableValuedInsert<FieldValueElementRow>(commandProvider, provider.DatabaseContext).SelectFromInserted();

                // Reassign with our added identities
                // TODO: create dictionary for seeks
                var insertedElementRows = elementsCommand.ExecuteForResults(elementsList).ToList();

                foreach (var element in elementsList)
                {
                    var input = insertedElementRows.First(row => row.FieldValueId == element.FieldValueId && row.Order == element.Order);
                    this.mapper.MapTo(input, element);
                }

                var dateElementsCommand = new TableValuedInsert<DateElementRow>(commandProvider, provider.DatabaseContext)
                    .InsertInto(row => row.DateElementId, row => row.Value)
                    .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.DateElement);

                dateElementsCommand.Execute(elementsList.Where(row => row.DateElement.HasValue));

                var floatElementsCommand = new TableValuedInsert<FloatElementRow>(commandProvider, provider.DatabaseContext)
                    .InsertInto(row => row.FloatElementId, row => row.Value)
                    .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.FloatElement);

                floatElementsCommand.Execute(elementsList.Where(row => row.FloatElement.HasValue));

                var integerElementsCommand = new TableValuedInsert<IntegerElementRow>(commandProvider, provider.DatabaseContext)
                    .InsertInto(row => row.IntegerElementId, row => row.Value)
                    .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.IntegerElement);

                integerElementsCommand.Execute(elementsList.Where(row => row.IntegerElement.HasValue));

                var moneyElementsCommand = new TableValuedInsert<MoneyElementRow>(commandProvider, provider.DatabaseContext)
                    .InsertInto(row => row.MoneyElementId, row => row.Value)
                    .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.MoneyElement);

                moneyElementsCommand.Execute(elementsList.Where(row => row.MoneyElement.HasValue));

                var textElementsCommand = new TableValuedInsert<TextElementRow>(commandProvider, provider.DatabaseContext)
                    .InsertInto(row => row.TextElementId, row => row.Value)
                    .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.TextElement);

                textElementsCommand.Execute(elementsList.Where(row => row.TextElement != null));

                // Attach the values to the submission
                var genericValueSubmissions = from v in insertedValues
                                              select new GenericSubmissionValueTableTypeRow
                                              {
                                                  GenericSubmissionId = submissionId,
                                                  GenericSubmissionValueId = v.FieldValueId
                                              };

                var submissionCommand = new TableValuedInsert<GenericSubmissionValueTableTypeRow>(commandProvider, provider.DatabaseContext);
                submissionCommand.Execute(genericValueSubmissions);
                transaction.Commit();
            }
        }

        /// <summary>
        /// The execute test.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous unit test.
        /// </returns>
        [TestMethod]
        [TestCategory("Integration")]
        public async Task ExecuteAsync_TableValueInsertForFields_DoesNotThrowException()
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

            await using (var provider = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
            {
                var transaction = await provider.BeginTransactionAsync().ConfigureAwait(false);

                // Set up the structured command provider.
                var structuredCommandProvider = new TableValuedParameterCommandFactory(provider.DatabaseContext);
                var fieldRepository = new EntityRepository<Field, FieldRow>(provider, this.mapper);
                var fieldValueRepository = new EntityRepository<FieldValue, FieldValueRow>(provider, this.mapper);

                // Delete the existing rows.
                var fieldSelection = Query.Select<FieldRow>().Where(set => set.AreEqual(row => row.Name, "INS_%"));
                var existingFields = await fieldRepository.SelectEntitiesAsync(fieldSelection).ConfigureAwait(false);

                await fieldValueRepository.DeleteSelectionAsync(
                        Query.Select<FieldValueRow>()
                            .Where(set => set.Include(row => row.FieldId, existingFields.Select(field => field.FieldId).ToArray())))
                    .ConfigureAwait(false);

                await fieldRepository.DeleteSelectionAsync(fieldSelection).ConfigureAwait(false);
                var fieldInsertCommand = new TableValuedInsert<FieldRow>(structuredCommandProvider, provider.DatabaseContext);

                await fieldInsertCommand.ExecuteAsync(
                        fields.Select(
                            field => new FieldTableTypeRow
                            {
                                Name = field.Name,
                                Description = field.Description
                            }))
                    .ConfigureAwait(false);

                await transaction.CommitAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// The execute test.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous unit test.
        /// </returns>
        [TestMethod]
        [TestCategory("Integration")]
        public async Task ExecuteWithIdentityUpdateAsync_TableValuedInsertForGenericSubmission_DoesNotThrowException()
        {
            var providerFactory = new SqlClientProviderFactory(new DataAnnotationsDefinitionProvider());

            await using (var provider = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
            {
                var identityRepository = new EntityRepository<DomainIdentity, DomainIdentityRow>(provider, this.mapper);

                var domainIdentity = await identityRepository.FirstOrDefaultAsync(
                                         Query.Select<DomainIdentity>()
                                             .Where(set => set.AreEqual(identity => identity.UniqueIdentifier, Environment.UserName))).ConfigureAwait(false)
                                     ?? await identityRepository.SaveAsync(
                                         new DomainIdentity(Environment.UserName)
                                         {
                                             FirstName = "King",
                                             MiddleName = "T.",
                                             LastName = "Animal"
                                         }).ConfigureAwait(false);

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

                // TODO: Return names only from the repo as a dynamic
                var fields = expected.SubmissionValues.Select(value => value.Field).Distinct().ToDictionary(field => field.Name, field => field);
                var inclusionValues = fields.Keys.ToArray();
                var existingFields = await fieldRepository.SelectEntitiesAsync(
                                         new EntitySelection<Field>().Where(set => set.Include(field => field.Name, inclusionValues))).ConfigureAwait(false);

                foreach (var field in existingFields)
                {
                    var output = fields[field.Name];
                    this.mapper.MapTo(field, output);
                }

                foreach (var field in fields.Values.Where(field => field.FieldId.HasValue == false))
                {
                    await fieldRepository.SaveAsync(field).ConfigureAwait(false);
                }

                var submissionRepository = new EntityRepository<GenericSubmission, GenericSubmissionRow>(provider, this.mapper);

                var transaction = await provider.BeginTransactionAsync().ConfigureAwait(false);
                await submissionRepository.SaveAsync(expected).ConfigureAwait(false);

                var submissionId = expected.GenericSubmissionId.GetValueOrDefault();
                Assert.AreNotEqual(0, submissionId);

                // Set up the structured command provider.
                var commandProvider = new TableValuedParameterCommandFactory(provider.DatabaseContext);

                // Do the field values
                var valuesList = from v in expected.SubmissionValues
                                 select new FieldValueTableTypeRow
                                 {
                                     FieldId = v.Field.FieldId.GetValueOrDefault(),
                                     LastModifiedByDomainIdentifierId = domainIdentity.DomainIdentityId.GetValueOrDefault(),
                                     LastModifiedTime = expected.SubmittedTime
                                 };

                var valuesCommand = new TableValuedInsert<FieldValueRow>(commandProvider, provider.DatabaseContext).SelectFromInserted();
                var insertedValues = (await valuesCommand.ExecuteForResultsAsync(valuesList).ConfigureAwait(false)).ToList();

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

                var elementsCommand = new TableValuedInsert<FieldValueElementRow>(commandProvider, provider.DatabaseContext).SelectFromInserted();

                // Reassign with our added identities
                // TODO: create dictionary for seeks
                var insertedElementRows = (await elementsCommand.ExecuteForResultsAsync(elementsList).ConfigureAwait(false)).ToList();

                foreach (var element in elementsList)
                {
                    var input = insertedElementRows.First(row => row.FieldValueId == element.FieldValueId && row.Order == element.Order);
                    this.mapper.MapTo(input, element);
                }

                var dateElementsCommand = new TableValuedInsert<DateElementRow>(commandProvider, provider.DatabaseContext)
                    .InsertInto(row => row.DateElementId, row => row.Value)
                    .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.DateElement);

                await dateElementsCommand.ExecuteAsync(elementsList.Where(row => row.DateElement.HasValue)).ConfigureAwait(false);

                var floatElementsCommand = new TableValuedInsert<FloatElementRow>(commandProvider, provider.DatabaseContext)
                    .InsertInto(row => row.FloatElementId, row => row.Value)
                    .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.FloatElement);

                await floatElementsCommand.ExecuteAsync(elementsList.Where(row => row.FloatElement.HasValue)).ConfigureAwait(false);

                var integerElementsCommand = new TableValuedInsert<IntegerElementRow>(commandProvider, provider.DatabaseContext)
                    .InsertInto(row => row.IntegerElementId, row => row.Value)
                    .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.IntegerElement);

                await integerElementsCommand.ExecuteAsync(elementsList.Where(row => row.IntegerElement.HasValue)).ConfigureAwait(false);

                var moneyElementsCommand = new TableValuedInsert<MoneyElementRow>(commandProvider, provider.DatabaseContext)
                    .InsertInto(row => row.MoneyElementId, row => row.Value)
                    .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.MoneyElement);

                await moneyElementsCommand.ExecuteAsync(elementsList.Where(row => row.MoneyElement.HasValue)).ConfigureAwait(false);

                var textElementsCommand = new TableValuedInsert<TextElementRow>(commandProvider, provider.DatabaseContext)
                    .InsertInto(row => row.TextElementId, row => row.Value)
                    .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.TextElement);

                await textElementsCommand.ExecuteAsync(elementsList.Where(row => row.TextElement != null)).ConfigureAwait(false);

                // Attach the values to the submission
                var genericValueSubmissions = from v in insertedValues
                                              select new GenericSubmissionValueTableTypeRow
                                              {
                                                  GenericSubmissionId = submissionId,
                                                  GenericSubmissionValueId = v.FieldValueId
                                              };

                var submissionCommand = new TableValuedInsert<GenericSubmissionValueTableTypeRow>(commandProvider, provider.DatabaseContext);
                await submissionCommand.ExecuteAsync(genericValueSubmissions).ConfigureAwait(false);
                await transaction.CommitAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// The execute test.
        /// </summary>
        [TestMethod]
        public void CommandText_TableValuedInsertWithIdentityColumnSelectResults_CommandTextMatchesExpected()
        {
            var databaseContext = new Mock<IDatabaseContext>();
            var mockProvider = new Mock<IRepositoryProvider>();
            mockProvider.Setup(provider => provider.DatabaseContext).Returns(databaseContext.Object);

            using (mockProvider.Object)
            {
                var structuredCommandProvider = new Mock<IDbTableCommandFactory>();
                var repositoryAdapter = new Mock<IRepositoryAdapter>();
                repositoryAdapter.Setup(adapter => adapter.DefinitionProvider).Returns(new DataAnnotationsDefinitionProvider());
                repositoryAdapter.Setup(adapter => adapter.NameQualifier).Returns(new TransactSqlQualifier());
                databaseContext.Setup(context => context.RepositoryAdapter).Returns(repositoryAdapter.Object);
                ////structuredCommandProvider.Setup(provider => provider.DatabaseContext).Returns(databaseContext.Object);
                structuredCommandProvider
                    .Setup(
                        provider => provider.Create(
                            It.IsAny<ITableCommand>(),
                            It.IsAny<IEnumerable<FieldValueTableTypeRow>>()))
                    .Returns(new Mock<IDbCommand>().Object);

                var valuesCommand = new TableValuedInsert<FieldValueRow>(structuredCommandProvider.Object, databaseContext.Object)
                    .SelectFromInserted(row => row.FieldValueId, row => row.FieldId)
                    .SelectFromSource<FieldValueTableTypeRow>(
                        set => set.On(row => row.FieldId, row => row.FieldId),
                        row => row.LastModifiedByDomainIdentifierId,
                        row => row.LastModifiedTime);

                valuesCommand.Execute(new List<FieldValueTableTypeRow>());

                const string Expected = @"DECLARE @inserted table([FieldValueId] bigint, [FieldId] int);
INSERT INTO [dbo].[FieldValue]
([FieldId], [LastModifiedByDomainIdentifierId], [LastModifiedTime])
OUTPUT INSERTED.[FieldValueId], INSERTED.[FieldId]
INTO @inserted ([FieldValueId], [FieldId])
SELECT [FieldId], [LastModifiedByDomainIdentifierId], [LastModifiedTime] FROM @FieldValueRows AS source;
SELECT i.[FieldValueId], i.[FieldId], s.[LastModifiedByDomainIdentifierId], s.[LastModifiedTime]
FROM @inserted AS i
INNER JOIN @FieldValueRows AS s
ON i.[FieldId] = s.[FieldId];
";

                var actual = valuesCommand.CommandText;
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
                repositoryAdapter.Setup(adapter => adapter.NameQualifier).Returns(new TransactSqlQualifier());
                databaseContext.Setup(context => context.RepositoryAdapter).Returns(repositoryAdapter.Object);
                ////structuredCommandProvider.Setup(provider => provider.DatabaseContext).Returns(databaseContext.Object);
                structuredCommandProvider
                    .Setup(
                        provider => provider.Create(
                            It.IsAny<ITableCommand>(),
                            It.IsAny<IEnumerable<FieldValueElementTableTypeRow>>()))
                    .Returns(new Mock<IDbCommand>().Object);

                var dateElementsCommand = new TableValuedInsert<DateElementRow>(structuredCommandProvider.Object, databaseContext.Object)
                    .InsertInto(row => row.DateElementId, row => row.Value)
                    .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.DateElement);

                dateElementsCommand.Execute(new List<FieldValueElementTableTypeRow>());

                const string Expected = @"INSERT INTO [dbo].[DateElement]
([DateElementId], [Value])
SELECT [FieldValueElementId], [DateElement] FROM @FieldValueElementRows AS source;
";
                var actual = dateElementsCommand.CommandText;
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
                var repositoryAdapter = new Mock<IRepositoryAdapter>();
                repositoryAdapter.Setup(adapter => adapter.DefinitionProvider).Returns(new DataAnnotationsDefinitionProvider());
                repositoryAdapter.Setup(adapter => adapter.NameQualifier).Returns(new TransactSqlQualifier());
                databaseContext.Setup(context => context.RepositoryAdapter).Returns(repositoryAdapter.Object);

                // Attach the values to the submission
                var genericValueSubmissions = (from v in Enumerable.Range(1, 5)
                                               select new GenericSubmissionValueTableTypeRow
                                               {
                                                   GenericSubmissionId = 6,
                                                   GenericSubmissionValueId = v
                                               }).ToList();

                var structuredCommandProvider = genericValueSubmissions.MockCommandProvider(
                    new DataAnnotationsDefinitionProvider(),
                    new TransactSqlQualifier());

                var submissionCommand = new TableValuedInsert<GenericSubmissionValueRow>(structuredCommandProvider.Object, databaseContext.Object);
                submissionCommand.Execute(genericValueSubmissions);

                const string Expected = @"INSERT INTO [dbo].[GenericSubmissionValue]
([GenericSubmissionValueId], [GenericSubmissionId])
SELECT [GenericSubmissionValueId], [GenericSubmissionId] FROM @GenericSubmissionValueRows AS source;
";
                var actual = submissionCommand.CommandText;
                Assert.AreEqual(Expected, actual);
            }
        }
    }
}
