// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectorySecurityPrincipalFactory.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides methods to create and populate <see cref="DirectorySecurityPrincipal" />s.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActiveDirectory.Factories
{
    using System;
    using System.DirectoryServices;
    using System.DirectoryServices.AccountManagement;
    using System.Security.Permissions;

    /// <summary>
    /// Provides methods to create and populate <see cref="DirectorySecurityPrincipal"/>s.
    /// </summary>
    public class DirectorySecurityPrincipalFactory : DirectoryPrincipalFactory<DirectorySecurityPrincipal, Principal>
    {
        /// <summary>
        /// Populates a <see cref="DirectorySecurityPrincipal"/> with the specified <see cref="Principal"/>.
        /// </summary>
        /// <param name="entity">The <see cref="DirectorySecurityPrincipal"/> to populate.</param>
        /// <param name="principal">The <see cref="Principal"/> to populate the <see cref="DirectorySecurityPrincipal"/> with.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="entity"/> or <paramref name="principal"/> are null.
        /// </exception>
        /// <exception cref="ArgumentException"><paramref name="principal"/> has not been persisted to the directory store.</exception>
        [DirectoryServicesPermission(SecurityAction.Demand)]
        public override void PopulateWith(DirectorySecurityPrincipal entity, Principal principal)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            if (principal == null)
            {
                throw new ArgumentNullException("principal");
            }

            if (!principal.Guid.HasValue)
            {
                throw new ArgumentException(ErrorMessages.PersistedPrincipalRequired, "principal");
            }

            entity.DirectoryGuid = principal.Guid.Value;
            entity.DistinguishedName = principal.DistinguishedName;
            entity.Name = principal.Name;
            entity.AccountName = principal.SamAccountName;
            entity.DisplayName = principal.DisplayName;
            entity.Description = principal.Description;

            // Don't dispose of the underlying entry or subsequent calls to the principal may throw ObjectDisposedException.
            DirectoryEntry entry = principal.GetUnderlyingObject() as DirectoryEntry;

            entity.Created = DateTimeOffset.Parse(ObjectProperty.GetValueString(entry, ObjectProperty.WhenCreated));
            entity.LastModified = DateTimeOffset.Parse(ObjectProperty.GetValueString(entry, ObjectProperty.WhenChanged));
        }

        /// <summary>
        /// Generates a new entity.
        /// </summary>
        /// <returns>A newly generated entity.</returns>
        protected override DirectorySecurityPrincipal GenerateEntity()
        {
            return new DirectorySecurityPrincipal();
        }

        /// <summary>
        /// Finds a principal using the specified identity parameters.
        /// </summary>
        /// <param name="context">The context of the search.</param>
        /// <param name="identityType">The type of identity property to use.</param>
        /// <param name="identityValue">A <see cref="String"/> representation of the identity.</param>
        /// <returns>A principal object that matches the specified identity, or null if no principal could be found.</returns>
        [DirectoryServicesPermission(SecurityAction.Demand)]
        protected override Principal FindByIdentity(PrincipalContext context, IdentityType identityType, string identityValue)
        {
            return Principal.FindByIdentity(context, identityType, identityValue);
        }
    }
}
