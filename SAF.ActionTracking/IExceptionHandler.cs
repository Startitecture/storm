// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExceptionHandler.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface for exception handling.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActionTracking
{
    using System;

    /// <summary>
    /// Provides an interface for exception handling.
    /// </summary>
    public interface IExceptionHandler
    {
        /// <summary>
        /// Evaluates an exception and returns a value indicating whether the exception was handled.
        /// </summary>
        /// <param name="action">
        /// The action that was taken.
        /// </param>
        /// <param name="item">
        /// The item associated with the action.
        /// </param>
        /// <param name="exception">
        /// The exception to evaluate.
        /// </param>
        void HandleException(Delegate action, object item, Exception exception);

        /// <summary>
        /// Evaluates an exception and returns a value indicating whether the exception was handled.
        /// </summary>
        /// <param name="request">
        /// The request that caused the exception.
        /// </param>
        /// <param name="exception">
        /// The exception to evaluate.
        /// </param>
        void HandleException(ActionRequest request, Exception exception);
    }
}
