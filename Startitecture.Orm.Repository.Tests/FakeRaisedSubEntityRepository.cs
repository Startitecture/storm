// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeRaisedSubEntityRepository.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository.Tests
{
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Repository.Tests.Models;

    /// <summary>
    /// The fake raised sub entity repository.
    /// </summary>
    public class FakeRaisedSubEntityRepository : EntityRepository<FakeSubEntity, FakeRaisedSubRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeRaisedSubEntityRepository"/> class.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider.
        /// </param>
        public FakeRaisedSubEntityRepository(IRepositoryProvider repositoryProvider)
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
        protected override ItemSelection<FakeRaisedSubRow> GetUniqueItemSelection(FakeRaisedSubRow item)
        {
            return this.GetKeySelection(item, row => row.FakeSubEntityId, row => row.UniqueName);
        }

        /// <summary>
        /// Saves the dependencies of the specified entity prior to saving the entity itself.
        /// </summary>
        /// <param name="entity">
        /// The entity to save.
        /// </param>
        /// <param name="provider">
        /// The repository provider for the current operation.
        /// </param>
        /// <param name="dataItem">
        /// The data item mapped from the entity.
        /// </param>
        /// <remarks>
        /// Use repositories with the entity to save dependencies and apply the results to the <paramref name="dataItem"/>.
        /// </remarks>
        protected override void SaveDependencies(FakeSubEntity entity, IRepositoryProvider provider, FakeRaisedSubRow dataItem)
        {
            var subSubEntityRepo = new FakeSubSubEntityRepository(provider);
            subSubEntityRepo.Save(entity.FakeSubSubEntity);
            ////dataItem.FakeSubSubEntityId = entity.FakeSubSubEntity.FakeSubSubEntityId.GetValueOrDefault();
        }
    }
}