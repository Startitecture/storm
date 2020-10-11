// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntitySet.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Specifies a set of entities in a repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using JetBrains.Annotations;

    /// <summary>
    /// Specifies a set of entities in a repository.
    /// </summary>
    /// <typeparam name="T">
    /// The type of entity specified in the set.
    /// </typeparam>
    public class EntitySet<T> : IEntitySet
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
        private readonly List<IValueFilter> valueFilters = new List<IValueFilter>();

        /// <summary>
        /// The order by expressions.
        /// </summary>
        private readonly List<OrderExpression> orderByExpressions = new List<OrderExpression>();

        /// <inheritdoc />
        public IEntityExpression ParentExpression { get; private set; }

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
                IEntitySet selection = this;

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

                foreach (var value in this.ParentExpression.Expression.PropertyValues)
                {
                    yield return value;
                }
            }
        }

        /// <inheritdoc />
        public IEnumerable<IValueFilter> Filters => this.valueFilters;

        /// <inheritdoc />
        public IEnumerable<OrderExpression> OrderByExpressions => this.orderByExpressions;

        /// <inheritdoc />
        public ResultPage Page { get; } = new ResultPage();

        /////// <summary>
        /////// Order the results by the specified <paramref name="propertyExpression"/>.
        /////// </summary>
        /////// <param name="propertyExpression">
        /////// The property expression.
        /////// </param>
        /////// <returns>
        /////// The current <see cref="EntitySelection{T}"/>.
        /////// </returns>
        ////public EntitySet<T> OrderBy(Expression<Func<T, object>> propertyExpression)
        ////{
        ////    this.orderByExpressions.Add(new OrderExpression(propertyExpression));
        ////    return this;
        ////}

        /////// <summary>
        /////// Order the results by the specified <paramref name="propertyExpression"/>.
        /////// </summary>
        /////// <param name="propertyExpression">
        /////// The property expression.
        /////// </param>
        /////// <returns>
        /////// The current <see cref="EntitySelection{T}"/>.
        /////// </returns>
        ////public EntitySet<T> OrderByDescending(Expression<Func<T, object>> propertyExpression)
        ////{
        ////    this.orderByExpressions.Add(new OrderExpression(propertyExpression, true));
        ////    return this;
        ////}

        /////// <summary>
        /////// Skips the specified number of rows in the result set.
        /////// </summary>
        /////// <param name="rows">
        /////// The rows to skip.
        /////// </param>
        /////// <returns>
        /////// The current <see cref="EntitySelection{T}"/>.
        /////// </returns>
        ////public EntitySet<T> Skip(int rows)
        ////{
        ////    this.Page.RowOffset = rows < 0 ? 0 : rows;
        ////    return this;
        ////}

        /////// <summary>
        /////// Limits the number of rows returned to the number specified.
        /////// </summary>
        /////// <param name="rows">
        /////// The number of rows to take.
        /////// </param>
        /////// <returns>
        /////// The current <see cref="EntitySelection{T}"/>.
        /////// </returns>
        ////public EntitySet<T> Take(int rows)
        ////{
        ////    this.Page.Size = rows < 0 ? 0 : rows;
        ////    return this;
        ////}

        ////#region Predicates

        /////// <summary>
        /////// Matches the primary key of the specified <paramref name="entity"/>.
        /////// </summary>
        /////// <param name="entity">
        /////// The entity to match the key for.
        /////// </param>
        /////// <param name="definitionProvider">
        /////// The definition provider for the entity.
        /////// </param>
        /////// <returns>
        /////// The current <see cref="EntitySelection{T}"/>.
        /////// </returns>
        /////// <remarks>
        /////// This operation will clear any existing filters.
        /////// </remarks>
        ////public EntitySet<T> MatchKey([NotNull] T entity, [NotNull] IEntityDefinitionProvider definitionProvider)
        ////{
        ////    if (entity == null)
        ////    {
        ////        throw new ArgumentNullException(nameof(entity));
        ////    }

        ////    if (definitionProvider == null)
        ////    {
        ////        throw new ArgumentNullException(nameof(definitionProvider));
        ////    }

        ////    this.valueFilters.Clear();
        ////    var entityDefinition = definitionProvider.Resolve<T>();

        ////    foreach (var keyAttribute in entityDefinition.PrimaryKeyAttributes)
        ////    {
        ////        var entityReference = new EntityReference
        ////                                  {
        ////                                      EntityAlias = keyAttribute.Entity.Alias,
        ////                                      EntityType = keyAttribute.Entity.EntityType
        ////                                  };
        ////        var attributeLocation = new AttributeLocation(keyAttribute.PropertyInfo, entityReference);
        ////        var valueFilter = new ValueFilter(attributeLocation, FilterType.Equality, keyAttribute.GetValueDelegate.DynamicInvoke(entity));

        ////        this.AddFilter(valueFilter);
        ////    }

        ////    // Use all available values if no keys are defined.
        ////    if (this.Filters.Any())
        ////    {
        ////        return this;
        ////    }

        ////    Trace.TraceWarning($"{typeof(TimeZoneInfo).FullName} does not have any key attributes defined.");

        ////    foreach (var attribute in entityDefinition.DirectAttributes)
        ////    {
        ////        var entityReference = new EntityReference
        ////                                  {
        ////                                      EntityAlias = attribute.Entity.Alias,
        ////                                      EntityType = attribute.Entity.EntityType
        ////                                  };
        ////        var attributeLocation = new AttributeLocation(attribute.PropertyInfo, entityReference);
        ////        var valueFilter = new ValueFilter(attributeLocation, FilterType.Equality, attribute.GetValueDelegate.DynamicInvoke(entity));
        ////        this.AddFilter(valueFilter);
        ////    }

        ////    return this;
        ////}

        /////// <summary>
        /////// Adds a match filter for the specified example item.
        /////// </summary>
        /////// <param name="example">
        /////// The example to match.
        /////// </param>
        /////// <param name="selectors">
        /////// The selectors of the attributes to match.
        /////// </param>
        /////// <returns>
        /////// The current <see cref="EntitySelection{T}"/>.
        /////// </returns>
        ////public EntitySet<T> Matching(T example, params Expression<Func<T, object>>[] selectors)
        ////{
        ////    if (Evaluate.IsNull(example))
        ////    {
        ////        throw new ArgumentNullException(nameof(example));
        ////    }

        ////    if (selectors == null)
        ////    {
        ////        throw new ArgumentNullException(nameof(selectors));
        ////    }

        ////    foreach (var selector in selectors)
        ////    {
        ////        var value = selector.Compile().Invoke(example);
        ////        this.valueFilters.Add(new ValueFilter(selector, FilterType.Equality, value));
        ////    }

        ////    return this;
        ////}

        /////// <summary>
        /////// Adds a equality filter for the specified property.
        /////// </summary>
        /////// <param name="valueExpression">
        /////// The value expression.
        /////// </param>
        /////// <param name="value">
        /////// The value to match.
        /////// </param>
        /////// <typeparam name="TValue">
        /////// The type of the value.
        /////// </typeparam>
        /////// <returns>
        /////// The current <see cref="EntitySelection{T}"/>.
        /////// </returns>
        /////// <exception cref="ArgumentNullException">
        /////// <paramref name="valueExpression"/> is null.
        /////// </exception>
        ////public EntitySet<T> WhereEqual<TValue>([NotNull] Expression<Func<T, TValue>> valueExpression, TValue value)
        ////{
        ////    return this.WhereEqual(valueExpression as LambdaExpression, value);
        ////}

        /////// <summary>
        /////// Adds a equality filter for the specified property.
        /////// </summary>
        /////// <param name="valueExpression">
        /////// The value expression.
        /////// </param>
        /////// <param name="value">
        /////// The value to match.
        /////// </param>
        /////// <typeparam name="TValue">
        /////// The type of the value.
        /////// </typeparam>
        /////// <returns>
        /////// The current <see cref="EntitySelection{T}"/>.
        /////// </returns>
        /////// <exception cref="ArgumentNullException">
        /////// <paramref name="valueExpression"/> is null.
        /////// </exception>
        ////public EntitySet<T> WhereEqual<TValue>([NotNull] LambdaExpression valueExpression, TValue value)
        ////{
        ////    if (valueExpression == null)
        ////    {
        ////        throw new ArgumentNullException(nameof(valueExpression));
        ////    }

        ////    var valueFilter = Evaluate.IsNull(value)
        ////                          ? new ValueFilter(valueExpression, FilterType.IsNull, value)
        ////                          : new ValueFilter(valueExpression, FilterType.Equality, value);

        ////    this.valueFilters.Add(valueFilter);
        ////    return this;
        ////}

        /////// <summary>
        /////// Adds a greater than filter for the specified property and <paramref name="value"/>.
        /////// </summary>
        /////// <param name="valueExpression">
        /////// The value expression.
        /////// </param>
        /////// <param name="value">
        /////// The value.
        /////// </param>
        /////// <typeparam name="TValue">
        /////// The type of value to evaluate.
        /////// </typeparam>
        /////// <returns>
        /////// The current <see cref="EntitySelection{T}"/>.
        /////// </returns>
        /////// <exception cref="ArgumentNullException">
        /////// <paramref name="valueExpression"/> is null.
        /////// </exception>
        ////public EntitySet<T> GreaterThan<TValue>([NotNull] Expression<Func<T, TValue>> valueExpression, TValue value)
        ////{
        ////    if (valueExpression == null)
        ////    {
        ////        throw new ArgumentNullException(nameof(valueExpression));
        ////    }

        ////    this.valueFilters.Add(new ValueFilter(valueExpression, FilterType.GreaterThan, value));
        ////    return this;
        ////}

        /////// <summary>
        /////// Adds a greater than filter for the specified property and <paramref name="value"/>.
        /////// </summary>
        /////// <param name="valueExpression">
        /////// The value expression.
        /////// </param>
        /////// <param name="value">
        /////// The value.
        /////// </param>
        /////// <typeparam name="TValue">
        /////// The type of value to evaluate.
        /////// </typeparam>
        /////// <returns>
        /////// The current <see cref="EntitySelection{T}"/>.
        /////// </returns>
        /////// <exception cref="ArgumentNullException">
        /////// <paramref name="valueExpression"/> is null.
        /////// </exception>
        ////public EntitySet<T> GreaterThanOrEqualTo<TValue>([NotNull] Expression<Func<T, TValue>> valueExpression, TValue value)
        ////{
        ////    if (valueExpression == null)
        ////    {
        ////        throw new ArgumentNullException(nameof(valueExpression));
        ////    }

        ////    this.valueFilters.Add(new ValueFilter(valueExpression, FilterType.GreaterThanOrEqualTo, value));
        ////    return this;
        ////}

        /////// <summary>
        /////// Adds a less than filter for the specified property and <paramref name="value"/>.
        /////// </summary>
        /////// <param name="valueExpression">
        /////// The value expression.
        /////// </param>
        /////// <param name="value">
        /////// The value.
        /////// </param>
        /////// <typeparam name="TValue">
        /////// The type of value to evaluate.
        /////// </typeparam>
        /////// <returns>
        /////// The current <see cref="EntitySelection{T}"/>.
        /////// </returns>
        /////// <exception cref="ArgumentNullException">
        /////// <paramref name="valueExpression"/> is null.
        /////// </exception>
        ////public EntitySet<T> LessThan<TValue>([NotNull] Expression<Func<T, TValue>> valueExpression, TValue value)
        ////{
        ////    if (valueExpression == null)
        ////    {
        ////        throw new ArgumentNullException(nameof(valueExpression));
        ////    }

        ////    this.valueFilters.Add(new ValueFilter(valueExpression, FilterType.LessThan, value));
        ////    return this;
        ////}

        /////// <summary>
        /////// Adds a less than or equal to filter for the specified property and <paramref name="value"/>.
        /////// </summary>
        /////// <param name="valueExpression">
        /////// The value expression.
        /////// </param>
        /////// <param name="value">
        /////// The value.
        /////// </param>
        /////// <typeparam name="TValue">
        /////// The type of value to evaluate.
        /////// </typeparam>
        /////// <returns>
        /////// The current <see cref="EntitySelection{T}"/>.
        /////// </returns>
        /////// <exception cref="ArgumentNullException">
        /////// <paramref name="valueExpression"/> is null.
        /////// </exception>
        ////public EntitySet<T> LessThanOrEqualTo<TValue>([NotNull] Expression<Func<T, TValue>> valueExpression, TValue value)
        ////{
        ////    if (valueExpression == null)
        ////    {
        ////        throw new ArgumentNullException(nameof(valueExpression));
        ////    }

        ////    this.valueFilters.Add(new ValueFilter(valueExpression, FilterType.LessThanOrEqualTo, value));
        ////    return this;
        ////}

        /////// <summary>
        /////// Adds an inclusion filter for the specified example item.
        /////// </summary>
        /////// <typeparam name="TValue">
        /////// The type of value specified by the selector.
        /////// </typeparam>
        /////// <param name="selector">
        /////// The property selector.
        /////// </param>
        /////// <param name="inclusionValues">
        /////// The inclusion values.
        /////// </param>
        /////// <returns>
        /////// The current <see cref="EntitySelection{T}"/>.
        /////// </returns>
        ////public EntitySet<T> Include<TValue>(Expression<Func<T, TValue>> selector, params TValue[] inclusionValues)
        ////{
        ////    if (selector == null)
        ////    {
        ////        throw new ArgumentNullException(nameof(selector));
        ////    }

        ////    if (inclusionValues == null)
        ////    {
        ////        throw new ArgumentNullException(nameof(inclusionValues));
        ////    }

        ////    var valueFilter = new ValueFilter(selector, FilterType.MatchesSet, inclusionValues.Cast<object>().ToArray());
        ////    this.valueFilters.Add(valueFilter);
        ////    return this;
        ////}

        /////// <summary>
        /////// Adds a between filter for the specified attribute
        /////// </summary>
        /////// <param name="selector">
        /////// The attribute selector.
        /////// </param>
        /////// <param name="minValue">
        /////// The minimum value.
        /////// </param>
        /////// <param name="maxValue">
        /////// The maximum value.
        /////// </param>
        /////// <typeparam name="TValue">
        /////// The type of the value to compare.
        /////// </typeparam>
        /////// <returns>
        /////// The current <see cref="EntitySelection{T}"/>.
        /////// </returns>
        /////// <exception cref="ArgumentNullException">
        /////// <paramref name="selector"/>, <paramref name="minValue"/> or <paramref name="maxValue"/> is null.
        /////// </exception>
        ////public EntitySet<T> Between<TValue>([NotNull] Expression<Func<T, TValue>> selector, [NotNull] TValue minValue, [NotNull] TValue maxValue)
        ////    where TValue : IComparable
        ////{
        ////    if (selector == null)
        ////    {
        ////        throw new ArgumentNullException(nameof(selector));
        ////    }

        ////    if (minValue == null)
        ////    {
        ////        throw new ArgumentNullException(nameof(minValue));
        ////    }

        ////    if (maxValue == null)
        ////    {
        ////        throw new ArgumentNullException(nameof(maxValue));
        ////    }

        ////    var min = minValue.CompareTo(maxValue) < 0 ? minValue : maxValue;
        ////    var max = maxValue.CompareTo(minValue) > 0 ? maxValue : minValue;

        ////    var valueFilter = new ValueFilter(selector, FilterType.Between, min, max);
        ////    return this.AddFilter(valueFilter);
        ////}

        /////// <summary>
        /////// Adds a between filter for the specified example item.
        /////// </summary>
        /////// <param name="baseline">
        /////// The baseline item.
        /////// </param>
        /////// <param name="boundary">
        /////// The boundary item.
        /////// </param>
        /////// <param name="selectors">
        /////// The selectors of the attributes to match.
        /////// </param>
        /////// <returns>
        /////// The current <see cref="EntitySelection{T}"/>.
        /////// </returns>
        ////public EntitySet<T> Between(T baseline, T boundary, params Expression<Func<T, object>>[] selectors)
        ////{
        ////    if (Evaluate.IsNull(baseline))
        ////    {
        ////        throw new ArgumentNullException(nameof(baseline));
        ////    }

        ////    if (Evaluate.IsNull(boundary))
        ////    {
        ////        throw new ArgumentNullException(nameof(boundary));
        ////    }

        ////    if (selectors == null)
        ////    {
        ////        throw new ArgumentNullException(nameof(selectors));
        ////    }

        ////    if (selectors.Any() == false)
        ////    {
        ////        throw new ArgumentException(ValidationMessages.SpecifyAtLeastOneParameter, nameof(selectors));
        ////    }

        ////    foreach (var selector in selectors)
        ////    {
        ////        var compiledSelector = selector.Compile();
        ////        this.AddRangeFilter(selector, compiledSelector.Invoke(baseline), compiledSelector.Invoke(boundary));
        ////    }

        ////    return this;
        ////}

        /////// <summary>
        /////// Adds a predicate that requires an attribute of the current <see cref="EntitySelection{T}"/> be found in the specified sub-query,
        /////// represented by <paramref name="entitySet"/>.
        /////// </summary>
        /////// <param name="sourceAttribute">
        /////// The source attribute to require a match with.
        /////// </param>
        /////// <param name="entitySet">
        /////// The entity set to query.
        /////// </param>
        /////// <param name="matchAttribute">
        /////// The match attribute in the <paramref name="entitySet"/>.
        /////// </param>
        /////// <typeparam name="TSet">
        /////// The type of entity in which the match is to be found.
        /////// </typeparam>
        /////// <returns>
        /////// The current <see cref="EntitySelection{T}"/>.
        /////// </returns>
        /////// <exception cref="ArgumentNullException">
        /////// <paramref name="sourceAttribute"/>, <paramref name="entitySet"/>, or <paramref name="matchAttribute"/> is null.
        /////// </exception>
        ////public EntitySet<T> ExistsIn<TSet>(
        ////    [NotNull] Expression<Func<T, object>> sourceAttribute,
        ////    [NotNull] EntitySet<TSet> entitySet,
        ////    [NotNull] Expression<Func<TSet, object>> matchAttribute)
        ////{
        ////    if (entitySet == null)
        ////    {
        ////        throw new ArgumentNullException(nameof(entitySet));
        ////    }

        ////    if (sourceAttribute == null)
        ////    {
        ////        throw new ArgumentNullException(nameof(sourceAttribute));
        ////    }

        ////    if (matchAttribute == null)
        ////    {
        ////        throw new ArgumentNullException(nameof(matchAttribute));
        ////    }

        ////    var entityRelation = new EntityRelation(EntityRelationType.InnerJoin);
        ////    entityRelation.Join(sourceAttribute, matchAttribute);

        ////    var entityRelations = new List<IEntityRelation>
        ////                              {
        ////                                  entityRelation
        ////                              };

        ////    var setExpression = new RelationExpression(entitySet, entityRelations);

        ////    this.existsPredicates.Add(setExpression);
        ////    return this;
        ////}

        ////#endregion

        #region Relations

        /// <summary>
        /// Sets the default relations for the entity set using the specified <paramref name="definitionProvider"/>.
        /// </summary>
        /// <param name="definitionProvider">
        /// The definition provider to get the default relations from.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="definitionProvider"/> is null.
        /// </exception>
        public void SetDefaultRelations([NotNull] IEntityDefinitionProvider definitionProvider)
        {
            if (definitionProvider == null)
            {
                throw new ArgumentNullException(nameof(definitionProvider));
            }

            this.relations.Clear();
            this.relations.AddRange(definitionProvider.Resolve<T>().DefaultRelations);
        }

        /// <summary>
        /// Define the set of relations from which the selection should be taken.
        /// </summary>
        /// <param name="setRelations">
        /// The set of relations to include.
        /// </param>
        /// <returns>
        /// The current <see cref="EntitySet{T}"/>.
        /// </returns>
        public EntitySet<T> From([NotNull] Action<EntityRelationSet<T>> setRelations)
        {
            if (setRelations == null)
            {
                throw new ArgumentNullException(nameof(setRelations));
            }

            var relationSet = new EntityRelationSet<T>();
            setRelations.Invoke(relationSet);
            this.AddRelations(relationSet.Relations);
            return this;
        }

        /// <summary>
        /// Defines the filters to restrict results to.
        /// </summary>
        /// <param name="setFilter">
        /// The entity set filter.
        /// </param>
        /// <returns>
        /// The current <see cref="EntitySet{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="setFilter"/> is null.
        /// </exception>
        public EntitySet<T> Where([NotNull] Action<ValueFilterSet<T>> setFilter)
        {
            if (setFilter == null)
            {
                throw new ArgumentNullException(nameof(setFilter));
            }

            var filterSet = new ValueFilterSet<T>();
            setFilter.Invoke(filterSet);
            this.SetFilters(filterSet);
            return this;
        }

        /// <summary>
        /// Sorts the results of the entity selection.
        /// </summary>
        /// <param name="sortOrder">
        /// The sort order to apply.
        /// </param>
        /// <returns>
        /// The current <see cref="EntitySet{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sortOrder"/> is null.
        /// </exception>
        public EntitySet<T> Sort([NotNull] Action<OrderExpressionSet<T>> sortOrder)
        {
            if (sortOrder == null)
            {
                throw new ArgumentNullException(nameof(sortOrder));
            }

            var expressionSet = new OrderExpressionSet<T>();
            sortOrder.Invoke(expressionSet);
            this.SortResults(expressionSet);
            return this;
        }

        /// <summary>
        /// Seeks a subset of results from the matching result set.
        /// </summary>
        /// <param name="limit">
        /// The result set limits to apply.
        /// </param>
        /// <returns>
        /// The current <see cref="EntitySet{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="limit"/> is null.
        /// </exception>
        public EntitySet<T> Seek([NotNull] Action<Subset> limit)
        {
            if (limit == null)
            {
                throw new ArgumentNullException(nameof(limit));
            }

            var subset = new Subset();
            limit.Invoke(subset);
            this.SetOffsetAndSize(subset);
            return this;
        }

        /////// <summary>
        /////// Appends an INNER JOIN clause to the selection.
        /////// </summary>
        /////// <param name="leftSelector">
        /////// The left selector of the JOIN clause.
        /////// </param>
        /////// <param name="rightSelector">
        /////// The right selector of the JOIN clause.
        /////// </param>
        /////// <returns>
        /////// The current <see cref="EntitySelection{T}"/>.
        /////// </returns>
        ////public EntitySet<T> InnerJoin([NotNull] Expression<Func<T, object>> leftSelector, [NotNull] Expression<Func<T, object>> rightSelector)
        ////{
        ////    if (leftSelector == null)
        ////    {
        ////        throw new ArgumentNullException(nameof(leftSelector));
        ////    }

        ////    if (rightSelector == null)
        ////    {
        ////        throw new ArgumentNullException(nameof(rightSelector));
        ////    }

        ////    var relation = new EntityRelation(EntityRelationType.InnerJoin);
        ////    relation.Join<T>(leftSelector, rightSelector);
        ////    return this.AddRelation(relation);
        ////}

        /////// <summary>
        /////// Appends an INNER JOIN clause to the selection.
        /////// </summary>
        /////// <param name="leftSelector">
        /////// The left selector of the JOIN clause.
        /////// </param>
        /////// <param name="rightSelector">
        /////// The right selector of the JOIN clause.
        /////// </param>
        /////// <typeparam name="TRelation">
        /////// The type of item on the right side of the INNER JOIN.
        /////// </typeparam>
        /////// <returns>
        /////// The current <see cref="EntitySelection{T}"/>.
        /////// </returns>
        ////public EntitySet<T> InnerJoin<TRelation>(Expression<Func<T, object>> leftSelector, Expression<Func<TRelation, object>> rightSelector)
        ////{
        ////    if (leftSelector == null)
        ////    {
        ////        throw new ArgumentNullException(nameof(leftSelector));
        ////    }

        ////    if (rightSelector == null)
        ////    {
        ////        throw new ArgumentNullException(nameof(rightSelector));
        ////    }

        ////    var entityRelation = new EntityRelation(EntityRelationType.InnerJoin);
        ////    entityRelation.Join(leftSelector, rightSelector, null, null);
        ////    return this.AddRelation(entityRelation);
        ////}

        /////// <summary>
        /////// Appends an INNER JOIN clause to the selection.
        /////// </summary>
        /////// <param name="leftSelector">
        /////// The left selector of the JOIN clause.
        /////// </param>
        /////// <param name="rightSelector">
        /////// The right selector of the JOIN clause.
        /////// </param>
        /////// <param name="relationAlias">
        /////// The alias for the joined entity.
        /////// </param>
        /////// <typeparam name="TRelation">
        /////// The type of item on the right side of the INNER JOIN.
        /////// </typeparam>
        /////// <returns>
        /////// The current <see cref="EntitySelection{T}"/>.
        /////// </returns>
        ////public EntitySet<T> InnerJoin<TRelation>(
        ////    Expression<Func<T, object>> leftSelector,
        ////    Expression<Func<TRelation, object>> rightSelector,
        ////    string relationAlias)
        ////{
        ////    if (leftSelector == null)
        ////    {
        ////        throw new ArgumentNullException(nameof(leftSelector));
        ////    }

        ////    if (rightSelector == null)
        ////    {
        ////        throw new ArgumentNullException(nameof(rightSelector));
        ////    }

        ////    var entityRelation = new EntityRelation(EntityRelationType.InnerJoin);
        ////    entityRelation.Join(leftSelector, rightSelector, null, relationAlias);
        ////    return this.AddRelation(entityRelation);
        ////}

        /////// <summary>
        /////// Appends an INNER JOIN clause to the selection. The table represented by <typeparamref name="TSource"/> must already be
        /////// a
        /////// JOIN member.
        /////// </summary>
        /////// <param name="leftSelector">
        /////// The left selector of the JOIN clause.
        /////// </param>
        /////// <param name="rightSelector">
        /////// The right selector of the JOIN clause.
        /////// </param>
        /////// <typeparam name="TSource">
        /////// The type of item on the left side of the INNER JOIN.
        /////// </typeparam>
        /////// <typeparam name="TRelation">
        /////// The type of item on the right side of the INNER JOIN.
        /////// </typeparam>
        /////// <returns>
        /////// The current <see cref="EntitySelection{T}"/>.
        /////// </returns>
        ////public EntitySet<T> InnerJoin<TSource, TRelation>(
        ////    Expression<Func<TSource, object>> leftSelector,
        ////    Expression<Func<TRelation, object>> rightSelector)
        ////{
        ////    var entityRelation = new EntityRelation(EntityRelationType.InnerJoin);
        ////    entityRelation.Join(leftSelector, rightSelector, null, null);
        ////    return this.AddRelation(entityRelation);
        ////}

        /////// <summary>
        /////// Appends an INNER JOIN clause to the selection. The table represented by <typeparamref name="TSource"/> must already be
        /////// a
        /////// JOIN member.
        /////// </summary>
        /////// <param name="leftSelector">
        /////// The left selector of the JOIN clause.
        /////// </param>
        /////// <param name="rightSelector">
        /////// The right selector of the JOIN clause.
        /////// </param>
        /////// <param name="relationAlias">
        /////// The alias for the joined entity.
        /////// </param>
        /////// <typeparam name="TSource">
        /////// The type of item on the left side of the INNER JOIN.
        /////// </typeparam>
        /////// <typeparam name="TRelation">
        /////// The type of item on the right side of the INNER JOIN.
        /////// </typeparam>
        /////// <returns>
        /////// The current <see cref="EntitySelection{T}"/>.
        /////// </returns>
        ////public EntitySet<T> InnerJoin<TSource, TRelation>(
        ////    Expression<Func<TSource, object>> leftSelector,
        ////    Expression<Func<TRelation, object>> rightSelector,
        ////    string relationAlias)
        ////{
        ////    return this.InnerJoin(leftSelector, null, rightSelector, relationAlias);
        ////}

        /////// <summary>
        /////// Appends an INNER JOIN clause to the selection. The table represented by <typeparamref name="TSource"/> must already be
        /////// a
        /////// JOIN member.
        /////// </summary>
        /////// <param name="leftSelector">
        /////// The left selector of the JOIN clause.
        /////// </param>
        /////// <param name="sourceAlias">
        /////// The alias for the related entity.
        /////// </param>
        /////// <param name="rightSelector">
        /////// The right selector of the JOIN clause.
        /////// </param>
        /////// <param name="relationAlias">
        /////// The alias for the joined entity.
        /////// </param>
        /////// <typeparam name="TSource">
        /////// The type of item on the left side of the INNER JOIN.
        /////// </typeparam>
        /////// <typeparam name="TRelation">
        /////// The type of item on the right side of the INNER JOIN.
        /////// </typeparam>
        /////// <returns>
        /////// The current <see cref="EntitySelection{T}"/>.
        /////// </returns>
        ////[System.Diagnostics.CodeAnalysis.SuppressMessage(
        ////    "Microsoft.Design",
        ////    "CA1006:DoNotNestGenericTypesInMemberSignatures",
        ////    Justification = "Allows fluent usage of the method.")]
        ////public EntitySet<T> InnerJoin<TSource, TRelation>(
        ////    Expression<Func<TSource, object>> leftSelector,
        ////    string sourceAlias,
        ////    Expression<Func<TRelation, object>> rightSelector,
        ////    string relationAlias)
        ////{
        ////    if (leftSelector == null)
        ////    {
        ////        throw new ArgumentNullException(nameof(leftSelector));
        ////    }

        ////    if (rightSelector == null)
        ////    {
        ////        throw new ArgumentNullException(nameof(rightSelector));
        ////    }

        ////    var entityRelation = new EntityRelation(EntityRelationType.InnerJoin);
        ////    entityRelation.Join(leftSelector, rightSelector, sourceAlias, relationAlias);
        ////    return this.AddRelation(entityRelation);
        ////}

        /////// <summary>
        /////// Appends a LEFT JOIN clause to the selection.
        /////// </summary>
        /////// <param name="leftSelector">
        /////// The left selector of the JOIN clause.
        /////// </param>
        /////// <param name="rightSelector">
        /////// The right selector of the JOIN clause.
        /////// </param>
        /////// <returns>
        /////// The current <see cref="EntitySelection{T}"/>.
        /////// </returns>
        ////public EntitySet<T> LeftJoin([NotNull] Expression<Func<T, object>> leftSelector, [NotNull] Expression<Func<T, object>> rightSelector)
        ////{
        ////    if (leftSelector == null)
        ////    {
        ////        throw new ArgumentNullException(nameof(leftSelector));
        ////    }

        ////    if (rightSelector == null)
        ////    {
        ////        throw new ArgumentNullException(nameof(rightSelector));
        ////    }

        ////    var relation = new EntityRelation(EntityRelationType.LeftJoin);
        ////    relation.Join<T>(leftSelector, rightSelector);
        ////    return this.AddRelation(relation);
        ////}

        /////// <summary>
        /////// Appends a LEFT JOIN clause to the selection.
        /////// </summary>
        /////// <param name="leftSelector">
        /////// The left selector of the JOIN clause.
        /////// </param>
        /////// <param name="rightSelector">
        /////// The right selector of the JOIN clause.
        /////// </param>
        /////// <typeparam name="TRelation">
        /////// The type of item on the right side of the LEFT JOIN.
        /////// </typeparam>
        /////// <returns>
        /////// The current <see cref="EntitySelection{T}"/>.
        /////// </returns>
        ////public EntitySet<T> LeftJoin<TRelation>(Expression<Func<T, object>> leftSelector, Expression<Func<TRelation, object>> rightSelector)
        ////{
        ////    if (leftSelector == null)
        ////    {
        ////        throw new ArgumentNullException(nameof(leftSelector));
        ////    }

        ////    if (rightSelector == null)
        ////    {
        ////        throw new ArgumentNullException(nameof(rightSelector));
        ////    }

        ////    var entityRelation = new EntityRelation(EntityRelationType.LeftJoin);
        ////    entityRelation.Join(leftSelector, rightSelector, null, null);
        ////    return this.AddRelation(entityRelation);
        ////}

        /////// <summary>
        /////// Appends a LEFT JOIN clause to the selection.
        /////// </summary>
        /////// <param name="leftSelector">
        /////// The left selector of the JOIN clause.
        /////// </param>
        /////// <param name="rightSelector">
        /////// The right selector of the JOIN clause.
        /////// </param>
        /////// <param name="relationAlias">
        /////// The alias for the joined entity.
        /////// </param>
        /////// <typeparam name="TRelation">
        /////// The type of item on the right side of the LEFT JOIN.
        /////// </typeparam>
        /////// <returns>
        /////// The current <see cref="EntitySelection{T}"/>.
        /////// </returns>
        ////public EntitySet<T> LeftJoin<TRelation>(
        ////    Expression<Func<T, object>> leftSelector,
        ////    Expression<Func<TRelation, object>> rightSelector,
        ////    string relationAlias)
        ////{
        ////    if (leftSelector == null)
        ////    {
        ////        throw new ArgumentNullException(nameof(leftSelector));
        ////    }

        ////    if (rightSelector == null)
        ////    {
        ////        throw new ArgumentNullException(nameof(rightSelector));
        ////    }

        ////    var entityRelation = new EntityRelation(EntityRelationType.LeftJoin);
        ////    entityRelation.Join(leftSelector, rightSelector, null, relationAlias);
        ////    return this.AddRelation(entityRelation);
        ////}

        /////// <summary>
        /////// Appends a LEFT JOIN clause to the selection. The table represented by <typeparamref name="TSource"/> must already be a
        /////// JOIN member.
        /////// </summary>
        /////// <param name="leftSelector">
        /////// The left selector of the JOIN clause.
        /////// </param>
        /////// <param name="rightSelector">
        /////// The right selector of the JOIN clause.
        /////// </param>
        /////// <typeparam name="TSource">
        /////// The type of item on the left side of the LEFT JOIN.
        /////// </typeparam>
        /////// <typeparam name="TRelation">
        /////// The type of item on the right side of the LEFT JOIN.
        /////// </typeparam>
        /////// <returns>
        /////// The current <see cref="EntitySelection{T}"/>.
        /////// </returns>
        ////public EntitySet<T> LeftJoin<TSource, TRelation>(
        ////    Expression<Func<TSource, object>> leftSelector,
        ////    Expression<Func<TRelation, object>> rightSelector)
        ////{
        ////    var entityRelation = new EntityRelation(EntityRelationType.LeftJoin);
        ////    entityRelation.Join(leftSelector, rightSelector, null, null);
        ////    return this.AddRelation(entityRelation);
        ////}

        /////// <summary>
        /////// Appends a LEFT JOIN clause to the selection. The table represented by <typeparamref name="TSource"/> must already be a
        /////// JOIN member.
        /////// </summary>
        /////// <param name="leftSelector">
        /////// The left selector of the JOIN clause.
        /////// </param>
        /////// <param name="rightSelector">
        /////// The right selector of the JOIN clause.
        /////// </param>
        /////// <param name="joinAlias">
        /////// The join tableAlias.
        /////// </param>
        /////// <typeparam name="TSource">
        /////// The type of item on the left side of the LEFT JOIN.
        /////// </typeparam>
        /////// <typeparam name="TRelation">
        /////// The type of item on the right side of the LEFT JOIN.
        /////// </typeparam>
        /////// <returns>
        /////// The current <see cref="EntitySelection{T}"/>.
        /////// </returns>
        ////public EntitySet<T> LeftJoin<TSource, TRelation>(
        ////    Expression<Func<TSource, object>> leftSelector,
        ////    Expression<Func<TRelation, object>> rightSelector,
        ////    string joinAlias)
        ////{
        ////    return this.LeftJoin(leftSelector, null, rightSelector, joinAlias);
        ////}

        /////// <summary>
        /////// Appends a LEFT JOIN clause to the selection. The table represented by <typeparamref name="TSource"/> must already be a
        /////// JOIN member.
        /////// </summary>
        /////// <param name="leftSelector">
        /////// The left selector of the JOIN clause.
        /////// </param>
        /////// <param name="sourceAlias">
        /////// The relation table alias.
        /////// </param>
        /////// <param name="rightSelector">
        /////// The right selector of the JOIN clause.
        /////// </param>
        /////// <param name="relationAlias">
        /////// The join table alias.
        /////// </param>
        /////// <typeparam name="TSource">
        /////// The type of item on the left side of the LEFT JOIN.
        /////// </typeparam>
        /////// <typeparam name="TRelation">
        /////// The type of item on the right side of the LEFT JOIN.
        /////// </typeparam>
        /////// <returns>
        /////// The current <see cref="EntitySelection{T}"/>.
        /////// </returns>
        ////[System.Diagnostics.CodeAnalysis.SuppressMessage(
        ////    "Microsoft.Design",
        ////    "CA1006:DoNotNestGenericTypesInMemberSignatures",
        ////    Justification = "Allows fluent usage of the method.")]
        ////public EntitySet<T> LeftJoin<TSource, TRelation>(
        ////    Expression<Func<TSource, object>> leftSelector,
        ////    string sourceAlias,
        ////    Expression<Func<TRelation, object>> rightSelector,
        ////    string relationAlias)
        ////{
        ////    if (leftSelector == null)
        ////    {
        ////        throw new ArgumentNullException(nameof(leftSelector));
        ////    }

        ////    if (rightSelector == null)
        ////    {
        ////        throw new ArgumentNullException(nameof(rightSelector));
        ////    }

        ////    var entityRelation = new EntityRelation(EntityRelationType.LeftJoin);
        ////    entityRelation.Join(leftSelector, rightSelector, sourceAlias, relationAlias);
        ////    return this.AddRelation(entityRelation);
        ////}

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
        public EntitySet<T> Union(EntitySet<T> selection)
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
        public EntitySet<T> Intersect(EntitySet<T> selection)
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
        public EntitySet<T> Except(EntitySet<T> selection)
        {
            this.LinkedSelection = new LinkedSelection(selection, SelectionLinkType.Exception);
            return this;
        }

        #endregion

        /// <summary>
        /// Sets the current entity set's <see cref="ParentExpression"/> to the specified <paramref name="expression"/>.
        /// </summary>
        /// <param name="expression">
        /// The expression to set as the parent expression.
        /// </param>
        /// <typeparam name="TExpression">
        /// The type of entity selected by the expression.
        /// </typeparam>
        public void WithAs<TExpression>(EntityExpression<TExpression> expression)
        {
            this.ParentExpression = expression ?? throw new ArgumentNullException(nameof(expression));
        }

        /// <inheritdoc />
        public virtual EntitySet<TDestEntity> MapSet<TDestEntity>()
            where TDestEntity : class, new()
        {
            var mappedSet = new EntitySet<TDestEntity>();
            this.MapSet(this, mappedSet);
            return mappedSet;
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
            return string.Join(ValueSeparator, this.Filters.OrderBy(x => x.AttributeLocation?.ToString()));
        }

        /// <summary>
        /// Sets the filters for the result set.
        /// </summary>
        /// <param name="filterSet">
        /// The filters to set.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="filterSet"/> is null.
        /// </exception>
        protected void SetFilters([NotNull] ValueFilterSet<T> filterSet)
        {
            if (filterSet == null)
            {
                throw new ArgumentNullException(nameof(filterSet));
            }

            this.valueFilters.Clear();
            this.valueFilters.AddRange(filterSet.ValueFilters);
        }

        /// <summary>
        /// Sets the row offset and size values for the entity set.
        /// </summary>
        /// <param name="subset">
        /// The subset of rows to return.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="subset"/> is null.
        /// </exception>
        protected void SetOffsetAndSize([NotNull] Subset subset)
        {
            if (subset == null)
            {
                throw new ArgumentNullException(nameof(subset));
            }

            this.Page.RowOffset = subset.Page.RowOffset;
            this.Page.Size = subset.Page.Size;
        }

        /// <summary>
        /// Adds relations to the selection.
        /// </summary>
        /// <param name="entityRelations">
        /// The relations.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="entityRelations"/> is null.
        /// </exception>
        protected void AddRelations([NotNull] IEnumerable<IEntityRelation> entityRelations)
        {
            if (entityRelations == null)
            {
                throw new ArgumentNullException(nameof(entityRelations));
            }

            this.relations.AddRange(entityRelations);
        }

        /// <summary>
        /// Sorts the results of the entity set.
        /// </summary>
        /// <param name="order">
        /// The set of order expressions that define the sort.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="order"/> is null.
        /// </exception>
        protected void SortResults([NotNull] OrderExpressionSet<T> order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            this.orderByExpressions.Clear();
            this.orderByExpressions.AddRange(order.Expressions);
        }

        /// <summary>
        /// Maps an entity set of one type to one of the <typeparamref name="TDestEntity"/> type.
        /// </summary>
        /// <param name="sourceSet">
        /// The source entity set.
        /// </param>
        /// <param name="targetSet">
        /// The target entity set.
        /// </param>
        /// <typeparam name="TDestEntity">
        /// The type of entity to map the entity set to.
        /// </typeparam>
        protected void MapSet<TDestEntity>([NotNull] IEntitySet sourceSet, [NotNull] EntitySet<TDestEntity> targetSet)
            where TDestEntity : class, new()
        {
            if (sourceSet == null)
            {
                throw new ArgumentNullException(nameof(sourceSet));
            }

            if (targetSet == null)
            {
                throw new ArgumentNullException(nameof(targetSet));
            }

            var currentSet = sourceSet;
            var linkedSelection = targetSet;

            while (currentSet.LinkedSelection != null)
            {
                var targetSelection = new EntitySet<TDestEntity>();
                this.MapSet(currentSet.LinkedSelection.Selection, targetSelection);
                LinkSelection(currentSet.LinkedSelection.LinkType, linkedSelection, targetSelection);

                currentSet = currentSet.LinkedSelection.Selection;
                linkedSelection = targetSelection;
            }

            targetSet.ParentExpression = sourceSet.ParentExpression;
            targetSet.valueFilters.AddRange(sourceSet.Filters);
            targetSet.relations.AddRange(sourceSet.Relations);
            targetSet.Page.RowOffset = sourceSet.Page.RowOffset;
            targetSet.Page.Size = sourceSet.Page.Size;
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
            EntitySet<TDestEntity> sourceSelection,
            EntitySet<TDestEntity> targetSelection)
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

/*
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
        private EntitySet<T> AddFilter([NotNull] ValueFilter valueFilter)
        {
            if (valueFilter == null)
            {
                throw new ArgumentNullException(nameof(valueFilter));
            }

            this.valueFilters.Add(valueFilter);
            return this;
        }
*/

/*
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
                bool valuesFlipped = leftValue is IComparable comparable && rightValue is IComparable value && comparable.CompareTo(value) > 0;

                if (valuesFlipped)
                {
                    var tempValue = leftValue;
                    leftValue = rightValue;
                    rightValue = tempValue;
                }

                this.valueFilters.Add(new ValueFilter(propertyExpression, FilterType.Between, leftValue, rightValue));
            }
        }
*/
    }
}