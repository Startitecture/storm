// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Query.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   A static class for creating entity selections.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using System;
    using System.Linq.Expressions;

    using JetBrains.Annotations;

    /// <summary>
    /// A static class for creating entity selections.
    /// </summary>
#pragma warning disable CA1716 // Identifiers should not match keywords
    public static class Query
#pragma warning restore CA1716 // Identifiers should not match keywords
    {
        /// <summary>
        /// Creates an entity selection for the specified entity type.
        /// </summary>
        /// <param name="selections">
        /// The properties to return.
        /// </param>
        /// <typeparam name="T">
        /// The type of entity to query.
        /// </typeparam>
        /// <returns>
        /// A new <see cref="EntitySelection{T}"/> for the specified type.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="selections"/> is null.
        /// </exception>
        public static EntitySelection<T> Select<T>([NotNull] params Expression<Func<T, object>>[] selections)
        {
            if (selections == null)
            {
                throw new ArgumentNullException(nameof(selections));
            }

            return new EntitySelection<T>().Select(selections);
        }

        /// <summary>
        /// Creates an entity set for the specified entity type.
        /// </summary>
        /// <typeparam name="T">
        /// The type of entity to query.
        /// </typeparam>
        /// <returns>
        /// A new <see cref="EntitySet{T}"/>for the specified type.
        /// </returns>
        public static EntitySet<T> From<T>()
        {
            return new EntitySet<T>();
        }

        /// <summary>
        /// Creates an entity selection for the specified entity type.
        /// </summary>
        /// <param name="defineSelection">
        /// Defines the selection to return.
        /// </param>
        /// <typeparam name="T">
        /// The type of entity to query.
        /// </typeparam>
        /// <returns>
        /// A new <see cref="EntitySelection{T}"/> for the specified type.
        /// </returns>
        public static EntitySelection<T> SelectEntities<T>([NotNull] Action<EntitySelection<T>> defineSelection)
        {
            if (defineSelection == null)
            {
                throw new ArgumentNullException(nameof(defineSelection));
            }

            var entitySelection = new EntitySelection<T>();
            defineSelection.Invoke(entitySelection);
            return entitySelection;
        }

        /// <summary>
        /// Creates an entity set for the specified entity type.
        /// </summary>
        /// <param name="setRelations">
        /// Set the relations for the entity set.
        /// </param>
        /// <typeparam name="T">
        /// The type of entity to query.
        /// </typeparam>
        /// <returns>
        /// A new <see cref="EntitySet{T}"/>for the specified type.
        /// </returns>
        public static EntitySet<T> From<T>(Action<EntityRelationSet<T>> setRelations)
        {
            return new EntitySet<T>().From(setRelations);
        }

        /// <summary>
        /// Creates an entity expression to use with a subsequent query.
        /// </summary>
        /// <param name="selection">
        /// The entity selection for the expression.
        /// </param>
        /// <param name="name">
        /// The name of the expression.
        /// </param>
        /// <typeparam name="TExpression">
        /// The type of entity the expression will query.
        /// </typeparam>
        /// <returns>
        /// A new <see cref="EntityExpression{T}"/> for the <typeparamref name="TExpression"/> entity.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="selection"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="name"/> is null or whitespace.
        /// </exception>
        public static EntityExpression<TExpression> With<TExpression>([NotNull] EntitySelection<TExpression> selection, [NotNull] string name)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            }

            var expression = new EntityExpression<TExpression>();
            expression.As(selection, name);
            return expression;
        }

        /// <summary>
        /// Creates an entity expression to use with a subsequent query.
        /// </summary>
        /// <param name="selectAction">
        /// The entity selection action for the expression.
        /// </param>
        /// <param name="name">
        /// The name of the expression.
        /// </param>
        /// <typeparam name="TExpression">
        /// The type of entity the expression will query.
        /// </typeparam>
        /// <returns>
        /// A new <see cref="EntityExpression{T}"/> for the <typeparamref name="TExpression"/> entity.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="selectAction"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="name"/> is null or whitespace.
        /// </exception>
        public static EntityExpression<TExpression> With<TExpression>([NotNull] Action<EntitySelection<TExpression>> selectAction, [NotNull] string name)
        {
            if (selectAction == null)
            {
                throw new ArgumentNullException(nameof(selectAction));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            }

            var expression = new EntityExpression<TExpression>();
            expression.As(selectAction, name);
            return expression;
        }
    }
}