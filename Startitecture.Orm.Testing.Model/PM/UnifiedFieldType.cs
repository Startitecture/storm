// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnifiedFieldType.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model.PM
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
        /// The field type is a drop down.
        /// </summary>
        DropDown = 1,

        /// <summary>
        /// The field type is a pick list.
        /// </summary>
        PickList = 2,

        /// <summary>
        /// The field type is a radio button.
        /// </summary>
        RadioButton = 3,

        /// <summary>
        /// The field type is a checkbox.
        /// </summary>
        Checkbox = 4,

        /// <summary>
        /// The field type is a single field.
        /// </summary>
        SingleField = 5,

        /// <summary>
        /// The field type is a date picker.
        /// </summary>
        DatePicker = 6,

        /// <summary>
        /// The field type is a numerical picker.
        /// </summary>
        NumericalPicker = 7,

        /// <summary>
        /// The field type is a rad upload.
        /// </summary>
        RadUpload = 8,

        /// <summary>
        /// The field type is a multiline field.
        /// </summary>
        MultilineField = 9,

        /// <summary>
        /// The field type is a load on demand dropdown.
        /// </summary>
        LoadOnDemandDropdown = 10,

        /// <summary>
        /// The field type is a popup.
        /// </summary>
        Popup = 11
    }
}