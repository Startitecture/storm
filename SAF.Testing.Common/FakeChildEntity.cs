// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeChildEntity.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The fake child entity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Testing.Common
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using Startitecture.Core;

    /// <summary>
    /// The fake child entity.
    /// </summary>
    public class FakeChildEntity : IEquatable<FakeChildEntity>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<FakeChildEntity, object>[] ComparisonProperties =
            {
                item => item.Name,
                item => item.SomeValue,
                item => item.FakeComplexEntity,
                item => item.Parent
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeChildEntity"/> class.
        /// </summary>
        /// <param name="fakeComplexEntity">
        /// The complex entity.
        /// </param>
        public FakeChildEntity(FakeComplexEntity fakeComplexEntity)
            : this(fakeComplexEntity, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeChildEntity"/> class.
        /// </summary>
        /// <param name="fakeComplexEntity">
        /// The complex entity.
        /// </param>
        /// <param name="fakeChildEntityId">
        /// The fake child entity id.
        /// </param>
        public FakeChildEntity([NotNull] FakeComplexEntity fakeComplexEntity, int? fakeChildEntityId)
        {
            if (fakeComplexEntity == null)
            {
                throw new ArgumentNullException(nameof(fakeComplexEntity));
            }

            this.FakeComplexEntity = fakeComplexEntity;
            this.FakeChildEntityId = fakeChildEntityId;
        }

        /// <summary>
        /// Gets the fake child entity id.
        /// </summary>
        public int? FakeChildEntityId { get; private set; }

        /// <summary>
        /// Gets the complex entity.
        /// </summary>
        public FakeComplexEntity FakeComplexEntity { get; private set; }

        /// <summary>
        /// Gets the fake complex entity id.
        /// </summary>
        public int? FakeComplexEntityId
        {
            get
            {
                return this.FakeComplexEntity?.FakeComplexEntityId;
            }
        }

        /// <summary>
        /// Gets the fake enumeration.
        /// </summary>
        public FakeEnumeration FakeEnumeration
        {
            get
            {
                return this.FakeComplexEntity == null ? FakeEnumeration.Unknown : this.FakeComplexEntity.FakeEnumeration;
            }
        }

        /// <summary>
        /// Gets the fake other enumeration.
        /// </summary>
        public FakeOtherEnumeration FakeOtherEnumeration
        {
            get
            {
                return this.FakeComplexEntity == null ? FakeOtherEnumeration.Unknown : this.FakeComplexEntity.FakeOtherEnumeration;
            }
        }

        /// <summary>
        /// Gets the fake dependent entity, if any.
        /// </summary>
        public FakeDependentEntity FakeDependentEntity
        {
            get
            {
                return this.FakeComplexEntity?.FakeDependentEntity;
            }
        }

        /// <summary>
        /// Gets the fake dependent entity ID, if any.
        /// </summary>
        public int? FakeDependentEntityId
        {
            get
            {
                // Because the entity is dependent upon FakeComplexEntity, we rely on FakeComplexEntity's implementation.
                return this.FakeDependentEntity?.FakeDependentEntityId;
            }
        }

        /// <summary>
        /// Gets the sub entity.
        /// </summary>
        public FakeSubEntity FakeSubEntity
        {
            get
            {
                return this.FakeComplexEntity?.FakeSubEntity;
            }
        }

        /// <summary>
        /// Gets the fake complex entity id.
        /// </summary>
        public int? FakeSubEntityId
        {
            get
            {
                return this.FakeSubEntity?.FakeSubEntityId;
            }
        }

        /// <summary>
        /// Gets the sub sub entity.
        /// </summary>
        public FakeSubSubEntity FakeSubSubEntity
        {
            get
            {
                return this.FakeSubEntity?.FakeSubSubEntity;
            }
        }

        /// <summary>
        /// Gets the fake complex entity id.
        /// </summary>
        public int? FakeSubSubEntityId
        {
            get
            {
                return this.FakeSubSubEntity?.FakeSubSubEntityId;
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the some value.
        /// </summary>
        public int SomeValue { get; set; }

        /// <summary>
        /// Gets the creation entity.
        /// </summary>
        public FakeCreatedBy CreatedBy
        {
            get
            {
                return this.FakeComplexEntity?.CreatedBy;
            }
        }

        /// <summary>
        /// Gets the fake complex entity id.
        /// </summary>
        public int? CreatedByFakeMultiReferenceEntityId
        {
            get
            {
                return this.CreatedBy?.FakeMultiReferenceEntityId;
            }
        }

        /// <summary>
        /// Gets the modified entity.
        /// </summary>
        public FakeModifiedBy ModifiedBy
        {
            get
            {
                return this.FakeComplexEntity?.ModifiedBy;
            }
        }

        /// <summary>
        /// Gets the fake complex entity id.
        /// </summary>
        public int? ModifiedByFakeMultiReferenceEntityId
        {
            get
            {
                return this.ModifiedBy?.FakeMultiReferenceEntityId;
            }
        }

        /// <summary>
        /// Gets or sets the parent of the child.
        /// </summary>
        public FakeChildEntity Parent { get; set; }

        /// <summary>
        /// Gets the parent fake child entity id.
        /// </summary>
        public int? ParentFakeChildEntityId
        {
            get
            {
                return this.Parent?.FakeChildEntityId;
            }
        }

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
        public static bool operator ==(FakeChildEntity valueA, FakeChildEntity valueB)
        {
            return EqualityComparer<FakeChildEntity>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(FakeChildEntity valueA, FakeChildEntity valueB)
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
            return this.Name;
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
        public bool Equals(FakeChildEntity other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }
    }
}
