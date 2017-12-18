namespace SAF.MessageQueuing.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using SAF.Core;

    public class FakeService : IEquatable<FakeService>
    {
        public Guid Id { get; private set; }

        public FakeService()
        {
            this.Id = Guid.NewGuid();
        }

        public override int GetHashCode()
        {
            return Evaluate.GenerateHashCode(this.Id);
        }

        public override bool Equals(object obj)
        {
            return Evaluate.Equals(this, obj);
        }

        public bool Equals(FakeService other)
        {
            return Evaluate.Equals(this, other, service => service.Id);
        }

        public override string ToString()
        {
            return this.Id.ToString();
        }
    }
}
