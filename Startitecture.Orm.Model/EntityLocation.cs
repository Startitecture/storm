// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityLocation.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Contains the location for a data entity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using System;

    using Startitecture.Core;

    /// <summary>
    /// Contains the location for a data entity.
    /// </summary>
    public struct EntityLocation : IEquatable<EntityLocation>
    {
        /// <summary>
        /// The to string format.
        /// </summary>
        private const string ToStringFormat = "{0}/{1}";

        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<EntityLocation, object>[] ComparisonProperties =
            {
                item => item.EntityType,
                item => item.Name,
                item => item.Container,
                item => item.Alias,
                item => item.IsVirtual
            };

        /// <summary>
        /// The hash code.
        /// </summary>
        private readonly Lazy<int> hashCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityLocation"/> struct.
        /// </summary>
        /// <param name="entityType">
        /// The entity type.
        /// </param>
        /// <param name="container">
        /// The entity's container.
        /// </param>
        /// <param name="name">
        /// The entity's name.
        /// </param>
        public EntityLocation(Type entityType, string container, string name)
            : this(entityType, container, name, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityLocation"/> struct.
        /// </summary>
        /// <param name="entityType">
        /// The entity type.
        /// </param>
        /// <param name="container">
        /// The entity's container.
        /// </param>
        /// <param name="name">
        /// The entity's name.
        /// </param>
        /// <param name="alias">
        /// The entity's alias.
        /// </param>
        public EntityLocation(Type entityType, string container, string name, string alias)
            : this(entityType, container, name, false, alias)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityLocation"/> struct.
        /// </summary>
        /// <param name="entityType">
        /// The entity type.
        /// </param>
        /// <param name="container">
        /// The entity's container.
        /// </param>
        /// <param name="name">
        /// The entity's name.
        /// </param>
        /// <param name="isVirtual">
        /// A value indicating whether the entity location is virtual. If true, the current location will not be traversed when setting
        /// physical properties.
        /// </param>
        /// <param name="alias">
        /// The entity's alias.
        /// </param>
        public EntityLocation(Type entityType, string container, string name, bool isVirtual, string alias)
            : this()
        {
            this.Container = container;
            this.Name = name;
            this.IsVirtual = isVirtual;
            this.Alias = alias;
            this.EntityType = entityType;
            this.hashCode = new Lazy<int>(this.CreateHashCode);
        }

        /// <summary>
        /// Gets the type of the entity location.
        /// </summary>
        public Type EntityType { get; }

        /// <summary>
        /// Gets the entity's container.
        /// </summary>
        public string Container { get; }

        /// <summary>
        /// Gets the entity's name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the qualified name of the entity location.
        /// </summary>
        public string QualifiedName => $"[{this.Container}].[{this.Name}]";

        /// <summary>
        /// Gets the reference name of the entity location.
        /// </summary>
        public string ReferenceName
        {
            get
            {
                var isEntityAliased = string.IsNullOrWhiteSpace(this.Alias) == false;
                return isEntityAliased ? string.Concat('[', this.Alias, ']') : string.Concat('[', this.Container, ']', '.', '[', this.Name, ']');
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the location is virtual. When true, this location will not be traversed for setting 
        /// physical properties.
        /// </summary>
        public bool IsVirtual { get; set; }

        /// <summary>
        /// Gets the entity's alias.
        /// </summary>
        public string Alias { get; }

        /// <summary>
        /// Compares the equality of two values of the same type.
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
        public static bool operator ==(EntityLocation valueA, EntityLocation valueB)
        {
            return Evaluate.Equals(valueA, valueB);
        }

        /// <summary>
        /// Compares the inequality of two values of the same type.
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
        public static bool operator !=(EntityLocation valueA, EntityLocation valueB)
        {
            return !(valueA == valueB);
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> containing a fully qualified type name.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format(ToStringFormat, this.Container, this.Alias ?? this.Name);
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
            return this.hashCode.Value;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">
        /// The object to compare with the current instance. 
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
        public bool Equals(EntityLocation other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        /// <summary>
        /// Creates hash code.
        /// </summary>
        /// <returns>
        /// The hash code for this value as an <see cref="int" />.
        /// </returns>
        private int CreateHashCode()
        {
            return Evaluate.GenerateHashCode(this, ComparisonProperties);
        }
    }
}