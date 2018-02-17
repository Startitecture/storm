// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RaisedPocoFactoryTests.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    using System;
    using System.Diagnostics;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SAF.Testing.Common;

    /// <summary>
    /// The raised poco factory tests.
    /// </summary>
    [TestClass]
    public class RaisedPocoFactoryTests
    {
        /////// <summary>
        /////// The create delegate test.
        /////// </summary>
        ////[TestMethod]
        ////public void CreateDelegate_RaisedPocoFactoryForFlatComplexRow_IsNotNull()
        ////{
        ////    using (var target = new RaisedPocoFactory())
        ////    {
        ////        var fakeComplexRow = Generate.CreateFakeComplexRow();
        ////        var pocoDataRequest = Generate.CreatePocoDataRequest(fakeComplexRow);
        ////        var actual = target.CreateDelegate<FakeComplexRow>(pocoDataRequest);
        ////        Assert.IsNotNull(actual);
        ////    }
        ////}

        /// <summary>
        /// The create delegate test.
        /// </summary>
        [TestMethod]
        public void CreateDelegate_RaisedPocoFactoryForFlatComplexRow_DelegateSetsPocoAsExpected()
        {
            using (var target = new RaisedPocoFactory())
            {
                var expected = Generate.CreateFakeComplexRow();

                var pocoDataRequest = Generate.CreatePocoDataRequest(expected);

                var stopwatch = Stopwatch.StartNew();
                stopwatch.Start();
                var actual = target.CreatePoco<FakeComplexRow>(pocoDataRequest);
                Trace.TraceInformation($"{stopwatch.Elapsed} Invoke delegate #1");
                stopwatch.Reset();

                Assert.IsNotNull(actual);
                Assert.AreEqual(expected.FakeComplexEntityId, actual.FakeComplexEntityId);
                Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));

                stopwatch.Start();
                target.CreatePoco<FakeComplexRow>(pocoDataRequest);
                Trace.TraceInformation($"{stopwatch.Elapsed} Invoke delegate #2");
                stopwatch.Reset();

                stopwatch.Start();
                target.CreatePoco<FakeComplexRow>(pocoDataRequest);
                Trace.TraceInformation($"{stopwatch.Elapsed} Invoke delegate #3");
                stopwatch.Reset();
            }
        }

        /// <summary>
        /// The create delegate test.
        /// </summary>
        [TestMethod]
        public void CreateDelegate_RaisedPocoFactoryForRaisedComplexRow_IsNotNull()
        {
            using (var target = new RaisedPocoFactory())
            {
                var pocoDataRequest = Generate.CreatePocoDataRequest(Generate.CreateFakeRaisedComplexRow(true));
                var actual = target.CreatePoco<FakeRaisedComplexRow>(pocoDataRequest);
                Assert.IsNotNull(actual);
            }
        }

        /// <summary>
        /// The create delegate test.
        /// </summary>
        [TestMethod]
        public void CreateDelegate_RaisedPocoFactoryForRaisedComplexRow_DelegateSetsPocoAsExpected()
        {
            using (var target = new RaisedPocoFactory())
            {
                var expected = Generate.CreateFakeRaisedComplexRow(true);

                var pocoDataRequest = Generate.CreatePocoDataRequest(expected);

                var stopwatch = Stopwatch.StartNew();
                stopwatch.Start();
                var actual = target.CreatePoco<FakeRaisedComplexRow>(pocoDataRequest);
                Trace.TraceInformation($"{stopwatch.Elapsed} Invoke delegate #1");
                stopwatch.Reset();

                Assert.IsNotNull(actual);
                Assert.AreEqual(expected.FakeComplexEntityId, actual.FakeComplexEntityId);

                Assert.AreEqual(
                    expected.FakeSubEntity,
                    actual.FakeSubEntity,
                    string.Join(Environment.NewLine, expected.FakeSubEntity.GetDifferences(actual.FakeSubEntity)));

                Assert.AreEqual(
                    expected.FakeDependentEntity,
                    actual.FakeDependentEntity,
                    string.Join(Environment.NewLine, expected.FakeDependentEntity.GetDifferences(actual.FakeDependentEntity)));

                Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));

                stopwatch.Start();
                target.CreatePoco<FakeRaisedComplexRow>(pocoDataRequest);
                Trace.TraceInformation($"{stopwatch.Elapsed} Invoke delegate #2");
                stopwatch.Reset();

                stopwatch.Start();
                target.CreatePoco<FakeRaisedComplexRow>(pocoDataRequest);
                Trace.TraceInformation($"{stopwatch.Elapsed} Invoke delegate #3");
                stopwatch.Reset();
            }
        }

        /// <summary>
        /// The create delegate test.
        /// </summary>
        [TestMethod]
        public void CreateDelegate_RaisedPocoFactoryForRaisedComplexRowWithoutDependentEntity_DelegateSetsPocoAsExpected()
        {
            using (var target = new RaisedPocoFactory())
            {
                var expected = Generate.CreateFakeRaisedComplexRow(false);

                var pocoDataRequest = Generate.CreatePocoDataRequest(expected);

                var stopwatch = Stopwatch.StartNew();
                stopwatch.Start();
                var actual = target.CreatePoco<FakeRaisedComplexRow>(pocoDataRequest);
                Trace.TraceInformation($"{stopwatch.Elapsed} Invoke delegate #1");
                stopwatch.Reset();

                Assert.IsNotNull(actual);
                Assert.AreEqual(expected.FakeComplexEntityId, actual.FakeComplexEntityId);

                Assert.AreEqual(
                    expected.FakeSubEntity,
                    actual.FakeSubEntity,
                    string.Join(Environment.NewLine, expected.FakeSubEntity.GetDifferences(actual.FakeSubEntity)));

                Assert.IsNull(expected.FakeDependentEntity);
                Assert.IsNull(actual.FakeDependentEntity);

                Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));

                stopwatch.Start();
                target.CreatePoco<FakeRaisedComplexRow>(pocoDataRequest);
                Trace.TraceInformation($"{stopwatch.Elapsed} Invoke delegate #2");
                stopwatch.Reset();

                stopwatch.Start();
                target.CreatePoco<FakeRaisedComplexRow>(pocoDataRequest);
                Trace.TraceInformation($"{stopwatch.Elapsed} Invoke delegate #3");
                stopwatch.Reset();
            }
        }

        /// <summary>
        /// The create delegate test.
        /// </summary>
        [TestMethod]
        public void CreateDelegate_RaisedPocoFactoryForRaisedDuplicateAttributeRow_DelegateSetsPocoAsExpected()
        {
            // It is vitally important that we use separate (duplicate) POCOs when creating this test object so that any failure to 
            // update one of the instance properties is caught.
            var expected = new InstanceSection
                               {
                                   InstanceSectionId = 234,
                                   EndDate = DateTimeOffset.MaxValue,
                                   InstanceId = 45873,
                                   Instance =
                                       new Instance
                                           {
                                               InstanceId = 45873,
                                               Name = "MyInstance",
                                               OwnerId = 4875,
                                               TemplateVersionId = 43,
                                               TemplateVersion =
                                                   new TemplateVersion
                                                       {
                                                           TemplateVersionId = 43,
                                                           TemplateId = 87,
                                                           Revision = 3,
                                                           FakeSubSubEntityId = 56,
                                                           FakeSubSubUniqueName = "FakeSubSubUniqueName",
                                                           OtherEntityUniqueName = "OtherEntityUniqueName",
                                                           Description = "TemplateVersion",
                                                           Template =
                                                               new Template
                                                                   {
                                                                       TemplateId = 87,
                                                                       Name = "Template"
                                                                   }
                                                       }
                                           },
                                   FakeDependentId = 345,
                                   FakeDependentEntityDependentTimeValue = DateTimeOffset.MinValue.AddYears(2000),
                                   SomeEntityUniqueName = "SomeEntityUniqueName",
                                   InstanceExtension = new InstanceExtension { InstanceExtensionId = 234, Enabled = true },
                                   OwnerId = 7458,
                                   StartDate = DateTimeOffset.Now,
                                   TemplateSectionId = 48753,
                                   TemplateSection =
                                       new TemplateSection
                                           {
                                               TemplateSectionId = 48753,
                                               Header = "My Section Header",
                                               Order = 2,
                                               TemplateVersionId = 43,
                                               TemplateVersion =
                                                   new TemplateVersion
                                                       {
                                                           TemplateVersionId = 43,
                                                           TemplateId = 87,
                                                           Revision = 3,
                                                           FakeSubSubEntityId = 56,
                                                           FakeSubSubUniqueName =
                                                               "FakeSubSubUniqueName",
                                                           OtherEntityUniqueName =
                                                               "OtherEntityUniqueName",
                                                           Description = "TemplateVersion",
                                                           Template =
                                                               new Template
                                                                   {
                                                                       Name = "Template",
                                                                       TemplateId = 87
                                                                   }
                                                       }
                                           }
                               };

            var stopwatch = Stopwatch.StartNew();
            var pocoDataRequest = Generate.CreatePocoDataRequest(expected);
            Trace.TraceInformation($"{stopwatch.Elapsed} Create data request");
            stopwatch.Reset();

            using (var target = new RaisedPocoFactory())
            {
                stopwatch.Start();
                var actual = target.CreatePoco<InstanceSection>(pocoDataRequest);
                Trace.TraceInformation($"{stopwatch.Elapsed} Invoke delegate #1");
                stopwatch.Reset();

                Assert.IsNotNull(actual);
                Assert.AreEqual(expected.InstanceSectionId, actual.InstanceSectionId);

                Assert.AreEqual(
                    expected.Instance.TemplateVersion,
                    actual.Instance.TemplateVersion,
                    string.Join(Environment.NewLine, expected.Instance.TemplateVersion.GetDifferences(actual.Instance.TemplateVersion)));

                Assert.AreEqual(
                    expected.Instance,
                    actual.Instance,
                    string.Join(Environment.NewLine, expected.Instance.GetDifferences(actual.Instance)));

                Assert.AreEqual(
                    expected.TemplateSection,
                    actual.TemplateSection,
                    string.Join(Environment.NewLine, expected.TemplateSection.GetDifferences(actual.TemplateSection)));

                Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));

                stopwatch.Start();
                target.CreatePoco<InstanceSection>(pocoDataRequest);
                Trace.TraceInformation($"{stopwatch.Elapsed} Invoke delegate #2");
                stopwatch.Reset();

                stopwatch.Start();
                target.CreatePoco<InstanceSection>(pocoDataRequest);
                Trace.TraceInformation($"{stopwatch.Elapsed} Invoke delegate #3");
                stopwatch.Reset();
            }
        }

        /// <summary>
        /// The create delegate test.
        /// </summary>
        [TestMethod]
        public void CreateDelegate_RaisedPocoFactoryForRaisedPhysicalNameOverriddenRow_DelegateSetsPocoAsExpected()
        {
            using (var target = new RaisedPocoFactory())
            {
                var expected = new RaisedOverriddenColumnNameRow
                                   {
                                       OverriddenColumnNameId = 345,
                                       Name = "RaisdName",
                                       Description = "RaisedDescription",
                                       EntryTime = DateTimeOffset.Now,
                                       RelatedRowId = 344,
                                       RelatedRow = new RelatedRow { RelatedRowId = 344, Name = "RelatedName" }
                                   };

                var pocoDataRequest = Generate.CreatePocoDataRequest(expected);

                var stopwatch = Stopwatch.StartNew();
                var actual = target.CreatePoco<RaisedOverriddenColumnNameRow>(pocoDataRequest);
                Trace.TraceInformation($"{stopwatch.Elapsed} Invoke delegate #1");
                stopwatch.Reset();

                Assert.IsNotNull(actual);
                Assert.AreEqual(expected.OverriddenColumnNameId, actual.OverriddenColumnNameId);

                Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));

                stopwatch.Start();
                target.CreatePoco<RaisedOverriddenColumnNameRow>(pocoDataRequest);
                Trace.TraceInformation($"{stopwatch.Elapsed} Invoke delegate #2");
                stopwatch.Reset();

                stopwatch.Start();
                target.CreatePoco<RaisedOverriddenColumnNameRow>(pocoDataRequest);
                Trace.TraceInformation($"{stopwatch.Elapsed} Invoke delegate #3");
                stopwatch.Reset();
            }
        }
    }
}