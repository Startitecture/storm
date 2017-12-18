namespace SAF.Data.Integration
{
    using SAF.Data;
    using SAF.Data.Persistence;
    using SAF.ProcessEngine;

    /// <summary>
    /// Contains a set of rules for a data integration process.
    /// </summary>
    /// <typeparam name="TItem">The type of the items in the source.</typeparam>
    /// <typeparam name="TModel">The type of persistent model converted from the items.</typeparam>
    public class IntegrationPolicy<TItem, TModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IntegrationPolicy&lt;TItem, TModel&gt;"/> class.
        /// </summary>
        /// <param name="converter">The converter that will convert items into models.</param>
        /// <param name="adapter">The adapter that will persist the models.</param>
        /// <param name="preparationCommand">The command to use for target preparation.</param>
        /// <param name="finalizationCommand">The command to use for target finalization.</param>
        public IntegrationPolicy(
            IDataConverter<TItem, TModel> converter,
            IPersistenceAdapter<TModel> adapter,
            IExecutable preparationCommand,
            IExecutable finalizationCommand)
        {
            this.Converter = converter;
            this.Adapter = adapter;
            this.PreparationCommand = preparationCommand;
            this.FinalizationCommand = finalizationCommand;
        }

        /// <summary>
        /// Gets the converter that will convert imported items into models.
        /// </summary>
        public IDataConverter<TItem, TModel> Converter { get; private set; }

        /// <summary>
        /// Gets the persistence adapter that will persist the models provided by the converter.
        /// </summary>
        public IPersistenceAdapter<TModel> Adapter { get; private set; }

        /// <summary>
        /// Gets the command that will prepare the target prior to the update.
        /// </summary>
        public IExecutable PreparationCommand { get; private set; }

        /// <summary>
        /// Gets the command that will finalize the target once the update is complete.
        /// </summary>
        public IExecutable FinalizationCommand { get; private set; }
    }
}
