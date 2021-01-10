// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueFilterSet.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Resources;

    /// <summary>
    /// Creates filters for the results of an entity selection.
    /// </summary>
    /// <typeparam name="T">
    /// The type of entity to filter values for.
    /// </typeparam>
    public class ValueFilterSet<T>
    {
        /// <summary>
        /// The value filters.
        /// </summary>
        private readonly List<IValueFilter> valueFilters = new List<IValueFilter>();

        /// <summary>
        /// Gets the value filters for the filter set.
        /// </summary>
        public IEnumerable<IValueFilter> ValueFilters => this.valueFilters;

        /// <summary>
        /// Adds an <see cref="IValueFilter"/> to the value filter set.
        /// </summary>
        /// <param name="valueFilter">
        /// The filter to add.
        /// </param>
        /// <returns>
        /// The current <see cref="ValueFilterSet{T}"/>.
        /// </returns>
        public ValueFilterSet<T> Add([NotNull] IValueFilter valueFilter)
        {
            if (valueFilter == null)
            {
                throw new ArgumentNullException(nameof(valueFilter));
            }

            this.valueFilters.Add(valueFilter);
            return this;
        }

        /// <summary>
        /// Matches the primary key of the specified <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">
        /// The entity to match the key for.
        /// </param>
        /// <param name="definitionProvider">
        /// The definition provider for the entity.
        /// </param>
        /// <param name="explicitKeyAttributes">
        /// The explicit key attributes for this match.
        /// </param>
        /// <returns>
        /// The current <see cref="ValueFilterSet{T}"/>.
        /// </returns>
        /// <remarks>
        /// This operation will clear any existing filters.
        /// </remarks>
        public ValueFilterSet<T> MatchKey(
            [NotNull] T entity,
            [NotNull] IEntityDefinitionProvider definitionProvider,
            [NotNull] params Expression<Func<T, object>>[] explicitKeyAttributes)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (definitionProvider == null)
            {
                throw new ArgumentNullException(nameof(definitionProvider));
            }

            if (explicitKeyAttributes == null)
            {
                throw new ArgumentNullException(nameof(explicitKeyAttributes));
            }

            this.valueFilters.Clear();
            var entityDefinition = definitionProvider.Resolve<T>();
            var keyAttributes = explicitKeyAttributes.Any()
                                    ? explicitKeyAttributes.Select(entityDefinition.Find)
                                    : entityDefinition.PrimaryKeyAttributes;

            foreach (var keyAttribute in keyAttributes)
            {
                var entityReference = new EntityReference
                                      {
                                          EntityAlias = keyAttribute.Entity.Alias,
                                          EntityType = keyAttribute.Entity.EntityType
                                      };
                var attributeLocation = new AttributeLocation(keyAttribute.PropertyInfo, entityReference);
                var valueFilter = new ValueFilter(attributeLocation, FilterType.Equality, keyAttribute.GetValueDelegate.DynamicInvoke(entity));

                this.valueFilters.Add(valueFilter);
            }

            // Use all available values if no keys are defined.
            if (this.valueFilters.Any())
            {
                return this;
            }

            Trace.TraceWarning($"{typeof(T).FullName} does not have any key attributes defined.");

            foreach (var attribute in entityDefinition.DirectAttributes)
            {
                var entityReference = new EntityReference
                                      {
                                          EntityAlias = attribute.Entity.Alias,
                                          EntityType = attribute.Entity.EntityType
                                      };
                var attributeLocation = new AttributeLocation(attribute.PropertyInfo, entityReference);
                var valueFilter = new ValueFilter(attributeLocation, FilterType.Equality, attribute.GetValueDelegate.DynamicInvoke(entity));
                this.valueFilters.Add(valueFilter);
            }

            return this;
        }

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
        /// The current <see cref="ValueFilterSet{T}"/>.
        /// </returns>
        public ValueFilterSet<T> Matching(T example, params Expression<Func<T, object>>[] selectors)
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
        /// The current <see cref="ValueFilterSet{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="valueExpression"/> is null.
        /// </exception>
        public ValueFilterSet<T> AreEqual<TValue>([NotNull] Expression<Func<T, TValue>> valueExpression, TValue value)
        {
            return this.AreEqual(valueExpression as LambdaExpression, value);
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
        /// <typeparam name="TEntity">
        /// The type of entity on which the specified attribute is located.
        /// </typeparam>
        /// <typeparam name="TValue">
        /// The type of the value.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="ValueFilterSet{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="valueExpression"/> is null.
        /// </exception>
        public ValueFilterSet<T> AreEqual<TEntity, TValue>([NotNull] Expression<Func<TEntity, TValue>> valueExpression, TValue value)
        {
            return this.AreEqual(valueExpression as LambdaExpression, value);
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
        /// The current <see cref="ValueFilterSet{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="valueExpression"/> is null.
        /// </exception>
        public ValueFilterSet<T> GreaterThan<TValue>([NotNull] Expression<Func<T, TValue>> valueExpression, TValue value)
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
        /// <typeparam name="TEntity">
        /// The type of entity on which the specified attribute is located.
        /// </typeparam>
        /// <typeparam name="TValue">
        /// The type of value to evaluate.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="ValueFilterSet{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="valueExpression"/> is null.
        /// </exception>
        public ValueFilterSet<T> GreaterThan<TEntity, TValue>([NotNull] Expression<Func<TEntity, TValue>> valueExpression, TValue value)
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
        /// The current <see cref="ValueFilterSet{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="valueExpression"/> is null.
        /// </exception>
        public ValueFilterSet<T> GreaterThanOrEqualTo<TValue>([NotNull] Expression<Func<T, TValue>> valueExpression, TValue value)
        {
            if (valueExpression == null)
            {
                throw new ArgumentNullException(nameof(valueExpression));
            }

            this.valueFilters.Add(new ValueFilter(valueExpression, FilterType.GreaterThanOrEqualTo, value));
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
        /// <typeparam name="TEntity">
        /// The type of entity on which the specified attribute is located.
        /// </typeparam>
        /// <typeparam name="TValue">
        /// The type of value to evaluate.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="ValueFilterSet{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="valueExpression"/> is null.
        /// </exception>
        public ValueFilterSet<T> GreaterThanOrEqualTo<TEntity, TValue>([NotNull] Expression<Func<TEntity, TValue>> valueExpression, TValue value)
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
        /// The current <see cref="ValueFilterSet{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="valueExpression"/> is null.
        /// </exception>
        public ValueFilterSet<T> LessThan<TValue>([NotNull] Expression<Func<T, TValue>> valueExpression, TValue value)
        {
            if (valueExpression == null)
            {
                throw new ArgumentNullException(nameof(valueExpression));
            }

            this.valueFilters.Add(new ValueFilter(valueExpression, FilterType.LessThan, value));
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
        /// <typeparam name="TEntity">
        /// The type of entity on which the specified attribute is located.
        /// </typeparam>
        /// <typeparam name="TValue">
        /// The type of value to evaluate.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="ValueFilterSet{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="valueExpression"/> is null.
        /// </exception>
        public ValueFilterSet<T> LessThan<TEntity, TValue>([NotNull] Expression<Func<TEntity, TValue>> valueExpression, TValue value)
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
        /// The current <see cref="ValueFilterSet{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="valueExpression"/> is null.
        /// </exception>
        public ValueFilterSet<T> LessThanOrEqualTo<TValue>([NotNull] Expression<Func<T, TValue>> valueExpression, TValue value)
        {
            if (valueExpression == null)
            {
                throw new ArgumentNullException(nameof(valueExpression));
            }

            this.valueFilters.Add(new ValueFilter(valueExpression, FilterType.LessThanOrEqualTo, value));
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
        /// <typeparam name="TEntity">
        /// The type of entity on which the specified attribute is located.
        /// </typeparam>
        /// <typeparam name="TValue">
        /// The type of value to evaluate.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="ValueFilterSet{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="valueExpression"/> is null.
        /// </exception>
        public ValueFilterSet<T> LessThanOrEqualTo<TEntity, TValue>([NotNull] Expression<Func<TEntity, TValue>> valueExpression, TValue value)
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
        /// The current <see cref="ValueFilterSet{T}"/>.
        /// </returns>
        public ValueFilterSet<T> Include<TValue>(Expression<Func<T, TValue>> selector, params TValue[] inclusionValues)
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
        /// Adds an inclusion filter for the specified example item.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The type of entity on which the specified attribute is located.
        /// </typeparam>
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
        /// The current <see cref="ValueFilterSet{T}"/>.
        /// </returns>
        public ValueFilterSet<T> Include<TEntity, TValue>(Expression<Func<TEntity, TValue>> selector, params TValue[] inclusionValues)
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
        /// Adds a between filter for the specified attribute.
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
        /// The current <see cref="ValueFilterSet{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="selector"/>, <paramref name="minValue"/> or <paramref name="maxValue"/> is null.
        /// </exception>
        public ValueFilterSet<T> Between<TValue>([NotNull] Expression<Func<T, TValue>> selector, [NotNull] TValue minValue, [NotNull] TValue maxValue)
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
            this.valueFilters.Add(valueFilter);
            return this;
        }

        /// <summary>
        /// Adds a between filter for the specified attribute.
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
        /// <typeparam name="TEntity">
        /// The type of entity on which the specified attribute is located.
        /// </typeparam>
        /// <typeparam name="TValue">
        /// The type of the value to compare.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="ValueFilterSet{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="selector"/>, <paramref name="minValue"/> or <paramref name="maxValue"/> is null.
        /// </exception>
        public ValueFilterSet<T> Between<TEntity, TValue>(
            [NotNull] Expression<Func<TEntity, TValue>> selector,
            [NotNull] TValue minValue,
            [NotNull] TValue maxValue)
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
            this.valueFilters.Add(valueFilter);
            return this;
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
        /// The current <see cref="ValueFilterSet{T}"/>.
        /// </returns>
        public ValueFilterSet<T> Between(T baseline, T boundary, params Expression<Func<T, object>>[] selectors)
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

        /// <summary>
        /// Adds a predicate that requires an attribute of the current <see cref="EntitySelection{T}"/> be found in the specified
        /// sub-query,
        /// represented by <paramref name="entitySet"/>.
        /// </summary>
        /// <param name="sourceAttribute">
        /// The source attribute to require a match with.
        /// </param>
        /// <param name="entitySet">
        /// The entity set to query.
        /// </param>
        /// <param name="matchAttribute">
        /// The match attribute in the <paramref name="entitySet"/>.
        /// </param>
        /// <typeparam name="TSet">
        /// The type of entity in which the match is to be found.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="ValueFilterSet{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sourceAttribute"/>, <paramref name="entitySet"/>, or <paramref name="matchAttribute"/> is null.
        /// </exception>
        public ValueFilterSet<T> ExistsIn<TSet>(
            [NotNull] Expression<Func<T, object>> sourceAttribute,
            [NotNull] EntitySet<TSet> entitySet,
            [NotNull] Expression<Func<TSet, object>> matchAttribute)
        {
            if (entitySet == null)
            {
                throw new ArgumentNullException(nameof(entitySet));
            }

            if (sourceAttribute == null)
            {
                throw new ArgumentNullException(nameof(sourceAttribute));
            }

            if (matchAttribute == null)
            {
                throw new ArgumentNullException(nameof(matchAttribute));
            }

            var entityRelation = new EntityRelation(EntityRelationType.InnerJoin);
            entityRelation.Join(sourceAttribute, matchAttribute);

            var entityRelations = new List<IEntityRelation>
                                      {
                                          entityRelation
                                      };

            var setExpression = new RelationExpression(entitySet, entityRelations);
            this.valueFilters.Add(setExpression);
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
        /// The current <see cref="ValueFilterSet{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="valueExpression"/> is null.
        /// </exception>
        private ValueFilterSet<T> AreEqual<TValue>([NotNull] LambdaExpression valueExpression, TValue value)
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
    }
}