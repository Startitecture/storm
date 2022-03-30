﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

using Startitecture.Orm.Model;
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueFilterSetTests.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model.Tests
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Core;
    using Startitecture.Orm.Schema;
    using Startitecture.Orm.Testing.Entities;

    /// <summary>
    /// The value filter set tests.
    /// </summary>
    [TestClass]
    public class ValueFilterSetTests
    {
        /// <summary>
        /// The exists in test.
        /// </summary>
        [TestMethod]
        public void ExistsIn_EntityOutsideAggregateRelations_MatchesExpected()
        {
            var target = new ValueFilterSet<DomainAggregateRow>().ExistsIn(
                row => row.DomainAggregateId,
                Query.From<DomainAggregateFlagAttributeRow>(
                        set => set.InnerJoin<FlagAttributeRow>(row => row.FlagAttributeId, row => row.FlagAttributeId))
                    .Where(set => set.Include((FlagAttributeRow row) => row.Name, "type1", "type2", "type3")),
                row => row.DomainAggregateId);

            Assert.IsTrue(target.ValueFilters.Any());
            Assert.AreEqual(1, target.ValueFilters.Count());
            Assert.AreEqual(3, (target.ValueFilters.First() as RelationExpression)?.FilterValues.Count());
            CollectionAssert.AreEqual(
                new[]
                    {
                        "type1",
                        "type2",
                        "type3"
                    },
                (target.ValueFilters.First() as RelationExpression)?.EntitySet.Filters.First().FilterValues.OfType<string>().ToList());

            var expectedRelation = new EntityRelation(EntityRelationType.InnerJoin);
            expectedRelation.Join<DomainAggregateRow, DomainAggregateFlagAttributeRow>(row => row.DomainAggregateId, row => row.DomainAggregateId);
            Assert.AreEqual(expectedRelation, (target.ValueFilters.First() as RelationExpression)?.Relations.First());
        }

        /// <summary>
        /// The between test.
        /// </summary>
        [TestMethod]
        public void Between_ItemSelectionWithOrderedValues_ValuesMatchExpected()
        {
            var target = new ValueFilterSet<SelectionTestRow>().Between(row => row.SomeDate, DateTime.Today, DateTime.Today.AddDays(1));
            Assert.AreEqual(DateTime.Today, target.ValueFilters.First().FilterValues.First());
        }

        /// <summary>
        /// The between test.
        /// </summary>
        [TestMethod]
        public void Between_ItemSelectionWithUnorderedValues_ValuesMatchExpected()
        {
            var target = new ValueFilterSet<SelectionTestRow>().Between(row => row.SomeDate, DateTime.Today.AddDays(1), DateTime.Today);
            Assert.AreEqual(DateTime.Today, target.ValueFilters.First().FilterValues.First());
        }

        /// <summary>
        /// The between test.
        /// </summary>
        [TestMethod]
        public void Between_ItemSelectionWithUnorderedValues_FilterMatchesExpected()
        {
            Expression<Func<SelectionTestRow, DateTime>> selector = row => row.SomeDate;
            var maxValue = DateTime.Today;
            var minValue = maxValue.AddDays(-1);
            var expected = new ValueFilter(new AttributeLocation(selector), FilterType.Between, minValue, maxValue);
            var target = new ValueFilterSet<SelectionTestRow>().Between(row => row.SomeDate, minValue, maxValue);
            var actual = target.ValueFilters.First();

            Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
        }

        /// <summary>
        /// The greater than test.
        /// </summary>
        [TestMethod]
        public void GreaterThan_DataRowAttribute_MatchesExpected()
        {
            Expression<Func<DataRow, int>> valueExpression = row => row.FakeDataId;
            var target = new ValueFilterSet<DataRow>().GreaterThan(row => row.FakeDataId, 35);
            var expected = new ValueFilter(valueExpression, FilterType.GreaterThan, 35);
            var actual = target.ValueFilters.First();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// The greater than test.
        /// </summary>
        [TestMethod]
        public void GreaterThanOrEqualTo_DataRowAttribute_MatchesExpected()
        {
            Expression<Func<DataRow, int>> valueExpression = row => row.FakeDataId;
            var target = new ValueFilterSet<DataRow>().GreaterThanOrEqualTo(row => row.FakeDataId, 35);
            var expected = new ValueFilter(valueExpression, FilterType.GreaterThanOrEqualTo, 35);
            var actual = target.ValueFilters.First();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// The greater than test.
        /// </summary>
        [TestMethod]
        public void LessThan_DataRowAttribute_MatchesExpected()
        {
            Expression<Func<DataRow, int>> valueExpression = row => row.FakeDataId;
            var target = new ValueFilterSet<DataRow>().LessThan(row => row.FakeDataId, 35);
            var expected = new ValueFilter(valueExpression, FilterType.LessThan, 35);
            var actual = target.ValueFilters.First();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// The greater than test.
        /// </summary>
        [TestMethod]
        public void LessThanOrEqualTo_DataRowAttribute_MatchesExpected()
        {
            Expression<Func<DataRow, int>> valueExpression = row => row.FakeDataId;
            var target = new ValueFilterSet<DataRow>().LessThanOrEqualTo(row => row.FakeDataId, 35);
            var expected = new ValueFilter(valueExpression, FilterType.LessThanOrEqualTo, 35);
            var actual = target.ValueFilters.First();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests the MatchKey method.
        /// </summary>
        [TestMethod]
        public void MatchKey_ImplicitKey_MatchesPrimaryKey()
        {
            var dataRow = new DataRow
            {
                FakeDataId = 43,
                AnotherColumn = "test",
                AnotherValueColumn = 33,
                NormalColumn = "foo",
                NullableColumn = null,
                NullableValueColumn = null,
                ValueColumn = 44
            };

            var target = new ValueFilterSet<DataRow>().MatchKey(
                dataRow,
                new DataAnnotationsDefinitionProvider());

            Expression<Func<DataRow, int>> valueExpression = row => row.FakeDataId;
            var expected = new ValueFilter(valueExpression, FilterType.Equality, dataRow.FakeDataId);
            var actual = target.ValueFilters.First();

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests the MatchKey method.
        /// </summary>
        [TestMethod]
        public void MatchKey_ExplicitKey_MatchesExplicitKey()
        {
            var dataRow = new DataRow
            {
                FakeDataId = 43,
                AnotherColumn = "test",
                AnotherValueColumn = 33,
                NormalColumn = "foo",
                NullableColumn = null,
                NullableValueColumn = null,
                ValueColumn = 44
            };

            var target = new ValueFilterSet<DataRow>().MatchKey(
                dataRow,
                new DataAnnotationsDefinitionProvider(),
                row => row.AnotherColumn,
                row => row.AnotherValueColumn);

            Expression<Func<DataRow, string>> keyColumn1 = row => row.AnotherColumn;
            Expression<Func<DataRow, int>> keyColumn2 = row => row.AnotherValueColumn;
            var expected = new[]
                           {
                               new ValueFilter(keyColumn1, FilterType.Equality, dataRow.AnotherColumn),
                               new ValueFilter(keyColumn2, FilterType.Equality, dataRow.AnotherValueColumn)
                           };

            var actual = target.ValueFilters.ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void AddTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void MatchingTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AreEqualTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AreEqualTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GreaterThanTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GreaterThanOrEqualToTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void LessThanTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void LessThanOrEqualToTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void IncludeTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void BetweenTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void BetweenTest1()
        {
            Assert.Fail();
        }
    }
}