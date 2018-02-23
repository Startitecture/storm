namespace Startitecture.Orm.Testing.Model
{
    using System;

    using Startitecture.Core;

    public class FakeDataSource : IEquatable<FakeDataSource>
    {
        private static readonly Func<FakeDataSource, object>[] ComparisonProperties =
            {
                item => item.StringValue,
                item => item.DateValue,
                item => item.LongValue,
                item => item.DoubleValue,
                item => item.BoolValue
            };

        public string StringValue { get; set; }

        public DateTime DateValue { get; set; }

        public long LongValue { get; set; }

        public long? LongValueOption { get; set; }

        public double DoubleValue { get; set; }

        public bool BoolValue { get; set; }

        public override int GetHashCode()
        {
            return Evaluate.GenerateHashCode(this, ComparisonProperties);
        }

        public override bool Equals(object obj)
        {
            return Evaluate.Equals(this, obj);
        }

        public bool Equals(FakeDataSource other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }
    }
}
