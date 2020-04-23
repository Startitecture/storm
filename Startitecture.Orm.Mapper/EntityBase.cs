// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityBase.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The transaction item base.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Mapper
{
    using System;

    using Startitecture.Orm.Model;
    using Startitecture.Orm.Schema;

    /// <summary>
    /// The transaction item base.
    /// </summary>
    public class EntityBase : ITransactionContext
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
        public void SetTransactionProvider([JetBrains.Annotations.NotNull] IRepositoryProvider repositoryProvider)
        {
            this.TransactionProvider = repositoryProvider ?? throw new ArgumentNullException(nameof(repositoryProvider));
        }
    }
}