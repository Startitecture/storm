// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntitySet.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
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
    /// Specifies a set of entities in a repository.
    /// </summary>
    /// <typeparam name="T">
    /// The type of entity specified in the set.
    /// </typeparam>
    public class EntitySet<T> : IEntitySet
    {
        /// <summary>
        /// The relations.
        /// </summary>
        private readonly List<IEntityRelation> relations = new List<IEntityRelation>();

        /// <summary>
        /// The value filters.
        /// </summary>
        private readonly List<ValueFilter> valueFilters = new List<ValueFilter>();

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
                foreach (var value in this.Filters.SelectMany(ValueFilter.SelectNonNullValues))
                {
                    yield return value;
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
        public IEnumerable<ValueFilter> Filters => this.valueFilters;

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
        public EntitySet<T> WithAs<TExpression>(
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

        #region Predicates

        /// <summary>
        /// Matches the primary key of the specified <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">
        /// The entity to match the key for.
        /// </param>
        /// <param name="definitionProvider">
        /// The definition provider for the entity.
        /// </param>
        /// <returns>
        /// The current <see cref="EntitySelection{T}"/>.
        /// </returns>
        /// <remarks>
        /// This operation will clear any existing filters.
        /// </remarks>
        public EntitySet<T> MatchKey([NotNull] T entity, [NotNull] IEntityDefinitionProvider definitionProvider)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (definitionProvider == null)
            {
                throw new ArgumentNullException(nameof(definitionProvider));
            }

            this.valueFilters.Clear();
            var entityDefinition = definitionProvider.Resolve<T>();

            foreach (var keyAttribute in entityDefinition.PrimaryKeyAttributes)
            {
                var entityReference = new EntityReference
                                          {
                                              EntityAlias = keyAttribute.Entity.Alias,
                                              EntityType = keyAttribute.Entity.EntityType
                                          };
                var attributeLocation = new AttributeLocation(keyAttribute.PropertyInfo, entityReference);
                var valueFilter = new ValueFilter(attributeLocation, FilterType.Equality, keyAttribute.GetValueDelegate.DynamicInvoke(entity));

                this.AddFilter(valueFilter);
            }

            // Use all available values if no keys are defined.
            if (this.Filters.Any())
            {
                return this;
            }

            Trace.TraceWarning($"{typeof(TimeZoneInfo).FullName} does not have any key attributes defined.");

            foreach (var attribute in entityDefinition.DirectAttributes)
            {
                var entityReference = new EntityReference
                                          {
                                              EntityAlias = attribute.Entity.Alias,
                                              EntityType = attribute.Entity.EntityType
                                          };
                var attributeLocation = new AttributeLocation(attribute.PropertyInfo, entityReference);
                var valueFilter = new ValueFilter(attributeLocation, FilterType.Equality, attribute.GetValueDelegate.DynamicInvoke(entity));
                this.AddFilter(valueFilter);
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
        /// The current <see cref="EntitySelection{T}"/>.
        /// </returns>
        public EntitySet<T> Matching(T example, params Expression<Func<T, object>>[] selectors)
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
        public EntitySet<T> WhereEqual<TValue>([NotNull] Expression<Func<T, TValue>> valueExpression, TValue value)
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
        public EntitySet<T> WhereEqual<TValue>([NotNull] LambdaExpression valueExpression, TValue value)
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
        public EntitySet<T> GreaterThan<TValue>([NotNull] Expression<Func<T, TValue>> valueExpression, TValue value)
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
        public EntitySet<T> GreaterThanOrEqualTo<TValue>([NotNull] Expression<Func<T, TValue>> valueExpression, TValue value)
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
        public EntitySet<T> LessThan<TValue>([NotNull] Expression<Func<T, TValue>> valueExpression, TValue value)
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
        public EntitySet<T> LessThanOrEqualTo<TValue>([NotNull] Expression<Func<T, TValue>> valueExpression, TValue value)
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
        public EntitySet<T> Include<TValue>(Expression<Func<T, TValue>> selector, params TValue[] inclusionValues)
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
        public EntitySet<T> Between<TValue>([NotNull] Expression<Func<T, TValue>> selector, [NotNull] TValue minValue, [NotNull] TValue maxValue)
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
        public EntitySet<T> Between(T baseline, T boundary, params Expression<Func<T, object>>[] selectors)
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

        #region Relations

        /// <summary>
        /// Adds a relation to the selection.
        /// </summary>
        /// <param name="relation">
        /// The relation to add.
        /// </param>
        /// <returns>
        /// The current <see cref="EntitySelection{T}"/>.
        /// </returns>
        public EntitySet<T> AddRelation([NotNull] IEntityRelation relation)
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
        public EntitySet<T> InnerJoin([NotNull] Expression<Func<T, object>> leftSelector, [NotNull] Expression<Func<T, object>> rightSelector)
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
        public EntitySet<T> InnerJoin<TRelation>(Expression<Func<T, object>> leftSelector, Expression<Func<TRelation, object>> rightSelector)
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
        public EntitySet<T> InnerJoin<TRelation>(
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
        /// Appends an INNER JOIN clause to the selection. The table represented by <typeparamref name="TSource"/> must already be
        /// a
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
        public EntitySet<T> InnerJoin<TSource, TRelation>(
            Expression<Func<TSource, object>> leftSelector,
            Expression<Func<TRelation, object>> rightSelector)
        {
            var entityRelation = new EntityRelation(EntityRelationType.InnerJoin);
            entityRelation.Join(leftSelector, rightSelector, null, null);
            return this.AddRelation(entityRelation);
        }

        /// <summary>
        /// Appends an INNER JOIN clause to the selection. The table represented by <typeparamref name="TSource"/> must already be
        /// a
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
        public EntitySet<T> InnerJoin<TSource, TRelation>(
            Expression<Func<TSource, object>> leftSelector,
            Expression<Func<TRelation, object>> rightSelector,
            string relationAlias)
        {
            return this.InnerJoin(leftSelector, null, rightSelector, relationAlias);
        }

        /// <summary>
        /// Appends an INNER JOIN clause to the selection. The table represented by <typeparamref name="TSource"/> must already be
        /// a
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
            "Microsoft.Design",
            "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "Allows fluent usage of the method.")]
        public EntitySet<T> InnerJoin<TSource, TRelation>(
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
        public EntitySet<T> LeftJoin([NotNull] Expression<Func<T, object>> leftSelector, [NotNull] Expression<Func<T, object>> rightSelector)
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
        public EntitySet<T> LeftJoin<TRelation>(Expression<Func<T, object>> leftSelector, Expression<Func<TRelation, object>> rightSelector)
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
        public EntitySet<T> LeftJoin<TRelation>(
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
        public EntitySet<T> LeftJoin<TSource, TRelation>(
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
        public EntitySet<T> LeftJoin<TSource, TRelation>(
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
            "Microsoft.Design",
            "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "Allows fluent usage of the method.")]
        public EntitySet<T> LeftJoin<TSource, TRelation>(
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
        public EntitySet<T> Union(EntitySelection<T> selection)
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
        public EntitySet<T> Intersect(EntitySelection<T> selection)
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
        public EntitySet<T> Except(EntitySelection<T> selection)
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
        /// An <see cref="EntitySelection{T}" /> for the destination type.
        /// </returns>
        public EntitySet<TDestEntity> MapTo<TDestEntity>()
            where TDestEntity : class, new()
        {
            var targetSelection = new EntitySet<TDestEntity>();
            targetSelection.valueFilters.AddRange(this.valueFilters);
            targetSelection.relations.AddRange(this.relations);
            return targetSelection;
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
        private EntitySet<T> AddFilter([NotNull] ValueFilter valueFilter)
        {
            if (valueFilter == null)
            {
                throw new ArgumentNullException(nameof(valueFilter));
            }

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