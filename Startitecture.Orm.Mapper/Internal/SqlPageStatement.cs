// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlPageStatement.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Contains the elements of a SQL page statement.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Mapper.Internal
{
    /// <summary>
    /// Contains the elements of a SQL page statement.
    /// </summary>
    internal struct SqlPageStatement
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the SQL statement.
        /// </summary>
        public string Sql { get; set; }

        /// <summary>
        /// Gets or sets the SQL count clause.
        /// </summary>
        public string SqlCount { get; set; }

        /// <summary>
        /// Gets or sets the SQL ORDER BY clause.
        /// </summary>
        public string SqlOrderBy { get; set; }

        /// <summary>
        /// Gets or sets the SQL statement with SELECT removed.
        /// </summary>
        public string SqlSelectRemoved { get; set; }

        #endregion
    }
}