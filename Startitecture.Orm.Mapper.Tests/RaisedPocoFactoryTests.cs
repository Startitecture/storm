﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RaisedPocoFactoryTests.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Mapper.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;

    using Microsoft.CSharp.RuntimeBinder;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Schema;
    using Startitecture.Orm.Testing.Entities;
    using Startitecture.Orm.Testing.Moq;

    /// <summary>
    /// The raised POCO factory tests.
    /// </summary>
    [TestClass]
    public class RaisedPocoFactoryTests
    {
        /// <summary>
        /// The create delegate test.
        /// </summary>
        [TestMethod]
        public void CreateDelegate_RaisedPocoFactoryForFlatComplexRow_DelegateSetsPocoAsExpected()
        {
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var entityDefinition = definitionProvider.Resolve<ComplexFlatRow>();
            var databaseContext = new Mock<IDatabaseContext>();
            databaseContext.Setup(context => context.GetValueMapper(It.IsAny<Type>(), It.IsAny<Type>())).Returns((IValueMapper)null);

            using (var target = new RaisedPocoFactory(definitionProvider))
            {
                var stopwatch = new Stopwatch();
                var expected = Generate.CreateFakeComplexRow();
                ComplexFlatRow actual;

                using (var reader = expected.MockDataReader(entityDefinition.ReturnableAttributes).Object)
                {
                    reader.Read();
                    var pocoDataRequest = new PocoDataRequest(reader, entityDefinition, databaseContext.Object);

                    stopwatch.Start();
                    actual = target.CreatePoco<ComplexFlatRow>(pocoDataRequest);
                }

                Trace.TraceInformation($"{stopwatch.Elapsed} Invoke delegate #1");
                stopwatch.Reset();

                Assert.IsNotNull(actual);
                Assert.AreEqual(expected.FakeComplexEntityId, actual.FakeComplexEntityId);
                Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));

                using (var reader = expected.MockDataReader(entityDefinition.ReturnableAttributes).Object)
                {
                    reader.Read();
                    var pocoDataRequest = new PocoDataRequest(reader, entityDefinition, databaseContext.Object);

                    stopwatch.Start();
                    target.CreatePoco<ComplexFlatRow>(pocoDataRequest);
                }

                Trace.TraceInformation($"{stopwatch.Elapsed} Invoke delegate #2");
                stopwatch.Reset();

                using (var reader = expected.MockDataReader(entityDefinition.ReturnableAttributes).Object)
                {
                    reader.Read();
                    var pocoDataRequest = new PocoDataRequest(reader, entityDefinition, databaseContext.Object);

                    stopwatch.Start();
                    target.CreatePoco<ComplexFlatRow>(pocoDataRequest);
                }

                Trace.TraceInformation($"{stopwatch.Elapsed} Invoke delegate #3");
                stopwatch.Reset();
            }
        }

        /// <summary>
        /// The create delegate test.
        /// </summary>
        [TestMethod]
        public void CreateDelegate_RaisedPocoFactoryForDynamic_DelegateSetsPocoAsExpected()
        {
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var databaseContext = new Mock<IDatabaseContext>();
            databaseContext.Setup(context => context.GetValueMapper(It.IsAny<Type>(), It.IsAny<Type>())).Returns((IValueMapper)null);

            using (var target = new RaisedPocoFactory(definitionProvider))
            {
                var expected = Generate.CreateFakeComplexRow();
                var entityDefinition = definitionProvider.Resolve<ComplexFlatRow>();

                Expression<Func<ComplexFlatRow, object>> expression1 = row => row.FakeComplexEntityId;
                Expression<Func<ComplexFlatRow, object>> expression2 = row => row.FakeDependentEntityDependentIntegerValue;
                Expression<Func<ComplexFlatRow, object>> expression3 = row => row.FakeSubSubEntityUniqueName;

                var attributes = new[]
                                     {
                                         expression1,
                                         expression2,
                                         expression3
                                     }.Select(entityDefinition.Find)
                    .ToList();

                dynamic actual;

                using (var reader = expected.MockDataReader(attributes).Object)
                {
                    reader.Read();
                    var pocoDataRequest = new PocoDataRequest(reader, attributes, databaseContext.Object);
                    actual = target.CreatePoco<dynamic>(pocoDataRequest);
                }

                Assert.IsNotNull(actual);
                Assert.AreEqual(expected.FakeComplexEntityId, actual.FakeComplexEntityId);
                Assert.AreEqual(expected.FakeDependentEntityDependentIntegerValue, actual.FakeDependentEntityDependentIntegerValue);
                Assert.AreEqual(expected.FakeSubSubEntityUniqueName, actual.FakeSubSubEntityUniqueName);
                Assert.ThrowsException<RuntimeBinderException>(() => Assert.IsNull(actual.Description));
            }
        }

        /// <summary>
        /// The create delegate test.
        /// </summary>
        [TestMethod]
        public void CreateDelegate_RaisedPocoFactoryForRaisedComplexRow_IsNotNull()
        {
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var entityDefinition = definitionProvider.Resolve<ComplexRaisedRow>();
            var databaseContext = new Mock<IDatabaseContext>();
            databaseContext.Setup(context => context.GetValueMapper(It.IsAny<Type>(), It.IsAny<Type>())).Returns((IValueMapper)null);

            using (var target = new RaisedPocoFactory(definitionProvider))
            {
                var expected = Generate.CreateFakeRaisedComplexRow(true);

                using (var reader = expected.MockDataReader(entityDefinition.ReturnableAttributes).Object)
                {
                    reader.Read();
                    var pocoDataRequest = new PocoDataRequest(reader, entityDefinition, databaseContext.Object);
                    var actual = target.CreatePoco<ComplexRaisedRow>(pocoDataRequest);
                    Assert.IsNotNull(actual);
                }
            }
        }

        /// <summary>
        /// The create delegate test.
        /// </summary>
        [TestMethod]
        public void CreateDelegate_RaisedPocoFactoryForRaisedComplexRow_DelegateSetsPocoAsExpected()
        {
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var entityDefinition = definitionProvider.Resolve<ComplexRaisedRow>();
            var databaseContext = new Mock<IDatabaseContext>();
            databaseContext.Setup(context => context.GetValueMapper(It.IsAny<Type>(), It.IsAny<Type>())).Returns((IValueMapper)null);

            using (var target = new RaisedPocoFactory(definitionProvider))
            {
                var expected = Generate.CreateFakeRaisedComplexRow(true);

                Stopwatch stopwatch = new Stopwatch();

                using (var reader = expected.MockDataReader(entityDefinition.ReturnableAttributes).Object)
                {
                    reader.Read();
                    var pocoDataRequest = new PocoDataRequest(reader, entityDefinition, databaseContext.Object);

                    stopwatch.Start();
                    var actual = target.CreatePoco<ComplexRaisedRow>(pocoDataRequest);
                    Trace.TraceInformation($"{stopwatch.Elapsed} Invoke delegate #1");
                    stopwatch.Reset();

                    Assert.IsNotNull(actual);
                    Assert.AreEqual(expected.ComplexEntityId, actual.ComplexEntityId);

                    Assert.AreEqual(
                        expected.SubEntity,
                        actual.SubEntity,
                        string.Join(Environment.NewLine, expected.SubEntity.GetDifferences(actual.SubEntity)));

                    Assert.AreEqual(
                        expected.DependentEntity,
                        actual.DependentEntity,
                        string.Join(Environment.NewLine, expected.DependentEntity.GetDifferences(actual.DependentEntity)));

                    Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
                }

                stopwatch.Start();
                using (var reader = expected.MockDataReader(entityDefinition.ReturnableAttributes).Object)
                {
                    reader.Read();
                    var pocoDataRequest = new PocoDataRequest(reader, entityDefinition, databaseContext.Object);

                    stopwatch.Start();
                    target.CreatePoco<ComplexRaisedRow>(pocoDataRequest);
                    Trace.TraceInformation($"{stopwatch.Elapsed} Invoke delegate #2");
                }

                stopwatch.Reset();

                stopwatch.Start();
                using (var reader = expected.MockDataReader(entityDefinition.ReturnableAttributes).Object)
                {
                    reader.Read();
                    var pocoDataRequest = new PocoDataRequest(reader, entityDefinition, databaseContext.Object);

                    stopwatch.Start();
                    target.CreatePoco<ComplexRaisedRow>(pocoDataRequest);
                    Trace.TraceInformation($"{stopwatch.Elapsed} Invoke delegate #3");
                }

                stopwatch.Reset();
            }
        }

        /// <summary>
        /// The create delegate test.
        /// </summary>
        [TestMethod]
        public void CreateDelegate_RaisedPocoFactoryForRaisedDynamic_DelegateSetsPocoAsExpected()
        {
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var expected = Generate.CreateFakeRaisedComplexRow(true);
            var databaseContext = new Mock<IDatabaseContext>();
            databaseContext.Setup(context => context.GetValueMapper(It.IsAny<Type>(), It.IsAny<Type>())).Returns((IValueMapper)null);

            using (var target = new RaisedPocoFactory(definitionProvider))
            {
                var entityDefinition = definitionProvider.Resolve<ComplexRaisedRow>();

                Expression<Func<ComplexRaisedRow, object>> expression1 = row => row.ComplexEntityId;
                Expression<Func<ComplexRaisedRow, object>> expression2 = row => row.DependentEntity.DependentIntegerValue;
                Expression<Func<ComplexRaisedRow, object>> expression3 = row => row.SubEntity.SubSubEntity.UniqueName;

                var attributes = new[]
                                     {
                                         expression1,
                                         expression2,
                                         expression3
                                     }.Select(entityDefinition.Find)
                    .ToList();

                dynamic actual;

                using (var reader = expected.MockDataReader(attributes).Object)
                {
                    reader.Read();
                    var pocoDataRequest = new PocoDataRequest(reader, attributes, databaseContext.Object);
                    actual = target.CreatePoco<dynamic>(pocoDataRequest);
                }

                Assert.IsNotNull(actual);
                Assert.AreEqual(expected.ComplexEntityId, actual.ComplexEntityId);
                Assert.AreEqual(expected.DependentEntity.DependentIntegerValue, actual.DependentEntityDependentIntegerValue);
                Assert.AreEqual(expected.SubEntity.SubSubEntity.UniqueName, actual.SubSubEntityUniqueName);
                Assert.ThrowsException<RuntimeBinderException>(() => Assert.IsNull(actual.Description));
            }
        }

        /// <summary>
        /// The create delegate test.
        /// </summary>
        [TestMethod]
        public void CreateDelegate_RaisedPocoFactoryForDomainAggregateList_SharedEntitiesHaveReferenceEquality()
        {
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var databaseContext = new Mock<IDatabaseContext>();
            databaseContext.Setup(context => context.GetValueMapper(It.IsAny<Type>(), It.IsAny<Type>())).Returns((IValueMapper)null);

            using (var target = new RaisedPocoFactory(definitionProvider))
            {
                var aggregateOptionRow =
                    new AggregateOptionRow { AggregateOptionId = 3, AggregateOptionTypeId = 4, Name = "Slim Shady", Value = 324.10m };

                var topContainerRow = new TopContainerRow
                {
                    Name = "All TV Evar",
                    TopContainerId = 2
                };

                var tomNJerry = new DomainIdentityRow
                {
                    DomainIdentityId = 23,
                    FirstName = "Tom",
                    MiddleName = "N.",
                    LastName = "Jerry",
                    UniqueIdentifier = "tomnjerry@what.com"
                };

                var mahCatzCategory = new CategoryAttributeRow { CategoryAttributeId = 3, Name = "Mah Catz", IsActive = true, IsSystem = true };
                var fooBarCategory = new CategoryAttributeRow { CategoryAttributeId = 4, Name = "foobar", IsActive = true, IsSystem = false };

                var porkyPig = new DomainIdentityRow
                {
                    DomainIdentityId = 55,
                    FirstName = "Porky",
                    MiddleName = "That's All Folks",
                    LastName = "Pig",
                    UniqueIdentifier = "pp@whatwhat.com"
                };
                var warnerBros = new SubContainerRow
                {
                    Name = "Warner Bros",
                    SubContainerId = 234,
                    TopContainerId = 2,
                    TopContainer = topContainerRow
                };
                var template1 = new TemplateRow { Name = "Template1", TemplateId = 44 };
                var template2 = new TemplateRow { Name = "Template34", TemplateId = 45 };
                var expected = new List<DomainAggregateRow>
                                   {
                                       new DomainAggregateRow
                                           {
                                               DomainAggregateId = 2342,
                                               AggregateOption = aggregateOptionRow,
                                               Name = "DomainAg1",
                                               CategoryAttribute = mahCatzCategory,
                                               CategoryAttributeId = mahCatzCategory.CategoryAttributeId,
                                               CreatedBy = tomNJerry,
                                               CreatedByDomainIdentityId = tomNJerry.DomainIdentityId,
                                               CreatedTime = DateTimeOffset.Now.AddDays(-5),
                                               Description = "My First Domain Aggregate YAY",
                                               OtherAggregate =
                                                   new OtherAggregateRow
                                                       {
                                                           OtherAggregateId = 29,
                                                           AggregateOptionTypeId = 5,
                                                           Name = "OtherAggregate"
                                                       },
                                               LastModifiedBy = porkyPig,
                                               LastModifiedByDomainIdentityId = porkyPig.DomainIdentityId,
                                               LastModifiedTime = DateTimeOffset.Now,
                                               SubContainer = warnerBros,
                                               SubContainerId = warnerBros.SubContainerId,
                                               TemplateId = template1.TemplateId,
                                               Template = template1
                                           },
                                       new DomainAggregateRow
                                           {
                                               DomainAggregateId = 2343,
                                               AggregateOption = aggregateOptionRow,
                                               Name = "DomainAg2",
                                               CategoryAttribute = mahCatzCategory,
                                               CategoryAttributeId = mahCatzCategory.CategoryAttributeId,
                                               CreatedBy = tomNJerry,
                                               CreatedByDomainIdentityId = tomNJerry.DomainIdentityId,
                                               CreatedTime = DateTimeOffset.Now.AddDays(-4),
                                               Description = "My Second Domain Aggregate YAY",
                                               LastModifiedBy = porkyPig,
                                               LastModifiedByDomainIdentityId = porkyPig.DomainIdentityId,
                                               LastModifiedTime = DateTimeOffset.Now.AddHours(-1),
                                               SubContainer = warnerBros,
                                               SubContainerId = warnerBros.SubContainerId,
                                               TemplateId = template1.TemplateId,
                                               Template = template1
                                           },
                                       new DomainAggregateRow
                                           {
                                               DomainAggregateId = 2345,
                                               AggregateOption = aggregateOptionRow,
                                               Name = "DomainAg3",
                                               CategoryAttribute = fooBarCategory,
                                               CategoryAttributeId = fooBarCategory.CategoryAttributeId,
                                               CreatedBy = tomNJerry,
                                               CreatedByDomainIdentityId = tomNJerry.DomainIdentityId,
                                               CreatedTime = DateTimeOffset.Now.AddDays(-2),
                                               Description = "My Third Domain Aggregate YAY",
                                               LastModifiedBy = porkyPig,
                                               LastModifiedByDomainIdentityId = porkyPig.DomainIdentityId,
                                               LastModifiedTime = DateTimeOffset.Now.AddSeconds(-97),
                                               SubContainer = warnerBros,
                                               SubContainerId = warnerBros.SubContainerId,
                                               TemplateId = template1.TemplateId,
                                               Template = template1
                                           },
                                       new DomainAggregateRow
                                           {
                                               DomainAggregateId = 2346,
                                               AggregateOption = aggregateOptionRow,
                                               Name = "DomainAg4",
                                               CategoryAttribute = fooBarCategory,
                                               CategoryAttributeId = fooBarCategory.CategoryAttributeId,
                                               CreatedBy = porkyPig,
                                               CreatedByDomainIdentityId = porkyPig.DomainIdentityId,
                                               CreatedTime = DateTimeOffset.Now.AddDays(-1),
                                               Description = "My Fourth Domain Aggregate YAY",
                                               LastModifiedBy = porkyPig,
                                               LastModifiedByDomainIdentityId = porkyPig.DomainIdentityId,
                                               LastModifiedTime = DateTimeOffset.Now.AddHours(-3),
                                               SubContainer = warnerBros,
                                               SubContainerId = warnerBros.SubContainerId,
                                               TemplateId = template2.TemplateId,
                                               Template = template2
                                           },
                                       new DomainAggregateRow
                                           {
                                               DomainAggregateId = 2347,
                                               AggregateOption = aggregateOptionRow,
                                               Name = "DomainAg7",
                                               CategoryAttribute = mahCatzCategory,
                                               CategoryAttributeId = mahCatzCategory.CategoryAttributeId,
                                               CreatedBy = porkyPig,
                                               CreatedByDomainIdentityId = porkyPig.DomainIdentityId,
                                               CreatedTime = DateTimeOffset.Now.AddDays(-5),
                                               Description = "My Last Domain Aggregate YAY",
                                               LastModifiedBy = porkyPig,
                                               LastModifiedByDomainIdentityId = porkyPig.DomainIdentityId,
                                               LastModifiedTime = DateTimeOffset.Now.AddMinutes(-16),
                                               SubContainer = warnerBros,
                                               SubContainerId = warnerBros.SubContainerId,
                                               TemplateId = template1.TemplateId,
                                               Template = template1
                                           }
                                   };

                var watch = Stopwatch.StartNew();
                var actual = new List<DomainAggregateRow>();
                var entityDefinition = definitionProvider.Resolve<DomainAggregateRow>();

                using (var reader = expected.MockDataReaderForList(entityDefinition.ReturnableAttributes.ToList()).Object)
                {
                    while (reader.Read())
                    {
                        actual.Add(target.CreatePoco<DomainAggregateRow>(new PocoDataRequest(reader, entityDefinition, databaseContext.Object)));
                    }
                }

                watch.Stop();
                Trace.TraceInformation($"{watch.Elapsed}");

                Assert.AreEqual(expected.First(), actual.First(), string.Join(Environment.NewLine, expected.First().GetDifferences(actual.First())));

                CollectionAssert.AreEqual(expected, actual);

                Assert.IsTrue(actual.Select(x => x.SubContainer?.TopContainer).All(x => x != null));
                Assert.IsTrue(actual.Select(x => x.SubContainer).All(x => x != null));
                Assert.IsTrue(actual.Select(x => x.Template).All(x => x != null));
                Assert.IsTrue(actual.Select(x => x.CreatedBy).All(x => x != null));
                Assert.IsTrue(actual.Select(x => x.LastModifiedBy).All(x => x != null));
                Assert.IsTrue(actual.Select(x => x.CategoryAttribute).All(x => x != null));
                Assert.IsTrue(actual.Select(x => x.AggregateOption).All(x => x != null));

                foreach (var group in from c in actual.Select(x => x.SubContainer?.TopContainer) group c by c.TopContainerId)
                {
                    Assert.IsTrue(group.All(x => ReferenceEquals(x, group.First())));
                }

                foreach (var group in from c in actual.Select(x => x.SubContainer) group c by c.SubContainerId)
                {
                    Assert.IsTrue(group.All(x => ReferenceEquals(x, group.First())));
                }

                foreach (var group in from t in actual.Select(x => x.Template) group t by t.TemplateId)
                {
                    Assert.IsTrue(group.All(x => ReferenceEquals(x, group.First())));
                }

                foreach (var group in from o in actual.Select(x => x.AggregateOption) group o by o.AggregateOptionId)
                {
                    Assert.IsTrue(group.All(x => ReferenceEquals(x, group.First())));
                }

                foreach (var group in from i in actual.Select(x => x.LastModifiedBy) group i by i.DomainIdentityId)
                {
                    Assert.IsTrue(group.All(x => ReferenceEquals(x, group.First())));
                }

                foreach (var group in from i in actual.Select(x => x.CreatedBy) group i by i.DomainIdentityId)
                {
                    Assert.IsTrue(group.All(x => ReferenceEquals(x, group.First())));
                }

                foreach (var group in from a in actual.Select(x => x.CategoryAttribute) group a by a.CategoryAttributeId)
                {
                    Assert.IsTrue(group.All(x => ReferenceEquals(x, group.First())));
                }
            }
        }

        /// <summary>
        /// The create delegate test.
        /// </summary>
        [TestMethod]
        public void CreateDelegate_RaisedPocoFactoryForRaisedComplexRowWithoutDependentEntity_DelegateSetsPocoAsExpected()
        {
            var expected = Generate.CreateFakeRaisedComplexRow(false);

            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var entityDefinition = definitionProvider.Resolve<ComplexRaisedRow>();
            var databaseContext = new Mock<IDatabaseContext>();
            databaseContext.Setup(context => context.GetValueMapper(It.IsAny<Type>(), It.IsAny<Type>())).Returns((IValueMapper)null);

            using (var target = new RaisedPocoFactory(definitionProvider))
            {
                var stopwatch = Stopwatch.StartNew();
                ComplexRaisedRow actual;

                using (var reader = expected.MockDataReader(entityDefinition.ReturnableAttributes).Object)
                {
                    stopwatch.Start();
                    reader.Read();
                    actual = target.CreatePoco<ComplexRaisedRow>(new PocoDataRequest(reader, entityDefinition, databaseContext.Object));
                    Trace.TraceInformation($"{stopwatch.Elapsed} Invoke delegate #1");
                }

                stopwatch.Reset();

                Assert.IsNotNull(actual);
                Assert.AreEqual(expected.ComplexEntityId, actual.ComplexEntityId);

                Assert.AreEqual(
                    expected.SubEntity,
                    actual.SubEntity,
                    string.Join(Environment.NewLine, expected.SubEntity.GetDifferences(actual.SubEntity)));

                Assert.IsNull(expected.DependentEntity);
                Assert.IsNull(actual.DependentEntity);

                Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));

                stopwatch.Start();
                using (var reader = expected.MockDataReader(entityDefinition.ReturnableAttributes).Object)
                {
                    reader.Read();
                    target.CreatePoco<ComplexRaisedRow>(new PocoDataRequest(reader, entityDefinition, databaseContext.Object));
                }

                Trace.TraceInformation($"{stopwatch.Elapsed} Invoke delegate #2");
                stopwatch.Reset();

                stopwatch.Start();
                using (var reader = expected.MockDataReader(entityDefinition.ReturnableAttributes).Object)
                {
                    reader.Read();
                    target.CreatePoco<ComplexRaisedRow>(new PocoDataRequest(reader, entityDefinition, databaseContext.Object));
                }

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

            var stopwatch = new Stopwatch();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var entityDefinition = definitionProvider.Resolve<InstanceSection>();
            var databaseContext = new Mock<IDatabaseContext>();
            databaseContext.Setup(context => context.GetValueMapper(It.IsAny<Type>(), It.IsAny<Type>())).Returns((IValueMapper)null);
            stopwatch.Reset();

            using (var target = new RaisedPocoFactory(definitionProvider))
            {
                InstanceSection actual;

                using (var reader = expected.MockDataReader(entityDefinition.ReturnableAttributes).Object)
                {
                    stopwatch.Start();
                    reader.Read();
                    var pocoDataRequest = new PocoDataRequest(reader, entityDefinition, databaseContext.Object);
                    Trace.TraceInformation($"{stopwatch.Elapsed} Create data request");
                    stopwatch.Reset();
                    actual = target.CreatePoco<InstanceSection>(pocoDataRequest);
                }

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

                using (var reader = expected.MockDataReader(entityDefinition.ReturnableAttributes).Object)
                {
                    stopwatch.Start();
                    reader.Read();
                    var pocoDataRequest = new PocoDataRequest(reader, entityDefinition, databaseContext.Object);
                    target.CreatePoco<InstanceSection>(pocoDataRequest);
                }

                Trace.TraceInformation($"{stopwatch.Elapsed} Invoke delegate #2");
                stopwatch.Reset();

                using (var reader = expected.MockDataReader(entityDefinition.ReturnableAttributes).Object)
                {
                    stopwatch.Start();
                    reader.Read();
                    var pocoDataRequest = new PocoDataRequest(reader, entityDefinition, databaseContext.Object);
                    target.CreatePoco<InstanceSection>(pocoDataRequest);
                }

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
            var stopwatch = new Stopwatch();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var entityDefinition = definitionProvider.Resolve<RaisedOverriddenColumnNameRow>();
            var databaseContext = new Mock<IDatabaseContext>();
            databaseContext.Setup(context => context.GetValueMapper(It.IsAny<Type>(), It.IsAny<Type>())).Returns((IValueMapper)null);

            using (var target = new RaisedPocoFactory(definitionProvider))
            {
                var expected = new RaisedOverriddenColumnNameRow
                {
                    OverriddenColumnNameId = 345,
                    Name = "RaisedName",
                    Description = "RaisedDescription",
                    EntryTime = DateTimeOffset.Now,
                    RelatedRowId = 344,
                    RelatedRow = new RelatedRow { RelatedRowId = 344, Name = "RelatedName" }
                };

                RaisedOverriddenColumnNameRow actual;

                using (var reader = expected.MockDataReader(entityDefinition.ReturnableAttributes).Object)
                {
                    stopwatch.Start();
                    reader.Read();
                    var pocoDataRequest = new PocoDataRequest(reader, entityDefinition, databaseContext.Object);

                    stopwatch.Reset();
                    actual = target.CreatePoco<RaisedOverriddenColumnNameRow>(pocoDataRequest);
                    Trace.TraceInformation($"{stopwatch.Elapsed} Invoke delegate #1");
                }

                stopwatch.Reset();

                Assert.IsNotNull(actual);
                Assert.AreEqual(expected.OverriddenColumnNameId, actual.OverriddenColumnNameId);

                Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));

                using (var reader = expected.MockDataReader(entityDefinition.ReturnableAttributes).Object)
                {
                    stopwatch.Start();
                    reader.Read();
                    var pocoDataRequest = new PocoDataRequest(reader, entityDefinition, databaseContext.Object);

                    stopwatch.Reset();
                    target.CreatePoco<RaisedOverriddenColumnNameRow>(pocoDataRequest);
                    Trace.TraceInformation($"{stopwatch.Elapsed} Invoke delegate #2");
                }

                stopwatch.Reset();

                using (var reader = expected.MockDataReader(entityDefinition.ReturnableAttributes).Object)
                {
                    stopwatch.Start();
                    reader.Read();
                    var pocoDataRequest = new PocoDataRequest(reader, entityDefinition, databaseContext.Object);

                    stopwatch.Reset();
                    target.CreatePoco<RaisedOverriddenColumnNameRow>(pocoDataRequest);
                }

                Trace.TraceInformation($"{stopwatch.Elapsed} Invoke delegate #3");
                stopwatch.Reset();
            }
        }
    }
}