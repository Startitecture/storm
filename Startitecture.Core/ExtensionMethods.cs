﻿// --------------------------------------------------------------------------------------------------------------------
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

            var comparisonList = from p in allProperties
                                 select new
                                            {
                                                p.Name,
                                                OriginalValue = p.GetValue(baseline),
                                                NewValue = p.GetValue(comparison)
                                            };

            return from c in comparisonList
                   where !Evaluate.RecursiveEquals(c.OriginalValue, c.NewValue)
                   select new PropertyComparisonResult(c.Name, c.OriginalValue, c.NewValue);
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
        private static IEnumerable<PropertyInfo> GetAllProperties<TItem>(string[] propertiesToCompare)
        {
            var allProperties = propertiesToCompare.Any()
                                    ? typeof(TItem).GetProperties()
                                        .Where(x => propertiesToCompare.Contains(x.Name) && x.GetIndexParameters().Any() == false)
                                        .OrderBy(x => x.Name)
                                        .ToList()
                                    : typeof(TItem).GetProperties()
                                        .Where(x => x.GetIndexParameters().Any() == false)
                                        .OrderBy(x => x.Name)
                                        .ToList();

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
