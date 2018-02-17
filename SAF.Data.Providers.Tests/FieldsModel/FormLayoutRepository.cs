// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormLayoutRepository.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The form layout repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using SAF.Testing.Common;

    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Sql;

    /// <summary>
    /// The form layout repository.
    /// </summary>
    public class FormLayoutRepository : EntityRepository<FormLayout, FormLayoutRow>, IFormLayoutRepository
    {
        /// <summary>
        /// The structured command provider.
        /// </summary>
        private readonly IStructuredCommandProvider structuredCommandProvider;

        /// <summary>
        /// The page section service.
        /// </summary>
        [NotNull]
        private readonly ILayoutPageSectionService pageSectionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormLayoutRepository"/> class.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider.
        /// </param>
        /// <param name="structuredCommandProvider">
        /// The structured command provider.
        /// </param>
        /// <param name="pageSectionService">
        /// The page section service.
        /// </param>
        public FormLayoutRepository(
            [NotNull] IRepositoryProvider repositoryProvider,
            [NotNull] IStructuredCommandProvider structuredCommandProvider,
            [NotNull] ILayoutPageSectionService pageSectionService)
            : base(repositoryProvider)
        {
            if (repositoryProvider == null)
            {
                throw new ArgumentNullException(nameof(repositoryProvider));
            }

            if (structuredCommandProvider == null)
            {
                throw new ArgumentNullException(nameof(structuredCommandProvider));
            }

            if (pageSectionService == null)
            {
                throw new ArgumentNullException(nameof(pageSectionService));
            }

            this.structuredCommandProvider = structuredCommandProvider;
            this.pageSectionService = pageSectionService;
        }

        /// <summary>
        /// Gets the form layouts for the specified form version.
        /// </summary>
        /// <param name="formVersion">
        /// The form version to get the layouts for.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="FormLayout"/> items for the specified <paramref name="formVersion"/>.
        /// </returns>
        public IEnumerable<FormLayout> GetLayouts([NotNull] FormVersion formVersion)
        {
            if (formVersion == null)
            {
                throw new ArgumentNullException(nameof(formVersion));
            }

            var itemSelection = Query.From<FormLayoutRow>().Matching(row => row.FormVersionId, formVersion.FormVersionId.GetValueOrDefault());
            return this.SelectEntities(itemSelection);
        }

        /// <summary>
        /// Gets the <see cref="FormLayout"/> with the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">
        /// The ID of the form layout to get.
        /// </param>
        /// <returns>
        /// The <see cref="FormLayout"/> with the associated <paramref name="id"/>, or null if the ID doesn't exist.
        /// </returns>
        public FormLayout GetLayout(int id)
        {
            return this.FirstOrDefaultWithChildren(id);
        }

        /// <summary>
        /// Saves a layout in the repository.
        /// </summary>
        /// <param name="layout">
        /// The layout to save.
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

            return this.SaveWithChildren(layout);
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
        protected override ItemSelection<FormLayoutRow> GetUniqueItemSelection(FormLayoutRow item)
        {
            return this.GetKeySelection(item, row => row.FormLayoutId);
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
        protected override void LoadChildren([NotNull] FormLayout entity, [NotNull] IRepositoryProvider provider)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            provider.DependencyContainer.SetDependency(entity.FormLayoutId, entity);
            entity.Load(this.pageSectionService);
        }

        /// <summary>
        /// Saves the children of the specified entity once the entity itself has been saved.
        /// </summary>
        /// <param name="entity">
        /// The entity to save.
        /// </param>
        /// <param name="provider">
        /// The repository provider for the current operation.
        /// </param>
        protected override void SaveChildren([NotNull] FormLayout entity, [NotNull] IRepositoryProvider provider)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            Singleton<FormLayoutValidator>.Instance.ThrowOnValidationFailure(entity);

            var layoutPageRepository = new LayoutPageRepository(provider, this.pageSectionService, this.structuredCommandProvider);

            foreach (var layoutPage in entity.LayoutPages)
            {
                layoutPageRepository.SaveWithChildren(layoutPage);
            }
        }
    }
}