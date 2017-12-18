// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IActionEventProxy.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActionTracking
{
    using System;

    /// <summary>
    /// Provides an interface to an event-based service proxy.
    /// </summary>
    public interface IActionEventProxy
    {
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
        void PerformAction<TItem>(TItem item, Action<TItem> action);

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
        void PerformAction<TItem>(ActionRequest request, TItem item, Action<TItem> action);

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
        TResult PerformActionWithResult<TItem, TResult>(TItem item, Func<TItem, TResult> action);

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
        TResult PerformActionWithResult<TItem, TResult>(TItem item, Func<TItem, Guid, TResult> action);

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
        TResult PerformActionWithResult<TItem, TResult>(ActionRequest request, TItem item, Func<TItem, TResult> action);

        /// <summary>
        /// Records an action event.
        /// </summary>
        /// <param name="actionEvent">
        /// The action request.
        /// </param>
        void RecordAction(ActionEvent actionEvent);

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
        void PerformAction<TItem>(TItem item, Action<TItem, Guid> action);

        /// <summary>
        /// Records an action on the specified item.
        /// </summary>
        /// <param name="item">
        /// The target item.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item associated with the action.
        /// </typeparam>
        void RecordAction<TItem>(TItem item);

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
        void RecordAction<TItem>(TItem item, Delegate action);

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
        void RecordAction<TItem>(TItem item, Delegate action, Exception actionError);

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
        void RecordAction<TItem>(TItem item, string actionName, string description);

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
        void RecordAction<TItem>(TItem item, Exception actionError);
    }
}