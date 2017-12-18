// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceRoutingPath.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The service routing path.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using SAF.Core;
    using SAF.StringResources;

    /// <summary>
    /// The service routing path.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of message that is routed.
    /// </typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Naming", 
        "CA1710:IdentifiersShouldHaveCorrectSuffix",
        Justification = "This class is intended to be used as a linked list so Path is a more appropriate description.")]
    public class ServiceRoutingPath<TMessage> : IEnumerable<IServiceRoute<TMessage>>
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        /// <summary>
        /// The routes.
        /// </summary>
        private readonly LinkedList<IServiceRoute<TMessage>> routes;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRoutingPath{TMessage}"/> class.
        /// </summary>
        /// <param name="serviceRoutes">
        /// The service routes.
        /// </param>
        public ServiceRoutingPath(params IServiceRoute<TMessage>[] serviceRoutes)
        {
            if (serviceRoutes == null)
            {
                throw new ArgumentNullException("serviceRoutes");
            }

            if (!serviceRoutes.Any())
            {
                throw new BusinessException(serviceRoutes, ValidationMessages.ServiceRoutesNotSpecified);
            }

            if (serviceRoutes.Any(x => x == null))
            {
                throw new BusinessException(serviceRoutes, ValidationMessages.RoutingProfileContainedNullRoute);
            }

            this.routes = new LinkedList<IServiceRoute<TMessage>>(serviceRoutes);
        }

        /// <summary>
        /// Gets the first route in the path.
        /// </summary>
        public IServiceRoute<TMessage> First
        {
            get
            {
                return this.routes.First == null ? null : this.routes.First.Value;
            }
        }

        /// <summary>
        /// Gets the last route in the path.
        /// </summary>
        public IServiceRoute<TMessage> Last
        {
            get
            {
                return this.routes.Last == null ? null : this.routes.Last.Value;
            }
        }

        /// <summary>
        /// Gets the next route based on the current route.
        /// </summary>
        /// <param name="current">
        /// The current route.
        /// </param>
        /// <returns>
        /// The next <see cref="T:SAF.MessageQueueing.IServiceRoute`1"/> in the routing path, or <c>null</c> if this is the last route.
        /// </returns>
        /// <exception cref="BusinessException">
        /// The routing path does not contain the route specified in <paramref name="current"/>.
        /// </exception>
        public IServiceRoute<TMessage> GetNext(IServiceRoute<TMessage> current)
        {
            if (current == null)
            {
                throw new ArgumentNullException("current");
            }

            // TODO: Getting the route, not the pool.
            // Get the current route, and ensure it is set.
            LinkedListNode<IServiceRoute<TMessage>> currentNode = this.routes.Find(current);

            if (currentNode == null)
            {
                throw new BusinessException(current, String.Format(ValidationMessages.RoutingProfileDidNotContainRoute, current));
            }

            return currentNode.Next == null ? null : currentNode.Next.Value;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<IServiceRoute<TMessage>> GetEnumerator()
        {
            return this.routes.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}