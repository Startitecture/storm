﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainIdentity.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Resources;

    /// <summary>
    /// The domain identity.
    /// </summary>
    public class DomainIdentity : IEquatable<DomainIdentity>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<DomainIdentity, object>[] ComparisonProperties =
            {
                item => item.UniqueIdentifier,
                item => item.FirstName,
                item => item.MiddleName,
                item => item.LastName
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainIdentity"/> class.
        /// </summary>
        /// <param name="uniqueIdentifier">
        /// The unique identifier.
        /// </param>
        public DomainIdentity([NotNull] string uniqueIdentifier)
        {
            if (string.IsNullOrWhiteSpace(uniqueIdentifier))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(uniqueIdentifier));
            }

            this.UniqueIdentifier = uniqueIdentifier;
        }

        /// <summary>
        /// Gets the domain identity ID.
        /// </summary>
        public int? DomainIdentityId { get; private set; }

        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        public string UniqueIdentifier { get; private set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the middle name.
        /// </summary>
        public string MiddleName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        public string LastName { get; set; }

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
        public static bool operator ==(DomainIdentity valueA, DomainIdentity valueB)
        {
            return EqualityComparer<DomainIdentity>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(DomainIdentity valueA, DomainIdentity valueB)
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
            return $"({this.UniqueIdentifier}) {this.FirstName} {this.LastName}";
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
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        public bool Equals(DomainIdentity other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }
    }
}