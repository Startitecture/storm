namespace SAF.Data.Persistence
{
    using SAF.ProcessEngine;

    /// <summary>
    /// Provides an interface to a data store.
    /// </summary>
    public interface IDataProxy : IProcessEngine, IProducer
    {
        /// <summary>
        /// Gets or sets the maximum length of the data proxy's item queue.
        /// </summary>
        long MaxQueueLength { get; set; }
    }

    /// <summary>
    /// Provides an interface to a data store.
    /// </summary>
    /// <typeparam name="TItem">The type of item that the proxy produces.</typeparam>
    public interface IDataProxy<TItem> : IDataProxy
    {
        /// <summary>
        /// Gets a queue consumer for items the data store has loaded since the last retrieval.
        /// </summary>
        QueueConsumer<TItem> Items { get; }
    }
}
