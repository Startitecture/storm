// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessControllerBase.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Base class for controlling state-oriented processes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ProcessEngine
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// Base class for controlling state-oriented processes.
    /// </summary>
    /// <typeparam name="TState">The type that represents the possible states of the process.</typeparam>
    public class ProcessControllerBase<TState> : 
        StateMachineBase<TState>, 
        IProcessController<TState>
    {
        #region Constants

        #endregion

        #region Fields

        /// <summary>
        /// A list of registered processes.
        /// </summary>
        private readonly List<ProcessRegistration> registeredProcesses = new List<ProcessRegistration>();

        /// <summary>
        /// The list of <see cref="ICommandTrigger"/>s that fire commands when matching events are received.
        /// </summary>
        private readonly List<ICommandTrigger> commandTriggers = new List<ICommandTrigger>();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessControllerBase&lt;TState&gt;"/> class.
        /// </summary>
        /// <param name="name">The name of the process controller.</param>
        /// <param name="initialState">The initial state of this machine.</param>
        /// <param name="idleState">The idle state of this machine.</param>
        protected ProcessControllerBase(string name, TState initialState, TState idleState)
            : base(name, initialState, idleState)
        {
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs immediately before the command executes.
        /// </summary>
        public event EventHandler<CommandExecutingEventArgs> CommandExecuting;

        /// <summary>
        /// Occurs when a command issued by the process has finished executing.
        /// </summary>
        public event EventHandler<CommandCompletedEventArgs> CommandCompleted;

        /// <summary>
        /// Occurs when a command issued by the process has failed during execution.
        /// </summary>
        public event EventHandler<CommandFailedEventArgs> CommandFailed;

        #endregion

        #region Properties
        
        /// <summary>
        /// Gets the average time per task.
        /// </summary>
        public double TasksPerSecond
        {
            get
            {
                if (!this.Consumers.OfType<ITaskEngine>().Where(Queries.TasksGreaterThanZero).Any())
                {
                    return 0;
                }

                IEnumerable<ITaskEngine> consumers = this.Consumers.OfType<ITaskEngine>().Where(Queries.TasksGreaterThanZero);
                return consumers.Sum(Queries.CompletedTasks) / this.ProcessTime.TotalSeconds;
            }
        }

        /// <summary>
        /// Gets the health of the current process in terms of whether tasks or commands have failed.
        /// </summary>
        public UserResultState ProcessHealth { get; private set; }

        /// <summary>
        /// Gets the total number of tasks provided to the process.
        /// </summary>
        public long TotalTasks
        {
            get 
            {
                return this.Producers.OfType<ITaskEngine>().Sum(Queries.TotalTasks);
            }
        }

        /// <summary>
        /// Gets the total number of tasks completed by the process.
        /// </summary>
        public long WaitingTasks
        {
            get 
            {
                return this.Producers.OfType<ITaskEngine>().Sum(Queries.WaitingTasks);
            }
        }

        /// <summary>
        /// Gets the total number of tasks completed by the process.
        /// </summary>
        public long CompletedTasks
        {
            get 
            { 
                return this.Consumers.OfType<ITaskEngine>().Sum(Queries.CompletedTasks);
            }
        }

        /// <summary>
        /// Gets the progress of the process.
        /// </summary>
        public double Progress 
        { 
            get
            {
                return this.TaskEngines.Where(Queries.TasksGreaterThanZero).Any()
                           ? this.TaskEngines.Where(Queries.TasksGreaterThanZero).Average(Queries.Progress)
                           : 0;
            }
        }

        /// <summary>
        /// Gets the number of tasks that were processed successfully.
        /// </summary>
        public long SuccessfulResults
        {
            get 
            {
                return this.Consumers.OfType<ITaskEngine>().Sum(Queries.SuccessfulResults);
            }
        }

        /// <summary>
        /// Gets the number of tasks that returned no result.
        /// </summary>
        public long EmptyResults
        {
            get
            {
                return this.Consumers.OfType<ITaskEngine>().Sum(Queries.EmptyResults);
            }
        }

        /// <summary>
        /// Gets the number of tasks that failed due to an exception.
        /// </summary>
        public long FailedResults
        {
            get
            {
                return this.Consumers.OfType<ITaskEngine>().Sum(Queries.FailedResults);
            }
        }

        /// <summary>
        /// Gets all process engines registered to this process controller.
        /// </summary>
        protected IEnumerable<IProcessEngine> Processes
        {
            get { return this.registeredProcesses.Select(Queries.EngineSelector).Distinct(); }
        }

        /// <summary>
        /// Gets all task engines registered to this process controller.
        /// </summary>
        protected IEnumerable<ITaskEngine> TaskEngines
        {
            get { return this.registeredProcesses.Select(Queries.EngineSelector).OfType<ITaskEngine>().Distinct(); }
        }

        /// <summary>
        /// Gets all producers registered to this process controller.
        /// </summary>
        protected IEnumerable<IProcessEngine> Producers
        {
            get
            {
                return this.registeredProcesses.Where(Queries.ProducerSelector).Select(Queries.EngineSelector).Distinct();
            }
        }

        /// <summary>
        /// Gets all consumers registered to this process controller.
        /// </summary>
        protected IEnumerable<IProcessEngine> Consumers
        {
            get
            {
                return this.registeredProcesses.Where(Queries.ConsumerSelector).Select(Queries.EngineSelector).Distinct();
            }
        }

        /// <summary>
        /// Gets all auxiliary processes registered to this process controller.
        /// </summary>
        protected IEnumerable<IProcessEngine> Auxiliaries
        {
            get
            {
                return this.registeredProcesses.Where(Queries.AuxiliarySelector).Select(Queries.EngineSelector).Distinct();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Cancels the current process.
        /// </summary>
        /// <param name="userState"></param>
        /// <param name="processError"></param>
        protected virtual void CancelProcess(object userState, Exception processError)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether this instance is stopping.
        /// </summary>
        /// <returns>True if all registered processes are stopped, otherwise false.</returns>
        protected override bool IsStopping()
        {
// ReSharper disable LoopCanBeConvertedToQuery - for performance.
            foreach (ProcessRegistration registration in this.registeredProcesses)
// ReSharper restore LoopCanBeConvertedToQuery
            {
                // If any registered process is still busy then the controller has not stopped.
                if (registration.Process.IsBusy)
                {
                    return false;
                }
            }

            return true;
        }

        #region Command Logic

        /// <summary>
        /// Registers command triggers for this process.
        /// </summary>
        /// <param name="triggers">
        /// A list of command triggers that will execute commands when certain event conditions are met.</param>
        [DebuggerHidden]
        protected void RegisterCommandTriggers(params ICommandTrigger[] triggers)
        {
            lock (this.commandTriggers)
            {
                this.commandTriggers.AddRange(triggers);
            }
        }

        /// <summary>
        /// Deregisters command triggers for this process.
        /// </summary>
        /// <param name="commands">The commands to deregister triggers for.</param>
        protected void DeregisterCommandTriggers(params IExecutable[] commands)
        {
            // ToList() is required, otherwise the expression is evaluated each time.
            List<ICommandTrigger> triggersToRemove = 
                this.commandTriggers.Where(x => commands.Contains(x.Command)).ToList();

            foreach (ICommandTrigger trigger in triggersToRemove)
            {
                this.commandTriggers.Remove(trigger);
            }
        }

        /// <summary>
        /// Clears all command triggers for this process.
        /// </summary>
        protected void ClearCommandTriggers()
        {
            lock (this.commandTriggers)
            {
                this.commandTriggers.Clear();
            }
        }

        /// <summary>
        /// Executes a command directly by adding it to the command queue.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this 
        /// object can be set to null.</param>
        [DebuggerHidden]
        protected void ExecuteCommand(IExecutable command, object parameter)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            command.Execute(parameter);
        }

        #endregion

        #region Event Registration

        /// <summary>
        /// Registers state changed events for the provided state machines.
        /// </summary>
        /// <remarks>
        /// Only register state machines that are associated with state change or command triggers for this process.
        /// </remarks>
        /// <typeparam name="T">The type of state machine to register.</typeparam>
        /// <param name="stateMachines">The state machines to register.</param>
        [DebuggerHidden]
        protected void RegisterStateMachineEvents<T>(params IStateMachine<T>[] stateMachines)
        {
            if (stateMachines == null)
            {
                throw new ArgumentNullException("stateMachines");
            }

            foreach (IStateMachine<T> stateMachine in stateMachines)
            {
                stateMachine.StateChanging += this.RegisteredEventHandler;
                stateMachine.StateChanged += this.RegisteredEventHandler;
            }
        }

        /// <summary>
        /// Deregisters state changed events for the provided state machines.
        /// </summary>
        /// <typeparam name="T">The type of state machine to deregister.</typeparam>
        /// <param name="stateMachines">The state machines to deregister.</param>
        [DebuggerHidden]
        protected void DeregisterStateMachineEvents<T>(params IStateMachine<T>[] stateMachines)
        {
            if (stateMachines == null)
            {
                throw new ArgumentNullException("stateMachines");
            }

            foreach (IStateMachine<T> stateMachine in stateMachines)
            {
                stateMachine.StateChanging -= this.RegisteredEventHandler;
                stateMachine.StateChanged -= this.RegisteredEventHandler;
            }
        }

        /// <summary>
        /// Registers command events for the specified commands.
        /// </summary>
        /// <param name="commands">The commands to register.</param>
        /// <remarks>
        /// Only register commands that are associated with state change or command triggers for this process.
        /// </remarks>
        [DebuggerHidden]
        protected void RegisterCommandEvents(params ICommandDispatcher[] commands)
        {
            if (commands == null)
            {
                throw new ArgumentNullException("commands");
            }

            foreach (ICommandDispatcher command in commands)
            {
                command.CommandExecuting += this.RegisteredEventHandler;
                command.CommandCompleted += this.RegisteredEventHandler;
                command.CommandFailed += this.RegisteredEventHandler;
            }
        }

        /// <summary>
        /// Deregisters command events for the specified commands.
        /// </summary>
        /// <param name="commands">The commands to deregister.</param>
        protected void DeregisterCommandEvents(params ICommandDispatcher[] commands)
        {
            if (commands == null)
            {
                throw new ArgumentNullException("commands");
            }

            foreach (IExecutable command in commands)
            {
                command.CommandExecuting -= this.RegisteredEventHandler;
                command.CommandCompleted -= this.RegisteredEventHandler;
                command.CommandFailed -= this.RegisteredEventHandler;
            }
        }

        #endregion

        #region Component Registration

        /// <summary>
        /// Registers components with this process controller.
        /// </summary>
        /// <param name="processType">The type of process to register.</param>
        /// <param name="components">The components to register.</param>
        protected void RegisterComponents(ProcessType processType, params IProcessEngine[] components)
        {
            if (components == null)
            {
                throw new ArgumentNullException("components");
            }

            // Register events unless they have already been registered.
            foreach (IProcessEngine component in components)
            {
                if (!this.registeredProcesses.Select(Queries.EngineSelector).Contains(component))
                {
                    component.ProcessStarted += this.RegisteredEventHandler;
                    component.ProcessStopped += this.RegisteredEventHandler;
                }
            }

            lock (this.registeredProcesses)
            {
                this.registeredProcesses.AddRange(
                    from p in components
                    select new ProcessRegistration(p, processType));
            }
        }

        /// <summary>
        /// Deregisters components from this process controller.
        /// </summary>
        /// <param name="processType">The type of process to deregister.</param>
        /// <param name="components">The components to deregister.</param>
        protected void DeregisterComponents(ProcessType processType, params IProcessEngine[] components)
        {
            if (components == null)
            {
                throw new ArgumentNullException("components");
            }

            // Deregister events for the specified process engines.
            foreach (IProcessEngine component in components)
            {
                component.ProcessStarted -= this.RegisteredEventHandler;
                component.ProcessStopped -= this.RegisteredEventHandler;
            }

            var registrations =
                from p in this.registeredProcesses
                where p.ProcessType == processType && components.Contains(p.Process)
                select p;

            // Using ToList() avoids modifying the collection while it is still being enumerated. Otherwise, the evaluation will 
            // happen as the removal is taking place.
            foreach (ProcessRegistration registration in registrations.ToList())
            {
                lock (this.registeredProcesses)
                {
                    this.registeredProcesses.Remove(registration);
                }
            }
        }

        #endregion

        #region Event Processing

        /// <summary>
        /// Processes real-time events in the worker's thread.
        /// </summary>
        /// <typeparam name="TArgs">The type of event arguments to process.</typeparam>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">Event data associated with the event.</param>
        /// <remarks>This takes place in the worker thread and is not UI thread-safe.</remarks>
        protected override void ProcessEvent<TArgs>(object sender, TArgs e)
        {
            this.UpdateInternalState(sender, e);

            if (e is CommandExecutingEventArgs)
            {
                this.OnCommandExecuting(e as CommandExecutingEventArgs);
            }
            else if (e is CommandCompletedEventArgs)
            {
                this.OnCommandExecuted(e as CommandCompletedEventArgs);
            }
            else if (e is CommandFailedEventArgs)
            {
                this.OnCommandFailed(e as CommandFailedEventArgs);
            }

            foreach (ICommandTrigger trigger in this.commandTriggers)
            {
                if (trigger.IsConditionTrue(sender, e))
                {
                    trigger.Command.Execute(null);
                }
            }
        }
        
        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the internal state of this process controller.
        /// </summary>
        /// <typeparam name="TArgs">The type of event arguments affecting the internal state.</typeparam>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">Event data associated with the event.</param>
        private void UpdateInternalState<TArgs>(object sender, TArgs e)
            where TArgs : EventArgs
        {
            if (Object.ReferenceEquals(sender, this))
            {
                // If this process has a state change with error, set the process health to the error state.
                if (e is StateChangedEventArgs<TState>)
                {
                    var eventArgs = e as StateChangedEventArgs<TState>;

                    if (eventArgs.EventError != null)
                    {
                        this.ProcessHealth = UserResultState.ProcessError;
                    }
                }
            }

// ReSharper disable LoopCanBeConvertedToQuery - Performance.
            foreach (ProcessRegistration registration in this.registeredProcesses)
// ReSharper restore LoopCanBeConvertedToQuery
            {
                if (!Object.ReferenceEquals(registration.Process, sender))
                {
                    continue;
                }

                // If the state of this process is nominal, check for failed tasks. 
                if (e is ItemsProducedEventArgs && this.ProcessHealth == UserResultState.Nominal)
                {
                    if (sender is ITaskEngine && (sender as ITaskEngine).FailedResults > 0)
                    {
                        this.ProcessHealth = UserResultState.TaskError;
                    }
                }

                // If the state of the process is not already in process error, check for a failed component.
                if (e is ProcessStoppedEventArgs && this.ProcessHealth != UserResultState.ProcessError)
                {
                    var eventArgs = e as ProcessStoppedEventArgs;

                    if (eventArgs.EventError != null)
                    {
                        this.ProcessHealth = UserResultState.TaskError;
                    }
                }

                break;
            }

            // If the state of the process is not already in process error, check for a failed command.
            if (e is CommandFailedEventArgs && this.ProcessHealth != UserResultState.ProcessError)
            {
                this.ProcessHealth = UserResultState.ProcessError;
            }
        }

        #region Event Methods

        /// <summary>
        /// Triggers the CommandExecuting event.
        /// </summary>
        /// <param name="e">The <see cref="CommandExecutingEventArgs"/> associated with the event.</param>
        private void OnCommandExecuting(CommandExecutingEventArgs e)
        {
            EventHandler<CommandExecutingEventArgs> temp = this.CommandExecuting;

            if (temp != null)
            {
                temp(this, e);
            }
        }

        /// <summary>
        /// Triggers the CommandExecuted event and the CommandFailed event, if applicable.
        /// </summary>
        /// <param name="e">Event data associated with the event.</param>
        private void OnCommandExecuted(CommandCompletedEventArgs e)
        {
            EventHandler<CommandCompletedEventArgs> temp = this.CommandCompleted;

            if (temp != null)
            {
                temp(this, e);
            }
        }

        /// <summary>
        /// Triggers the <see cref="CommandFailed"/> event.
        /// </summary>
        /// <param name="e">The <see cref="CommandFailedEventArgs"/> associated with the event.</param>
        private void OnCommandFailed(CommandFailedEventArgs e)
        {
            EventHandler<CommandFailedEventArgs> temp = this.CommandFailed;

            if (temp != null)
            {
                temp(this, e);
            }
        }

        #endregion

        #endregion
    }
}
