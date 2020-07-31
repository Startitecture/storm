// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlClientProviderFactory.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.SqlClient
{
    using System;
    using System.Data.Common;
    using System.Diagnostics;
    using System.Linq;

    using JetBrains.Annotations;

    using Microsoft.Data.SqlClient;

    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Mapper;
    using Startitecture.Orm.Model;
    using Startitecture.Resources;

    /// <summary>
    /// The SQL Server provider factory.
    /// </summary>
    public class SqlClientProviderFactory : IRepositoryProviderFactory
    {
        /// <summary>
        /// The provider invariant name.
        /// </summary>
        private const string ProviderInvariantName = "System.Data.SqlClient";

        /// <summary>
        /// The connection string.
        /// </summary>
        private readonly string connectionString;

        /// <summary>
        /// The definition provider.
        /// </summary>
        private readonly IEntityDefinitionProvider definitionProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlClientProviderFactory"/> class.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <param name="definitionProvider">
        /// The definition provider.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="definitionProvider"/> is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="connectionString"/> is null or whitespace.
        /// </exception>
        public SqlClientProviderFactory([NotNull] string connectionString, [NotNull] IEntityDefinitionProvider definitionProvider)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(connectionString));
            }

            this.connectionString = connectionString;
            this.definitionProvider = definitionProvider ?? throw new ArgumentNullException(nameof(definitionProvider));
        }

        /// <inheritdoc />
        public IRepositoryProvider Create()
        {
#if !NET472
            if (DbProviderFactories.GetProviderInvariantNames().Any(s => string.Equals(s, ProviderInvariantName, StringComparison.Ordinal)) == false)
            {
                Trace.WriteLine($"Registering {ProviderInvariantName} factory");
                DbProviderFactories.RegisterFactory(ProviderInvariantName, SqlClientFactory.Instance);
            }
#endif
            var databaseFactory = new DefaultDatabaseFactory(
                this.connectionString,
                ProviderInvariantName,
                this.definitionProvider,
                Singleton<TransactSqlQualifier>.Instance);

            return new DatabaseRepositoryProvider(databaseFactory, new TransactSqlFactory(this.definitionProvider));
        }
    }
}