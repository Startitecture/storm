// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PetaPocoRepositoryProviderTests.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SAF.Data.Providers.Tests.PM;
    using SAF.Testing.Common;

    using Startitecture.Orm.Common;
    using Startitecture.Orm.Mapper;
    using Startitecture.Orm.Sql;
    using Startitecture.Repository.Mapping;

    /// <summary>
    /// The peta poco repository provider tests.
    /// </summary>
    [TestClass]
    public class PetaPocoRepositoryProviderTests
    {
        /// <summary>
        /// The generator.
        /// </summary>
        private static readonly Random Generator = new Random();

        /// <summary>
        /// The entity mapper.
        /// </summary>
        private readonly AutoMapperEntityMapper entityMapper = new AutoMapperEntityMapper();

        /// <summary>
        /// The save test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void Save_NewAttachment_NoExceptionThrown()
        {
            using (var target = new DatabaseRepositoryProvider<TestDb>(this.entityMapper))
            {
                target.ChangeDatabase("DEVTEST01");

                target.StartTransaction();

                try
                {
                    var item = new AttachmentRow
                    {
                        Subject = "UNIT_TEST:MyTestAttachment",
                        CreatedBy = "Stan",
                        CreatedTime = DateTimeOffset.Now,
                        LastModifiedBy = "Stan",
                        LastModifiedTime = DateTimeOffset.Now,
                        SortOrder = 1,
                        DocumentTypeId = 1
                    };

                    target.Save(item, item.ToExampleSelection(row => row.AttachmentId));
                }
                finally
                {
                    target.AbortTransaction();
                }
            }
        }

        /// <summary>
        /// The get selection test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void GetSelection_ExistingAttachments_DoesNotThrowException()
        {
            using (var target = new DatabaseRepositoryProvider<TestDb>(this.entityMapper))
            {
                target.ChangeDatabase("DEVTEST01");

                var attachments = new List<AttachmentRow>();

                try
                {
                    var document1 = new DocumentRow { Identifier = $"UNIT_TEST:MyDoc-{Generator.NextDouble()}" };
                    var document2 = new DocumentRow { Identifier = $"UNIT_TEST:MyOtherDoc-{Generator.NextDouble()}" };

                    target.Save(document1, document1.ToExampleSelection(row => row.DocumentId));
                    target.Save(document2, document2.ToExampleSelection(row => row.DocumentId));

                    // Save some stuff.
                    for (int i = 0; i < 10; i++)
                    {
                        var contractType = new DocumentTypeRow { DocumentTypeId = 1, IsSystemType = true, Name = "Contract" };
                        var item = new AttachmentRow
                                       {
                                           Subject = $"UNIT_TEST:MyTestAttachment{i}",
                                           CreatedBy = "Stan",
                                           CreatedTime = DateTimeOffset.Now,
                                           LastModifiedBy = "Stan",
                                           LastModifiedTime = DateTimeOffset.Now,
                                           SortOrder = i + 1,
                                           DocumentTypeId = 1,
                                           DocumentType = contractType
                                       };

                        target.Save(item, item.ToExampleSelection(row => row.AttachmentId));

                        switch (i % 2)
                        {
                            case 0:
                                var currentDocument = i > 5 ? document2 : document1;
                                var documentVersion = new DocumentVersionRow
                                                          {
                                                              DocumentId = currentDocument.DocumentId,
                                                              Name = $"{currentDocument.Identifier}-{i}.docx",
                                                              VersionNumber = i,
                                                              Document = currentDocument
                                                          };

                                target.Save(documentVersion, documentVersion.ToExampleSelection(row => row.DocumentId, row => row.VersionNumber));

                                var attachmentDocument = new AttachmentDocumentRow
                                                         {
                                                             AttachmentDocumentId = item.AttachmentId,
                                                             DocumentVersionId = documentVersion.DocumentVersionId,
                                                             DocumentVersion = documentVersion
                                                         };

                                target.Save(attachmentDocument, attachmentDocument.ToExampleSelection(row => row.AttachmentDocumentId));

                                break;

                            default:
                                var attachmentNote = new AttachmentNoteRow
                                                         {
                                                             AttachmentNoteId = item.AttachmentId,
                                                             Content = $"UNIT_TEST:blah blah blah for {i}"
                                                         };

                                target.Save(attachmentNote, attachmentNote.ToExampleSelection(row => row.AttachmentNoteId));

                                break;
                        }

                        attachments.Add(item);
                    }

                    var transactSqlSelection = Query.From<AttachmentRow>().Matching(row => row.Subject, "UNIT_TEST:MyTestAttachment%");
                    transactSqlSelection.Limit = 10;
                    var actual = target.GetSelection(transactSqlSelection).ToList();
                    Assert.AreEqual(10, actual.Count);
                    CollectionAssert.AreEquivalent(attachments, actual);
                }
                finally
                {
                    DeleteUnitTestItems(target);
                }
            }
        }

        /// <summary>
        /// The get first or default test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void GetFirstOrDefault_ExistingAttachment_MatchesExpected()
        {
            var contractType = new DocumentTypeRow { DocumentTypeId = 1, IsSystemType = true, Name = "Contract" };
            var expected = new AttachmentRow
                               {
                                   Subject = $"UNIT_TEST:{Generator.NextDouble()}",
                                   CreatedBy = "Stan",
                                   CreatedTime = DateTimeOffset.Now,
                                   LastModifiedBy = "Stan",
                                   LastModifiedTime = DateTimeOffset.Now,
                                   SortOrder = 1,
                                   DocumentTypeId = 1,
                                   DocumentType = contractType
                               };

            using (var provider = new DatabaseRepositoryProvider<TestDb>(this.entityMapper))
            {
                provider.ChangeDatabase("DEVTEST01");
                provider.Save(expected, expected.ToExampleSelection(row => row.AttachmentId));
            }

            using (var target = new DatabaseRepositoryProvider<TestDb>(this.entityMapper))
            {
                try
                {
                    target.ChangeDatabase("DEVTEST01");
                    var actual = target.GetFirstOrDefault(Query.From<AttachmentRow>().Matching(row => row.Subject, expected.Subject));
                    Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
                }
                finally
                {
                    DeleteUnitTestItems(target);    
                }       
            }
        }

        /// <summary>
        /// The get first or default test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void GetFirstOrDefault_ExistingWorkflowPhase_ExpectedPropertiesAreNull()
        {
            using (var target = new DatabaseRepositoryProvider<TestDb>(this.entityMapper))
            {
                target.ChangeDatabase("DEVTEST01");

                var stopwatch = Stopwatch.StartNew();
                var actual = target.GetFirstOrDefault(Query.From<WorkflowPhaseRow>().Matching(row => row.WorkflowPhaseId, 269));
                Trace.TraceInformation($"First run: {stopwatch.Elapsed}");

                Assert.IsNull(actual.ProcessPhase?.PhaseActionDeadline);
                Assert.IsNull(actual.ProcessPhase?.SignatureOption);
                Assert.IsNull(actual.WorkflowSignatureOption);

                stopwatch.Restart();
                target.GetFirstOrDefault(Query.From<WorkflowPhaseRow>().Matching(row => row.WorkflowPhaseId, 214));
                Trace.TraceInformation($"Second run (different ID): {stopwatch.Elapsed}");

                stopwatch.Restart();
                target.GetFirstOrDefault(Query.From<WorkflowPhaseRow>().Matching(row => row.WorkflowPhaseId, 269));
                Trace.TraceInformation($"Third run (same ID): {stopwatch.Elapsed}");
            }
        }

        /// <summary>
        /// The get first or default test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void GetFirstOrDefault_NonExistingAttachment_ReturnsNull()
        {
            using (var target = new DatabaseRepositoryProvider<TestDb>(this.entityMapper))
            {
                target.ChangeDatabase("DEVTEST01");
                var itemSelection = Query.From<AttachmentRow>().Matching(row => row.Subject, $"UNIT_TEST:{Generator.NextDouble()}");
                var actual = target.GetFirstOrDefault(itemSelection);
                Assert.AreEqual(null, actual);
            }
        }

        /// <summary>
        /// The contains test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void Contains_ExistingAttachment_ReturnsTrue()
        {
            var contractType = new DocumentTypeRow { DocumentTypeId = 1, IsSystemType = true, Name = "Contract" };
            var expected = new AttachmentRow
            {
                Subject = $"UNIT_TEST:{Generator.NextDouble()}",
                CreatedBy = "Stan",
                CreatedTime = DateTimeOffset.Now,
                LastModifiedBy = "Stan",
                LastModifiedTime = DateTimeOffset.Now,
                SortOrder = 1,
                DocumentTypeId = 1,
                DocumentType = contractType
            };

            using (var provider = new DatabaseRepositoryProvider<TestDb>(this.entityMapper))
            {
                provider.ChangeDatabase("DEVTEST01");
                provider.Save(expected, expected.ToExampleSelection(row => row.AttachmentId));
            }

            using (var target = new DatabaseRepositoryProvider<TestDb>(this.entityMapper))
            {
                try
                {
                    target.ChangeDatabase("DEVTEST01");
                    var actual = target.Contains(Query.From<AttachmentRow>().Matching(row => row.Subject, expected.Subject));
                    Assert.IsTrue(actual);
                }
                finally
                {
                    DeleteUnitTestItems(target);
                }
            }
        }

        /// <summary>
        /// The contains test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void Contains_NonExistingAttachment_ReturnsFalse()
        {
            using (var target = new DatabaseRepositoryProvider<TestDb>(this.entityMapper))
            {
                target.ChangeDatabase("DEVTEST01");
                var itemSelection = Query.From<AttachmentRow>().Matching(row => row.Subject, $"UNIT_TEST:{Generator.NextDouble()}");
                var actual = target.Contains(itemSelection);
                Assert.AreEqual(false, actual);
            }
        }

        /// <summary>
        /// The delete items test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void DeleteItems_ExistingAttachment_ItemDeleted()
        {
            var contractType = new DocumentTypeRow { DocumentTypeId = 1, IsSystemType = true, Name = "Contract" };
            var expected = new AttachmentRow
                               {
                                   Subject = $"UNIT_TEST:{Generator.NextDouble()}",
                                   CreatedBy = "Stan",
                                   CreatedTime = DateTimeOffset.Now,
                                   LastModifiedBy = "Stan",
                                   LastModifiedTime = DateTimeOffset.Now,
                                   SortOrder = 1,
                                   DocumentTypeId = 1,
                                   DocumentType = contractType
                               };

            using (var target = new DatabaseRepositoryProvider<TestDb>(this.entityMapper))
            {
                try
                {
                    target.ChangeDatabase("DEVTEST01");
                    target.Save(expected, expected.ToExampleSelection(row => row.AttachmentId));
                    var itemSelection = Query.From<AttachmentRow>().Matching(row => row.Subject, expected.Subject);
                    var result = target.DeleteItems(itemSelection);

                    Assert.AreEqual(1, result);

                    var actual = target.GetFirstOrDefault(itemSelection);
                    Assert.AreEqual(null, actual);
                }
                finally
                {
                    DeleteUnitTestItems(target);
                }
            }
        }

        /// <summary>
        /// The insert item test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void InsertItem_AttachmentRow_MatchesExpected()
        {
            var contractType = new DocumentTypeRow { DocumentTypeId = 1, IsSystemType = true, Name = "Contract" };
            var expected = new AttachmentRow
                               {
                                   Subject = $"UNIT_TEST:{Generator.NextDouble()}",
                                   CreatedBy = "Stan",
                                   CreatedTime = DateTimeOffset.Now,
                                   LastModifiedBy = "Stan",
                                   LastModifiedTime = DateTimeOffset.Now,
                                   SortOrder = 1,
                                   DocumentTypeId = 1,
                                   DocumentType = contractType
                               };

            using (var target = new DatabaseRepositoryProvider<TestDb>(this.entityMapper))
            {
                try
                {
                    target.ChangeDatabase("DEVTEST01");
                    var actual = target.InsertItem(expected);
                    Assert.AreSame(expected, actual);
                    Assert.IsTrue(actual.AttachmentId > 0);
                }
                finally
                {
                    DeleteUnitTestItems(target);
                }
            }
        }

        /// <summary>
        /// The update test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void Update_ExistingAttachmentRow_MatchesExpected()
        {
            var contractType = new DocumentTypeRow { DocumentTypeId = 1, IsSystemType = true, Name = "Contract" };
            var expected = new AttachmentRow
            {
                Subject = $"UNIT_TEST:{Generator.NextDouble()}",
                CreatedBy = "Stan",
                CreatedTime = DateTimeOffset.Now,
                LastModifiedBy = "Stan",
                LastModifiedTime = DateTimeOffset.Now,
                SortOrder = 1,
                DocumentTypeId = 1,
                DocumentType = contractType
            };

            using (var target = new DatabaseRepositoryProvider<TestDb>(this.entityMapper))
            {
                try
                {
                    target.ChangeDatabase("DEVTEST01");
                    var inserted = target.InsertItem(expected);
                    expected.SortOrder = 2;

                    var itemSelection = Query.From<AttachmentRow>().Matching(row => row.AttachmentId, inserted.AttachmentId);
                    var result = target.Update(expected, itemSelection);
                    Assert.AreEqual(1, result);

                    var actual = target.GetFirstOrDefault(itemSelection);
                    Assert.AreEqual(expected, actual);
                }
                finally
                {
                    DeleteUnitTestItems(target);
                }
            }
        }

        //// TODO: Need to make an operation for us to execute.
        /////// <summary>
        /////// The execute test.
        /////// </summary>
        ////[TestMethod]
        ////[TestCategory("Integration")]
        ////public void ExecuteTest()
        ////{
        ////    Assert.Fail();
        ////}

        /// <summary>
        /// Deletes unit test items.
        /// </summary>
        /// <param name="provider">
        /// The provider to use to delete the items.
        /// </param>
        private static void DeleteUnitTestItems(IRepositoryProvider provider)
        {
            // Delete the attachment documents based on finding their versions.
            var versionSelection = Query.From<DocumentVersionRow>(row => row.DocumentVersionId).Matching(row => row.Name, "UNIT_TEST:%");
            var docVersionIds = provider.GetSelection(versionSelection).Select(x => x.DocumentVersionId);
            provider.DeleteItems(Query.From<AttachmentDocumentRow>().Include(row => row.DocumentVersionId, docVersionIds.ToArray()));

            // Delete the rest using a filter.
            provider.DeleteItems(Query.From<AttachmentNoteRow>().Matching(row => row.Content, "UNIT_TEST:%"));
            provider.DeleteItems(Query.From<AttachmentRow>().Matching(row => row.Subject, "UNIT_TEST:MyTestAttachment%"));
            provider.DeleteItems(Query.From<DocumentVersionRow>().Matching(row => row.Name, "UNIT_TEST:%"));
            provider.DeleteItems(Query.From<DocumentRow>().Matching(row => row.Identifier, "UNIT_TEST:%"));
        }
    }
}