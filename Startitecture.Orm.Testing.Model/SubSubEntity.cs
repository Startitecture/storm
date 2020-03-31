// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubSubEntity.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model
{
    using System;
    using System.Collections.Generic;

    using Startitecture.Core;

    /// <summary>
    /// The fake sub sub entity.
    /// </summary>
    public class SubSubEntity : IEquatable<SubSubEntity>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<SubSubEntity, object>[] ComparisonProperties =
            {
                item => item.UniqueName,
                item => item.Description
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="SubSubEntity"/> class.
        /// </summary>
        /// <param name="uniqueName">
        /// The unique name.
        /// </param>
        public SubSubEntity(string uniqueName)
            : this(uniqueName, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubSubEntity"/> class.
        /// </summary>
        /// <param name="uniqueName">
        /// The unique name.
        /// </param>
        /// <param name="fakeSubSubEntityId">
        /// The fake sub sub entity id.
        /// </param>
        public SubSubEntity(string uniqueName, int? fakeSubSubEntityId)
        {
            this.UniqueName = uniqueName;
            this.FakeSubSubEntityId = fakeSubSubEntityId;
        }

        /// <summary>
        /// Gets the fake sub sub entity id.
        /// </summary>
        public int? FakeSubSubEntityId { get; private set; }

        /// <summary>
        /// Gets the unique name.
        /// </summary>
        public string UniqueName { get; private set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        #region Equality and Comparison Methods

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
        public static bool operator ==(SubSubEntity valueA, SubSubEntity valueB)
        {
            return EqualityComparer<SubSubEntity>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(SubSubEntity valueA, SubSubEntity valueB)
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
            return this.UniqueName;
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
        public bool Equals(SubSubEntity other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        #endregion
    }
}
