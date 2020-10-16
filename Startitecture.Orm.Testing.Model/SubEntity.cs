﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubEntity.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model
{
    using System;
    using System.Collections.Generic;

    using Startitecture.Core;

    /// <summary>
    /// The fake sub entity.
    /// </summary>
    public class SubEntity : IEquatable<SubEntity>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<SubEntity, object>[] ComparisonProperties =
            {
                item => item.UniqueName,
                item => item.Description,
                item => item.UniqueOtherId,
                item => item.SubSubEntity
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="SubEntity"/> class.
        /// </summary>
        /// <param name="uniqueName">
        /// The unique name.
        /// </param>
        /// <param name="uniqueOtherId">
        /// The unique other id.
        /// </param>
        /// <param name="subSubEntity">
        /// The sub sub entity.
        /// </param>
        public SubEntity(string uniqueName, short uniqueOtherId, SubSubEntity subSubEntity)
            : this(uniqueName, uniqueOtherId, subSubEntity, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubEntity"/> class.
        /// </summary>
        /// <param name="uniqueName">
        /// The unique name.
        /// </param>
        /// <param name="uniqueOtherId">
        /// The unique other id.
        /// </param>
        /// <param name="subSubEntity">
        /// The sub sub entity.
        /// </param>
        /// <param name="fakeSubEntityId">
        /// The fake sub entity id.
        /// </param>
        public SubEntity(
            string uniqueName,
            short uniqueOtherId,
            SubSubEntity subSubEntity,
            int? fakeSubEntityId)
        {
            this.UniqueName = uniqueName;
            this.UniqueOtherId = uniqueOtherId;
            this.FakeSubEntityId = fakeSubEntityId;
            this.SubSubEntity = subSubEntity;
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="SubEntity"/> class from being created.
        /// </summary>
        private SubEntity()
        {
        }

        /// <summary>
        /// Gets the fake sub entity id.
        /// </summary>
        public int? FakeSubEntityId { get; private set; }

        /// <summary>
        /// Gets the sub sub entity.
        /// </summary>
        public SubSubEntity SubSubEntity { get; private set; }

        /// <summary>
        /// Gets the fake sub sub entity id.
        /// </summary>
        public int? FakeSubSubEntityId
        {
            get
            {
                return this.SubSubEntity == null ? null : this.SubSubEntity.FakeSubSubEntityId;
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
        public static bool operator ==(SubEntity valueA, SubEntity valueB)
        {
            return EqualityComparer<SubEntity>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(SubEntity valueA, SubEntity valueB)
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
            return this.UniqueName;
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
        public bool Equals(SubEntity other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }
    }
}
