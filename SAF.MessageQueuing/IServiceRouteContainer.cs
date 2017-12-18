// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceRouteContainer.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface for classes that resolve document queue pool interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;

    using SAF.Core;

    /// <summary>
    /// Provides an interface for classes that resolve document queue pools.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of message handled by the service routes.
    /// </typeparam>
    public interface IServiceRouteContainer<TMessage> : IDisposable
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        /// <summary>
        /// Resolves the document routing queue pool for the specified queue route type name.
        /// </summary>
        /// <param name="routeType">
        /// The name of the route type.
        /// </param>
        /// <returns>
        /// The queue pool as a <see cref="IServiceRoute{TMessage}"/>.
        /// </returns>
        IServiceRoute<TMessage> Resolve(string routeType);

        /// <summary>
        /// Registers a service route with the service route container.
        /// </summary>
        /// <param name="serviceRoute">
        /// The service route to register.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="serviceRoute"/> is null.
        /// </exception>
        /// <exception cref="BusinessException">
        /// A service route by the same name is already registered.
        /// </exception>
        void Register(IServiceRoute<TMessage> serviceRoute);

        /// <summary>
        /// Removes a service route from the route container.
        /// </summary>
        /// <param name="serviceRoute">
        /// The service route to remove.
        /// </param>
        void Remove(IServiceRoute<TMessage> serviceRoute);
    }
}