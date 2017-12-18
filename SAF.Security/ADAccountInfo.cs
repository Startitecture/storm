namespace SAF.Security
{
    using System;
    using System.Collections.Generic;
    using System.DirectoryServices;
    using System.DirectoryServices.AccountManagement;
    using System.Security.Principal;

    using SAF.ActiveDirectory;

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
        /// The email address of the accout.
        /// </summary>
        private string email;

        /// <summary>
        /// The groups that this account is a member of.
        /// </summary>
        private List<GroupMembership> memberships = new List<GroupMembership>();

        /// <summary>
        /// Prevents a default instance of the <see cref="ADAccountInfo"/> class from being created.
        /// </summary>
        private ADAccountInfo()
        {
        }

        /// <summary>
        /// Gets the name of the account.
        /// </summary>
        public override string AccountName
        {
            get { return this.accountName; }
        }

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
        /// Creates an <see cref="ADAccountInfo"/> instance from the specified <see cref="IIdentity"/>.
        /// </summary>
        /// <param name="account">The account to create the account info instance from.</param>
        /// <returns>The account info for the specified identity, or null if the identity cannot be found.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="account"/> is null.</exception>
        /// <exception cref="NoMatchingPrincipalException">No account could be found for the specified identity.</exception>
        internal static ADAccountInfo CreateFrom(IIdentity account)
        {
            if (account == null)
            {
                throw new ArgumentNullException("account");
            }

            using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
            using (UserAccount principal = UserAccount.FindByIdentity(context, IdentityType.SamAccountName, account.Name))
            {
                if (principal == null)
                {
                    throw new NoMatchingPrincipalException(String.Format("The account '{0}' could not be found.", account.Name));
                }

                List<GroupMembership> groupMembership = new List<GroupMembership>();

                // Somehow, the groups returned by GetAuthorizationGroups() are being cached. Using GetGroups is no help either. This
                // approach is required to ensure group membership is reflected immediately (barring AD replication).
                DirectoryEntry userEntry = (DirectoryEntry)principal.GetUnderlyingObject();

                foreach (string group in ObjectProperty.GetValueStrings(userEntry, ObjectProperty.MemberOf))
                {
                    using (EntityGroup entityGroup = EntityGroup.FindByIdentity(context, IdentityType.DistinguishedName, group))
                    {
                        groupMembership.Add(
                            new GroupMembership(
                                entityGroup.EntityType.HasValue ? entityGroup.EntityType.Value : 0,
                                entityGroup.EntityId.HasValue ? entityGroup.EntityId.Value : 0,
                                entityGroup.SamAccountName));
                    }
                }

                return new ADAccountInfo()
                {
                    accountName = principal.SamAccountName,
                    email = principal.EmailAddress,
                    fullName = principal.Name,
                    title = principal.Title,
                    memberships = groupMembership
                };
            }
        }
    }
}
