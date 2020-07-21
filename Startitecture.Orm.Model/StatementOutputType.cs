// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatementOutputType.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    /// <summary>
    /// The SQL statement type.
    /// </summary>
    public enum StatementOutputType
    {
        /// <summary>
        /// The statement is a selection.
        /// </summary>
        Select = 0,

        /// <summary>
        /// The statement is a contains query.
        /// </summary>
        Contains = 1,

        /// <summary>
        /// The statement is an update statement.
        /// </summary>
        Update = 2,

        /// <summary>
        /// The statement is a delete statement.
        /// </summary>
        Delete = 3,

        /// <summary>
        /// The statement is a page select.
        /// </summary>
        CteSelect = 4
    }
}