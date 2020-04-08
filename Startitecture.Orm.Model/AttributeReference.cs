// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttributeReference.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using System.Reflection;

    /// <summary>
    /// The attribute reference.
    /// </summary>
    public class AttributeReference
    {
        /// <summary>
        /// Gets or sets the entity reference.
        /// </summary>
        public EntityReference EntityReference { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the physical name.
        /// </summary>
        public string PhysicalName { get; set; }

        /// <summary>
        /// Gets or sets the property info for the attribute.
        /// </summary>
        public PropertyInfo PropertyInfo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the attribute is a primary key.
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the attribute is an identity attribute.
        /// </summary>
        public bool IsIdentity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the attribute is a related attribute.
        /// </summary>
        public bool IsRelatedAttribute { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the attribute is a relation.
        /// </summary>
        public bool IsRelation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to ignore the reference.
        /// </summary>
        public bool IgnoreReference { get; set; }
    }
}