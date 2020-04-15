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

    using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        /// The merge into test.
        /// </summary>
        [TestMethod]
        public void MergeInto_StructuredMergeCommandForFields_ReturnedItemsMatchExpected()
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
        public void DeleteUnmatchedInSource_StructuredMergeCommandForGenericSubmissionValues_CommandTextMatchesExpected()
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
    }
}