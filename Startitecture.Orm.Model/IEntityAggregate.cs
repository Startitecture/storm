// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityAggregate.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides an interface to entity aggregates that are composed of relations to other entities.
    /// </summary>
    public interface IEntityAggregate
    {
        /// <summary>
        /// Gets the entity relations.
        /// </summary>
        IEnumerable<IEntityRelation> EntityRelations { get; }
    }
}