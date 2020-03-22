// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Generate.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The generate.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Startitecture.Core;
    using Startitecture.Orm.Testing.Model;

    /// <summary>
    /// The generate.
    /// </summary>
    internal static class Generate
    {
        /// <summary>
        /// The number generator.
        /// </summary>
        public static readonly Random NumberGenerator = new Random();

        /// <summary>
        /// The system field sources.
        /// </summary>
        public static readonly List<Type> SystemFieldSources = new List<Type>
                                                                   {
                                                                       typeof(Contract),
                                                                       typeof(ContractType),
                                                                       typeof(Department),
                                                                       typeof(ContractingEntity),
                                                                       typeof(ExternalEntity),
                                                                       typeof(Market),
                                                                       typeof(Site),
                                                                       typeof(WorkflowInstance)
                                                                   };

        /// <summary>
        /// The number types.
        /// </summary>
        private static readonly List<Type> NumberTypes =
            new List<Type> { typeof(long), typeof(int), typeof(short), typeof(decimal), typeof(double), typeof(float) };

        /// <summary>
        /// The unified property types.
        /// </summary>
        private static readonly List<Type> UnifiedPropertyTypes =
            new List<Type> { typeof(long), typeof(double), typeof(decimal), typeof(DateTimeOffset), typeof(string) };

        /// <summary>
        /// The numeric field types.
        /// </summary>
        private static readonly List<UnifiedFieldType> NumericFieldTypes =
            new List<UnifiedFieldType>
                {
                    UnifiedFieldType.SingleField,
                    UnifiedFieldType.DropDownList,
                    UnifiedFieldType.PickList,
                    UnifiedFieldType.NumericalPicker,
                    UnifiedFieldType.CheckBox,
                    UnifiedFieldType.RadioButton
                };

        /// <summary>
        /// The text field types.
        /// </summary>
        private static readonly List<UnifiedFieldType> TextFieldTypes =
            new List<UnifiedFieldType>
                {
                    UnifiedFieldType.SingleField,
                    UnifiedFieldType.DropDownList,
                    UnifiedFieldType.PickList,
                    UnifiedFieldType.CheckBox,
                    UnifiedFieldType.RadioButton,
                    UnifiedFieldType.TextBox
                };

        /// <summary>
        /// Creates a form submission.
        /// </summary>
        /// <param name="submissionExists">
        /// A value indicating whether the submission already exists.
        /// </param>
        /// <param name="isDraft">
        /// A value indicating whether to create the submission as a draft.
        /// </param>
        /// <returns>
        /// A <see cref="FormSubmission"/> instance.
        /// </returns>
        public static FormSubmission CreateFormSubmission(bool submissionExists, bool isDraft)
        {
            return CreateFormSubmission(submissionExists, isDraft, false);
        }

        /// <summary>
        /// Creates a form submission.
        /// </summary>
        /// <param name="submissionExists">
        /// A value indicating whether the submission already exists.
        /// </param>
        /// <param name="isDraft">
        /// A value indicating whether to create the submission as a draft.
        /// </param>
        /// <param name="valuesExist">
        /// A value indicating whether the values already exist.
        /// </param>
        /// <returns>
        /// A <see cref="FormSubmission"/> instance.
        /// </returns>
        public static FormSubmission CreateFormSubmission(bool submissionExists, bool isDraft, bool valuesExist)
        {
            var person = new Testing.Model.PM.Person(2345) { FirstName = "Some", LastName = "Dood" };

            var submission = submissionExists
                                 ? new FormSubmission(person, DateTimeOffset.MinValue, 9843) { Name = "MySubmission" }
                                 : new FormSubmission { Name = "MySubmission" };

            var formSubmissionValues = CreateFormSubmissionValues(valuesExist, submission, person).ToList();

            if (isDraft)
            {
                submission.SaveDraft(person, formSubmissionValues.ToArray());
            }
            else
            {
                submission.Submit(person, formSubmissionValues.ToArray());
            }

            return submission;
        }

/*
        /// <summary>
        /// Creates form submission values.
        /// </summary>
        /// <param name="person">
        /// The person who submitted the submission.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="FormSubmissionValue"/> items.
        /// </returns>
        public static IEnumerable<UnifiedFieldValue> CreateUnifiedFieldValues(Testing.Model.PM.Person person)
        {
            var unifiedDate =
                new UnifiedField("MyDateField", UnifiedFieldType.DatePicker, UnifiedValueType.Date, 459345)
                    {
                        Caption =
                            "My date field that I like to use",
                        Label = "Put in the date here",
                        SourceType = "Custom",
                        ToolTip =
                            "Enter in a date so that it will be saved"
                    };

            var unifiedIdentifier =
                new UnifiedField("User.UserId", UnifiedFieldType.DropDownList, UnifiedValueType.Identifier, 4343654)
                    {
                        Caption =
                            "My dropdown user field that I like to use",
                        Label =
                            "Put in the users here",
                        SourceType =
                            "Khepri.Common.Model.User",
                        ToolTip =
                            "Enter the users so that they will be saved"
                    };

            var unifiedEnumeration =
                new UnifiedField("MyCustomEnum", UnifiedFieldType.PickList, UnifiedValueType.Integer, 63297)
                    {
                        Caption =
                            "My enumeration field that I like to use",
                        Label =
                            "Put the enumerations in here",
                        SourceType = "Custom",
                        ToolTip =
                            "Enter the enumerations so they will be saved"
                    };

            var unifiedInteger =
                new UnifiedField("MyIntegerField", UnifiedFieldType.SingleField, UnifiedValueType.Integer, 8756)
                    {
                        Caption =
                            "My integer field that I like to use",
                        Label =
                            "Put in the integer here",
                        SourceType = "Custom",
                        ToolTip =
                            "Enter in an integer so that it will be saved"
                    };

            var unifiedCurrency =
                new UnifiedField("MyCurrencyField", UnifiedFieldType.SingleField, UnifiedValueType.Currency, 3243235)
                    {
                        Caption =
                            "My currency field that I like to use",
                        Label =
                            "Put in the currency here",
                        SourceType = "Custom",
                        ToolTip =
                            "Enter in a currency so that it will be saved"
                    };

            var unifiedNumeric =
                new UnifiedField("MyNumericField", UnifiedFieldType.SingleField, UnifiedValueType.Numeric, 98745)
                    {
                        Caption =
                            "My numeric field that I like to use",
                        Label =
                            "Put in the number here",
                        SourceType = "Custom",
                        ToolTip =
                            "Enter in a number so that it will be saved"
                    };

            var unifiedText =
                new UnifiedField("MyTextField", UnifiedFieldType.TextBox, UnifiedValueType.Text, 4434345)
                    {
                        Caption =
                            "My text field that I like to use",
                        Label = "Put in the text here",
                        SourceType = "Custom",
                        ToolTip =
                            "Enter in some text so that it will be saved"
                    };

            var dateValue = new UnifiedFieldValue(unifiedDate);

            dateValue.SetValues(person, DateTimeOffset.Now.AddDays(-7), DateTimeOffset.Now.AddDays(-3), DateTimeOffset.Now);

            var identifierValue = new UnifiedFieldValue(unifiedIdentifier);
            identifierValue.SetValues(person, 46, 3934, 584);

            var enumerationValue = new UnifiedFieldValue(unifiedEnumeration);
            var unifiedSourceValue1 =
                new UnifiedSourceValue(unifiedEnumeration, 392774)
                    {
                        Caption = "Oh Caption My Caption",
                        Name = "Enum1",
                        Order = 1,
                        StringValue = "56"
                    };

            var unifiedSourceValue2 =
                new UnifiedSourceValue(unifiedEnumeration, 3242323) { Caption = "Caption Planet", Name = "Enum2", Order = 2, StringValue = "65" };

            var unifiedSourceValue3 =
                new UnifiedSourceValue(unifiedEnumeration, 76576657) { Caption = "Caption America", Name = "Enum3", Order = 3, StringValue = "23" };

            enumerationValue.SetValues(person, unifiedSourceValue1, unifiedSourceValue2, unifiedSourceValue3);

            var integerValue = new UnifiedFieldValue(unifiedInteger);
            integerValue.SetValues(person, 4598722345, 83435, 4343);

            var currencyValue = new UnifiedFieldValue(unifiedCurrency);
            currencyValue.SetValues(person, 23452.33D, 224.56D, 108.01D);

            var numericValue = new UnifiedFieldValue(unifiedNumeric);
            numericValue.SetValues(person, 32789.332, 32734598.4395234598, 3453.8345);

            var textValue = new UnifiedFieldValue(unifiedText);
            textValue.SetValues(person, "Test1", "Test2", "alkjfaslkjdsa");

            return new List<UnifiedFieldValue> { dateValue, identifierValue, enumerationValue, integerValue, currencyValue, numericValue, textValue };
        }
*/

        /// <summary>
        /// Creates form submission values.
        /// </summary>
        /// <param name="valuesExist">
        /// A value indicating whether the form submission values exist in the repository.
        /// </param>
        /// <param name="submission">
        /// The submission to add the values to.
        /// </param>
        /// <param name="person">
        /// The person who submitted the submission.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="FormSubmissionValue"/> items.
        /// </returns>
        public static IEnumerable<FormSubmissionValue> CreateFormSubmissionValues(bool valuesExist, FormSubmission submission, Testing.Model.PM.Person person)
        {
            var unifiedDate =
                new UnifiedField("MyDateField", UnifiedFieldType.DatePicker, UnifiedValueType.Date, 459345)
                    {
                        Caption =
                            "My date field that I like to use",
                        Label = "Put in the date here",
                        SourceType = "Custom",
                        ToolTip =
                            "Enter in a date so that it will be saved"
                    };

            var unifiedIdentifier =
                new UnifiedField("User.UserId", UnifiedFieldType.DropDownList, UnifiedValueType.Identifier, 4343654)
                    {
                        Caption =
                            "My dropdown user field that I like to use",
                        Label =
                            "Put in the users here",
                        SourceType =
                            "Khepri.Common.Model.User",
                        ToolTip =
                            "Enter the users so that they will be saved"
                    };

            var unifiedEnumeration =
                new UnifiedField("MyCustomEnum", UnifiedFieldType.PickList, UnifiedValueType.Integer, 63297)
                    {
                        Caption =
                            "My enumeration field that I like to use",
                        Label =
                            "Put the enumerations in here",
                        SourceType = "Custom",
                        ToolTip =
                            "Enter the enumerations so they will be saved"
                    };

            var unifiedInteger =
                new UnifiedField("MyIntegerField", UnifiedFieldType.SingleField, UnifiedValueType.Integer, 8756)
                    {
                        Caption =
                            "My integer field that I like to use",
                        Label =
                            "Put in the integer here",
                        SourceType = "Custom",
                        ToolTip =
                            "Enter in an integer so that it will be saved"
                    };

            var unifiedCurrency =
                new UnifiedField("MyCurrencyField", UnifiedFieldType.SingleField, UnifiedValueType.Currency, 3243235)
                    {
                        Caption =
                            "My currency field that I like to use",
                        Label =
                            "Put in the currency here",
                        SourceType = "Custom",
                        ToolTip =
                            "Enter in a currency so that it will be saved"
                    };

            var unifiedNumeric =
                new UnifiedField("MyNumericField", UnifiedFieldType.SingleField, UnifiedValueType.Numeric, 98745)
                    {
                        Caption =
                            "My numeric field that I like to use",
                        Label =
                            "Put in the number here",
                        SourceType = "Custom",
                        ToolTip =
                            "Enter in a number so that it will be saved"
                    };

            var unifiedText =
                new UnifiedField("MyTextField", UnifiedFieldType.TextBox, UnifiedValueType.Text, 4434345)
                    {
                        Caption =
                            "My text field that I like to use",
                        Label = "Put in the text here",
                        SourceType = "Custom",
                        ToolTip =
                            "Enter in some text so that it will be saved"
                    };

            var dateValue = valuesExist
                                ? new FormSubmissionValue(unifiedDate, submission, person, DateTimeOffset.Now, 293423)
                                : new FormSubmissionValue(unifiedDate, submission, person, DateTimeOffset.Now, null);

            dateValue.LoadValues(DateTimeOffset.Now.AddDays(-7), DateTimeOffset.Now.AddDays(-3), DateTimeOffset.Now);

            var identifierValue = valuesExist
                                      ? new FormSubmissionValue(unifiedIdentifier, submission, person, DateTimeOffset.Now, 457982342)
                                      : new FormSubmissionValue(unifiedIdentifier, submission, person, DateTimeOffset.Now, null);

            identifierValue.LoadValues(46, 3934, 584);

            var enumerationValue = valuesExist
                                       ? new FormSubmissionValue(unifiedEnumeration, submission, person, DateTimeOffset.Now, 54327923)
                                       : new FormSubmissionValue(unifiedEnumeration, submission, person, DateTimeOffset.Now, null);

            var unifiedSourceValue1 =
                new UnifiedSourceValue(unifiedEnumeration, 392774)
                    {
                        Caption = "Oh Caption My Caption",
                        Name = "Enum1",
                        Order = 1,
                        StringValue = "56"
                    };

            var unifiedSourceValue2 =
                new UnifiedSourceValue(unifiedEnumeration, 3242323) { Caption = "Caption Planet", Name = "Enum2", Order = 2, StringValue = "65" };

            var unifiedSourceValue3 =
                new UnifiedSourceValue(unifiedEnumeration, 76576657) { Caption = "Caption America", Name = "Enum3", Order = 3, StringValue = "23" };

            enumerationValue.LoadValues(unifiedSourceValue1, unifiedSourceValue2, unifiedSourceValue3);

            var integerValue = valuesExist
                                   ? new FormSubmissionValue(unifiedInteger, submission, person, DateTimeOffset.Now, 5497322)
                                   : new FormSubmissionValue(unifiedInteger, submission, person, DateTimeOffset.Now, null);

            integerValue.LoadValues(4598722345, 83435, 4343);

            var currencyValue = valuesExist
                                    ? new FormSubmissionValue(unifiedCurrency, submission, person, DateTimeOffset.Now, 43729347)
                                    : new FormSubmissionValue(unifiedCurrency, submission, person, DateTimeOffset.Now, null);

            currencyValue.LoadValues(23452.33D, 224.56D, 108.01D);

            var numericValue = valuesExist
                                   ? new FormSubmissionValue(unifiedNumeric, submission, person, DateTimeOffset.Now, 342324)
                                   : new FormSubmissionValue(unifiedNumeric, submission, person, DateTimeOffset.Now, null);

            numericValue.LoadValues(32789.332, 32734598.4395234598, 3453.8345);

            var textValue = valuesExist
                                ? new FormSubmissionValue(unifiedText, submission, person, DateTimeOffset.Now, 5280932)
                                : new FormSubmissionValue(unifiedText, submission, person, DateTimeOffset.Now, null);

            textValue.LoadValues("Test1", "Test2", "alkjfaslkjdsa");

            return new List<FormSubmissionValue>
                       {
                           dateValue,
                           identifierValue,
                           enumerationValue,
                           integerValue,
                           currencyValue,
                           numericValue,
                           textValue
                       };
        }

        /// <summary>
        /// Generates a system field.
        /// </summary>
        /// <param name="info">
        /// The <see cref="PropertyInfo"/> for the field.
        /// </param>
        /// <returns>
        /// A <see cref="UnifiedField"/> representing the <paramref name="info"/>.
        /// </returns>
        public static UnifiedField GenerateSystemField(PropertyInfo info)
        {
            return new UnifiedField($"{info.DeclaringType}-{info.Name}", GetFieldType(info), GetValueType(info))
                       {
                           Caption = info.Name,
                           IsCustom = false,
                           Label = info.Name,
                           SourceType =
                               (info.ReflectedType
                                ?? info.DeclaringType)
                               ?.FullName
                       };
        }

        /// <summary>
        /// Generates a custom field.
        /// </summary>
        /// <param name="index">
        /// The index of the current field generation operation.
        /// </param>
        /// <returns>
        /// A <see cref="UnifiedField"/>.
        /// </returns>
        public static UnifiedField GenerateCustomField(int index)
        {
            var sourceType = SystemFieldSources.ElementAt(index % SystemFieldSources.Count);
            var propertyType = UnifiedPropertyTypes.ElementAt(index % UnifiedPropertyTypes.Count);

            UnifiedFieldType fieldType;
            var valueType = GetValueType(propertyType);

            if (propertyType == typeof(DateTimeOffset))
            {
                fieldType = UnifiedFieldType.DatePicker;
            }
            else if (NumberTypes.Contains(propertyType))
            {
                fieldType = NumericFieldTypes.ElementAt(NumberGenerator.Next(NumericFieldTypes.Count));
            }
            else
            {
                fieldType = TextFieldTypes.ElementAt(NumberGenerator.Next(TextFieldTypes.Count));
            }

            var caption = $"My {fieldType} {valueType} Field #{index}";
            return new UnifiedField($"{sourceType.Name}-{propertyType.Name}-{index}", fieldType, valueType)
                       {
                           SourceType = sourceType.FullName,
                           Caption = caption,
                           IsCustom = true,
                           Label = caption,
                           ToolTip = "This is custom!!!"
                       };
        }

        /// <summary>
        /// Gets the field type.
        /// </summary>
        /// <param name="info">
        /// The <see cref="PropertyInfo"/> for the field.
        /// </param>
        /// <returns>
        /// The <see cref="UnifiedFieldType"/> for the <paramref name="info"/>.
        /// </returns>
        private static UnifiedFieldType GetFieldType(PropertyInfo info)
        {
            var propertyType = Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType;

            return info.Name == $"{propertyType}Id" ? UnifiedFieldType.DropDownList : GetFieldType(propertyType);
        }

        /// <summary>
        /// Gets the field type.
        /// </summary>
        /// <param name="propertyType">
        /// The property type to get the field type for.
        /// </param>
        /// <returns>
        /// The <see cref="UnifiedFieldType"/> for the <paramref name="propertyType"/>.
        /// </returns>
        /// <exception cref="BusinessException">
        /// <paramref name="propertyType"/> is not supported.
        /// </exception>
        private static UnifiedFieldType GetFieldType(Type propertyType)
        {
            if (NumberTypes.Contains(propertyType))
            {
                return UnifiedFieldType.SingleField;
            }

            if (propertyType == typeof(DateTimeOffset))
            {
                return UnifiedFieldType.DatePicker;
            }

            if (propertyType.IsEnum)
            {
                return UnifiedFieldType.PickList;
            }

            if (propertyType == typeof(string))
            {
                return UnifiedFieldType.TextBox;
            }

            throw new BusinessException(propertyType, FieldsMessages.UnifiedFieldPropertyTypeNotSupported);
        }

        /// <summary>
        /// Gets the value type.
        /// </summary>
        /// <param name="info">
        /// The <see cref="PropertyInfo"/> for the field.
        /// </param>
        /// <returns>
        /// The <see cref="UnifiedValueType"/> for the <paramref name="info"/>.
        /// </returns>
        private static UnifiedValueType GetValueType(PropertyInfo info)
        {
            var propertyType = Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType;

            return info.Name == $"{propertyType}Id" ? UnifiedValueType.Identifier : GetValueType(propertyType);
        }

        /// <summary>
        /// Gets the value type.
        /// </summary>
        /// <param name="propertyType">
        /// The property type to get the value type for.
        /// </param>
        /// <returns>
        /// The <see cref="UnifiedValueType"/>.
        /// </returns>
        /// <exception cref="BusinessException">
        /// <paramref name="propertyType"/> is not supported.
        /// </exception>
        private static UnifiedValueType GetValueType(Type propertyType)
        {
            if (propertyType == typeof(long) || propertyType == typeof(int) || propertyType == typeof(short))
            {
                return UnifiedValueType.Integer;
            }

            if (propertyType == typeof(double) || propertyType == typeof(float))
            {
                return UnifiedValueType.Numeric;
            }

            if (propertyType == typeof(decimal))
            {
                return UnifiedValueType.Currency;
            }

            if (propertyType == typeof(DateTimeOffset))
            {
                return UnifiedValueType.Date;
            }

            if (propertyType.IsEnum)
            {
                return UnifiedValueType.Identifier;
            }

            if (propertyType == typeof(string))
            {
                return UnifiedValueType.Text;
            }

            throw new BusinessException(propertyType, FieldsMessages.UnifiedFieldPropertyTypeNotSupported);
        }
    }
}
