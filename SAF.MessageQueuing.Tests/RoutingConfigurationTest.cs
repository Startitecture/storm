// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoutingConfigurationTest.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Rhino.Mocks;

    using SAF.ActionTracking;

    /// <summary>
    /// This is a test class for RoutingConfigurationTest and is intended
    /// to contain all RoutingConfigurationTest Unit Tests
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class RoutingConfigurationTest
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
        /// The routing configuration constructor test.
        /// </summary>
        [TestMethod]
        public void RoutingConfigurationConstructor_FirstRouteNotSpecified_MatchesFirstRouteOfProfile()
        {
            var queueRouteOne = new FakeQueueRouteOne(this.actionEventProxy);
            var queueRouteTwo = new FakeQueueRouteTwo(this.actionEventProxy);
            var queueRouteThree = new FakeQueueRouteThree(this.actionEventProxy);
            IMessageRoutingProfile<FakeMessage> profile = new FakeCustomProfile<FakeMessage>(
                queueRouteOne,
                queueRouteTwo,
                queueRouteThree);

            var expected = queueRouteOne;
            var fakeMessage = new FakeMessage(
                DateTimeOffset.Now,
                DateTimeOffset.Now.Date.AddDays(1).AddHours(7),
                TimeSpan.FromHours(3));

            var profileProvider = new FakeRoutingProvider(new FakeFailureQueueRoute(this.actionEventProxy), profile);
            var target = new RoutingConfiguration<FakeMessage>(profileProvider, fakeMessage, null);
            Assert.AreEqual(expected, target.ServiceRoutes.First);
        }

        /// <summary>
        /// The routing configuration constructor_ with first route specified_ matches first route.
        /// </summary>
        [TestMethod]
        public void RoutingConfigurationConstructor_FirstRouteSpecified_MatchesSpecifiedRoute()
        {
            var queueRouteOne = new FakeQueueRouteOne(this.actionEventProxy);
            var queueRouteTwo = new FakeQueueRouteTwo(this.actionEventProxy);
            var queueRouteThree = new FakeQueueRouteThree(this.actionEventProxy);
            IMessageRoutingProfile<FakeMessage> profile = new FakeCustomProfile<FakeMessage>(queueRouteOne, queueRouteTwo, queueRouteThree);

            IServiceRoute<FakeMessage> firstRoute = queueRouteTwo;
            var fakeMessage = new FakeMessage(
                DateTimeOffset.Now,
                DateTimeOffset.Now.Date.AddDays(1).AddHours(7),
                TimeSpan.FromHours(3));

            var profileProvider = new FakeRoutingProvider(new FakeFailureQueueRoute(this.actionEventProxy), profile);
            var target = new RoutingConfiguration<FakeMessage>(profileProvider, fakeMessage, firstRoute);
            Assert.AreEqual(firstRoute, target.ServiceRoutes.First);
        }

        #endregion
    }
}