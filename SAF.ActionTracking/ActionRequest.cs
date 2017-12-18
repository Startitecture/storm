// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionRequest.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActionTracking
{
    using System;

    using JetBrains.Annotations;

    using SAF.Core;

    /// <summary>
    /// Contains information about requests made to perform an action in the application.
    /// </summary>
    public struct ActionRequest : IEquatable<ActionRequest>
    {
        #region Static Fields

        /// <summary>
        /// The empty.
        /// </summary>
        public static readonly ActionRequest Empty = new ActionRequest();

        /// <summary>
        /// The action request format.
        /// </summary>
        private const string ActionRequestFormat = "{0} '{1}' from {2}";

        /// <summary>
        /// The description format.
        /// </summary>
        private const string DescriptionFormat = "{0} on {1} {2}";

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionRequest"/> struct.
        /// </summary>
        /// <param name="globalIdentifier">
        /// The global identifier for the current request.
        /// </param>
        /// <param name="actionName">
        /// The action name.
        /// </param>
        /// <param name="actionSource">
        /// The action source.
        /// </param>
        /// <param name="actionEndpoint">
        /// The action endpoint.
        /// </param>
        /// <param name="actionHost">
        /// The action host.
        /// </param>
        /// <param name="targetObjectType">
        /// The target object type.
        /// </param>
        /// <param name="targetObject">
        /// The target object.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="additionalData">
        /// The additional data.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="actionName"/>, <paramref name="actionSource"/> or <paramref name="description"/> is null.
        /// </exception>
        private ActionRequest(
            Guid globalIdentifier,
            string actionName,
            string actionSource,
            string actionEndpoint,
            string actionHost,
            string targetObjectType,
            string targetObject,
            string description,
            string additionalData)
            : this()
        {
            if (String.IsNullOrWhiteSpace(actionName))
            {
                throw new ArgumentNullException("actionName");
            }

            if (String.IsNullOrWhiteSpace(actionSource))
            {
                throw new ArgumentNullException("actionSource");
            }

            if (String.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentNullException("description");
            }

            this.GlobalIdentifier = globalIdentifier;
            this.ActionName = actionName;
            this.ActionSource = actionSource;
            this.Item = targetObject;
            this.ItemType = targetObjectType;
            this.Description = description;
            this.AdditionalData = additionalData;
            this.ActionHost = actionHost;
            this.ActionEndpoint = actionEndpoint;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the global identifier for this event.
        /// </summary>
        public Guid GlobalIdentifier { get; private set; }

        /// <summary>
        /// Gets the action name.
        /// </summary>
        public string ActionName { get; private set; }

        /// <summary>
        /// Gets the action source.
        /// </summary>
        public string ActionSource { get; private set; }

        /// <summary>
        /// Gets the action endpoint.
        /// </summary>
        public string ActionEndpoint { get; private set; }

        /// <summary>
        /// Gets the action host.
        /// </summary>
        public string ActionHost { get; private set; }

        /// <summary>
        /// Gets the additional data.
        /// </summary>
        public string AdditionalData { get; private set; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets the target item.
        /// </summary>
        public string Item { get; private set; }

        /// <summary>
        /// Gets the target item type.
        /// </summary>
        public string ItemType { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Determines whether two values of the same type are equal.
        /// </summary>
        /// <param name="left">
        /// The first value.
        /// </param>
        /// <param name="right">
        /// The second value.
        /// </param>
        /// <returns>
        /// <c>true</c> if the values are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(ActionRequest left, ActionRequest right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two values of the same type not are equal.
        /// </summary>
        /// <param name="left">
        /// The first value.
        /// </param>
        /// <param name="right">
        /// The second value.
        /// </param>
        /// <returns>
        /// <c>true</c> if the values are not equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(ActionRequest left, ActionRequest right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Creates an action request.
        /// </summary>
        /// <param name="actionContext">
        /// The action context.
        /// </param>
        /// <returns>
        /// An <see cref="ActionRequest"/> for the specified action.
        /// </returns>
        public static ActionRequest Create([NotNull] IActionContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException(nameof(actionContext));
            }

            string currentAction = actionContext.CurrentAction;
            string actionSource = actionContext.CurrentActionSource;
            string actionEndpoint = actionContext.Endpoint;
            string actionHost = actionContext.Host;
            return new ActionRequest(
                Guid.NewGuid(),
                currentAction,
                actionSource,
                actionEndpoint,
                actionHost,
                null,
                null,
                currentAction,
                null);
        }

        /// <summary>
        /// Creates an action request.
        /// </summary>
        /// <param name="actionContext">
        /// The action context.
        /// </param>
        /// <param name="targetItem">
        /// The target item.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item that is the target of the action.
        /// </typeparam>
        /// <returns>
        /// An <see cref="ActionRequest"/> for the specified action and item.
        /// </returns>
        public static ActionRequest Create<TItem>([NotNull] IActionContext actionContext, TItem targetItem)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException(nameof(actionContext));
            }

            var currentAction = actionContext.CurrentAction;
            var itemDescription = GetDescription(targetItem, currentAction);
            string actionSource = actionContext.CurrentActionSource;

            return Create(currentAction, actionSource, targetItem, itemDescription, targetItem.ToPropertyValueString());
        }

        /// <summary>
        /// Creates an action request.
        /// </summary>
        /// <param name="item">
        /// The target item.
        /// </param>
        /// <param name="action">
        /// The action to create the request for.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item that is the target of the action.
        /// </typeparam>
        /// <returns>
        /// An <see cref="ActionRequest"/> for the specified action and item.
        /// </returns>
        public static ActionRequest Create<TItem>(TItem item, Delegate action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            var currentAction = action.Method.Name;
            var itemDescription = GetDescription(item, currentAction);

            string actionSource = action.Method.DeclaringType == null ? "Unknown" : action.Method.DeclaringType.FullName;
            return Create(currentAction, actionSource, item, itemDescription, item.ToPropertyValueString());
        }

        /// <summary>
        /// Creates an action request.
        /// </summary>
        /// <param name="actionName">
        /// The name of the action.
        /// </param>
        /// <param name="description">
        /// A description of the action.
        /// </param>
        /// <param name="targetItem">
        /// The item that is the target of the action.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item that is the target of the action.
        /// </typeparam>
        /// <returns>
        /// An <see cref="ActionRequest"/> for the specified action and item.
        /// </returns>
        public static ActionRequest Create<TItem>(string actionName, string description, TItem targetItem)
        {
            var actionSource = ApplicationOperationContext.Current.CurrentActionSource;
            return Create(actionName, actionSource, targetItem, description, null);
        }

        /// <summary>
        /// Creates an action request.
        /// </summary>
        /// <param name="actionName">
        /// The name of the action.
        /// </param>
        /// <param name="actionSource">
        /// The source of the action.
        /// </param>
        /// <param name="targetItem">
        /// The item that is the target of the action.
        /// </param>
        /// <param name="description">
        /// A description of the action.
        /// </param>
        /// <param name="additionalData">
        /// Additional data, if any, associated with the action.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item that is the target of the action.
        /// </typeparam>
        /// <returns>
        /// An <see cref="ActionRequest"/> for the specified action and item.
        /// </returns>
        public static ActionRequest Create<TItem>(
            string actionName, string actionSource, TItem targetItem, string description, string additionalData)
        {
            return Create(Guid.NewGuid(), actionName, actionSource, targetItem, description, additionalData);
        }

        /// <summary>
        /// Creates an action request.
        /// </summary>
        /// <param name="actionIdentifier">
        /// The global identifier for the action.
        /// </param>
        /// <param name="actionName">
        /// The name of the action.
        /// </param>
        /// <param name="actionSource">
        /// The source of the action.
        /// </param>
        /// <param name="targetItem">
        /// The item that is the target of the action.
        /// </param>
        /// <param name="description">
        /// A description of the action.
        /// </param>
        /// <param name="additionalData">
        /// Additional data, if any, associated with the action.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item that is the target of the action.
        /// </typeparam>
        /// <returns>
        /// An <see cref="ActionRequest"/> for the specified action and item.
        /// </returns>
        public static ActionRequest Create<TItem>(
            Guid actionIdentifier, 
            string actionName, 
            string actionSource, 
            TItem targetItem, 
            string description, 
            string additionalData)
        {
            var isNull = Evaluate.IsNull(targetItem);
            string itemType = isNull ? typeof(TItem).ToRuntimeName() : targetItem.GetType().ToRuntimeName();
            string itemDescription = isNull ? null : Convert.ToString(targetItem);
            return Create(actionIdentifier, actionName, actionSource, itemType, itemDescription, description, additionalData);
        }


        /// <summary>
        /// Creates an action request.
        /// </summary>
        /// <param name="globalIdentifier">
        /// The global Identifier.
        /// </param>
        /// <param name="actionName">
        /// The name of the action.
        /// </param>
        /// <param name="actionSource">
        /// The source of the action.
        /// </param>
        /// <param name="actionItemType">
        /// The type of item associated with the action.
        /// </param>
        /// <param name="targetItem">
        /// The item that is the target of the action.
        /// </param>
        /// <param name="actionDescription">
        /// The action description.
        /// </param>
        /// <param name="additionalData">
        /// Additional data, if any, associated with the action.
        /// </param>
        /// <returns>
        /// An <see cref="ActionRequest"/> for the specified action and item.
        /// </returns>
        public static ActionRequest Create(
            Guid globalIdentifier,
            string actionName,
            string actionSource,
            string actionItemType,
            string targetItem,
            string actionDescription,
            string additionalData)
        {
            if (globalIdentifier == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException("globalIdentifier");
            }

            if (String.IsNullOrWhiteSpace(actionName))
            {
                throw new ArgumentNullException("actionName");
            }

            if (string.IsNullOrWhiteSpace(actionSource))
            {
                throw new ArgumentNullException("actionSource");
            }

            if (String.IsNullOrWhiteSpace(targetItem) == false && String.IsNullOrWhiteSpace(actionItemType))
            {
                throw new ArgumentNullException("actionItemType");
            }

            if (String.IsNullOrWhiteSpace(actionDescription))
            {
                throw new ArgumentNullException("actionDescription");
            }

            return Create(
                globalIdentifier,
                actionName,
                actionSource,
                ApplicationOperationContext.Current.Endpoint,
                ApplicationOperationContext.Current.Host,
                actionItemType,
                targetItem,
                actionDescription,
                additionalData);
        }

        /// <summary>
        /// Creates an action request.
        /// </summary>
        /// <param name="globalIdentifier">
        /// The global Identifier.
        /// </param>
        /// <param name="actionName">
        /// The name of the action.
        /// </param>
        /// <param name="actionSource">
        /// The source of the action.
        /// </param>
        /// <param name="actionEndpoint">
        /// The endpoint that executed the action.
        /// </param>
        /// <param name="actionHost">
        /// The host where the action was executed.
        /// </param>
        /// <param name="actionItemType">
        /// The type of item associated with the action.
        /// </param>
        /// <param name="targetItem">
        /// The item that is the target of the action.
        /// </param>
        /// <param name="actionDescription">
        /// The action description.
        /// </param>
        /// <param name="additionalData">
        /// Additional data, if any, associated with the action.
        /// </param>
        /// <returns>
        /// An <see cref="ActionRequest"/> for the specified action and item.
        /// </returns>
        public static ActionRequest Create(
            Guid globalIdentifier, 
            string actionName, 
            string actionSource, 
            string actionEndpoint,
            string actionHost,
            string actionItemType, 
            string targetItem, 
            string actionDescription, 
            string additionalData)
        {
            if (globalIdentifier == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException("globalIdentifier");
            }

            if (String.IsNullOrWhiteSpace(actionName))
            {
                throw new ArgumentNullException("actionName");
            }

            if (string.IsNullOrWhiteSpace(actionSource))
            {
                throw new ArgumentNullException("actionSource");
            }

            if (string.IsNullOrWhiteSpace(actionEndpoint))
            {
                throw new ArgumentNullException("actionEndpoint");
            }

            if (string.IsNullOrWhiteSpace(actionHost))
            {
                throw new ArgumentNullException("actionHost");
            }

            if (String.IsNullOrWhiteSpace(targetItem) == false && String.IsNullOrWhiteSpace(actionItemType))
            {
                throw new ArgumentNullException("actionItemType");
            }

            if (String.IsNullOrWhiteSpace(actionDescription))
            {
                throw new ArgumentNullException("actionDescription");
            }

            return new ActionRequest(
                globalIdentifier, 
                actionName, 
                actionSource, 
                actionEndpoint,
                actionHost,
                actionItemType, 
                targetItem, 
                actionDescription, 
                additionalData);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">
        /// Another object to compare to. 
        /// </param>
        /// <filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            return obj is ActionRequest && this.Equals((ActionRequest)obj);
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
        public bool Equals(ActionRequest other)
        {
            var theseProperties = new object[]
                                      {
                                          this.ActionName, 
                                          this.ActionSource, 
                                          this.Description, 
                                          this.Item, 
                                          this.ItemType, 
                                          this.AdditionalData,
                                          this.ActionEndpoint,
                                          this.ActionHost
                                      };

            var otherProperties = new object[]
                                      {
                                          other.ActionName, 
                                          other.ActionSource, 
                                          other.Description, 
                                          other.Item, 
                                          other.ItemType, 
                                          other.AdditionalData,
                                          other.ActionEndpoint,
                                          other.ActionHost
                                      };

            return Evaluate.CollectionEquals(theseProperties, otherProperties);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return Evaluate.GenerateHashCode(
                this.ActionName,
                this.ActionSource,
                this.Description,
                this.Item,
                this.ItemType,
                this.AdditionalData,
                this.ActionEndpoint,
                this.ActionHost);
        }

        /// <summary>
        /// Returns a description of this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> containing all the properties of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return String.Format(ActionRequestFormat, this.ActionName, this.Item, this.ActionSource);
        }

        #endregion

        /// <summary>
        /// Gets a description of the specified item.
        /// </summary>
        /// <param name="targetItem">
        /// The target item.
        /// </param>
        /// <param name="currentAction">
        /// The current action.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item to get a description for.
        /// </typeparam>
        /// <returns>
        /// The description as a <see cref="string"/>.
        /// </returns>
        private static string GetDescription<TItem>(TItem targetItem, string currentAction)
        {
            string itemType = Evaluate.IsNull(targetItem) ? typeof(TItem).ToRuntimeName() : targetItem.GetType().ToRuntimeName();
            return String.Format(DescriptionFormat, currentAction, itemType, Convert.ToString(targetItem));
        }
    }
}