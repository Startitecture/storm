// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityRelation.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The base class for defining relations between two entities.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Startitecture.Orm.Query
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using JetBrains.Annotations;

    using Startitecture.Core;

    /// <summary>
    /// The base class for defining relations between two entities.
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

            return sourceProperty?.PropertyType == otherSourceProperty?.PropertyType && sourceProperty?.Name == otherSourceProperty?.Name
                                                                                     && this.SourceEntityAlias == other?.SourceEntityAlias
                                                                                     && relationProperty?.PropertyType
                                                                                     == otherRelationProperty?.PropertyType
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
            if (leftAttribute == null)
            {
                throw new ArgumentNullException(nameof(leftAttribute));
            }

            if (rightAttribute == null)
            {
                throw new ArgumentNullException(nameof(rightAttribute));
            }

            this.SourceExpression = leftAttribute;
            this.RelationExpression = rightAttribute;

            ////var leftReference = this.definitionProvider.GetEntityReference(leftAttribute);

            ////this.SourceAttribute =
            ////    this.definitionProvider.Resolve(leftReference.EntityType)
            ////        .DirectAttributes.FirstOrDefault(x => x.PropertyName == leftAttribute.GetPropertyName());

            ////this.SourceLocation = this.definitionProvider.GetEntityLocation(leftReference);

            ////var rightReference = this.definitionProvider.GetEntityReference(rightAttribute);

            ////this.RelationLocation = this.definitionProvider.GetEntityLocation(rightReference);
            ////this.RelationAttribute =
            ////    this.definitionProvider.Resolve(rightReference.EntityType)
            ////        .DirectAttributes.FirstOrDefault(x => x.PropertyName == rightAttribute.GetPropertyName());
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
            if (sourceAttribute == null)
            {
                throw new ArgumentNullException(nameof(sourceAttribute));
            }

            if (relationAttribute == null)
            {
                throw new ArgumentNullException(nameof(relationAttribute));
            }

            this.SourceExpression = sourceAttribute;
            this.SourceEntityAlias = sourceAlias;
            this.RelationExpression = relationAttribute;
            this.RelationEntityAlias = relationAlias;

            ////var sourceReference = new EntityReference { EntityType = typeof(TSource), EntityAlias = sourceAlias };
            ////this.SourceLocation = this.definitionProvider.GetEntityLocation(sourceReference);

            ////var relationReference = new EntityReference { EntityType = typeof(TRelation), EntityAlias = relationAlias };
            ////this.RelationLocation = this.definitionProvider.GetEntityLocation(relationReference);

            ////var sourceDefinition = this.definitionProvider.Resolve<TSource>();
            ////var relationDefinition = this.definitionProvider.Resolve<TRelation>();
            ////this.SourceAttribute = sourceDefinition.DirectAttributes.FirstOrDefault(x => x.PropertyName == sourceAttribute.GetPropertyName());
            ////this.RelationAttribute = relationDefinition.DirectAttributes.FirstOrDefault(x => x.PropertyName == relationAttribute.GetPropertyName());
        }
    }
}