namespace SAF.Security
{
    using System;
    using System.DirectoryServices.AccountManagement;
    using System.Security.Permissions;
    using System.Security.Principal;

    /// <summary>
    /// Provides account information from Active Directory.
    /// </summary>
    public class ADAccountInfoProvider : IAccountInfoProvider
    {
        /// <summary>
        /// Retrieves an <see cref="AccountInfo"/> object based on the specified <see cref="IIdentity"/>.
        /// </summary>
        /// <param name="account">The account to retrieve information for.</param>
        /// <returns>An <see cref="AccountInfo"/> instance with information about the specified account.</returns>
        /// <exception cref="ArgumentNullException">Account is null.</exception>
        /// <exception cref="NoMatchingPrincipalException">No account could be found for the specified identity.</exception>
        [PermissionSet(SecurityAction.LinkDemand)]
        public AccountInfo GetAccountInfo(IIdentity account)
        {
            return ADAccountInfo.CreateFrom(account);
        }
    }
}
