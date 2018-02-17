// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICompositeEntity.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Query
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