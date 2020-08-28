// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttributeReference.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Represents a reference to an entity attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using System.Reflection;

    /// <summary>
    /// Represents a reference to an entity attribute.
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
        /// Gets or sets a value indicating whether the attribute is a related attribute.
        /// </summary>
        public bool IsRelatedAttribute { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the attribute is a relation.
        /// </summary>
        public bool IsRelation { get; set; }
    }
}