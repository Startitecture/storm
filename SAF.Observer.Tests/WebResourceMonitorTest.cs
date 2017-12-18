// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebResourceMonitorTest.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Observer.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The web resource monitor test.
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class WebResourceMonitorTest
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
        /// The check status_ existing web resource_ matches expected.
        /// </summary>
        [TestMethod]
        public void CheckStatus_ExistingWebResource_MatchesExpected()
        {
            const string Name = "http://www.google.com";

            using (var target = new WebResourceMonitor(Name))
            {
                var qualifiedName = String.Concat(target.GetType().FullName, ':', target.Location);
                var expected = new ResourceStatus(qualifiedName, true, 0.0);
                var actual = target.ResourceStatus;
                Assert.AreEqual<string>(expected.QualifiedName, actual.QualifiedName);
                Assert.AreEqual<bool>(expected.IsAvailable, actual.IsAvailable);
            }
        }

        /// <summary>
        /// The check status_ existing web resource_ matches expected.
        /// </summary>
        [TestMethod]
        public void CheckStatus_MissingWebResource_MatchesExpected()
        {
            const string Name = "http://nowhere.servers.tractmanager.com";

            using (var target = new WebResourceMonitor(Name))
            {
                var qualifiedName = String.Concat(target.GetType().FullName, ':', target.Location);
                var expected = new ResourceStatus(qualifiedName, false, 0.0);
                var actual = target.ResourceStatus;
                Assert.AreEqual<string>(expected.QualifiedName, actual.QualifiedName);
                Assert.AreEqual<bool>(expected.IsAvailable, actual.IsAvailable);
            }
        }

        #endregion
    }
}