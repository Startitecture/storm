// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionContainer.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides a event-aware implementation of the <see cref="ITransactionContext" /> interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data
{
    using System;

    using SAF.Core;
    using SAF.StringResources;

    /// <summary>
    /// Provides a event-aware implementation of the <see cref="ITransactionContext"/> interface.
    /// </summary>
    /// <remarks>
    /// This class can either be inherited from, or used as an implementation for a class implementing 
    /// <see cref="ITransactionContext"/>.
    /// </remarks>
    public class TransactionContainer : ITransactionContext
    {
        /// <summary>
        /// Gets the provider for the current transaction.
        /// </summary>
        public IRepositoryProvider TransactionProvider { get; private set; }

        /// <summary>
        /// Sets the transaction provider for the current object.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider to use for the transaction context.
        /// </param>
        public void SetTransactionProvider(IRepositoryProvider repositoryProvider)
        {
            if (repositoryProvider == null)
            {
                throw new ArgumentNullException("repositoryProvider");
            }

            if (repositoryProvider.IsDisposed)
            {
                throw new OperationException(repositoryProvider, ErrorMessages.RepositoryProviderIsDisposed);
            }

            repositoryProvider.Disposed += this.HandleProviderDisposed;
            this.TransactionProvider = repositoryProvider;
        }

        /// <summary>
        /// Handles the <see cref="IRepositoryProvider.Disposed"/> event.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="eventArgs">
        /// The event data associated with the event.
        /// </param>
        private void HandleProviderDisposed(object sender, EventArgs eventArgs)
        {
            this.TransactionProvider = null;
            ((IRepositoryProvider)sender).Disposed -= this.HandleProviderDisposed;
        }
    }
}
