namespace SAF.MessageQueuing.Tests
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;

    public class FakeNotification<TMessage> : INotifyMessageRouted<TMessage>
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        private readonly object updateLock = new object();

        private readonly ConcurrentQueue<MessageEvaluation<TMessage>> evaluations = new ConcurrentQueue<MessageEvaluation<TMessage>>();

        private readonly ConcurrentQueue<MessageEntry<TMessage>> routingEvents = new ConcurrentQueue<MessageEntry<TMessage>>();

        private readonly ConcurrentQueue<MessageExit<TMessage>> routedEvents = new ConcurrentQueue<MessageExit<TMessage>>();

        private readonly ConcurrentQueue<MessageEntry<TMessage>> receiptEvents = new ConcurrentQueue<MessageEntry<TMessage>>();

        private readonly ConcurrentQueue<MessageExit<TMessage>> responseEvents = new ConcurrentQueue<MessageExit<TMessage>>();

        private readonly ConcurrentQueue<MessageDelivery<TMessage>> deliveries = new ConcurrentQueue<MessageDelivery<TMessage>>();

        private long evaluationCount;

        private long deliveryCount;

        public bool CaptureAllEvents { get; set; }

        public bool SkipSuccessfulDeliveries { get; set; }

        public IEnumerable<MessageEvaluation<TMessage>> Evaluations
        {
            get
            {
                return this.evaluations;
            }
        }

        public IEnumerable<MessageEntry<TMessage>> RoutingEvents
        {
            get
            {
                return this.routingEvents;
            }
        }

        public IEnumerable<MessageExit<TMessage>> RoutedEvents
        {
            get
            {
                return this.routedEvents;
            }
        }

        public IEnumerable<MessageEntry<TMessage>> ReceiptEvents
        {
            get
            {
                return this.receiptEvents;
            }
        }

        public IEnumerable<MessageExit<TMessage>> ResponseEvents
        {
            get
            {
                return this.responseEvents;
            }
        }

        public IEnumerable<MessageDelivery<TMessage>> Deliveries
        {
            get
            {
                return this.deliveries;
            }
        }

        public void OnMessageEvaluated(MessageEvaluation<TMessage> evaluationEvent)
        {
            ////if (evaluationEvent.EvaluationError == null)
            ////{
                Interlocked.Increment(ref this.evaluationCount);
            ////}

            if (this.CaptureAllEvents)
            {
                this.evaluations.Enqueue(evaluationEvent);
            }
        }

        public void OnMessageRouting(MessageEntry<TMessage> requestEvent)
        {
            if (this.CaptureAllEvents)
            {
                this.routingEvents.Enqueue(requestEvent);
            }
        }

        public void OnMessageRouted(MessageExit<TMessage> responseEvent)
        {
            if (this.CaptureAllEvents)
            {
                this.routedEvents.Enqueue(responseEvent);
            }
        }

        public void OnMessageReceived(MessageEntry<TMessage> requestEvent)
        {
            if (this.CaptureAllEvents)
            {
                this.receiptEvents.Enqueue(requestEvent);
            }
        }

        public void OnMessageReturned(MessageExit<TMessage> responseEvent)
        {
            if (this.CaptureAllEvents)
            {
                this.responseEvents.Enqueue(responseEvent);
            }
        }

        public void OnMessageDelivered(MessageRoutingRequest<TMessage> routingRequest)
        {
            Interlocked.Increment(ref this.deliveryCount);

            if (!this.SkipSuccessfulDeliveries || routingRequest.RoutingError != null)
            {
                this.deliveries.Enqueue(new MessageDelivery<TMessage>(routingRequest));
            }

            if (this.deliveryCount < this.evaluationCount)
            {
                return;
            }

            lock (this.updateLock)
            {
                if (this.deliveryCount == this.evaluationCount)
                {
                    Monitor.Pulse(this.updateLock);
                }
            }
        }

        public bool WaitForDelivery()
        {
            return this.WaitForDelivery(TimeSpan.FromMilliseconds(-1));
        }

        public bool WaitForDelivery(TimeSpan time)
        {
            if (this.deliveryCount == this.evaluationCount)
            {
                return true;
            }

            lock (this.updateLock)
            {
                return this.deliveryCount == this.evaluationCount || Monitor.Wait(this.updateLock, time);
            }
        }
    }
}