// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceRouteContainer.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The document queue pool provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using JetBrains.Annotations;

    using SAF.Core;
    using SAF.ProcessEngine;
    using SAF.StringResources;

    /// <summary>
    /// Resolves document queue pools using the queue route type name.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of message processed by the service routes.
    /// </typeparam>
    public sealed class ServiceRouteContainer<TMessage> : IServiceRouteContainer<TMessage>
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        /// <summary>
        /// The default provider.
        /// </summary>
        private static readonly ServiceRouteContainer<TMessage> DefaultProvider = new ServiceRouteContainer<TMessage>();

        /// <summary>
        /// The pool dictionary.
        /// </summary>
        private readonly Dictionary<string, IServiceRoute<TMessage>> poolDictionary = new Dictionary<string, IServiceRoute<TMessage>>();

        /// <summary>
        /// Gets the default document queue pool provider.
        /// </summary>
        public static ServiceRouteContainer<TMessage> Default
        {
            get
            {
                return DefaultProvider;
            }
        }

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
        public void Register(IServiceRoute<TMessage> serviceRoute)
        {
            if (serviceRoute == null)
            {
                throw new ArgumentNullException("serviceRoute");
            }

            try
            {
                this.poolDictionary.Add(serviceRoute.Name, serviceRoute);
            }
            catch (ArgumentException ex)
            {
                throw new BusinessException(serviceRoute, ex.Message, ex);
            }
        }

        /// <summary>
        /// Removes a service route from the route container.
        /// </summary>
        /// <param name="serviceRoute">
        /// The service route to remove.
        /// </param>
        public void Remove([NotNull] IServiceRoute<TMessage> serviceRoute)
        {
            if (serviceRoute == null)
            {
                throw new ArgumentNullException("serviceRoute");
            }

            this.poolDictionary.Remove(serviceRoute.Name);
        }

        /// <summary>
        /// Resolves the document routing queue pool for the specified queue route type name.
        /// </summary>
        /// <param name="routeType">
        /// The name of the route type.
        /// </param>
        /// <returns>
        /// The queue pool as a <see cref="IServiceRoute{TMessage}"/>.
        /// </returns>
        public IServiceRoute<TMessage> Resolve(string routeType)
        {
            IServiceRoute<TMessage> serviceRoute;

            if (this.poolDictionary.TryGetValue(routeType, out serviceRoute) == false)
            {
                throw new BusinessException(routeType, ValidationMessages.ServiceRouteTypeNotFound);
            }

            return serviceRoute;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            foreach (var processRoute in this.poolDictionary.Select(serviceRoute => serviceRoute.Value).OfType<IProcessEngine>())
            {
                processRoute.Cancel();
            }

            this.poolDictionary.Clear();
        }
    }
}
