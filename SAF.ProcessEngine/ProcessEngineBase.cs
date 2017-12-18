// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessEngineBase.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Base class for all process engines.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ProcessEngine
{
    using System;
    using System.Diagnostics;
    using System.Threading;

    using SAF.Core;
    using SAF.StringResources;

    /// <summary>
    /// Base class for all process engines.
    /// </summary>
    public abstract class ProcessEngineBase : IProcessEngine
    {
        /// <summary>
        /// An object that controls access to the process state.
        /// </summary>
        private readonly object processControl = new object();

        /// <summary>
        /// The completion lock.
        /// </summary>
        private readonly object completionLock = new object();

        /// <summary>
        /// A stopwatch that tracks the amount of time between process starts and stops.
        /// </summary>
        private readonly Stopwatch processTimer = new Stopwatch();

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessEngineBase"/> class.
        /// </summary>
        protected ProcessEngineBase()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessEngineBase"/> class.
        /// </summary>
        /// <param name="name">The name of the process.</param>
        protected ProcessEngineBase(string name)
        {
            this.Name = name ?? this.GetType().ToRuntimeName();
        }

        /// <summary>
        /// Occurs when the task engine starts.
        /// </summary>
        public event EventHandler<ProcessStartedEventArgs> ProcessStarted;

        /// <summary>
        /// Occurs when the task engine stops.
        /// </summary>
        public event EventHandler<ProcessStoppedEventArgs> ProcessStopped;

        /// <summary>
        /// Gets the name of the process.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the task engine is busy (true) or not (false).
        /// </summary>
        public bool IsBusy { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the task engine has been canceled.
        /// </summary>
        public bool Canceled { get; private set; }

        /// <summary>
        /// Gets the amount of time spent processing tasks.
        /// </summary>
        public TimeSpan ProcessTime 
        {
            get { return this.processTimer.Elapsed; }
        }

        /// <summary>
        /// Cancels the current process.
        /// </summary>
        /// <exception cref="OperationException">
        /// The process is not running and therefore cannot be canceled.
        /// </exception>
        public void Cancel()
        {
            this.Cancel(TimeSpan.FromMilliseconds(-1));
        }

        /// <summary>
        /// Cancels the current process.
        /// </summary>
        /// <exception cref="OperationException">
        /// The process is not running and therefore cannot be canceled.
        /// </exception>
        /// <param name="timeout">
        /// The amount of time to wait for completion of the request.
        /// </param>
        /// <returns>
        /// <c>true</c> if the process stopped prior to the timeout; otherwise <c>false</c>.
        /// </returns>
        public bool Cancel(TimeSpan timeout)
        {
            lock (this.completionLock)
            {
                if (this.Canceled)
                {
                    return true;
                }

                this.Canceled = true;
            }

            this.CancelProcess();
            return this.WaitForCompletion(timeout);
        }

        /// <summary>
        /// Blocks the current thread until the queue is emptied.
        /// </summary>
        public void WaitForCompletion()
        {
            this.WaitForCompletion(TimeSpan.FromMilliseconds(-1));
        }

        /// <summary>
        /// Blocks the current thread until the queue is emptied.
        /// </summary>
        /// <param name="timeout">
        /// The amount of time to wait for completion of the request.
        /// </param>
        /// <returns>
        /// <c>true</c> if the queue emptied prior to the timeout; otherwise <c>false</c>.
        /// </returns>
        public bool WaitForCompletion(TimeSpan timeout)
        {
            if (!this.IsBusy)
            {
                return true;
            }

            lock (this.completionLock)
            {
                if (this.IsBusy)
                {
                    return Monitor.Wait(this.completionLock, timeout);
                }
            }

            return true;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> representation of this <see cref="TaskEngine&lt;TDir, TResult&gt;"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> representation of this 
        /// <see cref="TaskEngine&lt;TDir, TResult&gt;"/>.</returns>
        public override string ToString()
        {
            return String.IsNullOrEmpty(this.Name) ? this.Name : base.ToString();
        }

        /// <summary>
        /// Determines whether the process is stopping.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the process is stopping; otherwise <c>false</c>.
        /// </returns>
        protected virtual bool IsStopping()
        {
            return true;
        }

        /// <summary>
        /// Cancels the current process.
        /// </summary>
        protected virtual void CancelProcess()
        {
        }

        /// <summary>
        /// Starts the process if the process has not already started.
        /// </summary>
        protected void StartProcess()
        {
            if (this.IsBusy)
            {
                return;
            }

            lock (this.processControl)
            {
                if (this.IsBusy)
                {
                    return;
                }

                if (this.Canceled)
                {
                    throw new ComponentAbortedException(String.Format(ErrorMessages.ProcessCanceledAndCannotBeRestarted, this));
                }

                this.processTimer.Start();
                this.IsBusy = true;
                this.OnProcessStarted(ProcessStartedEventArgs.Empty);
            }
        }

        /// <summary>
        /// Stops the process if the <see cref="IsStopping"/> method evaluates to true.
        /// </summary>
        /// <param name="error">The exception, if any, associated with the process stop check.</param>
        protected void StopProcessIfStopping(Exception error)
        {
            if (this.IsBusy == false && error == null)
            {
                return;
            }

            lock (this.processControl)
            {
                if (this.IsBusy == false && error == null)
                {
                    return;
                }

                if (this.IsStopping() == false && this.Canceled == false && error == null)
                {
                    return;
                }

                this.IsBusy = false;
                this.Canceled = this.Canceled || error != null;
                this.processTimer.Stop();

                // TFS item 23128 - Since we are raising an event, possibly outside the caller thread, we must ensure the completion 
                // lock is still released.
                try
                {
                    this.OnProcessStopped(error == null ? ProcessStoppedEventArgs.Empty : new ProcessStoppedEventArgs(error));
                }
                catch (Exception ex)
                {
                    // Here we are canceling the process using the virtual method if it is not already canceled. Avoid returning to 
                    // this method at all costs.
                    if (this.Canceled == false)
                    {
                        this.Canceled = true;
                        this.CancelProcess();
                    }

                    // Re-throw the exception so the caller can handle it, but don't hide the original error either.
                    throw Normalize.AggregateExceptions(ex, error);
                }
                finally
                {
                    lock (this.completionLock)
                    {
                        Monitor.Pulse(this.completionLock);
                    }
                }
            }
        }

        /// <summary>
        /// Triggers the ProcessStarted event.
        /// </summary>
        /// <param name="e">Event data associated with the event.</param>
        protected virtual void OnProcessStarted(ProcessStartedEventArgs e)
        {
            EventHandler<ProcessStartedEventArgs> temp = this.ProcessStarted;

            if (temp != null)
            {
                temp(this, e);
            }
        }

        /// <summary>
        /// Triggers the ProcessStopped event.
        /// </summary>
        /// <param name="eventArgs">Event data associated with the event.</param>
        protected virtual void OnProcessStopped(ProcessStoppedEventArgs eventArgs)
        {
            EventHandler<ProcessStoppedEventArgs> temp = this.ProcessStopped;

            if (temp != null)
            {
                temp(this, eventArgs);
            }
        }
    }
}
