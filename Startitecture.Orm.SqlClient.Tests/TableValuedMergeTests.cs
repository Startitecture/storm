﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableValuedMergeTests.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.SqlClient.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using global::AutoMapper;

    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Startitecture.Core;
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
    /// The structured merge command tests.
    /// </summary>
    [TestClass]
    public class TableValuedMergeTests
    {
        /// <summary>
        /// The entity this.mapper.
        /// </summary>
        private readonly IEntityMapper mapper = new AutoMapperEntityMapper(
            new Mapper(new MapperConfiguration(expression => { expression.AddProfile<GenericSubmissionMappingProfile>(); })));

        /// <summary>
        /// Gets the configuration root.
        /// </summary>
        private static IConfigurationRoot ConfigurationRoot => new ConfigurationBuilder().AddJsonFile("appsettings.json", false).Build();

        /// <summary>
        /// The merge into test.
        /// </summary>
        [TestMethod]
        public void SelectFromInserted_ItemsWithIdentityColumn_ReturnedItemsMatchExpected()
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
            var databaseContext = new Mock<IDatabaseContext>();
            var repositoryAdapter = new Mock<IRepositoryAdapter>();

            var nameQualifier = new TransactSqlQualifier();
            var commandFactory = mergeItems.MockCommandFactory(definitionProvider, nameQualifier);

            repositoryAdapter.Setup(adapter => adapter.DefinitionProvider).Returns(definitionProvider);
            repositoryAdapter.Setup(adapter => adapter.NameQualifier).Returns(nameQualifier);
            databaseContext.Setup(context => context.RepositoryAdapter).Returns(repositoryAdapter.Object);
            var target = new TableValuedMerge<FieldRow>(commandFactory.Object, databaseContext.Object);
            var typeRows = target.OnImplicit(row => row.Name).SelectFromInserted().ExecuteForResults(mergeItems);

            Assert.IsNotNull(typeRows);
            var actual = typeRows.Select(
                    row => row.FieldId > 0
                               ? new Field(row.FieldId)
                               {
                                   Name = row.Name,
                                   Description = row.Description
                               }
                               : new Field
                               {
                                   Name = row.Name,
                                   Description = row.Description
                               })
                .ToList();

            Assert.IsTrue(actual.All(field => field.FieldId.HasValue));
            CollectionAssert.AreEqual(fields, actual);
        }

        /// <summary>
        /// The merge into test.
        /// </summary>
        [TestMethod]
        public void SelectFromInserted_ItemsWithIdentityColumn_CommandTextMatchesExpected()
        {
            var commandProvider = new Mock<IDbTableCommandFactory>();
            var databaseContext = new Mock<IDatabaseContext>();
            var repositoryAdapter = new Mock<IRepositoryAdapter>();
            repositoryAdapter.Setup(adapter => adapter.NameQualifier).Returns(new TransactSqlQualifier());
            repositoryAdapter.Setup(adapter => adapter.DefinitionProvider).Returns(new DataAnnotationsDefinitionProvider());
            databaseContext.Setup(context => context.RepositoryAdapter).Returns(repositoryAdapter.Object);
            ////commandProvider.Setup(provider => provider.DatabaseContext).Returns(databaseContext.Object);
            commandProvider
                .Setup(
                    provider => provider.Create(
                        It.IsAny<IDatabaseContext>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<IEnumerable<FieldValueTableTypeRow>>()))
                .Returns(new Mock<IDbCommand>().Object);

            var fieldValueCommand = new TableValuedMerge<FieldValueRow>(databaseContext.Object);

            const string Expected = @"DECLARE @inserted table([FieldValueId] bigint, [FieldId] int, [LastModifiedByDomainIdentifierId] int, [LastModifiedTime] datetimeoffset);
MERGE [dbo].[FieldValue] AS [Target]
USING @FieldValueRows AS [Source]
ON ([Target].[FieldValueId] = [Source].[FieldValueId])
WHEN MATCHED THEN
UPDATE SET [FieldId] = [Source].[FieldId], [LastModifiedByDomainIdentifierId] = [Source].[LastModifiedByDomainIdentifierId], [LastModifiedTime] = [Source].[LastModifiedTime]
WHEN NOT MATCHED BY TARGET THEN
INSERT ([FieldId], [LastModifiedByDomainIdentifierId], [LastModifiedTime])
VALUES ([Source].[FieldId], [Source].[LastModifiedByDomainIdentifierId], [Source].[LastModifiedTime])
OUTPUT INSERTED.[FieldValueId], INSERTED.[FieldId], INSERTED.[LastModifiedByDomainIdentifierId], INSERTED.[LastModifiedTime]
INTO @inserted ([FieldValueId], [FieldId], [LastModifiedByDomainIdentifierId], [LastModifiedTime]);
SELECT i.[FieldValueId], i.[FieldId], i.[LastModifiedByDomainIdentifierId], i.[LastModifiedTime]
FROM @inserted AS i;
";

            var target = fieldValueCommand.OnImplicit(row => row.FieldValueId).SelectFromInserted();

            Assert.AreEqual(Expected, target.GetCommandText<FieldValueTableTypeRow>("FieldValueRows"));
        }

        /// <summary>
        /// The merge into test.
        /// </summary>
        [TestMethod]
        public void DeleteUnmatchedInSource_ItemsWithIdentityColumn_CommandTextMatchesExpected()
        {
            var commandProvider = new Mock<IDbTableCommandFactory>();
            var databaseContext = new Mock<IDatabaseContext>();
            var repositoryAdapter = new Mock<IRepositoryAdapter>();
            repositoryAdapter.Setup(adapter => adapter.NameQualifier).Returns(new TransactSqlQualifier());
            repositoryAdapter.Setup(adapter => adapter.DefinitionProvider).Returns(new DataAnnotationsDefinitionProvider());
            databaseContext.Setup(context => context.RepositoryAdapter).Returns(repositoryAdapter.Object);
            commandProvider
                .Setup(
                    provider => provider.Create(
                        It.IsAny<IDatabaseContext>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<IEnumerable<FieldValueElementTableTypeRow>>()))
                .Returns(new Mock<IDbCommand>().Object);

            const string Expected = @"DECLARE @inserted table([FieldValueElementId] bigint, [FieldValueId] bigint, [Order] int);
MERGE [dbo].[FieldValueElement] AS [Target]
USING @FieldValueElementRows AS [Source]
ON ([Target].[FieldValueId] = [Source].[FieldValueId] AND [Target].[Order] = [Source].[Order])
WHEN MATCHED THEN
UPDATE SET [FieldValueId] = [Source].[FieldValueId], [Order] = [Source].[Order]
WHEN NOT MATCHED BY TARGET THEN
INSERT ([FieldValueId], [Order])
VALUES ([Source].[FieldValueId], [Source].[Order])
WHEN NOT MATCHED BY SOURCE AND [Target].FieldValueId IN (SELECT [FieldValueId] FROM @FieldValueElementRows) THEN DELETE
OUTPUT INSERTED.[FieldValueElementId], INSERTED.[FieldValueId], INSERTED.[Order]
INTO @inserted ([FieldValueElementId], [FieldValueId], [Order]);
SELECT i.[FieldValueElementId], i.[FieldValueId], i.[Order]
FROM @inserted AS i;
";

            var elementMergeCommand = new TableValuedMerge<FieldValueElementRow>(databaseContext.Object)
                .OnImplicit(row => row.FieldValueId, row => row.Order)
                .DeleteUnmatchedInSource<FieldValueElementTableTypeRow>(row => row.FieldValueId)
                .SelectFromInserted();

            var actual = elementMergeCommand.GetCommandText<FieldValueElementTableTypeRow>("FieldValueElementRows");
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The merge into test.
        /// </summary>
        [TestMethod]
        public void DeleteUnmatchedInSourceWithSelectFromSource_ItemsWithIdentityColumn_CommandTextMatchesExpected()
        {
            var commandProvider = new Mock<IDbTableCommandFactory>();
            var databaseContext = new Mock<IDatabaseContext>();
            var repositoryAdapter = new Mock<IRepositoryAdapter>();
            repositoryAdapter.Setup(adapter => adapter.NameQualifier).Returns(new TransactSqlQualifier());
            repositoryAdapter.Setup(adapter => adapter.DefinitionProvider).Returns(new DataAnnotationsDefinitionProvider());
            databaseContext.Setup(context => context.RepositoryAdapter).Returns(repositoryAdapter.Object);
            ////commandProvider.Setup(provider => provider.DatabaseContext).Returns(databaseContext.Object);
            commandProvider
                .Setup(
                    provider => provider.Create(
                        It.IsAny<IDatabaseContext>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<IEnumerable<FieldValueElementTableTypeRow>>()))
                .Returns(new Mock<IDbCommand>().Object);

            const string Expected = @"DECLARE @inserted table([FieldValueElementId] bigint, [FieldValueId] bigint, [Order] int);
MERGE [dbo].[FieldValueElement] AS [Target]
USING @FieldValueElementRows AS [Source]
ON ([Target].[FieldValueId] = [Source].[FieldValueId] AND [Target].[Order] = [Source].[Order])
WHEN MATCHED THEN
UPDATE SET [FieldValueId] = [Source].[FieldValueId], [Order] = [Source].[Order]
WHEN NOT MATCHED BY TARGET THEN
INSERT ([FieldValueId], [Order])
VALUES ([Source].[FieldValueId], [Source].[Order])
WHEN NOT MATCHED BY SOURCE AND [Target].FieldValueId IN (SELECT [FieldValueId] FROM @FieldValueElementRows) THEN DELETE
OUTPUT INSERTED.[FieldValueElementId], INSERTED.[FieldValueId], INSERTED.[Order]
INTO @inserted ([FieldValueElementId], [FieldValueId], [Order]);
SELECT i.[FieldValueElementId], i.[FieldValueId], i.[Order], s.[DateElement], s.[FloatElement], s.[IntegerElement], s.[MoneyElement], s.[TextElement]
FROM @inserted AS i
INNER JOIN @FieldValueElementRows AS s
ON i.[FieldValueId] = s.[FieldValueId] AND i.[Order] = s.[Order];
";

            var elementMergeCommand = new TableValuedMerge<FieldValueElementRow>(databaseContext.Object)
                .OnImplicit(row => row.FieldValueId, row => row.Order)
                .DeleteUnmatchedInSource<FieldValueElementTableTypeRow>(row => row.FieldValueId)
                .SelectFromInserted()
                .SelectFromSource<FieldValueElementTableTypeRow>(
                    set => set.On(row => row.FieldValueId, row => row.FieldValueId).On(row => row.Order, row => row.Order));

            var actual = elementMergeCommand.GetCommandText<FieldValueElementTableTypeRow>("FieldValueElementRows");
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The merge into test.
        /// </summary>
        [TestMethod]
        public void DeleteUnmatchedInSource_ItemsWithoutIdentityColumn_CommandTextMatchesExpected()
        {
            var commandProvider = new Mock<IDbTableCommandFactory>();
            var databaseContext = new Mock<IDatabaseContext>();
            var repositoryAdapter = new Mock<IRepositoryAdapter>();
            repositoryAdapter.Setup(adapter => adapter.NameQualifier).Returns(new TransactSqlQualifier());
            repositoryAdapter.Setup(adapter => adapter.DefinitionProvider).Returns(new DataAnnotationsDefinitionProvider());
            databaseContext.Setup(context => context.RepositoryAdapter).Returns(repositoryAdapter.Object);
            ////commandProvider.Setup(provider => provider.DatabaseContext).Returns(databaseContext.Object);
            commandProvider
                .Setup(
                    provider => provider.Create(
                        It.IsAny<IDatabaseContext>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<IEnumerable<GenericSubmissionValueTableTypeRow>>()))
                .Returns(new Mock<IDbCommand>().Object);

            const string Expected = @"DECLARE @inserted table([GenericSubmissionValueId] bigint, [GenericSubmissionId] int);
MERGE [dbo].[GenericSubmissionValue] AS [Target]
USING @GenericSubmissionValueRows AS [Source]
ON ([Target].[GenericSubmissionValueId] = [Source].[GenericSubmissionValueId])
WHEN MATCHED THEN
UPDATE SET [GenericSubmissionId] = [Source].[GenericSubmissionId]
WHEN NOT MATCHED BY TARGET THEN
INSERT ([GenericSubmissionValueId], [GenericSubmissionId])
VALUES ([Source].[GenericSubmissionValueId], [Source].[GenericSubmissionId])
WHEN NOT MATCHED BY SOURCE AND [Target].GenericSubmissionId IN (SELECT [GenericSubmissionId] FROM @GenericSubmissionValueRows) THEN DELETE
OUTPUT INSERTED.[GenericSubmissionValueId], INSERTED.[GenericSubmissionId]
INTO @inserted ([GenericSubmissionValueId], [GenericSubmissionId]);
SELECT i.[GenericSubmissionValueId], i.[GenericSubmissionId]
FROM @inserted AS i;
";

            var target = new TableValuedMerge<GenericSubmissionValueRow>(databaseContext.Object);
            target.OnImplicit(row => row.GenericSubmissionValueId)
                .SelectFromInserted()
                .DeleteUnmatchedInSource<GenericSubmissionValueTableTypeRow>(row => row.GenericSubmissionId);

            Assert.AreEqual(Expected, target.GetCommandText<GenericSubmissionValueTableTypeRow>("GenericSubmissionValueRows"));
        }

        /// <summary>
        /// The merge into test.
        /// </summary>
        [TestMethod]
        public void OnFrom_RelatedItems_CommandTextMatchesExpected()
        {
            var commandProvider = new Mock<IDbTableCommandFactory>();
            var databaseContext = new Mock<IDatabaseContext>();
            var repositoryAdapter = new Mock<IRepositoryAdapter>();
            repositoryAdapter.Setup(adapter => adapter.NameQualifier).Returns(new TransactSqlQualifier());
            repositoryAdapter.Setup(adapter => adapter.DefinitionProvider).Returns(new DataAnnotationsDefinitionProvider());
            databaseContext.Setup(context => context.RepositoryAdapter).Returns(repositoryAdapter.Object);
            ////commandProvider.Setup(provider => provider.DatabaseContext).Returns(databaseContext.Object);
            commandProvider
                .Setup(
                    provider => provider.Create(
                        It.IsAny<IDatabaseContext>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<IEnumerable<FieldValueElementTableTypeRow>>()))
                .Returns(new Mock<IDbCommand>().Object);

            const string Expected = @"MERGE [dbo].[DateElement] AS [Target]
USING @FieldValueElementRows AS [Source]
ON ([Target].[DateElementId] = [Source].[FieldValueElementId])
WHEN MATCHED THEN
UPDATE SET [Value] = [Source].[DateElement]
WHEN NOT MATCHED BY TARGET THEN
INSERT ([DateElementId], [Value])
VALUES ([Source].[FieldValueElementId], [Source].[DateElement])
;
";

            var target = new TableValuedMerge<DateElementRow>(databaseContext.Object)
                .On<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.DateElementId)
                .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.DateElement);

            Assert.AreEqual(Expected, target.GetCommandText<FieldValueElementTableTypeRow>("FieldValueElementRows"));
        }

        /// <summary>
        /// The merge into test.
        /// </summary>
        [TestMethod]
        public void OnFromImplicit_RelatedItems_CommandTextMatchesExpected()
        {
            var commandFactory = new Mock<IDbTableCommandFactory>();
            var databaseContext = new Mock<IDatabaseContext>();
            var repositoryAdapter = new Mock<IRepositoryAdapter>();
            repositoryAdapter.Setup(adapter => adapter.NameQualifier).Returns(new TransactSqlQualifier());
            repositoryAdapter.Setup(adapter => adapter.DefinitionProvider).Returns(new DataAnnotationsDefinitionProvider());
            databaseContext.Setup(context => context.RepositoryAdapter).Returns(repositoryAdapter.Object);
            commandFactory
                .Setup(
                    provider => provider.Create(
                        It.IsAny<IDatabaseContext>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<IEnumerable<FieldTableTypeRow>>()))
                .Returns(new Mock<IDbCommand>().Object);

            const string Expected = @"DECLARE @inserted table([FieldId] int, [Name] nvarchar(50), [Description] nvarchar(max));
MERGE [dbo].[Field] AS [Target]
USING @FieldRows AS [Source]
ON ([Target].[Name] = [Source].[Name])
WHEN MATCHED THEN
UPDATE SET [Name] = [Source].[Name], [Description] = [Source].[Description]
WHEN NOT MATCHED BY TARGET THEN
INSERT ([Name], [Description])
VALUES ([Source].[Name], [Source].[Description])
OUTPUT INSERTED.[FieldId], INSERTED.[Name], INSERTED.[Description]
INTO @inserted ([FieldId], [Name], [Description]);
SELECT i.[FieldId], i.[Name], i.[Description]
FROM @inserted AS i;
";

            // Merge in the field values.
            var target = new TableValuedMerge<FieldRow>(databaseContext.Object).OnImplicit(row => row.Name)
                .SelectFromInserted();

            Assert.AreEqual(Expected, target.GetCommandText<FieldTableTypeRow>("FieldRows"));
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

            using (var provider = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
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
                this.MergeSubmission(expected, provider);

                var submissionRepository = new EntityRepository<GenericSubmission, GenericSubmissionRow>(provider, this.mapper);
                actual = submissionRepository.FirstOrDefault(expected.GenericSubmissionId);

                var fieldValueRepository = new EntityRepository<FieldValue, GenericSubmissionValueRow>(provider, this.mapper);
                var values = fieldValueRepository.SelectEntities(
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
                                e.DateElement ?? e.FloatElement ?? e.IntegerElement ?? e.MoneyElement?.ToDecimal() ?? e.TextElement as object,
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
        /// The execute for results generic submission database matches expected.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous unit test.
        /// </returns>
        [TestMethod]
        [TestCategory("Integration")]
        public async Task ExecuteForResultsAsync_GenericSubmission_DatabaseMatchesExpected()
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
            var cancellationToken = CancellationToken.None;

            GenericSubmission baselineSubmission;
            DomainIdentity domainIdentity2;

            await using (var provider = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
            {
                // Set up the domain identity, not part of our validity testing.
                var identityRepository = new EntityRepository<DomainIdentity, DomainIdentityRow>(provider, this.mapper);
                var domainIdentity = await identityRepository.FirstOrDefaultAsync(
                                             Query.Select<DomainIdentity>()
                                                 .Where(set => set.AreEqual(identity => identity.UniqueIdentifier, Environment.UserName)),
                                             cancellationToken)
                                         .ConfigureAwait(false)
                                     ?? await identityRepository.SaveAsync(
                                             new DomainIdentity(Environment.UserName)
                                             {
                                                 FirstName = "King",
                                                 MiddleName = "T.",
                                                 LastName = "Animal"
                                             },
                                             cancellationToken)
                                         .ConfigureAwait(false);

                var domainIdentifier2 = $"{Environment.UserName}2";
                domainIdentity2 = await identityRepository.FirstOrDefaultAsync(
                                          Query.Select<DomainIdentity>()
                                              .Where(set => set.AreEqual(identity => identity.UniqueIdentifier, domainIdentifier2)),
                                          cancellationToken)
                                      .ConfigureAwait(false)
                                  ?? await identityRepository.SaveAsync(
                                          new DomainIdentity(domainIdentifier2)
                                          {
                                              FirstName = "Foo",
                                              MiddleName = "J.",
                                              LastName = "Bar"
                                          },
                                          cancellationToken)
                                      .ConfigureAwait(false);

                // We will add to this submission later.
                baselineSubmission = new GenericSubmission("My MERGE Submission", domainIdentity);
                baselineSubmission.SetValue(internalId, 9234);
                baselineSubmission.SetValue(firstName, "Dan");
                baselineSubmission.SetValue(lastName, "The Man");
                baselineSubmission.SetValue(yearlyWage, 72150.35m); // gonna get updated so lets check that this value got scrapped
                baselineSubmission.Submit();

                await this.MergeSubmissionAsync(baselineSubmission, provider).ConfigureAwait(false);
            }

            Assert.IsTrue(baselineSubmission.GenericSubmissionId.HasValue);

            // Reusing the key lets us test whether updates are in fact working as expected.
            var expected = new GenericSubmission(
                "My Final MERGE Submission",
                domainIdentity2,
                baselineSubmission.GenericSubmissionId.GetValueOrDefault());

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
            await using (var provider = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
            {
                await this.MergeSubmissionAsync(expected, provider).ConfigureAwait(false);

                var submissionRepository = new EntityRepository<GenericSubmission, GenericSubmissionRow>(provider, this.mapper);
                actual = await submissionRepository.FirstOrDefaultAsync(expected.GenericSubmissionId, cancellationToken).ConfigureAwait(false);

                var fieldValueRepository = new EntityRepository<FieldValue, GenericSubmissionValueRow>(provider, this.mapper);
                var values = new Dictionary<long, FieldValue>();

                await foreach (var item in fieldValueRepository.SelectEntitiesAsync(
                                       Query.Select<GenericSubmissionValueRow>()
                                           .From(
                                               set => set.InnerJoin(row => row.GenericSubmissionValueId, row => row.FieldValue.FieldValueId)
                                                   .InnerJoin(row => row.FieldValue.FieldId, row => row.FieldValue.Field.FieldId)
                                                   .InnerJoin(
                                                       row => row.FieldValue.LastModifiedByDomainIdentifierId,
                                                       row => row.FieldValue.LastModifiedBy.DomainIdentityId))
                                           .Where(
                                               set => set.AreEqual(row => row.GenericSubmissionId, expected.GenericSubmissionId.GetValueOrDefault())),
                                       cancellationToken)
                                   .ConfigureAwait(false))
                {
                    values.Add(item.FieldValueId.GetValueOrDefault(), item);
                }

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
                                e.DateElement ?? e.FloatElement ?? e.IntegerElement ?? e.MoneyElement?.ToDecimal() ?? e.TextElement as object,
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
        private void MergeSubmission(GenericSubmission submission, IRepositoryProvider provider)
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
            var transaction = provider.BeginTransaction();

            var fieldsCommand = new TableValuedMerge<FieldRow>(provider.DatabaseContext);
            var mergedFields = fieldsCommand.OnImplicit(row => row.Name)
                .SelectFromInserted()
                .ExecuteForResults(fieldItems)
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
            var fieldValueCommand = new TableValuedMerge<FieldValueRow>(provider.DatabaseContext);
            var mergedFieldValues = fieldValueCommand.OnImplicit(row => row.FieldValueId)
                .SelectFromInserted()
                .ExecuteForResults(fieldValues)
                .ToList();

            Assert.IsTrue(mergedFieldValues.All(row => row.FieldValueId > 0));

            // Map back to the domain object. TODO: Automate?
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

            var elementMergeCommand = new TableValuedMerge<FieldValueElementRow>(provider.DatabaseContext);
            var mergedValueElements = elementMergeCommand.OnImplicit(row => row.FieldValueId, row => row.Order)
                .DeleteUnmatchedInSource<FieldValueElementTableTypeRow>(row => row.FieldValueId) // Get rid of extraneous elements
                .SelectFromInserted()
                .ExecuteForResults(valueElements)
                .ToList();

            foreach (var element in valueElements)
            {
                var input = mergedValueElements.First(row => row.FieldValueId == element.FieldValueId && row.Order == element.Order);
                this.mapper.MapTo(input, element);
                Assert.IsTrue(element.FieldValueElementId.HasValue);
            }

            var dateElementsCommand = new TableValuedMerge<DateElementRow>(provider.DatabaseContext)
                .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.DateElement)
                .On<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.DateElementId);

            dateElementsCommand.Execute(valueElements.Where(row => row.DateElement.HasValue));

            var floatElementsCommand = new TableValuedMerge<FloatElementRow>(provider.DatabaseContext)
                .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.FloatElement)
                .On<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.FloatElementId);

            floatElementsCommand.Execute(valueElements.Where(row => row.FloatElement.HasValue));

            var integerElementsCommand = new TableValuedMerge<IntegerElementRow>(provider.DatabaseContext)
                .On<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.IntegerElementId)
                .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.IntegerElement);

            integerElementsCommand.Execute(valueElements.Where(row => row.IntegerElement.HasValue));

            var moneyElementsCommand = new TableValuedMerge<MoneyElementRow>(provider.DatabaseContext)
                .On<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.MoneyElementId)
                .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.MoneyElement);

            moneyElementsCommand.Execute(valueElements.Where(row => row.MoneyElement.HasValue));

            var textElementsCommand = new TableValuedMerge<TextElementRow>(provider.DatabaseContext)
                .On<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.TextElementId)
                .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.TextElement);

            textElementsCommand.Execute(valueElements.Where(row => row.TextElement != null));

            // Attach the values to the submission
            var genericValueSubmissions = from v in mergedFieldValues
                                          select new GenericSubmissionValueTableTypeRow
                                          {
                                              GenericSubmissionId = submission.GenericSubmissionId.GetValueOrDefault(),
                                              GenericSubmissionValueId = v.FieldValueId
                                          };

            var submissionCommand = new TableValuedMerge<GenericSubmissionValueRow>(provider.DatabaseContext)
                .OnImplicit(row => row.GenericSubmissionValueId)
                .DeleteUnmatchedInSource<GenericSubmissionValueTableTypeRow>(row => row.GenericSubmissionId);

            submissionCommand.Execute(genericValueSubmissions);
            transaction.Commit();
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
        private async Task MergeSubmissionAsync(GenericSubmission submission, IRepositoryProvider provider)
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
            var cancellationToken = CancellationToken.None;
            var transaction = await provider.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

            var fieldsCommand = new TableValuedMerge<FieldRow>(provider.DatabaseContext);
            var mergedFields = new List<FieldRow>();

            await foreach (var item in fieldsCommand.OnImplicit(row => row.Name)
                               .SelectFromInserted()
                               .ExecuteForResultsAsync(fieldItems, cancellationToken)
                               .ConfigureAwait(false))
            {
                mergedFields.Add(item);
            }

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
            await submissionRepository.SaveAsync(submission, cancellationToken).ConfigureAwait(false);

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
            var fieldValueCommand = new TableValuedMerge<FieldValueRow>(provider.DatabaseContext);
            var mergedFieldValues = new List<FieldValueRow>();

            await foreach (var item in fieldValueCommand.OnImplicit(row => row.FieldValueId)
                               .SelectFromInserted()
                               .ExecuteForResultsAsync(fieldValues, cancellationToken)
                               .ConfigureAwait(false))
            {
                mergedFieldValues.Add(item);
            }

            Assert.IsTrue(mergedFieldValues.All(row => row.FieldValueId > 0));

            // Map back to the domain object. TODO: Automate?
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

            var elementMergeCommand = new TableValuedMerge<FieldValueElementRow>(provider.DatabaseContext);
            var mergedValueElements = new List<FieldValueElementRow>();

            await foreach (var item in elementMergeCommand.OnImplicit(row => row.FieldValueId, row => row.Order)
                               .DeleteUnmatchedInSource<FieldValueElementTableTypeRow>(row => row.FieldValueId) // Get rid of extraneous elements
                               .SelectFromInserted()
                               .ExecuteForResultsAsync(valueElements, cancellationToken)
                               .ConfigureAwait(false))
            {
                mergedValueElements.Add(item);
            }

            foreach (var element in valueElements)
            {
                var input = mergedValueElements.First(row => row.FieldValueId == element.FieldValueId && row.Order == element.Order);
                this.mapper.MapTo(input, element);
                Assert.IsTrue(element.FieldValueElementId.HasValue);
            }

            var dateElementsCommand = new TableValuedMerge<DateElementRow>(provider.DatabaseContext)
                .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.DateElement)
                .On<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.DateElementId);

            await dateElementsCommand.ExecuteAsync(valueElements.Where(row => row.DateElement.HasValue), cancellationToken).ConfigureAwait(false);

            var floatElementsCommand = new TableValuedMerge<FloatElementRow>(provider.DatabaseContext)
                .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.FloatElement)
                .On<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.FloatElementId);

            await floatElementsCommand.ExecuteAsync(valueElements.Where(row => row.FloatElement.HasValue), cancellationToken).ConfigureAwait(false);

            var integerElementsCommand = new TableValuedMerge<IntegerElementRow>(provider.DatabaseContext)
                .On<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.IntegerElementId)
                .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.IntegerElement);

            await integerElementsCommand.ExecuteAsync(valueElements.Where(row => row.IntegerElement.HasValue), cancellationToken).ConfigureAwait(false);

            var moneyElementsCommand = new TableValuedMerge<MoneyElementRow>(provider.DatabaseContext)
                .On<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.MoneyElementId)
                .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.MoneyElement);

            await moneyElementsCommand.ExecuteAsync(valueElements.Where(row => row.MoneyElement.HasValue), cancellationToken).ConfigureAwait(false);

            var textElementsCommand = new TableValuedMerge<TextElementRow>(provider.DatabaseContext)
                .On<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.TextElementId)
                .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.TextElement);

            await textElementsCommand.ExecuteAsync(valueElements.Where(row => row.TextElement != null), cancellationToken).ConfigureAwait(false);

            // Attach the values to the submission
            var genericValueSubmissions = from v in mergedFieldValues
                                          select new GenericSubmissionValueTableTypeRow
                                          {
                                              GenericSubmissionId = submission.GenericSubmissionId.GetValueOrDefault(),
                                              GenericSubmissionValueId = v.FieldValueId
                                          };

            var submissionCommand = new TableValuedMerge<GenericSubmissionValueRow>(provider.DatabaseContext)
                .OnImplicit(row => row.GenericSubmissionValueId)
                .DeleteUnmatchedInSource<GenericSubmissionValueTableTypeRow>(row => row.GenericSubmissionId);

            await submissionCommand.ExecuteAsync(genericValueSubmissions, cancellationToken).ConfigureAwait(false);
            await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}