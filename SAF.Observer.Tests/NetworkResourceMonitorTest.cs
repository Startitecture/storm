// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetworkResourceMonitorTest.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Observer.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SAF.Core;

    /// <summary>
    /// The network resource monitor test.
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class NetworkResourceMonitorTest
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
        /// The check status_ existing network resource_ matches expected.
        /// </summary>
        [TestMethod]
        public void CheckStatus_ExistingLocalNetworkResource_MatchesExpected()
        {
            var name = String.Format(@"\\{0}\C$", Environment.MachineName);

            using (var target = new NetworkResourceMonitor(name))
            {
                var qualifiedName = String.Concat(target.GetType().FullName, ':', target.Location);
                var expected = new ResourceStatus(qualifiedName, true, 1);
                var actual = target.ResourceStatus;
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        /// The check status_ existing network resource_ matches expected.
        /// </summary>
        [TestMethod]
        public void CheckStatus_ExistingRemoteNetworkResource_MatchesExpected()
        {
            const string Name = @"\\tractserv\Development";

            using (var target = new NetworkResourceMonitor(Name))
            {
                var qualifiedName = String.Concat(target.GetType().FullName, ':', target.Location);
                var expected = new ResourceStatus(qualifiedName, true, 5);
                var actual = target.ResourceStatus;
                Assert.AreEqual(expected, actual, String.Join(Environment.NewLine, Evaluate.GetDifferences(expected, actual)));
            }
        }

        /// <summary>
        /// The check status_ existing network resource_ matches expected.
        /// </summary>
        [TestMethod]
        public void CheckStatus_MissingRemoteNetworkResource_MatchesExpected()
        {
            const string Name = @"\\storcenter2\NotAShare";

            using (var target = new NetworkResourceMonitor(Name))
            {
                var qualifiedName = String.Concat(target.GetType().FullName, ':', target.Location);
                var expected = new ResourceStatus(qualifiedName, false, 5);
                var actual = target.ResourceStatus;
                Assert.AreEqual(expected, actual);
            }
        }

        #endregion
    }
}