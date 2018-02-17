﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeRaisedComplexEntityRepository.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    using SAF.Testing.Common;

    using Startitecture.Orm.Common;
    using Startitecture.Orm.Query;

    /// <summary>
    /// The fake raised complex entity repository.
    /// </summary>
    public class FakeRaisedComplexEntityRepository : EntityRepository<FakeComplexEntity, FakeRaisedComplexRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeRaisedComplexEntityRepository"/> class.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider.
        /// </param>
        public FakeRaisedComplexEntityRepository(IRepositoryProvider repositoryProvider)
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
        protected override ItemSelection<FakeRaisedComplexRow> GetUniqueItemSelection(FakeRaisedComplexRow item)
        {
            return this.GetKeySelection(item, row => row.FakeComplexEntityId, row => row.UniqueName);
        }

        /// <summary>
        /// Loads the children of the specified entity.
        /// </summary>
        /// <param name="entity">
        /// The entity to load the children for.
        /// </param>
        /// <param name="provider">
        /// The repository provider.
        /// </param>
        protected override void LoadChildren(FakeComplexEntity entity, IRepositoryProvider provider)
        {
            provider.DependencyContainer.SetDependency(entity.FakeComplexEntityId, entity);

            // Load the children of the entity using their repository.
            var childRepo = new FakeRaisedChildEntityRepository(provider);

            // In this case, when the fake child entity is created, it is automatically added to the entity's list of children. Some 
            // other method could also be used if the child constructor does not do this.
            var children = childRepo.SelectForComplexEntity(entity.FakeComplexEntityId.GetValueOrDefault());
            entity.Load(children);
        }

        /// <summary>
        /// Saves the dependencies of the specified entity.
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
        protected override void SaveDependencies(FakeComplexEntity entity, IRepositoryProvider provider, FakeRaisedComplexRow dataItem)
        {
            var subEntityRepo = new FakeRaisedSubEntityRepository(provider);
            subEntityRepo.Save(entity.FakeSubEntity);

            var subMultiReferenceEntityRepo = new FakeMultiReferenceEntityRepository(provider);
            subMultiReferenceEntityRepo.Save(entity.CreatedBy);
            subMultiReferenceEntityRepo.Save(entity.ModifiedBy);

            ////dataItem.FakeSubEntityId = entity.FakeSubEntity.FakeSubEntityId.GetValueOrDefault();
        }

        /// <summary>
        /// Saves the dependents of the specified entity after saving the entity itself.
        /// </summary>
        /// <param name="entity">
        /// The saved entity.
        /// </param>
        /// <param name="provider">
        /// The repository provider for the current operation.
        /// </param>
        /// <param name="dataItem">
        /// The data item mapped from the entity.
        /// </param>
        /// <remarks>
        /// Use repositories with the entity to save dependents and apply the results to the <paramref name="dataItem"/>.
        /// </remarks>
        protected override void SaveDependents(FakeComplexEntity entity, IRepositoryProvider provider, FakeRaisedComplexRow dataItem)
        {
            var fakeDependentEntityRepo = new FakeDependentEntityRepository(provider);
            this.SaveDependentItem(entity, complexEntity => complexEntity.FakeDependentEntity, fakeDependentEntityRepo, dataItem);
        }

        /// <summary>
        /// Saves the children of the specified entity once the entity itself has been saved.
        /// </summary>
        /// <param name="entity">
        /// The entity to save.
        /// </param>
        /// <param name="provider">
        /// The repository provider for the current operation.
        /// </param>
        protected override void SaveChildren(FakeComplexEntity entity, IRepositoryProvider provider)
        {
            var childRepo = new FakeRaisedChildEntityRepository(provider);

            foreach (var childEntity in entity.ChildEntities)
            {
                childRepo.Save(childEntity);
            }
        }
    }
}