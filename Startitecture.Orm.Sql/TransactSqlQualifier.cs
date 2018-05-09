// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactSqlQualifier.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Sql
{
    using Startitecture.Orm.Model;

    /// <summary>
    /// Qualifies an <see cref="EntityAttributeDefinition"/> for Transact-SQL.
    /// </summary>
    public class TransactSqlQualifier : INameQualifier
    {
        /// <inheritdoc />
        public string Qualify(EntityAttributeDefinition attribute)
        {
            var entityQualifiedName = string.IsNullOrWhiteSpace(attribute.Alias)
                                          ? attribute.Entity.ReferenceName
                                          : string.Concat('[', attribute.Alias, ']');

            return string.Concat(entityQualifiedName, '.', '[', attribute.PhysicalName, ']');
        }

        /// <inheritdoc />
        public string GetCanonicalName(EntityAttributeDefinition attribute)
        {
            return string.Concat(this.GetCanonicalName(attribute.Entity), '.', '[', attribute.PhysicalName, ']');
        }

        /// <inheritdoc />
        public string GetCanonicalName(EntityLocation location)
        {
            return string.Concat('[', location.Container, ']', '.', '[', location.Name, ']');
        }
    }
}