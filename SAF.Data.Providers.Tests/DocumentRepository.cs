// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentRepository.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    /// <summary>
    /// The document repository.
    /// </summary>
    public class DocumentRepository : EntityRepository<Document, DocumentRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentRepository"/> class.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider.
        /// </param>
        public DocumentRepository(IRepositoryProvider repositoryProvider)
            : base(repositoryProvider)
        {
        }

        /// <summary>
        /// Gets all the versions for the specified document.
        /// </summary>
        /// <param name="document">
        /// The document to load all versions for.
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

            var documentVersionRepository = new DocumentVersionRepository(this.RepositoryProvider);
            var example = new DocumentVersionRow { DocumentId = document.DocumentId.GetValueOrDefault() };
            var exampleQuery = new ExampleQuery<DocumentVersionRow>(example, row => row.DocumentId);
            return documentVersionRepository.SelectEntities(exampleQuery);
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
        protected override ItemSelection<DocumentRow> GetUniqueItemSelection(DocumentRow item)
        {
            return this.GetKeySelection(item, row => row.DocumentId, row => row.Identifier);
        }
    }
}