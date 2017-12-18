// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestController.cs" company="TractManager">
//   Copyright 2013 TractManager. All rights reserved.
// </copyright>
// <summary>
//   The test controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ProcessEngine.Tests
{
    using System;

    /// <summary>
    /// The test controller.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class TestController<T> : ProcessControllerBase<ProcessState>
    {
        #region Fields

        /// <summary>
        /// The command.
        /// </summary>
        private readonly TestCommand command = new TestCommand();

        /// <summary>
        /// The engine.
        /// </summary>
        private readonly TestEngine<T> engine = new TestEngine<T>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestController{T}"/> class.
        /// </summary>
        public TestController()
            : base("Test", ProcessState.NotStarted, ProcessState.Stopped)
        {
            this.RegisterComponents(ProcessType.Producer, this.engine);
            this.RegisterComponents(ProcessType.Consumer, this.engine);
            this.RegisterStateTriggers(new ProcessStartStateTrigger<ProcessState>(ProcessState.Started, this.engine));
            this.RegisterStateTriggers(new ProcessStopStateTrigger<ProcessState>(ProcessState.Stopped, this.engine));
            this.engine.ItemsProduced += this.engine_QueueReady;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The execute command.
        /// </summary>
        /// <param name="time">
        /// The time.
        /// </param>
        public void ExecuteCommand(TimeSpan time)
        {
            this.command.LoadDirective(time);
            this.ExecuteCommand(time);
        }

        /// <summary>
        /// The start engine.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        public void StartEngine(params T[] items)
        {
            foreach (T item in items)
            {
                this.engine.Add(item);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The engine_ queue ready.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void engine_QueueReady(object sender, ItemsProducedEventArgs e)
        {
            while (this.engine.TaskResultConsumer.ConsumeNext())
            {
                TestResult<T> item = this.engine.TaskResultConsumer.Current;
            }
        }

        #endregion
    }
}