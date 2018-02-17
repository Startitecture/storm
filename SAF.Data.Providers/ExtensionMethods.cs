// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Contains extension methods for the common repository provider library.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.Caching;

    using JetBrains.Annotations;

    using SAF.StringResources;

    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Query;

    /// <summary>
    /// Contains extension methods for the common repository provider library.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// The qualified property name format.
        /// </summary>
        private const string QualifiedPropertyNameFormat = "{0}.{1}";

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
        /// A collection of all property names associated with the <see cref="ITransactionContext"/> interface.
        /// </summary>
        private static readonly IEnumerable<string> TransactionProperties =
            typeof(ITransactionContext).GetNonIndexedProperties().Select(NameSelector);

        /// <summary>
        /// The not indexed property selector.
        /// </summary>
        private static readonly Func<PropertyInfo, bool> NotIndexedProperty = x => x.GetIndexParameters().Length == 0;

        /// <summary>
        /// Selects items based on the data item type of the current repository.
        /// </summary>
        /// <param name="repository">
        /// The current repository.
        /// </param>
        /// <param name="selectExpressions">
        /// The column selection expressions.
        /// </param>
        /// <typeparam name="TEntity">
        /// The type of entity stored in the repository.
        /// </typeparam>
        /// <typeparam name="TDataItem">
        /// The type of data item that represents the entity.
        /// </typeparam>
        /// <returns>
        /// An <see cref="Startitecture.Orm.Query.ItemSelection{TItem}"/> for the specified <typeparamref name="TDataItem"/>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="repository"/> or <paramref name="selectExpressions"/> is null.
        /// </exception>
        public static ItemSelection<TDataItem> Select<TEntity, TDataItem>(
            [NotNull] this EntityRepository<TEntity, TDataItem> repository,
            [NotNull] params Expression<Func<TDataItem, object>>[] selectExpressions) 
            where TDataItem : class, ITransactionContext, new()
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            if (selectExpressions == null)
            {
                throw new ArgumentNullException(nameof(selectExpressions));
            }

            return Query.From(selectExpressions);
        }

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
        public static TransactSqlSelection<TItem> ToExampleSelection<TItem>(
            this TItem example, 
            params Expression<Func<TItem, object>>[] selectors) where TItem : ITransactionContext, new()
        {
            return new TransactSqlSelection<TItem>(example, selectors);
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
        public static TransactSqlSelection<TItem> ToRangeSelection<TItem>(
            this TItem lowerLimit,
            TItem upperLimit,
            params Expression<Func<TItem, object>>[] selectors) where TItem : ITransactionContext, new()
        {
            return new TransactSqlSelection<TItem>(lowerLimit, upperLimit, selectors);
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
        /// Gets the qualified name for the specified entity.
        /// </summary>
        /// <param name="location">
        /// The location of the entity.
        /// </param>
        /// <returns>
        /// The qualified name as a <see cref="string"/>.
        /// </returns>
        public static string GetQualifiedName(this EntityLocation location)
        {
            var isEntityAliased = string.IsNullOrWhiteSpace(location.Alias) == false;
            return isEntityAliased
                       ? string.Concat('[', location.Alias, ']')
                       : string.Concat('[', location.Container, ']', '.', '[', location.Name, ']');
        }

        /// <summary>
        /// Gets the qualified name for the specified entity.
        /// </summary>
        /// <param name="definition">
        /// The location of the entity.
        /// </param>
        /// <returns>
        /// The qualified name as a <see cref="string"/>.
        /// </returns>
        public static string GetQualifiedName([NotNull] this IEntityDefinition definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            return string.Concat('[', definition.EntityContainer, ']', '.', '[', definition.EntityName, ']');
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
                        filterTokens.Add(
                            string.Format(EqualityFilter, qualifiedName, GetEqualityOperand(filter.FilterValues.First()), index++));

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
        /// Gets a value from the cache or lazily adds an existing value.
        /// </summary>
        /// <param name="cache">
        /// The cache to retrieve or store the value in.
        /// </param>
        /// <param name="synchronizationLock">
        /// The synchronization lock for the cache.
        /// </param>
        /// <param name="cacheKey">
        /// The cache key.
        /// </param>
        /// <param name="retrievalKey">
        /// The retrieval key.
        /// </param>
        /// <param name="getValue">
        /// A function that retrieves the value from the real store.
        /// </param>
        /// <param name="policy">
        /// The policy to apply.
        /// </param>
        /// <typeparam name="TKey">
        /// The type of key that retrieves the value.
        /// </typeparam>
        /// <typeparam name="TValue">
        /// The type of value stored in the cache.
        /// </typeparam>
        /// <returns>
        /// A <typeparamref name="TValue"/> instance, either from the cache or from the retrieval function <paramref name="getValue"/>.
        /// </returns>
        public static TValue GetOrLazyAddExisting<TKey, TValue>(
            this ObjectCache cache,
            object synchronizationLock,
            string cacheKey,
            TKey retrievalKey,
            Func<TKey, TValue> getValue,
            CacheItemPolicy policy)
        {
            if (cache == null)
            {
                throw new ArgumentNullException(nameof(cache));
            }

            if (synchronizationLock == null)
            {
                throw new ArgumentNullException(nameof(synchronizationLock));
            }

            if (cacheKey == null)
            {
                throw new ArgumentNullException(nameof(cacheKey));
            }

            if (getValue == null)
            {
                throw new ArgumentNullException(nameof(getValue));
            }

            if (policy == null)
            {
                throw new ArgumentNullException(nameof(policy));
            }

            var value = cache.Get(cacheKey);

            if (value is TValue optimisticValue)
            {
                return optimisticValue;
            }

            lock (synchronizationLock)
            {
                value = cache.Get(cacheKey);

                if (value is TValue cachedValue)
                {
                    return cachedValue;
                }

                var retrievedValue = getValue(retrievalKey);

                if (Evaluate.IsNull(retrievedValue) == false)
                {
                    cache.Set(cacheKey, retrievedValue, policy);
                }

                return retrievedValue;
            }
        }

        /// <summary>
        /// Gets a value from the cache or lazily adds an existing value.
        /// </summary>
        /// <param name="cache">
        /// The cache to retrieve or store the value in.
        /// </param>
        /// <param name="synchronizationLock">
        /// The synchronization lock for the cache.
        /// </param>
        /// <param name="cacheKey">
        /// The cache key.
        /// </param>
        /// <param name="retrievalKey">
        /// The retrieval key.
        /// </param>
        /// <param name="getValue">
        /// A function that retrieves the value from the real store.
        /// </param>
        /// <param name="policy">
        /// The policy to apply.
        /// </param>
        /// <typeparam name="TKey">
        /// The type of key that retrieves the value.
        /// </typeparam>
        /// <typeparam name="TValue">
        /// The type of value stored in the cache.
        /// </typeparam>
        /// <returns>
        /// A <typeparamref name="TValue"/> instance, either from the cache or from the retrieval function <paramref name="getValue"/>.
        /// </returns>
        public static CacheResult<TValue> GetOrLazyAddExistingWithResult<TKey, TValue>(
            this ObjectCache cache,
            object synchronizationLock,
            string cacheKey,
            TKey retrievalKey,
            Func<TKey, TValue> getValue,
            CacheItemPolicy policy)
        {
            if (cache == null)
            {
                throw new ArgumentNullException(nameof(cache));
            }

            if (synchronizationLock == null)
            {
                throw new ArgumentNullException(nameof(synchronizationLock));
            }

            if (cacheKey == null)
            {
                throw new ArgumentNullException(nameof(cacheKey));
            }

            if (getValue == null)
            {
                throw new ArgumentNullException(nameof(getValue));
            }

            if (policy == null)
            {
                throw new ArgumentNullException(nameof(policy));
            }

            var value = cache.Get(cacheKey);

            if (value is TValue optimisticValue)
            {
                return new CacheResult<TValue>(optimisticValue, true, cacheKey);
            }

            lock (synchronizationLock)
            {
                value = cache.Get(cacheKey);

                if (value is TValue lockedValue)
                {
                    return new CacheResult<TValue>(lockedValue, true, cacheKey);
                }

                var retrievedValue = getValue(retrievalKey);

                if (Evaluate.IsNull(retrievedValue) == false)
                {
                    cache.Set(cacheKey, retrievedValue, policy);
                }

                return new CacheResult<TValue>(retrievedValue, false, cacheKey);
            }
        }

        /// <summary>
        /// Determines if the value at the specified index within the current <see cref="NameValueCollection"/> is equivalent to the 
        /// <see cref="Boolean"/> value <c>true</c>.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of item to apply the value to.
        /// </typeparam>
        /// <typeparam name="TValue">
        /// The type of value to be applied.
        /// </typeparam>
        /// <param name="collection">
        /// The collection containing the value.
        /// </param>
        /// <param name="target">
        /// The target to apply the setting to.
        /// </param>
        /// <param name="propertyExpression">
        /// The property expression of the source value.
        /// </param>
        /// <param name="defaultValue">
        /// The default value if the configured value is not set.
        /// </param>
        /// <param name="parser">
        /// A parser that will convert the string value into the typed value.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="propertyExpression"/> cannot be evaluated as a property.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection"/>, <paramref name="target"/> or <paramref name="propertyExpression"/> is null.
        /// </exception>
        public static void ApplySetting<TItem, TValue>(
            this NameValueCollection collection,
            TItem target,
            Expression<Func<TItem, TValue>> propertyExpression,
            TValue defaultValue,
            Func<string, TValue> parser)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (Evaluate.IsNull(target))
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (propertyExpression == null)
            {
                throw new ArgumentNullException(nameof(propertyExpression));
            }

            // Note: use full name and not GetRuntimeName() otherwise each item type would require a different setting.
            var name = string.Format(QualifiedPropertyNameFormat, typeof(TItem).FullName, propertyExpression.GetPropertyName());

            var newValue = collection.AllKeys.Contains(name)
                               ? TryParse(collection[name], defaultValue, parser)
                               : defaultValue;

            if (!(propertyExpression.Body is MemberExpression memberSelection))
            {
                throw new ArgumentException(ValidationMessages.SelectorCannotBeEvaluated, nameof(propertyExpression));
            }

            var property = memberSelection.Member as PropertyInfo;

            if (property == null)
            {
                throw new ArgumentException(ValidationMessages.SelectorCannotBeEvaluated, nameof(propertyExpression));
            }

            property.SetValue(target, newValue, null);
        }

        /// <summary>
        /// Gets a collection of properties for the specified type.
        /// </summary>
        /// <param name="entityType">
        /// The entity type.
        /// </param>
        /// <param name="propertyNames">
        /// The names of the properties to collect. If no names are specified, all the entity's properties are returned.
        /// </param>
        /// <returns>
        /// A collection of non-indexed properties for the specified type.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// None of the specified property names (case-sensitive) matched properties of the entity type.
        /// </exception>
        public static IEnumerable<PropertyInfo> GetNonIndexedProperties(this Type entityType, params string[] propertyNames)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException(nameof(entityType));
            }

            var nonIndexedProperties = entityType.GetProperties().Where(prop => !prop.GetIndexParameters().Any());

            return propertyNames.Any() ? nonIndexedProperties.Where(x => propertyNames.Contains(x.Name)) : nonIndexedProperties;
        }

        /// <summary>
        /// Gets object values except for indexed and <see cref="ITransactionContext"/> properties.
        /// </summary>
        /// <param name="obj">
        /// The <see cref="ITransactionContext"/> to obtain the properties for.
        /// </param>
        /// <returns>
        /// A collection of <see cref="System.Object"/> items containing property values of the object.
        /// </returns>
        internal static IEnumerable<object> ToValueCollection(this ITransactionContext obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            var values = new List<object>();

            var nonIndexedProperties =
                obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(NotIndexedProperty);

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
        /// Checks that the specified entity dependency is valid and throws a <see cref="Startitecture.Core.OperationException"/>
        /// if the check fails.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of entity with the dependency.
        /// </typeparam>
        /// <typeparam name="TDependency">
        /// The type of dependency to check.
        /// </typeparam>
        /// <param name="entity">
        /// The entity with the dependency.
        /// </param>
        /// <param name="selector">
        /// The selector of the property to verify.
        /// </param>
        /// <remarks>
        /// Dependency checks are intended to ensure that the entity's dependencies exist.
        /// </remarks>
        internal static void ThrowOnDependencyFailure<TItem, TDependency>(this TItem entity, Expression<Func<TItem, TDependency>> selector)
        {
            if (Evaluate.IsNull(entity))
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            if (Evaluate.IsSet(selector.Compile().Invoke(entity)))
            {
                return;
            }

            string message = string.Format(
                ValidationMessages.EntityDependencyCheckFailed,
                typeof(TDependency).Name,
                selector.GetPropertyName());

            throw new OperationException(entity, message);
        }

        /// <summary>
        /// Gets the table info from the generic type.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        /// <returns>
        /// The <see cref="TableInfo"/> for the specified type.
        /// </returns>
        internal static TableInfo ToTableInfo(this IEntityDefinition definition)
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
                                          ? attribute.Entity.GetQualifiedName()
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
            var sourceName = entityRelation.SourceAttribute.GetQualifiedName(entityRelation.SourceLocation.Alias);
            var relationEntity = entityRelation.RelationAttribute.Entity.GetCanonicalName();
            var relationName = entityRelation.RelationAttribute.GetQualifiedName(entityRelation.RelationLocation.Alias);

            if (string.IsNullOrWhiteSpace(entityRelation.RelationLocation.Alias))
            {
                // Use the entity names for the inner join if no alias has been requested.
                return string.Format(
                    RelationStatementFormat,
                    joinType,
                    relationEntity,
                    sourceName,
                    relationName);
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

        /// <summary>
        /// Attempts to parse a value from the specified string value.
        /// </summary>
        /// <param name="value">
        /// The string value to parse.
        /// </param>
        /// <param name="defaultValue">
        /// The default value if the parse fails.
        /// </param>
        /// <param name="parser">
        /// The string parser.
        /// </param>
        /// <typeparam name="TValue">
        /// The type of the expected value.
        /// </typeparam>
        /// <returns>
        /// A <typeparamref name="TValue"/> value parsed from <paramref name="value"/>, or <paramref name="defaultValue"/> if the
        /// <paramref name="parser"/> is unable to parse the string.
        /// </returns>
        private static TValue TryParse<TValue>(string value, TValue defaultValue, Func<string, TValue> parser)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (parser == null)
            {
                throw new ArgumentNullException(nameof(parser));
            }

            try
            {
                return parser(value);
            }
            catch (ArgumentException)
            {
            }
            catch (FormatException)
            {
            }
            catch (OverflowException)
            {
            }

            return defaultValue;
        }
    }
}
