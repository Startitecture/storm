// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactSqlQualifier.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.SqlClient
{
    using System;

    using Model;

    using Startitecture.Resources;

    /// <summary>
    /// Qualifies an <see cref="Startitecture.Orm.Model.EntityAttributeDefinition"/> for Transact-SQL.
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
    }
}