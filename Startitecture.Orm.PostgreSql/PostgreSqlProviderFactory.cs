// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostgreSqlProviderFactory.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Factory class for creating DatabaseRepositoryProvider instances for PostgreSQL using NPGSQL.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.PostgreSql
{
    using System;
    using System.Data.Common;
    using System.Diagnostics;
    using System.Linq;

    using JetBrains.Annotations;

    using Npgsql;

    using Startitecture.Orm.Common;
    using Startitecture.Orm.Mapper;
    using Startitecture.Orm.Model;

    /// <summary>
    /// Factory class for creating <see cref="DatabaseRepositoryProvider"/> instances for PostgreSQL using NPGSQL.
    /// </summary>
    public class PostgreSqlProviderFactory : IRepositoryProviderFactory
    {
        /// <summary>
        /// The provider invariant name.
        /// </summary>
        private const string ProviderInvariantName = "Npgsql";

        /// <summary>
        /// The definition provider.
        /// </summary>
        private readonly IEntityDefinitionProvider definitionProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlProviderFactory"/> class.
        /// </summary>
        /// <param name="definitionProvider">
        /// The definition provider.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="definitionProvider"/> is null.
        /// </exception>
        public PostgreSqlProviderFactory([NotNull] IEntityDefinitionProvider definitionProvider)
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
                DbProviderFactories.RegisterFactory(ProviderInvariantName, NpgsqlFactory.Instance);
            }
#endif
            var statementCompiler = new PostgreSqlAdapter(this.definitionProvider);
            var contextFactory = new DefaultDatabaseContextFactory(connectionString, ProviderInvariantName, statementCompiler);
            return new DatabaseRepositoryProvider(contextFactory);
        }
    }
}