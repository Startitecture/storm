// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectoryEntityFactory.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides methods to create and populate <see cref="DirectoryEntity" /> instances.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActiveDirectory.Factories
{
    using System;
    using System.DirectoryServices;
    using System.Security.Permissions;

    /// <summary>
    /// Provides methods to create and populate <see cref="DirectoryEntity"/> instances.
    /// </summary>
    public class DirectoryEntityFactory : DirectoryObjectFactory<DirectoryEntity>
    {
        /// <summary>
        /// Populates a <see cref="DirectoryEntity"/> using the specified <see cref="DirectoryEntry"/>.
        /// </summary>
        /// <param name="entity">The <see cref="DirectoryEntity"/> to populate.</param>
        /// <param name="entry">The <see cref="DirectoryEntry"/> to use for populating the <see cref="DirectoryEntity"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/> or <paramref name="entry"/> are null.</exception>
        [PermissionSet(SecurityAction.LinkDemand)]
        public override void PopulateWith(DirectoryEntity entity, DirectoryEntry entry)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            if (entry == null)
            {
                throw new ArgumentNullException("entry");
            }

            entity.DirectoryGuid = entry.Guid;
            entity.DistinguishedName = ObjectProperty.GetValueString(entry, ObjectProperty.DistinguishedName, null);
            entity.Name = ObjectProperty.GetValueString(entry, ObjectProperty.Name, null);
            entity.Created = DateTimeOffset.Parse(ObjectProperty.GetValueString(entry, ObjectProperty.WhenCreated));
            entity.LastModified = DateTimeOffset.Parse(ObjectProperty.GetValueString(entry, ObjectProperty.WhenChanged));
        }

        /// <summary>
        /// Generates a new entity.
        /// </summary>
        /// <returns>A newly generated entity.</returns>
        protected override DirectoryEntity GenerateEntity()
        {
            return new DirectoryEntity();
        }
    }
}
