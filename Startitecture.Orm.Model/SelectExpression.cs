// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectExpression.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using System;
    using System.Linq.Expressions;

    using JetBrains.Annotations;

    /// <summary>
    /// Represents a selection expression for a query.
    /// </summary>
    public class SelectExpression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectExpression"/> class.
        /// </summary>
        /// <param name="attributeExpression">
        /// The attribute expression.
        /// </param>
        /// <param name="aggregateFunction">
        /// The aggregate function.
        /// </param>
        public SelectExpression([NotNull] LambdaExpression attributeExpression, AggregateFunction aggregateFunction)
        {
            this.AttributeExpression = attributeExpression ?? throw new ArgumentNullException(nameof(attributeExpression));
            this.AggregateFunction = aggregateFunction;
        }

        /// <summary>
        /// Gets the attribute expression.
        /// </summary>
        public LambdaExpression AttributeExpression { get; }

        /// <summary>
        /// Gets the aggregate function.
        /// </summary>
        public AggregateFunction AggregateFunction { get; }
    }
}