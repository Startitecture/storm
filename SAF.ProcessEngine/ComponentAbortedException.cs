// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComponentAbortedException.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Defines the ComponentAbortedException type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ProcessEngine
{
    using System;
    using System.Runtime.Serialization;

    using SAF.Core;

    /// <summary>
    /// The exception that is thrown when an add method is called on an aborted component.
    /// </summary>
    [Serializable]
    public class ComponentAbortedException : DomainException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentAbortedException"/> class. 
        /// </summary>
        public ComponentAbortedException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentAbortedException"/> class with a message describing the exception.
        /// </summary>
        /// <param name="message">
        /// The message describing the exception.
        /// </param>
        public ComponentAbortedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentAbortedException"/> class with a message describing the exception and the 
        /// underlying exception.
        /// </summary>
        /// <param name="message">
        /// The message describing the exception.
        /// </param>
        /// <param name="innerException">
        /// The underlying exception that caused the exception.
        /// </param>
        public ComponentAbortedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentAbortedException"/> class. 
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
        protected ComponentAbortedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }    
    }
}
