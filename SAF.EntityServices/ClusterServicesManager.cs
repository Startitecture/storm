// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClusterServicesManager.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.EntityServices
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;

    using SAF.ActionTracking;
    using SAF.Core;

    /// <summary>
    /// Manages the queue service.
    /// </summary>
    public sealed class ClusterServicesManager : IDisposable
    {
        #region Fields

        /// <summary>
        /// The service lock.
        /// </summary>
        private readonly object serviceLock = new object();

        /// <summary>
        /// The working directory for the queue service.
        /// </summary>
        private readonly string clusterPath;

        /// <summary>
        /// The local services.
        /// </summary>
        private readonly List<LocalServiceManager> localServiceManagers = new List<LocalServiceManager>();

        /// <summary>
        /// The action event proxy.
        /// </summary>
        private readonly IActionEventProxy actionEventProxy;

        /// <summary>
        /// The check timer for the working directory.
        /// </summary>
        private Timer checkTimer;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ClusterServicesManager"/> class.
        /// </summary>
        /// <param name="actionEventProxy">
        /// The action event proxy.
        /// </param>
        /// <param name="clusterPath">
        /// A path to a cluster resource which will indicate whether the current machine is the active node.
        /// </param>
        /// <param name="localServices">
        /// The local services to manage.
        /// </param>
        /// <exception cref="ApplicationConfigurationException">
        /// The QueueWorkingFolder application configuration setting could not be found.
        /// </exception>
        public ClusterServicesManager(IActionEventProxy actionEventProxy, string clusterPath, params ILocalService[] localServices)
        {
            if (String.IsNullOrWhiteSpace(clusterPath))
            {
                throw new ArgumentNullException("clusterPath");
            }

            this.actionEventProxy = actionEventProxy;
            this.clusterPath = clusterPath;
            this.localServiceManagers.AddRange(localServices.Select(x => new LocalServiceManager(x)));
        }

        #endregion

        /// <summary>
        /// Gets a value indicating whether the current machine is the active node.
        /// </summary>
        public bool IsActiveNode
        {
            get
            {
                return Directory.Exists(this.clusterPath);
            }
        }

        #region Public Methods and Operators

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (this.checkTimer != null)
            {
                this.checkTimer.Dispose();
                this.checkTimer = null;
            }

            foreach (var serviceManager in this.localServiceManagers)
            {
                serviceManager.LocalService.Dispose();
            }

            this.localServiceManagers.Clear();
        }

        /// <summary>
        /// Activates the service monitor for this instance, which starts the services if the current machine is on the active cluster
        /// node, and stops the services when the cluster fails over.
        /// </summary>
        public void ActivateServiceMonitor()
        {
            this.checkTimer = new Timer(this.StartServiceIfActiveNode, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        }

        /// <summary>
        /// Explicitly stops the services managed in this instance.
        /// </summary>
        public void StopServices()
        {
            lock (this.serviceLock)
            {
                foreach (var serviceManager in this.localServiceManagers.Where(x => x.IsActive))
                {
                    try
                    {
                        serviceManager.Stop();
                    }
                    catch (DomainException ex)
                    {
                        this.actionEventProxy.RecordAction(serviceManager, ex);
                    }
                    catch (Exception ex)
                    {
                        this.actionEventProxy.RecordAction(serviceManager, ex);
                        throw;
                    }
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Starts the service if the working folder is available.
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        private void StartServiceIfActiveNode(object state)
        {
            lock (this.serviceLock)
            {
                if (this.IsActiveNode)
                {
                    this.StartServices();
                }
                else
                {
                    this.StopServices();
                }
            }
        }

        /// <summary>
        /// Starts the services owned by this services manager.
        /// </summary>
        private void StartServices()
        {
            lock (this.serviceLock)
            {
                foreach (var serviceManager in this.localServiceManagers.Where(x => !x.IsActive))
                {
                    try
                    {
                        serviceManager.Start();
                    }
                    catch (DomainException ex)
                    {
                        this.actionEventProxy.RecordAction(serviceManager, ex);
                    }
                    catch (Exception ex)
                    {
                        this.actionEventProxy.RecordAction(serviceManager, ex);
                        throw;
                    }
                }
            }
        }

        #endregion
    }
}