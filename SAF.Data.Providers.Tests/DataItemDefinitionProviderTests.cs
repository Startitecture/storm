// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataItemDefinitionProviderTests.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Orm.Model;

    /// <summary>
    /// The data item definition provider tests.
    /// </summary>
    [TestClass]
    public class DataItemDefinitionProviderTests
    {
        /// <summary>
        /// The resolve test.
        /// </summary>
        [TestMethod]
        public void Resolve_EntityWithAutoNumberPrimaryKey_KeyIncludedInPrimaryKeys()
        {
            var actual = new DataItemDefinitionProvider().Resolve<EntityRow>();
            Assert.IsNotNull(actual.AutoNumberPrimaryKey);
            var autoNumberPrimaryKey = actual.AutoNumberPrimaryKey.GetValueOrDefault();
            Assert.IsTrue(actual.PrimaryKeyAttributes.Contains(autoNumberPrimaryKey));
        }

        /// <summary>
        /// The resolve test.
        /// </summary>
        [TestMethod]
        public void Resolve_EntityWithAutoNumberPrimaryKey_KeyIncludedInDirectAttributes()
        {
            var actual = new DataItemDefinitionProvider().Resolve<EntityRow>();
            Assert.IsNotNull(actual.AutoNumberPrimaryKey);
            var autoNumberPrimaryKey = actual.AutoNumberPrimaryKey.GetValueOrDefault();
            Assert.IsTrue(actual.DirectAttributes.Contains(autoNumberPrimaryKey));
        }

        /// <summary>
        /// The resolve test.
        /// </summary>
        [TestMethod]
        public void Resolve_EntityWithIdentifyingPrimaryKey_KeyIsNotAutoNumberKey()
        {
            var actual = new DataItemDefinitionProvider().Resolve<ExtendedEntityRow>();
            Assert.IsNull(actual.AutoNumberPrimaryKey);
        }

        /// <summary>
        /// The resolve test.
        /// </summary>
        [TestMethod]
        public void Resolve_EntityWithIdentifyingPrimaryKey_KeyIsContainedInPrimaryKey()
        {
            var actual = new DataItemDefinitionProvider().Resolve<ExtendedEntityRow>();
            Assert.IsNotNull(actual.PrimaryKeyAttributes.FirstOrDefault());
        }

        /// <summary>
        /// The resolve test.
        /// </summary>
        [TestMethod]
        public void Resolve_EntityWithIdentifyingPrimaryKey_KeyIncludedInDirectAttributes()
        {
            var actual = new DataItemDefinitionProvider().Resolve<ExtendedEntityRow>();
            var primaryKey = actual.PrimaryKeyAttributes.FirstOrDefault();
            Assert.IsTrue(actual.DirectAttributes.Contains(primaryKey));
        }

        /// <summary>
        /// The resolve test.
        /// </summary>
        [TestMethod]
        public void Resolve_RaisedEntityWithAutoNumberPrimaryKey_KeyIncludedInPrimaryKeys()
        {
            var actual = new DataItemDefinitionProvider().Resolve<RaisedEntityRow>();
            Assert.IsNotNull(actual.AutoNumberPrimaryKey);
            var autoNumberPrimaryKey = actual.AutoNumberPrimaryKey.GetValueOrDefault();
            Assert.IsTrue(actual.PrimaryKeyAttributes.Contains(autoNumberPrimaryKey));
        }

        /// <summary>
        /// The resolve test.
        /// </summary>
        [TestMethod]
        public void Resolve_RaisedEntityWithAutoNumberPrimaryKey_KeyIncludedInDirectAttributes()
        {
            var actual = new DataItemDefinitionProvider().Resolve<RaisedEntityRow>();
            Assert.IsNotNull(actual.AutoNumberPrimaryKey);
            var autoNumberPrimaryKey = actual.AutoNumberPrimaryKey.GetValueOrDefault();
            Assert.IsTrue(actual.DirectAttributes.Contains(autoNumberPrimaryKey));
        }

        /// <summary>
        /// The resolve test.
        /// </summary>
        [TestMethod]
        public void Resolve_RaisedEntityWithRelations_MatchesExpected()
        {
            var actual = new DataItemDefinitionProvider().Resolve<RaisedEntityRow>();

            Assert.AreEqual(actual.EntityName, "Entity");
            Assert.AreEqual(actual.EntityContainer, "dbo");

            var entityLocation = new EntityLocation(typeof(RaisedEntityRow), actual.EntityContainer, actual.EntityName);
            var raisedEntityPath = new LinkedList<EntityLocation>();
            raisedEntityPath.AddLast(entityLocation);

            var entityIdDefinition = new EntityAttributeDefinition(
                                         raisedEntityPath,
                                         typeof(RaisedEntityRow).GetProperty("EntityId"),
                                         "EntityId",
                                         EntityAttributeTypes.DirectAutoNumberKey);

            var lastModifiedDefinition = new EntityAttributeDefinition(
                                             raisedEntityPath,
                                             typeof(RaisedEntityRow).GetProperty("LastModified"),
                                             "LastModified",
                                             EntityAttributeTypes.DirectAttribute);

            var lastModifiedPersonIdDefinition = new EntityAttributeDefinition(
                                                     raisedEntityPath,
                                                     typeof(RaisedEntityRow).GetProperty("LastModifiedByPersonId"),
                                                     "LastModifiedByPersonId",
                                                     EntityAttributeTypes.DirectAttribute);

            var nameDefinition = new EntityAttributeDefinition(
                                     raisedEntityPath,
                                     typeof(RaisedEntityRow).GetProperty("Name"),
                                     "Name",
                                     EntityAttributeTypes.DirectAttribute);

            var foreignFirstContainerIdDefinition = new EntityAttributeDefinition(
                                                       raisedEntityPath,
                                                       typeof(RaisedEntityRow).GetProperty("FirstContainerId"),
                                                       "FirstContainerId",
                                                       EntityAttributeTypes.DirectAttribute);

            var extendedEntityRelation = new EntityAttributeDefinition(
                                             raisedEntityPath,
                                             typeof(RaisedEntityRow).GetProperty("ExtendedEntity"),
                                             "ExtendedEntity",
                                             EntityAttributeTypes.Relation);

            var extendedEntityLocation = new EntityLocation(typeof(ExtendedEntityRow), "dbo", "ExtendedEntity");
            var extendedEntityPath = new LinkedList<EntityLocation>();
            extendedEntityPath.AddLast(entityLocation);
            extendedEntityPath.AddLast(extendedEntityLocation);

            var extendedEntityIdDefinition = new EntityAttributeDefinition(
                                                 extendedEntityPath,
                                                 typeof(ExtendedEntityRow).GetProperty("ExtendedEntityId"),
                                                 "ExtendedEntityId",
                                                 EntityAttributeTypes.RelatedPrimaryKey,
                                                 string.Concat(extendedEntityLocation.Name, '.', "ExtendedEntityId"));

            var otherStuffDefinition = new EntityAttributeDefinition(
                                           extendedEntityPath,
                                           typeof(ExtendedEntityRow).GetProperty("OtherStuff"),
                                           "OtherStuff",
                                           EntityAttributeTypes.RelatedAttribute,
                                           string.Concat(extendedEntityLocation.Name, '.', "OtherStuff"));

            var lastModifiedByRelation = new EntityAttributeDefinition(
                                             raisedEntityPath,
                                             typeof(RaisedEntityRow).GetProperty("LastModifiedBy"),
                                             "Person",
                                             EntityAttributeTypes.Relation,
                                             "LastModifiedBy");

            var lastModifiedbyLocation = new EntityLocation(typeof(PersonRow), "dbo", "Person", "LastModifiedBy");

            var lastModifiedByPath = new LinkedList<EntityLocation>();
            lastModifiedByPath.AddLast(entityLocation);
            lastModifiedByPath.AddLast(lastModifiedbyLocation);

            var personIdDefinition = new EntityAttributeDefinition(
                                         lastModifiedByPath,
                                         typeof(PersonRow).GetProperty("PersonId"),
                                         "PersonId",
                                         EntityAttributeTypes.RelatedAutoNumberKey,
                                         string.Concat(lastModifiedbyLocation.Alias, '.', "PersonId"));

            var accountNameDefinition = new EntityAttributeDefinition(
                                            lastModifiedByPath,
                                            typeof(PersonRow).GetProperty("AccountName"),
                                            "AccountName",
                                            EntityAttributeTypes.RelatedAttribute,
                                            string.Concat(lastModifiedbyLocation.Alias, '.', "AccountName"));

            var firstNameDefinition = new EntityAttributeDefinition(
                                          lastModifiedByPath,
                                          typeof(PersonRow).GetProperty("FirstName"),
                                          "FirstName",
                                          EntityAttributeTypes.RelatedAttribute,
                                          string.Concat(lastModifiedbyLocation.Alias, '.', "FirstName"));

            var lastNameDefinition = new EntityAttributeDefinition(
                                         lastModifiedByPath,
                                         typeof(PersonRow).GetProperty("LastName"),
                                         "LastName",
                                         EntityAttributeTypes.RelatedAttribute,
                                         string.Concat(lastModifiedbyLocation.Alias, '.', "LastName"));

            var firstContainerRelation = new EntityAttributeDefinition(
                                             raisedEntityPath,
                                             typeof(RaisedEntityRow).GetProperty("FirstContainer"),
                                             "FirstContainer",
                                             EntityAttributeTypes.Relation);

            var firstContainerLocation = new EntityLocation(typeof(FirstContainerRow), "dbo", "FirstContainer");
            var firstContainerPath = new LinkedList<EntityLocation>();
            firstContainerPath.AddLast(entityLocation);
            firstContainerPath.AddLast(firstContainerLocation);

            var firstContainerIdDefinition = new EntityAttributeDefinition(
                                                 firstContainerPath,
                                                 typeof(FirstContainerRow).GetProperty("FirstContainerId"),
                                                 "FirstContainerId",
                                                 EntityAttributeTypes.RelatedAutoNumberKey,
                                                 string.Concat(firstContainerLocation.Name, '.', "FirstContainerId"));

            var foreignTopContainerIdDefinition = new EntityAttributeDefinition(
                                                     firstContainerPath,
                                                     typeof(FirstContainerRow).GetProperty("TopContainerId"),
                                                     "TopContainerId",
                                                     EntityAttributeTypes.RelatedAttribute,
                                                     string.Concat(firstContainerLocation.Name, '.', "TopContainerId"));

            var firstContainerNameDefinition = new EntityAttributeDefinition(
                                                 firstContainerPath,
                                                 typeof(FirstContainerRow).GetProperty("Name"),
                                                 "Name",
                                                 EntityAttributeTypes.RelatedAttribute,
                                                 string.Concat(firstContainerLocation.Name, '.', "Name"));

            var topContainerRelation = new EntityAttributeDefinition(
                                             firstContainerPath,
                                             typeof(FirstContainerRow).GetProperty("TopContainer"),
                                             "TopContainer",
                                             EntityAttributeTypes.Relation);

            var topContainerLocation = new EntityLocation(typeof(TopContainerRow), "dbo", "TopContainer");
            var topContainerPath = new LinkedList<EntityLocation>();
            topContainerPath.AddLast(entityLocation);
            topContainerPath.AddLast(firstContainerLocation);
            topContainerPath.AddLast(topContainerLocation);

            var topContainerIdDefinition = new EntityAttributeDefinition(
                                                 topContainerPath,
                                                 typeof(TopContainerRow).GetProperty("TopContainerId"),
                                                 "TopContainerId",
                                                 EntityAttributeTypes.RelatedAutoNumberKey,
                                                 string.Concat(topContainerLocation.Name, '.', "TopContainerId"));

            var topContainerNameDefinition = new EntityAttributeDefinition(
                                                 topContainerPath,
                                                 typeof(TopContainerRow).GetProperty("Name"),
                                                 "Name",
                                                 EntityAttributeTypes.RelatedAttribute,
                                                 string.Concat(topContainerLocation.Name, '.', "Name"));

            var definitions = new List<EntityAttributeDefinition>
                                  {
                                      entityIdDefinition,
                                      lastModifiedDefinition,
                                      lastModifiedPersonIdDefinition,
                                      nameDefinition,
                                      foreignFirstContainerIdDefinition,
                                      extendedEntityIdDefinition,
                                      extendedEntityRelation,
                                      otherStuffDefinition,
                                      lastModifiedByRelation,
                                      personIdDefinition,
                                      accountNameDefinition,
                                      firstNameDefinition,
                                      lastNameDefinition,
                                      firstContainerRelation,
                                      firstContainerIdDefinition,
                                      foreignTopContainerIdDefinition,
                                      firstContainerNameDefinition,
                                      topContainerRelation,
                                      topContainerIdDefinition,
                                      topContainerNameDefinition
                                  };

            Assert.AreEqual(definitions.Count, actual.AllAttributes.Count());

            foreach (var definition in definitions)
            {
                Assert.IsTrue(actual.AllAttributes.Contains(definition), Convert.ToString(definition));
            }
        }

        /// <summary>
        /// The top container row.
        /// </summary>
        [TableName("[dbo].[TopContainer]")]
        [PrimaryKey("TopContainerId", AutoIncrement = true)]
        [ExplicitColumns]
        private class TopContainerRow
        {
            /// <summary>
            /// Gets or sets the first container id.
            /// </summary>
            [Column]
            public int TopContainerId { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            [Column]
            public string Name { get; set; }
        }

        /// <summary>
        /// The first container row.
        /// </summary>
        [TableName("[dbo].[FirstContainer]")]
        [PrimaryKey("FirstContainerId", AutoIncrement = true)]
        [ExplicitColumns]
        private class FirstContainerRow
        {
            /// <summary>
            /// Gets or sets the first container id.
            /// </summary>
            [Column]
            public int FirstContainerId { get; set; }

            /// <summary>
            /// Gets or sets the top container id.
            /// </summary>
            [Column]
            public int TopContainerId { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            [Column]
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the top container.
            /// </summary>
            [Relation]
            public TopContainerRow TopContainer { get; set; }
        }

        /// <summary>
        /// The entity row.
        /// </summary>
        [TableName("[dbo].[Entity]")]
        [PrimaryKey("EntityId", AutoIncrement = true)]
        [ExplicitColumns]
        private class EntityRow
        {
            /// <summary>
            /// Gets or sets the entity id.
            /// </summary>
            [Column]
            public int EntityId { get; set; }

            /// <summary>
            /// Gets or sets the last modified.
            /// </summary>
            [Column]
            public DateTimeOffset LastModified { get; set; }

            /// <summary>
            /// Gets or sets the last modified person id.
            /// </summary>
            [Column]
            public int LastModifiedByPersonId { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            [Column]
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the last modified account name.
            /// </summary>
            [RelatedEntity(typeof(PersonRow), true)]
            public string LastModifiedAccountName { get; set; }

            /// <summary>
            /// Gets or sets the last modified first name.
            /// </summary>
            [RelatedEntity(typeof(PersonRow), true)]
            public string LastModifiedFirstName { get; set; }

            /// <summary>
            /// Gets or sets the last modified last name.
            /// </summary>
            [RelatedEntity(typeof(PersonRow), true)]
            public string LastModifiedLastName { get; set; }

            /// <summary>
            /// Gets or sets the first container id.
            /// </summary>
            [RelatedEntity(typeof(FirstContainerRow), true)]
            public int FirstContainerId { get; set; }

            /// <summary>
            /// Gets or sets the top container id.
            /// </summary>
            [RelatedEntity(typeof(FirstContainerRow), true)]
            public int TopContainerId { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            [RelatedEntity(typeof(FirstContainerRow), true)]
            public string FirstContainerName { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            [RelatedEntity(typeof(TopContainerRow), true)]
            public string TopContainerName { get; set; }
        }

        /// <summary>
        /// The raised entity row.
        /// </summary>
        [TableName("[dbo].[Entity]")]
        [PrimaryKey("EntityId", AutoIncrement = true)]
        [ExplicitColumns]
        private class RaisedEntityRow
        {
            /// <summary>
            /// Gets or sets the entity id.
            /// </summary>
            [Column]
            public int EntityId { get; set; }

            /// <summary>
            /// Gets or sets the first container id.
            /// </summary>
            [Column]
            public int FirstContainerId { get; set; }

            /// <summary>
            /// Gets or sets the last modified.
            /// </summary>
            [Column]
            public DateTimeOffset LastModified { get; set; }

            /// <summary>
            /// Gets or sets the last modified person id.
            /// </summary>
            [Column]
            public int LastModifiedByPersonId { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            [Column]
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the extended entity.
            /// </summary>
            [Relation]
            public ExtendedEntityRow ExtendedEntity { get; set; }

            /// <summary>
            /// Gets or sets the last modified by.
            /// </summary>
            [Relation]
            public PersonRow LastModifiedBy { get; set; }

            /// <summary>
            /// Gets or sets the first container.
            /// </summary>
            [Relation]
            public FirstContainerRow FirstContainer { get; set; }
        }

        /// <summary>
        /// The entity row.
        /// </summary>
        [TableName("[dbo].[ExtendedEntity]")]
        [PrimaryKey("ExtendedEntityId", AutoIncrement = false)]
        [ExplicitColumns]
        private class ExtendedEntityRow
        {
            /// <summary>
            /// Gets or sets the entity id.
            /// </summary>
            [Column]
            public int ExtendedEntityId { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            [Column]
            public string OtherStuff { get; set; }
        }

        /// <summary>
        /// The person row.
        /// </summary>
        [TableName("[dbo].[Person]")]
        [PrimaryKey("PersonId", AutoIncrement = true)]
        [ExplicitColumns]
        private class PersonRow
        {
            /// <summary>
            /// Gets or sets the person ID.
            /// </summary>
            [Column]
            public int PersonId { get; set; }

            /// <summary>
            /// Gets or sets the account name.
            /// </summary>
            [Column]
            public string AccountName { get; set; }

            /// <summary>
            /// Gets or sets the first name.
            /// </summary>
            [Column]
            public string FirstName { get; set; }

            /// <summary>
            /// Gets or sets the last name.
            /// </summary>
            [Column]
            public string LastName { get; set; }
        }
    }
}