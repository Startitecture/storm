// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructuredSqlCommandProviderFactory.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers
{
    using System;

    /// <summary>
    /// The structured SQL command provider factory.
    /// </summary>
    public class StructuredSqlCommandProviderFactory : IStructuredCommandProviderFactory
    {
        /// <summary>
        /// Creates an <see cref="IStructuredCommandProvider"/> for the specified <paramref name="databaseContextProvider"/>.
        /// </summary>
        /// <param name="databaseContextProvider">
        /// The database context provider.
        /// </param>
        /// <returns>
        /// A <see cref="IStructuredCommandProvider"/> instance for the specified <paramref name="databaseContextProvider"/>.
        /// </returns>
        public IStructuredCommandProvider Create(IDatabaseContextProvider databaseContextProvider)
        {
            if (databaseContextProvider == null)
            {
                throw new ArgumentNullException(nameof(databaseContextProvider));
            }

            return new StructuredSqlCommandProvider(databaseContextProvider);
        }
    }
}