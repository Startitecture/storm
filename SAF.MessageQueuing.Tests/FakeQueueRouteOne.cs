namespace SAF.MessageQueuing.Tests
{
    using SAF.ActionTracking;

    public class FakeQueueRouteOne : FakeQueueRouteBase
    {
        public FakeQueueRouteOne(IActionEventProxy actionEventProxy)
            : base(actionEventProxy)
        {
        }
    }
}
