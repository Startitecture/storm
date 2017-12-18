// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectoryAccountPrincipalFactory.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides methods to create and populate <see cref="DirectoryAccountPrincipal" />s.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActiveDirectory.Factories
{
    using System;
    using System.DirectoryServices;
    using System.DirectoryServices.AccountManagement;
    using System.Security.Permissions;

    /// <summary>
    /// Provides methods to create and populate <see cref="DirectoryAccountPrincipal"/>s.
    /// </summary>
    public class DirectoryAccountPrincipalFactory : 
        DirectoryPrincipalFactory<DirectoryAccountPrincipal, AuthenticablePrincipal>
    {
        /// <summary>
        /// Builds the base portion of the directory account principal.
        /// </summary>
        private DirectorySecurityPrincipalFactory principalFactory = new DirectorySecurityPrincipalFactory();

        /// <summary>
        /// Populates a <see cref="DirectoryAccountPrincipal"/> with the specified <see cref="AuthenticablePrincipal"/>.
        /// </summary>
        /// <param name="entity">The <see cref="DirectoryAccountPrincipal"/> to populate.</param>
        /// <param name="principal">The <see cref="AuthenticablePrincipal"/> to populate the <see cref="DirectoryAccountPrincipal"/> with.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="entity"/> or <paramref name="principal"/> are null.
        /// </exception>
        /// <exception cref="ArgumentException"><paramref name="principal"/> has not been persisted to the directory store.</exception>
        [DirectoryServicesPermission(SecurityAction.Demand)]
        public override void PopulateWith(DirectoryAccountPrincipal entity, AuthenticablePrincipal principal)
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

            this.principalFactory.PopulateWith(entity, principal);
            entity.AccountStatus = UserManagement.GetAccountStatus(principal);
            entity.UserPrincipalName = principal.UserPrincipalName;
        }

        /// <summary>
        /// Generates a new entity.
        /// </summary>
        /// <returns>A newly generated entity.</returns>
        protected override DirectoryAccountPrincipal GenerateEntity()
        {
            return new DirectoryAccountPrincipal();
        }

        /// <summary>
        /// Finds a principal using the specified identity parameters.
        /// </summary>
        /// <param name="context">The context of the search.</param>
        /// <param name="identityType">The type of identity property to use.</param>
        /// <param name="identityValue">A <see cref="String"/> representation of the identity.</param>
        /// <returns>A principal object that matches the specified identity, or null if no principal could be found.</returns>
        [DirectoryServicesPermission(SecurityAction.Demand)]
        protected override AuthenticablePrincipal FindByIdentity(
            PrincipalContext context, IdentityType identityType, string identityValue)
        {
            return (AuthenticablePrincipal)AuthenticablePrincipal.FindByIdentity(context, identityType, identityValue);
        }
    }
}
