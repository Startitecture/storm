﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultDatabaseContextFactory.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Mapper
{
    using System;
    using System.Data.Common;

    using JetBrains.Annotations;

    using Startitecture.Orm.Common;
    using Startitecture.Resources;

    /// <summary>
    /// The default database factory.
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
        /// <paramref name="repositoryAdapter"/> is null
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
    }
}