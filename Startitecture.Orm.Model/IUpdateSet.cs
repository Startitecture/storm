// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUpdateSet.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface to expose common properties of an update set.
// </summary>
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
        IEntityExpression ParentExpression { get; }

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
        IEnumerable<IValueFilter> Filters { get; }

        /// <summary>
        /// Gets the entity attributes to set.
        /// </summary>
        IEnumerable<ValueState> AttributesToSet { get; }

        /// <summary>
        /// Maps the current selection to the target selection type.
        /// </summary>
        /// <typeparam name="TDestEntity">
        /// The destination entity type.
        /// </typeparam>
        /// <returns>
        /// An <see cref="EntitySet{T}" /> for the destination type.
        /// </returns>
        UpdateSet<TDestEntity> MapSet<TDestEntity>()
            where TDestEntity : class, new();
    }
}