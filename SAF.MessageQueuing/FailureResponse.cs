// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FailureResponse.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Contains instructions for responding to a route failure.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;

    using SAF.Core;

    /// <summary>
    /// Contains instructions for responding to a route failure.
    /// </summary>
    public struct FailureResponse : IEquatable<FailureResponse>
    {
        /// <summary>
        /// The response that sends the request to the failure route.
        /// </summary>
        public static readonly FailureResponse SendToFailureRoute = new FailureResponse();

        /// <summary>
        /// The retry format.
        /// </summary>
        private const string RetryFormat = "Retry after {0}";

        /// <summary>
        /// The fail message.
        /// </summary>
        private const string FailMessage = "Fail delivery";

        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<FailureResponse, object>[] ComparisonProperties = { item => item.Retry, item => item.WaitTime };

        /// <summary>
        /// Initializes a new instance of the <see cref="FailureResponse"/> struct.
        /// </summary>
        /// <param name="waitTime">
        /// The wait time.
        /// </param>
        public FailureResponse(TimeSpan waitTime)
            : this(waitTime, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FailureResponse"/> struct.
        /// </summary>
        /// <param name="waitTime">
        /// The wait time.
        /// </param>
        /// <param name="blockQueue">
        /// A value indicating whether to block the current queue.
        /// </param>
        public FailureResponse(TimeSpan waitTime, bool blockQueue)
            : this()
        {
            this.Retry = true;
            this.WaitTime = waitTime;
            this.BlockQueue = blockQueue;
        }

        /// <summary>
        /// Gets a value indicating whether to retry the action.
        /// </summary>
        public bool Retry { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the retry attempt should block the queue.
        /// </summary>
        public bool BlockQueue { get; private set; }

        /// <summary>
        /// Gets the amount of time to wait before retrying the action.
        /// </summary>
        public TimeSpan WaitTime { get; private set; }

        /// <summary>
        /// Determines whether two values of the same type are equal.
        /// </summary>
        /// <param name="valueA">
        /// The baseline value.
        /// </param>
        /// <param name="valueB">
        /// The comparison value.
        /// </param>
        /// <returns>
        /// <c>true</c> if the two values are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(FailureResponse valueA, FailureResponse valueB)
        {
            return valueA.Equals(valueB);
        }

        /// <summary>
        /// Determines whether two values of the same type are not equal.
        /// </summary>
        /// <param name="valueA">
        /// The baseline value.
        /// </param>
        /// <param name="valueB">
        /// The comparison value.
        /// </param>
        /// <returns>
        /// <c>true</c> if the two values are not equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(FailureResponse valueA, FailureResponse valueB)
        {
            return !(valueA == valueB);
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> containing a fully qualified type name.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return this.Retry ? String.Format(RetryFormat, this.WaitTime) : FailMessage;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return Evaluate.GenerateHashCode(this, ComparisonProperties);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">Another object to compare to. </param><filterpriority>2</filterpriority>
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
        public bool Equals(FailureResponse other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }
    }
}
