// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageDelivery.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Contains information about a message delivery event.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;

    using SAF.StringResources;

    /// <summary>
    /// Contains information about a message delivery event.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of message being delivered.
    /// </typeparam>
    public class MessageDelivery<TMessage>
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        /// <summary>
        /// The delivery message.
        /// </summary>
        private const string DeliveryMessage = "{0} delivered at {1}";

        /// <summary>
        /// The failure message.
        /// </summary>
        private const string FailureMessage = "{0} delivery failed at {1}: {2}";

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageDelivery{TMessage}"/> class.
        /// </summary>
        /// <param name="routingRequest">
        /// The routing request.
        /// </param>
        public MessageDelivery(MessageRoutingRequest<TMessage> routingRequest)
        {
            if (routingRequest == null)
            {
                throw new ArgumentNullException("routingRequest");
            }

            if (routingRequest.CurrentLocation == null)
            {
                throw new ArgumentException(ValidationMessages.RoutingStatusNotSet, "routingRequest");
            }

            if (routingRequest.CurrentLocation.ServiceRoute == null)
            {
                throw new ArgumentException(ValidationMessages.RoutingStatusRouteNotSet, "routingRequest");
            }

            this.DeliveryTime = DateTimeOffset.Now;
            this.RoutingRequest = routingRequest;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the delivery error.
        /// </summary>
        public Exception DeliveryError
        {
            get
            {
                return this.RoutingRequest.RoutingError;
            }
        }

        /// <summary>
        /// Gets the delivery time.
        /// </summary>
        public DateTimeOffset DeliveryTime { get; private set; }

        /// <summary>
        /// Gets the delivered message.
        /// </summary>
        public MessageRoutingRequest<TMessage> RoutingRequest { get; private set; }

        /// <summary>
        /// Gets the service route that delivered the message.
        /// </summary>
        public string ServiceRoute
        {
            get
            {
                return this.RoutingRequest.CurrentLocation.ServiceRoute.Name;
            }
        }

        #endregion

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="MessageDelivery{TMessage}"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="MessageDelivery{TMessage}"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return this.DeliveryError == null
                       ? String.Format(DeliveryMessage, this.RoutingRequest, this.DeliveryTime)
                       : String.Format(FailureMessage, this.RoutingRequest, this.DeliveryTime, this.DeliveryError.Message);
        }
    }
}