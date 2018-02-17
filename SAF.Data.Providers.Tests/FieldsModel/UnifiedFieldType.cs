// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnifiedFieldType.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The unified field type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    /// <summary>
    /// The unified field type.
    /// </summary>
    public enum UnifiedFieldType
    {
        /// <summary>
        /// The field type is unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The field type is a checkbox.
        /// </summary>
        CheckBox = 1,

        /// <summary>
        /// The field type is a radio button.
        /// </summary>
        RadioButton = 2,

        /// <summary>
        /// The field type is a date picker.
        /// </summary>
        DatePicker = 3,

        /// <summary>
        /// The field type is a pick list.
        /// </summary>
        PickList = 4,

        /// <summary>
        /// The field type is a drop down list.
        /// </summary>
        DropDownList = 5,

        /// <summary>
        /// The field type is a single field.
        /// </summary>
        SingleField = 6,

        /// <summary>
        /// The field type is a numerical picker.
        /// </summary>
        NumericalPicker = 7,

        /// <summary>
        /// The field type is a text box.
        /// </summary>
        TextBox = 8
    }
}
