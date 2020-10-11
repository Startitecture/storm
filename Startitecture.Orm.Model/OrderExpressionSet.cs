// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrderExpressionSet.cs" company="Startitecture">
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
    /// Defines a sort order for an entity of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of entity to sort.
    /// </typeparam>
    public class OrderExpressionSet<T>
    {
        /// <summary>
        /// The expressions.
        /// </summary>
        private readonly List<OrderExpression> expressions = new List<OrderExpression>();

        /// <summary>
        /// The expressions.
        /// </summary>
        public IEnumerable<OrderExpression> Expressions => this.expressions;

        /// <summary>
        /// Sorts the result set by the specified <paramref name="attribute"/> in ascending order.
        /// </summary>
        /// <param name="attribute">
        /// The attribute to sort by.
        /// </param>
        /// <returns>
        /// The current <see cref="OrderExpressionSet{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="attribute"/> is null.
        /// </exception>
        public OrderExpressionSet<T> OrderBy([NotNull] Expression<Func<T, object>> attribute)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException(nameof(attribute));
            }

            this.expressions.Add(new OrderExpression(attribute));
            return this;
        }

        /// <summary>
        /// Sorts the result set by the specified <paramref name="attribute"/> in descending order.
        /// </summary>
        /// <param name="attribute">
        /// The attribute to sort by.
        /// </param>
        /// <returns>
        /// The current <see cref="OrderExpressionSet{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="attribute"/> is null.
        /// </exception>
        public OrderExpressionSet<T> OrderByDescending([NotNull] Expression<Func<T, object>> attribute)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException(nameof(attribute));
            }

            this.expressions.Add(new OrderExpression(attribute, true));
            return this;
        }
    }
}