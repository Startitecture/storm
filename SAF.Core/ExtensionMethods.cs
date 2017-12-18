// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Contains methods that extend existing classes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;

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
        /// <exception cref="ArgumentNullException">
        /// <paramref name="propertyInfo"/> or <paramref name="entity"/> is null.
        /// </exception>
        /// <exception cref="NotSupportedException">
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
        /// not implement the <see cref="ISerializable"/> interface.
        /// </summary>
        /// <param name="dictionary">
        /// The current dictionary.
        /// </param>
        /// <param name="item">
        /// The item with the properties to insert into the dictionary.
        /// </param>
        /// <exception cref="ArgumentNullException">
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
    }
}
