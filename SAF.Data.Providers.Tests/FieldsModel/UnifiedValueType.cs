// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnifiedValueType.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The unified value type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
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
        /// The value type is an integer.
        /// </summary>
        Integer = 1,

        /// <summary>
        /// The value type is a decimal.
        /// </summary>
        Numeric = 2,

        /// <summary>
        /// The value type is currency.
        /// </summary>
        Currency = 3,

        /// <summary>
        /// The value type is a date.
        /// </summary>
        Date = 4,

        /// <summary>
        /// The value type is text.
        /// </summary>
        Text = 5,

        /// <summary>
        /// The value type is an entity identifier.
        /// </summary>
        Identifier = 6
    }
}
