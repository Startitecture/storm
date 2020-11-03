﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonInsert.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using JetBrains.Annotations;

    using Startitecture.Orm.Common;
    using Startitecture.Orm.Model;

    /// <summary>
    /// Builds a command to insert multiple rows using JSON.
    /// </summary>
    /// <typeparam name="T">
    /// The type that represents the JSON data.
    /// </typeparam>
    public class JsonInsert<T> : TransactSqlInsertBase<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonInsert{T}" /> class.
        /// </summary>
        /// <param name="tableCommandFactory">
        /// The table command factory.
        /// </param>
        /// <param name="databaseContext">
        /// The database context.
        /// </param>
        public JsonInsert([NotNull] IDbTableCommandFactory tableCommandFactory, IDatabaseContext databaseContext)
            : base(tableCommandFactory, databaseContext)
        {
        }

        /// <inheritdoc />
        protected override string DeclareSourceTable([NotNull] IEnumerable<EntityAttributeDefinition> sourceAttributes)
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
            [NotNull] IReadOnlyDictionary<EntityAttributeDefinition, EntityAttributeDefinition> matchedAttributes)
        {
            if (matchedAttributes == null)
            {
                throw new ArgumentNullException(nameof(matchedAttributes));
            }

            var sourceColumns = matchedAttributes.Values.Select(x => this.NameQualifier.Escape(x.PropertyName));
            return $"SELECT {string.Join(", ", sourceColumns)} FROM {this.NameQualifier.AddParameterPrefix("sourceRows")} AS source";
        }

        /// <inheritdoc />
        protected override string SourceTableReference(IEnumerable<EntityAttributeDefinition> sourceAttributes)
        {
            return this.NameQualifier.AddParameterPrefix("sourceRows");
        }
    }
}