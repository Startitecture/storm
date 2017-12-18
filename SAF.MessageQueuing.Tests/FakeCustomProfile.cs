namespace SAF.MessageQueuing.Tests
{
    using System;

    public class FakeCustomProfile<TMessage> : IMessageRoutingProfile<TMessage>
        where TMessage : IEquatable<TMessage>, IComparable<TMessage>
    {
        private readonly Func<TMessage, bool> acceptanceCriteria;

        public FakeCustomProfile(params IServiceRoute<TMessage>[] routes)
        {
            this.RoutingPath = new ServiceRoutingPath<TMessage>(routes);
        }

        public FakeCustomProfile(Func<TMessage, bool> acceptanceCriteria, params IServiceRoute<TMessage>[] routes)
            : this(routes)
        {
            this.acceptanceCriteria = acceptanceCriteria;
        }

        public ServiceRoutingPath<TMessage> RoutingPath { get; private set; }

        public bool MatchesProfile(TMessage message)
        {
            return acceptanceCriteria == null || acceptanceCriteria(message);
        }
    }
}
