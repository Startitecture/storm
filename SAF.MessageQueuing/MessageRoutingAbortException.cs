// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageRoutingAbortException.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The exception that is thrown when message routing fails in a message routing component.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    using SAF.Core;

    /// <summary>
    /// The exception that is thrown when message routing fails in a message routing component.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of message processed by the component.
    /// </typeparam>
    [Serializable]
    public class MessageRoutingAbortException<TMessage> : DomainException
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        /// <summary>
        /// The pending items.
        /// </summary>
        private readonly MessageRoutingRequestCollection<TMessage> pendingItems;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRoutingAbortException{TMessage}"/> class. 
        /// </summary>
        public MessageRoutingAbortException()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRoutingAbortException{TMessage}"/> class. 
        /// </summary>
        /// <param name="message">
        /// A message describing the exception.
        /// </param>
        public MessageRoutingAbortException(string message)
            : this(message, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRoutingAbortException{TMessage}"/> class. 
        /// </summary>
        /// <param name="message">
        /// A message describing the exception.
        /// </param>
        /// <param name="innerException">
        /// The underlying exception that caused the exception.
        /// </param>
        public MessageRoutingAbortException(string message, Exception innerException)
            : this(message, null, new MessageRoutingRequestCollection<TMessage>(), innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRoutingAbortException{TMessage}"/> class. 
        /// </summary>
        /// <param name="message">
        /// A message describing the exception.
        /// </param>
        /// <param name="abortedItem">
        /// The item that caused the queue to abort.
        /// </param>
        /// <param name="pendingItems">
        /// The items pending in the queue at the time it was aborted.
        /// </param>
        /// <param name="innerException">
        /// The underlying exception that caused the exception.
        /// </param>
        public MessageRoutingAbortException(
            string message, 
            MessageRoutingRequest<TMessage> abortedItem, 
            // ReSharper disable once ParameterTypeCanBeEnumerable.Local
            MessageRoutingRequestCollection<TMessage> pendingItems, 
            Exception innerException)
            : base(message, innerException)
        {
            this.AbortedItem = abortedItem;
            this.pendingItems = pendingItems;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRoutingAbortException{TMessage}"/> class. 
        /// </summary>
        /// <param name="info">
        /// The System.Runtime.Serialization.SerializationInfo that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The System.Runtime.Serialization.StreamingContext that contains contextual information about the source or destination.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// The info parameter is null.
        /// </exception>
        /// <exception cref="System.Runtime.Serialization.SerializationException">
        /// The class name is null or 
        /// System.Exception.HResult is zero (0).
        /// </exception>
        protected MessageRoutingAbortException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.pendingItems = new MessageRoutingRequestCollection<TMessage>();
        }

        /// <summary>
        /// Gets the request that was being processed when the queue aborted.
        /// </summary>
        public MessageRoutingRequest<TMessage> AbortedItem { get; private set; }

        /// <summary>
        /// Gets the unprocessed requests from the queue.
        /// </summary>
        public MessageRoutingRequestCollection<TMessage> PendingItems
        {
            get
            {
                return this.pendingItems;
            }
        }

        /// <summary>
        /// Adds directive information to the exception.
        /// </summary>
        /// <param name="info">
        /// The System.Runtime.Serialization.SerializationInfo that holds the serialized object 
        /// data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The System.Runtime.Serialization.StreamingContext that contains contextual 
        /// information about the source or destination.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// The info parameter is null.
        /// </exception>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("AbortedItem", this.AbortedItem);
            info.AddValue("PendingItems", this.PendingItems);
        }
    }
}
