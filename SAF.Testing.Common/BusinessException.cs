// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BusinessException.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Represents an error that is the direct result of invalid commands or values provided by the user.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Testing.Common
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    using SAF.StringResources;

    /// <summary>
    /// Represents an error that is the direct result of invalid commands or values provided by the user.
    /// </summary>
    [Serializable]
    public sealed class BusinessException : Exception
    {
        #region Constants

        /// <summary>
        /// The entity error key format.
        /// </summary>
        private const string EntityErrorKeyFormat = "Entity Error {0}";

        /// <summary>
        /// The no errors message.
        /// </summary>
        private const string NoErrorsMessage = "No errors specified.";

        /// <summary>
        /// The target entity key.
        /// </summary>
        private const string TargetEntityKey = "Target Entity";

        #endregion

        #region Fields

        /// <summary>
        /// A collection of error messages associated with the entity.
        /// </summary>
        private readonly StringCollection entityErrors = new StringCollection();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessException" /> class.
        /// </summary>
        public BusinessException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessException"/> class with a message describing
        /// the exception.
        /// </summary>
        /// <param name="message">
        /// The message describing the exception.
        /// </param>
        public BusinessException(string message)
            : base(message)
        {
            this.entityErrors.Add(message);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessException"/> class with a message describing
        /// the exception and the underlying exception that caused the <see cref="BusinessException"/>.
        /// </summary>
        /// <param name="message">
        /// The message describing the exception.
        /// </param>
        /// <param name="innerException">
        /// The underlying exception that caused the BusinessException.
        /// </param>
        public BusinessException(string message, Exception innerException)
            : base(message, innerException)
        {
            this.entityErrors.Add(message);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessException"/> class with the business object
        /// associated with the exception and a message describing the exception.
        /// </summary>
        /// <param name="targetEntity">
        /// The business object associated with the exception.
        /// </param>
        /// <param name="message">
        /// The message describing the exception.
        /// </param>
        public BusinessException(object targetEntity, string message)
            : this(targetEntity, message, new[] { message })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessException"/> class with the business object
        /// associated with the exception and a message describing the exception.
        /// </summary>
        /// <param name="targetEntity">
        /// The business object associated with the exception.
        /// </param>
        /// <param name="entityErrors">
        /// A list of errors associated with the exception.
        /// </param>
        public BusinessException(object targetEntity, IList<string> entityErrors)
            : this(targetEntity, CreateMessage(entityErrors), null, entityErrors)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessException"/> class with the business object
        /// associated with the exception and a message describing the exception.
        /// </summary>
        /// <param name="targetEntity">
        /// The business object associated with the exception.
        /// </param>
        /// <param name="entityErrors">
        /// A list of errors associated with the exception.
        /// </param>
        /// <param name="innerException">
        /// The underlying exception that caused the BusinessException.
        /// </param>
        public BusinessException(object targetEntity, Collection<string> entityErrors, Exception innerException)
            : this(targetEntity, CreateMessage(entityErrors), innerException, entityErrors)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessException"/> class with the business object
        /// associated with the exception, a message describing the exception and the underlying exception that caused
        /// the <see cref="BusinessException"/>.
        /// </summary>
        /// <param name="targetEntity">
        /// The business object associated with the exception.
        /// </param>
        /// <param name="message">
        /// The message describing the exception.
        /// </param>
        /// <param name="innerException">
        /// The underlying exception that caused the BusinessException.
        /// </param>
        public BusinessException(object targetEntity, string message, Exception innerException)
            : this(targetEntity, message, innerException, new List<string> { message })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessException"/> class with the business object
        /// associated with the exception, a message describing the exception and the underlying exception that caused
        /// the <see cref="BusinessException"/>.
        /// </summary>
        /// <param name="targetEntity">
        /// The business object associated with the exception.
        /// </param>
        /// <param name="message">
        /// The message describing the exception.
        /// </param>
        /// <param name="innerException">
        /// The underlying exception that caused the BusinessException.
        /// </param>
        /// <param name="entityErrors">
        /// A collection of errors associated with the exception.
        /// </param>
        private BusinessException(object targetEntity, string message, Exception innerException, IEnumerable<string> entityErrors)
            : base(message, innerException)
        {
            this.TargetEntity = targetEntity;
            this.Data.Add(TargetEntityKey, Convert.ToString(this.TargetEntity));

            IEnumerable<string> errors = entityErrors as IList<string> ?? entityErrors.ToList();

            if (errors.Any())
            {
                this.entityErrors.AddRange(errors.ToArray());

                int index = 0;

                foreach (string error in errors)
                {
                    this.Data.Add(string.Format(EntityErrorKeyFormat, index), error);
                    index++;
                }
            }
            else
            {
                this.entityErrors.Add(message);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessException"/> class with the business object
        /// associated with the exception and a message describing the exception.
        /// </summary>
        /// <param name="targetEntity">
        /// The business object associated with the exception.
        /// </param>
        /// <param name="message">
        /// The message describing the exception.
        /// </param>
        /// <param name="entityErrors">
        /// A list of errors associated with the exception.
        /// </param>
        private BusinessException(object targetEntity, string message, IEnumerable<string> entityErrors)
            : this(targetEntity, message, null, entityErrors)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessException"/> class.
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
        /// <exception cref="System.Runtime.Serialization.SerializationException">
        /// The class name is null or
        /// System.Exception.HResult is zero (0).
        /// </exception>
        private BusinessException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a collection of errors related to the business entity.
        /// </summary>
        public StringCollection EntityErrors => this.entityErrors;

        /// <summary>
        /// Gets the business object associated with the exception.
        /// </summary>
        public object TargetEntity { get; private set; }

        #endregion

        #region Public Methods and Operators

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

            info.AddValue("TargetEntity", Convert.ToString(this.TargetEntity));
            info.AddValue("EntityErrors", this.EntityErrors);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a message based on the target item and entity errors.
        /// </summary>
        /// <param name="entityErrors">
        /// The entity errors.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string CreateMessage(IEnumerable<string> entityErrors)
        {
            IList<string> errors = entityErrors as IList<string> ?? entityErrors.ToList();
            return errors.Count > 1
                       ? string.Format(ValidationMessages.EntityValidationFailedMultiple, errors.Count, errors.First())
                       : string.Format(ValidationMessages.EntityValidationFailedSingle, errors.FirstOrDefault() ?? NoErrorsMessage);
        }

        #endregion
    }
}