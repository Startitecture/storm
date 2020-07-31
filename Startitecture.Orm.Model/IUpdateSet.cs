// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUpdateSet.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides an interface to expose common properties of an update set.
    /// </summary>
    public interface IUpdateSet
    {
        /// <summary>
        /// Gets the entity type.
        /// </summary>
        Type EntityType { get; }

        /// <summary>
        /// Gets the parent expression for the set operation.
        /// </summary>
        EntityExpression ParentExpression { get; }

        /// <summary>
        /// Gets the relations that are part of the set.
        /// </summary>
        IEnumerable<IEntityRelation> Relations { get; }

        /// <summary>
        /// Gets the property values of the set's filter attributes.
        /// </summary>
        IEnumerable<object> PropertyValues { get; }

        /// <summary>
        /// Gets the filters for the entity set.
        /// </summary>
        IEnumerable<ValueFilter> Filters { get; }

        /// <summary>
        /// Gets the entity attributes to set.
        /// </summary>
        IEnumerable<ValueState> AttributesToSet { get; }
    }
}