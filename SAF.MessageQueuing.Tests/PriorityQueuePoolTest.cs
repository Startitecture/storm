// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PriorityQueuePoolTest.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
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
    using SAF.Observer;

    /// <summary>
    /// This is a test class for PriorityQueuePoolTest and is intended
    /// to contain all PriorityQueuePoolTest Unit Tests
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class PriorityQueuePoolTest
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
        /// The action event proxy.
        /// </summary>
        private readonly IActionEventProxy actionEventProxy = MockRepository.GenerateMock<IActionEventProxy>();

        #region Public Properties

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        #endregion

        /// <summary>
        /// Use ClassInitialize to run code before running the first test in the class.
        /// </summary>
        /// <param name="testContext">
        /// The test context.
        /// </param>
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
        }

        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup()
        // {
        // }

        /// <summary>
        /// The my test initialize.
        /// </summary>
        [TestInitialize]
        public void MyTestInitialize()
        {
        }

        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup()
        // {
        // }
        #region Public Methods and Operators

        /// <summary>
        /// The average response latency_ request latency of ten millis_ response latency greater than nine millis.
        /// </summary>
        [TestMethod]
        public void SendMessage_MultipleMessagesToQueuePool_AllRequestsReceived()
        {
            var target = new FakeQueuePool<FakeQueueRouteOne>(new FakeQueueRouteFactory<FakeQueueRouteOne>(this.actionEventProxy), this.actionEventProxy);
            var receives = new ConcurrentQueue<RequestEventArgs<FakeMessage>>();
            var routingNotification = new FakeNotification<FakeMessage>();
            target.RequestReceived += (sender, args) => receives.Enqueue(args);
            target.SendMessage(
                new MessageRoutingRequest<FakeMessage>(
                    Generate.PriorityMessage("Name", this.deadline, DateTimeOffset.Now, this.escalationTime), routingNotification));
            target.SendMessage(
                new MessageRoutingRequest<FakeMessage>(
                    Generate.PriorityMessage("Name", this.deadline, DateTimeOffset.Now, this.escalationTime), routingNotification));
            target.SendMessage(
                new MessageRoutingRequest<FakeMessage>(
                    Generate.PriorityMessage("Name", this.deadline, DateTimeOffset.Now, this.escalationTime), routingNotification));
            target.SendMessage(
                new MessageRoutingRequest<FakeMessage>(
                    Generate.PriorityMessage("Name", this.deadline, DateTimeOffset.Now, this.escalationTime), routingNotification));
            target.SendMessage(
                new MessageRoutingRequest<FakeMessage>(
                    Generate.PriorityMessage("Name", this.deadline, DateTimeOffset.Now, this.escalationTime), routingNotification));
            target.SendMessage(
                new MessageRoutingRequest<FakeMessage>(
                    Generate.PriorityMessage("Name", this.deadline, DateTimeOffset.Now, this.escalationTime), routingNotification));
            target.SendMessage(
                new MessageRoutingRequest<FakeMessage>(
                    Generate.PriorityMessage("Name", this.deadline, DateTimeOffset.Now, this.escalationTime), routingNotification));
            target.SendMessage(
                new MessageRoutingRequest<FakeMessage>(
                    Generate.PriorityMessage("Name", this.deadline, DateTimeOffset.Now, this.escalationTime), routingNotification));
            target.SendMessage(
                new MessageRoutingRequest<FakeMessage>(
                    Generate.PriorityMessage("Name", this.deadline, DateTimeOffset.Now, this.escalationTime), routingNotification));
            Assert.IsTrue(target.WaitForCompletion(TimeSpan.FromSeconds(2)), "The test timed out.");
            Assert.AreEqual(9, receives.Count);
        }

        /// <summary>
        /// A test for RequeueAbortedRequests
        /// </summary>
        [TestMethod]
        public void SendMessage_LimitedResourceQueuingPolicySlowerThanResponseTime_QueuePoolLessThanMaximum()
        {
            var performanceMonitor = MockRepository.GenerateMock<IPerformanceMonitor>();
            var policy = new LimitedResourceQueuingPolicy(performanceMonitor);
            var target = new FakeQueuePool<FakeQueueRouteOne>(new FakeQueueRouteFactory<FakeQueueRouteOne>(this.actionEventProxy), policy, this.actionEventProxy);
            const int MessagesToSend = 50;

            var routingNotification = new FakeNotification<FakeMessage>();

            for (int i = 0; i < MessagesToSend; i++)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(50));
                var message = Generate.PriorityMessage(
                    String.Format("Message {0}", i), 
                    this.deadline, 
                    DateTimeOffset.Now, 
                    this.escalationTime, 
                    2);

                target.SendMessage(new MessageRoutingRequest<FakeMessage>(message, routingNotification));
            }

            if (Debugger.IsAttached)
            {
                target.WaitForCompletion();
            }
            else
            {
                Assert.IsTrue(target.WaitForCompletion(TimeSpan.FromSeconds(2)));
            }

            Assert.IsTrue(target.HighestConcurrency < policy.MaxQueueCount);
        }

        /// <summary>
        /// A test for RequeueAbortedRequests
        /// </summary>
        [TestMethod]
        public void SendMessage_LimitedResourceQueuingPolicyFasterThanResponseTime_QueuePoolGreaterThanMinCount()
        {
            var processorSamples = new EnumerableQuery<double>(new[] { 12.3, 2.5, 27.2, 3.5 });
            var performanceMonitor = MockRepository.GenerateMock<IPerformanceMonitor>();
            performanceMonitor.Stub(monitor => monitor.ProcessorUsageSamples).Return(processorSamples);
            var policy = new LimitedResourceQueuingPolicy(performanceMonitor);
            var target = new FakeQueuePool<FakeQueueRouteOne>(new FakeQueueRouteFactory<FakeQueueRouteOne>(this.actionEventProxy), policy, this.actionEventProxy);
            const int MessagesToSend = 50;

            var routingNotification = new FakeNotification<FakeMessage>();

            for (int i = 0; i < MessagesToSend; i++)
            {
                var message = Generate.PriorityMessage(
                    String.Format("Message {0}", i), 
                    this.deadline, 
                    DateTimeOffset.Now, 
                    this.escalationTime, 
                    20);

                target.SendMessage(new MessageRoutingRequest<FakeMessage>(message, routingNotification));
            }

            if (Debugger.IsAttached)
            {
                target.WaitForCompletion();
            }
            else
            {
                Assert.IsTrue(target.WaitForCompletion(TimeSpan.FromSeconds(5)), "The test timed out.");
            }

            var actual = target.HighestConcurrency;
            Assert.IsTrue(actual > policy.MinQueueCount, "Actual {0} <= policy.MinQueueCount {1}", actual, policy.MinQueueCount);
        }

        /// <summary>
        /// A test for RequeueAbortedRequests
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void SendMessage_StaticQueuingPolicySlowerThanResponseTime_QueuePoolRemainsAtMinimum()
        {
            var policy = new StaticQueuingPolicy { TrimIdleQueues = true };
            var target = new FakeQueuePool<FakeQueueRouteOne>(new FakeQueueRouteFactory<FakeQueueRouteOne>(this.actionEventProxy), policy, this.actionEventProxy);
            const int MessagesToSend = 50;

            var routingNotification = new FakeNotification<FakeMessage>();

            for (int i = 0; i < MessagesToSend; i++)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(10));
                var message = Generate.PriorityMessage(
                    String.Format("Message {0}", i), 
                    this.deadline, 
                    DateTimeOffset.Now, 
                    this.escalationTime, 
                    5);

                target.SendMessage(new MessageRoutingRequest<FakeMessage>(message, routingNotification));
            }

            var actual = target.QueueCount;

            if (Debugger.IsAttached)
            {
                target.WaitForCompletion();
            }
            else
            {
                Assert.IsTrue(target.WaitForCompletion(TimeSpan.FromSeconds(10)), "The test timed out.");
            }

            Assert.AreEqual(1, actual);
        }

        /// <summary>
        /// A test for RequeueAbortedRequests
        /// </summary>
        [TestMethod]
        public void SendMessage_StaticQueuingPolicyFasterThanResponseTime_QueuePoolGreaterThanMinCount()
        {
            var policy = new StaticQueuingPolicy();
            var target = new FakeQueuePool<FakeQueueRouteOne>(new FakeQueueRouteFactory<FakeQueueRouteOne>(this.actionEventProxy), policy, this.actionEventProxy);
            const int MessagesToSend = 50;

            var routingNotification = new FakeNotification<FakeMessage>();

            for (int i = 0; i < MessagesToSend; i++)
            {
                var message = Generate.PriorityMessage(
                    String.Format("Message {0}", i), 
                    this.deadline, 
                    DateTimeOffset.Now, 
                    this.escalationTime, 
                    20);

                target.SendMessage(new MessageRoutingRequest<FakeMessage>(message, routingNotification));
            }

            if (Debugger.IsAttached)
            {
                target.WaitForCompletion();
            }
            else
            {
                Assert.IsTrue(target.WaitForCompletion(TimeSpan.FromSeconds(2)));
            }

            var actual = target.HighestConcurrency;
            Assert.IsTrue(actual > 1);
        }

        /// <summary>
        /// A test for RequeueAbortedRequests
        /// </summary>
        [TestMethod]
        public void SendMessage_MultipleMessagesToQueuePool_AllRequestsProcessed()
        {
            var target = new FakeQueuePool<FakeQueueRouteOne>(new FakeQueueRouteFactory<FakeQueueRouteOne>(this.actionEventProxy), this.actionEventProxy);
            var requests = new List<MessageEntry<FakeMessage>>();
            var responses = new List<MessageExit<FakeMessage>>();
            const int MessagesToSend = 5;

            target.RequestReceived += (sender, args) => requests.Add(args.RequestEvent);
            target.RequestCompleted += (sender, args) => responses.Add(args.ResponseEvent);

            var routingNotification = new FakeNotification<FakeMessage>();

            for (int i = 0; i < MessagesToSend; i++)
            {
                var message = Generate.PriorityMessage(
                    String.Format("Message {0}", i), 
                    this.deadline, 
                    DateTimeOffset.Now, 
                    this.escalationTime, 
                    1);

                target.SendMessage(new MessageRoutingRequest<FakeMessage>(message, routingNotification));
            }

            if (Debugger.IsAttached)
            {
                target.WaitForCompletion();
            }
            else
            {
                Assert.IsTrue(
                    target.WaitForCompletion(TimeSpan.FromSeconds(2)), 
                    "The test timed out. {0} requests, {1} responses.", 
                    requests.Count, 
                    responses.Count);
            }

            target.Cancel();
            Assert.AreEqual(MessagesToSend, requests.Count, "Not all requests were received.");
            Assert.AreEqual(MessagesToSend, responses.Count, "Not all valid requests were processed.");
        }

        /// <summary>
        /// A test for RequeueAbortedRequests
        /// </summary>
        [TestMethod]
        public void SendMessage_LimitedResourceQueueingPolicy_QueuePoolLessOrEqualToMaxQueues()
        {
            var processorSamples = new EnumerableQuery<double>(new[] { 12.3, 2.5, 27.2, 3.5 });
            var performanceMonitor = MockRepository.GenerateMock<IPerformanceMonitor>();
            performanceMonitor.Stub(monitor => monitor.ProcessorUsageSamples).Return(processorSamples);
            var policy = new LimitedResourceQueuingPolicy(performanceMonitor) { MinQueueCount = 1, MaxQueueCount = 4 };
            var target = new FakeQueuePool<FakeQueueRouteOne>(new FakeQueueRouteFactory<FakeQueueRouteOne>(this.actionEventProxy), policy, this.actionEventProxy);
            IEnumerable<RequestEventArgs<FakeMessage>> requests;
            IEnumerable<ResponseEventArgs<FakeMessage>> responses;
            const int MessagesToSend = 50;

            var messages = new List<FakeMessage>();

            for (int i = 0; i < MessagesToSend; i++)
            {
                var message = Generate.PriorityMessage(
                    String.Format("Message {0}", i), 
                    this.deadline, 
                    DateTimeOffset.Now, 
                    this.escalationTime, 
                    1);

                messages.Add(message);
            }

            var queueCount = target.QueueCount;

            TimeSpan sendDelay;
            SendRequests(target, out requests, out responses, out sendDelay, messages.ToArray());
            Assert.IsTrue(
                queueCount <= policy.MaxQueueCount, 
                "Max count {0}, actual count {1}", 
                policy.MaxQueueCount, 
                target.QueueCount);
        }

        /// <summary>
        /// A test for RequeueAbortedRequests
        /// </summary>
        [TestMethod]
        public void SendMessage_StaticQueueingPolicy_QueuePoolLessThanOrEqualToMaxQueues()
        {
            var policy = new StaticQueuingPolicy { MaxQueueCount = 4 };
            var target = new FakeQueuePool<FakeQueueRouteOne>(new FakeQueueRouteFactory<FakeQueueRouteOne>(this.actionEventProxy), policy, this.actionEventProxy);
            IEnumerable<RequestEventArgs<FakeMessage>> requests;
            IEnumerable<ResponseEventArgs<FakeMessage>> responses;
            const int MessagesToSend = 50;

            var messages = new List<FakeMessage>();

            for (int i = 0; i < MessagesToSend; i++)
            {
                var message = Generate.PriorityMessage(
                    String.Format("Message {0}", i), 
                    this.deadline, 
                    DateTimeOffset.Now, 
                    this.escalationTime, 
                    1);

                messages.Add(message);
            }

            var queueCount = target.QueueCount;

            TimeSpan sendDelay;
            SendRequests(target, out requests, out responses, out sendDelay, messages.ToArray());
            Assert.IsTrue(
                queueCount <= policy.MaxQueueCount, 
                "Max count {0}, actual count {1}", 
                policy.MaxQueueCount, 
                target.QueueCount);
        }

        /// <summary>
        /// A test for RequeueAbortedRequests
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void SendMessage_MultipleMessagesToQueuePool_AllRequestsReceivedWithoutDelay()
        {
            var target = new FakeQueuePool<FakeQueueRouteOne>(new FakeQueueRouteFactory<FakeQueueRouteOne>(this.actionEventProxy), this.actionEventProxy);
            IEnumerable<RequestEventArgs<FakeMessage>> requests;
            IEnumerable<ResponseEventArgs<FakeMessage>> responses;
            const int MessagesToSend = 20;
            const int MessageDelayMillis = 5;

            var messages = new List<FakeMessage>();

            for (int i = 0; i < MessagesToSend; i++)
            {
                var message = Generate.PriorityMessage(
                    String.Format("Message {0}", i), 
                    this.deadline, 
                    DateTimeOffset.Now, 
                    this.escalationTime, 
                    MessageDelayMillis);

                messages.Add(message);
            }

            TimeSpan sendDelay;
            SendRequests(target, out requests, out responses, out sendDelay, messages.ToArray());

            Assert.IsTrue(
                sendDelay < TimeSpan.FromMilliseconds(MessageDelayMillis), 
                "Send delay was {0}ms (expected less than {1}ms).", 
                sendDelay.TotalMilliseconds, 
                MessageDelayMillis);
        }

        /// <summary>
        /// A test for RequeueAbortedRequests
        /// </summary>
        [TestMethod]
        public void SendMessage_BadResponsePriorToThreeMoreRequests_AllGoodRequestsProcessed()
        {
            var target = new FakeQueuePool<FakeQueueRouteOne>(new FakeQueueRouteFactory<FakeQueueRouteOne>(this.actionEventProxy), this.actionEventProxy);
            IEnumerable<RequestEventArgs<FakeMessage>> requests;
            IEnumerable<ResponseEventArgs<FakeMessage>> responses;

            TimeSpan sendDelay;

            var failMessage = Generate.PriorityMessage("ThrowInResponse", this.deadline, DateTimeOffset.Now, this.escalationTime, 10);
            failMessage.ResponseShouldFailUnhandled = true;

            SendRequests(
                target, 
                out requests, 
                out responses, 
                out sendDelay, 
                Generate.PriorityMessage("One", this.deadline, DateTimeOffset.Now, this.escalationTime, 1), 
                Generate.PriorityMessage("Two", this.deadline, DateTimeOffset.Now, this.escalationTime, 1), 
                Generate.PriorityMessage("Three", this.deadline, DateTimeOffset.Now, this.escalationTime, 1), 
                failMessage, 
                Generate.PriorityMessage("Four", this.deadline, DateTimeOffset.Now, this.escalationTime, 1), 
                Generate.PriorityMessage("Five", this.deadline, DateTimeOffset.Now, this.escalationTime, 1), 
                Generate.PriorityMessage("Six", this.deadline, DateTimeOffset.Now, this.escalationTime, 1));
        }

        /// <summary>
        /// A test for RequeueAbortedRequests
        /// </summary>
        [TestMethod]
        public void SendMessage_BadRequestPriorToThreeMoreRequests_AllGoodRequestsProcessed()
        {
            var target = new FakeQueuePool<FakeQueueRouteOne>(new FakeQueueRouteFactory<FakeQueueRouteOne>(this.actionEventProxy), this.actionEventProxy);
            IEnumerable<RequestEventArgs<FakeMessage>> requests;
            IEnumerable<ResponseEventArgs<FakeMessage>> responses;

            TimeSpan sendDelay;

            SendRequests(
                target, 
                out requests, 
                out responses, 
                out sendDelay, 
                Generate.PriorityMessage("One", this.deadline, DateTimeOffset.Now, this.escalationTime, 1), 
                Generate.PriorityMessage("Two", this.deadline, DateTimeOffset.Now, this.escalationTime, 1), 
                Generate.PriorityMessage("Three", this.deadline, DateTimeOffset.Now, this.escalationTime, 1), 
                Generate.FailureRequest("Bad", this.deadline, DateTimeOffset.Now, this.escalationTime, 10), 
                Generate.PriorityMessage("Four", this.deadline, DateTimeOffset.Now, this.escalationTime, 1), 
                Generate.PriorityMessage("Five", this.deadline, DateTimeOffset.Now, this.escalationTime, 1), 
                Generate.PriorityMessage("Six", this.deadline, DateTimeOffset.Now, this.escalationTime, 1));
        }

        /// <summary>
        /// A test for SendRequest
        /// </summary>
        [TestMethod]
        public void SendMessage_FakeRequestToQueuePool_RequestIsProcessed()
        {
            var target = new FakeQueuePool<FakeQueueRouteOne>(new FakeQueueRouteFactory<FakeQueueRouteOne>(this.actionEventProxy), this.actionEventProxy);
            FakeMessage message = Generate.PriorityMessage("First", this.deadline, DateTimeOffset.Now, this.escalationTime);

            IEnumerable<RequestEventArgs<FakeMessage>> requests;
            IEnumerable<ResponseEventArgs<FakeMessage>> responses;
            TimeSpan sendDelay;
            SendRequests(target, out requests, out responses, out sendDelay, message);

            var requestList = requests as IList<RequestEventArgs<FakeMessage>> ?? requests.ToList();
            var responseList = responses as IList<ResponseEventArgs<FakeMessage>> ?? responses.ToList();

            Assert.IsTrue(requestList.Any());
            Assert.IsTrue(responseList.Any());
        }

        #endregion

        #region Methods

        /// <summary>
        /// The send requests.
        /// </summary>
        /// <typeparam name="TQueue">
        /// The type of queue to queue requests to.
        /// </typeparam>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="requestsReceived">
        /// The requests received.
        /// </param>
        /// <param name="responsesReceived">
        /// The responses received.
        /// </param>
        /// <param name="sendDelay">
        /// The send Delay.
        /// </param>
        /// <param name="messages">
        /// The requests.
        /// </param>
        private static void SendRequests<TQueue>(
            PriorityQueuePool<FakeMessage, TQueue> target, 
            out IEnumerable<RequestEventArgs<FakeMessage>> requestsReceived, 
            out IEnumerable<ResponseEventArgs<FakeMessage>> responsesReceived, 
            out TimeSpan sendDelay, 
            params FakeMessage[] messages) where TQueue : FakeQueueRouteBase
        {
            var requestsReceivedQueue = new ConcurrentQueue<RequestEventArgs<FakeMessage>>();
            var responsesReceivedQueue = new ConcurrentQueue<ResponseEventArgs<FakeMessage>>();

            target.RequestReceived += (sender, args) => requestsReceivedQueue.Enqueue(args);
            target.RequestCompleted += (sender, args) => responsesReceivedQueue.Enqueue(args);

            var watch = Stopwatch.StartNew();

            var routingNotification = new FakeNotification<FakeMessage>();

            foreach (FakeMessage message in messages)
            {
                target.SendMessage(new MessageRoutingRequest<FakeMessage>(message, routingNotification));
            }

            watch.Stop();
            sendDelay = watch.Elapsed;

            if (Debugger.IsAttached)
            {
                target.WaitForCompletion();
            }
            else
            {
                Assert.IsTrue(
                    target.WaitForCompletion(TimeSpan.FromSeconds(5)), 
                    "Timeout occurred. Got {0} requests, {1} responses.", 
                    requestsReceivedQueue.Count, 
                    responsesReceivedQueue.Count);
            }

            var requestsReceivedList = new List<RequestEventArgs<FakeMessage>>();
            RequestEventArgs<FakeMessage> requestEventArgs;

            while (requestsReceivedQueue.TryDequeue(out requestEventArgs))
            {
                requestsReceivedList.Add(requestEventArgs);
            }

            var responsesReceivedList = new List<ResponseEventArgs<FakeMessage>>();
            ResponseEventArgs<FakeMessage> responseEventArgs;

            while (responsesReceivedQueue.TryDequeue(out responseEventArgs))
            {
                responsesReceivedList.Add(responseEventArgs);
            }

            ////Assert.AreEqual(
            ////    messages.Length,
            ////    requestsReceivedList.Count,
            ////    "Not all requests were received.");

            ////Assert.AreEqual(
            ////    messages.Length,
            ////    responsesReceivedList.Count, // + target.AbortedMessages.Count(),
            ////    "Not all requests were processed, or a request was duplicated.");
            requestsReceived = requestsReceivedList;
            responsesReceived = responsesReceivedList;
        }

        #endregion
    }
}