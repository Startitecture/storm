// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemProducerTest.cs" company="TractManager">
//   Copyright 2013 TractManager. All rights reserved.
// </copyright>
// <summary>
//   This is a test class for ItemProducerTest and is intended
//   to contain all ItemProducerTest Unit Tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ProcessEngine.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for ItemProducerTest and is intended
    /// to contain all ItemProducerTest Unit Tests
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ItemProducerTest
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        #endregion

        #region Public Methods and Operators

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

        /// <summary>
        /// The item producer_ multiple items_ start state triggered.
        /// </summary>
        [TestMethod]
        public void ItemProducer_MultipleItems_StartStateTriggered()
        {
            bool started = false;
            var target = new ItemProducer<DateTime>();
            bool completed = false;
            bool exitedBeforeTimeout;
            const long ItemsToAdd = 7;

            target.ProcessStarted += delegate { started = true; };

            target.ItemsProduced += delegate
                {
                    while (target.ItemQueueConsumer.ConsumeNext())
                    {
                        // Consume the items and move on.
                    }
                };

            target.ProcessStopped += delegate
                {
                    lock (target)
                    {
                        completed = true;
                        Monitor.Pulse(target);
                    }
                };

            for (int i = 0; i < ItemsToAdd; i++)
            {
                target.ProduceItem(new DateTime(i));
            }

            lock (target)
            {
                exitedBeforeTimeout = completed ? completed : Monitor.Wait(target, 2000, true);
            }

            Assert.IsTrue(started);
            Assert.IsTrue(exitedBeforeTimeout, "The test timed out.");
        }

        /// <summary>
        /// The item producer multiple items stop state triggered.
        /// </summary>
        [TestMethod]
        public void ItemProducer_MultipleItems_StopStateTriggered()
        {
            var target = new ItemProducer<DateTime>();
            bool completed = false;
            bool exitedBeforeTimeout;
            const long ItemsToAdd = 7;

            target.ItemsProduced += delegate
                {
                    while (target.ItemQueueConsumer.ConsumeNext())
                    {
                        // Consume the items and move on.
                    }
                };

            target.ProcessStopped += delegate
                {
                    lock (target)
                    {
                        completed = true;
                        Monitor.Pulse(target);
                    }
                };

            for (int i = 0; i < ItemsToAdd; i++)
            {
                target.ProduceItem(new DateTime(i));
            }

            lock (target)
            {
                exitedBeforeTimeout = completed ? completed : Monitor.Wait(target, 2000, true);
            }

            Assert.IsTrue(exitedBeforeTimeout);
        }

        /// <summary>
        /// The cancel item producer in progress canceled and completed items equal queued.
        /// </summary>
        [TestMethod]
        public void Cancel_ItemProducer_CanceledAndCompletedItemsEqualQueued()
        {
            var target = new ItemProducer<int>();
            var consumed = new List<int>();

            target.ItemsProduced += (sender, args) =>
                {
                    var producer = sender as ItemProducer<int>;

                    if (producer != null)
                    {
                        while (producer.ItemQueueConsumer.ConsumeNext())
                        {
                            consumed.Add(producer.ItemQueueConsumer.Current);
                            Thread.Sleep(1);
                        }
                    }
                };

            for (int i = 0; i < 100; i++)
            {
                target.ProduceItem(i);
            }

            if (Debugger.IsAttached)
            {
                target.Cancel();
            }
            else
            {
                Assert.IsTrue(target.Cancel(TimeSpan.FromSeconds(5)), "The test timed out.");
            }

            Assert.AreEqual(100, consumed.Count + target.QueuedItems.Count());
        }

        /// <summary>
        /// The cancel item producer in progress canceled and completed items equal queued.
        /// </summary>
        [TestMethod]
        public void Cancel_ItemProducerInProgress_ThrowsException()
        {
            var target = new ItemProducer<int>();
            var consumed = new List<int>();

            target.ItemsProduced += (sender, args) =>
            {
                var producer = sender as ItemProducer<int>;

                if (producer != null)
                {
                    while (producer.ItemQueueConsumer.ConsumeNext())
                    {
                        consumed.Add(producer.ItemQueueConsumer.Current);
                        Thread.Sleep(1);
                    }
                }
            };

            for (int i = 0; i < 100; i++)
            {
                target.ProduceItem(i);
            }

            if (Debugger.IsAttached)
            {
                target.Cancel();
            }
            else
            {
                Assert.IsTrue(target.Cancel(TimeSpan.FromSeconds(5)), "The test timed out.");
            }

            try
            {
                target.ProduceItem(1001);
                Assert.Fail("An exception should have been thrown.");
            }
            catch (ComponentAbortedException)
            {
            }
        }

        /// <summary>
        /// The cancel item producer in progress canceled and completed items equal queued.
        /// </summary>
        [TestMethod]
        public void ProduceItem_WithUnhandledException_AbortsWithRemainingItems()
        {
            var target = new ItemProducer<string>();
            var consumed = new List<string>();
            QueueAbortException<string> abortException = null;

            ////target.Aborted += (sender, args) =>
            ////{
            ////    abortedEvent = args; 
            ////};

            target.ProcessStopped += (sender, args) => abortException = args.EventError as QueueAbortException<string>;

            target.ItemsProduced += (sender, args) =>
            {
                var producer = sender as ItemProducer<string>;

                if (producer == null)
                {
                    return;
                }

                while (producer.ItemQueueConsumer.ConsumeNext())
                {
                    if (consumed.Count == 50)
                    {
                        throw new InvalidOperationException("Blow up the queue.");
                    }

                    consumed.Add(producer.ItemQueueConsumer.Current);
                    Thread.Sleep(1);
                }
            };

            for (int i = 0; i < 100; i++)
            {
                try
                {
                    target.ProduceItem(i.ToString("D"));
                }
                catch (ComponentAbortedException)
                {
                    break;
                }
            }

            if (Debugger.IsAttached)
            {
                target.WaitForCompletion();
            }
            else
            {
                Assert.IsTrue(target.WaitForCompletion(TimeSpan.FromSeconds(5)), "The test timed out.");
            }

            Assert.IsNotNull(abortException, "The abort event was null.");
            Assert.IsNotNull(abortException.AbortedItem, "The aborted request was null.");
            Assert.IsNotNull(abortException.PendingItems, "The pending requests were null.");
            Assert.AreEqual(target.ItemsAdded, consumed.Count + 1 + abortException.PendingItems.Count());
        }

        /// <summary>
        /// The produce item_ count greater than item limit_ waiting items limited.
        /// </summary>
        [TestMethod]
        public void ProduceItem_CountGreaterThanItemLimit_WaitingItemsLimited()
        {
            var target = new ItemProducer<int> { MaxQueueLength = 5 };
            const int ExpectedItems = 50;
            int actualItems = 0;
            int preFailures = 0;
            int postFailures = 0;
            bool completed = false;

            target.ItemsProduced += delegate
                {
                    long waiting = target.ItemsPending;
                    long limit = target.MaxQueueLength;
                    Trace.TraceInformation("pre:Waiting Items = {0}, ItemLimit = {1}", waiting, limit);

                    if (waiting > limit)
                    {
                        preFailures++;
                    }

                    Thread.Sleep(20);

                    while (target.ItemQueueConsumer.ConsumeNext())
                    {
                        actualItems++;
                    }

                    waiting = target.ItemsPending;
                    limit = target.MaxQueueLength;
                    Trace.TraceInformation("post:Waiting Items = {0}, ItemLimit = {1}", waiting, limit);

                    if (waiting > limit)
                    {
                        postFailures++;
                    }

                    lock (target)
                    {
                        if (ExpectedItems == actualItems)
                        {
                            completed = true;
                            Monitor.Pulse(target);
                        }
                    }
                };

            for (int i = 0; i < ExpectedItems; i++)
            {
                target.ProduceItem(i);
            }

            lock (target)
            {
                completed = completed ? completed : Monitor.Wait(target, 2000, true);
            }

            Assert.IsTrue(completed, "The test timed out.");
            Assert.IsTrue(preFailures == 0, "{0} pre-failures", preFailures);
            Assert.IsTrue(postFailures == 0, "{0} post-failures", postFailures);
        }

        /// <summary>
        /// A test for RetrieveItems
        /// </summary>
        [TestMethod]
        public void ProduceItem_MultipleItems_EventDoesNotFireOnEmptyQueue()
        {
            var target = new ItemProducer<DateTime>();

            bool completed = false;
            const int Produced = 50;
            int retrieved = 0;
            const int Expected = 0;
            int actual = 0;

            target.ItemsProduced += delegate
                {
                    bool leastOneRetrieved = false;

                    while (target.ItemQueueConsumer.ConsumeNext())
                    {
                        leastOneRetrieved = true;
                        retrieved++;
                    }

                    if (!leastOneRetrieved)
                    {
                        actual++;
                    }

                    lock (target)
                    {
                        if (retrieved == Produced)
                        {
                            completed = true;
                            Monitor.Pulse(target);
                        }
                    }
                };

            for (int i = 0; i < Produced; i++)
            {
                target.ProduceItem(new DateTime());
            }

            lock (target)
            {
                completed = completed ? completed : Monitor.Wait(target, 1000, true);
            }

            Assert.IsTrue(completed, "The test timed out.");
            Assert.AreEqual(Expected, actual, "There were {0} empty retrieval events.", actual);
        }

        /// <summary>
        /// A test for RetrieveItems
        /// </summary>
        [TestMethod]
        public void RetrieveItems_MultipleItems_ReturnsAllItems()
        {
            var target = new ItemProducer<DateTime>();
            bool completed = false;
            const int Expected = 500;
            int actual = 0;

            target.ItemsProduced += delegate
                {
                    while (target.ItemQueueConsumer.ConsumeNext())
                    {
                        actual++;
                    }

                    lock (target)
                    {
                        if (actual == Expected)
                        {
                            completed = true;
                            Monitor.Pulse(target);
                        }
                    }
                };

            for (int i = 0; i < Expected; i++)
            {
                target.ProduceItem(new DateTime(i));
            }

            lock (target)
            {
                completed = completed ? completed : Monitor.Wait(target, 1000, true);
            }

            Assert.IsTrue(completed, "The test timed out.");
            Assert.AreEqual(Expected, actual, "The number of items returned was not equal to the expected items.");
        }

        /// <summary>
        /// The wait for completion test.
        /// </summary>
        [TestMethod]
        public void WaitForCompletion_FailureInProcessStoppedEvent_ThreadCanceledWithoutException()
        {
            var target = new ItemProducer<FakeItem>();

            target.ItemsProduced += (sender, args) =>
            {
                while (target.ItemQueueConsumer.ConsumeNext())
                {
                    Trace.TraceInformation("Consuming {0}", target.ItemQueueConsumer.Current);
                }
            };

            target.ProcessStopped += (sender, args) =>
            {
                throw new InvalidOperationException();
            };

            target.ProduceItem(new FakeItem { IntegerValue = 10, StringValue = "Whatever" });
            bool completed;

            if (Debugger.IsAttached)
            {
                target.WaitForCompletion();
                completed = true;
            }
            else
            {
                completed = target.WaitForCompletion(TimeSpan.FromSeconds(5));               
            }

            Assert.IsTrue(completed, "The test timed out.");

            try
            {
                target.ProduceItem(new FakeItem { IntegerValue = 12, StringValue = "Test" });
                Assert.Fail("An exception should have been thrown.");
            }
            catch (ComponentAbortedException ex)
            {
                Assert.IsNotNull(ex.InnerException);
                Assert.IsInstanceOfType(ex.InnerException, typeof(InvalidOperationException));
            }
        }

        /// <summary>
        /// The wait for completion test.
        /// </summary>
        [TestMethod]
        public void WaitForCompletion_FailureInProcessAndProcessStoppedEvent_ThreadCanceledWithoutException()
        {
            var target = new ItemProducer<FakeItem>();

            target.ItemsProduced += (sender, args) =>
            {
                while (target.ItemQueueConsumer.ConsumeNext())
                {
                    throw new InvalidOperationException("This happened consuming items.");
                }
            };

            target.ProcessStopped += (sender, args) =>
            {
                throw new InvalidOperationException("This happened notifying the subscriber.");
            };

            target.ProduceItem(new FakeItem { IntegerValue = 10, StringValue = "Whatever" });
            bool completed;

            if (Debugger.IsAttached)
            {
                target.WaitForCompletion();
                completed = true;
            }
            else
            {
                completed = target.WaitForCompletion(TimeSpan.FromSeconds(5));
            }

            Assert.IsTrue(completed, "The test timed out.");

            try
            {
                target.ProduceItem(new FakeItem { IntegerValue = 12, StringValue = "Test" });
                Assert.Fail("An exception should have been thrown.");
            }
            catch (ComponentAbortedException ex)
            {
                Assert.IsNotNull(ex.InnerException);
                Assert.IsInstanceOfType(ex.InnerException, typeof(AggregateException));
            }
        }

        #endregion
    }
}