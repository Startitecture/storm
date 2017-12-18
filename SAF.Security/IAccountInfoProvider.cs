// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAccountInfoProvider.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface for retrieving account information for <see cref="IIdentity" /> identities.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Security
{
    using System.Security.Principal;

    using SAF.Core;

    /// <summary>
    /// Provides an interface for retrieving account information for <see cref="IIdentity"/> identities.
    /// </summary>
    public interface IAccountInfoProvider
    {
        /// <summary>
        /// Returns a <see cref="SAF.Security.AccountInfo"/> instance for the specified identity.
        /// </summary>
        /// <param name="account">
        /// The account identity.
        /// </param>
        /// <returns>
        /// A <see cref="SAF.Security.AccountInfo"/> based on the specified identity.
        /// </returns>
        IAccountInfo GetAccountInfo(IIdentity account);

        /// <summary>
        /// Returns a <see cref="SAF.Security.AccountInfo"/> instance for the specified account name.
        /// </summary>
        /// <param name="accountName">
        /// The account name.
        /// </param>
        /// <returns>
        /// A <see cref="SAF.Security.AccountInfo"/> based on the specified account name.
        /// </returns>
        IAccountInfo GetAccountInfo(string accountName);
    }
}
