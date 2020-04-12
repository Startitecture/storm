// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlServerProviderFactory.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Sql
{
    using System;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Mapper;
    using Startitecture.Orm.Model;
    using Startitecture.Resources;

    /// <summary>
    /// The SQL Server provider factory.
    /// </summary>
    public class SqlServerProviderFactory : IRepositoryProviderFactory
    {
        /// <summary>
        /// The connection string.
        /// </summary>
        private readonly string connectionString;

        /// <summary>
        /// The definition provider.
        /// </summary>
        private readonly IEntityDefinitionProvider definitionProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerProviderFactory"/> class.
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
        public SqlServerProviderFactory([NotNull] string connectionString, [NotNull] IEntityDefinitionProvider definitionProvider)
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
            var databaseFactory = new DefaultDatabaseFactory(
                this.connectionString,
                "System.Data.SqlClient",
                this.definitionProvider,
                Singleton<TransactSqlQualifier>.Instance);

            return new DatabaseRepositoryProvider(databaseFactory, Singleton<SqlServerAdapterFactory>.Instance);
        }
    }
}