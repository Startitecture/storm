// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceUsageComparer.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Compares  values for availability, with higher availability preceding lower availability.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System.Collections.Generic;

    /// <summary>
    /// Compares <see cref="ServiceRequestStatus"/> values for availability, with higher availability preceding lower availability.
    /// </summary>
    /// <remarks>
    /// This comparison is not thread safe. To ensure that the <see cref="ServiceRequestStatus"/> values do not change during the 
    /// comparison, synchronize the comparison operation with any other operations that could change the values.
    /// </remarks>
    public class ServiceUsageComparer : Comparer<IServiceAvailabilityMetric>
    {
        /// <summary>
        /// The availability comparer.
        /// </summary>
        private static readonly ServiceUsageComparer UsageComparer = new ServiceUsageComparer();

        /// <summary>
        /// Gets the availability comparer.
        /// </summary>
        public static ServiceUsageComparer Usage
        {
            get
            {
                return UsageComparer;
            }
        }

        /// <summary>
        /// Performs a comparison of two <see cref="ServiceRequestStatus"/> values and returns a value indicating whether one value is
        /// less than, equal to, or greater than the other in terms of time to next availability.
        /// </summary>
        /// <returns>
        /// A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>.
        /// Less than zero: Time to availability for <paramref name="x"/> is less than <paramref name="y"/>.
        /// Zero: Time to availability for <paramref name="x"/> equals <paramref name="y"/>.
        /// Greater than zero: Time to availability for <paramref name="x"/> is greater than <paramref name="y"/>.
        /// </returns>
        /// <param name="x">
        /// The first object to compare.
        /// </param>
        /// <param name="y">
        /// The second object to compare.
        /// </param>
        public override int Compare(IServiceAvailabilityMetric x, IServiceAvailabilityMetric y)
        {
            if (x == null && y == null)
            {
                return 0;
            }

            if (x == null)
            {
                return -1;
            }

            if (y == null)
            {
                return 1;
            }

            // Available services preceded unvailable services.
            var requestStateComparison = x.RequestState.CompareTo(y.RequestState);

            if (requestStateComparison != 0)
            {
                return requestStateComparison;
            }

            // If both are in progress, the request time takes precedence - the older the request, the more likely it is statistically
            // to complete first. If both are idle, then the service that has been idle the longest takes precedence.
            if (x.RequestState == ServiceRequestState.InProgress)
            {
                return x.LastRequest.CompareTo(y.LastRequest);
            }

            return x.LastResponse.CompareTo(y.LastResponse);
        }
    }
}
