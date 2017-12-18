// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IActionContext.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActionTracking
{
    using System.Security.Principal;

    using SAF.Core;
    using SAF.Security;

    /// <summary>
    /// Provides an interface for accessing an action context within an application.
    /// </summary>
    public interface IActionContext
    {
        #region Public Properties

        /// <summary>
        /// Gets the current action.
        /// </summary>
        string CurrentAction { get; }

        /// <summary>
        /// Gets the current action source.
        /// </summary>
        string CurrentActionSource { get; }

        /// <summary>
        /// Gets the endpoint at which the action is executing.
        /// </summary>
        string Endpoint { get; }

        /// <summary>
        /// Gets the host on which the action is executing.
        /// </summary>
        string Host { get; }

        /// <summary>
        /// Gets the identity for the current context.
        /// </summary>
        IIdentity CurrentIdentity { get; }

        /// <summary>
        /// Gets the current account.
        /// </summary>
        IAccountInfo CurrentAccount { get; }

        #endregion

        /////// <summary>
        /////// Records an event associated with the specified item with the calling method as the action and the calling class as the 
        /////// action source.
        /////// </summary>
        /////// <param name="item">
        /////// The item to record.
        /////// </param>
        /////// <param name="exception">
        /////// The error associated with the action.
        /////// </param>
        ////void RecordAction(object item, Exception exception);

        /////// <summary>
        /////// Records an event associated with the specified item with the calling method as the action and the calling class as the 
        /////// action source.
        /////// </summary>
        /////// <param name="item">
        /////// The item to record.
        /////// </param>
        /////// <param name="action">
        /////// The action that was taken.
        /////// </param>
        ////void RecordAction(object item, Delegate action);

        /////// <summary>
        /////// Records an event associated with the specified item with the calling method as the action and the calling class as the 
        /////// action source.
        /////// </summary>
        /////// <param name="item">
        /////// The item to record.
        /////// </param>
        /////// <param name="action">
        /////// The action that was taken.
        /////// </param>
        /////// <param name="exception">
        /////// The error associated with the action.
        /////// </param>
        ////void RecordAction(object item, Delegate action, Exception exception);
    }
}