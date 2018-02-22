namespace Startitecture.Orm.Repository.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class FakeDataEntity
    {
        public int FakeEntityId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime DateValue { get; set; }
    }
}
