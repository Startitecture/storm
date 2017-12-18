// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyOrdinal.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Mock
{
    using System.Reflection;

    /// <summary>
    /// The property ordinal.
    /// </summary>
    internal class PropertyOrdinal
    {
        /// <summary>
        /// Gets or sets the ordinal.
        /// </summary>
        public int Ordinal { get; set; }

        /// <summary>
        /// Gets or sets the property info.
        /// </summary>
        public PropertyInfo Property { get; set; }
    }
}