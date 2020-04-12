// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStructuredCommand.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Sql
{
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
    }
}