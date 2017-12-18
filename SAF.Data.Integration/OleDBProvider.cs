// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OleDBProvider.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Data.OleDb;

    using SAF.Data.Providers;

    /// <summary>
    /// Provides access to items in a DBase III file.
    /// </summary>
    /// <typeparam name="TItem">
    /// The type of item stored in the data source.
    /// </typeparam>
    public class OleDBProvider<TItem> : IDataProvider<OleDbCommand, TItem>
    {
        /// <summary>
        /// Converts items from an object array to the specified item type.
        /// </summary>
        private readonly IDataConverter<object[], TItem> converter;

        /// <summary>
        /// Initializes a new instance of the <see cref="OleDBProvider{TItem}"/> class. 
        /// Initializes a new instance of the <see cref="OleDBProvider&lt;TItem&gt;"/> class.
        /// </summary>
        /// <param name="converter">
        /// The converter to use for converting between an object array and the item.
        /// </param>
        public OleDBProvider(IDataConverter<object[], TItem> converter)
        {
            this.converter = converter;
        }

        /// <summary>
        /// Gets or sets the amount of time to wait before timing out the command.
        /// </summary>
        public int CommandTimeout { get; set; }

        /// <summary>
        /// Returns all items from the data source.
        /// </summary>
        /// <param name="dataSource">
        /// The data source containing the items. The Location property is expected to be a file path.
        /// </param>
        /// <returns>
        /// An enumerable of all the items in the data source.
        /// </returns>
        public IEnumerable<TItem> GetItems(OleDbCommand dataSource)
        {
            if (dataSource == null)
            {
                throw new ArgumentNullException("dataSource");
            }

            dataSource.Connection.Open();
            dataSource.CommandTimeout = this.CommandTimeout;

            using (OleDbDataReader reader = dataSource.ExecuteReader())
            {
                while (reader.Read())
                {
                    object[] values = new object[reader.FieldCount];
                    reader.GetValues(values);
                    yield return this.converter.Convert(values);
                }
            }

            dataSource.Connection.Close();
        }
    }
}
