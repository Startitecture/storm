// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Mapper
{
    using System;
    using System.Linq;

    using JetBrains.Annotations;

    using Startitecture.Orm.Model;
    using Startitecture.Orm.Schema;

    /// <summary>
    /// The extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Gets the table info from the generic type.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        /// <returns>
        /// The <see cref="Startitecture.Orm.Schema.TableInfo"/> for the specified type.
        /// </returns>
        public static TableInfo ToTableInfo([NotNull] this IEntityDefinition definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            // Get the table name
            var tableName = $"[{definition.EntityContainer}].[{definition.EntityName}]";

            // Get the primary key
            var primaryKeyDefinition = definition.PrimaryKeyAttributes.FirstOrDefault();

            ////var primaryKeyAttribute = pocoType.GetCustomAttributes(typeof(PrimaryKeyAttribute), true).OfType<PrimaryKeyAttribute>().FirstOrDefault();
            // TODO: Find another way to deal with Oracle or just forget it.
            ////var sequenceName = primaryKeyAttribute?.SequenceName;
            var primaryKey = primaryKeyDefinition.ReferenceName;
            var autoIncrement = primaryKeyDefinition.IsIdentityColumn;

            return new TableInfo(tableName, null)
                       {
                           AutoIncrement = autoIncrement,
                           PrimaryKey = primaryKey
                       };
        }
    }
}