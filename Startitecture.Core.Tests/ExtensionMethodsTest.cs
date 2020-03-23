// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethodsTest.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The extension methods test.
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ExtensionMethodsTest
    {
        #region Public Methods and Operators

        /// <summary>
        /// The to property dictionary_ test object all properties_ matches expected.
        /// </summary>
        [TestMethod]
        public void PopulateDictionary_TestObjectAllProperties_MatchesExpected()
        {
            var item = new FakeTestItem { TestDateTime = DateTime.Today, TestInt = 23, TestString = "Test String" };
            IDictionary<string, object> expected = new Dictionary<string, object>
                                                       {
                                                           { "{Item}", item.GetType().FullName },
                                                           { "TestDateTime", item.TestDateTime },
                                                           { "TestInt", item.TestInt },
                                                           { "TestString", item.TestString }
                                                       };

            var actual = new Dictionary<string, object>();
            actual.PopulateDictionary(item);
            string Func(KeyValuePair<string, object> x) => $"{x.Key}={x.Value}";

            var expectedCollection = expected.Select(Func).ToList();
            var actualCollection = actual.Select(Func).ToList();
            CollectionAssert.AreEqual(expectedCollection, actualCollection);
        }

        /// <summary>
        /// A test for GetPropertyValue
        /// </summary>
        [TestMethod]
        public void GetPropertyValue_ExistingProperty_MatchesExpected()
        {
            var propertyInfo = typeof(FakeTestItem).GetProperty("TestString");
            var entity = new FakeTestItem { TestInt = 12, TestString = "Test" };
            const string Expected = "Test";
            var actual = propertyInfo.GetPropertyValue(entity);
            Assert.AreEqual(Expected, Convert.ToString(actual, CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// A test for GetDifferences
        /// </summary>
        [TestMethod]
        public void GetDifferences_AllValuesDifferentWithSpecifiedProperties_MatchesExpected()
        {
            var baseline = new FakeTestItem { TestDateTime = DateTime.MinValue, TestInt = 20, TestString = "TestString" };
            var comparison = new FakeTestItem { TestDateTime = DateTime.MaxValue, TestInt = 21, TestString = "TestString2" };
            var propertiesToCompare = new[] { "TestInt", "TestString" };
            IEnumerable<PropertyComparisonResult> expected = new List<PropertyComparisonResult>
                                                                 {
                                                                     new PropertyComparisonResult
                                                                         {
                                                                             PropertyName
                                                                                 =
                                                                                 "TestInt",
                                                                             OriginalValue
                                                                                 =
                                                                                 20,
                                                                             NewValue
                                                                                 =
                                                                                 21
                                                                         },
                                                                     new PropertyComparisonResult
                                                                         {
                                                                             PropertyName
                                                                                 =
                                                                                 "TestString",
                                                                             OriginalValue
                                                                                 =
                                                                                 "TestString",
                                                                             NewValue
                                                                                 =
                                                                                 "TestString2"
                                                                         }
                                                                 };

            IEnumerable<PropertyComparisonResult> actual = baseline.GetDifferences(comparison, propertiesToCompare);
            CollectionAssert.AreEqual(expected.ToList(), actual.ToList());
        }

        /// <summary>
        /// A test for GetDifferences
        /// </summary>
        [TestMethod]
        public void GetDifferences_AllValuesDifferent_MatchesExpected()
        {
            var baseline = new FakeTestItem { TestInt = 20, TestString = "TestString" };
            var comparison = new FakeTestItem { TestInt = 21, TestString = "TestString2" };
            var propertiesToCompare = Array.Empty<string>();
            IEnumerable<PropertyComparisonResult> expected = new List<PropertyComparisonResult>
                                                                 {
                                                                     new PropertyComparisonResult
                                                                         {
                                                                             PropertyName
                                                                                 =
                                                                                 "TestInt",
                                                                             OriginalValue
                                                                                 =
                                                                                 20,
                                                                             NewValue
                                                                                 =
                                                                                 21
                                                                         },
                                                                     new PropertyComparisonResult
                                                                         {
                                                                             PropertyName
                                                                                 =
                                                                                 "TestString",
                                                                             OriginalValue
                                                                                 =
                                                                                 "TestString",
                                                                             NewValue
                                                                                 =
                                                                                 "TestString2"
                                                                         }
                                                                 };

            IEnumerable<PropertyComparisonResult> actual = baseline.GetDifferences(comparison, propertiesToCompare);
            CollectionAssert.AreEqual(expected.ToList(), actual.ToList());
        }

        /// <summary>
        /// A test for GetDifferences
        /// </summary>
        [TestMethod]
        public void GetDifferences_AllValuesEqual_ReturnsEmpty()
        {
            var baseline = new FakeTestItem { TestInt = 20, TestString = "TestString" };
            var comparison = new FakeTestItem { TestInt = 20, TestString = "TestString" };
            var propertiesToCompare = Array.Empty<string>();
            IEnumerable<PropertyComparisonResult> expected = new List<PropertyComparisonResult>();
            IEnumerable<PropertyComparisonResult> actual = baseline.GetDifferences(comparison, propertiesToCompare);
            CollectionAssert.AreEqual(expected.ToList(), actual.ToList());
        }

        #endregion
    }
}