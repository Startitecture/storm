namespace SAF.ActiveDirectory.Search
{
    using System;
    using System.DirectoryServices;
    using System.Security.Permissions;

    using SAF.Data;

    /// <summary>
    /// Represents a directory store as a data source.
    /// </summary>
    public class DirectoryDataSource : DataSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryDataSource"/> class.
        /// </summary>
        /// <param name="query">The query that defines the scope of data to retrieve.</param>
        public DirectoryDataSource(DirectoryQuery query)
            : base(Convert.ToString(query))
        {
            this.Query = query;
        }

        /// <summary>
        /// Gets the query that represents the data source.
        /// </summary>
        public DirectoryQuery Query { get; private set; }

        /// <summary>
        /// Gets the location of the data source.
        /// </summary>
        public override string Location
        {
            [DirectoryServicesPermission(SecurityAction.Demand)]
            get { return this.Query.BaseDirectoryPath; }
        }

        /// <summary>
        /// Gets the last time the search was modified.
        /// </summary>
        public override DateTimeOffset LastModified
        {
            get { return DateTimeOffset.Now; }
        }
    }
}
