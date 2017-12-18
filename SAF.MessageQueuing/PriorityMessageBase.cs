// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PriorityMessageBase.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The base class for priority requests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;
    using System.Linq.Expressions;

    using SAF.Core;

    /// <summary>
    /// The base class for priority requests.
    /// </summary>
    public abstract class PriorityMessageBase : IPriorityMessage
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Expression<Func<PriorityMessageBase, object>>[] ComparisonProperties =
            {
                item => item.Deadline,
                item => item.RequestTime,
                item => item.EscalationThreshold,
                item => item.RequestGuid
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityMessageBase"/> class.
        /// </summary>
        /// <param name="requestIdentifier">
        /// The globally unique request identifier.
        /// </param>
        /// <param name="requestTime">
        /// The request time.
        /// </param>
        /// <param name="deadline">
        /// The deadline for delivery of the request.
        /// </param>
        /// <param name="escalationThreshold">
        /// The amount of time prior to the deadline when the request should be escalated relative to other requests.
        /// </param>
        protected PriorityMessageBase(
            Guid requestIdentifier, DateTimeOffset requestTime, DateTimeOffset deadline, TimeSpan escalationThreshold)
        {
            this.EscalationThreshold = escalationThreshold;
            this.Deadline = deadline;
            this.RequestTime = requestTime;
            this.RequestGuid = requestIdentifier;
        }

        /// <summary>
        /// Gets the globally unique ID for the request.
        /// </summary>
        public Guid RequestGuid { get; private set; }

        /// <summary>
        /// Gets the request time.
        /// </summary>
        public DateTimeOffset RequestTime { get; private set; }

        /// <summary>
        /// Gets the processing deadline for this request.
        /// </summary>
        public DateTimeOffset Deadline { get; private set; }

        /// <summary>
        /// Gets the amount of time relative to the deadline when the request's priority should be escalated.
        /// </summary>
        public TimeSpan EscalationThreshold { get; private set; }

        /// <summary>
        /// Gets the error, if any, that caused the request to fail.
        /// </summary>
        public DomainException RequestError { get; private set; }

        /// <summary>
        /// Determines whether two objects of the same type are equal.
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
        public static bool operator ==(PriorityMessageBase valueA, PriorityMessageBase valueB)
        {
            return Evaluate.Equals(valueA, valueB);
        }

        /// <summary>
        /// Determines whether two objects of the same type are not equal.
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
        public static bool operator !=(PriorityMessageBase valueA, PriorityMessageBase valueB)
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
        public static bool operator <(PriorityMessageBase valueA, PriorityMessageBase valueB)
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
        public static bool operator >(PriorityMessageBase valueA, PriorityMessageBase valueB)
        {
            return Evaluate.Compare(valueA, valueB) > 0;
        }

        /// <summary>
        /// Fails the request with the error that prevented the request from being completed.
        /// </summary>
        /// <param name="requestError">
        /// The exception that prevented the request from being completed.
        /// </param>
        public void FailRequest(DomainException requestError)
        {
            this.RequestError = requestError;
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="PriorityMessageBase"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return Evaluate.GenerateHashCode(this, ComparisonProperties);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="PriorityMessageBase"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="PriorityMessageBase"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="PriorityMessageBase"/>. </param>
        /// <filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            return Evaluate.Equals(this, obj);
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance is less than <paramref name="obj"/>. Zero This instance is equal to <paramref name="obj"/>. Greater than zero This instance is greater than <paramref name="obj"/>. 
        /// </returns>
        /// <param name="obj">An object to compare with this instance. </param><exception cref="T:System.ArgumentException"><paramref name="obj"/> is not the same type as this instance. </exception><filterpriority>2</filterpriority>
        public abstract int CompareTo(object obj);
    }
}