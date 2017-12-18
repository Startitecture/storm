// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityRelation.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface for classes that describe a relation between two entities.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data
{
    /// <summary>
    /// Provides an interface for classes that describe a relation between two entities.
    /// </summary>
    public interface IEntityRelation
    {
        /// <summary>
        /// Gets the type of entity relation.
        /// </summary>
        EntityRelationType RelationType { get; }

        /// <summary>
        /// Gets the source location.
        /// </summary>
        EntityLocation SourceLocation { get; }

        /// <summary>
        /// Gets the source selector.
        /// </summary>
        EntityAttributeDefinition SourceAttribute { get; }

        /// <summary>
        /// Gets the relation location.
        /// </summary>
        EntityLocation RelationLocation { get; }

        /// <summary>
        /// Gets the relation selector.
        /// </summary>
        EntityAttributeDefinition RelationAttribute { get; }
    }
}