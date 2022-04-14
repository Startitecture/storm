// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactSqlMergeBase.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;

    using JetBrains.Annotations;

    using Startitecture.Orm.Common;
    using Startitecture.Orm.Mapper;
    using Startitecture.Orm.Model;

    /// <summary>
    /// Base class for creating T-SQL MERGE table commands.
    /// </summary>
    /// <typeparam name="T">
    /// The type of entity that will be merged into.
    /// </typeparam>
    public abstract class TransactSqlMergeBase<T> : TableCommand<T>
    {
        /// <summary>
        /// The insert attributes.
        /// </summary>
        private readonly List<EntityAttributeDefinition> insertAttributes = new List<EntityAttributeDefinition>();

        /// <summary>
        /// The primary key attributes.
        /// </summary>
        private readonly List<EntityAttributeDefinition> mergeMatchAttributes = new List<EntityAttributeDefinition>();

        /// <summary>
        /// The attributes to select from the inserted columns.
        /// </summary>
        private readonly List<EntityAttributeDefinition> insertedSelectionAttributes = new List<EntityAttributeDefinition>();

        /// <summary>
        /// The attributes to select from the source columns.
        /// </summary>
        private readonly List<EntityAttributeDefinition> sourceSelectionAttributes = new List<EntityAttributeDefinition>();

        /// <summary>
        /// The entity relation set that matches source rows with target table rows.
        /// </summary>
        private readonly List<IEntityRelation> targetSourceRelations = new List<IEntityRelation>();

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
        /// Initializes a new instance of the <see cref="TransactSqlMergeBase{T}"/> class.
        /// </summary>
        /// <param name="tableCommandFactory">
        /// The table command factory.
        /// </param>
        /// <param name="databaseContext">
        /// The database context.
        /// </param>
        protected TransactSqlMergeBase(IDbTableCommandFactory tableCommandFactory, IDatabaseContext databaseContext)
            : base(tableCommandFactory, databaseContext)
        {
            this.insertAttributes.AddRange(this.EntityDefinition.InsertableAttributes);
            this.mergeMatchAttributes.AddRange(this.EntityDefinition.PrimaryKeyAttributes);
        }

        /// <inheritdoc />
        public override string GetCommandText<TItem>(string parameterName)
        {
            if (string.IsNullOrWhiteSpace(parameterName))
            {
                throw new ArgumentException($"'{nameof(parameterName)}' cannot be null or whitespace.", nameof(parameterName));
            }

            var itemDefinition = this.DatabaseContext.RepositoryAdapter.DefinitionProvider.Resolve<TItem>();
            var allAttributes = itemDefinition.AllAttributes.Where(definition => definition.IsReferencedDirect).ToList();
            var matchedAttributes = this.GetMatchedAttributes<TItem>(this.insertAttributes);

            var keyAttributes = this.explicitRelationSet.Relations.Any()
                                    ? (from r in this.explicitRelationSet.Relations
                                       select new
                                              {
                                                  TargetKey = this.EntityDefinition.Find(r.RelationExpression),
                                                  SourceKey = itemDefinition.Find(r.SourceExpression)
                                              }).ToDictionary(arg => arg.TargetKey, arg => arg.SourceKey)
                                    : (from key in this.mergeMatchAttributes
                                       join fk in allAttributes on key.ResolvedLocation equals fk.ResolvedLocation
                                       select new
                                              {
                                                  TargetKey = key,
                                                  SourceKey = fk
                                              }).ToDictionary(arg => arg.TargetKey, arg => arg.SourceKey);

            // In order for the merge to work, the key attributes must be included in the source.
            // TODO: See if the UNION will automatically drop duplicate columns.
            var declareSourceTable = this.DeclareSourceTable(
                parameterName,
                keyAttributes.Values.Union(
                    this.FromColumnExpressions.Any()
                        ? this.FromColumnExpressions.Select(itemDefinition.Find)
                        : itemDefinition.ReturnableAttributes.Where(definition => definition.IsIdentityColumn == false)));

            var commandBuilder = new StringBuilder();

            // TVPs don't declare source tables as the table type is the parameter.
            if (string.IsNullOrWhiteSpace(declareSourceTable) == false)
            {
                commandBuilder.AppendLine(declareSourceTable);
            }

            var selectResults = this.insertedSelectionAttributes.Union(this.sourceSelectionAttributes).Any();

            if (selectResults)
            {
                var insertedAttributes = this.GetInsertedAttributes(
                    this.insertedSelectionAttributes.Any() ? this.insertedSelectionAttributes : this.EntityDefinition.DirectAttributes);

                commandBuilder.AppendLine(this.DeclareInsertedTable(insertedAttributes));
            }

            // Always use the primary key for merge match.
            var mergeMatchClauses = keyAttributes.Select(
                x => $"{this.NameQualifier.Escape("Target")}.{this.NameQualifier.Escape(x.Key.PhysicalName)} = "
                     + $"{this.NameQualifier.Escape("Source")}.{this.NameQualifier.Escape(x.Value.PropertyName)}");

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
                    .Select(x => $"{this.NameQualifier.Escape(x.TargetColumn)} = {this.NameQualifier.Escape("Source")}.{this.NameQualifier.Escape(x.SourceColumn)}"));

            // If there's an auto number primary key, then don't try to insert it. Only use the updateable attributes.
            var targetColumns = this.insertAttributes.OrderBy(x => x.Ordinal).Select(x => x.PhysicalName);
            var sourceColumns = matchedAttributes.Values.Select(x => x.PropertyName);

            var entityName = $"{this.NameQualifier.Escape(this.EntityDefinition.EntityContainer)}.{this.NameQualifier.Escape(this.EntityDefinition.EntityName)}";
            commandBuilder.AppendLine($"MERGE {entityName} AS {this.NameQualifier.Escape("Target")}")
                .AppendLine($"USING {this.SourceSelection(parameterName, matchedAttributes)} AS {this.NameQualifier.Escape("Source")}")
                .AppendLine($"ON ({string.Join(" AND ", mergeMatchClauses)})")
                .AppendLine("WHEN MATCHED THEN")
                .AppendLine($"UPDATE SET {updateClause}")
                .AppendLine("WHEN NOT MATCHED BY TARGET THEN")
                .AppendLine($"INSERT ({string.Join(", ", targetColumns.Select(x => $"{this.NameQualifier.Escape(x)}"))})")
                .AppendLine(
                    $"VALUES ({string.Join(", ", sourceColumns.Distinct().Select(x => $"{this.NameQualifier.Escape("Source")}.{this.NameQualifier.Escape(x)}"))})");

            if (this.deleteUnmatchedInSource)
            {
                var deleteFilterClauses = this.deleteConstraints.Select(
                        x =>
                        {
                            var sourceAttribute = itemDefinition.Find(x);
                            var sourceReferenceName = sourceAttribute.PropertyName;
                            var targetReferenceName = this.EntityDefinition.DirectAttributes
                                .FirstOrDefault(a => a.PhysicalName == sourceAttribute.PhysicalName)
                                .PhysicalName;

                            return $"AND {this.NameQualifier.Escape("Target")}.{targetReferenceName} IN "
                                   + $"(SELECT {this.NameQualifier.Escape(sourceReferenceName)} FROM {this.SourceSelection(parameterName, matchedAttributes)})";
                        })
                    .ToList();

                commandBuilder.AppendLine(
                    deleteFilterClauses.Any()
                        ? $"WHEN NOT MATCHED BY SOURCE {string.Join(" ", deleteFilterClauses)} THEN DELETE"
                        : "WHEN NOT MATCHED BY SOURCE THEN DELETE");
            }

            if (selectResults)
            {
                this.SelectOutput<TItem>(commandBuilder, parameterName);
            }
            else
            {
                commandBuilder.AppendLine(";");
            }

            var mergeStatement = commandBuilder.ToString();
            return mergeStatement;
        }

        /// <summary>
        /// Specifies the columns to select into the table.
        /// </summary>
        /// <typeparam name="TStructure">
        /// The type of structure that the source items will be provided in.
        /// </typeparam>
        /// <param name="fromColumns">
        /// The columns to select into the table. These must be the same number of columns as target columns.
        /// </param>
        /// <returns>
        /// The current <see cref="TransactSqlMergeBase{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="fromColumns"/> is null.
        /// </exception>
        public TransactSqlMergeBase<T> From<TStructure>([NotNull] params Expression<Func<TStructure, object>>[] fromColumns)
        {
            if (fromColumns == null)
            {
                throw new ArgumentNullException(nameof(fromColumns));
            }

            this.FromColumnExpressions.Clear();
            this.FromColumnExpressions.AddRange(fromColumns);
            return this;
        }

        /// <summary>
        /// Implicitly defines the columns from the target table that should be implicitly matched to the source.
        /// </summary>
        /// <param name="mergeMatchExpressions">
        /// The merge match expressions to match columns on. If no expressions are defined, then the primary key attributes are used.
        /// </param>
        /// <returns>
        /// The current <see cref="TransactSqlMergeBase{T}"/>.
        /// </returns>
        /// <remarks>
        /// This method matches attributes on the source type using <see cref="EntityAttributeDefinition.ResolvedLocation"/>.
        /// </remarks>
        public TransactSqlMergeBase<T> OnImplicit([NotNull] params Expression<Func<T, object>>[] mergeMatchExpressions)
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
        /// Explicitly defines a pair of columns upon which the source and target will be matched. Call once for each column pair.
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
        /// The current <see cref="TransactSqlMergeBase{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sourceExpression"/> or <paramref name="targetExpression"/> is null.
        /// </exception>
        public TransactSqlMergeBase<T> On<TItem>(
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
        /// The current <see cref="TransactSqlMergeBase{T}"/>.
        /// </returns>
        public TransactSqlMergeBase<T> DeleteUnmatchedInSource<TItem>([NotNull] params Expression<Func<TItem, object>>[] constraints)
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
        /// Selects the specified attributes from the inserted columns.
        /// </summary>
        /// <param name="insertedAttributes">
        /// The inserted attributes to return.
        /// </param>
        /// <returns>
        /// The current <see cref="JsonInsert{T}"/>.
        /// </returns>
        public TransactSqlMergeBase<T> SelectFromInserted([NotNull] params Expression<Func<T, object>>[] insertedAttributes)
        {
            if (insertedAttributes == null)
            {
                throw new ArgumentNullException(nameof(insertedAttributes));
            }

            this.insertedSelectionAttributes.Clear();
            this.insertedSelectionAttributes.AddRange(
                insertedAttributes.Any() ? insertedAttributes.Select(this.EntityDefinition.Find) : this.EntityDefinition.DirectAttributes);

            return this;
        }

        /// <summary>
        /// Selects the specified attributes from the source columns.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of the source item.
        /// </typeparam>
        /// <param name="matchSetAction">
        /// Defines the set of matching attributes.
        /// </param>
        /// <param name="sourceAttributes">
        /// The source attributes to return.
        /// </param>
        /// <returns>
        /// The current <see cref="JsonInsert{T}"/>.
        /// </returns>
        public TransactSqlMergeBase<T> SelectFromSource<TItem>(
            [NotNull] Action<AttributeMatchSet<T, TItem>> matchSetAction,
            [NotNull] params Expression<Func<TItem, object>>[] sourceAttributes)
        {
            if (matchSetAction == null)
            {
                throw new ArgumentNullException(nameof(matchSetAction));
            }

            if (sourceAttributes == null)
            {
                throw new ArgumentNullException(nameof(sourceAttributes));
            }

            var itemDefinition = this.DatabaseContext.RepositoryAdapter.DefinitionProvider.Resolve<TItem>();

            var matchSet = new AttributeMatchSet<T, TItem>();
            matchSetAction.Invoke(matchSet);
            this.targetSourceRelations.Clear();

            foreach (var attributeMatch in matchSet.Matches)
            {
                var entityRelation = new EntityRelation(EntityRelationType.InnerJoin);
                entityRelation.Join(attributeMatch.SourceExpression, attributeMatch.RelationExpression);
                this.targetSourceRelations.Add(entityRelation);
            }

            this.sourceSelectionAttributes.AddRange(
                sourceAttributes.Any() ? sourceAttributes.Select(itemDefinition.Find) : itemDefinition.ReturnableAttributes);

            return this;
        }

        /// <summary>
        /// Builds the source table declaration statement.
        /// </summary>
        /// <param name="parameterName">
        /// The name of the table parameter.
        /// </param>
        /// <param name="sourceAttributes">
        /// The attributes of the source table.
        /// </param>
        /// <returns>
        /// A statement for declaring the source table.
        /// </returns>
        protected abstract string DeclareSourceTable(string parameterName, IEnumerable<EntityAttributeDefinition> sourceAttributes);

        /// <summary>
        /// Builds the table declaration statement. Called only if the caller indicates that inserted values should be selected.
        /// </summary>
        /// <param name="insertedAttributes">
        /// A dictionary where the inserted attributes are the key and the source attributes are the value.
        /// </param>
        /// <returns>
        /// A statement for declaring the inserted table.
        /// </returns>
        protected abstract string DeclareInsertedTable(IEnumerable<EntityAttributeDefinition> insertedAttributes);

        /// <summary>
        /// Builds the source selection statement.
        /// </summary>
        /// <param name="parameterName">
        /// The name of the table parameter.
        /// </param>
        /// <param name="matchedAttributes">
        /// A dictionary where the inserted attributes are the key and the source attributes are the value.
        /// </param>
        /// <returns>
        /// A statement for building the source selection.
        /// </returns>
        protected abstract string SourceSelection(string parameterName, Dictionary<EntityAttributeDefinition, EntityAttributeDefinition> matchedAttributes);

        /// <summary>
        /// Selects the output from an inserted table variable.
        /// </summary>
        /// <param name="commandBuilder">
        /// The command builder to update.
        /// </param>
        /// <param name="parameterName">
        /// The name of the table parameter.
        /// </param>
        private void SelectOutput<TItem>(StringBuilder commandBuilder, string parameterName)
        {
            var outputAttributes =
                (this.insertedSelectionAttributes.Any() ? this.insertedSelectionAttributes : this.EntityDefinition.DirectAttributes).ToList();

            // TODO: Detect if we've requested something that requires a JOIN
            // For our selection from the @inserted table, we need to get the key from the insert and everything else from the original
            // table valued parameter.
            var insertedColumns = outputAttributes.Select(
                    x => new
                         {
                             Column = $"i.{this.NameQualifier.Escape(x.PhysicalName)}",
                             Attribute = x
                         })
                .ToList();

            // Only get source columns if asked. Don't duplicate columns from the inserted attributes, either by physical name (which would indicate
            // a duplication of the same column) or property name (which would result in returning two columns with the same name).
            var sourceColumns = this.sourceSelectionAttributes
                .Where(definition => insertedColumns.Select(arg => arg.Attribute.PhysicalName).Contains(definition.PhysicalName) == false)
                .Where(definition => insertedColumns.Select(arg => arg.Attribute.PhysicalName).Contains(definition.PropertyName) == false)
                .Select(
                    x => new
                         {
                             Column = $"s.{this.NameQualifier.Escape(x.PropertyName)}",
                             Attribute = x
                         });

            var matchedAttributes = new Dictionary<EntityAttributeDefinition, EntityAttributeDefinition>();
            var itemDefinition = this.DatabaseContext.RepositoryAdapter.DefinitionProvider.Resolve<TItem>();

            // Only need to match if we've got columns from both the inserted and source tables.
            if (this.insertedSelectionAttributes.Any() && this.sourceSelectionAttributes.Any())
            {
                if (this.targetSourceRelations.Any())
                {
                    matchedAttributes = (from match in this.targetSourceRelations
                                         select new
                                                {
                                                    InsertedAttribute = this.EntityDefinition.Find(match.SourceExpression),
                                                    SourceAttribute = itemDefinition.Find(match.RelationExpression)
                                                }).ToDictionary(arg => arg.InsertedAttribute, arg => arg.SourceAttribute);
                }
                else
                {
                    // If a match isn't explicitly defined, look for a physical key match so that the item type can declare a matching physical name
                    // using ColumnAttribute.
                    matchedAttributes = (from ea in this.EntityDefinition.PrimaryKeyAttributes
                                         join ia in itemDefinition.DirectAttributes on ea.PhysicalName equals ia.PhysicalName
                                         select new
                                                {
                                                    InsertedAttribute = ea,
                                                    SourceAttribute = ia
                                                }).ToDictionary(arg => arg.InsertedAttribute, arg => arg.SourceAttribute);
                }
            }

            var selectedColumns = insertedColumns.Union(sourceColumns).OrderBy(x => x.Attribute.Ordinal).Select(x => x.Column);

            var outputColumns = outputAttributes.OrderBy(x => x.Ordinal).Select(x => $"INSERTED.{this.NameQualifier.Escape(x.PhysicalName)}");
            commandBuilder.AppendLine($"OUTPUT {string.Join(", ", outputColumns)}")
                .Append($"INTO {this.NameQualifier.AddParameterPrefix("inserted")} ")
                .AppendLine($"({string.Join(", ", insertedColumns.Select(x => this.NameQualifier.Escape(x.Attribute.PhysicalName)))});");

            if (matchedAttributes.Any())
            {
                var selectionJoinMatchColumns = matchedAttributes.Select(
                    x => $"i.{this.NameQualifier.Escape(x.Key.PhysicalName)} = s.{this.NameQualifier.Escape(x.Value.PropertyName)}");

                commandBuilder.AppendLine($"SELECT {string.Join(", ", selectedColumns)}")
                    .AppendLine($"FROM {this.NameQualifier.AddParameterPrefix("inserted")} AS i")
                    .AppendLine($"INNER JOIN {this.SourceSelection(parameterName, matchedAttributes)} AS s")
                    .AppendLine($"ON {string.Join(" AND ", selectionJoinMatchColumns)};");
            }
            else if (this.insertedSelectionAttributes.Any())
            {
                commandBuilder.AppendLine($"SELECT {string.Join(", ", selectedColumns)}")
                    .AppendLine($"FROM {this.NameQualifier.AddParameterPrefix("inserted")} AS i;");
            }
            else
            {
                commandBuilder.AppendLine($"SELECT {string.Join(", ", selectedColumns)}")
                    .AppendLine($"FROM {this.NameQualifier.AddParameterPrefix(parameterName)} AS s;");
            }
        }

        ////private void SelectOutput(List<EntityAttributeDefinition> allAttributes, Dictionary<EntityAttributeDefinition, EntityAttributeDefinition> keyAttributes, StringBuilder commandBuilder, Dictionary<EntityAttributeDefinition, EntityAttributeDefinition> matchedAttributes)
        ////{
        ////    // These will be the column names from the table.
        ////    var outputColumns = this.directAttributes.OrderBy(x => x.Ordinal).Select(x => $"INSERTED.{this.NameQualifier.Escape(x.PhysicalName)}");

        ////    // Here we use property name because the assumption is the UDTT uses aliases, not physical names.
        ////    var insertedColumns = allAttributes
        ////        .Join(
        ////            this.directAttributes,
        ////            source => this.NameQualifier.Qualify(source),
        ////            i => this.NameQualifier.GetCanonicalName(i),
        ////            (structure, entity) => structure)
        ////        .OrderBy(x => x.Ordinal)
        ////        .Select(x => $"{this.NameQualifier.Escape(x.PropertyName)}")
        ////        .ToList();

        ////    // For our selection from the @inserted table, we need to get the key from the insert and everything else from the original
        ////    // table valued parameter.
        ////    var selectedKeyAttributes = this.directAttributes.Where(x => x.IsIdentityColumn)
        ////        .Select(
        ////            x => new
        ////                 {
        ////                     Column = $"i.{this.NameQualifier.Escape(x.PropertyName)}",
        ////                     Attribute = x
        ////                 })
        ////        .ToList();

        ////    // Everything for selecting from the TVP uses property name in order to match UDTT columns.
        ////    var nonKeyAttributes = allAttributes.Where(definition => definition.IsIdentityColumn == false)
        ////        .Select(
        ////            x => new
        ////                 {
        ////                     Column = $"s.{this.NameQualifier.Escape(x.PropertyName)}",
        ////                     Attribute = x
        ////                 });

        ////    var selectedColumns = selectedKeyAttributes.Union(nonKeyAttributes).OrderBy(x => x.Attribute.Ordinal).Select(x => x.Column);

        ////    var matchAttributes = (from key in this.selectionMatchAttributes
        ////                           join fk in allAttributes on key.PhysicalName equals fk.PhysicalName
        ////                           select new
        ////                                  {
        ////                                      TargetKey = key,
        ////                                      SourceKey = fk
        ////                                  }).ToList();

        ////    // If there are match attributes use those instead of the primary key.
        ////    var selectionJoinMatchColumns = (matchAttributes.Any() ? matchAttributes : keyAttributes).Select(
        ////        x => $"i.{this.NameQualifier.Escape(x.Key.PhysicalName)} = s.{this.NameQualifier.Escape(x.Value.PropertyName)}");

        ////    commandBuilder.AppendLine($"OUTPUT {string.Join(", ", outputColumns)}")
        ////        .AppendLine($"INTO {this.NameQualifier.AddParameterPrefix("inserted")} ({string.Join(", ", insertedColumns)});")
        ////        .AppendLine($"SELECT {string.Join(", ", selectedColumns)}")
        ////        .AppendLine("FROM @inserted AS i")
        ////        .AppendLine($"INNER JOIN {this.SourceSelection(matchedAttributes)} AS s")
        ////        .AppendLine($"ON {string.Join(" AND ", selectionJoinMatchColumns)};");
        ////}
    }
}