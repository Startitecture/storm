// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseRepositoryProviderTests.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Mapper.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Core;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Repository;
    using Startitecture.Orm.Schema;
    using Startitecture.Orm.Testing.Entities;

    /// <summary>
    /// The database repository provider tests.
    /// </summary>
    [TestClass]
    public class DatabaseRepositoryProviderTests
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
        /// The configuration root.
        /// </summary>
        private static IConfigurationRoot ConfigurationRoot => new ConfigurationBuilder().AddJsonFile("appSettings.json", false).Build();

        /// <summary>
        /// The save test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void Save_NewField_IdSet()
        {
            var databaseFactory = new DefaultDatabaseFactory(
                ConfigurationRoot.GetConnectionString("OrmTestDb"),
                "System.Data.SqlClient",
                new DataAnnotationsDefinitionProvider());

            using (var target = new DatabaseRepositoryProvider(databaseFactory,  this.entityMapper))
            {
                target.StartTransaction();

                try
                {
                    var item = new FieldRow { Name = "MahField", Description = "Mah Field Description" };
                    var actual = target.Save(item);
                    Assert.AreNotEqual(0, actual.FieldId);
                }
                finally
                {
                    target.AbortTransaction();
                }
            }
        }

        /// <summary>
        /// The save test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void Save_ExistingField_ChangesMatchExpected()
        {
            FieldRow item;

            var databaseFactory = new DefaultDatabaseFactory(
                ConfigurationRoot.GetConnectionString("OrmTestDb"),
                "System.Data.SqlClient",
                new DataAnnotationsDefinitionProvider());

            using (var target = new DatabaseRepositoryProvider(databaseFactory, this.entityMapper))
            {
                item = new FieldRow
                           {
                               Name = $"UNIT_TEST-Field{Generator.Next(int.MaxValue)}",
                               Description = "Mah Field Description"
                           };

                target.Save(item);
            }

            // Completely new context to test that caching is not involved.
            FieldRow expected;
            using (var target = new DatabaseRepositoryProvider(databaseFactory, this.entityMapper))
            {
                expected = new FieldRow
                             {
                                 FieldId = item.FieldId,
                                 Name = item.Name,
                                 Description = "Mah Field Description The Second of That Name"
                             };

                target.Save(expected);
            }

            // New context again
            using (var target = new DatabaseRepositoryProvider(databaseFactory, this.entityMapper))
            {
                var actual = target.GetFirstOrDefault(Select.From<FieldRow>().WhereEqual(row => row.FieldId, item.FieldId));
                Assert.IsNotNull(actual);
                Assert.AreEqual(expected, actual);
                Assert.AreEqual(expected.FieldId, actual.FieldId);
            }
        }

        /// <summary>
        /// The get selection test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void GetSelection_ExistingDomainAggregates_MatchesExpected()
        {
            List<DomainAggregateRow> expected;

            var databaseFactory = new DefaultDatabaseFactory(
                ConfigurationRoot.GetConnectionString("OrmTestDb"),
                "System.Data.SqlClient",
                new DataAnnotationsDefinitionProvider());

            using (var target = new DatabaseRepositoryProvider(databaseFactory, this.entityMapper))
            {
                var topContainer2 = new TopContainerRow
                                        {
                                            Name = $"UNIT_TEST:TopContainer2-{Generator.Next(int.MaxValue)}"
                                        };

                target.Save(topContainer2);

                var subContainerA = new SubContainerRow
                                        {
                                            Name = $"UNIT_TEST:SubContainerA-{Generator.Next(int.MaxValue)}",
                                            TopContainer = topContainer2,
                                            TopContainerId = topContainer2.TopContainerId
                                        };

                target.Save(subContainerA);

                var categoryAttribute20 = new CategoryAttributeRow
                                              {
                                                  Name = $"UNIT_TEST:CatAttr20-{Generator.Next(int.MaxValue)}",
                                                  IsActive = true,
                                                  IsSystem = false
                                              };

                target.Save(categoryAttribute20);

                var timBobIdentity = new DomainIdentityRow
                                         {
                                             FirstName = "Tim",
                                             LastName = "Bob",
                                             UniqueIdentifier = $"UNIT_TEST:timbob@unittest.com-{Generator.Next(int.MaxValue)}"
                                         };

                target.Save(timBobIdentity);

                var fooBarIdentity = new DomainIdentityRow
                                         {
                                             FirstName = "Foo",
                                             LastName = "Bar",
                                             UniqueIdentifier = $"UNIT_TEST:foobar@unittest.com-{Generator.Next(int.MaxValue)}"
                                         };

                target.Save(fooBarIdentity);

                var otherAggregate10 = new OtherAggregateRow
                                           {
                                               Name = $"UNIT_TEST:OtherAggregate10-{Generator.Next(int.MaxValue)}",
                                               AggregateOptionTypeId = 3
                                           };

                target.Save(otherAggregate10);

                var template23 = new TemplateRow
                                     {
                                         Name = $"UNIT_TEST:Template23-{Generator.Next(int.MaxValue)}"
                                     };

                target.Save(template23);

                var aggregateOption1 = new AggregateOptionRow
                                           {
                                               Name = $"UNIT_TEST:AgOption1-{Generator.Next(int.MaxValue)}",
                                               AggregateOptionTypeId = 2,
                                               Value = 439034.0332m
                                           };

                var aggregateOption2 = new AggregateOptionRow
                                           {
                                               Name = $"UNIT_TEST:AgOption2-{Generator.Next(int.MaxValue)}",
                                               AggregateOptionTypeId = 4,
                                               Value = 32453253
                                           };

                var domainAggregate1 = new DomainAggregateRow
                                           {
                                               AggregateOption = aggregateOption1,
                                               CategoryAttribute = categoryAttribute20,
                                               CategoryAttributeId = categoryAttribute20.CategoryAttributeId,
                                               Name = $"UNIT_TEST:Aggregate1-{Generator.Next(int.MaxValue)}",
                                               Description = "My First Domain Aggregate",
                                               CreatedBy = timBobIdentity,
                                               CreatedByDomainIdentityId = timBobIdentity.DomainIdentityId,
                                               CreatedTime = DateTimeOffset.Now.AddMonths(-1),
                                               LastModifiedBy = fooBarIdentity,
                                               LastModifiedByDomainIdentityId = fooBarIdentity.DomainIdentityId,
                                               LastModifiedTime = DateTimeOffset.Now,
                                               SubContainer = subContainerA,
                                               SubContainerId = subContainerA.SubContainerId,
                                               Template = template23,
                                               TemplateId = template23.TemplateId
                                           };

                target.Save(domainAggregate1);

                var domainAggregate2 = new DomainAggregateRow
                                           {
                                               AggregateOption = aggregateOption2,
                                               CategoryAttribute = categoryAttribute20,
                                               CategoryAttributeId = categoryAttribute20.CategoryAttributeId,
                                               Name = $"UNIT_TEST:Aggregate2-{Generator.Next(int.MaxValue)}",
                                               Description = "My Second Domain Aggregate",
                                               CreatedBy = timBobIdentity,
                                               CreatedByDomainIdentityId = timBobIdentity.DomainIdentityId,
                                               CreatedTime = DateTimeOffset.Now.AddMonths(-1),
                                               LastModifiedBy = fooBarIdentity,
                                               LastModifiedByDomainIdentityId = fooBarIdentity.DomainIdentityId,
                                               LastModifiedTime = DateTimeOffset.Now,
                                               OtherAggregate = otherAggregate10,
                                               SubContainer = subContainerA,
                                               SubContainerId = subContainerA.SubContainerId,
                                               Template = template23,
                                               TemplateId = template23.TemplateId
                                           };

                target.Save(domainAggregate2);

                var domainAggregate3 = new DomainAggregateRow
                                           {
                                               CategoryAttribute = categoryAttribute20,
                                               CategoryAttributeId = categoryAttribute20.CategoryAttributeId,
                                               Name = $"UNIT_TEST:Aggregate3-{Generator.Next(int.MaxValue)}",
                                               Description = "My Third Domain Aggregate",
                                               CreatedBy = timBobIdentity,
                                               CreatedByDomainIdentityId = timBobIdentity.DomainIdentityId,
                                               CreatedTime = DateTimeOffset.Now.AddMonths(-1),
                                               LastModifiedBy = timBobIdentity,
                                               LastModifiedByDomainIdentityId = timBobIdentity.DomainIdentityId,
                                               LastModifiedTime = DateTimeOffset.Now,
                                               SubContainer = subContainerA,
                                               SubContainerId = subContainerA.SubContainerId,
                                               Template = template23,
                                               TemplateId = template23.TemplateId
                                           };

                target.Save(domainAggregate3);

                aggregateOption1.AggregateOptionId = domainAggregate1.DomainAggregateId;
                target.Save(aggregateOption1);

                aggregateOption2.AggregateOptionId = domainAggregate2.DomainAggregateId;
                target.Save(aggregateOption2);

                var associationRow = new AssociationRow
                                         {
                                             DomainAggregateId = domainAggregate2.DomainAggregateId,
                                             OtherAggregateId = otherAggregate10.OtherAggregateId
                                         };

                target.Save(associationRow);

                expected = new List<DomainAggregateRow>
                               {
                                   domainAggregate1,
                                   domainAggregate2,
                                   domainAggregate3
                               };
            }

            using (var target = new DatabaseRepositoryProvider(databaseFactory, this.entityMapper))
            {
                var itemSelection = Select.From<DomainAggregateRow>()
                    .WhereEqual(row => row.SubContainerId, expected.First().SubContainerId)
                    .LeftJoin<AssociationRow>(row => row.DomainAggregateId, row => row.DomainAggregateId)
                    .LeftJoin<AssociationRow, OtherAggregateRow>(row => row.OtherAggregateId, row => row.OtherAggregateId)
                    .InnerJoin(row => row.CategoryAttributeId, row => row.CategoryAttribute.CategoryAttributeId)
                    .InnerJoin(row => row.CreatedByDomainIdentityId, row => row.CreatedBy.DomainIdentityId)
                    .LeftJoin(row => row.DomainAggregateId, row => row.AggregateOption.AggregateOptionId)
                    .InnerJoin(row => row.LastModifiedByDomainIdentityId, row => row.LastModifiedBy.DomainIdentityId)
                    .InnerJoin(row => row.SubContainerId, row => row.SubContainer.SubContainerId)
                    .InnerJoin(row => row.SubContainer.TopContainerId, row => row.SubContainer.TopContainer.TopContainerId)
                    .InnerJoin(row => row.TemplateId, row => row.Template.TemplateId);

                var actual = target.GetSelection(itemSelection).OrderBy(x => x.Name).ToList();
                Assert.AreEqual(
                    expected.First(),
                    actual.First(),
                    string.Join(Environment.NewLine, expected.First().GetDifferences(actual.First())));

                CollectionAssert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        /// The get first or default test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void GetFirstOrDefault_ExistingField_MatchesExpected()
        {
            FieldRow expected;

            var databaseFactory = new DefaultDatabaseFactory(
                ConfigurationRoot.GetConnectionString("OrmTestDb"),
                "System.Data.SqlClient",
                new DataAnnotationsDefinitionProvider());

            using (var target = new DatabaseRepositoryProvider(databaseFactory, this.entityMapper))
            {
                expected = new FieldRow
                           {
                               Name = $"UNIT_TEST-Field{Generator.Next(int.MaxValue)}",
                               Description = "Mah Field Description"
                           };

                target.Save(expected);
            }

            // New context again
            using (var target = new DatabaseRepositoryProvider(databaseFactory, this.entityMapper))
            {
                var actual = target.GetFirstOrDefault(Select.From<FieldRow>().WhereEqual(row => row.FieldId, expected.FieldId));
                Assert.IsNotNull(actual);
                Assert.AreEqual(expected, actual);
                Assert.AreEqual(expected.FieldId, actual.FieldId);
            }
        }

        /// <summary>
        /// The get first or default test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void GetFirstOrDefault_ExistingDomainAggregate_ExpectedPropertiesAreNull()
        {
            var databaseFactory = new DefaultDatabaseFactory(
                ConfigurationRoot.GetConnectionString("OrmTestDb"),
                "System.Data.SqlClient",
                new DataAnnotationsDefinitionProvider());

            DomainAggregateRow expected;
            using (var target = new DatabaseRepositoryProvider(databaseFactory, this.entityMapper))
            {
                var topContainer2 = new TopContainerRow
                                        {
                                            Name = $"UNIT_TEST:TopContainer2-{Generator.Next(int.MaxValue)}"
                                        };

                target.Save(topContainer2);

                var subContainerA = new SubContainerRow
                                        {
                                            Name = $"UNIT_TEST:SubContainerA-{Generator.Next(int.MaxValue)}",
                                            TopContainer = topContainer2,
                                            TopContainerId = topContainer2.TopContainerId
                                        };

                target.Save(subContainerA);

                var categoryAttribute20 = new CategoryAttributeRow
                                              {
                                                  Name = $"UNIT_TEST:CatAttr20-{Generator.Next(int.MaxValue)}",
                                                  IsActive = true,
                                                  IsSystem = false
                                              };

                target.Save(categoryAttribute20);

                var timBobIdentity = new DomainIdentityRow
                                         {
                                             FirstName = "Tim",
                                             LastName = "Bob",
                                             UniqueIdentifier = $"UNIT_TEST:timbob@unittest.com-{Generator.Next(int.MaxValue)}"
                                         };

                target.Save(timBobIdentity);

                var fooBarIdentity = new DomainIdentityRow
                                         {
                                             FirstName = "Foo",
                                             LastName = "Bar",
                                             UniqueIdentifier = $"UNIT_TEST:foobar@unittest.com-{Generator.Next(int.MaxValue)}"
                                         };

                target.Save(fooBarIdentity);

                var otherAggregate10 = new OtherAggregateRow
                                           {
                                               Name = $"UNIT_TEST:OtherAggregate10-{Generator.Next(int.MaxValue)}",
                                               AggregateOptionTypeId = 3
                                           };

                target.Save(otherAggregate10);

                var template23 = new TemplateRow
                                     {
                                         Name = $"UNIT_TEST:Template23-{Generator.Next(int.MaxValue)}"
                                     };

                target.Save(template23);

                expected = new DomainAggregateRow
                               {
                                   CategoryAttribute = categoryAttribute20,
                                   CategoryAttributeId = categoryAttribute20.CategoryAttributeId,
                                   Name = $"UNIT_TEST:Aggregate3-{Generator.Next(int.MaxValue)}",
                                   Description = "My Third Domain Aggregate",
                                   CreatedBy = timBobIdentity,
                                   CreatedByDomainIdentityId = timBobIdentity.DomainIdentityId,
                                   CreatedTime = DateTimeOffset.Now.AddMonths(-1),
                                   LastModifiedBy = timBobIdentity,
                                   LastModifiedByDomainIdentityId = timBobIdentity.DomainIdentityId,
                                   LastModifiedTime = DateTimeOffset.Now,
                                   SubContainer = subContainerA,
                                   SubContainerId = subContainerA.SubContainerId,
                                   Template = template23,
                                   TemplateId = template23.TemplateId
                               };

                target.Save(expected);

                ////var aggregateOption1 = new AggregateOptionRow
                ////                           {
                ////                               AggregateOptionId = expected.DomainAggregateId,
                ////                               Name = $"UNIT_TEST:AgOption1-{Generator.Next(int.MaxValue)}",
                ////                               AggregateOptionTypeId = 2,
                ////                               Value = 439034.0332m
                ////                           };

                ////target.Save(aggregateOption1);
            }

            using (var target = new DatabaseRepositoryProvider(databaseFactory, this.entityMapper))
            {
                var itemSelection = Select.From<DomainAggregateRow>()
                    .WhereEqual(row => row.DomainAggregateId, expected.DomainAggregateId)
                    .LeftJoin<AssociationRow>(row => row.DomainAggregateId, row => row.DomainAggregateId)
                    .LeftJoin<AssociationRow, OtherAggregateRow>(row => row.OtherAggregateId, row => row.OtherAggregateId)
                    .InnerJoin(row => row.CategoryAttributeId, row => row.CategoryAttribute.CategoryAttributeId)
                    .InnerJoin(row => row.CreatedByDomainIdentityId, row => row.CreatedBy.DomainIdentityId)
                    .LeftJoin(row => row.DomainAggregateId, row => row.AggregateOption.AggregateOptionId)
                    .InnerJoin(row => row.LastModifiedByDomainIdentityId, row => row.LastModifiedBy.DomainIdentityId)
                    .InnerJoin(row => row.SubContainerId, row => row.SubContainer.SubContainerId)
                    .InnerJoin(row => row.SubContainer.TopContainerId, row => row.SubContainer.TopContainer.TopContainerId)
                    .InnerJoin(row => row.TemplateId, row => row.Template.TemplateId);

                var actual = target.GetFirstOrDefault(itemSelection);

                Assert.IsNotNull(actual);
                Assert.IsNull(actual.AggregateOption);
                Assert.IsNull(actual.OtherAggregate);
            }
        }

        /// <summary>
        /// The get first or default test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void GetFirstOrDefault_NonExistentField_ReturnsNull()
        {
            var databaseFactory = new DefaultDatabaseFactory(
                ConfigurationRoot.GetConnectionString("OrmTestDb"),
                "System.Data.SqlClient",
                new DataAnnotationsDefinitionProvider());

            using (var target = new DatabaseRepositoryProvider(databaseFactory, this.entityMapper))
            {
                var actual = target.GetFirstOrDefault(Select.From<FieldRow>().WhereEqual(row => row.FieldId, -13));
                Assert.IsNull(actual);
            }
        }

        /// <summary>
        /// The contains test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void Contains_ExistingField_ReturnsTrue()
        {
            FieldRow expected;

            var databaseFactory = new DefaultDatabaseFactory(
                ConfigurationRoot.GetConnectionString("OrmTestDb"),
                "System.Data.SqlClient",
                new DataAnnotationsDefinitionProvider());

            using (var target = new DatabaseRepositoryProvider(databaseFactory, this.entityMapper))
            {
                expected = new FieldRow
                               {
                                   Name = $"UNIT_TEST-Field{Generator.Next(int.MaxValue)}",
                                   Description = "Mah Field Description"
                               };

                target.Save(expected);
            }

            // New context again
            using (var target = new DatabaseRepositoryProvider(databaseFactory, this.entityMapper))
            {
                var actual = target.Contains(Select.From<FieldRow>().WhereEqual(row => row.FieldId, expected.FieldId));
                Assert.IsTrue(actual);
            }
        }

        /// <summary>
        /// The contains test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void Contains_NonExistentField_ReturnsFalse()
        {
            var databaseFactory = new DefaultDatabaseFactory(
                ConfigurationRoot.GetConnectionString("OrmTestDb"),
                "System.Data.SqlClient",
                new DataAnnotationsDefinitionProvider());

            using (var target = new DatabaseRepositoryProvider(databaseFactory, this.entityMapper))
            {
                var actual = target.Contains(Select.From<FieldRow>().WhereEqual(row => row.FieldId, -13));
                Assert.IsFalse(actual);
            }
        }

        /// <summary>
        /// The delete items test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void DeleteItems_ExistingField_ItemDeleted()
        {
            FieldRow expected;

            var databaseFactory = new DefaultDatabaseFactory(
                ConfigurationRoot.GetConnectionString("OrmTestDb"),
                "System.Data.SqlClient",
                new DataAnnotationsDefinitionProvider());

            using (var target = new DatabaseRepositoryProvider(databaseFactory, this.entityMapper))
            {
                expected = new FieldRow
                               {
                                   Name = $"UNIT_TEST-Field{Generator.Next(int.MaxValue)}",
                                   Description = "Mah Field Description"
                               };

                target.Save(expected);
            }

            Assert.AreNotEqual(0, expected.FieldId);

            // New context again
            using (var target = new DatabaseRepositoryProvider(databaseFactory, this.entityMapper))
            {
                target.DeleteItems(Select.From<FieldRow>().WhereEqual(row => row.FieldId, expected.FieldId));

                var actual = target.Contains(Select.From<FieldRow>().WhereEqual(row => row.FieldId, expected.FieldId));
                Assert.IsFalse(actual);
            }
        }

        /// <summary>
        /// The delete items test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void DeleteItems_ExistingSetOfFields_ItemsDeleted()
        {
            var description = $"Mah Field Description {nameof(this.DeleteItems_ExistingSetOfFields_ItemsDeleted)}";

            var databaseFactory = new DefaultDatabaseFactory(
                ConfigurationRoot.GetConnectionString("OrmTestDb"),
                "System.Data.SqlClient",
                new DataAnnotationsDefinitionProvider());

            using (var target = new DatabaseRepositoryProvider(databaseFactory, this.entityMapper))
            {
                var field1 = new FieldRow
                                 {
                                     Name = $"UNIT_TEST-Field{Generator.Next(int.MaxValue)}",
                                     Description = description
                                 };

                target.Save(field1);

                var field2 = new FieldRow
                                 {
                                     Name = $"UNIT_TEST-Field{Generator.Next(int.MaxValue)}",
                                     Description = description
                                 };

                target.Save(field2);

                var field3 = new FieldRow
                                 {
                                     Name = $"UNIT_TEST-Field{Generator.Next(int.MaxValue)}",
                                     Description = description
                                 };

                target.Save(field3);
            }

            // New context again
            using (var target = new DatabaseRepositoryProvider(databaseFactory, this.entityMapper))
            {
                var itemSelection = Select.From<FieldRow>().WhereEqual(row => row.Description, description);
                var affected = target.DeleteItems(itemSelection);

                Assert.AreEqual(3, affected);

                var actual = target.GetSelection(itemSelection);
                Assert.AreEqual(0, actual.Count());
            }
        }

        /// <summary>
        /// The insert item test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void InsertItem_NewField_MatchesExpected()
        {
            var databaseFactory = new DefaultDatabaseFactory(
                ConfigurationRoot.GetConnectionString("OrmTestDb"),
                "System.Data.SqlClient",
                new DataAnnotationsDefinitionProvider());

            using (var target = new DatabaseRepositoryProvider(databaseFactory, this.entityMapper))
            {
                target.StartTransaction();

                try
                {
                    var item = new FieldRow { Name = "MahField", Description = "Mah Field Description" };
                    var actual = target.InsertItem(item);
                    Assert.AreNotEqual(0, actual.FieldId);
                }
                finally
                {
                    target.AbortTransaction();
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
            FieldRow item;

            var databaseFactory = new DefaultDatabaseFactory(
                ConfigurationRoot.GetConnectionString("OrmTestDb"),
                "System.Data.SqlClient",
                new DataAnnotationsDefinitionProvider());

            using (var target = new DatabaseRepositoryProvider(databaseFactory, this.entityMapper))
            {
                item = new FieldRow
                           {
                               Name = $"UNIT_TEST-Field{Generator.Next(int.MaxValue)}",
                               Description = "Mah Field Description"
                           };

                target.Save(item);
            }

            // Completely new context to test that caching is not involved.
            FieldRow expected;
            using (var target = new DatabaseRepositoryProvider(databaseFactory, this.entityMapper))
            {
                expected = new FieldRow
                               {
                                   FieldId = item.FieldId,
                                   Name = item.Name,
                                   Description = "Mah Field Description The Second of That Name"
                               };

                target.UpdateItem(expected);
            }

            // New context again
            using (var target = new DatabaseRepositoryProvider(databaseFactory, this.entityMapper))
            {
                var actual = target.GetFirstOrDefault(Select.From<FieldRow>().WhereEqual(row => row.FieldId, item.FieldId));
                Assert.IsNotNull(actual);
                Assert.AreEqual(expected, actual);
                Assert.AreEqual(expected.FieldId, actual.FieldId);
            }
        }

        /// <summary>
        /// Deletes unit test items.
        /// </summary>
        [TestCleanup]
        public void DeleteUnitTestItems()
        {
            var databaseFactory = new DefaultDatabaseFactory(
                ConfigurationRoot.GetConnectionString("OrmTestDb"),
                "System.Data.SqlClient",
                new DataAnnotationsDefinitionProvider());

            using (var provider = new DatabaseRepositoryProvider(databaseFactory, this.entityMapper))
            {
                // Delete the attachment documents based on finding their versions.
                provider.DeleteItems(Select.From<AggregateOptionRow>().WhereEqual(row => row.Name, "UNIT_TEST:%"));
                provider.DeleteItems(Select.From<OtherAggregateRow>().WhereEqual(row => row.Name, "UNIT_TEST:%"));
                provider.DeleteItems(Select.From<DomainAggregateRow>().WhereEqual(row => row.Name, "UNIT_TEST:%"));
                provider.DeleteItems(Select.From<SubContainerRow>().WhereEqual(row => row.Name, "UNIT_TEST:%"));
                provider.DeleteItems(Select.From<TopContainerRow>().WhereEqual(row => row.Name, "UNIT_TEST:%"));
                provider.DeleteItems(Select.From<CategoryAttributeRow>().WhereEqual(row => row.Name, "UNIT_TEST:%"));
                provider.DeleteItems(Select.From<TemplateRow>().WhereEqual(row => row.Name, "UNIT_TEST:%"));
                provider.DeleteItems(Select.From<DomainIdentityRow>().WhereEqual(row => row.UniqueIdentifier, "UNIT_TEST:%"));
            }
        }
    }
}