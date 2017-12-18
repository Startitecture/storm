// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskEngineTest.cs" company="TractManager">
//   Copyright 2013 TractManager. All rights reserved.
// </copyright>
// <summary>
//   This is a test class for TaskEngineBaseTest and is intended
//   to contain all TaskEngineBaseTest Unit Tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ProcessEngine.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for TaskEngineBaseTest and is intended
    /// to contain all TaskEngineBaseTest Unit Tests
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TaskEngineTest
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The queue tasks and wait for completion.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="itemsToEnqueue">
        /// The items to enqueue.
        /// </param>
        /// <param name="generator">
        /// The generator.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of task to queue.
        /// </typeparam>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        public static long QueueTasksAndWaitForCompletion<TItem>(TestEngine<TItem> target, long itemsToEnqueue, Func<TItem> generator)
        {
            bool internalTasksComplete = false;
            var taskControl = new object();
            long itemsRetrieved = 0;

            target.ItemsProduced += (sender, args) =>
                {
                    while (target.TaskResultConsumer.ConsumeNext())
                    {
                        itemsRetrieved++;
                    }
                };

            target.ProcessStopped += (sender, args) =>
                {
                    lock (taskControl)
                    {
                        if (itemsRetrieved != itemsToEnqueue)
                        {
                            return;
                        }

                        internalTasksComplete = true;
                        Monitor.Pulse(taskControl);
                    }
                };

            AddItems(target, itemsToEnqueue, generator);

            lock (taskControl)
            {
                if (!internalTasksComplete)
                {
                    Monitor.Wait(taskControl);
                }
            }

            return itemsRetrieved;
        }

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
        /// The task engine_ count greater than item limit_ waiting items limited.
        /// </summary>
        [TestMethod]
        public void TaskEngine_CountGreaterThanItemLimit_WaitingItemsLimited()
        {
            var target = new TestEngine<int>();
            target.MaxQueueLength = 5;
            var objects = new List<int>();
            int preFailures = 0;
            int postFailures = 0;
            bool completed = false;

            target.ItemsProduced += delegate
                {
                    long waiting = target.WaitingTasks;
                    long limit = target.MaxQueueLength;

                    Trace.TraceInformation("pre: Waiting Items = {0}, ItemLimit = {1}", waiting, limit);

                    if (waiting > limit)
                    {
                        preFailures++;
                    }

                    while (target.TaskResultConsumer.ConsumeNext())
                    {
                        TestResult<int> result = target.TaskResultConsumer.Current;
                    }

                    waiting = target.WaitingTasks;
                    limit = target.MaxQueueLength;

                    Trace.TraceInformation("post:Waiting Items = {0}, ItemLimit = {1}", waiting, limit);

                    if (waiting > limit)
                    {
                        postFailures++;
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

            for (int i = 0; i < 500; i++)
            {
                target.Add(i);
            }

            lock (target)
            {
                completed = completed ? true : Monitor.Wait(target, 1000, true);
            }

            Assert.IsTrue(completed, "Test timed out.");
            Assert.IsTrue(preFailures == 0 && postFailures == 0, "{0} pre-failures, {1} post-failures", preFailures, postFailures);
        }

        /// <summary>
        /// The task engine_ multiple items_ consumed count equals produced count.
        /// </summary>
        [TestMethod]
        public void TaskEngine_MultipleItems_ConsumedCountEqualsProducedCount()
        {
            var processControl = new object();
            bool exitedBeforeProcessTimeout = false;
            long expected = 500;
            long actual = 0;
            var target = new TestEngine<int>();

            target.ItemsProduced += delegate
                {
                    while (target.TaskResultConsumer.ConsumeNext())
                    {
                        actual++;
                    }

                    lock (processControl)
                    {
                        if (expected == actual)
                        {
                            exitedBeforeProcessTimeout = true;
                            Monitor.Pulse(processControl);
                        }
                    }
                };

            for (int i = 0; i < expected; i++)
            {
                target.Add(i);
            }

            lock (processControl)
            {
                exitedBeforeProcessTimeout = actual >= expected ? true : Monitor.Wait(processControl, 2000, true);
            }

            Assert.IsTrue(exitedBeforeProcessTimeout, "Process timed out");
            Assert.IsTrue(actual == expected, "Number of hits was {0}, expecting {1}", actual, expected);
        }

        /// <summary>
        /// The task engine_ multiple items_ start state triggered.
        /// </summary>
        [TestMethod]
        public void TaskEngine_MultipleItems_StartStateTriggered()
        {
            bool started = false;
            var target = new TestEngine<int>();
            bool completed = false;
            bool exitedBeforeTimeout = false;
            long itemsToAdd = 7;

            target.ProcessStarted += delegate { started = true; };

            target.ItemsProduced += delegate
                {
                    while (target.TaskResultConsumer.ConsumeNext())
                    {
                        // Consume the items and move on.
                    }
                };

            target.ProcessStopped += delegate
                {
                    lock (target)
                    {
                        Thread.Sleep(500);
                        completed = true;
                        Monitor.Pulse(target);
                    }
                };

            for (int i = 0; i < itemsToAdd; i++)
            {
                target.Add(i);
            }

            lock (target)
            {
                exitedBeforeTimeout = completed ? completed : Monitor.Wait(target, 2000, true);
            }

            Assert.IsTrue(started);
        }

        /// <summary>
        /// The task engine_ multiple items_ stop state triggered.
        /// </summary>
        [TestMethod]
        public void TaskEngine_MultipleItems_StopStateTriggered()
        {
            int items = 500;
            bool completed = false;
            bool exitedBeforeTimeout = false;
            var target = new TestEngine<int>();

            target.ItemsProduced += delegate
                {
                    while (target.TaskResultConsumer.ConsumeNext())
                    {
                        TestResult<int> result = target.TaskResultConsumer.Current;
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

            for (int i = 0; i < items; i++)
            {
                target.Add(i);
            }

            lock (target)
            {
                exitedBeforeTimeout = completed ? completed : Monitor.Wait(target, 2000, true);
            }

            Assert.IsTrue(exitedBeforeTimeout);
        }

        /// <summary>
        /// The task engine_ multiple items_ total increased by count.
        /// </summary>
        [TestMethod]
        public void TaskEngine_MultipleItems_TotalIncreasedByCount()
        {
            var target = new TestEngine<int>();
            long itemsToCreate = 7;

            for (int i = 0; i < itemsToCreate; i++)
            {
                target.Add(i);
            }

            Assert.AreEqual(target.TotalTasks, itemsToCreate);
        }

        #endregion

        ////[TestMethod()]
        ////public void TaskEngine_MultipleItems_StopFiresOnlyOnce()
        ////{
        ////    object taskControl = new object();
        ////    object processControl = new object();
        ////    bool exitedBeforeTaskTimeout = false;
        ////    bool exitedBeforeProcessTimeout = false;
        ////    int itemCount = 500;
        ////    int taskHits = 0;
        ////    int expected = 1;
        ////    int actual = 0;
        ////    TestEngine<int> target = new TestEngine<int>();
        ////    ItemProducer<int> producer = new ItemProducer<int>();
        ////    target.RegisterKeepAliveProcess(producer);

        ////    target.ItemsProduced += delegate(object o, ItemsProducedEventArgs e)
        ////    {
        ////        target.RetrieveTaskResults();
        ////    };

        ////    target.ItemsConsumed += delegate(object o, ItemsConsumedEventArgs e)
        ////    {
        ////        lock (taskControl)
        ////        {
        ////            taskHits += e.ItemCount;

        ////            if (taskHits == itemCount)
        ////            {
        ////                exitedBeforeTaskTimeout = true;
        ////                Monitor.Pulse(taskControl);
        ////            }
        ////        }
        ////    };

        ////    target.ProcessStopped += delegate(object o, ProcessStoppedEventArgs e)
        ////    {
        ////        lock (processControl)
        ////        {
        ////            actual++;
        ////            exitedBeforeProcessTimeout = true;
        ////            Monitor.Pulse(processControl);
        ////        }
        ////    };

        ////    for (int i = 0; i < itemCount; i++)
        ////    {
        ////        target.Add(i);
        ////    }

        ////    lock (taskControl)
        ////    {
        ////        exitedBeforeTaskTimeout = exitedBeforeTaskTimeout ? true : Monitor.Wait(taskControl, 2000, true);
        ////    }

        ////    lock (processControl)
        ////    {
        ////        exitedBeforeProcessTimeout = exitedBeforeProcessTimeout ? true : Monitor.Wait(processControl, 2000, true);
        ////    }

        ////    Assert.IsTrue(exitedBeforeTaskTimeout, "Tasks timed out.");
        ////    Assert.IsTrue(exitedBeforeProcessTimeout, "Process timed out");
        ////    Assert.IsTrue(actual == expected, "Number of hits was {0}, expecting {1}", actual, expected);
        ////}

        ////[TestMethod()]
        ////public void TaskEngine_MultipleItems_StopFiresAfterLastItem()
        ////{
        ////    object processControl = new object();
        ////    object taskControl = new object();
        ////    bool exitedBeforeProcessTimeout = false;
        ////    bool exitedBeforeTaskTimeout = false;
        ////    int itemCount = 500;
        ////    int taskHits = 0;
        ////    EventArgs expected = new ProcessStoppedEventArgs(null);
        ////    EventArgs actual = null;
        ////    TestEngine<int> target = new TestEngine<int>();
        ////    Queue<EventArgs> eventQueue = new Queue<EventArgs>();

        ////    target.ItemsProduced += delegate(object o, ItemsProducedEventArgs e)
        ////    {
        ////        target.RetrieveTaskResults();
        ////        eventQueue.Enqueue(e);
        ////    };

        ////    target.ItemsConsumed += delegate(object o, ItemsConsumedEventArgs e)
        ////    {

        ////        lock (taskControl)
        ////        {
        ////            taskHits += e.ItemCount;

        ////            if (taskHits == itemCount)
        ////            {
        ////                exitedBeforeTaskTimeout = true;
        ////                Monitor.Pulse(taskControl);
        ////            }
        ////        }
        ////    };

        ////    target.ProcessStopped += delegate(object o, ProcessStoppedEventArgs e)
        ////    {
        ////        eventQueue.Enqueue(e);

        ////        lock (processControl)
        ////        {
        ////            exitedBeforeProcessTimeout = true;
        ////            Monitor.Pulse(processControl);
        ////        }
        ////    };

        ////    for (int i = 0; i < itemCount; i++)
        ////    {
        ////        target.Add(i);
        ////    }

        ////    lock (taskControl)
        ////    {
        ////        exitedBeforeTaskTimeout = exitedBeforeTaskTimeout ? true : Monitor.Wait(taskControl, 2000, true);
        ////    }

        ////    lock (processControl)
        ////    {
        ////        exitedBeforeProcessTimeout = exitedBeforeProcessTimeout ? true : Monitor.Wait(processControl, 2000, true);
        ////    }

        ////    while (eventQueue.Count > 0)
        ////    {
        ////        actual = eventQueue.Dequeue();
        ////    }

        ////    Assert.IsTrue(exitedBeforeProcessTimeout, "Process timed out");
        ////    Assert.IsNotNull(actual, "No events were triggered.");
        ////    Assert.AreEqual(expected.GetType(), actual.GetType());
        ////}

        ////[TestMethod()]
        ////public void TaskEngine_MultipleItems_AllItemsRetrievedBeforeExit()
        ////{
        ////    long expected = 500;
        ////    long itemsRemainingAtCompletion = -1;
        ////    long actual = 0;
        ////    bool completed = false;
        ////    bool exitedBeforeTimeout = false;
        ////    TestEngine<int> target = new TestEngine<int>();

        ////    target.ItemsProduced += delegate(object o, ItemsProducedEventArgs e)
        ////    {
        ////        actual += target.RetrieveTaskResults().Count();
        ////    };

        ////    target.ProcessStopped += delegate(Object o, ProcessStoppedEventArgs e)
        ////    {
        ////        lock (target)
        ////        {
        ////            itemsRemainingAtCompletion = actual - expected;
        ////            completed = true;
        ////            Monitor.Pulse(target);
        ////        }
        ////    };

        ////    for (int i = 0; i < expected; i++)
        ////    {
        ////        target.Add(i);
        ////    }

        ////    lock (target)
        ////    {
        ////        exitedBeforeTimeout = completed ? completed : Monitor.Wait(target, 2000, true);
        ////    }

        ////    Assert.IsTrue(completed, "Process was not completed before timeout.");
        ////    Assert.AreEqual(expected, actual, "Expected {0} tasks, but {1} were retrieved.", expected, actual);
        ////}
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
        private static void AddItems<TItem>(TestEngine<TItem> process, long count, Func<TItem> createItem)
        {
            for (int i = 0; i < count; i++)
            {
                process.Add(createItem());
            }
        }

        #endregion
    }
}