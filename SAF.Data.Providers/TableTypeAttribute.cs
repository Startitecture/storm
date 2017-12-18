// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableTypeAttribute.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers
{
    using System;

    /// <summary>
    /// The table type attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class TableTypeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableTypeAttribute"/> class.
        /// </summary>
        /// <param name="typeName">
        /// The type name.
        /// </param>
        public TableTypeAttribute(string typeName)
        {
            this.TypeName = typeName;
        }

        /// <summary>
        /// Gets the type name.
        /// </summary>
        public string TypeName { get; private set; }
    }
}