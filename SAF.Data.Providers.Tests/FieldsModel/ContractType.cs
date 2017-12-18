namespace SAF.Data.Providers.Tests.FieldsModel
{
    /// <summary>
    /// The contract type.
    /// </summary>
    public class ContractType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContractType"/> class.
        /// </summary>
        /// <param name="contractCategory">
        /// The contract category.
        /// </param>
        public ContractType(ContractCategory contractCategory)
            : this(contractCategory, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContractType"/> class.
        /// </summary>
        /// <param name="contractCategory">
        /// The contract category.
        /// </param>
        /// <param name="contractTypeId">
        /// The contract type ID.
        /// </param>
        public ContractType(ContractCategory contractCategory, int? contractTypeId)
        {
            this.ContractCategory = contractCategory;
            this.ContractTypeId = contractTypeId;
        }

        /// <summary>
        /// Gets the contract type ID.
        /// </summary>
        public int? ContractTypeId { get; private set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the contract category.
        /// </summary>
        public ContractCategory ContractCategory { get; private set; }
    }
}