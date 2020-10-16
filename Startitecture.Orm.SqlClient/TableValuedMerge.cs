// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableValuedMerge.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
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
    /// A MERGE command that uses a table-valued parameter (TVP) to merge multiple rows of data.
    /// </summary>
    /// <typeparam name="T">
    /// The type of table to use as the source of the merge.
    /// </typeparam>
    public class TableValuedMerge<T> : TableCommand<T>
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
        private readonly List<LambdaExpression> deleteConstraints = new List<LambdaExpression>();

        /// <summary>
        /// The explicit relation set.
        /// </summary>
        private readonly EntityRelationSet<T> explicitRelationSet = new EntityRelationSet<T>();

        /// <summary>
        /// The delete unmatched in source.
        /// </summary>
        private bool deleteUnmatchedInSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableValuedMerge{T}"/> class.
        /// </summary>
        /// <param name="tableCommandFactory">
        /// The table command provider.
        /// </param>
        /// <param name="databaseContext">
        /// The database context.
        /// </param>
        public TableValuedMerge([NotNull] IDbTableCommandFactory tableCommandFactory, IDatabaseContext databaseContext)
            : base(tableCommandFactory, databaseContext)
        {
            this.directAttributes.AddRange(this.EntityDefinition.DirectAttributes);
            this.insertAttributes.AddRange(this.EntityDefinition.InsertableAttributes);
            this.mergeMatchAttributes.AddRange(this.EntityDefinition.PrimaryKeyAttributes);
        }

        /// <inheritdoc />
        public override string CommandText => this.CompileCommandText();

        /// <summary>
        /// Specifies the columns to select into the table.
        /// </summary>
        /// <typeparam name="TStructure">
        /// The type of structure that the source items will be provided in.
        /// </typeparam>
        /// <param name="fromColumns">
        /// The columns to select into the table. These must be the same number of columns as target columns..
        /// </param>
        /// <returns>
        /// The current <see cref="TableValuedInsert{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="fromColumns"/> is null.
        /// </exception>
        public TableValuedMerge<T> From<TStructure>([NotNull] params Expression<Func<TStructure, object>>[] fromColumns)
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
        /// Defines the columns from the target table that should be implicitly matched to the source.
        /// </summary>
        /// <param name="mergeMatchExpressions">
        /// The merge match expressions to match columns on. If no expressions are defined, then the primary key attributes are used.
        /// </param>
        /// <returns>
        /// The current <see cref="TableValuedMerge{T}"/>.
        /// </returns>
        public TableValuedMerge<T> OnImplicit([NotNull] params Expression<Func<T, object>>[] mergeMatchExpressions)
        {
            if (mergeMatchExpressions == null)
            {
                throw new ArgumentNullException(nameof(mergeMatchExpressions));
            }

            // Use the expressions provided if any.
            this.mergeMatchAttributes.Clear();
            this.mergeMatchAttributes.AddRange(
                mergeMatchExpressions.Any() ? mergeMatchExpressions.Select(this.EntityDefinition.Find) : this.EntityDefinition.PrimaryKeyAttributes);

            return this;
        }

        /// <summary>
        /// Explicitly defines a pair of columns upon which the source and target will be matched. Call once for each
        /// column pair.
        /// </summary>
        /// <param name="sourceExpression">
        /// An expression that selects the key on the table value parameter.
        /// </param>
        /// <param name="targetExpression">
        /// An expression that selects the matching key on the target table.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of structure that the source items will be provided in.
        /// </typeparam>
        /// <returns>
        /// The current <see cref="TableValuedMerge{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sourceExpression"/> or <paramref name="targetExpression"/> is null.
        /// </exception>
        public TableValuedMerge<T> On<TItem>(
            [NotNull] Expression<Func<TItem, object>> sourceExpression,
            [NotNull] Expression<Func<T, object>> targetExpression)
        {
            if (sourceExpression == null)
            {
                throw new ArgumentNullException(nameof(sourceExpression));
            }

            if (targetExpression == null)
            {
                throw new ArgumentNullException(nameof(targetExpression));
            }

            this.explicitRelationSet.InnerJoin(sourceExpression, targetExpression);
            return this;
        }

        /// <summary>
        /// Deletes entities that are unmatched in the source. Use with caution.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of the table valued parameter.
        /// </typeparam>
        /// <param name="constraints">
        /// The delete constraints to filter which rows to delete.
        /// </param>
        /// <returns>
        /// The current <see cref="TableValuedMerge{T}"/>.
        /// </returns>
        public TableValuedMerge<T> DeleteUnmatchedInSource<TItem>([NotNull] params Expression<Func<TItem, object>>[] constraints)
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
        /// The current <see cref="TableValuedMerge{T}"/>.
        /// </returns>
        public TableValuedMerge<T> SelectFromInserted([NotNull] params Expression<Func<T, object>>[] matchKeys)
        {
            if (matchKeys == null)
            {
                throw new ArgumentNullException(nameof(matchKeys));
            }

            this.selectionMatchAttributes.Clear();
            this.selectionMatchAttributes.AddRange(matchKeys.Select(this.EntityDefinition.Find));
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
            var qualifier = this.DatabaseContext.RepositoryAdapter.NameQualifier;
            var allAttributes = this.ItemDefinition.AllAttributes.Where(definition => definition.IsReferencedDirect).ToList();

            // If there's an auto number primary key, then don't try to insert it. Only use the updateable attributes.
            var targetColumns = this.insertAttributes.OrderBy(x => x.Ordinal).Select(x => x.PhysicalName);

            var sourceAttributes = this.fromColumnExpressions.Any()
                                       ? this.fromColumnExpressions.Select(this.ItemDefinition.Find)
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
                                           TargetKey = this.EntityDefinition.Find(r.RelationExpression),
                                           SourceKey = this.ItemDefinition.Find(r.SourceExpression)
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

            // If all elements are part of the match attributes then this will end up blank.
            var updateAttributes = this.EntityDefinition.UpdateableAttributes;

            var updateClauses = from targetColumn in updateAttributes
                                join sourceColumn in allAttributes on targetColumn.ResolvedLocation equals sourceColumn.ResolvedLocation
                                select new
                                {
                                    TargetColumn = targetColumn.PhysicalName,
                                    SourceColumn = sourceColumn.PropertyName
                                };

            var updateClause = string.Join(
                ", ",
                updateClauses.Distinct()
                    .Select(x => $"{qualifier.Escape(x.TargetColumn)} = {qualifier.Escape("Source")}.{qualifier.Escape(x.SourceColumn)}"));

            var tableTypeName = this.ItemType.GetCustomAttribute<TableTypeAttribute>()?.TypeName ?? this.ItemType?.Name;
            var mergeStatementBuilder = new StringBuilder().AppendLine($"DECLARE {qualifier.AddParameterPrefix("inserted")} {tableTypeName};")
                .AppendLine(
                    $"MERGE {qualifier.Escape(this.EntityDefinition.EntityContainer)}.{qualifier.Escape(this.EntityDefinition.EntityName)} AS {qualifier.Escape("Target")}")
                .AppendLine($"USING {qualifier.AddParameterPrefix(this.ParameterName)} AS {qualifier.Escape("Source")}")
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
                                var sourceAttribute = this.ItemDefinition.Find(x);
                                var tableReferenceName = sourceAttribute.PropertyName;
                                var itemReferenceName = this.EntityDefinition.DirectAttributes
                                    .FirstOrDefault(a => a.PhysicalName == sourceAttribute.PhysicalName)
                                    .PhysicalName;

                                return
                                    $"AND {qualifier.Escape("Target")}.{itemReferenceName} IN (SELECT {qualifier.Escape(tableReferenceName)} FROM {qualifier.AddParameterPrefix(this.ParameterName)})";
                            })
                    .ToList();

                mergeStatementBuilder.AppendLine(
                    deleteFilterClauses.Any()
                        ? $"WHEN NOT MATCHED BY SOURCE {string.Join(" ", deleteFilterClauses)} THEN DELETE"
                        : "WHEN NOT MATCHED BY SOURCE THEN DELETE");
            }

            // TODO: See if we can just get the fully inserted rows since we are not pulling back the TVP
            if (this.selectionMatchAttributes.Any())
            {
                // These will be the column names from the table.
                var outputColumns = this.directAttributes.OrderBy(x => x.Ordinal).Select(x => $"INSERTED.{qualifier.Escape(x.PhysicalName)}");

                // Here we use property name because the assumption is the UDTT uses aliases, not physical names.
                var insertedColumns =
                    allAttributes.Join(
                        this.directAttributes,
                        tvp => qualifier.Qualify(tvp),
                        i => qualifier.GetCanonicalName(i),
                        (structure, entity) => structure).OrderBy(x => x.Ordinal).Select(x => $"{qualifier.Escape(x.PropertyName)}").ToList();

                // For our selection from the @inserted table, we need to get the key from the insert and everything else from the original
                // table valued parameter.
                var selectedKeyAttributes = this.directAttributes.Where(x => x.IsIdentityColumn)
                    .Select(
                        x => new
                        {
                            Column = $"i.{qualifier.Escape(x.PropertyName)}",
                            Attribute = x
                        })
                    .ToList();

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
                                       select new
                                       {
                                           TargetKey = key,
                                           SourceKey = fk
                                       }).ToList();

                // If there are match attributes use those instead of the primary key.
                var selectionJoinMatchColumns =
                    (matchAttributes.Any() ? matchAttributes : keyAttributes).Select(
                        x => $"i.{qualifier.Escape(x.SourceKey.PropertyName)} = tvp.{qualifier.Escape(x.SourceKey.PropertyName)}");

                mergeStatementBuilder.AppendLine($"OUTPUT {string.Join(", ", outputColumns)}")
                    .AppendLine($"INTO {qualifier.AddParameterPrefix("inserted")} ({string.Join(", ", insertedColumns)});")
                    .AppendLine($"SELECT {string.Join(", ", selectedColumns)}")
                    .AppendLine("FROM @inserted AS i")
                    .AppendLine($"INNER JOIN {qualifier.AddParameterPrefix(this.ParameterName)} AS tvp")
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