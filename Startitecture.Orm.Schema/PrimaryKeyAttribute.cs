// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PrimaryKeyAttribute.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Specifies the primary key column of a poco class, whether the column is auto incrementing and the sequence name for
//   Oracle
//   sequence columns.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Schema
{
    using System;

    /// <summary>
    /// Specifies the primary key column of a poco class, whether the column is auto incrementing and the sequence name for Oracle 
    /// sequence columns.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class PrimaryKeyAttribute : Attribute
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PrimaryKeyAttribute"/> class.
        /// </summary>
        /// <param name="columnName">
        /// The primary key.
        /// </param>
        public PrimaryKeyAttribute(string columnName)
        {
            this.ColumnName = columnName;
            this.AutoIncrement = true;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether auto increment.
        /// </summary>
        public bool AutoIncrement { get; set; }

        /// <summary>
        /// Gets or sets the sequence name.
        /// </summary>
        public string SequenceName { get; set; }

        /// <summary>
        /// Gets the name of the column that is the primary key.
        /// </summary>
        public string ColumnName { get; private set; }

        #endregion
    }
}