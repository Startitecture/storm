// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FlatPocoFactoryTests.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Mapper.Tests
{
    using System;
    using System.Data;
    using System.Diagnostics;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Core;
    using Startitecture.Orm.Schema;
    using Startitecture.Orm.Testing.Model;

    /// <summary>
    /// The flat poco factory tests.
    /// </summary>
    [TestClass]
    public class FlatPocoFactoryTests
    {
        /// <summary>
        /// The create delegate test.
        /// </summary>
        [TestMethod]
        public void CreateDelegate_FlatPocoFactoryForComplexRow_IsNotNull()
        {
            var target = new FlatPocoFactory();
            var fakeComplexRow = Generate.CreateFakeComplexRow();
            var pocoDataRequest = Generate.CreatePocoDataRequest(fakeComplexRow, new PetaPocoDefinitionProvider());
            var actual = target.CreateDelegate<FakeComplexRow>(pocoDataRequest);
            Assert.IsNotNull(actual);
        }

        /// <summary>
        /// The create delegate test.
        /// </summary>
        [TestMethod]
        public void CreateDelegate_FlatPocoFactoryForComplexRow_DelegateSetsPocoAsExpected()
        {
            var target = new FlatPocoFactory();
            var expected = Generate.CreateFakeComplexRow();
            var pocoDataRequest = Generate.CreatePocoDataRequest(expected, new PetaPocoDefinitionProvider());

            var stopwatch = Stopwatch.StartNew();
            var pocoDelegate = target.CreateDelegate<FakeComplexRow>(pocoDataRequest).MappingDelegate as Func<IDataReader, FakeComplexRow>;
            Trace.TraceInformation($"{stopwatch.Elapsed} Create delegate");
            stopwatch.Reset();

            Assert.IsNotNull(pocoDelegate);

            stopwatch.Start();
            var actual = pocoDelegate.Invoke(pocoDataRequest.DataReader);
            Trace.TraceInformation($"{stopwatch.Elapsed} Invoke delegate #1");
            stopwatch.Reset();

            Assert.IsNotNull(actual);
            Assert.AreEqual<int>(expected.FakeComplexEntityId, actual.FakeComplexEntityId);
            Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));

            stopwatch.Start();
            pocoDelegate.Invoke(pocoDataRequest.DataReader);
            Trace.TraceInformation($"{stopwatch.Elapsed} Invoke delegate #2");
            stopwatch.Reset();

            stopwatch.Start();
            pocoDelegate.Invoke(pocoDataRequest.DataReader);
            Trace.TraceInformation($"{stopwatch.Elapsed} Invoke delegate #3");
            stopwatch.Reset();
        }


        /// <summary>
        /// The create delegate test.
        /// </summary>
        [TestMethod]
        public void CreateDelegate_FlatPocoFactoryForPhysicalNameOverriddenRow_DelegateSetsPocoAsExpected()
        {
            var target = new FlatPocoFactory();
            var expected = new OverriddenColumnNameRow
                               {
                                   OverriddenColumnNameId = 43,
                                   Name = "OcnName",
                                   Description = "OcnDescription",
                                   EntryTime = DateTimeOffset.Now,
                                   RelatedRowId = 3433,
                                   RelatedRowName = "RelatedName"
                               };

            var pocoDataRequest = Generate.CreatePocoDataRequest(expected, new PetaPocoDefinitionProvider());

            var stopwatch = Stopwatch.StartNew();
            var pocoDelegate = target.CreateDelegate<OverriddenColumnNameRow>(pocoDataRequest).MappingDelegate as Func<IDataReader, OverriddenColumnNameRow>;
            Trace.TraceInformation($"{stopwatch.Elapsed} Create delegate");
            stopwatch.Reset();

            Assert.IsNotNull(pocoDelegate);

            stopwatch.Start();
            var actual = pocoDelegate.Invoke(pocoDataRequest.DataReader);
            Trace.TraceInformation($"{stopwatch.Elapsed} Invoke delegate #1");
            stopwatch.Reset();

            Assert.IsNotNull(actual);
            Assert.AreEqual<int>(expected.OverriddenColumnNameId, actual.OverriddenColumnNameId);
            Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));

            stopwatch.Start();
            pocoDelegate.Invoke(pocoDataRequest.DataReader);
            Trace.TraceInformation($"{stopwatch.Elapsed} Invoke delegate #2");
            stopwatch.Reset();

            stopwatch.Start();
            pocoDelegate.Invoke(pocoDataRequest.DataReader);
            Trace.TraceInformation($"{stopwatch.Elapsed} Invoke delegate #3");
            stopwatch.Reset();
        }
    }
}