// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataTableLoader.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reflection;

    using JetBrains.Annotations;

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
        /// The property dictionary.
        /// </summary>
        private readonly Lazy<Dictionary<string, PropertyInfo>> propertyDictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTableLoader{T}"/> class.
        /// </summary>
        /// <param name="definitionProvider">
        /// The definition provider.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="definitionProvider"/> is null.
        /// </exception>
        public DataTableLoader([NotNull] IEntityDefinitionProvider definitionProvider)
        {
            if (definitionProvider == null)
            {
                throw new ArgumentNullException(nameof(definitionProvider));
            }

            this.propertyDictionary = new Lazy<Dictionary<string, PropertyInfo>>(
                () => definitionProvider.Resolve<T>()
                    .AllAttributes.OrderBy(definition => definition.Ordinal)
                    .ToDictionary(definition => definition.PropertyInfo.Name, definition => definition.PropertyInfo));
        }

        /// <summary>
        /// Loads and returns a data table with the specified values.
        /// </summary>
        /// <param name="values">
        /// The values to load.
        /// </param>
        /// <returns>
        /// A <see cref="System.Data.DataTable"/> with the loaded values.
        /// </returns>
        public DataTable Load([NotNull] IEnumerable<T> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            var dataTable = new DataTable();
            var dataColumns = this.propertyDictionary.Value.Values.Select(CreateDataColumn).ToArray();
            dataTable.Columns.AddRange(dataColumns);

            foreach (var value in values)
            {
                var dataRow = dataTable.NewRow();

                foreach (var column in dataColumns)
                {
                    SetDataRowColumnValue(this.propertyDictionary.Value, column, value, dataRow);
                }

                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }

        /// <summary>
        /// Creates a data column.
        /// </summary>
        /// <param name="propertyInfo">
        /// The property info to base the column on.
        /// </param>
        /// <returns>
        /// A <see cref="System.Data.DataColumn"/> based on the property info.
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