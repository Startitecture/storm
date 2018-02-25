// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttachmentDocumentRepository.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The attachment document repository.
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
    using Startitecture.Orm.Testing.Model.DocumentEntities;

    /// <summary>
    /// The attachment document repository.
    /// </summary>
    public class AttachmentDocumentRepository : EntityRepository<AttachmentDocument, AttachmentDocumentRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttachmentDocumentRepository"/> class.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider.
        /// </param>
        public AttachmentDocumentRepository(IRepositoryProvider repositoryProvider)
            : base(repositoryProvider)
        {
        }

        /// <summary>
        /// Queries attachment documents.
        /// </summary>
        /// <param name="selection">
        /// The selection to make.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="Startitecture.Orm.Testing.Model.AttachmentDocument"/> items matching the <paramref name="selection"/>.
        /// </returns>
        public IEnumerable<AttachmentDocument> QueryAttachmentDocuments([NotNull] ItemSelection<AttachmentDocumentRow> selection)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            return this.SelectEntities(selection);
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
        protected override ItemSelection<AttachmentDocumentRow> GetUniqueItemSelection(AttachmentDocumentRow item)
        {
            return this.GetKeySelection(item, row => row.AttachmentDocumentId);
        }

        /// <summary>
        /// Saves the dependencies of the specified entity prior to saving the entity itself.
        /// </summary>
        /// <param name="entity">
        /// The entity to save.
        /// </param>
        /// <param name="provider">
        /// The repository provider for the current operation.
        /// </param>
        /// <param name="dataItem">
        /// The data item mapped from the entity.
        /// </param>
        /// <remarks>
        /// Use repositories with the entity to save dependencies and apply the results to the <paramref name="dataItem"/>.
        /// </remarks>
        protected override void SaveDependencies(AttachmentDocument entity, IRepositoryProvider provider, AttachmentDocumentRow dataItem)
        {
            var documentVersionRepo = new DocumentVersionRepository(provider);
            documentVersionRepo.Save(entity.DocumentVersion);

            var attachmentRepo = new AttachmentRepository(provider);
            attachmentRepo.Save(entity);
        }
    }
}