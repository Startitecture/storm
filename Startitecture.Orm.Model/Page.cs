// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Page.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Holds the results of a paged request.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// Holds the results of a paged request.
    /// </summary>
    /// <typeparam name="T">
    /// The type of data item in the returned result set.
    /// </typeparam>
    public class Page<T>
    {
        /// <summary>
        /// The items.
        /// </summary>
        private readonly List<T> items = new List<T>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Page{T}"/> class.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        public Page(IEnumerable<T> items)
        {
            this.items.AddRange(items);
        }

        #region Public Properties

        /// <summary>
        /// Gets or sets the user-supplied context for the page.
        /// </summary>
        public object Context { get; set; }

        /// <summary>
        /// Gets or sets the current page number contained in this page of result set.
        /// </summary>
        public long CurrentPage { get; set; }

        /// <summary>
        /// Gets the items in the current page.
        /// </summary>
        public IEnumerable<T> Items
        {
            get
            {
                return this.items;
            }
        }

        /// <summary>
        /// Gets or sets the number of items per page.
        /// </summary>
        public long ItemsPerPage { get; set; }

        /// <summary>
        /// Gets or sets the total number of records in the full result set.
        /// </summary>
        public long TotalItems { get; set; }

        /// <summary>
        /// Gets or sets the total number of pages in the full result set.
        /// </summary>
        public long TotalPages { get; set; }

        #endregion
    }
}