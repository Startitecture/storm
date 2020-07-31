// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStatementFactory.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    /// <summary>
    /// Provides an interface for creating data manipulation language (DML) statements.
    /// </summary>
    public interface IStatementFactory
    {
        /// <summary>
        /// Creates a query language statement to determine if any results exist for the specified <paramref name="entitySet"/>.
        /// </summary>
        /// <param name="entitySet">
        /// The entity set to test for existence.
        /// </param>
        /// <returns>
        /// The query language statement as a <see cref="string"/>.
        /// </returns>
        string CreateExistsStatement(IEntitySet entitySet);

        /// <summary>
        /// Creates a query language selection statement for the specified <paramref name="selection"/>.
        /// </summary>
        /// <param name="selection">
        /// The selection to create the statement for.
        /// </param>
        /// <returns>
        /// The query language statement as a <see cref="string"/>.
        /// </returns>
        string CreateSelectionStatement(ISelection selection);

        /// <summary>
        /// Creates a statement to update a specific set of entities.
        /// </summary>
        /// <param name="updateSet">
        /// The update set.
        /// </param>
        /// <returns>
        /// The query language statement as a <see cref="string"/>.
        /// </returns>
        string CreateUpdateStatement(IUpdateSet updateSet);

        /// <summary>
        /// Creates a statement to delete a specific set of entities.
        /// </summary>
        /// <param name="entitySet">
        /// The entity set.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string CreateDeletionStatement(IEntitySet entitySet);
    }
}