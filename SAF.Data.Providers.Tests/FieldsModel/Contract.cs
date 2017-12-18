namespace SAF.Data.Providers.Tests.FieldsModel
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using SAF.Core;
    using SAF.Testing.Common;

    /// <summary>
    /// The contract.
    /// </summary>
    public class Contract
    {
        /// <summary>
        /// The sites.
        /// </summary>
        private readonly List<Site> sites = new List<Site>();

        /// <summary>
        /// The departments.
        /// </summary>
        private readonly List<Department> departments = new List<Department>();

        /// <summary>
        /// The markets.
        /// </summary>
        private readonly List<Market> markets = new List<Market>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Contract"/> class.
        /// </summary>
        /// <param name="contractCategory">
        /// The contract category.
        /// </param>
        /// <param name="subject">
        /// The subject.
        /// </param>
        public Contract(ContractCategory contractCategory, string subject)
            : this(contractCategory, subject, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Contract"/> class.
        /// </summary>
        /// <param name="contractCategory">
        /// The contract category.
        /// </param>
        /// <param name="subject">
        /// The subject.
        /// </param>
        /// <param name="contractId">
        /// The contract ID.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="subject"/> is null or whitespace.
        /// </exception>
        public Contract(ContractCategory contractCategory, [NotNull] string subject, long? contractId)
        {
            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentException(FieldsMessages.StringCannotBeNullOrWhiteSpace, nameof(subject));
            }

            this.ContractCategory = contractCategory;
            this.Subject = subject;
            this.ContractId = contractId;
        }

        /// <summary>
        /// Gets the contract id.
        /// </summary>
        public long? ContractId { get; private set; }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        public string Identifier
        {
            get
            {
                return $"{this.ContractingEntity.ContractingEntityId}.{this.ContractId}";
            }
        }

        /// <summary>
        /// Gets the subject.
        /// </summary>
        public string Subject { get; private set; }

        /// <summary>
        /// Gets the contract category.
        /// </summary>
        public ContractCategory ContractCategory { get; private set; }

        /// <summary>
        /// Gets the contract type.
        /// </summary>
        public ContractType ContractType { get; private set; }

        /// <summary>
        /// Gets the contracting contractingEntity.
        /// </summary>
        public ContractingEntity ContractingEntity { get; private set; }

        /// <summary>
        /// Gets the external contractingEntity.
        /// </summary>
        public ExternalEntity ExternalEntity { get; private set; }

        /// <summary>
        /// Gets the sites associated with the contract.
        /// </summary>
        public IEnumerable<Site> Sites
        {
            get
            {
                return this.sites;
            }
        }

        /// <summary>
        /// Gets the departments associated with the contract.
        /// </summary>
        public IEnumerable<Department> Departments
        {
            get
            {
                return this.departments;
            }
        }

        /// <summary>
        /// Gets the markets associated with the contract.
        /// </summary>
        public IEnumerable<Market> Markets
        {
            get
            {
                return this.markets;
            }
        }

        /// <summary>
        /// Sets the contract type.
        /// </summary>
        /// <param name="contractType">
        /// The contract type to set for this contract.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="contractType"/> is null.
        /// </exception>
        /// <exception cref="BusinessException">
        /// <paramref name="contractType"/> is not valid for the current <see cref="ContractCategory"/>.
        /// </exception>
        public void SetContractType([NotNull] ContractType contractType)
        {
            if (contractType == null)
            {
                throw new ArgumentNullException(nameof(contractType));
            }

            if (contractType.ContractCategory != this.ContractCategory)
            {
                throw new BusinessException(contractType, string.Format(FieldsMessages.ContractTypeInvalidForContractCategory, this.ContractCategory));
            }

            this.ContractType = contractType;
        }

        /// <summary>
        /// Sets the parties of the contract.
        /// </summary>
        /// <param name="contractingEntity">
        /// The contracting entity.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="contractingEntity"/> is null.
        /// </exception>
        public void SetParties([NotNull] ContractingEntity contractingEntity)
        {
            if (contractingEntity == null)
            {
                throw new ArgumentNullException(nameof(contractingEntity));
            }

            this.SetParties(contractingEntity, null);
        }

        /// <summary>
        /// Sets the parties of the contract.
        /// </summary>
        /// <param name="contractingEntity">
        /// The contracting entity.
        /// </param>
        /// <param name="externalEntity">
        /// The external entity.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="contractingEntity"/> is null.
        /// </exception>
        public void SetParties([NotNull] ContractingEntity contractingEntity, ExternalEntity externalEntity)
        {
            if (contractingEntity == null)
            {
                throw new ArgumentNullException(nameof(contractingEntity));
            }

            this.ContractingEntity = contractingEntity;
            this.ExternalEntity = externalEntity;
        }

        /// <summary>
        /// Associates sites with the current contract.
        /// </summary>
        /// <param name="associatedSites">
        /// The sites to associate.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="associatedSites"/> is null.
        /// </exception>
        public void AssociateSites([NotNull] IEnumerable<Site> associatedSites)
        {
            if (associatedSites == null)
            {
                throw new ArgumentNullException(nameof(associatedSites));
            }

            this.sites.Clear();
            this.sites.AddRange(associatedSites);
        }

        /// <summary>
        /// Associates departments with the current contract.
        /// </summary>
        /// <param name="associatedDepartments">
        /// The departments to associate.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="associatedDepartments"/> is null.
        /// </exception>
        public void AssociateDepartments([NotNull] IEnumerable<Department> associatedDepartments)
        {
            if (associatedDepartments == null)
            {
                throw new ArgumentNullException(nameof(associatedDepartments));
            }

            this.departments.Clear();
            this.departments.AddRange(associatedDepartments);
        }

        /// <summary>
        /// Associates markets with the current contract.
        /// </summary>
        /// <param name="associatedMarkets">
        /// The markets to associate.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="associatedMarkets"/> is null. 
        /// </exception>
        public void AssociateMarkets([NotNull] IEnumerable<Market> associatedMarkets)
        {
            if (associatedMarkets == null)
            {
                throw new ArgumentNullException(nameof(associatedMarkets));
            }

            this.markets.Clear();
            this.markets.AddRange(associatedMarkets);
        }
    }
}