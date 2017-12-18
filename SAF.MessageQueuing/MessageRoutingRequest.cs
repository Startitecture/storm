// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageRoutingRequest.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.MessageQueuing
{
    using System;

    using SAF.Core;
    using SAF.StringResources;

    /// <summary>
    /// Contains request properties for routing a message.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of message to route.
    /// </typeparam>
    public class MessageRoutingRequest<TMessage> : IEquatable<MessageRoutingRequest<TMessage>>,
                                                   IComparable,
                                                   IComparable<MessageRoutingRequest<TMessage>>
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        /// <summary>
        /// The to string format with first route.
        /// </summary>
        private const string ToStringFormat = "{0}: '{1}'";

        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<MessageRoutingRequest<TMessage>, object>[] ComparisonProperties =
                {
                    item => item.Message
                };

        /// <summary>
        /// The synchronization context.
        /// </summary>
        private readonly object syncLock = new object();

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRoutingRequest{TMessage}"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="routingNotification">
        /// The routing notification.
        /// </param>
        public MessageRoutingRequest(TMessage message, INotifyMessageRouted<TMessage> routingNotification)
        {
            if (Evaluate.IsNull(message))
            {
                throw new ArgumentNullException("message");
            }

            if (routingNotification == null)
            {
                throw new ArgumentNullException("routingNotification");
            }

            this.Message = message;
            this.RoutingNotification = routingNotification;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRoutingRequest{TMessage}"/> class.
        /// </summary>
        /// <param name="initialLocation">
        /// The initial location of the message.
        /// </param>
        /// <param name="routingNotification">
        /// The routing notification.
        /// </param>
        public MessageRoutingRequest(IRoutingStatus<TMessage> initialLocation, INotifyMessageRouted<TMessage> routingNotification)
        {
            if (initialLocation == null)
            {
                throw new ArgumentNullException("initialLocation");
            }

            if (routingNotification == null)
            {
                throw new ArgumentNullException("routingNotification");
            }

            Evaluate.ThrowOnDependencyFailure(initialLocation, status => status.Message);
            this.Message = initialLocation.Message;
            this.RoutingNotification = routingNotification;
            this.InitialLocation = initialLocation;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the message to route.
        /// </summary>
        public TMessage Message { get; private set; }

        /// <summary>
        /// Gets the routing error, if any, associated with the request.
        /// </summary>
        public DomainException RoutingError { get; private set; }

        /// <summary>
        /// Gets the initial location, if any, of the message at the time it was created.
        /// </summary>
        public IRoutingStatus<TMessage> InitialLocation { get; private set; }

        /// <summary>
        /// Gets the current location, if any, of the message.
        /// </summary>
        public IRoutingStatus<TMessage> CurrentLocation { get; private set; }

        /// <summary>
        /// Gets the routing notification, if any, for the current message.
        /// </summary>
        public INotifyMessageRouted<TMessage> RoutingNotification { get; private set; }

        /// <summary>
        /// Gets the routing configuration, if any, for the current message.
        /// </summary>
        public RoutingConfiguration<TMessage> RoutingConfiguration { get; private set; }

        /// <summary>
        /// Gets a value indicating whether a requeue is pending for this request.
        /// </summary>
        public bool RequeuePending { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the request has been canceled.
        /// </summary>
        public bool Canceled { get; private set; }

        #endregion

        /// <summary>
        /// Determines whether two objects are equal.
        /// </summary>
        /// <param name="valueA">
        /// The first value.
        /// </param>
        /// <param name="valueB">
        /// The second value.
        /// </param>
        /// <returns>
        /// <c>true</c> if the values are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(MessageRoutingRequest<TMessage> valueA, MessageRoutingRequest<TMessage> valueB)
        {
            return Evaluate.Equals(valueA, valueB);
        }

        /// <summary>
        /// Determines whether two objects are not equal.
        /// </summary>
        /// <param name="valueA">
        /// The first value.
        /// </param>
        /// <param name="valueB">
        /// The second value.
        /// </param>
        /// <returns>
        /// <c>true</c> if the values are not equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(MessageRoutingRequest<TMessage> valueA, MessageRoutingRequest<TMessage> valueB)
        {
            return !(valueA == valueB);
        }

        /// <summary>
        /// Determines whether the first value is less than the second value.
        /// </summary>
        /// <param name="valueA">
        /// The first value.
        /// </param>
        /// <param name="valueB">
        /// The second value.
        /// </param>
        /// <returns>
        /// <c>true</c> if the first value is less than the second value; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator <(MessageRoutingRequest<TMessage> valueA, MessageRoutingRequest<TMessage> valueB)
        {
            return Evaluate.Compare(valueA, valueB) < 0;
        }

        /// <summary>
        /// Determines whether the first value is greater than the second value.
        /// </summary>
        /// <param name="valueA">
        /// The first value.
        /// </param>
        /// <param name="valueB">
        /// The second value.
        /// </param>
        /// <returns>
        /// <c>true</c> if the first value is greater than the second value; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator >(MessageRoutingRequest<TMessage> valueA, MessageRoutingRequest<TMessage> valueB)
        {
            return Evaluate.Compare(valueA, valueB) > 0;
        }

        /// <summary>
        /// Cancels the current request.
        /// </summary>
        public void Cancel()
        {
            lock (this.syncLock)
            { 
                this.Canceled = true;
                this.RoutingError = new MessageCanceledException(
                    this.Message,
                    ErrorMessages.DeliveryAdministrativelyCanceled,
                    this.RoutingError);
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="MessageRoutingRequest{TMessage}"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="MessageRoutingRequest{TMessage}"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return String.Format(ToStringFormat, this.CurrentLocation, this.Message);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return Evaluate.GenerateHashCode(this, ComparisonProperties);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. </param>
        /// <filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            return Evaluate.Equals(this, obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(MessageRoutingRequest<TMessage> other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance is less than <paramref name="obj"/>. Zero This instance is equal to <paramref name="obj"/>. Greater than zero This instance is greater than <paramref name="obj"/>. 
        /// </returns>
        /// <param name="obj">An object to compare with this instance. </param><exception cref="T:System.ArgumentException"><paramref name="obj"/> is not the same type as this instance. </exception><filterpriority>2</filterpriority>
        public int CompareTo(object obj)
        {
            return Evaluate.Compare(this, obj);
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(MessageRoutingRequest<TMessage> other)
        {
            return Evaluate.Compare(this, other, ComparisonProperties);
        }

        /// <summary>
        /// Loads the routing configuration for the current request.
        /// </summary>
        /// <param name="configuration">
        /// The routing profile.
        /// </param>
        internal void LoadRoutingConfiguration(RoutingConfiguration<TMessage> configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            this.RoutingConfiguration = configuration;
        }

        /// <summary>
        /// Starts the routing request.
        /// </summary>
        /// <param name="entryEvent">
        /// The entry event.
        /// </param>
        internal void StartRequest(MessageEntry<TMessage> entryEvent)
        {
            lock (this.syncLock)
            {
                this.RequeuePending = false;
                this.RoutingError = null;
            }

            this.CurrentLocation = new RoutingStatus<TMessage>(
                entryEvent.Identifier,
                entryEvent.InitiationTime,
                entryEvent.RoutingRequest.Message,
                entryEvent.ServiceRoute);
        }

        /// <summary>
        /// Sets the route that contains the message.
        /// </summary>
        /// <param name="entryEvent">
        /// The service route that contains the message.
        /// </param>
        internal void UpdateLocation(MessageEntry<TMessage> entryEvent)
        {
            if (entryEvent == null)
            {
                throw new ArgumentNullException("entryEvent");
            }

            if (this.CurrentLocation == null)
            {
                this.CurrentLocation = new RoutingStatus<TMessage>(
                    entryEvent.Identifier,
                    entryEvent.InitiationTime,
                    this.Message,
                    entryEvent.ServiceRoute);
            }
            else
            {
                this.CurrentLocation.UpdateLocation(entryEvent);
            }
        }

        /// <summary>
        /// Fails the current request.
        /// </summary>
        /// <param name="routingError">
        /// The routing error.
        /// </param>
        internal void FailRequest(Exception routingError)
        {
            if (routingError == null)
            {
                throw new ArgumentNullException("routingError");
            }

            lock (this.syncLock)
            {
                this.RoutingError = routingError as DomainException ?? new OperationException(this, routingError.Message, routingError);
            }
        }

        /// <summary>
        /// Supersedes this message for another message.
        /// </summary>
        internal void Supersede()
        {
            lock (this.syncLock)
            {
                this.Canceled = true;
                this.RoutingError = new MessageCanceledException(
                    this.Message,
                    ErrorMessages.MessageCanceledByNewerMessage,
                    this.RoutingError);
            }
        }

        /// <summary>
        /// Requests a restart of the routing request.
        /// </summary>
        internal void RequestRestart()
        {
            lock (this.syncLock)
            {
                this.RequeuePending = true;
                this.RoutingError = null;
                this.Canceled = false;
            }
        }

        /// <summary>
        /// Clears the routing error for the request.
        /// </summary>
        internal void ClearFailure()
        {
            lock (this.syncLock)
            {
                this.RoutingError = null;
                this.Canceled = false;
            }
        }
    }
}