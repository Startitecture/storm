// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeMultiReferenceEntity.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Testing.Common
{
    using System;

    /// <summary>
    /// The fake multi reference entity.
    /// </summary>
    public class FakeMultiReferenceEntity : IEquatable<FakeMultiReferenceEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeMultiReferenceEntity"/> class.
        /// </summary>
        /// <param name="uniqueName">
        /// The unique name.
        /// </param>
        public FakeMultiReferenceEntity(string uniqueName)
            : this(uniqueName, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeMultiReferenceEntity"/> class.
        /// </summary>
        /// <param name="uniqueName">
        /// The unique name.
        /// </param>
        /// <param name="fakeMultiReferenceEntityId">
        /// The fake multi reference entity id.
        /// </param>
        public FakeMultiReferenceEntity(string uniqueName, int? fakeMultiReferenceEntityId)
        {
            this.UniqueName = uniqueName;
            this.FakeMultiReferenceEntityId = fakeMultiReferenceEntityId;
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="FakeMultiReferenceEntity"/> class from being created.
        /// </summary>
        private FakeMultiReferenceEntity()
        {
        }

        /// <summary>
        /// Gets the fake multi reference entity id.
        /// </summary>
        public int? FakeMultiReferenceEntityId { get; private set; }

        /// <summary>
        /// Gets the unique name.
        /// </summary>
        public string UniqueName { get; private set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Serves as the default hash function. 
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((this.UniqueName != null ? this.UniqueName.GetHashCode() : 0) * 397)
                       ^ (this.Description != null ? this.Description.GetHashCode() : 0);
            }
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
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((FakeMultiReferenceEntity)obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(FakeMultiReferenceEntity other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(this.UniqueName, other.UniqueName) && string.Equals(this.Description, other.Description);
        }
    }
}
