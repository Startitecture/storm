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
            : this(entityType, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelatedEntityAttribute"/> class.
        /// </summary>
        /// <param name="entityType">
        /// The entity type of the related entity.
        /// </param>
        /// <param name="useAttributeAlias">
        /// A value indicating whether the value should be aliased using the convention {EntityName}{AttributeName}.
        /// </param>
        public RelatedEntityAttribute(Type entityType, bool useAttributeAlias)
            : this(entityType, useAttributeAlias, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelatedEntityAttribute"/> class.
        /// </summary>
        /// <param name="entityType">
        /// The entity type of the related entity.
        /// </param>
        /// <param name="useAttributeAlias">
        /// A value indicating whether the value should be aliased using the convention {EntityName}{AttributeName}.
        /// </param>
        /// <param name="entityAlias">
        /// The entity alias. This should match the alias used for all JOIN operations.
        /// </param>
        public RelatedEntityAttribute([NotNull] Type entityType, bool useAttributeAlias, string entityAlias)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException(nameof(entityType));
            }

            this.EntityType = entityType;
            this.UseAttributeAlias = useAttributeAlias;
            this.EntityAlias = entityAlias;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the entity type to which the attribute is related.
        /// </summary>
        public Type EntityType { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the attribute should be aliased.
        /// </summary>
        public bool UseAttributeAlias { get; private set; }

        /// <summary>
        /// Gets the entity alias for the attribute, if any.
        /// </summary>
        public string EntityAlias { get; private set; }

        /// <summary>
        /// Gets or sets the physical name of the associated column, if different from the property name.
        /// </summary>
        public string PhysicalName { get; set; }

        #endregion
    }
}