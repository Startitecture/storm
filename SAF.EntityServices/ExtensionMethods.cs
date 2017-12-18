// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.EntityServices
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.ServiceModel;

    using SAF.ActionTracking;
    using SAF.Core;

    /// <summary>
    /// Extension methods for entity services.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// The fault exception format.
        /// </summary>
        private const string FaultExceptionFormat = "({0}) Action '{1}' failed: {2}";

        /// <summary>
        /// The fault exception detail property name.
        /// </summary>
        private const string FaultExceptionDetailPropertyName = "Detail";

        /// <summary>
        /// The no reason specified message.
        /// </summary>
        private const string NoReasonSpecified = "No reason specified.";

        /// <summary>
        /// The unspecified fault code.
        /// </summary>
        private const string UnspecifiedFaultCode = "NotSpecified";

        /// <summary>
        /// The database mapping fault code.
        /// </summary>
        private const string EntityMappingFaultCode = "EntityMapping";

        /// <summary>
        /// The removal validation fault code.
        /// </summary>
        private const string EntityValidationFaultCode = "EntityValidation";

        /// <summary>
        /// The entity persistence fault code.
        /// </summary>
        private const string EntityPersistenceFaultCode = "EntityPersistence";

        /// <summary>
        /// The action fault format.
        /// </summary>
        private const string ActionFaultFormat = "{0}-{1}";

        /// <summary>
        /// The fault namespace.
        /// </summary>
        private const string FaultNamespace = "SAF.EntityServices";

        #region Create Faults for Exceptions

        /// <summary>
        /// Creates a fault of the specified type based on an action event.
        /// </summary>
        /// <param name="error">
        /// The exception to create the fault for.
        /// </param>
        /// <typeparam name="TFault">
        /// The type of fault to create.
        /// </typeparam>
        /// <returns>
        /// A fault of type <see cref="TFault"/>.
        /// </returns>
        public static TFault CreateFault<TFault>(this Exception error) where TFault : FaultBase, new()
        {
            if (error == null)
            {
                throw new ArgumentNullException("error");
            }

            // TODO: If this is already a fault exception, or contains one, get the original codes, etc.
            return CreateFault<TFault>(ActionRequest.Create(EntityOperationContext.Current), error, UnspecifiedFaultCode, error.Message);
        }

        #endregion

        #region Create Faults For Action Requests

        /// <summary>
        /// Creates a repository fault exception.
        /// </summary>
        /// <param name="request">
        /// The action request associated with the exception.
        /// </param>
        /// <param name="error">
        /// The exception that was thrown.
        /// </param>
        /// <param name="repositoryIdentifier">
        /// The repository identifier.
        /// </param>
        /// <returns>
        /// A <see cref="FaultException"/> for the specified error and fault code.
        /// </returns>
        public static FaultException<EntityRepositoryFault> CreateRepositoryException(
            this ActionRequest request, 
            Exception error, 
            string repositoryIdentifier)
        {
            if (error == null)
            {
                throw new ArgumentNullException("error");
            }

            if (repositoryIdentifier == null)
            {
                throw new ArgumentNullException("repositoryIdentifier");
            }

            var fault = request.CreateFault<EntityRepositoryFault>(error, EntityPersistenceFaultCode);
            fault.RepositoryIdentifier = repositoryIdentifier;

            return new FaultException<EntityRepositoryFault>(
                fault, error.Message, FaultCode.CreateReceiverFaultCode(fault.ErrorCode, FaultNamespace));
        }

        /// <summary>
        /// Creates application configuration exceptions.
        /// </summary>
        /// <param name="request">
        /// The request associated with the exception.
        /// </param>
        /// <param name="ex">
        /// The configuration exception that was thrown.
        /// </param>
        /// <returns>
        /// A <see cref="FaultException"/> for the specified exception.
        /// </returns>
        public static FaultException<ApplicationConfigurationFault> CreateConfigurationException(
            this ActionRequest request, ApplicationConfigurationException ex)
        {
            if (ex == null)
            {
                throw new ArgumentNullException("ex");
            }

            var fault = request.CreateFault<ApplicationConfigurationFault>(ex, EntityMappingFaultCode);
            fault.ConfigurationArea = ex.ConfigurationArea;

            return new FaultException<ApplicationConfigurationFault>(
                fault, fault.Reason, FaultCode.CreateReceiverFaultCode(fault.ErrorCode, FaultNamespace));
        }

        /// <summary>
        /// Creates entity validation exceptions.
        /// </summary>
        /// <param name="request">
        /// The request associated with the exception.
        /// </param>
        /// <param name="error">
        /// The business exception that was thrown.
        /// </param>
        /// <returns>
        /// A <see cref="FaultException"/> for the specified request and business exception.
        /// </returns>
        public static FaultException<EntityValidationFault> CreateValidationException(
            this ActionRequest request, BusinessException error)
        {
            if (error == null)
            {
                throw new ArgumentNullException("error");
            }

            var fault = request.CreateFault<EntityValidationFault>(error, EntityValidationFaultCode);
            fault.ValidationErrors = new Collection<string>(error.EntityErrors.ToEnumerable().ToList());

            return new FaultException<EntityValidationFault>(
                fault, fault.Reason, FaultCode.CreateSenderFaultCode(fault.ErrorCode, FaultNamespace));
        }

        #endregion

        #region Create Fault Exceptions For Exceptions

        /// <summary>
        /// Creates a fault of the specified type based on an action event.
        /// </summary>
        /// <param name="error">
        /// The exception to create the fault for.
        /// </param>
        /// <typeparam name="TFault">
        /// The type of fault to create.
        /// </typeparam>
        /// <returns>
        /// A fault of type <see cref="TFault"/>.
        /// </returns>
        public static FaultException<TFault> ToFaultException<TFault>(this Exception error)
            where TFault : FaultBase, new()
        {
            return CreateException<TFault>(ActionRequest.Create(EntityOperationContext.Current), error, null, null);
        }

        /// <summary>
        /// Creates a fault of the specified type based on an action event.
        /// </summary>
        /// <param name="error">
        /// The exception to create the fault for.
        /// </param>
        /// <param name="faultCode">
        /// The fault code to apply to the fault.
        /// </param>
        /// <typeparam name="TFault">
        /// The type of fault to create.
        /// </typeparam>
        /// <returns>
        /// A fault of type <see cref="TFault"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="faultCode"/> is null.
        /// </exception>
        public static FaultException<TFault> ToFaultException<TFault>(this Exception error, string faultCode)
            where TFault : FaultBase, new()
        {
            if (String.IsNullOrWhiteSpace(faultCode))
            {
                throw new ArgumentNullException("faultCode");
            }

            return CreateException<TFault>(ActionRequest.Create(EntityOperationContext.Current), error, faultCode, null);
        }

        #endregion

        #region Create Fault Exceptions for Action Requests

        /// <summary>
        /// Creates a fault of the specified type based on an action event.
        /// </summary>
        /// <param name="actionRequest">
        /// The action event to create the fault for.
        /// </param>
        /// <param name="reason">
        /// The reason for the fault.
        /// </param>
        /// <typeparam name="TFault">
        /// The type of fault to create.
        /// </typeparam>
        /// <returns>
        /// A fault of type <see cref="TFault"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reason"/> is null.
        /// </exception>
        public static FaultException<TFault> CreateException<TFault>(this ActionRequest actionRequest, string reason)
            where TFault : FaultBase, new()
        {
            return CreateException<TFault>(actionRequest, reason, null);
        }

        /// <summary>
        /// Creates a fault of the specified type based on an action event.
        /// </summary>
        /// <param name="actionRequest">
        /// The action event to create the fault for.
        /// </param>
        /// <param name="reason">
        /// The reason for the fault.
        /// </param>
        /// <param name="faultCode">
        /// The fault code to apply to the fault.
        /// </param>
        /// <typeparam name="TFault">
        /// The type of fault to create.
        /// </typeparam>
        /// <returns>
        /// A fault of type <see cref="TFault"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reason"/> is null.
        /// </exception>
        public static FaultException<TFault> CreateException<TFault>(this ActionRequest actionRequest, string reason, string faultCode)
            where TFault : FaultBase, new()
        {
            if (String.IsNullOrWhiteSpace(reason))
            {
                throw new ArgumentNullException("reason");
            }

            return CreateException<TFault>(actionRequest, null, faultCode, reason);
        }

        /// <summary>
        /// Creates a fault of the specified type based on an action event.
        /// </summary>
        /// <param name="actionRequest">
        /// The action event to create the fault for.
        /// </param>
        /// <param name="error">
        /// The error associated with the event.
        /// </param>
        /// <param name="faultCode">
        /// The fault code to apply to the fault.
        /// </param>
        /// <typeparam name="TFault">
        /// The type of fault to create.
        /// </typeparam>
        /// <returns>
        /// A fault of type <see cref="TFault"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="error"/> is null.
        /// </exception>
        public static FaultException<TFault> CreateException<TFault>(
            this ActionRequest actionRequest, Exception error, string faultCode)
            where TFault : FaultBase, new()
        {
            if (error == null)
            {
                throw new ArgumentNullException("error");
            }

            return CreateException<TFault>(actionRequest, error, faultCode, null);
        }

        /// <summary>
        /// Creates a fault of the specified type based on an action event.
        /// </summary>
        /// <param name="actionRequest">
        /// The action event to create the fault for.
        /// </param>
        /// <param name="error">
        /// The error associated with the event.
        /// </param>
        /// <param name="faultCode">
        /// The fault code to send to the service.
        /// </param>
        /// <param name="reason">
        /// The reason for the fault.
        /// </param>
        /// <typeparam name="TFault">
        /// The type of fault to create.
        /// </typeparam>
        /// <returns>
        /// A fault of type <see cref="TFault"/>.
        /// </returns>
        public static FaultException<TFault> CreateException<TFault>(
            this ActionRequest actionRequest, Exception error, string faultCode, string reason)
            where TFault : FaultBase, new()
        {
            string code = faultCode ?? String.Format(ActionFaultFormat, actionRequest.ActionName, typeof(TFault).ToRuntimeName());
            var fault = CreateFault<TFault>(actionRequest, error, code, reason);
            var receiverFaultCode = FaultCode.CreateReceiverFaultCode(code, FaultNamespace);

            return new FaultException<TFault>(fault, fault.Reason, receiverFaultCode, fault.Action);
        }

        #endregion

        #region FaultException Extensions

        /// <summary>
        /// Gets an exception message for the current <see cref="FaultException"/>.
        /// </summary>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> with a message related to the fault.
        /// </returns>
        public static string GetFaultMessage(this FaultException exception)
        {
            return String.Format(FaultExceptionFormat, exception.GetFaultCode(), exception.Action, exception.Reason);
        }

        /// <summary>
        /// Gets a string representation of the <see cref="FaultCode"/> for the current <see cref="FaultException"/>.
        /// </summary>
        /// <param name="exception">
        /// The current exception.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> containing all of the code and sub-code names in order of highest to lowest.
        /// </returns>
        public static string GetFaultCode(this FaultException exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }

            var codes = new List<string>();
            FaultCode faultCode = exception.Code;

            while (faultCode != null)
            {
                codes.Add(faultCode.Name);
                faultCode = faultCode.SubCode;
            }

            return String.Join(" -> ", codes);
        }

        /// <summary>
        /// Determines whether the current <see cref="FaultException"/> contains an <see cref="IActionFault"/>.
        /// </summary>
        /// <param name="exception">
        /// The fault exception to test.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="exception"/> contains an <see cref="IActionFault"/>; otherwise, <c>false</c>.
        /// </returns>
        [SuppressMessage(
            "Microsoft.Design", 
            "CA1011:ConsiderPassingBaseTypesAsParameters", 
            Justification = "This method is specific to FaultExceptions and would be confusing and useless on the base Exception class.")]
        public static bool IsActionFault(this FaultException exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }

            Type errorType = exception.GetType();

            if (errorType.IsGenericType && errorType.GetGenericTypeDefinition() == typeof(FaultException<>))
            {
                PropertyInfo propertyInfo = errorType.GetProperty(FaultExceptionDetailPropertyName);
                return typeof(IActionFault).IsAssignableFrom(propertyInfo.PropertyType);
            }

            return false;
        }

        /// <summary>
        /// Gets an <see cref="IActionFault"/> from the specified fault exception.
        /// </summary>
        /// <param name="exception">
        /// The fault exception to retrieve the <see cref="IActionFault"/> from.
        /// </param>
        /// <returns>
        /// The detail of the fault as an <see cref="IActionFault"/>.
        /// </returns>
        public static IActionFault GetActionFault(this FaultException exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }

            if (exception.IsActionFault())
            {
                PropertyInfo propertyInfo = exception.GetType().GetProperty(FaultExceptionDetailPropertyName);
                return propertyInfo.GetValue(exception, null) as IActionFault;
            }

            return null;
        }

        /// <summary>
        /// Determines whether an exception is a fault for a specific exception type.
        /// </summary>
        /// <param name="exception">
        /// The exception to evaluate.
        /// </param>
        /// <typeparam name="TException">
        /// The type of exception to evaluate for.
        /// </typeparam>
        /// <returns>
        /// <c>true</c> if the fault code contains the name of the <typeparamref name="TException"/> type; otherwise, <c>false</c>.
        /// </returns>
        [SuppressMessage(
            "Microsoft.Design", 
            "CA1004:GenericMethodsShouldProvideTypeParameter", 
            Justification = "This is common practice for methods with only one generic parameter.")]
        public static bool IsFaultFor<TException>(this FaultException exception)
            where TException : Exception
        {
            return exception != null && exception.GetFaultCode().Contains(typeof(TException).Name);
        }

        /// <summary>
        /// Determines whether a <see cref="FaultException"/> is for a specific <see cref="IActionFault"/>.
        /// </summary>
        /// <param name="exception">
        /// The exception to evaluate.
        /// </param>
        /// <typeparam name="TFault">
        /// The type of <see cref="IActionFault"/> to evaluate for.
        /// </typeparam>
        /// <returns>
        /// <c>true</c> if the fault is of the specified <typeparamref name="TFault"/> type; otherwise, <c>false</c>.
        /// </returns>
        [SuppressMessage(
            "Microsoft.Design", 
            "CA1004:GenericMethodsShouldProvideTypeParameter", 
            Justification = "This is common practice for methods with only one generic parameter.")]
        public static bool IsFault<TFault>(this FaultException exception)
            where TFault : IActionFault
        {
            return exception != null && exception.IsActionFault() && exception.GetActionFault() is TFault;
        }

        #endregion

        /// <summary>
        /// Creates a fault of the specified type based on an action event.
        /// </summary>
        /// <param name="actionRequest">
        /// The action event to create the fault for.
        /// </param>
        /// <param name="error">
        /// The error associated with the event.
        /// </param>
        /// <param name="faultCode">
        /// The fault code associated with the error.
        /// </param>
        /// <typeparam name="TFault">
        /// The type of fault to create.
        /// </typeparam>
        /// <returns>
        /// A fault of type <see cref="TFault"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="error"/> is null.
        /// </exception>
        private static TFault CreateFault<TFault>(this ActionRequest actionRequest, Exception error, string faultCode)
            where TFault : FaultBase, new()
        {
            if (error == null)
            {
                throw new ArgumentNullException("error");
            }

            return CreateFault<TFault>(actionRequest, error, faultCode, null);
        }

        /// <summary>
        /// Creates a fault of the specified type based on an action event.
        /// </summary>
        /// <param name="actionRequest">
        /// The action event to create the fault for.
        /// </param>
        /// <param name="error">
        /// The error associated with the request.
        /// </param>
        /// <param name="faultCode">
        /// The fault code associated with the error.
        /// </param>
        /// <param name="reason">
        /// The reason for the fault.
        /// </param>
        /// <typeparam name="TFault">
        /// The type of fault to create.
        /// </typeparam>
        /// <returns>
        /// A fault of type <see cref="TFault"/>.
        /// </returns>
        private static TFault CreateFault<TFault>(this ActionRequest actionRequest, Exception error, string faultCode, string reason)
            where TFault : FaultBase, new()
        {
            if (actionRequest == ActionRequest.Empty)
            {
                throw new ArgumentNullException("actionRequest");
            }

            var errorInfo = error == null ? ErrorInfo.Empty : ServiceErrorMapping.Default.MapErrorInfo(actionRequest.ActionName, error);

            return new TFault
            {
                ActionIdentifier = actionRequest.GlobalIdentifier, 
                ActionSource = actionRequest.ActionSource, 
                Action = actionRequest.ActionName, 
                EntityType = actionRequest.ItemType, 
                TargetEntity = actionRequest.Item, 
                FaultTime = DateTimeOffset.Now, 
                Reason = reason ?? errorInfo.ErrorMessage ?? NoReasonSpecified, 
                ErrorType = errorInfo.ErrorType ?? typeof(TFault).ToRuntimeName(), 
                ErrorCode = faultCode ?? errorInfo.ErrorCode, 
                ErrorData = errorInfo.ErrorData ?? actionRequest.AdditionalData
            };
        }
    }
}
