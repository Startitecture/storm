// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDatabaseFactory.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface for classes that create database instances.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers
{
    /// <summary>
    /// Provides an interface for classes that create database instances.
    /// </summary>
    /// <typeparam name="TContext">
    /// The type of database to create.
    /// </typeparam>
    public interface IDatabaseFactory<out TContext>
        where TContext : IDatabaseContext
    {
        /// <summary>
        /// Creates a new database instance.
        /// </summary>
        /// <returns>
        /// A new <see cref="Database"/> instance.
        /// </returns>
        TContext Create();
    }
}