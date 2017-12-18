// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityRelation.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The base class for defining relations between two entities.
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

    /// <summary>
    /// The base class for defining relations between two entities.
    /// </summary>
    public class EntityRelation : IEntityRelation, IEquatable<EntityRelation>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<EntityRelation, object>[] ComparisonProperties =
            {
                item => item.RelationLocation,
                item => item.RelationAttribute,
                item => item.RelationType,
                item => item.SourceLocation,
                item => item.SourceAttribute,
            };

        /// <summary>
        /// The definition provider.
        /// </summary>
        private readonly IEntityDefinitionProvider definitionProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityRelation"/> class.
        /// </summary>
        /// <param name="definitionProvider">
        /// The definition provider.
        /// </param>
        /// <param name="relationType">
        /// The relation type.
        /// </param>
        public EntityRelation(IEntityDefinitionProvider definitionProvider, EntityRelationType relationType)
        {
            if (definitionProvider == null)
            {
                throw new ArgumentNullException(nameof(definitionProvider));
            }

            this.RelationType = relationType;
            this.definitionProvider = definitionProvider;
        }

        /// <summary>
        /// Gets the type of entity relation.
        /// </summary>
        public EntityRelationType RelationType { get; }

        /// <summary>
        /// Gets the source location.
        /// </summary>
        public EntityLocation SourceLocation { get; private set; }

        /// <summary>
        /// Gets the source selector.
        /// </summary>
        public EntityAttributeDefinition SourceAttribute { get; private set; }

        /// <summary>
        /// Gets the relation location.
        /// </summary>
        public EntityLocation RelationLocation { get; private set; }

        /// <summary>
        /// Gets the relation selector.
        /// </summary>
        public EntityAttributeDefinition RelationAttribute { get; private set; }

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
            return
                string.Concat(
                    $"{this.SourceLocation}.{this.SourceLocation} ",
                    $"{this.RelationType} ",
                    $"{this.RelationLocation}.{this.RelationLocation} ",
                    $"([{this.SourceAttribute}] = [{this.RelationAttribute}])");
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
            return Evaluate.GenerateHashCode(this, ComparisonProperties);
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

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(EntityRelation other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
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

            var leftReference = leftAttribute.GetEntityReference();

            this.SourceLocation = this.definitionProvider.GetEntityLocation(leftReference);
            this.SourceAttribute =
                this.definitionProvider.Resolve(leftReference.EntityType)
                    .DirectAttributes.FirstOrDefault(x => x.PropertyName == leftAttribute.GetPropertyName());

            var rightReference = rightAttribute.GetEntityReference();

            this.RelationLocation = this.definitionProvider.GetEntityLocation(rightReference);
            this.RelationAttribute =
                this.definitionProvider.Resolve(rightReference.EntityType)
                    .DirectAttributes.FirstOrDefault(x => x.PropertyName == rightAttribute.GetPropertyName());
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

            var sourceReference = new EntityReference { EntityType = typeof(TSource), EntityAlias = sourceAlias };
            this.SourceLocation = this.definitionProvider.GetEntityLocation(sourceReference);

            var relationReference = new EntityReference { EntityType = typeof(TRelation), EntityAlias = relationAlias };
            this.RelationLocation = this.definitionProvider.GetEntityLocation(relationReference);

            var sourceDefinition = this.definitionProvider.Resolve<TSource>();
            var relationDefinition = this.definitionProvider.Resolve<TRelation>();
            this.SourceAttribute = sourceDefinition.DirectAttributes.FirstOrDefault(x => x.PropertyName == sourceAttribute.GetPropertyName());
            this.RelationAttribute = relationDefinition.DirectAttributes.FirstOrDefault(x => x.PropertyName == relationAttribute.GetPropertyName());
        }
    }
}