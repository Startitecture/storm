namespace SAF.Data.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SAF.Data;
    using SAF.Data.Persistence;
    using SAF.ProcessEngine;

    /// <summary>
    /// Updates a target dataset with data from a data source.
    /// </summary>
    /// <typeparam name="TItem">The type of item in the source.</typeparam>
    /// <typeparam name="TEntity">The type of entity converted from the items.</typeparam>
    public class IntegrationController<TItem, TEntity> : 
        ProcessControllerBase<IntegrationState>,
        IIntegrationController<TItem, TEntity>
    {
        #region Constants

        /// <summary>
        /// Name mask for this class.
        /// </summary>
        private const string NameMask = "Integration Controller ({0})";

        #endregion

        #region Fields

        /// <summary>
        /// A function for determining whether the update has completed.
        /// </summary>
        private readonly Func<object, ProcessStoppedEventArgs, bool> integrationCompleteCondition;

        /// <summary>
        /// A locking object for data proxy trigger registration.
        /// </summary>
        private readonly object registrationLock = new object();

        /////// <summary>
        /////// A locking object for policy initialization.
        /////// </summary>
        ////private readonly object policyLock = new object();

        /////// <summary>
        /////// A locking object for target preparation operations.
        /////// </summary>
        ////private readonly object targetPreparationLock = new object();

        /////// <summary>
        /////// A list of currently registered policy state triggers.
        /////// </summary>
        ////private readonly List<IStateTrigger<IntegrationState>> stateTriggers = 
        ////    new List<IStateTrigger<IntegrationState>>();

        /// <summary>
        /// The adapter that saves individual entities.
        /// </summary>
        private IPersistenceAdapter<TEntity> persistenceAdapter;

        /////// <summary>
        /////// When executed, prepares the integration process.
        /////// </summary>
        ////private IExecutable preparationCommand;

        /////// <summary>
        /////// When executed, finalizes the integration process.
        /////// </summary>
        ////private IExecutable finalizationCommand;

        /// <summary>
        /// The data converter for this integration controller.
        /// </summary>
        private IDataConverter<TItem, TEntity> dataConverter;

        /// <summary>
        /// A list of data proxies currently registered to this integration controller.
        /// </summary>
        private List<IDataProxy<TItem>> dataProxies = new List<IDataProxy<TItem>>();

        /// <summary>
        /// The maximum length of the data store, converter and persister queues.
        /// </summary>
        private long maxQueueLength = Int64.MaxValue;

        /// <summary>
        /// The item converter that will convert the items.
        /// </summary>
        private ItemConverter<TItem, TEntity> conversionEngine = new ItemConverter<TItem, TEntity>();

        /// <summary>
        /// The data updater that will update the target.
        /// </summary>
        private EntityUpdater<TEntity> updater = new EntityUpdater<TEntity>();

        /////// <summary>
        /////// A value that determines whether the target has been prepared. This is done once per process.
        /////// </summary>
        ////private bool targetPrepared = false;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="IntegrationController&lt;TItem, TEntity&gt;"/> class with
        /// the specified update policy, target preparation command and source finalization command.
        /// </summary>
        /// <param name="name">The name of this integration controller.</param>
        public IntegrationController(string name)
            : base(name, IntegrationState.NotStarted, IntegrationState.Stopped)
        {
            this.RegisterStateMachineEvents(this);
            this.RegisterComponents(ProcessType.Producer, this.conversionEngine);
            this.RegisterComponents(ProcessType.Consumer, this.updater);

            ////// Reset the preparation states.
            ////this.ProcessStopped += delegate(object o, ProcessStoppedEventArgs e)
            ////{
            ////    this.targetPrepared = false;
            ////};

            this.integrationCompleteCondition = (s, e) => this.IsIntegrationCompleted(s, e);

            // Pass items from the converter to the persister.
            this.conversionEngine.ItemsProduced += this.PersistEntity;

            // Process the results of the persister.
            this.updater.ItemsProduced += this.UpdatePersistenceResults;

            this.RegisterStateTriggers(
                new ProcessStartStateTrigger<IntegrationState>(
                    IntegrationState.IntegrationStarted, this.conversionEngine));

            // The integration completed state change always fires.
            this.RegisterStateTriggers(
                new ProcessStopStateTrigger<IntegrationState>(
                    IntegrationState.IntegrationCompleted, this.integrationCompleteCondition));

            this.RegisterStateTriggers(
                new ProcessStopStateTrigger<IntegrationState>(
                    IntegrationState.Stopped, this.integrationCompleteCondition));
        }

        #endregion

        /// <summary>
        /// Occurs when an item cannot be converted.
        /// </summary>
        public event EventHandler<FailedItemEventArgs<TItem>> ConversionFailed;

        /// <summary>
        /// Occurs when an entity cannot be persisted.
        /// </summary>
        public event EventHandler<FailedItemEventArgs<TEntity>> PersistenceFailed;

        #region Properties

        /// <summary>
        /// Gets an interface to the item converter.
        /// </summary>
        public ITaskEngine Converter
        {
            get { return this.conversionEngine; }
        }

        /// <summary>
        /// Gets an interface to the entity updater.
        /// </summary>
        public ITaskEngine Updater
        {
            get { return this.updater; }
        }

        /// <summary>
        /// Gets or sets the maximum length of the update queue.
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
                this.conversionEngine.MaxQueueLength = this.maxQueueLength;
                this.updater.MaxQueueLength = this.maxQueueLength;

                foreach (IDataProxy<TItem> proxy in this.dataProxies)
                {
                    proxy.MaxQueueLength = this.maxQueueLength;
                }
            }
        }

        /// <summary>
        /// Gets the number of added items this updater has processed.
        /// </summary>
        public long AddedItems
        {
            get { return this.updater.AddedItems; }
        }

        /// <summary>
        /// Gets the number of modified items this updater has processed.
        /// </summary>
        public long ModifiedItems
        {
            get { return this.updater.ModifiedItems; }
        }

        /// <summary>
        /// Gets the number of unchanged items this updater has processed.
        /// </summary>
        public long UnchangedItems
        {
            get { return this.updater.UnchangedItems;  }
        }

        /// <summary>
        /// Gets the number of removed items this updater has processed.
        /// </summary>
        public long RemovedItems
        {
            get { return this.updater.RemovedItems; }
        }

        /// <summary>
        /// Gets the number of rolled back items this updater has processed.
        /// </summary>
        public long RolledBackItems
        {
            get { return this.updater.RolledBackItems; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a string representation of this target update controller.
        /// </summary>
        /// <returns>A string containing the name of the dataset that this controller
        /// is using to update the target database.</returns>
        public override string ToString()
        {
            return String.Format(NameMask, this.Name);
        }

        #region Registration

        /// <summary>
        /// Registers integration components for this controller.
        /// </summary>
        /// <param name="converter">The data converter for the integration.</param>
        /// <param name="adapter">The adapter for the target of the integration.</param>
        /// <param name="proxies">The data proxies containing the source data.</param>
        public void RegisterIntegrationComponents(
            ////AggregateCommand preparationCommands, 
            ////AggregateCommand finalizationCommands,
            IDataConverter<TItem, TEntity> converter, 
            IPersistenceAdapter<TEntity> adapter, 
            params IDataProxy<TItem>[] proxies)
        {
            ////if (preparationCommands == null)
            ////{
            ////    throw new ArgumentNullException("preparationCommands");
            ////}

            ////if (finalizationCommands == null)
            ////{
            ////    throw new ArgumentNullException("finalizationCommands");
            ////}

            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }

            if (adapter == null)
            {
                throw new ArgumentNullException("adapter");
            }

            if (proxies == null)
            {
                throw new ArgumentNullException("proxies");
            }

            lock (this.registrationLock)
            {
                this.dataConverter = converter;
                this.persistenceAdapter = adapter;
                ////this.RegisterPreparationCommand(preparationCommands);
                ////this.RegisterFinalizationCommand(finalizationCommands);

                foreach (var proxy in proxies)
                {
                    ////// Validate that the source and target are ready for the integration process.
                    ////proxy.ProcessStarted += this.PrepareTarget;

                    // Pass items from the source data store to the converter.
                    proxy.ItemsProduced += this.ConvertItemToEntity;

                    // Keep the queue at the max queue length specified for this controller.
                    proxy.MaxQueueLength = this.MaxQueueLength;

                    // Create registrations and triggers for the new data store and policy.
                    this.RegisterComponents(ProcessType.Auxiliary, proxy);
                }

                this.dataProxies.AddRange(proxies);
            }
        }

        /// <summary>
        /// Deregisters integration components for this controller.
        /// </summary>
        public void DeregisterIntegrationComponents()
        {
            lock (this.registrationLock)
            {
                this.dataConverter = null;
                this.persistenceAdapter = null;
                
                ////if (this.preparationCommand != null)
                ////{
                ////    this.preparationCommand.CommandCompleted -= this.SetTargetAsPrepared;
                ////    this.DeregisterCommand(this.preparationCommand);
                ////}

                ////if (this.finalizationCommand != null)
                ////{
                ////    this.DeregisterCommand(this.finalizationCommand);
                ////}

                foreach (var proxy in this.dataProxies)
                {
                    proxy.ItemsProduced -= this.ConvertItemToEntity;
                    ////proxy.ProcessStarted -= this.PrepareTarget;
                    this.DeregisterComponents(ProcessType.Auxiliary, proxy);
                }

                this.dataProxies.Clear();
            }
        }

        /////// <summary>
        /////// Registers the preparation command.
        /////// </summary>
        /////// <param name="command">The command to register.</param>
        ////private void RegisterPreparationCommand(AggregateCommand command)
        ////{
        ////    if (command != AggregateCommand.Empty)
        ////    {
        ////        command.CommandCompleted += this.SetTargetAsPrepared;

        ////        RegisterCommandEvents(command);

        ////        this.stateTriggers.AddRange(
        ////            RegisterStateTriggers(
        ////                TriggerGeneration.CreateCommandStateTriggers(
        ////                    command,
        ////                    IntegrationState.PreparingTarget,
        ////                    IntegrationState.Stopped,
        ////                    IntegrationState.TargetPrepared)));
        ////    }

        ////    this.preparationCommand = command;
        ////}

        /////// <summary>
        /////// Registers the finalization command.
        /////// </summary>
        /////// <param name="command">The command to register.</param>
        ////private void RegisterFinalizationCommand(AggregateCommand command)
        ////{
        ////    if (command != AggregateCommand.Empty)
        ////    {
        ////        RegisterCommandEvents(command);
        ////        RegisterCommandTriggers(
        ////            new StateChangedCommandTrigger<IntegrationState>(
        ////                command, this, IntegrationState.IntegrationCompleted));

        ////        this.stateTriggers.AddRange(
        ////            RegisterStateTriggers(
        ////                TriggerGeneration.CreateCommandStateTriggers(
        ////                    command,
        ////                    IntegrationState.FinalizingTarget,
        ////                    IntegrationState.Stopped,
        ////                    IntegrationState.Stopped)));
        ////    }
        ////    else
        ////    {
        ////    }

        ////    ////this.finalizationCommand = command;
        ////}

        /////// <summary>
        /////// Deregisters the specified command by removing its command events, command triggers and state triggers.
        /////// </summary>
        /////// <param name="command">The command to deregister.</param>
        ////private void DeregisterCommand(IExecutable command)
        ////{
        ////    this.DeregisterCommandEvents(command);
        ////    this.DeregisterCommandTriggers(command);

        ////    var triggers =
        ////        this.stateTriggers.OfType<CommandCompletedStateTrigger<IntegrationState>>()
        ////            .Where(x => command.Equals(x.Command)).ToList();

        ////    foreach (var item in triggers)
        ////    {
        ////        this.DeregisterStateTriggers(item);
        ////    }
        ////}

        #endregion

        #region Event Handlers

        /////// <summary>
        /////// Prepares the target (if necessary) before the data extraction begins.
        /////// </summary>
        /////// <param name="sender">The sender of the <see cref="ProcessStartedEventArgs"/> event. This should be a 
        /////// <see cref="IDataProxy&lt;TItem&gt;"/> registered to the controller.</param>
        /////// <param name="e"><see cref="ProcessStartedEventArgs"/> associated with the event.</param>
        ////private void PrepareTarget(object sender, ProcessStartedEventArgs e)
        ////{
        ////    lock (this.targetPreparationLock)
        ////    {
        ////        if (!Evaluate.Equals(this.preparationCommand, AggregateCommand.Empty) && !this.targetPrepared)
        ////        {
        ////            this.preparationCommand.ExecuteAsync = false;
        ////            this.ExecuteCommand(this.preparationCommand, null);
        ////        }
        ////    }
        ////}

        /////// <summary>
        /////// Sets the target as prepared.
        /////// </summary>
        /////// <param name="sender">The sender of the <see cref="CommandCompletedEventArgs"/> event.</param>
        /////// <param name="e"><see cref="CommandCompletedEventArgs"/> associated with the event.</param>
        ////private void SetTargetAsPrepared(object sender, CommandCompletedEventArgs e)
        ////{
        ////    lock (this.targetPreparationLock)
        ////    {
        ////        this.targetPrepared = true;
        ////    }
        ////}

        /// <summary>
        /// Determines whether the integration process has been completed.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e"><see cref="ProcessStoppedEventArgs"/> associated with the event.</param>
        /// <returns>True if the persister, source data store and converter are all idle, otherwise false.</returns>
        private bool IsIntegrationCompleted(object sender, ProcessStoppedEventArgs e)
        {
            if (sender is IProcessEngine && this.Processes.Contains(sender as IProcessEngine))
            {
                // Check error conditions.
                if (e.EventError != null)
                {
                    this.StopAllComponents();
                    return true;
                }

                // Check normal conditions.
                if (!this.updater.IsBusy && !this.conversionEngine.IsBusy)
                {
                    foreach (IDataProxy<TItem> proxy in this.dataProxies)
                    {
                        if (proxy.IsBusy)
                        {
                            return false;
                        }
                    }

                    // An error associated with the failure of any component means the entire process stops.
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Stops all integration components.
        /// </summary>
        private void StopAllComponents()
        {
            foreach (var proxy in this.dataProxies)
            {
                if (proxy.IsBusy)
                {
                    proxy.Cancel();
                }
            }

            if (this.conversionEngine.IsBusy)
            {
                this.conversionEngine.Cancel();
            }

            if (this.updater.IsBusy)
            {
                this.updater.Cancel();
            }
        }

        /// <summary>
        /// Queues an item to be persisted as an entity.
        /// </summary>
        /// <param name="sender">The data store.</param>
        /// <param name="e">Event data associated with the event.</param>
        private void ConvertItemToEntity(object sender, ItemsProducedEventArgs e)
        {
            IDataProxy<TItem> proxy = sender as IDataProxy<TItem>;

            while (proxy.Items.ConsumeNext())
            {
                TItem result = proxy.Items.Current;

                this.conversionEngine.ConvertItem(
                    new ItemConversionDirective<TItem, TEntity>(
                        result, this.dataConverter));
            }
        }

        /// <summary>
        /// Queues a entity update directive to be persisted.
        /// </summary>
        /// <param name="sender">The converter.</param>
        /// <param name="e">Event data associated with the event.</param>
        private void PersistEntity(object sender, ItemsProducedEventArgs e)
        {
            ItemConverter<TItem, TEntity> itemConverter =
                sender as ItemConverter<TItem, TEntity>;

            while (itemConverter.TaskResultConsumer.ConsumeNext())
            {
                ItemConversionResult<TItem, TEntity> result =
                    itemConverter.TaskResultConsumer.Current;

                if (result.ResultState == ResultState.Success)
                {
                    this.updater.UpdateEntity(
                        new EntityUpdateDirective<TEntity>(
                            this.persistenceAdapter, result.Result));
                }
                else if (result.ResultState == ResultState.Error)
                {
                    this.OnConversionFailed(
                        new FailedItemEventArgs<TItem>(
                            result.Directive.SourceItem, result.ItemError));
                }
            }
        }

        /// <summary>
        /// Updates the data integration controller with the results of the persistence action.
        /// </summary>
        /// <param name="sender">The sender of the entity update results.</param>
        /// <param name="e">The <see cref="ItemsProducedEventArgs"/> associated with the update results.</param>
        private void UpdatePersistenceResults(object sender, ItemsProducedEventArgs e)
        {
            EntityUpdater<TEntity> entityUpdater = sender as EntityUpdater<TEntity>;

            while (entityUpdater.TaskResultConsumer.ConsumeNext())
            {
                EntityUpdateResult<TEntity> result = entityUpdater.TaskResultConsumer.Current;

                if (result.Result == EntityAction.Rollback)
                {
                    this.OnPersistenceFailed(
                        new FailedItemEventArgs<TEntity>(
                            result.Directive.Entity, result.ItemError));
                }
            }
        }

        #endregion

        #region Event Methods

        /// <summary>
        /// Triggers the <see cref="ConversionFailed"/> event.
        /// </summary>
        /// <param name="e"><see cref="FailedItemEventArgs&lt;TItem&gt;"/> associated with the event.</param>
        private void OnConversionFailed(FailedItemEventArgs<TItem> e)
        {
            EventHandler<FailedItemEventArgs<TItem>> temp = this.ConversionFailed;

            if (temp != null)
            {
                temp(this, e);
            }
        }

        /// <summary>
        /// Triggers the <see cref="PersistenceFailed"/> event.
        /// </summary>
        /// <param name="e"><see cref="FailedItemEventArgs&lt;TItem&gt;"/> associated with the event.</param>
        private void OnPersistenceFailed(FailedItemEventArgs<TEntity> e)
        {
            EventHandler<FailedItemEventArgs<TEntity>> temp = this.PersistenceFailed;

            if (temp != null)
            {
                temp(this, e);
            }
        }

        #endregion

        #endregion
    }
}
