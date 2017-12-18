// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegistryProxyTest.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.LocalMachine.Tests
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Win32;

    /// <summary>
    /// This is a test class for RegistryProxyTest and is intended
    /// to contain all RegistryProxyTest Unit Tests
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class RegistryProxyTest
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        #endregion

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
        #region Public Methods and Operators

        /// <summary>
        /// A test for RegistryProxy Constructor
        /// </summary>
        [TestMethod]
        public void Load_StandardQuery_ReturnsResult()
        {
            bool queryFailed = false;
            bool completed = false;
            var completeLock = new object();
            var target = new RegistryProxy();
            var registryData = new ConcurrentDictionary<string, RegistryValueCollection>();

            target.ItemsProduced += delegate
                {
                    while (target.Items.ConsumeNext())
                    {
                        RegistryValueCollection result = target.Items.Current;

                        RegistryValue key = result.FirstOrDefault(x => Convert.ToString(x.ParentKey).Length > 0);

                        if (key == null)
                        {
                            queryFailed = true;
                        }
                        else
                        {
                            registryData.AddOrUpdate(key.ParentKey, result, (s, collection) => result);
                        }
                    }
                };

            target.ProcessStopped += delegate
                {
                    lock (completeLock)
                    {
                        completed = true;
                        Monitor.Pulse(completeLock);
                    }
                };

            var query = new RegistryQuery
                            {
                                Hive = RegistryHive.LocalMachine, 
                                SubkeyPath = @"Software\Microsoft\Windows\CurrentVersion\Uninstall\", 
                                RequireAllRequiredValues = true, 
                                SearchSubkeys = true
                            };

            query.RequiredValueNames.AddRange(new[] { "DisplayName", "Publisher" });
            query.IncludeValueNames.AddRange(
                new[] { "UninstallString", "Publisher", "DisplayVersion", "InstallLocation", "QuietUninstallString", "ParentKeyName" });

            foreach (RegistryValue item in 
                RegistryQuery.GenerateKeyValueExclusions("ParentKeyName", "OperatingSystem", "ie7Hotfix", "VISPRO", "PROPLUS"))
            {
                query.KeyValueExclusions.Add(item);
            }

            foreach (RegistryValue item in
                RegistryQuery.GenerateKeyValueExclusions(
                    "InstallLocation", 
                    Environment.ExpandEnvironmentVariables("%PROGRAMFILES%") + @"\Microsoft Office\", 
                    Environment.ExpandEnvironmentVariables("%PROGRAMFILES%") + @"\Microsoft Office"))
            {
                query.KeyValueExclusions.Add(item);
            }

            target.BeginExtraction(query);

            lock (completeLock)
            {
                completed = completed || Monitor.Wait(completeLock, 1000);
            }

            Assert.IsFalse(queryFailed, "Query returned an empty result set marked successful.");
            Assert.IsTrue(completed, "Test timed out.");
            Assert.IsTrue(registryData.Count > 0, "No results were returned.");

            foreach (var item in registryData)
            {
                Trace.TraceInformation("{0}:", item.Key);

                foreach (RegistryValue value in item.Value)
                {
                    Trace.TraceInformation(" - {0} ({1}) = {2}", value.Name, value.ValueKind, value.Data);
                }
            }
        }

        #endregion
    }
}