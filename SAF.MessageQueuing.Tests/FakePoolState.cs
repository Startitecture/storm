namespace SAF.MessageQueuing.Tests
{
    using System.Collections.Generic;

    public class FakePoolState : IObservablePoolState
    {
        public FakePoolState()
        {
            this.PoolStates = new List<IPriorityQueueState>();
        }

        public string Name { get; set; }

        public IPriorityQueueState QueueState { get; set; }

        public int QueueCount { get; set; }

        public IEnumerable<IPriorityQueueState> PoolStates { get; set; }

        public int HighestConcurrency { get; set; }
    }
}