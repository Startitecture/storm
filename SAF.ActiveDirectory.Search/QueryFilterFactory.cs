namespace SAF.ActiveDirectory.Search
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SAF.ActiveDirectory;

    /// <summary>
    /// Creates query filters from a variety of inputs.
    /// </summary>
    public static class QueryFilterFactory
    {
        /// <summary>
        /// Builds an LDAP search filter based on the provided object categories, 
        /// attribute and search mask.
        /// </summary>
        /// <param name="categories">List of object categories to include in the
        /// search</param>
        /// <param name="attribute">Attribute to search on</param>
        /// <param name="mask">Search mask (*=wildcard)</param>
        /// <returns>An LDAP filter to use for searching for the specified
        /// object categories using the attribute and search mask.</returns>
        public static QueryFilter CreateFrom(IEnumerable<string> categories, string attribute, string mask)
        {
            // Return null if nothing has been set.
            if (String.IsNullOrEmpty(attribute) && String.IsNullOrEmpty(mask) && (categories.Count() == 0))
            {
                return null;
            }

            if (String.IsNullOrEmpty(attribute) && String.IsNullOrEmpty(mask))
            {
                // Immediately return only the categories if there is no attribute
                // or search mask specified.
                return CreateFrom(categories);
            }
            else if (String.IsNullOrEmpty(attribute))
            {
                // Default to CN (common name) if no attribute is provided but a
                // search mask is.
                attribute = "cn";
            }
            else if (String.IsNullOrEmpty(mask))
            {
                // Default to searching for everything if the search mask is not
                // specified.
                mask = "*";
            }

            string cleanMask = EscapeFilterLiterals(mask);

            // The filter mask must "AND" the categories and the attribute search -
            // this will cause the search to use all the categories but select only
            // those entries with the specified attribute matching the search mask.
            return new QueryFilter(
                String.Format("({0}({1}={2}))", CreateFrom(categories), attribute, cleanMask), attribute, cleanMask, categories);
        }

        /// <summary>
        /// Builds an LDAP search filter based on the provided object categories and 
        /// raw filter.
        /// </summary>
        /// <param name="categories">List of object categories to include in the
        /// search</param>
        /// <param name="rawFilter">A raw LDAP filter for the object search</param>
        /// <returns>An LDAP filter to use for searching for the specified
        /// object categories using the raw filter.</returns>
        public static QueryFilter CreateFrom(IEnumerable<string> categories, string rawFilter)
        {
            if (categories == null)
            {
                throw new ArgumentNullException("categories");
            }

            // Return null if nothing has been set.
            if (String.IsNullOrEmpty(rawFilter) && categories.Count() == 0)
            {
                throw new ArgumentException("rawFilter cannot be null if no categories are specified.", "rawFilter");
            }

            // If the raw filter is set but the categories are not, return only the raw filter.
            if (categories.Count() == 0)
            {
                return new QueryFilter(rawFilter, null, null, categories);
            }

            return new QueryFilter(String.Format("({0}({1}))", CreateFrom(categories), rawFilter), null, null, categories);
        }

        /// <summary>
        /// Builds a category filter given the specified categories.
        /// </summary>
        /// <param name="categories">The categories to build an LDAP filter for.</param>
        /// <returns>An LDAP filter that includes the specified categories.</returns>
        public static QueryFilter CreateFrom(IEnumerable<string> categories)
        {
            // Each category must be separated by an "OR", or in LDAP, a "|",
            // otherwise the search will try to find an object that conforms
            // to multiple object categories.
            string categoryFilter = "&(|{0})";
            string categoryMask = "({0}={1})";
            string categoryList = String.Empty;

            if (categories != null)
            {
                foreach (string category in categories)
                {
                    categoryList += String.Format(categoryMask, ObjectProperty.Category, category);
                }

                // If there are no categories, categoryList will remain empty. Otherwise
                // this will add the needed LDAP filter OR statement.
                if (!String.IsNullOrEmpty(categoryList))
                {
                    categoryList = String.Format(categoryFilter, categoryList);
                }
            }

            return new QueryFilter(categoryList, null, null, categories);
        }

        /// <summary>
        /// Escapes literal strings for use in an LDAP filter.
        /// </summary>
        /// <param name="value">The value that will be placed in an LDAP filter.</param>
        /// <returns>A string with all filter literals escaped.</returns>
        private static string EscapeFilterLiterals(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            // The backslash can also be used as an escape character, so it must be replaced before other characters 
            // are escaped.
            List<char> escapeChars = new List<char>(new char[] { '*', '(', ')', '/' });
            List<int> replaceIndices = new List<int>();

            // Only escape backslashes that are not escaping the characters we plan to escape.
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == '\\' && i + 1 < value.Length && !escapeChars.Contains(value[i + 1]))
                {
                    replaceIndices.Add(i);
                }
            }

            foreach (int i in replaceIndices)
            {
                value.Remove(i, 1);
                value.Insert(i, @"\5c");
            }

            return
                value
                    .Replace("*", @"\2a")
                    .Replace("(", @"\28")
                    .Replace(")", @"\29")
                    .Replace("/", @"\2f");
        }
    }
}
