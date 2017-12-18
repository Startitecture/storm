namespace SAF.Data.Persistence
{
    using System.ComponentModel;

    using SAF.ProcessEngine;

    /// <summary>
    /// Monitors an <see cref="IPersistenceEngine"/> for property changes and notifies <see cref="INotifyPropertyChanged"/> subscribers.
    /// </summary>
    public class PersistenceMonitor : TaskMonitor
    {
        /// <summary>
        /// The <see cref="IPersistenceEngine"/> to monitor.
        /// </summary>
        private readonly IPersistenceEngine engine;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistenceMonitor"/> class.
        /// </summary>
        /// <param name="engine">The <see cref="IPersistenceEngine"/> to monitor.</param>
        public PersistenceMonitor(IPersistenceEngine engine)
            : base(engine)
        {
            this.engine = engine;
        }

        /// <summary>
        /// Gets the number of added items this updater has processed.
        /// </summary>
        public long AddedItems { get; private set; }

        /// <summary>
        /// Gets the number of modified items this updater has processed.
        /// </summary>
        public long ModifiedItems { get; private set; }

        /// <summary>
        /// Gets the number of unchanged items this updater has processed.
        /// </summary>
        public long UnchangedItems { get; private set; }

        /// <summary>
        /// Gets the number of removed items this updater has processed.
        /// </summary>
        public long RemovedItems { get; private set; }

        /// <summary>
        /// Gets the number of rolled back items this updater has processed.
        /// </summary>
        public long RolledBackItems { get; private set; }

        /// <summary>
        /// Detects changed properties for task engines.
        /// </summary>
        protected override void DetectChangedProperties()
        {
            base.DetectChangedProperties();

            if (this.engine.AddedItems != this.AddedItems)
            {
                this.AddedItems = this.engine.AddedItems;
                this.AddChangedProperty(Constants.AddedItemsProperty);
            }

            if (this.engine.ModifiedItems != this.ModifiedItems)
            {
                this.ModifiedItems = this.engine.ModifiedItems;
                this.AddChangedProperty(Constants.ModifiedItemsProperty);
            }

            if (this.engine.RemovedItems != this.RemovedItems)
            {
                this.RemovedItems = this.engine.RemovedItems;
                this.AddChangedProperty(Constants.RemovedItemsProperty);
            }

            if (this.engine.UnchangedItems != this.UnchangedItems)
            {
                this.UnchangedItems = this.engine.UnchangedItems;
                this.AddChangedProperty(Constants.UnchangedItemsProperty);
            }

            if (this.engine.RolledBackItems != this.RolledBackItems)
            {
                this.RolledBackItems = this.engine.RolledBackItems;
                this.AddChangedProperty(Constants.RolledBackItemsProperty);
            }
        }
    }
}
