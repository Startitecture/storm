// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComplexEntity.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Startitecture.Orm.Testing.Model
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using Startitecture.Core;

    /// <summary>
    /// The fake complex entity.
    /// </summary>
    public class ComplexEntity : IEquatable<ComplexEntity>
    {
        /// <summary>
        /// The comparison properties.
        /// </summary>
        private static readonly Func<ComplexEntity, object>[] ComparisonProperties =
            {
                item => item.UniqueName,
                item => item.Description,
                item => item.DependentEntity,
                item => item.FakeEnumeration,
                item => item.FakeOtherEnumeration,
                item => item.SubEntity,
                item => item.SubSubEntity,
                item => item.CreationTime,
                item => item.CreatedBy,
                item => item.ModifiedTime,
                item => item.ModifiedBy
            };

        /// <summary>
        /// The child entities.
        /// </summary>
        private readonly List<ChildEntity> childEntities = new List<ChildEntity>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ComplexEntity" /> class.
        /// </summary>
        /// <param name="uniqueName">
        /// The unique name.
        /// </param>
        /// <param name="subEntity">
        /// The sub entity.
        /// </param>
        /// <param name="fakeEnumeration">
        /// The fake enumeration.
        /// </param>
        /// <param name="createdBy">
        /// The creation entity.
        /// </param>
        public ComplexEntity(string uniqueName, SubEntity subEntity, FakeEnumeration fakeEnumeration, CreatedBy createdBy)
            : this(uniqueName, subEntity, fakeEnumeration, createdBy, DateTimeOffset.Now)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComplexEntity" /> class.
        /// </summary>
        /// <param name="uniqueName">
        /// The unique name.
        /// </param>
        /// <param name="subEntity">
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
        public ComplexEntity(
            string uniqueName,
            SubEntity subEntity,
            FakeEnumeration fakeEnumeration,
            CreatedBy createdBy,
            DateTimeOffset creationTime)
            : this(uniqueName, subEntity, fakeEnumeration, createdBy, creationTime, 0)
        {
            this.ComplexEntityId = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComplexEntity" /> class.
        /// </summary>
        /// <param name="uniqueName">
        /// The unique name.
        /// </param>
        /// <param name="subEntity">
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
        /// <param name="complexEntityId">
        /// The fake complex entity id.
        /// </param>
        public ComplexEntity(
            string uniqueName,
            SubEntity subEntity,
            FakeEnumeration fakeEnumeration,
            CreatedBy createdBy,
            DateTimeOffset creationTime,
            int complexEntityId)
        {
            this.UniqueName = uniqueName;
            this.ComplexEntityId = complexEntityId;
            this.FakeEnumeration = fakeEnumeration;
            this.SubEntity = subEntity;
            this.CreationTime = creationTime;
            this.CreatedBy = createdBy;
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="ComplexEntity"/> class from being created.
        /// </summary>
        private ComplexEntity()
        {
        }

        /// <summary>
        /// Gets the child entities.
        /// </summary>
        public IEnumerable<ChildEntity> ChildEntities
        {
            get
            {
                return this.childEntities;
            }
        }

        /// <summary>
        /// Gets the creation entity.
        /// </summary>
        public CreatedBy CreatedBy { get; private set; }

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
        public int? ComplexEntityId { get; private set; }

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
        public SubEntity SubEntity { get; private set; }

        /// <summary>
        /// Gets the fake complex entity id.
        /// </summary>
        public int? FakeSubEntityId
        {
            get
            {
                return this.SubEntity == null ? null : this.SubEntity.FakeSubEntityId;
            }
        }

        /// <summary>
        /// Gets the sub sub entity.
        /// </summary>
        public SubSubEntity SubSubEntity
        {
            get
            {
                return this.SubEntity == null ? null : this.SubEntity.SubSubEntity;
            }
        }

        /// <summary>
        /// Gets the fake complex entity id.
        /// </summary>
        public int? FakeSubSubEntityId
        {
            get
            {
                return this.SubSubEntity == null ? null : this.SubSubEntity.FakeSubSubEntityId;
            }
        }

        /// <summary>
        /// Gets or sets the modified entity.
        /// </summary>
        public ModifiedBy ModifiedBy { get; set; }

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
        public DependentEntity DependentEntity { get; private set; }

        /// <summary>
        /// Gets the fake dependent entity ID.
        /// </summary>
        public int? FakeDependentEntityId
        {
            get
            {
                // If FakeDependentEntityId hasn't been set, then use ComplexEntityId which is the same.
                return this.DependentEntity == null ? null : this.DependentEntity.FakeDependentEntityId ?? this.ComplexEntityId;
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
        public static bool operator ==(ComplexEntity valueA, ComplexEntity valueB)
        {
            return EqualityComparer<ComplexEntity>.Default.Equals(valueA, valueB);
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
        public static bool operator !=(ComplexEntity valueA, ComplexEntity valueB)
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
        public bool Equals(ComplexEntity other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }

        /// <summary>
        /// Sets the dependent entity.
        /// </summary>
        /// <param name="someIntegerValue">
        /// The some integer value.
        /// </param>
        /// <returns>
        /// The <see cref="DependentEntity"/>.
        /// </returns>
        public DependentEntity SetDependentEntity(int someIntegerValue)
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
        /// The <see cref="DependentEntity"/>.
        /// </returns>
        public DependentEntity SetDependentEntity(int someIntegerValue, DateTimeOffset someTimeValue)
        {
            if (someIntegerValue == 0)
            {
                this.DependentEntity = null;
            }
            else
            {
                this.DependentEntity = new DependentEntity(this.ComplexEntityId)
                {
                    DependentIntegerValue = someIntegerValue,
                    DependentTimeValue = someTimeValue
                };
            }

            return this.DependentEntity;
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
        public void Load([NotNull] IEnumerable<ChildEntity> items)
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
        public void AddChildEntity([NotNull] ChildEntity childEntity)
        {
            if (childEntity == null)
            {
                throw new ArgumentNullException(nameof(childEntity));
            }

            this.childEntities.Add(childEntity);
        }
    }
}