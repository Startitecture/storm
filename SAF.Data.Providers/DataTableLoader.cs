﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataTableLoader.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reflection;

    using JetBrains.Annotations;

    using SAF.StringResources;

    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Model;

    /// <summary>
    /// A base class for data table loaders.
    /// </summary>
    /// <typeparam name="T">
    /// The type of data to load.
    /// </typeparam>
    public class DataTableLoader<T>
    {
        /// <summary>
        /// The form submission data columns.
        /// </summary>
        /// <remarks>
        /// Avoid interface properties by filtering out non-writable properties.
        /// </remarks>
        private static readonly Lazy<Dictionary<string, PropertyInfo>> DataProperties =
            new Lazy<Dictionary<string, PropertyInfo>>(
                () =>
                    (from p in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                     where p.CanWrite && p.GetCustomAttribute<RelationAttribute>() == null
                     orderby p.Name
                     select new
                                {
                                    p.Name, PropertyInfo = p
                                })
                .ToDictionary(property => property.Name, property => property.PropertyInfo));

        /// <summary>
        /// Loads and returns a data table with the specified values.
        /// </summary>
        /// <param name="values">
        /// The values to load.
        /// </param>
        /// <returns>
        /// A <see cref="DataTable"/> with the loaded values.
        /// </returns>
        public DataTable Load([NotNull] IEnumerable<T> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            var dataColumnProperties = DataProperties.Value;
            var dataTable = new DataTable();
            var dataColumns = dataColumnProperties.Values.Select(CreateDataColumn).ToArray();
            dataTable.Columns.AddRange(dataColumns);

            foreach (var value in values)
            {
                var dataRow = dataTable.NewRow();

                foreach (var column in dataColumns)
                {
                    SetDataRowColumnValue(dataColumnProperties, column, value, dataRow);
                }

                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }

        /// <summary>
        /// Loads and returns a data table with the specified values mapped from the <paramref name="items"/>.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of item to map.
        /// </typeparam>
        /// <param name="items">
        /// The items to load.
        /// </param>
        /// <param name="entityMapper">
        /// The entity mapper to use to map the items into the typed values.
        /// </param>
        /// <returns>
        /// A <see cref="DataTable"/> with the loaded values.
        /// </returns>
        public DataTable Load<TItem>([NotNull] IEnumerable<TItem> items, [NotNull] IEntityMapper entityMapper)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (entityMapper == null)
            {
                throw new ArgumentNullException(nameof(entityMapper));
            }

            var mappedValues = entityMapper.Map<List<T>>(items);
            return this.Load(mappedValues);
        }

        /// <summary>
        /// Refreshes an existing data table with the values in the enumerable. This method assumes that the enumerable items and
        /// the
        /// table rows are in the same order.
        /// </summary>
        /// <param name="values">
        /// The values to apply.
        /// </param>
        /// <param name="dataTable">
        /// The data table to refresh.
        /// </param>
        public void Refresh([NotNull] IEnumerable<T> values, [NotNull] DataTable dataTable)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (dataTable == null)
            {
                throw new ArgumentNullException(nameof(dataTable));
            }

            using (var enumerator = values.GetEnumerator())
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    if (enumerator.MoveNext() == false)
                    {
                        throw new OperationException(values, ErrorMessages.EnumerationCountMismatch);
                    }

                    var dataRow = dataTable.Rows[i];

                    foreach (var column in dataTable.Columns.OfType<DataColumn>())
                    {
                        var dataColumnValue = dataRow[column];
                        var itemValue = DataProperties.Value[column.ColumnName].GetMethod.Invoke(enumerator.Current, null);

                        if (dataColumnValue.Equals(itemValue))
                        {
                            continue;
                        }

                        SetDataRowColumnValue(DataProperties.Value, column, enumerator.Current, dataRow);
                    }
                }
            }
        }

        /// <summary>
        /// Refreshes an existing data table with the values in the enumerable. This method assumes that the enumerable items and
        /// the
        /// table rows are in the same order.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of item to map.
        /// </typeparam>
        /// <param name="items">
        /// The values to apply.
        /// </param>
        /// <param name="dataTable">
        /// The data table to refresh.
        /// </param>
        /// <param name="entityMapper">
        /// The entity mapper to use to map the items into the typed values.
        /// </param>
        public void Refresh<TItem>([NotNull] IEnumerable<TItem> items, [NotNull] DataTable dataTable, [NotNull] IEntityMapper entityMapper)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (dataTable == null)
            {
                throw new ArgumentNullException(nameof(dataTable));
            }

            if (entityMapper == null)
            {
                throw new ArgumentNullException(nameof(entityMapper));
            }

            var mappedValues = entityMapper.Map<List<T>>(items);
            this.Refresh(mappedValues, dataTable);
        }

        /// <summary>
        /// Creates a data column.
        /// </summary>
        /// <param name="propertyInfo">
        /// The property info to base the column on.
        /// </param>
        /// <returns>
        /// A <see cref="DataColumn"/> based on the property info.
        /// </returns>
        private static DataColumn CreateDataColumn(PropertyInfo propertyInfo)
        {
            var propertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
            return new DataColumn(propertyInfo.Name, propertyType);
        }

        /// <summary>
        /// Sets a data row column value.
        /// </summary>
        /// <param name="dataColumnProperties">
        /// The data column properties.
        /// </param>
        /// <param name="column">
        /// The column to set.
        /// </param>
        /// <param name="value">
        /// The value to apply.
        /// </param>
        /// <param name="dataRow">
        /// The data row to set the value in.
        /// </param>
        private static void SetDataRowColumnValue(
            IReadOnlyDictionary<string, PropertyInfo> dataColumnProperties,
            DataColumn column,
            T value,
            DataRow dataRow)
        {
            // Remember the early 2000's? DataTable/Row doesn't like null and let's be honest the music was really bad
            var rowValue = dataColumnProperties[column.ColumnName].GetMethod.Invoke(value, null) ?? DBNull.Value;
            dataRow[column] = rowValue;
        }
    }
}