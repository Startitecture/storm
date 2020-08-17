// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITableCommand.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface to SQL table command.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common
{
    /// <summary>
    /// Provides an interface to SQL table command.
    /// </summary>
    public interface ITableCommand
    {
        /// <summary>
        /// Gets the parameter for the source data.
        /// </summary>
        string ParameterName { get; }

        /// <summary>
        /// Gets the command text.
        /// </summary>
        string CommandText { get; }
    }
}