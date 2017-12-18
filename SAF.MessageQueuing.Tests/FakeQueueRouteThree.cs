namespace SAF.MessageQueuing.Tests
{
    using SAF.ActionTracking;

    public class FakeQueueRouteThree : FakeQueueRouteBase
    {
        public FakeQueueRouteThree(IActionEventProxy actionEventProxy)
            : base(actionEventProxy)
        {
        }
    }
}
