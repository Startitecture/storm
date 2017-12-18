// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceRequestStatusTest.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   This is a test class for ServiceRequestStatusTest and is intended
//   to contain all ServiceRequestStatusTest Unit Tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for ServiceRequestStatusTest and is intended
    /// to contain all ServiceRequestStatusTest Unit Tests
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ServiceRequestStatusTest
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
        /// A test for AcceptResponse
        /// </summary>
        [TestMethod]
        public void RequestState_AfterAcceptResponse_EqualsCompleted()
        {

            var target = new ServiceRequestStatus();
            target.Send();
            target.AcceptResponse();
            Assert.AreEqual(ServiceRequestState.Completed, target.RequestState);
        }

        /// <summary>
        /// A test for AcceptResponse
        /// </summary>
        [TestMethod]
        public void ResponseTime_AfterAcceptResponse_ResponseTimeGreaterThanOrEqualToRequestTime()
        {
            var target = new ServiceRequestStatus();
            target.Send();
            target.AcceptResponse();
            Assert.IsTrue(target.ResponseTime >= target.RequestTime);
        }

        /// <summary>
        /// A test for CompareTo
        /// </summary>
        [TestMethod]
        public void CompareTo_ServiceRequestStatusPendingToInProgress_ReturnsValueLessThanZero()
        {
            var target = new ServiceRequestStatus();
            var other = new ServiceRequestStatus();
            other.Send();
            int actual = target.CompareTo(other);
            Assert.IsTrue(actual < 0);
        }

        /// <summary>
        /// A test for CompareTo
        /// </summary>
        [TestMethod]
        public void CompareTo_ServiceRequestStatusOlderToNewer_ReturnsValueLessThanZero()
        {
            var target = new ServiceRequestStatus();
            target.Send();
            Thread.Sleep(10);
            var other = new ServiceRequestStatus();
            other.Send();
            int actual = target.CompareTo(other);
            Assert.IsTrue(actual < 0);
        }

        /// <summary>
        /// A test for Equals
        /// </summary>
        [TestMethod]
        public void Equals_EquivalentServiceRequestStatus_ReturnsTrue()
        {
            var target = new ServiceRequestStatus();
            var other = new ServiceRequestStatus();
            bool actual = target.Equals(other);
            Assert.AreEqual(true, actual);
        }

        /// <summary>
        /// A test for Equals
        /// </summary>
        [TestMethod]
        public void Equals_NonEquivalentServiceRequestStatus_ReturnsFalse()
        {
            var target = new ServiceRequestStatus();
            var other = new ServiceRequestStatus();
            other.Send();
            bool actual = target.Equals(other);
            Assert.AreEqual(false, actual);
        }

        /// <summary>
        /// A test for SendNew
        /// </summary>
        [TestMethod]
        public void Send_NewServiceRequestStatus_RequestTimeGreaterThanOrEqualToNow()
        {
            var target = new ServiceRequestStatus();
            var now = DateTimeOffset.Now;
            target.Send();
            Assert.IsTrue(target.RequestTime >= now);
        }

        /// <summary>
        /// A test for SendNew
        /// </summary>
        [TestMethod]
        public void RequestState_EmptyServiceRequestStatus_EqualsPending()
        {
            // Empty is the same as creating a new Service Request status.
            var target = ServiceRequestStatus.Empty;
            Assert.AreEqual(ServiceRequestState.Pending, target.RequestState);
        }

        /// <summary>
        /// A test for SendNew
        /// </summary>
        [TestMethod]
        public void Send_NewServiceRequestStatus_InProgressIsTrue()
        {
            var target = new ServiceRequestStatus();
            target.Send();
            Assert.AreEqual(ServiceRequestState.InProgress, target.RequestState);
        }

        #endregion
    }
}