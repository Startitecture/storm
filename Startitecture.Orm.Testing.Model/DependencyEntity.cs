// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyEntity.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model
{
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
        /// <param name="complexEntityId">
        /// The fake complex entity id.
        /// </param>
        public DependencyEntity(string uniqueName, int? complexEntityId)
            : this(uniqueName, complexEntityId, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyEntity"/> class.
        /// </summary>
        /// <param name="uniqueName">
        /// The unique name.
        /// </param>
        /// <param name="complexEntityId">
        /// The fake complex entity id.
        /// </param>
        /// <param name="fakeDependencyEntityId">
        /// The fake dependency entity id.
        /// </param>
        public DependencyEntity(string uniqueName, int? complexEntityId, long? fakeDependencyEntityId)
        {
            this.UniqueName = uniqueName;
            this.ComplexEntityId = complexEntityId;
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
        public int? ComplexEntityId { get; private set; }

        /// <summary>
        /// Gets the unique name.
        /// </summary>
        public string UniqueName { get; private set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }
    }
}
