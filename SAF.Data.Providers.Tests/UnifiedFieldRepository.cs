// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnifiedFieldRepository.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    using System;

    using Startitecture.Orm.Common;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Sql;

    /// <summary>
    /// The unified field repository.
    /// </summary>
    public class UnifiedFieldRepository : EntityRepository<UnifiedField, UnifiedFieldRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnifiedFieldRepository"/> class.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider.
        /// </param>
        public UnifiedFieldRepository(IRepositoryProvider repositoryProvider)
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
        protected override ItemSelection<UnifiedFieldRow> GetUniqueItemSelection(UnifiedFieldRow item)
        {
            return this.GetKeySelection(item, row => row.UnifiedFieldId);
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
        protected override void SaveDependents(UnifiedField entity, IRepositoryProvider provider, UnifiedFieldRow dataItem)
        {
            if (entity.IsSystemField)
            {
                var source = new UnifiedFieldSystemSourceRow
                                 {
                                     UnifiedFieldId = Convert.ToInt32(entity.UnifiedFieldId),
                                     SystemFieldSourceId = Convert.ToInt32(entity.SystemFieldSourceId)
                                 };

                provider.Save(source, source.ToExampleSelection(row => row.UnifiedFieldId));
            }
            else
            {
                var source = new UnifiedFieldCustomSourceRow
                                 {
                                     UnifiedFieldId = Convert.ToInt32(entity.UnifiedFieldId),
                                     CustomFieldId = Convert.ToInt32(entity.CustomFieldId)
                                 };

                provider.Save(source, source.ToExampleSelection(row => row.UnifiedFieldId));
            }
        }

        /// <summary>
        /// Deletes the children of the specified entity before the entity itself is deleted.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        protected override void DeleteChildren(UnifiedField entity, IRepositoryProvider provider)
        {
            if (entity.IsSystemField)
            {
                var source = new UnifiedFieldSystemSourceRow
                                 {
                                     UnifiedFieldId = Convert.ToInt32(entity.UnifiedFieldId),
                                     SystemFieldSourceId = Convert.ToInt32(entity.SystemFieldSourceId)
                                 };

                var uniqueSelection = source.ToExampleSelection(row => row.UnifiedFieldId, row => row.SystemFieldSourceId);

                provider.DeleteItems(uniqueSelection);
            }
            else
            {
                var source = new UnifiedFieldCustomSourceRow
                                 {
                                     UnifiedFieldId = Convert.ToInt32(entity.UnifiedFieldId),
                                     CustomFieldId = Convert.ToInt32(entity.CustomFieldId)
                                 };

                var uniqueSelection = source.ToExampleSelection(row => row.UnifiedFieldId, row => row.CustomFieldId);

                provider.DeleteItems(uniqueSelection);
            }

            // delete associated categories
            var category = new UnifiedFieldCategoryRow { UnifiedFieldId = Convert.ToInt32(entity.UnifiedFieldId) };

            var categorySelection = category.ToExampleSelection(row => row.UnifiedFieldId);

            provider.DeleteItems(categorySelection);
        }
    }
}