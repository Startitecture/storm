// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityMapper.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface to an entity mapper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data
{
    using System;

    using SAF.Core;

    /// <summary>
    /// Provides an interface to an entity mapper.
    /// </summary>
    public interface IEntityMapper
    {
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
        /// <exception cref="OperationException">
        /// The mapping could not be performed because a supported mapping has not been configured.
        /// </exception>
        TOutput Map<TOutput>(object input);

        /// <summary>
        /// Maps one object type to another object type.
        /// </summary>
        /// <param name="input">
        /// The input object.
        /// </param>
        /// <param name="serviceConstructor">
        /// The service constructor to use to create the new object.
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
        TOutput Map<TOutput>(object input, Func<Type, object> serviceConstructor);

        /// <summary>
        /// Maps one object to an object of a different type.
        /// </summary>
        /// <param name="input">
        /// The source object.
        /// </param>
        /// <param name="output">
        /// The target object.
        /// </param>
        /// <typeparam name="TSource">
        /// The type of object to map from.
        /// </typeparam>
        /// <typeparam name="TDestination">
        /// The type of object to map to.
        /// </typeparam>
        /// <returns>
        /// The mapped object as a <typeparamref name="TDestination"/>.
        /// </returns>
        /// <exception cref="OperationException">
        /// The mapping could not be performed because a supported mapping has not been configured.
        /// </exception>
        TDestination MapTo<TSource, TDestination>(TSource input, TDestination output);
    }
}