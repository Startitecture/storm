// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChildEntity.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The fake child entity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using Startitecture.Core;

    /// <summary>
    /// The fake child entity.
    /// </summary>
    public class ChildEntity : IEquatable<ChildEntity>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<ChildEntity, object>[] ComparisonProperties =
            {
                item => item.Name,
                item => item.SomeValue,
                item => item.ComplexEntity,
                item => item.Parent
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildEntity"/> class.
        /// </summary>
        /// <param name="complexEntity">
        /// The complex entity.
        /// </param>
        public ChildEntity(ComplexEntity complexEntity)
            : this(complexEntity, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildEntity"/> class.
        /// </summary>
        /// <param name="complexEntity">
        /// The complex entity.
        /// </param>
        /// <param name="fakeChildEntityId">
        /// The fake child entity id.
        /// </param>
        public ChildEntity([NotNull] ComplexEntity complexEntity, int fakeChildEntityId)
        {
            if (complexEntity == null)
            {
                throw new ArgumentNullException(nameof(complexEntity));
            }

            this.ComplexEntity = complexEntity;
            this.FakeChildEntityId = fakeChildEntityId;
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="ChildEntity"/> class from being created.
        /// </summary>
        private ChildEntity()
        {
        }

        /// <summary>
        /// Gets the fake child entity id.
        /// </summary>
        public int? FakeChildEntityId { get; private set; }

        /// <summary>
        /// Gets the complex entity.
        /// </summary>
        public ComplexEntity ComplexEntity { get; private set; }

        /// <summary>
        /// Gets the fake complex entity id.
        /// </summary>
        public int? ComplexEntityId
        {
            get
            {
                return this.ComplexEntity?.ComplexEntityId;
            }
        }

        /// <summary>
        /// Gets the fake enumeration.
        /// </summary>
        public FakeEnumeration FakeEnumeration
        {
            get
            {
                return this.ComplexEntity == null ? FakeEnumeration.Unknown : this.ComplexEntity.FakeEnumeration;
            }
        }

        /// <summary>
        /// Gets the fake other enumeration.
        /// </summary>
        public FakeOtherEnumeration FakeOtherEnumeration
        {
            get
            {
                return this.ComplexEntity == null ? FakeOtherEnumeration.Unknown : this.ComplexEntity.FakeOtherEnumeration;
            }
        }

        /// <summary>
        /// Gets the fake dependent entity, if any.
        /// </summary>
        public DependentEntity DependentEntity
        {
            get
            {
                return this.ComplexEntity?.DependentEntity;
            }
        }

        /// <summary>
        /// Gets the fake dependent entity ID, if any.
        /// </summary>
        public int? FakeDependentEntityId
        {
            get
            {
                // Because the entity is dependent upon ComplexEntity, we rely on ComplexEntity's implementation.
                return this.DependentEntity?.FakeDependentEntityId;
            }
        }

        /// <summary>
        /// Gets the sub entity.
        /// </summary>
        public SubEntity SubEntity
        {
            get
            {
                return this.ComplexEntity?.SubEntity;
            }
        }

        /// <summary>
        /// Gets the fake complex entity id.
        /// </summary>
        public int? FakeSubEntityId
        {
            get
            {
                return this.SubEntity?.FakeSubEntityId;
            }
        }

        /// <summary>
        /// Gets the sub sub entity.
        /// </summary>
        public SubSubEntity SubSubEntity
        {
            get
            {
                return this.SubEntity?.SubSubEntity;
            }
        }

        /// <summary>
        /// Gets the fake complex entity id.
        /// </summary>
        public int? FakeSubSubEntityId
        {
            get
            {
                return this.SubSubEntity?.FakeSubSubEntityId;
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
        public CreatedBy CreatedBy
        {
            get
            {
                return this.ComplexEntity?.CreatedBy;
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
        public ModifiedBy ModifiedBy
        {
            get
            {
                return this.ComplexEntity?.ModifiedBy;
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
        public ChildEntity Parent { get; set; }

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
        public static bool operator ==(ChildEntity valueA, ChildEntity valueB)
        {
            return EqualityComparer<ChildEntity>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(ChildEntity valueA, ChildEntity valueB)
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
        public bool Equals(ChildEntity other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }
    }
}
