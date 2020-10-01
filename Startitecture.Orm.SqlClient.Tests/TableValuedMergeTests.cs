// --------------------------------------------------------------------------------------------------------------------
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
            new Mapper(
                new MapperConfiguration(
                    expression =>
                        {
                            expression.AddProfile<GenericSubmissionMappingProfile>();
                        })));

        /// <summary>
        /// The configuration root.
        /// </summary>
        private static IConfigurationRoot ConfigurationRoot => new ConfigurationBuilder().AddJsonFile("appSettings.json", false).Build();

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
            var commandProvider = mergeItems.MockCommandProvider(definitionProvider, new TransactSqlQualifier());
            var target = new TableValuedMerge<FieldRow>(commandProvider.Object);
            var typeRows = target.OnImplicit(row => row.Name).SelectFromInserted(row => row.Name).ExecuteForResults(mergeItems);

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
                                 }).ToList();

            Assert.IsTrue(actual.All(field => field.FieldId.HasValue));
            CollectionAssert.AreEqual(fields, actual);
        }

        /// <summary>
        /// The merge into test.
        /// </summary>
        [TestMethod]
        public void SelectFromInserted_ItemsWithIdentityColumn_CommandTextMatchesExpected()
        {
            var commandProvider = new Mock<ITableCommandProvider>();
            var databaseContext = new Mock<IDatabaseContext>();
            var repositoryAdapter = new Mock<IRepositoryAdapter>();
            repositoryAdapter.Setup(adapter => adapter.NameQualifier).Returns(new TransactSqlQualifier());
            repositoryAdapter.Setup(adapter => adapter.DefinitionProvider).Returns(new DataAnnotationsDefinitionProvider());
            databaseContext.Setup(context => context.RepositoryAdapter).Returns(repositoryAdapter.Object);
            commandProvider.Setup(provider => provider.DatabaseContext).Returns(databaseContext.Object);
            commandProvider.Setup(provider => provider.DatabaseContext).Returns(databaseContext.Object);
            commandProvider
                .Setup(
                    provider => provider.CreateCommand(
                        It.IsAny<ITableCommand>(),
                        It.IsAny<IEnumerable<FieldValueTableTypeRow>>(),
                        It.IsAny<IDbTransaction>()))
                .Returns(new Mock<IDbCommand>().Object);

            var fieldValueCommand = new TableValuedMerge<FieldValueRow>(commandProvider.Object);

            const string Expected = @"DECLARE @inserted FieldValueTableType;
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
SELECT i.[FieldValueId], tvp.[FieldId], tvp.[LastModifiedByDomainIdentifierId], tvp.[LastModifiedTime]
FROM @inserted AS i
INNER JOIN @FieldValueRows AS tvp
ON i.[FieldId] = tvp.[FieldId];
";

            var target = fieldValueCommand.OnImplicit(row => row.FieldValueId).SelectFromInserted(row => row.FieldId);
            target.Execute(new List<FieldValueTableTypeRow>());

            Assert.AreEqual(Expected, target.CommandText);
        }

        /// <summary>
        /// The merge into test.
        /// </summary>
        [TestMethod]
        public void DeleteUnmatchedInSource_ItemsWithIdentityColumn_CommandTextMatchesExpected()
        {
            var commandProvider = new Mock<ITableCommandProvider>();
            var databaseContext = new Mock<IDatabaseContext>();
            var repositoryAdapter = new Mock<IRepositoryAdapter>();
            repositoryAdapter.Setup(adapter => adapter.NameQualifier).Returns(new TransactSqlQualifier());
            repositoryAdapter.Setup(adapter => adapter.DefinitionProvider).Returns(new DataAnnotationsDefinitionProvider());
            databaseContext.Setup(context => context.RepositoryAdapter).Returns(repositoryAdapter.Object);
            commandProvider.Setup(provider => provider.DatabaseContext).Returns(databaseContext.Object);
            commandProvider.Setup(provider => provider.DatabaseContext).Returns(databaseContext.Object);
            commandProvider
                .Setup(
                    provider => provider.CreateCommand(
                        It.IsAny<ITableCommand>(),
                        It.IsAny<IEnumerable<FieldValueElementTableTypeRow>>(),
                        It.IsAny<IDbTransaction>()))
                .Returns(new Mock<IDbCommand>().Object);

            const string Expected = @"DECLARE @inserted FieldValueElementTableType;
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
SELECT i.[FieldValueElementId], tvp.[FieldValueId], tvp.[Order], tvp.[DateElement], tvp.[FloatElement], tvp.[IntegerElement], tvp.[MoneyElement], tvp.[TextElement]
FROM @inserted AS i
INNER JOIN @FieldValueElementRows AS tvp
ON i.[FieldValueId] = tvp.[FieldValueId] AND i.[Order] = tvp.[Order];
";

            var elementMergeCommand = new TableValuedMerge<FieldValueElementRow>(commandProvider.Object)
                .OnImplicit(row => row.FieldValueId, row => row.Order)
                .DeleteUnmatchedInSource<FieldValueElementTableTypeRow>(row => row.FieldValueId)
                .SelectFromInserted(row => row.FieldValueId, row => row.Order);

            elementMergeCommand.Execute(new List<FieldValueElementTableTypeRow>());
            var actual = elementMergeCommand.CommandText;
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The merge into test.
        /// </summary>
        [TestMethod]
        public void DeleteUnmatchedInSource_ItemsWithoutIdentityColumn_CommandTextMatchesExpected()
        {
            var commandProvider = new Mock<ITableCommandProvider>();
            var databaseContext = new Mock<IDatabaseContext>();
            var repositoryAdapter = new Mock<IRepositoryAdapter>();
            repositoryAdapter.Setup(adapter => adapter.NameQualifier).Returns(new TransactSqlQualifier());
            repositoryAdapter.Setup(adapter => adapter.DefinitionProvider).Returns(new DataAnnotationsDefinitionProvider());
            databaseContext.Setup(context => context.RepositoryAdapter).Returns(repositoryAdapter.Object);
            commandProvider.Setup(provider => provider.DatabaseContext).Returns(databaseContext.Object);
            commandProvider.Setup(provider => provider.DatabaseContext).Returns(databaseContext.Object);
            commandProvider
                .Setup(
                    provider => provider.CreateCommand(
                        It.IsAny<ITableCommand>(),
                        It.IsAny<IEnumerable<GenericSubmissionValueTableTypeRow>>(),
                        It.IsAny<IDbTransaction>()))
                .Returns(new Mock<IDbCommand>().Object);

            var expected = @"DECLARE @inserted GenericSubmissionValueTableType;
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
SELECT tvp.[GenericSubmissionValueId], tvp.[GenericSubmissionId]
FROM @inserted AS i
INNER JOIN @GenericSubmissionValueRows AS tvp
ON i.[GenericSubmissionValueId] = tvp.[GenericSubmissionValueId];" + Environment.NewLine;

            var target = new TableValuedMerge<GenericSubmissionValueRow>(commandProvider.Object);
            target.OnImplicit(row => row.GenericSubmissionValueId)
                .SelectFromInserted(row => row.GenericSubmissionValueId)
                .DeleteUnmatchedInSource<GenericSubmissionValueTableTypeRow>(row => row.GenericSubmissionId);

            target.Execute(new List<GenericSubmissionValueTableTypeRow>());

            Assert.AreEqual(expected, target.CommandText);
        }

        /// <summary>
        /// The merge into test.
        /// </summary>
        [TestMethod]
        public void OnFrom_RelatedItems_CommandTextMatchesExpected()
        {
            var commandProvider = new Mock<ITableCommandProvider>();
            var databaseContext = new Mock<IDatabaseContext>();
            var repositoryAdapter = new Mock<IRepositoryAdapter>();
            repositoryAdapter.Setup(adapter => adapter.NameQualifier).Returns(new TransactSqlQualifier());
            repositoryAdapter.Setup(adapter => adapter.DefinitionProvider).Returns(new DataAnnotationsDefinitionProvider());
            databaseContext.Setup(context => context.RepositoryAdapter).Returns(repositoryAdapter.Object);
            commandProvider.Setup(provider => provider.DatabaseContext).Returns(databaseContext.Object);
            commandProvider.Setup(provider => provider.DatabaseContext).Returns(databaseContext.Object);
            commandProvider
                .Setup(
                    provider => provider.CreateCommand(
                        It.IsAny<ITableCommand>(),
                        It.IsAny<IEnumerable<FieldValueElementTableTypeRow>>(),
                        It.IsAny<IDbTransaction>()))
                .Returns(new Mock<IDbCommand>().Object);

            const string Expected = @"DECLARE @inserted FieldValueElementTableType;
MERGE [dbo].[DateElement] AS [Target]
USING @FieldValueElementRows AS [Source]
ON ([Target].[DateElementId] = [Source].[FieldValueElementId])
WHEN MATCHED THEN
UPDATE SET [Value] = [Source].[DateElement]
WHEN NOT MATCHED BY TARGET THEN
INSERT ([DateElementId], [Value])
VALUES ([Source].[FieldValueElementId], [Source].[DateElement])
;
";
            var target = new TableValuedMerge<DateElementRow>(commandProvider.Object)
                .On<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.DateElementId)
                .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.DateElement);

            target.Execute(new List<FieldValueElementTableTypeRow>());

            Assert.AreEqual(Expected, target.CommandText);
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
            using (var provider = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
            {
                this.MergeSubmission(expected, provider);

                var submissionRepository = new EntityRepository<GenericSubmission, GenericSubmissionRow>(provider, this.mapper);
                actual = submissionRepository.FirstOrDefault(expected.GenericSubmissionId);

                var fieldValueRepository = new EntityRepository<FieldValue, GenericSubmissionValueRow>(provider, this.mapper);
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
            var commandProvider = new TableValuedCommandProvider(provider.DatabaseContext);
            var transaction = provider.StartTransaction();

            var fieldsCommand = new TableValuedMerge<FieldRow>(commandProvider, transaction);
            var mergedFields = fieldsCommand.OnImplicit(row => row.Name).SelectFromInserted(row => row.Name).ExecuteForResults(fieldItems).ToList();

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
            var fieldValueCommand = new TableValuedMerge<FieldValueRow>(commandProvider, transaction);
            var mergedFieldValues = fieldValueCommand.OnImplicit(row => row.FieldValueId)
                .SelectFromInserted(row => row.FieldId)
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

            var elementMergeCommand = new TableValuedMerge<FieldValueElementRow>(commandProvider, transaction);
            var mergedValueElements = elementMergeCommand.OnImplicit(row => row.FieldValueId, row => row.Order)
                .DeleteUnmatchedInSource<FieldValueElementTableTypeRow>(row => row.FieldValueId) // Get rid of extraneous elements
                .SelectFromInserted(row => row.FieldValueId, row => row.Order) // Generally this is the same as the MERGE INTO 
                .ExecuteForResults(valueElements)
                .ToList();

            foreach (var element in valueElements)
            {
                var input = mergedValueElements.First(row => row.FieldValueId == element.FieldValueId && row.Order == element.Order);
                this.mapper.MapTo(input, element);
                Assert.IsTrue(element.FieldValueElementId.HasValue);
            }

            var dateElementsCommand = new TableValuedMerge<DateElementRow>(commandProvider, transaction)
                .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.DateElement)
                .On<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.DateElementId);

            dateElementsCommand.Execute(valueElements.Where(row => row.DateElement.HasValue));

            var floatElementsCommand = new TableValuedMerge<FloatElementRow>(commandProvider, transaction)
                .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.FloatElement)
                .On<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.FloatElementId);

            floatElementsCommand.Execute(valueElements.Where(row => row.FloatElement.HasValue));

            var integerElementsCommand = new TableValuedMerge<IntegerElementRow>(commandProvider, transaction)
                .On<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.IntegerElementId)
                .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.IntegerElement);

            integerElementsCommand.Execute(valueElements.Where(row => row.IntegerElement.HasValue));

            var moneyElementsCommand = new TableValuedMerge<MoneyElementRow>(commandProvider, transaction)
                .On<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.MoneyElementId)
                .From<FieldValueElementTableTypeRow>(row => row.FieldValueElementId, row => row.MoneyElement);

            moneyElementsCommand.Execute(valueElements.Where(row => row.MoneyElement.HasValue));

            var textElementsCommand = new TableValuedMerge<TextElementRow>(commandProvider, transaction)
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

            var submissionCommand = new TableValuedMerge<GenericSubmissionValueRow>(commandProvider, transaction)
                .OnImplicit(row => row.GenericSubmissionValueId)
                .DeleteUnmatchedInSource<GenericSubmissionValueTableTypeRow>(row => row.GenericSubmissionId);

            submissionCommand.Execute(genericValueSubmissions);
            transaction.Commit();
        }
    }
}