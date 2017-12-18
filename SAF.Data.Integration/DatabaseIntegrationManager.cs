// -----------------------------------------------------------------------
// <copyright file="DatabaseIntegrationManager.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SAF.Data.Integration
{
    using System;

    using SAF.Data;
    using SAF.Data.Persistence;
    using SAF.ProcessEngine;

    /// <summary>
    /// Manages a data integration for a database target.
    /// </summary>
    /// <typeparam name="TItem">The type of item that is extracted.</typeparam>
    /// <typeparam name="TEntity">The type of item that is loaded into the target system.</typeparam>
    public class DatabaseIntegrationManager<TItem, TEntity> : IDisposable
    {
        /// <summary>
        /// The controller that manages the integration.
        /// </summary>
        private readonly IntegrationController<TItem, TEntity> integrationController;

        /// <summary>
        /// The adapter to use for transactions.
        /// </summary>
        private IDatabaseAdapter databaseAdapter;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseIntegrationManager&lt;TItem, TEntity&gt;"/> class.
        /// </summary>
        /// <param name="name">The name of the integration manager.</param>
        public DatabaseIntegrationManager(string name)
        {
            this.integrationController = new IntegrationController<TItem, TEntity>(name);
        }

        /// <summary>
        /// Gets an interface to the process performing the integration.
        /// </summary>
        public IProcessEngine Process
        {
            get { return this.integrationController; }
        }

        /// <summary>
        /// Gets the integration controller that performs the integration.
        /// </summary>
        public IIntegrationController<TItem, TEntity> Controller
        {
            get { return this.integrationController; }
        }

        /// <summary>
        /// Gets the database adapter used to update the target database.
        /// </summary>
        protected IDatabaseAdapter DatabaseAdapter
        {
            get { return this.databaseAdapter; }
        }

        /// <summary>
        /// Registers the target adapter for this updater. Necessary because there is no way to convert the TransactionAdapter into 
        /// a persistence adapater of an unknown type.
        /// </summary>
        /// <typeparam name="TAdapter">The type of adapter to register.</typeparam>
        /// <param name="connectionString">The connection string for the adapter to use.</param>
        /// <param name="preparationCommand">The command to run to prepare the integration.</param>
        /// <param name="finalizationCommand">The command to run to finalize the integration.</param>
        /// <param name="converter">The converter that will perform the conversion between items and entities.</param>
        /// <param name="proxy">The data proxy that provides data from the data sources.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design", 
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "Required in order to instantiate the generic adapter without a third generic type for the class.")]
        public void RegisterComponents<TAdapter>(
            string connectionString,
            AggregateCommand preparationCommand,
            AggregateCommand finalizationCommand,
            IDataConverter<TItem, TEntity> converter,
            IDataProxy<TItem> proxy)
            where TAdapter : IDatabaseAdapter, IPersistenceAdapter<TEntity>, new()
        {
            TAdapter adapter = new TAdapter();
            this.databaseAdapter = adapter;
            this.databaseAdapter.Initialize(this.integrationController.Name, connectionString);

            this.integrationController.RegisterIntegrationComponents(converter, adapter, proxy);
            this.integrationController.ProcessStarted += this.OpenDatabaseConnection;

            if (proxy is IDataExtractor)
            {
                (proxy as IDataExtractor).DataExtractionStarting += delegate(object o1, DataExtractionStartingEventArgs e1)
                {
                    if (preparationCommand != AggregateCommand.Empty && preparationCommand.CanExecute(e1.DataSource))
                    {
                        preparationCommand.Execute(e1.DataSource);
                    }
                };
            }

            this.integrationController.ProcessStopped += delegate(object o, ProcessStoppedEventArgs e)
            {
                if (e.EventError == null && finalizationCommand.CanExecute(null))
                {
                    finalizationCommand.Execute(null);
                }
            };

            this.integrationController.ProcessStopped += this.CloseDatabaseConnection;
        }

        /// <summary>
        /// Unregisters all the components currently registered to this updater.
        /// </summary>
        public void UnregisterComponents()
        {
            this.databaseAdapter = null;
            this.integrationController.DeregisterIntegrationComponents();

            this.integrationController.ProcessStarted -= this.OpenDatabaseConnection;
            this.integrationController.ProcessStopped -= this.CloseDatabaseConnection;
        }

        /// <summary>
        /// Disposes of unmanaged resources in this instance.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of unmanaged resources.
        /// </summary>
        /// <param name="disposing">A value indicating whether the Dispose() method is being called (true) or the 
        /// destructor (false).</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.databaseAdapter is IDisposable)
                {
                    (this.databaseAdapter as IDisposable).Dispose();
                }
            }
        }

        /// <summary>
        /// Closes the database connection for the specified adapter.
        /// </summary>
        /// <param name="adapter">The adapter with the connection to close.</param>
        private static void OpenDatabaseConnection(IDatabaseAdapter adapter)
        {
            adapter.CommandAdapter.Open();
        }

        /// <summary>
        /// Closes the database connection for the specified adapter.
        /// </summary>
        /// <param name="adapter">The adapter with the connection to close.</param>
        private static void CloseDatabaseConnection(IDatabaseAdapter adapter)
        {
            adapter.CommandAdapter.Close();
        }

        #region Event Handlers

        /// <summary>
        /// Opens the database connection when the process starts.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">Event data associated with the event.</param>
        private void OpenDatabaseConnection(object sender, ProcessStartedEventArgs e)
        {
            OpenDatabaseConnection(this.databaseAdapter);
        }

        /// <summary>
        /// Closes the database connection when the process stops.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">Event data associated with the event.</param>
        private void CloseDatabaseConnection(object sender, ProcessStoppedEventArgs e)
        {
            CloseDatabaseConnection(this.databaseAdapter);
        }

        #endregion
    }
}
