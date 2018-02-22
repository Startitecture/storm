// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CacheTests.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Core.Tests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The cache tests.
    /// </summary>
    [TestClass]
    public class CacheTests
    {
        /// <summary>
        /// The get test.
        /// </summary>
        [TestMethod]
        public void Get_IntegerFromCache_HitCountEqualToOne()
        {
            var target = new Cache<string, int>();
            int hitCount = 0;

            int Factory()
            {
                hitCount++;
                return 21 + 45;
            }

            target.Get("foo", Factory);
            target.Get("foo", Factory);

            Assert.AreEqual(1, hitCount);
        }

        /// <summary>
        /// The get test.
        /// </summary>
        [TestMethod]
        public void Get_IntegerFromCache_ValueMatchesExpected()
        {
            var target = new Cache<string, int>();

            int Factory()
            {
                return 21 + 45;
            }

            target.Get("foo", Factory);
            var actual = target.Get("foo", Factory);

            Assert.AreEqual(21 + 45, actual);
        }
    }
}