// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormService.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The form service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Orm.Testing.Model;

    /// <summary>
    /// The form service.
    /// </summary>
    public class FormService
    {
        /// <summary>
        /// The action context.
        /// </summary>
        private readonly IActionContext actionContext;

        /// <summary>
        /// The form repository.
        /// </summary>
        private readonly IFormRepository formRepository;

        /// <summary>
        /// The version service.
        /// </summary>
        private readonly IFormVersionService versionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormService"/> class.
        /// </summary>
        /// <param name="actionContext">
        /// The action context.
        /// </param>
        /// <param name="formRepository">
        /// The form repository.
        /// </param>
        /// <param name="versionService">
        /// The version Service.
        /// </param>
        public FormService(
            [NotNull] IActionContext actionContext,
            [NotNull] IFormRepository formRepository,
            [NotNull] IFormVersionService versionService)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException(nameof(actionContext));
            }

            if (formRepository == null)
            {
                throw new ArgumentNullException(nameof(formRepository));
            }

            if (versionService == null)
            {
                throw new ArgumentNullException(nameof(versionService));
            }

            this.actionContext = actionContext;
            this.formRepository = formRepository;
            this.versionService = versionService;
        }

        /// <summary>
        /// Gets a form by ID.
        /// </summary>
        /// <param name="id">
        /// The ID of the form.
        /// </param>
        /// <returns>
        /// The <see cref="Form"/> with the specified <paramref name="id"/>, or null if no form is found.
        /// </returns>
        public Form GetById(int id)
        {
            var form = this.formRepository.GetForm(id);
            form?.Load(this.versionService);
            return form;
        }

        /// <summary>
        /// Gets a form by name.
        /// </summary>
        /// <param name="name">
        /// The name of the form.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="name"/> is null or whitespace.
        /// </exception>
        /// <returns>
        /// The <see cref="Form"/> matching the name, or null if there is no match.
        /// </returns>
        public Form GetByName([NotNull] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var form = this.formRepository.GetForm(name);
            form?.Load(this.versionService);
            return form;
        }

        /// <summary>
        /// Searches for a form by name.
        /// </summary>
        /// <param name="nameSearch">
        /// The name search.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="Form"/> items matching the search.
        /// </returns>
        public IEnumerable<FormSearchResult> SearchByName(string nameSearch)
        {
            return
                Enumerable.Select<Form, FormSearchResult>(this.formRepository.SearchForms(nameSearch), form => new FormSearchResult { FormId = form.FormId.GetValueOrDefault(), Name = form.Name });
        }

        /// <summary>
        /// Creates a form and saves it to the repository.
        /// </summary>
        /// <param name="name">
        /// The name of the form.
        /// </param>
        /// <returns>
        /// The <see cref="FormVersion"/> for the newly created form.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="name"/> is null or whitespace.
        /// </exception>
        public FormVersion CreateForm([NotNull] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var form = new Form(name);
            var formVersion = form.Revise(this.actionContext.CurrentPerson);
            this.formRepository.Save(form);
            return this.versionService.SaveVersion(formVersion);
        }

        /// <summary>
        /// Changes the name of the specified form.
        /// </summary>
        /// <param name="form">
        /// The form to change.
        /// </param>
        /// <param name="newName">
        /// The new name of the form.
        /// </param>
        /// <returns>
        /// The saved <see cref="Form"/> with the new name.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="form"/> or <paramref name="newName"/> is null.
        /// </exception>
        public Form ChangeName([NotNull] Form form, [NotNull] string newName)
        {
            if (form == null)
            {
                throw new ArgumentNullException(nameof(form));
            }

            if (string.IsNullOrWhiteSpace(newName))
            {
                throw new ArgumentNullException(nameof(newName));
            }

            form.Rename(newName);
            Singleton<FormValidator>.Instance.ThrowOnValidationFailure(form);
            return this.formRepository.Save(form);
        }

        /// <summary>
        /// Revises the form with a new version and returns the version.
        /// </summary>
        /// <param name="form">
        /// The form to revise.
        /// </param>
        /// <returns>
        /// The new, unsaved <see cref="FormVersion"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="form"/> is null.
        /// </exception>
        public FormVersion Revise([NotNull] Form form)
        {
            if (form == null)
            {
                throw new ArgumentNullException(nameof(form));
            }

            if (form.FormId.HasValue == false || form.FormId < 1)
            {
                throw new BusinessException(form, FieldsMessages.CantReviseUnsavedForm);
            }

            return form.Revise(this.actionContext.CurrentPerson);
        }
    }
}