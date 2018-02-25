// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnifiedFieldSourceType.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model.PM
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