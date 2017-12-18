// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IErrorMapping.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.ActionTracking
{
    using System;

    /// <summary>
    /// Provides an interface for classes that define a mapping from an exception to normalized data.
    /// </summary>
    public interface IErrorMapping
    {
        /// <summary>
        /// Maps an exception to an <see cref="ErrorInfo"/> value.
        /// </summary>
        /// <param name="action">
        /// The action that caused the error.
        /// </param>
        /// <param name="actionError">
        /// The error to map.
        /// </param>
        /// <returns>
        /// An <see cref="ErrorInfo"/> value for the specified error.
        /// </returns>
        ErrorInfo MapErrorInfo(string action, Exception actionError);
    }
}