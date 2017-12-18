// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserManagement.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Contains methods and properties related to Active Directory user management.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActiveDirectory
{
    using System;
    using System.DirectoryServices;
    using System.DirectoryServices.AccountManagement;
    using System.Security;
    using System.Security.Permissions;

    using SAF.Core;

    /// <summary>
    /// Contains methods and properties related to Active Directory user management.
    /// </summary>
    public static class UserManagement
    {
        /// <summary>
        /// Integer value of the timestamp for users whose accounts never expire.
        /// </summary>
        public const long AccountNeverExpiresTimestamp = 504911052000000000;

        /// <summary>
        /// Creates directory account principals.
        /// </summary>
        private static readonly Factories.DirectoryAccountPrincipalFactory PrincipalFactory =
            new Factories.DirectoryAccountPrincipalFactory();

        /// <summary>
        /// Creates a primary user account in Active Directory.
        /// </summary>
        /// <param name="container">The distinguished name of the new user's container.</param>
        /// <param name="userName">The account name for the user.</param>
        /// <param name="userPassword">The password for the user.</param>
        /// <param name="firstName">The first name of the user.</param>
        /// <param name="middleInitials">The middle initials of the user.</param>
        /// <param name="lastName">The last name of the user.</param>
        /// <param name="description">The description of the user.</param>
        /// <returns>A <see cref="DirectorySecurityPrincipal"/> instance representing the newly created user.</returns>
        /// <exception cref="InvalidOperationException">
        /// <para>The container is not valid or does not exist.</para>
        /// <para>-or-</para>
        /// <para>A <see cref="UserPrincipal"/> cannot be inserted in the store.</para>
        /// </exception>
        /// <exception cref="PrincipalExistsException">
        /// The principal already occurs in the store. 
        /// </exception>
        /// <exception cref="PasswordException">
        /// The password does not meet complexity requirements.
        /// </exception>
        [DirectoryServicesPermission(SecurityAction.LinkDemand)]
        public static DirectoryAccountPrincipal CreateUser(
            string container,
            string userName,
            SecureString userPassword,
            string firstName,
            string middleInitials,
            string lastName,
            string description)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            if (userName == null)
            {
                throw new ArgumentNullException("userName");
            }

            if (userPassword == null)
            {
                throw new ArgumentNullException("userPassword");
            }

            if (firstName == null)
            {
                throw new ArgumentNullException("firstName");
            }

            if (lastName == null)
            {
                throw new ArgumentNullException("lastName");
            }

            string domain = DomainNameContext.DomainName.ToLowerInvariant();
            string name = 
                String.Format(
                    "{0} {1}{2}{3}", 
                    firstName, 
                    middleInitials, 
                    String.IsNullOrEmpty(middleInitials) ? String.Empty : ". ", 
                    lastName);

            string upn = String.Format("{0}@{1}", userName, domain);

            using (PrincipalContext context = new PrincipalContext(ContextType.Domain, domain, container))
            {
                using (UserPrincipal user = new UserPrincipal(context))
                {
                    user.Name = name;
                    user.Description = description;
                    user.SamAccountName = userName;
                    user.UserPrincipalName = upn;
                    user.DisplayName = name;
                    user.GivenName = firstName;
                    user.MiddleName = middleInitials;
                    user.Surname = lastName;
                    user.SetPassword(userPassword.ToInsecureString());
                    user.Save();

                    return PrincipalFactory.CreateFrom(user);
                }
            }

            ////try
            ////{
            ////    using (DirectoryEntry newUser = location.Children.Add("CN=" + name, ObjectClass.User))
            ////    {
            ////        newUser.Properties[Schema.SamAccountNameAttribute].Value = userName;
            ////        newUser.Properties[Schema.UserPrincipalNameAttribute].Value = upn;
            ////        newUser.Properties[Schema.DisplayNameAttribute].Value = name;
            ////        newUser.Properties[Schema.FirstNameAttribute].Value = firstName;
            ////        newUser.Properties[Schema.LastNameAttribute].Value = lastName;
            ////        newUser.Properties[Schema.DescriptionAttribute].Value = description;

            ////        Trace.TraceInformation(
            ////            "Setting up new user with: " +
            ////            "{0}={1}; {2}={3}; {4}={5}; {6}={7}; {8}={9}; {10}={11}",
            ////            Schema.SamAccountNameAttribute.ToUpper(), userName,
            ////            Schema.UserPrincipalNameAttribute.ToUpper(), upn,
            ////            Schema.DisplayNameAttribute.ToUpper(), name,
            ////            Schema.FirstNameAttribute.ToUpper(), firstName,
            ////            Schema.LastNameAttribute.ToUpper(), lastName,
            ////            Schema.DescriptionAttribute.ToUpper(), description);

            ////        newUser.CommitChanges();

            ////        newUser.Invoke("SetPassword", new object[] { Normalize.GetInsecureString(userPassword) });
            ////        newUser.Properties[Schema.PasswordLastSetAttribute].Value = 0;
            ////        newUser.Properties[Schema.UacAttribute].Value = 512;

            ////        Trace.TraceInformation("Setting random password and forcing change", userPassword);

            ////        newUser.CommitChanges();
            ////    }
            ////}
            ////catch (DirectoryServicesCOMException ex)
            ////{
            ////    Trace.TraceError("User creation process failed: {0}", ex);
            ////    throw;
            ////}
        }

        /// <summary>
        /// Sets a User Account Control (UAC) setting for the specified user.
        /// </summary>
        /// <param name="principal">The user to set the setting for.</param>
        /// <param name="setting">The setting to set.</param>
        /// <param name="enable">A value indicating whether to enable the setting (true) or disable it (false).</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "The base type will not have a User Account Control property."), 
        DirectoryServicesPermission(SecurityAction.LinkDemand)]
        public static void SetUserAccountControl(AuthenticablePrincipal principal, int setting, bool enable)
        {
            if (principal == null)
            {
                throw new ArgumentNullException("principal");
            }

            var userEntry = (DirectoryEntry)principal.GetUnderlyingObject();

            int val = (int)userEntry.Properties[ObjectProperty.AccountControl].Value;

            if (enable)
            {
                userEntry.Properties[ObjectProperty.AccountControl].Value = val | setting;
            }
            else
            {
                userEntry.Properties[ObjectProperty.AccountControl].Value = val & ~setting;
            }

            userEntry.CommitChanges();
        }

        /// <summary>
        /// Determines whether a User Account Control (UAC) setting is set.
        /// </summary>
        /// <param name="principal">The user to determine the setting for.</param>
        /// <param name="setting">The setting to get the status of.</param>
        /// <returns><c>true</c> if the setting is set; otherwise, <c>false</c>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design", 
            "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "The base type will not have a User Account Control property."), 
        DirectoryServicesPermission(SecurityAction.LinkDemand)]
        public static bool IsUserAccountControlSet(AuthenticablePrincipal principal, int setting)
        {
            if (principal == null)
            {
                throw new ArgumentNullException("principal");
            }

            int val = 
                (int)((DirectoryEntry)principal.GetUnderlyingObject())
                    .Properties[ObjectProperty.AccountControl].Value;

            return (val & setting) == setting;
        }

        /// <summary>
        /// Retrieves the account status of <see cref="AuthenticablePrincipal"/>.
        /// </summary>
        /// <param name="principal">The <see cref="AuthenticablePrincipal"/> for which to retrieve the status.</param>
        /// <returns>The account status of the <see cref="AuthenticablePrincipal"/>.</returns>
        [DirectoryServicesPermission(SecurityAction.LinkDemand)]
        public static AccountStatus GetAccountStatus(AuthenticablePrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException("principal");
            }

            AccountStatus status;

            if (principal.Enabled.HasValue)
            {
                if (principal.Enabled.Value)
                {
                    status = AccountStatus.Active;

                    if (principal.AccountExpirationDate.GetValueOrDefault() < DateTime.Now)
                    {
                        status = AccountStatus.Expired;
                    }
                }
                else
                {
                    status = AccountStatus.Disabled;
                }
            }
            else
            {
                throw new InvalidOperationException("Cannot evaluate a principal that has not been persisted.");
            }

            return status;
        }

        /// <summary>
        /// Determines whether the entry matches the specified SAM account type constant.
        /// </summary>
        /// <param name="entry">The entry to check.</param>
        /// <param name="type">The constant to compare against.</param>
        /// <returns>True if the SAM account type of the entry matches the specified constant, otherwise false.</returns>
        [SecurityPermission(SecurityAction.LinkDemand)]
        public static bool IsSamAccountTypeMatch(DirectoryEntry entry, int type)
        {
            if (entry == null)
            {
                throw new ArgumentNullException("entry");
            }

            return
                Int32.Parse(
                    entry.Properties[ObjectProperty.SamAccountType].Value.ToString()) == type;
        }

        /////// <summary>
        /////// Sets an expiration date for a user account.
        /////// </summary>
        /////// <param name="user">The user to set an expiration date for</param>
        /////// <param name="expirationDate">The expiration date</param>
        ////public static void SetUserExpirationDate(DirectoryEntry user, DateTime expirationDate)
        ////{
        ////    Type type = user.NativeObject.GetType();
        ////    object adsNative = user.NativeObject;
        ////    type.InvokeMember(
        ////        "AccountExpirationDate", BindingFlags.SetProperty, null,
        ////        adsNative, new object[] { expirationDate.ToShortDateString() });

        ////    user.CommitChanges();
        ////}

        /////// <summary>
        /////// Enables a user account.
        /////// </summary>
        /////// <param name="user">The user to enable</param>
        ////public static void Enable(DirectoryEntry user)
        ////{
        ////    try
        ////    {
        ////        int val = (int)user.Properties[Schema.UacAttribute].Value;
        ////        user.Properties[Schema.UacAttribute].Value = val & ~UacAccountDisabled;
        ////        user.CommitChanges();
        ////    }
        ////    catch (System.DirectoryServices.DirectoryServicesCOMException)
        ////    {
        ////        throw;
        ////    }
        ////}
    }
}
