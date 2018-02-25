namespace Startitecture.Orm.Testing.Model
{
    /// <summary>
    /// The contracting entity.
    /// </summary>
    public class ContractingEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContractingEntity"/> class.
        /// </summary>
        public ContractingEntity()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContractingEntity"/> class.
        /// </summary>
        /// <param name="contractingEntityId">
        /// The contracting entity ID.
        /// </param>
        public ContractingEntity(int? contractingEntityId)
        {
            this.ContractingEntityId = contractingEntityId;
        }

        /// <summary>
        /// Gets the contracting entity ID.
        /// </summary>
        public int? ContractingEntityId { get; private set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }
    }
}