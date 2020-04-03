// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionContainerTests.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common.Tests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// The transaction container tests.
    /// </summary>
    [TestClass]
    public class TransactionContainerTests
    {
        /// <summary>
        /// The set transaction provider test.
        /// </summary>
        [TestMethod]
        public void SetTransactionProvider_TransactionContainer_TransactionProviderSet()
        {
            var target = new TransactionContainer();
            var provider = new Mock<IRepositoryProvider>().Object;
            target.SetTransactionProvider(provider);
            Assert.AreSame(provider, target.TransactionProvider);
        }

        /// <summary>
        /// The set transaction provider test.
        /// </summary>
        [TestMethod]
        public void TransactionContainer_TransactionProviderDisposed_TransactionProviderNull()
        {
            var target = new TransactionContainer();
            var provider = new Mock<IRepositoryProvider>();
            provider.SetupAdd(repositoryProvider => repositoryProvider.Disposed += It.IsAny<EventHandler>());
            target.SetTransactionProvider(provider.Object);
            Assert.AreSame(provider.Object, target.TransactionProvider);
            provider.Raise(repositoryProvider => repositoryProvider.Disposed += null, EventArgs.Empty);
            Assert.IsNull(target.TransactionProvider);
        }
    }
}