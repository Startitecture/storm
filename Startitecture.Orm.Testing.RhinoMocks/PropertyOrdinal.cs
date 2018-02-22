// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyOrdinal.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.RhinoMocks
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