// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageDispatchException.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The exception that is thrown when an evaluated message cannot be sent to the message router.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    using SAF.Core;

    /// <summary>
    /// The exception that is thrown when an evaluated message cannot be sent to the message router.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of message being sent.
    /// </typeparam>
    [Serializable]
    public class MessageDispatchException<TMessage> : DomainException
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageDispatchException{TMessage}"/> class. 
        /// </summary>
        public MessageDispatchException()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageDispatchException{TMessage}"/> class. 
        /// the exception.
        /// </summary>
        /// <param name="message">
        /// A message describing the exception.
        /// </param>
        public MessageDispatchException(string message)
            : this(message, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageDispatchException{TMessage}"/> class. 
        /// the exception and the underlying exception.
        /// </summary>
        /// <param name="message">
        /// A message describing the exception.
        /// </param>
        /// <param name="innerException">
        /// The underlying exception that caused the exception.
        /// </param>
        public MessageDispatchException(string message, Exception innerException)
            : this(message, null, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageDispatchException{TMessage}"/> class. 
        /// </summary>
        /// <param name="message">
        /// A message describing the exception.
        /// </param>
        /// <param name="routingRequest">
        /// The item that caused the queue to abort.
        /// </param>
        /// <param name="innerException">
        /// The underlying exception that caused the exception.
        /// </param>
        public MessageDispatchException(
            string message, 
            MessageRoutingRequest<TMessage> routingRequest, 
            Exception innerException)
            : base(message, innerException)
        {
            this.RoutingRequest = routingRequest;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageDispatchException{TMessage}"/> class. 
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
        protected MessageDispatchException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets the request that was being processed when the queue aborted.
        /// </summary>
        public MessageRoutingRequest<TMessage> RoutingRequest { get; private set; }

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

            info.AddValue("RoutingRequest", this.RoutingRequest);
        }
    }
}
