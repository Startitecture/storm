// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableValuedInsert.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Mapper;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Schema;
    using Startitecture.Resources;

    /// <summary>
    /// The structured insert command.
    /// </summary>
    /// <typeparam name="TStructure">
    /// The type of structure that is the source of the command data.
    /// </typeparam>
    public class TableValuedInsert<TStructure> : StructuredCommand<TStructure>
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
        /// The entity definition.
        /// </summary>
        private IEntityDefinition itemDefinition;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableValuedInsert{TStructure}"/> class.
        /// </summary>
        /// <param name="structuredCommandProvider">
        /// The structured command provider.
        /// </param>
        public TableValuedInsert([NotNull] IStructuredCommandProvider structuredCommandProvider)
            : base(structuredCommandProvider)
        {
            this.commandText = new Lazy<string>(this.CompileCommandText);
            this.nameQualifier = this.StructuredCommandProvider.DatabaseContext.RepositoryAdapter.NameQualifier;
            this.StructureTypeName = GetTableTypeAttribute().TypeName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableValuedInsert{TStructure}"/> class.
        /// </summary>
        /// <param name="structuredCommandProvider">
        /// The structured command provider.
        /// </param>
        /// <param name="databaseTransaction">
        /// The database transaction.
        /// </param>
        public TableValuedInsert([NotNull] IStructuredCommandProvider structuredCommandProvider, IDbTransaction databaseTransaction)
            : base(structuredCommandProvider, databaseTransaction)
        {
            this.commandText = new Lazy<string>(this.CompileCommandText);
            this.nameQualifier = this.StructuredCommandProvider.DatabaseContext.RepositoryAdapter.NameQualifier;
            this.StructureTypeName = GetTableTypeAttribute().TypeName;
        }

        /// <inheritdoc />
        public override string StructureTypeName { get; }

        /// <inheritdoc />
        public override string CommandText => this.commandText.Value;

        /// <summary>
        /// Declares the table to insert into.
        /// </summary>
        /// <param name="insertItems">
        /// The entities to insert.
        /// </param>
        /// <param name="targetColumns">
        /// The target columns of the insert table.
        /// </param>
        /// <typeparam name="TEntity">
        /// The type of entity to insert the table into.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="TableValuedInsert{TStructure}"/>.
        /// </returns>
        public TableValuedInsert<TStructure> InsertInto<TEntity>(
            [NotNull] IEnumerable<TStructure> insertItems,
            params Expression<Func<TEntity, object>>[] targetColumns)
        {
            if (insertItems == null)
            {
                throw new ArgumentNullException(nameof(insertItems));
            }

            this.itemDefinition = this.StructuredCommandProvider.DatabaseContext.RepositoryAdapter.DefinitionProvider.Resolve<TEntity>();
            this.Items.Clear();
            this.Items.AddRange(insertItems);
            this.insertColumnExpressions.Clear();
            this.insertColumnExpressions.AddRange(targetColumns);
            return this;
        }

        /// <summary>
        /// Specifies the columns to select into the table.
        /// </summary>
        /// <param name="fromColumns">
        /// The columns to select into the table. These must be the same number of columns as target columns..
        /// </param>
        /// <returns>
        /// The current <see cref="TableValuedInsert{TStructure}"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="fromColumns"/> is null.
        /// </exception>
        public TableValuedInsert<TStructure> From([NotNull] params Expression<Func<TStructure, object>>[] fromColumns)
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
        public TableValuedInsert<TStructure> SelectResults(params Expression<Func<TStructure, object>>[] matchProperties)
        {
            var structureDefinition = this.StructuredCommandProvider.DatabaseContext.RepositoryAdapter.DefinitionProvider.Resolve<TStructure>();
            this.selectionAttributes.Clear();
            this.selectionAttributes.AddRange(matchProperties.Select(structureDefinition.Find));
            return this;
        }

        /// <summary>
        /// Gets the table type attribute for the current command.
        /// </summary>
        /// <returns>
        /// The <see cref="TableTypeAttribute"/> for the current command.
        /// </returns>
        /// <exception cref="OperationException">
        /// <typeparamref name="TStructure"/> is not decorated with a <see cref="TableTypeAttribute"/>
        /// </exception>
        private static TableTypeAttribute GetTableTypeAttribute()
        {
            var tableTypeAttribute = typeof(TStructure).GetCustomAttributes<TableTypeAttribute>().FirstOrDefault();

            if (tableTypeAttribute != null)
            {
                return tableTypeAttribute;
            }

            var message = string.Format(
                CultureInfo.CurrentCulture,
                ErrorMessages.AttributeRequiredForType,
                typeof(TStructure),
                nameof(TableTypeAttribute));

            throw new OperationException(typeof(TStructure), message);
        }

        /// <summary>
        /// Creates the output for an output table.
        /// </summary>
        /// <param name="structureDefinition">
        /// The structure definition of the table valued parameter.
        /// </param>
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
            IEntityDefinition structureDefinition,
            IReadOnlyCollection<EntityAttributeDefinition> directAttributes,
            StringBuilder commandBuilder,
            bool terminateStatement)
        {
            var outputAttributes = from tvpAttribute in structureDefinition.AllAttributes
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
            var structureDefinition = this.StructuredCommandProvider.DatabaseContext.RepositoryAdapter.DefinitionProvider.Resolve<TStructure>();
            var directAttributes = this.itemDefinition.DirectAttributes.ToList();
            var insertAttributes = (this.insertColumnExpressions.Any() ? this.insertColumnExpressions.Select(this.itemDefinition.Find) :
                                    this.itemDefinition.DirectAttributes.Any(x => x.IsIdentityColumn) ? this.itemDefinition.UpdateableAttributes :
                                    directAttributes).ToList();

            var targetColumns = insertAttributes.OrderBy(x => x.Ordinal).Select(x => this.nameQualifier.Escape(x.PhysicalName));

            var sourceAttributes = (this.fromColumnExpressions.Any()
                                        ? this.fromColumnExpressions.Select(structureDefinition.Find)
                                        : from tvpAttribute in structureDefinition.AllAttributes
                                          join insertAttribute in insertAttributes on tvpAttribute.PhysicalName equals insertAttribute.PhysicalName
                                          orderby tvpAttribute.Ordinal
                                          select tvpAttribute).ToList();

            var sourceColumns = sourceAttributes.Select(x => this.nameQualifier.Escape(x.PropertyName));

            var commandBuilder = new StringBuilder();

            var selectResults = this.selectionAttributes.Any();

            if (selectResults)
            {
                commandBuilder.AppendLine($"DECLARE @inserted {this.StructureTypeName};");
            }

            commandBuilder.AppendLine(
                    $"INSERT INTO {this.nameQualifier.Escape(this.itemDefinition.EntityContainer)}.{this.nameQualifier.Escape(this.itemDefinition.EntityName)}")
                .AppendLine($"({string.Join(", ", targetColumns)})");

            if (selectResults)
            {
                // In an INSERT we do not terminate this statement with a semi-colon. However in a MERGE we would do that.
                this.CreateOutput(structureDefinition, directAttributes, commandBuilder, false);
            }

            commandBuilder.AppendLine($"SELECT {string.Join(", ", sourceColumns)} FROM @{this.Parameter} AS tvp;");

            if (selectResults)
            {
                this.SelectOutput(structureDefinition, commandBuilder);
            }

            var compileCommandText = commandBuilder.ToString();
            return compileCommandText;
        }

        /// <summary>
        /// Selects the output from an inserted table variable.
        /// </summary>
        /// <param name="structureDefinition">
        /// The structure definition of the table valued parameter.
        /// </param>
        /// <param name="commandBuilder">
        /// The command builder to update.
        /// </param>
        private void SelectOutput(IEntityDefinition structureDefinition, StringBuilder commandBuilder)
        {
            var keyAttributes = (from key in this.itemDefinition.AllAttributes.Where(x => x.IsPrimaryKey)
                                 join fk in structureDefinition.AllAttributes on key.PhysicalName equals fk.PhysicalName
                                 select new { TargetKey = key, SourceKey = fk }).ToList();

            var matchAttributes = (from key in this.selectionAttributes
                                   join fk in structureDefinition.AllAttributes on key.PhysicalName equals fk.PhysicalName
                                   select new { TargetKey = key, SourceKey = fk }).ToList();

            var selectionJoinMatchColumns =
                (matchAttributes.Any() ? matchAttributes : keyAttributes).Select(
                    x => $"i.{this.nameQualifier.Escape(x.SourceKey.PropertyName)} = tvp.{this.nameQualifier.Escape(x.SourceKey.PropertyName)}");

            // For our selection from the @inserted table, we need to get the key from the insert and everything else from the original
            // table valued parameter.
            var selectedKeyAttributes =
                keyAttributes.Select(x => new { Column = $"i.{this.nameQualifier.Escape(x.SourceKey.PropertyName)}", Attribute = x.SourceKey }).ToList();

            // Everything for selecting from the TVP uses property name in order to match UDTT columns.
            var nonKeyAttributes = structureDefinition.AllAttributes.Except(selectedKeyAttributes.Select(x => x.Attribute)).Select(
                x => new
                         {
                             Column = $"tvp.{this.nameQualifier.Escape(x.PropertyName)}",
                             Attribute = x
                         });

            var selectedColumns = selectedKeyAttributes.Union(nonKeyAttributes).OrderBy(x => x.Attribute.Ordinal).Select(x => x.Column);

            commandBuilder.AppendLine($"SELECT {string.Join(", ", selectedColumns)}")
                .AppendLine("FROM @inserted AS i")
                .AppendLine($"INNER JOIN @{this.Parameter} AS tvp")
                .AppendLine($"ON {string.Join(" AND ", selectionJoinMatchColumns)};");
        }
    }
}