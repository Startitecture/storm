// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageCanceledException.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   An exception that is thrown when a message is canceled by user action.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;
    using System.Security.Permissions;

    using SAF.Core;

    /// <summary>
    /// An exception that is thrown when a message is canceled by user action.
    /// </summary>
    [Serializable]
    public sealed class MessageCanceledException : DomainException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageCanceledException"/> class.
        /// </summary>
        public MessageCanceledException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageCanceledException"/> class with a message describing 
        /// the exception.
        /// </summary>
        /// <param name="message">The message describing the exception.</param>
        public MessageCanceledException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageCanceledException"/> class with a message describing 
        /// the exception and the underlying exception that caused the <see cref="MessageCanceledException"/>.
        /// </summary>
        /// <param name="message">The message describing the exception.</param>
        /// <param name="innerException">The underlying exception that caused the SystemException.</param>
        public MessageCanceledException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageCanceledException"/> class with the business object 
        /// associated with the exception.
        /// </summary>
        /// <param name="targetEntity">The business object associated with the exception.</param>
        public MessageCanceledException(object targetEntity)
            : this(targetEntity, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageCanceledException"/> class with the business object 
        /// associated with the exception and a message describing the exception.
        /// </summary>
        /// <param name="targetEntity">
        /// The business object associated with the exception.
        /// </param>
        /// <param name="message">
        /// The message describing the exception.
        /// </param>
        public MessageCanceledException(object targetEntity, string message)
            : this(targetEntity, message, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageCanceledException"/> class with the business object 
        /// associated with the exception, a message describing the exception and the underlying exception that caused 
        /// the <see cref="MessageCanceledException"/>.
        /// </summary>
        /// <param name="targetEntity">The business object associated with the exception.</param>
        /// <param name="message">The message describing the exception.</param>
        /// <param name="innerException">The underlying exception that caused the BusinessException.</param>
        public MessageCanceledException(object targetEntity, string message, Exception innerException)
            : base(message, innerException)
        {
            this.TargetEntity = targetEntity;
            this.Data.PopulateDictionary(this.TargetEntity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageCanceledException"/> class.
        /// </summary>
        /// <param name="info">The System.Runtime.Serialization.SerializationInfo that holds the serialized object 
        /// data about the exception being thrown.</param>
        /// <param name="context">The System.Runtime.Serialization.StreamingContext that contains contextual 
        /// information about the source or destination.</param>
        /// <exception cref="System.ArgumentNullException">The info parameter is null.</exception>
        /// <exception cref="System.Runtime.Serialization.SerializationException">The class name is null or 
        /// System.Exception.HResult is zero (0).</exception>
        private MessageCanceledException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets the business object associated with the exception.
        /// </summary>
        public object TargetEntity { get; private set; }

        /// <summary>
        /// Adds directive information to the exception.
        /// </summary>
        /// <param name="info">The System.Runtime.Serialization.SerializationInfo that holds the serialized object 
        /// data about the exception being thrown.</param>
        /// <param name="context">The System.Runtime.Serialization.StreamingContext that contains contextual 
        /// information about the source or destination.</param>
        /// <exception cref="System.ArgumentNullException">The info parameter is null.</exception>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(
            System.Runtime.Serialization.SerializationInfo info, 
            System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("TargetEntity", this.TargetEntity);
        }
    }
}
