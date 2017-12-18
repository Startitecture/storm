namespace SAF.UserInterface.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using System;
    using System.Globalization;

    /// <summary>
    ///This is a test class for MonitorItemStateConverterTest and is intended
    ///to contain all MonitorItemStateConverterTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MonitorItemStateConverterTest
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
        ///A test for ConvertBack
        ///</summary>
        [TestMethod()]
        public void ConvertBack_StatusPTSF_ArrayTPSF()
        {
            MonitorItemStateConverter target = new MonitorItemStateConverter();
            object value =
                String.Format("Processed {1} of {0} ({2} skipped, {3} failed) {4} tps", 10m, 4m, 1m, 0m, 2.5m);

            Type[] targetTypes =
                new Type[] { typeof(Int32), typeof(Int32), typeof(Int32), typeof(Int32) };

            object parameter = "Processed {1} of {0} ({2} skipped, {3} failed) {4} tps";
            CultureInfo culture = CultureInfo.CurrentCulture;
            object[] expected = new object[] { 10m, 4m, 1m, 0m, 2.5m };
            object[] actual;
            actual = target.ConvertBack(value, targetTypes, parameter, culture);
            for (int i = 0; i < expected.Length; i++)
                Assert.AreEqual(expected[i], actual[i]);
        }

        /// <summary>
        ///A test for Convert
        ///</summary>
        [TestMethod()]
        public void Convert_ArrayTPSF_StatusPTSF()
        {
            MonitorItemStateConverter target = new MonitorItemStateConverter();
            object[] values = new object[] { 10m, 4m, 1m, 0m, 2.5m };
            Type targetType = typeof(String);
            object parameter = "Processed {1} of {0} ({2} skipped, {3} failed) {4} tps";
            CultureInfo culture = CultureInfo.CurrentCulture;
            object expected =
                String.Format("Processed {1} of {0} ({2} skipped, {3} failed) {4} tps", 10m, 4m, 1m, 0m, 2.5m);

            object actual;
            actual = target.Convert(values, targetType, parameter, culture);
            Assert.AreEqual(expected, actual);
        }
    }
}
