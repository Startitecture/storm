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
    using System.Reflection;
    using System.Text;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Schema;

    /// <summary>
    /// The structured merge command.
    /// </summary>
    /// <typeparam name="TStructure">
    /// The type of structure to use as the source of the merge.
    /// </typeparam>
    public class StructuredMergeCommand<TStructure> : StructuredSqlCommand<TStructure>
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
        /// The delete unmatched in source.
        /// </summary>
        private bool deleteUnmatchedInSource;

        /// <summary>
        /// The item definition.
        /// </summary>
        private IEntityDefinition itemDefinition;

        /// <summary>
        /// The delete constraints.
        /// </summary>
        private List<Expression<Func<TStructure, object>>> deleteConstraints;

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

        /// <summary>
        /// Gets the command text.
        /// </summary>
        public override string CommandText
        {
            get
            {
                return this.CompileCommandText();
            }
        }

        /// <summary>
        /// Merges a structured table value into the specified data item.
        /// </summary>
        /// <param name="mergeMatchExpressions">
        /// The merge match expressions to match columns on.
        /// </param>
        /// <typeparam name="TDataItem">
        /// The type of the data item to merge into.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="StructuredMergeCommand{TStructure}"/>.
        /// </returns>
        public StructuredMergeCommand<TStructure> MergeInto<TDataItem>([NotNull] params Expression<Func<TDataItem, object>>[] mergeMatchExpressions)
        {
            if (mergeMatchExpressions == null)
            {
                throw new ArgumentNullException(nameof(mergeMatchExpressions));
            }

            var entityDefinition = Singleton<PetaPocoDefinitionProvider>.Instance.Resolve<TDataItem>();
            this.itemDefinition = entityDefinition;
            this.directAttributes.AddRange(entityDefinition.DirectAttributes);
            this.insertAttributes.AddRange(
                entityDefinition.AutoNumberPrimaryKey.HasValue ? this.itemDefinition.UpdateableAttributes : this.directAttributes);

            // Use the expressions provided if any.
            this.mergeMatchAttributes.AddRange(
                mergeMatchExpressions.Any()
                    ? mergeMatchExpressions.Select<Expression<Func<TDataItem, object>>, string>(x => x.GetPropertyName()).Select(entityDefinition.Find)
                    : entityDefinition.PrimaryKeyAttributes);

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

            this.deleteConstraints = constraints.ToList();
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

            this.selectionMatchAttributes.AddRange(matchKeys.Select(x => this.itemDefinition.Find(x.GetPropertyName())));
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
            var structureDefinition = Singleton<PetaPocoDefinitionProvider>.Instance.Resolve<TStructure>();
            var allAttributes = structureDefinition.AllAttributes.Where(definition => definition.IsReferencedDirect).ToList();

            // If there's an auto number primary key, then don't try to insert it. Only use the updatable attributes.
            var targetColumns = this.insertAttributes.OrderBy(x => x.PhysicalName).Select(x => x.PhysicalName);

            var sourceAttributes = (from tvpAttribute in allAttributes
                                    join insertAttribute in this.insertAttributes on tvpAttribute.PhysicalName equals insertAttribute.PhysicalName
                                    orderby tvpAttribute.PhysicalName
                                    select tvpAttribute).ToList();

            var sourceColumns = sourceAttributes.Select(x => x.PhysicalName);

            var keyAttributes = (from key in this.mergeMatchAttributes
                                 join fk in allAttributes on key.PhysicalName equals fk.PhysicalName
                                 select new { TargetKey = key, SourceKey = fk }).ToList();

            var updateAttributes = this.itemDefinition.UpdateableAttributes.Except(keyAttributes.Select(x => x.TargetKey));

            var updateClauses = from targetColumn in updateAttributes
                                join sourceColumn in allAttributes on targetColumn.PhysicalName equals sourceColumn.PhysicalName
                                select new { TargetColumn = targetColumn.PhysicalName, SourceColumn = sourceColumn.PhysicalName };

            // Throws an error if there's not a table type attribute.
            var tableTypeName = typeof(TStructure).GetCustomAttributes<TableTypeAttribute>().First().TypeName;

            // Always use the primary key for merge match.
            var mergeMatchClauses = keyAttributes.Select(x => $"[Target].[{x.TargetKey.PhysicalName}] = [Source].[{x.SourceKey.PhysicalName}]");

            var mergeStatementBuilder =
                new StringBuilder().AppendLine($"DECLARE @inserted {tableTypeName};")
                    .AppendLine($"MERGE [{this.itemDefinition.EntityContainer}].[{this.itemDefinition.EntityName}] AS [Target]")
                    .AppendLine($"USING {this.Parameter} AS [Source]")
                    .AppendLine($"ON ({string.Join(" AND ", mergeMatchClauses)})")
                    .AppendLine("WHEN MATCHED THEN")
                    .AppendLine(
                        $"UPDATE SET {string.Join(", ", Enumerable.Distinct(updateClauses).Select(x => $"[{x.TargetColumn}] = [Source].[{x.SourceColumn}]"))}")
                    .AppendLine("WHEN NOT MATCHED BY TARGET THEN")
                    .AppendLine($"INSERT ({string.Join(", ", targetColumns.Select<string, string>(x => $"[{x}]"))})")
                    .AppendLine($"VALUES ({string.Join(", ", Enumerable.Distinct<string>(sourceColumns).Select(x => $"[Source].[{x}]"))})");

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

                                return $"AND [Target].{itemPhysicalName} IN (SELECT [{tablePhysicalName}] FROM {this.Parameter})";
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
                var outputColumns = this.directAttributes.OrderBy(x => x.PhysicalName).Select(x => $"INSERTED.[{x.PhysicalName}]");

                // Here we use property name because the assumption is the UDTT uses aliases, not physical names.
                var insertedColumns =
                    Enumerable.ToList<string>(
                        allAttributes.Join(
                            this.directAttributes,
                            tvp => tvp.GetQualifiedName(),
                            i => i.GetCanonicalName(),
                            (structure, entity) => structure).OrderBy(x => x.PhysicalName).Select(x => $"[{x.PropertyName}]"));

                // For our selection from the @inserted table, we need to get the key from the insert and everything else from the original
                // table valued parameter.
                var selectedKeyAttributes =
                    this.directAttributes.Where(x => x.IsIdentityColumn).Select(x => new { Column = $"i.[{x.PropertyName}]", Attribute = x }).ToList();

                // Everything for selecting from the TVP uses property name in order to match UDTT columns.
                var nonKeyAttributes =
                    Enumerable.Select(allAttributes.Where(definition => definition.IsIdentityColumn == false), x => new { Column = $"tvp.[{x.PropertyName}]", Attribute = x });

                var selectedColumns = selectedKeyAttributes.Union(nonKeyAttributes).OrderBy(x => x.Attribute.PropertyName).Select(x => x.Column);

                var matchAttributes = (from key in this.selectionMatchAttributes
                                       join fk in allAttributes on key.PhysicalName equals fk.PhysicalName
                                       select new { TargetKey = key, SourceKey = fk }).ToList();

                // If there are match attributes use those instead of the primary key.
                var selectionJoinMatchColumns =
                    (matchAttributes.Any() ? matchAttributes : keyAttributes).Select(
                        x => $"i.[{x.SourceKey.PropertyName}] = tvp.[{x.SourceKey.PropertyName}]");

                mergeStatementBuilder.AppendLine($"OUTPUT {string.Join(", ", outputColumns)}")
                    .AppendLine($"INTO @inserted ({string.Join(", ", insertedColumns)});")
                    .AppendLine($"SELECT {string.Join(", ", selectedColumns)}")
                    .AppendLine("FROM @inserted AS i")
                    .AppendLine($"INNER JOIN {this.Parameter} AS tvp")
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