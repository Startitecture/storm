namespace SAF.MessageQueuing.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using SAF.ActionTracking;

    public class FakeQueueRouteTwo : FakeQueueRouteBase
    {
        public FakeQueueRouteTwo(IActionEventProxy actionEventProxy)
            : base(actionEventProxy)
        {
        }
    }
}
