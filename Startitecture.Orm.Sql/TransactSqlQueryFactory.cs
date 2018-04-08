// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactSqlQueryFactory.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Sql
{
    using System;
    using System.Linq;

    using JetBrains.Annotations;

    using Startitecture.Orm.Model;
    using Startitecture.Orm.Query;

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
        /// The derived table statement.
        /// </summary>
        private const string DerivedTableStatement = "({0}) AS [{1}]";

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

        /// <inheritdoc />
        public string Create<TItem>([NotNull] ItemSelection<TItem> selection, StatementOutputType outputType)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            switch (outputType)
            {
                case StatementOutputType.Select:
                    return this.CreateCompleteStatement(selection, false);
                case StatementOutputType.Contains:
                    return string.Format(IfExistsClause, this.CreateCompleteStatement(selection, true));
                case StatementOutputType.Delete:
                    // Rely on the underlying entity definition for delimiters.
                    var primaryTableName = selection.ItemDefinition.QualifiedName;
                    var filter = selection.Filters.Any()
                                     ? string.Concat(
                                         Environment.NewLine,
                                         string.Format(SqlWhereClause, selection.Filters.CreateFilter(0)))
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
                        selection.Relations.CreateJoinClause(),
                        filter);

                default:
                    throw new ArgumentOutOfRangeException();
            }
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
        private static string GetQualifiedColumnName(EntityAttributeDefinition attribute)
        {
            var qualifiedColumnName = string.IsNullOrWhiteSpace(attribute.Alias)
                                          ? string.Format(SelectColumnFormat, attribute.GetQualifiedName())
                                          : string.Format(AliasColumnFormat, attribute.GetQualifiedName(), attribute.Alias);

            return qualifiedColumnName;
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
        /// Creates a complete selection statement.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of item being selected.
        /// </typeparam>
        /// <param name="itemSelection">
        /// The item selection to create the statement for.
        /// </param>
        /// <param name="isContains">
        /// A value indicating whether the statement is a contains-type statement.
        /// </param>
        /// <returns>
        /// The selection statement as a <see cref="string"/>.
        /// </returns>
        private string CreateCompleteStatement<TItem>(ItemSelection<TItem> itemSelection, bool isContains)
        {
            int offset = 0;
            var statement = this.CreateSelectionStatement(itemSelection, offset, isContains);
            offset = itemSelection.Filters.SelectMany(ValueFilter.SelectNonNullValues).Count();
            var linkedSelection = itemSelection.LinkedSelection;

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
            // Contains statements do not need any columns.
            string selectColumns = isContains
                                       ? '1'.ToString()
                                       : string.Join(
                                           string.Concat(",", Environment.NewLine),
                                           selection.PropertiesToReturn.Select(GetQualifiedColumnName));

            string fromClause;

            if (selection.SelectionSource == selection.ItemDefinition.EntityName)
            {
                // Select as we normally would. Do not add delimiters for tables.
                fromClause = string.Concat(FromStatement, selection.ItemDefinition.QualifiedName);
            }
            else
            {
                // Select the derived table as the current entity name.
                fromClause = string.Concat(
                    FromStatement,
                    Environment.NewLine,
                    string.Format(DerivedTableStatement, selection.SelectionSource, selection.ItemDefinition.EntityName));
            }

            var joinClause = selection.Relations.CreateJoinClause();
            var filter = selection.Filters.Any()
                             ? string.Concat(
                                 Environment.NewLine,
                                 string.Format(SqlWhereClause, selection.Filters.CreateFilter(indexOffset)))
                             : string.Empty;

            return string.Concat(
                selection.Limit > 0 ? string.Format(SelectTopStatement, selection.Limit) : SelectStatement,
                Environment.NewLine,
                selectColumns,
                Environment.NewLine,
                fromClause,
                selection.Relations.Any() ? string.Concat(Environment.NewLine, joinClause) : string.Empty,
                filter);
        }
    }
}