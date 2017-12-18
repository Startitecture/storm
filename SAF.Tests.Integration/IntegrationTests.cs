namespace SAF.Tests.Integration
{
    using System;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SAF.ProcessEngine;
    using SAF.ProcessEngine.Tests;

    [TestClass()]
    public class IntegrationTests
    {
        /////// <summary>
        /////// Tests that a data integration controller can process 1,000,000 items without a memory increase.
        /////// </summary>
        ////[TestMethod()]
        ////public void IntegrationController_1000000Items_MemoryDeltaUnder100KB()
        ////{
        ////    IntegrationPolicy<long, DateTime> policy = new IntegrationPolicy<long, DateTime>(
        ////        new TimeConverter(), new TimePersistenceAdapter(), null, null);

        ////    TestIntegrationController target = new TestIntegrationController();
        ////    long finalMemory;
        ////    long expected = 1024 * 100;
        ////    long actual;
        ////    long expectedTasks = 1000000;

        ////    long originalMemory = GC.GetTotalMemory(true);
        ////    StartIntegrationAndWaitForCompletion(target, expectedTasks);
        ////    long actualTasks = target.CompletedTasks;

        ////    GC.Collect();
        ////    finalMemory = GC.GetTotalMemory(true);
        ////    actual = finalMemory - originalMemory;

        ////    var collections = from c in Enumerable.Range(0, GC.MaxGeneration)
        ////                      select String.Format("{0} -> {1}", c, GC.CollectionCount(c));

        ////    Assert.AreEqual(expectedTasks, actualTasks, "Expected {0} tasks, but only {1} were retrieved.", expectedTasks, actualTasks);
        ////    Assert.IsTrue(
        ////        actual < expected,
        ////        "Original: {0}; Final: {1} (Delta {2}, Collections {3})",
        ////        originalMemory,
        ////        finalMemory,
        ////        actual,
        ////        String.Join(";", collections.ToArray()));
        ////}

        /// <summary>
        /// Tests that a process controller can process 1,000,000 items without a memory increase.
        /// </summary>
        [TestMethod()]
        [TestCategory("Integration")]
        public void ProcessController_1000000Items_MemoryDeltaUnder100KB()
        {
            TestController<Guid> target = new TestController<Guid>();
            long finalMemory;
            long expected = 1024 * 1024;
            long actual;
            long expectedTasks = 1000000;

            long originalMemory = GC.GetTotalMemory(true);
            ProcessControllerBaseTest.QueueProcessItemsAndWaitForCompletion(target, expectedTasks, () => Guid.NewGuid());
            long actualTasks = target.CompletedTasks;

            GC.Collect();
            finalMemory = GC.GetTotalMemory(true);
            actual = finalMemory - originalMemory;

            var collections = from c in Enumerable.Range(0, GC.MaxGeneration)
                              select String.Format("{0} -> {1}", c, GC.CollectionCount(c));

            Assert.AreEqual(expectedTasks, actualTasks, "Expected {0} tasks, but only {1} were retrieved.", expectedTasks, actualTasks);
            Assert.IsTrue(
                actual < expected,
                "Original: {0}; Final: {1} (Delta {2}, Collections {3})",
                originalMemory,
                finalMemory,
                actual,
                String.Join(";", collections.ToArray()));
        }

        /// <summary>
        /// Tests that a task engine can process 1,000,000 tasks without a memory increase.
        /// </summary>
        [TestMethod()]
        [TestCategory("Integration")]
        public void TaskEngine_1000000Items_MemoryDeltaUnder100KB()
        {
            TestEngine<Guid> target = new TestEngine<Guid>();
            long finalMemory;
            long expected = 1024 * 1024;
            long actual;
            long expectedTasks = 1000000;

            long originalMemory = GC.GetTotalMemory(true);
            long actualTasks = TaskEngineTest.QueueTasksAndWaitForCompletion(target, expectedTasks, () => Guid.NewGuid());

            GC.Collect();
            finalMemory = GC.GetTotalMemory(true);
            actual = finalMemory - originalMemory;

            var collections = from c in Enumerable.Range(0, GC.MaxGeneration)
                              select String.Format("{0} -> {1}", c, GC.CollectionCount(c));

            Assert.AreEqual(expectedTasks, actualTasks, "Expected {0} tasks, but only {1} were retrieved.", expectedTasks, actualTasks);
            Assert.IsTrue(
                actual < expected,
                "Original: {0}; Final: {1} (Delta {2}, Collections {3})",
                originalMemory,
                finalMemory,
                actual,
                String.Join(";", collections.ToArray()));
        }

        ////[TestMethod]
        ////public void ItemProducer_1000000Items_MemoryDeltaUnder100KB()
        ////{
        ////    ItemProducer<Guid> target = new ItemProducer<Guid>();
        ////    long finalMemory;
        ////    long expected = 1024 * 1024;
        ////    long actual;
        ////    long expectedTasks = 1000000;
        ////    long actualTasks;

        ////    long originalMemory = GC.GetTotalMemory(true);
        ////    bool completedBeforeTimeout = 
        ////        ItemProducerTest.ProduceItemsAndWaitForCompletion(
        ////            target, expectedTasks, new TimeSpan(TimeSpan.TicksPerMinute), () => Guid.NewGuid(), out actualTasks);

        ////    GC.Collect();
        ////    finalMemory = GC.GetTotalMemory(true);
        ////    actual = finalMemory - originalMemory;

        ////    var collections = from c in Enumerable.Range(0, GC.MaxGeneration)
        ////                      select String.Format("{0} -> {1}", c, GC.CollectionCount(c));

        ////    Assert.AreEqual(expectedTasks, actualTasks, "Expected {0} tasks, but only {1} were retrieved.", expectedTasks, actualTasks);
        ////    Assert.IsTrue(
        ////        actual < expected,
        ////        "Original: {0}; Final: {1} (Delta {2}, Collections {3})",
        ////        originalMemory,
        ////        finalMemory,
        ////        actual,
        ////        String.Join(";", collections.ToArray()));
        ////}

        ////[TestMethod]
        ////public void ItemProducer_100000Items_OverheadLessThan5Ticks()
        ////{
        ////    long expected = 0;
        ////    double actual;
        ////    long expectedTasks = 100000;
        ////    TimeSpan expectedTime;
        ////    TimeSpan actualTime = new TimeSpan(0);
        ////    long iterations = 200;
        ////    System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        ////    Func<Guid> generator = () => { return Guid.Empty; };

        ////    for (int i = 0; i < iterations; i++)
        ////    {
        ////        int someCounter = 0;
        ////        watch.Start();

        ////        for (int j = 0; j < expectedTasks; j++)
        ////        {
        ////            Guid guid = generator();
        ////            someCounter++;
        ////        }

        ////        watch.Stop();

        ////        long actualTasks;
        ////        ItemProducer<Guid> target = new ItemProducer<Guid>();
        ////        bool completedBeforeTimeout =
        ////            ItemProducerTest.ProduceItemsAndWaitForCompletion(
        ////                target, expectedTasks, new TimeSpan(TimeSpan.TicksPerMinute), generator, out actualTasks);

        ////        actualTime += target.ProcessTime;
        ////        Assert.AreEqual(
        ////            expectedTasks, actualTasks, "Expected {0} tasks, but only {1} were retrieved.", expectedTasks, actualTasks);
        ////    }

        ////    expectedTime = watch.Elapsed;
        ////    TimeSpan overhead = actualTime - expectedTime;
        ////    actual = (double)overhead.Ticks / (double)(expectedTasks * iterations);

        ////    Assert.IsTrue(actual < expected, "Expected overhead < {0}, actual overhead {1}", expected, actual);
        ////}

        /// <summary>
        /// Tests that an event process can process 1,000,000 events without a memory increase.
        ///</summary>
        [TestMethod()]
        [TestCategory("Integration")]
        public void StateMachineBase_1000000EventsTriggered_MemoryIsConstant()
        {
            TestStateMachine target = new TestStateMachine();
            Type expectedArgsType = typeof(ItemsAddedEventArgs);
            bool triggered = false;
            int actualEventCount = 0;
            int expectedEventCount = 1000000;
            long originalMemory = 0;
            long finalMemory = Int64.MaxValue;
            long expected = 0;
            long actual;

            target.EventReceived += delegate(object o, EventArgs e)
            {
                lock (target)
                {
                    actualEventCount++;

                    if (actualEventCount == expectedEventCount)
                    {
                        triggered = true;
                        System.Threading.Monitor.Pulse(target);
                    }
                }
            };

            originalMemory = GC.GetTotalMemory(true);

            for (int i = 0; i < expectedEventCount; i++)
            {
                target.TriggerGenericEvent(new TestEventArgs(3));
            }

            lock (target)
            {
                triggered = triggered ? true : System.Threading.Monitor.Wait(target, 1000);
            }

            GC.Collect();
            finalMemory = GC.GetTotalMemory(true);
            actual = finalMemory - originalMemory;

            var collections = from c in Enumerable.Range(0, GC.MaxGeneration)
                              select String.Format("{0} -> {1}", c, GC.CollectionCount(c));

            Assert.AreEqual(expectedEventCount, actualEventCount);
            Assert.IsTrue(
                actual <= expected,
                "Original: {0}; Final: {1} (Delta {2}, Collections {3})",
                originalMemory,
                finalMemory,
                actual,
                String.Join(";", collections.ToArray()));
        }

        #region Helper Methods

        ////public static void StartIntegrationAndWaitForCompletion(TestIntegrationController controller, long itemsToEnqueue)
        ////{
        ////    TimeProxy proxy = new TimeProxy();

        ////    object triggerControl = new object();
        ////    bool triggered = false;

        ////    controller.ProcessStopped += delegate(object o, ProcessStoppedEventArgs e)
        ////    {
        ////        lock (triggerControl)
        ////        {
        ////            triggered = true;
        ////            System.Threading.Monitor.Pulse(triggerControl);
        ////        }
        ////    };

        ////    controller.RegisterDataProxy(proxy);
        ////    proxy.Load(new LongGenerator(itemsToEnqueue));

        ////    lock (triggerControl)
        ////    {
        ////        triggered = triggered ? triggered : System.Threading.Monitor.Wait(triggerControl);
        ////    }
        ////}

        #endregion
    }
}
