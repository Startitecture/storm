// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceRequestStatus.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Maintains the state of a service request.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;
    using System.Collections.Generic;

    using SAF.Core;

    /// <summary>
    /// Maintains the state of a service request.
    /// </summary>
    public class ServiceRequestStatus : IEquatable<ServiceRequestStatus>, IComparable, IComparable<ServiceRequestStatus>
    {
        /// <summary>
        /// Represents an empty service request status.
        /// </summary>
        public static readonly ServiceRequestStatus Empty = new ServiceRequestStatus();

        #region Constants

        /// <summary>
        /// The available to string format.
        /// </summary>
        private const string AvailableToStringFormat = "Available since: {0}";

        /// <summary>
        /// The in progress to string format.
        /// </summary>
        private const string InProgressToStringFormat = "In progress: {0}";

        #endregion

        #region Static Fields

        /// <summary>
        /// The equality properties.
        /// </summary>
        private static readonly Func<ServiceRequestStatus, object>[] ComparisonProperties = 
            new Func<ServiceRequestStatus, object>[]
            {
                item => item.RequestTime, 
                item => item.ResponseTime,
                item => item.RequestState,
                item => item.RequestId
            };

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the request ID for the current request.
        /// </summary>
        public Guid RequestId { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the current request is in progress.
        /// </summary>
        public ServiceRequestState RequestState { get; private set; }

        /// <summary>
        /// Gets the request time.
        /// </summary>
        public DateTimeOffset RequestTime { get; private set; }

        /// <summary>
        /// Gets the response time.
        /// </summary>
        public DateTimeOffset ResponseTime { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Determines if two values of the same type are equal.
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
        public static bool operator ==(ServiceRequestStatus valueA, ServiceRequestStatus valueB)
        {
            return Evaluate.Equals(valueA, valueB);
        }

        /// <summary>
        /// Determines if two values of the same type are not equal.
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
        public static bool operator !=(ServiceRequestStatus valueA, ServiceRequestStatus valueB)
        {
            return !(valueA == valueB);
        }

        /// <summary>
        /// Determines if the first value is less than the second value.
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
        public static bool operator <(ServiceRequestStatus valueA, ServiceRequestStatus valueB)
        {
            return Comparer<ServiceRequestStatus>.Default.Compare(valueA, valueB) < 0;
        }

        /// <summary>
        /// Determines if the first value is greater than the second value.
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
        public static bool operator >(ServiceRequestStatus valueA, ServiceRequestStatus valueB)
        {
            return Comparer<ServiceRequestStatus>.Default.Compare(valueA, valueB) > 0;
        }

        /// <summary>
        /// Records the time when a new request is sent.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The state of the current request is already <see cref="ServiceRequestState.InProgress"/>.
        /// </exception>
        public void Send()
        {
            if (this.RequestState == ServiceRequestState.InProgress)
            {
                throw new InvalidOperationException("The request is already in progress.");
            }

            if (this.RequestId == Guid.Empty)
            {
                this.RequestId = Guid.NewGuid();
            }

            this.RequestTime = DateTimeOffset.Now;
            this.RequestState = ServiceRequestState.InProgress;
        }

        /// <summary>
        /// Records the time when the response is received.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The state of the current request is not <see cref="ServiceRequestState.InProgress"/>.
        /// </exception>
        public void AcceptResponse()
        {
            if (this.RequestState != ServiceRequestState.InProgress)
            {
                throw new InvalidOperationException("The request is already completed.");
            }

            this.ResponseTime = DateTimeOffset.Now;
            this.RequestState = ServiceRequestState.Completed;
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the 
        /// current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings: 
        /// Value Meaning Less than zero This instance is less than <paramref name="obj"/>.
        /// Zero This instance is equal to <paramref name="obj"/>. 
        /// Greater than zero This instance is greater than <paramref name="obj"/>. 
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
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: 
        /// Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.
        /// Zero This object is equal to <paramref name="other"/>. 
        /// Greater than zero This object is greater than <paramref name="other"/>.
        /// </returns>
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        public int CompareTo(ServiceRequestStatus other)
        {
            return Evaluate.Compare(this, other, ComparisonProperties);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">
        /// Another object to compare to. 
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
        public bool Equals(ServiceRequestStatus other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
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
        /// Returns a description of this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> containing a description of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return this.RequestState == ServiceRequestState.InProgress
                       ? String.Format(InProgressToStringFormat, this.RequestTime)
                       : String.Format(AvailableToStringFormat, this.ResponseTime);
        }

        #endregion
    }
}