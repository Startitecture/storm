// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserAccount.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Extends the <see cref="UserPrincipal" /> class to include logon count and title attributes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActiveDirectory
{
    using System;
    using System.DirectoryServices.AccountManagement;
    using System.Security.Permissions;

    /// <summary>
    /// Extends the <see cref="UserPrincipal"/> class to include logon count and title attributes.
    /// </summary>
    [DirectoryRdnPrefix("CN")]
    [DirectoryObjectClass("person")]
    public class UserAccount : UserPrincipal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserAccount"/> class.
        /// </summary>
        /// <param name="context">The <see cref="PrincipalContext"/> that specifies the server or domain against which 
        /// operations are performed.</param>
        [PermissionSet(SecurityAction.LinkDemand)]
        public UserAccount(PrincipalContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAccount"/> class.
        /// </summary>
        /// <param name="context">The <see cref="PrincipalContext"/> that specifies the server or domain against which 
        /// operations are performed.</param>
        /// <param name="samAccountName">The SAM account name for this account.</param>
        /// <param name="password">The password for this account.</param>
        /// <param name="enabled">A Boolean value that specifies whether the account is enabled.</param>
        [PermissionSet(SecurityAction.LinkDemand)]
        public UserAccount(PrincipalContext context, string samAccountName, string password, bool enabled)
            : base(context, samAccountName, password, enabled)
        {
        }

        /// <summary>
        /// Gets the number of times the <see cref="UserAccount"/> has logged in.
        /// </summary>
        [DirectoryProperty("logonCount")]
        public int? LogOnCount
        {
            [PermissionSet(SecurityAction.LinkDemand)]
            get
            {
                if (this.ExtensionGet("logonCount").Length != 1)
                {
                    return null;
                }

                return (int?)this.ExtensionGet("logonCount")[0];
            }
        }

        /// <summary>
        /// Gets or sets the title of the <see cref="UserAccount"/>.
        /// </summary>
        [DirectoryProperty("title")]
        public string Title
        {
            [PermissionSet(SecurityAction.LinkDemand)]
            get
            {
                if (this.ExtensionGet("title").Length != 1)
                {
                    return null;
                }

                return Convert.ToString(this.ExtensionGet("title")[0]);
            }

            [PermissionSet(SecurityAction.LinkDemand)]
            set
            {
                this.ExtensionSet("title", value);
            }
        }

        /// <summary>
        /// Gets or sets the title of the <see cref="UserAccount"/>.
        /// </summary>
        [DirectoryProperty("info")]
        public string Info
        {
            [PermissionSet(SecurityAction.LinkDemand)]
            get
            {
                if (this.ExtensionGet("info").Length != 1)
                {
                    return null;
                }

                return Convert.ToString(this.ExtensionGet("info")[0]);
            }

            [PermissionSet(SecurityAction.LinkDemand)]
            set
            {
                this.ExtensionSet("info", value);
            }
        }

        /// <summary>
        /// Returns a principal object that matches the specified identity value.
        /// </summary>
        /// <param name="context">The <see cref="PrincipalContext"/> that specifies the server or domain against which 
        /// operations are performed.</param>
        /// <param name="identityValue">The identity of the principal. This parameter can be any format that is 
        /// contained in the <see cref="IdentityType"/> enumeration.</param>
        /// <returns>A <see cref="EntityGroup"/> object that matches the specified identity value and type, or null if 
        /// no matches are found.</returns>
        [PermissionSet(SecurityAction.LinkDemand)]
        public static new UserAccount FindByIdentity(PrincipalContext context, string identityValue)
        {
            return (UserAccount)FindByIdentityWithType(context, typeof(UserAccount), identityValue);
        }

        /// <summary>
        /// Returns a principal object that matches the specified identity value.
        /// </summary>
        /// <param name="context">The <see cref="PrincipalContext"/> that specifies the server or domain against which 
        /// operations are performed.</param>
        /// <param name="identityType">An <see cref="IdentityType"/> enumeration value that specifies the format of the
        /// <paramref name="identityValue"/> parameter. </param>
        /// <param name="identityValue">The identity of the principal. This parameter can be any format that is 
        /// contained in the <see cref="IdentityType"/> enumeration.</param>
        /// <returns>A <see cref="EntityGroup"/> object that matches the specified identity value and type, or null if 
        /// no matches are found.</returns>
        [PermissionSet(SecurityAction.LinkDemand)]
        public static new UserAccount FindByIdentity(
            PrincipalContext context,
            IdentityType identityType,
            string identityValue)
        {
            return (UserAccount)FindByIdentityWithType(context, typeof(UserAccount), identityType, identityValue);
        }
    }
}
