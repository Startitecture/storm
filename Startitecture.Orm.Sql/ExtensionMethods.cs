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

    using JetBrains.Annotations;

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
        /// Gets a <see cref="QueryContext{TItem}"/> as a SELECT statement.
        /// </summary>
        /// <param name="selection">
        /// The selection to use to create the context.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item being queried.
        /// </typeparam>
        /// <returns>
        /// A new <see cref="QueryContext{TItem}"/> for the specified <paramref name="selection"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="selection"/> is null.
        /// </exception>
        public static QueryContext<TItem> AsSelect<TItem>([NotNull] this ItemSelection<TItem> selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            return new QueryContext<TItem>(selection, StatementOutputType.Select, 0);
        }

        /// <summary>
        /// Gets a <see cref="QueryContext{TItem}"/> as a CONTAINS statement.
        /// </summary>
        /// <param name="selection">
        /// The selection to use to create the context.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item being queried.
        /// </typeparam>
        /// <returns>
        /// A new <see cref="QueryContext{TItem}"/> for the specified <paramref name="selection"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="selection"/> is null.
        /// </exception>
        public static QueryContext<TItem> AsContains<TItem>([NotNull] this ItemSelection<TItem> selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            return new QueryContext<TItem>(selection, StatementOutputType.Contains, 0);
        }

        /// <summary>
        /// Gets a <see cref="QueryContext{TItem}"/> as a DELETE statement.
        /// </summary>
        /// <param name="selection">
        /// The selection to use to create the context.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item being deleted.
        /// </typeparam>
        /// <returns>
        /// A new <see cref="QueryContext{TItem}"/> for the specified <paramref name="selection"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="selection"/> is null.
        /// </exception>
        public static QueryContext<TItem> AsDelete<TItem>([NotNull] this ItemSelection<TItem> selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            return new QueryContext<TItem>(selection, StatementOutputType.Delete, 0);
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
    }
}