// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldPlacementRepository.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The field placement repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using Startitecture.Orm.Common;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Repository;
    using Startitecture.Orm.Sql;

    /// <summary>
    /// The field placement repository.
    /// </summary>
    public class FieldPlacementRepository : ReadOnlyRepository<FieldPlacement, FieldPlacementRow>, IFieldPlacementRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldPlacementRepository"/> class.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider.
        /// </param>
        public FieldPlacementRepository(IRepositoryProvider repositoryProvider)
            : base(repositoryProvider)
        {
        }

        /// <summary>
        /// Gets the field placements for the specified layout section.
        /// </summary>
        /// <param name="layoutSection">
        /// The layout section.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="FieldPlacement"/> items for the specified
        /// <paramref name="layoutSection"/>.
        /// </returns>
        public IEnumerable<FieldPlacement> GetPlacements([NotNull] LayoutSection layoutSection)
        {
            if (layoutSection == null)
            {
                throw new ArgumentNullException(nameof(layoutSection));
            }

            var layoutSectionId = layoutSection.LayoutSectionId.GetValueOrDefault();
            var itemSelection = Select.From<FieldPlacementRow>().Matching(row => row.LayoutSectionId, layoutSectionId);
            return this.SelectEntities(itemSelection);
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
        protected override ItemSelection<FieldPlacementRow> GetUniqueItemSelection(FieldPlacementRow item)
        {
            return this.GetKeySelection(item, row => row.FieldPlacementId, row => row.LayoutSectionId, row => row.Order);
        }
    }
}