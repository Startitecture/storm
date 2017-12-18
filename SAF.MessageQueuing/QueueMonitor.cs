// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueueMonitor.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Monitors the activity of a <see cref="IPriorityMessage" /> queue.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.MessageQueuing
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;

    using SAF.ProcessEngine;

    /// <summary>
    /// Monitors the activity of a <see cref="IPriorityMessage"/> queue.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of <see cref="IPriorityMessage"/> processed by the queue.
    /// </typeparam>
    public class QueueMonitor<TMessage> : IPriorityQueueState
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        /// <summary>
        /// The latency sample size.
        /// </summary>
        private const int LatencySampleSize = 10;

        /// <summary>
        /// The failure sample size.
        /// </summary>
        private const int FailureSampleSize = 100;

        /// <summary>
        /// The request watch.
        /// </summary>
        private readonly Stopwatch requestWatch = Stopwatch.StartNew();

        /// <summary>
        /// The process.
        /// </summary>
        private readonly IProcessEngine process;

        /// <summary>
        /// The request latencies.
        /// </summary>
        private readonly ConcurrentQueue<TimeSpan> requestLatencies = new ConcurrentQueue<TimeSpan>();

        /// <summary>
        /// The response latencies.
        /// </summary>
        private readonly ConcurrentQueue<TimeSpan> responseLatencies = new ConcurrentQueue<TimeSpan>();

        /// <summary>
        /// The request failures.
        /// </summary>
        private readonly ConcurrentQueue<bool> failureEvents = new ConcurrentQueue<bool>();

        /// <summary>
        /// The message monitors.
        /// </summary>
        private readonly ConcurrentDictionary<TMessage, Stopwatch> messageTimers = new ConcurrentDictionary<TMessage, Stopwatch>();

        /// <summary>
        /// The get total milliseconds.
        /// </summary>
        private readonly Func<TimeSpan, double> getTotalMilliseconds = x => x.TotalMilliseconds;

        /// <summary>
        /// The message requests.
        /// </summary>
        private long messageRequests;

        /// <summary>
        /// The messages received.
        /// </summary>
        private long messagesReceived;

        /// <summary>
        /// The messages processed.
        /// </summary>
        private long messagesProcessed;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueMonitor{TMessage}"/> class.
        /// </summary>
        /// <param name="process">
        /// The process to monitor.
        /// </param>
        public QueueMonitor(IProcessEngine process)
        {
            this.process = process;
        }

        /// <summary>
        /// Gets the number of message requests.
        /// </summary>
        public long MessageRequests
        {
            get
            {
                return this.messageRequests;
            }
        }

        /// <summary>
        /// Gets the average time between the last hundred requests.
        /// </summary>
        public TimeSpan AverageRequestLatency
        {
            get
            {
                var responses = this.requestLatencies.ToList();

                if (responses.Count == 0)
                {
                    return TimeSpan.Zero;
                }

                var firstTime = responses.First();
                var elapsedTime = this.requestWatch.Elapsed - firstTime;
                var averageTicks = elapsedTime.Ticks / responses.Count;
                return TimeSpan.FromTicks(averageTicks);
            }
        }

        /// <summary>
        /// Gets the average time between the last hundred responses to requests.
        /// </summary>
        public TimeSpan AverageResponseLatency
        {
            get
            {
                // Use the max value if there's been no response yet.
                var responses = this.responseLatencies.ToList();

                return responses.Any() ? TimeSpan.FromMilliseconds(responses.Average(this.getTotalMilliseconds)) : TimeSpan.MaxValue;
            }
        }

        /// <summary>
        /// Gets the failure rate over the last hundred requests.
        /// </summary>
        public double FailureRate
        {
            get
            {
                var results = this.failureEvents.ToList();

                if (results.Any())
                {
                    return results.Count(x => x) / (double)results.Count;
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the queue is busy.
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return this.process.IsBusy;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the queue has been aborted.
        /// </summary>
        public bool QueueAborted { get; private set; }

        /// <summary>
        /// Gets the number of elements in the queue.
        /// </summary>
        public long QueueLength
        {
            get
            {
                return Interlocked.Read(ref this.messageRequests) - Interlocked.Read(ref this.messagesProcessed);
            }
        }

        /// <summary>
        /// Gets the number of messages processed by the queue route.
        /// </summary>
        public long MessagesProcessed
        {
            get
            {
                return this.messagesProcessed;
            }
        }

        /// <summary>
        /// Notifies the monitor of a message processing request.
        /// </summary>
        public void NotifyPending()
        {
            Interlocked.Increment(ref this.messageRequests);
            UpdateSampleCollection(this.requestWatch.Elapsed, this.requestLatencies, LatencySampleSize);
        }

        /// <summary>
        /// Notifies the current monitor that a message has been received by the queue.
        /// </summary>
        /// <param name="requestEvent">
        /// The request event.
        /// </param>
        public void NotifyReceipt(MessageEntry<TMessage> requestEvent)
        {
            if (requestEvent == null)
            {
                throw new ArgumentNullException("requestEvent");
            }

            Interlocked.Increment(ref this.messagesReceived);
            this.messageTimers.TryAdd(requestEvent.RoutingRequest.Message, Stopwatch.StartNew());
        }

        /// <summary>
        /// Notifies the current monitor that the queue has responded to a message.
        /// </summary>
        /// <param name="responseEvent">
        /// The response event.
        /// </param>
        public void NotifyResponse(MessageExit<TMessage> responseEvent)
        {
            if (responseEvent == null)
            {
                throw new ArgumentNullException("responseEvent");
            }

            Interlocked.Increment(ref this.messagesProcessed);
            UpdateSampleCollection(responseEvent.EventError != null, this.failureEvents, FailureSampleSize);

            Stopwatch timer;

            if (!this.messageTimers.TryRemove(responseEvent.RoutingRequest.Message, out timer))
            {
                return;
            }

            timer.Stop();
            UpdateSampleCollection(timer.Elapsed, this.responseLatencies, LatencySampleSize);
        }

        /// <summary>
        /// Notifies the current monitor that the queue has aborted.
        /// </summary>
        public void NotifyQueueAbort()
        {
            this.QueueAborted = true;
        }

        /// <summary>
        /// Updates latency counters.
        /// </summary>
        /// <typeparam name="T">
        /// The type of item to track.
        /// </typeparam>
        /// <param name="item">
        /// The item to add.
        /// </param>
        /// <param name="history">
        /// The stack of historical latencies.
        /// </param>
        /// <param name="sampleSize">
        /// The sample size to maintain.
        /// </param>
        private static void UpdateSampleCollection<T>(T item, ConcurrentQueue<T> history, int sampleSize)
        {
            history.Enqueue(item);

            while (history.Count > sampleSize)
            {
                T result;
                history.TryDequeue(out result);
            }
        }
    }
}
