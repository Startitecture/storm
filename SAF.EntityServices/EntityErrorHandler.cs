// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityErrorHandler.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.EntityServices
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;

    using SAF.ActionTracking;
    using SAF.Core;
    using SAF.Security;
    using SAF.StringResources;

    /// <summary>
    /// Provides error handling capability for unhandled exceptions.
    /// </summary>
    public class EntityErrorHandler : IErrorHandler
    {
        /// <summary>
        /// The fault code format.
        /// </summary>
        private const string FaultCodeFormat = "Unhandled{0}";

        /// <summary>
        /// The service security context.
        /// </summary>
        private static readonly ApplicationOperationContext ServiceOperationContext =
            new ApplicationOperationContext(
                new WindowsAccountInfoProvider(), 
                typeof(EntityProxy<>), 
                typeof(EntityErrorHandler), 
                typeof(ExtensionMethods));

        /// <summary>
        /// The action event proxy.
        /// </summary>
        private readonly ActionEventProxy actionEventProxy;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityErrorHandler"/> class.
        /// </summary>
        public EntityErrorHandler()
            : this(ServiceOperationContext, ConfiguredEventRepositoryFactory.Default, ServiceErrorMapping.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityErrorHandler"/> class.
        /// </summary>
        /// <param name="actionContext">
        /// The action context.
        /// </param>
        /// <param name="repositoryFactory">
        /// The repository provider.
        /// </param>
        /// <param name="errorMapping">
        /// The error Mapping.
        /// </param>
        public EntityErrorHandler(IActionContext actionContext, IEventRepositoryFactory repositoryFactory, IErrorMapping errorMapping)
        {
            this.actionEventProxy = new ActionEventProxy(actionContext, ServiceExceptionHandler.Default, errorMapping, repositoryFactory, false);
        }

        /// <summary>
        /// Enables the creation of a custom <see cref="T:System.ServiceModel.FaultException`1"/> that is returned from an exception 
        /// in the course of a service method.
        /// </summary>
        /// <param name="error">
        /// The <see cref="T:System.Exception"/> object thrown in the course of the service operation.
        /// </param>
        /// <param name="version">
        /// The SOAP version of the message.
        /// </param>
        /// <param name="fault">
        /// The <see cref="T:System.ServiceModel.Channels.Message"/> object that is returned to the client, or service, in the duplex
        /// case.
        /// </param>
        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            if (error == null)
            {
                throw new ArgumentNullException("error");
            }

            if (error is FaultException)
            {
                return;
            }

            string faultCode = String.Format(FaultCodeFormat, error.GetType().Name);
            var faultException = error.ToFaultException<InternalOperationFault>(faultCode);
            var faultMessage = faultException.CreateMessageFault();
            fault = Message.CreateMessage(version, faultMessage, faultException.Action);
        }

        /// <summary>
        /// Enables error-related processing and returns a value that indicates whether the dispatcher aborts the session and the 
        /// instance context in certain cases. 
        /// </summary>
        /// <param name="error">
        /// The exception thrown during processing.
        /// </param>
        /// <returns>
        /// true if  should not abort the session (if there is one) and instance context if the instance context is not 
        /// <see cref="F:System.ServiceModel.InstanceContextMode.Single"/>; otherwise, false. The default is false.
        /// </returns>
        [SuppressMessage(
            "Microsoft.Design", 
            "CA1031:DoNotCatchGeneralExceptionTypes", 
            Justification = "This method implementation is not designed for throwing exceptions as it would hide the original exception.")]
        public bool HandleError(Exception error)
        {
            if (error == null)
            {
                throw new ArgumentNullException("error");
            }

            if (error is FaultException)
            {
                // Assume fault exceptions were already handled.
                return true;
            }

            try
            {
                // Create and fail a custom action.
                ////var currentUser = this.actionContext.CurrentAccount;
                ////var actionRequest = ActionRequest.Create();
                ////var actionEvent = ActionEvent.StartAction(actionRequest, currentUser);
                ////ErrorInfo mapErrorInfo = this.errorMapping.MapErrorInfo(actionRequest.ActionName, error);
                ////actionEvent.CompleteAction(mapErrorInfo);
                ////var repository = this.repositoryFactory.Create();
                ////repository.Save(actionEvent);
                this.actionEventProxy.RecordAction(OperationContext.Current, error);
                return error is DomainException;
            }
            catch (Exception ex)
            {
                // Avoid throwing exceptions here so we don't get into a loop.
                Trace.TraceError(ErrorMessages.FaultRepositoryFailureOnSave, error.Message, ex);
                return false;
            }
        }
    }
}
