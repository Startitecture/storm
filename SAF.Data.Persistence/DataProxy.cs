namespace SAF.Data.Persistence
{
    using System;
    using System.Diagnostics;

    using SAF.ProcessEngine;

    /// <summary>
    /// A proxy for a data source.
    /// </summary>
    /// <typeparam name="TSource">The type of source that the proxy gathers data for.</typeparam>
    /// <typeparam name="TItem">The type of item that the proxy produces.</typeparam>
    public abstract class DataProxy<TSource, TItem> : ProcessEngineBase, IDataExtractor<TSource>, IDataProxy<TItem>
        where TSource : IDataSource
    {
        #region Fields

        /// <summary>
        /// The producer that produces import results to consumers.
        /// </summary>
        private ItemProducer<TItem> producer = new ItemProducer<TItem>();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DataProxy&lt;TSource, TItem&gt;"/> class.
        /// </summary>
        protected DataProxy()
            : base(String.Format("{0} Proxy", typeof(TItem).Name))
        {
            this.producer.ItemsProduced += delegate(object o, ItemsProducedEventArgs e)
            {
                this.OnItemsProduced(e);
            };
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs before the data source starts loading into the data proxy.
        /// </summary>
        public event EventHandler<DataExtractionStartingEventArgs> DataExtractionStarting;

        /// <summary>
        /// Occurs when items have been retrieved from the data source.
        /// </summary>
        public event EventHandler<ItemsProducedEventArgs> ItemsProduced;

        /// <summary>
        /// Occurs when an item cannot be retrieved.
        /// </summary>
        public event EventHandler<FailedItemEventArgs<TSource>> RetrievalFailed;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the maximum length of the data store's item queue.
        /// </summary>
        [DebuggerHidden]
        public long MaxQueueLength 
        {
            get { return this.producer.MaxQueueLength; }
            set { this.producer.MaxQueueLength = value; }
        }

        /// <summary>
        /// Gets the items from the data store that have been imported since the last retrieval.
        /// </summary>
        [DebuggerHidden]
        public QueueConsumer<TItem> Items
        {
            get { return this.producer.ItemQueueConsumer; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Extracts data from the specified source. The <see cref="M:ItemsProduced"/> event indicates when items are available in the
        /// <see cref="M:Items"/> property.
        /// </summary>
        /// <param name="dataSource">The data source to load into the data proxy.</param>
        public void BeginExtraction(TSource dataSource)
        {
            if (dataSource == null)
            {
                throw new ArgumentNullException("dataSource");
            }

            this.StartProcess();

            Exception error = null;

            try
            {
                this.OnDataExtractionStarting(new DataExtractionStartingEventArgs(dataSource));
                this.EmitItems(dataSource);
            }
            catch (Exception ex)
            {
                error = ex;
                throw;
            }
            finally
            {
                if (error != null)
                {
                    Trace.TraceError("Unable to retrieve {0} items from {1}: {2}", typeof(TItem).Name, typeof(TSource).Name, error);
                }

                this.StopProcessIfStopping(error);
            }
        }

        /// <summary>
        /// Loads data from a data store into the dataset.
        /// </summary>
        /// <param name="dataSource">The source of the data store</param>
        [DebuggerHidden]
        protected abstract void EmitItems(TSource dataSource);

        /// <summary>
        /// Registers a data source with the application.
        /// </summary>
        /// <typeparam name="TIdentity">The type of identity key to provide to the data source.</typeparam>
        /// <param name="dataSource">The data source to register.</param>
        /// <param name="registrar">The registrar that will register the data source.</param>
        /// <returns>A registration for the specified data source.</returns>
        protected DataSourceRegistration<TIdentity> RegisterDataSource<TIdentity>(
            IDataSource dataSource, 
            IDataSourceRegistrar<TIdentity> registrar)
        {
            if (registrar == null)
            {
                throw new ArgumentNullException("registrar");
            }

            return registrar.Register(dataSource);
        }

        /// <summary>
        /// Emits an item from the data store.
        /// </summary>
        /// <param name="item">The item to emit.</param>
        protected void EmitItem(TItem item)
        {
            if (this.Canceled)
            {
                throw new InvalidOperationException("The data load has been canceled.");
            }

            this.producer.ProduceItem(item);
        }

        #region Event Methods

        /// <summary>
        /// Triggers the <see cref="RetrievalFailed"/> event.
        /// </summary>
        /// <param name="e"><see cref="FailedItemEventArgs&lt;TSource&gt;"/> associated with the event.</param>
        protected void OnRetrievalFailed(FailedItemEventArgs<TSource> e)
        {
            EventHandler<FailedItemEventArgs<TSource>> temp = this.RetrievalFailed;

            if (temp != null)
            {
                temp(this, e);
            }
        }

        /// <summary>
        /// Triggers the DataSourceChanged event.
        /// </summary>
        /// <param name="e">Event data associated with the event.</param>
        [System.Diagnostics.DebuggerHidden]
        private void OnDataExtractionStarting(DataExtractionStartingEventArgs e)
        {
            EventHandler<DataExtractionStartingEventArgs> temp = this.DataExtractionStarting;

            if (temp != null)
            {
                temp(this, e);
            }
        }

        /// <summary>
        /// Triggers the <see cref="ItemsProduced"/> event.
        /// </summary>
        /// <param name="e"><see cref="ItemsProducedEventArgs"/> event data associated with the event.</param>
        [System.Diagnostics.DebuggerHidden]
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
