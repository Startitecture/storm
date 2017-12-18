// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemSelection.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Creates selection criteria for repository items.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using JetBrains.Annotations;

    using SAF.Core;
    using SAF.StringResources;

    /// <summary>
    /// Creates selection criteria for repository items.
    /// </summary>
    /// <typeparam name="TItem">
    /// The type of item to select.
    /// </typeparam>
    public abstract class ItemSelection<TItem>
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
        protected ItemSelection(IEntityDefinitionProvider definitionProvider)
        {
            if (definitionProvider == null)
            {
                throw new ArgumentNullException(nameof(definitionProvider));
            }

            this.definitionProvider = definitionProvider;
            this.ItemDefinition = definitionProvider.Resolve<TItem>();
            this.SetPropertiesToReturn(this.ItemDefinition.ReturnableAttributes.Distinct(this.distinctAttributeEqualityComparer).ToArray());
        }

        /// <summary>
        /// Gets or sets the maximum number of items to return with the selection.
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// Gets a statement that selects items from the repository based on the current selection.
        /// </summary>
        public abstract string SelectionStatement { get; }

        /// <summary>
        /// Gets a statement that determines whether the repository contains the current selection.
        /// </summary>
        public abstract string ContainsStatement { get; }

        /// <summary>
        /// Gets a statement that removes items from the repository based on the current selection.
        /// </summary>
        public abstract string RemovalStatement { get; }

        /// <summary>
        /// Gets the entity relations represented in the selection.
        /// </summary>
        public IEnumerable<IEntityRelation> Relations
        {
            get
            {
                return this.relations;
            }
        }

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
        public abstract string SelectionSource { get; }

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
        public IEnumerable<EntityAttributeDefinition> PropertiesToReturn
        {
            get
            {
                return this.propertiesToReturn;
            }
        }

        /// <summary>
        /// Gets the filters for the current selection.
        /// </summary>
        public IEnumerable<ValueFilter> Filters
        {
            get
            {
                return this.valueFilters;
            }
        }

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
        /// Adds a match filter for the specified example item.
        /// </summary>
        /// <param name="example">
        /// The example to match.
        /// </param>
        /// <param name="selectors">
        /// The selectors of the properties to match.
        /// </param>
        /// <returns>
        /// The current <see cref="T:SAF.Data.ItemSelection`1"/>.
        /// </returns>
        public ItemSelection<TItem> Matching(
            TItem example,
            params Expression<Func<TItem, object>>[] selectors)
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
                this.AddEqualityFilter(this.FindAttribute(selector), selector.Compile().Invoke(example));
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
        /// The current <see cref="T:SAF.Data.ItemSelection`1"/>.
        /// </returns>
        public ItemSelection<TItem> Matching<TDataItem>(
            TDataItem example,
            params Expression<Func<TDataItem, object>>[] selectors)
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
                this.AddEqualityFilter(
                    this.definitionProvider.Resolve<TDataItem>().Find(selector.GetPropertyName()),
                    selector.Compile().Invoke(example));
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
        /// <param name="nameSelection">
        /// The selection of the property names to include in the filter.
        /// </param>
        /// <returns>
        /// The current <see cref="T:SAF.Data.ItemSelection`1"/>.
        /// </returns>
        public ItemSelection<TItem> Matching<TDataItem>(TDataItem example, IPropertyNameSelection nameSelection)
        {
            if (Evaluate.IsNull(example))
            {
                throw new ArgumentNullException(nameof(example));
            }

            if (nameSelection == null)
            {
                throw new ArgumentNullException(nameof(nameSelection));
            }

            foreach (var selector in nameSelection.PropertiesToInclude)
            {
                this.AddEqualityFilter(
                    this.definitionProvider.Resolve<TDataItem>().Find(selector),
                    ExtensionMethods.GetPropertyValue(example, selector));
            }

            return this;
        }

        /// <summary>
        /// Adds a match filter for the specified property.
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
        /// The current <see cref="T:SAF.Data.ItemSelection`1"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="valueExpression"/> is null.
        /// </exception>
        public ItemSelection<TItem> Matching<TValue>([NotNull] Expression<Func<TItem, TValue>> valueExpression, TValue value)
        {
            if (valueExpression == null)
            {
                throw new ArgumentNullException(nameof(valueExpression));
            }

            this.AddEqualityFilter(this.FindAttribute(valueExpression), value);
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
        /// The current <see cref="T:SAF.Data.ItemSelection`1"/>.
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
            var valueFilter = new ValueFilter(attributeDefinition, true, inclusionValues.Cast<object>().ToArray());
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
        /// The current <see cref="T:SAF.Data.ItemSelection`1"/>.
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
            var valueFilter = new ValueFilter(attributeDefinition, true, inclusionValues.Cast<object>().ToArray());
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
        /// The current <see cref="T:SAF.Data.ItemSelection`1"/>.
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
        /// The current <see cref="T:SAF.Data.ItemSelection`1"/>.
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
        /// <param name="nameSelection">
        /// The selection of the property names to include in the filter.
        /// </param>
        /// <returns>
        /// The current <see cref="T:SAF.Data.ItemSelection`1"/>.
        /// </returns>
        public ItemSelection<TItem> Between<TDataItem>(
            TDataItem baseline,
            TDataItem boundary,
            IPropertyNameSelection nameSelection)
        {
            if (Evaluate.IsNull(baseline))
            {
                throw new ArgumentNullException(nameof(baseline));
            }

            if (Evaluate.IsNull(boundary))
            {
                throw new ArgumentNullException(nameof(boundary));
            }

            if (nameSelection == null)
            {
                throw new ArgumentNullException(nameof(nameSelection));
            }

            if (nameSelection.PropertiesToInclude == null || nameSelection.PropertiesToInclude.Any() == false)
            {
                throw new ArgumentException(ValidationMessages.SpecifyAtLeastOneParameter, nameof(nameSelection));
            }

            foreach (var propertyName in nameSelection.PropertiesToInclude)
            {
                this.AddRangeFilter(
                    this.definitionProvider.Resolve<TDataItem>().Find(propertyName),
                    ExtensionMethods.GetPropertyValue(baseline, propertyName),
                    ExtensionMethods.GetPropertyValue(boundary, propertyName));
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
        /// The current <see cref="T:SAF.Data.ItemSelection`1"/>.
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
        /// The current <see cref="T:SAF.Data.ItemSelection`1"/>.
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
        /// The current <see cref="T:SAF.Data.ItemSelection`1"/>.
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
        /// The <see cref="EntityAttributeDefinition"/> associated with the selector, or <see cref="EntityAttributeDefinition.Empty"/>
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
        /// Adds an equality filter.
        /// </summary>
        /// <param name="attribute">
        /// The property name.
        /// </param>
        /// <param name="value">
        /// The value. If null, the equality filter will create a clause that includes any non-null value of that property.
        /// </param>
        private void AddEqualityFilter(EntityAttributeDefinition attribute, object value)
        {
            this.valueFilters.Add(value == null ? new ValueFilter(attribute) : new ValueFilter(attribute, value));
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
                this.AddEqualityFilter(attribute, leftValue);
            }
            else
            {
                // Needed when caller can't or won't assign the values such that the lower bound property value is less than the upper 
                // bound property value.
                bool valuesFlipped = leftValue is IComparable && rightValue is IComparable
                                     && (leftValue as IComparable).CompareTo(rightValue as IComparable) > 0;

                if (valuesFlipped)
                {
                    var tempValue = leftValue;
                    leftValue = rightValue;
                    rightValue = tempValue;
                }

                this.valueFilters.Add(new ValueFilter(attribute, leftValue, rightValue));
            }
        }
    }
}
