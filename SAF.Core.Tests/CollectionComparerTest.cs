// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionComparerTest.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Core.Tests
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SAF.Testing.Common;

    /// <summary>
    /// This is a test class for CollectionComparerTest and is intended
    /// to contain all CollectionComparerTest Unit Tests
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class CollectionComparerTest
    {
        #region Public Methods and Operators

        /// <summary>
        /// A test for Equals
        /// </summary>
        [TestMethod]
        public void EqualsTest_DifferentObjectTypes_ReturnsFalse()
        {
            var target = new CollectionComparer<FakeTestItem>();
            IEnumerable<FakeTestItem> x = new[]
                                            {
                                                new FakeTestItem { TestInt = 9, TestString = "Test1" }, 
                                                new FakeTestItem { TestInt = 232, TestString = "Test2" }
                                            };
            IEnumerable<FakeTestItem> y = new[]
                                            {
                                                new FakeTestItem { TestInt = 9, TestString = "Test1" }, 
                                                new FakeTestItem { TestInt = 232, TestString = "Test3" }
                                            };

            bool actual = target.Equals(x, y);
            Assert.AreEqual(false, actual);
        }

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

        /// <summary>
        /// A test for Equals
        /// </summary>
        [TestMethod]
        public void EqualsTest_DifferentValueTypes_ReturnsFalse()
        {
            var target = new CollectionComparer<int>();
            IEnumerable<int> x = new[] { 2, 9, 39, 10349 };
            IEnumerable<int> y = new[] { 2, 9, 38, 10349 };
            bool actual = target.Equals(x, y);
            Assert.AreEqual(false, actual);
        }

        /// <summary>
        /// A test for Equals
        /// </summary>
        [TestMethod]
        public void EqualsTest_EquivalentObjectTypes_ReturnsTrue()
        {
            var target = new CollectionComparer<FakeTestItem>();
            IEnumerable<FakeTestItem> x = new[]
                                            {
                                                new FakeTestItem { TestInt = 9, TestString = "Test1" }, 
                                                new FakeTestItem { TestInt = 232, TestString = "Test2" }
                                            };
            IEnumerable<FakeTestItem> y = new[]
                                            {
                                                new FakeTestItem { TestInt = 9, TestString = "Test1" }, 
                                                new FakeTestItem { TestInt = 232, TestString = "Test2" }
                                            };

            bool actual = target.Equals(x, y);
            Assert.AreEqual(true, actual);
        }

        /// <summary>
        /// A test for Equals
        /// </summary>
        [TestMethod]
        public void EqualsTest_EquivalentValueTypes_ReturnsTrue()
        {
            var target = new CollectionComparer<int>();
            IEnumerable<int> x = new[] { 2, 9, 39, 10349 };
            IEnumerable<int> y = new[] { 2, 9, 39, 10349 };
            bool actual = target.Equals(x, y);
            Assert.AreEqual(true, actual);
        }

        #endregion
    }
}