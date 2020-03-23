// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityAttributeInstance.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Stores the value of an entity attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using System;

    using Startitecture.Core;

    /// <summary>
    /// Stores the value of an entity attribute.
    /// </summary>
    public struct EntityAttributeInstance : IEquatable<EntityAttributeInstance>
    {
        /// <summary>
        /// Matches attributes that have non-null values.
        /// </summary>
        public static readonly Func<EntityAttributeInstance, bool> AttributeNotNullPredicate = x => x.Value != null;

        /// <summary>
        /// Selects the attribute value from the instance.
        /// </summary>
        public static readonly Func<EntityAttributeInstance, object> SelectAttributeValue = x => x.Value;

        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<EntityAttributeInstance, object>[] ComparisonProperties =
            {
                item => item.AttributeDefinition,
                item => item.Value
            };

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityAttributeInstance"/> struct. 
        /// </summary>
        /// <param name="attributeDefinition">
        /// The attribute definition.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public EntityAttributeInstance(EntityAttributeDefinition attributeDefinition, object value)
            : this()
        {
            this.Value = value;
            this.AttributeDefinition = attributeDefinition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the attribute definition for the current instance.
        /// </summary>
        public EntityAttributeDefinition AttributeDefinition { get; }

        /// <summary>
        /// Gets the attribute value for the current instance.
        /// </summary>
        public object Value { get; }

        #endregion

        /// <summary>
        /// Determines whether two values of the same type are equal.
        /// </summary>
        /// <param name="firstValue">
        /// The first value.
        /// </param>
        /// <param name="secondValue">
        /// The second value.
        /// </param>
        /// <returns>
        /// <c>true</c> if the values are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(EntityAttributeInstance firstValue, EntityAttributeInstance secondValue)
        {
            return Evaluate.Equals(firstValue, secondValue);
        }

        /// <summary>
        /// Determines whether two values of the same type are not equal.
        /// </summary>
        /// <param name="firstValue">
        /// The first value.
        /// </param>
        /// <param name="secondValue">
        /// The second value.
        /// </param>
        /// <returns>
        /// <c>true</c> if the values are not equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(EntityAttributeInstance firstValue, EntityAttributeInstance secondValue)
        {
            return !(firstValue == secondValue);
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="String"/> containing a fully qualified type name.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Concat(this.AttributeDefinition, '=', this.Value);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return Evaluate.GenerateHashCode(this, ComparisonProperties);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">Another object to compare to. </param><filterpriority>2</filterpriority>
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
        public bool Equals(EntityAttributeInstance other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }
    }
}