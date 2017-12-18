// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComponentMonitor.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.Observer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SAF.Core;
    using SAF.StringResources;

    /// <summary>
    /// Monitors the status of a component and its resources.
    /// </summary>
    public class ComponentMonitor : IComponentMonitor
    {
        /// <summary>
        /// The is available predicate.
        /// </summary>
        private static readonly Func<ResourceMonitor, bool> IsAvailablePredicate = x => x.ResourceStatus.IsAvailable;

        /// <summary>
        /// The is unavailable predicate.
        /// </summary>
        private static readonly Func<ResourceMonitor, bool> IsUnavailablePredicate = x => x.ResourceStatus.IsAvailable == false;

        /// <summary>
        /// The qualified name.
        /// </summary>
        private readonly string qualifiedName;

        /// <summary>
        /// The component status.
        /// </summary>
        private readonly Lazy<ComponentStatus> componentStatus; 

        /// <summary>
        /// The monitors.
        /// </summary>
        private readonly List<ResourceMonitor> monitors = new List<ResourceMonitor>();

        /// <summary>
        /// The monitors lock.
        /// </summary>
        private readonly object availabilityLock = new object();

        /// <summary>
        /// Indicates whether the current object is disposed.
        /// </summary>
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentMonitor"/> class.
        /// </summary>
        /// <param name="qualifiedName">
        /// The qualified name of the monitor.
        /// </param>
        protected ComponentMonitor(string qualifiedName)
        {
            if (String.IsNullOrWhiteSpace(qualifiedName))
            {
                throw new ArgumentNullException("qualifiedName");
            }

            this.qualifiedName = qualifiedName;
            this.componentStatus = new Lazy<ComponentStatus>(this.InitializeComponentStatus);
        }

        /// <summary>
        /// Occurs when the resource becomes available.
        /// </summary>
        public event EventHandler ComponentAvailable;

        /// <summary>
        /// Occurs when the resource becomes unavailable.
        /// </summary>
        public event EventHandler ComponentUnavailable;

        /// <summary>
        /// Gets the status of the current component.
        /// </summary>
        public ComponentStatus Status
        {
            get
            {
                if (this.isDisposed)
                {
                    throw new ObjectDisposedException(Convert.ToString(this));
                }

                return this.componentStatus.Value;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">
        /// Indicates whether the current instance is being explicitly disposed.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (this.availabilityLock)
                {
                    this.isDisposed = true;

                    foreach (var monitor in this.monitors)
                    {
                        monitor.ResourceAvailable -= this.NotifyAvailable;
                        monitor.ResourceUnavailable -= this.NotifyUnavailable;
                    }

                    this.monitors.Clear();
                }
            }
        }

        /// <summary>
        /// The register monitor.
        /// </summary>
        /// <param name="monitor">
        /// The monitor.
        /// </param>
        protected void RegisterMonitor(ResourceMonitor monitor)
        {
            if (monitor == null)
            {
                throw new ArgumentNullException("monitor");
            }

            lock (this.availabilityLock)
            {
                if (this.isDisposed)
                {
                    throw new ObjectDisposedException(Convert.ToString(this));
                }
            }

            // Prevent the caller from adding a monitor that will not be contained in the status.
            if (this.componentStatus.IsValueCreated)
            {
                throw new BusinessException(this, String.Format(ValidationMessages.ComponentMonitorAlreadyInitialized, monitor));
            }

            lock (this.availabilityLock)
            {
                this.monitors.Add(monitor);
                monitor.ResourceAvailable += this.NotifyAvailable;
                monitor.ResourceUnavailable += this.NotifyUnavailable;
            }

            monitor.UpdateStatus(monitor.ResourceStatus);
        }

        /// <summary>
        /// Initializes the component status.
        /// </summary>
        /// <returns>
        /// A <see cref="ComponentStatus"/> instance containing the current collection of resource statuses.
        /// </returns>
        private ComponentStatus InitializeComponentStatus()
        {
            return new ComponentStatus(this.qualifiedName, this.monitors.Select(x => x.ResourceStatus).ToArray());
        }

        /// <summary>
        /// Triggers the <see cref="ComponentAvailable"/> event.
        /// </summary>
        private void OnComponentAvailable()
        {
            var handler = this.ComponentAvailable;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Triggers the <see cref="ComponentUnavailable"/> event.
        /// </summary>
        private void OnComponentUnavailable()
        {
            var handler = this.ComponentUnavailable;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Handles resource available events.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="eventArgs">
        /// The event data associated with the event.
        /// </param>
        private void NotifyAvailable(object sender, EventArgs eventArgs)
        {
            lock (this.availabilityLock)
            {
                if (this.monitors.All(IsAvailablePredicate))
                {
                    this.OnComponentAvailable();
                }
            }
        }

        /// <summary>
        /// Handles resource unavailable events.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="eventArgs">
        /// The event data associated with the event.
        /// </param>
        private void NotifyUnavailable(object sender, EventArgs eventArgs)
        {
            lock (this.availabilityLock)
            {
                if (this.monitors.Any(IsUnavailablePredicate))
                {
                    this.OnComponentUnavailable();
                }
            }
        }
    }
}
