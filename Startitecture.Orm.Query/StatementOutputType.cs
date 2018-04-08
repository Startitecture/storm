// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatementOutputType.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Query
{
    /// <summary>
    /// The SQL statement type.
    /// </summary>
    public enum StatementOutputType
    {
        /// <summary>
        /// The statement is a select.
        /// </summary>
        Select = 0,

        /// <summary>
        /// The statement is a contains.
        /// </summary>
        Contains = 1,

        /// <summary>
        /// The statement is a delete.
        /// </summary>
        Delete = 2
    }
}