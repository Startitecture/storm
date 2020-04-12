// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStructuredCommandProvider.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The StructuredCommandProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Sql
{
    using System.Data;

    using Startitecture.Orm.Model;

    /// <summary>
    /// The StructuredCommandProvider interface.
    /// </summary>
    public interface IStructuredCommandProvider
    {
        /// <summary>
        /// Gets the entity definition provider.
        /// </summary>
        IEntityDefinitionProvider EntityDefinitionProvider { get; }

        /// <summary>
        /// Gets the name qualifier.
        /// </summary>
        INameQualifier NameQualifier { get; }

        /// <summary>
        /// Creates an <see cref="IDbCommand"/> for the specified <paramref name="structuredCommand"/>.
        /// </summary>
        /// <param name="structuredCommand">
        /// The structured command.
        /// </param>
        /// <param name="dataTable">
        /// The data table to include with the command.
        /// </param>
        /// <param name="transaction">
        /// The transaction to use with the command.
        /// </param>
        /// <returns>
        /// An <see cref="IDbCommand"/> that will execute the structured command.
        /// </returns>
        IDbCommand CreateCommand(IStructuredCommand structuredCommand, DataTable dataTable, IDbTransaction transaction);
    }
}