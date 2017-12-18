namespace SAF.Data.Integration
{
    using SAF.Data.Persistence;

    using Constants = SAF.ProcessEngine.Constants;

    /// <summary>
    /// Monitors an <see cref="IIntegrationController"/> for property changes and notifies 
    /// <see cref="System.ComponentModel.INotifyPropertyChanged"/> subscribers.
    /// </summary>
    public class IntegrationMonitor : PersistenceMonitor
    {
        /// <summary>
        /// The <see cref="IIntegrationController"/> to monitor.
        /// </summary>
        private readonly IIntegrationController controller;

        /// <summary>
        /// Initializes a new instance of the <see cref="IntegrationMonitor"/> class.
        /// </summary>
        /// <param name="controller">The <see cref="IIntegrationController"/> to monitor.</param>
        public IntegrationMonitor(IIntegrationController controller)
            : base(controller)
        {
            this.controller = controller;
        }

        /// <summary>
        /// Gets the state of the process.
        /// </summary>
        public IntegrationState State { get; private set; }

        /// <summary>
        /// Detects changed properties for process controllers.
        /// </summary>
        protected override void DetectChangedProperties()
        {
            base.DetectChangedProperties();

            if (!this.controller.State.Equals(this.State))
            {
                this.State = this.controller.State;
                this.AddChangedProperty(Constants.StateProperty);
            }
        }
    }
}
