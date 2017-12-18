namespace SAF.LocalMachine.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    ///This is a test class for WmiQueryTest and is intended
    ///to contain all WmiQueryTest Unit Tests
    ///</summary>
    [TestClass()]
    [ExcludeFromCodeCoverage]
    public class WmiQueryTest
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


        /// <summary>
        ///A test for QueryLocal
        ///</summary>
        [TestMethod()]
        public void QueryLocal_ComputerSystem_ReturnsResult()
        {
            IEnumerable<WmiPropertyCollection> actual = RunQuery(WmiQuery.ComputerSystemQuery);
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Count() > 0);
        }

        /// <summary>
        ///A test for QueryLocal
        ///</summary>
        [TestMethod()]
        public void QueryLocal_SystemEnclosure_ReturnsResult()
        {
            IEnumerable<WmiPropertyCollection> actual = RunQuery(WmiQuery.SystemEnclosureQuery);
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Count() > 0);
        }

        /// <summary>
        ///A test for QueryLocal
        ///</summary>
        [TestMethod()]
        public void QueryLocal_ComputerSystemProduct_ReturnsResult()
        {
            IEnumerable<WmiPropertyCollection> actual = RunQuery(WmiQuery.ComputerSystemProductQuery);
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Count() > 0);
        }

        /// <summary>
        ///A test for QueryLocal
        ///</summary>
        [TestMethod()]
        public void QueryLocal_Bios_ReturnsResult()
        {
            IEnumerable<WmiPropertyCollection> actual = RunQuery(WmiQuery.BiosQuery);
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Count() > 0);
        }

        /// <summary>
        ///A test for QueryLocal
        ///</summary>
        [TestMethod()]
        public void QueryLocal_Processor_ReturnsResult()
        {
            IEnumerable<WmiPropertyCollection> actual = RunQuery(WmiQuery.ProcessorQuery);
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Count() > 0);
        }

        private static IEnumerable<WmiPropertyCollection> RunQuery(WmiQuery query)
        {
            IEnumerable<WmiPropertyCollection> results = WmiQuery.QueryLocal(query);

            System.Diagnostics.Trace.TraceInformation(" :::: {0} ::::", query.ObjectClass);

            foreach (var item in results)
            {
                System.Diagnostics.Trace.TraceInformation("   -- {0} --", item.First(x => x.Name == query.UniqueProperty).Value);

                foreach (var property in item)
                {
                    System.Diagnostics.Trace.TraceInformation("    {0} ({1}) = {2}", property.Name, property.Type, property.Value);
                }
            }

            return results;
        }
    }
}
