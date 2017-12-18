// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IActionFault.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface to faults associated with user actions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.EntityServices
{
    using System;

    /// <summary>
    /// Provides an interface to faults associated with user actions.
    /// </summary>
    public interface IActionFault
    {
        /// <summary>
        /// Gets the global identifier of the action that resulted in this specific fault.
        /// </summary>
        Guid ActionIdentifier { get; }

        /// <summary>
        /// Gets the time the fault occurred.
        /// </summary>
        DateTimeOffset FaultTime { get; }

        /// <summary>
        /// Gets the action source.
        /// </summary>
        string ActionSource { get; }

        /// <summary>
        /// Gets the action name.
        /// </summary>
        string Action { get; }

        /// <summary>
        /// Gets the entity type.
        /// </summary>
        string EntityType { get; }

        /// <summary>
        /// Gets the target entity.
        /// </summary>
        string TargetEntity { get; }

        /// <summary>
        /// Gets the additional data associated with the current fault.
        /// </summary>
        string AdditionalData { get; }

        /// <summary>
        /// Gets the reason for the fault.
        /// </summary>
        string Reason { get; }

        /// <summary>
        /// Gets the error type.
        /// </summary>
        string ErrorType { get; }

        /// <summary>
        /// Gets the error data.
        /// </summary>
        string ErrorData { get; }

        /// <summary>
        /// Gets the error code.
        /// </summary>
        string ErrorCode { get; }
    }
}