// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableNameAttribute.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Sets the database table name to be used for a POCO class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers
{
    using System;

    /// <summary>
    /// Sets the database table name to be used for a POCO class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class TableNameAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableNameAttribute"/> class.
        /// </summary>
        /// <param name="tableName">
        /// The table name.
        /// </param>
        public TableNameAttribute(string tableName)
        {
            this.TableName = tableName;
        }

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        public string TableName { get; private set; }
    }
}