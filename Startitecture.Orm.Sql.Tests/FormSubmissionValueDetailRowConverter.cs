// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormSubmissionValueDetailRowConverter.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Sql.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Startitecture.Orm.Testing.Model;
    using Startitecture.Orm.Testing.Model.DocumentEntities;
    using Startitecture.Orm.Testing.Model.FieldEntities;
    using Startitecture.Orm.Testing.Model.PM;

    using FormSubmissionValue = Startitecture.Orm.Testing.Model.PM.FormSubmissionValue;

    /// <summary>
    /// The form submission value detail row converter.
    /// </summary>
    public class FormSubmissionValueDetailRowConverter
    {
        /// <summary>
        /// Converts form submission values into form submission value detail rows.
        /// </summary>
        /// <param name="values">
        /// The values to convert
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="FormSubmissionValueDetailRow"/> items.
        /// </returns>
        public IEnumerable<FormSubmissionValueDetailRow> Convert(IEnumerable<FormSubmissionValue> values)
        {
            foreach (var value in values)
            {
                if (value.FieldValues.Any())
                {
                    foreach (var fieldValue in value.FieldValues)
                    {
                        // Handle enumerated values.
                        var enumeratedValue = fieldValue as EnumeratedValue;

                        yield return
                            new FormSubmissionValueDetailRow
                                {
                                    FormSubmissionId = value.FormSubmissionId.GetValueOrDefault(),
                                    FormSubmissionValueId = value.FormSubmissionValueId.GetValueOrDefault(),
                                    LastModifiedPersonId = value.LastModifiedPersonId.GetValueOrDefault(),
                                    UnifiedFieldId = value.UnifiedFieldId.GetValueOrDefault(),
                                    UnifiedFieldValueId = value.UnifiedFieldValueId.GetValueOrDefault(),
                                    UnifiedFieldTypeId = (int)value.UnifiedFieldType,
                                    UnifiedValueTypeId = (int)value.UnifiedValueType,
                                    LastModifiedTime = value.LastModifiedTime,
                                    UnifiedFieldName = value.UnifiedField.Name,
                                    UnifiedFieldCaption = value.UnifiedField.Caption,
                                    UnifiedFieldLabel = value.UnifiedField.Label,
                                    DateValue =
                                        value.UnifiedField.IsUserSourcedField && value.UnifiedField.IsDateValueType
                                            ? fieldValue as DateTimeOffset?
                                            : null,
                                    IntegerValue = enumeratedValue?.EnumeratedValueId,
                                    NumericValue =
                                        value.UnifiedField.IsUserSourcedField && value.UnifiedField.IsNumericValueType
                                            ? System.Convert.ToDouble(fieldValue)
                                            : (double?)null,
                                    StringValue =
                                        value.UnifiedField.IsUserSourcedField && value.UnifiedField.IsTextValueType
                                            ? System.Convert.ToString(fieldValue)
                                            : null,
                                    EnumeratedValueName = enumeratedValue?.Name,
                                    EnumeratedValueSortOrder = enumeratedValue?.SortOrder
                                };
                    }
                }
                else
                {
                    // Return blank values too.
                    yield return
                        new FormSubmissionValueDetailRow
                            {
                                FormSubmissionId = value.FormSubmissionId.GetValueOrDefault(),
                                FormSubmissionValueId = value.FormSubmissionValueId.GetValueOrDefault(),
                                LastModifiedPersonId = value.LastModifiedPersonId.GetValueOrDefault(),
                                UnifiedFieldId = value.UnifiedFieldId.GetValueOrDefault(),
                                UnifiedFieldValueId = value.UnifiedFieldValueId.GetValueOrDefault(),
                                UnifiedFieldTypeId = (int)value.UnifiedFieldType,
                                UnifiedValueTypeId = (int)value.UnifiedValueType,
                                LastModifiedTime = value.LastModifiedTime,
                                UnifiedFieldName = value.UnifiedField.Name,
                                UnifiedFieldCaption = value.UnifiedField.Caption,
                                UnifiedFieldLabel = value.UnifiedField.Label
                            };
                }
            }
        }
    }
}