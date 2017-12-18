// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageExit.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.MessageQueuing
{
    using System;

    using SAF.ActionTracking;
    using SAF.Core;

    /// <summary>
    /// Contains information about the exit of a request from a queue.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of <see cref="IPriorityMessage"/> exiting the queue.
    /// </typeparam>
    public class MessageExit<TMessage> : IEquatable<MessageExit<TMessage>>, 
                                         IComparable, 
                                         IComparable<MessageExit<TMessage>>
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        /// <summary>
        /// The failure format.
        /// </summary>
        private const string FailureFormat = "{0} - {1}";

        /// <summary>
        /// The to string format.
        /// </summary>
        private const string ToStringFormat = "{0} exiting at {1}: {2}";

        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<MessageExit<TMessage>, object>[] ComparisonProperties =
            {
                item => item.RoutingRequest, 
                item => item.InitiationTime, 
                item => item.CompletionTime, 
                item => item.ServiceRoute, 
                item => item.Request, 
                item => item.Identifier
            };

        /// <summary>
        /// Gets the original entry event.
        /// </summary>
        private readonly MessageEntry<TMessage> requestEvent;

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageExit{TMessage}"/> class. 
        /// Initializes a new instance of the <see cref="T:SAF.MessageQueuing.MessageExit`1"/> class.
        /// </summary>
        /// <param name="entry">
        /// The entry event.
        /// </param>
        public MessageExit(MessageEntry<TMessage> entry)
            : this(entry, DateTimeOffset.Now)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageExit{TMessage}"/> class. 
        /// Initializes a new instance of the <see cref="T:SAF.MessageQueuing.MessageExit`1"/> class.
        /// </summary>
        /// <param name="entry">
        /// The entry event.
        /// </param>
        /// <param name="error">
        /// The error associated with the process.
        /// </param>
        public MessageExit(MessageEntry<TMessage> entry, Exception error)
            : this(entry, error, DateTimeOffset.Now)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageExit{TMessage}"/> class. 
        /// Initializes a new instance of the <see cref="T:SAF.MessageQueuing.MessageExit`1"/> class.
        /// </summary>
        /// <param name="serviceRoute">
        /// The service route.
        /// </param>
        /// <param name="responseEvent">
        /// The response event.
        /// </param>
        internal MessageExit(IServiceRoute<TMessage> serviceRoute, MessageExit<TMessage> responseEvent)
        {
            this.requestEvent = new MessageEntry<TMessage>(serviceRoute, responseEvent);
            this.CompletionTime = responseEvent.CompletionTime;
            this.EventError = responseEvent.EventError;

            if (this.EventError != null)
            {
                this.requestEvent.RoutingRequest.FailRequest(this.EventError);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageExit{TMessage}"/> class.
        /// </summary>
        /// <param name="exit">
        /// The exit.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        internal MessageExit(MessageExit<TMessage> exit, Exception error)
        {
            if (exit == null)
            {
                throw new ArgumentNullException("exit");
            }

            if (error == null)
            {
                throw new ArgumentNullException("error");
            }

            this.requestEvent = new MessageEntry<TMessage>(exit.ServiceRoute, exit);
            this.CompletionTime = exit.CompletionTime;
            this.EventError = error.CorrelateError(exit.Identifier, exit.RoutingRequest.Message);
            this.requestEvent.RoutingRequest.FailRequest(this.EventError);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageExit{TMessage}"/> class.
        /// </summary>
        /// <param name="routingRequest">
        /// The routing request.
        /// </param>
        internal MessageExit(MessageRoutingRequest<TMessage> routingRequest)
        {
            this.requestEvent = new MessageEntry<TMessage>(routingRequest);
            this.CompletionTime = DateTimeOffset.Now;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageExit{TMessage}"/> class. 
        /// Initializes a new instance of the <see cref="T:SAF.MessageQueuing.MessageExit`1"/> class.
        /// </summary>
        /// <param name="entry">
        /// The entry event.
        /// </param>
        /// <param name="error">
        /// The error associated with the process.
        /// </param>
        /// <param name="exitTime">
        /// The time that the request exited the queue.
        /// </param>
        private MessageExit(MessageEntry<TMessage> entry, Exception error, DateTimeOffset exitTime)
            : this(entry, exitTime)
        {
            if (entry == null)
            {
                throw new ArgumentNullException("entry");
            }

            if (error == null)
            {
                throw new ArgumentNullException("error");
            }

            this.EventError = error.CorrelateError(entry.Identifier, entry.RoutingRequest.Message);
            this.requestEvent.RoutingRequest.FailRequest(this.EventError);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageExit{TMessage}"/> class. 
        /// Initializes a new instance of the <see cref="T:SAF.MessageQueuing.MessageExit`1"/> class.
        /// </summary>
        /// <param name="entry">
        /// The entry event.
        /// </param>
        /// <param name="exitTime">
        /// The time that the request exited the queue.
        /// </param>
        private MessageExit(MessageEntry<TMessage> entry, DateTimeOffset exitTime)
        {
            if (entry == null)
            {
                throw new ArgumentNullException("entry");
            }

            this.requestEvent = entry;
            this.CompletionTime = exitTime;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the request associated with the event.
        /// </summary>
        public ActionRequest Request
        {
            get
            {
                return this.requestEvent == null ? ActionRequest.Empty : this.requestEvent.Request;
            }
        }

        /// <summary>
        /// Gets the globally unique ID for the current event.
        /// </summary>
        public Guid Identifier
        {
            get
            {
                return this.requestEvent == null ? Guid.Empty : this.requestEvent.Identifier;
            }
        }

        /// <summary>
        /// Gets the request message.
        /// </summary>
        public MessageRoutingRequest<TMessage> RoutingRequest
        {
            get
            {
                return this.requestEvent == null ? null : this.requestEvent.RoutingRequest;
            }
        }

        /// <summary>
        /// Gets the service route reporting the event.
        /// </summary>
        public IServiceRoute<TMessage> ServiceRoute
        {
            get
            {
                return this.requestEvent == null ? null : this.requestEvent.ServiceRoute;
            }
        }

        /// <summary>
        /// Gets the request time.
        /// </summary>
        public DateTimeOffset InitiationTime
        {
            get
            {
                return this.requestEvent == null ? DateTimeOffset.MinValue : this.requestEvent.InitiationTime;
            }
        }

        /// <summary>
        /// Gets the error, if any, associated with the queue exit event.
        /// </summary>
        public DomainException EventError { get; private set; }

        /// <summary>
        /// Gets the time the queue was exited.
        /// </summary>
        public DateTimeOffset CompletionTime { get; private set; }

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
        public static bool operator ==(MessageExit<TMessage> valueA, MessageExit<TMessage> valueB)
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
        public static bool operator !=(MessageExit<TMessage> valueA, MessageExit<TMessage> valueB)
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
        public static bool operator <(MessageExit<TMessage> valueA, MessageExit<TMessage> valueB)
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
        public static bool operator >(MessageExit<TMessage> valueA, MessageExit<TMessage> valueB)
        {
            return Evaluate.Compare(valueA, valueB) > 0;
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
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance is less than <paramref name="obj"/>. Zero This instance is equal to <paramref name="obj"/>. Greater than zero This instance is greater than <paramref name="obj"/>. 
        /// </returns>
        /// <param name="obj">
        /// An object to compare with this instance. 
        /// </param>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="obj"/> is not the same type as this instance. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public int CompareTo(object obj)
        {
            return Evaluate.Compare(this, obj);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">
        /// The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. 
        /// </param>
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
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        public bool Equals(MessageExit<TMessage> other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        public int CompareTo(MessageExit<TMessage> other)
        {
            return Evaluate.Compare(this, other, ComparisonProperties);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current request exit event.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current request exit event.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            var resultString = this.EventError != null ? String.Format(FailureFormat, "Failed", this.EventError.Message) : "Completed";

            return String.Format(ToStringFormat, this.requestEvent, this.CompletionTime, resultString);
        }

        /////// <summary>
        /////// Returns a list of validation errors for the current entity.
        /////// </summary>
        /////// <returns>
        /////// A list of validation errors for the current entity. If the list is empty, the entity is correctly formed.
        /////// </returns>
        ////public IEnumerable<string> Validate()
        ////{
        ////    return
        ////        this.CreateValidation()
        ////            .PropertyIsValid(exit => exit.requestEvent)
        ////            .Between(exit => exit.CompletionTime, exit => exit.InitiationTime, DateTimeOffset.MaxValue)
        ////            .Evaluate();
        ////}
    }
}