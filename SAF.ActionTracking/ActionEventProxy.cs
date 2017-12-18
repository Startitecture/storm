// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionEventProxy.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActionTracking
{
    using System;
    using System.Diagnostics;
    using System.Security;
    using System.Security.Principal;

    using SAF.Core;
    using SAF.Security;
    using SAF.StringResources;

    /// <summary>
    /// Acts as a proxy for creating, completing and recording action events.
    /// </summary>
    public sealed class ActionEventProxy : IActionEventProxy
    {
        /// <summary>
        /// The action format.
        /// </summary>
        private const string ActionFormat = "Perform '{0}' on '{1}'";

        #region Fields

        /// <summary>
        /// The action context.
        /// </summary>
        private readonly IActionContext actionContext;

        /// <summary>
        /// The error handler.
        /// </summary>
        private readonly IExceptionHandler errorHandler;

        /// <summary>
        /// The error mapping.
        /// </summary>
        private readonly IErrorMapping errorMapping;

        /// <summary>
        /// The event repository provider.
        /// </summary>
        private readonly IEventRepositoryFactory eventRepositoryFactory;

        /// <summary>
        /// The record failure only.
        /// </summary>
        private readonly bool recordFailureOnly;

        /// <summary>
        /// The windows account info provider.
        /// </summary>
        private readonly WindowsAccountInfoProvider windowsAccountInfoProvider = new WindowsAccountInfoProvider();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionEventProxy"/> class.
        /// </summary>
        /// <param name="actionContext">
        /// The action context.
        /// </param>
        /// <param name="errorHandler">
        /// The error Handler.
        /// </param>
        /// <param name="errorMapping">
        /// The error Mapping.
        /// </param>
        /// <param name="eventRepositoryFactory">
        /// The event Repository provider.
        /// </param>
        /// <param name="recordFailureOnly">
        /// A value indicating whether only failed actions will be recorded.
        /// </param>
        public ActionEventProxy(
            IActionContext actionContext,
            IExceptionHandler errorHandler,
            IErrorMapping errorMapping,
            IEventRepositoryFactory eventRepositoryFactory,
            bool recordFailureOnly)
        {
            this.actionContext = actionContext;
            this.errorHandler = errorHandler;
            this.errorMapping = errorMapping;
            this.eventRepositoryFactory = eventRepositoryFactory;
            this.recordFailureOnly = recordFailureOnly;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Performs an action on the specified item, creating and completing an action request.
        /// </summary>
        /// <param name="item">
        /// The target item.
        /// </param>
        /// <param name="action">
        /// The action to perform.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item upon which the action will be performed.
        /// </typeparam>
        public void PerformAction<TItem>(TItem item, Action<TItem> action)
        {
            if (Evaluate.IsNull(item))
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            this.PerformAction(ActionRequest.Create(item, action), item, action);
        }

        /// <summary>
        /// Performs an action on the specified item with the action identifier, creating and completing an action request.
        /// </summary>
        /// <param name="item">
        /// The target item.
        /// </param>
        /// <param name="action">
        /// The action to perform.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item upon which the action will be performed.
        /// </typeparam>
        public void PerformAction<TItem>(TItem item, Action<TItem, Guid> action)
        {
            if (Evaluate.IsNull(item))
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var request = ActionRequest.Create(item, action);
            var actionEvent = this.BeginAction(request);

            try
            {
                action(item, actionEvent.Request.GlobalIdentifier);
                this.CompleteAction(actionEvent);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Action '{0}' failed: {1}", request, ex);
                this.CompleteAction(actionEvent, ex);
                this.errorHandler.HandleException(action, item, ex);
                throw;
            }
        }

        /// <summary>
        /// Performs an action on the specified item, creating and completing an action request.
        /// </summary>
        /// <param name="request">
        /// The action request.
        /// </param>
        /// <param name="item">
        /// The target item.
        /// </param>
        /// <param name="action">
        /// The action to perform.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item upon which the action will be performed.
        /// </typeparam>
        public void PerformAction<TItem>(ActionRequest request, TItem item, Action<TItem> action)
        {
            if (request == ActionRequest.Empty)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (Evaluate.IsNull(item))
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            ActionEvent actionEvent = this.BeginAction(request);

            try
            {
                action(item);
                this.CompleteAction(actionEvent);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Action '{0}' failed: {1}", request, ex);
                this.CompleteAction(actionEvent, ex);
                this.errorHandler.HandleException(request, ex);
                throw;
            }
        }

        /// <summary>
        /// Performs an action on the specified item, creating and completing an action request.
        /// </summary>
        /// <param name="item">
        /// The target item.
        /// </param>
        /// <param name="action">
        /// The action to perform.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item upon which the action will be performed.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of result that the action will produce.
        /// </typeparam>
        /// <returns>
        /// The result of the action as a <typeparamref name="TResult"/>.
        /// </returns>
        public TResult PerformActionWithResult<TItem, TResult>(TItem item, Func<TItem, TResult> action)
        {
            return this.PerformActionWithResult(ActionRequest.Create(item, action), item, action);
        }

        /// <summary>
        /// Performs an action on the specified item, creating and completing an action request.
        /// </summary>
        /// <param name="item">
        /// The target item.
        /// </param>
        /// <param name="action">
        /// The action to perform.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item upon which the action will be performed.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of result that the action will produce.
        /// </typeparam>
        /// <returns>
        /// The result of the action as a <typeparamref name="TResult"/>.
        /// </returns>
        public TResult PerformActionWithResult<TItem, TResult>(TItem item, Func<TItem, Guid, TResult> action)
        {
            if (Evaluate.IsNull(item))
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var request = ActionRequest.Create(item, action);
            var actionEvent = this.BeginAction(request);

            try
            {
                var result = action(item, actionEvent.Request.GlobalIdentifier);
                this.CompleteAction(actionEvent);
                return result;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Action '{0}' failed: {1}", request, ex);
                this.CompleteAction(actionEvent, ex);
                this.errorHandler.HandleException(action, item, ex);
                throw;
            }
        }

        /// <summary>
        /// Performs an action on the specified item, creating and completing an action request.
        /// </summary>
        /// <param name="request">
        /// The action request.
        /// </param>
        /// <param name="item">
        /// The target item.
        /// </param>
        /// <param name="action">
        /// The action to perform.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item upon which the action will be performed.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of result that the action will produce.
        /// </typeparam>
        /// <returns>
        /// The result of the action as a <typeparamref name="TResult"/>.
        /// </returns>
        public TResult PerformActionWithResult<TItem, TResult>(ActionRequest request, TItem item, Func<TItem, TResult> action)
        {
            if (request == ActionRequest.Empty)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (Evaluate.IsNull(item))
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var actionEvent = this.BeginAction(request);

            try
            {
                var result = action(item);
                this.CompleteAction(actionEvent, null);
                return result;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Action '{0}' failed: {1}", request, ex);
                this.CompleteAction(actionEvent, ex);
                this.errorHandler.HandleException(request, ex);
                throw;
            }
        }

        /// <summary>
        /// Records an action on the specified item.
        /// </summary>
        /// <param name="item">
        /// The target item.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item associated with the action.
        /// </typeparam>
        public void RecordAction<TItem>(TItem item)
        {
            string additionalData = item.ToPropertyValueString();
            var currentAction = this.actionContext.CurrentAction;
            var actionDescription = String.Format(ActionFormat, currentAction, item);
            var request = this.CreateRequest(currentAction, actionDescription, item, additionalData);
            var actionEvent = ActionEvent.StartAction(request, this.GetAccount());
            actionEvent.CompleteAction(actionEvent.InitiationTime);
            this.RecordAction(actionEvent);
        }

        /// <summary>
        /// Records an action on the specified item.
        /// </summary>
        /// <param name="item">
        /// The target item.
        /// </param>
        /// <param name="action">
        /// The action that was taken.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item associated with the action.
        /// </typeparam>
        public void RecordAction<TItem>(TItem item, Delegate action)
        {
            var request = ActionRequest.Create(item, action);
            var actionEvent = ActionEvent.StartAction(request, this.GetAccount());
            actionEvent.CompleteAction(actionEvent.InitiationTime);
            this.RecordAction(actionEvent);
        }

        /// <summary>
        /// Records an action on the specified item.
        /// </summary>
        /// <param name="item">
        /// The target item.
        /// </param>
        /// <param name="action">
        /// The action that was taken.
        /// </param>
        /// <param name="actionError">
        /// The action error.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item associated with the action.
        /// </typeparam>
        public void RecordAction<TItem>(TItem item, Delegate action, Exception actionError)
        {
            var request = ActionRequest.Create(item, action);
            var actionEvent = ActionEvent.StartAction(request, this.GetAccount());
            actionEvent.CompleteAction(this.errorMapping.MapErrorInfo(request.ActionName, actionError), actionEvent.InitiationTime);
            this.RecordAction(actionEvent);
        }

        /// <summary>
        /// Records an action on the specified item.
        /// </summary>
        /// <param name="item">
        /// The target item.
        /// </param>
        /// <param name="actionName">
        /// The name of the action that was taken.
        /// </param>
        /// <param name="description">
        /// The description of the action.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item associated with the action.
        /// </typeparam>
        public void RecordAction<TItem>(TItem item, string actionName, string description)
        {
            var request = ActionRequest.Create(
                Guid.NewGuid(),
                actionName,
                ApplicationOperationContext.Current.CurrentActionSource,
                item,
                description,
                item.ToPropertyValueString());

            var actionEvent = ActionEvent.StartAction(request, this.GetAccount());
            actionEvent.CompleteAction(actionEvent.InitiationTime);
            this.RecordAction(actionEvent);
        }

        /// <summary>
        /// Records an action on the specified item.
        /// </summary>
        /// <param name="item">
        /// The target item.
        /// </param>
        /// <param name="actionError">
        /// The error associated with the action.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item associated with the action.
        /// </typeparam>
        public void RecordAction<TItem>(TItem item, Exception actionError)
        {
            if (actionError == null)
            {
                throw new ArgumentNullException(nameof(actionError));
            }

            var request = ActionRequest.Create(this.actionContext, item);
            var actionEvent = ActionEvent.StartAction(request, this.GetAccount());
            actionEvent.CompleteAction(this.errorMapping.MapErrorInfo(request.ActionName, actionError), DateTimeOffset.Now);
            this.RecordAction(actionEvent);
        }

        /// <summary>
        /// Records an action event.
        /// </summary>
        /// <param name="actionEvent">
        /// The action request.
        /// </param>
        public void RecordAction(ActionEvent actionEvent)
        {
            this.RecordActionEvent(actionEvent);
        }

        #endregion

        #region Methods

        #region Actions

        /// <summary>
        /// Creates an action request for the specified item.
        /// </summary>
        /// <param name="actionName">
        /// The name of the action.
        /// </param>
        /// <param name="actionDescription">
        /// A description of the action.
        /// </param>
        /// <param name="targetItem">
        /// The target item.
        /// </param>
        /// <param name="additionalData">
        /// Additional data associated with the action.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item that the action will be performed on.
        /// </typeparam>
        /// <returns>
        /// An <see cref="ActionRequest"/> for the specified action parameters.
        /// </returns>
        private ActionRequest CreateRequest<TItem>(string actionName, string actionDescription, TItem targetItem, string additionalData)
        {
            if (actionName == null)
            {
                throw new ArgumentNullException(nameof(actionName));
            }

            if (actionDescription == null)
            {
                throw new ArgumentNullException(nameof(actionDescription));
            }

            return ActionRequest.Create(
                actionName, 
                this.actionContext.CurrentActionSource, 
                targetItem, 
                actionDescription, 
                additionalData);
        }

        /// <summary>
        /// Begins an action and returns the action event to the caller.
        /// </summary>
        /// <param name="actionRequest">
        /// The action Request.
        /// </param>
        /// <returns>
        /// An <see cref="ActionRequest"/> for the specified action.
        /// </returns>
        private ActionEvent BeginAction(ActionRequest actionRequest)
        {
            var actionEvent = ActionEvent.StartAction(actionRequest, this.GetAccount());
            return this.recordFailureOnly ? actionEvent : this.RecordActionEvent(actionEvent);
        }

        /// <summary>
        /// Gets the account for the specified identity.
        /// </summary>
        /// <returns>
        /// An <see cref="AccountInfo"/> for the specified identity.
        /// </returns>
        private IAccountInfo GetAccount()
        {
            var identity = this.actionContext.CurrentIdentity;
            IAccountInfo accountInfo;

            try
            {
                accountInfo = this.actionContext.CurrentAccount;
            }
            catch (DomainException ex)
            {
                Trace.TraceError(ex.ToString());

                try
                {
                    accountInfo = this.windowsAccountInfoProvider.GetAccountInfo(identity);
                }
                catch (SecurityException ex2)
                {
                    Trace.TraceError(ex2.ToString());
                    accountInfo =
                        new WindowsAccountInfo(new WindowsPrincipal(WindowsIdentity.GetCurrent() ?? WindowsIdentity.GetAnonymous()));
                }
                catch (DomainException ex2)
                {
                    Trace.TraceError(ex2.ToString());
                    accountInfo =
                        new WindowsAccountInfo(new WindowsPrincipal(WindowsIdentity.GetCurrent() ?? WindowsIdentity.GetAnonymous()));
                }
            }

            return accountInfo;
        }

        /// <summary>
        /// Completes the specified action and saves it to the repository.
        /// </summary>
        /// <param name="actionEvent">
        /// The action event to complete.
        /// </param>
        private void CompleteAction(ActionEvent actionEvent)
        {
            this.CompleteAction(actionEvent, null);
        }

        /// <summary>
        /// Completes the specified action and saves it to the repository.
        /// </summary>
        /// <param name="actionEvent">
        /// The action event to complete.
        /// </param>
        /// <param name="eventError">
        /// The error, if any, associated with the event.
        /// </param>
        private void CompleteAction(ActionEvent actionEvent, Exception eventError)
        {
            if (actionEvent == null)
            {
                throw new ArgumentNullException(nameof(actionEvent));
            }

            if (eventError == null)
            {
                actionEvent.CompleteAction();

                if (this.recordFailureOnly == false)
                {
                    this.RecordActionEvent(actionEvent);
                }
            }
            else
            {
                actionEvent.CompleteAction(this.errorMapping.MapErrorInfo(actionEvent.Request.ActionName, eventError));
                this.RecordActionEvent(actionEvent);
            }
        }

        /// <summary>
        /// Records an action event.
        /// </summary>
        /// <param name="actionEvent">
        /// The action event to record.
        /// </param>
        /// <returns>
        /// The saved <see cref="ActionEvent"/>.
        /// </returns>
        private ActionEvent RecordActionEvent(ActionEvent actionEvent)
        {
            try
            {
                return this.eventRepositoryFactory.Create().Save(actionEvent);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ErrorMessages.SaveEventFailed, actionEvent.ToPropertyValueString(), ex.Message);
                this.errorHandler.HandleException(actionEvent.Request, ex);
                throw;
            }
        }

        #endregion

        #endregion
    }
}