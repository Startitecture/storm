namespace SAF.MessageQueuing.Tests
{
    using System;
    using System.Linq;
    using System.Text;

    using SAF.ActionTracking;

    public class FakeFailureQueueRoute : PriorityQueueRoute<FakeMessage>
    {
        public FakeFailureQueueRoute(IActionEventProxy actionEventProxy)
            : base(actionEventProxy, FakeMessageComparer.MessageDeadline)
        {
        }

        protected override void ProcessMessage(FakeMessage message)
        {
        }
    }
}
