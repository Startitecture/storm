namespace SAF.ActiveDirectory.Search
{
    using System;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.DirectoryServices;
    using System.Security.Permissions;

    using SAF.ActiveDirectory;

    using SearchScope = System.DirectoryServices.SearchScope;

    /// <summary>
    /// Encapsulates instructions for searching in Active Directory.
    /// </summary>
    public class DirectoryQuery
    {
        #region Constants

        #region Filters

        /// <summary>
        /// Searches all Active Directory objects.
        /// </summary>
        public const string SearchAllFilter = null;

        /// <summary>
        /// Searches only top-level domain containers.
        /// </summary>
        public static readonly string DomainSearchFilter =
            String.Format(NameValueSearchFormat, ObjectProperty.Category, ObjectCategory.Domain);

        /// <summary>
        /// Searches only server objects.
        /// </summary>
        public static readonly string ServerSearchFilter =
            String.Format(NameValueSearchFormat, ObjectProperty.Category, ObjectCategory.Server);

        /// <summary>
        /// Searches only site container objects.
        /// </summary>
        public static readonly string SiteSearchFilter =
            String.Format(NameValueSearchFormat, ObjectProperty.Category, ObjectCategory.Site);

        /// <summary>
        /// Searches only replication partner objects (NTDS connections).
        /// </summary>
        public static readonly string ReplicationPartnerSearchFilter =
            String.Format(NameValueSearchFormat, ObjectProperty.Category, ObjectCategory.ReplicationPartner);

        /// <summary>
        /// Searches only user object.
        /// </summary>
        public static readonly string UserSearchFilter =
            String.Format(NameValueSearchFormat, ObjectProperty.Category, ObjectCategory.Person);

        /// <summary>
        /// Searches only computer objects.
        /// </summary>
        public static readonly string ComputerSearchFilter =
            String.Format(NameValueSearchFormat, ObjectProperty.Category, ObjectCategory.Computer);

        /// <summary>
        /// Searches only group objects.
        /// </summary>
        public static readonly string GroupSearchFilter =
            String.Format(NameValueSearchFormat, ObjectProperty.Category, ObjectCategory.Group);

        /// <summary>
        /// Searches only organizational unit objects.
        /// </summary>
        public static readonly string OUSearchFilter =
            String.Format(NameValueSearchFormat, ObjectProperty.Category, ObjectCategory.OrganizationalUnit);

        /// <summary>
        /// Searches only Life Care entities with EntityType set.
        /// </summary>
        public static readonly string EntitySearchFilter =
            String.Format("(&{0}(entityType=*))", OUSearchFilter);

        /// <summary>
        /// Searches all container types.
        /// </summary>
        public static readonly string ContainerSearchFilter =
            String.Format(
                "(|({0}={1}){2}" +
                "({0}={3})({0}={4})" +
                "({0}={5})({0}={6}){7}{8})",
                ObjectProperty.Category,
                ObjectCategory.Container,
                OUSearchFilter,
                ObjectCategory.SitesContainer,
                ObjectCategory.ReplicationPartnerContainer,
                ObjectCategory.ServersContainer,
                ObjectCategory.SubnetContainer,
                SiteSearchFilter,
                ServerSearchFilter);

        #endregion

        /// <summary>
        /// An LDAP search filter for searching a specific attribute.
        /// </summary>
        private const string NameValueSearchFormat = "({0}={1})";

        /// <summary>
        /// The format string for the ToString() method.
        /// </summary>
        private const string ToStringFormat = "{0} [{1}, {2}, {3}]";

        #endregion 

        #region Fields

        /// <summary>
        /// The base distinguished name of the search, excluding the protocol specifier.
        /// </summary>
        private string baseDistinguishedName;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryQuery"/> class.
        /// </summary>
        public DirectoryQuery()
        {
            this.Properties = new StringCollection();
            ////this.ObjectCategories = new StringCollection();
            this.Scope = SearchScope.Subtree;
            this.CacheResults = false;
            this.WidenSearchOnFail = false;
            this.PageSize = 1000;
            this.SizeLimit = 15000;
            this.SearchType = SearchMethod.DirectoryServices;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the query filter.
        /// </summary>
        public QueryFilter Filter { get; set; }

        /// <summary>
        /// Gets or sets the base distinguished name to use in the search, including the protocol specifier for use 
        /// with the DirectoryServices namespace. Any forward slashes (/) that are part of the distinguished name must
        /// be escaped with a backslash (\) for use with the ADSI API.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Specifying an LDAP server or domain in the distinguished name along with the LDAP protocol specifier 
        /// (LDAP://) will result in the <see cref="Server"/> property being set to the specified server or domain.
        /// </para>
        /// <para>
        /// See http://www.rlmueller.net/CharactersEscaped.htm for a detailed list of characters that must be escaped.
        /// </para>
        /// </remarks>
        public string BaseDirectoryPath
        {
            [DirectoryServicesPermission(SecurityAction.LinkDemand)]
            get 
            {
                return 
                    NamingContext.GetDirectoryPath(
                        this.Server, this.BaseDistinguishedName ?? NamingContext.DefaultNamingContext);
            }

            set
            {
                // TODO: Create a class for this.
                // For the LDAP base DN, remove the LDAP server designation. First, determine if the LDAP:// protocol
                // specifier is in use. If so, move forward through the string until we encounter either a forward 
                // slash (/) or a backslash (\). ADSI requires forward slashes to be escaped. If we find a backslash
                // first, we know there is no server to remove. If we find a forward slash first, we assume there is a
                // server to remove and remove it. 
                if (String.IsNullOrEmpty(value))
                {
                    this.baseDistinguishedName = null;
                }
                else if (value.StartsWith(NamingContext.LdapProtocolSpecifier))
                {
                    bool? hasServer = null;
                    int subStringAt = NamingContext.LdapProtocolSpecifier.Length;

                    // Find out where to substring at.
                    for (int i = NamingContext.LdapProtocolSpecifier.Length; i < value.Length; i++)
                    {
                        switch (value[i])
                        {
                            case '\\':
                                hasServer = false;
                                break;

                            case '/':
                                hasServer = true;

                                // Set up the server using the base DN's specification.
                                this.Server = 
                                    value.Substring(
                                        NamingContext.LdapProtocolSpecifier.Length,
                                        i - NamingContext.LdapProtocolSpecifier.Length);

                                break;
                        }

                        if (hasServer.HasValue)
                        {
                            break;
                        }
                    }

                    this.baseDistinguishedName = value.Substring(subStringAt);
                }
                else
                {
                    // No protocol specifier means we assume the value is just the DN.
                    this.baseDistinguishedName = value;
                }
            }
        }

        /// <summary>
        /// Gets the base Distinguished Name to use in the search, formatted for use with LDAP services.
        /// </summary>
        public string BaseDistinguishedName
        {
            get 
            {
                return this.baseDistinguishedName; 
            }
        }

        /// <summary>
        /// Gets or sets the scope of the Directory Services search.
        /// </summary>
        public SearchScope Scope { get; set; }

        /// <summary>
        /// Gets the scope of the LDAP services search. This value is computed from the <see cref="Scope"/> value.
        /// </summary>
        public System.DirectoryServices.Protocols.SearchScope LdapScope
        {
            get
            {
                switch (this.Scope)
                {
                    case SearchScope.Base:
                        return System.DirectoryServices.Protocols.SearchScope.Base;
                    case SearchScope.OneLevel:
                        return System.DirectoryServices.Protocols.SearchScope.OneLevel;
                    case SearchScope.Subtree:
                        return System.DirectoryServices.Protocols.SearchScope.Subtree;
                    default:
                        return System.DirectoryServices.Protocols.SearchScope.Base;
                }
            }
        }

        /// <summary>
        /// Gets the properties to include in the results.
        /// </summary>
        public StringCollection Properties { get; private set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether results of searches will be cached 
        /// (true) or not (false).
        /// </summary>
        public bool CacheResults { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entire directory will be searched
        /// if a narrower search fails.
        /// </summary>
        public bool WidenSearchOnFail { get; set; }

        /// <summary>
        /// Gets or sets the size of the pages to return.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of results to return.
        /// </summary>
        public int SizeLimit { get; set; }

        /// <summary>
        /// Gets or sets the type of search to use (LDAP or directory services).
        /// </summary>
        public SearchMethod SearchType { get; set; }

        /// <summary>
        /// Gets or sets the server (or domain) to use for the search.
        /// </summary>
        public string Server { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Searches for Active Directory objects using the provided directive.
        /// </summary>
        /// <param name="query">The <see cref="DirectoryQuery"/> that specifies the search parameters.</param>
        /// <returns>
        /// A <see cref="SearchResultCollection"/> containing the results of the search or null on failure.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para>
        /// The <see cref="M:DirectoryQuery.BaseDirectoryPath"/> property of <paramref name="query"/> is not set.
        /// </para>
        /// <para>
        /// -or-
        /// </para>
        /// <para>
        /// The <see cref="M:DirectoryQuery.BaseDirectoryPath"/> property of <paramref name="query"/> points to a path
        /// that does not exist.
        /// </para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// A <see cref="System.Runtime.InteropServices.COMException"/> was thrown attempting to confirm the existence 
        /// of the base directory path.
        /// </exception>
        [DirectoryServicesPermission(SecurityAction.LinkDemand)]
        public static SearchResultCollection QueryDirectoryServices(DirectoryQuery query)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            if (String.IsNullOrEmpty(query.BaseDirectoryPath))
            {
                throw new ArgumentException(
                    "The base directory path of the directive must be set.", "query");
            }

            try
            {
                if (!DirectoryEntry.Exists(query.BaseDirectoryPath))
                {
                    throw new ArgumentException(
                        String.Format("The path '{0}' does not exist.", query.BaseDirectoryPath), "query");
                }
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                Trace.TraceError(
                    "Could not search for '{0}' at '{1}' (scope '{2}':{3}",
                    query.Filter,
                    query.BaseDirectoryPath,
                    query.Scope,
                    ex);

                throw new InvalidOperationException(
                    String.Format("Searching for path '{0}' resulted in a COM exception.", query.BaseDirectoryPath));
            }

            SearchResultCollection results = null;

            using (DirectoryEntry root = new DirectoryEntry(query.BaseDirectoryPath))
            {
                using (DirectorySearcher searcher = new DirectorySearcher(root))
                {
                    searcher.ReferralChasing = ReferralChasingOption.All;
                    searcher.SearchScope = query.Scope;
                    searcher.Filter = query.Filter.RawFilter;
                    searcher.PageSize = query.PageSize;
                    searcher.SizeLimit = query.SizeLimit;
                    searcher.CacheResults = query.CacheResults;
                    searcher.Sort = new SortOption(ObjectProperty.Name, SortDirection.Ascending);
                 
                    if (query.Properties != null && query.Properties.Count > 0)
                    {
                        foreach (string property in query.Properties)
                        {
                            searcher.PropertiesToLoad.Add(property);
                        }
                    }

                    try
                    {
                        results = searcher.FindAll();
                    }
                    catch (InvalidOperationException ex)
                    {
                        Trace.TraceError("Could not search '{0}' using '{1}': {2}", query.BaseDirectoryPath, query.Filter, ex);
                    }
                    catch (NotSupportedException ex)
                    {
                        Trace.TraceError("Could not search '{0}' using '{1}': {2}", query.BaseDirectoryPath, query.Filter, ex);
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Executes an LDAP search, returning an LDAP SearchResponse object.
        /// </summary>
        /// <param name="directive">The search directive containing instructions for the search.</param>
        /// <param name="connection">LDAP connection to use for the search. The Directory property will override
        /// the <see cref="DirectoryQuery"/>'s <see cref="M:Server"/> property.</param>
        /// <returns>An LDAP <see cref="SDSP.SearchResponse"/> object or null if no result was returned.</returns>
        /// <exception cref="SDSP.DirectoryOperationException">
        /// Thrown if the server returned a <see cref="SDSP.DirectoryResponse"/> object with an error.
        /// </exception>
        /// <exception cref="SDSP.LdapException">
        /// The error code returned by LDAP does not map to a <see cref="SDSP.ResultCode"/> enumeration error code.
        /// </exception>
        [DirectoryServicesPermission(SecurityAction.LinkDemand)]
        public static System.DirectoryServices.Protocols.SearchResponse QueryProtocolServices(DirectoryQuery directive, System.DirectoryServices.Protocols.DirectoryConnection connection)
        {
            if (directive == null)
            {
                throw new ArgumentNullException("directive");
            }

            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }

            if (directive.BaseDirectoryPath == null)
            {
                directive.BaseDirectoryPath = String.Empty;
            }

            // Match the directive's Server property to the LDAP connection's Directory property.
            directive.Server = Convert.ToString(connection.Directory);

            System.DirectoryServices.Protocols.SearchRequest search = new System.DirectoryServices.Protocols.SearchRequest()
            {
                Scope = directive.LdapScope,
                DistinguishedName = directive.BaseDirectoryPath,
                Filter = directive.Filter,
                SizeLimit = directive.SizeLimit
            };

            if (directive.Properties.Count > 0)
            {
                string[] attributes = new string[directive.Properties.Count];
                directive.Properties.CopyTo(attributes, 0);
                search.Attributes.AddRange(attributes);
            }

            return (System.DirectoryServices.Protocols.SearchResponse)connection.SendRequest(search);
        }

        /// <summary>
        /// Returns the raw filter and basic settings of the search directive.
        /// </summary>
        /// <returns>The raw filter and basic settings of the search directive.</returns>
        [DirectoryServicesPermission(SecurityAction.Demand)]
        public override string ToString()
        {
            return String.Format(ToStringFormat, this.Filter, this.BaseDirectoryPath, this.Server, this.Scope);
        }

        #endregion
    }
}
