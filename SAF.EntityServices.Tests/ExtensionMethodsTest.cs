// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethodsTest.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.EntityServices.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.ServiceModel;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SAF.Testing.Common;

    /// <summary>
    /// This is a test class for ExtensionMethodsTest and is intended
    /// to contain all ExtensionMethodsTest Unit Tests
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ExtensionMethodsTest
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
        /// A test for GetActionFault
        /// </summary>
        [TestMethod]
        public void GetActionFault_FaultExceptionBase_ReturnsNull()
        {
            var exception = new FaultException();
            IActionFault actual = exception.GetActionFault();
            Assert.IsNull(actual);
        }

        /// <summary>
        /// A test for GetActionFault
        /// </summary>
        [TestMethod]
        public void GetActionFault_FaultExceptionWithActionFault_MatchesExpectedType()
        {
            FaultException exception = new InvalidOperationException("A test.").ToFaultException<InternalOperationFault>();
            IActionFault actual = exception.GetActionFault();
            Assert.IsInstanceOfType(actual, typeof(InternalOperationFault));
        }

        /// <summary>
        /// A test for GetActionFault
        /// </summary>
        [TestMethod]
        public void GetActionFault_FaultExceptionWithActionFault_ReturnsNull()
        {
            var exception = new FaultException<FakeDto>(new FakeDto { FakeEntityId = 1, Description = "Test" });
            IActionFault actual = exception.GetActionFault();
            Assert.IsNull(actual);
        }

        /// <summary>
        /// A test for IsActionFault
        /// </summary>
        [TestMethod]
        public void IsActionFault_FaultExceptionBase_ReturnsFalse()
        {
            var exception = new FaultException();
            bool actual = exception.IsActionFault();
            Assert.AreEqual(false, actual);
        }

        /// <summary>
        /// A test for IsActionFault
        /// </summary>
        [TestMethod]
        public void IsActionFault_FaultExceptionWithIActionFault_ReturnsTrue()
        {
            FaultException exception = new InvalidOperationException("A test.").ToFaultException<InternalOperationFault>();
            bool actual = exception.IsActionFault();
            Assert.AreEqual(true, actual);
        }

        /// <summary>
        /// A test for IsActionFault
        /// </summary>
        [TestMethod]
        public void IsActionFault_FaultExceptionWithNonActionFault_ReturnsFalse()
        {
            var exception = new FaultException<FakeDto>(new FakeDto { FakeEntityId = 1, Description = "Test" });
            bool actual = exception.IsActionFault();
            Assert.AreEqual(false, actual);
        }

        #endregion
    }
}