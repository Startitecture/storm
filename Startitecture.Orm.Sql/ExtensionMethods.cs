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
        public static TableInfo ToTableInfo([NotNull] this IEntityDefinition definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

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