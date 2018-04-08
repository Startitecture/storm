// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultDatabaseFactory.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Mapper
{
    using System;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Schema;
    using Startitecture.Resources;

    /// <summary>
    /// The default database factory.
    /// </summary>
    public class DefaultDatabaseFactory : IDatabaseFactory
    {
        /// <summary>
        /// The connection name.
        /// </summary>
        private readonly string connectionName;

        /// <summary>
        /// The definition provider.
        /// </summary>
        private readonly IEntityDefinitionProvider definitionProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDatabaseFactory"/> class.
        /// </summary>
        /// <param name="connectionName">
        /// The connection name.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="connectionName"/> is null or whitespace.
        /// </exception>
        public DefaultDatabaseFactory([NotNull] string connectionName)
            : this(connectionName, Singleton<DataAnnotationsDefinitionProvider>.Instance)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDatabaseFactory"/> class.
        /// </summary>
        /// <param name="connectionName">
        /// The connection name.
        /// </param>
        /// <param name="definitionProvider">
        /// The definition provider.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="connectionName"/> is null or whitespace.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="definitionProvider"/> is null
        /// </exception>
        public DefaultDatabaseFactory([NotNull] string connectionName, [NotNull] IEntityDefinitionProvider definitionProvider)
        {
            if (string.IsNullOrWhiteSpace(connectionName))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(connectionName));
            }

            if (definitionProvider == null)
            {
                throw new ArgumentNullException(nameof(definitionProvider));
            }

            this.connectionName = connectionName;
            this.definitionProvider = definitionProvider;
        }

        /// <inheritdoc />
        public IDatabaseContext Create()
        {
            return new Database(this.connectionName, this.definitionProvider);
        }
    }
}