// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeSubEntity.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Testing.Common
{
    using System;
    using System.Collections.Generic;

    using SAF.Core;

    /// <summary>
    /// The fake sub entity.
    /// </summary>
    public class FakeSubEntity : IEquatable<FakeSubEntity>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<FakeSubEntity, object>[] ComparisonProperties =
            {
                item => item.UniqueName,
                item => item.Description,
                item => item.UniqueOtherId,
                item => item.FakeSubSubEntity
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeSubEntity"/> class.
        /// </summary>
        /// <param name="uniqueName">
        /// The unique name.
        /// </param>
        /// <param name="uniqueOtherId">
        /// The unique other id.
        /// </param>
        /// <param name="fakeSubSubEntity">
        /// The sub sub entity.
        /// </param>
        public FakeSubEntity(string uniqueName, short uniqueOtherId, FakeSubSubEntity fakeSubSubEntity)
            : this(uniqueName, uniqueOtherId, fakeSubSubEntity, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeSubEntity"/> class.
        /// </summary>
        /// <param name="uniqueName">
        /// The unique name.
        /// </param>
        /// <param name="uniqueOtherId">
        /// The unique other id.
        /// </param>
        /// <param name="fakeSubSubEntity">
        /// The sub sub entity.
        /// </param>
        /// <param name="fakeSubEntityId">
        /// The fake sub entity id.
        /// </param>
        public FakeSubEntity(
            string uniqueName, 
            short uniqueOtherId, 
            FakeSubSubEntity fakeSubSubEntity, 
            int? fakeSubEntityId)
        {
            this.UniqueName = uniqueName;
            this.UniqueOtherId = uniqueOtherId;
            this.FakeSubEntityId = fakeSubEntityId;
            this.FakeSubSubEntity = fakeSubSubEntity;
        }

        /// <summary>
        /// Gets the fake sub entity id.
        /// </summary>
        public int? FakeSubEntityId { get; private set; }

        /// <summary>
        /// Gets the sub sub entity.
        /// </summary>
        public FakeSubSubEntity FakeSubSubEntity { get; private set; }

        /// <summary>
        /// Gets the fake sub sub entity id.
        /// </summary>
        public int? FakeSubSubEntityId
        {
            get
            {
                return this.FakeSubSubEntity == null ? null : this.FakeSubSubEntity.FakeSubSubEntityId;
            }
        }

        /// <summary>
        /// Gets the unique name.
        /// </summary>
        public string UniqueName { get; private set; }

        /// <summary>
        /// Gets the unique other id.
        /// </summary>
        public short UniqueOtherId { get; private set; }

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
        public static bool operator ==(FakeSubEntity valueA, FakeSubEntity valueB)
        {
            return EqualityComparer<FakeSubEntity>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(FakeSubEntity valueA, FakeSubEntity valueB)
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
        public bool Equals(FakeSubEntity other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        #endregion
    }
}
