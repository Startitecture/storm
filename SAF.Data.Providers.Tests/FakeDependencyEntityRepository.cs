// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeDependencyEntityRepository.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    using SAF.Testing.Common;

    /// <summary>
    /// The fake dependency entity repository.
    /// </summary>
    public class FakeDependencyEntityRepository : EntityRepository<FakeDependencyEntity, FakeDependencyRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeDependencyEntityRepository"/> class.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider.
        /// </param>
        public FakeDependencyEntityRepository(IRepositoryProvider repositoryProvider)
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
        protected override ItemSelection<FakeDependencyRow> GetUniqueItemSelection(FakeDependencyRow item)
        {
            return this.GetKeySelection(
                item, 
                row => row.FakeDependencyEntityId, 
                row => row.FakeComplexEntityId, 
                row => row.UniqueName);
        }
    }
}
