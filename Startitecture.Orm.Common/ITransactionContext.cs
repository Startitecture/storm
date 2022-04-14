// <copyright file="ITransactionContext.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>

namespace Startitecture.Orm.Common
{
    using System;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// An interface for wrapping a transaction with an event so shared references of the transaction can be disposed.
    /// </summary>
    public interface ITransactionContext : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// Triggered when the transaction context is disposed.
        /// </summary>
        event EventHandler Disposed;

        /// <summary>
        /// Gets a value indicating whether the transaction context is disposed.
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// Gets the isolation level of the underlying transaction.
        /// </summary>
        IsolationLevel IsolationLevel { get; }

        /// <summary>
        /// Commits the underlying transaction.
        /// </summary>
        void Commit();

        /// <summary>
        /// Commits the underlying transaction.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> that is committing the transaction.
        /// </returns>
        Task CommitAsync();

        /// <summary>
        /// Commits the underlying transaction.
        /// </summary>
        /// <param name="cancellationToken">
        /// The cancellation token for this task.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> that is committing the transaction.
        /// </returns>
        Task CommitAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Rolls back the underlying transaction.
        /// </summary>
        void Rollback();

        /// <summary>
        /// Rolls back the underlying transaction.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> that is rolling back the transaction.
        /// </returns>
        Task RollbackAsync();

        /// <summary>
        /// Rolls back the underlying transaction.
        /// </summary>
        /// <param name="cancellationToken">
        /// The cancellation token for this task.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> that is rolling back the transaction.
        /// </returns>
        Task RollbackAsync(CancellationToken cancellationToken);
    }
}