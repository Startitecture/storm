// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainExceptionHandler.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Traces domain exceptions to the trace handler. All other exceptions are not handled.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.ActionTracking
{
    using System;
    using System.Diagnostics;

    using JetBrains.Annotations;

    using SAF.Core;

    /// <summary>
    /// Traces domain exceptions to the trace handler. All other exceptions are not handled.
    /// </summary>
    public class DomainExceptionHandler : IExceptionHandler
    {
        /// <summary>
        /// The failed action.
        /// </summary>
        private const string FailedAction = "{0} {1} failed at {2:O} ({3}): {4}";

        /// <summary>
        /// The failed action on item.
        /// </summary>
        private const string FailedActionOnItem = "{0} {1} on {2} '{3}' failed at {4:O} ({5}): {6}";

        /// <summary>
        /// The default exception handler.
        /// </summary>
        private static readonly DomainExceptionHandler DefaultExceptionHandler = new DomainExceptionHandler();

        /// <summary>
        /// Prevents a default instance of the <see cref="DomainExceptionHandler"/> class from being created.
        /// </summary>
        private DomainExceptionHandler()
        {
        }

        /// <summary>
        /// Gets the default exception handler.
        /// </summary>
        public static DomainExceptionHandler Default
        {
            get
            {
                return DefaultExceptionHandler;
            }
        }

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
        public void HandleException([NotNull] Delegate action, object item, [NotNull] Exception exception)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            var domainException = exception as DomainException;

            string message;

            if (domainException != null)
            {
                if (item == null)
                {
                    message = string.Format(
                        FailedAction,
                        action.Method.DeclaringType,
                        action.Method.Name,
                        DateTimeOffset.Now,
                        exception.GetType().Name,
                        exception.Message);
                }
                else
                {
                    message = string.Format(
                        FailedActionOnItem,
                        action.Method.DeclaringType,
                        action.Method.Name,
                        item.GetType().Name,
                        item,
                        DateTimeOffset.Now,
                        exception.GetType().Name,
                        exception.Message);
                }
            }
            else
            {
                // Here we give the full stack trace as well.
                if (item == null)
                {
                    message = string.Format(
                        FailedAction,
                        action.Method.DeclaringType,
                        action.Method.Name,
                        DateTimeOffset.Now,
                        exception.GetType().Name,
                        exception);
                }
                else
                {
                    message = string.Format(
                        FailedActionOnItem,
                        action.Method.DeclaringType,
                        action.Method.Name,
                        item.GetType().Name,
                        item,
                        DateTimeOffset.Now,
                        exception.GetType().Name,
                        exception);
                }
            }

            Trace.TraceError(message);
        }

        /// <summary>
        /// Evaluates an exception and returns a value indicating whether the exception was handled.
        /// </summary>
        /// <param name="request">
        /// The request that caused the exception.
        /// </param>
        /// <param name="exception">
        /// The exception to evaluate.
        /// </param>
        public void HandleException(ActionRequest request, [NotNull] Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            var domainException = exception as DomainException;

            string message;

            if (domainException != null)
            {
                if (request.Item == null)
                {
                    message = string.Format(
                        FailedAction,
                        request.ActionSource,
                        request.ActionName,
                        DateTimeOffset.Now,
                        exception.GetType().Name,
                        exception.Message);
                }
                else
                {
                    message = string.Format(
                        FailedActionOnItem,
                        request.ActionSource,
                        request.ActionName,
                        request.ItemType,
                        request.Item,
                        DateTimeOffset.Now,
                        exception.GetType().Name,
                        exception.Message);
                }
            }
            else
            {
                // Here we give the full stack trace as well.
                if (request.Item == null)
                {
                    message = string.Format(
                        FailedAction,
                        request.ActionSource,
                        request.ActionName,
                        DateTimeOffset.Now,
                        exception.GetType().Name,
                        exception);
                }
                else
                {
                    message = string.Format(
                        FailedActionOnItem,
                        request.ActionSource,
                        request.ActionName,
                        request.ItemType,
                        request.Item,
                        DateTimeOffset.Now,
                        exception.GetType().Name,
                        exception);
                }
            }

            Trace.TraceError(message);
        }
    }
}
