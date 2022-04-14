// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultDatabaseContextFactory.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Creates an IDatabaseContext based on the ADO.NET provider name.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Mapper
{
    using System;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Startitecture.Orm.Common;
    using Startitecture.Resources;

    /// <summary>
    /// Creates an <see cref="IDatabaseContext"/> based on the ADO.NET provider name.
    /// </summary>
    public class DefaultDatabaseContextFactory : IDatabaseContextFactory
    {
        /// <summary>
        /// The connection string.
        /// </summary>
        private readonly string connectionString;

        /// <summary>
        /// The provider name.
        /// </summary>
        private readonly string providerName;

        /// <summary>
        /// The definition provider.
        /// </summary>
        private readonly IRepositoryAdapter repositoryAdapter;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDatabaseContextFactory"/> class.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <param name="providerName">
        /// The provider name.
        /// </param>
        /// <param name="repositoryAdapter">
        /// The definition provider.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="connectionString"/> or <paramref name="providerName"/> is null or whitespace.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="repositoryAdapter"/> is null.
        /// </exception>
        public DefaultDatabaseContextFactory(
            [NotNull] string connectionString,
            [NotNull] string providerName,
            [NotNull] IRepositoryAdapter repositoryAdapter)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(connectionString));
            }

            if (string.IsNullOrWhiteSpace(providerName))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(providerName));
            }

            this.repositoryAdapter = repositoryAdapter ?? throw new ArgumentNullException(nameof(repositoryAdapter));
            this.connectionString = connectionString;
            this.providerName = providerName;
        }

        /// <inheritdoc />
        public IDatabaseContext Create()
        {
            var providerFactory = DbProviderFactories.GetFactory(this.providerName);
            return new DatabaseContext(this.connectionString, providerFactory, this.repositoryAdapter);
        }

        /// <inheritdoc />
        public async Task<IDatabaseContext> CreateAsync()
        {
            return await this.CreateAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IDatabaseContext> CreateAsync(CancellationToken cancellationToken)
        {
            var providerFactory = DbProviderFactories.GetFactory(this.providerName);
            return await Task.FromResult(new DatabaseContext(this.connectionString, providerFactory, this.repositoryAdapter));
        }
    }
}