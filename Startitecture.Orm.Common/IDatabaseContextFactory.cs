// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDatabaseContextFactory.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Provides an interface for classes that create database instances.
// </summary>

namespace Startitecture.Orm.Common
{
    /// <summary>
    /// Provides an interface for classes that create database instances.
    /// </summary>
    public interface IDatabaseContextFactory
    {
        /// <summary>
        /// Creates a new database instance.
        /// </summary>
        /// <returns>
        /// A new <see cref="IDatabaseContext"/> instance.
        /// </returns>
        IDatabaseContext Create();
    }
}