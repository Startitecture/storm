// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutPageSectionService.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The layout page section service.
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
    /// The layout page section service.
    /// </summary>
    public class LayoutPageSectionService : ILayoutPageSectionService
    {
        /// <summary>
        /// The section repository.
        /// </summary>
        private readonly ILayoutPageSectionRepository sectionRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutPageSectionService"/> class.
        /// </summary>
        /// <param name="sectionRepository">
        /// The section repository.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sectionRepository"/> is null.
        /// </exception>
        public LayoutPageSectionService([NotNull] ILayoutPageSectionRepository sectionRepository)
        {
            if (sectionRepository == null)
            {
                throw new ArgumentNullException(nameof(sectionRepository));
            }

            this.sectionRepository = sectionRepository;
        }

        /// <summary>
        /// Gets the page sections for the specified <paramref name="page"/>.
        /// </summary>
        /// <param name="page">
        /// The layout page to get the page sections for.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="LayoutPageSection"/> items for the specified
        /// <paramref name="page"/>.
        /// </returns>
        public IEnumerable<LayoutPageSection> GetSections([NotNull] LayoutPage page)
        {
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page));
            }

            if (page.LayoutPageId.HasValue == false || page.LayoutPageId < 1)
            {
                throw new BusinessException(page, FieldsMessages.IdValueLessThanOne);
            }

            return this.sectionRepository.GetSections(page);
        }

        /// <summary>
        /// Gets the page sections for the specified <paramref name="layout"/>.
        /// </summary>
        /// <param name="layout">
        /// The form layout to get the page sections for.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="LayoutPageSection"/> items for the specified
        /// <paramref name="layout"/>.
        /// </returns>
        public IEnumerable<LayoutPageSection> GetSections([NotNull] FormLayout layout)
        {
            if (layout == null)
            {
                throw new ArgumentNullException(nameof(layout));
            }

            if (layout.FormLayoutId.HasValue == false || layout.FormLayoutId < 1)
            {
                throw new BusinessException(layout, FieldsMessages.IdValueLessThanOne);
            }

            return this.sectionRepository.GetSections(layout);
        }

        /// <summary>
        /// Gets the <see cref="LayoutPageSection"/> with the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">
        /// The ID of the section to get.
        /// </param>
        /// <returns>
        /// The <see cref="LayoutPageSection"/> associated with the specified <paramref name="id"/>, or null if no section is
        /// found.
        /// </returns>
        public LayoutPageSection GetSection(int id)
        {
            if (id < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(id), id, FieldsMessages.IdValueLessThanOne);
            }

            return this.sectionRepository.GetSection(id);
        }
    }
}