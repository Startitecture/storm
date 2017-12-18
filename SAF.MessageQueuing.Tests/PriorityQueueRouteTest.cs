// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PriorityQueueRouteTest.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing.Tests
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Rhino.Mocks;

    using SAF.ActionTracking;
    using SAF.ProcessEngine;

    /// <summary>
    /// This is a test class for PriorityQueueTest and is intended
    /// to contain all PriorityQueueTest Unit Tests
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class PriorityQueueRouteTest
    {
        /// <summary>
        /// The deadline.
        /// </summary>
        private readonly DateTimeOffset deadline = DateTimeOffset.Now.Date.AddDays(1).AddHours(7);

        /// <summary>
        /// The escalation time.
        /// </summary>
        private readonly TimeSpan escalationTime = TimeSpan.FromHours(3);

        /// <summary>
        /// The stub i action event proxy.
        /// </summary>
        private readonly IActionEventProxy actionEventProxy = MockRepository.GenerateMock<IActionEventProxy>();

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
        /// The average request latency_ sleep ten millis_ request latency greater than nine millis.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void AverageRequestLatency_SleepTenMillis_RequestLatencyGreaterThanNineMillis()
        {
            var target = new FakeQueueRouteOne(this.actionEventProxy);
            var routingNotification = new FakeNotification<FakeMessage>();
            target.SendMessage(
                new MessageRoutingRequest<FakeMessage>(
                    Generate.PriorityMessage("First", this.deadline, DateTimeOffset.Now, this.escalationTime), routingNotification));
            Thread.Sleep(10);
            target.SendMessage(
                new MessageRoutingRequest<FakeMessage>(
                    Generate.PriorityMessage("First", this.deadline, DateTimeOffset.Now, this.escalationTime), routingNotification));
            Thread.Sleep(10);
            var averageRequestLatency = target.QueueState.AverageRequestLatency;
            Assert.IsTrue(
                averageRequestLatency > TimeSpan.FromMilliseconds(9),
                "Actual Time: {0}",
                averageRequestLatency.TotalMilliseconds);
        }

        /// <summary>
        /// The average response latency_ request latency of ten millis_ response latency greater than nine millis.
        /// </summary>
        [TestMethod]
        public void AverageResponseLatency_RequestLatencyOfTenMillis_ResponseLatencyGreaterThanNineMillis()
        {
            var target = new FakeQueueRouteOne(this.actionEventProxy);
            var routingNotification = new FakeNotification<FakeMessage>();
            target.SendMessage(
                new MessageRoutingRequest<FakeMessage>(
                    Generate.PriorityMessage("Name", this.deadline, DateTimeOffset.Now, this.escalationTime, 10),
                    routingNotification));
            target.SendMessage(
                new MessageRoutingRequest<FakeMessage>(
                    Generate.PriorityMessage("Name", this.deadline, DateTimeOffset.Now, this.escalationTime, 10),
                    routingNotification));
            target.SendMessage(
                new MessageRoutingRequest<FakeMessage>(
                    Generate.PriorityMessage("Name", this.deadline, DateTimeOffset.Now, this.escalationTime, 10),
                    routingNotification));
            target.SendMessage(
                new MessageRoutingRequest<FakeMessage>(
                    Generate.PriorityMessage("Name", this.deadline, DateTimeOffset.Now, this.escalationTime, 10),
                    routingNotification));
            target.SendMessage(
                new MessageRoutingRequest<FakeMessage>(
                    Generate.PriorityMessage("Name", this.deadline, DateTimeOffset.Now, this.escalationTime, 10),
                    routingNotification));
            target.SendMessage(
                new MessageRoutingRequest<FakeMessage>(
                    Generate.PriorityMessage("Name", this.deadline, DateTimeOffset.Now, this.escalationTime, 10),
                    routingNotification));
            target.SendMessage(
                new MessageRoutingRequest<FakeMessage>(
                    Generate.PriorityMessage("Name", this.deadline, DateTimeOffset.Now, this.escalationTime, 10),
                    routingNotification));
            target.SendMessage(
                new MessageRoutingRequest<FakeMessage>(
                    Generate.PriorityMessage("Name", this.deadline, DateTimeOffset.Now, this.escalationTime, 10),
                    routingNotification));
            target.SendMessage(
                new MessageRoutingRequest<FakeMessage>(
                    Generate.PriorityMessage("Name", this.deadline, DateTimeOffset.Now, this.escalationTime, 10),
                    routingNotification));

            Assert.IsTrue(target.WaitForCompletion(TimeSpan.FromSeconds(5)), "The test timed out.");
            var averageResponseLatency = target.QueueState.AverageResponseLatency;
            Assert.IsTrue(averageResponseLatency >= TimeSpan.FromMilliseconds(9), "Actual Time: {0}", averageResponseLatency);
        }

        /// <summary>
        /// The average response latency_ request latency of ten millis_ response latency greater than nine millis.
        /// </summary>
        [TestMethod]
        public void SendMessage_MultipleMessages_AllRequestsReceived()
        {
            var target = new FakeQueueRouteOne(this.actionEventProxy);
            var receives = new ConcurrentQueue<RequestEventArgs<FakeMessage>>();
            target.RequestReceived += (sender, args) => receives.Enqueue(args);

            const int MessagesToSend = 50;
            var routingNotification = new FakeNotification<FakeMessage>();

            for (int i = 0; i < MessagesToSend; i++)
            {
                var message = Generate.PriorityMessage(
                    String.Format("Message {0}", i), this.deadline, DateTimeOffset.Now, this.escalationTime, 1);

                target.SendMessage(new MessageRoutingRequest<FakeMessage>(message, routingNotification));
            }

            if (Debugger.IsAttached)
            {
                target.WaitForCompletion();
            }
            else
            {
                Assert.IsTrue(target.WaitForCompletion(TimeSpan.FromSeconds(2)), "Timeout occurred.");
            }

            Assert.AreEqual(MessagesToSend, receives.Count);
        }

        /// <summary>
        /// The average response latency_ request latency of ten millis_ response latency greater than nine millis.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void SendMessage_MultipleMessages_AllMessagesQueuedWithoutDelay()
        {
            const int MaxTimeMilliseconds = 2;
            var target = new FakeQueueRouteOne(this.actionEventProxy);
            var receives = new ConcurrentQueue<RequestEventArgs<FakeMessage>>();
            target.RequestReceived += (sender, args) => receives.Enqueue(args);
            const int MessagesToSend = 20;
            var routingNotification = new FakeNotification<FakeMessage>();
            Stopwatch watch = Stopwatch.StartNew();

            for (int i = 0; i < MessagesToSend; i++)
            {
                var priorityMessage = Generate.PriorityMessage(
                    "Name", this.deadline, DateTimeOffset.Now, this.escalationTime, MaxTimeMilliseconds);

                target.SendMessage(new MessageRoutingRequest<FakeMessage>(priorityMessage, routingNotification));
            }

            var totalMilliseconds = watch.Elapsed.TotalMilliseconds;
            target.WaitForCompletion(TimeSpan.FromSeconds(2));
            Assert.IsTrue(
                totalMilliseconds < MaxTimeMilliseconds,
                "Took {0}ms to queue messages. Expected < {1}ms.",
                totalMilliseconds,
                MaxTimeMilliseconds);

            Assert.AreEqual(MessagesToSend, receives.Count);
        }

        /// <summary>
        /// The failure rate_ one failed request out of ten_ failure rate equals ten percent.
        /// </summary>
        [TestMethod]
        public void SendMessage_MultipleMessagesWithAbort_AbortEventReceived()
        {
            const int PreMessages = 5;
            const int PostMessages = 10;
            var target = new FakeQueueRouteOne(this.actionEventProxy);
            var receives = new ConcurrentBag<FakeMessage>();

            MessageRoutingAbortException<FakeMessage> abortException = null;
            target.RequestCompleted += (sender, args) => receives.Add(args.ResponseEvent.RoutingRequest.Message);
            target.ProcessStopped += (sender, args) =>
                {
                    var abortEx = args.EventError as MessageRoutingAbortException<FakeMessage>;

                    if (abortEx != null)
                    {
                        abortException = abortEx;
                    }
                };

            var routingNotification = new FakeNotification<FakeMessage>();
            for (int i = 0; i < PreMessages; i++)
            {
                target.SendMessage(
                    new MessageRoutingRequest<FakeMessage>(
                        Generate.PriorityMessage(Convert.ToString(i), this.deadline, DateTimeOffset.Now, this.escalationTime), routingNotification));
            }

            target.SendMessage(
                new MessageRoutingRequest<FakeMessage>(
                    Generate.FailureRequest("Bad", this.deadline, DateTimeOffset.Now, this.escalationTime, 1),
                    routingNotification));

            var nextMessages = new Queue<MessageRoutingRequest<FakeMessage>>();

            for (int i = 0; i < PostMessages; i++)
            {
                nextMessages.Enqueue(
                    new MessageRoutingRequest<FakeMessage>(
                        Generate.PriorityMessage(
                            Convert.ToString(i + PreMessages),
                            this.deadline,
                            DateTimeOffset.Now,
                            this.escalationTime),
                        routingNotification));
            }

            while (nextMessages.Count > 0)
            {
                try
                {
                    target.SendMessage(nextMessages.Dequeue());
                }
                catch (ComponentAbortedException)
                {
                }
            }

            target.WaitForCompletion(TimeSpan.FromSeconds(2));
            Assert.IsNotNull(abortException, "The expected abort event does not exist.");
            Trace.TraceInformation("Queue aborted with: {0}", abortException);
            Assert.AreEqual(
                1 + PreMessages + PostMessages,
                receives.Count + nextMessages.Count + abortException.PendingItems.Count());
        }

        /// <summary>
        /// The failure rate_ one failed request out of ten_ failure rate equals ten percent.
        /// </summary>
        [TestMethod]
        public void FailureRate_OneFailedRequestOutOfTen_FailureRateEqualsTenPercent()
        {
            var target = new FakeQueueRouteOne(this.actionEventProxy);
            var routingNotification = new FakeNotification<FakeMessage>();

            target.SendMessage(
                new MessageRoutingRequest<FakeMessage>(
                    Generate.PriorityMessage("Name", this.deadline, DateTimeOffset.Now, this.escalationTime),
                    routingNotification));
            target.SendMessage(
                new MessageRoutingRequest<FakeMessage>(
                    Generate.PriorityMessage("Name", this.deadline, DateTimeOffset.Now, this.escalationTime),
                    routingNotification));
            target.SendMessage(
                new MessageRoutingRequest<FakeMessage>(
                    Generate.PriorityMessage("Name", this.deadline, DateTimeOffset.Now, this.escalationTime),
                    routingNotification));
            target.SendMessage(
                new MessageRoutingRequest<FakeMessage>(
                    Generate.PriorityMessage("Name", this.deadline, DateTimeOffset.Now, this.escalationTime),
                    routingNotification));
            target.SendMessage(
                new MessageRoutingRequest<FakeMessage>(
                    Generate.PriorityMessage("Name", this.deadline, DateTimeOffset.Now, this.escalationTime),
                    routingNotification));
            target.SendMessage(
                new MessageRoutingRequest<FakeMessage>(
                    Generate.PriorityMessage("Bad", this.deadline, DateTimeOffset.Now, this.escalationTime, 1, true),
                    routingNotification));
            target.SendMessage(
                new MessageRoutingRequest<FakeMessage>(
                    Generate.PriorityMessage("Name", this.deadline, DateTimeOffset.Now, this.escalationTime),
                    routingNotification));
            target.SendMessage(
                new MessageRoutingRequest<FakeMessage>(
                    Generate.PriorityMessage("Name", this.deadline, DateTimeOffset.Now, this.escalationTime),
                    routingNotification));
            target.SendMessage(
                new MessageRoutingRequest<FakeMessage>(
                    Generate.PriorityMessage("Name", this.deadline, DateTimeOffset.Now, this.escalationTime),
                    routingNotification));
            target.SendMessage(
                new MessageRoutingRequest<FakeMessage>(
                    Generate.PriorityMessage("Name", this.deadline, DateTimeOffset.Now, this.escalationTime),
                    routingNotification));
            target.WaitForCompletion(TimeSpan.FromSeconds(2));
            Assert.AreEqual(0.1, target.QueueState.FailureRate);
        }

        /// <summary>
        /// The failure rate_ one failed request out of ten_ failure rate equals ten percent.
        /// </summary>
        [TestMethod]
        public void Cancel_ActiveQueueRoute_ProcessedAndRemainingMessagesEqualTotalQueued()
        {
            var target = new FakeQueueRouteOne(this.actionEventProxy);
            var routingNotification = new FakeNotification<FakeMessage>();
            target.SendMessage(
                new MessageRoutingRequest<FakeMessage>(
                    Generate.PriorityMessage("Name", this.deadline, DateTimeOffset.Now, this.escalationTime, 5), routingNotification));
            target.SendMessage(
                new MessageRoutingRequest<FakeMessage>(
                    Generate.PriorityMessage("Name", this.deadline, DateTimeOffset.Now, this.escalationTime, 5), routingNotification));
            target.SendMessage(
                new MessageRoutingRequest<FakeMessage>(
                    Generate.PriorityMessage("Name", this.deadline, DateTimeOffset.Now, this.escalationTime, 5), routingNotification));
            target.SendMessage(
                new MessageRoutingRequest<FakeMessage>(
                    Generate.PriorityMessage("Name", this.deadline, DateTimeOffset.Now, this.escalationTime, 5), routingNotification));
            target.SendMessage(
                new MessageRoutingRequest<FakeMessage>(
                    Generate.PriorityMessage("Name", this.deadline, DateTimeOffset.Now, this.escalationTime, 5), routingNotification));
            target.SendMessage(
                new MessageRoutingRequest<FakeMessage>(
                    Generate.PriorityMessage("Name", this.deadline, DateTimeOffset.Now, this.escalationTime, 5), routingNotification));
            target.SendMessage(
                new MessageRoutingRequest<FakeMessage>(
                    Generate.PriorityMessage("Name", this.deadline, DateTimeOffset.Now, this.escalationTime, 5), routingNotification));
            target.SendMessage(
                new MessageRoutingRequest<FakeMessage>(
                    Generate.PriorityMessage("Name", this.deadline, DateTimeOffset.Now, this.escalationTime, 5), routingNotification));
            target.SendMessage(
                new MessageRoutingRequest<FakeMessage>(
                    Generate.PriorityMessage("Name", this.deadline, DateTimeOffset.Now, this.escalationTime, 5), routingNotification));
            target.SendMessage(
                new MessageRoutingRequest<FakeMessage>(
                    Generate.PriorityMessage("Name", this.deadline, DateTimeOffset.Now, this.escalationTime, 5), routingNotification));

            if (Debugger.IsAttached)
            {
                target.Cancel();
            }
            else
            {
                Assert.IsTrue(target.Cancel(TimeSpan.FromSeconds(2)));
            }

            Assert.AreEqual(target.QueueState.MessageRequests, target.QueueState.MessagesProcessed + target.QueuedMessages.Count());
        }

        /// <summary>
        /// The send request_ fake request_ queue not aborted.
        /// </summary>
        [TestMethod]
        public void SendMessage_FakeMessage_QueueNotAborted()
        {
            var target = new FakeQueueRouteOne(this.actionEventProxy);
            var routingNotification = new FakeNotification<FakeMessage>();
            target.SendMessage(
                new MessageRoutingRequest<FakeMessage>(
                    Generate.PriorityMessage("Name", this.deadline, DateTimeOffset.Now, this.escalationTime),
                    routingNotification));

            Assert.IsTrue(target.WaitForCompletion(TimeSpan.FromSeconds(1)), "Operation timed out.");
            Assert.IsFalse(target.QueueState.QueueAborted);
        }

        #endregion
    }
}