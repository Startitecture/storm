// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NameQualifier.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common
{
    using System;

    using Startitecture.Orm.Model;
    using Startitecture.Resources;

    /// <summary>
    /// Qualifies names and identifiers for entity and attributes.
    /// </summary>
    public class NameQualifier : INameQualifier
    {
        /// <inheritdoc />
        public virtual string Escape(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(identifier));
            }

            return $"[{identifier}]";
        }

        /// <inheritdoc />
        public string Qualify(EntityAttributeDefinition attribute)
        {
            return this.Qualify(attribute, attribute.Entity);
        }

        /// <inheritdoc />
        public string Qualify(EntityAttributeDefinition attribute, EntityLocation entityLocation)
        {
            return $"{this.GetEntityCanonicalName(entityLocation)}.{this.GetAttributeCanonicalName(attribute)}";
        }

        /// <inheritdoc />
        public string GetReferenceName(EntityAttributeDefinition attribute)
        {
            return $"{this.GetEntityCanonicalName(attribute.Entity)}.{this.Escape(attribute.PhysicalName)}";
        }

        /// <inheritdoc />
        public string GetCanonicalName(EntityAttributeDefinition attribute)
        {
            return $"{this.GetPhysicalName(attribute.Entity)}.{this.GetAttributeCanonicalName(attribute)}";
        }

        /// <inheritdoc />
        public string GetPhysicalName(EntityLocation location)
        {
            return $"{this.Escape(location.Container)}.{this.Escape(location.Name)}";
        }

        /// <summary>
        /// Gets the entity qualified name.
        /// </summary>
        /// <param name="entityLocation">
        /// The entity location.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetEntityCanonicalName(EntityLocation entityLocation)
        {
            return string.IsNullOrWhiteSpace(entityLocation.Alias)
                       ? $"{this.Escape(entityLocation.Container)}.{this.Escape(entityLocation.Name)}"
                       : this.Escape(entityLocation.Alias);
        }

        /// <summary>
        /// Gets the canonical attribute name.
        /// </summary>
        /// <param name="attribute">
        /// The attribute to get the name for.
        /// </param>
        /// <returns>
        /// The physical name of the attribute, or the alias, if the attribute is aliased. <see cref="string"/>.
        /// </returns>
        private string GetAttributeCanonicalName(EntityAttributeDefinition attribute)
        {
            return this.Escape(attribute.Alias ?? attribute.PhysicalName);
        }
    }
}