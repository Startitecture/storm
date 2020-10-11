// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityExpression.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Expresses a named entity selection that is related to another entity set or selection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using Startitecture.Resources;

    /// <summary>
    /// Expresses a named entity selection that is related to another entity set or selection.
    /// </summary>
    /// <typeparam name="T">
    /// The type of entity that the expression is based on.
    /// </typeparam>
    public class EntityExpression<T> : IEntityExpression
    {
        /// <summary>
        /// The table relations.
        /// </summary>
        private readonly List<IEntityRelation> relations = new List<IEntityRelation>();

        /// <summary>
        /// Gets the expression name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the entity selection in the expression.
        /// </summary>
        public ISelection Expression { get; private set; }

        /// <summary>
        /// Gets the table relations between the expression and the other set or selection.
        /// </summary>
        public IEnumerable<IEntityRelation> Relations => this.relations;

        /// <summary>
        /// Defines the entity set and name of the expression.
        /// </summary>
        /// <param name="expression">
        /// A selection expression that defines the result set to create.
        /// </param>
        /// <param name="name">
        /// The name to use to alias the expression.
        /// </param>
        /// <returns>
        /// The current <see cref="EntityExpression{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="expression"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="name"/> is null or whitespace.
        /// </exception>
        public EntityExpression<T> As([NotNull] ISelection expression, string name)
        {
            this.Expression = expression ?? throw new ArgumentNullException(nameof(expression));

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(name));
            }

            this.Name = name;
            return this;
        }

        /// <summary>
        /// Defines the entity set and name of the expression.
        /// </summary>
        /// <param name="defineQuery">
        /// A selection expression that defines the result set to create.
        /// </param>
        /// <param name="name">
        /// The name to use to alias the expression.
        /// </param>
        /// <returns>
        /// The current <see cref="EntityExpression{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="defineQuery"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="name"/> is null or whitespace.
        /// </exception>
        public EntityExpression<T> As(Action<EntitySelection<T>> defineQuery, string name)
        {
            if (defineQuery == null)
            {
                throw new ArgumentNullException(nameof(defineQuery));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(name));
            }

            var expression = new EntitySelection<T>();
            defineQuery.Invoke(expression);
            this.Expression = expression;
            this.Name = name;
            return this;
        }

        /// <summary>
        /// Creates a selection based on the current entity expression.
        /// </summary>
        /// <param name="matchAttributes">
        /// The attributes that match between the entity expression and the target selection.
        /// </param>
        /// <typeparam name="TSelection">
        /// The type of entity to be returned by the target selection.
        /// </typeparam>
        /// <returns>
        /// A new <see cref="EntitySelection{T}"/> for entities of type <typeparamref name="TSelection"/>, with the current entity expression as the
        /// <see cref="EntitySelection{T}.ParentExpression"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="matchAttributes"/> is null.
        /// </exception>
        public EntitySelection<TSelection> ForSelection<TSelection>([NotNull] Action<AttributeMatchSet<T, TSelection>> matchAttributes)
        {
            if (matchAttributes == null)
            {
                throw new ArgumentNullException(nameof(matchAttributes));
            }

            var attributeMatchSet = new AttributeMatchSet<T, TSelection>();
            matchAttributes.Invoke(attributeMatchSet);

            foreach (var attributeMatch in attributeMatchSet.Matches)
            {
                var entityRelation = new EntityRelation(EntityRelationType.InnerJoin);
                entityRelation.Join(attributeMatch.SourceExpression, attributeMatch.RelationExpression);
                this.relations.Add(entityRelation);
            }

            var selection = new EntitySelection<TSelection>();
            selection.WithAs(this);
            return selection;
        }
    }
}