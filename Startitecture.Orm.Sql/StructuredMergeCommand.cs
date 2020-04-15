// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructuredMergeCommand.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Sql
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;

    using JetBrains.Annotations;

    using Startitecture.Orm.Model;

    /// <summary>
    /// The structured merge command.
    /// </summary>
    /// <typeparam name="TStructure">
    /// The type of structure to use as the source of the merge.
    /// </typeparam>
    public class StructuredMergeCommand<TStructure> : StructuredCommand<TStructure>
    {
        /// <summary>
        /// The direct attributes.
        /// </summary>
        private readonly List<EntityAttributeDefinition> directAttributes = new List<EntityAttributeDefinition>();

        /// <summary>
        /// The insert attributes.
        /// </summary>
        private readonly List<EntityAttributeDefinition> insertAttributes = new List<EntityAttributeDefinition>();

        /// <summary>
        /// The primary key attributes.
        /// </summary>
        private readonly List<EntityAttributeDefinition> mergeMatchAttributes = new List<EntityAttributeDefinition>();

        /// <summary>
        /// The selection match attributes.
        /// </summary>
        private readonly List<EntityAttributeDefinition> selectionMatchAttributes = new List<EntityAttributeDefinition>();

        /// <summary>
        /// The delete constraints.
        /// </summary>
        private readonly List<Expression<Func<TStructure, object>>> deleteConstraints = new List<Expression<Func<TStructure, object>>>();

        /// <summary>
        /// The delete unmatched in source.
        /// </summary>
        private bool deleteUnmatchedInSource;

        /// <summary>
        /// The item definition.
        /// </summary>
        private IEntityDefinition itemDefinition;

        /// <summary>
        /// Initializes a new instance of the <see cref="StructuredMergeCommand{TStructure}"/> class.
        /// </summary>
        /// <param name="structuredCommandProvider">
        /// The structured command provider.
        /// </param>
        public StructuredMergeCommand([NotNull] IStructuredCommandProvider structuredCommandProvider)
            : base(structuredCommandProvider)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StructuredMergeCommand{TStructure}"/> class.
        /// </summary>
        /// <param name="structuredCommandProvider">
        /// The structured command provider.
        /// </param>
        /// <param name="databaseTransaction">
        /// The database transaction.
        /// </param>
        public StructuredMergeCommand([NotNull] IStructuredCommandProvider structuredCommandProvider, IDbTransaction databaseTransaction)
            : base(structuredCommandProvider, databaseTransaction)
        {
        }

        /// <inheritdoc />
        public override string CommandText => this.CompileCommandText();

        /// <summary>
        /// Merges a structured table value into the specified data item.
        /// </summary>
        /// <param name="mergeItems">
        /// The items to merge.
        /// </param>
        /// <param name="mergeMatchExpressions">
        /// The merge match expressions to match columns on.
        /// </param>
        /// <typeparam name="TDataItem">
        /// The type of the data item to merge into.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="StructuredMergeCommand{TStructure}"/>.
        /// </returns>
        public StructuredMergeCommand<TStructure> MergeInto<TDataItem>(
            [NotNull] IEnumerable<TStructure> mergeItems,
            [NotNull] params Expression<Func<TDataItem, object>>[] mergeMatchExpressions)
        {
            if (mergeItems == null)
            {
                throw new ArgumentNullException(nameof(mergeItems));
            }

            if (mergeMatchExpressions == null)
            {
                throw new ArgumentNullException(nameof(mergeMatchExpressions));
            }

            this.itemDefinition = this.StructuredCommandProvider.EntityDefinitionProvider.Resolve<TDataItem>();
            this.directAttributes.AddRange(this.itemDefinition.DirectAttributes);
            this.insertAttributes.AddRange(this.directAttributes.Where(definition => definition.IsIdentityColumn == false));

            // Use the expressions provided if any.
            this.mergeMatchAttributes.AddRange(
                mergeMatchExpressions.Any()
                    ? mergeMatchExpressions.Select(this.itemDefinition.Find)
                    : this.itemDefinition.PrimaryKeyAttributes);

            return this;
        }

        /// <summary>
        /// Deletes items that are unmatched in the source. Use with caution.
        /// </summary>
        /// <param name="constraints">
        /// The delete constraints to filter which rows to delete.
        /// </param>
        /// <returns>
        /// The current <see cref="StructuredMergeCommand{TStructure}"/>.
        /// </returns>
        public StructuredMergeCommand<TStructure> DeleteUnmatchedInSource([NotNull] params Expression<Func<TStructure, object>>[] constraints)
        {
            if (constraints == null)
            {
                throw new ArgumentNullException(nameof(constraints));
            }

            this.deleteConstraints.Clear();
            this.deleteConstraints.AddRange(constraints);
            this.deleteUnmatchedInSource = true;
            return this;
        }

        /// <summary>
        /// Selects output from inserted rows.
        /// </summary>
        /// <param name="matchKeys">
        /// The keys to match the original table value parameter with the output.
        /// </param>
        /// <returns>
        /// The current <see cref="StructuredMergeCommand{TStructure}"/>.
        /// </returns>
        public StructuredMergeCommand<TStructure> SelectFromInserted([NotNull] params Expression<Func<TStructure, object>>[] matchKeys)
        {
            if (matchKeys == null)
            {
                throw new ArgumentNullException(nameof(matchKeys));
            }

            var structureDefinition = this.StructuredCommandProvider.EntityDefinitionProvider.Resolve<TStructure>();
            this.selectionMatchAttributes.AddRange(matchKeys.Select(structureDefinition.Find));
            return this;
        }

        /// <summary>
        /// Compiles the command text.
        /// </summary>
        /// <returns>
        /// The command text as a <see cref="string" />.
        /// </returns>
        private string CompileCommandText()
        {
            var qualifier = this.StructuredCommandProvider.NameQualifier;
            var structureDefinition = this.StructuredCommandProvider.EntityDefinitionProvider.Resolve<TStructure>();
            var allAttributes = structureDefinition.AllAttributes.Where(definition => definition.IsReferencedDirect).ToList();

            // If there's an auto number primary key, then don't try to insert it. Only use the updateable attributes.
            var targetColumns = this.insertAttributes.OrderBy(x => x.Ordinal).Select(x => x.PhysicalName);

            var sourceAttributes = (from tvpAttribute in allAttributes
                                    join insertAttribute in this.insertAttributes on tvpAttribute.ResolvedLocation equals insertAttribute
                                        .ResolvedLocation
                                    orderby tvpAttribute.Ordinal
                                    select tvpAttribute).ToList();

            var sourceColumns = sourceAttributes.Select(x => x.PhysicalName);

            var keyAttributes = (from key in this.mergeMatchAttributes
                                 join fk in allAttributes on key.ResolvedLocation equals fk.ResolvedLocation
                                 select new { TargetKey = key, SourceKey = fk }).ToList();

            var updateAttributes = this.itemDefinition.UpdateableAttributes.Except(keyAttributes.Select(x => x.TargetKey));

            var updateClauses = from targetColumn in updateAttributes
                                join sourceColumn in allAttributes on targetColumn.ResolvedLocation equals sourceColumn.ResolvedLocation
                                select new { TargetColumn = targetColumn.PhysicalName, SourceColumn = sourceColumn.PhysicalName };

            // Always use the primary key for merge match.
            var mergeMatchClauses = keyAttributes.Select(
                x => $"{qualifier.Escape("Target")}.{qualifier.Escape(x.TargetKey.PhysicalName)} = "
                     + $"{qualifier.Escape("Source")}.{qualifier.Escape(x.SourceKey.PhysicalName)}");

            var updateClause = string.Join(
                ", ",
                updateClauses.Distinct()
                    .Select(x => $"{qualifier.Escape(x.TargetColumn)} = {qualifier.Escape("Source")}.{qualifier.Escape(x.SourceColumn)}"));

            var mergeStatementBuilder = new StringBuilder().AppendLine($"DECLARE @inserted {this.StructureTypeName};")
                .AppendLine(
                    $"MERGE {qualifier.Escape(this.itemDefinition.EntityContainer)}.{qualifier.Escape(this.itemDefinition.EntityName)} AS {qualifier.Escape("Target")}")
                .AppendLine($"USING @{this.Parameter} AS {qualifier.Escape("Source")}")
                .AppendLine($"ON ({string.Join(" AND ", mergeMatchClauses)})")
                .AppendLine("WHEN MATCHED THEN")
                .AppendLine($"UPDATE SET {updateClause}")
                .AppendLine("WHEN NOT MATCHED BY TARGET THEN")
                .AppendLine($"INSERT ({string.Join(", ", targetColumns.Select(x => $"{qualifier.Escape(x)}"))})")
                .AppendLine(
                    $"VALUES ({string.Join(", ", sourceColumns.Distinct().Select(x => $"{qualifier.Escape("Source")}.{qualifier.Escape(x)}"))})");

            if (this.deleteUnmatchedInSource)
            {
                var deleteFilterClauses = this.deleteConstraints.Select(
                        x =>
                            {
                                var sourceAttribute = structureDefinition.Find(x);
                                var tablePhysicalName = sourceAttribute.PhysicalName;
                                var itemPhysicalName = this.itemDefinition.DirectAttributes
                                    .FirstOrDefault(a => a.PhysicalName == sourceAttribute.PhysicalName)
                                    .PhysicalName;

                                return $"AND {qualifier.Escape("Target")}.{itemPhysicalName} IN (SELECT {qualifier.Escape(tablePhysicalName)} FROM @{this.Parameter})";
                            })
                    .ToList();

                mergeStatementBuilder.AppendLine(
                    deleteFilterClauses.Any()
                        ? $"WHEN NOT MATCHED BY SOURCE {string.Join(" ", deleteFilterClauses)} THEN DELETE"
                        : "WHEN NOT MATCHED BY SOURCE THEN DELETE");
            }

            if (this.selectionMatchAttributes.Any())
            {
                // These will be the column names from the table.
                var outputColumns = this.directAttributes.OrderBy(x => x.Ordinal).Select(x => $"INSERTED.{qualifier.Escape(x.PhysicalName)}");

                // Here we use property name because the assumption is the UDTT uses aliases, not physical names.
                var insertedColumns =
                    allAttributes.Join(
                        this.directAttributes,
                        tvp => qualifier.Qualify(tvp),
                        i => qualifier.GetCanonicalName(i), //// i.GetCanonicalName(),
                        (structure, entity) => structure).OrderBy(x => x.Ordinal).Select(x => $"{qualifier.Escape(x.PropertyName)}").ToList();

                // For our selection from the @inserted table, we need to get the key from the insert and everything else from the original
                // table valued parameter.
                var selectedKeyAttributes =
                    this.directAttributes.Where(x => x.IsIdentityColumn).Select(x => new { Column = $"i.{qualifier.Escape(x.PropertyName)}", Attribute = x }).ToList();

                // Everything for selecting from the TVP uses property name in order to match UDTT columns.
                var nonKeyAttributes = allAttributes.Where(definition => definition.IsIdentityColumn == false)
                    .Select(
                        x => new
                                 {
                                     Column = $"tvp.{qualifier.Escape(x.PropertyName)}",
                                     Attribute = x
                                 });

                var selectedColumns = selectedKeyAttributes.Union(nonKeyAttributes).OrderBy(x => x.Attribute.Ordinal).Select(x => x.Column);

                var matchAttributes = (from key in this.selectionMatchAttributes
                                       join fk in allAttributes on key.PhysicalName equals fk.PhysicalName
                                       select new { TargetKey = key, SourceKey = fk }).ToList();

                // If there are match attributes use those instead of the primary key.
                var selectionJoinMatchColumns =
                    (matchAttributes.Any() ? matchAttributes : keyAttributes).Select(
                        x => $"i.{qualifier.Escape(x.SourceKey.PropertyName)} = tvp.{qualifier.Escape(x.SourceKey.PropertyName)}");

                mergeStatementBuilder.AppendLine($"OUTPUT {string.Join(", ", outputColumns)}")
                    .AppendLine($"INTO @inserted ({string.Join(", ", insertedColumns)});")
                    .AppendLine($"SELECT {string.Join(", ", selectedColumns)}")
                    .AppendLine("FROM @inserted AS i")
                    .AppendLine($"INNER JOIN @{this.Parameter} AS tvp")
                    .AppendLine($"ON {string.Join(" AND ", selectionJoinMatchColumns)};");
            }
            else
            {
                mergeStatementBuilder.AppendLine(";");
            }

            var mergeStatement = mergeStatementBuilder.ToString();
            return mergeStatement;
        }
    }
}