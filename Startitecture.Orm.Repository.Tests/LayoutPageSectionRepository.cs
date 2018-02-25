// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutPageSectionRepository.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The layout page section repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using JetBrains.Annotations;

    using Startitecture.Orm.Common;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Sql;
    using Startitecture.Orm.Testing.Model;
    using Startitecture.Orm.Testing.Model.FieldEntities;

    /// <summary>
    /// The layout page section repository.
    /// </summary>
    public class LayoutPageSectionRepository : ReadOnlyRepository<LayoutPageSection, LayoutPageSectionRow>, ILayoutPageSectionRepository
    {
        /// <summary>
        /// The placement service.
        /// </summary>
        private readonly IFieldPlacementService placementService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutPageSectionRepository"/> class.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider.
        /// </param>
        /// <param name="placementService">
        /// The placement service.
        /// </param>
        public LayoutPageSectionRepository(IRepositoryProvider repositoryProvider, [NotNull] IFieldPlacementService placementService)
            : base(repositoryProvider)
        {
            if (placementService == null)
            {
                throw new ArgumentNullException(nameof(placementService));
            }

            this.placementService = placementService;
        }

        /// <summary>
        /// Gets the <see cref="LayoutPageSection"/> items for the specified <paramref name="page"/>.
        /// </summary>
        /// <param name="page">
        /// The layout to get the layout page sections for.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="LayoutPageSection"/> items for the specified <paramref name="page"/>.
        /// </returns>
        public IEnumerable<LayoutPageSection> GetSections([NotNull] LayoutPage page)
        {
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page));
            }

            var itemSelection = Select.From<LayoutPageSectionRow>().Matching(row => row.LayoutPageId, page.LayoutPageId.GetValueOrDefault());
            var sections = Enumerable.ToList<LayoutPageSection>(this.SelectEntities(itemSelection));

            // TODO: To make this a single selection, add a method to FieldPlacementRepository that takes the entire page.
            foreach (var layoutPageSection in sections)
            {
                layoutPageSection.LayoutSection.Load(this.placementService);
            }

            return sections;
        }

        /// <summary>
        /// Gets the <see cref="LayoutPageSection"/> items for the specified <paramref name="layout"/>.
        /// </summary>
        /// <param name="layout">
        /// The layout to get the layout page sections for.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="LayoutPageSection"/> items for the specified <paramref name="layout"/>
        /// .
        /// </returns>
        public IEnumerable<LayoutPageSection> GetSections([NotNull] FormLayout layout)
        {
            if (layout == null)
            {
                throw new ArgumentNullException(nameof(layout));
            }

            var itemSelection = Select.From<LayoutPageSectionRow>()
                .Matching(row => row.LayoutPage.FormLayoutId, layout.FormLayoutId.GetValueOrDefault());

            return this.SelectEntities(itemSelection);
        }

        /// <summary>
        /// Gets the <see cref="LayoutPageSection"/> with the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">
        /// The ID of the layout page section to get.
        /// </param>
        /// <returns>
        /// The <see cref="LayoutPageSection"/> with the specified <paramref name="id"/>, or null if no section is found.
        /// </returns>
        public LayoutPageSection GetSection(int id)
        {
            return this.FirstOrDefaultWithChildren(id);
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
        protected override ItemSelection<LayoutPageSectionRow> GetUniqueItemSelection([NotNull] LayoutPageSectionRow item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return this.GetKeySelection(item, row => row.LayoutPageSectionId, row => row.LayoutPageId, row => row.Order);
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
        protected override void LoadChildren([NotNull] LayoutPageSection entity, [NotNull] IRepositoryProvider provider)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            provider.DependencyContainer.SetDependency(entity.LayoutSection.LayoutSectionId, entity.LayoutSection);
            entity.LayoutSection.Load(this.placementService);
        }
    }
}