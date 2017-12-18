// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityErrorHandlerTest.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.EntityServices.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Security.Principal;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Rhino.Mocks;

    using SAF.ActionTracking;
    using SAF.Core;
    using SAF.Data;
    using SAF.Security;
    using SAF.Testing.Common;

    /// <summary>
    /// This is a test class for EntityErrorHandlerTest and is intended
    /// to contain all EntityErrorHandlerTest Unit Tests
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class EntityErrorHandlerTest
    {
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
            var mapper = new AutoMapperEntityMapper();

            mapper.Initialize(
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
        }

        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup()
        // {
        // }
        #region Public Methods and Operators

        /// <summary>
        /// A test for HandleError
        /// </summary>
        [TestMethod]
        public void HandleError_UnhandledException_ReturnsFalse()
        {
            var eventRepository = MockRepository.GenerateMock<IEventRepository>();
            var eventRepositoryFactory = MockRepository.GenerateMock<IEventRepositoryFactory>();
            eventRepositoryFactory.Stub(factory => factory.Create()).Return(eventRepository);

            var actionContext = MockRepository.GenerateMock<IActionContext>();
            actionContext.Stub(context => context.CurrentAccount).Return(new WindowsAccountInfo(WindowsIdentity.GetCurrent()));

            var target = new EntityErrorHandler(actionContext, eventRepositoryFactory, ServiceErrorMapping.Default);
            Exception error = new InvalidOperationException("Unhandled Exception.");
            bool actual = target.HandleError(error);
            Assert.AreEqual(false, actual);
        }

        /// <summary>
        /// A test for ProvideFault
        /// </summary>
        [TestMethod]
        public void ProvideFault_UnhandledException_MatchesExpectedProperties()
        {
            var eventRepository = MockRepository.GenerateMock<IEventRepository>();
            var eventRepositoryFactory = MockRepository.GenerateMock<IEventRepositoryFactory>();
            eventRepositoryFactory.Stub(factory => factory.Create()).Return(eventRepository);

            var actionContext = MockRepository.GenerateMock<IActionContext>();
            actionContext.Stub(context => context.CurrentAccount).Return(new WindowsAccountInfo(WindowsIdentity.GetCurrent()));

            var target = new EntityErrorHandler(actionContext, eventRepositoryFactory, ServiceErrorMapping.Default);
            Exception error = new InvalidOperationException("Unhandled Exception.");
            MessageVersion version = MessageVersion.Default;
            Message fault = null;
            target.ProvideFault(error, version, ref fault);
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.IsFault);
        }

        /// <summary>
        /// A test for ProvideFault
        /// </summary>
        [TestMethod]
        public void HandleError_UnhandledException_MatchesExpectedProperties()
        {
            ServiceHost serviceHost = null;
            ChannelFactory<IFakeService> factory = null;
            FaultException<InternalOperationFault> faultException = null;

            try
            {
                serviceHost = new ServiceHost(
                    new FakeService(MockRepository.GenerateMock<IEntityProxyFactory>()), 
                    new Uri("http://localhost:20201/FakeService"));
                serviceHost.Description.Behaviors.Add(new EntityErrorHandlerExtension());
                serviceHost.Open();

                var basicHttpBinding = new BasicHttpBinding
                                           {
                                               SendTimeout = TimeSpan.FromHours(1), 
                                               ReceiveTimeout = TimeSpan.FromHours(1)
                                           };
                factory = new ChannelFactory<IFakeService>(basicHttpBinding, "http://localhost:20201/FakeService");
                IFakeService serviceProxy = factory.CreateChannel();

                serviceProxy.ThrowUnhandledException(new FakeDto());
            }
            catch (FaultException<InternalOperationFault> ex)
            {
                faultException = ex;
            }
            finally
            {
                if (serviceHost != null && serviceHost.State == CommunicationState.Opened)
                {
                    serviceHost.Close();
                }

                if (factory != null && factory.State == CommunicationState.Opened)
                {
                    factory.Close();
                }
            }

            Assert.IsNotNull(faultException, "Exception not set.");
            Assert.IsNotNull(faultException.Code, "Code not set.");
            Assert.IsFalse(String.IsNullOrWhiteSpace(Convert.ToString(faultException.Reason)), "Reason not set.");
        }

        /// <summary>
        /// A test for HandleError
        /// </summary>
        [TestMethod]
        public void HandleError_DomainException_ReturnsTrue()
        {
            var eventRepository = MockRepository.GenerateMock<IEventRepository>();
            var eventRepositoryFactory = MockRepository.GenerateMock<IEventRepositoryFactory>();
            eventRepositoryFactory.Stub(factory => factory.Create()).Return(eventRepository);

            var actionContext = MockRepository.GenerateMock<IActionContext>();
            actionContext.Stub(context => context.CurrentAccount).Return(new WindowsAccountInfo(WindowsIdentity.GetCurrent()));
            actionContext.Stub(context => context.CurrentAction).Return("The current action.");
            actionContext.Stub(context => context.CurrentActionSource).Return("The current action source.");
            actionContext.Stub(context => context.CurrentIdentity).Return(WindowsIdentity.GetCurrent());
            actionContext.Stub(context => context.Endpoint).Return("Endpoint");
            actionContext.Stub(context => context.Host).Return(Environment.MachineName);

            var target = new EntityErrorHandler(actionContext, eventRepositoryFactory, ServiceErrorMapping.Default);
            bool actual;

            try
            {
                throw new OperationException("Domain Exception.");
            }
            catch (OperationException ex)
            {
                actual = target.HandleError(ex);
            }

            Assert.AreEqual(true, actual);
        }

        /// <summary>
        /// A test for ProvideFault
        /// </summary>
        [TestMethod]
        public void ProvideFault_DomainException_MatchesExpectedProperties()
        {
            var eventRepository = MockRepository.GenerateMock<IEventRepository>();
            var eventRepositoryFactory = MockRepository.GenerateMock<IEventRepositoryFactory>();
            eventRepositoryFactory.Stub(factory => factory.Create()).Return(eventRepository);

            var actionContext = MockRepository.GenerateMock<IActionContext>();
            actionContext.Stub(context => context.CurrentAccount).Return(new WindowsAccountInfo(WindowsIdentity.GetCurrent()));

            var target = new EntityErrorHandler(actionContext, eventRepositoryFactory, ServiceErrorMapping.Default);

            Exception error = new OperationException("Domain Exception.");
            MessageVersion version = MessageVersion.Default;
            Message fault = null;
            target.ProvideFault(error, version, ref fault);
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.IsFault);
        }

        /// <summary>
        /// A test for ProvideFault
        /// </summary>
        [TestMethod]
        public void HandleError_DomainException_MatchesExpectedProperties()
        {
            ServiceHost serviceHost = null;
            ChannelFactory<IFakeService> factory = null;
            FaultException<InternalOperationFault> faultException = null;

            try
            {
                serviceHost = new ServiceHost(
                    new FakeService(MockRepository.GenerateMock<IEntityProxyFactory>()), 
                    new Uri("http://localhost:20201/FakeService"));
                serviceHost.Description.Behaviors.Add(new EntityErrorHandlerExtension());
                serviceHost.Open();

                factory = new ChannelFactory<IFakeService>(new BasicHttpBinding(), "http://localhost:20201/FakeService");
                IFakeService serviceProxy = factory.CreateChannel();

                serviceProxy.ThrowDomainException(new FakeDto());
            }
            catch (FaultException<InternalOperationFault> ex)
            {
                faultException = ex;
            }
            finally
            {
                if (serviceHost != null && serviceHost.State == CommunicationState.Opened)
                {
                    serviceHost.Close();
                }

                if (factory != null && factory.State == CommunicationState.Opened)
                {
                    factory.Close();
                }
            }

            Assert.IsNotNull(faultException, "Exception not set.");
            Assert.IsNotNull(faultException.Code, "Code not set.");
            Assert.IsFalse(String.IsNullOrWhiteSpace(Convert.ToString(faultException.Reason)), "Reason not set.");
            Assert.IsTrue(faultException.IsActionFault(), "Not ActionFault.");
        }

        #endregion
    }
}