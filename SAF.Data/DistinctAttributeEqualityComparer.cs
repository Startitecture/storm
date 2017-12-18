// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DistinctAttributeEqualityComparer.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The distinct attribute equality comparer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data
{
    using System.Collections.Generic;

    using SAF.Core;

    /// <summary>
    /// The distinct attribute equality comparer.
    /// </summary>
    public class DistinctAttributeEqualityComparer : EqualityComparer<EntityAttributeDefinition>
    {
        /// <summary>
        /// Determines whether two <see cref="EntityAttributeDefinition"/> objects are equal.
        /// </summary>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        /// <param name="x">
        /// The first object to compare.
        /// </param>
        /// <param name="y">
        /// The second object to compare.
        /// </param>
        public override bool Equals(EntityAttributeDefinition x, EntityAttributeDefinition y)
        {
            return x.ReferenceName == y.ReferenceName && x.Entity == y.Entity;
        }

        /// <summary>
        /// When overridden in a derived class, serves as a hash function for the specified object for hashing algorithms
        /// and data structures, such as a hash table.
        /// </summary>
        /// <returns>
        /// A hash code for the specified object.
        /// </returns>
        /// <param name="obj">
        /// The object for which to get a hash code.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The type of <paramref name="obj" /> is a reference type and
        /// <paramref name="obj" /> is null.
        /// </exception>
        public override int GetHashCode(EntityAttributeDefinition obj)
        {
            return Evaluate.GenerateHashCode(obj.ReferenceName, obj.Entity);
        }
    }
}