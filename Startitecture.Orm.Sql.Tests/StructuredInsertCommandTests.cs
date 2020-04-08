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
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;

    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Orm.Common;
    using Startitecture.Orm.Mapper;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Repository;
    using Startitecture.Orm.Schema;
    using Startitecture.Orm.Testing.Entities;
    using Startitecture.Orm.Testing.Entities.TableTypes;
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
        /// Gets or sets the test context.
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// The configuration root.
        /// </summary>
        private IConfigurationRoot ConfigurationRoot =>
            new ConfigurationBuilder().SetBasePath(this.TestContext.DeploymentDirectory)
                .AddJsonFile("appSettings.json", false)
                .Build();

        /// <summary>
        /// The execute test.
        /// </summary>
        [TestMethod]
        public void Execute_StructuredInsertCommand_DoesNotThrowException()
        {
            long submissionId = 0;

            var domainIdentity = new DomainIdentity(Thread.CurrentPrincipal.Identity.Name)
                                     {
                                         FirstName = "King",
                                         MiddleName = "T.",
                                         LastName = "Animal"
                                     };

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

            var databaseFactory = new DefaultDatabaseFactory(
                this.ConfigurationRoot.GetConnectionString("OrmTestDb"),
                nameof(System.Data.SqlClient),
                new DataAnnotationsDefinitionProvider());

            using (var provider = new DatabaseRepositoryProvider(databaseFactory, this.entityMapper))
            {
                var identityRepository = new EntityRepository<DomainIdentity, DomainIdentityRow>(provider, this.entityMapper);
                identityRepository.Save(domainIdentity);

                var fieldRepository = new EntityRepository<Field, FieldRow>(provider, this.entityMapper);

                foreach (var field in expected.SubmissionValues.Select(value => value.Field).Distinct())
                {
                    if (fieldRepository.Contains(Select.From<FieldRow>().WhereEqual(row => row.Name, field.Name)) == false)
                    {
                        fieldRepository.Save(field);
                    }
                }

                var submissionRepository = new EntityRepository<GenericSubmission, GenericSubmissionRow>(provider, this.entityMapper);

                var transaction = provider.StartTransaction();
                submissionRepository.Save(expected);

                submissionId = expected.GenericSubmissionId.GetValueOrDefault();
                Assert.AreNotEqual(0, submissionId);

                // Set up the structured command provider.
                var databaseContextProvider = (IDatabaseContextProvider)provider;
                var structuredCommandProvider = new StructuredSqlCommandProvider(databaseContextProvider);

                // Do the field values
                var valuesList = from v in expected.SubmissionValues
                                 select new FieldValueDataTypeRow
                                            {
                                                FieldId = v.Field.FieldId.GetValueOrDefault(),
                                                LastModifiedByDomainIdentifierId = domainIdentity.DomainIdentityId.GetValueOrDefault(),
                                                LastModifiedTime = expected.SubmissionTime
                                            };

                var valuesCommand =
                    new StructuredInsertCommand<FieldValueDataTypeRow>(structuredCommandProvider, transaction)
                        .InsertInto<FieldValueRow>(valuesList)
                        .SelectResults(row => row.FieldId);

                var insertedValues = valuesCommand.ExecuteWithIdentityUpdate(row => row.FieldValueId);
                this.entityMapper.MapTo(insertedValues, expected.SubmissionValues);

                // Do the field value elements
                var elementsList = (from e in expected.SubmissionValues.SelectMany(value => value.Elements)
                                    select new FieldValueElementTableTypeRow
                                               {
                                                   FieldValueElementId = e.FieldValueElementId,
                                                   FieldValueId = e.FieldValue.FieldValueId.GetValueOrDefault(),
                                                   Order = e.Order,
                                                   DateElement = e.Element as DateTimeOffset?, // TODO: See if we can implicit cast instead
                                                   FloatElement = e.Element as double?,
                                                   IntegerElement = e.Element as long?,
                                                   MoneyElement = e.Element as decimal?,
                                                   TextElement = e.Element as string // here we actually want it to be null if it is not a string
                                               }).ToList();

                var elementsCommand = new StructuredInsertCommand<FieldValueElementTableTypeRow>(structuredCommandProvider, transaction)
                    .InsertInto<FieldValueElementRow>(elementsList)
                    .SelectResults(row => row.FieldValueId, row => row.Order);

                elementsCommand.ExecuteWithIdentityUpdate(row => row.FieldValueElementId);

                var dateElementsCommand = new StructuredInsertCommand<FieldValueElementTableTypeRow>(structuredCommandProvider, transaction)
                    .InsertInto<DateElementRow>(elementsList);

                dateElementsCommand.Execute();

                ////// Attach the values to the submission
                ////var submissionLoader = new DataTableLoader<GenericSubmissionValueTableTypeRow>(provider.EntityDefinitionProvider);
                ////var submissionTable = submissionLoader.Load(expected.SubmissionValues, this.entityMapper);
                ////var submissionCommand = new StructuredInsertCommand<GenericSubmissionValueTableTypeRow>(structuredCommandProvider, transaction)
                ////    .InsertInto<GenericSubmissionValueRow>();

                ////submissionCommand.Execute(submissionTable);

                transaction.Commit();
            }
        }

        /// <summary>
        /// The execute test.
        /// </summary>
        [TestMethod]
        public void ExecuteReader_StructuredInsertCommandWithReturn_MatchesExpected()
        {
            long submissionId = 0;
            var domainIdentity = new DomainIdentity(Thread.CurrentPrincipal.Identity.Name)
            {
                FirstName = "King",
                MiddleName = "T.",
                LastName = "Animal"
            };

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

            var databaseFactory = new DefaultDatabaseFactory(
                this.ConfigurationRoot.GetConnectionString("OrmTestDb"),
                nameof(System.Data.SqlClient),
                new DataAnnotationsDefinitionProvider());

            using (var provider = new DatabaseRepositoryProvider(databaseFactory, this.entityMapper))
            {
                try
                {
                    var transaction = provider.StartTransaction();

                    submissionId = expected.GenericSubmissionId.GetValueOrDefault();

                    ////var dataTableLoader = new DataTableLoader<FieldValueDataTypeRow>(provider.EntityDefinitionProvider);
                    ////var valueDataTable = dataTableLoader.Load(expected.SubmissionValues, this.entityMapper);

                    ////var databaseContextProvider = (IDatabaseContextProvider)provider;
                    ////Assert.IsNotNull(databaseContextProvider);
                    ////var structuredCommandProvider = new StructuredSqlCommandProvider(databaseContextProvider);

                    ////var target =
                    ////    new StructuredInsertCommand<GenericSubmissionRow>(structuredCommandProvider, transaction)
                    ////        .InsertInto<FieldValueRow>()
                    ////        .SelectResults(row => row.GenericSubmissionId);

                    ////using (var reader = target.ExecuteReader(valueDataTable))
                    ////{
                    ////    while (reader.Read())
                    ////    {
                    ////        var comparisonRow = new GenericSubmissionValueRow
                    ////                                {
                    ////                                    GenericSubmissionValueId = reader.GetInt64(0),
                    ////                                    GenericSubmissionId = reader.GetInt32(1),
                    ////                                    GenericSubmission = GetGenericSubmissionFromRecord(reader, provider),
                    ////                                    Field = GetFieldFromRecord(reader, provider)
                    ////                                };

                    ////        Assert.AreNotEqual(0, comparisonRow.Field.FieldId);
                    ////    }
                    ////}
                }
                finally
                {
                    provider.AbortTransaction();
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
