// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDbTableCommandFactory.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   An interface for a table command provider, which abstracts the underlying database from the command.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common
{
    using System.Collections.Generic;
    using System.Data;

    /// <summary>
    /// An interface for a table command provider, which abstracts the underlying database from the command.
    /// </summary>
    public interface IDbTableCommandFactory
    {
        /// <summary>
        /// Creates an <see cref="IDbCommand"/> for the specified <paramref name="items"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of items in the table command.
        /// </typeparam>
        /// <param name="databaseContext">
        /// The database context in which to create the command.
        /// </param>
        /// <param name="commandText">
        /// The command text to execute.
        /// </param>
        /// <param name="parameterName">
        /// The name to use for the table parameter.
        /// </param>
        /// <param name="items">
        /// The items to pass to the command.
        /// </param>
        /// <returns>
        /// An <see cref="IDbCommand"/> that will execute with <paramref name="items"/> as the parameter <paramref name="parameterName"/>.
        /// </returns>
        IDbCommand Create<T>(IDatabaseContext databaseContext, string commandText, string parameterName, IEnumerable<T> items);
    }
}