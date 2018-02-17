// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColumnAttribute.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Declares a property as a column and optionally supplies the database column name.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers
{
    using System;

    /// <summary>
    /// Declares a property as a column and optionally supplies the database column name.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "Attributes inherit from this attribute.")]
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnAttribute"/> class.
        /// </summary>
        public ColumnAttribute()
        {
            this.ForceToUtc = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnAttribute"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public ColumnAttribute(string name)
        {
            this.Name = name;
            this.ForceToUtc = false;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether force <see cref="DateTime"/> values to UTC time.
        /// </summary>
        public bool ForceToUtc { get; set; }

        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        public string Name { get; private set; }

        #endregion
    }
}