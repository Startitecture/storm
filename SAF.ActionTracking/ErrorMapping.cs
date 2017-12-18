// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorMapping.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.ActionTracking
{
    using System;

    using SAF.Core;

    /// <summary>
    /// Provides a mapping between exceptions and normalized error data.
    /// </summary>
    public class ErrorMapping : IErrorMapping
    {
        /// <summary>
        /// The error code format.
        /// </summary>
        private const string ErrorCodeFormat = "{0}-{1}";

        /// <summary>
        /// The default error mapping.
        /// </summary>
        private static readonly ErrorMapping DefaultErrorMapping = new ErrorMapping();

        /// <summary>
        /// Prevents a default instance of the <see cref="ErrorMapping"/> class from being created.
        /// </summary>
        private ErrorMapping()
        {
        }

        /// <summary>
        /// Gets the default error mapping.
        /// </summary>
        public static ErrorMapping Default
        {
            get
            {
                return DefaultErrorMapping;
            }
        }

        /// <summary>
        /// Maps an exception to an <see cref="ErrorInfo"/> value.
        /// </summary>
        /// <param name="action">
        /// The action that caused the error.
        /// </param>
        /// <param name="actionError">
        /// The error to map.
        /// </param>
        /// <returns>
        /// An <see cref="ErrorInfo"/> value for the specified error.
        /// </returns>
        public virtual ErrorInfo MapErrorInfo(string action, Exception actionError)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            if (actionError == null)
            {
                throw new ArgumentNullException("actionError");
            }

            var errorCode = String.Format(ErrorCodeFormat, action, actionError.GetType().ToRuntimeName());
            var errorData = actionError.Data.Count == 0 ? null : actionError.Data.ToNameValueString();
            var errorMessage = actionError.Message;
            var errorType = actionError.GetType().ToRuntimeName();
            var fullErrorOutput = Convert.ToString(actionError);
            return new ErrorInfo(errorCode, errorType, errorMessage, errorData, fullErrorOutput);
        }
    }
}
