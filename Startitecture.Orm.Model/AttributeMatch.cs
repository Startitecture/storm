// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttributeMatch.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using System;
    using System.Linq.Expressions;

    using JetBrains.Annotations;

    /// <summary>
    /// Defines a match between two attributes of different entities.
    /// </summary>
    /// <typeparam name="TSource">
    /// The type of the source entity.
    /// </typeparam>
    /// <typeparam name="TRelation">
    /// The type of the related entity.
    /// </typeparam>
    public class AttributeMatch<TSource, TRelation>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeMatch{TSource, TRelation}"/> class.
        /// </summary>
        /// <param name="sourceExpression">
        /// The source attribute expression.
        /// </param>
        /// <param name="relationExpression">
        /// The related attribute expression.
        /// </param>
        public AttributeMatch(
            [NotNull] Expression<Func<TSource, object>> sourceExpression,
            [NotNull] Expression<Func<TRelation, object>> relationExpression)
        {
            this.SourceExpression = sourceExpression ?? throw new ArgumentNullException(nameof(sourceExpression));
            this.RelationExpression = relationExpression ?? throw new ArgumentNullException(nameof(relationExpression));
        }

        /// <summary>
        /// Gets the source expression for the match.
        /// </summary>
        public Expression<Func<TSource, object>> SourceExpression { get; }

        /// <summary>
        /// Gets the relation expression for the match.
        /// </summary>
        public Expression<Func<TRelation, object>> RelationExpression { get; }
    }
}