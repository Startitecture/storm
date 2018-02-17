// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutPageRepository.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The layout page repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using JetBrains.Annotations;

    using SAF.Testing.Common;

    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Query;

    /// <summary>
    /// The layout page repository.
    /// </summary>
    public class LayoutPageRepository : EntityRepository<LayoutPage, LayoutPageRow>, ILayoutPageRepository
    {
        /// <summary>
        /// The layout page section service.
        /// </summary>
        private readonly ILayoutPageSectionService layoutPageSectionService;

        /// <summary>
        /// The structured command provider.
        /// </summary>
        private readonly IStructuredCommandProvider structuredCommandProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutPageRepository"/> class.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider.
        /// </param>
        /// <param name="layoutPageSectionService">
        /// The layout page section service.
        /// </param>
        /// <param name="structuredCommandProvider">
        /// The structured command provider.
        /// </param>
        public LayoutPageRepository(
            [NotNull] IRepositoryProvider repositoryProvider,
            [NotNull] ILayoutPageSectionService layoutPageSectionService,
            [NotNull] IStructuredCommandProvider structuredCommandProvider)
            : base(repositoryProvider)
        {
            if (repositoryProvider == null)
            {
                throw new ArgumentNullException(nameof(repositoryProvider));
            }

            if (layoutPageSectionService == null)
            {
                throw new ArgumentNullException(nameof(layoutPageSectionService));
            }

            if (structuredCommandProvider == null)
            {
                throw new ArgumentNullException(nameof(structuredCommandProvider));
            }

            this.layoutPageSectionService = layoutPageSectionService;
            this.structuredCommandProvider = structuredCommandProvider;
        }

        /// <summary>
        /// Gets the <see cref="LayoutPage"/> associated with the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">
        /// The ID of the <see cref="LayoutPage"/> to get.
        /// </param>
        /// <returns>
        /// The <see cref="LayoutPage"/> associated with the <paramref name="id"/>, or null if no page is found.
        /// </returns>
        public LayoutPage GetPage(int id)
        {
            return this.FirstOrDefaultWithChildren(id);
        }

        /// <summary>
        /// Gets the layout pages for the specified form layout.
        /// </summary>
        /// <param name="layout">
        /// The form layout to get the pages for.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="LayoutPage"/> items associated with the <paramref name="layout"/>.
        /// </returns>
        public IEnumerable<LayoutPage> GetPages([NotNull] FormLayout layout)
        {
            if (layout == null)
            {
                throw new ArgumentNullException(nameof(layout));
            }

            var itemSelection = Query.From<LayoutPageRow>().Matching(row => row.FormLayoutId, layout.FormLayoutId.GetValueOrDefault());
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
        protected override ItemSelection<LayoutPageRow> GetUniqueItemSelection(LayoutPageRow item)
        {
            return this.GetKeySelection(item, row => row.LayoutPageId);
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
        protected override void LoadChildren([NotNull] LayoutPage entity, [NotNull] IRepositoryProvider provider)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            provider.DependencyContainer.SetDependency(entity.LayoutPageId, entity);
            entity.Load(this.layoutPageSectionService);
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
        protected override void SaveChildren([NotNull] LayoutPage entity, [NotNull] IRepositoryProvider provider)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            Singleton<LayoutPageValidator>.Instance.ThrowOnValidationFailure(entity);

            var transaction = provider.StartTransaction();

            var tableLoader = Singleton<DataTableLoader<LayoutPageSectionRow>>.Instance;
            var dataTable = tableLoader.Load(entity.LayoutPageSections, provider.EntityMapper);

            var mergeCommand =
                new StructuredMergeCommand<LayoutPageSectionRow>(this.structuredCommandProvider, transaction)
                    .MergeInto<LayoutPageSectionRow>(row => row.LayoutPageId, row => row.Order)
                    .DeleteUnmatchedInSource(row => row.LayoutPageId)
                    .SelectFromInserted(row => row.LayoutPageId, row => row.Order);

            using (var reader = mergeCommand.ExecuteReader(dataTable))
            {
                var orderOrdinal = reader.GetOrdinal(nameof(LayoutPageSectionRow.Order));
                var layoutPageSectionIdOrdinal = reader.GetOrdinal(nameof(LayoutPageSectionRow.LayoutPageSectionId));

                while (reader.Read())
                {
                    // Note - although we could use reader.GetInt32(), etc., the StructuredCommandProvider.StubForList() method does 
                    // does not stub those calls. 
                    var values = new object[reader.FieldCount];
                    reader.GetValues(values);

                    // Another side-effect of the stub is that it will not get the updated parent ID. So we use the already available
                    // entity ID instead.
                    var layoutPageId = entity.LayoutPageId;

                    var order = (short)values[orderOrdinal];
                    var layoutPageSectionId = (int)values[layoutPageSectionIdOrdinal];

                    var layoutPageSection = entity.LayoutPageSections.First(x => x.LayoutPageId == layoutPageId && x.Order == order);
                    this.EntityMapper.MapTo(layoutPageSectionId, layoutPageSection);
                }
            }

            provider.CompleteTransaction();
        }
    }
}