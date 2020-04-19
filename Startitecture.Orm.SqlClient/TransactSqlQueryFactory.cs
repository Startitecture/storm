// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactSqlQueryFactory.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using JetBrains.Annotations;

    using Query;

    using Startitecture.Core;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Sql;

    /// <summary>
    /// The Transact-SQL statement factory.
    /// </summary>
    public class TransactSqlQueryFactory : IQueryFactory
    {
        /// <summary>
        /// The select statement.
        /// </summary>
        private const string SelectStatement = "SELECT";

        /// <summary>
        /// The select top statement.
        /// </summary>
        private const string SelectTopStatement = "SELECT TOP {0}";

        /// <summary>
        /// The if exists clause.
        /// </summary>
        private const string IfExistsClause = @"IF EXISTS (
{0}
) SELECT 1  ELSE SELECT 0";

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
        /// The select column format.
        /// </summary>
        private const string SelectColumnFormat = "    {0}";

        /// <summary>
        /// The alias column format.
        /// </summary>
        private const string AliasColumnFormat = "    {0} AS [{1}]";

        /// <summary>
        /// The SQL where clause.
        /// </summary>
        private const string SqlWhereClause = "WHERE {0}";

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
        private const string LessThanPredicate = "{0} <= @{1}";

        /// <summary>
        /// The greater than predicate.
        /// </summary>
        private const string GreaterThanPredicate = "{0} >= @{1}";

        /// <summary>
        /// The like operand.
        /// </summary>
        private const string LikeOperand = "LIKE";

        /// <summary>
        /// The equality operand.
        /// </summary>
        private const string EqualityOperand = "=";

        /// <summary>
        /// The inclusive predicate.
        /// </summary>
        private const string InclusionPredicate = "{0} IN ({1})";

        /// <summary>
        /// The parameter format.
        /// </summary>
        private const string ParameterFormat = "@{0}";

        /// <summary>
        /// The parameter separator.
        /// </summary>
        private const string ParameterSeparator = ", ";

        /// <summary>
        /// The SQL qualifier.
        /// </summary>
        private static readonly TransactSqlQualifier SqlQualifier = Singleton<TransactSqlQualifier>.Instance;

        /// <summary>
        /// The definition provider.
        /// </summary>
        [NotNull]
        private readonly IEntityDefinitionProvider definitionProvider;

        /// <summary>
        /// The transact SQL join.
        /// </summary>
        private readonly JoinClause joinClause;

        /// <summary>
        /// The name qualifier.
        /// TODO: Create a base class with this concrete definition providing context
        /// </summary>
        private readonly INameQualifier nameQualifier = new TransactSqlQualifier();

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactSqlQueryFactory"/> class.
        /// </summary>
        /// <param name="definitionProvider">
        /// The definition provider.
        /// </param>
        public TransactSqlQueryFactory([NotNull] IEntityDefinitionProvider definitionProvider)
        {
            this.definitionProvider = definitionProvider ?? throw new ArgumentNullException(nameof(definitionProvider));
            this.joinClause = new JoinClause(this.definitionProvider, this.nameQualifier);
        }

        /// <inheritdoc />
        public string Create<TItem>([NotNull] QueryContext<TItem> queryContext)
        {
            if (queryContext == null)
            {
                throw new ArgumentNullException(nameof(queryContext));
            }

            var selection = queryContext.Selection;
            var entityDefinition = this.definitionProvider.Resolve<TItem>();

            return this.CompleteStatement(queryContext, queryContext.OutputType, entityDefinition, selection);
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
        /// Gets the operand for the specified value.
        /// </summary>
        /// <param name="value">
        /// The value to return an operand for.
        /// </param>
        /// <returns>
        /// The operand as a <see cref="string"/>.
        /// </returns>
        private static string GetEqualityOperand(object value)
        {
            return value is string ? LikeOperand : EqualityOperand;
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
        /// <paramref name="filterType"/> is outside the range of values for <see cref="Startitecture.Orm.Query.FilterType"/>.
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
                    filterTokens.Add(
                        string.Format(CultureInfo.InvariantCulture, EqualityFilter, referenceName, GetEqualityOperand(firstFilterValue), index++));
                    break;
                case FilterType.Inequality:
                    throw new NotImplementedException();
                case FilterType.LessThan:
                    throw new NotImplementedException();
                case FilterType.LessThanOrEqualTo:
                    filterTokens.Add(string.Format(CultureInfo.InvariantCulture, LessThanPredicate, referenceName, index++));
                    break;
                case FilterType.GreaterThan:
                    throw new NotImplementedException();
                case FilterType.GreaterThanOrEqualTo:
                    filterTokens.Add(string.Format(CultureInfo.InvariantCulture, GreaterThanPredicate, referenceName, index++));
                    break;
                case FilterType.Between:
                    filterTokens.Add(string.Format(CultureInfo.InvariantCulture, BetweenFilter, referenceName, index++, index++));
                    break;
                case FilterType.MatchesSet:
                    filterTokens.Add(GetInclusionFilter(referenceName, index, setValues));
                    index += setValues.Count;
                    break;
                case FilterType.DoesNotMatchSet:
                    throw new NotImplementedException();
                case FilterType.IsSet:
                    filterTokens.Add(string.Format(CultureInfo.InvariantCulture, NotNullPredicate, referenceName));
                    break;
                case FilterType.IsNotSet:
                    filterTokens.Add(string.Format(CultureInfo.InvariantCulture, NullPredicate, referenceName));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(filterType));
            }

            return index;
        }

        /// <summary>
        /// Gets a reference name from an <paramref name="location"/>.
        /// </summary>
        /// <param name="location">
        /// The location to get the reference name from.
        /// </param>
        /// <returns>
        /// The reference name as a <see cref="string"/>.
        /// </returns>
        private string GetReferenceName(EntityLocation location)
        {
            var isEntityAliased = string.IsNullOrWhiteSpace(location.Alias) == false;
            return isEntityAliased
                       ? this.nameQualifier.Escape(location.Alias)
                       : string.Concat(this.nameQualifier.Escape(location.Container), '.', this.nameQualifier.Escape(location.Name));
        }

        /// <summary>
        /// Gets a reference name from an <paramref name="attribute"/>.
        /// </summary>
        /// <param name="attribute">
        /// The attribute to get the reference name from.
        /// </param>
        /// <returns>
        /// The reference name as a <see cref="string"/>.
        /// </returns>
        private string GetReferenceName(EntityAttributeDefinition attribute)
        {
            return $"{this.GetReferenceName(attribute.Entity)}.[{attribute.PhysicalName}]";
        }

        /// <summary>
        /// The get column selection.
        /// </summary>
        /// <param name="attribute">
        /// The attribute to create a column selection for.
        /// </param>
        /// <returns>
        /// The column selection as a <see cref="string"/>.
        /// </returns>
        private string GetQualifiedColumnName(EntityAttributeDefinition attribute)
        {
            var qualifiedName = SqlQualifier.Qualify(attribute);
            var referenceName = this.GetReferenceName(attribute);

            var qualifiedColumnName = string.IsNullOrWhiteSpace(attribute.Alias)
                                          ? string.Format(CultureInfo.InvariantCulture, SelectColumnFormat, qualifiedName)
                                          : string.Format(CultureInfo.InvariantCulture, AliasColumnFormat, referenceName, attribute.Alias);

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

                var referenceName = this.GetReferenceName(attribute);
                var setValues = filter.FilterValues.Where(Evaluate.IsSet).ToList();

                index = AddTokens(filter.FilterType, filter.FilterValues.First(), filterTokens, referenceName, setValues, index);
            }

            return string.Join(string.Concat(FilterSeparator, Environment.NewLine), filterTokens);
        }

        /// <summary>
        /// Creates a complete selection statement.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of item being selected.
        /// </typeparam>
        /// <param name="queryContext">
        /// The query context.
        /// </param>
        /// <param name="isContains">
        /// A value indicating whether the statement is a contains-type statement.
        /// </param>
        /// <returns>
        /// The selection statement as a <see cref="string"/>.
        /// </returns>
        private string CreateCompleteStatement<TItem>(QueryContext<TItem> queryContext, bool isContains)
        {
            int offset = queryContext.ParameterOffset;
            var statement = this.CreateSelectionStatement(queryContext.Selection, offset, isContains);
            offset = queryContext.Selection.Filters.SelectMany(ValueFilter.SelectNonNullValues).Count();
            var linkedSelection = queryContext.Selection.LinkedSelection;

            while (linkedSelection != null)
            {
                var linkType = linkedSelection.LinkType;
                var linkStatement = CreateLinkStatement(linkType);

                statement = string.Concat(
                    statement,
                    Environment.NewLine,
                    linkStatement,
                    Environment.NewLine,
                    this.CreateSelectionStatement(linkedSelection.Selection, offset, isContains));

                offset += linkedSelection.Selection.Filters.SelectMany(ValueFilter.SelectNonNullValues).Count();
                linkedSelection = linkedSelection.Selection.LinkedSelection;
            }

            return statement;
        }

        /// <summary>
        /// Creates the selection statement for the current selection.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of item being selected.
        /// </typeparam>
        /// <param name="selection">
        /// The selection to create a statement for.
        /// </param>
        /// <param name="indexOffset">
        /// The index offset.
        /// </param>
        /// <param name="isContains">
        /// A value indicating whether the statement is a contains-type statement.
        /// </param>
        /// <returns>
        /// The T-SQL statement for the current selection as a <see cref="string"/>.
        /// </returns>
        private string CreateSelectionStatement<TItem>(ItemSelection<TItem> selection, int indexOffset, bool isContains)
        {
            var entityDefinition = this.definitionProvider.Resolve<TItem>();

            // Contains statements do not need any columns.
            var selectAttributes = selection.SelectExpressions.Select(entityDefinition.Find).ToList();

            // Add all returnable attributes if no explicit columns are selected.
            if (selectAttributes.Any() == false)
            {
                selectAttributes.AddRange(entityDefinition.ReturnableAttributes);
            }

            string selectColumns = isContains
                                       ? '1'.ToString(CultureInfo.InvariantCulture)
                                       : string.Join(string.Concat(",", Environment.NewLine), selectAttributes.Select(this.GetQualifiedColumnName));

            var fromClause = string.Concat(FromStatement, $"{this.nameQualifier.Escape(entityDefinition.EntityContainer)}.{this.nameQualifier.Escape(entityDefinition.EntityName)}");
            var joinClauseText = this.joinClause.Create(selection);
            var filter = selection.Filters.Any()
                             ? string.Concat(
                                 Environment.NewLine,
                                 string.Format(CultureInfo.InvariantCulture, SqlWhereClause, this.CreateFilter(entityDefinition, selection.Filters, indexOffset)))
                             : string.Empty;

            return string.Concat(
                selection.Limit > 0 ? string.Format(CultureInfo.InvariantCulture, SelectTopStatement, selection.Limit) : SelectStatement,
                Environment.NewLine,
                selectColumns,
                Environment.NewLine,
                fromClause,
                selection.Relations.Any() ? string.Concat(Environment.NewLine, joinClauseText) : string.Empty,
                filter);
        }

        /// <summary>
        /// Completes the query statement.
        /// </summary>
        /// <param name="queryContext">
        /// The query context.
        /// </param>
        /// <param name="queryContextOutputType">
        /// The query context output type.
        /// </param>
        /// <param name="entityDefinition">
        /// The entity definition.
        /// </param>
        /// <param name="selection">
        /// The selection.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item being queried.
        /// </typeparam>
        /// <returns>
        /// The query as a <see cref="string"/>.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="queryContextOutputType"/> is not a value in <see cref="Startitecture.Orm.Query.StatementOutputType"/>.
        /// </exception>
        private string CompleteStatement<TItem>(
            QueryContext<TItem> queryContext,
            StatementOutputType queryContextOutputType,
            IEntityDefinition entityDefinition,
            ItemSelection<TItem> selection)
        {
            var completeStatement = this.CreateFilter(entityDefinition, selection.Filters, queryContext.ParameterOffset);

            switch (queryContextOutputType)
            {
                case StatementOutputType.Select:
                    return this.CreateCompleteStatement(queryContext, false);
                case StatementOutputType.Contains:
                    return string.Format(CultureInfo.InvariantCulture, IfExistsClause, this.CreateCompleteStatement(queryContext, true));
                case StatementOutputType.Update:
                    return completeStatement;
                case StatementOutputType.Delete:
                    // Rely on the underlying entity definition for delimiters.
                    var primaryTableName = $"{this.nameQualifier.Escape(entityDefinition.EntityContainer)}.{this.nameQualifier.Escape(entityDefinition.EntityName)}";
                    var filter = selection.Filters.Any()
                                     ? string.Concat(
                                         Environment.NewLine,
                                         string.Format(CultureInfo.InvariantCulture, SqlWhereClause, completeStatement))
                                     : string.Empty;

                    if (selection.Relations.Any() == false)
                    {
                        return string.Concat(DeleteFromStatement, primaryTableName, filter);
                    }

                    return string.Concat(
                        DeleteStatement,
                        primaryTableName,
                        Environment.NewLine,
                        FromStatement,
                        primaryTableName,
                        Environment.NewLine,
                        this.joinClause.Create(selection), //// selection.Relations.CreateJoinClause(),
                        filter);

                default:
                    throw new ArgumentOutOfRangeException(nameof(queryContextOutputType));
            }
        }
    }
}