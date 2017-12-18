namespace SAF.UserInterface
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    using SAF.Data.Integration;
    using SAF.ProcessEngine;

    /// <summary>
    /// Extends the <see cref="IIntegrationController"/> interface to include an <see cref="ICommand"/> to start
    /// the data integration process and a monitor to monitor the process.
    /// </summary>
    public interface IDataIntegrationViewModel
    {
        /// <summary>
        /// Occurs when there is a state change in the view model.
        /// </summary>
        event EventHandler<StateChangedEventArgs<IntegrationState>> StateChanged;

        /// <summary>
        /// Occurs when a command completes.
        /// </summary>
        event EventHandler<CommandCompletedEventArgs> CommandCompleted;

        /// <summary>
        /// Occurs when a command fails.
        /// </summary>
        event EventHandler<CommandFailedEventArgs> CommandFailed;

        /// <summary>
        /// Gets the name of the integration controller.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets an <see cref="ICommand"/> that starts the integration process.
        /// </summary>
        ICommand IntegrationCommand { get; }

        /// <summary>
        /// Gets a <see cref="IntegrationMonitor"/> to monitor the data integration process.
        /// </summary>
        IntegrationMonitor IntegrationMonitor { get; }

        /// <summary>
        /// Gets a <see cref="TaskMonitor"/> to monitor the conversion process.
        /// </summary>
        TaskMonitor ConverterMonitor { get; }

        /// <summary>
        /// Gets a <see cref="TaskMonitor"/> to monitor the update process.
        /// </summary>
        TaskMonitor UpdateMonitor { get; }

        /// <summary>
        /// Gets a collection of items that failed to convert.
        /// </summary>
        ObservableCollection<FailedItem> FailedItems { get; }

        /// <summary>
        /// Gets a collection of models that failed to update.
        /// </summary>
        ObservableCollection<FailedItem> FailedModels { get; }
    }
}
