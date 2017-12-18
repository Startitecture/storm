using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAF.MessageQueuing.Tests
{
    using SAF.ActionTracking;

    public class FakeQueueRouteFactory<TQueue> : IQueueRouteFactory<TQueue>
    {
        private readonly IActionEventProxy actionEventProxy;

        public FakeQueueRouteFactory(IActionEventProxy actionEventProxy)
        {
            this.actionEventProxy = actionEventProxy;
        }

        public TQueue Create()
        {
            return (TQueue)Activator.CreateInstance(typeof(TQueue), this.actionEventProxy);
        }
    }
}
