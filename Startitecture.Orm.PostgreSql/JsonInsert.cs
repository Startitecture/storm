// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonInsert.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Creates command text for inserting JSON objects into PostgreSQL tables.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.PostgreSql
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;

    using JetBrains.Annotations;

    using Startitecture.Orm.Common;
    using Startitecture.Orm.Mapper;
    using Startitecture.Orm.Model;

    /// <summary>
    /// Creates command text for inserting JSON objects into PostgreSQL tables.
    /// </summary>
    /// <typeparam name="T">
    /// The type of item to insert into the repository.
    /// </typeparam>
    public class JsonInsert<T> : TableCommand<T>
    {
        /// <summary>
        /// The command text.
        /// </summary>
        private readonly Lazy<string> commandText;

        /// <summary>
        /// The match expressions.
        /// </summary>
        private readonly List<EntityAttributeDefinition> selectionAttributes = new List<EntityAttributeDefinition>();

        /// <summary>
        /// The name qualifier.
        /// </summary>
        private readonly INameQualifier nameQualifier;

        /// <summary>
        /// The values expression.
        /// </summary>
        private readonly List<LambdaExpression> insertColumnExpressions = new List<LambdaExpression>();

        /// <summary>
        /// The from expressions.
        /// </summary>
        private readonly List<LambdaExpression> fromColumnExpressions = new List<LambdaExpression>();

        /// <summary>
        /// The constraint hint expressions.
        /// </summary>
        private readonly List<LambdaExpression> constraintHintExpressions = new List<LambdaExpression>();

        /// <summary>
        /// The update expressions.
        /// </summary>
        private readonly List<LambdaExpression> updateColumnExpressions = new List<LambdaExpression>();

        /// <summary>
        /// The insert conflict action.
        /// </summary>
        private InsertConflictAction insertConflictAction = InsertConflictAction.RaiseConstraintViolation;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonInsert{T}"/> class.
        /// </summary>
        /// <param name="tableCommandProvider">
        /// The structured command provider.
        /// </param>
        public JsonInsert([NotNull] ITableCommandProvider tableCommandProvider)
            : base(tableCommandProvider)
        {
            this.commandText = new Lazy<string>(this.CompileCommandText);
            this.nameQualifier = this.TableCommandProvider.DatabaseContext.RepositoryAdapter.NameQualifier;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonInsert{T}"/> class.
        /// </summary>
        /// <param name="tableCommandProvider">
        /// The structured command provider.
        /// </param>
        /// <param name="databaseTransaction">
        /// The database transaction.
        /// </param>
        public JsonInsert([NotNull] ITableCommandProvider tableCommandProvider, IDbTransaction databaseTransaction)
            : base(tableCommandProvider, databaseTransaction)
        {
            this.commandText = new Lazy<string>(this.CompileCommandText);
            this.nameQualifier = this.TableCommandProvider.DatabaseContext.RepositoryAdapter.NameQualifier;
        }

        /// <inheritdoc />
        public override string CommandText => this.commandText.Value;

        /// <summary>
        /// Declares the table to insert into.
        /// </summary>
        /// <param name="targetColumns">
        /// The target columns of the insert table.
        /// </param>
        /// <returns>
        /// The current <see cref="JsonInsert{T}"/>.
        /// </returns>
        public JsonInsert<T> InsertInto(params Expression<Func<T, object>>[] targetColumns)
        {
            this.insertColumnExpressions.Clear();
            this.insertColumnExpressions.AddRange(targetColumns);
            return this;
        }

        /// <summary>
        /// Specifies the columns to select into the table.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of items to insert into the table.
        /// </typeparam>
        /// <param name="fromColumns">
        /// The columns to select into the table. These must be the same number of columns as target columns..
        /// </param>
        /// <returns>
        /// The current <see cref="JsonInsert{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="fromColumns"/> is null.
        /// </exception>
        public JsonInsert<T> From<TItem>([NotNull] params Expression<Func<TItem, object>>[] fromColumns)
        {
            if (fromColumns == null)
            {
                throw new ArgumentNullException(nameof(fromColumns));
            }

            this.fromColumnExpressions.Clear();
            this.fromColumnExpressions.AddRange(fromColumns);
            return this;
        }

        /// <summary>
        /// Specifies that on a constraint violation, the row will not be inserted and no error will be raised.
        /// </summary>
        /// <returns>
        /// The current <see cref="JsonInsert{T}"/>.
        /// </returns>
        public JsonInsert<T> OnConflictDoNothing()
        {
            this.insertConflictAction = InsertConflictAction.DoNothing;
            return this;
        }

        /// <summary>
        /// Specify the attributes to update when there is a constraint violation.
        /// </summary>
        /// <param name="constraintHintAttributes">
        /// The constraint hint attributes.
        /// </param>
        /// <returns>
        /// The current <see cref="JsonInsert{T}"/>.
        /// </returns>
        public JsonInsert<T> OnConflict([NotNull] params Expression<Func<T, object>>[] constraintHintAttributes)
        {
            if (constraintHintAttributes == null)
            {
                throw new ArgumentNullException(nameof(constraintHintAttributes));
            }

            this.insertConflictAction = InsertConflictAction.Update;
            this.constraintHintExpressions.Clear();
            this.constraintHintExpressions.AddRange(constraintHintAttributes);
            return this;
        }

        /// <summary>
        /// Defines the source attributes to upsert from. These should be in the same order as non-database generated
        /// columns in the target table.
        /// </summary>
        /// <param name="targetColumns">
        /// The update source attributes.
        /// </param>
        /// <returns>
        /// The current <see cref="JsonInsert{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="targetColumns"/> is null.
        /// </exception>
        public JsonInsert<T> Upsert([NotNull] params Expression<Func<T, object>>[] targetColumns)
        {
            if (targetColumns == null)
            {
                throw new ArgumentNullException(nameof(targetColumns));
            }

            this.insertConflictAction = InsertConflictAction.Update;
            this.updateColumnExpressions.Clear();
            this.updateColumnExpressions.AddRange(targetColumns);
            return this;
        }

        /// <summary>
        /// Select the results of the insert, using the specified match keys to link the inserted values with the original values.
        /// </summary>
        /// <param name="matchProperties">
        /// The match properties.
        /// </param>
        /// <returns>
        /// The current <see cref="JsonInsert{T}"/>.
        /// </returns>
        public JsonInsert<T> Returning(params Expression<Func<T, object>>[] matchProperties)
        {
            this.selectionAttributes.Clear();
            this.selectionAttributes.AddRange(matchProperties.Select(this.EntityDefinition.Find));
            return this;
        }

        /// <summary>
        /// The compile command text.
        /// </summary>
        /// <returns>
        /// The <see cref="string" />.
        /// </returns>
        private string CompileCommandText()
        {
            var insertAttributes = (this.insertColumnExpressions.Any()
                                        ? this.insertColumnExpressions.Select(this.EntityDefinition.Find)
                                        : this.EntityDefinition.InsertableAttributes).ToList();

            var targetColumns = insertAttributes.OrderBy(x => x.Ordinal).Select(x => this.nameQualifier.Escape(x.PhysicalName)).ToList();

            // Direct attributes if none are specified because this could be a raised POCO.
            var sourceAttributes = (this.fromColumnExpressions.Any()
                                        ? this.fromColumnExpressions.Select(this.ItemDefinition.Find)
                                        : from objectAttribute in this.ItemDefinition.DirectAttributes
                                          join insertAttribute in insertAttributes on objectAttribute.PhysicalName equals insertAttribute.PhysicalName
                                          orderby objectAttribute.Ordinal
                                          select objectAttribute).ToList();

            var sourceColumns = sourceAttributes.Select(x => $"t.{this.nameQualifier.Escape(x.PropertyName)}").ToList();
            var jsonProperties = sourceAttributes.Select(
                x => $"{this.nameQualifier.Escape(x.PropertyName)} {PostgresTypeLookup.GetType(x.PropertyInfo.PropertyType)}");

            var commandBuilder = new StringBuilder();

            var selectResults = this.selectionAttributes.Any();
            commandBuilder.AppendLine(
                    $"INSERT INTO {this.nameQualifier.Escape(this.EntityDefinition.EntityContainer)}.{this.nameQualifier.Escape(this.EntityDefinition.EntityName)}")
                .AppendLine($"({string.Join(", ", targetColumns)})");

            commandBuilder.Append($@"SELECT {string.Join(", ", sourceColumns)}
FROM jsonb_to_recordset({this.nameQualifier.AddParameterPrefix(this.ParameterName)}::jsonb) AS t ({string.Join(", ", jsonProperties)})");

            if (this.insertConflictAction != InsertConflictAction.RaiseConstraintViolation)
            {
                switch (this.insertConflictAction)
                {
                    case InsertConflictAction.DoNothing:
                        commandBuilder.AppendLine();
                        commandBuilder.Append("ON CONFLICT DO NOTHING");
                        break;
                    case InsertConflictAction.Update:
                        var conflictColumnHints = this.constraintHintExpressions.Select(this.EntityDefinition.Find)
                            .Select(definition => this.nameQualifier.Escape(definition.PhysicalName))
                            .ToList();

                        var primaryKeyColumns =
                            this.EntityDefinition.PrimaryKeyAttributes.Select(definition => this.nameQualifier.Escape(definition.PhysicalName));

                        var excludedColumns = this.updateColumnExpressions.Select(this.EntityDefinition.Find)
                            .Select(x => $"{this.nameQualifier.Escape(x.PhysicalName)} = EXCLUDED.{this.nameQualifier.Escape(x.PhysicalName)}")
                            .ToList();

                        ////var setColumnClauses = 
                            ////this.EntityDefinition.DirectAttributes.Where(definition => definition.IsIdentityColumn)
                            ////    .Select(id => $"{this.nameQualifier.Escape(id.PhysicalName)} = DEFAULT")
                            ////    .Union(targetColumns.Select((t, i) => $"{t} = {excludedColumns[i]}")).ToList();

                        commandBuilder.AppendLine()
                            .AppendLine($"ON CONFLICT ({string.Join(", ", conflictColumnHints.Any() ? conflictColumnHints : primaryKeyColumns)})")
                            .Append($"DO UPDATE SET {string.Join(", ", excludedColumns)}");

                        break;
                }
            }

            if (selectResults)
            {
                var selectionColumns = from c in this.selectionAttributes
                                       select $"{this.nameQualifier.Escape(c.PhysicalName)}";

                commandBuilder.AppendLine();
                commandBuilder.Append($"RETURNING {string.Join(", ", selectionColumns)}");
            }

            commandBuilder.AppendLine(";");
            var compileCommandText = commandBuilder.ToString();
            return compileCommandText;
        }
    }
}