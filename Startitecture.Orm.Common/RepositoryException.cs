// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RepositoryException.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Thrown when there is an error accessing a data repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common
{
    using System;
    using System.Runtime.Serialization;

    using Startitecture.Core;

    /// <summary>
    /// Thrown when there is an error accessing a data repository.
    /// </summary>
    [Serializable]
    public sealed class RepositoryException : OperationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryException"/> class.
        /// </summary>
        public RepositoryException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryException"/> class with a message describing
        /// the exception.
        /// </summary>
        /// <param name="message">The message describing the exception.</param>
        public RepositoryException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryException"/> class with a message describing
        /// the exception and the underlying exception that caused the <see cref="OperationException"/>.
        /// </summary>
        /// <param name="message">The message describing the exception.</param>
        /// <param name="innerException">The underlying exception that caused the SystemException.</param>
        public RepositoryException(string message, Exception innerException)
            : base(message, (Exception)innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryException"/> class with the business object
        /// associated with the exception.
        /// </summary>
        /// <param name="targetEntity">The business object associated with the exception.</param>
        public RepositoryException(object targetEntity)
            : this(targetEntity, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryException"/> class with the business object
        /// associated with the exception and a message describing the exception.
        /// </summary>
        /// <param name="targetEntity">
        /// The business object associated with the exception.
        /// </param>
        /// <param name="message">
        /// The message describing the exception.
        /// </param>
        public RepositoryException(object targetEntity, string message)
            : this(targetEntity, message, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryException"/> class with the business object
        /// associated with the exception, a message describing the exception and the underlying exception that caused
        /// the <see cref="OperationException"/>.
        /// </summary>
        /// <param name="targetEntity">The business object associated with the exception.</param>
        /// <param name="message">The message describing the exception.</param>
        /// <param name="innerException">The underlying exception that caused the BusinessException.</param>
        public RepositoryException(object targetEntity, string message, Exception innerException)
            : base(targetEntity, message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryException"/> class.
        /// </summary>
        /// <param name="info">The System.Runtime.Serialization.SerializationInfo that holds the serialized object
        /// data about the exception being thrown.</param>
        /// <param name="context">The System.Runtime.Serialization.StreamingContext that contains contextual
        /// information about the source or destination.</param>
        /// <exception cref="System.ArgumentNullException">The info parameter is null.</exception>
        /// <exception cref="System.Runtime.Serialization.SerializationException">The class name is null or
        /// System.Exception.HResult is zero (0).</exception>
        private RepositoryException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}
