// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITransactionContext.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface to items that have a transaction context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data
{
    /// <summary>
    /// Provides an interface to items that have a transaction context.
    /// </summary>
    public interface ITransactionContext
    {
        /// <summary>
        /// Gets the provider for the current transaction.
        /// </summary>
        IRepositoryProvider TransactionProvider { get; }

        /// <summary>
        /// Sets the transaction provider for the current object.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider to use for the transaction context.
        /// </param>
        void SetTransactionProvider(IRepositoryProvider repositoryProvider);
    }
}