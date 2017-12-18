namespace SAF.MessageQueuing.Tests
{
    using System;

    using SAF.ActionTracking;

    public class FakeNormalSpeedRoute : FakeQueueRouteBase
    {
        public FakeNormalSpeedRoute(IActionEventProxy actionEventProxy)
            : base(actionEventProxy)
        {
            this.MessageDelay = TimeSpan.FromMilliseconds(100);
        }
    }
}
