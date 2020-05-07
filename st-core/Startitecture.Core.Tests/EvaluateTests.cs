// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EvaluateTests.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The evaluate tests.
    /// </summary>
    [TestClass]
    public class EvaluateTests
    {
        #region Public Methods and Operators

        /// <summary>
        /// A test for Equals
        /// </summary>
        [TestMethod]
        public void Equals_BothNull_ReturnsTrue()
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            // ReSharper disable once EqualExpressionComparison
            var actual = Evaluate.Equals(null, null);
            Assert.IsTrue(actual);
        }

        /// <summary>
        /// A test for Equals
        /// </summary>
        [TestMethod]
        public void Equals_LeftNull_ReturnsFalse()
        {
            var valueB = new FakeTestItem { TestDateTime = DateTime.Now, TestInt = 290, TestString = "blah" };
            var actual = Evaluate.Equals(null, valueB);
            Assert.IsFalse(actual);
        }

        /// <summary>
        /// A test for Equals
        /// </summary>
        [TestMethod]
        public void Equals_RightNull_ReturnsFalse()
        {
            var valueA = new FakeTestItem { TestDateTime = DateTime.Now, TestInt = 290, TestString = "blah" };
            var actual = Evaluate.Equals(valueA, null);
            Assert.IsFalse(actual);
        }

        /// <summary>
        /// A test for Equals
        /// </summary>
        [TestMethod]
        public void Equals_LeftNullWithParameters_ReturnsFalse()
        {
            var valueB = new FakeTestItem { TestDateTime = DateTime.Now, TestInt = 290, TestString = "blah" };
            var actual = Evaluate.Equals(null, valueB, item => item.TestDateTime, item => item.TestInt, item => item.TestString);
            Assert.IsFalse(actual);
        }

        /// <summary>
        /// A test for Equals
        /// </summary>
        [TestMethod]
        public void Equals_RightNullWithParameters_ReturnsFalse()
        {
            var valueA = new FakeTestItem { TestDateTime = DateTime.Now, TestInt = 290, TestString = "blah" };
            var actual = Evaluate.Equals(valueA, null, item => item.TestDateTime, item => item.TestInt, item => item.TestString);
            Assert.IsFalse(actual);
        }

        /// <summary>
        /// A test for Equals
        /// </summary>
        [TestMethod]
        public void Equals_TestObjectsWithEqualProperties_ReturnsTrue()
        {
            var valueA = new FakeTestItem { TestString = "test", TestInt = 1 };
            var valueB = new FakeTestItem { TestString = "test", TestInt = 1 };
            var actual = Evaluate.Equals(valueA, valueB);
            Assert.IsTrue(actual);
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
        /// The recursive equals test.
        /// </summary>
        [TestMethod]
        public void RecursiveEquals_EqualObjects_ReturnsTrue()
        {
            var valueA = new FakeTestItem { TestString = "test", TestInt = 1, TestDateTime = DateTime.MinValue };
            valueA.AddItem("Test1");
            valueA.AddItem("Test2");
            valueA.AddItem("Test3");

            var valueB = new FakeTestItem { TestString = "test", TestInt = 1, TestDateTime = DateTime.MinValue };
            valueB.AddItem("Test1");
            valueB.AddItem("Test2");
            valueB.AddItem("Test3");

            Assert.IsTrue(Evaluate.RecursiveEquals(valueA, valueB));
        }

        /// <summary>
        /// The recursive equals test.
        /// </summary>
        [TestMethod]
        public void RecursiveEquals_UnequalObjectsByProperty_ReturnsFalse()
        {
            var valueA = new FakeTestItem { TestString = "test", TestInt = 1, TestDateTime = DateTime.MinValue };
            valueA.AddItem("Test1");
            valueA.AddItem("Test2");
            valueA.AddItem("Test3");

            var valueB = new FakeTestItem { TestString = "test", TestInt = 4, TestDateTime = DateTime.MinValue };
            valueB.AddItem("Test1");
            valueB.AddItem("Test2");
            valueB.AddItem("Test3");

            Assert.IsFalse(Evaluate.RecursiveEquals(valueA, valueB));
        }

        /// <summary>
        /// The recursive equals test.
        /// </summary>
        [TestMethod]
        public void RecursiveEquals_UnequalObjectsByList_ReturnsFalse()
        {
            var valueA = new FakeTestItem { TestString = "test", TestInt = 1, TestDateTime = DateTime.MinValue };
            valueA.AddItem("Test1");
            valueA.AddItem("Test2");
            valueA.AddItem("Test3");

            var valueB = new FakeTestItem { TestString = "test", TestInt = 1, TestDateTime = DateTime.MinValue };
            valueB.AddItem("Test1");
            valueB.AddItem("Test2");
            valueB.AddItem("Test4");

            Assert.IsFalse(Evaluate.RecursiveEquals(valueA, valueB));
        }

        /// <summary>
        /// A test for GenerateHashCode
        /// </summary>
        [TestMethod]
        public void GenerateHashCode_WithObjectEnumerable_MatchesExpected()
        {
            IEnumerable<object> values = Array.Empty<object>();
            int expected = values.Aggregate(0, (i, o) => (i * 397) + Evaluate.GetHashCode(o));
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
                                                                        dest => dest.TestDateTime,
                                                                        dest => dest.TestInt,
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
            bool actual = Evaluate.IsDefaultValue((int?)null);
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
            bool actual = Evaluate.IsNull((int?)null);
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

        /// <summary>
        /// The compare_ less than value type comparison_ returns less than zero.
        /// </summary>
        [TestMethod]
        public void Compare_LessThanValueTypeComparisonFirstValueNull_ReturnsLessThanZero()
        {
            object y = 11;
            var actual = Evaluate.Compare(null, y);
            Assert.IsTrue(actual < 0);
        }

        /// <summary>
        /// The compare_ less than value type comparison_ returns less than zero.
        /// </summary>
        [TestMethod]
        public void Compare_LessThanValueTypeComparisonSecondValueNullForOverload_ReturnsGreaterThanZero()
        {
            object x = 11;
            var actual = Evaluate.Compare(x, null);
            Assert.IsTrue(actual > 0);
        }

        /// <summary>
        /// The compare_ less than value type comparison_ returns less than zero.
        /// </summary>
        [TestMethod]
        public void Compare_LessThanValueTypeComparisonSecondValueNull_ReturnsGreaterThanZero()
        {
            var x = 11;
            var actual = Evaluate.Compare(x, null);
            Assert.IsTrue(actual < 0);
        }

        /// <summary>
        /// The compare_ equal value type comparison_ returns zero.
        /// </summary>
        [TestMethod]
        public void Compare_EqualValueTypeComparisonBothValuesNullGenericMethod_ReturnsZero()
        {
            var actual = Evaluate.Compare<string>(null, null);
            Assert.IsTrue(actual == 0);
        }

        /// <summary>
        /// The compare_ less than value type comparison_ returns less than zero.
        /// </summary>
        [TestMethod]
        public void Compare_LessThanValueTypeComparison_ReturnsLessThanZero()
        {
            var x = 10;
            object y = 11;
            var actual = Evaluate.Compare(x, y);
            Assert.IsTrue(actual < 0);
        }

        /// <summary>
        /// The compare_ greater than value type comparison_ returns greater than zero.
        /// </summary>
        [TestMethod]
        public void Compare_GreaterThanValueTypeComparison_ReturnsGreaterThanZero()
        {
            var x = 12;
            object y = 11;
            var actual = Evaluate.Compare(x, y);
            Assert.IsTrue(actual > 0);
        }

        /// <summary>
        /// The compare_ equal value type comparison_ returns zero.
        /// </summary>
        [TestMethod]
        public void Compare_EqualValueTypeComparison_ReturnsZero()
        {
            var x = 11;
            object y = 11;
            var actual = Evaluate.Compare(x, y);
            Assert.IsTrue(actual == 0);
        }

        /// <summary>
        /// The compare_ less than object type comparison first expression less than_ returns less than zero.
        /// </summary>
        [TestMethod]
        public void Compare_LessThanObjectTypeComparisonFirstExpressionLessThan_ReturnsLessThanZero()
        {
            var x = new ComparisonClass
            {
                Compared = 10,
                AlsoCompared = "Compare1",
                NotCompared = 239
            };
            var y = new ComparisonClass
            {
                Compared = 11,
                AlsoCompared = "Compare1",
                NotCompared = 94353
            };
            var actual = Evaluate.Compare(x, y, c => c.Compared, c => c.AlsoCompared);
            Assert.IsTrue(actual < 0);
        }

        /// <summary>
        /// The compare_ less than object type comparison second expression less than_ returns less than zero.
        /// </summary>
        [TestMethod]
        public void Compare_LessThanObjectTypeComparisonSecondExpressionLessThan_ReturnsLessThanZero()
        {
            var x = new ComparisonClass
            {
                Compared = 10,
                AlsoCompared = "Compare1",
                NotCompared = 239
            };
            var y = new ComparisonClass
            {
                Compared = 10,
                AlsoCompared = "Compare2",
                NotCompared = 94353
            };
            var actual = Evaluate.Compare(x, y, c => c.Compared, c => c.AlsoCompared);
            Assert.IsTrue(actual < 0);
        }

        /// <summary>
        /// The compare_ less than object type comparison second expression greater than_ returns less than zero.
        /// </summary>
        [TestMethod]
        public void Compare_LessThanObjectTypeComparisonSecondExpressionGreaterThan_ReturnsLessThanZero()
        {
            var x = new ComparisonClass
            {
                Compared = 10,
                AlsoCompared = "Compare2",
                NotCompared = 239
            };
            var y = new ComparisonClass
            {
                Compared = 11,
                AlsoCompared = "Compare1",
                NotCompared = 94353
            };
            var actual = Evaluate.Compare(x, y, c => c.Compared, c => c.AlsoCompared);
            Assert.IsTrue(actual < 0);
        }

        /// <summary>
        /// The compare_ greater than object type comparison first expression greater than_ returns greater than zero.
        /// </summary>
        [TestMethod]
        public void Compare_GreaterThanObjectTypeComparisonFirstExpressionGreaterThan_ReturnsGreaterThanZero()
        {
            var x = new ComparisonClass
            {
                Compared = 12,
                AlsoCompared = "Compare1",
                NotCompared = 239
            };
            var y = new ComparisonClass
            {
                Compared = 11,
                AlsoCompared = "Compare1",
                NotCompared = 94353
            };
            var actual = Evaluate.Compare(x, y, c => c.Compared, c => c.AlsoCompared);
            Assert.IsTrue(actual > 0);
        }

        /// <summary>
        /// The compare_ greater than object type comparison second expression greater than_ returns greater than zero.
        /// </summary>
        [TestMethod]
        public void Compare_GreaterThanObjectTypeComparisonSecondExpressionGreaterThan_ReturnsGreaterThanZero()
        {
            var x = new ComparisonClass
            {
                Compared = 11,
                AlsoCompared = "Compare2",
                NotCompared = 239
            };
            var y = new ComparisonClass
            {
                Compared = 11,
                AlsoCompared = "Compare1",
                NotCompared = 94353
            };
            var actual = Evaluate.Compare(x, y, c => c.Compared, c => c.AlsoCompared);
            Assert.IsTrue(actual > 0);
        }

        /// <summary>
        /// The compare_ greater than object type comparison second expression less than_ returns greater than zero.
        /// </summary>
        [TestMethod]
        public void Compare_GreaterThanObjectTypeComparisonSecondExpressionLessThan_ReturnsGreaterThanZero()
        {
            var x = new ComparisonClass
            {
                Compared = 12,
                AlsoCompared = "Compare1",
                NotCompared = 239
            };
            var y = new ComparisonClass
            {
                Compared = 11,
                AlsoCompared = "Compare2",
                NotCompared = 94353
            };
            var actual = Evaluate.Compare(x, y, c => c.Compared, c => c.AlsoCompared);
            Assert.IsTrue(actual > 0);
        }

        /// <summary>
        /// The compare_ equal object type comparison_ returns zero.
        /// </summary>
        [TestMethod]
        public void Compare_EqualObjectTypeComparison_ReturnsZero()
        {
            var x = new ComparisonClass
            {
                Compared = 11,
                AlsoCompared = "Compare1",
                NotCompared = 239
            };
            var y = new ComparisonClass
            {
                Compared = 11,
                AlsoCompared = "Compare1",
                NotCompared = 239
            };
            var actual = Evaluate.Compare(x, y, c => c.Compared, c => c.AlsoCompared);
            Assert.IsTrue(actual == 0);
        }

        /// <summary>
        /// The compare_ equal object type comparison except non compared property_ returns zero.
        /// </summary>
        [TestMethod]
        public void Compare_EqualObjectTypeComparisonExceptNonComparedProperty_ReturnsZero()
        {
            var x = new ComparisonClass
            {
                Compared = 11,
                AlsoCompared = "Compare1",
                NotCompared = 239
            };
            var y = new ComparisonClass
            {
                Compared = 11,
                AlsoCompared = "Compare1",
                NotCompared = 432532
            };
            var actual = Evaluate.Compare(x, y, c => c.Compared, c => c.AlsoCompared);
            Assert.IsTrue(actual == 0);
        }

        /// <summary>
        /// The comparison class.
        /// </summary>
        private class ComparisonClass
        {
            /// <summary>
            /// Gets or sets the not compared.
            /// </summary>
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public int NotCompared { get; set; }

            /// <summary>
            /// Gets or sets the compared.
            /// </summary>
            public int Compared { get; set; }

            /// <summary>
            /// Gets or sets the also compared.
            /// </summary>
            public string AlsoCompared { get; set; }
        }
    }
}