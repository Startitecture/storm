// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultDatabaseFactory.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Mapper
{
    using System;
    using System.Data.Common;

    using JetBrains.Annotations;

    using Startitecture.Orm.Common;
    using Startitecture.Orm.Model;
    using Startitecture.Resources;

    /// <summary>
    /// The default database factory.
    /// </summary>
    public class DefaultDatabaseFactory : IDatabaseFactory
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
        private readonly IEntityDefinitionProvider definitionProvider;

        /// <summary>
        /// The name qualifier.
        /// </summary>
        private readonly INameQualifier nameQualifier;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDatabaseFactory"/> class.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <param name="providerName">
        /// The provider name.
        /// </param>
        /// <param name="definitionProvider">
        /// The definition provider.
        /// </param>
        /// <param name="nameQualifier">
        /// The name Qualifier.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="connectionString"/> or <paramref name="providerName"/> is null or whitespace.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="definitionProvider"/> is null
        /// </exception>
        public DefaultDatabaseFactory(
            [NotNull] string connectionString,
            [NotNull] string providerName,
            [NotNull] IEntityDefinitionProvider definitionProvider,
            [NotNull] INameQualifier nameQualifier)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(connectionString));
            }

            if (string.IsNullOrWhiteSpace(providerName))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(providerName));
            }

            if (nameQualifier == null)
            {
                throw new ArgumentNullException(nameof(nameQualifier));
            }

            this.definitionProvider = definitionProvider ?? throw new ArgumentNullException(nameof(definitionProvider));
            this.nameQualifier = nameQualifier;
            this.connectionString = connectionString;
            this.providerName = providerName;
        }

        /// <inheritdoc />
        public IDatabaseContext Create()
        {
            var providerFactory = DbProviderFactories.GetFactory(this.providerName);
            return new Database(this.connectionString, providerFactory, this.definitionProvider, this.nameQualifier);
        }
    }
}