namespace SAF.MessageQueuing.Tests
{
    using System;
    using System.Linq;

    public class FakeContinuationProvider : IRoutingContinuationProvider<FakeMessage>
    {
        public MessageContinuation<FakeMessage> GetReentryRoute(
            FakeMessage message,
            IServiceRoute<FakeMessage> serviceRoute,
            IMessageRoutingProfile<FakeMessage> profile)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            if (serviceRoute == null)
            {
                throw new ArgumentNullException("serviceRoute");
            }

            if (profile == null)
            {
                throw new ArgumentNullException("profile");
            }

            return profile.RoutingPath.Contains(serviceRoute) ? new MessageContinuation<FakeMessage>(message, serviceRoute) : null;
        }
    }
}