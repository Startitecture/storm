// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityRelation.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Defines a relation between two entities.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using JetBrains.Annotations;

    using Startitecture.Core;

    /// <summary>
    /// Defines a relation between two entities.
    /// </summary>
    public class EntityRelation : IEntityRelation, IEquatable<EntityRelation>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityRelation"/> class.
        /// </summary>
        /// <param name="relationType">
        /// The relation type.
        /// </param>
        public EntityRelation(EntityRelationType relationType)
        {
            this.RelationType = relationType;
        }

        /// <inheritdoc />
        public EntityRelationType RelationType { get; }

        /// <inheritdoc />
        public LambdaExpression SourceExpression { get; private set; }

        /// <inheritdoc />
        public string SourceEntityAlias { get; private set; }

        /// <inheritdoc />
        public LambdaExpression RelationExpression { get; private set; }

        /// <inheritdoc />
        public string RelationEntityAlias { get; private set; }

        #region Equality Methods

        /// <summary>
        /// Determines if two values of the same type are equal.
        /// </summary>
        /// <param name="valueA">
        /// The first value to compare.
        /// </param>
        /// <param name="valueB">
        /// The second value to compare.
        /// </param>
        /// <returns>
        /// <c>true</c> if the values are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(EntityRelation valueA, EntityRelation valueB)
        {
            return EqualityComparer<EntityRelation>.Default.Equals(valueA, valueB);
        }

        /// <summary>
        /// Determines if two values of the same type are not equal.
        /// </summary>
        /// <param name="valueA">
        /// The first value to compare.
        /// </param>
        /// <param name="valueB">
        /// The second value to compare.
        /// </param>
        /// <returns>
        /// <c>true</c> if the values are not equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(EntityRelation valueA, EntityRelation valueB)
        {
            return !(valueA == valueB);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return $"([{this.SourceExpression}] = [{this.RelationExpression}])";
        }

        /// <summary>
        /// Serves as the default hash function. 
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            var sourceProperty = this.SourceExpression?.GetProperty();
            var relationProperty = this.RelationExpression?.GetProperty();

            return Evaluate.GenerateHashCode(
                sourceProperty?.PropertyType,
                sourceProperty?.Name,
                this.SourceEntityAlias,
                relationProperty?.PropertyType,
                relationProperty?.Name,
                this.RelationEntityAlias);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            return Evaluate.Equals(this, obj);
        }

        /// <inheritdoc />
        public bool Equals(EntityRelation other)
        {
            var sourceProperty = this.SourceExpression?.GetProperty();
            var relationProperty = this.RelationExpression?.GetProperty();

            var otherSourceProperty = other?.SourceExpression?.GetProperty();
            var otherRelationProperty = other?.RelationExpression.GetProperty();

            return sourceProperty?.PropertyType == otherSourceProperty?.PropertyType 
                   && sourceProperty?.Name == otherSourceProperty?.Name
                   && this.SourceEntityAlias == other?.SourceEntityAlias
                   && relationProperty?.PropertyType == otherRelationProperty?.PropertyType
                   && relationProperty?.Name == otherRelationProperty?.Name
                   && this.RelationEntityAlias == other?.RelationEntityAlias;
        }

        #endregion

        /// <summary>
        /// Applies the join attributes using the specified items.
        /// </summary>
        /// <param name="leftAttribute">
        /// The left attribute.
        /// </param>
        /// <param name="rightAttribute">
        /// The right attribute.
        /// </param>
        /// <typeparam name="TSource">
        /// The type of item on the left side of the join.
        /// </typeparam>
        public void Join<TSource>(
            [NotNull] Expression<Func<TSource, object>> leftAttribute,
            [NotNull] Expression<Func<TSource, object>> rightAttribute)
        {
            this.SourceExpression = leftAttribute ?? throw new ArgumentNullException(nameof(leftAttribute));
            this.RelationExpression = rightAttribute ?? throw new ArgumentNullException(nameof(rightAttribute));
        }

        /// <summary>
        /// Applies the join attributes using the specified items.
        /// </summary>
        /// <param name="sourceAttribute">
        /// The source attribute.
        /// </param>
        /// <param name="relationAttribute">
        /// The relation attribute.
        /// </param>
        /// <typeparam name="TSource">
        /// The type of item on the left side of the join.
        /// </typeparam>
        /// <typeparam name="TRelation">
        /// The type of item on the right side of the join.
        /// </typeparam>
        public void Join<TSource, TRelation>(Expression<Func<TSource, object>> sourceAttribute, Expression<Func<TRelation, object>> relationAttribute)
        {
            this.Join(sourceAttribute, relationAttribute, null, null);
        }

        /// <summary>
        /// Applies the join attributes using the specified items.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of item on the left side of the join.
        /// </typeparam>
        /// <typeparam name="TRelation">
        /// The type of item on the right side of the join.
        /// </typeparam>
        /// <param name="sourceAttribute">
        /// The source attribute.
        /// </param>
        /// <param name="relationAttribute">
        /// The relation attribute.
        /// </param>
        /// <param name="sourceAlias">
        /// The source alias.
        /// </param>
        /// <param name="relationAlias">
        /// The relation alias.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sourceAttribute"/> or <paramref name="relationAttribute"/> is null.
        /// </exception>
        public void Join<TSource, TRelation>(
            [NotNull] Expression<Func<TSource, object>> sourceAttribute,
            [NotNull] Expression<Func<TRelation, object>> relationAttribute,
            string sourceAlias,
            string relationAlias)
        {
            this.SourceExpression = sourceAttribute ?? throw new ArgumentNullException(nameof(sourceAttribute));
            this.SourceEntityAlias = sourceAlias;
            this.RelationExpression = relationAttribute ?? throw new ArgumentNullException(nameof(relationAttribute));
            this.RelationEntityAlias = relationAlias;
        }
    }
}