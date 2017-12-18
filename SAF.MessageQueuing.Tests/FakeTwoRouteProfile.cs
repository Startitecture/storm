namespace SAF.MessageQueuing.Tests
{
    using SAF.ActionTracking;

    public class FakeTwoRouteProfile : IMessageRoutingProfile<FakeMessage>
    {
        private readonly ServiceRoutingPath<FakeMessage> routes;

        public FakeTwoRouteProfile(IActionEventProxy actionEventProxy)
        {
            this.routes = new ServiceRoutingPath<FakeMessage>(new FakeQueueRouteOne(actionEventProxy), new FakeQueueRouteTwo(actionEventProxy));
        }

        public ServiceRoutingPath<FakeMessage> RoutingPath
        {
            get
            {
                return this.routes;
            }
        }

        public bool MatchesProfile(FakeMessage message)
        {
            return true;
        }
    }
}
