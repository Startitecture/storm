// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExcelFileProvider.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Integration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Microsoft.Office.Interop.Excel;

    using SAF.Core;
    using SAF.Data.Providers;

    /// <summary>
    /// Provides access to items stored in Excel worksheets.
    /// </summary>
    /// <typeparam name="TSource">
    /// The type of <see cref="IFileDataSource"/> containing the worksheets.
    /// </typeparam>
    /// <typeparam name="TItem">
    /// The type of item contained in the worksheets.
    /// </typeparam>
    public class ExcelProvider<TSource, TItem> : IDataProvider<TSource, TItem>
        where TSource : IFileDataSource
    {
        /// <summary>
        /// Converts named value rows into the specified item type.
        /// </summary>
        private readonly IDataConverter<NamedValueRow, TItem> converter;

        /// <summary>
        /// The collection of columns processed by this provider.
        /// </summary>
        private readonly ExcelColumnCollection columns;

        /// <summary>
        /// Filters rows based on the criteria provided to the engine.
        /// </summary>
        private readonly FilterMatchEngine<NamedValueRow> filterEngine;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelProvider{TSource,TItem}"/> class. 
        /// Initializes a new instance of the <see cref="ExcelProvider&lt;TSource, TItem&gt;"/> class.
        /// </summary>
        /// <param name="converter">
        /// A converter that converts a <see cref="NamedValueRow"/> into the specified item type.
        /// </param>
        /// <param name="columns">
        /// The column collection to extract from the Excel worksheet.
        /// </param>
        /// <param name="filterEngine">
        /// A filter engine that includes or excludes rows based on a set of criteria.
        /// </param>
        public ExcelProvider(
            IDataConverter<NamedValueRow, TItem> converter, 
            ExcelColumnCollection columns, 
            FilterMatchEngine<NamedValueRow> filterEngine)
        {
            this.converter = converter;
            this.columns = columns;
            this.filterEngine = filterEngine;
        }

        /// <summary>
        /// Gets or sets the number of empty rows that are allowed before the parser exits.
        /// </summary>
        public int EmptyRowLimit { get; set; }

        /// <summary>
        /// Retrieves items from the data source.
        /// </summary>
        /// <param name="dataSource">
        /// The data source containing the items.
        /// </param>
        /// <returns>
        /// An enumerable of items from the data source.
        /// </returns>
        public IEnumerable<TItem> GetItems(TSource dataSource)
        {
            if (Evaluate.IsNull(dataSource))
            {
                throw new ArgumentNullException("dataSource");
            }

            Workbook workbook = null;
            var excelApplication = new Application();

            foreach (FileInfo file in dataSource.Files)
            {
                try
                {
                    workbook = excelApplication.Workbooks.Open(file.FullName);

                    // Assume that the sheet we want is always the first one (index = 1).
                    var worksheet = (Worksheet)workbook.Sheets[1];

                    int emptyRows = 0;

                    // Row 1 is the headers.
                    int rowIndex = 2;

                    // Check multiple columns to ensure we are not skipping anyone.
                    while (emptyRows < this.EmptyRowLimit)
                    {
                        if (CellsAreEmpty(worksheet, this.columns, rowIndex))
                        {
                            emptyRows++;
                        }
                        else
                        {
                            var valueArray = this.columns.ToDictionary<ExcelColumn, NamedColumn, object>(
                                column => column.NamedColumn, 
                                column => GetCellValue(worksheet, column, rowIndex));

                            // TODO: Have Excel File Provider detect the columns from a named column collection.
                            var row = new NamedValueRow(valueArray);

                            if (this.filterEngine.GetMatch(row).Action == FilterAction.Include)
                            {
                                yield return this.converter.Convert(row);
                            }
                        }

                        rowIndex++;
                    }
                }
                finally
                {
                    if (workbook != null)
                    {
                        workbook.Close();
                    }

                    excelApplication.Workbooks.Close();
                }
            }
        }

        /// <summary>
        /// Detects whether a specific cell is empty.
        /// </summary>
        /// <param name="worksheet">
        /// The worksheet containing the cell.
        /// </param>
        /// <param name="requiredColumns">
        /// The columns that must be empty to qualify as an empty row.
        /// </param>
        /// <param name="row">
        /// The row to retrieve the data from.
        /// </param>
        /// <returns>
        /// <c>true</c> if all the cells specified are empty; otherwise <c>false</c>.
        /// </returns>
        private static bool CellsAreEmpty(Worksheet worksheet, IEnumerable<ExcelColumn> requiredColumns, int row)
        {
            return requiredColumns.All(column => String.IsNullOrEmpty(GetCellValue(worksheet, column, row)));
        }

        /// <summary>
        /// Retrieves the value of the cell from the specified worksheet.
        /// </summary>
        /// <param name="worksheet">
        /// The worksheet containing the cell.
        /// </param>
        /// <param name="column">
        /// The cell's column.
        /// </param>
        /// <param name="row">
        /// The row to retrieve the data from.
        /// </param>
        /// <returns>
        /// The string value of the cell.
        /// </returns>
        private static string GetCellValue(Worksheet worksheet, ExcelColumn column, int row)
        {
            string value = Convert.ToString(((Range)worksheet.Cells[row, column.ExcelColumnName]).Value);

            return String.IsNullOrEmpty(value) ? null : value.Trim();
        }
    }
}
