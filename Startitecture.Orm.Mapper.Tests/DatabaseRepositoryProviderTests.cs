// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseRepositoryProviderTests.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Mapper.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Schema;
    using Startitecture.Orm.SqlClient;
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
        /// The configuration root.
        /// </summary>
        private static IConfigurationRoot ConfigurationRoot => new ConfigurationBuilder().AddJsonFile("appSettings.json", false).Build();

        /// <summary>
        /// The change database test.
        /// </summary>
        [TestMethod]
        public void ChangeDatabase_DatabaseRepositoryProvider_DatabaseChanged()
        {
            var mockDatabaseFactory = new Mock<IDatabaseFactory>();
            var mockDatabaseContext = new Mock<IDatabaseContext>();
            mockDatabaseFactory.Setup(factory => factory.Create()).Returns(mockDatabaseContext.Object);

            var mockConnection = new Mock<IDbConnection>();
            mockDatabaseContext.Setup(context => context.Connection).Returns(mockConnection.Object);

            mockConnection.Setup(connection => connection.ChangeDatabase(It.IsAny<string>()))
                .Callback(
                    (string s) =>
                        {
                            mockConnection.Setup(connection => connection.Database).Returns(s);
                        });

            var databaseFactory = mockDatabaseFactory.Object;
            var repositoryAdapterFactory = new Mock<IRepositoryAdapterFactory>().Object;

            using (var target = new DatabaseRepositoryProvider(databaseFactory, repositoryAdapterFactory))
            {
                target.DatabaseContext.Connection.ChangeDatabase("newDatabase");
                Assert.AreEqual("newDatabase", target.DatabaseContext.Connection.Database);
            }
        }

        /// <summary>
        /// The start transaction test.
        /// </summary>
        [TestMethod]
        public void StartTransaction_TransactionWithIsolationLevel_ReturnsTransactionWithExpectedIsolationLevel()
        {
            var mockDatabaseFactory = new Mock<IDatabaseFactory>();
            var mockDatabaseContext = new Mock<IDatabaseContext>();
            mockDatabaseFactory.Setup(factory => factory.Create()).Returns(mockDatabaseContext.Object);

            var mockConnection = new Mock<IDbConnection>();
            mockDatabaseContext.Setup(context => context.Connection).Returns(mockConnection.Object);

            mockConnection.Setup(connection => connection.BeginTransaction(It.IsAny<IsolationLevel>()))
                .Returns((IsolationLevel i) =>
                    {
                        var transaction = new Mock<IDbTransaction>();
                        transaction.Setup(dbTransaction => dbTransaction.IsolationLevel).Returns(i);
                        return transaction.Object;
                    });

            var databaseFactory = mockDatabaseFactory.Object;
            var repositoryAdapterFactory = new Mock<IRepositoryAdapterFactory>().Object;

            using (var target = new DatabaseRepositoryProvider(databaseFactory, repositoryAdapterFactory))
            {
                var actual = target.StartTransaction(IsolationLevel.Serializable);
                Assert.AreEqual(IsolationLevel.Serializable, actual.IsolationLevel);
            }
        }

        /////// <summary>
        /////// The save test.
        /////// </summary>
        ////[TestMethod]
        ////[TestCategory("Integration")]
        ////public void Save_NewField_IdSet()
        ////{
        ////    var providerFactory = new SqlClientProviderFactory(ConfigurationRoot.GetConnectionString("OrmTestDb"), new DataAnnotationsDefinitionProvider());

        ////    using (var target = providerFactory.Create())
        ////    {
        ////        target.StartTransaction();

        ////        try
        ////        {
        ////            var item = new FieldRow { Name = "MahField", Description = "Mah Field Description" };
        ////            var actual = target.Save(item);
        ////            Assert.AreNotEqual(0, actual.FieldId);
        ////        }
        ////        finally
        ////        {
        ////            target.AbortTransaction();
        ////        }
        ////    }
        ////}

        /////// <summary>
        /////// The save test.
        /////// </summary>
        ////[TestMethod]
        ////[TestCategory("Integration")]
        ////public void Save_ExistingField_ChangesMatchExpected()
        ////{
        ////    FieldRow item;

        ////    var providerFactory = new SqlClientProviderFactory(ConfigurationRoot.GetConnectionString("OrmTestDb"), new DataAnnotationsDefinitionProvider());

        ////    using (var target = providerFactory.Create())
        ////    {
        ////        item = new FieldRow
        ////        {
        ////            Name = $"UNIT_TEST-Field{Generator.Next(int.MaxValue)}",
        ////            Description = "Mah Field Description"
        ////        };

        ////        target.Save(item);
        ////    }

        ////    // Completely new context to test that caching is not involved.
        ////    FieldRow expected;
        ////    using (var target = providerFactory.Create())
        ////    {
        ////        expected = new FieldRow
        ////        {
        ////            FieldId = item.FieldId,
        ////            Name = item.Name,
        ////            Description = "Mah Field Description The Second of That Name"
        ////        };

        ////        target.Save(expected);
        ////    }

        ////    // New context again
        ////    using (var target = providerFactory.Create())
        ////    {
        ////        var actual = target.GetFirstOrDefault(Select.From<FieldRow>().WhereEqual(row => row.FieldId, item.FieldId));
        ////        Assert.IsNotNull(actual);
        ////        Assert.AreEqual(expected, actual);
        ////        Assert.AreEqual(expected.FieldId, actual.FieldId);
        ////    }
        ////}

        /// <summary>
        /// The get selection test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void GetSelection_ExistingDomainAggregates_MatchesExpected()
        {
            List<DomainAggregateRow> expected;

            var providerFactory = new SqlClientProviderFactory(ConfigurationRoot.GetConnectionString("OrmTestDb"), new DataAnnotationsDefinitionProvider());

            using (var target = providerFactory.Create())
            {
                var topContainer2 = new TopContainerRow
                {
                    Name = $"UNIT_TEST:TopContainer2-{Generator.Next(int.MaxValue)}"
                };

                target.InsertItem(topContainer2);

                var subContainerA = new SubContainerRow
                {
                    Name = $"UNIT_TEST:SubContainerA-{Generator.Next(int.MaxValue)}",
                    TopContainer = topContainer2,
                    TopContainerId = topContainer2.TopContainerId
                };

                target.InsertItem(subContainerA);

                var categoryAttribute20 = new CategoryAttributeRow
                {
                    Name = $"UNIT_TEST:CatAttr20-{Generator.Next(int.MaxValue)}",
                    IsActive = true,
                    IsSystem = false
                };

                target.InsertItem(categoryAttribute20);

                var timBobIdentity = new DomainIdentityRow
                {
                    FirstName = "Tim",
                    LastName = "Bob",
                    UniqueIdentifier = $"UNIT_TEST:timbob@unittest.com-{Generator.Next(int.MaxValue)}"
                };

                target.InsertItem(timBobIdentity);

                var fooBarIdentity = new DomainIdentityRow
                {
                    FirstName = "Foo",
                    LastName = "Bar",
                    UniqueIdentifier = $"UNIT_TEST:foobar@unittest.com-{Generator.Next(int.MaxValue)}"
                };

                target.InsertItem(fooBarIdentity);

                var otherAggregate10 = new OtherAggregateRow
                {
                    Name = $"UNIT_TEST:OtherAggregate10-{Generator.Next(int.MaxValue)}",
                    AggregateOptionTypeId = 3
                };

                target.InsertItem(otherAggregate10);

                var template23 = new TemplateRow
                {
                    Name = $"UNIT_TEST:Template23-{Generator.Next(int.MaxValue)}"
                };

                target.InsertItem(template23);

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

                target.InsertItem(domainAggregate1);

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

                target.InsertItem(domainAggregate2);

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

                target.InsertItem(domainAggregate3);

                aggregateOption1.AggregateOptionId = domainAggregate1.DomainAggregateId;
                target.InsertItem(aggregateOption1);

                aggregateOption2.AggregateOptionId = domainAggregate2.DomainAggregateId;
                target.InsertItem(aggregateOption2);

                var associationRow = new AssociationRow
                {
                    DomainAggregateId = domainAggregate2.DomainAggregateId,
                    OtherAggregateId = otherAggregate10.OtherAggregateId
                };

                target.InsertItem(associationRow);

                expected = new List<DomainAggregateRow>
                               {
                                   domainAggregate1,
                                   domainAggregate2,
                                   domainAggregate3
                               };
            }

            using (var target = providerFactory.Create())
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

            var providerFactory = new SqlClientProviderFactory(ConfigurationRoot.GetConnectionString("OrmTestDb"), new DataAnnotationsDefinitionProvider());

            using (var target = providerFactory.Create())
            {
                expected = new FieldRow
                {
                    Name = $"UNIT_TEST-Field{Generator.Next(int.MaxValue)}",
                    Description = "Mah Field Description"
                };

                target.InsertItem(expected);
            }

            // New context again
            using (var target = providerFactory.Create())
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
            var providerFactory = new SqlClientProviderFactory(ConfigurationRoot.GetConnectionString("OrmTestDb"), new DataAnnotationsDefinitionProvider());

            DomainAggregateRow expected;

            using (var target = providerFactory.Create())
            {
                var topContainer2 = new TopContainerRow
                {
                    Name = $"UNIT_TEST:TopContainer2-{Generator.Next(int.MaxValue)}"
                };

                target.InsertItem(topContainer2);

                var subContainerA = new SubContainerRow
                {
                    Name = $"UNIT_TEST:SubContainerA-{Generator.Next(int.MaxValue)}",
                    TopContainer = topContainer2,
                    TopContainerId = topContainer2.TopContainerId
                };

                target.InsertItem(subContainerA);

                var categoryAttribute20 = new CategoryAttributeRow
                {
                    Name = $"UNIT_TEST:CatAttr20-{Generator.Next(int.MaxValue)}",
                    IsActive = true,
                    IsSystem = false
                };

                target.InsertItem(categoryAttribute20);

                var timBobIdentity = new DomainIdentityRow
                {
                    FirstName = "Tim",
                    LastName = "Bob",
                    UniqueIdentifier = $"UNIT_TEST:timbob@unittest.com-{Generator.Next(int.MaxValue)}"
                };

                target.InsertItem(timBobIdentity);

                var fooBarIdentity = new DomainIdentityRow
                {
                    FirstName = "Foo",
                    LastName = "Bar",
                    UniqueIdentifier = $"UNIT_TEST:foobar@unittest.com-{Generator.Next(int.MaxValue)}"
                };

                target.InsertItem(fooBarIdentity);

                var otherAggregate10 = new OtherAggregateRow
                {
                    Name = $"UNIT_TEST:OtherAggregate10-{Generator.Next(int.MaxValue)}",
                    AggregateOptionTypeId = 3
                };

                target.InsertItem(otherAggregate10);

                var template23 = new TemplateRow
                {
                    Name = $"UNIT_TEST:Template23-{Generator.Next(int.MaxValue)}"
                };

                target.InsertItem(template23);

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

                target.InsertItem(expected);
            }

            using (var target = providerFactory.Create())
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
            var providerFactory = new SqlClientProviderFactory(ConfigurationRoot.GetConnectionString("OrmTestDb"), new DataAnnotationsDefinitionProvider());

            using (var target = providerFactory.Create())
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

            var providerFactory = new SqlClientProviderFactory(ConfigurationRoot.GetConnectionString("OrmTestDb"), new DataAnnotationsDefinitionProvider());

            using (var target = providerFactory.Create())
            {
                expected = new FieldRow
                {
                    Name = $"UNIT_TEST-Field{Generator.Next(int.MaxValue)}",
                    Description = "Mah Field Description"
                };

                target.InsertItem(expected);
            }

            // New context again
            using (var target = providerFactory.Create())
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
            var providerFactory = new SqlClientProviderFactory(ConfigurationRoot.GetConnectionString("OrmTestDb"), new DataAnnotationsDefinitionProvider());

            using (var target = providerFactory.Create())
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

            var providerFactory = new SqlClientProviderFactory(ConfigurationRoot.GetConnectionString("OrmTestDb"), new DataAnnotationsDefinitionProvider());

            using (var target = providerFactory.Create())
            {
                expected = new FieldRow
                {
                    Name = $"UNIT_TEST-Field{Generator.Next(int.MaxValue)}",
                    Description = "Mah Field Description"
                };

                target.InsertItem(expected);
            }

            Assert.AreNotEqual(0, expected.FieldId);

            // New context again
            using (var target = providerFactory.Create())
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

            var providerFactory = new SqlClientProviderFactory(ConfigurationRoot.GetConnectionString("OrmTestDb"), new DataAnnotationsDefinitionProvider());

            using (var target = providerFactory.Create())
            {
                var field1 = new FieldRow
                {
                    Name = $"UNIT_TEST-Field{Generator.Next(int.MaxValue)}",
                    Description = description
                };

                target.InsertItem(field1);

                var field2 = new FieldRow
                {
                    Name = $"UNIT_TEST-Field{Generator.Next(int.MaxValue)}",
                    Description = description
                };

                target.InsertItem(field2);

                var field3 = new FieldRow
                {
                    Name = $"UNIT_TEST-Field{Generator.Next(int.MaxValue)}",
                    Description = description
                };

                target.InsertItem(field3);
            }

            // New context again
            using (var target = providerFactory.Create())
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
            var providerFactory = new SqlClientProviderFactory(ConfigurationRoot.GetConnectionString("OrmTestDb"), new DataAnnotationsDefinitionProvider());

            using (var target = providerFactory.Create())
            {
                var transaction = target.StartTransaction();

                try
                {
                    var item = new FieldRow { Name = "MahField", Description = "Mah Field Description" };
                    var actual = target.InsertItem(item);
                    Assert.AreNotEqual(0, actual.FieldId);
                }
                finally
                {
                    transaction.Rollback();
                }
            }
        }

        /// <summary>
        /// The update test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void Update_ExistingFieldRow_MatchesExpected()
        {
            FieldRow item;

            var providerFactory = new SqlClientProviderFactory(ConfigurationRoot.GetConnectionString("OrmTestDb"), new DataAnnotationsDefinitionProvider());

            using (var target = providerFactory.Create())
            {
                item = new FieldRow
                {
                    Name = $"UNIT_TEST-Field{Generator.Next(int.MaxValue)}",
                    Description = "Mah Field Description"
                };

                target.InsertItem(item);
            }

            // Completely new context to test that caching is not involved.
            FieldRow expected;
            using (var target = providerFactory.Create())
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
            using (var target = providerFactory.Create())
            {
                var actual = target.GetFirstOrDefault(Select.From<FieldRow>().WhereEqual(row => row.FieldId, item.FieldId));
                Assert.IsNotNull(actual);
                Assert.AreEqual(expected, actual);
                Assert.AreEqual(expected.FieldId, actual.FieldId);
            }
        }

        /// <summary>
        /// The update test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void Update_ExistingDomainIdentityRowWithSpecificUpdateAttributes_MatchesExpected()
        {
            DomainIdentityRow item;

            var providerFactory = new SqlClientProviderFactory(ConfigurationRoot.GetConnectionString("OrmTestDb"), new DataAnnotationsDefinitionProvider());

            var uniqueIdentifier = Guid.NewGuid().ToString();
            using (var target = providerFactory.Create())
            {
                item = new DomainIdentityRow
                           {
                               UniqueIdentifier = uniqueIdentifier,
                               FirstName = "First Name",
                               MiddleName = "Middle Name",
                               LastName = "Last Name"
                           };

                target.InsertItem(item);
            }

            // Completely new context to test that caching is not involved.
            DomainIdentityRow expected;

            using (var target = providerFactory.Create())
            {
                expected = new DomainIdentityRow
                               {
                                   DomainIdentityId = item.DomainIdentityId,
                                   UniqueIdentifier = uniqueIdentifier,
                                   FirstName = "New First Name",
                                   MiddleName = "Middle Name Should Not Match",
                                   LastName = "New Last Name"
                               };

                target.UpdateItem(expected, row => row.FirstName, row => row.LastName);
            }

            // New context again
            using (var target = providerFactory.Create())
            {
                var actual = target.GetFirstOrDefault(Select.From<DomainIdentityRow>().WhereEqual(row => row.DomainIdentityId, item.DomainIdentityId));
                Assert.IsNotNull(actual);
                Assert.AreNotEqual(expected.MiddleName, actual.MiddleName);
                Assert.AreEqual(expected.FirstName, actual.FirstName);
                Assert.AreEqual(expected.LastName, actual.LastName);
                Assert.AreEqual(expected.DomainIdentityId, actual.DomainIdentityId);
            }
        }

        /// <summary>
        /// Deletes unit test items.
        /// </summary>
        [TestCleanup]
        public void DeleteUnitTestItems()
        {
            var providerFactory = new SqlClientProviderFactory(ConfigurationRoot.GetConnectionString("OrmTestDb"), new DataAnnotationsDefinitionProvider());

            using (var provider = providerFactory.Create())
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