// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowsAccountInfo.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Contains information for Windows accounts.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Security
{
    using System;
    using System.Collections.Generic;
    using System.DirectoryServices.AccountManagement;
    using System.Security.Principal;

    using JetBrains.Annotations;

    /// <summary>
    /// Contains information for Windows accounts.
    /// </summary>
    public class WindowsAccountInfo : AccountInfo
    {
        /// <summary>
        /// The to string format.
        /// </summary>
        private const string ToStringFormat = "{0} ({1})";

        /// <summary>
        /// The account name.
        /// </summary>
        private readonly string accountName;

        /// <summary>
        /// The full name.
        /// </summary>
        private readonly string fullName;

        /// <summary>
        /// The title.
        /// </summary>
        private readonly string title = null;

        /// <summary>
        /// The email.
        /// </summary>
        private readonly string email;

        /// <summary>
        /// The group memberships.
        /// </summary>
        private readonly List<GroupMembership> groupMemberships = new List<GroupMembership>();

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsAccountInfo"/> class.
        /// </summary>
        /// <param name="identity">
        /// The identity.
        /// </param>
        public WindowsAccountInfo(IIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException("identity");
            }

            UserPrincipal principal;

            using (var machineContext = new PrincipalContext(ContextType.Machine))
            using (var domainContext = new PrincipalContext(ContextType.Domain))
            {
                principal = UserPrincipal.FindByIdentity(domainContext, identity.Name)
                            ?? UserPrincipal.FindByIdentity(machineContext, identity.Name);
            }

            if (principal == null)
            {
                this.accountName = identity.Name;
                this.fullName = identity.Name;
            }
            else
            {
                this.accountName = principal.UserPrincipalName;
                this.fullName = principal.DisplayName;
                this.email = principal.EmailAddress;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsAccountInfo"/> class.
        /// </summary>
        /// <param name="principal">
        /// The windows principal that is the basis for the account information.
        /// </param>
        public WindowsAccountInfo(IPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException("principal");
            }

            UserPrincipal userPrincipal;
            var identity = principal.Identity;

            using (var machineContext = new PrincipalContext(ContextType.Machine))
            using (var domainContext = new PrincipalContext(ContextType.Domain))
            {
                userPrincipal = UserPrincipal.FindByIdentity(domainContext, identity.Name)
                            ?? UserPrincipal.FindByIdentity(machineContext, identity.Name);
            }

            if (userPrincipal == null)
            {
                this.accountName = identity.Name;
                this.fullName = identity.Name;
            }
            else
            {
                this.accountName = userPrincipal.UserPrincipalName;
                this.fullName = userPrincipal.DisplayName;
                this.email = userPrincipal.EmailAddress;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsAccountInfo"/> class.
        /// </summary>
        /// <param name="principal">
        /// The principal that is the basis for the account information.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="principal"/> is null.
        /// </exception>
        public WindowsAccountInfo([NotNull] Principal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException("principal");
            }

            this.accountName = principal.UserPrincipalName ?? principal.SamAccountName;
            this.email = principal.UserPrincipalName;
            this.fullName = principal.DisplayName;
        }

        /// <summary>
        /// Gets the name of the account.
        /// </summary>
        public override string AccountName
        {
            get
            {
                return this.accountName;
            }
        }

        /// <summary>
        /// Gets the full name of the user.
        /// </summary>
        public override string FullName
        {
            get
            {
                return this.fullName;
            }
        }

        /// <summary>
        /// Gets the job title of the user.
        /// </summary>
        public override string Title
        {
            get
            {
                return this.title;
            }
        }

        /// <summary>
        /// Gets the user's email address.
        /// </summary>
        public override string Email
        {
            get
            {
                return this.email;
            }
        }

        /// <summary>
        /// Gets a list of group memberships for the user account.
        /// </summary>
        public override IEnumerable<GroupMembership> GroupMemberships
        {
            get
            {
                return this.groupMemberships;
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return String.Format(ToStringFormat, this.accountName, this.fullName);
        }
    }
}
