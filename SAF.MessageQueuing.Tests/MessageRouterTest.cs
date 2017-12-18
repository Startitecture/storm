// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageRouterTest.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Rhino.Mocks;

    using SAF.ActionTracking;
    using SAF.Data;
    using SAF.Observer;

    /// <summary>
    /// This is a test class for DocumentRouterTest and is intended
    /// to contain all DocumentRouterTest Unit Tests
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class MessageRouterTest
    {
        /// <summary>
        /// The action event proxy.
        /// </summary>
        private readonly IActionEventProxy actionEventProxy = MockRepository.GenerateMock<IActionEventProxy>();

        #region Public Properties

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        #endregion

        // You can use the following additional attributes as you write your tests:
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext)
        // {
        // }
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup()
        // {
        // }

        /// <summary>
        /// Use TestInitialize to run code before running each test
        /// </summary>
        [TestInitialize]
        public void MyTestInitialize()
        {
        }

        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup()
        // {
        // }
        #region Public Methods and Operators

        /// <summary>
        /// A test for Start
        /// </summary>
        [TestMethod]
        public void Start_MultipleMessagesDelayedAtSlowRate_QueueIncreaseRelativeToLatency()
        {
            var notification = new FakeNotification<FakeMessage>();
            var processorSamples = new EnumerableQuery<double>(new[] { 12.3, 2.5, 27.2, 3.5 });
            var performanceMonitor = MockRepository.GenerateMock<IPerformanceMonitor>();
            performanceMonitor.Stub(monitor => monitor.ProcessorUsageSamples).Return(processorSamples);
            var queuingPolicy = new LimitedResourceQueuingPolicy(performanceMonitor);
            var slowPool = new FakeQueuePool<FakeSlowRoute>(
                new FakeQueueRouteFactory<FakeSlowRoute>(this.actionEventProxy), 
                queuingPolicy, 
                this.actionEventProxy);
            var normalPool = new FakeQueuePool<FakeNormalSpeedRoute>(
                new FakeQueueRouteFactory<FakeNormalSpeedRoute>(this.actionEventProxy), 
                queuingPolicy, 
                this.actionEventProxy);
            var fastPool = new FakeQueuePool<FakeFastRoute>(
                new FakeQueueRouteFactory<FakeFastRoute>(this.actionEventProxy), 
                queuingPolicy, 
                this.actionEventProxy);
            var routingProfile = new FakeCustomProfile<FakeMessage>(slowPool, normalPool, fastPool);

            using (var serviceRouteContainer = new ServiceRouteContainer<FakeMessage>())
            using (var target = this.CreateMessageRouter(routingProfile, new ZeroRetryPolicy<FakeMessage>(), serviceRouteContainer))
            {
                const int MessageCount = 50;

                for (int i = 0; i < MessageCount; i++)
                {
                    var message = Generate.PriorityMessage(
                        Convert.ToString(i), 
                        DateTimeOffset.Now, 
                        DateTimeOffset.Now.Date.AddDays(1).AddHours(7), 
                        TimeSpan.FromHours(3));

                    target.Send(new MessageRoutingRequest<FakeMessage>(message, notification));
                    Thread.Sleep(50); // Same speed as slow queue.
                }

                if (Debugger.IsAttached)
                {
                    notification.WaitForDelivery();
                }
                else
                {
                    Assert.IsTrue(
                        notification.WaitForDelivery(TimeSpan.FromSeconds(20)), 
                        "Operation timed out: {0} of {1} delivered.", 
                        notification.Deliveries.Count(), 
                        MessageCount);
                }

                Assert.IsFalse(
                    slowPool.QueueCount < normalPool.QueueCount, 
                    "Slow Pool: {0}; Normal Pool {1}", 
                    slowPool.QueueCount, 
                    normalPool.QueueCount);

                Assert.IsFalse(
                    normalPool.QueueCount < fastPool.QueueCount, 
                    "Normal Pool: {0}; Fast Pool {1}", 
                    normalPool.QueueCount, 
                    fastPool.QueueCount);
            }
        }

        /// <summary>
        /// A test for Start
        /// </summary>
        [TestMethod]
        public void Start_MultipleMessagesDelayedAtNormalRate_QueueIncreaseRelativeToLatency()
        {
            var notification = new FakeNotification<FakeMessage>();
            var processorSamples = new EnumerableQuery<double>(new[] { 12.3, 2.5, 27.2, 3.5 });
            var performanceMonitor = MockRepository.GenerateMock<IPerformanceMonitor>();
            performanceMonitor.Stub(monitor => monitor.ProcessorUsageSamples).Return(processorSamples);
            var queuingPolicy = new LimitedResourceQueuingPolicy(performanceMonitor);
            var slowPool = new FakeQueuePool<FakeSlowRoute>(
                new FakeQueueRouteFactory<FakeSlowRoute>(this.actionEventProxy), 
                queuingPolicy, 
                this.actionEventProxy);
            var normalPool = new FakeQueuePool<FakeNormalSpeedRoute>(
                new FakeQueueRouteFactory<FakeNormalSpeedRoute>(this.actionEventProxy), 
                queuingPolicy, 
                this.actionEventProxy);
            var fastPool = new FakeQueuePool<FakeFastRoute>(
                new FakeQueueRouteFactory<FakeFastRoute>(this.actionEventProxy), 
                queuingPolicy, 
                this.actionEventProxy);
            var routingProfile = new FakeCustomProfile<FakeMessage>(slowPool, normalPool, fastPool);

            using (var serviceRouteContainer = new ServiceRouteContainer<FakeMessage>())
            using (var target = this.CreateMessageRouter(routingProfile, new ZeroRetryPolicy<FakeMessage>(), serviceRouteContainer))
            {
                for (int i = 0; i < 50; i++)
                {
                    var message = Generate.PriorityMessage(
                        Convert.ToString(i), 
                        DateTimeOffset.Now, 
                        DateTimeOffset.Now.Date.AddDays(1).AddHours(7), 
                        TimeSpan.FromHours(3));

                    target.Send(new MessageRoutingRequest<FakeMessage>(message, notification));
                    Thread.Sleep(12); // Same speed as normal queue.
                }

                if (Debugger.IsAttached)
                {
                    notification.WaitForDelivery();
                }
                else
                {
                    Assert.IsTrue(notification.WaitForDelivery(TimeSpan.FromSeconds(15)), "Operation timed out.");
                }

                Assert.IsFalse(
                    slowPool.QueueCount < normalPool.QueueCount, 
                    "Slow Pool: {0}; Normal Pool {1}", 
                    slowPool.QueueCount, 
                    normalPool.QueueCount);

                Assert.IsFalse(
                    normalPool.QueueCount < fastPool.QueueCount, 
                    "Normal Pool: {0}; Fast Pool {1}", 
                    normalPool.QueueCount, 
                    fastPool.QueueCount);
            }
        }

        /// <summary>
        /// A test for Start
        /// </summary>
        [TestMethod]
        public void Start_MultipleMessagesDelayedAtFastRate_QueueIncreaseRelativeToLatency()
        {
            var notification = new FakeNotification<FakeMessage>();
            var processorSamples = new EnumerableQuery<double>(new[] { 12.3, 2.5, 27.2, 3.5 });
            var performanceMonitor = MockRepository.GenerateMock<IPerformanceMonitor>();
            performanceMonitor.Stub(monitor => monitor.ProcessorUsageSamples).Return(processorSamples);

            var queuingPolicy = new LimitedResourceQueuingPolicy(performanceMonitor);
            var slowPool = new FakeQueuePool<FakeSlowRoute>(
                new FakeQueueRouteFactory<FakeSlowRoute>(this.actionEventProxy), 
                queuingPolicy, 
                this.actionEventProxy);
            var normalPool = new FakeQueuePool<FakeNormalSpeedRoute>(
                new FakeQueueRouteFactory<FakeNormalSpeedRoute>(this.actionEventProxy), 
                queuingPolicy, 
                this.actionEventProxy);
            var fastPool = new FakeQueuePool<FakeFastRoute>(
                new FakeQueueRouteFactory<FakeFastRoute>(this.actionEventProxy), 
                queuingPolicy, 
                this.actionEventProxy);
            var routingProfile = new FakeCustomProfile<FakeMessage>(slowPool, normalPool, fastPool);

            using (var serviceRouteContainer = new ServiceRouteContainer<FakeMessage>())
            using (var target = this.CreateMessageRouter(routingProfile, new ZeroRetryPolicy<FakeMessage>(), serviceRouteContainer))
            {
                for (int i = 0; i < 50; i++)
                {
                    var message = Generate.PriorityMessage(
                        Convert.ToString(i), 
                        DateTimeOffset.Now, 
                        DateTimeOffset.Now.Date.AddDays(1).AddHours(7), 
                        TimeSpan.FromHours(3));

                    target.Send(new MessageRoutingRequest<FakeMessage>(message, notification));
                    Thread.Sleep(5); // Same speed as fast queue.
                }

                if (Debugger.IsAttached)
                {
                    notification.WaitForDelivery();
                }
                else
                {
                    Assert.IsTrue(
                        notification.WaitForDelivery(TimeSpan.FromSeconds(10)), 
                        "Operation timed out. Deliveries: {0}", 
                        notification.Deliveries.Count());
                }

                Assert.IsFalse(
                    slowPool.QueueCount < normalPool.QueueCount, 
                    "Slow Pool: {0}; Normal Pool {1}", 
                    slowPool.QueueCount, 
                    normalPool.QueueCount);

                Assert.IsFalse(
                    normalPool.QueueCount < fastPool.QueueCount, 
                    "Normal Pool: {0}; Fast Pool {1}", 
                    normalPool.QueueCount, 
                    fastPool.QueueCount);
            }
        }

        /// <summary>
        /// A test for Start
        /// </summary>
        [TestMethod]
        public void Start_MultipleMessages_AllMessagesDelivered()
        {
            var notification = new FakeNotification<FakeMessage>();
            var firstRoute = new FakeQueueRouteOne(this.actionEventProxy);
            var secondRoute = new FakeQueueRouteTwo(this.actionEventProxy);
            var thirdRoute = new FakeQueueRouteThree(this.actionEventProxy);
            var routingProfile = new FakeCustomProfile<FakeMessage>(firstRoute, secondRoute, thirdRoute);

            using (var serviceRouteContainer = new ServiceRouteContainer<FakeMessage>())
            using (var target = this.CreateMessageRouter(routingProfile, new ZeroRetryPolicy<FakeMessage>(), serviceRouteContainer))
            {
                const int Expected = 150;

                for (int i = 0; i < Expected; i++)
                {
                    var message = Generate.PriorityMessage(
                        Convert.ToString(i), 
                        DateTimeOffset.Now, 
                        DateTimeOffset.Now.Date.AddDays(1).AddHours(7), 
                        TimeSpan.FromHours(3));

                    target.Send(new MessageRoutingRequest<FakeMessage>(message, notification));
                }

                if (Debugger.IsAttached)
                {
                    notification.WaitForDelivery();
                }
                else
                {
                    Assert.IsTrue(
                        notification.WaitForDelivery(TimeSpan.FromSeconds(10)), 
                        "Operation timed out: Deliveries {0}", 
                        notification.Deliveries.Count());
                }

                Assert.AreEqual(Expected, notification.Deliveries.Count());
            }
        }

        /////// <summary>
        /////// A test for Start
        /////// </summary>
        ////[TestMethod]
        ////public void Start_MultipleMessages_NoSendDelay()
        ////{
        ////    const int MaxDelay = 50;
        ////    var notification = new FakeNotification<FakeMessage>();
        ////    var slowPool = new FakeSlowRoute();
        ////    var normalPool = new FakeNormalSpeedRoute();
        ////    var fastPool = new FakeFastRoute();
        ////    var routingProfile = new FakeCustomProfile<FakeMessage>(slowPool, normalPool, fastPool);

        ////    var routingProfileProvider = new FakeRoutingProvider(new FakeContinuationProvider(), routingProfile);
        ////    var target = new MessageRouter<FakeMessage>(new FakeRoutingRequestPool(), routingProfileProvider, new FakeFailureQueueRoute());

        ////    var stopwatch = Stopwatch.StartNew();
        ////    for (int i = 0; i < 150; i++)
        ////    {
        ////        var message = Generate.PriorityMessage(
        ////            Convert.ToString(i), DateTimeOffset.Now, DateTimeOffset.Now.Date.AddDays(1).AddHours(7), TimeSpan.FromHours(3));

        ////        target.Start(message, notification);
        ////    }

        ////    stopwatch.Stop();
        ////    Assert.IsTrue(
        ////        stopwatch.Elapsed < TimeSpan.FromMilliseconds(MaxDelay),
        ////        "Took {0}ms to queue messages (expecting < {1}ms).",
        ////        stopwatch.Elapsed,
        ////        TimeSpan.FromMilliseconds(MaxDelay));

        ////    if (Debugger.IsAttached)
        ////    {
        ////        notification.WaitForDelivery();
        ////    }
        ////    else
        ////    {
        ////        Assert.IsTrue(notification.WaitForDelivery(TimeSpan.FromSeconds(15)), "Operation timed out.");
        ////    }
        ////}

        /// <summary>
        /// A test for Start
        /// </summary>
        [TestMethod]
        public void Start_WithCompleteMessage_AllQueuesReceiveMessageInOrder()
        {
            var routingProfile = new FakeTwoRouteProfile(this.actionEventProxy);

            using (var serviceRouteContainer = new ServiceRouteContainer<FakeMessage>())
            using (var target = this.CreateMessageRouter(routingProfile, new ZeroRetryPolicy<FakeMessage>(), serviceRouteContainer))
            {
                FakeMessage message = Generate.PriorityMessage(
                    "First", 
                    DateTimeOffset.Now, 
                    DateTimeOffset.Now.Date.AddDays(1).AddHours(7), 
                    TimeSpan.FromHours(3));

                var notification = StartMessage(target, message);

                AssertOrder(notification, new FakeQueueRouteOne(this.actionEventProxy), new FakeQueueRouteTwo(this.actionEventProxy));
            }
        }

        /// <summary>
        /// A test for Start
        /// </summary>
        [TestMethod]
        public void Start_WithMessageRetry_MessageRetriedSpecifiedNumberOfTimes()
        {
            var routingProfile = new FakeTwoRouteProfile(this.actionEventProxy);
            var failurePolicy = new LimitedWindowRetryPolicy<FakeMessage>(3, TimeSpan.FromMilliseconds(100));

            using (var serviceRouteContainer = new ServiceRouteContainer<FakeMessage>())
            using (var target = this.CreateMessageRouter(routingProfile, failurePolicy, serviceRouteContainer))
            {
                FakeMessage message = Generate.PriorityMessage(
                    "First", 
                    DateTimeOffset.Now, 
                    DateTimeOffset.Now.Date.AddDays(1).AddHours(7), 
                    TimeSpan.FromHours(3));

                message.RequestShouldFail = true;
                var notification = StartMessage(target, message);

                AssertOrder(
                    notification, 
                    new FakeQueueRouteOne(this.actionEventProxy), 
                    new FakeQueueRouteOne(this.actionEventProxy), 
                    new FakeQueueRouteOne(this.actionEventProxy), 
                    new FakeFailureQueueRoute(this.actionEventProxy));
            }
        }

        /// <summary>
        /// A test for Start
        /// </summary>
        [TestMethod]
        public void Start_WithMessageRetry_MessageDelayGreaterThanOrEqualToExpected()
        {
            var routingProfile = new FakeTwoRouteProfile(this.actionEventProxy);
            var waitTime = TimeSpan.FromMilliseconds(100);
            const int MaxAttempts = 3;
            var failurePolicy = new LimitedWindowRetryPolicy<FakeMessage>(MaxAttempts, waitTime);

            using (var serviceRouteContainer = new ServiceRouteContainer<FakeMessage>())
            using (var target = this.CreateMessageRouter(routingProfile, failurePolicy, serviceRouteContainer))
            {
                FakeMessage message = Generate.PriorityMessage(
                    "First", 
                    DateTimeOffset.Now, 
                    DateTimeOffset.Now.Date.AddDays(1).AddHours(7), 
                    TimeSpan.FromHours(3));

                message.RequestShouldFail = true;

                var stopwatch = Stopwatch.StartNew();
                var notification = StartMessage(target, message);
                stopwatch.Stop();

                var expected = (MaxAttempts - 1) * waitTime.TotalMilliseconds;
                Assert.IsTrue(
                    stopwatch.Elapsed >= TimeSpan.FromMilliseconds(expected), 
                    "Expected a delay of at least {0}; got {1}.", 
                    expected, 
                    stopwatch.Elapsed);

                AssertOrder(
                    notification, 
                    new FakeQueueRouteOne(this.actionEventProxy), 
                    new FakeQueueRouteOne(this.actionEventProxy), 
                    new FakeQueueRouteOne(this.actionEventProxy), 
                    new FakeFailureQueueRoute(this.actionEventProxy));
            }
        }

        /// <summary>
        /// A test for Start
        /// </summary>
        [TestMethod]
        public void Start_WithFailedMessage_FailureQueueReceivesMessageLast()
        {
            var routingProfile = new FakeTwoRouteProfile(this.actionEventProxy);

            using (var serviceRouteContainer = new ServiceRouteContainer<FakeMessage>())
            using (var target = this.CreateMessageRouter(routingProfile, new ZeroRetryPolicy<FakeMessage>(), serviceRouteContainer))
            {
                FakeMessage message = Generate.PriorityMessage(
                    "First", 
                    DateTimeOffset.Now, 
                    DateTimeOffset.Now.Date.AddDays(1).AddHours(7), 
                    TimeSpan.FromHours(3), 
                    5, 
                    true);

                var notification = StartMessage(target, message);
                AssertOrder(
                    notification, 
                    new FakeQueueRouteOne(this.actionEventProxy), 
                    new FakeFailureQueueRoute(this.actionEventProxy));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The create message router.
        /// </summary>
        /// <param name="routingProfile">
        /// The routing Profile.
        /// </param>
        /// <param name="failurePolicy">
        /// The failure Policy.
        /// </param>
        /// <param name="serviceRouteContainer">
        /// The service route container.
        /// </param>
        /// <returns>
        /// The <see cref="MessageRouter"/>.
        /// </returns>
        private MessageRouter<FakeMessage> CreateMessageRouter(
            IMessageRoutingProfile<FakeMessage> routingProfile, 
            IRoutingFailurePolicy<FakeMessage> failurePolicy, 
            IServiceRouteContainer<FakeMessage> serviceRouteContainer)
        {
            var continuationProvider = new FakeContinuationProvider();
            var failureRoute = new FakeFailureQueueRoute(this.actionEventProxy);
            var routingProfileProvider = new FakeRoutingProvider(continuationProvider, failureRoute, failurePolicy, routingProfile);

            ////new StubIRoutingProfileProvider<FakeMessage>
            ////                             {
            ////                                 ContinuationProviderGet = () => new StubIRoutingContinuationProvider<FakeMessage>(),
            ////                                 DuplicateEqualityComparerGet = () => FakeDuplicateEqualityComparer.DuplicateRequest,
            ////                                 FailurePolicyGet = () => new LimitedWindowRetryPolicy<FakeMessage>(3),
            ////                                 ResolveProfileT0 = message => routingProfile,
            ////                                 FailureRouteGet = () => new StubIServiceRoute<FakeMessage>()
            ////                             };

            var routingRepository = MockRepository.GenerateMock<IRoutingRepository<FakeMessage>>();
            var routingRepositoryFactory = MockRepository.GenerateMock<IRoutingRepositoryFactory<FakeMessage>>();
            routingRepositoryFactory.Stub(factory => factory.Create(Arg<IRepositoryProvider>.Is.Anything)).Return(routingRepository);

            var repositoryProvider = MockRepository.GenerateMock<IRepositoryProvider>();
            var repositoryProviderFactory = MockRepository.GenerateMock<IRepositoryProviderFactory>();
            repositoryProviderFactory.Stub(factory => factory.Create()).Return(repositoryProvider);

            return new MessageRouter<FakeMessage>(
                routingProfileProvider, 
                serviceRouteContainer, 
                routingRepositoryFactory, 
                this.actionEventProxy, 
                repositoryProviderFactory);
        }

        /// <summary>
        /// The start request.
        /// </summary>
        /// <param name="router">
        /// The router.
        /// </param>
        /// <param name="message">
        /// The requests.
        /// </param>
        /// <returns>
        /// The <see cref="FakeNotification"/>.
        /// </returns>
        private static FakeNotification<FakeMessage> StartMessage(MessageRouter<FakeMessage> router, FakeMessage message)
        {
            var notification = new FakeNotification<FakeMessage> { CaptureAllEvents = true };
            router.Send(new MessageRoutingRequest<FakeMessage>(message, notification));
            bool deliveryCompleted = Debugger.IsAttached
                                         ? notification.WaitForDelivery()
                                         : notification.WaitForDelivery(TimeSpan.FromSeconds(15));

            Assert.IsTrue(deliveryCompleted, "The test timed out. Delivered {0} items.", notification.Deliveries.Count());
            return notification;
        }

        /// <summary>
        /// The assert order.
        /// </summary>
        /// <param name="notification">
        /// The notification.
        /// </param>
        /// <param name="expectedOrder">
        /// The expected order.
        /// </param>
        private static void AssertOrder(FakeNotification<FakeMessage> notification, params IServiceRoute<FakeMessage>[] expectedOrder)
        {
            // TODO: Issue here is that we are not getting recipt events from the pending queue.
            var routingEventsList = new List<MessageEntry<FakeMessage>>(notification.RoutingEvents.OrderBy(x => x.InitiationTime));
            var routedEventsList = new List<MessageExit<FakeMessage>>(notification.RoutedEvents.OrderBy(x => x.InitiationTime));
            var receiptEventList = new List<MessageEntry<FakeMessage>>(notification.ReceiptEvents.OrderBy(x => x.InitiationTime));
            var responseEventsList = new List<MessageExit<FakeMessage>>(notification.ResponseEvents.OrderBy(x => x.CompletionTime));

            Assert.IsTrue(routingEventsList.Any(), "Did not get any requests.");
            Assert.IsTrue(routedEventsList.Any(), "Did not get any responses.");
            Assert.IsTrue(receiptEventList.Any(), "Did not get any requests.");
            Assert.IsTrue(responseEventsList.Any(), "Did not get any responses.");

            var serviceRoutes = expectedOrder as IList<IServiceRoute<FakeMessage>> ?? expectedOrder.ToList();

            // The first route should be the pending queue route, the second will be the response queue route. Then each route should 
            // be separated by the response queue route.
            int routeIndex = 0;

            for (int i = 1; i < routingEventsList.Count; i++)
            {
                var routingEvent = routingEventsList.ElementAtOrDefault(i);
                Assert.IsNotNull(
                    routingEvent, 
                    "Expected route #{0} '{1}' was not found in the routing event list.", 
                    i, 
                    serviceRoutes.ElementAt(routeIndex).Name);

                Assert.AreEqual(serviceRoutes.ElementAt(routeIndex).Name, routingEvent.ServiceRoute.Name);

                var routedEvent = routedEventsList.ElementAtOrDefault(i);
                Assert.IsNotNull(
                    routedEvent, 
                    "Expected route #{0} '{1}' was not found in the routed event list.", 
                    i, 
                    serviceRoutes.ElementAt(routeIndex).Name);

                Assert.AreEqual(serviceRoutes.ElementAt(routeIndex).Name, routedEvent.ServiceRoute.Name);

                var receiptEvent = receiptEventList.ElementAtOrDefault(i);
                Assert.IsNotNull(
                    receiptEvent, 
                    "Expected route #{0} '{1}' was not found in the receipt event list.", 
                    i, 
                    serviceRoutes.ElementAt(routeIndex).Name);

                Assert.AreEqual(serviceRoutes.ElementAt(routeIndex).Name, receiptEvent.ServiceRoute.Name);

                var responseEvent = responseEventsList.ElementAtOrDefault(i);
                Assert.IsNotNull(
                    responseEvent, 
                    "Expected route #{0} '{1}' was not found in the response event list.", 
                    i, 
                    serviceRoutes.ElementAt(routeIndex).Name);

                Assert.AreEqual(serviceRoutes.ElementAt(routeIndex).Name, responseEvent.ServiceRoute.Name);

                Assert.AreEqual(
                    receiptEvent.Identifier, 
                    responseEvent.Identifier, 
                    "The receipt and response events did not have the same identifier.");

                routeIndex++;
            }
        }

        #endregion
    }
}