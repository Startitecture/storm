// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILocalService.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface for classes that operate local services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.EntityServices
{
    using System;

    /// <summary>
    /// Provides an interface for classes that operate local services.
    /// </summary>
    public interface ILocalService : IDisposable
    {
        #region Public Methods and Operators

        /// <summary>
        /// Starts the service.
        /// </summary>
        void StartService();

        /// <summary>
        /// Stops the service.
        /// </summary>
        void StopService();

        #endregion
    }
}