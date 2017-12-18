namespace SAF.MessageQueuing.Tests
{
    using System.Collections.Generic;

    using SAF.Core;

    public class FakeSequenceComparer : Comparer<FakeMessage>
    {
        private static readonly FakeSequenceComparer SequenceComparer = new FakeSequenceComparer();

        public static FakeSequenceComparer RequestTime
        {
            get
            {
                return SequenceComparer;
            }
        }

        public override int Compare(FakeMessage x, FakeMessage y)
        {
            return Evaluate.Compare(x, y, message => message.RequestTime);
        }
    }
}
