namespace SAF.ActiveDirectory.Search
{
    using System.Collections.Generic;
    using System.DirectoryServices;
    using System.Linq;
    using System.Security.Permissions;

    using SAF.ActiveDirectory;

    /// <summary>
    /// Creates a <see cref="DirectoryQuery"/> for the specified search parameters.
    /// </summary>
    public static class DirectoryQueryFactory
    {
        /// <summary>
        /// Searches for Active Directory objects using the provided filter, starting in the specified BaseDN.
        /// </summary>
        /// <param name="filter">LDAP filter to use with the search</param>
        /// <param name="scope">Scope of the search (base object, one level, subtree).</param>
        /// <returns>
        /// A <see cref="SearchResultCollection"/> containing the results of the search or null on failure.
        /// </returns>
        [DirectoryServicesPermission(SecurityAction.LinkDemand)]
        public static DirectoryQuery CreateFrom(QueryFilter filter, SearchScope scope)
        {
            string rootDSE = NamingContext.DefaultNamingContext;

            if (rootDSE != null)
            {
                return CreateFrom(filter, rootDSE, scope);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Searches for Active Directory objects using the provided filter, starting in the specified BaseDN.
        /// </summary>
        /// <param name="filter">LDAP filter to use with the search</param>
        /// <param name="baseDN">BaseDN to begin the search in.</param>
        /// <param name="scope">Scope of the search (base object, one level, subtree).</param>
        /// <returns>
        /// A <see cref="SearchResultCollection"/> containing the results of the search or null on failure.
        /// </returns>
        [DirectoryServicesPermission(SecurityAction.LinkDemand)]
        public static DirectoryQuery CreateFrom(QueryFilter filter, string baseDN, SearchScope scope)
        {
            return CreateFrom(filter, baseDN, scope, ObjectProperty.CommonProperties);
        }

        /// <summary>
        /// Searches for Active Directory objects using the provided filter, starting in the specified BaseDN.
        /// </summary>
        /// <param name="filter">LDAP filter to use with the search</param>
        /// <param name="baseDN">BaseDN to begin the search in.</param>
        /// <param name="scope">Scope of the search (base object, one level, subtree).</param>
        /// <param name="properties">A list of properties to return for each object</param>
        /// <returns>
        /// A <see cref="SearchResultCollection"/> containing the results of the search or null on failure.
        /// </returns>
        [DirectoryServicesPermission(SecurityAction.LinkDemand)]
        public static DirectoryQuery CreateFrom(QueryFilter filter, string baseDN, SearchScope scope, IEnumerable<string> properties)
        {
            DirectoryQuery query = new DirectoryQuery()
            {
                BaseDirectoryPath = baseDN,
                Scope = scope,
                Filter = filter,
                PageSize = 15000,
                CacheResults = false
            };

            query.Properties.AddRange(properties.ToArray());
            return query;
        }
    }
}
