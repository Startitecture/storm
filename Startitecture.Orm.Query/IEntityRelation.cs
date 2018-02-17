// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityRelation.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface for classes that describe a relation between two entities.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Query
{
    using Startitecture.Orm.Model;

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