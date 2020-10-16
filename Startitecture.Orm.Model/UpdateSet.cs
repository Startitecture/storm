// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateSet.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Represents an update set specification for an entity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using JetBrains.Annotations;

    /// <summary>
    /// Represents an update set specification for an entity.
    /// </summary>
    /// <typeparam name="T">
    /// The type of entity to update.
    /// </typeparam>
    public class UpdateSet<T> : IUpdateSet
    {
        /// <summary>
        /// The entity reference.
        /// </summary>
        private static readonly EntityReference EntityReference = new EntityReference
        {
            EntityType = typeof(T)
        };

        /// <summary>
        /// The attributes to set.
        /// </summary>
        private readonly List<ValueState> attributesToSet = new List<ValueState>();

        /// <summary>
        /// The relations.
        /// </summary>
        private readonly List<IEntityRelation> relations = new List<IEntityRelation>();

        /// <summary>
        /// The filters.
        /// </summary>
        private readonly List<IValueFilter> filters = new List<IValueFilter>();

        /// <inheritdoc />
        public IEnumerable<ValueState> AttributesToSet => this.attributesToSet;

        /// <inheritdoc />
        public Type EntityType => typeof(T);

        /// <inheritdoc />
        public IEntityExpression ParentExpression { get; private set; }

        /// <inheritdoc />
        public IEnumerable<IEntityRelation> Relations => this.relations;

        /// <inheritdoc />
        public IEnumerable<object> PropertyValues
        {
            get
            {
                // First do our set expressions.
                foreach (var valueState in this.attributesToSet.Where(state => state.Value != null))
                {
                    yield return valueState.Value;
                }

                // Then filters.
                foreach (var value in this.filters.SelectMany(ValueFilter.SelectNonNullValues))
                {
                    yield return value;
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
        public IEnumerable<IValueFilter> Filters => this.filters;

        /// <summary>
        /// Sets all updateable values for the update set.
        /// </summary>
        /// <param name="entity">
        /// Get
        /// The entity containing the target values.
        /// </param>
        /// <param name="definitionProvider">
        /// The definition provider for the target repository.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="entity"/> or <paramref name="definitionProvider"/> is null.
        /// </exception>
        /// <returns>
        /// The current <see cref="UpdateSet{T}"/> with all updateable attribute values set.
        /// </returns>
        public UpdateSet<T> Set([NotNull] T entity, [NotNull] IEntityDefinitionProvider definitionProvider)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (definitionProvider == null)
            {
                throw new ArgumentNullException(nameof(definitionProvider));
            }

            this.attributesToSet.Clear();
            var entityDefinition = definitionProvider.Resolve<T>();

            foreach (var attributeDefinition in entityDefinition.UpdateableAttributes)
            {
                this.attributesToSet.Add(
                    new ValueState(
                        new AttributeLocation(attributeDefinition.PropertyInfo, EntityReference),
                        attributeDefinition.GetValueDelegate.Method.Invoke(entity, null)));
            }

            return this;
        }

        /// <summary>
        /// Sets the values to update as part of the operation. This clears any existing values.
        /// </summary>
        /// <param name="entity">
        /// The entity containing the target values.
        /// </param>
        /// <param name="attributes">
        /// The attributes to update.
        /// </param>
        /// <returns>
        /// The current <see cref="UpdateSet{T}"/> with the specified update values set.
        /// </returns>
        public UpdateSet<T> Set([NotNull] T entity, [NotNull] params Expression<Func<T, object>>[] attributes)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (attributes == null)
            {
                throw new ArgumentNullException(nameof(attributes));
            }

            if (attributes.Any())
            {
                this.attributesToSet.Clear();
                this.attributesToSet.AddRange(from a in attributes select new ValueState(new AttributeLocation(a), a.Compile().Invoke(entity)));
                return this;
            }

            throw new InvalidOperationException($@"At least one attribute must be specified in the {nameof(attributes)} parameter.");
        }

        /// <summary>
        /// Sets the values to update as part of the operation. This clears any existing values.
        /// </summary>
        /// <typeparam name="TValue">
        /// The type of value to set the attribute to.
        /// </typeparam>
        /// <param name="attribute">
        /// The attribute to set.
        /// </param>
        /// <param name="value">
        /// The value to set the attribute to.
        /// </param>
        /// <returns>
        /// The current <see cref="UpdateSet{T}"/> with the specified attribute update value set.
        /// </returns>
        public UpdateSet<T> Set<TValue>([NotNull] Expression<Func<T, TValue>> attribute, TValue value)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException(nameof(attribute));
            }

            var attributeLocation = new AttributeLocation(attribute);
            this.attributesToSet.Add(new ValueState(attributeLocation, value));
            return this;
        }

        /// <summary>
        /// Defines the relations to include in the update statement.
        /// </summary>
        /// <param name="setRelations">
        /// Sets the relations for the update set.
        /// </param>
        /// <returns>
        /// The current <see cref="UpdateSet{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="setRelations"/> is empty.
        /// </exception>
        public UpdateSet<T> From([NotNull] Action<EntityRelationSet<T>> setRelations)
        {
            if (setRelations == null)
            {
                throw new ArgumentNullException(nameof(setRelations));
            }

            var relationSet = new EntityRelationSet<T>();
            setRelations.Invoke(relationSet);
            this.relations.Clear();
            this.relations.AddRange(relationSet.Relations);
            return this;
        }

        /// <summary>
        /// Defines the entity set to update.
        /// </summary>
        /// <param name="set">
        /// The entity set to update.
        /// </param>
        /// <returns>
        /// The current <see cref="UpdateSet{T}"/>.
        /// </returns>
        public UpdateSet<T> Where([NotNull] Action<ValueFilterSet<T>> set)
        {
            if (set == null)
            {
                throw new ArgumentNullException(nameof(set));
            }

            var target = new ValueFilterSet<T>();
            set.Invoke(target);
            this.filters.Clear();
            this.filters.AddRange(target.ValueFilters);
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
    }
}