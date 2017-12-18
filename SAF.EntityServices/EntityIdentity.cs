namespace SAF.EntityServices
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Encapsulates the required properties of an entity selection.
    /// </summary>
    [DataContract]
    public class EntityIdentity
    {
        /// <summary>
        /// Gets or sets the entity's ID.
        /// </summary>
        [DataMember]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the ID of the organzational context to which the entity belongs.
        /// </summary>
        [DataMember]
        public long OrganizationalContextId { get; set; }
    }
}