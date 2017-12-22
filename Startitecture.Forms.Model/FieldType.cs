// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldType.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Forms.Model
{
    /// <summary>
    /// The field type.
    /// </summary>
    public enum FieldType
    {
        /// <summary>
        /// The field type is unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The field type is a textbox.
        /// </summary>
        Textbox = 1,

        /// <summary>
        /// The field type is a checkbox.
        /// </summary>
        Checkbox = 2,

        /// <summary>
        /// The field type is a radio button.
        /// </summary>
        RadioButton = 3,

        /// <summary>
        /// The field type is a date picker.
        /// </summary>
        DatePicker = 4,

        /// <summary>
        /// The field type is a single-select drop down.
        /// </summary>
        DropDownSingle = 5,

        /// <summary>
        /// The field type is a multi-select drop down.
        /// </summary>
        DropDownMultiple = 6,

        /// <summary>
        /// The field type is a large text box.
        /// </summary>
        LargeTextBox = 7
    }
}