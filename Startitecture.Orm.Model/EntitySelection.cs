// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntitySelection.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Creates selection criteria for repository entities.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Creates selection criteria for repository entities.
    /// </summary>
    /// <typeparam name="T">
    /// The type of entity to select.
    /// </typeparam>
    public class EntitySelection<T> : EntitySet<T>, ISelection
    {
        /// <summary>
        /// The selection expressions.
        /// </summary>
        private readonly List<SelectExpression> selectExpressions = new List<SelectExpression>();

        /// <inheritdoc />
        public IEnumerable<SelectExpression> SelectExpressions => this.selectExpressions;

        /// <summary>
        /// Selects the attributes to return with the query.
        /// </summary>
        /// <param name="selectors">
        /// The attribute selectors. If empty, all attributes are returned.
        /// </param>
        /// <returns>
        /// The current <see cref="EntitySelection{T}"/>.
        /// </returns>
        public EntitySelection<T> Select(params Expression<Func<T, object>>[] selectors)
        {
            if (selectors == null)
            {
                throw new ArgumentNullException(nameof(selectors));
            }

            this.selectExpressions.AddRange(selectors.Select(expression => new SelectExpression(expression, AggregateFunction.None)));
            return this;
        }

        /// <summary>
        /// Gets the count for the specified attribute.
        /// </summary>
        /// <param name="selector">
        /// The attribute selector.
        /// </param>
        /// <returns>
        /// The current <see cref="EntitySelection{T}"/>.
        /// </returns>
        public EntitySelection<T> Count(Expression<Func<T, object>> selector)
        {
            this.selectExpressions.Add(new SelectExpression(selector, AggregateFunction.Count));
            return this;
        }

        /// <summary>
        /// Maps the current selection to the target selection type.
        /// </summary>
        /// <typeparam name="TDestEntity">
        /// The destination entity type.
        /// </typeparam>
        /// <returns>
        /// An <see cref="EntitySet{T}" /> for the destination type.
        /// </returns>
        public override EntitySet<TDestEntity> MapSet<TDestEntity>()
        {
            return this.MapSelection<TDestEntity>();
        }

        /// <inheritdoc />
        public EntitySelection<TDestEntity> MapSelection<TDestEntity>()
            where TDestEntity : class, new()
        {
            var targetSelection = new EntitySelection<TDestEntity>();
            targetSelection.selectExpressions.AddRange(this.selectExpressions);
            this.MapSet(this, targetSelection);
            return targetSelection;
        }
    }
}
