// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutSectionService.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The layout section service.
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
    ///     The layout section service.
    /// </summary>
    public class LayoutSectionService : ILayoutSectionService
    {
        /// <summary>
        ///     The section repository.
        /// </summary>
        private readonly ILayoutSectionRepository sectionRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutSectionService"/> class.
        /// </summary>
        /// <param name="sectionRepository">
        /// The section repository.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sectionRepository"/> is null.
        /// </exception>
        public LayoutSectionService([NotNull] ILayoutSectionRepository sectionRepository)
        {
            if (sectionRepository == null)
            {
                throw new ArgumentNullException(nameof(sectionRepository));
            }

            this.sectionRepository = sectionRepository;
        }

        /// <summary>
        ///     Gets all the layout sections in the repository.
        /// </summary>
        /// <returns>
        ///     An <see cref="IEnumerable{T}" /> of <see cref="LayoutSection" /> items.
        /// </returns>
        public IEnumerable<LayoutSection> SelectAllSections()
        {
            return this.sectionRepository.SelectAll();
        }

        /// <summary>
        /// Gets the <see cref="LayoutSection"/> associated with the specified <paramref name="id"/>;
        /// </summary>
        /// <param name="id">
        /// The ID of the layout section to get.
        /// </param>
        /// <returns>
        /// The <see cref="LayoutSection"/> with the associated <paramref name="id"/>, or null if no section is found.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="id"/> is less than one.
        /// </exception>
        public LayoutSection GetSection(int id)
        {
            if (id < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(id), id, FieldsMessages.IdValueLessThanOne);
            }

            return this.sectionRepository.GetSection(id);
        }

        /// <summary>
        /// Gets the <see cref="LayoutSection"/> associated with the specified <paramref name="identifier"/>;
        /// </summary>
        /// <param name="identifier">
        /// The ID of the layout section to get.
        /// </param>
        /// <returns>
        /// The <see cref="LayoutSection"/> with the associated <paramref name="identifier"/>, or null if no section is
        ///     found.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="identifier"/> is less than one.
        /// </exception>
        public LayoutSection GetSection(Guid identifier)
        {
            if (identifier == Guid.Empty)
            {
                throw new BusinessException(identifier, FieldsMessages.IdentifierIsEmptyGuid);
            }

            return this.sectionRepository.GetSection(identifier);
        }

        /// <summary>
        /// Saves the <paramref name="section"/> to a repository.
        /// </summary>
        /// <param name="section">
        /// The section to save.
        /// </param>
        /// <returns>
        /// The saved <see cref="LayoutSection"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="section"/> is null.
        /// </exception>
        public LayoutSection SaveSection(LayoutSection section)
        {
            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            Singleton<LayoutSectionValidator>.Instance.ThrowOnValidationFailure(section);
            return this.sectionRepository.SaveWithPlacements(section);
        }
    }
}