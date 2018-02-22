// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutoMapperEntityMapper.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Contains mapping for transfer object types.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository
{
    using System;

    using AutoMapper;

    using Startitecture.Core;
    using Startitecture.Orm.Common;

    /// <summary>
    /// Implements <see cref="Startitecture.Orm.Common.IEntityMapper"/> using AutoMapper.
    /// </summary>
    public sealed class AutoMapperEntityMapper : IEntityMapper
    {
        /// <summary>
        /// The mapper engine.
        /// </summary>
        private IMapper mapperEngine;

        /// <summary>
        /// Initializes the default mapper and asserts the configuration is valid.
        /// </summary>
        /// <param name="configAction">
        /// The AutoMapper configuration action to apply.
        /// </param>
        public void Initialize(Action<IMapperConfigurationExpression> configAction)
        {
            if (configAction == null)
            {
                throw new ArgumentNullException(nameof(configAction));
            }

            var mappingConfiguration = new MapperConfiguration(configAction);
            mappingConfiguration.AssertConfigurationIsValid();

            this.mapperEngine = mappingConfiguration.CreateMapper();
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
        /// <exception cref="ArgumentNullException">
        /// <paramref name="input"/> was null.
        /// </exception>
        /// <exception cref="T:SAF.Core.ApplicationConfigurationException">
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
        /// <exception cref="ArgumentNullException">
        /// <paramref name="input"/> was null.
        /// </exception>
        /// <exception cref="T:SAF.Core.ApplicationConfigurationException">
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
        /// <exception cref="ApplicationConfigurationException">
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
