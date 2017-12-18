﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeComplexEntityRepository.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    using SAF.Testing.Common;

    /// <summary>
    /// The fake entity repository.
    /// </summary>
    public class FakeComplexEntityRepository : EntityRepository<FakeComplexEntity, FakeComplexRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeComplexEntityRepository"/> class.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider.
        /// </param>
        public FakeComplexEntityRepository(IRepositoryProvider repositoryProvider)
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
        protected override ItemSelection<FakeComplexRow> GetUniqueItemSelection(FakeComplexRow item)
        {
            return this.GetKeySelection(item, row => row.FakeComplexEntityId, row => row.UniqueName);
        }

        /// <summary>
        /// The construct entity.
        /// </summary>
        /// <param name="dataItem">
        /// The data item.
        /// </param>
        /// <param name="repositoryProvider">
        /// The repository provider.
        /// </param>
        /// <returns>
        /// The <see cref="FakeComplexEntity"/>.
        /// </returns>
        protected override FakeComplexEntity ConstructEntity(FakeComplexRow dataItem, IRepositoryProvider repositoryProvider)
        {
            return this.EntityMapper.Map<FakeComplexEntity>(
                dataItem,
                type =>
                new FakeComplexEntity(
                    dataItem.UniqueName,
                    this.EntityMapper.Map<FakeSubEntity>(dataItem),
                    (FakeEnumeration)dataItem.FakeEnumerationId,
                    this.EntityMapper.Map<FakeCreatedBy>(dataItem),
                    dataItem.CreationTime,
                    dataItem.FakeComplexEntityId));
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
            var childRepo = new FakeChildEntityRepository(this.RepositoryProvider);

            var example = new FakeChildRow { FakeComplexEntityId = entity.FakeComplexEntityId.GetValueOrDefault() };
            var exampleQuery = new ExampleQuery<FakeChildRow>(example, row => row.FakeComplexEntityId);

            // In this case, when the fake child entity is created, it is automatically added to the entity's list of children. Some 
            // other method could also be used if the child constructor does not do this.
            var children = childRepo.SelectEntities(exampleQuery);
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
        protected override void SaveDependencies(FakeComplexEntity entity, IRepositoryProvider provider, FakeComplexRow dataItem)
        {
            var subEntityRepo = new FakeSubEntityRepository(provider);
            subEntityRepo.Save(entity.FakeSubEntity);

            var subMultiReferenceEntityRepo = new FakeMultiReferenceEntityRepository(provider);
            subMultiReferenceEntityRepo.Save(entity.CreatedBy);
            subMultiReferenceEntityRepo.Save(entity.ModifiedBy);

            ////dataItem.FakeSubSubEntityId = entity.FakeSubEntity.FakeSubSubEntity.FakeSubSubEntityId.GetValueOrDefault();
            ////dataItem.FakeSubEntityId = entity.FakeSubEntity.FakeSubEntityId.GetValueOrDefault();
            ////dataItem.CreatedByFakeMultiReferenceEntityId =
            ////    entity.CreatedBy.FakeMultiReferenceEntityId.GetValueOrDefault();

            ////dataItem.ModifiedByFakeMultiReferenceEntityId =
            ////    entity.ModifiedBy.FakeMultiReferenceEntityId.GetValueOrDefault();
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
        protected override void SaveDependents(FakeComplexEntity entity, IRepositoryProvider provider, FakeComplexRow dataItem)
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
            var childRepo = new FakeChildEntityRepository(provider);

            foreach (var childEntity in entity.ChildEntities)
            {
                childRepo.Save(childEntity);
            }
        }
    }
}
