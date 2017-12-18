namespace SAF.MessageQueuing.Tests
{
    using System;

    public class FakeQueueState : IPriorityQueueState
    {
        public FakeQueueState()
        {
            this.InstanceIdentifier = Guid.NewGuid();
        }

        public Guid InstanceIdentifier { get; private set; }

        public bool IsBusy { get; set; }

        public bool QueueAborted { get; set; }

        public long QueueLength { get; set; }

        public TimeSpan AverageRequestLatency { get; set; }

        public TimeSpan AverageResponseLatency { get; set; }

        public double FailureRate { get; set; }

        public long MessagesProcessed { get; set; }

        public long MessageRequests { get; set; }
    }
}
