// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComponentAvailabilityComparerTest.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.MessageQueuing.Tests
{
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The component availability comparer test.
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ComponentAvailabilityComparerTest
    {
        #region Public Methods and Operators

        /// <summary>
        /// The compare_ component uptime availability_ higher uptime is lower value.
        /// </summary>
        [TestMethod]
        public void Compare_ComponentByTotalWeight_LowerWeightIsLowerValue()
        {
            var target = ComponentAvailabilityComparer.TotalWeight;
            var baselineMonitor1 = new FakeResourceMonitor("Location1");
            var baselineMonitor2 = new FakeResourceMonitor("Location2");
            var baselineMonitor3 = new FakeResourceMonitor("Location3");
            var baseline = new FakeComponentMonitor(
                "Baseline", 
                baselineMonitor1, 
                baselineMonitor2, 
                baselineMonitor3);

            var comparisonMonitor1 = new FakeResourceMonitor("Location1");
            var comparisonMonitor2 = new FakeResourceMonitor("Location2");
            var comparisonMonitor3 = new FakeResourceMonitor("Location3");
            var comparison = new FakeComponentMonitor(
                "Comparison", 
                comparisonMonitor1, 
                comparisonMonitor2, 
                comparisonMonitor3);

            baselineMonitor1.Weight = 5.0;
            baselineMonitor2.Weight = 4.0;
            baselineMonitor3.Weight = 2.0;
            comparisonMonitor1.Weight = 1.0;
            comparisonMonitor2.Weight = 2.0;
            comparisonMonitor3.Weight = 8.5;
            var actual = target.Compare(baseline.Status, comparison.Status);
            Assert.IsTrue(actual < 0, "{0} >= {1}", baseline.Status, comparison.Status);
        }

        /// <summary>
        /// The compare_ component uptime availability_ higher uptime is lower value.
        /// </summary>
        [TestMethod]
        public void Compare_ComponentByRankedWeight_LowerRankedWeightIsLowerValue()
        {
            var target = ComponentAvailabilityComparer.RankedWeight;
            var baselineMonitor1 = new FakeResourceMonitor("Location1");
            var baselineMonitor2 = new FakeResourceMonitor("Location2");
            var baselineMonitor3 = new FakeResourceMonitor("Location3");
            var baseline = new FakeComponentMonitor(
                "Baseline", 
                baselineMonitor1, 
                baselineMonitor2, 
                baselineMonitor3);

            var comparisonMonitor1 = new FakeResourceMonitor("Location1");
            var comparisonMonitor2 = new FakeResourceMonitor("Location2");
            var comparisonMonitor3 = new FakeResourceMonitor("Location3");
            var comparison = new FakeComponentMonitor(
                "Comparison", 
                comparisonMonitor1, 
                comparisonMonitor2, 
                comparisonMonitor3);

            baselineMonitor1.Weight = 1.0; // Same as comparison, should continue to rank #2.
            baselineMonitor2.Weight = 2.0; // Less than comparison, should return at this point.
            baselineMonitor3.Weight = 10.0; // Greater than comparison, should not be reached.
            comparisonMonitor1.Weight = 1.0;
            comparisonMonitor2.Weight = 4.0;
            comparisonMonitor3.Weight = 3.5;
            var actual = target.Compare(baseline.Status, comparison.Status);
            Assert.IsTrue(actual < 0, "{0} >= {1}", baseline.Status, comparison.Status);
        }

        /// <summary>
        /// The compare_ component uptime availability_ higher uptime is lower value.
        /// </summary>
        [TestMethod]
        public void Compare_ComponentByRankedTotalWeight_LowerRankedTotalWeightIsLowerValue()
        {
            var target = ComponentAvailabilityComparer.RankedTotalWeight;
            var baselineMonitor1 = new FakeResourceMonitor("Location1");
            var baselineMonitor2 = new FakeResourceMonitor("Location2");
            var baselineMonitor3 = new FakeResourceMonitor("Location3");
            var baseline = new FakeComponentMonitor(
                "Baseline", 
                baselineMonitor1, 
                baselineMonitor2, 
                baselineMonitor3);

            var comparisonMonitor1 = new FakeResourceMonitor("Location1");
            var comparisonMonitor2 = new FakeResourceMonitor("Location2");
            var comparisonMonitor3 = new FakeResourceMonitor("Location3");
            var comparison = new FakeComponentMonitor(
                "Comparison", 
                comparisonMonitor1, 
                comparisonMonitor2, 
                comparisonMonitor3);

            baselineMonitor1.Weight = 1.0; 
            baselineMonitor2.Weight = 3.0; 
            baselineMonitor3.Weight = 6.0; 
            comparisonMonitor1.Weight = 3.0; 
            comparisonMonitor2.Weight = 6.0; 
            comparisonMonitor3.Weight = 1.0; 
            var actual = target.Compare(baseline.Status, comparison.Status);
            Assert.IsTrue(actual < 0, "{0} >= {1}", baseline.Status, comparison.Status);
        }

        #endregion
    }
}