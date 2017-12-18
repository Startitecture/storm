namespace SAF.ActiveDirectory.Search
{
    using System.Collections.Generic;

    /// <summary>
    /// A filter specification for directory queries.
    /// </summary>
    public class QueryFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryFilter"/> class.
        /// </summary>
        /// <param name="filter">A raw LDAP filter.</param>
        /// <param name="searchAttribute">The search attribute to query.</param>
        /// <param name="searchMask">The search mask to apply to the attribute.</param>
        /// <param name="categories">A collection of object categories to include in the search.</param>
        internal QueryFilter(string filter, string searchAttribute, string searchMask, IEnumerable<string> categories)
        {
            this.RawFilter = filter;
            this.SearchAttribute = searchAttribute;
            this.SearchMask = searchMask;
            this.ObjectCategories = categories;
        }

        /// <summary>
        /// Gets the LDAP filter to use for searching. Setting this property will clear the 
        /// <see cref="SearchAttribute"/> and <see cref="SearchMask"/> properties.
        /// </summary>
        public string RawFilter { get; private set; }

        /// <summary>
        /// Getsthe attribute to use for searching.
        /// </summary>
        public string SearchAttribute { get; private set; }

        /// <summary>
        /// Gets the search mask used for the search.
        /// </summary>
        public string SearchMask { get; private set; }

        /// <summary>
        /// Gets the object categories to return.
        /// </summary>
        public IEnumerable<string> ObjectCategories { get; private set; }

        /// <summary>
        /// Returns the string representation of this instance.
        /// </summary>
        /// <returns>The search filter used by the current <see cref="QueryFilter"/>.</returns>
        public override string ToString()
        {
            return this.RawFilter;
        }
    }
}
