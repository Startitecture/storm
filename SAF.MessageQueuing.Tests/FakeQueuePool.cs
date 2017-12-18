namespace SAF.MessageQueuing.Tests
{
    using SAF.ActionTracking;

    public class FakeQueuePool<TQueue> : PriorityQueuePool<FakeMessage, TQueue>
        where TQueue : IPriorityQueueRoute<FakeMessage>
    {
        public FakeQueuePool(IQueueRouteFactory<TQueue> queueRouteFactory, IActionEventProxy actionEventProxy)
            : base(
                queueRouteFactory,
                FakeMessageComparer.MessageDeadline,
                StaticQueuingPolicy.DefaultPolicy,
                actionEventProxy)
        {
        }

        public FakeQueuePool(IQueueRouteFactory<TQueue> queueRouteFactory, IQueuingPolicy policy, IActionEventProxy actionEventProxy)
            : base(queueRouteFactory, FakeMessageComparer.MessageDeadline, policy, actionEventProxy)
        {
        }
    }
}
