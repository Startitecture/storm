// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDatabaseContextProvider.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common
{
    /// <summary>
    /// Provides an interface for classes that contain database contexts.
    /// </summary>
    public interface IDatabaseContextProvider
    {
        /// <summary>
        /// Gets the current database context.
        /// </summary>
        IDatabaseContext DatabaseContext { get; }
    }
}
