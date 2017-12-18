// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorInfo.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.ActionTracking
{
    using System;
    using System.Collections.Generic;

    using SAF.Core;

    /// <summary>
    /// Contains information about an exception.
    /// </summary>
    public struct ErrorInfo : IEquatable<ErrorInfo>
    {
        #region Static Fields

        /// <summary>
        /// Represents an empty error message.
        /// </summary>
        public static readonly ErrorInfo Empty = new ErrorInfo();

        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<ErrorInfo, object>[] ComparisonProperties =
            {
                item => item.ErrorCode, 
                item => item.ErrorData, 
                item => item.ErrorMessage, 
                item => item.ErrorType, 
                item => item.FullErrorOutput
            };

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInfo"/> struct.
        /// </summary>
        /// <param name="errorCode">
        /// The error code.
        /// </param>
        /// <param name="errorType">
        /// The error type.
        /// </param>
        /// <param name="errorMessage">
        /// The error message.
        /// </param>
        /// <param name="errorData">
        /// The error data.
        /// </param>
        /// <param name="fullErrorOutput">
        /// The full output of the error.
        /// </param>
        public ErrorInfo(string errorCode, string errorType, string errorMessage, string errorData, string fullErrorOutput)
            : this()
        {
            if (String.IsNullOrWhiteSpace(errorCode))
            {
                throw new ArgumentNullException("errorCode");
            }

            if (String.IsNullOrWhiteSpace(errorType))
            {
                throw new ArgumentNullException("errorType");
            }

            if (String.IsNullOrWhiteSpace(errorMessage))
            {
                throw new ArgumentNullException("errorMessage");
            }

            this.ErrorCode = errorCode;
            this.ErrorData = errorData;
            this.ErrorMessage = errorMessage;
            this.ErrorType = errorType;
            this.FullErrorOutput = fullErrorOutput;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the error code.
        /// </summary>
        public string ErrorCode { get; private set; }

        /// <summary>
        /// Gets the error data.
        /// </summary>
        public string ErrorData { get; private set; }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// Gets the error type.
        /// </summary>
        public string ErrorType { get; private set; }

        /// <summary>
        /// Gets the full error output.
        /// </summary>
        public string FullErrorOutput { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Determines if two objects of the same type are equal.
        /// </summary>
        /// <param name="valueA">
        /// The baseline value.
        /// </param>
        /// <param name="valueB">
        /// The comparison value.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the values are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(ErrorInfo valueA, ErrorInfo valueB)
        {
            return EqualityComparer<ErrorInfo>.Default.Equals(valueA, valueB);
        }

        /// <summary>
        /// Determines if two objects of the same type are not equal.
        /// </summary>
        /// <param name="valueA">
        /// The baseline value.
        /// </param>
        /// <param name="valueB">
        /// The comparison value.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the values are not equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(ErrorInfo valueA, ErrorInfo valueB)
        {
            return !(valueA == valueB);
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
            return Evaluate.Equals(this, obj);
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
        public bool Equals(ErrorInfo other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
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
            return Evaluate.GenerateHashCode(this, ComparisonProperties);
        }

        /// <summary>
        /// Returns the error message contained by this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> containing the error message for the current instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return this.ErrorMessage;
        }

        #endregion
    }
}