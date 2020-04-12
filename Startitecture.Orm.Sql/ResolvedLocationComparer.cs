// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolvedLocationComparer.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Sql
{
    using System;
    using System.Collections.Generic;

    using Startitecture.Core;
    using Startitecture.Orm.Model;

    /// <summary>
    /// The resolved location comparer.
    /// </summary>
    public class ResolvedLocationComparer : EqualityComparer<EntityAttributeDefinition>
    {
        /// <summary>
        /// The compare selectors.
        /// </summary>
        private static readonly List<Func<EntityAttributeDefinition, object>> CompareSelectors = new List<Func<EntityAttributeDefinition, object>>
                                                                                                     {
                                                                                                         item => item.ResolvedLocation
                                                                                                     };

        /// <inheritdoc />
        public override bool Equals(EntityAttributeDefinition x, EntityAttributeDefinition y)
        {
            return Evaluate.Equals(x, y, CompareSelectors.ToArray());
        }

        /// <inheritdoc />
        public override int GetHashCode(EntityAttributeDefinition obj)
        {
            return Evaluate.GenerateHashCode(obj, CompareSelectors);
        }
    }
}