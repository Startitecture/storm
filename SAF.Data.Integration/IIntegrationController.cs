namespace SAF.Data.Integration
{
    using System;

    using SAF.Data.Persistence;
    using SAF.ProcessEngine;

    /// <summary>
    /// Provides an interface to a data integration controller.
    /// </summary>
    public interface IIntegrationController : IProcessController<IntegrationState>, IPersistenceEngine
    {
        /// <summary>
        /// Gets an interface to the item converter.
        /// </summary>
        ITaskEngine Converter { get; }

        /// <summary>
        /// Gets an interface to the model updater.
        /// </summary>
        ITaskEngine Updater { get; }

        /// <summary>
        /// Gets or sets the maximum length of the update queue.
        /// </summary>
        long MaxQueueLength { get; set; }
    }

    /// <summary>
    /// Provides an interface to a data integration controller.
    /// </summary>
    /// <typeparam name="TItem">The source item type.</typeparam>
    /// <typeparam name="TEntity">The taret item type.</typeparam>
    public interface IIntegrationController<TItem, TEntity> : IIntegrationController
    {
        /// <summary>
        /// Occurs when an item cannot be converted.
        /// </summary>
        event EventHandler<FailedItemEventArgs<TItem>> ConversionFailed;

        /// <summary>
        /// Occurs when an entity cannot be persisted.
        /// </summary>
        event EventHandler<FailedItemEventArgs<TEntity>> PersistenceFailed;
    }
}
