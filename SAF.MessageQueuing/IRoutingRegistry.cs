// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRoutingRegistry.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.MessageQueuing
{
    using System;

    /// <summary>
    /// Provides an interface for classes that register service routes for a service route container.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of message being routed.
    /// </typeparam>
    public interface IRoutingRegistry<TMessage>
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        /// <summary>
        /// Registers service routes with the specified service route container.
        /// </summary>
        /// <param name="serviceRouteContainer">
        /// The service route container to register routes with.
        /// </param>
        void RegisterContainer(IServiceRouteContainer<TMessage> serviceRouteContainer);
    }
}