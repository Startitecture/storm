// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityMapperFactory.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.AutoMapper
{
    using System;

    using global::AutoMapper;

    using JetBrains.Annotations;

    using Startitecture.Orm.Model;

    /// <summary>
    /// The entity mapper factory.
    /// </summary>
    public class EntityMapperFactory : IEntityMapperFactory
    {
        /// <summary>
        /// The configuration provider.
        /// </summary>
        private readonly IConfigurationProvider configurationProvider;

        /// <summary>
        /// The service constructor.
        /// </summary>
        private readonly Func<Type, object> serviceConstructor;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityMapperFactory"/> class.
        /// </summary>
        /// <param name="configurationProvider">
        /// The configuration provider.
        /// </param>
        public EntityMapperFactory([NotNull] IConfigurationProvider configurationProvider)
        {
            this.configurationProvider = configurationProvider ?? throw new ArgumentNullException(nameof(configurationProvider));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityMapperFactory"/> class.
        /// </summary>
        /// <param name="configurationProvider">
        /// The configuration provider.
        /// </param>
        /// <param name="serviceConstructor">
        /// The service constructor.
        /// </param>
        public EntityMapperFactory([NotNull] IConfigurationProvider configurationProvider, [NotNull] Func<Type, object> serviceConstructor)
        {
            this.configurationProvider = configurationProvider ?? throw new ArgumentNullException(nameof(configurationProvider));
            this.serviceConstructor = serviceConstructor ?? throw new ArgumentNullException(nameof(serviceConstructor));
        }

        /// <summary>
        /// The create.
        /// </summary>
        /// <returns>
        /// The <see cref="IEntityMapper"/>.
        /// </returns>
        public IEntityMapper Create()
        {
            return this.serviceConstructor == null
                       ? new AutoMapperEntityMapper(this.configurationProvider)
                       : new AutoMapperEntityMapper(this.configurationProvider, this.serviceConstructor);
        }
    }
}