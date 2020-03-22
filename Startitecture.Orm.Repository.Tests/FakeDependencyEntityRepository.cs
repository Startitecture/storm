// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeDependencyEntityRepository.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository.Tests
{
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Testing.Model;

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
        /// <param name="entityMapper">
        /// The entity mapper.
        /// </param>
        public FakeDependencyEntityRepository(IRepositoryProvider repositoryProvider, IEntityMapper entityMapper)
            : base(repositoryProvider, entityMapper, entity => entity.FakeDependencyEntityId)
        {
        }

        /// <summary>
        /// Gets a unique item selection for the specified item.
        /// </summary>
        /// <param name="item">
        /// The item to create the selection for.
        /// </param>
        /// <returns>
        /// A <see cref="ItemSelection{T}"/> for the specified item.
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
