﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependentEntity.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model
{
    using System;
    using System.Collections.Generic;

    using Startitecture.Core;

    /// <summary>
    /// The fake dependent entity.
    /// </summary>
    public class DependentEntity : IEquatable<DependentEntity>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<DependentEntity, object>[] ComparisonProperties =
            {
                item => item.DependentIntegerValue,
                item => item.DependentTimeValue
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="DependentEntity"/> class.
        /// </summary>
        public DependentEntity()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependentEntity"/> class.
        /// </summary>
        /// <param name="fakeDependentEntityId">
        /// The fake dependent entity ID.
        /// </param>
        public DependentEntity(int? fakeDependentEntityId)
        {
            this.FakeDependentEntityId = fakeDependentEntityId;
        }

        /// <summary>
        /// Gets the fake dependent entity ID.
        /// </summary>
        public int? FakeDependentEntityId { get; private set; }

        /// <summary>
        /// Gets or sets the dependent integer value.
        /// </summary>
        public int DependentIntegerValue { get; set; }

        /// <summary>
        /// Gets or sets the dependent time value.
        /// </summary>
        public DateTimeOffset DependentTimeValue { get; set; }

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
        public static bool operator ==(DependentEntity valueA, DependentEntity valueB)
        {
            return EqualityComparer<DependentEntity>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(DependentEntity valueA, DependentEntity valueB)
        {
            return !(valueA == valueB);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return this.DependentIntegerValue.ToString();
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
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(DependentEntity other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }
    }
}