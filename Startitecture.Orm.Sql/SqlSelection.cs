// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlSelection.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Creates selections based on the Transact-SQL language.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Startitecture.Orm.Sql
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Startitecture.Core;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Schema;

    /// <summary>
    /// Creates selections based on the Transact-SQL language.
    /// </summary>
    /// <typeparam name="TItem">
    /// The type of item to generate the selections for.
    /// </typeparam>
    public class SqlSelection<TItem> : ItemSelection<TItem>
        where TItem : new()
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

        /// <summary>
        /// The selection source.
        /// </summary>
        private readonly string selectionSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlSelection{TItem}"/> class.
        /// </summary>
        public SqlSelection()
            : base(Singleton<PetaPocoDefinitionProvider>.Instance)
        {
            this.SetRelations(new TItem());
            var selectionAttribute = typeof(TItem).GetCustomAttributes<SelectWithAttribute>(true).FirstOrDefault();
            this.selectionSource = selectionAttribute == null ? this.ItemDefinition.EntityName : selectionAttribute.Statement;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlSelection{TItem}"/> class.
        /// </summary>
        /// <param name="example">
        /// The example to match.
        /// </param>
        /// <param name="selectors">
        /// The selectors of the properties to evaluate.
        /// </param>
        public SqlSelection(TItem example, params Expression<Func<TItem, object>>[] selectors)
            : this()
        {
            this.Matching(example, selectors);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlSelection{TItem}"/> class.
        /// </summary>
        /// <param name="example">
        /// The example to match.
        /// </param>
        /// <param name="propertyNames">
        /// The property names.
        /// </param>
        public SqlSelection(TItem example, IEnumerable<string> propertyNames)
            : this()
        {
            this.Matching(example, propertyNames);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlSelection{TItem}"/> class.
        /// </summary>
        /// <param name="lowerLimit">
        /// The item representing the lower limit.
        /// </param>
        /// <param name="upperLimit">
        /// The item representing the upper limit.
        /// </param>
        /// <param name="selectors">
        /// The selectors of the properties to evaluate.
        /// </param>
        internal SqlSelection(TItem lowerLimit, TItem upperLimit, params Expression<Func<TItem, object>>[] selectors)
            : this()
        {
            ////this.SetRelations(lowerLimit);
            this.Between(lowerLimit, upperLimit, selectors);
        }

        /// <summary>
        /// Gets or sets a value indicating whether null properties require a set value.
        /// </summary>
        public bool NullPropertiesRequireSetValue { get; set; }

        /// <summary>
        /// Gets the source of the selection.
        /// </summary>
        public override string SelectionSource
        {
            get
            {
                return this.selectionSource;
            }
        }

        /// <summary>
        /// Gets the selection statement for the current selection.
        /// </summary>
        public override string SelectionStatement
        {
            get
            {
                return this.CreateCompleteStatement(this, false);
            }
        }

        /// <summary>
        /// Gets a statement that determines whether the repository contains the current selection.
        /// </summary>
        public override string ContainsStatement
        {
            get
            {
                return string.Format(IfExistsClause, this.CreateCompleteStatement(this, true));
            }
        }

        /// <summary>
        /// Gets a statement that removes items from the repository based on the current selection.
        /// TODO: Either put this in its own operation that prevents UNIONs or fix updates to allow them.
        /// </summary>
        public override string RemovalStatement
        {
            get
            {
                // Rely on the underlying entity definition for delimiters.
                var primaryTableName = this.ItemDefinition.GetQualifiedName();
                var filter = this.Filters.Any()
                                 ? string.Concat(
                                     Environment.NewLine, 
                                     string.Format(SqlWhereClause, this.Filters.CreateFilter(0, this.NullPropertiesRequireSetValue)))
                                 : string.Empty;

                if (this.Relations.Any() == false)
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
                    this.Relations.CreateJoinClause(), 
                    filter);
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
                                          ? string.Format((string)SelectColumnFormat, (object)attribute.GetQualifiedName())
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
                    throw new ArgumentOutOfRangeException("linkType");
            }

            return linkStatement;
        }

        /// <summary>
        /// Creates a complete selection statement.
        /// </summary>
        /// <param name="itemSelection">
        /// The item selection to create the statement for.
        /// </param>
        /// <param name="isContains">
        /// A value indicating whether the statement is a contains-type statement.
        /// </param>
        /// <returns>
        /// The selection statement as a <see cref="string"/>.
        /// </returns>
        private string CreateCompleteStatement(ItemSelection<TItem> itemSelection, bool isContains)
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
        private string CreateSelectionStatement(ItemSelection<TItem> selection, int indexOffset, bool isContains)
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
                fromClause = string.Concat(FromStatement, selection.ItemDefinition.GetQualifiedName());
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
                                 string.Format(SqlWhereClause, selection.Filters.CreateFilter(indexOffset, this.NullPropertiesRequireSetValue)))
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

        /// <summary>
        /// Sets relations for the current example item.
        /// </summary>
        /// <param name="example">
        /// The example item to set relations for.
        /// </param>
        private void SetRelations(TItem example)
        {
            if (example is ICompositeEntity compositeEntity)
            {
                // Possible when the interface is implemented but the getter is not set.
                compositeEntity.ThrowOnDependencyFailure(entity => entity.EntityRelations);

                foreach (var entityRelation in compositeEntity.EntityRelations)
                {
                    this.AddRelation(entityRelation);
                }
            }
        }
    }
}
