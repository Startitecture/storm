// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowsAccountInfoProvider.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides Windows account information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Security
{
    using System;
    using System.Diagnostics;
    using System.DirectoryServices.AccountManagement;
    using System.Security;
    using System.Security.Principal;

    using SAF.Core;
    using SAF.StringResources;

    /// <summary>
    /// Provides Windows account information.
    /// </summary>
    public class WindowsAccountInfoProvider : IAccountInfoProvider
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
        public IAccountInfo GetAccountInfo(IIdentity account)
        {
            if (account == null)
            {
                throw new ArgumentNullException("account");
            }

            try
            {
                return new WindowsAccountInfo(account);
            }
            catch (SecurityException)
            {
                return new WindowsAccountInfo(new GenericIdentity(account.Name));
            }
        }

        /// <summary>
        /// Returns a <see cref="SAF.Security.AccountInfo"/> instance for the specified account name.
        /// </summary>
        /// <param name="accountName">
        /// The account name.
        /// </param>
        /// <returns>
        /// A <see cref="SAF.Security.AccountInfo"/> based on the specified account name.
        /// </returns>
        public IAccountInfo GetAccountInfo(string accountName)
        {
            if (accountName == null)
            {
                throw new ArgumentNullException("accountName");
            }

            // First try with just the account name.
            try
            {
                using (var identity = new WindowsIdentity(accountName))
                {
                    return new WindowsAccountInfo(identity);
                }
            }
            catch (SecurityException ex)
            {
                Trace.TraceError(ex.Message);
            }

            // Next see if it is a local NT account of some type.
            var user = new NTAccount(accountName);
            IdentityReference sid;

            try
            {
                sid = user.Translate(typeof(SecurityIdentifier));
            }
            catch (IdentityNotMappedException ex)
            {
                var message = String.Format(ErrorMessages.AccountCouldNotBeIdentified, accountName, ex.Message);
                throw new OperationException(accountName, message, ex);
            }

            using (var machineContext = new PrincipalContext(ContextType.Machine))
            using (var domainContext = new PrincipalContext(ContextType.Domain))
            {
                // For IIS APPPOOL accounts, the user SID resolves to a group prinicpal.
                var principal = Principal.FindByIdentity(machineContext, IdentityType.Sid, sid.Value)
                                ?? Principal.FindByIdentity(domainContext, IdentityType.Sid, sid.Value);

                if (principal != null)
                {
                    return new WindowsAccountInfo(principal);
                }
            }

            // WindowsIdentity is fine with user principal name (UPN, user@domain.net) but does not like DOMAIN\user which is naturally
            // the format provided in its own Name property. In the case where the @ symbol is present, we assume UPN. Otherwise we 
            // substring after the first index of the backslash. This works even if the backslash is missing because IndexOf returns -1
            // when it does not find anything, and -1 + 1 = 0. 
            var safeAccountName = accountName.IndexOf('@') > 0 ? accountName : accountName.Substring(accountName.IndexOf('\\') + 1);

            try
            {
                using (var identity = new WindowsIdentity(safeAccountName))
                {
                    return new WindowsAccountInfo(new WindowsPrincipal(identity));
                }
            }
            catch (SecurityException ex)
            {
                var message = String.Format(ErrorMessages.AccountCouldNotBeIdentified, safeAccountName, ex.Message);
                throw new OperationException(accountName, message, ex);
            }
        }
    }
}
