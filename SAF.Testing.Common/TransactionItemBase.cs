// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionItemBase.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The transaction item base.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Testing.Common
{
    using System.Diagnostics.CodeAnalysis;

    using Startitecture.Orm.Common;
    using Startitecture.Orm.Schema;

    /// <summary>
    /// The transaction item base.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class TransactionItemBase : ITransactionContext
    {
        /// <summary>
        /// Gets the provider for the current transaction.
        /// </summary>
        [Ignore]
        public IRepositoryProvider TransactionProvider { get; private set; }

        /// <summary>
        /// Sets the transaction provider for the current object.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider to use for the transaction context.
        /// </param>
        public void SetTransactionProvider(IRepositoryProvider repositoryProvider)
        {
            this.TransactionProvider = repositoryProvider;
        }
    }
}