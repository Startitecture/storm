// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutoMapperEntityMapper.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Implements IEntityMapper using AutoMapper.
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
        /// <param name="mapper">
        /// The mapper.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mapper"/> is null.
        /// </exception>
        public AutoMapperEntityMapper([NotNull] IMapper mapper)
        {
            this.mapperEngine = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <inheritdoc />
        public TOutput Map<TOutput>(object input)
        {
            return this.Map<TOutput>(input, null);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">
        /// <paramref name="input"/> was null.
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

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">
        /// <paramref name="input"/> is null.
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
