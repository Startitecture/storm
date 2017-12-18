// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateMachineBaseTest.cs" company="TractManager">
//   Copyright 2013 TractManager. All rights reserved.
// </copyright>
// <summary>
//   This is a test class for EventProcessBaseTest and is intended
//   to contain all EventProcessBaseTest Unit Tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ProcessEngine.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for EventProcessBaseTest and is intended
    /// to contain all EventProcessBaseTest Unit Tests
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class StateMachineBaseTest
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
        /// A test for RegisteredEventHandler
        /// </summary>
        [TestMethod]
        public void StateMachineBase_EventTriggered_EventReceived()
        {
            var target = new TestStateMachine();
            Type expected = typeof(ProcessStartedEventArgs);
            Type actual = null;

            var triggerControl = new object();
            bool triggered = false;

            target.EventReceived += delegate(object o, EventArgs e)
                {
                    actual = e.GetType();

                    lock (triggerControl)
                    {
                        triggered = true;
                        Monitor.Pulse(triggerControl);
                    }
                };

            var engine = new TestEngine<int>();
            target.RegisterProcessStarter(engine);
            engine.Add(1);

            lock (triggerControl)
            {
                triggered = triggered ? true : Monitor.Wait(triggerControl, 1000);
            }

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// The state machine base_ start event triggered_ event received.
        /// </summary>
        [TestMethod]
        public void StateMachineBase_StartEventTriggered_EventReceived()
        {
            var target = new TestStateMachine();
            var triggerControl = new object();
            bool triggered = false;

            target.ProcessStarted += delegate
                {
                    lock (triggerControl)
                    {
                        triggered = true;
                        Monitor.Pulse(triggerControl);
                    }
                };

            var engine = new TestEngine<int>();
            target.RegisterProcessStarter(engine);
            engine.Add(1);

            lock (triggerControl)
            {
                triggered = triggered ? true : Monitor.Wait(triggerControl, 100);
            }

            Assert.IsTrue(triggered, "Test timed out.");
        }

        /// <summary>
        /// The state machine base_ stop event triggered_ event received.
        /// </summary>
        [TestMethod]
        public void StateMachineBase_StopEventTriggered_EventReceived()
        {
            var target = new TestStateMachine();
            var triggerControl = new object();
            bool triggered = false;

            target.ProcessStopped += delegate
                {
                    lock (triggerControl)
                    {
                        triggered = true;
                        Monitor.Pulse(triggerControl);
                    }
                };

            var engine = new TestEngine<int>();

            engine.ItemsProduced += delegate
                {
                    while (engine.TaskResultConsumer.ConsumeNext())
                    {
                    }
                };

            target.RegisterProcessStarter(engine);
            target.RegisterProcessStopper(engine);
            engine.Add(1);

            lock (triggerControl)
            {
                triggered = triggered ? true : Monitor.Wait(triggerControl, 500);
            }

            Assert.IsTrue(triggered, "Test timed out.");
        }

        #endregion
    }
}