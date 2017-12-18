// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttachmentDocumentRepository.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
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