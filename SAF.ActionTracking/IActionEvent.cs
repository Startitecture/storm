// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IActionEvent.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The ActionEvent interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActionTracking
{
    using System;

    using SAF.Core;

    /// <summary>
    /// Provides an interface to events related to actions in the system.
    /// </summary>
    public interface IActionEvent
    {
        /// <summary>
        /// Gets the sequence number for the event.
        /// </summary>
        long? SequenceNumber { get; }

        /// <summary>
        /// Gets the action request.
        /// </summary>
        ActionRequest Request { get; }

        /// <summary>
        /// Gets the user account that initiated the request.
        /// </summary>
        string UserAccountName { get; }

        /// <summary>
        /// Gets the user display name.
        /// </summary>
        string UserDisplayName { get; }

        /// <summary>
        /// Gets the initiation time of the event.
        /// </summary>
        DateTimeOffset InitiationTime { get; }

        /// <summary>
        /// Gets the completion time of the event.
        /// </summary>
        DateTimeOffset CompletionTime { get; }

        /// <summary>
        /// Gets the exception, if any, associated with the event.
        /// </summary>
        string ErrorCode { get; }

        /// <summary>
        /// Gets the error type.
        /// </summary>
        string ErrorType { get; }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        string ErrorMessage { get; }

        /// <summary>
        /// Gets the stack trace.
        /// </summary>
        string FullErrorOutput { get; }

        /// <summary>
        /// Gets the error data.
        /// </summary>
        string ErrorData { get; }

        /// <summary>
        /// Completes the action.
        /// </summary>
        void Complete();

        /// <summary>
        /// Completes the action with an error.
        /// </summary>
        /// <param name="exception">
        /// The error associated with the action.
        /// </param>
        void Complete(Exception exception);
    }
}