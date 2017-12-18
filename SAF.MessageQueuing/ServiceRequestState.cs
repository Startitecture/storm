// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceRequestState.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Contains all the states of a service request.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    /// <summary>
    /// Contains all the states of a service request.
    /// </summary>
    /// <remarks>
    /// Enumerated values are ordered from least to most available to simplify comparison.
    /// </remarks>
    public enum ServiceRequestState
    {
        /// <summary>
        /// Indicates that the request has not yet been sent.
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Indicates that the request has been completed.
        /// </summary>
        Completed = 1,

        /// <summary>
        /// Indicates that the request is in progress.
        /// </summary>
        InProgress = 2
    }
}
