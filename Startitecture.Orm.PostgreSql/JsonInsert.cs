// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonInsert.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
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
    /// The json insert.
    /// </summary>
    /// <typeparam name="T">
    /// The type of item to insert into the repository.
    /// </typeparam>
    public class JsonInsert<T> : StructuredCommand<T>
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
        /// Initializes a new instance of the <see cref="JsonInsert{T}"/> class.
        /// </summary>
        /// <param name="structuredCommandProvider">
        /// The structured command provider.
        /// </param>
        public JsonInsert([NotNull] IStructuredCommandProvider structuredCommandProvider)
            : base(structuredCommandProvider)
        {
            this.commandText = new Lazy<string>(this.CompileCommandText);
            this.nameQualifier = this.StructuredCommandProvider.DatabaseContext.RepositoryAdapter.NameQualifier;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonInsert{T}"/> class.
        /// </summary>
        /// <param name="structuredCommandProvider">
        /// The structured command provider.
        /// </param>
        /// <param name="databaseTransaction">
        /// The database transaction.
        /// </param>
        public JsonInsert([NotNull] IStructuredCommandProvider structuredCommandProvider, IDbTransaction databaseTransaction)
            : base(structuredCommandProvider, databaseTransaction)
        {
            this.commandText = new Lazy<string>(this.CompileCommandText);
            this.nameQualifier = this.StructuredCommandProvider.DatabaseContext.RepositoryAdapter.NameQualifier;
        }

        /// <inheritdoc />
        public override string StructureTypeName => typeof(T).Name;

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
        /// The current <see cref="JsonInsert{T}"/>.
        /// </returns>
        public JsonInsert<T> InsertInto<TEntity>(
            [NotNull] IEnumerable<T> insertItems,
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
        /// The current <see cref="JsonInsert{T}"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="fromColumns"/> is null.
        /// </exception>
        public JsonInsert<T> From([NotNull] params Expression<Func<T, object>>[] fromColumns)
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
        /// The current <see cref="JsonInsert{T}"/>.
        /// </returns>
        public JsonInsert<T> SelectResults(params Expression<Func<T, object>>[] matchProperties)
        {
            var structureDefinition = this.StructuredCommandProvider.DatabaseContext.RepositoryAdapter.DefinitionProvider.Resolve<T>();
            this.selectionAttributes.Clear();
            this.selectionAttributes.AddRange(matchProperties.Select(structureDefinition.Find));
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
            var structureDefinition = this.StructuredCommandProvider.DatabaseContext.RepositoryAdapter.DefinitionProvider.Resolve<T>();
            var directAttributes = this.itemDefinition.DirectAttributes.ToList();
            var insertAttributes = (this.insertColumnExpressions.Any() ? this.insertColumnExpressions.Select(this.itemDefinition.Find) :
                                    this.itemDefinition.DirectAttributes.Any(x => x.IsIdentityColumn) ? this.itemDefinition.UpdateableAttributes :
                                    directAttributes).ToList();

            var targetColumns = insertAttributes.OrderBy(x => x.Ordinal).Select(x => this.nameQualifier.Escape(x.PhysicalName));

            // Direct attributes if none are specified because this could be a raised POCO.
            var sourceAttributes = (this.fromColumnExpressions.Any()
                                        ? this.fromColumnExpressions.Select(structureDefinition.Find)
                                        : from objectAttribute in structureDefinition.DirectAttributes
                                          join insertAttribute in insertAttributes on objectAttribute.PhysicalName equals insertAttribute.PhysicalName
                                          orderby objectAttribute.Ordinal
                                          select objectAttribute).ToList();

            var sourceColumns = sourceAttributes.Select(x => $"t.{this.nameQualifier.Escape(x.PropertyName)}");
            var jsonProperties = sourceAttributes.Select(
                x => $"{this.nameQualifier.Escape(x.PropertyName)} {PostgresTypeLookup.GetType(x.PropertyInfo.PropertyType)}");

            var commandBuilder = new StringBuilder();

            var selectResults = this.selectionAttributes.Any();
            commandBuilder.AppendLine(
                    $"INSERT INTO {this.nameQualifier.Escape(this.itemDefinition.EntityContainer)}.{this.nameQualifier.Escape(this.itemDefinition.EntityName)}")
                .AppendLine($"({string.Join(", ", targetColumns)})");

            commandBuilder.Append($@"SELECT {string.Join(", ", sourceColumns)}
FROM jsonb_to_recordset({this.nameQualifier.AddParameterPrefix(this.Parameter)}::jsonb) AS t ({string.Join(", ", jsonProperties)})");

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