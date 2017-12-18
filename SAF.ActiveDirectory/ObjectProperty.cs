// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectProperty.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActiveDirectory
{
    using System;
    using System.Collections.Generic;
    using System.DirectoryServices;
    using System.DirectoryServices.Protocols;
    using System.Security.Permissions;

    /// <summary>
    /// Contains attribute names and methods for using directory object properties.
    /// </summary>
    public static class ObjectProperty
    {
        #region Constants

        /// <summary>
        /// UserAccountControl attribute
        /// </summary>
        public const string AccountControl = "userAccountControl";

        /// <summary>
        /// AccountExpires attribute
        /// </summary>
        public const string AccountExpires = "accountExpires";

        /// <summary>
        /// BadPasswordCount attribute
        /// </summary>
        public const string BadPasswordCount = "badPwdCount";

        /// <summary>
        /// BadPasswordTime attribute
        /// </summary>
        public const string BadPasswordTime = "badPasswordTime";

        /// <summary>
        /// Object Category attribute
        /// </summary>
        public const string Category = "objectCategory";

        /// <summary>
        /// Canonical Name attribute
        /// </summary>
        public const string CommonName = "cn";

        /// <summary>
        /// Company attribute
        /// </summary>
        public const string Company = "company";

        /// <summary>
        /// Department attribute
        /// </summary>
        public const string Department = "department";

        /// <summary>
        /// Description attribute
        /// </summary>
        public const string Description = "description";

        /// <summary>
        /// DisplayName attribute
        /// </summary>
        public const string DisplayName = "displayName";

        /// <summary>
        /// Distinguished Name attribute
        /// </summary>
        public const string DistinguishedName = "distinguishedName";

        /// <summary>
        /// DNSHostName attribute
        /// </summary>
        public const string DnsHostName = "dnsHostName";

        /// <summary>
        /// Mail attribute
        /// </summary>
        public const string Email = "mail";

        /// <summary>
        /// GivenName attribute
        /// </summary>
        public const string FirstName = "givenName";

        /// <summary>
        /// GroupType attribute
        /// </summary>
        public const string GroupType = "groupType";

        /// <summary>
        /// The MS Exchange Hide from Address Lists attribute
        /// </summary>
        public const string HideFromAddressList = "msExchHideFromAddressLists";

        /// <summary>
        /// LastLogon attribute
        /// </summary>
        public const string LastLogOn = "lastLogon";

        /// <summary>
        /// SN (Surname) attribute
        /// </summary>
        public const string LastName = "sn";

        /// <summary>
        /// LogonCount attribute
        /// </summary>
        public const string LogOnCount = "logonCount";

        /// <summary>
        /// ManagedBy attribute
        /// </summary>
        public const string ManagedBy = "managedBy";

        /// <summary>
        /// Member attribute
        /// </summary>
        public const string Member = "member";

        /// <summary>
        /// MemberOf attribute
        /// </summary>
        public const string MemberOf = "memberOf";

        /// <summary>
        /// Initials attribute
        /// </summary>
        public const string MiddleName = "initials";

        /// <summary>
        /// Name attribute
        /// </summary>
        public const string Name = "name";

        /// <summary>
        /// The name of the naming context name attribute.
        /// </summary>
        public const string NamingContextName = "nCName";

        /// <summary>
        /// The name of the NetBIOS name attribute.
        /// </summary>
        public const string NetBiosName = "NetBIOSName";

        /// <summary>
        /// Operating System attribute
        /// </summary>
        public const string OS = "operatingSystem";

        /// <summary>
        /// OperatingSystemServicePack attribute
        /// </summary>
        public const string OSServicePack = "operatingSystemServicePack";

        /// <summary>
        /// OperatingSystemVersion attribute
        /// </summary>
        public const string OSVersion = "operatingSystemVersion";

        /// <summary>
        /// OU attribute
        /// </summary>
        public const string OU = "ou";

        /// <summary>
        /// PhysicalDeliveryOfficeName attribute
        /// </summary>
        public const string Office = "physicalDeliveryOfficeName";

        /// <summary>
        /// Gets the password last set AD attribute.
        /// </summary>
        public const string PasswordLastSet = "pwdLastSet";

        /// <summary>
        /// SAMAccountName attribute
        /// </summary>
        public const string SamAccountName = "sAMAccountName";

        /// <summary>
        /// SAMAccountType attribute
        /// </summary>
        public const string SamAccountType = "sAMAccountType";

        /// <summary>
        /// ScriptPath attribute
        /// </summary>
        public const string ScriptPath = "scriptPath";

        /// <summary>
        /// USNChanged attribute
        /// </summary>
        public const string SequenceChanged = "uSNChanged";

        /// <summary>
        /// USNCreated attribute
        /// </summary>
        public const string SequenceCreated = "uSNCreated";

        /// <summary>
        /// The name of the Location attribute in a Site.
        /// </summary>
        public const string SiteLocation = "location";

        /// <summary>
        /// The name of the Site Object attribute in a Subnet.
        /// </summary>
        public const string SubnetSiteObject = "siteObject";

        /// <summary>
        /// Title attribute
        /// </summary>
        public const string Title = "title";

        /// <summary>
        /// WWWHomePage attribute
        /// </summary>
        public const string Url = "wWWHomePage";

        /// <summary>
        /// UserPrincipalName attribute
        /// </summary>
        public const string UserPrincipalName = "userPrincipalName";

        /// <summary>
        /// When Changed attribute
        /// </summary>
        public const string WhenChanged = "whenChanged";

        /// <summary>
        /// When Created attribute
        /// </summary>
        public const string WhenCreated = "whenCreated";

        #endregion

        #region Static Fields

        /// <summary>
        /// A set of commonly used properties.
        /// </summary>
        public static readonly string[] CommonProperties = new[]
                                                               {
                                                                   BadPasswordCount, BadPasswordTime, DnsHostName, Department, Email, 
                                                                   FirstName, MiddleName, LastLogOn, LastName, LogOnCount, ManagedBy, 
                                                                   Member, MemberOf, Name, Office, OS, OSServicePack, OSVersion, OU, 
                                                                   SamAccountName, ScriptPath, Title, UserPrincipalName, WhenChanged, 
                                                                   WhenCreated, DistinguishedName, CommonName
                                                               };

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Returns the specified AD property value.
        /// </summary>
        /// <param name="entry">
        /// The directory entry to retrieve the property from.
        /// </param>
        /// <param name="name">
        /// The name of the property.
        /// </param>
        /// <returns>
        /// The property value or null if the property name is not in the entry.
        /// </returns>
        [DirectoryServicesPermission(SecurityAction.LinkDemand)]
        public static object GetValue(DirectoryEntry entry, string name)
        {
            if (entry == null)
            {
                throw new ArgumentNullException("entry");
            }

            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            return IsPropertySet(entry, name) ? entry.Properties[name].Value : null;
        }

        /// <summary>
        /// Returns the specified AD property value or the provided default value.
        /// </summary>
        /// <typeparam name="T">
        /// The type of value to use as a default.
        /// </typeparam>
        /// <param name="entry">
        /// The directory entry to retrieve the property from.
        /// </param>
        /// <param name="name">
        /// The name of the property.
        /// </param>
        /// <param name="defaultValue">
        /// The default value if no value is returned.
        /// </param>
        /// <returns>
        /// The property value or the specified default value if the property value does not exist.
        /// </returns>
        [DirectoryServicesPermission(SecurityAction.LinkDemand)]
        public static T GetValue<T>(DirectoryEntry entry, string name, T defaultValue)
        {
            if (entry == null)
            {
                throw new ArgumentNullException("entry");
            }

            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            return IsPropertySet(entry, name) ? (T)entry.Properties[name].Value : defaultValue;
        }

        /// <summary>
        /// Returns the <see cref="string"/> representation of the specified AD property value.
        /// </summary>
        /// <param name="entry">
        /// The directory entry to retrieve the property from.
        /// </param>
        /// <param name="name">
        /// The name of the property.
        /// </param>
        /// <returns>
        /// The property value in string format or <see cref="String.Empty"/> if could not be retrieved.
        /// </returns>
        [DirectoryServicesPermission(SecurityAction.LinkDemand)]
        public static string GetValueString(DirectoryEntry entry, string name)
        {
            if (entry == null)
            {
                throw new ArgumentNullException("entry");
            }

            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            return GetValueString(entry, name, string.Empty);
        }

        /// <summary>
        /// Returns the <see cref="string"/> representation of the specified AD property value.
        /// </summary>
        /// <param name="entry">
        /// The directory entry to retrieve the property from.
        /// </param>
        /// <param name="name">
        /// The name of the property.
        /// </param>
        /// <param name="defaultValue">
        /// The string value to return if the value is null.
        /// </param>
        /// <returns>
        /// The property in string format or <paramref name="defaultValue"/> if could not be retrieved.
        /// </returns>
        [DirectoryServicesPermission(SecurityAction.LinkDemand)]
        public static string GetValueString(DirectoryEntry entry, string name, string defaultValue)
        {
            if (entry == null)
            {
                throw new ArgumentNullException("entry");
            }

            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            if (entry.Properties[name] != null)
            {
                return Convert.ToString(GetValue(entry, name));
            }

            return defaultValue;
        }

        /// <summary>
        /// Returns the specified LDAP attribute.
        /// </summary>
        /// <param name="entry">
        /// The entry containing the attribute.
        /// </param>
        /// <param name="name">
        /// The name of the attribute.
        /// </param>
        /// <returns>
        /// The attribute in String format or null if the value does not exist.
        /// </returns>
        public static string GetValueString(SearchResultEntry entry, string name)
        {
            if (entry == null)
            {
                throw new ArgumentNullException("entry");
            }

            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (entry.Attributes[name] != null && entry.Attributes[name].Count > 0)
            {
                return Convert.ToString(entry.Attributes[name][0]);
            }

            return null;
        }

        /// <summary>
        /// Returns the values of the specified <see cref="DirectoryEntry"/> property.
        /// </summary>
        /// <param name="entry">
        /// The directory entry to retrieve the property values from.
        /// </param>
        /// <param name="name">
        /// The name of the property.
        /// </param>
        /// <returns>
        /// A list of property values in string format or an empty list if none could be retrieved.
        /// </returns>
        [DirectoryServicesPermission(SecurityAction.LinkDemand)]
        public static IEnumerable<string> GetValueStrings(DirectoryEntry entry, string name)
        {
            if (entry == null)
            {
                throw new ArgumentNullException("entry");
            }

            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            if (entry.Properties[name] == null)
            {
                return null;
            }

            var values = new List<string>();

            foreach (object item in entry.Properties[name])
            {
                if (item != null)
                {
                    values.Add(Convert.ToString(item));
                }
            }

            return values;
        }

        /// <summary>
        /// Determines whether a named property is set for the specified <see cref="DirectoryEntry"/>.
        /// </summary>
        /// <param name="entry">
        /// The <see cref="DirectoryEntry"/> to check.
        /// </param>
        /// <param name="name">
        /// The name of the property.
        /// </param>
        /// <returns>
        /// <c>true</c> if the property is set; otherwise, <c>false</c>.
        /// </returns>
        [DirectoryServicesPermission(SecurityAction.LinkDemand)]
        public static bool IsPropertySet(DirectoryEntry entry, string name)
        {
            if (entry == null)
            {
                throw new ArgumentNullException("entry");
            }

            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            return entry.Properties[name] != null && entry.Properties[name].Value != null;
        }

        #endregion
    }
}