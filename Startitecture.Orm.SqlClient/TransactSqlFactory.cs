// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactSqlFactory.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Orm.Model;

    /// <summary>
    /// The Transact-SQL statement factory.
    /// </summary>
    public class TransactSqlFactory : IStatementFactory
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
        private const string SetParameterFormat = "{0} = @{1}";

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
        private const string EqualityFilter = "{0} {1} @{2}";

        /// <summary>
        /// The between filter.
        /// </summary>
        private const string BetweenFilter = "{0} BETWEEN @{1} AND @{2}";

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
        private const string LessThanPredicate = "{0} < @{1}";

        /// <summary>
        /// The less than predicate.
        /// </summary>
        private const string LessThanOrEqualToPredicate = "{0} <= @{1}";

        /// <summary>
        /// The greater than predicate.
        /// </summary>
        private const string GreaterThanPredicate = "{0} > @{1}";

        /// <summary>
        /// The greater than predicate.
        /// </summary>
        private const string GreaterThanOrEqualToPredicate = "{0} >= @{1}";

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
        /// The parameter format.
        /// </summary>
        private const string ParameterFormat = "@{0}";

        /// <summary>
        /// The parameter separator.
        /// </summary>
        private const string ParameterSeparator = ", ";

        /// <summary>
        /// The default indent.
        /// </summary>
        private const int DefaultIndent = 0;

        /// <summary>
        /// The definition provider.
        /// </summary>
        [NotNull]
        private readonly IEntityDefinitionProvider definitionProvider;

        /// <summary>
        /// The name qualifier.
        /// TODO: Create a base class with this concrete definition providing context
        /// </summary>
        private readonly INameQualifier nameQualifier = new TransactSqlQualifier();

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactSqlFactory"/> class.
        /// </summary>
        /// <param name="definitionProvider">
        /// The definition provider.
        /// </param>
        public TransactSqlFactory([NotNull] IEntityDefinitionProvider definitionProvider)
        {
            this.definitionProvider = definitionProvider ?? throw new ArgumentNullException(nameof(definitionProvider));
        }

        /// <inheritdoc />
        /// <remarks>
        /// This implementation will create an IF EXISTS ({statement}) SELECT 1 ELSE SELECT 0 wrapper around the selection. For best performance,
        /// include only a single, non-nullable column in the selection, ideally a clustered key column.
        /// </remarks>
        public string CreateExistsStatement([NotNull] IEntitySet entitySet)
        {
            if (entitySet == null)
            {
                throw new ArgumentNullException(nameof(entitySet));
            }

            var entityDefinition = this.definitionProvider.Resolve(entitySet.EntityType);
            var qualifiedColumns = new List<string>
                                       {
                                           '1'.ToString()
                                       };

            var selectionStatement = this.CreateSelectionStatement(entitySet, entityDefinition, qualifiedColumns, 0, 0);
            var selectStatement = entitySet.ParentExpression == null
                                      ? this.CreateCompleteStatement(entitySet, entityDefinition)
                                      : this.CreateTableExpression(
                                          entitySet,
                                          entityDefinition,
                                          selectionStatement,
                                          entitySet.ParentExpression != null);

            return string.Format(CultureInfo.InvariantCulture, IfExistsClause, selectStatement);
        }

        /// <inheritdoc />
        /// <remarks>
        /// At this time, creating paging statements that include UNION, INTERSECT and EXCEPT clauses is not supported.
        /// </remarks>
        public string CreateSelectionStatement([NotNull] ISelection selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            var entityDefinition = this.definitionProvider.Resolve(selection.EntityType);
            var qualifiedColumns = this.GetQualifiedColumns(selection, entityDefinition);
            var selectionStatement = this.CreateSelectionStatement(selection, entityDefinition, qualifiedColumns, 0, 0);
            return selection.ParentExpression == null
                       ? this.CreateCompleteStatement(selection, entityDefinition)
                       : this.CreateTableExpression(selection, entityDefinition, selectionStatement, selection.OrderByExpressions.Any());
        }

        /// <inheritdoc />
        public string CreateUpdateStatement([NotNull] IUpdateSet updateSet)
        {
            if (updateSet == null)
            {
                throw new ArgumentNullException(nameof(updateSet));
            }

            var setClauses = new List<string>();
            int index = 0;
            var entityDefinition = this.definitionProvider.Resolve(updateSet.EntityType);

            foreach (var valueState in updateSet.AttributesToSet)
            {
                var attributeDefinition = entityDefinition.Find(valueState.AttributeLocation);

                // Skip attributes that can't be updated, as defined by the entity definition.
                if (entityDefinition.IsUpdateable(attributeDefinition) == false)
                {
                    continue;
                }

                var qualifiedName = this.nameQualifier.Qualify(attributeDefinition);
                setClauses.Add(
                    valueState.Value == null
                        ? string.Format(CultureInfo.CurrentCulture, SetNullParameterFormat, qualifiedName)
                        : string.Format(CultureInfo.CurrentCulture, SetParameterFormat, qualifiedName, index));

                if (valueState.Value != null)
                {
                    index++;
                }
            }

            var entityName = $"{this.nameQualifier.Escape(entityDefinition.EntityContainer)}.{this.nameQualifier.Escape(entityDefinition.EntityName)}";
            var joinClause = new JoinClause(this.definitionProvider, this.nameQualifier);
            string joinClauseText = updateSet.Relations.Any()
                                        ? string.Concat(
                                            Environment.NewLine,
                                            FromClause,
                                            entityName,
                                            Environment.NewLine,
                                            joinClause.Create(updateSet.Relations))
                                        : string.Empty;

            var setClause = string.Join(string.Concat(',', Environment.NewLine), setClauses);
            var predicateClause = this.CreateFilter(entityDefinition, updateSet.Filters, index);

            return string.Concat(
                string.Format(CultureInfo.InvariantCulture, SqlUpdateClause, entityName, setClause),
                joinClauseText,
                Environment.NewLine,
                WhereClause,
                Environment.NewLine,
                predicateClause);
        }

        /// <inheritdoc />
        public string CreateDeletionStatement([NotNull] IEntitySet entitySet)
        {
            if (entitySet == null)
            {
                throw new ArgumentNullException(nameof(entitySet));
            }

            var entityDefinition = this.definitionProvider.Resolve(entitySet.EntityType); //// statementContext.EntityDefinition;
            var primaryTableName = $"{this.nameQualifier.Escape(entityDefinition.EntityContainer)}.{this.nameQualifier.Escape(entityDefinition.EntityName)}";
            var filterStatement = this.CreateFilter(entityDefinition, entitySet.Filters, 0);
            var filter = entitySet.Filters.Any() ? string.Concat(Environment.NewLine, SqlWhereClause, filterStatement) : string.Empty;

            if (entitySet.Relations.Any() == false)
            {
                return string.Concat(DeleteFromStatement, primaryTableName, filter);
            }

            var joinClause = new JoinClause(this.definitionProvider, this.nameQualifier);

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
        private static string GetInclusionFilter(string qualifiedName, int filterIndex, IEnumerable<object> filterValues)
        {
            var indexTokens = filterValues.Select((o, i) => string.Format(CultureInfo.InvariantCulture, ParameterFormat, filterIndex + i));
            var inclusionToken = string.Format(CultureInfo.InvariantCulture, InclusionPredicate, qualifiedName, string.Join(ParameterSeparator, indexTokens));
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
        private static string GetExclusionFilter(string qualifiedName, int filterIndex, IEnumerable<object> filterValues)
        {
            var indexTokens = filterValues.Select((o, i) => string.Format(CultureInfo.InvariantCulture, ParameterFormat, filterIndex + i));
            var inclusionToken = string.Format(CultureInfo.InvariantCulture, ExclusionPredicate, qualifiedName, string.Join(ParameterSeparator, indexTokens));
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
        /// <exception cref="System.ArgumentOutOfRangeException">
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
        /// <exception cref="System.NotImplementedException">
        /// <paramref name="filterType"/> is a value that is not supported by this operation.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="filterType"/> is outside the range of values for <see cref="FilterType"/>.
        /// </exception>
        private static int AddTokens(
            FilterType filterType,
            object firstFilterValue,
            ICollection<string> filterTokens,
            string referenceName,
            IReadOnlyCollection<object> setValues,
            int index)
        {
            switch (filterType)
            {
                case FilterType.Equality:
                    var equalityItem = string.Format(
                        CultureInfo.InvariantCulture,
                        EqualityFilter,
                        referenceName,
                        firstFilterValue is string ? LikeOperand : EqualityOperand,
                        index++);

                    filterTokens.Add(equalityItem);
                    break;
                case FilterType.Inequality:
                    var inequalityItem = string.Format(
                        CultureInfo.InvariantCulture,
                        EqualityFilter,
                        referenceName,
                        firstFilterValue is string ? NotLikeOperand : InequalityOperand,
                        index++);

                    filterTokens.Add(inequalityItem);
                    break;
                case FilterType.LessThan:
                    filterTokens.Add(string.Format(CultureInfo.InvariantCulture, LessThanPredicate, referenceName, index++));
                    break;
                case FilterType.LessThanOrEqualTo:
                    filterTokens.Add(string.Format(CultureInfo.InvariantCulture, LessThanOrEqualToPredicate, referenceName, index++));
                    break;
                case FilterType.GreaterThan:
                    filterTokens.Add(string.Format(CultureInfo.InvariantCulture, GreaterThanPredicate, referenceName, index++));
                    break;
                case FilterType.GreaterThanOrEqualTo:
                    filterTokens.Add(string.Format(CultureInfo.InvariantCulture, GreaterThanOrEqualToPredicate, referenceName, index++));
                    break;
                case FilterType.Between:
                    filterTokens.Add(string.Format(CultureInfo.InvariantCulture, BetweenFilter, referenceName, index++, index++));
                    break;
                case FilterType.MatchesSet:
                    filterTokens.Add(GetInclusionFilter(referenceName, index, setValues));
                    index += setValues.Count;
                    break;
                case FilterType.DoesNotMatchSet:
                    filterTokens.Add(GetExclusionFilter(referenceName, index, setValues));
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

            switch (aggregateFunction)
            {
                case AggregateFunction.None:
                    return this.GetQualifiedColumnName(attribute);
                case AggregateFunction.Count:
                    return $"{aggregateFunction.ToString().ToUpperInvariant()}({this.GetQualifiedColumnName(attribute)})";
                default:
                    throw new ArgumentOutOfRangeException(nameof(aggregateFunction), aggregateFunction, @"The specified argument was out of range.");
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
            var qualifiedName = this.nameQualifier.Qualify(attribute);
            var referenceName = this.nameQualifier.GetReferenceName(attribute);

            var qualifiedColumnName = string.IsNullOrWhiteSpace(attribute.Alias)
                                          ? qualifiedName
                                          : string.Format(CultureInfo.InvariantCulture, AliasColumnFormat, referenceName, this.nameQualifier.Escape(attribute.Alias));

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
        /// <exception cref="System.IndexOutOfRangeException">
        /// The number of filter values is outside the range handled by the method.
        /// </exception>
        private string CreateFilter(IEntityDefinition entityDefinition, IEnumerable<ValueFilter> filters, int indexOffset)
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
        /// <exception cref="System.IndexOutOfRangeException">
        /// The number of filter values is outside the range handled by the method.
        /// </exception>
        private string CreateFilter(IEntityDefinition entityDefinition, IEnumerable<ValueFilter> filters, int indexOffset, int indent)
        {
            if (filters == null)
            {
                throw new ArgumentNullException(nameof(filters));
            }

            var filterTokens = new List<string>();
            var index = indexOffset;

            foreach (var filter in filters)
            {
                var entityReference = this.definitionProvider.GetEntityReference(filter.AttributeLocation.PropertyInfo);
                var entityLocation = this.definitionProvider.GetEntityLocation(entityReference);

                var attribute = entityDefinition.Find(
                    filter.AttributeLocation.EntityReference.EntityAlias ?? entityLocation.Alias ?? entityLocation.Name,
                    filter.AttributeLocation.PropertyInfo.Name);

                var referenceName = this.nameQualifier.GetReferenceName(attribute);
                var setValues = filter.FilterValues.Where(Evaluate.IsSet).ToList();

                index = AddTokens(filter.FilterType, filter.FilterValues.First(), filterTokens, referenceName, setValues, index);
            }

            return string.Join(string.Concat(FilterSeparator, Environment.NewLine, new string(' ', indent)), filterTokens);
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
        /// <returns>
        /// The selection statement as a <see cref="string"/>.
        /// </returns>
        private string CreateCompleteStatement(IEntitySet selection, IEntityDefinition definition)
        {
            int offset = 0;
            var statement = this.CreateSelectionStatement(selection, definition, this.GetQualifiedColumns(selection, definition), offset, 0);

            offset = selection.Filters.SelectMany(ValueFilter.SelectNonNullValues).Count();
            var linkedSelection = selection.LinkedSelection;
            var linkSelectionsList = new List<Tuple<string, string>>();

            while (linkedSelection != null)
            {
                var linkType = linkedSelection.LinkType;
                var linkStatement = CreateLinkStatement(linkType);
                var qualifiedColumns = this.GetQualifiedColumns(linkedSelection.Selection, definition);
                var selectionStatement = this.CreateSelectionStatement(linkedSelection.Selection, definition, qualifiedColumns, offset, 0);

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
        /// <returns>
        /// The T-SQL statement for the current selection as a <see cref="string"/>.
        /// </returns>
        private string CreateSelectionStatement(
            IEntitySet entitySet,
            IEntityDefinition entityDefinition,
            IEnumerable<string> qualifiedColumns,
            int indexOffset,
            int indent)
        {
            var separator = string.Concat(",", Environment.NewLine, new string(' ', indent + 4));
            var selectColumns = string.Join(separator, qualifiedColumns);
            var entitySetStatement = this.CreateEntitySetStatement(entitySet, entityDefinition, indexOffset, indent);

            if (entitySet is ISelection selection)
            {
                var orderByClause = string.Concat(new string(' ', indent), "ORDER BY ", this.CreateOrderByClause(selection.OrderByExpressions));
                var fetchSelect = new StringBuilder();

                if (selection.Page.RowOffset + selection.Page.Size > 0)
                {
                    fetchSelect.AppendLine().Append(new string(' ', indent));
                    fetchSelect.Append($"OFFSET @{indexOffset + selection.PropertyValues.Count() - 2} ROWS");

                    if (selection.Page.Size > 0)
                    {
                        fetchSelect.AppendLine().Append(new string(' ', indent));
                        fetchSelect.Append($"FETCH NEXT @{indexOffset + selection.PropertyValues.Count() - 1} ROWS ONLY");
                    }
                }

                return string.Concat(
                    SelectStatement,
                    Environment.NewLine,
                    new string(' ', indent + 4),
                    selectColumns,
                    Environment.NewLine,
                    entitySetStatement,
                    selection.OrderByExpressions.Any() ? string.Concat(Environment.NewLine, orderByClause) : string.Empty,
                    fetchSelect.ToString());
            }

            return string.Concat(
                SelectStatement,
                Environment.NewLine,
                new string(' ', indent + 4),
                selectColumns,
                Environment.NewLine,
                entitySetStatement);
        }

        /// <summary>
        /// Creates an entity set statement.
        /// </summary>
        /// <param name="set">
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
        /// <returns>
        /// The entity set statement as a <see cref="string"/>.
        /// </returns>
        private string CreateEntitySetStatement(IEntitySet set, IEntityDefinition entityDefinition, int indexOffset, int indent)
        {
            var hasFilters = set.ParentExpression != null || set.Filters.Any();

            var fromClause = string.Concat(
                new string(' ', indent),
                FromStatement,
                $"{this.nameQualifier.Escape(entityDefinition.EntityContainer)}.{this.nameQualifier.Escape(entityDefinition.EntityName)}");

            var joinClause = new JoinClause(this.definitionProvider, this.nameQualifier)
                                 {
                                     Indent = indent
                                 };
            var joinClauseText = joinClause.Create(set.Relations);
            var filter = new StringBuilder(new string(' ', indent)).Append(SqlWhereClause);

            if (set.ParentExpression != null)
            {
                // TODO: Support LEFT JOIN? This will have to be done in the JOIN clause.
                // For now, assume INNER JOIN and create an EXISTS clause.
                var pageTableName = this.nameQualifier.Escape(set.ParentExpression.TableName);
                var keyRelations = set.ParentExpression.TableRelations.ToList();

                var keyPredicate = string.Join(
                    string.Concat(FilterSeparator, Environment.NewLine),
                    keyRelations.Select(
                        relation => $"{pageTableName}.{this.nameQualifier.Escape(entityDefinition.Find(relation.SourceExpression).ReferenceName)} = "
                                    + $"{this.GetQualifiedColumnName(entityDefinition.Find(relation.RelationExpression))}"));

                filter.Append(
                    $@"EXISTS 
(SELECT 1 FROM {pageTableName} 
WHERE {keyPredicate})");
            }

            if (set.Filters.Any())
            {
                if (set.ParentExpression != null)
                {
                    filter.AppendLine(" AND");
                }

                filter.Append(this.CreateFilter(entityDefinition, set.Filters, indexOffset, indent));
            }

            var entitySet = string.Concat(
                fromClause,
                set.Relations.Any() ? string.Concat(Environment.NewLine, joinClauseText) : string.Empty,
                hasFilters ? string.Concat(Environment.NewLine, filter.ToString()) : string.Empty);
            return entitySet;
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
            var clauses = (from orderExpression in orderExpressions
                           let reference = this.definitionProvider.GetEntityReference(orderExpression.PropertyExpression)
                           let location = this.definitionProvider.GetEntityLocation(reference)
                           let attribute =
                               this.definitionProvider.Resolve(location.EntityType)
                                   .DirectAttributes.FirstOrDefault(x => x.PropertyName == orderExpression.PropertyExpression.GetPropertyName())
                           let referenceName = this.nameQualifier.Qualify(attribute, location)
                           select orderExpression.OrderDescending ? $"{referenceName} DESC" : referenceName).ToList();

            return string.Join(ParameterSeparator, clauses);
        }

        /// <summary>
        /// Creates a page selection statement.
        /// </summary>
        /// <param name="set">
        /// The selection to process.
        /// </param>
        /// <param name="definition">
        /// The entity definition.
        /// </param>
        /// <param name="selectionStatement">
        /// The selection statement to encapsulate with the parent expression.
        /// </param>
        /// <param name="orderByRecompile">
        /// A value indicating whether to have the statement use the RECOMPILE option.
        /// </param>
        /// <returns>
        /// The selection statement as a <see cref="string"/>.
        /// </returns>
        private string CreateTableExpression(IEntitySet set, IEntityDefinition definition, string selectionStatement, bool orderByRecompile)
        {
            var pageTableName = this.nameQualifier.Escape(set.ParentExpression.TableName);

            var tableSelection = set.ParentExpression.TableSelection;
            var qualifiedColumns = this.GetQualifiedColumns(tableSelection, definition);

            var indexOffset = set.PropertyValues.Count() - set.ParentExpression.TableSelection.PropertyValues.Count();
            var tableExpressionStatement = this.CreateSelectionStatement(tableSelection, definition, qualifiedColumns, indexOffset, 8);

            var fullStatement = $@";WITH {pageTableName} AS
    (
        {tableExpressionStatement}
    )
{selectionStatement}";

            if (orderByRecompile)
            {
                return fullStatement + " OPTION (RECOMPILE)";
            }

            return fullStatement;
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
            if (set is ISelection selection)
            {
                return selection.SelectExpressions.Any()
                           ? selection.SelectExpressions.Select(
                               expression => this.GetColumnSelection(definition, expression.AttributeExpression, expression.AggregateFunction))
                           : definition.ReturnableAttributes.Select(this.GetQualifiedColumnName);
            }

            return new List<string> { '1'.ToString() };
        }
    }
}