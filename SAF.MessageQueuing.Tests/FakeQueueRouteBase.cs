namespace SAF.MessageQueuing.Tests
{
    using System;
    using System.Diagnostics;
    using System.Threading;

    using SAF.ActionTracking;
    using SAF.Core;

    public class FakeQueueRouteBase : PriorityQueueRoute<FakeMessage>
    {
        public FakeQueueRouteBase(IActionEventProxy actionEventProxy)
            : base(actionEventProxy, FakeMessageComparer.MessageDeadline)
        {
            this.RequestCompleted += (sender, args) =>
                {
                    if (args.ResponseEvent.RoutingRequest.Message.ResponseShouldFailUnhandled)
                    {
                        throw new NotImplementedException();
                    }
                };
        }

        protected TimeSpan MessageDelay { get; set; }

        protected override void ProcessMessage(FakeMessage message)
        {
            Thread.Sleep(message.RequestLatency + this.MessageDelay);

            if (message is UnhandledExceptionMessage)
            {
                // Will break the queue.
                throw new NotImplementedException(); 
            }

            if (message.RequestShouldFail)
            {
                // Will not break the queue.
                throw new BusinessException(message, "You wanted an exception, here it is.");
            }
        }
    }
}