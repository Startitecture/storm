// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUnifiedFieldRepository.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The UnifiedFieldRepository interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The UnifiedFieldRepository interface.
    /// </summary>
    public interface IUnifiedFieldRepository
    {
        /// <summary>
        /// Gets all of the unified fields in the repository.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="UnifiedField"/> items.
        /// </returns>
        IEnumerable<UnifiedField> SelectAllFields();

        /// <summary>
        /// Gets all of the unified fields matching the specified <paramref name="sourceType"/>.
        /// </summary>
        /// <param name="sourceType">
        /// The field source type.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="UnifiedField"/> items matching the <paramref name="sourceType"/>
        /// </returns>
        IEnumerable<UnifiedField> GetFieldsBySourceType(Type sourceType);

        /// <summary>
        /// Saves all of the specified fields into the repository.
        /// </summary>
        /// <param name="fields">
        /// The fields to save.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of saved <see cref="UnifiedField"/> items.
        /// </returns>
        IEnumerable<UnifiedField> SaveFields(IEnumerable<UnifiedField> fields);
    }
}