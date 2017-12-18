// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessControllerBaseTest.cs" company="TractManager">
//   Copyright 2013 TractManager. All rights reserved.
// </copyright>
// <summary>
//   This is a test class for ProcessControllerBaseTest and is intended
//   to contain all ProcessControllerBaseTest Unit Tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ProcessEngine.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for ProcessControllerBaseTest and is intended
    /// to contain all ProcessControllerBaseTest Unit Tests
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ProcessControllerBaseTest
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
        /// The queue process items and wait for completion.
        /// </summary>
        /// <param name="controller">
        /// The controller.
        /// </param>
        /// <param name="itemsToEnqueue">
        /// The items to enqueue.
        /// </param>
        /// <param name="generator">
        /// The generator.
        /// </param>
        /// <typeparam name="TItem">
        /// </typeparam>
        public static void QueueProcessItemsAndWaitForCompletion<TItem>(
            TestController<TItem> controller, long itemsToEnqueue, Func<TItem> generator)
        {
            var triggerControl = new object();
            bool triggered = false;

            controller.ProcessStopped += delegate
                {
                    lock (triggerControl)
                    {
                        triggered = true;
                        Monitor.Pulse(triggerControl);
                    }
                };

            AddItems(controller, itemsToEnqueue, generator);

            lock (triggerControl)
            {
                triggered = triggered ? triggered : Monitor.Wait(triggerControl);
            }
        }

        /// <summary>
        /// Tests that the stop state is triggered by the test state machine.
        /// </summary>
        [TestMethod]
        public void ProcessController_StartedWithObjects_NotifiesStateChange()
        {
            var controller = new TestController<TimeSpan>();
            var triggerControl = new object();
            bool triggered = false;

            controller.StateChanged += delegate
                {
                    lock (triggerControl)
                    {
                        triggered = true;
                        Monitor.Pulse(triggerControl);
                    }
                };

            controller.StartEngine(new[] { new TimeSpan(), new TimeSpan(), new TimeSpan() });

            lock (triggerControl)
            {
                triggered = triggered ? triggered : Monitor.Wait(triggerControl, 1000);
            }

            Assert.IsTrue(triggered);
        }

        /// <summary>
        /// Tests that the start state is triggered by the test state machine.
        /// </summary>
        [TestMethod]
        public void ProcessController_StartedWithObjects_TriggersStart()
        {
            var controller = new TestController<TimeSpan>();
            var triggerControl = new object();
            bool triggered = false;

            controller.ProcessStarted += delegate
                {
                    lock (triggerControl)
                    {
                        triggered = true;
                        Monitor.Pulse(triggerControl);
                    }
                };

            controller.StartEngine(new TimeSpan(), new TimeSpan(), new TimeSpan());

            lock (triggerControl)
            {
                triggered = triggered ? triggered : Monitor.Wait(triggerControl, 1000);
            }

            Assert.IsTrue(triggered);
        }

        /// <summary>
        /// Tests that the stop state is triggered by the test state machine.
        /// </summary>
        [TestMethod]
        public void ProcessController_StartedWithObjects_TriggersStop()
        {
            var controller = new TestController<TimeSpan>();
            var triggerControl = new object();
            bool triggered = false;

            controller.ProcessStopped += delegate
                {
                    lock (triggerControl)
                    {
                        triggered = true;
                        Monitor.Pulse(triggerControl);
                    }
                };

            controller.StartEngine(new TimeSpan(), new TimeSpan(), new TimeSpan());

            lock (triggerControl)
            {
                triggered = triggered ? triggered : Monitor.Wait(triggerControl, 2000);
            }

            Assert.IsTrue(triggered);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The add items.
        /// </summary>
        /// <param name="process">
        /// The process.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <param name="createItem">
        /// The create item.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item to add.
        /// </typeparam>
        private static void AddItems<TItem>(TestController<TItem> process, long count, Func<TItem> createItem)
        {
            for (int i = 0; i < count; i++)
            {
                process.StartEngine(createItem());
            }
        }

        #endregion
    }
}