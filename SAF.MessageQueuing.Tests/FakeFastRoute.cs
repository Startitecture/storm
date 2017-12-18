namespace SAF.MessageQueuing.Tests
{
    using System;

    using SAF.ActionTracking;

    public class FakeFastRoute : FakeQueueRouteBase
    {
        public FakeFastRoute(IActionEventProxy actionEventProxy)
            : base(actionEventProxy)
        {
            this.MessageDelay = TimeSpan.FromMilliseconds(20);
        }
    }
}
