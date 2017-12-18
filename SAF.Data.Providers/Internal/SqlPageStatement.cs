// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlPageStatement.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Contains the elements of a SQL page statement.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Internal
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