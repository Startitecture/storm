// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttributeMatchSet.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using JetBrains.Annotations;

    /// <summary>
    /// Contains a set of <see cref="AttributeMatch{TSource, TRelation}"/> items.
    /// </summary>
    /// <typeparam name="TSource">
    /// The type of the source entity.
    /// </typeparam>
    /// <typeparam name="TRelation">
    /// The type of the related entity.
    /// </typeparam>
    public class AttributeMatchSet<TSource, TRelation>
    {
        /// <summary>
        /// The list of matches.
        /// </summary>
        private readonly List<AttributeMatch<TSource, TRelation>> matches = new List<AttributeMatch<TSource, TRelation>>();

        /// <summary>
        /// Gets the matches for the current set.
        /// </summary>
        public IEnumerable<AttributeMatch<TSource, TRelation>> Matches => this.matches;

        /// <summary>
        /// Adds a pair of attributes as a <see cref="AttributeMatch{TSource, TRelation}"/>.
        /// </summary>
        /// <param name="sourceExpression">
        /// The source attribute expression.
        /// </param>
        /// <param name="relationExpression">
        /// The related attribute expression.
        /// </param>
        /// <returns>
        /// The current <see cref="AttributeMatchSet{TSource, TRelation}"/> with the new match added.
        /// </returns>
        public AttributeMatchSet<TSource, TRelation> On(
            [NotNull] Expression<Func<TSource, object>> sourceExpression,
            [NotNull] Expression<Func<TRelation, object>> relationExpression)
        {
            if (sourceExpression == null)
            {
                throw new ArgumentNullException(nameof(sourceExpression));
            }

            if (relationExpression == null)
            {
                throw new ArgumentNullException(nameof(relationExpression));
            }

            this.matches.Add(new AttributeMatch<TSource, TRelation>(sourceExpression, relationExpression));
            return this;
        }
    }
}