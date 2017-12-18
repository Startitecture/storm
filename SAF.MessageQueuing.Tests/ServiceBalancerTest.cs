// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LeastRecentlyUsedServiceBalancerTest.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for LeastRecentlyUsedServiceBalancerTest and is intended
    /// to contain all LeastRecentlyUsedServiceBalancerTest Unit Tests
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ServiceBalancerTest
    {
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
        // Use TestInitialize to run code before running each test
        // [TestInitialize()]
        // public void MyTestInitialize()
        // {
        // }
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup()
        // {
        // }
        #region Public Methods and Operators

        /// <summary>
        /// A test for NextService
        /// </summary>
        [TestMethod]
        public void NextService_AllPendingServices_SelectsFirstService()
        {
            var expected = new FakeService();
            IEnumerable<FakeService> services = new List<FakeService> { expected, new FakeService(), new FakeService() };
            var target = new ServiceBalancer<FakeService>(services, new ServiceUsageComparer());
            FakeService actual = target.NextService;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for NextService
        /// </summary>
        [TestMethod]
        public void NextService_OneCompletedService_SelectsCompletedService()
        {
            var fakeService1 = new FakeService();
            var fakeService2 = new FakeService();
            var fakeService3 = new FakeService();
            IEnumerable<FakeService> services = new List<FakeService> { fakeService1, fakeService2, fakeService3 };
            var target = new ServiceBalancer<FakeService>(services, new ServiceUsageComparer());
            var id1 = target.NotifyRequestSent(fakeService1);
            var id2 = target.NotifyRequestSent(fakeService2);
            target.NotifyResponseReceived(fakeService1, id1);
            var id3 = target.NotifyRequestSent(fakeService3);
            FakeService actual = target.NextService;

            // Should pick the first service as it is the only one completed.
            Assert.AreEqual(fakeService1, actual);
        }

        /// <summary>
        /// A test for NextService
        /// </summary>
        [TestMethod]
        public void NextService_OneCompletedServiceWithMultipleRequests_SelectsCompletedService()
        {
            var fakeService1 = new FakeService();
            var fakeService2 = new FakeService();
            var fakeService3 = new FakeService();
            IEnumerable<FakeService> services = new List<FakeService> { fakeService1, fakeService2, fakeService3 };
            var target = new ServiceBalancer<FakeService>(services, new ServiceUsageComparer());
            SendAndCompleteRequest(target, fakeService1);
            SendAndCompleteRequest(target, fakeService1);
            SendAndCompleteRequest(target, fakeService2);
            SendAndCompleteRequest(target, fakeService3);
            SendAndCompleteRequest(target, fakeService3);
            SendAndCompleteRequest(target, fakeService1);
            var id1 = target.NotifyRequestSent(fakeService1);
            var id2 = target.NotifyRequestSent(fakeService2);
            target.NotifyResponseReceived(fakeService1, id1);
            var id3 = target.NotifyRequestSent(fakeService3);
            FakeService actual = target.NextService;

            // Should pick the first service as it is the only one completed.
            Assert.AreEqual(fakeService1, actual);
        }

        /// <summary>
        /// The notify request sent_ fake service_ request id not equal to empty guid.
        /// </summary>
        [TestMethod]
        public void NotifyRequestSent_FakeService_RequestIdNotEqualToEmptyGuid()
        {
            var fakeService1 = new FakeService();
            IEnumerable<FakeService> services = new List<FakeService> { fakeService1, new FakeService(), new FakeService() };
            var target = new ServiceBalancer<FakeService>(services, new ServiceUsageComparer());
            FakeService serviceConfiguration = fakeService1;
            Guid notExpected = Guid.Empty;
            Guid actual = target.NotifyRequestSent(serviceConfiguration);
            Assert.AreNotEqual(notExpected, actual);
        }

        /// <summary>
        /// The notify response received_ fake service_ expected service selected next.
        /// </summary>
        [TestMethod]
        public void NotifyResponseReceived_FakeService_ExpectedServiceSelectedNext()
        {
            var fakeService1 = new FakeService();
            var fakeService2 = new FakeService();
            var fakeService3 = new FakeService();
            IEnumerable<FakeService> services = new List<FakeService> { fakeService1, fakeService2, fakeService3 };
            var target = new ServiceBalancer<FakeService>(services, new ServiceUsageComparer());
            var requestInstance1 = target.NotifyRequestSent(fakeService1);
            var requestInstance2 = target.NotifyRequestSent(fakeService2);
            target.NotifyResponseReceived(fakeService1, requestInstance1);
            target.NotifyResponseReceived(fakeService2, requestInstance2);

            // Having used both the other services, fake service 3 should be selected next.
            Assert.AreEqual(fakeService3, target.NextService);
        }

        #endregion

        private static void SendAndCompleteRequest(IServiceBalancer<FakeService> balancer, FakeService service)
        {
            var id = balancer.NotifyRequestSent(service);
            balancer.NotifyResponseReceived(service, id);
        }
    }
}