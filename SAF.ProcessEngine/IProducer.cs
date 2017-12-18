namespace SAF.ProcessEngine
{
    using System;

    /// <summary>
    /// Provides an interface to an item producer.
    /// </summary>
    public interface IProducer
    {
        /// <summary>
        /// Occurs when items are ready to retrieve.
        /// </summary>
        event EventHandler<ItemsProducedEventArgs> ItemsProduced;
    }
}
