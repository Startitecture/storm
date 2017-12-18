// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectCategory.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Contains common object category names used in Active Directory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActiveDirectory
{
    /// <summary>
    /// Contains common object category names used in Active Directory.
    /// </summary>
    public static class ObjectCategory
    {
        /// <summary>
        /// Domain category name.
        /// </summary>
        public const string Domain = "domain";

        /// <summary>
        /// Person category name.
        /// </summary>
        public const string Person = "person";

        /// <summary>
        /// Computer category name.
        /// </summary>
        public const string Computer = "computer";

        /// <summary>
        /// User category name.
        /// </summary>
        public const string User = "user";

        /// <summary>
        /// Domain DNS category name.
        /// </summary>
        public const string BaseDomain = "domain-DNS";

        /// <summary>
        /// Organizational Unit category name.
        /// </summary>
        public const string OrganizationalUnit = "organizationalUnit";

        /// <summary>
        /// Group category name.
        /// </summary>
        public const string Group = "group";

        /// <summary>
        /// Container category name.
        /// </summary>
        public const string Container = "container";

        /// <summary>
        /// Built-in Domain category name.
        /// </summary>
        public const string BuiltInDomain = "builtinDomain";

        /// <summary>
        /// Subnet category name.
        /// </summary>
        public const string Subnet = "subnet";

        /// <summary>
        /// Subnet container category name.
        /// </summary>
        public const string SubnetContainer = "subnetContainer";

        /// <summary>
        /// Site category name.
        /// </summary>
        public const string Site = "site";

        /// <summary>
        /// Site container category name.
        /// </summary>
        public const string SitesContainer = "sitesContainer";

        /// <summary>
        /// Server category name.
        /// </summary>
        public const string Server = "server";

        /// <summary>
        /// Server container category name.
        /// </summary>
        public const string ServersContainer = "serversContainer";

        /// <summary>
        /// Replication partner category name.
        /// </summary>
        public const string ReplicationPartner = "nTDSConnection";

        /// <summary>
        /// Replication partner container category name.
        /// </summary>
        public const string ReplicationPartnerContainer = "nTDSDSA";
    }
}
