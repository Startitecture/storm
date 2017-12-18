// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectoryContainerFactory.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides methods to create and populate <see cref="DirectoryContainer" />s.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActiveDirectory.Factories
{
    using System;
    using System.DirectoryServices;
    using System.Security.Permissions;

    /// <summary>
    /// Provides methods to create and populate <see cref="DirectoryContainer"/>s.
    /// </summary>
    public class DirectoryContainerFactory : DirectoryObjectFactory<DirectoryContainer>
    {
        /// <summary>
        /// Populates the base entity.
        /// </summary>
        private DirectoryEntityFactory entityFactory = new DirectoryEntityFactory();

        /// <summary>
        /// Populates a <see cref="DirectoryContainer"/> using the specified <see cref="DirectoryEntry"/>.
        /// </summary>
        /// <param name="entity">The <see cref="DirectoryContainer"/> to populate.</param>
        /// <param name="entry">The <see cref="DirectoryEntry"/> to use for populating the <see cref="DirectoryContainer"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/> or <paramref name="entry"/> are null.</exception>
        [DirectoryServicesPermission(SecurityAction.Demand)]
        public override void PopulateWith(DirectoryContainer entity, DirectoryEntry entry)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            if (entry == null)
            {
                throw new ArgumentNullException("entry");
            }

            this.entityFactory.PopulateWith(entity, entry);

            switch (entry.SchemaClassName)
            {
                case ObjectCategory.Container:
                    entity.ContainerType = ActiveDirectory.ContainerType.Container;
                    break;

                case ObjectCategory.OrganizationalUnit:
                    entity.ContainerType = ActiveDirectory.ContainerType.OrganizationalUnit;
                    break;

                case ObjectCategory.ReplicationPartnerContainer:
                    entity.ContainerType = ActiveDirectory.ContainerType.ReplicationPartnerContainer;
                    break;

                case ObjectCategory.ServersContainer:
                    entity.ContainerType = ActiveDirectory.ContainerType.ServersContainer;
                    break;

                case ObjectCategory.SitesContainer:
                    entity.ContainerType = ActiveDirectory.ContainerType.SitesContainer;
                    break;

                case ObjectCategory.SubnetContainer:
                    entity.ContainerType = ActiveDirectory.ContainerType.SubnetContainer;
                    break;

                default:
                    throw new NotSupportedException(
                        String.Format(ValidationMessages.ContainerTypeInvalid, entry.Path, entry.SchemaClassName));
            }
        }

        /// <summary>
        /// Generates a new entity.
        /// </summary>
        /// <returns>A newly generated entity.</returns>
        protected override DirectoryContainer GenerateEntity()
        {
            return new DirectoryContainer();
        }
    }
}
