// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostgreSqlProviderFactory.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
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
    using Startitecture.Resources;

    /// <summary>
    /// The postgre sql provider factory.
    /// </summary>
    public class PostgreSqlProviderFactory : IRepositoryProviderFactory
    {
        /// <summary>
        /// The provider invariant name.
        /// </summary>
        private const string ProviderInvariantName = "Npgsql";

        /// <summary>
        /// The connection string.
        /// </summary>
        private readonly string connectionString;

        /// <summary>
        /// The definition provider.
        /// </summary>
        private readonly IEntityDefinitionProvider definitionProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlProviderFactory"/> class.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <param name="definitionProvider">
        /// The definition provider.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="definitionProvider"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="connectionString"/> is null or whitespace.
        /// </exception>
        public PostgreSqlProviderFactory([NotNull] string connectionString, [NotNull] IEntityDefinitionProvider definitionProvider)
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
                DbProviderFactories.RegisterFactory(ProviderInvariantName, NpgsqlFactory.Instance);
            }

#endif
            var statementCompiler = new PostgreSqlAdapter(this.definitionProvider);
            var contextFactory = new DefaultDatabaseContextFactory(this.connectionString, ProviderInvariantName, statementCompiler);
            return new DatabaseRepositoryProvider(contextFactory);
        }
    }
}