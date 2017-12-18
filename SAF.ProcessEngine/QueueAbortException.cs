// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueueAbortException.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.ProcessEngine
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    using SAF.Core;

    /// <summary>
    /// The exception that is thrown when an add method is called on an aborted queue.
    /// </summary>
    /// <typeparam name="TItem">
    /// The type of item processed by the queue.
    /// </typeparam>
    [Serializable]
    public class QueueAbortException<TItem> : DomainException
    {
        /// <summary>
        /// The aborted items.
        /// </summary>
        private readonly List<TItem> pendingItems = new List<TItem>();

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueAbortException{TItem}"/> class. 
        /// Initializes a new instance of the <see cref="T:SAF.ProcessEngine.QueueAbortException`1"/> class. 
        /// </summary>
        public QueueAbortException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueAbortException{TItem}"/> class. 
        /// Initializes a new instance of the <see cref="T:SAF.ProcessEngine.QueueAbortException`1"/> class with a message describing 
        /// the exception.
        /// </summary>
        /// <param name="message">
        /// A message describing the exception.
        /// </param>
        public QueueAbortException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueAbortException{TItem}"/> class. 
        /// Initializes a new instance of the <see cref="T:SAF.ProcessEngine.QueueAbortException`1"/> class with a message describing 
        /// the exception and the underlying exception.
        /// </summary>
        /// <param name="message">
        /// A message describing the exception.
        /// </param>
        /// <param name="innerException">
        /// The underlying exception that caused the exception.
        /// </param>
        public QueueAbortException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueAbortException{TItem}"/> class.
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
        public QueueAbortException(string message, TItem abortedItem, IEnumerable<TItem> pendingItems, Exception innerException)
            : base(message, innerException)
        {
            this.AbortedItem = abortedItem;
            this.pendingItems.AddRange(pendingItems);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueAbortException{TItem}"/> class. 
        /// Initializes a new instance of the <see cref="T:SAF.ProcessEngine.QueueAbortException`1"/> class.
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
        protected QueueAbortException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets the request that was being processed when the queue aborted.
        /// </summary>
        public TItem AbortedItem { get; private set; }

        /// <summary>
        /// Gets the unprocessed requests from the queue.
        /// </summary>
        public IEnumerable<TItem> PendingItems
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
