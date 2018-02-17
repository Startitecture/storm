// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormLayoutService.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The form layout service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using SAF.Testing.Common;

    using Startitecture.Core;

    /// <summary>
    /// The form layout service.
    /// </summary>
    public class FormLayoutService : IFormLayoutService
    {
        /// <summary>
        /// The layout repository.
        /// </summary>
        private readonly IFormLayoutRepository layoutRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormLayoutService"/> class.
        /// </summary>
        /// <param name="layoutRepository">
        /// The layout repository.
        /// </param>
        public FormLayoutService([NotNull] IFormLayoutRepository layoutRepository)
        {
            if (layoutRepository == null)
            {
                throw new ArgumentNullException(nameof(layoutRepository));
            }

            this.layoutRepository = layoutRepository;
        }

        /// <summary>
        /// Gets the layouts for the specified form version.
        /// </summary>
        /// <param name="version">
        /// The form version to get the layouts for.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="FormLayout"/> items for the specified <paramref name="version"/>.
        /// </returns>
        public IEnumerable<FormLayout> GetLayouts([NotNull] FormVersion version)
        {
            if (version == null)
            {
                throw new ArgumentNullException(nameof(version));
            }

            if (version.FormVersionId.HasValue == false || version.FormVersionId < 1)
            {
                throw new BusinessException(version, FieldsMessages.IdValueLessThanOne);
            }

            return this.layoutRepository.GetLayouts(version);
        }

        /// <summary>
        /// Adds a layout to the specified form version.
        /// </summary>
        /// <param name="version">
        /// The version to add the layout to.
        /// </param>
        /// <param name="name">
        /// The name of the layout.
        /// </param>
        /// <returns>
        /// The newly created <see cref="FormLayout"/>.
        /// </returns>
        public FormLayout AddLayout([NotNull] FormVersion version, [NotNull] string name)
        {
            if (version == null)
            {
                throw new ArgumentNullException(nameof(version));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(FieldsMessages.StringCannotBeNullOrWhiteSpace, nameof(name));
            }

            return version.AddLayout(name);
        }

        /// <summary>
        /// Saves a form layout into the repository.
        /// </summary>
        /// <param name="layout">
        /// The form layout to save.
        /// </param>
        /// <returns>
        /// The saved <see cref="FormLayout"/>.
        /// </returns>
        public FormLayout SaveLayout([NotNull] FormLayout layout)
        {
            if (layout == null)
            {
                throw new ArgumentNullException(nameof(layout));
            }

            Singleton<FormLayoutValidator>.Instance.ThrowOnValidationFailure(layout);
            return this.layoutRepository.SaveLayout(layout);
        }

        /// <summary>
        /// Gets the form layout with child objects loaded.
        /// </summary>
        /// <param name="id">
        /// The ID of the layout.
        /// </param>
        /// <returns>
        /// The <see cref="FormLayout"/> associated with the specified <paramref name="id"/>, or null if no match is found.
        /// </returns>
        public FormLayout GetLayout(int id)
        {
            if (id < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(id), id, FieldsMessages.IdValueLessThanOne);
            }

            return this.layoutRepository.GetLayout(id);
        }
    }
}