// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonMerge.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Startitecture.Orm.Common;
    using Startitecture.Orm.Model;

    /// <summary>
    /// Merges rows into a table using JSON.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the entity to merge the rows into.
    /// </typeparam>
    public class JsonMerge<T> : TransactSqlMergeBase<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonMerge{T}" /> class.
        /// </summary>
        /// <param name="tableCommandFactory">
        /// The table command factory for the merge.
        /// </param>
        /// <param name="databaseContext">
        /// The database context for the merge.
        /// </param>
        public JsonMerge(IDbTableCommandFactory tableCommandFactory, IDatabaseContext databaseContext)
            : base(tableCommandFactory, databaseContext)
        {
        }

        /// <inheritdoc />
        protected override string DeclareSourceTable(IEnumerable<EntityAttributeDefinition> sourceAttributes)
        {
            if (sourceAttributes == null)
            {
                throw new ArgumentNullException(nameof(sourceAttributes));
            }

            var commandBuilder = new StringBuilder();
            var sourceAttributeList = sourceAttributes.ToList();
            var sourceColumns = sourceAttributeList.Select(definition => this.NameQualifier.Escape(definition.PropertyName)).ToList();
            var sourceColumnDeclarations = sourceAttributeList.Select(
                definition => $"{this.NameQualifier.Escape(definition.PropertyName)} {definition.PropertyInfo.GetSqlType()}");

            var parameterName = this.NameQualifier.AddParameterPrefix("sourceRows");
            commandBuilder.AppendLine(
                $"DECLARE {parameterName} table({string.Join(", ", sourceColumnDeclarations)});");

            commandBuilder.AppendLine($"INSERT INTO {parameterName} ({string.Join(", ", sourceColumns)})");
            commandBuilder.AppendLine(
                $"SELECT {string.Join(", ", sourceColumns)} FROM OPENJSON({this.NameQualifier.AddParameterPrefix(this.ParameterName)})");

            var jsonColumns = from definition in sourceAttributeList
                              select $"{this.NameQualifier.Escape(definition.PropertyName)} "
                                     + $"{definition.PropertyInfo.GetSqlType()} '$.{definition.PropertyName}'";

            commandBuilder.Append($"WITH ({string.Join(", ", jsonColumns)});");
            return commandBuilder.ToString();
        }

        /// <inheritdoc />
        protected override string DeclareInsertedTable(IEnumerable<EntityAttributeDefinition> insertedAttributes)
        {
            // TODO: This is the same everywhere, so pull it up into the base class.
            if (insertedAttributes == null)
            {
                throw new ArgumentNullException(nameof(insertedAttributes));
            }

            var insertColumns = insertedAttributes.Select(
                definition => $"{this.NameQualifier.Escape(definition.PhysicalName)} {definition.PropertyInfo.GetSqlType()}");

            return $"DECLARE {this.NameQualifier.AddParameterPrefix("inserted")} table({string.Join(", ", insertColumns)});";
        }

        /// <inheritdoc />
        protected override string SourceSelection(Dictionary<EntityAttributeDefinition, EntityAttributeDefinition> matchedAttributes)
        {
            if (matchedAttributes == null)
            {
                throw new ArgumentNullException(nameof(matchedAttributes));
            }

            var sourceColumns = matchedAttributes.Values.Select(x => this.NameQualifier.Escape(x.PropertyName));
            return $"{this.NameQualifier.AddParameterPrefix("sourceRows")}";
        }
    }
}