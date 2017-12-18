// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueueAvailabilityComparerTest.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for QueueAvailabilityComparerTest and is intended
    /// to contain all QueueAvailabilityComparerTest Unit Tests
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class QueueAvailabilityComparerTest
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        #endregion

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
        #region Public Methods and Operators

        /// <summary>
        /// A test for Compare
        /// </summary>
        [TestMethod]
        public void Compare_FakeQueues_AbortStateSupersedesAllOtherProperties()
        {
            var target = new QueueAvailabilityComparer<FakeObservableQueue>();
            var x = new FakeObservableQueue
                        {
                            QueueState =
                                new FakeQueueState
                                    {
                                        AverageRequestLatency = TimeSpan.FromMilliseconds(30),
                                        AverageResponseLatency = TimeSpan.FromMilliseconds(20),
                                        FailureRate = 1,
                                        QueueAborted = false
                                    }
                        };

            var y = new FakeObservableQueue
                        {
                            QueueState =
                                new FakeQueueState
                                    {
                                        AverageRequestLatency = TimeSpan.FromMilliseconds(0),
                                        AverageResponseLatency = TimeSpan.FromMilliseconds(0),
                                        FailureRate = 0,
                                        QueueAborted = true
                                    }
                        };

            int actual = target.Compare(x, y);
            Assert.IsTrue(actual < 0);
        }

        /// <summary>
        /// A test for Compare
        /// </summary>
        [TestMethod]
        public void Compare_FakeQueues_LowerFailureRateLowerThanHigherFailureRate()
        {
            var target = new QueueAvailabilityComparer<FakeObservableQueue>();
            var x = new FakeObservableQueue
                        {
                            QueueState =
                                new FakeQueueState
                                    {
                                        AverageRequestLatency = TimeSpan.FromMilliseconds(30),
                                        AverageResponseLatency = TimeSpan.FromMilliseconds(20),
                                        FailureRate = 0.01,
                                        QueueAborted = false
                                    }
                        };

            var y = new FakeObservableQueue
                        {
                            QueueState =
                                new FakeQueueState
                                    {
                                        AverageRequestLatency = TimeSpan.FromMilliseconds(30),
                                        AverageResponseLatency = TimeSpan.FromMilliseconds(20),
                                        FailureRate = 0.011,
                                        QueueAborted = false
                                    }
                        };

            int actual = target.Compare(x, y);
            Assert.IsTrue(actual < 0);
        }

        /// <summary>
        /// A test for Compare
        /// </summary>
        [TestMethod]
        public void Compare_FakeQueues_NonAbortedLessThanAborted()
        {
            var target = new QueueAvailabilityComparer<FakeObservableQueue>();
            var x = new FakeObservableQueue
                        {
                            QueueState =
                                new FakeQueueState
                                    {
                                        AverageRequestLatency = TimeSpan.FromMilliseconds(30),
                                        AverageResponseLatency = TimeSpan.FromMilliseconds(20),
                                        FailureRate = 0,
                                        QueueAborted = false
                                    }
                        };

            var y = new FakeObservableQueue
                        {
                            QueueState =
                                new FakeQueueState
                                    {
                                        AverageRequestLatency = TimeSpan.FromMilliseconds(30),
                                        AverageResponseLatency = TimeSpan.FromMilliseconds(20),
                                        FailureRate = 0,
                                        QueueAborted = true
                                    }
                        };

            int actual = target.Compare(x, y);
            Assert.IsTrue(actual < 0);
        }

        /// <summary>
        /// A test for Compare
        /// </summary>
        [TestMethod]
        public void Compare_FakeQueues_ShorterQueueLengthLessThanLongerQueueLength()
        {
            var target = new QueueAvailabilityComparer<FakeObservableQueue>();
            var x = new FakeObservableQueue
                        {
                            QueueState =
                                new FakeQueueState
                                    {
                                        AverageRequestLatency = TimeSpan.FromMilliseconds(30),
                                        AverageResponseLatency = TimeSpan.FromMilliseconds(20),
                                        FailureRate = 0,
                                        QueueLength = 9,
                                        QueueAborted = false
                                    }
                        };

            var y = new FakeObservableQueue
                        {
                            QueueState =
                                new FakeQueueState
                                    {
                                        AverageRequestLatency = TimeSpan.FromMilliseconds(30),
                                        AverageResponseLatency = TimeSpan.FromMilliseconds(20),
                                        QueueLength = 10,
                                        FailureRate = 0,
                                        QueueAborted = false
                                    }
                        };

            int actual = target.Compare(x, y);
            Assert.IsTrue(actual < 0);
        }

        #endregion
    }
}