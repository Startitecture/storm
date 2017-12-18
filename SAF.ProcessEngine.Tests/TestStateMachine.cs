// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestStateMachine.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The test state machine.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ProcessEngine.Tests
{
    using System;

    /// <summary>
    /// The test state machine.
    /// </summary>
    public class TestStateMachine : StateMachineBase<ProcessState>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestStateMachine"/> class.
        /// </summary>
        public TestStateMachine()
            : base("Test State Machine", ProcessState.NotStarted, ProcessState.Stopped)
        {
            this.TestEvent += this.RegisteredEventHandler;
        }

        #endregion

        #region Public Events

        /// <summary>
        /// The event received.
        /// </summary>
        public event EventHandler<EventArgs> EventReceived;

        /// <summary>
        /// The test event.
        /// </summary>
        public event EventHandler<TestEventArgs> TestEvent;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The register process starter.
        /// </summary>
        /// <param name="engine">
        /// The engine.
        /// </param>
        public void RegisterProcessStarter(IProcessEngine engine)
        {
            this.RegisterStateTriggers(new ProcessStartStateTrigger<ProcessState>(ProcessState.Started, engine));
            engine.ProcessStarted += this.RegisteredEventHandler;
        }

        /// <summary>
        /// The register process stopper.
        /// </summary>
        /// <param name="engine">
        /// The engine.
        /// </param>
        public void RegisterProcessStopper(IProcessEngine engine)
        {
            this.RegisterStateTriggers(new ProcessStopStateTrigger<ProcessState>(ProcessState.Stopped, engine));
            engine.ProcessStopped += this.RegisteredEventHandler;
        }

        /// <summary>
        /// The trigger generic event.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        public void TriggerGenericEvent(TestEventArgs e)
        {
            EventHandler<TestEventArgs> temp = this.TestEvent;

            if (temp != null)
            {
                temp(this, e);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Processes real-time events in the worker's thread.
        /// </summary>
        /// <typeparam name="TArgs">The type of arguments to process.</typeparam>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">Event data associated with the event.</param>
        /// <remarks>This takes place in the worker thread and is not UI thread-safe. However, PropertyChanged events 
        /// can safely be triggered.</remarks>
        protected override void ProcessEvent<TArgs>(object sender, TArgs e)
        {
            this.OnEventReceived(e);
        }

        /// <summary>
        /// The on event received.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnEventReceived(EventArgs e)
        {
            EventHandler<EventArgs> temp = this.EventReceived;

            if (temp != null)
            {
                temp(this, e);
            }
        }

        #endregion
    }
}