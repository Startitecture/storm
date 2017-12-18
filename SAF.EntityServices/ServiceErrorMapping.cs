// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceErrorMapping.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.EntityServices
{
    using System;
    using System.Linq;
    using System.ServiceModel;

    using SAF.ActionTracking;
    using SAF.Core;

    /// <summary>
    /// Contains static methods for parsing exception information.
    /// </summary>
    public class ServiceErrorMapping : IErrorMapping
    {
        /// <summary>
        /// The default mapping.
        /// </summary>
        private static readonly ServiceErrorMapping DefaultMapping = new ServiceErrorMapping();

        /// <summary>
        /// Prevents a default instance of the <see cref="ServiceErrorMapping"/> class from being created.
        /// </summary>
        private ServiceErrorMapping()
        {
        }

        /// <summary>
        /// Gets the default service error mapping.
        /// </summary>
        public static ServiceErrorMapping Default
        {
            get
            {
                return DefaultMapping;
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
        public ErrorInfo MapErrorInfo(string action, Exception actionError)
        {
            ErrorInfo errorInfo;
            
            var faultException = actionError as FaultException;

            if (faultException == null)
            {
                errorInfo = ErrorMapping.Default.MapErrorInfo(action, actionError);
            }
            else
            {
                var errorMessage = faultException.Message;
                var errorOutput = faultException.ToString();
                var errorCode = faultException.IsActionFault()
                                    ? faultException.GetActionFault().ErrorCode
                                    : faultException.GetFaultCode();
                var actionFault = faultException.GetActionFault();

                // Not to be confused with the additional data for the action.
                // TODO: Fix the property collision with ActionRequest.AdditionalData.
                var errorData = actionFault.ErrorData ?? actionFault.AdditionalData;

                // We know that FaultException has one generic argument if it has any at all, which is the fault that we are 
                // really interested in.
                var errorType = actionError.GetType().GetGenericArguments().Any()
                                    ? actionError.GetType().GetGenericArguments()[0].Name
                                    : faultException.GetType().ToRuntimeName();

                errorInfo = new ErrorInfo(errorCode, errorType, errorMessage, errorData, errorOutput);
            }

            return errorInfo;
        }
    }
}
