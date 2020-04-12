// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactSqlQualifier.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Sql
{
    using System;

    using JetBrains.Annotations;

    using Startitecture.Orm.Model;
    using Startitecture.Resources;

    /// <summary>
    /// Qualifies an <see cref="EntityAttributeDefinition"/> for Transact-SQL.
    /// </summary>
    public class TransactSqlQualifier : INameQualifier
    {
        /// <inheritdoc />
        public string Escape(string identifier)
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
            var entityQualifiedName = string.IsNullOrWhiteSpace(entityLocation.Alias)
                                          ? $"[{entityLocation.Container}].[{entityLocation.Name}]"
                                          : string.Concat('[', entityLocation.Alias, ']');

            return string.Concat(entityQualifiedName, '.', '[', attribute.Alias ?? attribute.PhysicalName, ']');
        }

        /// <inheritdoc />
        public string GetCanonicalName(EntityAttributeDefinition attribute)
        {
            return string.Concat(this.GetCanonicalName(attribute.Entity), '.', '[', attribute.Alias ?? attribute.PhysicalName, ']');
        }

        /// <inheritdoc />
        public string GetCanonicalName(EntityLocation location)
        {
            return string.Concat('[', location.Container, ']', '.', '[', location.Name, ']');
        }

        /// <inheritdoc />
        public string GetReferenceName(EntityAttributeDefinition attribute)
        {
            return $"{this.GetReferenceName(attribute.Entity)}.[{attribute.PhysicalName}]";
        }

        /// <inheritdoc />
        public string GetReferenceName(EntityLocation location)
        {
            var isEntityAliased = string.IsNullOrWhiteSpace(location.Alias) == false;
            return isEntityAliased
                       ? string.Concat('[', location.Alias, ']')
                       : string.Concat('[', location.Container, ']', '.', '[', location.Name, ']');
        }
    }
}