// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityProxyTest.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.EntityServices.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Rhino.Mocks;

    using SAF.ActionTracking;
    using SAF.Data;
    using SAF.Testing.Common;

    /// <summary>
    /// This is a test class for EntityRepositoryServiceProxyTest and is intended
    /// to contain all EntityRepositoryServiceProxyTest Unit Tests
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class EntityProxyTest
    {
        /// <summary>
        /// The entity mapper.
        /// </summary>
        private readonly AutoMapperEntityMapper entityMapper = new AutoMapperEntityMapper();

        /// <summary>
        /// The action event proxy.
        /// </summary>
        private readonly IActionEventProxy actionEventProxy = CreateActionEventProxy();

        /// <summary>
        /// The initialized.
        /// </summary>
        private bool initialized;

        #region Public Properties

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        #endregion

        /// <summary>
        /// Use ClassInitialize to run code before running the first test in the class.
        /// </summary>
        /// <param name="testContext">
        /// The test context.
        /// </param>
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
        }

        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup()
        // {
        // }

        /// <summary>
        /// Use TestInitialize to run code before running each test.
        /// </summary>
        [TestInitialize]
        public void MyTestInitialize()
        {
            if (this.initialized)
            {
                return;
            }

            this.entityMapper.Initialize(
                configuration =>
                    {
                        configuration.CreateMap<FakeDto, FakeEntity>();
                        configuration.CreateMap<FakeEntity, FakeDataEntity>()
                            .ForMember(
                                dest => dest.FakeEntityId, 
                                expr => expr.MapFrom(source => source.FakeEntityId.GetValueOrDefault()));

                        configuration.CreateMap<FakeDataEntity, FakeEntity>();
                        configuration.CreateMap<FakeEntity, FakeDto>()
                            .ForMember(dest => dest.TransactionProvider, expr => expr.Ignore());

                        configuration.CreateMap<FakeDto, BadMappingDto>();
                    });

            this.initialized = true;
        }

        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup()
        // {
        // }
        #region Public Methods and Operators

        /// <summary>
        /// A test for Remove
        /// </summary>
        [TestMethod]
        public void Remove_IndexSelectionWithPropertySpecification_Returns1()
        {
            const int Expected = 1;
            var repository = MockRepository.GenerateMock<IEntityRepository<FakeEntity>>();
            repository.Stub(entityRepository => entityRepository.DeleteSelection(Arg<IExampleQuery<FakeDto>>.Is.Anything)).Return(Expected);
            var repositoryFactory = MockRepository.GenerateMock<IRepositoryFactory<FakeEntity>>();
            repositoryFactory.Stub(factory => factory.Create(Arg<IRepositoryProvider>.Is.Anything)).Return(repository);
            var repositoryProviderFactory = MockRepository.GenerateMock<IRepositoryProviderFactory>();

            var target = new EntityProxy<FakeEntity>(
                repositoryFactory, 
                repositoryProviderFactory, 
                this.entityMapper, 
                this.actionEventProxy);
            var baseline = new FakeDto { Name = "Name31", DateValue = new DateTime(2010, 5, 22) };
            var query = new FakeQuery(dto => dto.DateValue) { BaselineExample = baseline };
            int actual = target.RemoveSelection(query);
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// A test for Remove
        /// </summary>
        [TestMethod]
        public void Remove_MappedItemSelectionWithPropertySpecification_Returns2()
        {
            const int Expected = 2;
            var repository = MockRepository.GenerateMock<IEntityRepository<FakeEntity>>();
            repository.Stub(entityRepository => entityRepository.DeleteSelection(Arg<IExampleQuery<FakeDto>>.Is.Anything)).Return(Expected);
            var repositoryFactory = MockRepository.GenerateMock<IRepositoryFactory<FakeEntity>>();
            repositoryFactory.Stub(factory => factory.Create(Arg<IRepositoryProvider>.Is.Anything)).Return(repository);
            var repositoryProviderFactory = MockRepository.GenerateMock<IRepositoryProviderFactory>();

            var target = new EntityProxy<FakeEntity>(
                repositoryFactory, 
                repositoryProviderFactory, 
                this.entityMapper, 
                this.actionEventProxy);
            var baseline = new FakeDto { Name = "NameMin", DateValue = DateTime.MinValue };
            var boundary = new FakeDto { Name = "NameMax", DateValue = new DateTime(2014, 10, 13) };
            var query = new FakeQuery(dto => dto.DateValue) { BaselineExample = baseline, BoundaryExample = boundary };

            int actual = target.RemoveSelection(query);
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// A test for Remove
        /// </summary>
        [TestMethod]
        public void Remove_RangeSelectionWithPropertySpecification_Returns2()
        {
            const int Expected = 2;
            var repository = MockRepository.GenerateMock<IEntityRepository<FakeEntity>>();
            repository.Stub(entityRepository => entityRepository.DeleteSelection(Arg<IExampleQuery<FakeDto>>.Is.Anything)).Return(Expected);
            var repositoryFactory = MockRepository.GenerateMock<IRepositoryFactory<FakeEntity>>();
            repositoryFactory.Stub(factory => factory.Create(Arg<IRepositoryProvider>.Is.Anything)).Return(repository);
            var repositoryProviderFactory = MockRepository.GenerateMock<IRepositoryProviderFactory>();

            var target = new EntityProxy<FakeEntity>(
                repositoryFactory, 
                repositoryProviderFactory, 
                this.entityMapper, 
                this.actionEventProxy);
            var baseline = new FakeDto { Name = "NameMin", DateValue = DateTime.MinValue };
            var boundary = new FakeDto { Name = "NameMax", DateValue = new DateTime(2014, 10, 13) };
            var query = new FakeQuery(dto => dto.DateValue) { BaselineExample = baseline, BoundaryExample = boundary };

            int actual = target.RemoveSelection(query);
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// A test for Save
        /// </summary>
        [TestMethod]
        public void Save_FakeEntity_MatchesExpected()
        {
            var expected = new FakeDto
                               {
                                   FakeEntityId = 1, 
                                   Name = "Name", 
                                   Description = "Description", 
                                   DateValue = new DateTime(2011, 5, 22)
                               };

            var entityRepository = MockRepository.GenerateMock<IEntityRepository<FakeEntity>>();
            entityRepository.Stub(repository => repository.SaveAs<FakeDto>(Arg<FakeEntity>.Is.Anything)).Return(expected);
            var repositoryFactory = MockRepository.GenerateMock<IRepositoryFactory<FakeEntity>>();
            repositoryFactory.Stub(factory => factory.Create(Arg<IRepositoryProvider>.Is.Anything)).Return(entityRepository);
            var repositoryProviderFactory = MockRepository.GenerateMock<IRepositoryProviderFactory>();

            var target = new EntityProxy<FakeEntity>(
                repositoryFactory, 
                repositoryProviderFactory, 
                this.entityMapper, 
                this.actionEventProxy);

            var item = new FakeDto
                           {
                               FakeEntityId = 1, 
                               Name = "Name", 
                               Description = "Description", 
                               DateValue = new DateTime(2011, 5, 22)
                           };
            FakeDto actual = target.SaveItem(item);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for Select
        /// </summary>
        [TestMethod]
        public void Select_ExampleSelectionWithDtoAndSpecifiedProperties_MatchesExpected()
        {
            IEnumerable<FakeDto> expected = new List<FakeDto>
                                                {
                                                    new FakeDto
                                                        {
                                                            DateValue = DateTime.MinValue, 
                                                            Name = "Toto", 
                                                            Description = "None entered"
                                                        }, 
                                                    new FakeDto
                                                        {
                                                            DateValue = new DateTime(2010, 10, 1), 
                                                            Name = "The Sundance Kid", 
                                                            Description = "A Western"
                                                        }
                                                };

            var entityRepository = MockRepository.GenerateMock<IEntityRepository<FakeEntity>>();
            entityRepository.Stub(repository => repository.SelectAs(Arg<ExampleQuery<FakeDto>>.Is.Anything)).Return(expected);
            var repositoryFactory = MockRepository.GenerateMock<IRepositoryFactory<FakeEntity>>();
            repositoryFactory.Stub(factory => factory.Create(Arg<IRepositoryProvider>.Is.Anything)).Return(entityRepository);
            var repositoryProviderFactory = MockRepository.GenerateMock<IRepositoryProviderFactory>();

            var target = new EntityProxy<FakeEntity>(
                repositoryFactory, 
                repositoryProviderFactory, 
                this.entityMapper, 
                this.actionEventProxy);

            var baselineExample = new FakeDto { DateValue = DateTime.MinValue, Name = "Toto", Description = "None entered" };
            var boundaryExample = new FakeDto
                                      {
                                          DateValue = new DateTime(2010, 10, 1), 
                                          Name = "The Sundance Kid", 
                                          Description = "A Western"
                                      };
            var query = new FakeQuery(dto => dto.DateValue) { BaselineExample = baselineExample, BoundaryExample = boundaryExample };
            var actual = target.SelectItems(query);
            CollectionAssert.AreEqual(expected.ToList(), actual.ToList());
        }

        /// <summary>
        /// A test for Select
        /// </summary>
        [TestMethod]
        public void Select_KeySelectionWithDtoMissingItem_MatchesNull()
        {
            var key = new FakeDto { FakeEntityId = 1 };

            var entityRepository = MockRepository.GenerateMock<IEntityRepository<FakeEntity>>();
            entityRepository.Stub(repository => repository.FirstOrDefaultAs(Arg<FakeDto>.Is.Anything)).Return(key);
            var repositoryFactory = MockRepository.GenerateMock<IRepositoryFactory<FakeEntity>>();
            repositoryFactory.Stub(factory => factory.Create(Arg<IRepositoryProvider>.Is.Anything)).Return(entityRepository);
            var repositoryProviderFactory = MockRepository.GenerateMock<IRepositoryProviderFactory>();

            var target = new EntityProxy<FakeEntity>(
                repositoryFactory, 
                repositoryProviderFactory, 
                this.entityMapper, 
                this.actionEventProxy);

            FakeEntity actual = target.SelectEntity(key);
            Assert.AreEqual(null, actual);
        }

        /// <summary>
        /// A test for Select
        /// </summary>
        [TestMethod]
        public void Select_SelectAll_MatchesSingleDefaultInstance()
        {
            var expected = new List<FakeDto>();
            var entityRepository = MockRepository.GenerateMock<IEntityRepository<FakeEntity>>();
            entityRepository.Stub(repository => repository.SelectAs(Arg<ExampleQuery<FakeDto>>.Is.Anything)).Return(expected);
            var repositoryFactory = MockRepository.GenerateMock<IRepositoryFactory<FakeEntity>>();
            repositoryFactory.Stub(factory => factory.Create(Arg<IRepositoryProvider>.Is.Anything)).Return(entityRepository);
            var repositoryProviderFactory = MockRepository.GenerateMock<IRepositoryProviderFactory>();

            var target = new EntityProxy<FakeEntity>(
                repositoryFactory, 
                repositoryProviderFactory, 
                this.entityMapper, 
                this.actionEventProxy);
            var actual = target.SelectItems(new FakeQuery());
            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        /// <summary>
        /// The remove items_ baseline query with specified properties_ returns item.
        /// </summary>
        [TestMethod]
        public void RemoveItems_BaselineQueryWithSpecifiedProperties_ReturnsItem()
        {
            const int Expected = 1;

            var entityRepository = MockRepository.GenerateMock<IEntityRepository<FakeEntity>>();
            entityRepository.Stub(repository => repository.DeleteSelection(Arg<ExampleQuery<FakeDto>>.Is.Anything)).Return(Expected);
            var repositoryFactory = MockRepository.GenerateMock<IRepositoryFactory<FakeEntity>>();
            repositoryFactory.Stub(factory => factory.Create(Arg<IRepositoryProvider>.Is.Anything)).Return(entityRepository);
            var repositoryProviderFactory = MockRepository.GenerateMock<IRepositoryProviderFactory>();

            var target = new EntityProxy<FakeEntity>(
                repositoryFactory, 
                repositoryProviderFactory, 
                this.entityMapper, 
                this.actionEventProxy);
            var baselineExample = new FakeDto { Name = "Name21", DateValue = new DateTime(2012, 10, 22) };
            var query = new FakeQuery
                            {
                                BaselineExample = baselineExample, 
                                PropertiesToInclude = new Collection<string> { "DateValue" }
                            };

            int actual = target.RemoveSelection(query);
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The remove items_ baseline query_ returns item.
        /// </summary>
        [TestMethod]
        public void RemoveItems_BaselineQuery_ReturnsItem()
        {
            const int Expected = 1;

            var entityRepository = MockRepository.GenerateMock<IEntityRepository<FakeEntity>>();
            entityRepository.Stub(repository => repository.DeleteSelection(Arg<ExampleQuery<FakeDto>>.Is.Anything)).Return(Expected);
            var repositoryFactory = MockRepository.GenerateMock<IRepositoryFactory<FakeEntity>>();
            repositoryFactory.Stub(factory => factory.Create(Arg<IRepositoryProvider>.Is.Anything)).Return(entityRepository);
            var repositoryProviderFactory = MockRepository.GenerateMock<IRepositoryProviderFactory>();

            var target = new EntityProxy<FakeEntity>(
                repositoryFactory, 
                repositoryProviderFactory, 
                this.entityMapper, 
                this.actionEventProxy);
            var baselineExample = new FakeDto { Name = "TestName", DateValue = new DateTime(2012, 10, 22) };
            var query = new FakeQuery { BaselineExample = baselineExample };
            int actual = target.RemoveSelection(query);
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The remove items_ range query with specified properties_ returns items.
        /// </summary>
        [TestMethod]
        public void RemoveItems_RangeQueryWithSpecifiedProperties_ReturnsItems()
        {
            const int Expected = 2;
            var entityRepository = MockRepository.GenerateMock<IEntityRepository<FakeEntity>>();
            entityRepository.Stub(repository => repository.DeleteSelection(Arg<ExampleQuery<FakeDto>>.Is.Anything)).Return(Expected);
            var repositoryFactory = MockRepository.GenerateMock<IRepositoryFactory<FakeEntity>>();
            repositoryFactory.Stub(factory => factory.Create(Arg<IRepositoryProvider>.Is.Anything)).Return(entityRepository);
            var repositoryProviderFactory = MockRepository.GenerateMock<IRepositoryProviderFactory>();

            var target = new EntityProxy<FakeEntity>(
                repositoryFactory, 
                repositoryProviderFactory, 
                this.entityMapper, 
                this.actionEventProxy);
            var baselineExample = new FakeDto
                                      {
                                          Name = "NameLow", 
                                          Description = "LowDescription", 
                                          DateValue = new DateTime(2012, 10, 22)
                                      };
            var boundaryExample = new FakeDto
                                      {
                                          Name = "NameHigh", 
                                          Description = "HighDescription", 
                                          DateValue = new DateTime(2013, 2, 25)
                                      };
            var query = new FakeQuery
                            {
                                BaselineExample = baselineExample, 
                                BoundaryExample = boundaryExample, 
                                PropertiesToInclude = new Collection<string> { "DateValue" }
                            };

            // The fake repository will not set any properties not included in the query.
            int actual = target.RemoveSelection(query);
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The remove items_ range query_ returns items.
        /// </summary>
        [TestMethod]
        public void RemoveItems_RangeQuery_ReturnsItems()
        {
            const int Expected = 2;
            var entityRepository = MockRepository.GenerateMock<IEntityRepository<FakeEntity>>();
            entityRepository.Stub(repository => repository.DeleteSelection(Arg<ExampleQuery<FakeDto>>.Is.Anything)).Return(Expected);
            var repositoryFactory = MockRepository.GenerateMock<IRepositoryFactory<FakeEntity>>();
            repositoryFactory.Stub(factory => factory.Create(Arg<IRepositoryProvider>.Is.Anything)).Return(entityRepository);
            var repositoryProviderFactory = MockRepository.GenerateMock<IRepositoryProviderFactory>();

            var target = new EntityProxy<FakeEntity>(
                repositoryFactory, 
                repositoryProviderFactory, 
                this.entityMapper, 
                this.actionEventProxy);
            var baselineExample = new FakeDto { Name = "Name1", FakeEntityId = 1, DateValue = new DateTime(2012, 10, 22) };
            var boundaryExample = new FakeDto { Name = "Name10", FakeEntityId = 10, DateValue = new DateTime(2013, 2, 25) };
            var query = new FakeQuery(dto => dto.FakeEntityId, dto => dto.DateValue)
                            {
                                BaselineExample = baselineExample, 
                                BoundaryExample = boundaryExample
                            };

            int actual = target.RemoveSelection(query);
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// The select item_ valid item_ returns item.
        /// </summary>
        [TestMethod]
        public void SelectItem_MissingItem_ReturnsNull()
        {
            var entityRepository = MockRepository.GenerateMock<IEntityRepository<FakeEntity>>();
            entityRepository.Stub(repository => repository.FirstOrDefaultAs(Arg<ExampleQuery<FakeDto>>.Is.Anything)).Return(null);
            var repositoryFactory = MockRepository.GenerateMock<IRepositoryFactory<FakeEntity>>();
            repositoryFactory.Stub(factory => factory.Create(Arg<IRepositoryProvider>.Is.Anything)).Return(entityRepository);
            var repositoryProviderFactory = MockRepository.GenerateMock<IRepositoryProviderFactory>();

            var target = new EntityProxy<FakeEntity>(
                repositoryFactory, 
                repositoryProviderFactory, 
                this.entityMapper, 
                this.actionEventProxy);
            var fakeDto = new FakeDto
                              {
                                  FakeEntityId = 1, 
                                  Name = "TestName1", 
                                  Description = "DescriptionOfTheEntity", 
                                  DateValue = new DateTime(2010, 4, 22)
                              };

            FakeDto actual = target.SelectItem(fakeDto);
            Assert.AreEqual(null, actual);
        }

        /// <summary>
        /// The select item_ valid item_ returns item.
        /// </summary>
        [TestMethod]
        public void SaveItem_InsertedItem_ReturnsItem()
        {
            var expected = new FakeDto
                               {
                                   FakeEntityId = 1, 
                                   Name = "TestName1", 
                                   Description = "DescriptionOfTheEntity", 
                                   DateValue = new DateTime(2010, 4, 22)
                               };

            var entityRepository = MockRepository.GenerateMock<IEntityRepository<FakeEntity>>();
            entityRepository.Stub(repository => repository.SaveAs<FakeDto>(Arg<FakeEntity>.Is.Anything)).Return(expected);
            var repositoryFactory = MockRepository.GenerateMock<IRepositoryFactory<FakeEntity>>();
            repositoryFactory.Stub(factory => factory.Create(Arg<IRepositoryProvider>.Is.Anything)).Return(entityRepository);
            var repositoryProviderFactory = MockRepository.GenerateMock<IRepositoryProviderFactory>();

            var target = new EntityProxy<FakeEntity>(
                repositoryFactory, 
                repositoryProviderFactory, 
                this.entityMapper, 
                this.actionEventProxy);
            FakeDto actual = target.SaveItem(expected);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// The select items_ baseline query with specified properties_ returns item.
        /// </summary>
        [TestMethod]
        public void SelectItems_BaselineQueryWithSpecifiedProperties_ReturnsItem()
        {
            var expected = new List<FakeDto> { new FakeDto { Name = "SomeName", DateValue = new DateTime(2012, 10, 22) } };

            var entityRepository = MockRepository.GenerateMock<IEntityRepository<FakeEntity>>();
            entityRepository.Stub(repository => repository.SelectAs(Arg<ExampleQuery<FakeDto>>.Is.Anything)).Return(expected);
            var repositoryFactory = MockRepository.GenerateMock<IRepositoryFactory<FakeEntity>>();
            repositoryFactory.Stub(factory => factory.Create(Arg<IRepositoryProvider>.Is.Anything)).Return(entityRepository);
            var repositoryProviderFactory = MockRepository.GenerateMock<IRepositoryProviderFactory>();

            var target = new EntityProxy<FakeEntity>(
                repositoryFactory, 
                repositoryProviderFactory, 
                this.entityMapper, 
                this.actionEventProxy);
            var baselineExample = new FakeDto { Name = "SomeName", DateValue = new DateTime(2012, 10, 22) };
            var query = new FakeQuery
                            {
                                BaselineExample = baselineExample, 
                                PropertiesToInclude = new Collection<string> { "DateValue" }

                                // This is the only property returned by our fake repository.
                            };

            var actual = target.SelectItems(query);
            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        /// <summary>
        /// The select items_ baseline query_ returns item.
        /// </summary>
        [TestMethod]
        public void SelectItems_BaselineQuery_ReturnsItem()
        {
            var expected = new List<FakeDto> { new FakeDto { Name = "TestName", DateValue = new DateTime(2012, 10, 22) } };

            var entityRepository = MockRepository.GenerateMock<IEntityRepository<FakeEntity>>();
            entityRepository.Stub(repository => repository.SelectAs(Arg<ExampleQuery<FakeDto>>.Is.Anything)).Return(expected);
            var repositoryFactory = MockRepository.GenerateMock<IRepositoryFactory<FakeEntity>>();
            repositoryFactory.Stub(factory => factory.Create(Arg<IRepositoryProvider>.Is.Anything)).Return(entityRepository);
            var repositoryProviderFactory = MockRepository.GenerateMock<IRepositoryProviderFactory>();

            var target = new EntityProxy<FakeEntity>(
                repositoryFactory, 
                repositoryProviderFactory, 
                this.entityMapper, 
                this.actionEventProxy);
            var baselineExample = new FakeDto { Name = "TestName", DateValue = new DateTime(2012, 10, 22) };
            var query = new FakeQuery { BaselineExample = baselineExample };

            var actual = target.SelectItems(query);
            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        /// <summary>
        /// The select items_ range query with specified properties_ returns items.
        /// </summary>
        [TestMethod]
        public void SelectItems_RangeQueryWithSpecifiedProperties_ReturnsItems()
        {
            var expected = new List<FakeDto>
                               {
                                   new FakeDto
                                       {
                                           Name = "Name of the item.", 
                                           DateValue = new DateTime(2012, 10, 22), 
                                           Description = "LowDescription"
                                       }, 
                                   new FakeDto
                                       {
                                           Name = "Some other name.", 
                                           DateValue = new DateTime(2013, 2, 25), 
                                           Description = "HighDescription"
                                       }
                               };

            var entityRepository = MockRepository.GenerateMock<IEntityRepository<FakeEntity>>();
            entityRepository.Stub(repository => repository.SelectAs(Arg<ExampleQuery<FakeDto>>.Is.Anything)).Return(expected);
            var repositoryFactory = MockRepository.GenerateMock<IRepositoryFactory<FakeEntity>>();
            repositoryFactory.Stub(factory => factory.Create(Arg<IRepositoryProvider>.Is.Anything)).Return(entityRepository);
            var repositoryProviderFactory = MockRepository.GenerateMock<IRepositoryProviderFactory>();

            var target = new EntityProxy<FakeEntity>(
                repositoryFactory, 
                repositoryProviderFactory, 
                this.entityMapper, 
                this.actionEventProxy);

            var baselineExample = new FakeDto
                                      {
                                          Name = "Name of the item.", 
                                          Description = "LowDescription", 
                                          DateValue = new DateTime(2012, 10, 22)
                                      };
            var boundaryExample = new FakeDto
                                      {
                                          Name = "Some other name.", 
                                          Description = "HighDescription", 
                                          DateValue = new DateTime(2013, 2, 25)
                                      };

            var query = new FakeQuery
                            {
                                BaselineExample = baselineExample, 
                                BoundaryExample = boundaryExample, 
                                PropertiesToInclude = new Collection<string> { "DateValue" }
                            };

            var actual = target.SelectItems(query);
            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        /// <summary>
        /// The select items_ range query_ returns items.
        /// </summary>
        [TestMethod]
        public void SelectItems_RangeQuery_ReturnsItems()
        {
            var expected = new List<FakeDto>
                               {
                                   new FakeDto { Name = "Name1", FakeEntityId = 1, DateValue = new DateTime(2012, 10, 22) }, 
                                   new FakeDto
                                       {
                                           Name = "Name10", 
                                           FakeEntityId = 10, 
                                           DateValue = new DateTime(2013, 2, 25)
                                       }
                               };

            var entityRepository = MockRepository.GenerateMock<IEntityRepository<FakeEntity>>();
            entityRepository.Stub(repository => repository.SelectAs(Arg<ExampleQuery<FakeDto>>.Is.Anything)).Return(expected);
            var repositoryFactory = MockRepository.GenerateMock<IRepositoryFactory<FakeEntity>>();
            repositoryFactory.Stub(factory => factory.Create(Arg<IRepositoryProvider>.Is.Anything)).Return(entityRepository);
            var repositoryProviderFactory = MockRepository.GenerateMock<IRepositoryProviderFactory>();

            var target = new EntityProxy<FakeEntity>(
                repositoryFactory, 
                repositoryProviderFactory, 
                this.entityMapper, 
                this.actionEventProxy);
            var baselineExample = new FakeDto { Name = "Name1", FakeEntityId = 1, DateValue = new DateTime(2012, 10, 22) };
            var boundaryExample = new FakeDto { Name = "Name10", FakeEntityId = 10, DateValue = new DateTime(2013, 2, 25) };
            var query = new FakeQuery(dto => dto.FakeEntityId, dto => dto.DateValue)
                            {
                                BaselineExample = baselineExample, 
                                BoundaryExample = boundaryExample
                            };

            // The fake repository will use default values for any property not set in the query.
            var actual = target.SelectItems(query);
            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        #endregion

        /// <summary>
        /// The create action event proxy.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionEventProxy"/>.
        /// </returns>
        private static ActionEventProxy CreateActionEventProxy()
        {
            var eventRepository = MockRepository.GenerateMock<IEventRepository>();
            eventRepository.Stub(repository => repository.Save(Arg<ActionEvent>.Is.Anything)).Return(null).WhenCalled(
                invocation =>
                    {
                        var item = invocation.Arguments.OfType<ActionEvent>().First();
                        invocation.ReturnValue = item;
                    });

            var eventRepositoryFactory = MockRepository.GenerateMock<IEventRepositoryFactory>();
            eventRepositoryFactory.Stub(factory => factory.Create()).Return(eventRepository);

            return new ActionEventProxy(
                EntityOperationContext.Current, 
                ServiceExceptionHandler.Default, 
                ServiceErrorMapping.Default, 
                eventRepositoryFactory, 
                false);
        }
    }
}