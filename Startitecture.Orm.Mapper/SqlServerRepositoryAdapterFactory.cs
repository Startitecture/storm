// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlServerRepositoryAdapterFactory.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The query repository adapter factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Mapper
{
    using System;

    using Common;

    using JetBrains.Annotations;

    using Startitecture.Orm.Model;

    /// <summary>
    /// The query repository adapter factory.
    /// </summary>
    public class SqlServerRepositoryAdapterFactory : IRepositoryAdapterFactory
    {
        /// <summary>
        /// The definition provider.
        /// </summary>
        private readonly IEntityDefinitionProvider definitionProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerRepositoryAdapterFactory"/> class.
        /// </summary>
        /// <param name="definitionProvider">
        /// The definition provider.
        /// </param>
        public SqlServerRepositoryAdapterFactory([NotNull] IEntityDefinitionProvider definitionProvider)
        {
            if (definitionProvider == null)
            {
                throw new ArgumentNullException(nameof(definitionProvider));
            }

            this.definitionProvider = definitionProvider;
        }

        #region Public Methods and Operators

        /// <inheritdoc />
        public IRepositoryAdapter Create(IDatabaseContext dataContext)
        {
            return new SqlServerRepositoryAdapter(dataContext, this.definitionProvider);
        }

        #endregion
    }
}