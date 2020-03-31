// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyEntity.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model
{
    using System;

    using Core;

    using JetBrains.Annotations;

    using Startitecture.Resources;

    /// <summary>
    /// The fake dependency entity.
    /// </summary>
    public class DependencyEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyEntity"/> class.
        /// </summary>
        /// <param name="uniqueName">
        /// The unique name.
        /// </param>
        public DependencyEntity(string uniqueName)
            : this(uniqueName, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyEntity"/> class.
        /// </summary>
        /// <param name="uniqueName">
        /// The unique name.
        /// </param>
        /// <param name="fakeComplexEntityId">
        /// The fake complex entity id.
        /// </param>
        public DependencyEntity(string uniqueName, int? fakeComplexEntityId)
            : this(uniqueName, fakeComplexEntityId, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyEntity"/> class.
        /// </summary>
        /// <param name="uniqueName">
        /// The unique name.
        /// </param>
        /// <param name="fakeComplexEntityId">
        /// The fake complex entity id.
        /// </param>
        /// <param name="fakeDependencyEntityId">
        /// The fake dependency entity id.
        /// </param>
        public DependencyEntity(string uniqueName, int? fakeComplexEntityId, long? fakeDependencyEntityId)
        {
            this.UniqueName = uniqueName;
            this.FakeComplexEntityId = fakeComplexEntityId;
            this.FakeDependencyEntityId = fakeDependencyEntityId;
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="DependencyEntity"/> class from being created.
        /// </summary>
        private DependencyEntity()
        {
        }

        /// <summary>
        /// Gets the fake dependency entity id.
        /// </summary>
        public long? FakeDependencyEntityId { get; private set; }

        /// <summary>
        /// Gets the fake complex entity id.
        /// </summary>
        public int? FakeComplexEntityId { get; private set; }

        /// <summary>
        /// Gets the unique name.
        /// </summary>
        public string UniqueName { get; private set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The associate.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="entity"/> is null.
        /// </exception>
        /// <exception cref="Startitecture.Core.BusinessException">
        /// <see cref="ComplexEntity.FakeComplexEntityId"/> is not set in <paramref name="entity"/>.
        /// </exception>
        public void Associate([NotNull] ComplexEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (entity.FakeComplexEntityId.HasValue == false)
            {
                throw new BusinessException(entity, string.Format(ValidationMessages.PropertyMustBeSet, "FakeComplexEntityId"));
            }

            this.FakeComplexEntityId = entity.FakeComplexEntityId;
        }
    }
}
