namespace SAF.EntityServices
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Base class for entity search requests.
    /// </summary>
    [DataContract]
    public abstract class EntitySearchBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntitySearchBase"/> class.
        /// </summary>
        protected EntitySearchBase()
        {
        }

        /// <summary>
        /// Gets or sets the unique ID of the entity to search for.
        /// </summary>
        [DataMember]
        public long? EntityId { get; set; }

        /// <summary>
        /// Gets or sets the organizational context ID of the organizational context to select entities from.
        /// </summary>
        [DataMember]
        public long? OrganizationalContextId { get; set; }
    }
}