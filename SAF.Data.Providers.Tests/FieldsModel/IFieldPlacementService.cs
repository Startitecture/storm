// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFieldPlacementService.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The FieldPlacementService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using System.Collections.Generic;

    /// <summary>
    /// The FieldPlacementService interface.
    /// </summary>
    public interface IFieldPlacementService
    {
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
        IEnumerable<FieldPlacement> GetPlacements(LayoutSection layoutSection);
    }
}