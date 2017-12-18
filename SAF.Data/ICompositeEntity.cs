// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICompositeEntity.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides an interface for entities that contain their own entity relations.
    /// </summary>
    public interface ICompositeEntity
    {
        /// <summary>
        /// Gets the entity relations.
        /// </summary>
        IEnumerable<IEntityRelation> EntityRelations { get; }
    }
}