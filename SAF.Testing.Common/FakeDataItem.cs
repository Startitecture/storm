namespace SAF.Testing.Common
{
    using System;

    using SAF.Core;

    public class FakeDataItem : IEquatable<FakeDataItem>
    {
        private static readonly Func<FakeDataItem, object>[] ComparisonProperties =
            {
                item => item.StringItem,
                item => item.DateItem,
                item => item.LongItem,
                item => item.LongOption,
                item => item.DoubleItem,
                item => item.BoolItem
            };

        public string StringItem { get; set; }

        public DateTime DateItem { get; set; }

        public long LongItem { get; set; }

        public long? LongOption { get; set; }

        public double DoubleItem { get; set; }

        public bool BoolItem { get; set; }

        public override int GetHashCode()
        {
            return Evaluate.GenerateHashCode(this, ComparisonProperties);
        }

        public override bool Equals(object obj)
        {
            return Evaluate.Equals(this, obj);
        }

        public bool Equals(FakeDataItem other)
        {
            return Evaluate.Equals(this, other, ComparisonProperties);
        }
    }
}
