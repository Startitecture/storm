// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnifiedValueType.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    /// <summary>
    /// The unified value type.
    /// </summary>
    public enum UnifiedValueType
    {
        /// <summary>
        /// The value type is unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The value is an integer.
        /// </summary>
        Integer = 1,

        /// <summary>
        /// The value is a decimal.
        /// </summary>
        Decimal = 2,

        /// <summary>
        /// The value is currency.
        /// </summary>
        Currency = 3,

        /// <summary>
        /// The value is a date.
        /// </summary>
        Date = 4,

        /// <summary>
        /// The value is text.
        /// </summary>
        Text = 5,

        /// <summary>
        /// The value is an attachment.
        /// </summary>
        Attachment = 6
    }
}