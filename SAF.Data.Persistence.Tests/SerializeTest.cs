namespace SAF.Data.Persistence.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    ///This is a test class for SerializeTest and is intended
    ///to contain all SerializeTest Unit Tests
    ///</summary>
    [TestClass()]
    [ExcludeFromCodeCoverage]
    public class SerializeTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return this.testContextInstance;
            }
            set
            {
                this.testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        public class TestObject
        {
            public int Integer { get; set; }
            public string SomeString { get; set; }
            public DateTime SomeDateTime { get; set; }
            public Collection<TestCollectionItem> TestCollection { get; set; }

            public override int GetHashCode()
            {
                int colHashCode = 0;

                foreach (TestCollectionItem item in this.TestCollection)
                    colHashCode = colHashCode ^ item.GetHashCode();

                return
                    this.Integer.GetHashCode()
                    ^ this.SomeString.GetHashCode()
                    ^ this.SomeDateTime.GetHashCode()
                    ^ colHashCode;
            }

            public override bool Equals(object obj)
            {
                TestObject to = obj as TestObject;

                return 
                    to != null
                    && this.Integer.Equals(to.Integer)
                    && this.SomeString.Equals(to.SomeString)
                    && this.SomeDateTime.Equals(to.SomeDateTime)
                    && this.TestCollection.SequenceEqual(to.TestCollection);
            }
        }

        public class TestCollectionItem
        {
            public int AnInteger { get; set; }
            public string ThisString { get; set; }
            public DateTime AnotherDateTime { get; set; }

            public override int GetHashCode()
            {
                return 
                    this.AnInteger.GetHashCode() 
                    ^ this.ThisString.GetHashCode() 
                    ^ this.AnotherDateTime.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                TestCollectionItem tci = obj as TestCollectionItem;
                return
                    tci != null
                    && tci.AnInteger.Equals(this.AnInteger)
                    && tci.ThisString.Equals(this.ThisString)
                    && tci.AnotherDateTime.Equals(this.AnotherDateTime);
            }
        }


        /// <summary>
        ///A test for ObjectToXmlFile
        ///</summary>
        [TestMethod()]
        public void ObjectToXmlFile_DeserializedTestObject_EqualsTestObject()
        {
            TestObject obj = new TestObject()
            {
                Integer = 5,
                SomeString = "This is a test string.",
                SomeDateTime = DateTime.Now,
                TestCollection = new Collection<TestCollectionItem>(
                    new TestCollectionItem[] {
                        new TestCollectionItem()
                        {
                            AnInteger = 18,
                            AnotherDateTime = DateTime.Now.Subtract(new TimeSpan(TimeSpan.TicksPerDay)),
                            ThisString = "Test Collection Item number 1"
                        },
                        new TestCollectionItem()
                        {
                            AnInteger = 11,
                            AnotherDateTime = DateTime.Now.Subtract(new TimeSpan(TimeSpan.TicksPerHour)),
                            ThisString = "Test Collection Item number 2"
                        },
                        new TestCollectionItem()
                        {
                            AnInteger = 14,
                            AnotherDateTime = DateTime.Now.Subtract(new TimeSpan(TimeSpan.TicksPerMinute)),
                            ThisString = "Test Collection Item number 3"
                        },
                        new TestCollectionItem()
                        {
                            AnInteger = 58,
                            AnotherDateTime = DateTime.Now.Subtract(new TimeSpan(TimeSpan.TicksPerSecond)),
                            ThisString = "Test Collection Item number 4"
                        }
                    })
            };

            string path =
                String.Format(
                    "{0}{1}{2}.xml",
                    Environment.ExpandEnvironmentVariables("%temp%"), Path.DirectorySeparatorChar, Guid.NewGuid());

            Serialize.ObjectToXmlFile(obj, path);
            XmlSerializer xs = new XmlSerializer(typeof(TestObject));
            object actual = null;

            using (XmlReader reader = new XmlTextReader(path))
            {
                actual = xs.Deserialize(reader);
            }

            File.Delete(path);
            Assert.AreEqual(obj, actual);
        }

        /// <summary>
        ///A test for XmlFileToObject
        ///</summary>
        [TestMethod()]
        public void XmlFileToObject_SerializedTestObject_EqualsTestObject()
        {
            string path = 
                String.Format(
                    "{0}{1}{2}.xml",
                    Environment.ExpandEnvironmentVariables("%temp%"), Path.DirectorySeparatorChar, Guid.NewGuid());

            Type type = typeof(TestObject);
            object actual;
            TestObject expected = new TestObject()
            {
                Integer = 5,
                SomeString = "This is a test string.",
                SomeDateTime = DateTime.Now,
                TestCollection = new Collection<TestCollectionItem>(
                    new TestCollectionItem[] {
                        new TestCollectionItem()
                        {
                            AnInteger = 18,
                            AnotherDateTime = DateTime.Now.Subtract(new TimeSpan(TimeSpan.TicksPerDay)),
                            ThisString = "Test Collection Item number 1"
                        },
                        new TestCollectionItem()
                        {
                            AnInteger = 11,
                            AnotherDateTime = DateTime.Now.Subtract(new TimeSpan(TimeSpan.TicksPerHour)),
                            ThisString = "Test Collection Item number 2"
                        },
                        new TestCollectionItem()
                        {
                            AnInteger = 14,
                            AnotherDateTime = DateTime.Now.Subtract(new TimeSpan(TimeSpan.TicksPerMinute)),
                            ThisString = "Test Collection Item number 3"
                        },
                        new TestCollectionItem()
                        {
                            AnInteger = 58,
                            AnotherDateTime = DateTime.Now.Subtract(new TimeSpan(TimeSpan.TicksPerSecond)),
                            ThisString = "Test Collection Item number 4"
                        }
                    })
            };

            XmlSerializer xs = new XmlSerializer(typeof(TestObject));

            using (XmlWriter writer = new XmlTextWriter(path, System.Text.Encoding.Unicode))
            {
                xs.Serialize(writer, expected);
            }

            actual = Serialize.XmlFileToObject(path, type);
            File.Delete(path);
            Assert.AreEqual(expected, actual);
        }
    }
}
