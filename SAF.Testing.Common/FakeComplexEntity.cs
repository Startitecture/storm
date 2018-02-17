// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeComplexEntity.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.Testing.Common
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using Startitecture.Core;

    /// <summary>
    /// The fake complex entity.
    /// </summary>
    public class FakeComplexEntity : IEquatable<FakeComplexEntity>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<FakeComplexEntity, object>[] ComparisonProperties =
            {
                item => item.UniqueName,
                item => item.Description,
                item => item.FakeDependentEntity,
                item => item.FakeEnumeration,
                item => item.FakeOtherEnumeration,
                item => item.FakeSubEntity,
                item => item.FakeSubSubEntity,
                item => item.CreationTime,
                item => item.CreatedBy,
                item => item.ModifiedTime,
                item => item.ModifiedBy
            };

        /// <summary>
        /// The child entities.
        /// </summary>
        private readonly List<FakeChildEntity> childEntities = new List<FakeChildEntity>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeComplexEntity" /> class.
        /// </summary>
        /// <param name="uniqueName">
        /// The unique name.
        /// </param>
        /// <param name="fakeSubEntity">
        /// The sub entity.
        /// </param>
        /// <param name="fakeEnumeration">
        /// The fake enumeration.
        /// </param>
        /// <param name="createdBy">
        /// The creation entity.
        /// </param>
        public FakeComplexEntity(string uniqueName, FakeSubEntity fakeSubEntity, FakeEnumeration fakeEnumeration, FakeCreatedBy createdBy)
            : this(uniqueName, fakeSubEntity, fakeEnumeration, createdBy, DateTimeOffset.Now)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeComplexEntity" /> class.
        /// </summary>
        /// <param name="uniqueName">
        /// The unique name.
        /// </param>
        /// <param name="fakeSubEntity">
        /// The sub entity.
        /// </param>
        /// <param name="fakeEnumeration">
        /// The fake enumeration.
        /// </param>
        /// <param name="createdBy">
        /// The creation entity.
        /// </param>
        /// <param name="creationTime">
        /// The creation time.
        /// </param>
        public FakeComplexEntity(
            string uniqueName,
            FakeSubEntity fakeSubEntity,
            FakeEnumeration fakeEnumeration,
            FakeCreatedBy createdBy,
            DateTimeOffset creationTime)
            : this(uniqueName, fakeSubEntity, fakeEnumeration, createdBy, creationTime, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeComplexEntity" /> class.
        /// </summary>
        /// <param name="uniqueName">
        /// The unique name.
        /// </param>
        /// <param name="fakeSubEntity">
        /// The sub entity.
        /// </param>
        /// <param name="fakeEnumeration">
        /// The fake enumeration.
        /// </param>
        /// <param name="createdBy">
        /// The creation entity.
        /// </param>
        /// <param name="creationTime">
        /// The creation time.
        /// </param>
        /// <param name="fakeComplexEntityId">
        /// The fake complex entity id.
        /// </param>
        public FakeComplexEntity(
            string uniqueName,
            FakeSubEntity fakeSubEntity,
            FakeEnumeration fakeEnumeration,
            FakeCreatedBy createdBy,
            DateTimeOffset creationTime,
            int? fakeComplexEntityId)
        {
            this.UniqueName = uniqueName;
            this.FakeComplexEntityId = fakeComplexEntityId;
            this.FakeEnumeration = fakeEnumeration;
            this.FakeSubEntity = fakeSubEntity;
            this.CreationTime = creationTime;
            this.CreatedBy = createdBy;
        }

        /// <summary>
        /// Gets the child entities.
        /// </summary>
        public IEnumerable<FakeChildEntity> ChildEntities
        {
            get
            {
                return this.childEntities;
            }
        }

        /// <summary>
        /// Gets the creation entity.
        /// </summary>
        public FakeCreatedBy CreatedBy { get; private set; }

        /// <summary>
        /// Gets the fake complex entity id.
        /// </summary>
        public int? CreatedByFakeMultiReferenceEntityId
        {
            get
            {
                return this.CreatedBy == null ? null : this.CreatedBy.FakeMultiReferenceEntityId;
            }
        }

        /// <summary>
        /// Gets the creation time.
        /// </summary>
        public DateTimeOffset CreationTime { get; private set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets the fake complex entity id.
        /// </summary>
        public int? FakeComplexEntityId { get; private set; }

        /// <summary>
        /// Gets the fake enumeration.
        /// </summary>
        public FakeEnumeration FakeEnumeration { get; private set; }

        /// <summary>
        /// Gets or sets the fake other enumeration.
        /// </summary>
        public FakeOtherEnumeration FakeOtherEnumeration { get; set; }

        /// <summary>
        /// Gets the sub entity.
        /// </summary>
        public FakeSubEntity FakeSubEntity { get; private set; }

        /// <summary>
        /// Gets the fake complex entity id.
        /// </summary>
        public int? FakeSubEntityId
        {
            get
            {
                return this.FakeSubEntity == null ? null : this.FakeSubEntity.FakeSubEntityId;
            }
        }

        /// <summary>
        /// Gets the sub sub entity.
        /// </summary>
        public FakeSubSubEntity FakeSubSubEntity
        {
            get
            {
                return this.FakeSubEntity == null ? null : this.FakeSubEntity.FakeSubSubEntity;
            }
        }

        /// <summary>
        /// Gets the fake complex entity id.
        /// </summary>
        public int? FakeSubSubEntityId
        {
            get
            {
                return this.FakeSubSubEntity == null ? null : this.FakeSubSubEntity.FakeSubSubEntityId;
            }
        }

        /// <summary>
        /// Gets or sets the modified entity.
        /// </summary>
        public FakeModifiedBy ModifiedBy { get; set; }

        /// <summary>
        /// Gets the fake complex entity id.
        /// </summary>
        public int? ModifiedByFakeMultiReferenceEntityId
        {
            get
            {
                return this.ModifiedBy == null ? null : this.ModifiedBy.FakeMultiReferenceEntityId;
            }
        }

        /// <summary>
        /// Gets or sets the modified time.
        /// </summary>
        public DateTimeOffset ModifiedTime { get; set; }

        /// <summary>
        /// Gets the unique name.
        /// </summary>
        public string UniqueName { get; private set; }

        /// <summary>
        /// Gets the fake dependent entity, if any.
        /// </summary>
        public FakeDependentEntity FakeDependentEntity { get; private set; }

        /// <summary>
        /// Gets the fake dependent entity ID.
        /// </summary>
        public int? FakeDependentEntityId
        {
            get
            {
                // If FakeDependentEntityId hasn't been set, then use FakeComplexEntityId which is the same.
                return this.FakeDependentEntity == null ? null : this.FakeDependentEntity.FakeDependentEntityId ?? this.FakeComplexEntityId;
            }
        }

        #region Equality Methods and Operators

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
        public static bool operator ==(FakeComplexEntity valueA, FakeComplexEntity valueB)
        {
            return EqualityComparer<FakeComplexEntity>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(FakeComplexEntity valueA, FakeComplexEntity valueB)
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
        public bool Equals(FakeComplexEntity other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        #endregion

        /// <summary>
        /// Sets the dependent entity.
        /// </summary>
        /// <param name="someIntegerValue">
        /// The some integer value.
        /// </param>
        /// <returns>
        /// The <see cref="FakeDependentEntity"/>.
        /// </returns>
        public FakeDependentEntity SetDependentEntity(int someIntegerValue)
        {
            return this.SetDependentEntity(someIntegerValue, DateTimeOffset.Now);
        }

        /// <summary>
        /// Sets the dependent entity.
        /// </summary>
        /// <param name="someIntegerValue">
        /// The some integer value.
        /// </param>
        /// <param name="someTimeValue">
        /// The some time value.
        /// </param>
        /// <returns>
        /// The <see cref="FakeDependentEntity"/>.
        /// </returns>
        public FakeDependentEntity SetDependentEntity(int someIntegerValue, DateTimeOffset someTimeValue)
        {
            if (someIntegerValue == 0)
            {
                this.FakeDependentEntity = null;
            }
            else
            {
                this.FakeDependentEntity = new FakeDependentEntity(this.FakeComplexEntityId)
                                               {
                                                   DependentIntegerValue = someIntegerValue,
                                                   DependentTimeValue = someTimeValue
                                               };
            }

            return this.FakeDependentEntity;
        }

        /// <summary>
        /// Loads the children of the complex entity.
        /// </summary>
        /// <param name="items">
        /// The items to load.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items"/> is null.
        /// </exception>
        public void Load([NotNull] IEnumerable<FakeChildEntity> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            this.childEntities.Clear();
            this.childEntities.AddRange(items);
        }

        /// <summary>
        /// Adds a child entity.
        /// </summary>
        /// <param name="childEntity">
        /// The child entity to add.
        /// </param>
        public void AddChildEntity([NotNull] FakeChildEntity childEntity)
        {
            if (childEntity == null)
            {
                throw new ArgumentNullException(nameof(childEntity));
            }

            this.childEntities.Add(childEntity);
        }
    }
}