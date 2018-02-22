// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EvaluateTest.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for EvaluateTest and is intended
    /// to contain all EvaluateTest Unit Tests
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class EvaluateTest
    {
        #region Public Methods and Operators

        /// <summary>
        /// A test for CollectionEquals
        /// </summary>
        [TestMethod]
        public void CollectionEquals_IdenticalObjectArrays_ReturnsTrue()
        {
            var testObject = new FakeTestItem { TestString = "DKDSK", TestInt = 4908393 };
            string testString = "m,jioqw094ur8903";
            double testDouble = 9.393;
            long? testLong = 44839483948;
            long? testLong2 = null;

            var valuesA = new object[] { testString, testObject, null, testDouble, testLong, testLong2 };
            var valuesB = new object[] { testString, testObject, null, testDouble, testLong, testLong2 };
            bool expected = true;
            bool actual;
            actual = Evaluate.CollectionEquals(valuesA, valuesB);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for CollectionEquals
        /// </summary>
        [TestMethod]
        public void CollectionEquals_MisorderedEquivalentArrays_ReturnsFalse()
        {
            var testObject = new FakeTestItem { TestString = "DKDSK", TestInt = 4908393 };
            string testString = "m,jioqw094ur8903";
            double testDouble = 9.393;
            long? testLong = 44839483948;
            long? testLong2 = null;

            var valuesA = new object[] { testObject, testString, null, testDouble, testLong, testLong2 };
            var valuesB = new object[] { testString, testObject, null, testDouble, testLong, testLong2 };
            bool expected = true;
            bool actual;
            actual = Evaluate.CollectionEquals(valuesA, valuesB);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for Equals
        /// </summary>
        [TestMethod]
        public void Equals_BothNull_ReturnsTrue()
        {
            FakeTestItem valueA = null;
            FakeTestItem valueB = null;
            bool expected = true;
            bool actual;
            actual = Evaluate.Equals(valueA, valueB);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for Equals
        /// </summary>
        [TestMethod]
        public void Equals_LeftNull_ReturnsFalse()
        {
            FakeTestItem valueA = null;
            var valueB = new FakeTestItem { TestDateTime = DateTime.Now, TestInt = 290, TestString = "blah" };
            bool expected = false;
            bool actual;
            actual = Evaluate.Equals(valueA, valueB);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for Equals
        /// </summary>
        [TestMethod]
        public void Equals_RightNull_ReturnsFalse()
        {
            var valueA = new FakeTestItem { TestDateTime = DateTime.Now, TestInt = 290, TestString = "blah" };
            FakeTestItem valueB = null;
            bool expected = false;
            bool actual;
            actual = Evaluate.Equals(valueA, valueB);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for Equals
        /// </summary>
        [TestMethod]
        public void Equals_LeftNullWithParameters_ReturnsFalse()
        {
            FakeTestItem valueA = null;
            var valueB = new FakeTestItem { TestDateTime = DateTime.Now, TestInt = 290, TestString = "blah" };
            bool expected = false;
            bool actual;
            actual = Evaluate.Equals(valueA, valueB, item => item.TestDateTime, item => item.TestInt, item => item.TestString);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for Equals
        /// </summary>
        [TestMethod]
        public void Equals_RightNullWithParameters_ReturnsFalse()
        {
            var valueA = new FakeTestItem { TestDateTime = DateTime.Now, TestInt = 290, TestString = "blah" };
            FakeTestItem valueB = null;
            bool expected = false;
            bool actual;
            actual = Evaluate.Equals(valueA, valueB, item => item.TestDateTime, item => item.TestInt, item => item.TestString);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for Equals
        /// </summary>
        [TestMethod]
        public void Equals_TestObjectsWithEqualProperties_ReturnsTrue()
        {
            var valueA = new FakeTestItem { TestString = "test", TestInt = 1 };
            var valueB = new FakeTestItem { TestString = "test", TestInt = 1 };
            bool expected = true;
            bool actual;
            actual = Evaluate.Equals(valueA, valueB);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for Equals
        /// </summary>
        [TestMethod]
        public void Equals_TestObjectsWithSpecificEqualProperties_ReturnsTrue()
        {
            var valueA = new FakeTestItem { TestString = "test", TestInt = 1, TestDateTime = DateTime.MinValue };
            var valueB = new FakeTestItem { TestString = "test", TestInt = 4, TestDateTime = DateTime.MinValue };
            bool actual = Evaluate.Equals(valueA, valueB, o => o.TestString, o => o.TestDateTime);
            Assert.AreEqual(true, actual);
        }

        /// <summary>
        /// A test for GenerateHashCode
        /// </summary>
        [TestMethod]
        public void GenerateHashCode_WithObjectEnumerable_MatchesExpected()
        {
            IEnumerable<object> values = new object[] { };
            int expected = values.Aggregate(0, (i, o) => (i * 397) + Evaluate.GetHashCode<object>(o));
            int actual = Evaluate.GenerateHashCode(values);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for GenerateHashCode
        /// </summary>
        [TestMethod]
        public void GenerateHashCode_WithParamsArray_MatchesExpected()
        {
            var values = new object[] { DateTime.Today, 2342, "asdsdfa2342345" };
            int expected = values.Aggregate(0, (i, o) => (i * 397) + Evaluate.GetHashCode(o));
            int actual = Evaluate.GenerateHashCode(values);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for GenerateHashCode
        /// </summary>
        [TestMethod]
        public void GenerateHashCode_WithSelectors_MatchesExpected()
        {
            var item = new FakeTestItem { TestDateTime = DateTime.Today, TestInt = 2342, TestString = "asdsdfa2342345" };
            IEnumerable<Func<FakeTestItem, object>> selectors = new Func<FakeTestItem, object>[]
                                                                  {
                                                                      dest => dest.TestDateTime, dest => dest.TestInt, 
                                                                      dest => dest.TestString
                                                                  };

            int expected = new object[] { item.TestDateTime, item.TestInt, item.TestString }.Aggregate(
                0, (i, o) => (i * 397) + Evaluate.GetHashCode(o));

            int actual = Evaluate.GenerateHashCode(item, selectors);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for GetHashCode
        /// </summary>
        [TestMethod]
        public void GetHashCode_NullValue_ReturnsZero()
        {
            int expected = 0;
            int actual = Evaluate.GetHashCode(default(FakeTestItem));
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for GetHashCode
        /// </summary>
        [TestMethod]
        public void GetHashCode_TestObject_ReturnsHashCode()
        {
            var value = new FakeTestItem { TestInt = 23252, TestString = "SomeString" };
            int expected = value.GetHashCode();
            int actual = Evaluate.GetHashCode(value);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for IsDefaultValue
        /// </summary>
        [TestMethod]
        public void IsDefaultValue_DefaultValueOfObjectType_ReturnsTrue()
        {
            bool actual = Evaluate.IsDefaultValue(default(FakeTestItem));
            Assert.AreEqual(true, actual);
        }

        /// <summary>
        /// A test for IsDefaultValue
        /// </summary>
        [TestMethod]
        public void IsDefaultValue_DefaultValueOfValueType_ReturnsTrue()
        {
            bool actual = Evaluate.IsDefaultValue(default(int));
            Assert.AreEqual(true, actual);
        }

        /// <summary>
        /// A test for IsDefaultValue
        /// </summary>
        [TestMethod]
        public void IsDefaultValue_NonDefaultValueOfObjectType_ReturnsFalse()
        {
            bool actual = Evaluate.IsDefaultValue(new FakeTestItem { TestInt = 22, TestString = "asdlfkj" });
            Assert.AreEqual(false, actual);
        }

        /// <summary>
        /// A test for IsDefaultValue
        /// </summary>
        [TestMethod]
        public void IsDefaultValue_NonDefaultValueOfValueType_ReturnsFalse()
        {
            bool actual = Evaluate.IsDefaultValue(77);
            Assert.AreEqual(false, actual);
        }

        /// <summary>
        /// A test for IsDefaultValue
        /// </summary>
        [TestMethod]
        public void IsDefaultValue_NullValueForNullableType_ReturnsTrue()
        {
            int? nullableValue = null;
            bool actual = Evaluate.IsDefaultValue(nullableValue);
            Assert.AreEqual(true, actual);
        }

        /// <summary>
        /// A test for IsNull
        /// </summary>
        [TestMethod]
        public void IsNull_DefaultValueOfValueType_ReturnsFalse()
        {
            bool actual = Evaluate.IsNull(default(int));
            Assert.AreEqual(false, actual);
        }

        /// <summary>
        /// A test for IsDefaultValue
        /// </summary>
        [TestMethod]
        public void IsNull_NullValue_ReturnsTrue()
        {
            bool actual = Evaluate.IsDefaultValue(default(FakeTestItem));
            Assert.AreEqual(true, actual);
        }

        /// <summary>
        /// A test for IsDefaultValue
        /// </summary>
        [TestMethod]
        public void IsNull_SetValue_ReturnsFalse()
        {
            var value = new FakeTestItem { TestInt = 1, TestString = "blah" };
            bool actual = Evaluate.IsDefaultValue(value);
            Assert.AreEqual(false, actual);
        }

        /// <summary>
        /// A test for IsSet
        /// </summary>
        [TestMethod]
        public void IsNull_DefaultValueOfNullableType_ReturnsTrue()
        {
            int? nullableValue = null;
            bool actual = Evaluate.IsNull(nullableValue);
            Assert.AreEqual(true, actual);
        }

        /// <summary>
        /// A test for IsSet
        /// </summary>
        [TestMethod]
        public void IsSet_DefaultValueOfObjectType_ReturnsFalse()
        {
            bool actual = Evaluate.IsSet(default(FakeTestItem));
            Assert.AreEqual(false, actual);
        }

        /// <summary>
        /// A test for IsSet
        /// </summary>
        [TestMethod]
        public void IsSet_DefaultValueOfValueType_ReturnsTrue()
        {
            bool actual = Evaluate.IsSet(default(int));
            Assert.AreEqual(true, actual);
        }

        /// <summary>
        /// A test for IsSet
        /// </summary>
        [TestMethod]
        public void IsSet_NonDefaultValueOfObjectType_ReturnsTrue()
        {
            bool actual = Evaluate.IsSet(new FakeTestItem { TestInt = 3, TestString = "Test" });
            Assert.AreEqual(true, actual);
        }

        /// <summary>
        /// A test for IsSet
        /// </summary>
        [TestMethod]
        public void IsSet_NonDefaultValueOfValueType_ReturnsTrue()
        {
            bool actual = Evaluate.IsSet(7);
            Assert.AreEqual(true, actual);
        }

        /// <summary>
        /// A test for IsSet
        /// </summary>
        [TestMethod]
        public void IsSet_DefaultValueOfNullableType_ReturnsFalse()
        {
            bool actual = Evaluate.IsSet((int?)null);
            Assert.AreEqual(false, actual);
        }

        #endregion
    }
}