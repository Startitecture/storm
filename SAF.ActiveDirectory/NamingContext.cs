// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamingContext.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Contains static methods and constants related to directory naming contexts.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActiveDirectory
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.DirectoryServices;
    using System.DirectoryServices.ActiveDirectory;
    using System.Security.Permissions;

    /// <summary>
    /// Contains static methods and constants related to directory naming contexts.
    /// </summary>
    public static class NamingContext
    {
        /// <summary>
        /// The LDAP protocol string.
        /// </summary>
        public const string LdapProtocolSpecifier = "LDAP://";

        /// <summary>
        /// The Directory Services <see cref="DirectoryEntry"/> distinguished name path format. Placeholder 0 is the 
        /// server or domain, if any. Placeholder 1 is the forward slash if the server or domain is not null or empty. 
        /// Placeholder 2 is the distinguished name.
        /// </summary>
        public const string EntryPathFormat = LdapProtocolSpecifier + "{0}{1}{2}";

        /// <summary>
        /// Configuration naming context.
        /// </summary>
        public const string ConfigurationNamingContext = "CN=Configuration";

        /// <summary>
        /// Sites naming context.
        /// </summary>
        public const string SitesNamingContext = "CN=Sites," + ConfigurationNamingContext;

        /// <summary>
        /// Subnets naming context.
        /// </summary>
        public const string SubnetsNamingContext = "CN=Subnets," + SitesNamingContext;

        /// <summary>
        /// Schema naming context.
        /// </summary>
        public const string SchemaNamingContext = "CN=Schema," + ConfigurationNamingContext;

        /// <summary>
        /// The naming context for Active Directory partitions.
        /// </summary>
        public const string PartitionsNamingContext = "CN=Partitions" + ConfigurationNamingContext;

        /// <summary>
        /// Servers container for a specific site.
        /// </summary>
        public const string SitesServersContainer = "CN=Servers";

        /// <summary>
        /// NTDS connections container for a specific server.
        /// </summary>
        public const string SitesServersReplicationContainer = "CN=NTDS Settings";

        /// <summary>
        /// Active Directory DNS records under MicrosoftDNS.
        /// </summary>
        public const string ActiveDirectoryDnsNamingContext = "CN=MicrosoftDNS,CN=System";

        /// <summary>
        /// Characters that must be escaped in an LDAP distinguished name.
        /// </summary>
        public static readonly char[] DistinguishedNameEscapeChars = new char[] { ',', '\\', '#', '+', '<', '>', ';', '"' };

        /// <summary>
        /// The key for the Default Naming Context of the currently logged on user's security context.
        /// </summary>
        private const string DefaultContextMask = "({0})";

        /// <summary>
        /// A dictionary for caching the results of domain name queries.
        /// </summary>
        private static readonly Dictionary<string, string> CacheManager = new Dictionary<string, string>();

        /// <summary>
        /// Gets the default naming context of the current naming context.
        /// </summary>
        public static string DefaultNamingContext
        {
            [DirectoryServicesPermission(SecurityAction.LinkDemand)]
            get { return GetDefaultNamingContextFromDomain(null); }
        }

        /// <summary>
        /// Returns a <see cref="DirectoryEntry"/> path with the specified domain and distinguished name.
        /// </summary>
        /// <param name="domain">The domain the distinguished name is in, or null for the default domain.</param>
        /// <param name="distinguishedName">The distinguished name of the directory object.</param>
        /// <returns>A <see cref="DirectoryEntry"/> path with the specified domain and distinguished name.</returns>
        public static string GetDirectoryPath(string domain, string distinguishedName)
        {
            return String.Format(
                EntryPathFormat, domain, String.IsNullOrEmpty(domain) ? String.Empty : "/", distinguishedName);
        }

        /// <summary>
        /// Retrieves the default naming context for the provided domain.
        /// </summary>
        /// <param name="domainName">The domain to retrieve the default naming context for.</param>
        /// <returns>The default naming context for the provided domain, or null if it could not be resolved.</returns>
        [DirectoryServicesPermission(SecurityAction.LinkDemand)]
        public static string GetDefaultNamingContextFromDomain(string domainName)
        {
            string domainKey = domainName;
            string namingContext = String.Empty;
            bool useDefaultDomain = false;

            // Use the default domain if no domain name is provided.
            if (String.IsNullOrEmpty(domainName))
            {
                System.Security.Principal.WindowsIdentity identity =
                    System.Security.Principal.WindowsIdentity.GetCurrent(false);

                // Use the domain SID to keep the number of cached objects down.
                domainKey = String.Format(DefaultContextMask, identity.User.AccountDomainSid);
                useDefaultDomain = true;
            }
            else
            {
                if (DomainNameContext.MatchInvalidFullyQualifiedDomainNameCharacters.IsMatch(domainName))
                {
                    throw new ArgumentException(
                        String.Format("'{0}' is not a valid DNS domain name", domainName), "domainName");
                }
            }

            lock (CacheManager)
            {
                if (!CacheManager.ContainsKey(domainKey))
                {
                    try
                    {
                        if (useDefaultDomain)
                        {
                            using (Domain domain = Domain.GetCurrentDomain())
                            {
                                using (DirectoryEntry entry = domain.GetDirectoryEntry())
                                {
                                    namingContext = GetDefaultNamingContextFromDistinguishedName(entry.Path);
                                }
                            }
                        }
                        else
                        {
                            using (Domain domain = Domain.GetDomain(new DirectoryContext(DirectoryContextType.Domain, domainKey)))
                            {
                                using (DirectoryEntry entry = domain.GetDirectoryEntry())
                                {
                                    namingContext = GetDefaultNamingContextFromDistinguishedName(entry.Path);
                                }
                            }
                        }
                    }
                    catch (ActiveDirectoryObjectNotFoundException ex)
                    {
                        Trace.TraceError("The domain could not be discovered: {0}", ex);
                        namingContext = String.Empty;
                    }

                    CacheManager.Add(domainKey, namingContext);
                }
            }

            return Convert.ToString(CacheManager[domainKey]);
        }

        /// <summary>
        /// Retrieves the default naming context for the provided distinguished name.
        /// </summary>
        /// <param name="distinguishedName">The distinguished name to retrieve the default naming context for.</param>
        /// <returns>The default naming context for the provided distinguished name.</returns>
        public static string GetDefaultNamingContextFromDistinguishedName(string distinguishedName)
        {
            if (String.IsNullOrEmpty(distinguishedName))
            {
                throw new ArgumentNullException("distinguishedName");
            }

            if (!distinguishedName.ToLower().Contains("dc="))
            {
                throw new ArgumentException("The distinguished name is not valid.", "distinguishedName");
            }

            return distinguishedName.Substring(distinguishedName.ToLower().IndexOf("dc="));
        }

        /// <summary>
        /// Retrieves the default naming context for the current domain.
        /// </summary>
        /// <param name="netBiosDomain">The NetBIOS domain to search for.</param>
        /// <returns>The default naming context for the current domain.</returns>
        [DirectoryServicesPermission(SecurityAction.LinkDemand)]
        public static string GetDefaultNamingContextFromNetBiosDomain(string netBiosDomain)
        {
            return GetDefaultNamingContextFromNetBiosDomain(netBiosDomain, DomainNameContext.DomainName);
        }

        /// <summary>
        /// Retrieves the default naming context for the current domain.
        /// </summary>
        /// <param name="netBiosDomain">The NetBIOS domain to search for.</param>
        /// <param name="domain">The domain to perform the search in.</param>
        /// <returns>The default naming context for the current domain.</returns>
        [DirectoryServicesPermission(SecurityAction.LinkDemand)]
        public static string GetDefaultNamingContextFromNetBiosDomain(string netBiosDomain, string domain)
        {
            if (String.IsNullOrEmpty(netBiosDomain))
            {
                throw new ArgumentNullException("netBiosDomain");
            }

            if (String.IsNullOrEmpty(domain))
            {
                throw new ArgumentNullException("domain");
            }

            if (DomainNameContext.MatchInvalidNetBiosNameCharacters.IsMatch(netBiosDomain))
            {
                throw new ArgumentException(
                    String.Format("'{0}' is not a valid NetBIOS domain name", netBiosDomain), "netBiosDomain");
            }

            if (DomainNameContext.MatchInvalidFullyQualifiedDomainNameCharacters.IsMatch(domain))
            {
                throw new ArgumentException(
                    String.Format("'{0}' is not a valid DNS domain name", domain), "domain");
            }

            lock (CacheManager)
            {
                if (!CacheManager.ContainsKey(netBiosDomain))
                {
                    string namingContext = null;
                    string partitionsContainer =
                        String.Format("LDAP://{0}/{1},{2}", domain, PartitionsNamingContext, GetDefaultNamingContextFromDomain(domain));

                    using (DirectoryEntry partitions = new DirectoryEntry(partitionsContainer))
                    {
                        foreach (DirectoryEntry partition in partitions.Children)
                        {
                            using (partition)
                            {
                                string nbname = ObjectProperty.GetValueString(partition, ObjectProperty.NetBiosName);

                                if (nbname.Equals(netBiosDomain, StringComparison.OrdinalIgnoreCase))
                                {
                                    namingContext = ObjectProperty.GetValueString(partition, ObjectProperty.NamingContextName);
                                    break;
                                }
                            }
                        }
                    }

                    CacheManager.Add(netBiosDomain, namingContext);
                }
            }

            return Convert.ToString(CacheManager[netBiosDomain]);
        }
    }
}
