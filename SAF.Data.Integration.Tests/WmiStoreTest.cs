namespace SAF.Data.Integration.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    ///This is a test class for WmiStoreTest and is intended
    ///to contain all WmiStoreTest Unit Tests
    ///</summary>
    [TestClass()]
    public class WmiStoreTest
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


        /////// <summary>
        ///////A test for ParseWmiTime
        ///////</summary>
        ////[TestMethod()]
        ////public void ParseWmiTime_WmiTime_ParsedAsDateTimeOffset()
        ////{
        ////    ConnectionOptions co = new ConnectionOptions();
        ////    co.Impersonation = ImpersonationLevel.Impersonate;
        ////    ManagementScope ms =
        ////        new ManagementScope(
        ////            String.Format("{0}.{1}", WmiStore.WmiHostRoot, WmiStore.WmiPathRoot), co);

        ////    ManagementObjectCollection queryCollection = null;
        ////    ObjectQuery oq = new System.Management.ObjectQuery("select * from Win32_OperatingSystem");

        ////    using (ManagementObjectSearcher query = new ManagementObjectSearcher(ms, oq))
        ////    {
        ////        queryCollection = query.Get();
        ////    }

        ////    Assert.IsNotNull(queryCollection);

        ////    string timeString = string.Empty;

        ////    foreach (ManagementObject mo in queryCollection)
        ////    {
        ////        timeString = Convert.ToString(mo["LocalDateTime"]);
        ////    }

        ////    int offsetMinutes = Int32.Parse(timeString.Substring(timeString.Length - 3, 3));

        ////    TimeSpan offset = new TimeSpan(0, offsetMinutes, 0);

        ////    string fixedTimeString =
        ////        String.Format(
        ////            "{0}{1}:{2:00}", timeString.Substring(0, timeString.Length - 3), offset.Hours, offset.Minutes);

        ////    DateTimeOffset expected = 
        ////        DateTimeOffset.ParseExact(
        ////            fixedTimeString, WmiStore.FixedWmiDateTimeFormat, System.Globalization.CultureInfo.CurrentCulture);

        ////    DateTimeOffset actual;
        ////    actual = WmiStore.ParseWmiTime(timeString);
        ////    Assert.AreEqual(expected, actual);
        ////}
    }
}
