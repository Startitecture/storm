namespace SAF.Data.Providers.Tests.FieldsModel
{
    /// <summary>
    /// The contract category.
    /// </summary>
    public enum ContractCategory
    {
        /// <summary>
        /// No category defined.
        /// </summary>
        None = 0,

        /// <summary>
        /// The provider services category.
        /// </summary>
        ProviderServices = 1,

        /// <summary>
        /// The purchased services category.
        /// </summary>
        PurchasedServices = 2,

        /// <summary>
        /// The capital expense category.
        /// </summary>
        CapitalExpense = 3,

        /// <summary>
        /// The clinical services category.
        /// </summary>
        ClinicalServices = 4,

        /// <summary>
        /// The facility category.
        /// </summary>
        Facility = 5,

        /// <summary>
        /// The regulatory category.
        /// </summary>
        Regulatory = 6,

        /// <summary>
        /// The physician employment category.
        /// </summary>
        PhysicianEmployment = 7,

        /// <summary>
        /// The physician contract category.
        /// </summary>
        PhysicianContract = 8,

        /// <summary>
        /// The provider group contract category.
        /// </summary>
        ProviderGroupContract = 9,

        /// <summary>
        /// The medical equipment category.
        /// </summary>
        MedicalEquipment = 10,

        /// <summary>
        /// The non medical equipment category.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly",
            MessageId = "NonMedical",
            Justification = "The phrase is 'Non-Medical' and this casing is most readable for that.")]
        NonMedicalEquipment = 11,

        /// <summary>
        /// The fair market value category.
        /// </summary>
        FairMarketValue = 12
    }
}