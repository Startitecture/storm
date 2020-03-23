// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Contains methods that extend existing classes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.Caching;

    using Startitecture.Resources;

    /// <summary>
    /// Contains methods that extend existing classes.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// The item key.
        /// </summary>
        private const string ItemKey = "{Item}";

        /// <summary>
        /// The qualified property name format.
        /// </summary>
        private const string QualifiedPropertyNameFormat = "{0}.{1}";

        /// <summary>
        /// The friendly generic type format string.
        /// </summary>
        private const string FriendlyGenericTypeFormat = "{0}<{1}>";

        /// <summary>
        /// The type name selector.
        /// </summary>
        private static readonly Func<Type, string> TypeNameSelector = x => x.Name;

        /// <summary>
        /// The property name selector.
        /// </summary>
        private static readonly Func<PropertyInfo, string> PropertyNameSelector = x => x.Name;

        /// <summary>
        /// Gets a property value for the specified entity. Indexed properties are not supported.
        /// </summary>
        /// <param name="propertyInfo">
        /// The property info of the target property.
        /// </param>
        /// <param name="entity">
        /// The entity to retrieve the value from.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="propertyInfo"/> or <paramref name="entity"/> is null.
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        /// <paramref name="propertyInfo"/> is an indexed property.
        /// </exception>
        /// <returns>
        /// The value stored in the property of the specified entity. If the property is indexed, the first value is returned.
        /// </returns>
        public static object GetPropertyValue(this PropertyInfo propertyInfo, object entity)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (propertyInfo.GetIndexParameters().Any())
            {
                throw new NotSupportedException(ErrorMessages.IndexPropertiesNotSupported);
            }

            return propertyInfo.GetValue(entity, null);
        }

        /// <summary>
        /// Populates the current dictionary with the properties of the item. The property values are converted to strings if they do 
        /// not implement the <see cref="System.Runtime.Serialization.ISerializable"/> interface.
        /// </summary>
        /// <param name="dictionary">
        /// The current dictionary.
        /// </param>
        /// <param name="item">
        /// The item with the properties to insert into the dictionary.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="dictionary"/> is null.
        /// </exception>
        public static void PopulateDictionary(this IDictionary dictionary, object item)
        {
            // TODO: Move to base exception
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            if (item == null)
            {
                return;
            }

            dictionary.Add(ItemKey, Convert.ToString(item, CultureInfo.CurrentCulture));

            foreach (var keyValuePair in item.ToSerializableDictionary())
            {
                dictionary.Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        /// <summary>
        /// Gets the property name from a selector.
        /// </summary>
        /// <param name="selector">
        /// An expression that selects a property.
        /// </param>
        /// <returns>
        /// The property name as a <see cref="string"/>.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// The expression cannot be evaluated for a member name, or the member is not a property.
        /// </exception>
        public static PropertyInfo GetProperty(this LambdaExpression selector)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            if (!(selector.Body is MemberExpression body))
            {
                if (!(selector.Body is UnaryExpression unaryBody))
                {
                    throw new ArgumentException(ValidationMessages.SelectorCannotBeEvaluated, nameof(selector));
                }

                body = unaryBody.Operand as MemberExpression;

                if (body == null)
                {
                    throw new ArgumentException(ValidationMessages.SelectorCannotBeEvaluated, nameof(selector));
                }
            }

            var propertyInfo = body.Member as PropertyInfo;

            if (propertyInfo == null)
            {
                throw new ArgumentException(ValidationMessages.SelectorCannotBeEvaluated, nameof(selector));
            }

            return propertyInfo;
        }

        /// <summary>
        /// Converts a type to its runtime name, including generic arguments.
        /// </summary>
        /// <param name="type">
        /// The type to convert.
        /// </param>
        /// <returns>
        /// The runtime name, including generic arguments, as a <see cref="string"/>.
        /// </returns>
        public static string ToRuntimeName(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.GetGenericArguments().Any())
            {
                var arguments = string.Join(", ", type.GetGenericArguments().Select(TypeNameSelector));
                return string.Format(CultureInfo.CurrentCulture, FriendlyGenericTypeFormat, type.Name, arguments);
            }

            return type.Name;
        }

        /// <summary>
        /// Gets the member of the lambda expression.
        /// </summary>
        /// <param name="expression">
        /// The expression to evaluate.
        /// </param>
        /// <returns>
        /// The <see cref="System.Linq.Expressions.MemberExpression"/> in the body of the expression.
        /// </returns>
        /// <exception cref="OperationException">
        /// The expression cannot be evaluated as a property.
        /// </exception>
        public static MemberExpression GetMember(this LambdaExpression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (expression.Body is MemberExpression body)
            {
                return body;
            }

            if (!(expression.Body is UnaryExpression unaryBody))
            {
                throw new OperationException(expression, ValidationMessages.SelectorCannotBeEvaluated);
            }

            body = unaryBody.Operand as MemberExpression;

            if (body == null)
            {
                throw new OperationException(expression, ValidationMessages.SelectorCannotBeEvaluated);
            }

            return body;
        }

        /// <summary>
        /// Gets the property name from a selector.
        /// </summary>
        /// <param name="selector">
        /// An expression that selects a property.
        /// </param>
        /// <returns>
        /// The property name as a <see cref="string"/>.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// The expression cannot be evaluated for a member name.
        /// </exception>
        public static string GetPropertyName(this LambdaExpression selector)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return selector.GetMember().Member.Name;
        }

        /// <summary>
        /// The get property value.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <typeparam name="T">
        /// The type of entity to get the property value for.
        /// </typeparam>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="propertyName"/> or <paramref name="entity"/> is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="propertyName"/> specifies a property that does not exist in <typeparamref name="T"/>.
        /// </exception>
        public static object GetPropertyValue<T>(this T entity, string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (Evaluate.IsNull(entity))
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var info = typeof(T).GetProperty(propertyName);

            if (info == null)
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture, ValidationMessages.TypeDoesNotContainProperty, typeof(T).Name, propertyName),
                    nameof(propertyName));
            }

            return info.GetPropertyValue(entity);
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
            var name = string.Format(CultureInfo.InvariantCulture, QualifiedPropertyNameFormat, typeof(TItem).FullName, propertyExpression.GetPropertyName());

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
        public static void ThrowOnDependencyFailure<TItem, TDependency>(this TItem entity, Expression<Func<TItem, TDependency>> selector)
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
                CultureInfo.CurrentCulture,
                ValidationMessages.EntityDependencyCheckFailed,
                typeof(TDependency).Name,
                selector.GetPropertyName());

            throw new OperationException(entity, message);
        }

        /// <summary>
        /// Gets the property differences between two objects of the same type.
        /// </summary>
        /// <param name="baseline">
        /// The baseline object.
        /// </param>
        /// <param name="comparison">
        /// The comparison object.
        /// </param>
        /// <param name="propertiesToCompare">
        /// The properties to compare.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item to compare.
        /// </typeparam>
        /// <returns>
        /// A collection of <see cref="Startitecture.Core.PropertyComparisonResult"/> items containing the non-equivalent property values of the two 
        /// items.
        /// </returns>
        public static IEnumerable<PropertyComparisonResult> GetDifferences<TItem>(
            this TItem baseline,
            TItem comparison,
            params string[] propertiesToCompare)
        {
            if (Evaluate.IsNull(baseline))
            {
                throw new ArgumentNullException(nameof(baseline));
            }

            if (Evaluate.IsNull(comparison))
            {
                throw new ArgumentNullException(nameof(comparison));
            }

            if (Evaluate.IsNull(propertiesToCompare))
            {
                throw new ArgumentNullException(nameof(propertiesToCompare));
            }

            var allProperties = GetAllProperties<TItem>(propertiesToCompare);

            var originalProperties = allProperties.ToDictionary(info => info.Name, info => info.GetPropertyValue(baseline));

            var newProperties = allProperties.ToDictionary(info => info.Name, info => info.GetPropertyValue(comparison));

            return (from propertyName in allProperties.Select(x => x.Name)
                    let originalValue = originalProperties[propertyName]
                    let newValue = newProperties[propertyName]
                    where !ReferenceEquals(originalValue, newValue)
                    where (originalValue != null && !originalValue.Equals(newValue)) || !newValue.Equals(originalValue)
                    select
                        new PropertyComparisonResult
                        {
                            PropertyName = propertyName,
                            OriginalValue = originalValue,
                            NewValue = newValue
                        }).ToList();
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
            catch (ArgumentException ex)
            {
                Trace.TraceWarning($"Parse of '{value}' resulted in {ex.GetType().Name}: {ex.Message}");
            }
            catch (FormatException ex)
            {
                Trace.TraceWarning($"Parse of '{value}' resulted in {ex.GetType().Name}: {ex.Message}");
            }
            catch (OverflowException ex)
            {
                Trace.TraceWarning($"Parse of '{value}' resulted in {ex.GetType().Name}: {ex.Message}");
            }

            return defaultValue;
        }

        /// <summary>
        /// Gets all of the properties for the array of string properties to compare.
        /// </summary>
        /// <param name="propertiesToCompare">
        /// The properties to compare.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item to evaluate.
        /// </typeparam>
        /// <returns>
        /// A <see cref="List{T}"/> of <see cref="PropertyInfo"/> items matching the <paramref name="propertiesToCompare"/>.
        /// </returns>
        private static List<PropertyInfo> GetAllProperties<TItem>(string[] propertiesToCompare)
        {
            var allProperties = propertiesToCompare.Any()
                                    ? typeof(TItem).GetProperties().Where(x => propertiesToCompare.Contains(x.Name) && !x.GetIndexParameters().Any())
                                        .OrderBy(x => x.Name).ToList()
                                    : typeof(TItem).GetProperties().Where(x => !x.GetIndexParameters().Any()).OrderBy(x => x.Name).ToList();
            return allProperties;
        }

        /// <summary>
        /// Returns the property names and values of the current item.
        /// </summary>
        /// <param name="item">
        /// The item to evaluate.
        /// </param>
        /// <param name="propertiesToInclude">
        /// The properties to include. If no properties are specified, all valid properties are included.
        /// </param>
        /// <returns>
        /// A <see cref="Dictionary{TKey, TValue}"/> of the item's properties.
        /// </returns>
        private static Dictionary<string, object> ToPropertyDictionary(this object item, params string[] propertiesToInclude)
        {
            if (propertiesToInclude == null)
            {
                throw new ArgumentNullException(nameof(propertiesToInclude));
            }

            if (item == null)
            {
                return new Dictionary<string, object>();
            }

            var propertyValues = new Dictionary<string, object>();

            // If we use the generic type then we may get nothing back if the item is passed as an object.
            var properties = item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).OrderBy(PropertyNameSelector);
            bool filterByName = propertiesToInclude.Length > 0;

            // ReSharper disable LoopCanBeConvertedToQuery - performance
            foreach (var propertyInfo in properties)
            {
                // ReSharper restore LoopCanBeConvertedToQuery
                if (propertyInfo.GetIndexParameters().Length > 0)
                {
                    continue;
                }

                if (filterByName && !propertiesToInclude.Contains(propertyInfo.Name))
                {
                    continue;
                }

                if (propertyInfo.GetCustomAttributes(typeof(DoNotLogAttribute), false).Any())
                {
                    continue;
                }

                propertyValues.Add(propertyInfo.Name, propertyInfo.GetPropertyValue(item));
            }

            return propertyValues;
        }

        /// <summary>
        /// Gets property names and values for the specified item, replacing any non-serializable items with their string 
        /// representations.
        /// </summary>
        /// <param name="item">
        /// The item to retrieve the properties of.
        /// </param>
        /// <param name="propertiesToInclude">
        /// The properties to include.
        /// </param>
        /// <returns>
        /// A dictionary of name value pairs joined as <see cref="string"/>, ordered by the property name.
        /// </returns>
        private static Dictionary<string, object> ToSerializableDictionary(
            this object item,
            params string[] propertiesToInclude)
        {
            if (Evaluate.IsNull(item))
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (propertiesToInclude == null)
            {
                throw new ArgumentNullException(nameof(propertiesToInclude));
            }

            return item.ToPropertyDictionary(propertiesToInclude).ToDictionary(pair => pair.Key, GetSerializableValue);
        }

        /// <summary>
        /// Gets the serializable value of the value in the key value pair.
        /// </summary>
        /// <param name="pair">
        /// The pair to evaluate.
        /// </param>
        /// <returns>
        /// The serializable value of the key value pair as an <see cref="object"/>.
        /// </returns>
        private static object GetSerializableValue(KeyValuePair<string, object> pair)
        {
            if (pair.Value == null)
            {
                return null;
            }

            return pair.Value.GetType().IsSerializable ? pair.Value : Convert.ToString(pair.Value, CultureInfo.CurrentCulture);
        }
    }
}
