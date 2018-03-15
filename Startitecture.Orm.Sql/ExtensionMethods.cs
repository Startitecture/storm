// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Sql
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Schema;

    /// <summary>
    /// The extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
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
        /// The like operand.
        /// </summary>
        private const string LikeOperand = "LIKE";

        /// <summary>
        /// The equality operand.
        /// </summary>
        private const string EqualityOperand = "=";

        /// <summary>
        /// The less than predicate.
        /// </summary>
        private const string LessThanPredicate = "{0} <= @{1}";

        /// <summary>
        /// The greater than predicate.
        /// </summary>
        private const string GreaterThanPredicate = "{0} >= @{1}";

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
        /// The aliased relation statement format.
        /// </summary>
        private const string AliasedRelationStatementFormat = "{0} {1} AS [{2}] ON {3} = {4}";

        /// <summary>
        /// The relation statement format.
        /// </summary>
        private const string RelationStatementFormat = "{0} {1} ON {2} = {3}";

        /// <summary>
        /// The inner join clause.
        /// </summary>
        private const string InnerJoinClause = "INNER JOIN";

        /// <summary>
        /// The left join clause.
        /// </summary>
        private const string LeftJoinClause = "LEFT JOIN";

        /// <summary>
        /// The name selector.
        /// </summary>
        private static readonly Func<PropertyInfo, string> NameSelector = x => x.Name;

        /// <summary>
        /// A collection of all property names associated with the <see cref="ITransactionContext" /> interface.
        /// </summary>
        private static readonly IEnumerable<string> TransactionProperties = typeof(ITransactionContext).GetNonIndexedProperties().Select(NameSelector);

        /// <summary>
        /// Gets an example selection for the current item.
        /// </summary>
        /// <param name="example">
        /// The example item.
        /// </param>
        /// <param name="selectors">
        /// The property selectors.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item to generate an example selection for.
        /// </typeparam>
        /// <returns>
        /// A <see cref="T:SAF.Data.ExampleSelection`1"/> for the current item using the specified selectors.
        /// </returns>
        public static SqlSelection<TItem> ToExampleSelection<TItem>(this TItem example, params Expression<Func<TItem, object>>[] selectors)
            where TItem : ITransactionContext, new()
        {
            return new SqlSelection<TItem>(example, selectors);
        }

        /// <summary>
        /// Gets an example selection for the current item.
        /// </summary>
        /// <param name="lowerLimit">
        /// The item representing the lower limit.
        /// </param>
        /// <param name="upperLimit">
        /// The item representing the upper limit.
        /// </param>
        /// <param name="selectors">
        /// The property selectors.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item to generate an example selection for.
        /// </typeparam>
        /// <returns>
        /// A <see cref="T:SAF.Data.ExampleSelection`1"/> for the current item using the specified selectors.
        /// </returns>
        public static SqlSelection<TItem> ToRangeSelection<TItem>(
            this TItem lowerLimit,
            TItem upperLimit,
            params Expression<Func<TItem, object>>[] selectors)
            where TItem : ITransactionContext, new()
        {
            return new SqlSelection<TItem>(lowerLimit, upperLimit, selectors);
        }

        /// <summary>
        /// Creates a JOIN clause for the specified selection.
        /// </summary>
        /// <param name="selection">
        /// The item selection.
        /// </param>
        /// <returns>
        /// The JOIN clause as a <see cref="string"/>.
        /// </returns>
        public static string CreateJoinClause(this IEnumerable<IEntityRelation> selection)
        {
            return string.Join(Environment.NewLine, selection.Select(GenerateRelationStatement));
        }

        /// <summary>
        /// Gets the qualified name for the specified attribute.
        /// </summary>
        /// <param name="attribute">
        /// The attribute to evaluate.
        /// </param>
        /// <returns>
        /// The qualified name as a <see cref="string"/>.
        /// </returns>
        public static string GetQualifiedName(this EntityAttributeDefinition attribute)
        {
            return GetQualifiedName(attribute, null);
        }

        /// <summary>
        /// Gets the qualified name for the specified attribute.
        /// </summary>
        /// <param name="attribute">
        /// The attribute.
        /// </param>
        /// <returns>
        /// The qualified name as a <see cref="string"/>.
        /// </returns>
        public static string GetCanonicalName(this EntityAttributeDefinition attribute)
        {
            return string.Concat(attribute.Entity.GetCanonicalName(), '.', '[', attribute.PhysicalName, ']');
        }

        /// <summary>
        /// Creates a filter for the current selection.
        /// </summary>
        /// <param name="filters">
        /// The filters to apply.
        /// </param>
        /// <param name="indexOffset">
        /// The index offset.
        /// </param>
        /// <param name="nullValueIsNotNullPredicate">
        /// A value indicating whether a null value should be interpreted as a NOT NULL predicate.
        /// </param>
        /// <returns>
        /// The filter clause as a <see cref="string"/>.
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">
        /// The number of filter values is outside the range handled by the method.
        /// </exception>
        public static string CreateFilter(this IEnumerable<ValueFilter> filters, int indexOffset, bool nullValueIsNotNullPredicate)
        {
            if (filters == null)
            {
                throw new ArgumentNullException(nameof(filters));
            }

            var filterTokens = new List<string>();
            var index = indexOffset;

            foreach (var filter in filters)
            {
                var attribute = filter.ItemAttribute;
                var qualifiedName = attribute.GetQualifiedName();
                var count = filter.FilterValues.Count();
                var setValues = filter.FilterValues.Where(Evaluate.IsSet).ToList();

                var nullValuePredicate = nullValueIsNotNullPredicate ? NotNullPredicate : NullPredicate;

                switch (count)
                {
                    case 0:
                        filterTokens.Add(string.Format(nullValuePredicate, qualifiedName));
                        break;

                    case 1:
                        filterTokens.Add(string.Format(EqualityFilter, qualifiedName, GetEqualityOperand(filter.FilterValues.First()), index++));

                        break;

                    case 2:

                        if (filter.IsDiscrete)
                        {
                            filterTokens.Add(GetInclusionFilter(qualifiedName, index, setValues));
                            index += setValues.Count;
                        }
                        else
                        {
                            // If both values are null, add a NOT NULL predicate.
                            if (filter.FilterValues.All(Evaluate.IsNull))
                            {
                                filterTokens.Add(string.Format(nullValuePredicate, qualifiedName));
                            }
                            else if (filter.FilterValues.First() == null)
                            {
                                // If the first value is null, add a less than or equals (<=) predicate.
                                filterTokens.Add(string.Format(LessThanPredicate, qualifiedName, index++));
                            }
                            else if (filter.FilterValues.Last() == null)
                            {
                                // If the last value is null, add a greater than or equals (>=) predicate.
                                filterTokens.Add(string.Format(GreaterThanPredicate, qualifiedName, index++));
                            }
                            else
                            {
                                filterTokens.Add(string.Format(BetweenFilter, qualifiedName, index++, index++));
                            }
                        }

                        break;

                    default:
                        filterTokens.Add(GetInclusionFilter(qualifiedName, index, setValues));
                        index += setValues.Count;
                        break;
                }
            }

            return string.Join(string.Concat(FilterSeparator, Environment.NewLine), filterTokens);
        }

        /// <summary>
        /// Gets the table info from the generic type.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        /// <returns>
        /// The <see cref="Startitecture.Orm.Schema.TableInfo"/> for the specified type.
        /// </returns>
        public static TableInfo ToTableInfo(this IEntityDefinition definition)
        {
            // Get the table name
            var tableName = $"[{definition.EntityContainer}].[{definition.EntityName}]";

            // Get the primary key
            var primaryKeyDefinition = definition.PrimaryKeyAttributes.FirstOrDefault();

            ////var primaryKeyAttribute = pocoType.GetCustomAttributes(typeof(PrimaryKeyAttribute), true).OfType<PrimaryKeyAttribute>().FirstOrDefault();
            // TODO: Find another way to deal with Oracle or just forget it.
            ////var sequenceName = primaryKeyAttribute?.SequenceName;
            var primaryKey = primaryKeyDefinition.ReferenceName;
            var autoIncrement = primaryKeyDefinition.IsIdentityColumn;

            return new TableInfo(tableName, null) { AutoIncrement = autoIncrement, PrimaryKey = primaryKey };
        }

        /// <summary>
        /// Gets object values except for indexed and <see cref="Startitecture.Orm.Common.ITransactionContext"/> properties.
        /// </summary>
        /// <param name="obj">
        /// The <see cref="Startitecture.Orm.Common.ITransactionContext"/> to obtain the properties for.
        /// </param>
        /// <returns>
        /// A collection of <see cref="System.Object"/> items containing property values of the object.
        /// </returns>
        public static IEnumerable<object> ToValueCollection(this ITransactionContext obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            var values = new List<object>();

            var nonIndexedProperties = obj.GetType().GetNonIndexedProperties();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var propertyInfo in nonIndexedProperties.OrderBy(NameSelector))
            {
                if (TransactionProperties.Contains(propertyInfo.Name))
                {
                    continue;
                }

                values.Add(propertyInfo.GetPropertyValue(obj));
            }

            return values;
        }

        /// <summary>
        /// Gets the qualified name for the specified attribute.
        /// </summary>
        /// <param name="attribute">
        /// The attribute to evaluate.
        /// </param>
        /// <param name="entityAlias">
        /// The entity alias.
        /// </param>
        /// <returns>
        /// The qualified name as a <see cref="string"/>.
        /// </returns>
        private static string GetQualifiedName(this EntityAttributeDefinition attribute, string entityAlias)
        {
            var entityQualifiedName = string.IsNullOrWhiteSpace(entityAlias)
                                          ? attribute.Entity.ReferenceName
                                          : string.Concat('[', entityAlias, ']');

            return string.Concat(entityQualifiedName, '.', '[', attribute.PhysicalName, ']');
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
            var indexTokens = filterValues.Select((o, i) => string.Format(ParameterFormat, filterIndex + i));
            var inclusionToken = string.Format(InclusionPredicate, qualifiedName, string.Join(ParameterSeparator, indexTokens));
            return inclusionToken;
        }

        /// <summary>
        /// Generates a relation statement.
        /// </summary>
        /// <param name="entityRelation">
        /// The entity relation to generate a statement for.
        /// </param>
        /// <returns>
        /// The relation statement as a <see cref="string"/>.
        /// </returns>
        private static string GenerateRelationStatement(IEntityRelation entityRelation)
        {
            var joinType = GetJoinClause(entityRelation.RelationType);
            var sourceName = GetQualifiedName(entityRelation.SourceAttribute, entityRelation.SourceLocation.Alias);
            var relationEntity = entityRelation.RelationAttribute.Entity.GetCanonicalName();
            var relationName = GetQualifiedName(entityRelation.RelationAttribute, entityRelation.RelationLocation.Alias);

            if (string.IsNullOrWhiteSpace(entityRelation.RelationLocation.Alias))
            {
                // Use the entity names for the inner join if no alias has been requested.
                return string.Format(RelationStatementFormat, joinType, relationEntity, sourceName, relationName);
            }

            // Use the entity names names for the inner join and alias the table.
            return string.Format(
                AliasedRelationStatementFormat,
                joinType,
                relationEntity,
                entityRelation.RelationLocation.Alias,
                sourceName,
                relationName);
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
        /// Gets the JOIN clause for the specified relation type.
        /// </summary>
        /// <param name="relationType">
        /// The relation type.
        /// </param>
        /// <returns>
        /// The JOIN clause as a <see cref="string"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="relationType"/> is not one of the named enumerations.
        /// </exception>
        private static string GetJoinClause(EntityRelationType relationType)
        {
            string joinType;

            switch (relationType)
            {
                case EntityRelationType.InnerJoin:
                    joinType = InnerJoinClause;
                    break;
                case EntityRelationType.LeftJoin:
                    joinType = LeftJoinClause;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(relationType));
            }

            return joinType;
        }

        /// <summary>
        /// Gets the canonical (un-aliased) name for the specified entity.
        /// </summary>
        /// <param name="location">
        /// The location of the entity.
        /// </param>
        /// <returns>
        /// The qualified name as a <see cref="string"/>.
        /// </returns>
        private static string GetCanonicalName(this EntityLocation location)
        {
            return string.Concat('[', location.Container, ']', '.', '[', location.Name, ']');
        }
    }
}