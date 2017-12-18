namespace SAF.Data.Persistence.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    ///This is a test class for RetentionPolicyTest and is intended
    ///to contain all RetentionPolicyTest Unit Tests
    ///</summary>
    [TestClass()]
    [ExcludeFromCodeCoverage]
    public class RetentionPolicyTest
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
        ///A test for GetRetentionCutoff
        ///</summary>
        [TestMethod()]
        public void GetRetentionCutoff_10DayUnitDay_Date10DaysPast()
        {
            DateTime policyTime = DateTime.Now;
            TimeSpan period = new TimeSpan(10, 0, 0, 0);
            TimeUnit granularity = TimeUnit.Days;
            DateTime expected = (policyTime - period).Date;
            DateTime actual;
            actual = RetentionPolicy.GetRetentionCutoff(policyTime, period, granularity);
            Assert.AreEqual(expected, actual);
        }
    }
}
