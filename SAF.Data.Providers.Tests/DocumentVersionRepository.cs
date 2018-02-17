// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentVersionRepository.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using Startitecture.Orm.Common;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Sql;

    /// <summary>
    /// The document version repository.
    /// </summary>
    public class DocumentVersionRepository : EntityRepository<DocumentVersion, DocumentVersionRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentVersionRepository"/> class.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider.
        /// </param>
        public DocumentVersionRepository(IRepositoryProvider repositoryProvider)
            : base(repositoryProvider)
        {
        }

        /// <summary>
        /// Gets the versions for the specified document.
        /// </summary>
        /// <param name="document">
        /// The document to retrieve the versions for.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of document versions for the specified <paramref name="document"/>.
        /// </returns>
        public IEnumerable<DocumentVersion> GetVersions([NotNull] Document document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            this.RepositoryProvider.DependencyContainer.SetDependency(document.DocumentId, document);
            var itemSelection = Query.From<DocumentVersionRow>().Matching(row => row.DocumentId, document.DocumentId.GetValueOrDefault());
            return this.SelectEntities(itemSelection);
        }

        /// <summary>
        /// Selects the document versions for the specified document ID.
        /// </summary>
        /// <param name="documentId">
        /// The document ID of the document to get the versions for.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="DocumentVersion"/> items for the specified
        /// <paramref name="documentId"/>.
        /// </returns>
        public IEnumerable<DocumentVersion> SelectDocumentVersions(int documentId)
        {
            return this.SelectEntities(Query.From<DocumentVersionRow>().Matching(row => row.DocumentId, documentId));
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
        protected override ItemSelection<DocumentVersionRow> GetUniqueItemSelection(DocumentVersionRow item)
        {
            return this.GetKeySelection(item, row => row.DocumentVersionId, row => row.DocumentId, row => row.VersionNumber);
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
        protected override void SaveDependencies(DocumentVersion entity, IRepositoryProvider provider, DocumentVersionRow dataItem)
        {
            var documentRepo = new DocumentRepository(provider);
            documentRepo.Save(entity.Document);
        }
    }
}