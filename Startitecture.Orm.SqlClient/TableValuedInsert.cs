// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableValuedInsert.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   A command for inserting multiple rows into a SQL Server table using a table valued parameter (TVP).
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Model;

    /// <summary>
    /// A command for inserting multiple rows into a SQL Server table using a table valued parameter (TVP).
    /// </summary>
    /// <typeparam name="T">
    /// The type of structure that is the source of the command data.
    /// </typeparam>
    public class TableValuedInsert<T> : TransactSqlInsertBase<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableValuedInsert{T}"/> class.
        /// </summary>
        /// <param name="databaseContext">
        /// The database context.
        /// </param>
        public TableValuedInsert([NotNull] IDatabaseContext databaseContext)
            : base(Singleton<TableValuedParameterCommandFactory>.Instance, databaseContext)
        {
        }

        /// <inheritdoc />
        protected override string DeclareSourceTable(string parameterName, IEnumerable<EntityAttributeDefinition> sourceAttributes)
        {
            return string.Empty; // Already declared in the parameter
        }

        /// <inheritdoc />
        protected override string DeclareInsertedTable([NotNull] IEnumerable<EntityAttributeDefinition> insertedAttributes)
        {
            if (insertedAttributes == null)
            {
                throw new ArgumentNullException(nameof(insertedAttributes));
            }

            var insertColumns = insertedAttributes.Select(
                definition => $"{this.NameQualifier.Escape(definition.PhysicalName)} {definition.PropertyInfo.GetSqlType()}");

            return $"DECLARE {this.NameQualifier.AddParameterPrefix("inserted")} table({string.Join(", ", insertColumns)});";
        }

        /// <inheritdoc />
        protected override string SourceSelection(
            string parameterName,
            [NotNull] IReadOnlyDictionary<EntityAttributeDefinition, EntityAttributeDefinition> matchedAttributes)
        {
            if (matchedAttributes == null)
            {
                throw new ArgumentNullException(nameof(matchedAttributes));
            }

            var sourceColumns = matchedAttributes.Values.Select(x => this.NameQualifier.Escape(x.PropertyName));
            return $"SELECT {string.Join(", ", sourceColumns)} FROM {this.NameQualifier.AddParameterPrefix(parameterName)} AS source";
        }

        /// <inheritdoc />
        protected override string SourceTableReference(string parameterName, IEnumerable<EntityAttributeDefinition> sourceAttributes)
        {
            return this.NameQualifier.AddParameterPrefix(parameterName);
        }
    }
}