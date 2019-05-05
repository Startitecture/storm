// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlSelection.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Creates selections based on the Transact-SQL language.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Startitecture.Orm.Sql
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using Startitecture.Core;
    using Startitecture.Orm.Query;

    /// <summary>
    /// Creates selections based on the Transact-SQL language.
    /// </summary>
    /// <typeparam name="TItem">
    /// The type of item to generate the selections for.
    /// </typeparam>
    public class SqlSelection<TItem> : ItemSelection<TItem>
        where TItem : new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlSelection{TItem}"/> class.
        /// </summary>
        public SqlSelection()
        {
            this.SetRelations(new TItem());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlSelection{TItem}"/> class.
        /// </summary>
        /// <param name="example">
        /// The example to match.
        /// </param>
        /// <param name="selectors">
        /// The selectors of the properties to evaluate.
        /// </param>
        public SqlSelection(TItem example, params Expression<Func<TItem, object>>[] selectors)
            : this()
        {
            this.Matching(example, selectors);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlSelection{TItem}"/> class.
        /// </summary>
        /// <param name="example">
        /// The example to match.
        /// </param>
        /// <param name="propertyExpressions">
        /// The property names.
        /// </param>
        public SqlSelection(TItem example, IEnumerable<LambdaExpression> propertyExpressions)
            : this()
        {
            this.Matching(example, propertyExpressions);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlSelection{TItem}"/> class.
        /// </summary>
        /// <param name="lowerLimit">
        /// The item representing the lower limit.
        /// </param>
        /// <param name="upperLimit">
        /// The item representing the upper limit.
        /// </param>
        /// <param name="selectors">
        /// The selectors of the properties to evaluate.
        /// </param>
        internal SqlSelection(TItem lowerLimit, TItem upperLimit, params Expression<Func<TItem, object>>[] selectors)
            : this()
        {
            ////this.SetRelations(lowerLimit);
            this.Between(lowerLimit, upperLimit, selectors);
        }

        /// <summary>
        /// Sets relations for the current example item.
        /// </summary>
        /// <param name="example">
        /// The example item to set relations for.
        /// </param>
        private void SetRelations(TItem example)
        {
            if (example is ICompositeEntity compositeEntity)
            {
                // Possible when the interface is implemented but the getter is not set.
                compositeEntity.ThrowOnDependencyFailure(entity => entity.EntityRelations);

                foreach (var entityRelation in compositeEntity.EntityRelations)
                {
                    this.AddRelation(entityRelation);
                }
            }
        }
    }
}
