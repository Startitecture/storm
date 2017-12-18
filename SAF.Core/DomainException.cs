// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainException.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides a base exception for all explicitly-thrown framework exceptions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Core
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides a base exception for all explicitly-thrown framework exceptions. 
    /// </summary>
    [Serializable]
    public class DomainException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DomainException"/> class. 
        /// </summary>
        public DomainException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainException"/> class with a message describing the exception.
        /// </summary>
        /// <param name="message">
        /// The message describing the exception.
        /// </param>
        public DomainException(string message)
            : this(message, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainException"/> class with a message describing the exception and the 
        /// underlying exception.
        /// </summary>
        /// <param name="message">
        /// The message describing the exception.
        /// </param>
        /// <param name="innerException">
        /// The underlying exception that caused the exception.
        /// </param>
        public DomainException(string message, Exception innerException)
            : base(message, innerException)
        {
            if (innerException is DomainException exception)
            {
                this.CorrelationId = exception.CorrelationId;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainException"/> class. 
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
        protected DomainException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets or sets the correlation ID for the exception.
        /// </summary>
        public Guid CorrelationId { get; set; }

        /// <summary>
        /// When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with 
        /// information about the exception.
        /// </summary>
        /// <param name="info">
        /// The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the 
        /// exception being thrown. 
        /// </param>
        /// <param name="context">
        /// The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or
        /// destination. </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is a null reference (Nothing in Visual Basic). </exception>
        /// <filterpriority>2</filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*"/>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter"/>
        /// </PermissionSet>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("CorrelationId", this.CorrelationId);
        }
    }
}