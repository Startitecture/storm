// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupMembership.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Contains information about a group membership.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Security
{
    /// <summary>
    /// Contains information about a group membership.
    /// </summary>
    public class GroupMembership
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupMembership"/> class.
        /// </summary>
        /// <param name="entityType">The entity type of the group.</param>
        /// <param name="entityId">The entity ID of the group.</param>
        /// <param name="name">The name of the group.</param>
        public GroupMembership(int entityType, int entityId, string name)
        {
            this.EntityType = entityType;
            this.EntityId = entityId;
            this.Name = name;
        }

        /// <summary>
        /// Gets the entity type of the group.
        /// </summary>
        public int EntityType { get; private set; }

        /// <summary>
        /// Gets the entity ID of the group.
        /// </summary>
        public int EntityId { get; private set; }

        /// <summary>
        /// Gets the name of the group.
        /// </summary>
        public string Name { get; private set; }
    }
}
