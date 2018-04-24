// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormVersionRepository.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The form version repository.
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
    /// The form version repository.
    /// </summary>
    public class FormVersionRepository : EntityRepository<FormVersion, FormVersionRow>, IFormVersionRepository
    {
        /// <summary>
        /// The layout service.
        /// </summary>
        private readonly IFormLayoutService layoutService;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormVersionRepository"/> class.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider.
        /// </param>
        /// <param name="layoutService">
        /// The layout service.
        /// </param>
        public FormVersionRepository([NotNull] IRepositoryProvider repositoryProvider, [NotNull] IFormLayoutService layoutService)
            : base(repositoryProvider, version => version.FormVersionId)
        {
            if (repositoryProvider == null)
            {
                throw new ArgumentNullException(nameof(repositoryProvider));
            }

            if (layoutService == null)
            {
                throw new ArgumentNullException(nameof(layoutService));
            }

            this.layoutService = layoutService;
        }

        /// <summary>
        /// Gets the versions for the specified form.
        /// </summary>
        /// <param name="form">
        /// The form to get the versions for.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="FormVersion"/> items.
        /// </returns>
        public IEnumerable<FormVersion> GetVersions([NotNull] Form form)
        {
            if (form == null)
            {
                throw new ArgumentNullException(nameof(form));
            }

            return this.SelectEntities(this.Select<FormVersion, FormVersionRow>().WhereEqual(row => row.FormId, form.FormId.GetValueOrDefault()));
        }

        /// <summary>
        /// Gets the form version with the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">
        /// The ID of the form version to get.
        /// </param>
        /// <returns>
        /// The <see cref="FormVersion"/> with the specified <paramref name="id"/>, or null if no version is found.
        /// </returns>
        public FormVersion GetVersion(int id)
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
        protected override ItemSelection<FormVersionRow> GetUniqueItemSelection(FormVersionRow item)
        {
            return this.GetKeySelection(item, row => row.FormVersionId);
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
        protected override void LoadChildren([NotNull] FormVersion entity, [NotNull] IRepositoryProvider provider)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            provider.DependencyContainer.SetDependency(entity.FormVersionId, entity);
            entity.Load(this.layoutService);
        }
    }
}