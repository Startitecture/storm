// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceExceptionHandler.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Handles service exceptions by throwing a new <see cref="FaultException" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.EntityServices
{
    using System;
    using System.ServiceModel;

    using SAF.ActionTracking;
    using SAF.Core;
    using SAF.Data;

    /// <summary>
    /// Handles service exceptions by throwing a new <see cref="FaultException"/>.
    /// </summary>
    public class ServiceExceptionHandler : IExceptionHandler
    {
        /// <summary>
        /// The unhandled exception fault code.
        /// </summary>
        private const string UnhandledExceptionFaultCode = "UnhandledException";

        /// <summary>
        /// The default service exception handler.
        /// </summary>
        private static readonly ServiceExceptionHandler DefaultServiceExceptionHandler = new ServiceExceptionHandler();

        /// <summary>
        /// Prevents a default instance of the <see cref="ServiceExceptionHandler"/> class from being created.
        /// </summary>
        private ServiceExceptionHandler()
        {
        }

        /// <summary>
        /// Gets the default service exception handler.
        /// </summary>
        public static ServiceExceptionHandler Default
        {
            get
            {
                return DefaultServiceExceptionHandler;
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
        public void HandleException(Delegate action, object item, Exception exception)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }

            this.HandleException(ActionRequest.Create(item, action), exception);
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
        public void HandleException(ActionRequest request, Exception exception)
        {
            if (OperationContext.Current == null)
            {
                return;
            }

            if (exception is RepositoryException)
            {
                throw request.CreateRepositoryException(exception, request.ItemType);
            }

            var businessException = exception as BusinessException;

            if (businessException != null)
            {
                throw request.CreateValidationException(businessException);
            }

            var configurationException = exception as ApplicationConfigurationException;

            if (configurationException != null)
            {
                throw request.CreateConfigurationException(configurationException);
            }

            var faultException = exception as FaultException<EntityValidationFault>;

            if (faultException != null)
            {
                throw new FaultException<EntityValidationFault>(
                    faultException.Detail,
                    faultException.Reason,
                    faultException.Code,
                    faultException.Action);
            }

            var baseFaultException = exception as FaultException;

            if (baseFaultException != null)
            {
                throw request.CreateException<InternalOperationFault>(
                    baseFaultException, baseFaultException.GetFaultCode(), baseFaultException.Reason.ToString());
            }

            throw request.CreateException<InternalOperationFault>(exception, UnhandledExceptionFaultCode);
        }
    }
}
