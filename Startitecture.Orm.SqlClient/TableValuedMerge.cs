// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableValuedMerge.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.SqlClient
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
    /// The structured merge command.
    /// </summary>
    /// <typeparam name="TStructure">
    /// The type of structure to use as the source of the merge.
    /// </typeparam>
    public class TableValuedMerge<TStructure> : StructuredCommand<TStructure>
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
        /// The from expressions.
        /// </summary>
        private readonly List<LambdaExpression> fromColumnExpressions = new List<LambdaExpression>();

        /// <summary>
        /// The selection match attributes.
        /// </summary>
        private readonly List<EntityAttributeDefinition> selectionMatchAttributes = new List<EntityAttributeDefinition>();

        /// <summary>
        /// The delete constraints.
        /// </summary>
        private readonly List<Expression<Func<TStructure, object>>> deleteConstraints = new List<Expression<Func<TStructure, object>>>();

        /// <summary>
        /// The explicit relation set.
        /// </summary>
        private readonly EntityRelationSet<TStructure> explicitRelationSet = new EntityRelationSet<TStructure>();

        /// <summary>
        /// The delete unmatched in source.
        /// </summary>
        private bool deleteUnmatchedInSource;

        /// <summary>
        /// The entity definition.
        /// </summary>
        private IEntityDefinition itemDefinition;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableValuedMerge{TStructure}"/> class.
        /// </summary>
        /// <param name="structuredCommandProvider">
        /// The structured command provider.
        /// </param>
        public TableValuedMerge([NotNull] IStructuredCommandProvider structuredCommandProvider)
            : base(structuredCommandProvider)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableValuedMerge{TStructure}"/> class.
        /// </summary>
        /// <param name="structuredCommandProvider">
        /// The structured command provider.
        /// </param>
        /// <param name="databaseTransaction">
        /// The database transaction.
        /// </param>
        public TableValuedMerge([NotNull] IStructuredCommandProvider structuredCommandProvider, IDbTransaction databaseTransaction)
            : base(structuredCommandProvider, databaseTransaction)
        {
        }

        /// <inheritdoc />
        public override string CommandText => this.CompileCommandText();

        /// <summary>
        /// Merges a structured table value into the specified data item.
        /// </summary>
        /// <param name="mergeItems">
        /// The entities to merge.
        /// </param>
        /// <param name="mergeMatchExpressions">
        /// The merge match expressions to match columns on.
        /// </param>
        /// <typeparam name="TEntity">
        /// The type of the entity to merge into.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="TableValuedMerge{TStructure}"/>.
        /// </returns>
        public TableValuedMerge<TStructure> MergeInto<TEntity>(
            [NotNull] IEnumerable<TStructure> mergeItems,
            [NotNull] params Expression<Func<TEntity, object>>[] mergeMatchExpressions)
        {
            if (mergeItems == null)
            {
                throw new ArgumentNullException(nameof(mergeItems));
            }

            if (mergeMatchExpressions == null)
            {
                throw new ArgumentNullException(nameof(mergeMatchExpressions));
            }

            this.Items.Clear();
            this.Items.AddRange(mergeItems);
            this.itemDefinition = this.StructuredCommandProvider.EntityDefinitionProvider.Resolve<TEntity>();
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
        public TableValuedMerge<TStructure> From([NotNull] params Expression<Func<TStructure, object>>[] fromColumns)
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
        /// The on.
        /// </summary>
        /// <param name="sourceExpression">
        /// An expression that selects the key on the table value parameter.
        /// </param>
        /// <param name="targetExpression">
        /// An expression that selects the matching key on the target table.
        /// </param>
        /// <typeparam name="TDataItem">
        /// The type of entity representing the target table of the merge.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="TableValuedMerge{TStructure}"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="sourceExpression"/> or <paramref name="targetExpression"/> is null.
        /// </exception>
        public TableValuedMerge<TStructure> On<TDataItem>(
            [NotNull] Expression<Func<TStructure, object>> sourceExpression,
            [NotNull] Expression<Func<TDataItem, object>> targetExpression)
        {
            if (sourceExpression == null)
            {
                throw new ArgumentNullException(nameof(sourceExpression));
            }

            if (targetExpression == null)
            {
                throw new ArgumentNullException(nameof(targetExpression));
            }

            this.explicitRelationSet.InnerJoin<TDataItem>(sourceExpression, targetExpression);
            return this;
        }

        /// <summary>
        /// Deletes entities that are unmatched in the source. Use with caution.
        /// </summary>
        /// <param name="constraints">
        /// The delete constraints to filter which rows to delete.
        /// </param>
        /// <returns>
        /// The current <see cref="TableValuedMerge{TStructure}"/>.
        /// </returns>
        public TableValuedMerge<TStructure> DeleteUnmatchedInSource([NotNull] params Expression<Func<TStructure, object>>[] constraints)
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
        /// The current <see cref="TableValuedMerge{TStructure}"/>.
        /// </returns>
        public TableValuedMerge<TStructure> SelectFromInserted([NotNull] params Expression<Func<TStructure, object>>[] matchKeys)
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

            var sourceAttributes = this.fromColumnExpressions.Any()
                                       ? this.fromColumnExpressions.Select(structureDefinition.Find)
                                       : (from tvpAttribute in allAttributes
                                          join insertAttribute in this.insertAttributes on tvpAttribute.ResolvedLocation equals insertAttribute
                                              .ResolvedLocation
                                          orderby tvpAttribute.Ordinal
                                          select tvpAttribute).ToList();

            var sourceColumns = sourceAttributes.Select(x => x.PropertyName);

            var keyAttributes = this.explicitRelationSet.Relations.Any()
                                    ? (from r in this.explicitRelationSet.Relations
                                       select new
                                                  {
                                                      TargetKey = this.itemDefinition.Find(r.RelationExpression),
                                                      SourceKey = structureDefinition.Find(r.SourceExpression)
                                                  }).ToList()
                                    : (from key in this.mergeMatchAttributes
                                       join fk in allAttributes on key.ResolvedLocation equals fk.ResolvedLocation
                                       select new
                                                  {
                                                      TargetKey = key,
                                                      SourceKey = fk
                                                  }).ToList();

            // Always use the primary key for merge match.
            var mergeMatchClauses = keyAttributes.Select(
                x => $"{qualifier.Escape("Target")}.{qualifier.Escape(x.TargetKey.PhysicalName)} = "
                     + $"{qualifier.Escape("Source")}.{qualifier.Escape(x.SourceKey.PropertyName)}");

            // If all elements are part of the match attributes then this will end up blank. TODO: Skip update if update attributes has no items?
            var updateAttributes = this.itemDefinition.UpdateableAttributes; ////.Except(keyAttributes.Select(x => x.TargetKey)); 

            var updateClauses = from targetColumn in updateAttributes
                                join sourceColumn in allAttributes on targetColumn.ResolvedLocation equals sourceColumn.ResolvedLocation
                                select new { TargetColumn = targetColumn.PhysicalName, SourceColumn = sourceColumn.PropertyName };

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
                                var tableReferenceName = sourceAttribute.PropertyName; // TODO: Reference by alias?
                                var itemReferenceName = this.itemDefinition.DirectAttributes
                                    .FirstOrDefault(a => a.PhysicalName == sourceAttribute.PhysicalName)
                                    .PhysicalName;

                                return $"AND {qualifier.Escape("Target")}.{itemReferenceName} IN (SELECT {qualifier.Escape(tableReferenceName)} FROM @{this.Parameter})";
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

                // Here we use property name because the assumption is the UDTT uses aliases, not physical names. TODO: Use aliases?
                var insertedColumns =
                    allAttributes.Join(
                        this.directAttributes,
                        tvp => qualifier.Qualify(tvp),
                        i => qualifier.GetCanonicalName(i), //// i.GetPhysicalName(),
                        (structure, entity) => structure).OrderBy(x => x.Ordinal).Select(x => $"{qualifier.Escape(x.PropertyName)}").ToList();

                // For our selection from the @inserted table, we need to get the key from the insert and everything else from the original
                // table valued parameter.
                var selectedKeyAttributes =
                    this.directAttributes.Where(x => x.IsIdentityColumn).Select(x => new { Column = $"i.{qualifier.Escape(x.PropertyName)}", Attribute = x }).ToList();

                // Everything for selecting from the TVP uses property name in order to match UDTT columns.
                var nonKeyAttributes = allAttributes.Where(definition => definition.IsIdentityColumn == false).Select(
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