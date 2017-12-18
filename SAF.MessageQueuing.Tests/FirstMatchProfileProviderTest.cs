// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FirstMatchProfileProviderTest.cs" company="TractManager, Inc.">
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
    /// This is a test class for FirstMatchProfileProviderTest and is intended
    /// to contain all FirstMatchProfileProviderTest Unit Tests
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class FirstMatchProfileProviderTest
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
        /// A test for ResolveProfile
        /// </summary>
        [TestMethod]
        public void ResolveProfile_FakeRequestForCompletion_ResolvesFakeCompletionProfile()
        {
            var actionEventProxy = MockRepository.GenerateMock<IActionEventProxy>();
            var expected = new FakeCompletionProfile(actionEventProxy);
            var target = new FakeRoutingProvider(new FakeFailureQueueRoute(actionEventProxy), expected, new FakeTwoRouteProfile(actionEventProxy));
            var message = Generate.PriorityMessage(
                "Name", DateTimeOffset.Now, DateTimeOffset.Now.Date.AddDays(1).AddHours(7), TimeSpan.FromHours(3), 1);

            var actual = target.ResolveProfile(message);
            Assert.AreSame(expected, actual);
        }

        #endregion
    }
}