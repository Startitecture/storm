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
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;

    using JetBrains.Annotations;

    using Startitecture.Orm.Common;
    using Startitecture.Orm.Mapper;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Schema;

    /// <summary>
    /// A command for inserting multiple rows into a SQL Server table using a table valued parameter (TVP).
    /// </summary>
    /// <typeparam name="T">
    /// The type of structure that is the source of the command data.
    /// </typeparam>
    public class TableValuedInsert<T> : TableCommand<T>
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
        /// Initializes a new instance of the <see cref="TableValuedInsert{T}"/> class.
        /// </summary>
        /// <param name="tableCommandFactory">
        /// The table command provider.
        /// </param>
        /// <param name="databaseContext">
        /// The database context.
        /// </param>
        public TableValuedInsert([NotNull] IDbTableCommandFactory tableCommandFactory, [NotNull] IDatabaseContext databaseContext)
            : base(tableCommandFactory, databaseContext)
        {
            this.commandText = new Lazy<string>(this.CompileCommandText);
            this.nameQualifier = this.DatabaseContext.RepositoryAdapter.NameQualifier;
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
        /// The current <see cref="TableValuedInsert{TStructure}"/>.
        /// </returns>
        public TableValuedInsert<T> InsertInto([NotNull] params Expression<Func<T, object>>[] targetColumns)
        {
            if (targetColumns == null)
            {
                throw new ArgumentNullException(nameof(targetColumns));
            }

            this.insertColumnExpressions.Clear();
            this.insertColumnExpressions.AddRange(targetColumns);
            return this;
        }

        /// <summary>
        /// Specifies the columns to select into the table.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of items that will be inserted into the table.
        /// </typeparam>
        /// <param name="fromColumns">
        /// The columns to select into the table. These must be the same number of columns as target columns..
        /// </param>
        /// <returns>
        /// The current <see cref="TableValuedInsert{TStructure}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="fromColumns"/> is null.
        /// </exception>
        public TableValuedInsert<T> From<TItem>([NotNull] params Expression<Func<TItem, object>>[] fromColumns)
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
        /// Select the results of the insert, using the specified match keys to link the inserted values with the original values.
        /// </summary>
        /// <param name="matchProperties">
        /// The match properties.
        /// </param>
        /// <returns>
        /// The current <see cref="TableValuedInsert{TStructure}"/>.
        /// </returns>
        public TableValuedInsert<T> SelectResults([NotNull] params Expression<Func<T, object>>[] matchProperties)
        {
            if (matchProperties == null)
            {
                throw new ArgumentNullException(nameof(matchProperties));
            }

            this.selectionAttributes.Clear();
            this.selectionAttributes.AddRange(matchProperties.Select(this.EntityDefinition.Find));
            return this;
        }

        /// <summary>
        /// Creates the output for an output table.
        /// </summary>
        /// <param name="directAttributes">
        /// The direct attributes.
        /// </param>
        /// <param name="commandBuilder">
        /// The command builder.
        /// </param>
        /// <param name="terminateStatement">
        /// A value indicating whether to terminate the statement.
        /// </param>
        private void CreateOutput(
            IReadOnlyCollection<EntityAttributeDefinition> directAttributes,
            StringBuilder commandBuilder,
            bool terminateStatement)
        {
            var outputAttributes = from tvpAttribute in this.ItemDefinition.AllAttributes
                                   join attribute in directAttributes on tvpAttribute.ResolvedLocation equals attribute.ResolvedLocation
                                   orderby tvpAttribute.Ordinal
                                   select tvpAttribute;

            var statementTerminator = terminateStatement ? ";" : string.Empty;

            var insertedColumns = outputAttributes.OrderBy(x => x.Ordinal).Select(x => this.nameQualifier.Escape(x.PropertyName)).ToList();
            var outputColumns = directAttributes.OrderBy(x => x.Ordinal).Select(x => $"INSERTED.{this.nameQualifier.Escape(x.PhysicalName)}");

            commandBuilder.AppendLine($"OUTPUT {string.Join(",", outputColumns)}")
                .AppendLine($"INTO @inserted ({string.Join(", ", insertedColumns)}){statementTerminator}");
        }

        /// <summary>
        /// The compile command text.
        /// </summary>
        /// <returns>
        /// The <see cref="string" />.
        /// </returns>
        private string CompileCommandText()
        {
            var directAttributes = this.EntityDefinition.DirectAttributes.ToList();
            var insertAttributes = (this.insertColumnExpressions.Any()
                                        ? this.insertColumnExpressions.Select(this.EntityDefinition.Find)
                                        : this.EntityDefinition.InsertableAttributes).ToList();

            var targetColumns = insertAttributes.OrderBy(x => x.Ordinal).Select(x => this.nameQualifier.Escape(x.PhysicalName));

            var sourceAttributes = (this.fromColumnExpressions.Any()
                                        ? this.fromColumnExpressions.Select(this.ItemDefinition.Find)
                                        : from tvpAttribute in this.ItemDefinition.AllAttributes
                                          join insertAttribute in insertAttributes on tvpAttribute.PhysicalName equals insertAttribute.PhysicalName
                                          orderby tvpAttribute.Ordinal
                                          select tvpAttribute).ToList();

            var sourceColumns = sourceAttributes.Select(x => this.nameQualifier.Escape(x.PropertyName));

            var commandBuilder = new StringBuilder();
            var selectResults = this.selectionAttributes.Any();
            var tableTypeName = this.ItemType.GetCustomAttribute<TableTypeAttribute>()?.TypeName ?? this.ItemType.Name;

            if (selectResults)
            {
                commandBuilder.AppendLine($"DECLARE @inserted {tableTypeName};");
            }

            commandBuilder.AppendLine(
                    $"INSERT INTO {this.nameQualifier.Escape(this.EntityDefinition.EntityContainer)}.{this.nameQualifier.Escape(this.EntityDefinition.EntityName)}")
                .AppendLine($"({string.Join(", ", targetColumns)})");

            if (selectResults)
            {
                // In an INSERT we do not terminate this statement with a semi-colon. However in a MERGE we would do that.
                this.CreateOutput(directAttributes, commandBuilder, false);
            }

            commandBuilder.AppendLine($"SELECT {string.Join(", ", sourceColumns)} FROM @{this.ParameterName} AS tvp;");

            if (selectResults)
            {
                this.SelectOutput(commandBuilder);
            }

            var compileCommandText = commandBuilder.ToString();
            return compileCommandText;
        }

        /// <summary>
        /// Selects the output from an inserted table variable.
        /// </summary>
        /// <param name="commandBuilder">
        /// The command builder to update.
        /// </param>
        private void SelectOutput(StringBuilder commandBuilder)
        {
            var keyAttributes = (from key in this.EntityDefinition.AllAttributes.Where(x => x.IsPrimaryKey)
                                 join fk in this.ItemDefinition.AllAttributes on key.PhysicalName equals fk.PhysicalName
                                 select new
                                 {
                                     TargetKey = key,
                                     SourceKey = fk
                                 }).ToList();

            var matchAttributes = (from key in this.selectionAttributes
                                   join fk in this.ItemDefinition.AllAttributes on key.PhysicalName equals fk.PhysicalName
                                   select new
                                   {
                                       TargetKey = key,
                                       SourceKey = fk
                                   }).ToList();

            var selectionJoinMatchColumns = (matchAttributes.Any() ? matchAttributes : keyAttributes).Select(
                x => $"i.{this.nameQualifier.Escape(x.SourceKey.PropertyName)} = tvp.{this.nameQualifier.Escape(x.SourceKey.PropertyName)}");

            // For our selection from the @inserted table, we need to get the key from the insert and everything else from the original
            // table valued parameter.
            var selectedKeyAttributes = keyAttributes.Select(
                    x => new
                    {
                        Column = $"i.{this.nameQualifier.Escape(x.SourceKey.PropertyName)}",
                        Attribute = x.SourceKey
                    })
                .ToList();

            // Everything for selecting from the TVP uses property name in order to match UDTT columns.
            var nonKeyAttributes = this.ItemDefinition.AllAttributes.Except(selectedKeyAttributes.Select(x => x.Attribute))
                .Select(
                    x => new
                    {
                        Column = $"tvp.{this.nameQualifier.Escape(x.PropertyName)}",
                        Attribute = x
                    });

            var selectedColumns = selectedKeyAttributes.Union(nonKeyAttributes).OrderBy(x => x.Attribute.Ordinal).Select(x => x.Column);

            commandBuilder.AppendLine($"SELECT {string.Join(", ", selectedColumns)}")
                .AppendLine("FROM @inserted AS i")
                .AppendLine($"INNER JOIN @{this.ParameterName} AS tvp")
                .AppendLine($"ON {string.Join(" AND ", selectionJoinMatchColumns)};");
        }
    }
}