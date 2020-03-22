// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnifiedFieldRepository.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Sql.Tests
{
    using System;
    using System.Globalization;

    using JetBrains.Annotations;

    using Startitecture.Orm.Common;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Repository;
    using Startitecture.Orm.Testing.Model.DocumentEntities;
    using Startitecture.Orm.Testing.Model.PM;

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
        /// <param name="entityMapper">
        /// The entity mapper.
        /// </param>
        public UnifiedFieldRepository(IRepositoryProvider repositoryProvider, IEntityMapper entityMapper)
            : base(repositoryProvider, entityMapper, field => field.UnifiedFieldId)
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
        protected override void SaveDependents([NotNull] UnifiedField entity, [NotNull] IRepositoryProvider provider, [NotNull] UnifiedFieldRow dataItem)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            if (dataItem == null)
            {
                throw new ArgumentNullException(nameof(dataItem));
            }

            if (entity.IsSystemField)
            {
                var source = new UnifiedFieldSystemSourceRow
                                 {
                                     UnifiedFieldId = Convert.ToInt32(entity.UnifiedFieldId, CultureInfo.CurrentCulture),
                                     SystemFieldSourceId = Convert.ToInt32(entity.SystemFieldSourceId, CultureInfo.CurrentCulture)
                                 };

                provider.Save(source);
            }
            else
            {
                var source = new UnifiedFieldCustomSourceRow
                                 {
                                     UnifiedFieldId = Convert.ToInt32(entity.UnifiedFieldId, CultureInfo.CurrentCulture),
                                     CustomFieldId = Convert.ToInt32(entity.CustomFieldId, CultureInfo.CurrentCulture)
                                 };

                provider.Save(source);
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
        protected override void DeleteChildren([NotNull] UnifiedField entity, [NotNull] IRepositoryProvider provider)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            if (entity.IsSystemField)
            {
                var source = new UnifiedFieldSystemSourceRow
                                 {
                                     UnifiedFieldId = Convert.ToInt32(entity.UnifiedFieldId, CultureInfo.CurrentCulture),
                                     SystemFieldSourceId = Convert.ToInt32(entity.SystemFieldSourceId, CultureInfo.CurrentCulture)
                                 };

                var uniqueSelection = source.ToExampleSelection(row => row.UnifiedFieldId, row => row.SystemFieldSourceId);

                provider.DeleteItems(uniqueSelection);
            }
            else
            {
                var source = new UnifiedFieldCustomSourceRow
                                 {
                                     UnifiedFieldId = Convert.ToInt32(entity.UnifiedFieldId, CultureInfo.CurrentCulture),
                                     CustomFieldId = Convert.ToInt32(entity.CustomFieldId, CultureInfo.CurrentCulture)
                                 };

                var uniqueSelection = source.ToExampleSelection(row => row.UnifiedFieldId, row => row.CustomFieldId);

                provider.DeleteItems(uniqueSelection);
            }

            // delete associated categories
            var category = new UnifiedFieldCategoryRow
                               {
                                   UnifiedFieldId = Convert.ToInt32(entity.UnifiedFieldId, CultureInfo.CurrentCulture)
                               };

            var categorySelection = category.ToExampleSelection(row => row.UnifiedFieldId);

            provider.DeleteItems(categorySelection);
        }
    }
}