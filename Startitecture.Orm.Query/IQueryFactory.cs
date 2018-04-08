// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IQueryFactory.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Query
{
    /// <summary>
    /// The StatementFactory interface.
    /// </summary>
    public interface IQueryFactory
    {
        /// <summary>
        /// Creates a query language statement for the specified <paramref name="selection"/>.
        /// </summary>
        /// <param name="selection">
        /// The selection to create a statement for.
        /// </param>
        /// <param name="outputType">
        /// The output type for the statement.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item that is the target of the selection.
        /// </typeparam>
        /// <returns>
        /// The query language statement as a <see cref="string"/>.
        /// </returns>
        string Create<TItem>(ItemSelection<TItem> selection, StatementOutputType outputType);
    }
}