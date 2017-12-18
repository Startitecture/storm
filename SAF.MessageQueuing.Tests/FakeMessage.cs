// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeMessage.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Fake request for testing.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing.Tests
{
    using System;

    using SAF.Core;

    /// <summary>
    /// Fake request for testing.
    /// </summary>
    public class FakeMessage : TimedPriorityMessage, IComparable<FakeMessage>, IEquatable<FakeMessage>, IComparable
    {
        #region Fields

        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<FakeMessage, object>[] ComparisonProperties =
            {
                request => request.Name,
                request => request.RequestGuid,
                request => request.RequestTime,
                request => request.Deadline,
                request => request.EscalationThreshold
            };

        /// <summary>
        /// The hash code.
        /// </summary>
        private readonly Lazy<int> hashCode; 

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeMessage"/> class.
        /// </summary>
        /// <param name="requestTime">
        /// The time of this request.
        /// </param>
        /// <param name="deadline">
        /// The deadline for the request to be fulfilled.
        /// </param>
        /// <param name="escalationThreshold">
        /// The escalation threshold.
        /// </param>
        public FakeMessage(DateTimeOffset requestTime, DateTimeOffset deadline, TimeSpan escalationThreshold)
            : base(Guid.NewGuid(), requestTime, deadline, escalationThreshold)
        {
            this.RequestLatency = new TimeSpan(TimeSpan.TicksPerMillisecond);
            this.hashCode = new Lazy<int>(this.CreateHashCode); 
        }

        #region Public Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the request delay.
        /// </summary>
        public TimeSpan RequestLatency { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to fail the request.
        /// </summary>
        public bool RequestShouldFail { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether response should fail with an unhandled exception.
        /// </summary>
        public bool ResponseShouldFailUnhandled { get; set; }

        #endregion

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance is less than <paramref name="obj"/>. Zero This instance is equal to <paramref name="obj"/>. Greater than zero This instance is greater than <paramref name="obj"/>. 
        /// </returns>
        /// <param name="obj">An object to compare with this instance. </param><exception cref="T:System.ArgumentException"><paramref name="obj"/> is not the same type as this instance. </exception><filterpriority>2</filterpriority>
        public virtual int CompareTo(object obj)
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
        public int CompareTo(FakeMessage other)
        {
            return Evaluate.Compare(this, other, ComparisonProperties);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(FakeMessage other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return this.Name;
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="FakeMessage"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return this.hashCode.Value; //// Evaluate.GenerateHashCode(this, ComparisonProperties);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="FakeMessage"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="FakeMessage"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="FakeMessage"/>. </param>
        /// <filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            return Evaluate.Equals(this, obj);
        }

        /// <summary>
        /// Creates the hash code for this object.
        /// </summary>
        /// <returns>
        /// The hash code as an <see cref="int"/> value.
        /// </returns>
        private int CreateHashCode()
        {
            return Evaluate.GenerateHashCode(this, ComparisonProperties);
        }
    }
}