namespace SAF.ProcessEngine
{
    /// <summary>
    /// Monitors an <see cref="IProcessController&lt;TState&gt;"/> for property changes and notifies subscribers.
    /// </summary>
    /// <typeparam name="TState">The type that represents the possible states of the process.</typeparam>
    public class ProcessMonitor<TState> : TaskMonitor
    {
        /// <summary>
        /// The process to monitor.
        /// </summary>
        private readonly IProcessController<TState> process;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessMonitor&lt;TState&gt;"/> class.
        /// </summary>
        /// <param name="process">The <see cref="IProcessController&lt;TState&gt;"/> to monitor.</param>
        public ProcessMonitor(IProcessController<TState> process)
            : base(process)
        {
            this.process = process;
        }

        /// <summary>
        /// Gets the state of the process.
        /// </summary>
        public TState State { get; private set; }

        /// <summary>
        /// Detects changed properties for process controllers.
        /// </summary>
        protected override void DetectChangedProperties()
        {
            base.DetectChangedProperties();

            if (!this.process.State.Equals(this.State))
            {
                this.State = this.process.State;
                this.AddChangedProperty(Constants.StateProperty);
            }
        }
    }
}
