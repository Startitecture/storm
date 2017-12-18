namespace SAF.ActiveDirectory.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    ///This is a test class for SearchTest and is intended
    ///to contain all SearchTest Unit Tests
    ///</summary>
    [TestClass()]
    [ExcludeFromCodeCoverage]
    public class SearchTest
    {
        private static Random rand = new Random();
        ////private static string testContainerPath;
        ////private static string testUserPath;


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
        
        ////// You can use the following additional attributes as you write your tests:
        //////
        ////// Use ClassInitialize to run code before running the first test in the class
        ////[ClassInitialize()]
        ////public static void MyClassInitialize(TestContext testContext)
        ////{
        ////    using (Domain domain = Domain.GetCurrentDomain())
        ////    {
        ////        using (DirectoryEntry location = domain.GetDirectoryEntry().Children.Find("CN=Users"))
        ////        {
        ////            testContainerPath = location.Path;

        ////            using
        ////                (DirectoryEntry testUser =
        ////                    ManageObjects.CreateUser(
        ////                        cacheManager,
        ////                        location,
        ////                        String.Format("searchtest{0}", rand.Next(100000, 999999)),
        ////                        Normalize.GetSecureString("Password1234"),
        ////                        "Search",
        ////                        "Test",
        ////                        "Test user for SAF.ActiveDirectory.Tests.SearchTest"))
        ////            {
        ////                testUserPath = testUser.Path;
        ////            }
        ////        }
        ////    }
        ////}
        
        ////// Use ClassCleanup to run code after all tests in a class have run
        ////[ClassCleanup()]
        ////public static void MyClassCleanup()
        ////{
        ////    using (DirectoryEntry testUser = new DirectoryEntry(testUserPath))
        ////    {
        ////        using (DirectoryEntry container = testUser.Parent)
        ////        {
        ////            container.Children.Remove(testUser);
        ////            container.CommitChanges();
        ////        }
        ////    }
        ////}
        
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
        ///////A test for TryConvertTimeStamp
        ///////</summary>
        ////[TestMethod()]
        ////public void TryConvertTimeStamp_PasswordNeverSet_ReturnsCorrectTimestamp()
        ////{
        ////    using (DirectoryEntry entry = new DirectoryEntry(testUserPath))
        ////    {
        ////        string property = Schema.PasswordLastSetAttribute;
        ////        DateTime time;

        ////        // This is the time that AD uses as a minimum value.
        ////        DateTime timeExpected = new DateTime(1600, 12, 31, 19, 0, 0);
        ////        bool expected = true;
        ////        bool actual;
        ////        actual = Search.TryConvertTimeStamp(entry, property, out time);
        ////        Assert.CollectionEquals(timeExpected, time);
        ////        Assert.CollectionEquals(expected, actual);
        ////    }
        ////}

        /////// <summary>
        ///////A test for TryConvertTimeStamp
        ///////</summary>
        ////[TestMethod()]
        ////public void TryConvertTimeStamp_AccountNeverExpires_ReturnsCorrectTimestamp()
        ////{
        ////    using (DirectoryEntry entry = new DirectoryEntry(testUserPath))
        ////    {
        ////        string property = Schema.ATTR_ACCT_EXPIRES;
        ////        DateTime time;

        ////        // The API handles the overflow which occurs when the "AccountExpires" property is set to the maximum
        ////        // value of 
        ////        DateTime timeExpected = DateTime.MaxValue;
        ////        bool expected = true;
        ////        bool actual;
        ////        actual = Search.TryConvertTimeStamp(entry, property, out time);
        ////        Assert.CollectionEquals(timeExpected, time);
        ////        Assert.CollectionEquals(expected, actual);
        ////    }
        ////}

        /////// <summary>
        ///////A test for TryConvertTimeStamp
        ///////</summary>
        ////[TestMethod()]
        ////public void TryConvertTimeStamp_NoBadPassword_ReturnsCorrectTimestamp()
        ////{
        ////    using (DirectoryEntry entry = new DirectoryEntry(testUserPath))
        ////    {
        ////        string property = Schema.ATTR_BAD_PWD_TIME;
        ////        DateTime time;

        ////        // This is the time that AD uses as a minimum value.
        ////        DateTime timeExpected = new DateTime(1600, 12, 31, 19, 0, 0);
        ////        bool expected = true;
        ////        bool actual;
        ////        actual = Search.TryConvertTimeStamp(entry, property, out time);
        ////        Assert.CollectionEquals(timeExpected, time);
        ////        Assert.CollectionEquals(expected, actual);
        ////    }
        ////}
    }
}
