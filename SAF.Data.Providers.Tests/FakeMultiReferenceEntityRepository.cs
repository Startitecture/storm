// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeMultiReferenceEntityRepository.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Testing.Common
{
    using SAF.Data;
    using SAF.Data.Providers;

    /// <summary>
    /// The fake multi reference entity repository.
    /// </summary>
    public class FakeMultiReferenceEntityRepository : EntityRepository<FakeMultiReferenceEntity, FakeMultiReferenceRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeMultiReferenceEntityRepository"/> class.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider.
        /// </param>
        public FakeMultiReferenceEntityRepository(IRepositoryProvider repositoryProvider)
            : base(repositoryProvider)
        {
        }

        /// <summary>
        /// Gets a unique item selection for the specified item.
        /// </summary>
        /// <param name="item">
        /// The item to create the selection for.
        /// </param>
        /// <returns>
        /// A <see cref="T:SAF.Data.ItemSelection`1"/> for the specified item.
        /// </returns>
        protected override ItemSelection<FakeMultiReferenceRow> GetUniqueItemSelection(FakeMultiReferenceRow item)
        {
            return this.GetKeySelection(
                item,
                row => row.FakeMultiReferenceEntityId,
                row => row.UniqueName);
        }
    }
}
