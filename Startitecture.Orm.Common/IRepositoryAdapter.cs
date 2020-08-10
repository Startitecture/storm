// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepositoryAdapter.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using Startitecture.Orm.Model;

    /// <summary>
    /// Provides an interface for creating data manipulation language (DML) statements.
    /// </summary>
    public interface IRepositoryAdapter
    {
        /// <summary>
        /// Gets the definition provider.
        /// </summary>
        IEntityDefinitionProvider DefinitionProvider { get; }

        /// <summary>
        /// Gets the name qualifier.
        /// </summary>
        INameQualifier NameQualifier { get; }

        /// <summary>
        /// Gets the value mappers for the repository adapter.
        /// </summary>
        IReadOnlyDictionary<Tuple<Type, Type>, IValueMapper> ValueMappers { get; }

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
        /// The set of entities to update.
        /// </param>
        /// <returns>
        /// The query language statement as a <see cref="string"/>.
        /// </returns>
        string CreateUpdateStatement(IUpdateSet updateSet);

        /// <summary>
        /// Creates a statement to delete a specific set of entities.
        /// </summary>
        /// <param name="entitySet">
        /// The entity set to delete.
        /// </param>
        /// <returns>
        /// The query language statement as a <see cref="string"/>.
        /// </returns>
        string CreateDeletionStatement(IEntitySet entitySet);

        /// <summary>
        /// Creates an insertion statement for a single entity.
        /// </summary>
        /// <typeparam name="T">
        /// The type of entity to insert.
        /// </typeparam>
        /// <returns>
        /// The query language statement as a <see cref="string"/>.
        /// </returns>
        string CreateInsertionStatement<T>();

        /// <summary>
        /// Adds a prefix to a parameter for use in a prepared statement.
        /// </summary>
        /// <param name="parameterName">
        /// The parameter name.
        /// </param>
        /// <returns>
        /// The prefixed parameter name as a <see cref="string"/>.
        /// </returns>
        string AddPrefix(string parameterName);

        /// <summary>
        /// Maps a parameter from its runtime type to a compatible database type, if needed.
        /// </summary>
        /// <param name="command">
        /// The database command.
        /// </param>
        /// <param name="name">
        /// The parameter name.
        /// </param>
        /// <param name="value">
        /// The value to map.
        /// </param>
        /// <returns>
        /// The mapped value as an <see cref="object"/>.
        /// </returns>
        IDbDataParameter CreateParameter(IDbCommand command, string name, object value);
    }
}