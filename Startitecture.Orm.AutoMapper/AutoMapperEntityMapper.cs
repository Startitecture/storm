// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutoMapperEntityMapper.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Contains mapping for transfer object types.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.AutoMapper
{
    using System;

    using global::AutoMapper;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Orm.Model;

    /// <summary>
    /// Implements <see cref="IEntityMapper"/> using AutoMapper.
    /// </summary>
    public sealed class AutoMapperEntityMapper : IEntityMapper
    {
        /// <summary>
        /// The mapper engine.
        /// </summary>
        private readonly IMapper mapperEngine;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMapperEntityMapper"/> class.
        /// </summary>
        /// <param name="configurationProvider">
        /// The configuration provider.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="configurationProvider"/> is null.
        /// </exception>
        public AutoMapperEntityMapper([NotNull] IConfigurationProvider configurationProvider)
        {
            if (configurationProvider == null)
            {
                throw new ArgumentNullException(nameof(configurationProvider));
            }

            this.mapperEngine = configurationProvider.CreateMapper();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMapperEntityMapper"/> class.
        /// </summary>
        /// <param name="configurationProvider">
        /// The configuration provider.
        /// </param>
        /// <param name="serviceConstructor">
        /// The service constructor.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="configurationProvider"/> or <paramref name="serviceConstructor"/> is null.
        /// </exception>
        public AutoMapperEntityMapper([NotNull] IConfigurationProvider configurationProvider, [NotNull] Func<Type, object> serviceConstructor)
        {
            if (configurationProvider == null)
            {
                throw new ArgumentNullException(nameof(configurationProvider));
            }

            if (serviceConstructor == null)
            {
                throw new ArgumentNullException(nameof(serviceConstructor));
            }

            this.mapperEngine = configurationProvider.CreateMapper(serviceConstructor);
        }

        /// <summary>
        /// Maps one object type to another object type.
        /// </summary>
        /// <param name="input">
        /// The input object.
        /// </param>
        /// <typeparam name="TOutput">
        /// The type of object to map to.
        /// </typeparam>
        /// <returns>
        /// The mapped object as a <typeparamref name="TOutput"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="input"/> was null.
        /// </exception>
        /// <exception cref="Startitecture.Core.OperationException">
        /// A mapping failed between the input and output type. The inner exception will contain details of the failure.
        /// </exception>
        public TOutput Map<TOutput>(object input)
        {
            return this.Map<TOutput>(input, null);
        }

        /// <summary>
        /// Maps one object type to another object type.
        /// </summary>
        /// <param name="input">
        /// The input object.
        /// </param>
        /// <param name="serviceConstructor">
        /// The service constructor.
        /// </param>
        /// <typeparam name="TOutput">
        /// The type of object to map to.
        /// </typeparam>
        /// <returns>
        /// The mapped object as a <typeparamref name="TOutput"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="input"/> was null.
        /// </exception>
        /// <exception cref="Startitecture.Core.OperationException">
        /// A mapping failed between the input and output type. The inner exception will contain details of the failure.
        /// </exception>
        public TOutput Map<TOutput>(object input, Func<Type, object> serviceConstructor)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            try
            {
                return serviceConstructor == null
                   ? this.mapperEngine.Map<TOutput>(input)
                   : this.mapperEngine.Map<TOutput>(input, options => options.ConstructServicesUsing(serviceConstructor));
            }
            catch (AutoMapperMappingException ex)
            {
                throw new OperationException(input, ex.Message, ex);
            }        
        }

        /// <summary>
        /// Maps one object type to another object type.
        /// </summary>
        /// <param name="input">
        /// The input object.
        /// </param>
        /// <param name="output">
        /// The output object.
        /// </param>
        /// <typeparam name="TInput">
        /// The type of object to map from.
        /// </typeparam>
        /// <typeparam name="TOutput">
        /// The type of object to map to.
        /// </typeparam>
        /// <returns>
        /// The mapped object as a <typeparamref name="TOutput"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="input"/> is null.
        /// </exception>
        /// <exception cref="Startitecture.Core.OperationException">
        /// The mapping could not be performed because a supported mapping has not been configured.
        /// </exception>
        public TOutput MapTo<TInput, TOutput>(TInput input, TOutput output)
        {
            if (Evaluate.IsNull(input))
            {
                throw new ArgumentNullException(nameof(input));
            }

            // Use the other mapping approach if output is null. The result is the same.
            if (Evaluate.IsNull(output))
            {
                return this.Map<TOutput>(input);
            }

            try
            {
                var result = this.mapperEngine.Map(input, output);
                return result;
            }
            catch (AutoMapperMappingException ex)
            {
                throw new OperationException(input, ex.Message, ex);
            }
        }
    }
}
