namespace SAF.MessageQueuing.Tests
{
    using System.Collections.Generic;

    using SAF.Core;

    public class FakeMessageComparer : Comparer<FakeMessage>
    {
        private static readonly FakeMessageComparer FakePriorityComparer = new FakeMessageComparer();

        public static FakeMessageComparer MessageDeadline
        {
            get
            {
                return FakePriorityComparer;
            }
        }

        public override int Compare(FakeMessage x, FakeMessage y)
        {
            return Evaluate.Compare(
                x,
                y,
                message => message.Deadline,
                message => message.Deadline - message.EscalationThreshold,
                message => message.RequestTime,
                message => message.RequestGuid);
        }
    }
}
