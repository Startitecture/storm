namespace SAF.Testing.Common
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Xml.Serialization;

    using Startitecture.Core;

    [ExcludeFromCodeCoverage]
    public class FakeTestItem
    {
        private readonly List<string> testList = new List<string>();

        public string this[int index]
        {
            get
            {
                return this.testList[index];
            }
        }

        public string TestString { get; set; }

        [XmlElement]
        public int TestInt { get; set; }

        [XmlElement]
        public DateTime TestDateTime { get; set; }

        public override bool Equals(object obj)
        {
            return Object.ReferenceEquals(this, obj) || (obj is FakeTestItem && this.Equals(obj as FakeTestItem));
        }

        public bool Equals(FakeTestItem obj)
        {
            if (obj == null)
            {
                return false;
            }

            return Evaluate.CollectionEquals(
                new object[] { this.TestString, this.TestInt }, new object[] { obj.TestString, obj.TestInt });
        }

        public override int GetHashCode()
        {
            return Evaluate.GenerateHashCode(this.TestString, this.TestInt);
        }
    }
}
