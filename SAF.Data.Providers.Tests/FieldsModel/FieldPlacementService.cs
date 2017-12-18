// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldPlacementService.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The field placement service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using SAF.Core;
    using SAF.Testing.Common;

    /// <summary>
    /// The field placement service.
    /// </summary>
    public class FieldPlacementService : IFieldPlacementService
    {
        /// <summary>
        /// The field placement repository.
        /// </summary>
        private readonly IFieldPlacementRepository fieldPlacementRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldPlacementService"/> class.
        /// </summary>
        /// <param name="fieldPlacementRepository">
        /// The field placement repository.
        /// </param>
        public FieldPlacementService([NotNull] IFieldPlacementRepository fieldPlacementRepository)
        {
            if (fieldPlacementRepository == null)
            {
                throw new ArgumentNullException(nameof(fieldPlacementRepository));
            }

            this.fieldPlacementRepository = fieldPlacementRepository;
        }

        /// <summary>
        /// Gets the field placements for the specified layout section.
        /// </summary>
        /// <param name="layoutSection">
        /// The layout section to retrieve field placements for.
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

            if (layoutSection.LayoutSectionId.HasValue == false || layoutSection.LayoutSectionId < 1)
            {
                throw new BusinessException(layoutSection, FieldsMessages.IdValueLessThanOne);
            }

            return this.fieldPlacementRepository.GetPlacements(layoutSection);
        }
    }
}