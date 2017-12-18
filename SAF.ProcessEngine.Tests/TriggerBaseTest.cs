// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TriggerBaseTest.cs" company="TractManager">
//   Copyright 2013 TractManager. All rights reserved.
// </copyright>
// <summary>
//   This is a test class for TriggerBaseTest and is intended
//   to contain all TriggerBaseTest Unit Tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ProcessEngine.Tests
{
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for TriggerBaseTest and is intended
    /// to contain all TriggerBaseTest Unit Tests
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TriggerBaseTest
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
        /// A test for IsConditionTrue
        /// </summary>
        [TestMethod]
        public void IsConditionTrue_DefaultCondition_ReturnsTrue()
        {
            var sender = new TestSender(true);
            var args = new TestEventArgs(5);
            var target = new TestTrigger<TestEventArgs>();
            bool expected = true;
            bool actual = target.IsConditionTrue(sender, args);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for IsConditionTrue
        /// </summary>
        [TestMethod]
        public void IsConditionTrue_TrueCondition_ReturnsTrue()
        {
            var sender = new TestSender(true);
            var args = new TestEventArgs(5);
            var target = new TestTrigger<TestEventArgs>((s, e) => e.TestNumber == 5);
            bool expected = true;
            bool actual = target.IsConditionTrue(sender, args);
            Assert.AreEqual(expected, actual);
        }

        #endregion
    }
}