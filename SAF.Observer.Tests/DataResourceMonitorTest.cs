// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataResourceMonitorTest.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Observer.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The data resource monitor test.
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class DataResourceMonitorTest
    {
        #region Public Methods and Operators

        /// <summary>
        /// The initialize test.
        /// </summary>
        [TestInitialize]
        public void InitializeTest()
        {
        }

        /// <summary>
        /// The data resource monitor_ check status.
        /// </summary>
        [TestMethod]
        public void CheckStatus_ExistingDataResource_MatchesExpected()
        {
            const string Name = "TestConnection";

            using (var target = new DataResourceMonitor(Name))
            {
                var qualifiedName = String.Concat(target.GetType().FullName, ':', target.Location);
                var expected = new ResourceStatus(qualifiedName, true, 0.0);
                ResourceStatus actual = target.ResourceStatus;
                Assert.AreEqual<bool>(expected.IsAvailable, actual.IsAvailable);
                Assert.AreEqual<string>(expected.QualifiedName, actual.QualifiedName);
            }
        }

        /// <summary>
        /// The data resource monitor_ check status.
        /// </summary>
        [TestMethod]
        public void CheckStatus_MissingDataResource_MatchesExpected()
        {
            const string Name = "asdfa";

            using (var target = new DataResourceMonitor(Name))
            {
                var qualifiedName = String.Concat(target.GetType().FullName, ':', target.Location);
                var expected = new ResourceStatus(qualifiedName, false, 0.0);
                ResourceStatus actual = target.ResourceStatus;
                Assert.AreEqual<bool>(expected.IsAvailable, actual.IsAvailable);
                Assert.AreEqual<string>(expected.QualifiedName, actual.QualifiedName);
            }
        }

        #endregion
    }
}