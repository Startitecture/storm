// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionEventRepositoryBase.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.ActionTracking
{
    using System;
    using System.Runtime.Caching;

    using Data;

    using JetBrains.Annotations;

    using SAF.Core;

    /// <summary>
    /// The base class for action event repositories.
    /// </summary>
    /// <typeparam name="TEventItem">
    /// The type of data item that contains events.
    /// </typeparam>
    /// <typeparam name="TErrorItem">
    /// The type of data item that contains application errors.
    /// </typeparam>
    [Obsolete("Create specific action event repositories instead.")]
    [CLSCompliant(false)]
    public abstract class ActionEventRepositoryBase<TEventItem, TErrorItem> : IEventRepository
        where TEventItem : IEventData, ITransactionContext, new()
        where TErrorItem : IErrorData, ITransactionContext, new()
    {
        /// <summary>
        /// The action event cache key.
        /// </summary>
        private const string ActionEventCacheKey = "{0}.{1}";

        /// <summary>
        /// The provider factory.
        /// </summary>
        private readonly IRepositoryProviderFactory providerFactory;

        /// <summary>
        /// The cache lock.
        /// </summary>
        private readonly object cacheLock = new object();

        /// <summary>
        /// The action cache.
        /// </summary>
        private readonly ObjectCache actionCache = MemoryCache.Default;

        /// <summary>
        /// The SQL execution policy.
        /// </summary>
        private readonly ExecutionPolicy executionPolicy;

        /// <summary>
        /// The entity mapper.
        /// </summary>
        private readonly IEntityMapper entityMapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionEventRepositoryBase{TEventItem,TErrorItem}"/> class.
        /// </summary>
        /// <param name="entityMapper">
        /// The entity mapper.
        /// </param>
        /// <param name="providerFactory">
        /// The provider factory.
        /// </param>
        /// <param name="executionPolicy">
        /// The execution policy for the repository.
        /// </param>
        protected ActionEventRepositoryBase(
            IEntityMapper entityMapper,
            IRepositoryProviderFactory providerFactory,
            ExecutionPolicy executionPolicy)
        {
            this.entityMapper = entityMapper;
            this.providerFactory = providerFactory;
            this.executionPolicy = executionPolicy;
        }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        public string Connection { get; set; }

        /// <summary>
        /// Gets the ID for the specified action identifier.
        /// </summary>
        /// <param name="actionIdentifier">
        /// The action identifier.
        /// </param>
        /// <returns>
        /// The action event specified by the identifier, or null if an action event is not found.
        /// </returns>
        public ActionEvent GetById(Guid actionIdentifier)
        {
            var actionEventRow = this.executionPolicy.ExecuteForResult(this.GetActionFromRepository, actionIdentifier);

            if (Evaluate.IsNull(actionEventRow))
            {
                return null;
            }

            return this.entityMapper.Map<ActionEvent>(actionEventRow);
        }

        /// <summary>
        /// Saves an item to the database.
        /// </summary>
        /// <param name="item">
        /// The entity to save.
        /// </param>
        /// <returns>
        /// The saved <see cref="SAF.ActionTracking.ActionEvent"/> instance.
        /// </returns>
        /// <exception cref="ApplicationConfigurationException">
        /// The event could not be mapped to the row.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The insert or update operation was not successful.
        /// </exception>
        public ActionEvent Save(ActionEvent item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            string cacheKey = String.Format(ActionEventCacheKey, typeof(ActionEvent).FullName, item.Request.GlobalIdentifier);

            lock (this.cacheLock)
            {
                this.actionCache.Set(cacheKey, item, DateTimeOffset.Now.AddMinutes(5));
            }

            this.executionPolicy.Execute(this.SaveEvent, item);
            return item;
        }

        /// <summary>
        /// Gets a selection for the specified item.
        /// </summary>
        /// <param name="item">
        /// The item to create a selection for.
        /// </param>
        /// <typeparam name="TDataItem">
        /// The type of item to create a selection for.
        /// </typeparam>
        /// <returns>
        /// The <see cref="SAF.Data.ItemSelection{TItem}"/> for the specified item.
        /// </returns>
        protected abstract ItemSelection<TDataItem> GetSelection<TDataItem>(TDataItem item);

        /// <summary>
        /// Gets an error row from an <see cref="ErrorInfo"/> item.
        /// </summary>
        /// <param name="errorInfo">
        /// The error information.
        /// </param>
        /// <returns>
        /// An error item populated with the values in <paramref name="errorInfo"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorInfo"/> is null.
        /// </exception>
        private static TErrorItem GetErrorRow(ErrorInfo errorInfo)
        {
            if (errorInfo == null)
            {
                throw new ArgumentNullException("errorInfo");
            }

            return new TErrorItem
            {
                ErrorCode = errorInfo.ErrorCode, 
                ErrorData = errorInfo.ErrorData, 
                ErrorMessage = errorInfo.ErrorMessage, 
                ErrorType = errorInfo.ErrorType, 
                FullErrorOutput = errorInfo.FullErrorOutput, 
            };
        }

/*
        /// <summary>
        /// Gets an action event row by the global identifier.
        /// </summary>
        /// <param name="actionIdentifier">
        /// The action identifier.
        /// </param>
        /// <returns>
        /// The event item with the specified identifier, or null if no row exists.
        /// </returns>
        private TEventItem GetRowByGuid(Guid actionIdentifier)
        {
            string cacheKey = String.Format(ActionEventCacheKey, typeof(ActionEvent).FullName, actionIdentifier);

            return this.actionCache.GetOrLazyAddExisting(
                this.cacheLock, 
                cacheKey, 
                actionIdentifier, 
                this.GetActionFromRepository, 
                new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(5) });
        }
*/

        /// <summary>
        /// Gets the action event from the repository.
        /// </summary>
        /// <param name="actionIdentifier">
        /// The action identifier.
        /// </param>
        /// <returns>
        /// The event item.
        /// </returns>
        private TEventItem GetActionFromRepository(Guid actionIdentifier)
        {
            var candidate = new TEventItem { GlobalIdentifier = actionIdentifier };

            using (var repositoryProvider = this.providerFactory.Create())
            {
                repositoryProvider.EnableCaching = false;

                var itemSelection = this.GetSelection(candidate);
                return repositoryProvider.GetFirstOrDefault(itemSelection);
            }
        }

        /// <summary>
        /// Saves the event to the repository.
        /// </summary>
        /// <param name="actionEvent">
        /// The action event to save.
        /// </param>
        /// <exception cref="ApplicationConfigurationException">
        /// The event could not be mapped to the row.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The update operation was not successful.
        /// </exception>
        private void SaveEvent(ActionEvent actionEvent)
        {
            if (actionEvent == null)
            {
                throw new ArgumentNullException("actionEvent");
            }

            var actionEventRow = this.entityMapper.Map<TEventItem>(actionEvent);
            var errorRow = GetErrorRow(actionEvent.ErrorInfo);

            using (var repositoryProvider = this.providerFactory.Create())
            {
                repositoryProvider.EnableCaching = false;

                // For errors, use a transaction.
                if (actionEvent.ErrorInfo != ErrorInfo.Empty)
                {
                    repositoryProvider.StartTransaction();
                }

                var savedRow = this.Save<TEventItem>(repositoryProvider, actionEventRow);

                // Add the error if it exists.
                if (actionEvent.ErrorInfo != ErrorInfo.Empty)
                {
                    errorRow.ApplicationErrorId = savedRow.ActionEventId;
                    this.Save(repositoryProvider, errorRow);
                    repositoryProvider.CompleteTransaction();
                }
            }
        }

        /// <summary>
        /// Saves an item in the current repository by using provider's mapper.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider.
        /// </param>
        /// <param name="item">
        /// The item to save.
        /// </param>
        /// <typeparam name="TDataItem">
        /// The type of item to save.
        /// </typeparam>
        /// <returns>
        /// The saved item.
        /// </returns>
        private TDataItem Save<TDataItem>([NotNull]IRepositoryProvider repositoryProvider, TDataItem item)
            where TDataItem : ITransactionContext
        {
            if (repositoryProvider == null)
            {
                throw new ArgumentNullException("repositoryProvider");
            }

            if (Evaluate.IsNull(item))
            {
                throw new ArgumentNullException("item");
            }

            var itemSelection = this.GetSelection(item);
            return repositoryProvider.Save(item, itemSelection);
        }
    }
}
