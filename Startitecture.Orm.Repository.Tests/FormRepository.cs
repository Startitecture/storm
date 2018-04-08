// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormRepository.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The form repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository.Tests
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using Startitecture.Orm.Common;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Testing.Model;
    using Startitecture.Orm.Testing.Model.FieldEntities;

    /// <summary>
    /// The form repository.
    /// </summary>
    public class FormRepository : EntityRepository<Form, FormRow>, IFormRepository
    {
        /// <summary>
        /// The version service.
        /// </summary>
        private readonly IFormVersionService versionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormRepository"/> class.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider.
        /// </param>
        /// <param name="versionService">
        /// The version service.
        /// </param>
        public FormRepository(IRepositoryProvider repositoryProvider, [NotNull] IFormVersionService versionService)
            : base(repositoryProvider)
        {
            if (versionService == null)
            {
                throw new ArgumentNullException(nameof(versionService));
            }

            this.versionService = versionService;
        }

        /// <summary>
        /// Gets the <see cref="Form"/> with the specified ID.
        /// </summary>
        /// <param name="id">
        /// The ID of the form to get.
        /// </param>
        /// <returns>
        /// A <see cref="Form"/> with a matching <paramref name="id"/>, or null if no match is found.
        /// </returns>
        public Form GetForm(int id)
        {
            return this.FirstOrDefaultWithChildren(id);
        }

        /// <summary>
        /// Gets the <see cref="Form"/> with the specified name.
        /// </summary>
        /// <param name="name">
        /// The name of the form to get.
        /// </param>
        /// <returns>
        /// A <see cref="Form"/> with a matching <paramref name="name"/>, or null if no match is found.
        /// </returns>
        public Form GetForm([NotNull] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(FieldsMessages.StringCannotBeNullOrWhiteSpace, nameof(name));
            }

            return this.FirstOrDefaultWithChildren(name);
        }

        /// <summary>
        /// Searches the repository for forms that match the specified full or partial name.
        /// </summary>
        /// <param name="nameSearch">
        /// The name search.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="Form"/> items.
        /// </returns>
        public IEnumerable<Form> SearchForms([NotNull] string nameSearch)
        {
            if (nameSearch == null)
            {
                throw new ArgumentNullException(nameof(nameSearch));
            }

            var itemSelection = this.Select<Form, FormRow>().WhereEqual(row => row.Name, nameSearch);
            return this.SelectEntities(itemSelection);
        }

        /// <summary>
        /// Gets a unique item selection for the specified item.
        /// </summary>
        /// <param name="item">
        /// The item to create the selection for.
        /// </param>
        /// <returns>
        /// A <see cref="T:SAF.Data.ItemSelection`1"/> for the specified item.
        /// </returns>
        protected override ItemSelection<FormRow> GetUniqueItemSelection([NotNull] FormRow item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return this.GetKeySelection(item, row => row.FormId, row => row.Name);
        }

        /// <summary>
        /// Loads the children of the specified entity.
        /// </summary>
        /// <param name="entity">
        /// The entity to load the children for.
        /// </param>
        /// <param name="provider">
        /// The repository provider.
        /// </param>
        protected override void LoadChildren([NotNull] Form entity, [NotNull] IRepositoryProvider provider)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            provider.DependencyContainer.SetDependency(entity.FormId, entity);
            entity.Load(this.versionService);
        }
    }
}