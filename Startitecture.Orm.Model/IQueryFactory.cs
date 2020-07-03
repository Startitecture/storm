// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IQueryFactory.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    /// <summary>
    /// The StatementFactory interface.
    /// </summary>
    public interface IQueryFactory
    {
        /// <summary>
        /// Creates a query language statement for the specified <paramref name="queryContext"/>.
        /// </summary>
        /// <param name="queryContext">
        /// The query Context.
        /// </param>
        /// <returns>
        /// The query language statement as a <see cref="string"/>.
        /// </returns>
        string Create(QueryContext queryContext);
    }
}