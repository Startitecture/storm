// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormVersionService.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The form version service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using JetBrains.Annotations;

    using Startitecture.Core;

    /// <summary>
    /// The form version service.
    /// </summary>
    public class FormVersionService : IFormVersionService
    {
        /// <summary>
        /// The action context.
        /// </summary>
        private readonly IActionContext actionContext;

        /// <summary>
        /// The form version repository.
        /// </summary>
        private readonly IFormVersionRepository formVersionRepository;

        /// <summary>
        /// The form layout service.
        /// </summary>
        private readonly IFormLayoutService formLayoutService;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormVersionService"/> class.
        /// </summary>
        /// <param name="actionContext">
        /// The action context.
        /// </param>
        /// <param name="formVersionRepository">
        /// The form version repository.
        /// </param>
        /// <param name="formLayoutService">
        /// The form layout service.
        /// </param>
        public FormVersionService(
            [NotNull] IActionContext actionContext,
            [NotNull] IFormVersionRepository formVersionRepository,
            [NotNull] IFormLayoutService formLayoutService)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException(nameof(actionContext));
            }

            if (formVersionRepository == null)
            {
                throw new ArgumentNullException(nameof(formVersionRepository));
            }

            if (formLayoutService == null)
            {
                throw new ArgumentNullException(nameof(formLayoutService));
            }

            this.actionContext = actionContext;
            this.formVersionRepository = formVersionRepository;
            this.formLayoutService = formLayoutService;
        }

        /// <summary>
        /// Gets the <see cref="FormVersion"/> with the associated <paramref name="id"/>.
        /// </summary>
        /// <param name="id">
        /// The ID of the form version to get.
        /// </param>
        /// <returns>
        /// The <see cref="FormVersion"/> with the associated <paramref name="id"/>, or null if no version is found.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="id"/> is null.
        /// </exception>
        public FormVersion GetVersion(int id)
        {
            if (id < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(id), id, FieldsMessages.IdValueLessThanOne);
            }

            return this.formVersionRepository.GetVersion(id);
        }

        /// <summary>
        /// Gets the form versions for the specified <paramref name="form"/>.
        /// </summary>
        /// <param name="form">
        /// The form to load the versions for.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="FormVersion"/> items for the specified <paramref name="form"/>.
        /// </returns>
        public IEnumerable<FormVersion> GetVersions([NotNull] Form form)
        {
            if (form == null)
            {
                throw new ArgumentNullException(nameof(form));
            }

            if (form.FormId.HasValue == false || form.FormId < 1)
            {
                throw new BusinessException(form, FieldsMessages.IdValueLessThanOne);
            }

            var versions = this.formVersionRepository.GetVersions(form);
            return versions;
        }

        /// <summary>
        /// Saves the form version in a repository.
        /// </summary>
        /// <param name="version">
        /// The form version to save.
        /// </param>
        /// <returns>
        /// The saved <see cref="FormVersion"/>.
        /// </returns>
        public FormVersion SaveVersion(FormVersion version)
        {
            if (version == null)
            {
                throw new ArgumentNullException(nameof(version));
            }

            Singleton<FormVersionValidator>.Instance.ThrowOnValidationFailure(version);
            version.UpdateChangeTracking(this.actionContext.CurrentPerson);
            return this.formVersionRepository.Save(version);
        }

        /// <summary>
        /// Adds a layout to the form version.
        /// </summary>
        /// <param name="version">
        /// The version to add the layout to.
        /// </param>
        /// <param name="name">
        /// The name of the new layout.
        /// </param>
        /// <returns>
        /// The newly created <see cref="FormLayout"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="version"/> or <paramref name="name"/> is null.
        /// </exception>
        public FormLayout AddLayout([NotNull] FormVersion version, [NotNull] string name)
        {
            if (version == null)
            {
                throw new ArgumentNullException(nameof(version));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (version.FormLayouts.Any())
            {
                version.Load(this.formLayoutService);
            }

            if (version.FormLayouts.Any(x => x.Name == name))
            {
                throw new BusinessException(version, FieldsMessages.ItemAlreadyAdded);
            }

            return version.AddLayout(name);
        }
    }
}