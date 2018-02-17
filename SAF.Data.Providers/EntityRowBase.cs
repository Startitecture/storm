﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityRowBase.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers
{
    using System;

    using Startitecture.Orm.Common;
    using Startitecture.Orm.Schema;

    /// <summary>
    /// The entity row base.
    /// </summary>
    public class EntityRowBase : ITransactionContext
    {
        /// <summary>
        /// The transaction container.
        /// </summary>
        private readonly Lazy<TransactionContainer> transactionContainer = new Lazy<TransactionContainer>(() => new TransactionContainer());

        /// <summary>
        /// Gets the provider for the current transaction.
        /// </summary>
        [Ignore]
        public IRepositoryProvider TransactionProvider
        {
            get
            {
                return this.transactionContainer.Value.TransactionProvider;
            }
        }

        /// <summary>
        /// Sets the transaction provider for the current object.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider to use for the transaction context.
        /// </param>
        public void SetTransactionProvider(IRepositoryProvider repositoryProvider)
        {
            this.transactionContainer.Value.SetTransactionProvider(repositoryProvider);
        }
    }
}