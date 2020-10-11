// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlClientProviderFactoryTests.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.SqlClient.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Core;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Schema;
    using Startitecture.Orm.Testing.Entities;

    /// <summary>
    /// The sql client provider factory tests.
    /// </summary>
    [TestClass]
    public class SqlClientProviderFactoryTests
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
        /// The create test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void Create_SqlClientProvider_SysTablesReturnsResultGreaterThanZero()
        {
            var target = new SqlClientProviderFactory(new DataAnnotationsDefinitionProvider());

            using (var provider = target.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
            {
                var actual = provider.ExecuteScalar<int>("SELECT COUNT(1) FROM sys.tables");
                Assert.AreNotEqual(0, actual);
            }
        }

        /// <summary>
        /// The get selection test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void SelectEntities_ExistingDomainAggregates_MatchesExpected()
        {
            List<DomainAggregateRow> expected;

            var providerFactory = new SqlClientProviderFactory(new DataAnnotationsDefinitionProvider());

            using (var target = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
            {
                var topContainer2 = new TopContainerRow
                {
                    Name = $"UNIT_TEST:TopContainer2-{Generator.Next(int.MaxValue)}"
                };

                target.Insert(topContainer2);

                var subContainerA = new SubContainerRow
                {
                    Name = $"UNIT_TEST:SubContainerA-{Generator.Next(int.MaxValue)}",
                    TopContainer = topContainer2,
                    TopContainerId = topContainer2.TopContainerId
                };

                target.Insert(subContainerA);

                var categoryAttribute20 = new CategoryAttributeRow
                {
                    Name = $"UNIT_TEST:CatAttr20-{Generator.Next(int.MaxValue)}",
                    IsActive = true,
                    IsSystem = false
                };

                target.Insert(categoryAttribute20);

                var timBobIdentity = new DomainIdentityRow
                {
                    FirstName = "Tim",
                    LastName = "Bob",
                    UniqueIdentifier = $"UNIT_TEST:timbob@unittest.com-{Generator.Next(int.MaxValue)}"
                };

                target.Insert(timBobIdentity);

                var fooBarIdentity = new DomainIdentityRow
                {
                    FirstName = "Foo",
                    LastName = "Bar",
                    UniqueIdentifier = $"UNIT_TEST:foobar@unittest.com-{Generator.Next(int.MaxValue)}"
                };

                target.Insert(fooBarIdentity);

                var otherAggregate10 = new OtherAggregateRow
                {
                    Name = $"UNIT_TEST:OtherAggregate10-{Generator.Next(int.MaxValue)}",
                    AggregateOptionTypeId = 3
                };

                target.Insert(otherAggregate10);

                var template23 = new TemplateRow
                {
                    Name = $"UNIT_TEST:Template23-{Generator.Next(int.MaxValue)}"
                };

                target.Insert(template23);

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

                target.Insert(domainAggregate1);

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

                target.Insert(domainAggregate2);

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

                target.Insert(domainAggregate3);

                aggregateOption1.AggregateOptionId = domainAggregate1.DomainAggregateId;
                target.Insert(aggregateOption1);

                aggregateOption2.AggregateOptionId = domainAggregate2.DomainAggregateId;
                target.Insert(aggregateOption2);

                var associationRow = new AssociationRow
                {
                    DomainAggregateId = domainAggregate2.DomainAggregateId,
                    OtherAggregateId = otherAggregate10.OtherAggregateId
                };

                target.Insert(associationRow);

                expected = new List<DomainAggregateRow>
                               {
                                   domainAggregate1,
                                   domainAggregate2,
                                   domainAggregate3
                               };
            }

            using (var target = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
            {
                var itemSelection = Query.Select<DomainAggregateRow>()
                    .From(
                        set => set.LeftJoin<AssociationRow>(row => row.DomainAggregateId, row => row.DomainAggregateId)
                            .LeftJoin<AssociationRow, OtherAggregateRow>(row => row.OtherAggregateId, row => row.OtherAggregateId)
                            .InnerJoin(row => row.CategoryAttributeId, row => row.CategoryAttribute.CategoryAttributeId)
                            .InnerJoin(row => row.CreatedByDomainIdentityId, row => row.CreatedBy.DomainIdentityId)
                            .LeftJoin(row => row.DomainAggregateId, row => row.AggregateOption.AggregateOptionId)
                            .InnerJoin(row => row.LastModifiedByDomainIdentityId, row => row.LastModifiedBy.DomainIdentityId)
                            .InnerJoin(row => row.SubContainerId, row => row.SubContainer.SubContainerId)
                            .InnerJoin(row => row.SubContainer.TopContainerId, row => row.SubContainer.TopContainer.TopContainerId)
                            .InnerJoin(row => row.TemplateId, row => row.Template.TemplateId))
                    .Where(set => set.AreEqual(row => row.SubContainerId, expected.First().SubContainerId));

                var actual = target.SelectEntities(itemSelection).OrderBy(x => x.Name).ToList();
                Assert.AreEqual(
                    expected.First(),
                    actual.First(),
                    string.Join(Environment.NewLine, expected.First().GetDifferences(actual.First())));

                CollectionAssert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        /// The get selection test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void SelectEntities_ExistingDomainAggregatesWithSubQuery_MatchesExpected()
        {
            var flag1 = new FlagAttributeRow
                            {
                                Name = $"UNIT_TEST:Flag1-{Generator.Next(int.MaxValue)}"
                            };

            var flag2 = new FlagAttributeRow
                            {
                                Name = $"UNIT_TEST:Flag2-{Generator.Next(int.MaxValue)}"
                            };

            var flag3 = new FlagAttributeRow
                            {
                                Name = $"UNIT_TEST:Flag3-{Generator.Next(int.MaxValue)}"
                            };

            var providerFactory = new SqlClientProviderFactory(new DataAnnotationsDefinitionProvider());

            using (var target = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
            {
                var topContainer2 = new TopContainerRow
                                        {
                                            Name = $"UNIT_TEST:TopContainer2-{Generator.Next(int.MaxValue)}"
                                        };

                target.Insert(topContainer2);

                var subContainerA = new SubContainerRow
                                        {
                                            Name = $"UNIT_TEST:SubContainerA-{Generator.Next(int.MaxValue)}",
                                            TopContainer = topContainer2,
                                            TopContainerId = topContainer2.TopContainerId
                                        };

                target.Insert(subContainerA);

                var categoryAttribute20 = new CategoryAttributeRow
                                              {
                                                  Name = $"UNIT_TEST:CatAttr20-{Generator.Next(int.MaxValue)}",
                                                  IsActive = true,
                                                  IsSystem = false
                                              };

                target.Insert(categoryAttribute20);

                var timBobIdentity = new DomainIdentityRow
                                         {
                                             FirstName = "Tim",
                                             LastName = "Bob",
                                             UniqueIdentifier = $"UNIT_TEST:timbob@unittest.com-{Generator.Next(int.MaxValue)}"
                                         };

                target.Insert(timBobIdentity);

                var fooBarIdentity = new DomainIdentityRow
                                         {
                                             FirstName = "Foo",
                                             LastName = "Bar",
                                             UniqueIdentifier = $"UNIT_TEST:foobar@unittest.com-{Generator.Next(int.MaxValue)}"
                                         };

                target.Insert(fooBarIdentity);

                var otherAggregate10 = new OtherAggregateRow
                                           {
                                               Name = $"UNIT_TEST:OtherAggregate10-{Generator.Next(int.MaxValue)}",
                                               AggregateOptionTypeId = 3
                                           };

                target.Insert(otherAggregate10);

                var template23 = new TemplateRow
                                     {
                                         Name = $"UNIT_TEST:Template23-{Generator.Next(int.MaxValue)}"
                                     };

                target.Insert(template23);

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

                target.Insert(domainAggregate1);
                target.Insert(domainAggregate2);
                target.Insert(domainAggregate3);

                aggregateOption1.AggregateOptionId = domainAggregate1.DomainAggregateId;
                target.Insert(aggregateOption1);

                aggregateOption2.AggregateOptionId = domainAggregate2.DomainAggregateId;
                target.Insert(aggregateOption2);

                var associationRow = new AssociationRow
                                         {
                                             DomainAggregateId = domainAggregate2.DomainAggregateId,
                                             OtherAggregateId = otherAggregate10.OtherAggregateId
                                         };

                target.Insert(associationRow);

                target.Insert(flag1);
                target.Insert(flag2);
                target.Insert(flag3);

                var flagAssociation1 = new DomainAggregateFlagAttributeRow
                                           {
                                               DomainAggregateId = domainAggregate1.DomainAggregateId,
                                               FlagAttributeId = flag1.FlagAttributeId
                                           };

                // Test our INNER JOIN will bring back a distinct result
                var flagAssociation2 = new DomainAggregateFlagAttributeRow
                                           {
                                               DomainAggregateId = domainAggregate1.DomainAggregateId,
                                               FlagAttributeId = flag2.FlagAttributeId
                                           };

                var flagAssociation3 = new DomainAggregateFlagAttributeRow
                                           {
                                               DomainAggregateId = domainAggregate2.DomainAggregateId,
                                               FlagAttributeId = flag3.FlagAttributeId
                                           };

                var flagAssociation4 = new DomainAggregateFlagAttributeRow
                                           {
                                               DomainAggregateId = domainAggregate3.DomainAggregateId,
                                               FlagAttributeId = flag2.FlagAttributeId
                                           };

                var flagAssociation5 = new DomainAggregateFlagAttributeRow
                                           {
                                               DomainAggregateId = domainAggregate1.DomainAggregateId,
                                               FlagAttributeId = flag3.FlagAttributeId
                                           };

                target.Insert(flagAssociation1);
                target.Insert(flagAssociation2);
                target.Insert(flagAssociation3);
                target.Insert(flagAssociation4);
                target.Insert(flagAssociation5);

                var expected1 = new List<DomainAggregateRow>
                                    {
                                        domainAggregate1,
                                    };

                // Test that we only get a single domain aggregate 1.
                var itemSelection1 = Query.Select<DomainAggregateRow>()
                    .From(
                        set => set.LeftJoin<AssociationRow>(row => row.DomainAggregateId, row => row.DomainAggregateId)
                            .LeftJoin<AssociationRow, OtherAggregateRow>(row => row.OtherAggregateId, row => row.OtherAggregateId)
                            .InnerJoin(row => row.CategoryAttributeId, row => row.CategoryAttribute.CategoryAttributeId)
                            .InnerJoin(row => row.CreatedByDomainIdentityId, row => row.CreatedBy.DomainIdentityId)
                            .LeftJoin(row => row.DomainAggregateId, row => row.AggregateOption.AggregateOptionId)
                            .InnerJoin(row => row.LastModifiedByDomainIdentityId, row => row.LastModifiedBy.DomainIdentityId)
                            .InnerJoin(row => row.SubContainerId, row => row.SubContainer.SubContainerId)
                            .InnerJoin(row => row.SubContainer.TopContainerId, row => row.SubContainer.TopContainer.TopContainerId)
                            .InnerJoin(row => row.TemplateId, row => row.Template.TemplateId))
                    .Where(
                        set => set.AreEqual(row => row.AggregateOption.AggregateOptionId, domainAggregate1.AggregateOption.AggregateOptionId)
                            .ExistsIn(
                                row => row.DomainAggregateId,
                                Query.From<DomainAggregateFlagAttributeRow>(
                                        sub => sub.InnerJoin<FlagAttributeRow>(row => row.FlagAttributeId, row => row.FlagAttributeId))
                                    .Where(sub => sub.Include((FlagAttributeRow flagRow) => flagRow.Name, flag1.Name, flag2.Name)),
                                row => row.DomainAggregateId));

                var actual1 = target.SelectEntities(itemSelection1).OrderBy(x => x.Name).ToList();
                Assert.AreEqual(expected1.First(), actual1.First(), string.Join(Environment.NewLine, expected1.First().GetDifferences(actual1.First())));
                CollectionAssert.AreEqual(expected1, actual1);

                // Test that we get all the aggregates.
                var expected2 = new List<DomainAggregateRow>
                                {
                                    domainAggregate1,
                                    domainAggregate2,
                                    domainAggregate3
                                };

                var itemSelection2 = Query.Select<DomainAggregateRow>()
                    .From(
                        set => set.LeftJoin<AssociationRow>(row => row.DomainAggregateId, row => row.DomainAggregateId)
                            .LeftJoin<AssociationRow, OtherAggregateRow>(row => row.OtherAggregateId, row => row.OtherAggregateId)
                            .InnerJoin(row => row.CategoryAttributeId, row => row.CategoryAttribute.CategoryAttributeId)
                            .InnerJoin(row => row.CreatedByDomainIdentityId, row => row.CreatedBy.DomainIdentityId)
                            .LeftJoin(row => row.DomainAggregateId, row => row.AggregateOption.AggregateOptionId)
                            .InnerJoin(row => row.LastModifiedByDomainIdentityId, row => row.LastModifiedBy.DomainIdentityId)
                            .InnerJoin(row => row.SubContainerId, row => row.SubContainer.SubContainerId)
                            .InnerJoin(row => row.SubContainer.TopContainerId, row => row.SubContainer.TopContainer.TopContainerId)
                            .InnerJoin(row => row.TemplateId, row => row.Template.TemplateId))
                    .Where(
                        set => set.AreEqual(row => row.SubContainerId, domainAggregate1.SubContainerId)
                            .ExistsIn(
                                row => row.DomainAggregateId,
                                Query.From<DomainAggregateFlagAttributeRow>(
                                        sub => sub.InnerJoin<FlagAttributeRow>(row => row.FlagAttributeId, row => row.FlagAttributeId))
                                    .Where(
                                        sub => sub.Include(
                                            (FlagAttributeRow flagRow) => flagRow.Name,
                                            flag1.Name,
                                            flag2.Name,
                                            flag3.Name,
                                            "Foo",
                                            "Bar")),
                                row => row.DomainAggregateId));

                var actual2 = target.SelectEntities(itemSelection2).OrderBy(x => x.Name).ToList();
                Assert.AreEqual(expected2.First(), actual2.First(), string.Join(Environment.NewLine, expected1.First().GetDifferences(actual1.First())));
                CollectionAssert.AreEqual(expected2, actual2);
            }
        }

        /// <summary>
        /// The get selection test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void DynamicSelect_ExistingDomainAggregates_MatchesExpected()
        {
            List<DomainAggregateRow> expected;

            var providerFactory = new SqlClientProviderFactory(new DataAnnotationsDefinitionProvider());

            using (var target = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
            {
                var topContainer2 = new TopContainerRow
                                        {
                                            Name = $"UNIT_TEST:TopContainer2-{Generator.Next(int.MaxValue)}"
                                        };

                target.Insert(topContainer2);

                var subContainerA = new SubContainerRow
                                        {
                                            Name = $"UNIT_TEST:SubContainerA-{Generator.Next(int.MaxValue)}",
                                            TopContainer = topContainer2,
                                            TopContainerId = topContainer2.TopContainerId
                                        };

                target.Insert(subContainerA);

                var categoryAttribute20 = new CategoryAttributeRow
                                              {
                                                  Name = $"UNIT_TEST:CatAttr20-{Generator.Next(int.MaxValue)}",
                                                  IsActive = true,
                                                  IsSystem = false
                                              };

                target.Insert(categoryAttribute20);

                var timBobIdentity = new DomainIdentityRow
                                         {
                                             FirstName = "Tim",
                                             LastName = "Bob",
                                             UniqueIdentifier = $"UNIT_TEST:timbob@unittest.com-{Generator.Next(int.MaxValue)}"
                                         };

                target.Insert(timBobIdentity);

                var fooBarIdentity = new DomainIdentityRow
                                         {
                                             FirstName = "Foo",
                                             LastName = "Bar",
                                             UniqueIdentifier = $"UNIT_TEST:foobar@unittest.com-{Generator.Next(int.MaxValue)}"
                                         };

                target.Insert(fooBarIdentity);

                var otherAggregate10 = new OtherAggregateRow
                                           {
                                               Name = $"UNIT_TEST:OtherAggregate10-{Generator.Next(int.MaxValue)}",
                                               AggregateOptionTypeId = 3
                                           };

                target.Insert(otherAggregate10);

                var template23 = new TemplateRow
                                     {
                                         Name = $"UNIT_TEST:Template23-{Generator.Next(int.MaxValue)}"
                                     };

                target.Insert(template23);

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

                target.Insert(domainAggregate1);

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

                target.Insert(domainAggregate2);

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

                target.Insert(domainAggregate3);

                aggregateOption1.AggregateOptionId = domainAggregate1.DomainAggregateId;
                target.Insert(aggregateOption1);

                aggregateOption2.AggregateOptionId = domainAggregate2.DomainAggregateId;
                target.Insert(aggregateOption2);

                var associationRow = new AssociationRow
                                         {
                                             DomainAggregateId = domainAggregate2.DomainAggregateId,
                                             OtherAggregateId = otherAggregate10.OtherAggregateId
                                         };

                target.Insert(associationRow);

                expected = new List<DomainAggregateRow>
                               {
                                   domainAggregate1,
                                   domainAggregate2,
                                   domainAggregate3
                               };
            }

            using (var target = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
            {
                var itemSelection = Query.SelectEntities<DomainAggregateRow>(
                    select => select
                        .Select(
                            row => row.Name,
                            row => row.CategoryAttribute.IsSystem,
                            row => row.CreatedBy.UniqueIdentifier,
                            row => row.SubContainer.TopContainer.Name)
                        .From(
                            set => set.LeftJoin<AssociationRow>(row => row.DomainAggregateId, row => row.DomainAggregateId)
                                .LeftJoin<AssociationRow, OtherAggregateRow>(row => row.OtherAggregateId, row => row.OtherAggregateId)
                                .InnerJoin(row => row.CategoryAttributeId, row => row.CategoryAttribute.CategoryAttributeId)
                                .InnerJoin(row => row.CreatedByDomainIdentityId, row => row.CreatedBy.DomainIdentityId)
                                .LeftJoin(row => row.DomainAggregateId, row => row.AggregateOption.AggregateOptionId)
                                .InnerJoin(row => row.LastModifiedByDomainIdentityId, row => row.LastModifiedBy.DomainIdentityId)
                                .InnerJoin(row => row.SubContainerId, row => row.SubContainer.SubContainerId)
                                .InnerJoin(row => row.SubContainer.TopContainerId, row => row.SubContainer.TopContainer.TopContainerId)
                                .InnerJoin(row => row.TemplateId, row => row.Template.TemplateId))
                        .Where(set => set.AreEqual(row => row.SubContainerId, expected.First().SubContainerId)));

                var actual = target.DynamicSelect(itemSelection).OrderBy(x => x.Name).ToList();

                foreach (var result in actual)
                {
                    var expectedItem = expected.FirstOrDefault(row => row.Name == result.Name);

                    Assert.IsNotNull(expectedItem);
                    Assert.AreEqual(expectedItem.CategoryAttribute.IsSystem, result.CategoryAttributeIsSystem);
                    Assert.AreEqual(expectedItem.CreatedBy.UniqueIdentifier, result.CreatedByUniqueIdentifier);
                    Assert.AreEqual(expectedItem.SubContainer.TopContainer.Name, result.TopContainerName);
                }
            }
        }

        /// <summary>
        /// The get selection test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void SelectEntities_PagedExistingDomainAggregates_MatchesExpected()
        {
            List<DomainAggregateRow> expectedPage1;
            List<DomainAggregateRow> expectedPage2;

            var providerFactory = new SqlClientProviderFactory(new DataAnnotationsDefinitionProvider());

            using (var target = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
            {
                var topContainer2 = new TopContainerRow
                                        {
                                            Name = $"UNIT_TEST:TopContainer2-{Generator.Next(int.MaxValue)}"
                                        };

                target.Insert(topContainer2);

                var subContainerA = new SubContainerRow
                                        {
                                            Name = $"UNIT_TEST:SubContainerA-{Generator.Next(int.MaxValue)}",
                                            TopContainer = topContainer2,
                                            TopContainerId = topContainer2.TopContainerId
                                        };

                target.Insert(subContainerA);

                var categoryAttribute20 = new CategoryAttributeRow
                                              {
                                                  Name = $"UNIT_TEST:CatAttr20-{Generator.Next(int.MaxValue)}",
                                                  IsActive = true,
                                                  IsSystem = false
                                              };

                target.Insert(categoryAttribute20);

                var timBobIdentity = new DomainIdentityRow
                                         {
                                             FirstName = "Tim",
                                             LastName = "Bob",
                                             UniqueIdentifier = $"UNIT_TEST:timbob@unittest.com-{Generator.Next(int.MaxValue)}"
                                         };

                target.Insert(timBobIdentity);

                var fooBarIdentity = new DomainIdentityRow
                                         {
                                             FirstName = "Foo",
                                             LastName = "Bar",
                                             UniqueIdentifier = $"UNIT_TEST:foobar@unittest.com-{Generator.Next(int.MaxValue)}"
                                         };

                target.Insert(fooBarIdentity);

                var otherAggregate10 = new OtherAggregateRow
                                           {
                                               Name = $"UNIT_TEST:OtherAggregate10-{Generator.Next(int.MaxValue)}",
                                               AggregateOptionTypeId = 3
                                           };

                target.Insert(otherAggregate10);

                var template23 = new TemplateRow
                                     {
                                         Name = $"UNIT_TEST:Template23-{Generator.Next(int.MaxValue)}"
                                     };

                target.Insert(template23);

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

                target.Insert(domainAggregate1);

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

                target.Insert(domainAggregate2);

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

                target.Insert(domainAggregate3);

                var domainAggregate4 = new DomainAggregateRow
                                           {
                                               CategoryAttribute = categoryAttribute20,
                                               CategoryAttributeId = categoryAttribute20.CategoryAttributeId,
                                               Name = $"UNIT_TEST:Aggregate4-{Generator.Next(int.MaxValue)}",
                                               Description = "My Fourth Domain Aggregate",
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

                target.Insert(domainAggregate4);

                var domainAggregate5 = new DomainAggregateRow
                                           {
                                               CategoryAttribute = categoryAttribute20,
                                               CategoryAttributeId = categoryAttribute20.CategoryAttributeId,
                                               Name = $"UNIT_TEST:Aggregate5-{Generator.Next(int.MaxValue)}",
                                               Description = "My Fifth Domain Aggregate",
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

                target.Insert(domainAggregate5);

                aggregateOption1.AggregateOptionId = domainAggregate1.DomainAggregateId;
                target.Insert(aggregateOption1);

                aggregateOption2.AggregateOptionId = domainAggregate2.DomainAggregateId;
                target.Insert(aggregateOption2);

                var associationRow = new AssociationRow
                                         {
                                             DomainAggregateId = domainAggregate2.DomainAggregateId,
                                             OtherAggregateId = otherAggregate10.OtherAggregateId
                                         };

                target.Insert(associationRow);

                expectedPage1 = new List<DomainAggregateRow>
                                    {
                                        domainAggregate1,
                                        domainAggregate2,
                                        domainAggregate3
                                    };

                expectedPage2 = new List<DomainAggregateRow>
                                    {
                                        domainAggregate4,
                                        domainAggregate5
                                    };
            }

            using (var target = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
            {
                var countQuery = Query.SelectEntities<DomainAggregateRow>(
                    select => select.Count(row => row.DomainAggregateId)
                        .From(set => set.InnerJoin(row => row.SubContainerId, row => row.SubContainer.SubContainerId))
                        .Where(set => set.AreEqual(row => row.SubContainerId, expectedPage1.First().SubContainerId)));

                // TODO: How will we label our aggregate columns?
                var count = target.GetScalar<int>(countQuery);

                Assert.AreEqual(5, count);

                var tableExpression = Query.SelectEntities<DomainAggregateRow>(
                    select => select.Select(row => row.DomainAggregateId)
                        .From(
                            set => set.LeftJoin<AssociationRow>(row => row.DomainAggregateId, row => row.DomainAggregateId)
                                .LeftJoin<AssociationRow, OtherAggregateRow>(row => row.OtherAggregateId, row => row.OtherAggregateId)
                                .InnerJoin(row => row.CategoryAttributeId, row => row.CategoryAttribute.CategoryAttributeId)
                                .InnerJoin(row => row.CreatedByDomainIdentityId, row => row.CreatedBy.DomainIdentityId)
                                .LeftJoin(row => row.DomainAggregateId, row => row.AggregateOption.AggregateOptionId)
                                .InnerJoin(row => row.LastModifiedByDomainIdentityId, row => row.LastModifiedBy.DomainIdentityId)
                                .InnerJoin(row => row.SubContainerId, row => row.SubContainer.SubContainerId)
                                .InnerJoin(row => row.SubContainer.TopContainerId, row => row.SubContainer.TopContainer.TopContainerId)
                                .InnerJoin(row => row.TemplateId, row => row.Template.TemplateId))
                        .Where(set => set.AreEqual(row => row.SubContainerId, expectedPage1.First().SubContainerId))
                        .Sort(set => set.OrderBy(row => row.Name))
                        .Seek(subset => subset.Skip(0).Take(3)));

                var selection = Query.With(tableExpression, "pgCte")
                    .ForSelection<DomainAggregateRow>(matches => matches.On(row => row.DomainAggregateId, row => row.DomainAggregateId))
                    .From(
                        set => set.LeftJoin<AssociationRow>(row => row.DomainAggregateId, row => row.DomainAggregateId)
                            .LeftJoin<AssociationRow, OtherAggregateRow>(row => row.OtherAggregateId, row => row.OtherAggregateId)
                            .InnerJoin(row => row.CategoryAttributeId, row => row.CategoryAttribute.CategoryAttributeId)
                            .InnerJoin(row => row.CreatedByDomainIdentityId, row => row.CreatedBy.DomainIdentityId)
                            .LeftJoin(row => row.DomainAggregateId, row => row.AggregateOption.AggregateOptionId)
                            .InnerJoin(row => row.LastModifiedByDomainIdentityId, row => row.LastModifiedBy.DomainIdentityId)
                            .InnerJoin(row => row.SubContainerId, row => row.SubContainer.SubContainerId)
                            .InnerJoin(row => row.SubContainer.TopContainerId, row => row.SubContainer.TopContainer.TopContainerId)
                            .InnerJoin(row => row.TemplateId, row => row.Template.TemplateId))
                    .Where(set => set.AreEqual(row => row.SubContainerId, expectedPage1.First().SubContainerId))
                    .Sort(set => set.OrderBy(row => row.Name));

                var actualPage1 = target.SelectEntities(selection).ToList();
                Assert.AreEqual(
                    expectedPage1.First(),
                    actualPage1.First(),
                    string.Join(Environment.NewLine, expectedPage1.First().GetDifferences(actualPage1.First())));

                CollectionAssert.AreEqual(expectedPage1, actualPage1);

                // Advance the number of rows
                selection.ParentExpression.Expression.Page.SetPage(2);

                var actualPage2 = target.SelectEntities(selection).ToList();
                Assert.AreEqual(
                    expectedPage2.First(),
                    actualPage2.First(),
                    string.Join(Environment.NewLine, expectedPage2.First().GetDifferences(actualPage2.First())));

                CollectionAssert.AreEqual(expectedPage2, actualPage2);
            }
        }

        /// <summary>
        /// The get selection test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void SelectEntities_ExistingDomainAggregatesOrdered_MatchesExpected()
        {
            List<DomainAggregateRow> expected;
            List<DomainAggregateRow> expectedDesc;

            var providerFactory = new SqlClientProviderFactory(new DataAnnotationsDefinitionProvider());

            using (var target = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
            {
                var topContainer2 = new TopContainerRow
                {
                    Name = $"UNIT_TEST:TopContainer2-{Generator.Next(int.MaxValue)}"
                };

                target.Insert(topContainer2);

                var subContainerA = new SubContainerRow
                {
                    Name = $"UNIT_TEST:SubContainerA-{Generator.Next(int.MaxValue)}",
                    TopContainer = topContainer2,
                    TopContainerId = topContainer2.TopContainerId
                };

                target.Insert(subContainerA);

                var categoryAttribute20 = new CategoryAttributeRow
                {
                    Name = $"UNIT_TEST:CatAttr20-{Generator.Next(int.MaxValue)}",
                    IsActive = true,
                    IsSystem = false
                };

                target.Insert(categoryAttribute20);

                var timBobIdentity = new DomainIdentityRow
                {
                    FirstName = "Tim",
                    LastName = "Bob",
                    UniqueIdentifier = $"UNIT_TEST:timbob@unittest.com-{Generator.Next(int.MaxValue)}"
                };

                target.Insert(timBobIdentity);

                var fooBarIdentity = new DomainIdentityRow
                {
                    FirstName = "Foo",
                    LastName = "Bar",
                    UniqueIdentifier = $"UNIT_TEST:foobar@unittest.com-{Generator.Next(int.MaxValue)}"
                };

                target.Insert(fooBarIdentity);

                var otherAggregate10 = new OtherAggregateRow
                {
                    Name = $"UNIT_TEST:OtherAggregate10-{Generator.Next(int.MaxValue)}",
                    AggregateOptionTypeId = 3
                };

                target.Insert(otherAggregate10);

                var template23 = new TemplateRow
                {
                    Name = $"UNIT_TEST:Template23-{Generator.Next(int.MaxValue)}"
                };

                target.Insert(template23);

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
                    Name = $"UNIT_TEST:Aggregate3-{Generator.Next(int.MaxValue)}",
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

                target.Insert(domainAggregate1);

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

                target.Insert(domainAggregate2);

                var domainAggregate3 = new DomainAggregateRow
                {
                    CategoryAttribute = categoryAttribute20,
                    CategoryAttributeId = categoryAttribute20.CategoryAttributeId,
                    Name = $"UNIT_TEST:Aggregate1-{Generator.Next(int.MaxValue)}",
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

                target.Insert(domainAggregate3);

                aggregateOption1.AggregateOptionId = domainAggregate1.DomainAggregateId;
                target.Insert(aggregateOption1);

                aggregateOption2.AggregateOptionId = domainAggregate2.DomainAggregateId;
                target.Insert(aggregateOption2);

                var associationRow = new AssociationRow
                {
                    DomainAggregateId = domainAggregate2.DomainAggregateId,
                    OtherAggregateId = otherAggregate10.OtherAggregateId
                };

                target.Insert(associationRow);

                expected = new List<DomainAggregateRow>
                               {
                                   domainAggregate3,
                                   domainAggregate2,
                                   domainAggregate1
                               };

                expectedDesc = new List<DomainAggregateRow>
                                   {
                                       domainAggregate1,
                                       domainAggregate2,
                                       domainAggregate3
                                   };
            }

            using (var target = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
            {
                var itemSelection = Query.Select<DomainAggregateRow>()
                    .From(
                        set => set.LeftJoin<AssociationRow>(row => row.DomainAggregateId, row => row.DomainAggregateId)
                            .LeftJoin<AssociationRow, OtherAggregateRow>(row => row.OtherAggregateId, row => row.OtherAggregateId)
                            .InnerJoin(row => row.CategoryAttributeId, row => row.CategoryAttribute.CategoryAttributeId)
                            .InnerJoin(row => row.CreatedByDomainIdentityId, row => row.CreatedBy.DomainIdentityId)
                            .LeftJoin(row => row.DomainAggregateId, row => row.AggregateOption.AggregateOptionId)
                            .InnerJoin(row => row.LastModifiedByDomainIdentityId, row => row.LastModifiedBy.DomainIdentityId)
                            .InnerJoin(row => row.SubContainerId, row => row.SubContainer.SubContainerId)
                            .InnerJoin(row => row.SubContainer.TopContainerId, row => row.SubContainer.TopContainer.TopContainerId)
                            .InnerJoin(row => row.TemplateId, row => row.Template.TemplateId))
                    .Where(set => set.AreEqual(row => row.SubContainerId, expected.First().SubContainerId))
                    .Sort(set => set.OrderBy(row => row.Name));

                var actual = target.SelectEntities(itemSelection).ToList();
                Assert.AreEqual(expected.First(), actual.First(), string.Join(Environment.NewLine, expected.First().GetDifferences(actual.First())));

                CollectionAssert.AreEqual(expected, actual);

                var itemSelectionDesc = Query.Select<DomainAggregateRow>()
                    .From(
                        set => set.LeftJoin<AssociationRow>(row => row.DomainAggregateId, row => row.DomainAggregateId)
                            .LeftJoin<AssociationRow, OtherAggregateRow>(row => row.OtherAggregateId, row => row.OtherAggregateId)
                            .InnerJoin(row => row.CategoryAttributeId, row => row.CategoryAttribute.CategoryAttributeId)
                            .InnerJoin(row => row.CreatedByDomainIdentityId, row => row.CreatedBy.DomainIdentityId)
                            .LeftJoin(row => row.DomainAggregateId, row => row.AggregateOption.AggregateOptionId)
                            .InnerJoin(row => row.LastModifiedByDomainIdentityId, row => row.LastModifiedBy.DomainIdentityId)
                            .InnerJoin(row => row.SubContainerId, row => row.SubContainer.SubContainerId)
                            .InnerJoin(row => row.SubContainer.TopContainerId, row => row.SubContainer.TopContainer.TopContainerId)
                            .InnerJoin(row => row.TemplateId, row => row.Template.TemplateId))
                    .Where(set => set.AreEqual(row => row.SubContainerId, expected.First().SubContainerId))
                    .Sort(set => set.OrderByDescending(row => row.Name));

                var actualDesc = target.SelectEntities(itemSelectionDesc).ToList();
                Assert.AreEqual(
                    expectedDesc.First(),
                    actualDesc.First(),
                    string.Join(Environment.NewLine, expectedDesc.First().GetDifferences(actualDesc.First())));

                CollectionAssert.AreEqual(expectedDesc, actualDesc);
            }
        }

        /// <summary>
        /// The get first or default test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void FirstOrDefault_ExistingField_MatchesExpected()
        {
            FieldRow expected;

            var providerFactory = new SqlClientProviderFactory(new DataAnnotationsDefinitionProvider());

            using (var target = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
            {
                expected = new FieldRow
                {
                    Name = $"UNIT_TEST-Field{Generator.Next(int.MaxValue)}",
                    Description = "Mah Field Description"
                };

                target.Insert(expected);
            }

            // New context again
            using (var target = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
            {
                var actual = target.FirstOrDefault(Query.Select<FieldRow>().Where(set => set.AreEqual(row => row.FieldId, expected.FieldId)));
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
        public void FirstOrDefault_ExistingDomainAggregate_ExpectedPropertiesAreNull()
        {
            var providerFactory = new SqlClientProviderFactory(new DataAnnotationsDefinitionProvider());

            DomainAggregateRow expected;

            using (var target = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
            {
                var topContainer2 = new TopContainerRow
                {
                    Name = $"UNIT_TEST:TopContainer2-{Generator.Next(int.MaxValue)}"
                };

                target.Insert(topContainer2);

                var subContainerA = new SubContainerRow
                {
                    Name = $"UNIT_TEST:SubContainerA-{Generator.Next(int.MaxValue)}",
                    TopContainer = topContainer2,
                    TopContainerId = topContainer2.TopContainerId
                };

                target.Insert(subContainerA);

                var categoryAttribute20 = new CategoryAttributeRow
                {
                    Name = $"UNIT_TEST:CatAttr20-{Generator.Next(int.MaxValue)}",
                    IsActive = true,
                    IsSystem = false
                };

                target.Insert(categoryAttribute20);

                var timBobIdentity = new DomainIdentityRow
                {
                    FirstName = "Tim",
                    LastName = "Bob",
                    UniqueIdentifier = $"UNIT_TEST:timbob@unittest.com-{Generator.Next(int.MaxValue)}"
                };

                target.Insert(timBobIdentity);

                var fooBarIdentity = new DomainIdentityRow
                {
                    FirstName = "Foo",
                    LastName = "Bar",
                    UniqueIdentifier = $"UNIT_TEST:foobar@unittest.com-{Generator.Next(int.MaxValue)}"
                };

                target.Insert(fooBarIdentity);

                var otherAggregate10 = new OtherAggregateRow
                {
                    Name = $"UNIT_TEST:OtherAggregate10-{Generator.Next(int.MaxValue)}",
                    AggregateOptionTypeId = 3
                };

                target.Insert(otherAggregate10);

                var template23 = new TemplateRow
                {
                    Name = $"UNIT_TEST:Template23-{Generator.Next(int.MaxValue)}"
                };

                target.Insert(template23);

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

                target.Insert(expected);
            }

            using (var target = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
            {
                var itemSelection = Query.Select<DomainAggregateRow>()
                    .From(
                        set => set.LeftJoin<AssociationRow>(row => row.DomainAggregateId, row => row.DomainAggregateId)
                            .LeftJoin<AssociationRow, OtherAggregateRow>(row => row.OtherAggregateId, row => row.OtherAggregateId)
                            .InnerJoin(row => row.CategoryAttributeId, row => row.CategoryAttribute.CategoryAttributeId)
                            .InnerJoin(row => row.CreatedByDomainIdentityId, row => row.CreatedBy.DomainIdentityId)
                            .LeftJoin(row => row.DomainAggregateId, row => row.AggregateOption.AggregateOptionId)
                            .InnerJoin(row => row.LastModifiedByDomainIdentityId, row => row.LastModifiedBy.DomainIdentityId)
                            .InnerJoin(row => row.SubContainerId, row => row.SubContainer.SubContainerId)
                            .InnerJoin(row => row.SubContainer.TopContainerId, row => row.SubContainer.TopContainer.TopContainerId)
                            .InnerJoin(row => row.TemplateId, row => row.Template.TemplateId))
                    .Where(set => set.AreEqual(row => row.DomainAggregateId, expected.DomainAggregateId));

                var actual = target.FirstOrDefault(itemSelection);

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
        public void FirstOrDefault_NonExistentField_ReturnsNull()
        {
            var providerFactory = new SqlClientProviderFactory(new DataAnnotationsDefinitionProvider());

            using (var target = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
            {
                var actual = target.FirstOrDefault(Query.Select<FieldRow>().Where(set => set.AreEqual(row => row.FieldId, -13)));
                Assert.IsNull(actual);
            }
        }

        /// <summary>
        /// The first or default test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void DynamicFirstOrDefault_DynamicResults_MatchExpected()
        {
            var providerFactory = new SqlClientProviderFactory(new DataAnnotationsDefinitionProvider());

            using (var target = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
            {
                var timBobIdentity = new DomainIdentityRow
                                         {
                                             FirstName = "Tim",
                                             LastName = "Bob",
                                             UniqueIdentifier = $"UNIT_TEST:timbob@unittest.com-{Generator.Next(int.MaxValue)}"
                                         };

                target.Insert(timBobIdentity);

                var entitySelection = Query.SelectEntities<DomainIdentityRow>(
                    select => select.Select(row => row.UniqueIdentifier)
                        .Where(set => set.AreEqual(row => row.DomainIdentityId, timBobIdentity.DomainIdentityId)));

                var actual = target.DynamicFirstOrDefault(entitySelection);
                Assert.AreEqual(timBobIdentity.UniqueIdentifier, actual.UniqueIdentifier);
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

            var providerFactory = new SqlClientProviderFactory(new DataAnnotationsDefinitionProvider());

            using (var target = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
            {
                expected = new FieldRow
                {
                    Name = $"UNIT_TEST-Field{Generator.Next(int.MaxValue)}",
                    Description = "Mah Field Description"
                };

                target.Insert(expected);
            }

            // New context again
            using (var target = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
            {
                var actual = target.Contains(Query.From<FieldRow>().Where(set => set.AreEqual(row => row.FieldId, expected.FieldId)));
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
            var providerFactory = new SqlClientProviderFactory(new DataAnnotationsDefinitionProvider());

            using (var target = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
            {
                var actual = target.Contains(Query.From<FieldRow>().Where(set => set.AreEqual(row => row.FieldId, -13)));
                Assert.IsFalse(actual);
            }
        }

        /// <summary>
        /// The delete entities test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void Delete_ExistingField_ItemDeleted()
        {
            FieldRow expected;

            var providerFactory = new SqlClientProviderFactory(new DataAnnotationsDefinitionProvider());

            using (var target = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
            {
                expected = new FieldRow
                {
                    Name = $"UNIT_TEST-Field{Generator.Next(int.MaxValue)}",
                    Description = "Mah Field Description"
                };

                target.Insert(expected);
            }

            Assert.AreNotEqual(0, expected.FieldId);

            // New context again
            using (var target = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
            {
                target.Delete(Query.Select<FieldRow>().Where(set => set.AreEqual(row => row.FieldId, expected.FieldId)));

                var actual = target.Contains(Query.From<FieldRow>().Where(set => set.AreEqual(row => row.FieldId, expected.FieldId)));
                Assert.IsFalse(actual);
            }
        }

        /// <summary>
        /// The delete entities test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void Delete_ExistingSetOfFields_ItemsDeleted()
        {
            var description = $"Mah Field Description {nameof(this.Delete_ExistingSetOfFields_ItemsDeleted)}";

            var providerFactory = new SqlClientProviderFactory(new DataAnnotationsDefinitionProvider());

            using (var target = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
            {
                var field1 = new FieldRow
                {
                    Name = $"UNIT_TEST-Field{Generator.Next(int.MaxValue)}",
                    Description = description
                };

                target.Insert(field1);

                var field2 = new FieldRow
                {
                    Name = $"UNIT_TEST-Field{Generator.Next(int.MaxValue)}",
                    Description = description
                };

                target.Insert(field2);

                var field3 = new FieldRow
                {
                    Name = $"UNIT_TEST-Field{Generator.Next(int.MaxValue)}",
                    Description = description
                };

                target.Insert(field3);
            }

            // New context again
            using (var target = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
            {
                var itemSelection = Query.Select<FieldRow>().Where(set => set.AreEqual(row => row.Description, description));
                var affected = target.Delete(itemSelection);

                Assert.AreEqual(3, affected);

                var actual = target.SelectEntities(itemSelection);
                Assert.AreEqual(0, actual.Count());
            }
        }

        /// <summary>
        /// The insert entity test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void Insert_NewField_MatchesExpected()
        {
            var providerFactory = new SqlClientProviderFactory(new DataAnnotationsDefinitionProvider());

            using (var target = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
            {
                var transaction = target.StartTransaction();

                try
                {
                    var entity = new FieldRow { Name = "MahField", Description = "Mah Field Description" };
                    var actual = target.Insert(entity);
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

            var providerFactory = new SqlClientProviderFactory(new DataAnnotationsDefinitionProvider());

            using (var target = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
            {
                item = new FieldRow
                {
                    Name = $"UNIT_TEST-Field{Generator.Next(int.MaxValue)}",
                    Description = "Mah Field Description"
                };

                target.Insert(item);
            }

            // Completely new context to test that caching is not involved.
            FieldRow expected;
            using (var target = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
            {
                expected = new FieldRow
                {
                    FieldId = item.FieldId,
                    Name = item.Name,
                    Description = "Mah Field Description The Second of That Name"
                };

                target.UpdateSingle(expected);
            }

            // New context again
            using (var target = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
            {
                var actual = target.FirstOrDefault(Query.Select<FieldRow>().Where(set => set.AreEqual(row => row.FieldId, item.FieldId)));
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

            var providerFactory = new SqlClientProviderFactory(new DataAnnotationsDefinitionProvider());

            var uniqueIdentifier = Guid.NewGuid().ToString();
            using (var target = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
            {
                item = new DomainIdentityRow
                {
                    UniqueIdentifier = uniqueIdentifier,
                    FirstName = "First Name",
                    MiddleName = "Middle Name",
                    LastName = "Last Name"
                };

                target.Insert(item);
            }

            // Completely new context to test that caching is not involved.
            DomainIdentityRow expected;

            using (var target = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
            {
                expected = new DomainIdentityRow
                {
                    DomainIdentityId = item.DomainIdentityId,
                    UniqueIdentifier = uniqueIdentifier,
                    FirstName = "New First Name",
                    MiddleName = "Middle Name Should Not Match",
                    LastName = "New Last Name"
                };

                target.UpdateSingle(expected, row => row.FirstName, row => row.LastName);
            }

            // New context again
            using (var target = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
            {
                var actual = target.FirstOrDefault(
                    Query.Select<DomainIdentityRow>().Where(set => set.AreEqual(row => row.DomainIdentityId, item.DomainIdentityId)));

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
            var providerFactory = new SqlClientProviderFactory(new DataAnnotationsDefinitionProvider());

            using (var provider = providerFactory.Create(ConfigurationRoot.GetConnectionString("OrmTestDb")))
            {
                provider.Delete(Query.From<DomainAggregateFlagAttributeRow>());
                provider.Delete(Query.From<FlagAttributeRow>().Where(set => set.AreEqual(row => row.Name, "UNIT_TEST:%")));
                provider.Delete(Query.Select<AggregateOptionRow>().Where(set => set.AreEqual(row => row.Name, "UNIT_TEST:%")));
                provider.Delete(Query.Select<OtherAggregateRow>().Where(set => set.AreEqual(row => row.Name, "UNIT_TEST:%")));
                provider.Delete(Query.Select<DomainAggregateRow>().Where(set => set.AreEqual(row => row.Name, "UNIT_TEST:%")));
                provider.Delete(Query.Select<SubContainerRow>().Where(set => set.AreEqual(row => row.Name, "UNIT_TEST:%")));
                provider.Delete(Query.Select<TopContainerRow>().Where(set => set.AreEqual(row => row.Name, "UNIT_TEST:%")));
                provider.Delete(Query.Select<CategoryAttributeRow>().Where(set => set.AreEqual(row => row.Name, "UNIT_TEST:%")));
                provider.Delete(Query.Select<TemplateRow>().Where(set => set.AreEqual(row => row.Name, "UNIT_TEST:%")));
                provider.Delete(Query.Select<DomainIdentityRow>().Where(set => set.AreEqual(row => row.UniqueIdentifier, "UNIT_TEST:%")));
            }
        }
    }
}