// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionEvent.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActionTracking
{
    using System;

    using JetBrains.Annotations;

    using SAF.Core;
    using SAF.Security;

    /// <summary>
    /// Contains information about a specific action request.
    /// </summary>
    public class ActionEvent : IEquatable<ActionEvent>
    {
        /// <summary>
        /// The completed format.
        /// </summary>
        private const string CompletedFormat = "{0} performed {1} starting {2}, completed at {3}";

        /// <summary>
        /// The failed format.
        /// </summary>
        private const string FailedFormat = "{0} performed {1} starting {2}, failed at {3}: {4}";

        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<ActionEvent, object>[] ComparisonProperties =
            {
                item => item.CompletionTime, 
                item => item.ErrorInfo, 
                item => item.InitiationTime, 
                item => item.Request, 
                item => item.UserAccount
            };

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionEvent"/> class.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="userAccount">
        /// The user account.
        /// </param>
        /// <param name="initiationTime">
        /// The time the event was initiated.
        /// </param>
        public ActionEvent(ActionRequest request, IAccountInfo userAccount, DateTimeOffset initiationTime)
            : this(request, userAccount)
        {
            this.InitiationTime = initiationTime;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionEvent"/> class.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="userAccount">
        /// The user account.
        /// </param>
        /// <param name="initiationTime">
        /// The time the event was initiated.
        /// </param>
        /// <param name="actionEventId">
        /// The action event ID for the current event.
        /// </param>
        public ActionEvent(ActionRequest request, IAccountInfo userAccount, DateTimeOffset initiationTime, long actionEventId)
            : this(request, userAccount, initiationTime)
        {
            this.ActionEventId = actionEventId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionEvent"/> class.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="userAccount">
        /// The user account.
        /// </param>
        private ActionEvent(ActionRequest request, IAccountInfo userAccount)
        {
            this.Request = request;
            this.UserAccount = userAccount;
            this.InitiationTime = DateTimeOffset.Now;
            this.CompletionTime = this.InitiationTime;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the action event ID.
        /// </summary>
        public long? ActionEventId { get; private set; }

        /// <summary>
        /// Gets the completion time of the event.
        /// </summary>
        public DateTimeOffset CompletionTime { get; private set; }

        /// <summary>
        /// Gets the initiation time of the event.
        /// </summary>
        public DateTimeOffset InitiationTime { get; private set; }

        /// <summary>
        /// Gets the action request that initiated the event.
        /// </summary>
        public ActionRequest Request { get; private set; }

        /// <summary>
        /// Gets the user account that initiated the request.
        /// </summary>
        public IAccountInfo UserAccount { get; private set; }

        /// <summary>
        /// Gets the error info, if any, associated with the current event.
        /// </summary>
        public ErrorInfo ErrorInfo { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Starts an action event.
        /// </summary>
        /// <param name="actionContext">
        /// The action context.
        /// </param>
        /// <param name="item">
        /// The target of the action.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item that is the target of the action.
        /// </typeparam>
        /// <returns>
        /// An <see cref="ActionEvent"/> instance for the current action and target item.
        /// </returns>
        public static ActionEvent StartAction<TItem>([NotNull] IActionContext actionContext, [NotNull] TItem item)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException(nameof(actionContext));
            }

            return StartAction(ActionRequest.Create(actionContext, item), actionContext.CurrentAccount);
        }

        /// <summary>
        /// Starts an action event.
        /// </summary>
        /// <param name="actionRequest">
        /// The action request.
        /// </param>
        /// <returns>
        /// An <see cref="ActionEvent"/> for the specified request.
        /// </returns>
        public static ActionEvent StartAction(ActionRequest actionRequest)
        {
            return new ActionEvent(actionRequest, ApplicationOperationContext.Current.CurrentAccount);
        }

        /// <summary>
        /// Starts an action event.
        /// </summary>
        /// <param name="actionRequest">
        /// The action request.
        /// </param>
        /// <param name="userAccount">
        /// The user account making the request.
        /// </param>
        /// <returns>
        /// An <see cref="ActionEvent"/> for the specified request.
        /// </returns>
        public static ActionEvent StartAction(ActionRequest actionRequest, IAccountInfo userAccount)
        {
            if (userAccount == null)
            {
                throw new ArgumentNullException("userAccount");
            }

            return new ActionEvent(actionRequest, userAccount);
        }

        /// <summary>
        /// Completes the action request.
        /// </summary>
        public void CompleteAction()
        {
            this.CompleteAction(DateTimeOffset.Now);
        }

        /// <summary>
        /// Completes the action request.
        /// </summary>
        /// <param name="completionTime">
        /// The time of the completed request.
        /// </param>
        public void CompleteAction(DateTimeOffset completionTime)
        {
            this.CompleteAction(ErrorInfo.Empty, completionTime);
        }

        /// <summary>
        /// Completes the action request.
        /// </summary>
        /// <param name="errorInfo">
        /// The error associated with the event.
        /// </param>
        public void CompleteAction(ErrorInfo errorInfo)
        {
            this.CompleteAction(errorInfo, DateTimeOffset.Now);
        }

        /// <summary>
        /// Completes the action request.
        /// </summary>
        /// <param name="error">
        /// The error associated with the event.
        /// </param>
        public void CompleteAction(Exception error)
        {
            this.CompleteAction(error, DateTimeOffset.Now);
        }

        /// <summary>
        /// Completes the action request.
        /// </summary>
        /// <param name="error">
        /// The error associated with the event.
        /// </param>
        /// <param name="completionTime">
        /// The time the action was completed.
        /// </param>
        public void CompleteAction(Exception error, DateTimeOffset completionTime)
        {
            if (error == null)
            {
                throw new ArgumentNullException("error");
            }

            var errorInfo = ErrorMapping.Default.MapErrorInfo(this.Request.ActionName, error);
            this.CompleteAction(errorInfo, completionTime);
        }

        /// <summary>
        /// Completes the action request.
        /// </summary>
        /// <param name="errorInfo">
        /// The error info associated with the event.
        /// </param>
        /// <param name="completionTime">
        /// The time the action was completed.
        /// </param>
        public void CompleteAction(ErrorInfo errorInfo, DateTimeOffset completionTime)
        {
            this.CompletionTime = completionTime;
            this.ErrorInfo = errorInfo;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="ActionEvent"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="ActionEvent"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">
        /// The <see cref="T:System.Object"/> to compare with the current <see cref="ActionEvent"/>. 
        /// </param>
        /// <filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            return Evaluate.Equals(this, obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        public bool Equals(ActionEvent other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="ActionEvent"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return Evaluate.GenerateHashCode(this, ComparisonProperties);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="ActionEvent"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="ActionEvent"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            if (this.ErrorInfo == ErrorInfo.Empty)
            {
                return String.Format(CompletedFormat, this.UserAccount, this.Request, this.InitiationTime, this.CompletionTime);
            }

            return String.Format(
                FailedFormat, 
                this.UserAccount, 
                this.Request, 
                this.InitiationTime, 
                this.CompletionTime, 
                this.ErrorInfo.ErrorMessage);
        }

        #endregion
    }
}