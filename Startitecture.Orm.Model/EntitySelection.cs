// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntitySelection.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
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

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Resources;

    /// <summary>
    /// Creates selection criteria for repository entities.
    /// </summary>
    /// <typeparam name="T">
    /// The type of entity to select.
    /// </typeparam>
    public class EntitySelection<T> : ISelection
    {
        /// <summary>
        /// The value separator for the ToString() method.
        /// </summary>
        private const string ValueSeparator = "&";

        /// <summary>
        /// The relations.
        /// </summary>
        private readonly List<IEntityRelation> relations = new List<IEntityRelation>();

        /// <summary>
        /// The value filters.
        /// </summary>
        private readonly List<ValueFilter> valueFilters = new List<ValueFilter>();

        /// <summary>
        /// The selection expressions.
        /// </summary>
        private readonly List<SelectExpression> selectExpressions = new List<SelectExpression>();

        /// <summary>
        /// The order by expressions.
        /// </summary>
        private readonly List<OrderExpression> orderByExpressions = new List<OrderExpression>();

        /// <inheritdoc />
        public EntityExpression ParentExpression { get; private set; }

        /// <inheritdoc />
        public Type EntityType => typeof(T);

        /// <inheritdoc />
        public IEnumerable<IEntityRelation> Relations => this.relations;

        /// <inheritdoc />
        public LinkedSelection LinkedSelection { get; private set; }

        /// <inheritdoc />
        public IEnumerable<object> PropertyValues
        {
            get
            {
                ISelection selection = this;

                while (selection != null)
                {
                    foreach (var value in selection.Filters.SelectMany(ValueFilter.SelectNonNullValues))
                    {
                        yield return value;
                    }

                    // FETCH / OFFSET always last
                    if (selection.Page.Size + selection.Page.RowOffset > 0)
                    {
                        yield return selection.Page.RowOffset;
                        yield return selection.Page.Size;
                    }

                    selection = selection.LinkedSelection?.Selection;
                }

                // Finally, the values from our parent expression, if any.
                if (this.ParentExpression == null)
                {
                    yield break;
                }

                foreach (var value in this.ParentExpression.TableSelection.PropertyValues)
                {
                    yield return value;
                }
            }
        }

        /// <inheritdoc />
        public IEnumerable<SelectExpression> SelectExpressions => this.selectExpressions;

        /// <inheritdoc />
        public IEnumerable<OrderExpression> OrderByExpressions => this.orderByExpressions;

        /// <inheritdoc />
        public IEnumerable<ValueFilter> Filters => this.valueFilters;

        /// <inheritdoc />
        public ResultPage Page { get; } = new ResultPage();

        #region Selection

        /// <summary>
        /// Creates a table expression for this selection.
        /// </summary>
        /// <typeparam name="TExpression">
        /// The type of entity the table expression will select from.
        /// </typeparam>
        /// <param name="tableExpression">
        /// The selection for the table expression.
        /// </param>
        /// <param name="tableName">
        /// The table name.
        /// </param>
        /// <param name="tableRelationSet">
        /// The set of relations between the table expression and this selection.
        /// </param>
        /// <returns>
        /// The current <see cref="EntitySelection{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="tableExpression"/> or <paramref name="tableRelationSet"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="tableName"/> is null or whitespace.
        /// </exception>
        public EntitySelection<T> WithAs<TExpression>(
            [NotNull] EntitySelection<TExpression> tableExpression,
            [NotNull] string tableName,
            [NotNull] EntityRelationSet<TExpression> tableRelationSet)
        {
            if (tableExpression == null)
            {
                throw new ArgumentNullException(nameof(tableExpression));
            }

            if (tableRelationSet == null)
            {
                throw new ArgumentNullException(nameof(tableRelationSet));
            }

            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(tableName));
            }

            this.ParentExpression = new EntityExpression(tableExpression, tableName, new List<IEntityRelation>(tableRelationSet.Relations));
            return this;
        }

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

            this.Select(selectors as LambdaExpression[]);
            return this;
        }

        /// <summary>
        /// Gets the count for the specified attribute.
        /// </summary>
        /// <param name="selector">
        /// The attribute selector
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
        /// Skips the specified number of rows in the result set.
        /// </summary>
        /// <param name="rows">
        /// The rows to skip.
        /// </param>
        /// <returns>
        /// The current <see cref="EntitySelection{T}"/>.
        /// </returns>
        public EntitySelection<T> Skip(int rows)
        {
            this.Page.RowOffset = rows < 0 ? 0 : rows;
            return this;
        }

        /// <summary>
        /// Limits the number of rows returned to the number specified.
        /// </summary>
        /// <param name="rows">
        /// The number of rows to take.
        /// </param>
        /// <returns>
        /// The current <see cref="EntitySelection{T}"/>.
        /// </returns>
        public EntitySelection<T> Take(int rows)
        {
            this.Page.Size = rows < 0 ? 0 : rows;
            return this;
        }

        /// <summary>
        /// Order the results by the specified <paramref name="propertyExpression"/>.
        /// </summary>
        /// <param name="propertyExpression">
        /// The property expression.
        /// </param>
        /// <returns>
        /// The current <see cref="EntitySelection{T}"/>.
        /// </returns>
        public EntitySelection<T> OrderBy(Expression<Func<T, object>> propertyExpression)
        {
            this.orderByExpressions.Add(new OrderExpression(propertyExpression));
            return this;
        }

        /// <summary>
        /// Order the results by the specified <paramref name="propertyExpression"/>.
        /// </summary>
        /// <param name="propertyExpression">
        /// The property expression.
        /// </param>
        /// <returns>
        /// The current <see cref="EntitySelection{T}"/>.
        /// </returns>
        public EntitySelection<T> OrderByDescending(Expression<Func<T, object>> propertyExpression)
        {
            this.orderByExpressions.Add(new OrderExpression(propertyExpression, true));
            return this;
        }

        #endregion

        #region Predicates

        /// <summary>
        /// Adds a match filter for the specified example item.
        /// </summary>
        /// <param name="example">
        /// The example to match.
        /// </param>
        /// <param name="selectors">
        /// The selectors of the attributes to match.
        /// </param>
        /// <returns>
        /// The current <see cref="EntitySelection{T}"/>.
        /// </returns>
        public EntitySelection<T> Matching(T example, params Expression<Func<T, object>>[] selectors)
        {
            if (Evaluate.IsNull(example))
            {
                throw new ArgumentNullException(nameof(example));
            }

            if (selectors == null)
            {
                throw new ArgumentNullException(nameof(selectors));
            }

            foreach (var selector in selectors)
            {
                var value = selector.Compile().Invoke(example);
                this.valueFilters.Add(new ValueFilter(selector, FilterType.Equality, value));
            }

            return this;
        }

        /// <summary>
        /// Adds a equality filter for the specified property.
        /// </summary>
        /// <param name="valueExpression">
        /// The value expression.
        /// </param>
        /// <param name="value">
        /// The value to match.
        /// </param>
        /// <typeparam name="TValue">
        /// The type of the value.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="EntitySelection{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="valueExpression"/> is null.
        /// </exception>
        public EntitySelection<T> WhereEqual<TValue>([NotNull] Expression<Func<T, TValue>> valueExpression, TValue value)
        {
            return this.WhereEqual(valueExpression as LambdaExpression, value);
        }

        /// <summary>
        /// Adds a equality filter for the specified property.
        /// </summary>
        /// <param name="valueExpression">
        /// The value expression.
        /// </param>
        /// <param name="value">
        /// The value to match.
        /// </param>
        /// <typeparam name="TValue">
        /// The type of the value.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="EntitySelection{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="valueExpression"/> is null.
        /// </exception>
        public EntitySelection<T> WhereEqual<TValue>([NotNull] LambdaExpression valueExpression, TValue value)
        {
            if (valueExpression == null)
            {
                throw new ArgumentNullException(nameof(valueExpression));
            }

            var valueFilter = Evaluate.IsNull(value)
                                  ? new ValueFilter(valueExpression, FilterType.IsNull, value)
                                  : new ValueFilter(valueExpression, FilterType.Equality, value);

            this.valueFilters.Add(valueFilter);
            return this;
        }

        /// <summary>
        /// Adds a greater than filter for the specified property and <paramref name="value"/>.
        /// </summary>
        /// <param name="valueExpression">
        /// The value expression.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <typeparam name="TValue">
        /// The type of value to evaluate.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="EntitySelection{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="valueExpression"/> is null.
        /// </exception>
        public EntitySelection<T> GreaterThan<TValue>([NotNull] Expression<Func<T, TValue>> valueExpression, TValue value)
        {
            if (valueExpression == null)
            {
                throw new ArgumentNullException(nameof(valueExpression));
            }

            this.valueFilters.Add(new ValueFilter(valueExpression, FilterType.GreaterThan, value));
            return this;
        }

        /// <summary>
        /// Adds a greater than filter for the specified property and <paramref name="value"/>.
        /// </summary>
        /// <param name="valueExpression">
        /// The value expression.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <typeparam name="TValue">
        /// The type of value to evaluate.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="EntitySelection{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="valueExpression"/> is null.
        /// </exception>
        public EntitySelection<T> GreaterThanOrEqualTo<TValue>([NotNull] Expression<Func<T, TValue>> valueExpression, TValue value)
        {
            if (valueExpression == null)
            {
                throw new ArgumentNullException(nameof(valueExpression));
            }

            this.valueFilters.Add(new ValueFilter(valueExpression, FilterType.GreaterThanOrEqualTo, value));
            return this;
        }

        /// <summary>
        /// Adds a less than filter for the specified property and <paramref name="value"/>.
        /// </summary>
        /// <param name="valueExpression">
        /// The value expression.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <typeparam name="TValue">
        /// The type of value to evaluate.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="EntitySelection{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="valueExpression"/> is null.
        /// </exception>
        public EntitySelection<T> LessThan<TValue>([NotNull] Expression<Func<T, TValue>> valueExpression, TValue value)
        {
            if (valueExpression == null)
            {
                throw new ArgumentNullException(nameof(valueExpression));
            }

            this.valueFilters.Add(new ValueFilter(valueExpression, FilterType.LessThan, value));
            return this;
        }

        /// <summary>
        /// Adds a less than or equal to filter for the specified property and <paramref name="value"/>.
        /// </summary>
        /// <param name="valueExpression">
        /// The value expression.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <typeparam name="TValue">
        /// The type of value to evaluate.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="EntitySelection{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="valueExpression"/> is null.
        /// </exception>
        public EntitySelection<T> LessThanOrEqualTo<TValue>([NotNull] Expression<Func<T, TValue>> valueExpression, TValue value)
        {
            if (valueExpression == null)
            {
                throw new ArgumentNullException(nameof(valueExpression));
            }

            this.valueFilters.Add(new ValueFilter(valueExpression, FilterType.LessThanOrEqualTo, value));
            return this;
        }

        /// <summary>
        /// Adds an inclusion filter for the specified example item.
        /// </summary>
        /// <typeparam name="TValue">
        /// The type of value specified by the selector.
        /// </typeparam>
        /// <param name="selector">
        /// The property selector.
        /// </param>
        /// <param name="inclusionValues">
        /// The inclusion values.
        /// </param>
        /// <returns>
        /// The current <see cref="EntitySelection{T}"/>.
        /// </returns>
        public EntitySelection<T> Include<TValue>(
            Expression<Func<T, TValue>> selector,
            params TValue[] inclusionValues)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            if (inclusionValues == null)
            {
                throw new ArgumentNullException(nameof(inclusionValues));
            }

            var valueFilter = new ValueFilter(selector, FilterType.MatchesSet, inclusionValues.Cast<object>().ToArray());
            this.valueFilters.Add(valueFilter);
            return this;
        }

        /// <summary>
        /// Adds a between filter for the specified attribute
        /// </summary>
        /// <param name="selector">
        /// The attribute selector.
        /// </param>
        /// <param name="minValue">
        /// The minimum value.
        /// </param>
        /// <param name="maxValue">
        /// The maximum value.
        /// </param>
        /// <typeparam name="TValue">
        /// The type of the value to compare.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="EntitySelection{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="selector"/>, <paramref name="minValue"/> or <paramref name="maxValue"/> is null.
        /// </exception>
        public EntitySelection<T> Between<TValue>([NotNull] Expression<Func<T, TValue>> selector, [NotNull] TValue minValue, [NotNull] TValue maxValue)
            where TValue : IComparable
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            if (minValue == null)
            {
                throw new ArgumentNullException(nameof(minValue));
            }

            if (maxValue == null)
            {
                throw new ArgumentNullException(nameof(maxValue));
            }

            var min = minValue.CompareTo(maxValue) < 0 ? minValue : maxValue;
            var max = maxValue.CompareTo(minValue) > 0 ? maxValue : minValue;

            var valueFilter = new ValueFilter(selector, FilterType.Between, min, max);
            return this.AddFilter(valueFilter);
        }

        /// <summary>
        /// Adds a between filter for the specified example item.
        /// </summary>
        /// <param name="baseline">
        /// The baseline item.
        /// </param>
        /// <param name="boundary">
        /// The boundary item.
        /// </param>
        /// <param name="selectors">
        /// The selectors of the attributes to match.
        /// </param>
        /// <returns>
        /// The current <see cref="EntitySelection{T}"/>.
        /// </returns>
        public EntitySelection<T> Between(
            T baseline,
            T boundary,
            params Expression<Func<T, object>>[] selectors)
        {
            if (Evaluate.IsNull(baseline))
            {
                throw new ArgumentNullException(nameof(baseline));
            }

            if (Evaluate.IsNull(boundary))
            {
                throw new ArgumentNullException(nameof(boundary));
            }

            if (selectors == null)
            {
                throw new ArgumentNullException(nameof(selectors));
            }

            if (selectors.Any() == false)
            {
                throw new ArgumentException(ValidationMessages.SpecifyAtLeastOneParameter, nameof(selectors));
            }

            foreach (var selector in selectors)
            {
                var compiledSelector = selector.Compile();
                this.AddRangeFilter(selector, compiledSelector.Invoke(baseline), compiledSelector.Invoke(boundary));
            }

            return this;
        }

        #endregion

        #region JOINs

        /// <summary>
        /// Adds a relation to the selection.
        /// </summary>
        /// <param name="relation">
        /// The relation to add.
        /// </param>
        /// <returns>
        /// The current <see cref="EntitySelection{T}"/>.
        /// </returns>
        public EntitySelection<T> AddRelation([NotNull] IEntityRelation relation)
        {
            if (relation == null)
            {
                throw new ArgumentNullException(nameof(relation));
            }

            this.relations.Add(relation);
            return this;
        }

        /// <summary>
        /// Appends an INNER JOIN clause to the selection.
        /// </summary>
        /// <param name="leftSelector">
        /// The left selector of the JOIN clause.
        /// </param>
        /// <param name="rightSelector">
        /// The right selector of the JOIN clause.
        /// </param>
        /// <returns>
        /// The current <see cref="EntitySelection{T}"/>.
        /// </returns>
        public EntitySelection<T> InnerJoin(
            [NotNull] Expression<Func<T, object>> leftSelector,
            [NotNull] Expression<Func<T, object>> rightSelector)
        {
            if (leftSelector == null)
            {
                throw new ArgumentNullException(nameof(leftSelector));
            }

            if (rightSelector == null)
            {
                throw new ArgumentNullException(nameof(rightSelector));
            }

            var relation = new EntityRelation(EntityRelationType.InnerJoin);
            relation.Join<T>(leftSelector, rightSelector);
            return this.AddRelation(relation);
        }

        /// <summary>
        /// Appends an INNER JOIN clause to the selection.
        /// </summary>
        /// <param name="leftSelector">
        /// The left selector of the JOIN clause.
        /// </param>
        /// <param name="rightSelector">
        /// The right selector of the JOIN clause.
        /// </param>
        /// <typeparam name="TRelation">
        /// The type of item on the right side of the INNER JOIN.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="EntitySelection{T}"/>.
        /// </returns>
        public EntitySelection<T> InnerJoin<TRelation>(
            Expression<Func<T, object>> leftSelector,
            Expression<Func<TRelation, object>> rightSelector)
        {
            if (leftSelector == null)
            {
                throw new ArgumentNullException(nameof(leftSelector));
            }

            if (rightSelector == null)
            {
                throw new ArgumentNullException(nameof(rightSelector));
            }

            var entityRelation = new EntityRelation(EntityRelationType.InnerJoin);
            entityRelation.Join(leftSelector, rightSelector, null, null);
            return this.AddRelation(entityRelation);
        }

        /// <summary>
        /// Appends an INNER JOIN clause to the selection.
        /// </summary>
        /// <param name="leftSelector">
        /// The left selector of the JOIN clause.
        /// </param>
        /// <param name="rightSelector">
        /// The right selector of the JOIN clause.
        /// </param>
        /// <param name="relationAlias">
        /// The alias for the joined entity.
        /// </param>
        /// <typeparam name="TRelation">
        /// The type of item on the right side of the INNER JOIN.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="EntitySelection{T}"/>.
        /// </returns>
        public EntitySelection<T> InnerJoin<TRelation>(
            Expression<Func<T, object>> leftSelector,
            Expression<Func<TRelation, object>> rightSelector,
            string relationAlias)
        {
            if (leftSelector == null)
            {
                throw new ArgumentNullException(nameof(leftSelector));
            }

            if (rightSelector == null)
            {
                throw new ArgumentNullException(nameof(rightSelector));
            }

            var entityRelation = new EntityRelation(EntityRelationType.InnerJoin);
            entityRelation.Join(leftSelector, rightSelector, null, relationAlias);
            return this.AddRelation(entityRelation);
        }

        /// <summary>
        /// Appends an INNER JOIN clause to the selection. The table represented by <typeparamref name="TSource"/> must already be a 
        /// JOIN member.
        /// </summary>
        /// <param name="leftSelector">
        /// The left selector of the JOIN clause.
        /// </param>
        /// <param name="rightSelector">
        /// The right selector of the JOIN clause.
        /// </param>
        /// <typeparam name="TSource">
        /// The type of item on the left side of the INNER JOIN.
        /// </typeparam>
        /// <typeparam name="TRelation">
        /// The type of item on the right side of the INNER JOIN.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="EntitySelection{T}"/>.
        /// </returns>
        public EntitySelection<T> InnerJoin<TSource, TRelation>(
            Expression<Func<TSource, object>> leftSelector,
            Expression<Func<TRelation, object>> rightSelector)
        {
            var entityRelation = new EntityRelation(EntityRelationType.InnerJoin);
            entityRelation.Join(leftSelector, rightSelector, null, null);
            return this.AddRelation(entityRelation);
        }

        /// <summary>
        /// Appends an INNER JOIN clause to the selection. The table represented by <typeparamref name="TSource"/> must already be a 
        /// JOIN member.
        /// </summary>
        /// <param name="leftSelector">
        /// The left selector of the JOIN clause.
        /// </param>
        /// <param name="rightSelector">
        /// The right selector of the JOIN clause.
        /// </param>
        /// <param name="relationAlias">
        /// The alias for the joined entity.
        /// </param>
        /// <typeparam name="TSource">
        /// The type of item on the left side of the INNER JOIN.
        /// </typeparam>
        /// <typeparam name="TRelation">
        /// The type of item on the right side of the INNER JOIN.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="EntitySelection{T}"/>.
        /// </returns>
        public EntitySelection<T> InnerJoin<TSource, TRelation>(
            Expression<Func<TSource, object>> leftSelector,
            Expression<Func<TRelation, object>> rightSelector,
            string relationAlias)
        {
            return this.InnerJoin(leftSelector, null, rightSelector, relationAlias);
        }

        /// <summary>
        /// Appends an INNER JOIN clause to the selection. The table represented by <typeparamref name="TSource"/> must already be a 
        /// JOIN member.
        /// </summary>
        /// <param name="leftSelector">
        /// The left selector of the JOIN clause.
        /// </param>
        /// <param name="sourceAlias">
        /// The alias for the related entity.
        /// </param>
        /// <param name="rightSelector">
        /// The right selector of the JOIN clause.
        /// </param>
        /// <param name="relationAlias">
        /// The alias for the joined entity.
        /// </param>
        /// <typeparam name="TSource">
        /// The type of item on the left side of the INNER JOIN.
        /// </typeparam>
        /// <typeparam name="TRelation">
        /// The type of item on the right side of the INNER JOIN.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="EntitySelection{T}"/>.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Allows fluent usage of the method.")]
        public EntitySelection<T> InnerJoin<TSource, TRelation>(
            Expression<Func<TSource, object>> leftSelector,
            string sourceAlias,
            Expression<Func<TRelation, object>> rightSelector,
            string relationAlias)
        {
            if (leftSelector == null)
            {
                throw new ArgumentNullException(nameof(leftSelector));
            }

            if (rightSelector == null)
            {
                throw new ArgumentNullException(nameof(rightSelector));
            }

            var entityRelation = new EntityRelation(EntityRelationType.InnerJoin);
            entityRelation.Join(leftSelector, rightSelector, sourceAlias, relationAlias);
            return this.AddRelation(entityRelation);
        }

        /// <summary>
        /// Appends a LEFT JOIN clause to the selection.
        /// </summary>
        /// <param name="leftSelector">
        /// The left selector of the JOIN clause.
        /// </param>
        /// <param name="rightSelector">
        /// The right selector of the JOIN clause.
        /// </param>
        /// <returns>
        /// The current <see cref="EntitySelection{T}"/>.
        /// </returns>
        public EntitySelection<T> LeftJoin(
            [NotNull] Expression<Func<T, object>> leftSelector,
            [NotNull] Expression<Func<T, object>> rightSelector)
        {
            if (leftSelector == null)
            {
                throw new ArgumentNullException(nameof(leftSelector));
            }

            if (rightSelector == null)
            {
                throw new ArgumentNullException(nameof(rightSelector));
            }

            var relation = new EntityRelation(EntityRelationType.LeftJoin);
            relation.Join<T>(leftSelector, rightSelector);
            return this.AddRelation(relation);
        }

        /// <summary>
        /// Appends a LEFT JOIN clause to the selection.
        /// </summary>
        /// <param name="leftSelector">
        /// The left selector of the JOIN clause.
        /// </param>
        /// <param name="rightSelector">
        /// The right selector of the JOIN clause.
        /// </param>
        /// <typeparam name="TRelation">
        /// The type of item on the right side of the LEFT JOIN.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="EntitySelection{T}"/>.
        /// </returns>
        public EntitySelection<T> LeftJoin<TRelation>(
            Expression<Func<T, object>> leftSelector,
            Expression<Func<TRelation, object>> rightSelector)
        {
            if (leftSelector == null)
            {
                throw new ArgumentNullException(nameof(leftSelector));
            }

            if (rightSelector == null)
            {
                throw new ArgumentNullException(nameof(rightSelector));
            }

            var entityRelation = new EntityRelation(EntityRelationType.LeftJoin);
            entityRelation.Join(leftSelector, rightSelector, null, null);
            return this.AddRelation(entityRelation);
        }

        /// <summary>
        /// Appends a LEFT JOIN clause to the selection.
        /// </summary>
        /// <param name="leftSelector">
        /// The left selector of the JOIN clause.
        /// </param>
        /// <param name="rightSelector">
        /// The right selector of the JOIN clause.
        /// </param>
        /// <param name="relationAlias">
        /// The alias for the joined entity.
        /// </param>
        /// <typeparam name="TRelation">
        /// The type of item on the right side of the LEFT JOIN.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="EntitySelection{T}"/>.
        /// </returns>
        public EntitySelection<T> LeftJoin<TRelation>(
            Expression<Func<T, object>> leftSelector,
            Expression<Func<TRelation, object>> rightSelector,
            string relationAlias)
        {
            if (leftSelector == null)
            {
                throw new ArgumentNullException(nameof(leftSelector));
            }

            if (rightSelector == null)
            {
                throw new ArgumentNullException(nameof(rightSelector));
            }

            var entityRelation = new EntityRelation(EntityRelationType.LeftJoin);
            entityRelation.Join(leftSelector, rightSelector, null, relationAlias);
            return this.AddRelation(entityRelation);
        }

        /// <summary>
        /// Appends a LEFT JOIN clause to the selection. The table represented by <typeparamref name="TSource"/> must already be a 
        /// JOIN member.
        /// </summary>
        /// <param name="leftSelector">
        /// The left selector of the JOIN clause.
        /// </param>
        /// <param name="rightSelector">
        /// The right selector of the JOIN clause.
        /// </param>
        /// <typeparam name="TSource">
        /// The type of item on the left side of the LEFT JOIN.
        /// </typeparam>
        /// <typeparam name="TRelation">
        /// The type of item on the right side of the LEFT JOIN.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="EntitySelection{T}"/>.
        /// </returns>
        public EntitySelection<T> LeftJoin<TSource, TRelation>(
            Expression<Func<TSource, object>> leftSelector,
            Expression<Func<TRelation, object>> rightSelector)
        {
            var entityRelation = new EntityRelation(EntityRelationType.LeftJoin);
            entityRelation.Join(leftSelector, rightSelector, null, null);
            return this.AddRelation(entityRelation);
        }

        /// <summary>
        /// Appends a LEFT JOIN clause to the selection. The table represented by <typeparamref name="TSource"/> must already be a 
        /// JOIN member.
        /// </summary>
        /// <param name="leftSelector">
        /// The left selector of the JOIN clause.
        /// </param>
        /// <param name="rightSelector">
        /// The right selector of the JOIN clause.
        /// </param>
        /// <param name="joinAlias">
        /// The join tableAlias.
        /// </param>
        /// <typeparam name="TSource">
        /// The type of item on the left side of the LEFT JOIN.
        /// </typeparam>
        /// <typeparam name="TRelation">
        /// The type of item on the right side of the LEFT JOIN.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="EntitySelection{T}"/>.
        /// </returns>
        public EntitySelection<T> LeftJoin<TSource, TRelation>(
            Expression<Func<TSource, object>> leftSelector,
            Expression<Func<TRelation, object>> rightSelector,
            string joinAlias)
        {
            return this.LeftJoin(leftSelector, null, rightSelector, joinAlias);
        }

        /// <summary>
        /// Appends a LEFT JOIN clause to the selection. The table represented by <typeparamref name="TSource"/> must already be a 
        /// JOIN member.
        /// </summary>
        /// <param name="leftSelector">
        /// The left selector of the JOIN clause.
        /// </param>
        /// <param name="sourceAlias">
        /// The relation table alias.
        /// </param>
        /// <param name="rightSelector">
        /// The right selector of the JOIN clause.
        /// </param>
        /// <param name="relationAlias">
        /// The join table alias.
        /// </param>
        /// <typeparam name="TSource">
        /// The type of item on the left side of the LEFT JOIN.
        /// </typeparam>
        /// <typeparam name="TRelation">
        /// The type of item on the right side of the LEFT JOIN.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="EntitySelection{T}"/>.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Allows fluent usage of the method.")]
        public EntitySelection<T> LeftJoin<TSource, TRelation>(
            Expression<Func<TSource, object>> leftSelector,
            string sourceAlias,
            Expression<Func<TRelation, object>> rightSelector,
            string relationAlias)
        {
            if (leftSelector == null)
            {
                throw new ArgumentNullException(nameof(leftSelector));
            }

            if (rightSelector == null)
            {
                throw new ArgumentNullException(nameof(rightSelector));
            }

            var entityRelation = new EntityRelation(EntityRelationType.LeftJoin);
            entityRelation.Join(leftSelector, rightSelector, sourceAlias, relationAlias);
            return this.AddRelation(entityRelation);
        }

        #endregion

        #region Linked Statements

        /// <summary>
        /// Combines the results of the current selection with the specified selection.
        /// </summary>
        /// <param name="selection">
        /// The selection to combine.
        /// </param>
        /// <returns>
        /// The current <see cref="EntitySelection{T}"/>.
        /// </returns>
        public EntitySelection<T> Union(EntitySelection<T> selection)
        {
            this.LinkedSelection = new LinkedSelection(selection, SelectionLinkType.Union);
            return this;
        }

        /// <summary>
        /// Combines the results of the current selection with the specified selection.
        /// </summary>
        /// <param name="selection">
        /// The selection to combine.
        /// </param>
        /// <returns>
        /// The current <see cref="EntitySelection{T}"/>.
        /// </returns>
        public EntitySelection<T> Intersect(EntitySelection<T> selection)
        {
            this.LinkedSelection = new LinkedSelection(selection, SelectionLinkType.Intersection);
            return this;
        }

        /// <summary>
        /// Combines the results of the current selection with the specified selection.
        /// </summary>
        /// <param name="selection">
        /// The selection to combine.
        /// </param>
        /// <returns>
        /// The current <see cref="EntitySelection{T}"/>.
        /// </returns>
        public EntitySelection<T> Except(EntitySelection<T> selection)
        {
            this.LinkedSelection = new LinkedSelection(selection, SelectionLinkType.Exception);
            return this;
        }

        #endregion

        /// <summary>
        /// Maps the current selection to the target selection type.
        /// </summary>
        /// <typeparam name="TDestEntity">
        /// The destination entity type.
        /// </typeparam>
        /// <returns>
        /// An <see cref="EntitySelection{T}"/> for the destination type.
        /// </returns>
        public EntitySelection<TDestEntity> MapTo<TDestEntity>()
            where TDestEntity : class, new()
        {
            var mappedSelection = MapSelection<TDestEntity>(this);

            ISelection currentSelection = this;
            var linkedSelection = mappedSelection;

            while (currentSelection.LinkedSelection != null)
            {
                var targetSelection = MapSelection<TDestEntity>(currentSelection.LinkedSelection.Selection as EntitySelection<T>);
                LinkSelection(currentSelection.LinkedSelection.LinkType, linkedSelection, targetSelection);

                currentSelection = currentSelection.LinkedSelection.Selection;
                linkedSelection = targetSelection;
            }

            return mappedSelection;
        }

        /// <summary>
        /// Returns a <see cref="String"/> that represents the current <see cref="Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="String"/> that represents the current <see cref="Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Join(ValueSeparator, this.valueFilters.OrderBy(x => x.AttributeLocation?.ToString()));
        }

        /// <summary>
        /// Adds a <see cref="ValueFilter"/> to the selection.
        /// </summary>
        /// <param name="valueFilter">
        /// The value filter to add.
        /// </param>
        /// <returns>
        /// The current <see cref="EntitySelection{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="valueFilter"/> is null.
        /// </exception>
        protected EntitySelection<T> AddFilter([NotNull] ValueFilter valueFilter)
        {
            if (valueFilter == null)
            {
                throw new ArgumentNullException(nameof(valueFilter));
            }

            this.valueFilters.Add(valueFilter);
            return this;
        }

        /// <summary>
        /// Links two selections together based on the <paramref name="linkType"/>.
        /// </summary>
        /// <param name="linkType">
        /// The link type.
        /// </param>
        /// <param name="sourceSelection">
        /// The source selection.
        /// </param>
        /// <param name="targetSelection">
        /// The target selection.
        /// </param>
        /// <typeparam name="TDestEntity">
        /// The type of selection to link.
        /// </typeparam>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="linkType"/> is not one of the values of <see cref="SelectionLinkType"/>.
        /// </exception>
        private static void LinkSelection<TDestEntity>(
            SelectionLinkType linkType,
            EntitySelection<TDestEntity> sourceSelection,
            EntitySelection<TDestEntity> targetSelection)
            where TDestEntity : class, new()
        {
            switch (linkType)
            {
                case SelectionLinkType.Union:
                    sourceSelection.Union(targetSelection);
                    break;
                case SelectionLinkType.Intersection:
                    sourceSelection.Intersect(targetSelection);
                    break;
                case SelectionLinkType.Exception:
                    sourceSelection.Except(targetSelection);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(linkType));
            }
        }

        /// <summary>
        /// Maps the core part of the selection from this selection to an <see cref="EntitySelection{T}"/>.
        /// </summary>
        /// <param name="sourceSelection">
        /// The source selection.
        /// </param>
        /// <typeparam name="TDestEntity">
        /// The type of the selection items to map to.
        /// </typeparam>
        /// <returns>
        /// The mapped <see cref="EntitySelection{T}"/>.
        /// </returns>
        private static EntitySelection<TDestEntity> MapSelection<TDestEntity>(EntitySelection<T> sourceSelection)
            where TDestEntity : class, new()
        {
            var targetSelection = new EntitySelection<TDestEntity>();
            targetSelection.selectExpressions.AddRange(sourceSelection.selectExpressions);
            targetSelection.valueFilters.AddRange(sourceSelection.valueFilters);
            targetSelection.relations.AddRange(sourceSelection.relations);
            targetSelection.Page.RowOffset = sourceSelection.Page.RowOffset;
            targetSelection.Page.Size = sourceSelection.Page.Size;
            return targetSelection;
        }

        /// <summary>
        /// Selects the attributes to return with the query.
        /// </summary>
        /// <param name="selectors">
        /// The property name selectors. If empty, all attributes are returned.
        /// </param>
        private void Select(params LambdaExpression[] selectors)
        {
            if (selectors == null)
            {
                throw new ArgumentNullException(nameof(selectors));
            }

            this.selectExpressions.AddRange(selectors.Select(expression => new SelectExpression(expression, AggregateFunction.None)));
        }

        /// <summary>
        /// Adds a BETWEEN filter.
        /// </summary>
        /// <param name="propertyExpression">
        /// The property expression.
        /// </param>
        /// <param name="leftValue">
        /// The left value.
        /// </param>
        /// <param name="rightValue">
        /// The right value.
        /// </param>
        private void AddRangeFilter(LambdaExpression propertyExpression, object leftValue, object rightValue)
        {
            if (Evaluate.RecursiveEquals(leftValue, rightValue))
            {
                this.valueFilters.Add(new ValueFilter(propertyExpression, FilterType.Equality, leftValue));
            }
            else
            {
                // Needed when caller can't or won't assign the values such that the lower bound property value is less than the upper 
                // bound property value.
                bool valuesFlipped = leftValue is IComparable comparable && rightValue is IComparable value
                                     && comparable.CompareTo(value) > 0;

                if (valuesFlipped)
                {
                    var tempValue = leftValue;
                    leftValue = rightValue;
                    rightValue = tempValue;
                }

                this.valueFilters.Add(new ValueFilter(propertyExpression, FilterType.Between, leftValue, rightValue));
            }
        }
    }
}
