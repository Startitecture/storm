namespace SAF.MessageQueuing.Tests
{
    using System;
    using System.Collections.Generic;

    using SAF.Core;

    public class FakeDuplicateEqualityComparer : EqualityComparer<FakeMessage>
    {
        private static readonly Func<FakeMessage, object>[] ComparisonProperties = { item => item.Name };
        
        private static readonly FakeDuplicateEqualityComparer DuplicateEqualityComparer = new FakeDuplicateEqualityComparer();

        public static FakeDuplicateEqualityComparer DuplicateRequest
        {
            get
            {
                return DuplicateEqualityComparer;
            }
        }

        public override bool Equals(FakeMessage x, FakeMessage y)
        {
            return Evaluate.Equals(x, y, ComparisonProperties);
        }

        public override int GetHashCode(FakeMessage obj)
        {
            return Evaluate.GenerateHashCode(obj, ComparisonProperties);
        }
    }
}
