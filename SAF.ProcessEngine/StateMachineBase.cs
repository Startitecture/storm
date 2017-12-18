// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateMachineBase.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Base class for state machines.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ProcessEngine
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Base class for state machines.
    /// </summary>
    /// <typeparam name="TState">The type that represents the possible states of the state machine.</typeparam>
    public abstract class StateMachineBase<TState> : ProcessEngineBase, IStateMachine<TState>
    {
        #region Fields

        /////// <summary>
        /////// An object that controls access to the state.
        /////// </summary>
        ////private readonly object stateControl = new object();

        /// <summary>
        /// The pre-operation idle state of this machine.
        /// </summary>
        private readonly TState initialState;

        /// <summary>
        /// The post-operation idle state of this machine.
        /// </summary>
        private readonly TState idleState;

        /// <summary>
        /// A collection of <see cref="IStateTrigger&lt;TState&gt;"/>s that trigger a state change if the state 
        /// machine receives an event that matches the trigger.
        /// </summary>
        private readonly List<IStateTrigger<TState>> stateTriggers = new List<IStateTrigger<TState>>();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachineBase&lt;TState&gt;"/> class.
        /// </summary>
        /// <param name="name">The name of the state machine.</param>
        /// <param name="initialState">The initial state of this machine.</param>
        /// <param name="idleState">The idle state of this machine.</param>
        protected StateMachineBase(string name, TState initialState, TState idleState) 
            : base(name)
        {
            this.State = initialState;
            this.initialState = initialState;
            this.idleState = idleState;
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the state is changing.
        /// </summary>
        public event EventHandler<StateChangingEventArgs<TState>> StateChanging;

        /// <summary>
        /// Occurs when the state has changed.
        /// </summary>
        public event EventHandler<StateChangedEventArgs<TState>> StateChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current state of this state machine.
        /// </summary>
        public TState State { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the string representation of this state machine.
        /// </summary>
        /// <returns>The name of the state machine.</returns>
        public override string ToString()
        {
            return this.Name;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Processes real-time events in the worker's thread.
        /// </summary>
        /// <typeparam name="TArgs">The type of arguments to process.</typeparam>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">Event data associated with the event.</param>
        /// <remarks>This takes place in the worker thread and is not UI thread-safe. However, PropertyChanged events 
        /// can safely be triggered.</remarks>
        protected abstract void ProcessEvent<TArgs>(object sender, TArgs e)
            where TArgs : EventArgs;

        #region Trigger Registration

        /// <summary>
        /// Event handler for synchronous events.
        /// </summary>
        /// <typeparam name="TArgs">The type of event to register.</typeparam>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">Event data associated with the event.</param>
        protected void RegisteredEventHandler<TArgs>(object sender, TArgs e)
            where TArgs : EventArgs
        {
            this.ProcessEvent(sender, e);

            // Send state changes.
            foreach (IStateTrigger<TState> trigger in this.stateTriggers)
            {
                if (!trigger.IsConditionTrue(sender, e))
                {
                    continue;
                }

                Exception error = e is IErrorEvent ? (e as IErrorEvent).EventError : null;
                this.ProcessStateChange(new StateChange<TState>(sender, e, trigger.NewState), error);
            }
        }

        /// <summary>
        /// Registers <see cref="IStateTrigger{TState}"/>s with this state machine.
        /// </summary>
        /// <param name="triggers">The list of <see cref="IStateTrigger{TState}"/>s 
        /// to register.</param>
        /// <returns>The triggers that were registered.</returns>
        [DebuggerHidden]
        protected IStateTrigger<TState>[] RegisterStateTriggers(params IStateTrigger<TState>[] triggers)
        {
            lock (this.stateTriggers)
            {
                this.stateTriggers.AddRange(triggers);
            }

            return triggers;
        }

        /// <summary>
        /// Removes all specified triggers.
        /// </summary>
        /// <param name="triggers">The triggers to remove.</param>
        protected void DeregisterStateTriggers(params IStateTrigger<TState>[] triggers)
        {
            if (triggers == null)
            {
                throw new ArgumentNullException("triggers");
            }

            foreach (IStateTrigger<TState> trigger in triggers)
            {
                this.stateTriggers.Remove(trigger);
            }
        }

        /// <summary>
        /// Deregisters all state triggers.
        /// </summary>
        protected void ClearStateTriggers()
        {
            lock (this.stateTriggers)
            {
                this.stateTriggers.Clear();
            }
        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// Processes a state change.
        /// </summary>
        /// <param name="change">The state change to process.</param>
        /// <param name="error">The exception, if any, associated with the state change.</param>
        ////[DebuggerHidden]
        private void ProcessStateChange(StateChange<TState> change, Exception error)
        {
            this.OnStateChanging(new StateChangingEventArgs<TState>(change, error));

            if (this.State.Equals(this.initialState) || this.State.Equals(this.idleState))
            {
                this.StartProcess();
            }

            // In case the pending state change also changed the state.
            this.State = change.NewState;
            
            // This pre-check is a double-edged sword - it is faster than always running the query. However, if the state machine 
            // encounters an error that it can't handle, then it may never "stop".
            if (this.State.Equals(this.idleState))
            {
                this.StopProcessIfStopping(error);
            }

            this.OnStateChanged(new StateChangedEventArgs<TState>(change, error));
        }

        #region Event Methods

        /// <summary>
        /// Triggers the StateChanging event.
        /// </summary>
        /// <param name="e">The <see cref="StateChangingEventArgs&lt;TState&gt;"/> associated with the event.</param>
        [DebuggerHidden]
        private void OnStateChanging(StateChangingEventArgs<TState> e)
        {
            EventHandler<StateChangingEventArgs<TState>> temp = this.StateChanging;

            if (temp != null)
            {
                temp(this, e);
            }
        }

        /// <summary>
        /// Triggers the StateChanged event.
        /// </summary>
        /// <param name="e">The <see cref="StateChangedEventArgs&lt;TState&gt;"/> associated with the event.</param>
        [DebuggerHidden]
        private void OnStateChanged(StateChangedEventArgs<TState> e)
        {
            EventHandler<StateChangedEventArgs<TState>> temp = this.StateChanged;

            if (temp != null)
            {
                temp(this, e);
            }
        }

        #endregion

        #endregion
    }
}