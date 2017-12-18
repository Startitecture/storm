namespace SAF.UserInterface
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Threading;
    using System.Windows.Input;
    using System.Windows.Threading;

    using SAF.Data.Integration;
    using SAF.ProcessEngine;

    /// <summary>
    /// View model for a data integration.
    /// </summary>
    /// <typeparam name="TItem">The type of the items in the source.</typeparam>
    /// <typeparam name="TEntity">The type of persistent model converted from the items.</typeparam>
    public class DataIntegrationViewModel<TItem, TEntity> : IDataIntegrationViewModel, IDisposable
    {
        /// <summary>
        /// An object that controls access to the event queue.
        /// </summary>
        private readonly object messageQueueControl = new object();

        /// <summary>
        /// A queue that holds messages for the main thread.
        /// </summary>
        private readonly Queue<object> mainThreadMessageQueue = new Queue<object>();

        /// <summary>
        /// An object that controls access to integration controller registration.
        /// </summary>
        private readonly object setCommandLock = new object();

        /// <summary>
        /// Allows processing of background items in the owner's thread.
        /// </summary>
        private readonly BackgroundWorker eventWorker;

        /// <summary>
        /// Sends messages back to the main (UI) thread.
        /// </summary>
        private readonly Dispatcher messageDispatcher;

        /// <summary>
        /// Gets a <see cref="IntegrationMonitor"/> to monitor the data integration process.
        /// </summary>
        private IntegrationMonitor integrationMonitor;

        /// <summary>
        /// Gets a <see cref="TaskMonitor"/> to monitor the conversion process.
        /// </summary>
        private TaskMonitor converterMonitor;

        /// <summary>
        /// Gets a <see cref="TaskMonitor"/> to monitor the update process.
        /// </summary>
        private TaskMonitor updateMonitor;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataIntegrationViewModel&lt;TItem, TEntity&gt;"/> class with the specified 
        /// controller and command.
        /// </summary>
        /// <param name="controller">The controller that executes the integration.</param>
        /// <param name="messageDispatcher">A dispatcher from the UI thread.</param>
        protected DataIntegrationViewModel(IIntegrationController<TItem, TEntity> controller, Dispatcher messageDispatcher)
        {
            if (controller == null)
            {
                throw new ArgumentNullException("controller");
            }

            if (messageDispatcher == null)
            {
                throw new ArgumentNullException("messageDispatcher");
            }

            this.Name = controller.Name;
            this.messageDispatcher = messageDispatcher;
            this.FailedItems = new ObservableCollection<FailedItem>();
            this.FailedModels = new ObservableCollection<FailedItem>();

            this.integrationMonitor = new IntegrationMonitor(controller);
            this.converterMonitor = new TaskMonitor(controller.Converter);
            this.updateMonitor = new TaskMonitor(controller.Updater);

            ////this.integrationController.RetrievalFailed += this.AddItemToFailedList;
            controller.ConversionFailed += this.AddItemToFailedList;
            controller.PersistenceFailed += this.AddModelToFailedList;

            controller.StateChanged += delegate(object o, StateChangedEventArgs<IntegrationState> e)
            {
                this.OnStateChanged(e);
            };

            using (this.eventWorker = new BackgroundWorker())
            {
                this.eventWorker.WorkerReportsProgress = true;
                this.eventWorker.DoWork += this.WorkerUpdateQueue;
                this.eventWorker.ProgressChanged += new ProgressChangedEventHandler(this.ProcessWorkerProgressUpdates);
                this.eventWorker.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Occurs when there is a state change in the view model.
        /// </summary>
        public event EventHandler<StateChangedEventArgs<IntegrationState>> StateChanged;

        /// <summary>
        /// Occurs when a command completes.
        /// </summary>
        public event EventHandler<CommandCompletedEventArgs> CommandCompleted;

        /// <summary>
        /// Occurs when a command fails.
        /// </summary>
        public event EventHandler<CommandFailedEventArgs> CommandFailed;

        #region Properties

        /// <summary>
        /// Gets the name of the integration controller.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets an <see cref="ICommand"/> that starts the integration process.
        /// </summary>
        public ICommand IntegrationCommand { get; private set; }

        /// <summary>
        /// Gets a <see cref="IntegrationMonitor"/> to monitor the data integration process.
        /// </summary>
        public IntegrationMonitor IntegrationMonitor
        {
            get { return this.integrationMonitor; }
        }

        /// <summary>
        /// Gets a <see cref="TaskMonitor"/> to monitor the conversion process.
        /// </summary>
        public TaskMonitor ConverterMonitor
        {
            get { return this.converterMonitor; }
        }

        /// <summary>
        /// Gets a <see cref="TaskMonitor"/> to monitor the update process.
        /// </summary>
        public TaskMonitor UpdateMonitor
        {
            get { return this.updateMonitor; }
        }

        /// <summary>
        /// Gets a collection of items that failed to convert.
        /// </summary>
        public ObservableCollection<FailedItem> FailedItems { get; private set; }

        /// <summary>
        /// Gets a collection of models that failed to update.
        /// </summary>
        public ObservableCollection<FailedItem> FailedModels { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Disposes of resources managed by this instance.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of resources managed by this instance.
        /// </summary>
        /// <param name="disposing">True to indicate that the object is being explicitly disposed; otherwise, false.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.ConverterMonitor.Dispose();
                this.IntegrationMonitor.Dispose();
                this.UpdateMonitor.Dispose();
            }

            // Clean up native resources here.
        }

        /// <summary>
        /// Initializes an integration controller for this view model.
        /// </summary>
        /// <param name="command">The command that starts the integration process.</param>
        protected void SetIntegrationCommand(IObservableCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            lock (this.setCommandLock)
            {
                command.CommandFailed += this.TriggerCommandFailed;
                command.CommandCompleted += this.TriggerCommandCompleted;
                this.IntegrationCommand = command;
            }
        }

        /// <summary>
        /// Handles command completion.
        /// </summary>
        /// <param name="sender">The command that completed.</param>
        /// <param name="e">Event data associated with the event.</param>
        private void TriggerCommandCompleted(object sender, CommandCompletedEventArgs e)
        {
            EventHandler<CommandCompletedEventArgs> temp = this.CommandCompleted;

            if (temp != null)
            {
                this.messageDispatcher.BeginInvoke(temp, this, e);
            }
        }

        /// <summary>
        /// Handles command failures.
        /// </summary>
        /// <param name="sender">The command that failed.</param>
        /// <param name="e">Event data associated with the event.</param>
        private void TriggerCommandFailed(object sender, CommandFailedEventArgs e)
        {
            EventHandler<CommandFailedEventArgs> temp = this.CommandFailed;

            if (temp != null)
            {
                this.messageDispatcher.BeginInvoke(temp, this, e);
            }
        }

        /// <summary>
        /// Sends a message to be processed by the main thread.
        /// </summary>
        /// <param name="message">The message to process.</param>
        private void SendMessageToMainThread(object message)
        {
            lock (this.messageQueueControl)
            {
                this.mainThreadMessageQueue.Enqueue(message);
                Monitor.Pulse(this.messageQueueControl);
            }
        }

        /// <summary>
        /// Starts the event worker queue.
        /// </summary>
        /// <param name="sender">The event worker.</param>
        /// <param name="e">The <see cref="DoWorkEventArgs"/> to process.</param>
        private void WorkerUpdateQueue(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                lock (this.messageQueueControl)
                {
                    if (this.mainThreadMessageQueue.Count == 0)
                    {
                        Monitor.Wait(this.messageQueueControl);
                    }

                    this.eventWorker.ReportProgress(0, this.mainThreadMessageQueue.Dequeue());
                }
            }
        }

        /// <summary>
        /// Processes a worker update event.
        /// </summary>
        /// <param name="sender">The worker updating its progress.</param>
        /// <param name="e">Event data associated with the event.</param>
        private void ProcessWorkerProgressUpdates(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState is FailedItem)
            {
                FailedItem item = e.UserState as FailedItem;

                if (item.Item is TItem)
                {
                    this.FailedItems.Add(item);
                }
                else if (item.Item is TEntity)
                {
                    this.FailedModels.Add(item);
                }
            }
        }

        /// <summary>
        /// Sends the failed item to the failed item list.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e"><see cref="FailedItemEventArgs&lt;TItem&gt;"/> associated with the event.</param>
        private void AddItemToFailedList(object sender, FailedItemEventArgs<TItem> e)
        {
            this.SendMessageToMainThread(new FailedItem(e.FailedItem, e.ItemError));
        }

        /// <summary>
        /// Sends the failed item to the failed item list.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e"><see cref="FailedItemEventArgs&lt;TModel&gt;"/> associated with the event.</param>
        private void AddModelToFailedList(object sender, FailedItemEventArgs<TEntity> e)
        {
            this.SendMessageToMainThread(new FailedItem(e.FailedItem, e.ItemError));
        }

        /// <summary>
        /// Triggers the <see cref="StateChanged"/> event.
        /// </summary>
        /// <param name="e"><see cref="StateChangedEventArgs&lt;IntegrationState&gt;"/> associated with the event.</param>
        private void OnStateChanged(StateChangedEventArgs<IntegrationState> e)
        {
            EventHandler<StateChangedEventArgs<IntegrationState>> temp = this.StateChanged;

            if (temp != null)
            {
                temp(this, e);
            }
        }

        #endregion
    }
}
