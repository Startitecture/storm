// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableTypeAttribute.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Declares that a class represents a user-defined table type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Schema
{
    using System;

    /// <summary>
    /// Declares that a class represents a user-defined table type.
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
        /// Gets the name of the user-defined table type.
        /// </summary>
        public string TypeName { get; private set; }
    }
}