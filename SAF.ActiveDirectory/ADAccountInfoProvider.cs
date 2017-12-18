// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ADAccountInfoProvider.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides account information from Active Directory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActiveDirectory
{
    using System;
    using System.DirectoryServices.AccountManagement;
    using System.Security.Permissions;
    using System.Security.Principal;

    using SAF.Core;
    using SAF.Security;

    /// <summary>
    /// Provides account information from Active Directory.
    /// </summary>
    public class ADAccountInfoProvider : IAccountInfoProvider
    {
        /// <summary>
        /// Creates an <see cref="ADAccountInfo"/> instance from the specified <see cref="IIdentity"/>.
        /// </summary>
        /// <param name="account">
        /// The account to create the account info instance from.
        /// </param>
        /// <returns>
        /// The account info for the specified identity, or null if the identity cannot be found.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="account"/> is null.
        /// </exception>
        /// <exception cref="NoMatchingPrincipalException">
        /// No account could be found for the specified identity.
        /// </exception>
        [PermissionSet(SecurityAction.LinkDemand)]
        public IAccountInfo GetAccountInfo(IIdentity account)
        {
            if (account == null)
            {
                throw new ArgumentNullException("account");
            }

            return this.GetAccountInfo(account.Name);
        }

        /// <summary>
        /// Creates an <see cref="ADAccountInfo"/> instance from the specified account name.
        /// </summary>
        /// <param name="accountName">
        /// The account name.
        /// </param>
        /// <returns>
        /// The <see cref="ADAccountInfo"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="accountName"/> is null.
        /// </exception>
        /// <exception cref="OperationException">
        /// No account could be found for the specified identity.
        /// </exception>
        [PermissionSet(SecurityAction.LinkDemand)]
        public IAccountInfo GetAccountInfo(string accountName)
        {
            try
            {
                return ADAccountInfo.CreateFrom(accountName);
            }
            catch (NoMatchingPrincipalException ex)
            {
                throw new OperationException(accountName, ex.Message, ex);
            }
        }
    }
}
