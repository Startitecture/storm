// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEventRepositoryFactory.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides access to an IEventRepository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActionTracking
{
    /// <summary>
    /// Provides access to an <see cref="IEventRepository"/>.
    /// </summary>
    public interface IEventRepositoryFactory
    {
        #region Public Methods and Operators

        /// <summary>
        /// Creates an event repository.
        /// </summary>
        /// <returns>
        /// An <see cref="IEventRepository"/> instance.
        /// </returns>
        IEventRepository Create();

        #endregion
    }
}