namespace SAF.Tests.Integration
{
    using System;

    using SAF.Data;

    public class LongGenerator : DataSource
    {
        private Random rand = new Random();

        public LongGenerator(long itemsToGenerate)
            : base("Long Generator")
        {
            this.ItemsToGenerate = itemsToGenerate;
        }

        public override string Location
        {
            get { return String.Empty; }
        }

        public override DateTimeOffset LastModified
        {
            get { return DateTimeOffset.Now; }
        }

        public long ItemsToGenerate { get; private set; }

        public long GenerateLong()
        {
            return this.rand.Next(Int32.MaxValue);
        }
    }
}
