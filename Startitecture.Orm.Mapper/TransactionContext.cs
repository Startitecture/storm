// <copyright file="TransactionContext.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>

namespace Startitecture.Orm.Mapper
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Startitecture.Orm.Common;

    /// <summary>
    /// Wraps a <see cref="DbTransaction"/> so that shared references to the transaction can be disposed.
    /// </summary>
    public sealed class TransactionContext : ITransactionContext
    {
        private IDbTransaction transaction;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionContext"/> class.
        /// </summary>
        /// <param name="transaction">
        /// The transaction to wrap.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="transaction"/> is null.
        /// </exception>
        public TransactionContext([NotNull] IDbTransaction transaction)
        {
            this.transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        }

        /// <inheritdoc />
        public event EventHandler Disposed;

        /// <inheritdoc />
        public bool IsDisposed { get; private set; }

        /// <inheritdoc />
        public IsolationLevel IsolationLevel => this.transaction?.IsolationLevel ?? IsolationLevel.Unspecified;

        /// <inheritdoc />
        public void Dispose()
        {
            this.transaction?.Dispose();
            this.Disposed?.Invoke(this, EventArgs.Empty);
            this.transaction = null;
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
#if NET472
            if (this.transaction is IAsyncDisposable asyncTransaction)
            {
                await asyncTransaction.DisposeAsync().ConfigureAwait(false);
            }
#else
            if (this.transaction is DbTransaction asyncTransaction)
            {
                await asyncTransaction.DisposeAsync().ConfigureAwait(false);
            }
#endif
            this.Disposed?.Invoke(this, EventArgs.Empty);
            this.transaction = null;
            this.IsDisposed = true;
        }

        /// <inheritdoc />
        public void Commit()
        {
            if (this.transaction == null)
            {
                throw new ObjectDisposedException(nameof(this.transaction));
            }

            this.transaction?.Commit();
        }

        /// <inheritdoc />
        public async Task CommitAsync()
        {
            await this.CommitAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <param name="cancellationToken"></param>
        /// <inheritdoc />
        public async Task CommitAsync(CancellationToken cancellationToken)
        {
            if (this.transaction == null)
            {
                throw new ObjectDisposedException(nameof(this.transaction));
            }

            if (this.transaction is DbTransaction asyncTransaction)
            {
#if NET472
                asyncTransaction.Commit();
#else
                await asyncTransaction.CommitAsync(cancellationToken).ConfigureAwait(false);
#endif
            }
            else
            {
                this.transaction.Commit();
            }
        }

        /// <inheritdoc />
        public void Rollback()
        {
            if (this.transaction == null)
            {
                throw new ObjectDisposedException(nameof(this.transaction));
            }

            this.transaction.Rollback();
        }

        /// <inheritdoc />
        public Task RollbackAsync()
        {
            throw new NotImplementedException();
        }

        /// <param name="cancellationToken"></param>
        /// <inheritdoc />
        public async Task RollbackAsync(CancellationToken cancellationToken)
        {
            if (this.transaction == null)
            {
                throw new ObjectDisposedException(nameof(this.transaction));
            }

            if (this.transaction is DbTransaction asyncTransaction)
            {
#if NET472
                asyncTransaction.Rollback();
#else
                await asyncTransaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
#endif
            }
            else
            {
                this.transaction.Rollback();
            }
        }
    }
}