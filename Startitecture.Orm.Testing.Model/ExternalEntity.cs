namespace Startitecture.Orm.Testing.Model
{
    /// <summary>
    /// The external entity.
    /// </summary>
    public class ExternalEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalEntity"/> class.
        /// </summary>
        /// <param name="externalIdentifier">
        /// The external identifier.
        /// </param>
        public ExternalEntity(string externalIdentifier)
            : this(externalIdentifier, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalEntity"/> class.
        /// </summary>
        /// <param name="externalIdentifier">
        /// The external identifier.
        /// </param>
        /// <param name="externalEntityId">
        /// The external entity ID.
        /// </param>
        public ExternalEntity(string externalIdentifier, int? externalEntityId)
        {
            this.ExternalIdentifier = externalIdentifier;
            this.ExternalEntityId = externalEntityId;
        }

        /// <summary>
        /// Gets the external entity ID.
        /// </summary>
        public int? ExternalEntityId { get; private set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the external identifier.
        /// </summary>
        public string ExternalIdentifier { get; private set; }
    }
}