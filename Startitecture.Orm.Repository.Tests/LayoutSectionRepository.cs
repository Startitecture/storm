// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutSectionRepository.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The layout section repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository.Tests
{
    using System;
    using System.Linq;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Sql;
    using Startitecture.Orm.Testing.Model;
    using Startitecture.Orm.Testing.Model.FieldEntities;

    /// <summary>
    /// The layout section repository.
    /// </summary>
    public class LayoutSectionRepository : EntityRepository<LayoutSection, LayoutSectionRow>, ILayoutSectionRepository
    {
        /// <summary>
        /// The structured command provider.
        /// </summary>
        private readonly IStructuredCommandProvider structuredCommandProvider;

        /// <summary>
        /// The placement service.
        /// </summary>
        private readonly IFieldPlacementService placementService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutSectionRepository"/> class.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider.
        /// </param>
        /// <param name="entityMapper">
        /// The entity mapper.
        /// </param>
        /// <param name="structuredCommandProvider">
        /// The structured command provider.
        /// </param>
        /// <param name="placementService">
        /// The placement service.
        /// </param>
        public LayoutSectionRepository(
            [NotNull] IRepositoryProvider repositoryProvider,
            [NotNull] IEntityMapper entityMapper,
            [NotNull] IStructuredCommandProvider structuredCommandProvider,
            [NotNull] IFieldPlacementService placementService)
            : base(repositoryProvider, entityMapper, section => section.LayoutSectionId)
        {
            if (repositoryProvider == null)
            {
                throw new ArgumentNullException(nameof(repositoryProvider));
            }

            if (entityMapper == null)
            {
                throw new ArgumentNullException(nameof(entityMapper));
            }

            if (structuredCommandProvider == null)
            {
                throw new ArgumentNullException(nameof(structuredCommandProvider));
            }

            if (placementService == null)
            {
                throw new ArgumentNullException(nameof(placementService));
            }

            this.structuredCommandProvider = structuredCommandProvider;
            this.placementService = placementService;
        }

        /// <summary>
        /// Gets the layout section with the specified ID along with its field placements.
        /// </summary>
        /// <param name="id">
        /// The ID of the layout section to get.
        /// </param>
        /// <returns>
        /// The <see cref="LayoutSection"/> associated with the <paramref name="id"/>, or null if no section is found.
        /// </returns>
        public LayoutSection GetSection(int id)
        {
            return this.FirstOrDefaultWithChildren(id);
        }

        /// <summary>
        /// Gets the layout section with the specified identifier along with its field placements.
        /// </summary>
        /// <param name="identifier">
        /// The identifier of the layout section to get.
        /// </param>
        /// <returns>
        /// The <see cref="LayoutSection"/> associated with the <paramref name="identifier"/>, or null if no section is found.
        /// </returns>
        public LayoutSection GetSection(Guid identifier)
        {
            return this.FirstOrDefaultWithChildren(identifier);
        }

        /// <summary>
        /// Saves a <see cref="LayoutSection"/> to a repository along with field placements.
        /// </summary>
        /// <param name="section">
        /// The layout section to save.
        /// </param>
        /// <returns>
        /// The saved <see cref="LayoutSection"/>.
        /// </returns>
        public LayoutSection SaveWithPlacements([NotNull] LayoutSection section)
        {
            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            return this.SaveWithChildren(section);
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
        protected override ItemSelection<LayoutSectionRow> GetUniqueItemSelection(LayoutSectionRow item)
        {
            return this.GetKeySelection(item, row => row.LayoutSectionId);
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
        protected override void LoadChildren([NotNull] LayoutSection entity, [NotNull] IRepositoryProvider provider)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            var key = entity.LayoutSectionId.GetValueOrDefault();
            provider.DependencyContainer.SetDependency(key, entity);
            entity.Load(this.placementService);
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
        protected override void SaveChildren([NotNull] LayoutSection entity, [NotNull] IRepositoryProvider provider)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            this.SaveFieldPlacements(entity, provider);
        }

        /// <summary>
        /// The save field placements.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        private void SaveFieldPlacements(LayoutSection entity, IRepositoryProvider provider)
        {
            var transaction = provider.StartTransaction();

            var fieldPlacementTableLoader = Singleton<DataTableLoader<FieldPlacementTableType>>.Instance;
            var fieldPlacementDataTable = fieldPlacementTableLoader.Load(entity.FieldPlacements, this.EntityMapper);

            var mergeFieldPlacementCommand =
                new StructuredMergeCommand<FieldPlacementTableType>(this.structuredCommandProvider, transaction)
                    .MergeInto<FieldPlacementRow>(x => x.LayoutSectionId, row => row.Order)
                    .DeleteUnmatchedInSource(type => type.LayoutSectionId)
                    .SelectFromInserted(type => type.LayoutSectionId, type => type.Order);

            using (var dataReader = mergeFieldPlacementCommand.ExecuteReader(fieldPlacementDataTable))
            {
                var layoutSectionIdOrdinal = dataReader.GetOrdinal(nameof(FieldPlacementTableType.LayoutSectionId));
                var orderOrdinal = dataReader.GetOrdinal(nameof(FieldPlacementTableType.Order));
                var fieldPlacementIdOrdinal = dataReader.GetOrdinal(nameof(FieldPlacementTableType.FieldPlacementId));

                while (dataReader.Read())
                {
                    var values = new object[dataReader.FieldCount];
                    dataReader.GetValues(values);

                    // We need this ID to associate the unsaved values with the saved ones.
                    var layoutSectionId = values[layoutSectionIdOrdinal] as int?;
                    var order = values[orderOrdinal] as short?;

                    // We'll apply this ID to our existing values.
                    var fieldPlacementId = values[fieldPlacementIdOrdinal] as int?;

                    // Find the entity and map to that. Mapping to the row will not work because the ID is ignored by default.
                    var fieldPlacement = entity.FieldPlacements.First(x => x.LayoutSectionId == layoutSectionId && x.Order == order);
                    this.EntityMapper.MapTo(fieldPlacementId, fieldPlacement);
                }
            }

            provider.CompleteTransaction();
        }
    }
}