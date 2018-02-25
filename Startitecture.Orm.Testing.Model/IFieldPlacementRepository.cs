// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFieldPlacementRepository.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The FieldPlacementRepository interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model
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