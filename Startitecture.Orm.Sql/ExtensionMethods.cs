// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Startitecture">
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
    using Startitecture.Orm.Schema;

    /// <summary>
    /// The extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
        /*
        /// <summary>
        /// Gets object values except for indexed and <see cref="Startitecture.Orm.Common.ITransactionContext"/> properties.
        /// </summary>
        /// <param name="context">
        /// The <see cref="Startitecture.Orm.Common.ITransactionContext"/> to obtain the properties for.
        /// </param>
        /// <returns>
        /// A collection of <see cref="System.Object"/> items containing property values of the object.
        /// </returns>
        public static IEnumerable<object> ToValueCollection(this ITransactionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var values = new List<object>();

            var nonIndexedProperties = context.GetType().GetNonIndexedProperties();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var propertyInfo in nonIndexedProperties.OrderBy(NameSelector))
            {
                if (TransactionProperties.Contains(propertyInfo.Name))
                {
                    continue;
                }

                values.Add(propertyInfo.GetPropertyValue(context));
            }

            return values;
        }
*/
    }
}