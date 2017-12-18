// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WmiProxyTest.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SAF.LocalMachine.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    ///     This is a test class for WmiProxyTest and is intended
    ///     to contain all WmiProxyTest Unit Tests
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class WmiProxyTest
    {
        /// <summary>
        ///     The test context instance.
        /// </summary>
        private TestContext testContextInstance;

        /// <summary>
        ///     Gets or sets the test context which provides
        ///     information about and functionality for the current test run.
        /// </summary>
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

        /// <summary>
        ///     A test for WmiProxy Constructor
        /// </summary>
        [TestMethod]
        public void Load_StandardQuery_ReturnsResult()
        {
            var queryFailed = false;
            var completed = false;
            var completeLock = new object();
            var target = new WmiProxy();
            var propertyData = new Dictionary<string, WmiPropertyCollection>();

            target.ItemsProduced += delegate
                {
                    while (target.Items.ConsumeNext())
                    {
                        var result = target.Items.Current;

                        var query = result.Query;

                        var keyData = result.Properties.FirstOrDefault(x => x.Name == query.UniqueProperty);

                        if (keyData != null)
                        {
                            var key = Convert.ToString(keyData.Value);

                            if (!propertyData.ContainsKey(key))
                            {
                                propertyData.Add(key, result.Properties);
                            }
                        }
                        else
                        {
                            queryFailed = true;
                        }
                    }
                };

            // TODO: Stopping before the item is delivered.
            target.ProcessStopped += delegate
                {
                    lock (completeLock)
                    {
                        completed = true;
                        Monitor.Pulse(completeLock);
                    }
                };

            target.BeginExtraction(
                new WmiQuery(
                    new[]
                        {
                            "IPAddress"
                        }, 
                    new[] { "Caption", "DefaultIPGateway", "Description", "DHCPServer", "DNSDomain", "IPSubnet" })
                    {
                        ObjectClass
                            =
                            "Win32_NetworkAdapterConfiguration", 
                        UniqueProperty
                            =
                            "MACAddress"
                    });

            lock (completeLock)
            {
                completed = completed || Monitor.Wait(completeLock, 1000);
            }

            Assert.IsFalse(queryFailed, "Query returned a property collection without a unique property set.");
            Assert.IsTrue(completed, "Test timed out.");
            Assert.IsTrue(propertyData.Count > 0, "No items retrieved.");

            foreach (var item in propertyData)
            {
                Trace.TraceInformation("{0}:", item.Key);

                foreach (var value in item.Value)
                {
                    Trace.TraceInformation(
                        " - {0} ({1}, {2}) arr:{3} = {4}", 
                        value.Name, 
                        value.Origin, 
                        value.Type, 
                        value.IsArray, 
                        WmiQuery.GetValue(value));
                }
            }
        }

        #region Additional test attributes

        // You can use the following additional attributes as you write your tests:
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext)
        // {
        // }
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup()
        // {
        // }
        // Use TestInitialize to run code before running each test
        // [TestInitialize()]
        // public void MyTestInitialize()
        // {
        // }
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup()
        // {
        // }
        #endregion
    }
}