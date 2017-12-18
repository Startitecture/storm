// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskEngine.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Base class for task engines.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ProcessEngine
{
    using System;
    using System.Diagnostics;
    using System.Threading;

    using SAF.Core;

    /// <summary>
    /// Base class for task engines.
    /// </summary>
    /// <typeparam name="TDirective">The type of directive that represents the task for this task engine.</typeparam>
    /// <typeparam name="TResult">The type of result that this task engine produces.</typeparam>
    public abstract class TaskEngine<TDirective, TResult> : ProcessEngineBase, ITaskEngine, IProducer
        where TResult : ITaskResult
    {
        #region Constants

        /// <summary>
        /// The name format for the ToString() method.
        /// </summary>
        private const string NameFormat = "{0} engine";

        #endregion

        #region Fields

        /// <summary>
        /// The item producer that handles directives.
        /// </summary>
        private readonly ItemProducer<TDirective> directiveProducer = new ItemProducer<TDirective>();

        /// <summary>
        /// The result producer that handles task results.
        /// </summary>
        private readonly ItemProducer<TResult> resultProducer = new ItemProducer<TResult>();

        /// <summary>
        /// The maximum number of items that the collection can contain.
        /// </summary>
        private long maxQueueLength = Int64.MaxValue;

        /// <summary>
        /// The number of tasks provided to this task engine.
        /// </summary>
        private long totalTasks;

        /// <summary>
        /// The number of tasks that were successful.
        /// </summary>
        private long successfulTasks;

        /// <summary>
        /// The number of tasks that returned an empty result.
        /// </summary>
        private long emptyResults;

        /// <summary>
        /// The number of failed tasks.
        /// </summary>
        private long failedTasks;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskEngine&lt;TDirective, TResult&gt;"/> class.
        /// </summary>
        /// <param name="name">The name of this task engine.</param>
        protected TaskEngine(string name)
            : base(name)
        {
            // Check for process started if the directive producer is starting.
            this.directiveProducer.ProcessStarted += this.CheckForProcessStarted;

            // Process directives provided by callers of the ProduceItem method.
            this.directiveProducer.ItemsProduced += this.ProcessProducedDirectives;

            // Let the consumer know that events are ready.
            this.resultProducer.ItemsProduced += this.TriggerItemsProduced;

            // Check to see if the process has stopped.
            this.directiveProducer.ProcessStopped += this.CheckForProcessStopped;
            this.resultProducer.ProcessStopped += this.CheckForProcessStopped;

            // Update the health of the process once it has stopped.
            this.ProcessStopped += this.UpdateProcessHealth;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskEngine&lt;TDirective, TResult&gt;"/> class.
        /// </summary>
        protected TaskEngine()
            : this(String.Format(NameFormat, typeof(TDirective).Name))
        {
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when tasks have been provided to the task engine.
        /// </summary>
        public event EventHandler<ItemsAddedEventArgs> ItemsAdded;

        /// <summary>
        /// Occurs when results have been produced by the task engine.
        /// </summary>
        public event EventHandler<ItemsProducedEventArgs> ItemsProduced;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the results of the task directives.
        /// </summary>
        public QueueConsumer<TResult> TaskResultConsumer
        {
            get { return this.resultProducer.ItemQueueConsumer; }
        }

        /// <summary>
        /// Gets or sets the limit on the number of waiting tasks. Once the limit is reached, the caller's thread 
        /// will be blocked until another task is processed.
        /// </summary>
        public long MaxQueueLength
        {
            get 
            { 
                return this.maxQueueLength; 
            }

            set 
            {
                this.maxQueueLength = value > 0 ? value : 1;
                this.directiveProducer.MaxQueueLength = this.maxQueueLength;
                this.resultProducer.MaxQueueLength = this.maxQueueLength;
            }
        }

        /// <summary>
        /// Gets the health of the current process in terms of whether tasks or queues have failed.
        /// </summary>
        public UserResultState ProcessHealth { get; private set; }

        /// <summary>
        /// Gets the number of items added to the collection.
        /// </summary>
        public long TotalTasks 
        {
            get { return Interlocked.Read(ref this.totalTasks); }
        }

        /// <summary>
        /// Gets the number of tasks waiting to be processed.
        /// </summary>
        public long WaitingTasks
        {
            get { return this.directiveProducer.ItemsPending; }
        }

        /// <summary>
        /// Gets the number of results that are waiting to be retrieved.
        /// </summary>
        public long WaitingResults
        {
            get { return this.resultProducer.ItemsPending; }
        }

        /// <summary>
        /// Gets the number of tasks consumed by this task engine.
        /// </summary>
        public long CompletedTasks
        {
            get { return this.SuccessfulResults + this.EmptyResults + this.FailedResults; }
        }

        /// <summary>
        /// Gets the number of successful task results.
        /// </summary>
        public long SuccessfulResults
        {
            get { return Interlocked.Read(ref this.successfulTasks); }
        }

        /// <summary>
        /// Gets the number of empty task results.
        /// </summary>
        public long EmptyResults
        {
            get { return Interlocked.Read(ref this.emptyResults); }
        }

        /// <summary>
        /// Gets the number of failed task results.
        /// </summary>
        public long FailedResults
        {
            get { return Interlocked.Read(ref this.failedTasks); }
        }

        /// <summary>
        /// Gets the current progress of this process from 0 to 1.
        /// </summary>
        public double Progress
        {
            get { return this.TotalTasks > 0 ? this.CompletedTasks / (double)this.TotalTasks : 0; }
        }

        /// <summary>
        /// Gets the average time taken per task.
        /// </summary>
        public double TasksPerSecond
        {
            get 
            { 
                return this.ProcessTime.TotalSeconds > 0 ? 
                    this.CompletedTasks / this.ProcessTime.TotalSeconds : 0;
            }
        }

        #endregion

        #region Methods

        #region Base Class Overrides

        /// <summary>
        /// Determines whether the process is stopping.
        /// </summary>
        /// <returns>True if the process is stopping, otherwise false.</returns>
        protected override bool IsStopping()
        {
            return !this.directiveProducer.IsBusy && !this.resultProducer.IsBusy;
        }

        /// <summary>
        /// Cancels the current process.
        /// </summary>
        /// <param name="userState">
        /// The user state associated with the process.
        /// </param>
        /// <param name="processError">
        /// The exception, if any, associated with the cancellation.
        /// </param>
        protected virtual void CancelProcess(object userState, Exception processError)
        {
            this.directiveProducer.Cancel();
            this.resultProducer.Cancel();
        }

        #endregion

        /// <summary>
        /// Queues a task for the task engine.
        /// </summary>
        /// <param name="directive">The task directive to queue.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="directive"/> is null.
        /// </exception>
        protected void QueueTask(TDirective directive)
        {
            if (Evaluate.IsNull(directive))
            {
                throw new ArgumentNullException("directive");
            }

            if (this.Canceled)
            {
                return;
            }

            Interlocked.Increment(ref this.totalTasks);
            this.directiveProducer.ProduceItem(directive);
        }

        /// <summary>
        /// Processes an item.
        /// </summary>
        /// <param name="directive">A directive that provides instructions for the task.</param>
        /// <returns>The result of the task.</returns>
        [DebuggerHidden]
        protected abstract TResult ConsumeItem(TDirective directive);

        /// <summary>
        /// Processes an enumeration of directives.
        /// </summary>
        /// <param name="directive">The directives to process.</param>
        private void ProcessDirective(TDirective directive)
        {
            TResult result = default(TResult);

            try
            {
                result = this.ConsumeItem(directive);
            }
            finally
            {
                this.UpdateResultState(Evaluate.IsDefaultValue(result) ? ResultState.Error : result.ResultState);
            }

            this.resultProducer.ProduceItem(result);
        }

        /// <summary>
        /// Updates the result state of this task engine.
        /// </summary>
        /// <param name="resultState">The result state of a task result.</param>
        [DebuggerHidden]
        private void UpdateResultState(ResultState resultState)
        {
            if (resultState == ResultState.Error && this.ProcessHealth == UserResultState.Nominal)
            {
                this.ProcessHealth = UserResultState.TaskError;
            }

            switch (resultState)
            {
                case ResultState.Success:
                    Interlocked.Increment(ref this.successfulTasks);
                    break;

                case ResultState.Empty:
                    Interlocked.Increment(ref this.emptyResults);
                    break;

                case ResultState.Error:
                    Interlocked.Increment(ref this.failedTasks);
                    break;
            }
        }

        #region Event Handlers

        /// <summary>
        /// Checks whether the process has started.
        /// </summary>
        /// <param name="sender">The sender of the <see cref="ProcessStartedEventArgs"/> event.</param>
        /// <param name="e"><see cref="ProcessStartedEventArgs"/> associated with the event.</param>
        private void CheckForProcessStarted(object sender, ProcessStartedEventArgs e)
        {
            this.StartProcess();
        }

        /// <summary>
        /// Processes directives produced by the directive queue.
        /// </summary>
        /// <param name="sender">The sender of the <see cref="ItemsProducedEventArgs"/> event.</param>
        /// <param name="e"><see cref="ItemsProducedEventArgs"/> associated with the event.</param>
        private void ProcessProducedDirectives(object sender, ItemsProducedEventArgs e)
        {
            this.OnItemsAdded(ItemsAddedEventArgs.Empty);

            try
            {
                while (this.directiveProducer.ItemQueueConsumer.ConsumeNext())
                {
                    if (this.Canceled)
                    {
                        break;
                    }

                    this.ProcessDirective(this.directiveProducer.ItemQueueConsumer.Current);
                }
            }
            catch (Exception ex)
            {
                this.Cancel();
                Interlocked.Add(ref this.failedTasks, this.totalTasks - this.CompletedTasks);
                Trace.TraceError("'{0}' consumer aborted: {1}", this.Name, ex);
                throw;
            }
        }

        /// <summary>
        /// Triggers the <see cref="ItemsProduced"/> event.
        /// </summary>
        /// <param name="sender">The sender of the original <see cref="ItemsProducedEventArgs"/> event.</param>
        /// <param name="e"><see cref="ItemsProducedEventArgs"/> associated with the event.</param>
        private void TriggerItemsProduced(object sender, ItemsProducedEventArgs e)
        {
            this.OnItemsProduced(e);
        }

        /// <summary>
        /// Checks whether the process has stopped.
        /// </summary>
        /// <param name="sender">The sender of the <see cref="ProcessStoppedEventArgs"/> event.</param>
        /// <param name="e"><see cref="ProcessStoppedEventArgs"/> associated with the event.</param>
        private void CheckForProcessStopped(object sender, ProcessStoppedEventArgs e)
        {
            this.StopProcessIfStopping(e.EventError);
        }

        /// <summary>
        /// Updates the health state of this task engine.
        /// </summary>
        /// <param name="sender">The sender of the <see cref="ProcessStoppedEventArgs"/> event.</param>
        /// <param name="e"><see cref="ProcessStoppedEventArgs"/> associated with the event.</param>
        private void UpdateProcessHealth(object sender, ProcessStoppedEventArgs e)
        {
            if (e.EventError != null)
            {
                this.ProcessHealth = UserResultState.ProcessError;
            }
        }

        #endregion

        #region Event Methods

        /// <summary>
        /// Triggers the <see cref="ItemsAdded"/> event.
        /// </summary>
        /// <param name="e"><see cref="ItemsAddedEventArgs"/> associated with the event.</param>
        [DebuggerHidden]
        private void OnItemsAdded(ItemsAddedEventArgs e)
        {
            EventHandler<ItemsAddedEventArgs> temp = this.ItemsAdded;

            if (temp != null)
            {
                temp(this, e);
            }
        }

        /// <summary>
        /// Triggers the <see cref="ItemsProduced"/> event.
        /// </summary>
        /// <param name="e"><see cref="ItemsProducedEventArgs"/> associated with the event.</param>
        [DebuggerHidden]
        private void OnItemsProduced(ItemsProducedEventArgs e)
        {
            EventHandler<ItemsProducedEventArgs> temp = this.ItemsProduced;

            if (temp != null)
            {
                temp(this, e);
            }
        }

        #endregion

        #endregion
    }
}
