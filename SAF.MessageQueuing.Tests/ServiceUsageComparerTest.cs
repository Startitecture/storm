// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceAvailabilityComparerTest.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing.Tests
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Rhino.Mocks;

    using SAF.ActionTracking;

    /// <summary>
    /// This is a test class for ServiceAvailabilityComparerTest and is intended
    /// to contain all ServiceAvailabilityComparerTest Unit Tests
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ServiceUsageComparerTest
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
        /// A test for Compare
        /// </summary>
        [TestMethod]
        public void Compare_ServiceRequestStatusInProgress_RequestTimeIsCompared()
        {
            var target = new ServiceUsageComparer();
            var x = new ServiceRequestHistory<FakeQueueRouteBase>(new FakeQueueRouteOne(this.actionEventProxy));
            x.Send();
            Thread.Sleep(10);
            var y = new ServiceRequestHistory<FakeQueueRouteBase>(new FakeQueueRouteOne(this.actionEventProxy));
            y.Send();
            int expected = x.LastRequest.CompareTo(y.LastRequest);
            int actual = target.Compare(x, y);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for Compare
        /// </summary>
        [TestMethod]
        public void Compare_ServiceRequestStatusCompleted_ResponseTimeIsCompared()
        {
            var target = new ServiceUsageComparer();
            var x = new ServiceRequestHistory<FakeQueueRouteBase>(new FakeQueueRouteOne(this.actionEventProxy));
            var idx = x.Send();
            Thread.Sleep(10);
            var y = new ServiceRequestHistory<FakeQueueRouteBase>(new FakeQueueRouteOne(this.actionEventProxy));
            var idy = y.Send();
            x.AcceptResponse(idx);
            Thread.Sleep(10);
            y.AcceptResponse(idy);
            int expected = x.LastResponse.CompareTo(y.LastResponse);
            int actual = target.Compare(x, y);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for Compare
        /// </summary>
        [TestMethod]
        public void Compare_ServiceRequestStatusCompletedToInProgress_CompletedLessThanInProgress()
        {
            var target = new ServiceUsageComparer();
            var x = new ServiceRequestHistory<FakeQueueRouteBase>(new FakeQueueRouteOne(this.actionEventProxy));
            var idx = x.Send();
            var y = new ServiceRequestHistory<FakeQueueRouteBase>(new FakeQueueRouteOne(this.actionEventProxy));
            y.Send();
            x.AcceptResponse(idx);
            int actual = target.Compare(x, y);
        }

        /// <summary>
        /// A test for Compare
        /// </summary>
        [TestMethod]
        public void Compare_ServiceRequestStatusCompletedToInProgress_PendingLessThanCompleted()
        {
            var target = new ServiceUsageComparer();
            var x = new ServiceRequestHistory<FakeQueueRouteBase>(new FakeQueueRouteOne(this.actionEventProxy));
            var y = new ServiceRequestHistory<FakeQueueRouteBase>(new FakeQueueRouteOne(this.actionEventProxy));
            var idy = y.Send();
            y.AcceptResponse(idy);
            int expected = x.RequestState.CompareTo(y.RequestState);
            int actual = target.Compare(x, y);
            Assert.AreEqual(expected, actual);
        }

        #endregion
    }
}