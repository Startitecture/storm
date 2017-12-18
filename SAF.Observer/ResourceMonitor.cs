// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceMonitor.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.Observer
{
    using System;
    using System.Timers;

    using SAF.Core;

    /// <summary>
    /// The base class for all cluster resource status items.
    /// </summary>
    public abstract class ResourceMonitor : IDisposable
    {
        /// <summary>
        /// The check timer.
        /// </summary>
        private readonly Timer checkTimer;

        /// <summary>
        /// The update lock.
        /// </summary>
        private readonly object updateLock = new object();

        /// <summary>
        /// The resource status.
        /// </summary>
        private readonly Lazy<ResourceStatus> resourceStatus; 

        /// <summary>
        /// The last exception.
        /// </summary>
        private Exception lastException;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceMonitor"/> class.
        /// </summary>
        /// <param name="location">
        /// The location of the resource.
        /// </param>
        protected ResourceMonitor(string location)
            : this(location, TimeSpan.Zero)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceMonitor"/> class which periodically updates the resource status 
        /// according to the specified <paramref name="interval"/>.
        /// </summary>
        /// <param name="location">
        /// The location of the resource.
        /// </param>
        /// <param name="interval">
        /// The interval at which to check the resource. If less or equal to <see cref="TimeSpan.Zero"/>, the resource will only be
        /// checked when <see cref="M:UpdateStatus"/> is called.
        /// </param>
        protected ResourceMonitor(string location, TimeSpan interval)
        {
            if (String.IsNullOrWhiteSpace(location))
            {
                throw new ArgumentNullException(nameof(location));
            }

            this.Location = location;
            this.resourceStatus = new Lazy<ResourceStatus>(this.InitializeResourceStatus);

            if (interval <= TimeSpan.Zero)
            {
                return;
            }

            this.checkTimer = new Timer(interval.TotalMilliseconds);
            this.checkTimer.Elapsed += this.UpdateStatus;
            this.checkTimer.Enabled = true;
        }

        /// <summary>
        /// Occurs when the resource becomes available.
        /// </summary>
        public event EventHandler ResourceAvailable;

        /// <summary>
        /// Occurs when the resource becomes unavailable.
        /// </summary>
        public event EventHandler ResourceUnavailable;

        /// <summary>
        /// Occurs when the observation fails.
        /// </summary>
        public event EventHandler<ObservationErrorEventArgs> ObservationFailed;

        #region Public Properties

        /// <summary>
        /// Gets the current resource status.
        /// </summary>
        public ResourceStatus ResourceStatus
        {
            get
            {
                if (this.resourceStatus.IsValueCreated)
                {
                    return this.resourceStatus.Value;
                }

                var status = this.resourceStatus.Value;
                this.UpdateStatus(status);
                return status;
            }
        }

        /// <summary>
        /// Gets the location of the resource.
        /// </summary>
        public string Location { get; private set; }

        #endregion

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
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return String.Concat(this.GetType().FullName, ':', this.Location);
        }

        /// <summary>
        /// Synchronously updates the status of the currently monitored resource.
        /// </summary>
        /// <param name="status">
        /// The status to update.
        /// </param>
        protected internal void UpdateStatus(ResourceStatus status)
        {
            if (status == null)
            {
                throw new ArgumentNullException(nameof(status));
            }

            lock (this.updateLock)
            {
                // TODO: this is a mess with regard to exception handling.
                var wasAvailable = status.IsAvailable;
                var statusResult = this.CheckAvailability();
                var weight = this.CalculateWeight();
                status.Update(statusResult.IsAvailable, weight, status.StatusError);

                if (statusResult.IsAvailable == wasAvailable)
                {
                    return;
                }

                if (statusResult.IsAvailable)
                {                
                    this.OnResourceAvailable();
                }
                else
                {
                    this.OnResourceUnavailable();
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">
        /// A value indicating whether the current object is being explicitly disposed.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            if (this.checkTimer != null)
            {
                this.checkTimer.Dispose();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the service resource is available.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the resource is available; otherwise, <c>false</c>.
        /// </returns>
        protected abstract StatusResult CheckAvailability();

        /// <summary>
        /// Calculates the weight of accessing the resource.
        /// </summary>
        /// <returns>
        /// A <see cref="double"/> that represents the relative weight of accessing the resource.
        /// </returns>
        protected abstract double CalculateWeight();

        /// <summary>
        /// Reports an exception if it is a new exception.
        /// </summary>
        /// <param name="error">
        /// The error to report.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsNewException(Exception error)
        {
            if (error == null)
            {
                throw new ArgumentNullException(nameof(error));
            }

            bool isNewException;

            lock (this)
            {
                isNewException = this.lastException == null || this.lastException.GetType() != error.GetType()
                                 || this.lastException.GetBaseException().GetType() != error.GetBaseException().GetType();

                this.lastException = error;
            }

            return isNewException;
        }

        /// <summary>
        /// Initializes and returns a resource status for the current monitor.
        /// </summary>
        /// <returns>
        /// A <see cref="ResourceStatus"/> instance for the current monitor.
        /// </returns>
        private ResourceStatus InitializeResourceStatus()
        {
            return new ResourceStatus(this);
        }

        /// <summary>
        /// Triggers the <see cref="ResourceAvailable"/> event.
        /// </summary>
        private void OnResourceAvailable()
        {
            this.ResourceAvailable?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Triggers the <see cref="ResourceUnavailable"/> event.
        /// </summary>
        private void OnResourceUnavailable()
        {
            this.ResourceUnavailable?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Triggers the <see cref="ObservationFailed"/> event.
        /// </summary>
        /// <param name="eventArgs">
        /// The event data associated with the event.
        /// </param>
        private void OnObservationFailed(ObservationErrorEventArgs eventArgs)
        {
            this.ObservationFailed?.Invoke(this, eventArgs);
        }

        /// <summary>
        /// Updates the resource status.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="eventArgs">
        /// The event data associated with the event.
        /// </param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design", 
            "CA1031:DoNotCatchGeneralExceptionTypes", 
            Justification = "Do not throw exceptions in background threads.")]
        private void UpdateStatus(object sender, ElapsedEventArgs eventArgs)
        {
            try
            {
                this.UpdateStatus(this.ResourceStatus);
            }
            catch (DomainException ex)
            {
                if (this.IsNewException(ex))
                {
                    this.OnObservationFailed(new ObservationErrorEventArgs(ex));
                }
            }
            catch (Exception ex)
            {
                this.checkTimer.Enabled = false;
                this.OnObservationFailed(new ObservationErrorEventArgs(ex, true));
            }
        }
    }
}