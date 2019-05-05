// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttributeLocation.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    using JetBrains.Annotations;

    using Startitecture.Core;

    /// <summary>
    /// The attribute location.
    /// </summary>
    public class AttributeLocation : IEquatable<AttributeLocation>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<AttributeLocation, object>[] ComparisonProperties =
            {
                item => item.PropertyInfo,
                item => item.EntityReference
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeLocation"/> class.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        public AttributeLocation(LambdaExpression expression)
        {
            var member = expression.GetMember();
            var property = expression.GetProperty();
            this.EntityReference = new EntityReference
                                       {
                                           EntityType = member.Expression.Type,
                                           EntityAlias = (member.Expression as MemberExpression)?.Member.Name
                                       };

            this.PropertyInfo = property;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeLocation"/> class.
        /// </summary>
        /// <param name="propertyInfo">
        /// The attribute name.
        /// </param>
        /// <param name="entityReference">
        /// The entity reference.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="propertyInfo"/> is null.
        /// </exception>
        public AttributeLocation([NotNull] PropertyInfo propertyInfo, EntityReference entityReference)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            this.PropertyInfo = propertyInfo;
            this.EntityReference = entityReference;
        }

        /// <summary>
        /// Gets the attribute name.
        /// </summary>
        public PropertyInfo PropertyInfo { get; }

        /// <summary>
        /// Gets the entity reference.
        /// </summary>
        public EntityReference EntityReference { get; }

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
        public static bool operator ==(AttributeLocation valueA, AttributeLocation valueB)
        {
            return EqualityComparer<AttributeLocation>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(AttributeLocation valueA, AttributeLocation valueB)
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
            return $"{this.EntityReference}/{this.PropertyInfo}";
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
        /// <param name="obj">
        /// The object to compare with the current object.
        /// </param>
        /// <filterpriority>2</filterpriority>
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
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        public bool Equals(AttributeLocation other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }
    }
}