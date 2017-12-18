// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueuedItemProcessTest.cs" company="TractManager">
//   Copyright 2013 TractManager. All rights reserved.
// </copyright>
// <summary>
//   This is a test class for ItemStateMachineTest and is intended
//   to contain all ItemStateMachineTest Unit Tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ProcessEngine.Tests
{
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for ItemStateMachineTest and is intended
    /// to contain all ItemStateMachineTest Unit Tests
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class QueuedItemProcessTest
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

        ////public class QueueTester
        ////{
        ////    public event EventHandler TaskAccepted;

        ////    void ITaskAcceptor<object>.AddTask(object task)
        ////    {
        ////        EventHandler temp = TaskAccepted;

        ////        if (temp != null)
        ////        {
        ////            temp(this, EventArgs.Empty);
        ////        }
        ////    }
        ////}

        ////[TestMethod()]
        ////public void AddItems_CountGreaterThanItemLimit_WaitingItemsLimited()
        ////{
        ////    QueueTester tester = new QueueTester();
        ////    QueuedItemProcess<object> target = new QueuedItemProcess<object>(tester);
        ////    target.ItemLimit = 5;
        ////    List<object> objects = new List<object>();
        ////    int preFailures = 0;
        ////    int postFailures = 0;
        ////    bool completed = false;

        ////    tester.TaskAccepted += delegate(object o, EventArgs e)
        ////    {
        ////        long waiting;
        ////        long limit;

        ////        waiting = target.WaitingItems;
        ////        limit = target.ItemLimit;
        ////        Debug.WriteLine("pre:Waiting Items = {0}, ItemLimit = {1}", waiting, limit);

        ////        if (waiting > limit)
        ////            preFailures++;

        ////        Thread.Sleep(20);

        ////        waiting = target.WaitingItems;
        ////        limit = target.ItemLimit;
        ////        Debug.WriteLine("post:Waiting Items = {0}, ItemLimit = {1}", waiting, limit);

        ////        if (waiting > limit)
        ////            postFailures++;
        ////    };

        ////    for (int i = 0; i < 50; i++)
        ////    {
        ////        objects.Add(new object());
        ////    }

        ////    target.ProcessStopped += delegate(Object o, ProcessStoppedEventArgs e)
        ////    {
        ////        lock (target)
        ////        {
        ////            completed = true;
        ////            Monitor.Pulse(target);
        ////        }
        ////    };

        ////    target.AddItems(objects);

        ////    lock (target)
        ////    {
        ////        if (!completed)
        ////            Monitor.Wait(target, 1000, true);
        ////    }

        ////    Assert.IsTrue(preFailures == 0, "{0} pre-failures", preFailures);
        ////    Assert.IsTrue(postFailures == 0, "{0} post-failures", postFailures);
        ////}

        ////[TestMethod()]
        ////public void AddItems_MultipleItems_TotalIncreasedByCount()
        ////{
        ////    QueuedItemProcess<object> target = new QueuedItemProcess<object>(new QueueTester());

        ////    object[] objects = new object[] { new object(), new object() };
        ////    target.AddItems(objects);
        ////    Assert.AreEqual(objects.Length, target.TotalItems);
        ////}

        ////[TestMethod()]
        ////public void AddItems_MultipleItems_StartStateTriggered()
        ////{
        ////    bool started = false;
        ////    bool completed = false;
        ////    QueuedItemProcess<object> target = new QueuedItemProcess<object>(new QueueTester());

        ////    target.ProcessStarted += delegate(Object o, ProcessStartedEventArgs e)
        ////    {
        ////        started = true;
        ////    };

        ////    target.ProcessStopped += delegate(Object o, ProcessStoppedEventArgs e)
        ////    {
        ////        lock (target)
        ////        {
        ////            completed = true;
        ////            Monitor.Pulse(target);
        ////        }
        ////    };

        ////    object[] objects = new object[] { new object(), new object() };
        ////    target.AddItems(objects);

        ////    lock (target)
        ////    {
        ////        if (!completed)
        ////            Monitor.Wait(target, 1000, true);
        ////    }

        ////    Assert.IsTrue(started);
        ////}

        ////[TestMethod()]
        ////public void AddItems_MultipleItems_StartFiresOnlyOnce()
        ////{
        ////    int starts = 0;
        ////    bool completed = false;
        ////    bool exitedBeforeTimeout = false;
        ////    QueuedItemProcess<object> target = new QueuedItemProcess<object>(new QueueTester());

        ////    target.ProcessStarted += delegate(Object o, ProcessStartedEventArgs e)
        ////    {
        ////        starts++;
        ////    };

        ////    target.ProcessStopped += delegate(Object o, ProcessStoppedEventArgs e)
        ////    {
        ////        lock (target)
        ////        {
        ////            completed = true;
        ////            Monitor.Pulse(target);
        ////        }
        ////    };

        ////    object[] objects = new object[] { new object(), new object(), new object(), new object() };
        ////    target.AddItems(objects);

        ////    lock (target)
        ////    {
        ////        exitedBeforeTimeout = completed ? completed : Monitor.Wait(target, 2000, true);
        ////    }

        ////    Assert.AreEqual(1, starts);
        ////}

        ////[TestMethod()]
        ////public void AddItems_MultipleItems_StoppedStateTriggered()
        ////{
        ////    bool completed = false;
        ////    QueuedItemProcess<object> target = new QueuedItemProcess<object>(new QueueTester());
        ////    target.ProcessStopped += delegate(Object o, ProcessStoppedEventArgs e)
        ////    {
        ////        lock (target)
        ////        {
        ////            completed = true;
        ////            Monitor.Pulse(target);
        ////        }
        ////    };

        ////    object[] objects = new object[] { new object(), new object() };
        ////    target.AddItems(objects);

        ////    lock (target)
        ////    {
        ////        if (!completed)
        ////            Monitor.Wait(target, 1000, true);
        ////    }

        ////    Assert.IsTrue(completed);
        ////}

        ////[TestMethod()]
        ////public void AddItems_MultipleItems_ExitsBeforeTimeout()
        ////{
        ////    bool completed = false;
        ////    bool exitedBeforeTimeout = false;
        ////    QueuedItemProcess<object> target = new QueuedItemProcess<object>(new QueueTester());

        ////    target.ProcessStopped += delegate(Object o, ProcessStoppedEventArgs e)
        ////    {
        ////        lock (target)
        ////        {
        ////            Thread.Sleep(500);
        ////            completed = true;
        ////            Monitor.Pulse(target);
        ////        }
        ////    };

        ////    object[] objects = new object[] { new object(), new object() };
        ////    target.AddItems(objects);

        ////    lock (target)
        ////    {
        ////        exitedBeforeTimeout = completed ? completed : Monitor.Wait(target, 2000, true);
        ////    }

        ////    Assert.IsTrue(exitedBeforeTimeout);
        ////}

        ////[TestMethod()]
        ////public void AddItems_10000000Items_MemorySizeDoesNotIncrease()
        ////{
        ////    long originalMemory = 0;
        ////    long finalMemory = Int64.MaxValue;
        ////    bool triggered = false;
        ////    object triggerControl = new object();
        ////    QueuedItemProcess<object> target = new QueuedItemProcess<object>(new QueueTester());

        ////    target.ProcessStarted += delegate(Object o, ProcessStartedEventArgs e)
        ////    {
        ////        originalMemory = GC.GetTotalMemory(true);
        ////    };

        ////    target.ProcessStopped += delegate(Object o, ProcessStoppedEventArgs e)
        ////    {
        ////        lock (triggerControl)
        ////        {
        ////            triggered = true;
        ////            Monitor.Pulse(triggerControl);
        ////        }
        ////    };

        ////    //originalMemory = GC.GetTotalMemory(true);
        ////    target.AddItems(CreateValues(1000000, () => new object()));
        ////    //AddItems(target, 1000000, () => new object());

        ////    lock (triggerControl)
        ////    {
        ////        triggered = triggered ? triggered : System.Threading.Monitor.Wait(triggerControl, 5000);
        ////    }

        ////    GC.Collect();
        ////    finalMemory = GC.GetTotalMemory(true);

        ////    var collections = from c in Enumerable.Range(0, GC.MaxGeneration)
        ////                      select String.Format("{0} -> {1}", c, GC.CollectionCount(c));

        ////    Assert.IsTrue(
        ////        originalMemory >= finalMemory, 
        ////        "Original: {0}; Final: {1} (Delta {2}, Collections {3})", 
        ////        originalMemory, 
        ////        finalMemory, 
        ////        finalMemory - originalMemory,
        ////        String.Join(";", collections.ToArray()));
        ////}

        ////private void AddItems<TItem>(QueuedItemProcess<TItem> process, long count, Func<TItem> createItem)
        ////{
        ////    for (int i = 0; i < count; i++)
        ////    {
        ////        process.AddItems(createItem());
        ////    }
        ////}

        ////private IEnumerable<TItem> CreateValues<TItem>(long count, Func<TItem> createItem)
        ////{
        ////    TItem[] values = new TItem[count];

        ////    for (int i = 0; i < count; i++)
        ////    {
        ////        values[i] = createItem();
        ////    }

        ////    return values;
        ////}

        ////[TestMethod()]
        ////public void AddItems_MultipleItems_CompletionFiresOnlyOnce()
        ////{
        ////    int completeHits = 0;
        ////    bool completed = false;
        ////    bool exitedBeforeTimeout = false;
        ////    QueuedItemProcess<object> target = new QueuedItemProcess<object>(new QueueTester());

        ////    target.ProcessStopped += delegate(Object o, ProcessStoppedEventArgs e)
        ////    {
        ////        lock (target)
        ////        {
        ////            completeHits++;
        ////            completed = true;
        ////            Monitor.Pulse(target);
        ////        }
        ////    };

        ////    object[] objects = new object[] { new object(), new object() };
        ////    target.AddItems(objects);

        ////    lock (target)
        ////    {
        ////        exitedBeforeTimeout = completed ? completed : Monitor.Wait(target, 2000, true);
        ////    }

        ////    Assert.IsTrue(exitedBeforeTimeout && completeHits == 1);
        ////}
    }
}