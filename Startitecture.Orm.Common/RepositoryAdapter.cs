// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RepositoryAdapter.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Orm.Model;
    using Startitecture.Resources;

    /// <summary>
    /// Base class for repository adapters.
    /// </summary>
    /// <remarks>
    /// Repository adapters combine entity definition providers and name qualifiers to produce SQL statements specific to a target SQL server
    /// platform.
    /// </remarks>
    public abstract class RepositoryAdapter : IRepositoryAdapter
    {
        /// <summary>
        /// The select statement.
        /// </summary>
        private const string SelectStatement = "SELECT";

        /// <summary>
        /// The if exists clause.
        /// </summary>
        private const string IfExistsClause = @"IF EXISTS (
{0}
) SELECT 1  ELSE SELECT 0";

        /// <summary>
        /// The SQL UPDATE clause.
        /// </summary>
        private const string SqlUpdateClause = @"UPDATE {0}
SET
{1}";

        /// <summary>
        /// The SQL parameter format.
        /// </summary>
        private const string SetParameterFormat = "{0} = {1}";

        /// <summary>
        /// The null value.
        /// </summary>
        private const string SetNullParameterFormat = "{0} = NULL";

        /// <summary>
        /// The from clause.
        /// </summary>
        private const string FromClause = "FROM ";

        /// <summary>
        /// The where clause.
        /// </summary>
        private const string WhereClause = "WHERE";

        /// <summary>
        /// The delete statement.
        /// </summary>
        private const string DeleteStatement = "DELETE ";

        /// <summary>
        /// The delete from statement.
        /// </summary>
        private const string DeleteFromStatement = "DELETE FROM ";

        /// <summary>
        /// The from statement.
        /// </summary>
        private const string FromStatement = "FROM ";

        /// <summary>
        /// The alias column format.
        /// </summary>
        private const string AliasColumnFormat = "{0} AS {1}";

        /// <summary>
        /// The SQL where clause.
        /// </summary>
        private const string SqlWhereClause = "WHERE ";

        /// <summary>
        /// The union statement.
        /// </summary>
        private const string UnionStatement = "UNION";

        /// <summary>
        /// The intersection statement.
        /// </summary>
        private const string IntersectionStatement = "INTERSECT";

        /// <summary>
        /// The exception statement.
        /// </summary>
        private const string ExceptionStatement = "EXCEPT";

        /// <summary>
        /// The equality filter.
        /// </summary>
        private const string EqualityFilter = "{0} {1} {2}";

        /// <summary>
        /// The between filter.
        /// </summary>
        private const string BetweenFilter = "{0} BETWEEN {1} AND {2}";

        /// <summary>
        /// The not null predicate.
        /// </summary>
        private const string NullPredicate = "{0} IS NULL";

        /// <summary>
        /// The not null predicate.
        /// </summary>
        private const string NotNullPredicate = "{0} IS NOT NULL";

        /// <summary>
        /// The filter separator.
        /// </summary>
        private const string FilterSeparator = " AND";

        /// <summary>
        /// The less than predicate.
        /// </summary>
        private const string LessThanPredicate = "{0} < {1}";

        /// <summary>
        /// The less than predicate.
        /// </summary>
        private const string LessThanOrEqualToPredicate = "{0} <= {1}";

        /// <summary>
        /// The greater than predicate.
        /// </summary>
        private const string GreaterThanPredicate = "{0} > {1}";

        /// <summary>
        /// The greater than predicate.
        /// </summary>
        private const string GreaterThanOrEqualToPredicate = "{0} >= {1}";

        /// <summary>
        /// The like operand.
        /// </summary>
        private const string LikeOperand = "LIKE";

        /// <summary>
        /// The like operand.
        /// </summary>
        private const string NotLikeOperand = "NOT LIKE";

        /// <summary>
        /// The equality operand.
        /// </summary>
        private const string EqualityOperand = "=";

        /// <summary>
        /// The inequality operand.
        /// </summary>
        private const string InequalityOperand = "<>";

        /// <summary>
        /// The inclusive predicate.
        /// </summary>
        private const string InclusionPredicate = "{0} IN ({1})";

        /// <summary>
        /// The inclusive predicate.
        /// </summary>
        private const string ExclusionPredicate = "{0} NOT IN ({1})";

        /// <summary>
        /// The parameter separator.
        /// </summary>
        private const string ParameterSeparator = ", ";

        /// <summary>
        /// The default indent.
        /// </summary>
        private const int DefaultIndent = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryAdapter"/> class.
        /// </summary>
        /// <param name="definitionProvider">
        /// The definition Provider.
        /// </param>
        /// <param name="nameQualifier">
        /// The name Qualifier.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="definitionProvider"/> or <paramref name="nameQualifier"/> is null.
        /// </exception>
        protected RepositoryAdapter([NotNull] IEntityDefinitionProvider definitionProvider, [NotNull] INameQualifier nameQualifier)
        {
            this.DefinitionProvider = definitionProvider ?? throw new ArgumentNullException(nameof(definitionProvider));
            this.NameQualifier = nameQualifier ?? throw new ArgumentNullException(nameof(nameQualifier));
        }

        /// <inheritdoc />
        public IEntityDefinitionProvider DefinitionProvider { get; }

        /// <inheritdoc />
        public abstract IReadOnlyDictionary<Tuple<Type, Type>, IValueMapper> ValueMappers { get; }

        /// <inheritdoc />
        public INameQualifier NameQualifier { get; }

        /// <inheritdoc />
        /// <remarks>
        /// This implementation will create an IF EXISTS ({statement}) SELECT 1 ELSE SELECT 0 wrapper around the selection. For
        /// best performance,
        /// include only a single, non-nullable column in the selection, ideally a clustered key column.
        /// </remarks>
        public virtual string CreateExistsStatement([NotNull] IEntitySet entitySet)
        {
            if (entitySet == null)
            {
                throw new ArgumentNullException(nameof(entitySet));
            }

            // Force our qualified columns because this is an exists statement.
            var qualifiedColumns = new List<string>
                                       {
                                           '1'.ToString()
                                       };

            var entityDefinition = this.DefinitionProvider.Resolve(entitySet.EntityType);
            var completeStatement = this.CreateCompleteStatement(entitySet, entityDefinition, qualifiedColumns);
            return string.Format(CultureInfo.InvariantCulture, IfExistsClause, completeStatement);
        }

        /// <inheritdoc />
        /// <remarks>
        /// At this time, creating paging statements that include UNION, INTERSECT and EXCEPT clauses is not supported.
        /// </remarks>
        public virtual string CreateSelectionStatement([NotNull] IEntitySet entitySet)
        {
            if (entitySet == null)
            {
                throw new ArgumentNullException(nameof(entitySet));
            }

            var entityDefinition = this.DefinitionProvider.Resolve(entitySet.EntityType);
            var completeStatement = this.CreateCompleteStatement(entitySet, entityDefinition, null);
            return entitySet.ParentExpression == null
                       ? completeStatement
                       : this.CreateTableExpression(entitySet, entityDefinition, completeStatement);
        }

        /// <inheritdoc />
        public virtual string CreateUpdateStatement([NotNull] IUpdateSet updateSet)
        {
            if (updateSet == null)
            {
                throw new ArgumentNullException(nameof(updateSet));
            }

            var setClauses = new List<string>();
            int index = 0;
            var entityDefinition = this.DefinitionProvider.Resolve(updateSet.EntityType);

            foreach (var valueState in updateSet.AttributesToSet)
            {
                var attributeDefinition = entityDefinition.Find(valueState.AttributeLocation);

                if (attributeDefinition == default)
                {
                    throw new RepositoryException(
                        valueState.AttributeLocation,
                        $"The attribute location '{valueState.AttributeLocation}' could not be found in the definition for '{entityDefinition}'.");
                }

                // Skip attributes that can't be updated, as defined by the entity definition.
                // TODO: remove this safety? Only usage of the interface method.
                if (entityDefinition.IsUpdateable(attributeDefinition) == false)
                {
                    continue;
                }

                var physicalName = this.NameQualifier.Escape(attributeDefinition.PhysicalName);
                setClauses.Add(
                    valueState.Value == null
                        ? string.Format(CultureInfo.CurrentCulture, SetNullParameterFormat, physicalName)
                        : string.Format(
                            CultureInfo.CurrentCulture,
                            SetParameterFormat,
                            physicalName,
                            this.NameQualifier.AddParameterPrefix(index.ToString(CultureInfo.InvariantCulture))));

                if (valueState.Value != null)
                {
                    index++;
                }
            }

            var entityName =
                $"{this.NameQualifier.Escape(entityDefinition.EntityContainer)}.{this.NameQualifier.Escape(entityDefinition.EntityName)}";
            var joinClause = new JoinClause(this.DefinitionProvider, this.NameQualifier);
            string joinClauseText = updateSet.Relations.Any()
                                        ? string.Concat(
                                            Environment.NewLine,
                                            FromClause,
                                            entityName,
                                            Environment.NewLine,
                                            joinClause.Create(updateSet.Relations))
                                        : string.Empty;

            var setClause = string.Join(string.Concat(',', Environment.NewLine), setClauses);
            var predicateClause = this.CreatePredicateClause(entityDefinition, updateSet.Filters, index);

            return string.Concat(
                string.Format(CultureInfo.InvariantCulture, SqlUpdateClause, entityName, setClause),
                joinClauseText,
                Environment.NewLine,
                WhereClause,
                Environment.NewLine,
                predicateClause);
        }

        /// <inheritdoc />
        public virtual string CreateDeletionStatement([NotNull] IEntitySet entitySet)
        {
            if (entitySet == null)
            {
                throw new ArgumentNullException(nameof(entitySet));
            }

            var entityDefinition = this.DefinitionProvider.Resolve(entitySet.EntityType);
            var primaryTableName =
                $"{this.NameQualifier.Escape(entityDefinition.EntityContainer)}.{this.NameQualifier.Escape(entityDefinition.EntityName)}";
            var filterStatement = this.CreatePredicateClause(entityDefinition, entitySet.Filters, 0);
            var filter = entitySet.Filters.Any() ? string.Concat(Environment.NewLine, SqlWhereClause, filterStatement) : string.Empty;

            if (entitySet.Relations.Any() == false)
            {
                return string.Concat(DeleteFromStatement, primaryTableName, filter);
            }

            var joinClause = new JoinClause(this.DefinitionProvider, this.NameQualifier);

            return string.Concat(
                DeleteStatement,
                primaryTableName,
                Environment.NewLine,
                FromStatement,
                primaryTableName,
                Environment.NewLine,
                joinClause.Create(entitySet.Relations),
                filter);
        }

        /// <inheritdoc />
        public virtual string CreateInsertionStatement<T>()
        {
            var definition = this.DefinitionProvider.Resolve<T>();

            var escapeTableName = $"{this.NameQualifier.Escape(definition.EntityContainer)}.{this.NameQualifier.Escape(definition.EntityName)}";
            var columnNames = string.Join(
                ", ",
                definition.InsertableAttributes.Select(attribute => this.NameQualifier.Escape(attribute.PhysicalName)));
            var columnValues = string.Join(
                ", ",
                Enumerable.Range(0, definition.InsertableAttributes.Count())
                    .Select(i => this.NameQualifier.AddParameterPrefix(i.ToString(CultureInfo.InvariantCulture))));

            var commandText = $@"INSERT INTO {escapeTableName}
({columnNames})
VALUES ({columnValues})";

            return definition.RowIdentity.HasValue ? this.CaptureInsertedIdentity(commandText, definition) : commandText;
        }

        /// <inheritdoc />
        public virtual IDbDataParameter CreateParameter([NotNull] IDbCommand command, [NotNull] string name, object value)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(name));
            }

            var parameter = command.CreateParameter();
            parameter.ParameterName = this.NameQualifier.AddParameterPrefix(name);

            if (value == null)
            {
                parameter.Value = DBNull.Value;
            }
            else
            {
                var type = value.GetType();
                if (type.IsEnum)
                {
                    // PostgreSQL .NET driver wont cast enum to int
                    parameter.Value = (int)value;
                }
                else if (type == typeof(Guid))
                {
                    // TODO: why??
                    parameter.Value = value.ToString();
                    parameter.DbType = DbType.String;
                    parameter.Size = 40;
                }
                else if (type == typeof(string))
                {
                    // out of memory exception occurs if trying to save more than 4000 characters to SQL Server CE NText column.
                    // Set before attempting to set Size, or Size will always max out at 4000
                    var length = (value as string ?? Convert.ToString(value, CultureInfo.CurrentCulture))?.Length;

                    if (length + 1 > 4000 && parameter.GetType().Name == "SqlCeParameter")
                    {
                        parameter.GetType().GetProperty("SqlDbType")?.SetValue(parameter, SqlDbType.NText, null);
                    }

                    // Help query plan caching by using common size
                    parameter.Size = Math.Max(length.GetValueOrDefault() + 1, 4000);
                    parameter.Value = value;
                }
                else if (type == typeof(AnsiString))
                {
                    // Thanks @DataChomp for pointing out the SQL Server indexing performance hit of using wrong string type on varchar
                    parameter.Size = Math.Max((value as AnsiString ?? new AnsiString(string.Empty)).Value.Length + 1, 4000);
                    parameter.Value = (value as AnsiString ?? new AnsiString(string.Empty)).Value;
                    parameter.DbType = DbType.AnsiString;
                }
                else if (value.GetType().Name == "SqlGeography")
                {
                    // SqlGeography is a CLR Type
                    parameter.GetType().GetProperty("UdtTypeName")?.SetValue(parameter, "geography", null);

                    // geography is the equivalent SQL Server Type
                    parameter.Value = value;
                }
                else if (value.GetType().Name == "SqlGeometry")
                {
                    // SqlGeometry is a CLR Type
                    parameter.GetType()
                        .GetProperty("UdtTypeName")
                        ?.SetValue(parameter, "geometry", null); // geography is the equivalent SQL Server Type
                    parameter.Value = value;
                }
                else
                {
                    parameter.Value = value;
                }
            }

            return parameter;
        }

        /// <summary>
        /// The get selection statement.
        /// </summary>
        /// <param name="entitySet">
        /// The entity set.
        /// </param>
        /// <param name="qualifiedColumns">
        /// The qualified columns.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected string GetSelectionStatement([NotNull] IEntitySet entitySet, [NotNull] IEnumerable<string> qualifiedColumns)
        {
            if (entitySet == null)
            {
                throw new ArgumentNullException(nameof(entitySet));
            }

            if (qualifiedColumns == null)
            {
                throw new ArgumentNullException(nameof(qualifiedColumns));
            }

            var entityDefinition = this.DefinitionProvider.Resolve(entitySet.EntityType);
            var selectionStatement = this.CreateSelectionStatement(entitySet, entityDefinition, qualifiedColumns, 0, 0);
            return entitySet.ParentExpression == null
                       ? selectionStatement
                       : this.CreateTableExpression(entitySet, entityDefinition, selectionStatement);
        }

        /// <summary>
        /// The capture inserted identity.
        /// </summary>
        /// <param name="commandText">
        /// The command text.
        /// </param>
        /// <param name="definition">
        /// The definition.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected abstract string CaptureInsertedIdentity(string commandText, IEntityDefinition definition);

        /// <summary>
        /// Gets an inclusion filter for the specified filter values and column.
        /// </summary>
        /// <param name="qualifiedName">
        /// The qualified name of the column.
        /// </param>
        /// <param name="filterIndex">
        /// The index at which the filter will be inserted.
        /// </param>
        /// <param name="filterValues">
        /// The filter values.
        /// </param>
        /// <returns>
        /// An inclusion predicate for the <paramref name="filterValues"/> as a <see cref="string"/>.
        /// </returns>
        protected virtual string GetInclusionFilter(string qualifiedName, int filterIndex, IEnumerable<object> filterValues)
        {
            var indexTokens = filterValues.Select(
                (_, i) => this.NameQualifier.AddParameterPrefix((filterIndex + i).ToString(CultureInfo.InvariantCulture)));

            var inclusionToken = string.Format(
                CultureInfo.InvariantCulture,
                InclusionPredicate,
                qualifiedName,
                string.Join(ParameterSeparator, indexTokens));

            return inclusionToken;
        }

        /// <summary>
        /// Gets an exclusion filter for the specified filter values and column.
        /// </summary>
        /// <param name="qualifiedName">
        /// The qualified name of the column.
        /// </param>
        /// <param name="filterIndex">
        /// The index at which the filter will be inserted.
        /// </param>
        /// <param name="filterValues">
        /// The filter values.
        /// </param>
        /// <returns>
        /// An exclusion predicate for the <paramref name="filterValues"/> as a <see cref="string"/>.
        /// </returns>
        protected virtual string GetExclusionFilter(string qualifiedName, int filterIndex, IEnumerable<object> filterValues)
        {
            var indexTokens = filterValues.Select(
                (_, i) => this.NameQualifier.AddParameterPrefix((filterIndex + i).ToString(CultureInfo.InvariantCulture)));

            var inclusionToken = string.Format(
                CultureInfo.InvariantCulture,
                ExclusionPredicate,
                qualifiedName,
                string.Join(ParameterSeparator, indexTokens));

            return inclusionToken;
        }

        /// <summary>
        /// Creates a selection link statement for the specified type.
        /// </summary>
        /// <param name="linkType">
        /// The link type.
        /// </param>
        /// <returns>
        /// The selection link statement as a <see cref="string"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="linkType"/> is not one of the named enumerations.
        /// </exception>
        private static string CreateLinkStatement(SelectionLinkType linkType)
        {
            string linkStatement;

            switch (linkType)
            {
                case SelectionLinkType.Union:
                    linkStatement = UnionStatement;
                    break;
                case SelectionLinkType.Intersection:
                    linkStatement = IntersectionStatement;
                    break;
                case SelectionLinkType.Exception:
                    linkStatement = ExceptionStatement;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(linkType));
            }

            return linkStatement;
        }

        /// <summary>
        /// Creates the selection statement for the current selection.
        /// </summary>
        /// <param name="entitySet">
        /// The selection to create a statement for.
        /// </param>
        /// <param name="entityDefinition">
        /// The entity definition for the selection statement.
        /// </param>
        /// <param name="qualifiedColumns">
        /// A collection of qualified column names to select.
        /// </param>
        /// <param name="indexOffset">
        /// The index offset.
        /// </param>
        /// <param name="indent">
        /// The indent in spaces for the statement.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="entitySet"/>, <paramref name="entityDefinition"/>, or <paramref name="qualifiedColumns"/> is null.
        /// </exception>
        /// <returns>
        /// The T-SQL statement for the current selection as a <see cref="string"/>.
        /// </returns>
        private string CreateSelectionStatement(
            [NotNull] IEntitySet entitySet,
            [NotNull] IEntityDefinition entityDefinition,
            [NotNull] IEnumerable<string> qualifiedColumns,
            int indexOffset,
            int indent)
        {
            if (entitySet == null)
            {
                throw new ArgumentNullException(nameof(entitySet));
            }

            if (entityDefinition == null)
            {
                throw new ArgumentNullException(nameof(entityDefinition));
            }

            if (qualifiedColumns == null)
            {
                throw new ArgumentNullException(nameof(qualifiedColumns));
            }

            var separator = string.Concat(",", Environment.NewLine, new string(' ', indent + 4));
            var selectColumns = string.Join(separator, qualifiedColumns);
            var entitySetStatement = this.CreateEntitySetStatement(entitySet, entityDefinition, indexOffset, indent);

            var orderByClause = new Lazy<string>(
                () => string.Concat(new string(' ', indent), "ORDER BY ", this.CreateOrderByClause(entitySet.OrderByExpressions)));

            var fetchSelect = new StringBuilder();

            if (entitySet.Page.RowOffset + entitySet.Page.Size > 0)
            {
                if (entitySet.OrderByExpressions.Any() == false)
                {
                    // If we don't have an ORDER BY clause then we can't use OFFSET/FETCH NEXT.
                    throw new RepositoryException(entitySet, "Paging is not supported when no order expressions have been set.");
                }

                fetchSelect.AppendLine().Append(new string(' ', indent));
                fetchSelect.Append($"OFFSET @{indexOffset + entitySet.PropertyValues.Count() - 2} ROWS");

                if (entitySet.Page.Size > 0)
                {
                    fetchSelect.AppendLine().Append(new string(' ', indent));
                    fetchSelect.Append($"FETCH NEXT @{indexOffset + entitySet.PropertyValues.Count() - 1} ROWS ONLY");
                }
            }

            return string.Concat(
                SelectStatement,
                Environment.NewLine,
                new string(' ', indent + 4),
                selectColumns,
                Environment.NewLine,
                entitySetStatement,
                entitySet.OrderByExpressions.Any() ? string.Concat(Environment.NewLine, orderByClause.Value) : string.Empty,
                fetchSelect.ToString());
        }

        /// <summary>
        /// Creates an entity set statement (excludes selection columns).
        /// </summary>
        /// <param name="entitySet">
        /// The entity set.
        /// </param>
        /// <param name="entityDefinition">
        /// The entity definition.
        /// </param>
        /// <param name="indexOffset">
        /// The index offset.
        /// </param>
        /// <param name="indent">
        /// The indent for the statement.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="entitySet"/> or <paramref name="entityDefinition"/> is null.
        /// </exception>
        /// <returns>
        /// The entity set statement as a <see cref="string"/>.
        /// </returns>
        private string CreateEntitySetStatement(
            [NotNull] IEntitySet entitySet,
            [NotNull] IEntityDefinition entityDefinition,
            int indexOffset,
            int indent)
        {
            if (entitySet == null)
            {
                throw new ArgumentNullException(nameof(entitySet));
            }

            if (entityDefinition == null)
            {
                throw new ArgumentNullException(nameof(entityDefinition));
            }

            var hasFilters = entitySet.ParentExpression != null || entitySet.Filters.Any();

            var fromClause = string.Concat(
                new string(' ', indent),
                FromStatement,
                $"{this.NameQualifier.Escape(entityDefinition.EntityContainer)}.{this.NameQualifier.Escape(entityDefinition.EntityName)}");

            var joinClause = new JoinClause(this.DefinitionProvider, this.NameQualifier)
                             {
                                 Indent = indent
                             };

            var joinClauseText = joinClause.Create(entitySet.Relations);
            var filter = new StringBuilder(new string(' ', indent)).Append(SqlWhereClause);

            if (entitySet.ParentExpression != null)
            {
                // TODO: Support LEFT JOIN? This will have to be done in the JOIN clause.
                // For now, assume INNER JOIN and create an EXISTS clause.
                var pageTableName = this.NameQualifier.Escape(entitySet.ParentExpression.Name);
                var keyRelations = entitySet.ParentExpression.Relations.ToList();

                var keyPredicate = string.Join(
                    string.Concat(FilterSeparator, Environment.NewLine),
                    keyRelations.Select(
                        relation => $"{pageTableName}.{this.NameQualifier.Escape(entityDefinition.Find(relation.SourceExpression).ReferenceName)} = "
                                    + $"{this.GetQualifiedColumnName(entityDefinition.Find(relation.RelationExpression))}"));

                filter.Append(
                    $@"EXISTS 
(SELECT 1 FROM {pageTableName} 
WHERE {keyPredicate})");
            }

            if (entitySet.Filters.Any())
            {
                if (entitySet.ParentExpression != null)
                {
                    filter.AppendLine(" AND");
                }

                filter.Append(this.CreateFilter(entityDefinition, entitySet.Filters, indexOffset, indent));
            }

            var fullSet = string.Concat(
                fromClause,
                entitySet.Relations.Any() ? string.Concat(Environment.NewLine, joinClauseText) : string.Empty,
                hasFilters ? string.Concat(Environment.NewLine, filter.ToString()) : string.Empty);

            return fullSet;
        }

        /// <summary>
        /// Creates a table expression statement.
        /// </summary>
        /// <param name="entitySet">
        /// The selection to process.
        /// </param>
        /// <param name="definition">
        /// The entity definition.
        /// </param>
        /// <param name="selectionStatement">
        /// The selection statement to encapsulate with the parent expression.
        /// </param>
        /// <returns>
        /// The selection statement as a <see cref="string"/>.
        /// </returns>
        private string CreateTableExpression(
            [NotNull] IEntitySet entitySet,
            [NotNull] IEntityDefinition definition,
            [NotNull] string selectionStatement)
        {
            if (entitySet == null)
            {
                throw new ArgumentNullException(nameof(entitySet));
            }

            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            if (string.IsNullOrWhiteSpace(selectionStatement))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(selectionStatement));
            }

            var pageTableName = this.NameQualifier.Escape(entitySet.ParentExpression.Name);

            var tableSelection = entitySet.ParentExpression.Expression;
            var qualifiedColumns = this.GetQualifiedColumns(tableSelection, definition);

            var indexOffset = entitySet.PropertyValues.Count() - entitySet.ParentExpression.Expression.PropertyValues.Count();
            var tableExpressionStatement = this.CreateSelectionStatement(tableSelection, definition, qualifiedColumns, indexOffset, 8);

            var fullStatement = $@";WITH {pageTableName} AS
    (
        {tableExpressionStatement}
    )
{selectionStatement}";

            return fullStatement;
        }

        /// <summary>
        /// Creates a complete selection statement.
        /// </summary>
        /// <param name="selection">
        /// The selection to process.
        /// </param>
        /// <param name="definition">
        /// The entity definition.
        /// </param>
        /// <param name="qualifiedColumns">
        /// The qualified columns to select within the statement.
        /// </param>
        /// <returns>
        /// The selection statement as a <see cref="string"/>.
        /// </returns>
        private string CreateCompleteStatement(IEntitySet selection, IEntityDefinition definition, IEnumerable<string> qualifiedColumns)
        {
            int offset = 0;
            var statement = this.CreateSelectionStatement(
                selection,
                definition,
                qualifiedColumns ?? this.GetQualifiedColumns(selection, definition),
                offset,
                0);

            offset = selection.Filters.SelectMany(ValueFilter.SelectNonNullValues).Count();
            var linkedSelection = selection.LinkedSelection;
            var linkSelectionsList = new List<Tuple<string, string>>();

            while (linkedSelection != null)
            {
                var linkType = linkedSelection.LinkType;
                var linkStatement = CreateLinkStatement(linkType);
                var columns = qualifiedColumns ?? this.GetQualifiedColumns(linkedSelection.Selection, definition);
                var selectionStatement = this.CreateSelectionStatement(linkedSelection.Selection, definition, columns, offset, 0);

                linkSelectionsList.Add(new Tuple<string, string>(linkStatement, selectionStatement));
                offset += linkedSelection.Selection.Filters.SelectMany(ValueFilter.SelectNonNullValues).Count();
                linkedSelection = linkedSelection.Selection.LinkedSelection;
            }

            // If linked, protect each clause with parentheses
            var isLinked = linkSelectionsList.Any();
            return string.Concat(
                isLinked ? $"({statement})" : statement,
                isLinked ? Environment.NewLine : string.Empty,
                string.Join(Environment.NewLine, from ls in linkSelectionsList select $"{ls.Item1}{Environment.NewLine}({ls.Item2})"));
        }

        /// <summary>
        /// Gets qualified columns for an entity <paramref name="set"/>.
        /// </summary>
        /// <param name="set">
        /// The entity set.
        /// </param>
        /// <param name="definition">
        /// The entity definition.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of qualified columns for the selection.
        /// </returns>
        /// <remarks>
        /// If the <paramref name="set"/> is not an <see cref="ISelection"/>, then a column ordinal of '1' will be returned.
        /// </remarks>
        private IEnumerable<string> GetQualifiedColumns(IEntitySet set, IEntityDefinition definition)
        {
            if (set is ISelection selection && selection.SelectExpressions.Any())
            {
                return selection.SelectExpressions.Select(
                    expression => this.GetColumnSelection(definition, expression.AttributeExpression, expression.AggregateFunction));
            }

            return definition.ReturnableAttributes.Select(this.GetQualifiedColumnName);
        }

        /// <summary>
        /// Gets the column selection clause for the specified selection expression.
        /// </summary>
        /// <param name="entityDefinition">
        /// The entity definition.
        /// </param>
        /// <param name="attributeExpression">
        /// The attribute expression.
        /// </param>
        /// <param name="aggregateFunction">
        /// The aggregate function.
        /// </param>
        /// <returns>
        /// The column selection as a <see cref="string"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="aggregateFunction"/> is not one of the enumerated values.
        /// </exception>
        private string GetColumnSelection(
            [NotNull] IEntityDefinition entityDefinition,
            [NotNull] LambdaExpression attributeExpression,
            AggregateFunction aggregateFunction)
        {
            if (entityDefinition == null)
            {
                throw new ArgumentNullException(nameof(entityDefinition));
            }

            if (attributeExpression == null)
            {
                throw new ArgumentNullException(nameof(attributeExpression));
            }

            var attribute = entityDefinition.Find(attributeExpression);

            if (attribute == default)
            {
                throw new RepositoryException(
                    attributeExpression,
                    $"The expression '{attributeExpression}' could not be found in the definition for '{entityDefinition}'.");
            }

            switch (aggregateFunction)
            {
                case AggregateFunction.None:
                    return this.GetQualifiedColumnName(attribute);
                case AggregateFunction.Count:
                    return $"{aggregateFunction.ToString().ToUpperInvariant()}({this.GetQualifiedColumnName(attribute)})";
                default:
                    throw new ArgumentOutOfRangeException(nameof(aggregateFunction));
            }
        }

        /// <summary>
        /// The get column selection.
        /// </summary>
        /// <param name="attribute">
        /// The attribute to create a column selection for.
        /// </param>
        /// <returns>
        /// The column specification as a <see cref="string"/>.
        /// </returns>
        private string GetQualifiedColumnName(EntityAttributeDefinition attribute)
        {
            var qualifiedName = this.NameQualifier.Qualify(attribute);
            var referenceName = this.NameQualifier.GetReferenceName(attribute);

            var qualifiedColumnName = string.IsNullOrWhiteSpace(attribute.Alias)
                                          ? qualifiedName
                                          : string.Format(
                                              CultureInfo.InvariantCulture,
                                              AliasColumnFormat,
                                              referenceName,
                                              this.NameQualifier.Escape(attribute.Alias));

            return qualifiedColumnName;
        }

        /// <summary>
        /// Creates a filter for the current selection.
        /// </summary>
        /// <param name="entityDefinition">
        /// The entity definition.
        /// </param>
        /// <param name="filters">
        /// The filters to apply.
        /// </param>
        /// <param name="indexOffset">
        /// The index offset.
        /// </param>
        /// <returns>
        /// The filter clause as a <see cref="string"/>.
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">
        /// The number of filter values is outside the range handled by the method.
        /// </exception>
        private string CreatePredicateClause(IEntityDefinition entityDefinition, IEnumerable<IValueFilter> filters, int indexOffset)
        {
            return this.CreateFilter(entityDefinition, filters, indexOffset, DefaultIndent);
        }

        /// <summary>
        /// Creates a filter for the current selection.
        /// </summary>
        /// <param name="entityDefinition">
        /// The entity definition.
        /// </param>
        /// <param name="filters">
        /// The filters to apply.
        /// </param>
        /// <param name="indexOffset">
        /// The index offset.
        /// </param>
        /// <param name="indent">
        /// The indent to apply to each filter clause.
        /// </param>
        /// <returns>
        /// The filter clause as a <see cref="string"/>.
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">
        /// The number of filter values is outside the range handled by the method.
        /// </exception>
        private string CreateFilter(IEntityDefinition entityDefinition, IEnumerable<IValueFilter> filters, int indexOffset, int indent)
        {
            if (filters == null)
            {
                throw new ArgumentNullException(nameof(filters));
            }

            var filterBuilder = new StringBuilder();
            var index = indexOffset;
            var stringIndent = new string(' ', indent);

            var filterTokens = new List<string>();
            var valueFilters = filters.ToList();
            var subQueryCount = 0;

            foreach (var filter in valueFilters)
            {
                var filterDefinition = this.DefinitionProvider.Resolve(filter.AttributeLocation.EntityReference.EntityType);

                if (filter is RelationExpression subQuery)
                {
                    subQueryCount++;
                    var relation = subQuery.Relations.First();
                    var entityReference = this.DefinitionProvider.GetEntityReference(relation.RelationExpression);
                    var relationDefinition = this.DefinitionProvider.Resolve(entityReference.EntityType);
                    var subQueryEntityAlias =
                        this.NameQualifier.Escape(relation.RelationEntityAlias ?? $"{relationDefinition.EntityName}{subQueryCount}");
                    var relationEntityReference =
                        $"{this.NameQualifier.Escape(relationDefinition.EntityContainer)}.{this.NameQualifier.Escape(relationDefinition.EntityName)}";

                    var entitySet = subQuery.EntitySet;

                    var subQueryText = $@"{this.GetQualifiedColumnName(entityDefinition.Find(relation.SourceExpression))} IN
{stringIndent}(SELECT {subQueryEntityAlias}.{this.NameQualifier.Escape(relationDefinition.Find(relation.RelationExpression).ReferenceName)}
{stringIndent}{FromStatement}{relationEntityReference} AS {subQueryEntityAlias}
{new JoinClause(this.DefinitionProvider, this.NameQualifier) { Indent = indent, AliasSuffix = $"{subQueryCount}" }.Create(entitySet.Relations)}";

                    if (entitySet.Filters.Any())
                    {
                        var subQueryTokens = new List<string>();

                        foreach (var subFilter in entitySet.Filters)
                        {
                            var subFilterDefinition = this.DefinitionProvider.Resolve(subFilter.AttributeLocation.EntityReference.EntityType);
                            var attribute = this.GetAttributeDefinition(subFilterDefinition, subFilter.AttributeLocation);
                            var entityAlias = this.NameQualifier.Escape($"{attribute.Entity.Alias ?? attribute.Entity.Name}{subQueryCount}");
                            var referenceName = $"{entityAlias}.{this.NameQualifier.Escape(attribute.Alias ?? attribute.PhysicalName)}";
                            var setValues = subFilter.FilterValues.Where(Evaluate.IsSet).ToList();
                            index = this.AddTokens(
                                subFilter.FilterType,
                                subFilter.FilterValues.First(),
                                subQueryTokens,
                                referenceName,
                                setValues,
                                index);
                        }

                        subQueryText = string.Concat(
                            subQueryText,
                            Environment.NewLine,
                            stringIndent,
                            WhereClause,
                            ' ',
                            string.Join(string.Concat(FilterSeparator, Environment.NewLine, stringIndent), subQueryTokens));
                    }

                    filterTokens.Add(string.Concat(subQueryText, ')'));
                }
                else if (entityDefinition.Find(filter.AttributeLocation) == default)
                {
                    var referenceName = this.GetReferenceName(filterDefinition, filter.AttributeLocation);
                    var setValues = filter.FilterValues.Where(Evaluate.IsSet).ToList();
                    index = this.AddTokens(filter.FilterType, filter.FilterValues.First(), filterTokens, referenceName, setValues, index);
                }
                else
                {
                    var referenceName = this.GetReferenceName(entityDefinition, filter.AttributeLocation);
                    var setValues = filter.FilterValues.Where(Evaluate.IsSet).ToList();
                    index = this.AddTokens(filter.FilterType, filter.FilterValues.First(), filterTokens, referenceName, setValues, index);
                }
            }

            ////if (relationExpressions.Count > 0 && valueFilters.Count > 0)
            ////{
            ////    filterBuilder.Append(string.Concat(FilterSeparator, Environment.NewLine));
            ////}

            filterBuilder.Append(string.Join(string.Concat(FilterSeparator, Environment.NewLine, stringIndent), filterTokens));
            return filterBuilder.ToString();
        }

        /// <summary>
        /// Gets a reference name.
        /// </summary>
        /// <param name="entityDefinition">
        /// The entity definition.
        /// </param>
        /// <param name="attributeLocation">
        /// The location of the attribute.
        /// </param>
        /// <returns>
        /// The reference name of the attribute as a <see cref="string"/>.
        /// </returns>
        private string GetReferenceName(IEntityDefinition entityDefinition, AttributeLocation attributeLocation)
        {
            var attribute = this.GetAttributeDefinition(entityDefinition, attributeLocation);

            if (attribute == default)
            {
                throw new RepositoryException(
                    attributeLocation,
                    $"The attribute location '{attributeLocation}' could not be found in the definition for '{entityDefinition}'.");
            }

            return this.NameQualifier.GetReferenceName(attribute);
        }

        /// <summary>
        /// Gets the attribute definition for the specified <paramref name="attributeLocation"/>.
        /// </summary>
        /// <param name="entityDefinition">
        /// The entity definition.
        /// </param>
        /// <param name="attributeLocation">
        /// The attribute location.
        /// </param>
        /// <returns>
        /// The <see cref="EntityAttributeDefinition"/>.
        /// </returns>
        private EntityAttributeDefinition GetAttributeDefinition(IEntityDefinition entityDefinition, AttributeLocation attributeLocation)
        {
            var entityReference = this.DefinitionProvider.GetEntityReference(attributeLocation.PropertyInfo);
            var entityLocation = this.DefinitionProvider.GetEntityLocation(entityReference);

            return entityDefinition.Find(
                attributeLocation.EntityReference.EntityAlias ?? entityLocation.Alias ?? entityLocation.Name,
                attributeLocation.PropertyInfo.Name);
        }

        /// <summary>
        /// Adds tokens to the <paramref name="filterTokens"/> list.
        /// </summary>
        /// <param name="filterType">
        /// The filter type for this operation.
        /// </param>
        /// <param name="firstFilterValue">
        /// The first filter value of the filter. This will determine how equality is rendered.
        /// </param>
        /// <param name="filterTokens">
        /// The filter tokens.
        /// </param>
        /// <param name="referenceName">
        /// The reference name.
        /// </param>
        /// <param name="setValues">
        /// The set values.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The current index as an <see cref="int"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// <paramref name="filterType"/> is a value that is not supported by this operation.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="filterType"/> is outside the range of values for <see cref="FilterType"/>.
        /// </exception>
        private int AddTokens(
            FilterType filterType,
            object firstFilterValue,
            [NotNull] ICollection<string> filterTokens,
            [NotNull] string referenceName,
            [NotNull] IReadOnlyCollection<object> setValues,
            int index)
        {
            if (filterTokens == null)
            {
                throw new ArgumentNullException(nameof(filterTokens));
            }

            if (setValues == null)
            {
                throw new ArgumentNullException(nameof(setValues));
            }

            if (string.IsNullOrWhiteSpace(referenceName))
            {
                throw new ArgumentException(ErrorMessages.ValueCannotBeNullOrWhiteSpace, nameof(referenceName));
            }

            switch (filterType)
            {
                case FilterType.Equality:
                    var equalityItem = string.Format(
                        CultureInfo.InvariantCulture,
                        EqualityFilter,
                        referenceName,
                        firstFilterValue is string ? LikeOperand : EqualityOperand,
                        this.NameQualifier.AddParameterPrefix((index++).ToString(CultureInfo.InvariantCulture)));

                    filterTokens.Add(equalityItem);
                    break;
                case FilterType.Inequality:
                    var inequalityItem = string.Format(
                        CultureInfo.InvariantCulture,
                        EqualityFilter,
                        referenceName,
                        firstFilterValue is string ? NotLikeOperand : InequalityOperand,
                        this.NameQualifier.AddParameterPrefix((index++).ToString(CultureInfo.InvariantCulture)));

                    filterTokens.Add(inequalityItem);
                    break;
                case FilterType.LessThan:
                    filterTokens.Add(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            LessThanPredicate,
                            referenceName,
                            this.NameQualifier.AddParameterPrefix((index++).ToString(CultureInfo.InvariantCulture))));

                    break;
                case FilterType.LessThanOrEqualTo:
                    filterTokens.Add(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            LessThanOrEqualToPredicate,
                            referenceName,
                            this.NameQualifier.AddParameterPrefix((index++).ToString(CultureInfo.InvariantCulture))));

                    break;
                case FilterType.GreaterThan:
                    filterTokens.Add(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            GreaterThanPredicate,
                            referenceName,
                            this.NameQualifier.AddParameterPrefix((index++).ToString(CultureInfo.InvariantCulture))));

                    break;
                case FilterType.GreaterThanOrEqualTo:
                    filterTokens.Add(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            GreaterThanOrEqualToPredicate,
                            referenceName,
                            this.NameQualifier.AddParameterPrefix((index++).ToString(CultureInfo.InvariantCulture))));

                    break;
                case FilterType.Between:
                    filterTokens.Add(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            BetweenFilter,
                            referenceName,
                            this.NameQualifier.AddParameterPrefix((index++).ToString(CultureInfo.InvariantCulture)),
                            this.NameQualifier.AddParameterPrefix((index++).ToString(CultureInfo.InvariantCulture))));

                    break;
                case FilterType.MatchesSet:
                    filterTokens.Add(this.GetInclusionFilter(referenceName, index, setValues));
                    index += setValues.Count;
                    break;
                case FilterType.DoesNotMatchSet:
                    filterTokens.Add(this.GetExclusionFilter(referenceName, index, setValues));
                    index += setValues.Count;
                    break;
                case FilterType.IsNotNull:
                    filterTokens.Add(string.Format(CultureInfo.InvariantCulture, NotNullPredicate, referenceName));
                    break;
                case FilterType.IsNull:
                    filterTokens.Add(string.Format(CultureInfo.InvariantCulture, NullPredicate, referenceName));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(filterType));
            }

            return index;
        }

        /// <summary>
        /// Creates an order by clause from the provided expressions.
        /// </summary>
        /// <param name="orderExpressions">
        /// The order expressions.
        /// </param>
        /// <returns>
        /// The ORDER BY clause as a <see cref="string"/>.
        /// </returns>
        private string CreateOrderByClause(IEnumerable<OrderExpression> orderExpressions)
        {
            var clauses = new List<string>();

            foreach (var orderExpression in orderExpressions)
            {
                var reference = this.DefinitionProvider.GetEntityReference(orderExpression.PropertyExpression);
                var location = this.DefinitionProvider.GetEntityLocation(reference);
                var entityDefinition = this.DefinitionProvider.Resolve(location.EntityType);
                var attribute = entityDefinition.DirectAttributes.FirstOrDefault(x => x.PropertyName == orderExpression.PropertyExpression.GetPropertyName());

                if (attribute == default)
                {
                    throw new RepositoryException(
                        location,
                        $"The attribute location '{location}' could not be found in the definition for '{entityDefinition}'.");
                }

                string referenceName = this.NameQualifier.Qualify(attribute, location);
                clauses.Add(orderExpression.OrderDescending ? $"{referenceName} DESC" : referenceName);
            }

            return string.Join(ParameterSeparator, clauses);
        }
    }
}