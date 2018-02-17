// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStructuredCommandFactory.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// The StructuredCommandFactory interface.
    /// </summary>
    public interface IStructuredCommandFactory
    {
        /// <summary>
        /// Creates an <see cref="IStructuredCommand"/> with the specified database context provider and command text.
        /// </summary>
        /// <typeparam name="T">
        /// The type of item to create the command for.
        /// </typeparam>
        /// <param name="commandText">
        /// The command text.
        /// </param>
        /// <param name="databaseContextProvider">
        /// The database context provider.
        /// </param>
        /// <returns>
        /// An <see cref="IStructuredCommand"/> for the specified database context provider and command text.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="databaseContextProvider"/> or <paramref name="commandText"/> is null.
        /// </exception>
        IStructuredCommand Create<T>([NotNull] StructuredCommandText commandText, [NotNull] IDatabaseContextProvider databaseContextProvider);
    }
}