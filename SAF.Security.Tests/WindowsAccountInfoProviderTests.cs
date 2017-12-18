// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowsAccountInfoProviderTests.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Security.Tests
{
    using System.Diagnostics.CodeAnalysis;
    using System.DirectoryServices.AccountManagement;
    using System.Security.Claims;
    using System.Security.Principal;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SAF.Core;

    /// <summary>
    /// The windows account info provider tests.
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class WindowsAccountInfoProviderTests
    {
        /// <summary>
        /// The get account info test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void GetAccountInfo_LocalIisAccount_MatchesExpected()
        {
            var target = new WindowsAccountInfoProvider();
            var actual = target.GetAccountInfo(@"IIS APPPOOL\DefaultAppPool");
            Assert.AreEqual(@"DefaultAppPool", actual.AccountName);
        }

        /// <summary>
        /// The get account info test.
        /// </summary>
        [TestMethod]
        public void GetAccountInfo_CurrentIdentity_MatchesExpected()
        {
            var target = new WindowsAccountInfoProvider();
            var windowsIdentity = WindowsIdentity.GetCurrent();
            Assert.IsNotNull(windowsIdentity);
            Principal expected;

            using (var machineContext = new PrincipalContext(ContextType.Machine))
            using (var domainContext = new PrincipalContext(ContextType.Domain))
            {
                expected = UserPrincipal.FindByIdentity(domainContext, windowsIdentity.Name)
                            ?? UserPrincipal.FindByIdentity(machineContext, windowsIdentity.Name);
            }

            Assert.IsNotNull(expected);
            var actual = target.GetAccountInfo(windowsIdentity);
            Assert.AreEqual(expected.UserPrincipalName, actual.AccountName);
        }

        /// <summary>
        /// The get account info test.
        /// </summary>
        [TestMethod]
        public void GetAccountInfo_NonExistentAccount_ThrowsException()
        {
            var target = new WindowsAccountInfoProvider();

            try
            {
                target.GetAccountInfo(@"asdfas\asdfasd");
                Assert.Fail("An exception should have been thrown.");
            }
            catch (OperationException ex)
            {
                Assert.IsInstanceOfType(ex.InnerException, typeof(IdentityNotMappedException));
            }
        }
    }
}