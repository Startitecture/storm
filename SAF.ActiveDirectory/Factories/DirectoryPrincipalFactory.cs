// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectoryPrincipalFactory.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides methods to create and populate <see cref="DirectoryPrincipal" />s.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActiveDirectory.Factories
{
    using System;
    using System.DirectoryServices;
    using System.DirectoryServices.AccountManagement;
    using System.Security.Permissions;

    using SAF.Core;

    /// <summary>
    /// Provides methods to create and populate <see cref="DirectoryPrincipal"/>s.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity to create.</typeparam>
    /// <typeparam name="TPrincipal">The type of principal used to generate entities.</typeparam>
    public abstract class DirectoryPrincipalFactory<TEntity, TPrincipal>
        where TEntity : DirectoryPrincipal
        where TPrincipal : Principal
    {
        /// <summary>
        /// Returns a new <see cref="DirectoryPrincipal"/> using the specified <see cref="AccountIdentifier"/>.
        /// </summary>
        /// <param name="identifier">The <see cref="AccountIdentifier"/> to use for creating the new <see cref="DirectoryPrincipal"/>.</param>
        /// <param name="context">The <see cref="PrincipalContext"/> of the <see cref="AccountIdentifier"/>.</param>
        /// <returns>A new <see cref="DirectoryPrincipal"/> using the specified <see cref="AccountIdentifier"/>.</returns>
        [DirectoryServicesPermission(SecurityAction.LinkDemand)]
        public TEntity CreateFrom(AccountIdentifier identifier, PrincipalContext context)
        {
            if (identifier == null)
            {
                throw new ArgumentNullException("identifier");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            identifier.ThrowOnValidationFailure();

            using (TPrincipal principal = this.FindByIdentity(context, IdentityType.SamAccountName, identifier.AccountName))
            {
                return this.CreateFrom(principal);
            }
        }

        /// <summary>
        /// Returns a new <see cref="DirectoryPrincipal"/> using the specified <see cref="DirectoryEntry"/>.
        /// </summary>
        /// <param name="entry">The <see cref="DirectoryEntry"/> to use for creating the new <see cref="DirectoryPrincipal"/>.</param>
        /// <param name="context">The <see cref="PrincipalContext"/> of the <see cref="DirectoryEntry"/>.</param>
        /// <returns>A new <see cref="DirectoryPrincipal"/> using the specified <see cref="DirectoryEntry"/>.</returns>
        [DirectoryServicesPermission(SecurityAction.LinkDemand)]
        public TEntity CreateFrom(DirectoryEntry entry, PrincipalContext context)
        {
            if (entry == null)
            {
                throw new ArgumentNullException("entry");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            using (TPrincipal principal = this.FindByIdentity(context, IdentityType.Guid, entry.Guid.ToString()))
            {
                return this.CreateFrom(principal);
            }
        }

        /// <summary>
        /// Returns a new <see cref="DirectoryPrincipal"/> using the specified <see cref="Principal"/>.
        /// </summary>
        /// <param name="principal">The <see cref="Principal"/> to use for creating the new <see cref="Principal"/>.</param>
        /// <returns>A new <see cref="DirectoryPrincipal"/> using the specified <see cref="Principal"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="principal"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="principal"/> has not been persisted to the directory store.</exception>
        [DirectoryServicesPermission(SecurityAction.LinkDemand)]
        public TEntity CreateFrom(TPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException("principal");
            }

            if (!principal.Guid.HasValue)
            {
                throw new ArgumentException(ErrorMessages.PersistedPrincipalRequired, "principal");
            }

            TEntity entity = this.GenerateEntity();
            this.PopulateWith(entity, principal);
            return entity;
        }

        /// <summary>
        /// Populates a <see cref="DirectoryPrincipal"/> using the specified <see cref="DirectoryEntry"/>.
        /// </summary>
        /// <param name="entity">The <see cref="DirectoryPrincipal"/> to populate.</param>
        /// <param name="entry">The <see cref="DirectoryEntry"/> to use for populating the <see cref="DirectoryPrincipal"/>.</param>
        /// <param name="context">The <see cref="PrincipalContext"/> of the <see cref="DirectoryEntry"/>.</param>
        [DirectoryServicesPermission(SecurityAction.LinkDemand)]
        public void PopulateWith(TEntity entity, DirectoryEntry entry, PrincipalContext context)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            if (entry == null)
            {
                throw new ArgumentNullException("entry");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            using (TPrincipal principal = this.FindByIdentity(context, IdentityType.Guid, entry.Guid.ToString()))
            {
                this.PopulateWith(entity, principal);
            }
        }

        /// <summary>
        /// Populates a <see cref="DirectoryPrincipal"/> using the specified <see cref="AccountIdentifier"/>.
        /// </summary>
        /// <param name="entity">The <see cref="DirectoryPrincipal"/> to populate.</param>
        /// <param name="identifier">The <see cref="AccountIdentifier"/> to use for populating the <see cref="DirectoryPrincipal"/>.</param>
        /// <param name="context">The <see cref="PrincipalContext"/> of the <see cref="AccountIdentifier"/>.</param>
        public void PopulateWith(TEntity entity, AccountIdentifier identifier, PrincipalContext context)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            if (identifier == null)
            {
                throw new ArgumentNullException("identifier");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            identifier.ThrowOnValidationFailure();

            using (TPrincipal principal = this.FindByIdentity(context, IdentityType.SamAccountName, identifier.AccountName))
            {
                this.PopulateWith(entity, principal);
            }
        }

        /// <summary>
        /// Populates a <see cref="DirectoryPrincipal"/> using the specified <see cref="Principal"/>.
        /// </summary>
        /// <param name="entity">The <see cref="DirectoryPrincipal"/> to populate.</param>
        /// <param name="principal">The <see cref="Principal"/> to use for populating the <see cref="DirectoryPrincipal"/>.</param>
        public abstract void PopulateWith(TEntity entity, TPrincipal principal);

        /// <summary>
        /// Generates a new entity.
        /// </summary>
        /// <returns>A newly generated entity.</returns>
        protected abstract TEntity GenerateEntity();

        /// <summary>
        /// Finds a principal using the specified identity parameters.
        /// </summary>
        /// <param name="context">The context of the search.</param>
        /// <param name="identityType">The type of identity property to use.</param>
        /// <param name="identityValue">A <see cref="String"/> representation of the identity.</param>
        /// <returns>A principal object that matches the specified identity, or null if no principal could be found.</returns>
        protected abstract TPrincipal 
            FindByIdentity(PrincipalContext context, IdentityType identityType, string identityValue);
    }
}
