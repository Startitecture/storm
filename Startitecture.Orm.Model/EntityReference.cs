﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityReference.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Contains a reference to an entity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using System;
    using System.Collections.Generic;

    using Startitecture.Core;

    /// <summary>
    /// Contains a reference to an entity.
    /// </summary>
    public class EntityReference : IEquatable<EntityReference>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<EntityReference, object>[] ComparisonProperties =
            {
                item => item.EntityType,
                item => item.EntityAlias,
                item => item.ContainerType
            };

        /// <summary>
        /// Gets or sets the container type.
        /// </summary>
        public Type ContainerType { get; set; }

        /// <summary>
        /// Gets or sets the entity alias.
        /// </summary>
        public string EntityAlias { get; set; }

        /// <summary>
        /// Gets or sets the entity type.
        /// </summary>
        public Type EntityType { get; set; }

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
        public static bool operator ==(EntityReference valueA, EntityReference valueB)
        {
            return EqualityComparer<EntityReference>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(EntityReference valueA, EntityReference valueB)
        {
            return !(valueA == valueB);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        public override int GetHashCode()
        {
            return Evaluate.GenerateHashCode(this, ComparisonProperties);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"{(this.ContainerType ?? this.EntityType)?.Name}({this.EntityType?.Name}:[{this.EntityAlias}])";
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
        public bool Equals(EntityReference other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }
    }
}