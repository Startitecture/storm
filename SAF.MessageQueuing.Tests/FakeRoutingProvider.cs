namespace SAF.MessageQueuing.Tests
{
    using System.Collections.Generic;

    using SAF.Core;

    public class FakeRoutingProvider : FirstMatchProfileProvider<FakeMessage>
    {
        private readonly IServiceRoute<FakeMessage> failureRoute;

        private readonly IRoutingFailurePolicy<FakeMessage> failurePolicy;

        private readonly IMessageRoutingProfile<FakeMessage>[] routingProfiles;

        public FakeRoutingProvider(IServiceRoute<FakeMessage> failureRoute, params IMessageRoutingProfile<FakeMessage>[] routingProfiles)
            : this(new FakeContinuationProvider(), failureRoute, new ZeroRetryPolicy<FakeMessage>(), routingProfiles)
        {
        }

        public FakeRoutingProvider(
            IRoutingContinuationProvider<FakeMessage> fakeContinuationProvider,
            IServiceRoute<FakeMessage> failureRoute,
            IRoutingFailurePolicy<FakeMessage> failurePolicy,
            params IMessageRoutingProfile<FakeMessage>[] routingProfiles)
            : base(fakeContinuationProvider)
        {
            this.failureRoute = failureRoute;
            this.failurePolicy = failurePolicy;
            this.routingProfiles = routingProfiles;
        }

        public override string PendingMessageQueueName
        {
            get
            {
                return "PendingFakeMessageQueueRoute";
            }
        }

        public override IEqualityComparer<FakeMessage> IdentityComparer
        {
            get
            {
                return EqualityComparer<FakeMessage>.Default;
            }
        }

        public override IComparer<FakeMessage> SequenceComparer
        {
            get
            {
                return FakeSequenceComparer.RequestTime;
            }
        }

        public override IEqualityComparer<FakeMessage> DuplicateEqualityComparer
        {
            get
            {
                return FakeDuplicateEqualityComparer.DuplicateRequest;
            }
        }

        public override IServiceRoute<FakeMessage> FailureRoute
        {
            get
            {
                return this.failureRoute;
            }
        }

        public override IComparer<FakeMessage> PriorityComparer
        {
            get
            {
                return FakeMessageComparer.MessageDeadline;
            }
        }

        public override IRoutingFailurePolicy<FakeMessage> FailurePolicy
        {
            get
            {
                return this.failurePolicy;
            }
        }

        public override void FinalizeRequest(MessageRoutingRequest<FakeMessage> routingRequest)
        {
        }

        protected override IEnumerable<IMessageRoutingProfile<FakeMessage>> LoadProfiles()
        {
            return this.routingProfiles;
        }
    }
}