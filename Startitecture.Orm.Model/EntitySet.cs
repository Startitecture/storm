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
    using System.Linq.Expressions;

    using JetBrains.Annotations;

    using Startitecture.Core;

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
        /// Returns a <see cref="string"/> that represents the current <see cref="object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> that represents the current <see cref="object"/>.
        /// </returns>
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

            targetSet.orderByExpressions.AddRange(
                sourceSet.OrderByExpressions.Select(
                    source =>
                    {
                        var parameter = Expression.Parameter(typeof(TDestEntity), "value");
                        var property = Expression.Property(parameter, source.PropertyExpression.GetPropertyName());
                        var expression = Expression.Lambda(property, parameter);

                        return new OrderExpression(expression, source.OrderDescending);
                    }));

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
    }
}