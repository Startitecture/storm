// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnifiedFieldSourceType.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    /// <summary>
    /// The unified field source type.
    /// </summary>
    public enum UnifiedFieldSourceType
    {
        /// <summary>
        /// The field source is unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The field source is a system field.
        /// </summary>
        SystemField = 1,

        /// <summary>
        /// The field source is a custom field.
        /// </summary>
        CustomField = 2
    }
}