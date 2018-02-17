// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStructuredCommand.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Sql
{
    using System.Data;

    /// <summary>
    /// The structured SQL command interface.
    /// </summary>
    public interface IStructuredCommand
    {
        /// <summary>
        /// Gets the table value parameter.
        /// </summary>
        string Parameter { get; }

        /// <summary>
        /// Gets the structure type name.
        /// </summary>
        string StructureTypeName { get; }

        /// <summary>
        /// Gets the command text.
        /// </summary>
        string CommandText { get; }

        /// <summary>
        /// Executes the current command with the specified table.
        /// </summary>
        /// <param name="dataTable">
        /// The data table containing the data to send to the operation.
        /// </param>
        void Execute(DataTable dataTable);

        /// <summary>
        /// Executes a command and returns a data reader.
        /// </summary>
        /// <param name="dataTable">
        /// The data table.
        /// </param>
        /// <returns>
        /// The <see cref="IDataReader"/> associated with the command.
        /// </returns>
        IDataReader ExecuteReader(DataTable dataTable);
    }
}