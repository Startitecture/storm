// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FlatPocoFactoryTests.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Mapper.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;

    using Microsoft.CSharp.RuntimeBinder;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Startitecture.Core;
    using Startitecture.Orm.Common;
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
            var fakeComplexRow = Generate.CreateFakeComplexRow();
            var entityDefinition = new DataAnnotationsDefinitionProvider().Resolve<ComplexFlatRow>();
            var databaseContext = new Mock<IDatabaseContext>();
            databaseContext.Setup(context => context.GetValueMapper(It.IsAny<Type>(), It.IsAny<Type>())).Returns((IValueMapper)null);

            PocoDelegateInfo actual;

            using (var reader = fakeComplexRow.MockDataReader(entityDefinition.ReturnableAttributes).Object)
            {
                reader.Read();
                var pocoDataRequest = new PocoDataRequest(reader, entityDefinition, databaseContext.Object);
                actual = FlatPocoFactory.CreateDelegate<ComplexFlatRow>(pocoDataRequest);
            }

            Assert.IsNotNull(actual);
        }

        /// <summary>
        /// The create delegate test.
        /// </summary>
        [TestMethod]
        public void CreateDelegate_FlatPocoFactoryForComplexRow_DelegateSetsPocoAsExpected()
        {
            var expected = Generate.CreateFakeComplexRow();
            var entityDefinition = new DataAnnotationsDefinitionProvider().Resolve<ComplexFlatRow>();
            var databaseContext = new Mock<IDatabaseContext>();
            databaseContext.Setup(context => context.GetValueMapper(It.IsAny<Type>(), It.IsAny<Type>())).Returns((IValueMapper)null);
            Stopwatch stopwatch;
            ComplexFlatRow actual;

            using (var reader = expected.MockDataReader(entityDefinition.ReturnableAttributes).Object)
            {
                reader.Read();
                var pocoDataRequest = new PocoDataRequest(reader, entityDefinition, databaseContext.Object);

                stopwatch = Stopwatch.StartNew();
                var pocoDelegate = FlatPocoFactory.CreateDelegate<ComplexFlatRow>(pocoDataRequest).MappingDelegate as Func<IDataReader, ComplexFlatRow>;
                Trace.TraceInformation($"{stopwatch.Elapsed} Create delegate");
                stopwatch.Reset();

                Assert.IsNotNull(pocoDelegate);

                stopwatch.Start();
                actual = pocoDelegate.Invoke(reader);
                Trace.TraceInformation($"{stopwatch.Elapsed} Invoke delegate #1");
                stopwatch.Reset();
            }

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.FakeComplexEntityId, actual.FakeComplexEntityId);
            Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));

            using (var reader = expected.MockDataReader(entityDefinition.ReturnableAttributes).Object)
            {
                reader.Read();
                var pocoDelegate =
                    FlatPocoFactory.CreateDelegate<ComplexFlatRow>(new PocoDataRequest(reader, entityDefinition, databaseContext.Object)).MappingDelegate as
                        Func<IDataReader, ComplexFlatRow>;

                Assert.IsNotNull(pocoDelegate);
                stopwatch.Start();
                pocoDelegate.Invoke(reader);
            }

            Trace.TraceInformation($"{stopwatch.Elapsed} Invoke delegate #2");
            stopwatch.Reset();

            using (var reader = expected.MockDataReader(entityDefinition.ReturnableAttributes).Object)
            {
                reader.Read();
                var pocoDelegate =
                    FlatPocoFactory.CreateDelegate<ComplexFlatRow>(new PocoDataRequest(reader, entityDefinition, databaseContext.Object)).MappingDelegate as
                        Func<IDataReader, ComplexFlatRow>;

                Assert.IsNotNull(pocoDelegate);
                stopwatch.Start();
                pocoDelegate.Invoke(reader);
            }

            Trace.TraceInformation($"{stopwatch.Elapsed} Invoke delegate #3");
            stopwatch.Reset();
        }

        /// <summary>
        /// The create delegate test.
        /// </summary>
        [TestMethod]
        public void CreateDelegate_FlatPocoFactoryForDynamic_DelegateSetsPocoAsExpected()
        {
            var expected = Generate.CreateFakeComplexRow();
            var definitionProvider = new DataAnnotationsDefinitionProvider();
            var entityDefinition = definitionProvider.Resolve<ComplexFlatRow>();
            var databaseContext = new Mock<IDatabaseContext>();
            databaseContext.Setup(context => context.GetValueMapper(It.IsAny<Type>(), It.IsAny<Type>())).Returns((IValueMapper)null);

            Expression<Func<ComplexFlatRow, object>> expression1 = row => row.FakeComplexEntityId;
            Expression<Func<ComplexFlatRow, object>> expression2 = row => row.FakeDependentEntityDependentIntegerValue;
            Expression<Func<ComplexFlatRow, object>> expression3 = row => row.FakeSubSubEntityUniqueName;

            var attributes = new[]
                                 {
                                     expression1,
                                     expression2,
                                     expression3
                                 }.Select(entityDefinition.Find)
                .ToList();

            dynamic actual;

            // Set up the data reader with all attributes to ensure that the process is only getting the ones we want.
            using (var reader = expected.MockDataReader(attributes).Object)
            {
                reader.Read();
                var pocoDataRequest = new PocoDataRequest(reader, attributes, databaseContext.Object);

                var pocoDelegate = FlatPocoFactory.CreateDelegate<dynamic>(pocoDataRequest).MappingDelegate as Func<IDataReader, dynamic>;
                Assert.IsNotNull(pocoDelegate);
                actual = pocoDelegate.Invoke(reader);
            }

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.FakeComplexEntityId, actual.FakeComplexEntityId);
            Assert.AreEqual(expected.FakeDependentEntityDependentIntegerValue, actual.FakeDependentEntityDependentIntegerValue);
            Assert.AreEqual(expected.FakeSubSubEntityUniqueName, actual.FakeSubSubEntityUniqueName);
            Assert.ThrowsException<RuntimeBinderException>(() => Assert.IsNull(actual.Description));
        }

        /// <summary>
        /// The create delegate test.
        /// </summary>
        [TestMethod]
        public void CreateDelegate_FlatPocoFactoryForPhysicalNameOverriddenRow_DelegateSetsPocoAsExpected()
        {
            var expected = new OverriddenColumnNameRow
            {
                OverriddenColumnNameId = 43,
                Name = "OcnName",
                Description = "OcnDescription",
                EntryTime = DateTimeOffset.Now,
                RelatedRowId = 3433,
                RelatedRowName = "RelatedName"
            };

            var entityDefinition = new DataAnnotationsDefinitionProvider().Resolve<OverriddenColumnNameRow>();
            var databaseContext = new Mock<IDatabaseContext>();
            databaseContext.Setup(context => context.GetValueMapper(It.IsAny<Type>(), It.IsAny<Type>())).Returns((IValueMapper)null);
            Stopwatch stopwatch;

            OverriddenColumnNameRow actual;
            using (var reader = expected.MockDataReader(entityDefinition.ReturnableAttributes).Object)
            {
                reader.Read();
                var pocoDataRequest = new PocoDataRequest(reader, entityDefinition, databaseContext.Object);

                stopwatch = Stopwatch.StartNew();
                var pocoDelegate =
                    FlatPocoFactory.CreateDelegate<OverriddenColumnNameRow>(pocoDataRequest).MappingDelegate as Func<IDataReader, OverriddenColumnNameRow>;

                Trace.TraceInformation($"{stopwatch.Elapsed} Create delegate");
                stopwatch.Reset();

                Assert.IsNotNull(pocoDelegate);

                stopwatch.Start();
                actual = pocoDelegate.Invoke(pocoDataRequest.DataReader);
            }

            Trace.TraceInformation($"{stopwatch.Elapsed} Invoke delegate #1");
            stopwatch.Reset();

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.OverriddenColumnNameId, actual.OverriddenColumnNameId);
            Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));

            using (var reader = expected.MockDataReader(entityDefinition.ReturnableAttributes).Object)
            {
                reader.Read();
                var pocoDelegate =
                    FlatPocoFactory.CreateDelegate<OverriddenColumnNameRow>(new PocoDataRequest(reader, entityDefinition, databaseContext.Object)).MappingDelegate as
                        Func<IDataReader, OverriddenColumnNameRow>;

                Assert.IsNotNull(pocoDelegate);
                stopwatch.Start();
                pocoDelegate.Invoke(reader);
            }

            Trace.TraceInformation($"{stopwatch.Elapsed} Invoke delegate #2");
            stopwatch.Reset();

            using (var reader = expected.MockDataReader(entityDefinition.ReturnableAttributes).Object)
            {
                reader.Read();
                var pocoDelegate =
                    FlatPocoFactory.CreateDelegate<OverriddenColumnNameRow>(new PocoDataRequest(reader, entityDefinition, databaseContext.Object)).MappingDelegate as
                        Func<IDataReader, OverriddenColumnNameRow>;

                Assert.IsNotNull(pocoDelegate);
                stopwatch.Start();
                pocoDelegate.Invoke(reader);
            }

            Trace.TraceInformation($"{stopwatch.Elapsed} Invoke delegate #3");
            stopwatch.Reset();
        }

        /// <summary>
        /// The create delegate_ attribute for dynamic object_ matches expected.
        /// </summary>
        [TestMethod]
        public void CreateDelegate_AttributeForDynamicObject_MatchesExpected()
        {
            var entityDefinition = new DataAnnotationsDefinitionProvider().Resolve<OverriddenColumnNameRow>();
            var databaseContext = new Mock<IDatabaseContext>();
            databaseContext.Setup(context => context.GetValueMapper(It.IsAny<Type>(), It.IsAny<Type>())).Returns((IValueMapper)null);
            var expected = new OverriddenColumnNameRow
            {
                OverriddenColumnNameId = 12,
                Description = "my desc",
                EntryTime = DateTimeOffset.Now,
                Name = "name",
                RelatedRowId = 234,
                RelatedRowName = "relatedName"
            };

            using (var dataReader = expected.MockDataReader(entityDefinition.DirectAttributes).Object)
            {
                dataReader.Read();
                var dataRequest = new PocoDataRequest(dataReader, entityDefinition, databaseContext.Object);
                var func = FlatPocoFactory.CreateDelegate<dynamic>(dataRequest).MappingDelegate as Func<IDataReader, dynamic>;
                Assert.IsNotNull(func);

                var actual = func(dataReader);

                foreach (var definition in entityDefinition.DirectAttributes)
                {
                    Assert.AreEqual(definition.GetValueDelegate.DynamicInvoke(expected), ((IDictionary<string, object>)actual)[definition.ReferenceName]);
                }
            }
        }
    }
}