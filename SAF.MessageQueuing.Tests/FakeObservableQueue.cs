namespace SAF.MessageQueuing.Tests
{
    public class FakeObservableQueue : IObservableQueueState
    {
        public IPriorityQueueState QueueState { get; set; }

        public string Name { get; set; }
    }
}
