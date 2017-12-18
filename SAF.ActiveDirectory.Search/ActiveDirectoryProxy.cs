namespace SAF.ActiveDirectory.Search
{
    using System;
    using System.DirectoryServices;
    using System.Security.Permissions;

    using SAF.ActiveDirectory;
    using SAF.Data;
    using SAF.Data.Persistence;
    using SAF.ProcessEngine;

    /// <summary>
    /// A data store for Active Directory queries.
    /// </summary>
    /// <typeparam name="TItem">The type of item to build from the results of the queries.</typeparam>
    public abstract class ActiveDirectoryProxy<TItem> : DataProxy<DirectoryDataSource, TItem>, IDisposable
    {
        /// <summary>
        /// The LDAP connection to use for the default domain.
        /// </summary>
        private System.DirectoryServices.Protocols.LdapConnection defaultLdapConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveDirectoryProxy&lt;TItem&gt;"/> class with the specified
        /// <see cref="ICacheManager"/> for caching domain lookups.
        /// </summary>
        [DirectoryServicesPermission(SecurityAction.LinkDemand)]
        protected ActiveDirectoryProxy()
            : base()
        {
            this.defaultLdapConnection = new System.DirectoryServices.Protocols.LdapConnection(DomainNameContext.DomainName);
        }

        /// <summary>
        /// Disposes of all resources managed by this <see cref="ActiveDirectoryProxy&lt;TItem&gt;"/> instance.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of all resources managed by this <see cref="ActiveDirectoryProxy&lt;TItem&gt;"/> instance.
        /// </summary>
        /// <param name="disposing">A value indicating whether the <see cref="Dispose()"/> method is being called (true)
        /// or the class finalizer (false).</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.defaultLdapConnection != null)
                {
                    this.defaultLdapConnection.Dispose();
                }
            }

            // Clean up unmanaged resources.
        }

        /// <summary>
        /// Performs a search of Directory Services or LDAP services using the specified <see cref="DirectoryQuery"/>.
        /// Implementers should override the <see cref="GenerateItemFromResult(DirectoryQuery, SearchResult)"/>, 
        /// <see cref="GenerateItemFromResult(DirectoryQuery, SDSP.SearchResultEntry)"/>and 
        /// <see cref="GenerateItemFromEmptyResult"/> to process results from the <see cref="DirectoryQuery"/>.
        /// </summary>
        /// <param name="dataSource">The <see cref="DirectoryQuery"/> that identifies the source of the items.</param>
        [DirectoryServicesPermission(SecurityAction.Demand)]
        protected override void EmitItems(DirectoryDataSource dataSource)
        {
            if (dataSource == null)
            {
                throw new ArgumentNullException("dataSource");
            }

            switch (dataSource.Query.SearchType)
            {
                case SearchMethod.DirectoryServices:

                    using (SearchResultCollection results = DirectoryQuery.QueryDirectoryServices(dataSource.Query))
                    {
                        if (results != null && results.Count > 0)
                        {
                            foreach (SearchResult result in results)
                            {
                                this.EmitItem(this.GenerateItemFromResult(dataSource, result));
                            }
                        }
                        else
                        {
                            this.OnRetrievalFailed(new FailedItemEventArgs<DirectoryDataSource>(dataSource, null));
                        }
                    }

                    break;

                case SearchMethod.Ldap:

                    bool useDefaultServer =
                        String.IsNullOrEmpty(dataSource.Query.Server) ||
                        0 == dataSource.Query.Server.CompareTo(DomainNameContext.DomainName);

                    System.DirectoryServices.Protocols.SearchResponse response;

                    if (useDefaultServer)
                    {
                        response = DirectoryQuery.QueryProtocolServices(dataSource.Query, this.defaultLdapConnection);
                    }
                    else
                    {
                        using (System.DirectoryServices.Protocols.LdapConnection connection = new System.DirectoryServices.Protocols.LdapConnection(dataSource.Query.Server))
                        {
                            response = DirectoryQuery.QueryProtocolServices(dataSource.Query, connection);
                        }
                    }

                    if (response.ResultCode == System.DirectoryServices.Protocols.ResultCode.Success)
                    {
                        foreach (System.DirectoryServices.Protocols.SearchResultEntry entry in response.Entries)
                        {
                            this.EmitItem(this.GenerateItemFromResult(dataSource, entry));
                        }
                    }
                    else
                    {
                        this.OnRetrievalFailed(new FailedItemEventArgs<DirectoryDataSource>(dataSource, null));
                    }

                    break;

                default:
                    throw new NotSupportedException(
                        String.Format("The search method {0} is not supported.", dataSource.Query.SearchType));
            }
        }

        /// <summary>
        /// Generates an item from a Directory Services <see cref="SearchResult"/>.
        /// </summary>
        /// <param name="query">The <see cref="DirectoryQuery"/> that returned the result.</param>
        /// <param name="result">The <see cref="SearchResult"/> generated by the <see cref="DirectoryQuery"/>.</param>
        /// <returns>An item that represents the <see cref="SearchResult"/>.</returns>
        protected abstract TItem GenerateItemFromResult(DirectoryDataSource query, SearchResult result);

        /// <summary>
        /// Generates an item from an LDAP <see cref="System.DirectoryServices.Protocols.SearchResultEntry"/>.
        /// </summary>
        /// <param name="query">The <see cref="DirectoryQuery"/> that returned the result.</param>
        /// <param name="entry">The <see cref="System.DirectoryServices.Protocols.SearchResultEntry"/> generated by the <see cref="DirectoryQuery"/>.</param>
        /// <returns>An item that represents the <see cref="System.DirectoryServices.Protocols.SearchResultEntry"/>.</returns>
        protected abstract TItem GenerateItemFromResult(DirectoryDataSource query, System.DirectoryServices.Protocols.SearchResultEntry entry);
    }
}
