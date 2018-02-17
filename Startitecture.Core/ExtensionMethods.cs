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
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.Serialization;

    using SAF.StringResources;

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
                throw new NotSupportedException("Index properties are not supported by this method.");
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

            dictionary.Add(ItemKey, Convert.ToString(item));

            foreach (var keyValuePair in item.ToSerializableDictionary())
            {
                dictionary.Add(keyValuePair.Key, keyValuePair.Value);
            }
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
        /// A <see cref="T:System.Collections.Generic.Dictionary`2"/> of the item's properties.
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

            return pair.Value.GetType().IsSerializable ? pair.Value : Convert.ToString(pair.Value);
        }

        /// <summary>
        /// The friendly generic type format string.
        /// </summary>
        private const string FriendlyGenericTypeFormat = "{0}<{1}>";

        /// <summary>
        /// The type name selector.
        /// </summary>
        private static readonly Func<Type, string> TypeNameSelector = x => x.Name;

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
        /// Determines whether an exception contains an inner exception of an exact type.
        /// </summary>
        /// <param name="exception">
        /// The exception to evaluate.
        /// </param>
        /// <typeparam name="TException">
        /// The type of inner exception to evaluate for.
        /// </typeparam>
        /// <returns>
        /// <c>true</c> if the inner exception is the exact type of the <typeparamref name="TException"/> type; otherwise, <c>false</c>.
        /// </returns>
        [SuppressMessage(
            "Microsoft.Design", 
            "CA1004:GenericMethodsShouldProvideTypeParameter", 
            Justification = "This is now common practice for methods with a single generic parameter.")]
        public static bool ContainsInnerException<TException>(this Exception exception)
            where TException : Exception
        {
            return exception?.InnerException != null && exception.InnerException.GetType() == typeof(TException);
        }

        /// <summary>
        /// Retrieves the innermost exception message that is an instance of the specified types to include.
        /// </summary>
        /// <param name="exception">
        /// The current exception.
        /// </param>
        /// <param name="typesToExclude">
        /// The exception types to exclude when searching for the inner exception.
        /// </param>
        /// <returns>
        /// The error message as a <see cref="string"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="exception"/> or <paramref name="typesToExclude"/> are null.
        /// </exception>
        public static string GetInnerExceptionMessage(this Exception exception, params Type[] typesToExclude)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            if (typesToExclude == null)
            {
                throw new ArgumentNullException(nameof(typesToExclude));
            }

            string errorMessage = null;

            var currentException = exception;

            while (currentException != null)
            {
                if (!typesToExclude.Any(x => x.IsInstanceOfType(currentException)))
                {
                    errorMessage = currentException.Message;
                }

                currentException = currentException.InnerException;
            }

            return errorMessage ?? exception.Message;
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
                return String.Format(FriendlyGenericTypeFormat, type.Name, String.Join(", ", type.GetGenericArguments().Select(TypeNameSelector)));
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
            if (String.IsNullOrWhiteSpace(propertyName))
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
                    String.Format((string)ValidationMessages.TypeDoesNotContainProperty, typeof(T).Name, propertyName),
                    nameof(propertyName));
            }

            return info.GetPropertyValue(entity);
        }
    }
}
