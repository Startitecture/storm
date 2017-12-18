// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AvailabilityComparison.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    /// <summary>
    /// An enumeration of availability comparison approaches.
    /// </summary>
    public enum AvailabilityComparison
    {
        /// <summary>
        /// Availability is compared by the total weight of all resources, regardless of rank. If ranks are not equivalent when using
        /// <see cref="RankedWeight"/>, this method will be used instead.
        /// </summary>
        TotalWeight = 0, 

        /// <summary>
        /// Availability is compared within the resource rank, and the first non-equal comparison determines the result.
        /// </summary>
        RankedWeight = 1, 

        /// <summary>
        /// Availability is compared by multiplying weights by rank.
        /// </summary>
        RankedTotalWeight = 2, 
    }
}