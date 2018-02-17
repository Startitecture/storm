// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethodsTests.cs" company="">
//   
// </copyright>
// <summary>
//   The extension methods tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Testing.Common.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Core;

    /// <summary>
    /// The extension methods tests.
    /// </summary>
    [TestClass]
    public class ExtensionMethodsTests
    {
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
            var propertiesToCompare = new string[0];
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
            var propertiesToCompare = new string[0];
            IEnumerable<PropertyComparisonResult> expected = new List<PropertyComparisonResult>();
            IEnumerable<PropertyComparisonResult> actual = baseline.GetDifferences(comparison, propertiesToCompare);
            CollectionAssert.AreEqual(expected.ToList(), actual.ToList());
        }
    }
}