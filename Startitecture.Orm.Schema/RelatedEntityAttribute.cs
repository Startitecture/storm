// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RelatedEntityAttribute.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Marks a property as a related attribute from another table.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Schema
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// Marks a property as a related attribute from another table.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class RelatedEntityAttribute : Attribute
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RelatedEntityAttribute"/> class.
        /// </summary>
        /// <param name="entityType">
        /// The entity type.
        /// </param>
        public RelatedEntityAttribute(Type entityType)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException(nameof(entityType));
            }

            this.EntityType = entityType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelatedEntityAttribute"/> class.
        /// </summary>
        /// <param name="entityType">
        /// The entity type of the related entity.
        /// </param>
        /// <param name="entityAlias">
        /// The entity alias. This should match the alias used for all JOIN operations.
        /// </param>
        public RelatedEntityAttribute([NotNull] Type entityType, string entityAlias)
            : this(entityType)
        {
            this.EntityAlias = entityAlias;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the entity type to which the attribute is related.
        /// </summary>
        public Type EntityType { get; }

        /// <summary>
        /// Gets the entity alias for the attribute, if any.
        /// </summary>
        public string EntityAlias { get; }

        /// <summary>
        /// Gets or sets the physical name of the associated column, if different from the property name.
        /// </summary>
        public string PhysicalName { get; set; }

        #endregion
    }
}