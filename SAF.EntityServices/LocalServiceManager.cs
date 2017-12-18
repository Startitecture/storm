// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalServiceManager.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Manages a local service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.EntityServices
{
    /// <summary>
    /// Manages a local service.
    /// TODO: Create the service with a factory and handle threading and disposing internally.
    /// </summary>
    public class LocalServiceManager
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalServiceManager"/> class.
        /// </summary>
        /// <param name="localService">
        /// The local service.
        /// </param>
        public LocalServiceManager(ILocalService localService)
        {
            this.LocalService = localService;
            this.IsActive = false;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether the service is active.
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Gets the local service.
        /// </summary>
        public ILocalService LocalService { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Starts the service.
        /// </summary>
        public void Start()
        {
            lock (this)
            {
                this.LocalService.StartService();
                this.IsActive = true;
            }
        }

        /// <summary>
        /// Stops the service.
        /// </summary>
        public void Stop()
        {
            lock (this)
            {
                this.LocalService.StopService();
                this.IsActive = false;
            }
        }

        #endregion
    }
}