// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlClientProviderFactory.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Creates IRepositoryProvider instances for SQL Server connections.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.SqlClient
{
    using System;
    using System.Data.Common;
    using System.Diagnostics;
    using System.Linq;

    using JetBrains.Annotations;

    using Microsoft.Data.SqlClient;

    using Startitecture.Orm.Common;
    using Startitecture.Orm.Mapper;
    using Startitecture.Orm.Model;

    /// <summary>
    /// Creates <see cref="IRepositoryProvider"/> instances for SQL Server connections.
    /// </summary>
    public class SqlClientProviderFactory : IRepositoryProviderFactory
    {
        /// <summary>
        /// The provider invariant name.
        /// </summary>
        private const string ProviderInvariantName = "System.Data.SqlClient";

        /// <summary>
        /// The definition provider.
        /// </summary>
        private readonly IEntityDefinitionProvider definitionProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlClientProviderFactory"/> class.
        /// </summary>
        /// <param name="definitionProvider">
        /// The definition provider.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="definitionProvider"/> is null.
        /// </exception>
        public SqlClientProviderFactory([NotNull] IEntityDefinitionProvider definitionProvider)
        {
            this.definitionProvider = definitionProvider ?? throw new ArgumentNullException(nameof(definitionProvider));
        }

        /// <inheritdoc />
        public IRepositoryProvider Create(string connectionString)
        {
#if !NET472
            if (DbProviderFactories.GetProviderInvariantNames().Any(s => string.Equals(s, ProviderInvariantName, StringComparison.Ordinal)) == false)
            {
                Trace.WriteLine($"Registering {ProviderInvariantName} factory");
                DbProviderFactories.RegisterFactory(ProviderInvariantName, SqlClientFactory.Instance);
            }
#endif
            var statementCompiler = new TransactSqlAdapter(this.definitionProvider);
            var databaseFactory = new DefaultDatabaseContextFactory(connectionString, ProviderInvariantName, statementCompiler);
            return new DatabaseRepositoryProvider(databaseFactory);
        }
    }
}