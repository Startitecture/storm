namespace SAF.MessageQueuing.Tests
{
    using System;

    using SAF.ActionTracking;
    using SAF.Core;

    public class FakeSlowRoute : FakeQueueRouteBase
    {
        public FakeSlowRoute(IActionEventProxy actionEventProxy)
            : base(actionEventProxy)
        {
            this.MessageDelay = TimeSpan.FromMilliseconds(500);
        }
    }
}
