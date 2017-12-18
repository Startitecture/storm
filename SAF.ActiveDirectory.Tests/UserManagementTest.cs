namespace SAF.ActiveDirectory.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.DirectoryServices;
    using System.DirectoryServices.ActiveDirectory;
    using System.Security;
    using System.DirectoryServices.AccountManagement;

    using SAF.Core;

    /// <summary>
    ///This is a test class for ManageObjectsTest and is intended
    ///to contain all ManageObjectsTest Unit Tests
    ///</summary>
    [TestClass()]
    [ExcludeFromCodeCoverage]
    public class UserManagementTest
    {

        private static Random rand = new Random();
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
        ///////A test for CreateUser
        ///////</summary>
        ////[TestMethod()]
        ////public void CreateUser_TestUser_ExistsInDirectory()
        ////{
        ////    using (Domain domain = Domain.GetCurrentDomain())
        ////    {
        ////        string location = null;

        ////        using (DirectoryEntry locationEntry = domain.GetDirectoryEntry().Children.Find("CN=Users"))
        ////        {
        ////            location = ObjectProperty.GetValueString(locationEntry, ObjectProperty.DistinguishedName);
        ////        }

        ////        string userName = String.Format("managetest{0}", rand.Next(100000, 999999));
        ////        SecureString userPassword = Normalize.GetSecureString("Password1234");
        ////        string firstName = "SAF";
        ////        string middleInitials = "U";
        ////        string lastName = "Test";
        ////        string description = "Test account for SAF.ActiveDirectory.Tests.ManageObjects";

        ////        DirectorySecurityPrincipal newUser =
        ////            UserManagement.CreateUser(
        ////                location, userName, userPassword, firstName, middleInitials, lastName, description);

        ////        Assert.AreNotEqual(Guid.Empty, newUser.DirectoryGuid);

        ////        using 
        ////            (PrincipalContext context = 
        ////                new PrincipalContext(ContextType.Domain, DomainNameContext.DomainName, location))
        ////        {
        ////            using 
        ////                (UserPrincipal principal = 
        ////                    UserPrincipal.FindByIdentity(
        ////                        context, IdentityType.DistinguishedName, newUser.DistinguishedName))
        ////            {
        ////                principal.Delete();
        ////            }
        ////        }
        ////    }
        ////}
    }
}
