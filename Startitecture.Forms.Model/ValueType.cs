// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueType.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Forms.Model
{
    /// <summary>
    /// The value type.
    /// </summary>
    public enum ValueType
    {
        /// <summary>
        /// The value type is unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The value type is a discrete number.
        /// </summary>
        DiscreteNumber = 1,

        /// <summary>
        /// The value type is a floating number.
        /// </summary>
        FloatingNumber = 2,

        /// <summary>
        /// The value type is a currency.
        /// </summary>
        Currency = 3,

        /// <summary>
        /// The value type is a date.
        /// </summary>
        Date = 4,

        /// <summary>
        /// The value type is a text.
        /// </summary>
        Text = 5,

        /// <summary>
        /// The value type is a document.
        /// </summary>
        Document = 6,

        /// <summary>
        /// The value type is a reference.
        /// </summary>
        Reference = 7
    }
}