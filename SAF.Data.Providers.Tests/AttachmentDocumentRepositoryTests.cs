// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttachmentDocumentRepositoryTests.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The attachment document repository tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SAF.Mock;
    using SAF.Testing.Common;

    using Startitecture.Orm.Common;

    /// <summary>
    /// The attachment document repository tests.
    /// </summary>
    [TestClass]
    public class AttachmentDocumentRepositoryTests
    {
        /// <summary>
        /// The number generator.
        /// </summary>
        private static readonly Random NumberGenerator = new Random();

        /// <summary>
        /// The entity mapper.
        /// </summary>
        private readonly IEntityMapper entityMapper = RepositoryMockFactory.CreateEntityMapper(
            expression =>
            {
                expression.AddProfile<AttachmentMappingProfile>();
                expression.AddProfile<AttachmentDocumentMappingProfile>();
                expression.AddProfile<DocumentTypeMappingProfile>();
                expression.AddProfile<DocumentVersionMappingProfile>();
                expression.AddProfile<DocumentMappingProfile>();
            });

        /// <summary>
        /// The attachment document repository test.
        /// </summary>
        [TestMethod]
        public void Save_NewAttachmentDocument_MatchesExpected()
        {
            var document = new Document("workflow-123");
            var documentVersion = document.Revise("workflow-123.docx");
            var documentType = new DocumentType(true, 1) { Name = "Contract" };
            var attachmentDocument = new AttachmentDocument("MySubject", documentVersion, documentType);
            attachmentDocument.ChangeSortOrder(1);

            var adapter = RepositoryMockFactory.CreateAdapter();
            adapter.StubForExistingItem<DocumentTypeRow>(documentType, this.entityMapper);
            adapter.StubForNewItem<AttachmentRow>();
            adapter.StubForNewItem<AttachmentDocumentRow>();
            adapter.StubForNewItem<DocumentVersionRow>();
            adapter.StubForNewItem<DocumentRow>();

            using (var provider = RepositoryMockFactory.CreateConcreteProvider<TestDb>(this.entityMapper, adapter))
            {
                var expectedRow = this.entityMapper.Map<AttachmentDocumentRow>(attachmentDocument);
                expectedRow.SetTransactionProvider(provider);
                var expected = this.entityMapper.Map<AttachmentDocument>(expectedRow);

                var target = new AttachmentDocumentRepository(provider);
                var actual = target.Save(attachmentDocument);

                Assert.AreEqual(1, actual.DocumentType.DocumentTypeId);
                Assert.IsTrue(actual.AttachmentDocumentId > 0);
                Assert.IsTrue(actual.DocumentVersionId > 0);
                Assert.IsTrue(actual.AttachmentId > 0);
                Assert.IsTrue(actual.DocumentVersion.Document.DocumentId > 0);
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        /// The attachment document repository test.
        /// </summary>
        [TestMethod]
        public void Save_ExistingAttachmentDocument_MatchesExpected()
        {
            var document = new Document("workflow-123", 342);
            var documentVersion = new DocumentVersion(document, 1, 534) { Name = "workflow-123.docx" };
            var documentType = new DocumentType(true, 1) { Name = "Contract" };
            var attachmentDocument = new AttachmentDocument("MySubject", documentVersion, documentType, 3749);

            var adapter = RepositoryMockFactory.CreateAdapter();
            var attachmentDocumentRow = this.entityMapper.Map<AttachmentDocumentRow>(attachmentDocument);
            adapter.StubForExistingItem(attachmentDocumentRow);

            adapter.StubForExistingItem<AttachmentRow>(attachmentDocument, this.entityMapper);
            adapter.StubForExistingItem(attachmentDocumentRow.DocumentType);
            adapter.StubForExistingItem(attachmentDocumentRow.DocumentVersion);
            adapter.StubForExistingItem(attachmentDocumentRow.DocumentVersion.Document);

            using (var provider = RepositoryMockFactory.CreateConcreteProvider<TestDb>(this.entityMapper, adapter))
            {
                attachmentDocument.ChangeSortOrder(2);

                var expectedRow = this.entityMapper.Map<AttachmentDocumentRow>(attachmentDocument);
                expectedRow.SetTransactionProvider(provider);
                var expected = this.entityMapper.Map<AttachmentDocument>(expectedRow);

                var target = new AttachmentDocumentRepository(provider);
                var actual = target.Save(attachmentDocument);

                Assert.AreEqual(1, actual.DocumentType.DocumentTypeId);
                Assert.IsTrue(actual.AttachmentDocumentId > 0);
                Assert.IsTrue(actual.DocumentVersionId > 0);
                Assert.IsTrue(actual.AttachmentId > 0);
                Assert.IsTrue(actual.DocumentVersion.Document.DocumentId > 0);
                Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
            }
        }

        /// <summary>
        /// The attachment document repository test.
        /// </summary>
        [TestMethod]
        public void FirstOrDefault_ExistingAttachmentDocument_MatchesExpected()
        {
            var document = new Document("workflow-123", 342);
            var documentVersion = new DocumentVersion(document, 1, 534) { Name = "workflow-123.docx" };
            var documentType = new DocumentType(true, 1) { Name = "Contract" };
            var expected = new AttachmentDocument("MySubject", documentVersion, documentType, 3749);

            var adapter = RepositoryMockFactory.CreateAdapter();
            var attachmentDocumentRow = this.entityMapper.Map<AttachmentDocumentRow>(expected);
            adapter.StubForExistingItem(attachmentDocumentRow);

            using (var provider = RepositoryMockFactory.CreateConcreteProvider<TestDb>(this.entityMapper, adapter))
            {
                var target = new AttachmentDocumentRepository(provider);
                var actual = target.FirstOrDefault(expected.AttachmentDocumentId);

                Assert.AreEqual(1, actual.DocumentType.DocumentTypeId);
                Assert.IsTrue(actual.AttachmentDocumentId > 0);
                Assert.IsTrue(actual.DocumentVersionId > 0);
                Assert.IsTrue(actual.AttachmentId > 0);
                Assert.IsTrue(actual.DocumentVersion.Document.DocumentId > 0);
                Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
            }
        }

        /// <summary>
        /// The attachment document repository test.
        /// </summary>
        [TestMethod]
        public void SelectEntities_ExistingAttachmentDocuments_MatchesExpected()
        {
            var contractType = new DocumentType(true, 1) { Name = "Contract" };
            var contract = new Document("workflow-123", 342);
            var version1 = new DocumentVersion(contract, 1, 534) { Name = "workflow-123-1.doc" };
            var version2 = new DocumentVersion(contract, 3, 587) { Name = "workflow-123-3.docx" };
            var version3 = new DocumentVersion(contract, 6, 4352) { Name = "workflow-123-5.pdf" };

            var expected1 = new AttachmentDocument("MySubject", version1, contractType, 3749);
            var expected2 = new AttachmentDocument("My Subject", version2, contractType, 58473);
            var expected3 = new AttachmentDocument("My Final Subject", version3, contractType, 974358);

            var expected = new List<AttachmentDocument> { expected1, expected2, expected3 };

            var adapter = RepositoryMockFactory.CreateAdapter();
            adapter.StubForList(expected, new List<AttachmentDocumentRow>(), this.entityMapper);

            using (var provider = RepositoryMockFactory.CreateConcreteProvider<TestDb>(this.entityMapper, adapter))
            {
                var target = new AttachmentDocumentRepository(provider);
                var actual = target.QueryAttachmentDocuments(Query.From<AttachmentDocumentRow>()).ToList();

                CollectionAssert.AreEqual(expected, actual);

                var actualDocumentType = actual.FirstOrDefault()?.DocumentType;
                Assert.IsNotNull(actualDocumentType);

                var actualDocument = actual.FirstOrDefault()?.DocumentVersion?.Document;
                Assert.IsNotNull(actualDocument);

                foreach (var attachmentDocument in actual)
                {
                    Assert.AreSame(actualDocumentType, attachmentDocument.DocumentType);
                    Assert.AreSame(actualDocument, attachmentDocument.DocumentVersion?.Document);
                }

                foreach (var attachmentDocument in expected)
                {
                    var actualAttachment = actual.FirstOrDefault(x => x.AttachmentId == attachmentDocument.AttachmentId);
                    Assert.IsNotNull(actualAttachment);
                    Assert.AreEqual(attachmentDocument.DocumentVersionId, actualAttachment.DocumentVersionId);
                    Assert.AreEqual(attachmentDocument.DocumentTypeId, actualAttachment.DocumentTypeId);
                    Assert.AreEqual(attachmentDocument.DocumentVersion.DocumentId, actualAttachment.DocumentVersion?.DocumentId);
                }
            }
        }

        /// <summary>
        /// The attachment document repository test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void Save_NewAttachmentDocumentFromDatabase_MatchesExpected()
        {
            var document = new Document("UNIT_TEST.workflow-123");
            var documentVersion = document.Revise("UNIT_TEST.workflow-123.docx");
            var documentType = new DocumentType(true, 1) { Name = "Contract" };
            var attachmentDocument = new AttachmentDocument("UNIT_TEST.MySubject", documentVersion, documentType);
            attachmentDocument.ChangeSortOrder(1);

            try
            {
                using (var provider = new DatabaseRepositoryProvider<TestDb>(this.entityMapper))
                {
                    provider.ChangeDatabase("DEVTEST01");
                    var expectedRow = this.entityMapper.Map<AttachmentDocumentRow>(attachmentDocument);
                    expectedRow.SetTransactionProvider(provider);
                    var expected = this.entityMapper.Map<AttachmentDocument>(expectedRow);

                    var target = new AttachmentDocumentRepository(provider);
                    var actual = target.Save(attachmentDocument);

                    Assert.AreEqual(1, actual.DocumentType.DocumentTypeId);
                    Assert.IsTrue(actual.AttachmentDocumentId > 0);
                    Assert.IsTrue(actual.DocumentVersionId > 0);
                    Assert.IsTrue(actual.AttachmentId > 0);
                    Assert.IsTrue(actual.DocumentVersion.Document.DocumentId > 0);
                    Assert.AreEqual(expected, actual);
                }
            }
            finally
            {
                this.DeleteItems();
            }
        }

        /// <summary>
        /// The attachment document repository test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void Save_ExistingAttachmentDocumentFromDatabase_MatchesExpected()
        {
            var document = new Document($"UNIT_TEST.{NumberGenerator.NextDouble()}");
            var documentVersion = new DocumentVersion(document, 1) { Name = $"{document.Identifier}-1.docx" };
            var documentType = new DocumentType(true, 1) { Name = "Contract" };
            var attachmentDocument = new AttachmentDocument("UNIT_TEST.MySubject", documentVersion, documentType);
            attachmentDocument.ChangeSortOrder(1);

            try
            {
                using (var provider = new DatabaseRepositoryProvider<TestDb>(this.entityMapper))
                {
                    provider.ChangeDatabase("DEVTEST01");
                    var target = new AttachmentDocumentRepository(provider);
                    target.Save(attachmentDocument);
                }

                attachmentDocument.ChangeSortOrder(2);
                attachmentDocument.SetSubject("UNIT_TEST.My New Subject");

                using (var provider = new DatabaseRepositoryProvider<TestDb>(this.entityMapper))
                {
                    provider.ChangeDatabase("DEVTEST01");

                    var expectedRow = this.entityMapper.Map<AttachmentDocumentRow>(attachmentDocument);
                    expectedRow.SetTransactionProvider(provider);
                    var expected = this.entityMapper.Map<AttachmentDocument>(expectedRow);

                    var target = new AttachmentDocumentRepository(provider);
                    var actual = target.Save(attachmentDocument);

                    Assert.AreEqual(1, actual.DocumentType.DocumentTypeId);
                    Assert.IsTrue(actual.AttachmentDocumentId > 0);
                    Assert.IsTrue(actual.DocumentVersionId > 0);
                    Assert.IsTrue(actual.AttachmentId > 0);
                    Assert.IsTrue(actual.DocumentVersion.Document.DocumentId > 0);
                    Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
                }
            }
            finally
            {
                this.DeleteItems();
            }
        }

        /// <summary>
        /// The attachment document repository test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void FirstOrDefault_ExistingAttachmentDocumentFromDatabase_MatchesExpected()
        {
            var document = new Document($"UNIT_TEST.{NumberGenerator.NextDouble()}");
            var documentVersion = new DocumentVersion(document, 1) { Name = $"{document.Identifier}-1.docx" };
            var documentType = new DocumentType(true, 1) { Name = "Contract" };
            var expected = new AttachmentDocument("UNIT_TEST.MySubject", documentVersion, documentType);

            try
            {
                using (var provider = new DatabaseRepositoryProvider<TestDb>(this.entityMapper))
                {
                    provider.ChangeDatabase("DEVTEST01");
                    var target = new AttachmentDocumentRepository(provider);
                    target.Save(expected);
                }

                using (var provider = new DatabaseRepositoryProvider<TestDb>(this.entityMapper))
                {
                    provider.ChangeDatabase("DEVTEST01");
                    var target = new AttachmentDocumentRepository(provider);
                    var actual = target.FirstOrDefault(expected.AttachmentDocumentId);

                    Assert.AreEqual(1, actual.DocumentType.DocumentTypeId);
                    Assert.IsTrue(actual.AttachmentDocumentId > 0);
                    Assert.IsTrue(actual.DocumentVersionId > 0);
                    Assert.IsTrue(actual.AttachmentId > 0);
                    Assert.IsTrue(actual.DocumentVersion.Document.DocumentId > 0);
                    Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
                }
            }
            finally
            {
                this.DeleteItems();
            }
        }

        /// <summary>
        /// The attachment document repository test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void SelectEntities_ExistingAttachmentDocumentsFromDatabase_MatchesExpected()
        {
            var contractType = new DocumentType(true, 1) { Name = "Contract" };
            var contract = new Document($"UNIT_TEST.{NumberGenerator.NextDouble()}");
            var version1 = new DocumentVersion(contract, 1) { Name = $"{contract.Identifier}-1.doc" };
            var version2 = new DocumentVersion(contract, 3) { Name = $"{contract.Identifier}-3.docx" };
            var version3 = new DocumentVersion(contract, 6) { Name = $"{contract.Identifier}-6.pdf" };

            var expected1 = new AttachmentDocument("UNIT_TEST.MySubject", version1, contractType);
            var expected2 = new AttachmentDocument("UNIT_TEST.My Subject", version2, contractType);
            var expected3 = new AttachmentDocument("UNIT_TEST.My Final Subject", version3, contractType);

            var expected = new List<AttachmentDocument> { expected1, expected2, expected3 };

            try
            {
                using (var provider = new DatabaseRepositoryProvider<TestDb>(this.entityMapper))
                {
                    provider.ChangeDatabase("DEVTEST01");
                    var target = new AttachmentDocumentRepository(provider);

                    foreach (var attachmentDocument in expected)
                    {
                        target.Save(attachmentDocument);
                    }
                }

                using (var provider = new DatabaseRepositoryProvider<TestDb>(this.entityMapper))
                {
                    provider.ChangeDatabase("DEVTEST01");
                    var target = new AttachmentDocumentRepository(provider);
                    var actual = target.QueryAttachmentDocuments(Query.From<AttachmentDocumentRow>()).ToList();

                    CollectionAssert.AreEqual(expected, actual);

                    var actualDocumentType = actual.FirstOrDefault()?.DocumentType;
                    Assert.IsNotNull(actualDocumentType);

                    var actualDocument = actual.FirstOrDefault()?.DocumentVersion?.Document;
                    Assert.IsNotNull(actualDocument);

                    foreach (var attachmentDocument in actual)
                    {
                        Assert.AreSame(actualDocumentType, attachmentDocument.DocumentType);
                        Assert.AreSame(actualDocument, attachmentDocument.DocumentVersion?.Document);
                    }

                    foreach (var attachmentDocument in expected)
                    {
                        var actualAttachment = actual.FirstOrDefault(x => x.AttachmentId == attachmentDocument.AttachmentId);
                        Assert.IsNotNull(actualAttachment);
                        Assert.AreEqual(attachmentDocument.DocumentVersionId, actualAttachment.DocumentVersionId);
                        Assert.AreEqual(attachmentDocument.DocumentTypeId, actualAttachment.DocumentTypeId);
                        Assert.AreEqual(attachmentDocument.DocumentVersion.DocumentId, actualAttachment.DocumentVersion?.DocumentId);
                    }
                }
            }
            finally
            {
                this.DeleteItems();
            }
        }

        /// <summary>
        /// The delete items.
        /// </summary>
        private void DeleteItems()
        {
            using (var provider = new DatabaseRepositoryProvider<TestDb>(this.entityMapper))
            {
                provider.ChangeDatabase("DEVTEST01");

                // Delete the attachment documents based on finding their versions.
                var versionSelection = Query.From<DocumentVersionRow>().Matching(row => row.Name, "UNIT_TEST.%");
                var versionRows = provider.GetSelection(versionSelection);
                var docVersionIds = versionRows.Select(x => x.DocumentVersionId);
                provider.DeleteItems(Query.From<AttachmentDocumentRow>().Include(row => row.DocumentVersionId, docVersionIds.ToArray()));

                // Delete the rest using a filter.
                provider.DeleteItems(Query.From<AttachmentNoteRow>().Matching(row => row.Content, "UNIT_TEST.%"));
                provider.DeleteItems(Query.From<AttachmentRow>().Matching(row => row.Subject, "UNIT_TEST.%"));
                provider.DeleteItems(Query.From<DocumentVersionRow>().Matching(row => row.Name, "UNIT_TEST.%"));
                provider.DeleteItems(Query.From<DocumentRow>().Matching(row => row.Identifier, "UNIT_TEST.%"));
            } 
        } 
    }
}
