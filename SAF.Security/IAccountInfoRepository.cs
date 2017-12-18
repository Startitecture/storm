// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAccountInfoRepository.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface to user store provider classes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Security
{
    using SAF.Core;

    /// <summary>
    /// Provides an interface to user store provider classes.
    /// </summary>
    public interface IAccountInfoRepository
    {
        /// <summary>
        /// Gets the user account by the user account ID.
        /// </summary>
        /// <param name="userAccountId">
        /// The user account ID.
        /// </param>
        /// <returns>
        /// A <see cref="AccountInfo"/> instance for the specified user account ID.
        /// </returns>
        IAccountInfo GetAccountInfo(int userAccountId);
    }
}