// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAccountInfo.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The AccountInfo interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Security
{
    /// <summary>
    /// The AccountInfo interface.
    /// </summary>
    public interface IAccountInfo
    {
        /// <summary>
        /// Gets the name of the account.
        /// </summary>
        string AccountName { get; }

        /// <summary>
        /// Gets the full name of the user.
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// Gets the job title of the user.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Gets the user's email address.
        /// </summary>
        string Email { get; }
    }
}