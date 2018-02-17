// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityReference.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using System;

    /// <summary>
    /// The related entity reference.
    /// </summary>
    public class EntityReference
    {
        /// <summary>
        /// Gets or sets the container type.
        /// </summary>
        public Type ContainerType { get; set; }

        /// <summary>
        /// Gets or sets the entity alias.
        /// </summary>
        public string EntityAlias { get; set; }

        /// <summary>
        /// Gets or sets the entity type.
        /// </summary>
        public Type EntityType { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return $"{(this.ContainerType ?? this.EntityType)?.FullName}({this.EntityType?.FullName}:[{this.EntityAlias}])";
        }
    }
}