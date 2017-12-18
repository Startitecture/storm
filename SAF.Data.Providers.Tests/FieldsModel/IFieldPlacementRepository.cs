// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFieldPlacementRepository.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The FieldPlacementRepository interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using System.Collections.Generic;

    /// <summary>
    /// The FieldPlacementRepository interface.
    /// </summary>
    public interface IFieldPlacementRepository
    {
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
        IEnumerable<FieldPlacement> GetPlacements(LayoutSection layoutSection);
    }
}