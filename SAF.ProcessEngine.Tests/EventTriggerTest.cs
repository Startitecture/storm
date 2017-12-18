// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventTriggerTest.cs" company="TractManager">
//   Copyright 2013 TractManager. All rights reserved.
// </copyright>
// <summary>
//   This is a test class for EventTriggerTest and is intended
//   to contain all EventTriggerTest Unit Tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ProcessEngine.Tests
{
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for EventTriggerTest and is intended
    /// to contain all EventTriggerTest Unit Tests
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class EventTriggerTest
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
        /// A test for EventTrigger`1 Constructor
        /// </summary>
        [TestMethod]
        public void IsConditionTrue_DefaultCondition_ReturnsTrue()
        {
            var sender = new TestSender(true);
            var eventArgs = new TestEventArgs(5);
            var target = new EventTrigger<TestEventArgs>((s, e) => ReferenceEquals(s, sender));
            bool expected = true;
            bool actual = target.IsConditionTrue(sender, eventArgs);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for EventTrigger`1 Constructor
        /// </summary>
        [TestMethod]
        public void IsConditionTrue_TrueCondition_ReturnsTrue()
        {
            var sender = new TestSender(true);
            var eventArgs = new TestEventArgs(5);
            var target = new EventTrigger<TestEventArgs>((s, e) => ReferenceEquals(s, sender) && e.TestNumber == 5);
            bool expected = true;
            bool actual = target.IsConditionTrue(sender, eventArgs);
            Assert.AreEqual(expected, actual);
        }

        #endregion
    }
}