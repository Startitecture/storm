// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ADAccountInfo.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Represents an Active Directory account.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActiveDirectory
{
    using System;
    using System.Collections.Generic;
    using System.DirectoryServices;
    using System.DirectoryServices.AccountManagement;

    using SAF.Security;

    /// <summary>
    /// Represents an Active Directory account.
    /// </summary>
    public class ADAccountInfo : AccountInfo
    {
        /// <summary>
        /// The name of the account.
        /// </summary>
        private string accountName;

        /// <summary>
        /// The full name of the account.
        /// </summary>
        private string fullName;

        /// <summary>
        /// The title of the user.
        /// </summary>
        private string title;

        /// <summary>
        /// The email address of the account.
        /// </summary>
        private string email;

        /// <summary>
        /// The groups that this account is a member of.
        /// </summary>
        private List<GroupMembership> memberships = new List<GroupMembership>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ADAccountInfo"/> class. 
        /// </summary>
        protected ADAccountInfo()
        {
        }

        /// <summary>
        /// Gets the name of the account.
        /// </summary>
        public override string AccountName
        {
            get { return this.accountName; }
        }

        /////// <summary>
        /////// Gets the domain of the account.
        /////// </summary>
        ////public string Domain { get; private set; }

        /// <summary>
        /// Gets the full name of the user.
        /// </summary>
        public override string FullName
        {
            get { return this.fullName; }
        }

        /// <summary>
        /// Gets the job title of the user.
        /// </summary>
        public override string Title
        {
            get { return this.title; }
        }

        /// <summary>
        /// Gets the user's email address.
        /// </summary>
        public override string Email
        {
            get { return this.email; }
        }

        /// <summary>
        /// Gets the groups that this account is a member of.
        /// </summary>
        public override IEnumerable<GroupMembership> GroupMemberships
        {
            get { return this.memberships; }
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
        /// <exception cref="NoMatchingPrincipalException">
        /// No account could be found for the specified identity.
        /// </exception>
        internal static ADAccountInfo CreateFrom(string accountName)
        {
            if (String.IsNullOrWhiteSpace(accountName))
            {
                throw new ArgumentNullException("accountName");
            }

            IdentityType identityType = accountName.Contains("@") ? IdentityType.UserPrincipalName : IdentityType.SamAccountName;

            using (var context = new PrincipalContext(ContextType.Domain))
            using (var principal = UserAccount.FindByIdentity(context, identityType, accountName))
            {
                if (principal == null)
                {
                    throw new NoMatchingPrincipalException(String.Format(ErrorMessages.AccountCouldNotBeFound, accountName));
                }

                var groupMembership = new List<GroupMembership>();

                // Somehow, the groups returned by GetAuthorizationGroups() are being cached. Using GetGroups is no help either. This
                // approach is required to ensure group membership is reflected immediately (barring AD replication).
                var userEntry = (DirectoryEntry)principal.GetUnderlyingObject();

                foreach (string membership in ObjectProperty.GetValueStrings(userEntry, ObjectProperty.MemberOf))
                {
                    using (EntityGroup entityGroup = EntityGroup.FindByIdentity(context, IdentityType.DistinguishedName, membership))
                    {
                        // TODO: Remove EntityType/Id.
                        groupMembership.Add(
                            new GroupMembership(
                                entityGroup.EntityType.GetValueOrDefault(0),
                                entityGroup.EntityId.GetValueOrDefault(0),
                                entityGroup.SamAccountName));
                    }
                }

                return new ADAccountInfo
                           {
                               accountName = principal.UserPrincipalName,
                               email = principal.EmailAddress,
                               fullName = principal.Name,
                               title = principal.Title,
                               memberships = groupMembership,
                               //// TODO: Domain doesn't work here.
                           };
            }
        }
    }
}
