// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactSqlInsertBase.cs" company="Startitecture">
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
    /// Base class for Transact-SQL table insert commands.
    /// </summary>
    /// <typeparam name="T">
    /// The type of entity that will be inserted into.
    /// </typeparam>
    public abstract class TransactSqlInsertBase<T> : TableCommand<T>
    {
        /// <summary>
        /// The values expression.
        /// </summary>
        private readonly List<LambdaExpression> insertColumnExpressions = new List<LambdaExpression>();

        /// <summary>
        /// The entity relation set that matches source rows with target table rows.
        /// </summary>
        private readonly List<IEntityRelation> targetSourceRelations = new List<IEntityRelation>();

        /// <summary>
        /// The attributes to select from the inserted columns.
        /// </summary>
        private readonly List<EntityAttributeDefinition> insertedSelectionAttributes = new List<EntityAttributeDefinition>();

        /// <summary>
        /// The attributes to select from the source columns.
        /// </summary>
        private readonly List<EntityAttributeDefinition> sourceSelectionAttributes = new List<EntityAttributeDefinition>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactSqlInsertBase{T}" /> class.
        /// </summary>
        /// <param name="tableCommandFactory">
        /// The table command factory.
        /// </param>
        /// <param name="databaseContext">
        /// The database context.
        /// </param>
        protected TransactSqlInsertBase([NotNull] IDbTableCommandFactory tableCommandFactory, IDatabaseContext databaseContext)
            : base(tableCommandFactory, databaseContext)
        {
        }

        /// <inheritdoc />
        public override string GetCommandText<TItem>(string parameterName)
        {
            if (string.IsNullOrWhiteSpace(parameterName))
            {
                throw new ArgumentException($"'{nameof(parameterName)}' cannot be null or whitespace.", nameof(parameterName));
            }

            var insertAttributes = (this.insertColumnExpressions.Any()
                                        ? this.insertColumnExpressions.Select(this.EntityDefinition.Find)
                                        : this.EntityDefinition.InsertableAttributes).ToList();

            // If FROM columns are specified, then assume that we are matching on ordinals, not physical names.
            var matchedAttributes = this.GetMatchedAttributes<TItem>(insertAttributes);

            var commandBuilder = new StringBuilder();
            var itemDefinition = this.DatabaseContext.RepositoryAdapter.DefinitionProvider.Resolve<TItem>();

            // No point including IDENTITY columns on an insert, unless specified by the from expressions.
            var sourceTableAttributes = (this.FromColumnExpressions.Any()
                                             ? this.FromColumnExpressions.Select(itemDefinition.Find)
                                             : itemDefinition.ReturnableAttributes.Where(definition => definition.IsIdentityColumn == false))
                .ToList();

            var declareSourceTable = this.DeclareSourceTable(parameterName, sourceTableAttributes);

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

            var schema = this.NameQualifier.Escape(this.EntityDefinition.EntityContainer);
            var entity = this.NameQualifier.Escape(this.EntityDefinition.EntityName);
            var targetColumns = insertAttributes.OrderBy(x => x.Ordinal).Select(x => this.NameQualifier.Escape(x.PhysicalName));
            commandBuilder.AppendLine($"INSERT INTO {schema}.{entity}").AppendLine($"({string.Join(", ", targetColumns)})");

            if (selectResults)
            {
                this.CreateOutput(commandBuilder);
            }

            commandBuilder.Append(this.SourceSelection(parameterName, matchedAttributes)).AppendLine(";");

            if (selectResults)
            {
                this.SelectOutput<TItem>(commandBuilder, parameterName, sourceTableAttributes);
            }

            var compileCommandText = commandBuilder.ToString();
            return compileCommandText;
        }

        /// <summary>
        /// Declares the table to insert into.
        /// </summary>
        /// <param name="targetColumns">
        /// The target columns of the insert table.
        /// </param>
        /// <returns>
        /// The current <see cref="JsonInsert{T}"/>.
        /// </returns>
        public TransactSqlInsertBase<T> InsertInto([NotNull] params Expression<Func<T, object>>[] targetColumns)
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
        /// The current <see cref="JsonInsert{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="fromColumns"/> is null.
        /// </exception>
        public TransactSqlInsertBase<T> From<TItem>([NotNull] params Expression<Func<TItem, object>>[] fromColumns)
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
        /// Selects the specified attributes from the inserted columns.
        /// </summary>
        /// <param name="insertAttributes">
        /// The inserted attributes to return.
        /// </param>
        /// <returns>
        /// The current <see cref="JsonInsert{T}"/>.
        /// </returns>
        public TransactSqlInsertBase<T> SelectFromInserted([NotNull] params Expression<Func<T, object>>[] insertAttributes)
        {
            if (insertAttributes == null)
            {
                throw new ArgumentNullException(nameof(insertAttributes));
            }

            this.insertedSelectionAttributes.Clear();
            this.insertedSelectionAttributes.AddRange(
                insertAttributes.Any() ? insertAttributes.Select(this.EntityDefinition.Find) : this.EntityDefinition.DirectAttributes);

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
        public TransactSqlInsertBase<T> SelectFromSource<TItem>(
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
        /// A dictionary where the target attributes are the key and the source attributes are the value.
        /// </param>
        /// <returns>
        /// A statement for building the source selection.
        /// </returns>
        protected abstract string SourceSelection(string parameterName, IReadOnlyDictionary<EntityAttributeDefinition, EntityAttributeDefinition> matchedAttributes);

        /// <summary>
        /// Gets a reference to the source table.
        /// </summary>
        /// <param name="parameterName">
        /// The name of the table parameter.
        /// </param>
        /// <param name="sourceAttributes">
        /// The attributes of the source table.
        /// </param>
        /// <returns>
        /// A statement for referencing the source table.
        /// </returns>
        protected abstract string SourceTableReference(string parameterName, IEnumerable<EntityAttributeDefinition> sourceAttributes);

        /// <summary>
        /// Creates the output for an output table.
        /// </summary>
        /// <param name="commandBuilder">
        /// The command builder.
        /// </param>
        private void CreateOutput(StringBuilder commandBuilder)
        {
            // Return all inserted attributes unless otherwise specified.
            var outputAttributes =
                (this.insertedSelectionAttributes.Any() ? this.insertedSelectionAttributes : this.EntityDefinition.DirectAttributes).ToList();

            var insertedColumns = outputAttributes.OrderBy(x => x.Ordinal).Select(x => this.NameQualifier.Escape(x.PropertyName)).ToList();
            var outputColumns = outputAttributes.OrderBy(x => x.Ordinal).Select(x => $"INSERTED.{this.NameQualifier.Escape(x.PhysicalName)}");

            commandBuilder.AppendLine($"OUTPUT {string.Join(", ", outputColumns)}")
                .AppendLine($"INTO @inserted ({string.Join(", ", insertedColumns)})");
        }

        /// <summary>
        /// Selects the output from an inserted table variable.
        /// </summary>
        /// <param name="commandBuilder">
        /// The command builder to update.
        /// </param>
        /// <param name="parameterName">
        /// The name of the table parameter.
        /// </param>
        /// <param name="sourceTableAttributes">
        /// The attributes defined in the source table.
        /// </param>
        private void SelectOutput<TItem>(StringBuilder commandBuilder, string parameterName, IEnumerable<EntityAttributeDefinition> sourceTableAttributes)
        {
            // TODO: Detect if we've requested something that requires a JOIN
            // For our selection from the @inserted table, we need to get the key from the insert and everything else from the original
            // table valued parameter.
            var insertedColumns = this.insertedSelectionAttributes.Select(
                    x => new
                         {
                             Column = $"i.{this.NameQualifier.Escape(x.PhysicalName)}",
                             Attribute = x
                         })
                .ToList();

            // Only get source columns if asked, and not from attributes that don't exist in the source table.
            var sourceColumns = this.sourceSelectionAttributes.Intersect(sourceTableAttributes).Select(
                x => new
                     {
                         Column = $"s.{this.NameQualifier.Escape(x.PropertyName)}",
                         Attribute = x
                     });

            var matchedAttributes = new Dictionary<EntityAttributeDefinition, EntityAttributeDefinition>();

            // Only need to match if we've got columns from both the inserted and source tables.
            if (this.insertedSelectionAttributes.Any() && this.sourceSelectionAttributes.Any())
            {
                var itemDefinition = this.DatabaseContext.RepositoryAdapter.DefinitionProvider.Resolve<TItem>();

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

            if (matchedAttributes.Any())
            {
                var selectionJoinMatchColumns = matchedAttributes.Select(
                    x => $"i.{this.NameQualifier.Escape(x.Key.PhysicalName)} = s.{this.NameQualifier.Escape(x.Value.PropertyName)}");

                commandBuilder.AppendLine($"SELECT {string.Join(", ", selectedColumns)}")
                    .AppendLine($"FROM {this.NameQualifier.AddParameterPrefix("inserted")} AS i")
                    .AppendLine($"INNER JOIN {this.SourceTableReference(parameterName, this.sourceSelectionAttributes)} AS s")
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
                    .AppendLine($"FROM {this.SourceTableReference(parameterName, this.sourceSelectionAttributes)} AS s;");
            }
        }
    }
}