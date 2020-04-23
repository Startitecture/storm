// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FlatPocoFactoryTests.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Mapper.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Core;
    using Startitecture.Orm.Schema;
    using Startitecture.Orm.Testing.Entities;
    using Startitecture.Orm.Testing.Moq;

    /// <summary>
    /// The flat POCO factory tests.
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
            var pocoDataRequest = Generate.CreatePocoDataRequest(fakeComplexRow, new DataAnnotationsDefinitionProvider());
            var actual = target.CreateDelegate<ComplexFlatRow>(pocoDataRequest);
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
            var pocoDataRequest = Generate.CreatePocoDataRequest(expected, new DataAnnotationsDefinitionProvider());

            var stopwatch = Stopwatch.StartNew();
            var pocoDelegate = target.CreateDelegate<ComplexFlatRow>(pocoDataRequest).MappingDelegate as Func<IDataReader, ComplexFlatRow>;
            Trace.TraceInformation($"{stopwatch.Elapsed} Create delegate");
            stopwatch.Reset();

            Assert.IsNotNull(pocoDelegate);

            stopwatch.Start();
            var actual = pocoDelegate.Invoke(pocoDataRequest.DataReader);
            Trace.TraceInformation($"{stopwatch.Elapsed} Invoke delegate #1");
            stopwatch.Reset();

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.FakeComplexEntityId, actual.FakeComplexEntityId);
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

            var pocoDataRequest = Generate.CreatePocoDataRequest(expected, new DataAnnotationsDefinitionProvider());

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
            Assert.AreEqual(expected.OverriddenColumnNameId, actual.OverriddenColumnNameId);
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
        /// The create delegate_ attribute for dynamic object_ matches expected.
        /// </summary>
        [TestMethod]
        public void CreateDelegate_AttributeForDynamicObject_MatchesExpected()
        {
            var target = new FlatPocoFactory();
            var entityDefinition = new DataAnnotationsDefinitionProvider().Resolve<OverriddenColumnNameRow>();
            var expected = new OverriddenColumnNameRow
                               {
                                   OverriddenColumnNameId = 12,
                                   Description = "my desc",
                                   EntryTime = DateTimeOffset.Now,
                                   Name = "name",
                                   RelatedRowId = 234,
                                   RelatedRowName = "relatedName"
                               };

            var attributeDefinitions = entityDefinition.DirectAttributes.ToDictionary(definition => definition.ReferenceName, definition => definition);

            using (var dataReader = expected.MockDataReader(attributeDefinitions, entityDefinition).Object)
            {
                var dataRequest = new PocoDataRequest(dataReader, entityDefinition);
                var func = target.CreateDelegate<dynamic>(dataRequest).MappingDelegate as Func<IDataReader, dynamic>;
                Assert.IsNotNull(func);

                var actual = func(dataReader);

                foreach (var definition in attributeDefinitions)
                {
                    Assert.AreEqual(definition.Value.GetValueDelegate.DynamicInvoke(expected), ((IDictionary<string, object>)actual)[definition.Key]);
                }
            }
        }
    }
}