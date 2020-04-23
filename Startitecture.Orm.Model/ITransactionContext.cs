// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITransactionContext.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface to items that have a transaction context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
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