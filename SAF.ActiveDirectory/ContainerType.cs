// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContainerType.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Specifies the common types of containers available in Active Directory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActiveDirectory
{
    /// <summary>
    /// Specifies the common types of containers available in Active Directory.
    /// </summary>
    public enum ContainerType
    {
        /// <summary>
        /// The container type is not known.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Standard container type. Group Policy cannot be applied to standard containers.
        /// </summary>
        Container = 1,

        /// <summary>
        /// Organizational Unit type intended for subdividing directory objects into line of business entities.
        /// </summary>
        OrganizationalUnit = 2,

        /// <summary>
        /// A specialized container for domain controller replication partners.
        /// </summary>
        ReplicationPartnerContainer = 3,

        /// <summary>
        /// A specialized container for domain controllers within a specific site.
        /// </summary>
        ServersContainer = 4,

        /// <summary>
        /// A specialized container for Active Directory sites.
        /// </summary>
        SitesContainer = 5,

        /// <summary>
        /// A specialized container for all Active Directory subnets.
        /// </summary>
        SubnetContainer = 6
    }
}
