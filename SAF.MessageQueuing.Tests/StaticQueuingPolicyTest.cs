// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StaticQueuingPolicyTest.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.MessageQueuing.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SAF.StringResources;

    /// <summary>
    /// The static queuing policy test.
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class StaticQueuingPolicyTest
    {
        #region Public Methods and Operators

        /// <summary>
        /// The allow queue creation test.
        /// </summary>
        [TestMethod]
        public void AllowQueueCreation_NoActiveQueues_ReturnsTrue()
        {
            var target = new StaticQueuingPolicy();
            var poolState = new FakePoolState
                                {
                                    Name = "FakePoolState", 
                                    QueueCount = 0
                                };

            var expected = new PolicyDecision(true, String.Format(ActionMessages.AllowQueueCreationForEmptyPool, poolState.Name));
            var actual = target.AllowQueueCreation(poolState);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// The allow queue creation test.
        /// </summary>
        [TestMethod]
        public void AllowQueueCreation_AllQueuesActive_ReturnsFalse()
        {
            var target = new StaticQueuingPolicy { MaxQueueCount = 1 };
            var poolState = new FakePoolState
                                {
                                    Name = "FakePoolState", 
                                    QueueCount = target.MaxQueueCount, 
                                    PoolStates =
                                        new List<IPriorityQueueState>(
                                        Enumerable.Repeat(new FakeQueueState { QueueLength = 1 }, target.MaxQueueCount))
                                };

            var expected = new PolicyDecision(
                false, 
                String.Format(ActionMessages.DenyQueueCreationForMaxCount, poolState.Name, poolState.QueueCount, target.MaxQueueCount));

            var actual = target.AllowQueueCreation(poolState);
            Assert.AreEqual(expected, actual);
        }

        #endregion
    }
}