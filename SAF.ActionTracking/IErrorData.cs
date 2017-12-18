// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IErrorData.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface for classes that contain error data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActionTracking
{
    /// <summary>
    /// Provides an interface for classes that contain error data.
    /// </summary>
    public interface IErrorData
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the sequential application error ID.
        /// </summary>
        long ApplicationErrorId { get; set; }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        string ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the error data.
        /// </summary>
        string ErrorData { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the error type.
        /// </summary>
        string ErrorType { get; set; }

        /// <summary>
        /// Gets or sets the full error output.
        /// </summary>
        string FullErrorOutput { get; set; }

        #endregion
    }
}