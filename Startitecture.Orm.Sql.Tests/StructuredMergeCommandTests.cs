// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructuredMergeCommandTests.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Startitecture.Orm.Sql.Tests
{
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
        public void MergeInto_StructuredMergeCommandForFields_InsertsAndUpdatesExpectedFields()
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
            var structureDefinition = definitionProvider.Resolve<FieldTableTypeRow>();
            var orderedAttributes = structureDefinition.ReturnableAttributes.OrderBy(definition => definition.Ordinal).ToList();

            var commandProvider = mergeItems.MockCommandProvider(orderedAttributes, definitionProvider);

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
    }
}