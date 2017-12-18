namespace SAF.Data.Providers.Tests.FieldsModel
{
    /// <summary>
    /// The site.
    /// </summary>
    public class Site
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Site"/> class.
        /// </summary>
        public Site()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Site"/> class.
        /// </summary>
        /// <param name="siteId">
        /// The site ID.
        /// </param>
        public Site(int? siteId)
        {
            this.SiteId = siteId;
        }

        /// <summary>
        /// Gets the site ID.
        /// </summary>
        public int? SiteId { get; private set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }
    }
}