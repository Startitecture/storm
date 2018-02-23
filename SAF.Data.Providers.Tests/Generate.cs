// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Generate.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SAF.Data.Providers.Tests.FieldsModel;
    using SAF.Data.Providers.Tests.PM;

    using Startitecture.Orm.Common;
    using Startitecture.Orm.Sql;
    using Startitecture.Orm.Testing.Model.Models;

    /// <summary>
    /// The generate.
    /// </summary>
    internal class Generate
    {
        /// <summary>
        /// The create form submission.
        /// </summary>
        /// <param name="person">
        /// The person.
        /// </param>
        /// <param name="withIds">
        /// The with ids.
        /// </param>
        /// <param name="draft">
        /// The draft.
        /// </param>
        /// <returns>
        /// The <see cref="FormSubmission"/>.
        /// </returns>
        public static FormSubmission CreateFormSubmission(Person person, bool withIds, bool draft)
        {
            var personId = person.PersonId.GetValueOrDefault();
            var expected = withIds
                               ? new FormSubmission(2, personId, DateTimeOffset.Now, 234587) { Name = "My First Submission" }
                               : new FormSubmission(2, personId) { Name = "My First Submission" };

            var unifiedField1 = new UnifiedField("MyIntegerField", UnifiedFieldType.PickList, UnifiedValueType.Integer, 123498)
            {
                Caption = "Mah Integerz"
            };

            var unifiedField2 = new UnifiedField("MyTextField", UnifiedFieldType.SingleField, UnifiedValueType.Text, 34652) { Caption = "Mah Textz" };

            var unifiedField3 = new UnifiedField("MyDecimalField", UnifiedFieldType.SingleField, UnifiedValueType.Decimal, 98567)
            {
                Caption = "Mah Decimals"
            };

            var unifiedField4 = new UnifiedField("MyCurrencyField", UnifiedFieldType.SingleField, UnifiedValueType.Currency, 35467)
            {
                Caption = "Mah Decimals"
            };

            var unifiedField5 = new UnifiedField("MyDateField", UnifiedFieldType.DatePicker, UnifiedValueType.Date, 41564) { Caption = "Mah Dates" };

            var unifiedField6 = new UnifiedField("MyBlankField", UnifiedFieldType.SingleField, UnifiedValueType.Text, 389725)
            {
                Caption = "Mah Blanks"
            };

            var formSubmissionValues = new List<FormSubmissionValue>();

            if (withIds)
            {
                var formSubmissionValue1 = new FormSubmissionValue(unifiedField1, expected.FormSubmissionId.GetValueOrDefault(), 4752);
                formSubmissionValue1.SetValues(
                    person,
                    new EnumeratedValue(94) { Name = "First", SortOrder = 1 },
                    new EnumeratedValue(28) { Name = "Second", SortOrder = 3 },
                    new EnumeratedValue(29034) { Name = "Whatever", SortOrder = 99 },
                    new EnumeratedValue(993) { Name = "Foo", SortOrder = 5 });

                var formSubmissionValue2 = new FormSubmissionValue(unifiedField2, expected.FormSubmissionId.GetValueOrDefault(), 3562);
                formSubmissionValue2.SetValues(person, "SomeText1", "SomeText3", "SomeText2");

                var formSubmissionValue3 = new FormSubmissionValue(unifiedField3, expected.FormSubmissionId.GetValueOrDefault(), 67454);
                formSubmissionValue3.SetValues(person, 4930.903459, 23957.349, 9433.354);

                var formSubmissionValue4 = new FormSubmissionValue(unifiedField4, expected.FormSubmissionId.GetValueOrDefault(), 7842);
                formSubmissionValue4.SetValues(person, 10.00, 94.99, 101000.00);

                var formSubmissionValue5 = new FormSubmissionValue(unifiedField5, expected.FormSubmissionId.GetValueOrDefault(), 2345);
                formSubmissionValue5.SetValues(person, DateTimeOffset.Now, DateTimeOffset.Now.AddDays(4).Date, DateTimeOffset.Now.AddMinutes(5));

                var formSubmissionValue6 = new FormSubmissionValue(unifiedField6, expected.FormSubmissionId.GetValueOrDefault(), 487539);
                formSubmissionValue6.SetValues(person);

                formSubmissionValues.AddRange(
                    new[]
                        {
                            formSubmissionValue1, formSubmissionValue2, formSubmissionValue3, formSubmissionValue4, formSubmissionValue5,
                            formSubmissionValue6
                        });
            }
            else
            {
                var formSubmissionValue1 = new FormSubmissionValue(unifiedField1);
                formSubmissionValue1.SetValues(
                    person,
                    new EnumeratedValue(94) { Name = "First", SortOrder = 1 },
                    new EnumeratedValue(28) { Name = "Second", SortOrder = 3 },
                    new EnumeratedValue(29034) { Name = "Whatever", SortOrder = 99 },
                    new EnumeratedValue(993) { Name = "Foo", SortOrder = 5 });

                var formSubmissionValue2 = new FormSubmissionValue(unifiedField2);
                formSubmissionValue2.SetValues(person, "SomeText1", "SomeText3", "SomeText2");

                var formSubmissionValue3 = new FormSubmissionValue(unifiedField3);
                formSubmissionValue3.SetValues(person, 4930.903459, 23957.349, 9433.354);

                var formSubmissionValue4 = new FormSubmissionValue(unifiedField4);
                formSubmissionValue4.SetValues(person, 10.00, 94.99, 101000.00);

                var formSubmissionValue5 = new FormSubmissionValue(unifiedField5);
                formSubmissionValue5.SetValues(person, DateTimeOffset.Now, DateTimeOffset.Now.AddDays(4).Date, DateTimeOffset.Now.AddMinutes(5));

                var formSubmissionValue6 = new FormSubmissionValue(unifiedField6, expected.FormSubmissionId.GetValueOrDefault(), 487539);
                formSubmissionValue6.SetValues(person);

                formSubmissionValues.AddRange(
                    new[]
                        {
                            formSubmissionValue1, formSubmissionValue2, formSubmissionValue3, formSubmissionValue4, formSubmissionValue5,
                            formSubmissionValue6
                        });
            }

            if (draft)
            {
                expected.SaveDraft(person, formSubmissionValues.ToArray());
            }
            else
            {
                expected.Submit(person, formSubmissionValues.ToArray());
            }

            return expected;
        }

        /// <summary>
        /// The cleanup data.
        /// </summary>
        /// <param name="provider">
        /// The provider factory.
        /// </param>
        /// <param name="submissionId">
        /// The submission id.
        /// </param>
        public static void CleanupData(IRepositoryProvider provider, long submissionId)
        {
            var example = new FormSubmissionValueRow { FormSubmissionId = submissionId };
            var deleteSelection =
                Select.From<UnifiedFieldValueRow>()
                    .InnerJoin<FormSubmissionValueRow>(row => row.UnifiedFieldValueId, row => row.FormSubmissionValueId)
                    .Matching(example, row => row.FormSubmissionId);

            provider.DeleteItems(deleteSelection);
        }

        /// <summary>
        /// The get from submission values.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="person">
        /// The person.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/> of form submission values.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The value type enumeration is not labeled.
        /// </exception>
        public static IEnumerable<FormSubmissionValue> GetFormSubmissionValues(IRepositoryProvider provider, Person person)
        {
            var unifiedFieldRepository = new UnifiedFieldRepository(provider);
            var unifiedFields = unifiedFieldRepository.SelectAll().ToList();

            foreach (var unifiedField in unifiedFields)
            {
                if (unifiedField.IsUserSourcedField)
                {
                    var formSubmissionValue = SetUserSourcedValues(person, unifiedField);

                    if (formSubmissionValue != null)
                    {
                        yield return formSubmissionValue;
                    }
                }
                else
                {
                    var formSubmissionValue = new FormSubmissionValue(unifiedField);
                    formSubmissionValue.SetValues(
                        person,
                        new EnumeratedValue(34509) { Name = "Value 1", SortOrder = 3 },
                        new EnumeratedValue(320948) { Name = "Value 2", SortOrder = 5 },
                        new EnumeratedValue(6738) { Name = "Value 3", SortOrder = 6 });
                }
            }
        }

        /// <summary>
        /// The set user sourced values.
        /// </summary>
        /// <param name="person">
        /// The person.
        /// </param>
        /// <param name="unifiedField">
        /// The unified field.
        /// </param>
        /// <returns>
        /// The <see cref="FormSubmissionValue"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The enumeration isn't handled.
        /// </exception>
        public static FormSubmissionValue SetUserSourcedValues(Person person, UnifiedField unifiedField)
        {
            switch (unifiedField.UnifiedValueType)
            {
                case UnifiedValueType.Integer:
                    var integerValue = new FormSubmissionValue(unifiedField);
                    integerValue.SetValues(person, 58, 739843, 987);

                    return integerValue;
                case UnifiedValueType.Decimal:
                case UnifiedValueType.Currency:
                    var doubleValue = new FormSubmissionValue(unifiedField);
                    doubleValue.SetValues(person, 9345.3453, 24.6, 92.994);
                    return doubleValue;
                case UnifiedValueType.Date:
                    var dateValue = new FormSubmissionValue(unifiedField);
                    dateValue.SetValues(
                        person,
                        DateTimeOffset.Now.AddMinutes(9028),
                        DateTimeOffset.Now.AddMinutes(-2134435),
                        DateTimeOffset.Now.AddMinutes(843));

                    return dateValue;
                case UnifiedValueType.Text:
                    var textValue = new FormSubmissionValue(unifiedField);
                    textValue.SetValues(person, "Test 1", "Test 3", "Test 4");
                    return textValue;
                case UnifiedValueType.Attachment:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// The create form submission.
        /// </summary>
        /// <param name="entityMapper">
        /// The entity mapper.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <returns>
        /// The <see cref="FormSubmission"/>.
        /// </returns>
        public static FormSubmission CreateFormSubmission(IEntityMapper entityMapper, IRepositoryProvider provider)
        {
            var person = GetPerson(provider);
            var expected = CreateFormSubmission(person, entityMapper, provider);

            var formSubmissionValues = GetFormSubmissionValues(provider, person).ToList();

            expected.SaveDraft(person, formSubmissionValues.ToArray());
            return expected;
        }

        /// <summary>
        /// The create form submission.
        /// </summary>
        /// <param name="person">
        /// The person.
        /// </param>
        /// <param name="entityMapper">
        /// The entity mapper.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <returns>
        /// The <see cref="FormSubmission"/>.
        /// </returns>
        public static FormSubmission CreateFormSubmission(Person person, IEntityMapper entityMapper, IRepositoryProvider provider)
        {
            var formSubmissionRow = new FormSubmissionRow
                                        {
                                            Name = "UNIT TEST",
                                            ProcessFormId = 1,
                                            SubmittedByPersonId = person.PersonId.GetValueOrDefault(),
                                            SubmittedTime = DateTimeOffset.Now
                                        };

            formSubmissionRow = provider.InsertItem(formSubmissionRow);

            var processFormSubmissionRow = new ProcessFormSubmissionRow
                                               {
                                                   ProcessFormSubmissionId = formSubmissionRow.FormSubmissionId,
                                                   ProcessFormId = 1,
                                                   Name = formSubmissionRow.Name,
                                                   SubmittedByPersonId = formSubmissionRow.SubmittedByPersonId,
                                                   SubmittedTime = formSubmissionRow.SubmittedTime
                                               };

            provider.InsertItem(processFormSubmissionRow);
            var expected = entityMapper.Map<FormSubmission>(formSubmissionRow);
            return expected;
        }

        /// <summary>
        /// The get person.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <returns>
        /// The <see cref="Startitecture.Orm.Testing.Model.Person"/>.
        /// </returns>
        private static Person GetPerson(IRepositoryProvider provider)
        {
            var personRepository = new PersonRepository(provider);
            var selection = Select.From<ActionPrincipalRow>();
            selection.Limit = 1;
            var person = personRepository.SelectPeople(selection).First();
            return person;
        }
    }
}