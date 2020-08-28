// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrderExpression.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Represents a single ORDER BY column clause.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using System;
    using System.Linq.Expressions;

    using JetBrains.Annotations;

    /// <summary>
    /// Represents a single ORDER BY column clause.
    /// </summary>
    public class OrderExpression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderExpression"/> class.
        /// </summary>
        /// <param name="propertyExpression">
        /// The property expression.
        /// </param>
        public OrderExpression(LambdaExpression propertyExpression)
            : this(propertyExpression, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderExpression"/> class.
        /// </summary>
        /// <param name="propertyExpression">
        /// The property expression.
        /// </param>
        /// <param name="orderDescending">
        /// The order descending.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="propertyExpression"/> is null.
        /// </exception>
        public OrderExpression([NotNull] LambdaExpression propertyExpression, bool orderDescending)
        {
            this.PropertyExpression = propertyExpression ?? throw new ArgumentNullException(nameof(propertyExpression));
            this.OrderDescending = orderDescending;
        }

        /// <summary>
        /// Gets the property expression for the order clause.
        /// </summary>
        public LambdaExpression PropertyExpression { get; }

        /// <summary>
        /// Gets a value indicating whether the order should be descending.
        /// </summary>
        public bool OrderDescending { get; }
    }
}