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
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an interface for classes that create database contexts.
    /// </summary>
    public interface IDatabaseContextFactory
    {
        /// <summary>
        /// Creates a new database context instance.
        /// </summary>
        /// <returns>
        /// A new <see cref="IDatabaseContext"/> instance.
        /// </returns>
        IDatabaseContext Create();

        /// <summary>
        /// Asynchronously creates a new database context instance.
        /// </summary>
        /// <param name="cancellationToken">
        /// The cancellation token for the asynchronous operation.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> that is creating a new <see cref="IDatabaseContext"/> instance.
        /// </returns>
        Task<IDatabaseContext> CreateAsync(CancellationToken cancellationToken);
    }
}