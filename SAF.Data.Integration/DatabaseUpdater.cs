// -----------------------------------------------------------------------
// <copyright file="TargetUpdater.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SAF.Data.Integration
{
    using System;
    using System.IO;

    using SAF.Core;
    using SAF.Data;
    using SAF.Data.Persistence;
    using SAF.ProcessEngine;

    /// <summary>
    /// Registers components with the database integration manager and starts the integration process.
    /// </summary>
    /// <typeparam name="TItem">The type of item that is the source of the update.</typeparam>
    /// <typeparam name="TEntity">The type of entity that will be updated.</typeparam>
    public class DatabaseUpdater<TItem, TEntity> : IDisposable
    {
        /// <summary>
        /// The integration manager that processes the integration.
        /// </summary>
        private readonly DatabaseIntegrationManager<TItem, TEntity> manager;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseUpdater&lt;TItem, TEntity&gt;"/> class.
        /// </summary>
        /// <param name="manager">The database integration manager for the update process.</param>
        public DatabaseUpdater(DatabaseIntegrationManager<TItem, TEntity> manager)
        {
            this.manager = manager;
        }

        /// <summary>
        /// Gets an interface to the process being used to update the target database for this updater's set of data.
        /// </summary>
        public IProcessEngine Process
        {
            get { return this.manager.Process; }
        }

        /// <summary>
        /// Gets an interface to the controller being used to update the target database for this updater's set of data.
        /// </summary>
        public IIntegrationController<TItem, TEntity> Controller
        {
            get { return this.manager.Controller; }
        }

        /// <summary>
        /// Initializes the integration manager with the database adapter, data converter and data proxy to use for the integration.
        /// </summary>
        /// <typeparam name="TAdapter">The type of adapter that connects to the target database.</typeparam>
        /// <param name="targetConnection">The connection string to the target database.</param>
        /// <param name="preparationCommand">The command to run to prepare the integration.</param>
        /// <param name="finalizationCommand">The command to run to finalize the integration.</param>
        /// <param name="converter">A data converter that converts the item type into the entity type.</param>
        /// <param name="proxy">The data proxy that provides access to the source items.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "It is not possible to provide a type parameter because the TAdapter must be created within the method.")]
        public void InitializeManager<TAdapter>(
            string targetConnection, 
            AggregateCommand preparationCommand, 
            AggregateCommand finalizationCommand, 
            IDataConverter<TItem, TEntity> converter, 
            IDataProxy<TItem> proxy)
            where TAdapter : IDatabaseAdapter, IPersistenceAdapter<TEntity>, new()
        {
            // TODO: Fix with dependency injection?
            this.manager.UnregisterComponents();
            this.manager.RegisterComponents<TAdapter>(
                targetConnection,
                preparationCommand,
                finalizationCommand,
                converter,
                proxy);
        }

        /// <summary>
        /// Starts the extraction and catches common exceptions related to the extraction process.
        /// </summary>
        /// <typeparam name="TProxy">The type of proxy that will perform the extraction.</typeparam>
        /// <typeparam name="TSource">The type of source containing the items to extract.</typeparam>
        /// <param name="proxy">A proxy that will perform the extraction.</param>
        /// <param name="dataSource">A data source containing the items to extract.</param>
        public void StartUpdate<TProxy, TSource>(TProxy proxy, TSource dataSource)
            where TProxy : IDataExtractor<TSource>, IDataProxy<TItem>
            where TSource : IDataSource
        {
            if (proxy == null)
            {
                throw new ArgumentNullException("proxy");
            }

            if (dataSource == null)
            {
                throw new ArgumentNullException("dataSource");
            }

            try
            {
                proxy.BeginExtraction(dataSource);
            }
            catch (System.Data.DataException ex)
            {
                throw new OperationException(
                    dataSource,
                    String.Format("Accessing '{0}' failed: {1}", dataSource, ex.Message),
                    ex);
            }
            catch (System.Data.Common.DbException ex)
            {
                throw new OperationException(
                    dataSource,
                    String.Format("Failed to access a database for '{0}': {1}", dataSource, ex.Message),
                    ex);
            }
            catch (System.Security.SecurityException ex)
            {
                throw new AccessException(
                    dataSource,
                    String.Format("You do not have the necessary permissions to load data from '{0}'.", dataSource),
                    ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new AccessException(
                    dataSource,
                    String.Format("You do not have access to '{0}'.", dataSource),
                    ex);
            }
            catch (IOException ex)
            {
                throw new OperationException(
                    dataSource,
                    String.Format("There was a problem reading from '{0}'.", dataSource),
                    ex);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new BusinessException(
                    dataSource,
                    String.Format("'{0}' does not appear to be a valid '{1}': {2}", dataSource, typeof(TSource).Name, ex.Message),
                    ex);
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                throw new BusinessException(
                    dataSource,
                    String.Format("Extracting data from '{0}' failed: '{1}'", dataSource, ex.Message),
                    ex);
            }
        }

        /// <summary>
        /// Disposes of managed resources owned by this instance.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of managed resources owned by this instance.
        /// </summary>
        /// <param name="disposing"><c>true</c> if the instance is being explicitly disposed; otherwise, <c>false</c>.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.manager != null)
                {
                    this.manager.Dispose();
                }
            }
        }
    }
}