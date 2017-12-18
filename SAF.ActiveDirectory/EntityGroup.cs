// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityGroup.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Extends the <see cref="GroupPrincipal" /> class to include entity ID and entity type parameters.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActiveDirectory
{
    using System;
    using System.DirectoryServices.AccountManagement;
    using System.Security.Permissions;

    /// <summary>
    /// Extends the <see cref="GroupPrincipal"/> class to include entity ID and entity type parameters.
    /// </summary>
    [DirectoryRdnPrefix("CN")]
    [DirectoryObjectClass("group")]
    public class EntityGroup : GroupPrincipal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityGroup"/> class.
        /// </summary>
        /// <param name="context">The <see cref="PrincipalContext"/> that specifies the server or domain against which 
        /// operations are performed.</param>
        [PermissionSet(SecurityAction.LinkDemand)]
        public EntityGroup(PrincipalContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityGroup"/> class.
        /// </summary>
        /// <param name="context">The <see cref="PrincipalContext"/> that specifies the server or domain against which 
        /// operations are performed.</param>
        /// <param name="samAccountName">The SAM account name for this group.</param>
        [PermissionSet(SecurityAction.LinkDemand)]
        public EntityGroup(PrincipalContext context, string samAccountName)
            : base(context, samAccountName)
        {
        }

        /// <summary>
        /// Gets or sets the entity ID of the <see cref="EntityGroup"/>.
        /// </summary>
        [DirectoryProperty("entityId")]
        public int? EntityId
        {
            [PermissionSet(SecurityAction.LinkDemand)]
            get
            {
                if (this.ExtensionGet("entityId").Length != 1)
                {
                    return null;
                }

                return (int?)this.ExtensionGet("entityId")[0];
            }

            [PermissionSet(SecurityAction.LinkDemand)]
            set
            {
                this.ExtensionSet("entityId", value);
            }
        }

        /// <summary>
        /// Gets or sets the entity ID of the <see cref="EntityType"/>.
        /// </summary>
        [DirectoryProperty("entityType")]
        public int? EntityType
        {
            [PermissionSet(SecurityAction.LinkDemand)]
            get
            {
                if (this.ExtensionGet("entityType").Length != 1)
                {
                    return null;
                }

                return (int?)this.ExtensionGet("entityType")[0];
            }

            [PermissionSet(SecurityAction.LinkDemand)]
            set
            {
                this.ExtensionSet("entityType", value);
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
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> or <paramref name="identityValue"/> is null.
        /// </exception>
        [PermissionSet(SecurityAction.LinkDemand)]
        public static new EntityGroup FindByIdentity(PrincipalContext context, string identityValue)
        {
            return (EntityGroup)FindByIdentityWithType(context, typeof(EntityGroup), identityValue);
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
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> or <paramref name="identityValue"/> is null.
        /// </exception>
        [PermissionSet(SecurityAction.LinkDemand)]
        public static new EntityGroup FindByIdentity(
            PrincipalContext context,
            IdentityType identityType,
            string identityValue)
        {
            return (EntityGroup)FindByIdentityWithType(context, typeof(EntityGroup), identityType, identityValue);
        }
    }
}
