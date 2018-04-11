// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemSelection.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Creates selection criteria for repository items.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Query
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using JetBrains.Annotations;

    using Model;

    using Startitecture.Core;
    using Startitecture.Resources;

    /// <summary>
    /// Creates selection criteria for repository items.
    /// </summary>
    /// <typeparam name="TItem">
    /// The type of item to select.
    /// </typeparam>
    public class ItemSelection<TItem>
    {
        /// <summary>
        /// The value separator for the ToString() method.
        /// </summary>
        private const string ValueSeparator = "&";

        /// <summary>
        /// The definition provider.
        /// </summary>
        private readonly IEntityDefinitionProvider definitionProvider;

        /// <summary>
        /// The relations.
        /// </summary>
        private readonly List<IEntityRelation> relations = new List<IEntityRelation>();

        /// <summary>
        /// The value filters.
        /// </summary>
        private readonly List<ValueFilter> valueFilters = new List<ValueFilter>();

        /// <summary>
        /// The properties to return.
        /// </summary>
        private readonly List<EntityAttributeDefinition> propertiesToReturn = new List<EntityAttributeDefinition>();

        /// <summary>
        /// The distinct attribute equality comparer.
        /// </summary>
        private readonly DistinctAttributeEqualityComparer distinctAttributeEqualityComparer = Singleton<DistinctAttributeEqualityComparer>.Instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemSelection{TItem}"/> class.
        /// </summary>
        /// <param name="definitionProvider">
        /// The definition Provider.
        /// </param>
        public ItemSelection(IEntityDefinitionProvider definitionProvider)
        {
            if (definitionProvider == null)
            {
                throw new ArgumentNullException(nameof(definitionProvider));
            }

            this.definitionProvider = definitionProvider;
            this.ItemDefinition = definitionProvider.Resolve<TItem>();
            this.SelectionSource = this.ItemDefinition.EntityName;
            this.SetPropertiesToReturn(this.ItemDefinition.ReturnableAttributes.Distinct(this.distinctAttributeEqualityComparer).ToArray());
        }

        /// <summary>
        /// Gets or sets the maximum number of items to return with the selection.
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// Gets the entity relations represented in the selection.
        /// </summary>
        public IEnumerable<IEntityRelation> Relations => this.relations;

        /// <summary>
        /// Gets the child selection, if any, for the current selection.
        /// </summary>
        public LinkedSelection<TItem> LinkedSelection { get; private set; }

        /// <summary>
        /// Gets the item definition for the current selection.
        /// </summary>
        public IEntityDefinition ItemDefinition { get; }

        /// <summary>
        /// Gets the source of the selection.
        /// </summary>
        public virtual string SelectionSource { get; }

        /// <summary>
        /// Gets the property values for the filter.
        /// </summary>
        public IEnumerable<object> PropertyValues
        {
            get
            {
                var selection = this;

                while (selection != null)
                {
                    foreach (var value in selection.Filters.SelectMany(ValueFilter.SelectNonNullValues))
                    {
                        yield return value;
                    }

                    selection = selection.LinkedSelection?.Selection;
                }
            }
        }

        /// <summary>
        /// Gets the properties to return.
        /// </summary>
        public IEnumerable<EntityAttributeDefinition> PropertiesToReturn => this.propertiesToReturn;

        /// <summary>
        /// Gets the filters for the current selection.
        /// </summary>
        public IEnumerable<ValueFilter> Filters => this.valueFilters;

        #region Selection

        /// <summary>
        /// Selects the properties to return with the query.
        /// </summary>
        /// <param name="selectors">
        /// The property name selectors. If empty, all properties are returned.
        /// </param>
        /// <returns>
        /// The current <see cref="T:SAF.Data.ItemSelection"/>.
        /// </returns>
        public ItemSelection<TItem> Select(params Expression<Func<TItem, object>>[] selectors)
        {
            if (selectors == null)
            {
                throw new ArgumentNullException(nameof(selectors));
            }

            this.SetPropertiesToReturn(
                selectors.Any()
                    ? selectors.Select(this.FindAttribute).Distinct(this.distinctAttributeEqualityComparer).ToArray()
                    : this.ItemDefinition.ReturnableAttributes.ToArray());

            return this;
        }

        #endregion

        #region Entity Relations

        /// <summary>
        /// Clears any existing relations from the selection.
        /// </summary>
        /// <returns>
        /// The current <see cref="ItemSelection{TItem}"/>.
        /// </returns>
        public ItemSelection<TItem> ClearRelations()
        {
            this.relations.Clear();
            return this;
        }

        #endregion

        #region Predicates

        /// <summary>
        /// Adds a <see cref="ValueFilter"/> to the selection.
        /// </summary>
        /// <param name="valueFilter">
        /// The value filter to add.
        /// </param>
        /// <returns>
        /// The current <see cref="T:Startitecture.Orm.Query.ItemSelection`1"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="valueFilter"/> is null.
        /// </exception>
        public ItemSelection<TItem> AddFilter([NotNull] ValueFilter valueFilter)
        {
            if (valueFilter == null)
            {
                throw new ArgumentNullException(nameof(valueFilter));
            }

            this.valueFilters.Add(valueFilter);
            return this;
        }

        /// <summary>
        /// Adds a match filter for the specified example item.
        /// </summary>
        /// <param name="example">
        /// The example to match.
        /// </param>
        /// <param name="selectors">
        /// The selectors of the properties to match.
        /// </param>
        /// <returns>
        /// The current <see cref="T:Startitecture.Orm.Query.ItemSelection`1"/>.
        /// </returns>
        public ItemSelection<TItem> Matching(TItem example, params Expression<Func<TItem, object>>[] selectors)
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
                var attributeDefinition = this.FindAttribute(selector);
                var value = selector.Compile().Invoke(example);
                this.valueFilters.Add(new ValueFilter(attributeDefinition, FilterType.Equality, value));
            }

            return this;
        }

        /// <summary>
        /// Adds a match filter for the specified example item.
        /// </summary>
        /// <typeparam name="TDataItem">
        /// The type of data item to match.
        /// </typeparam>
        /// <param name="example">
        /// The example to match.
        /// </param>
        /// <param name="selectors">
        /// The selectors of the properties to match.
        /// </param>
        /// <returns>
        /// The current <see cref="T:Startitecture.Orm.Query.ItemSelection`1"/>.
        /// </returns>
        public ItemSelection<TItem> Matching<TDataItem>(TDataItem example, params Expression<Func<TDataItem, object>>[] selectors)
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
                var attributeDefinition = this.definitionProvider.Resolve<TDataItem>().Find(selector.GetPropertyName());
                var value = selector.Compile().Invoke(example);
                this.valueFilters.Add(new ValueFilter(attributeDefinition, FilterType.Equality, value));
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
        /// The current <see cref="T:Startitecture.Orm.Query.ItemSelection`1"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="valueExpression"/> is null.
        /// </exception>
        public ItemSelection<TItem> WhereEqual<TValue>([NotNull] Expression<Func<TItem, TValue>> valueExpression, TValue value)
        {
            if (valueExpression == null)
            {
                throw new ArgumentNullException(nameof(valueExpression));
            }

            var valueFilter = Evaluate.IsNull(value)
                                  ? new ValueFilter(this.FindAttribute(valueExpression), FilterType.IsNotSet, value)
                                  : new ValueFilter(this.FindAttribute(valueExpression), FilterType.Equality, value);

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
        /// The current <see cref="T:Startitecture.Orm.Query.ItemSelection`1"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="valueExpression"/> is null.
        /// </exception>
        public ItemSelection<TItem> GreaterThan<TValue>([NotNull] Expression<Func<TItem, TValue>> valueExpression, TValue value)
        {
            if (valueExpression == null)
            {
                throw new ArgumentNullException(nameof(valueExpression));
            }

            this.valueFilters.Add(new ValueFilter(this.FindAttribute(valueExpression), FilterType.GreaterThan, value));
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
        /// The current <see cref="T:Startitecture.Orm.Query.ItemSelection`1"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="valueExpression"/> is null.
        /// </exception>
        public ItemSelection<TItem> GreaterThanOrEqualTo<TValue>([NotNull] Expression<Func<TItem, TValue>> valueExpression, TValue value)
        {
            if (valueExpression == null)
            {
                throw new ArgumentNullException(nameof(valueExpression));
            }

            this.valueFilters.Add(new ValueFilter(this.FindAttribute(valueExpression), FilterType.GreaterThanOrEqualTo, value));
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
        /// The current <see cref="T:Startitecture.Orm.Query.ItemSelection`1"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="valueExpression"/> is null.
        /// </exception>
        public ItemSelection<TItem> LessThan<TValue>([NotNull] Expression<Func<TItem, TValue>> valueExpression, TValue value)
        {
            if (valueExpression == null)
            {
                throw new ArgumentNullException(nameof(valueExpression));
            }

            this.valueFilters.Add(new ValueFilter(this.FindAttribute(valueExpression), FilterType.LessThan, value));
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
        /// The current <see cref="T:Startitecture.Orm.Query.ItemSelection`1"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="valueExpression"/> is null.
        /// </exception>
        public ItemSelection<TItem> LessThanOrEqualTo<TValue>([NotNull] Expression<Func<TItem, TValue>> valueExpression, TValue value)
        {
            if (valueExpression == null)
            {
                throw new ArgumentNullException(nameof(valueExpression));
            }

            this.valueFilters.Add(new ValueFilter(this.FindAttribute(valueExpression), FilterType.LessThanOrEqualTo, value));
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
        /// The current <see cref="T:Startitecture.Orm.Query.ItemSelection`1"/>.
        /// </returns>
        public ItemSelection<TItem> Include<TValue>(
            Expression<Func<TItem, TValue>> selector,
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

            var attributeDefinition = this.FindAttribute(selector);
            var valueFilter = new ValueFilter(attributeDefinition, FilterType.MatchesSet, inclusionValues.Cast<object>().ToArray());
            this.valueFilters.Add(valueFilter);
            return this;
        }

        /// <summary>
        /// Adds an inclusion filter for the specified example item.
        /// </summary>
        /// <typeparam name="TDataItem">
        /// The type of data item to include.
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
        /// The current <see cref="T:Startitecture.Orm.Query.ItemSelection`1"/>.
        /// </returns>
        public ItemSelection<TItem> IncludeRelated<TDataItem, TValue>(
            Expression<Func<TDataItem, TValue>> selector,
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

            var attributeDefinition = this.definitionProvider.Resolve<TDataItem>().Find(selector.GetPropertyName());
            var valueFilter = new ValueFilter(attributeDefinition, FilterType.MatchesSet, inclusionValues.Cast<object>().ToArray());
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
        /// The selectors of the properties to match.
        /// </param>
        /// <returns>
        /// The current <see cref="T:Startitecture.Orm.Query.ItemSelection`1"/>.
        /// </returns>
        public ItemSelection<TItem> Between(
            TItem baseline,
            TItem boundary,
            params Expression<Func<TItem, object>>[] selectors)
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
                this.AddRangeFilter(this.FindAttribute(selector), compiledSelector.Invoke(baseline), compiledSelector.Invoke(boundary));
            }

            return this;
        }

        /// <summary>
        /// Adds a between filter for the specified example item.
        /// </summary>
        /// <typeparam name="TDataItem">
        /// The type of data item to evaluate.
        /// </typeparam>
        /// <param name="baseline">
        /// The baseline item.
        /// </param>
        /// <param name="boundary">
        /// The boundary item.
        /// </param>
        /// <param name="selectors">
        /// The selectors of the properties to match.
        /// </param>
        /// <returns>
        /// The current <see cref="T:Startitecture.Orm.Query.ItemSelection`1"/>.
        /// </returns>
        public ItemSelection<TItem> Between<TDataItem>(
            TDataItem baseline,
            TDataItem boundary,
            params Expression<Func<TDataItem, object>>[] selectors)
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
                this.AddRangeFilter(
                    this.definitionProvider.Resolve<TDataItem>().Find(selector.GetPropertyName()),
                    compiledSelector.Invoke(baseline),
                    compiledSelector.Invoke(boundary));
            }

            return this;
        }

        #endregion

        #region JOINs

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
        /// The current <see cref="T:SAF.Data.Providers.ItemSelection`1"/>.
        /// </returns>
        public ItemSelection<TItem> InnerJoin(
            [NotNull] Expression<Func<TItem, object>> leftSelector,
            [NotNull] Expression<Func<TItem, object>> rightSelector)
        {
            if (leftSelector == null)
            {
                throw new ArgumentNullException(nameof(leftSelector));
            }

            if (rightSelector == null)
            {
                throw new ArgumentNullException(nameof(rightSelector));
            }

            var relation = new EntityRelation(this.definitionProvider, EntityRelationType.InnerJoin);
            relation.Join<TItem>(leftSelector, rightSelector);
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
        /// The current <see cref="T:SAF.Data.Providers.ItemSelection`1"/>.
        /// </returns>
        public ItemSelection<TItem> InnerJoin<TRelation>(
            Expression<Func<TItem, object>> leftSelector,
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

            var entityRelation = new EntityRelation(this.definitionProvider, EntityRelationType.InnerJoin);
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
        /// The current <see cref="T:SAF.Data.Providers.ItemSelection`1"/>.
        /// </returns>
        public ItemSelection<TItem> InnerJoin<TRelation>(
            Expression<Func<TItem, object>> leftSelector,
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

            var entityRelation = new EntityRelation(this.definitionProvider, EntityRelationType.InnerJoin);
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
        /// The current <see cref="T:SAF.Data.Providers.ItemSelection`1"/>.
        /// </returns>
        public ItemSelection<TItem> InnerJoin<TSource, TRelation>(
            Expression<Func<TSource, object>> leftSelector,
            Expression<Func<TRelation, object>> rightSelector)
        {
            var entityRelation = new EntityRelation(this.definitionProvider, EntityRelationType.InnerJoin);
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
        /// The current <see cref="T:SAF.Data.Providers.ItemSelection`1"/>.
        /// </returns>
        public ItemSelection<TItem> InnerJoin<TSource, TRelation>(
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
        /// The current <see cref="T:SAF.Data.Providers.ItemSelection`1"/>.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Allows fluent usage of the method.")]
        public ItemSelection<TItem> InnerJoin<TSource, TRelation>(
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

            var entityRelation = new EntityRelation(this.definitionProvider, EntityRelationType.InnerJoin);
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
        /// The current <see cref="T:SAF.Data.Providers.ItemSelection`1"/>.
        /// </returns>
        public ItemSelection<TItem> LeftJoin(
            [NotNull] Expression<Func<TItem, object>> leftSelector,
            [NotNull] Expression<Func<TItem, object>> rightSelector)
        {
            if (leftSelector == null)
            {
                throw new ArgumentNullException(nameof(leftSelector));
            }

            if (rightSelector == null)
            {
                throw new ArgumentNullException(nameof(rightSelector));
            }

            var relation = new EntityRelation(this.definitionProvider, EntityRelationType.LeftJoin);
            relation.Join<TItem>(leftSelector, rightSelector);
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
        /// The current <see cref="T:SAF.Data.Providers.ItemSelection`1"/>.
        /// </returns>
        public ItemSelection<TItem> LeftJoin<TRelation>(
            Expression<Func<TItem, object>> leftSelector,
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

            var entityRelation = new EntityRelation(this.definitionProvider, EntityRelationType.LeftJoin);
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
        /// The current <see cref="T:SAF.Data.Providers.ItemSelection`1"/>.
        /// </returns>
        public ItemSelection<TItem> LeftJoin<TRelation>(
            Expression<Func<TItem, object>> leftSelector,
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

            var entityRelation = new EntityRelation(this.definitionProvider, EntityRelationType.LeftJoin);
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
        /// The current <see cref="T:SAF.Data.Providers.ItemSelection`1"/>.
        /// </returns>
        public ItemSelection<TItem> LeftJoin<TSource, TRelation>(
            Expression<Func<TSource, object>> leftSelector,
            Expression<Func<TRelation, object>> rightSelector)
        {
            var entityRelation = new EntityRelation(this.definitionProvider, EntityRelationType.LeftJoin);
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
        /// The current <see cref="T:SAF.Data.Providers.ItemSelection`1"/>.
        /// </returns>
        public ItemSelection<TItem> LeftJoin<TSource, TRelation>(
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
        /// The current <see cref="T:SAF.Data.Providers.ItemSelection`1"/>.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Allows fluent usage of the method.")]
        public ItemSelection<TItem> LeftJoin<TSource, TRelation>(
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

            var entityRelation = new EntityRelation(this.definitionProvider, EntityRelationType.LeftJoin);
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
        /// The current <see cref="T:Startitecture.Orm.Query.ItemSelection`1"/>.
        /// </returns>
        public ItemSelection<TItem> Union(ItemSelection<TItem> selection)
        {
            this.LinkedSelection = new LinkedSelection<TItem>(selection, SelectionLinkType.Union);
            return this;
        }

        /// <summary>
        /// Combines the results of the current selection with the specified selection.
        /// </summary>
        /// <param name="selection">
        /// The selection to combine.
        /// </param>
        /// <returns>
        /// The current <see cref="T:Startitecture.Orm.Query.ItemSelection`1"/>.
        /// </returns>
        public ItemSelection<TItem> Intersect(ItemSelection<TItem> selection)
        {
            this.LinkedSelection = new LinkedSelection<TItem>(selection, SelectionLinkType.Intersection);
            return this;
        }

        /// <summary>
        /// Combines the results of the current selection with the specified selection.
        /// </summary>
        /// <param name="selection">
        /// The selection to combine.
        /// </param>
        /// <returns>
        /// The current <see cref="T:Startitecture.Orm.Query.ItemSelection`1"/>.
        /// </returns>
        public ItemSelection<TItem> Except(ItemSelection<TItem> selection)
        {
            this.LinkedSelection = new LinkedSelection<TItem>(selection, SelectionLinkType.Exception);
            return this;
        }

        #endregion

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Join(ValueSeparator, this.valueFilters.OrderBy(x => x.ItemAttribute.PropertyName));
        }

        /// <summary>
        /// Adds a match filter for the specified example item.
        /// </summary>
        /// <typeparam name="TDataItem">
        /// The type of data item to match.
        /// </typeparam>
        /// <param name="example">
        /// The example to match.
        /// </param>
        /// <param name="propertyNames">
        /// The property names to include in the filter.
        /// </param>
        /// <returns>
        /// The current <see cref="T:Startitecture.Orm.Query.ItemSelection`1"/>.
        /// </returns>
        protected ItemSelection<TItem> Matching<TDataItem>(TDataItem example, IEnumerable<string> propertyNames)
        {
            if (Evaluate.IsNull(example))
            {
                throw new ArgumentNullException(nameof(example));
            }

            if (propertyNames == null)
            {
                throw new ArgumentNullException(nameof(propertyNames));
            }

            foreach (var selector in propertyNames)
            {
                var attributeDefinition = this.definitionProvider.Resolve<TDataItem>().Find(selector);
                var value = example.GetPropertyValue(selector);
                this.valueFilters.Add(new ValueFilter(attributeDefinition, FilterType.Equality, value));
            }

            return this;
        }

        /// <summary>
        /// Adds a relation to the selection.
        /// </summary>
        /// <param name="relation">
        /// The relation to add.
        /// </param>
        /// <returns>
        /// The current <see cref="T:SAF.Data.ItemSelection"/>.
        /// </returns>
        protected ItemSelection<TItem> AddRelation([NotNull] IEntityRelation relation)
        {
            if (relation == null)
            {
                throw new ArgumentNullException(nameof(relation));
            }

            if (this.relations.Contains(relation) == false)
            {
                this.relations.Add(relation);
            }

            return this;
        }

        /// <summary>
        /// Finds the specified attribute, first using a precise search and then a name-only search.
        /// </summary>
        /// <param name="selector">
        /// The selector to find the attribute for.
        /// </param>
        /// <returns>
        /// The <see cref="Startitecture.Orm.Model.EntityAttributeDefinition"/> associated with the selector, or <see cref="Startitecture.Orm.Model.EntityAttributeDefinition.Empty"/>
        /// if the attribute is not found.
        /// </returns>
        private EntityAttributeDefinition FindAttribute(LambdaExpression selector)
        {
            var preciseAttribute = this.ItemDefinition.Find(selector);

            // If we can't locate the attribute precisely, fall back to using only the name.
            return Evaluate.IsDefaultValue(preciseAttribute) ? this.ItemDefinition.Find(selector.GetPropertyName()) : preciseAttribute;
        }

        /// <summary>
        /// Sets properties to return to the current selection.
        /// </summary>
        /// <param name="properties">
        /// The property names.
        /// </param>
        private void SetPropertiesToReturn(params EntityAttributeDefinition[] properties)
        {
            this.propertiesToReturn.Clear();
            this.propertiesToReturn.AddRange(properties);
        }

        /// <summary>
        /// Adds a BETWEEN filter.
        /// </summary>
        /// <param name="attribute">
        /// The property name.
        /// </param>
        /// <param name="leftValue">
        /// The left value.
        /// </param>
        /// <param name="rightValue">
        /// The right value.
        /// </param>
        private void AddRangeFilter(EntityAttributeDefinition attribute, object leftValue, object rightValue)
        {
            if (Evaluate.Equals(leftValue, rightValue))
            {
                this.valueFilters.Add(new ValueFilter(attribute, FilterType.Equality, leftValue));
            }
            else
            {
                // Needed when caller can't or won't assign the values such that the lower bound property value is less than the upper 
                // bound property value.
                bool valuesFlipped = leftValue is IComparable comparable && rightValue is IComparable
                                     && comparable.CompareTo((IComparable)rightValue) > 0;

                if (valuesFlipped)
                {
                    var tempValue = leftValue;
                    leftValue = rightValue;
                    rightValue = tempValue;
                }

                this.valueFilters.Add(new ValueFilter(attribute, FilterType.Between, leftValue, rightValue));
            }
        }
    }
}
