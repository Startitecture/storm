// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainNameContext.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Contains static methods and properties related to domain names.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActiveDirectory
{
    using System;
    using System.Collections.Generic;
    using System.DirectoryServices;
    using System.Security.Permissions;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Contains static methods and properties related to domain names.
    /// </summary>
    public static class DomainNameContext
    {
        /// <summary>
        /// A regular expression that matches all invalid domain characters.
        /// </summary>
        public static readonly Regex MatchInvalidDomainCharacters = new Regex(@"^[a-zA-Z\d\.\-]");

        /// <summary>
        /// Regular expression that matches invalid characters in an AD SAM account name.
        /// </summary>
        public static readonly Regex MatchInvalidSamAccountNameCharacters = 
            new Regex(@"[^a-zA-Z\d\s\,\.\`\~\!\#\$\%\^\&\(\)\{\}\'\-_]");

        /// <summary>
        /// Regular expression that matches invalid characters in an Active Directory 
        /// computer name.
        /// </summary>
        public static readonly Regex MatchInvalidComputerNameCharacters = 
            new Regex(@"[^a-zA-Z\d\s\,\`\~\!\@\#\$\%\^\&\(\)\+\=\{\}\[\]\;\'\-_]");

        /// <summary>
        /// Regular expression that matches invalid characters in a NetBIOS domain name.
        /// </summary>
        public static readonly Regex MatchInvalidNetBiosNameCharacters = MatchInvalidComputerNameCharacters;

        /// <summary>
        /// Regular expression that matches invalid characters in a DNS name.
        /// </summary>
        public static readonly Regex MatchInvalidDomainNameCharacters = new Regex(@"[^a-zA-Z\d\-]");

        /// <summary>
        /// Regular expression that matches invalid characters in a DNS FQDN.
        /// </summary>
        public static readonly Regex MatchInvalidFullyQualifiedDomainNameCharacters = new Regex(@"[^a-zA-Z\d\-\.]");

        /// <summary>
        /// A dictionary for caching the results of domain name queries.
        /// </summary>
        private static readonly Dictionary<string, string> CacheManager = new Dictionary<string, string>();

        /// <summary>
        /// Gets the domain name of the current domain context.
        /// </summary>
        public static string DomainName
        {
            [DirectoryServicesPermission(SecurityAction.LinkDemand)]
            get { return ConvertFrom(NamingContext.DefaultNamingContext); }
        }

        /// <summary>
        /// Gets the NetBIOS name of the current domain context.
        /// </summary>
        public static string NetBiosDomainName
        {
            [DirectoryServicesPermission(SecurityAction.LinkDemand)]
            get { return FindNetBiosDomainName(DomainName); }
        }

        /// <summary>
        /// Retrieves the DNS domain name for the domain.
        /// </summary>
        /// <param name="namingContext">The naming context to retrieve the DNS name from.</param>
        /// <returns>The DNS domain name for the domain.</returns>
        public static string ConvertFrom(string namingContext)
        {
            if (String.IsNullOrEmpty(namingContext))
            {
                throw new ArgumentNullException("namingContext");
            }

            // This little bit of code takes the default naming context (i.e., "DC=domain,DC=com") and turns it into 
            // a DNS name.
            return String.Join(".", namingContext.Split(new string[] { "DC=", "," }, StringSplitOptions.RemoveEmptyEntries));
        }

        /// <summary>
        /// Gets the NetBIOS name of the specified domain.
        /// </summary>
        /// <param name="domain">The domain from which to retrieve the NetBIOS name.</param>
        /// <returns>The NetBIOS name of the current domain.</returns>
        [DirectoryServicesPermission(SecurityAction.LinkDemand)]
        public static string FindNetBiosDomainName(string domain)
        {
            if (MatchInvalidFullyQualifiedDomainNameCharacters.IsMatch(domain))
            {
                throw new ArgumentException(String.Format("'{0}' is not a valid DNS domain name", domain), "domain");
            }

            lock (CacheManager)
            {
                if (!CacheManager.ContainsKey(domain))
                {
                    string netBiosName = null;
                    string partitionContainer =
                        String.Format(
                            "{0},{1}",
                            NamingContext.PartitionsNamingContext,
                            NamingContext.GetDefaultNamingContextFromDomain(domain));

                    using (DirectoryEntry partitions = new DirectoryEntry(partitionContainer))
                    {
                        foreach (DirectoryEntry partition in partitions.Children)
                        {
                            using (partition)
                            {
                                if
                                    (ObjectProperty.GetValueString(partition, ObjectProperty.NamingContextName) ==
                                    NamingContext.GetDefaultNamingContextFromDomain(domain))
                                {
                                    netBiosName = ObjectProperty.GetValueString(partition, ObjectProperty.NetBiosName);
                                    break;
                                }
                            }
                        }
                    }

                    CacheManager.Add(domain, netBiosName);
                }
            }

            return Convert.ToString(CacheManager[domain]);
        }
    }
}
