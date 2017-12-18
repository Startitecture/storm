// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceRouteContainerTests.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing.Tests
{
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Rhino.Mocks;

    using SAF.ActionTracking;
    using SAF.Core;
    using SAF.StringResources;

    /// <summary>
    /// The service route container tests.
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ServiceRouteContainerTests
    {
        /// <summary>
        /// The action event proxy.
        /// </summary>
        private readonly IActionEventProxy actionEventProxy = MockRepository.GenerateMock<IActionEventProxy>();

        #region Public Methods and Operators

        /// <summary>
        /// The resolve_ document queue for text extraction_ matches expected.
        /// </summary>
        [TestMethod]
        public void Resolve_RegisteredQueueRoute_MatchesExpected()
        {
            var expected = typeof(FakeQueueRouteOne);
            var target = new ServiceRouteContainer<FakeMessage>();
            var queueRoute = new FakeQueueRouteOne(this.actionEventProxy);

            target.Register(queueRoute);
            var actual = target.Resolve(typeof(FakeQueueRouteOne).Name);
            Assert.IsInstanceOfType(actual, expected);
        }

        /// <summary>
        /// The resolve_ document queue for text extraction_ matches expected.
        /// </summary>
        [TestMethod]
        public void Resolve_RegisteredQueuePool_MatchesExpected()
        {
            var expected = typeof(PriorityQueuePool<FakeMessage, FakeQueueRouteOne>);
            var target = new ServiceRouteContainer<FakeMessage>();
            var queuePool = new FakeQueuePool<FakeQueueRouteOne>(
                new FakeQueueRouteFactory<FakeQueueRouteOne>(this.actionEventProxy),
                this.actionEventProxy);

            target.Register(queuePool);
            var actual = target.Resolve(typeof(FakeQueueRouteOne).Name);
            Assert.IsInstanceOfType(actual, expected);
        }

        /// <summary>
        /// The resolve_ document queue for text extraction_ matches expected.
        /// </summary>
        [TestMethod]
        public void Resolve_UnregisteredQueuePool_ThrowsException()
        {
            var target = new ServiceRouteContainer<FakeMessage>();

            try
            {
                target.Resolve(typeof(FakeQueueRouteOne).Name);
                Assert.Fail("An exception should have been thrown.");
            }
            catch (BusinessException ex)
            {
                Assert.AreEqual(ex.Message, ValidationMessages.ServiceRouteTypeNotFound, ex.Message);
            }
        }

        /// <summary>
        /// The resolve_ document queue for text extraction_ matches expected.
        /// </summary>
        [TestMethod]
        public void Resolve_InvalidQueueTypeName_ThrowsException()
        {
            var target = new ServiceRouteContainer<FakeMessage>();
            var queuePool = new FakeQueuePool<FakeQueueRouteOne>(
                new FakeQueueRouteFactory<FakeQueueRouteOne>(this.actionEventProxy),
                this.actionEventProxy);

            target.Register(queuePool);

            const string RouteType = "aslfkjslasd";

            try
            {
                target.Resolve(RouteType);
                Assert.Fail("An exception should have been thrown.");
            }
            catch (BusinessException ex)
            {
                Assert.AreEqual(ex.Message, ValidationMessages.ServiceRouteTypeNotFound, ex.Message);
                Assert.AreEqual(ex.TargetEntity, RouteType);
            }
        }

        #endregion
    }
}