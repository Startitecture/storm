// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueFilterSetTests.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model.Tests
{
    using System;
    using System.Collections.Generic;
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
        /// Tests the Add method.
        /// </summary>
        [TestMethod]
        public void Add_FilterWithExpression_FilterValueMatchesExpression()
        {
            Expression<Func<DomainAggregateRow, object>> expression = row => row.Description;
            var target = new ValueFilterSet<DomainAggregateRow>().Add(new ValueFilter(expression, FilterType.Equality, "test"));
            var actual = target.ValueFilters.First();
            Assert.AreEqual(FilterType.Equality, actual.FilterType);
            Assert.AreEqual(new AttributeLocation(expression), actual.AttributeLocation);
            Assert.AreEqual("test", actual.FilterValues.First()); // TODO: Rename FilterValues/ValueFilter to AttributeFilters or something
        }

        /// <summary>
        /// Tests the Add method.
        /// </summary>
        [TestMethod]
        public void Add_FilterWithAttributeLocation_FilterValueMatchesExpression()
        {
            Expression<Func<DomainAggregateRow, object>> expression = row => row.Description;
            var target = new ValueFilterSet<DomainAggregateRow>().Add(
                new ValueFilter(new AttributeLocation(expression), FilterType.Equality, "test"));

            var actual = target.ValueFilters.First();
            Assert.AreEqual(FilterType.Equality, actual.FilterType);
            Assert.AreEqual("test", actual.FilterValues.First());
        }

        /// <summary>
        /// Tests the Matching method.
        /// </summary>
        [TestMethod]
        public void Matching_MultipleAttributes_ValueFiltersMatchExpected()
        {
            var target = new ValueFilterSet<DomainAggregateRow>();

            var domainAggregateRow = new DomainAggregateRow
                                     {
                                         DomainAggregateId = 243,
                                         Name = "MyAggregate",
                                         Description = "Aggregate, First of its Name",
                                         CreatedByDomainIdentityId = 23,
                                         CreatedTime = DateTimeOffset.Now,
                                         CategoryAttributeId = 422,
                                         LastModifiedByDomainIdentityId = 23,
                                         LastModifiedTime = DateTimeOffset.Now,
                                         SubContainerId = 1,
                                         TemplateId = 4
                                     };

            target.Matching(domainAggregateRow, row => row.Description, row => row.CreatedTime, row => row.CreatedByDomainIdentityId);

            Assert.AreEqual(nameof(DomainAggregateRow.Description), target.ValueFilters.ElementAt(0).AttributeLocation.PropertyInfo.Name);
            Assert.AreEqual(FilterType.Equality, target.ValueFilters.ElementAt(0).FilterType);
            Assert.AreEqual(domainAggregateRow.Description, target.ValueFilters.ElementAt(0).FilterValues.First());

            Assert.AreEqual(nameof(DomainAggregateRow.CreatedTime), target.ValueFilters.ElementAt(1).AttributeLocation.PropertyInfo.Name);
            Assert.AreEqual(FilterType.Equality, target.ValueFilters.ElementAt(1).FilterType);
            Assert.AreEqual(domainAggregateRow.CreatedTime, target.ValueFilters.ElementAt(1).FilterValues.First());

            Assert.AreEqual(
                nameof(DomainAggregateRow.CreatedByDomainIdentityId),
                target.ValueFilters.ElementAt(2).AttributeLocation.PropertyInfo.Name);

            Assert.AreEqual(FilterType.Equality, target.ValueFilters.ElementAt(2).FilterType);
            Assert.AreEqual(domainAggregateRow.CreatedByDomainIdentityId, target.ValueFilters.ElementAt(2).FilterValues.First());
        }

        /// <summary>
        /// Tests the AreEqual method.
        /// </summary>
        [TestMethod]
        public void AreEqual_DirectAttribute_ValueFilterMatchesExpected()
        {
            var target = new ValueFilterSet<DomainAggregateRow>();
            target.AreEqual(row => row.SubContainerId, 7);

            Assert.AreEqual(nameof(DomainAggregateRow.SubContainerId), target.ValueFilters.ElementAt(0).AttributeLocation.PropertyInfo.Name);
            Assert.AreEqual(FilterType.Equality, target.ValueFilters.ElementAt(0).FilterType);
            Assert.AreEqual(7, target.ValueFilters.ElementAt(0).FilterValues.First());
        }

        /// <summary>
        /// Tests the AreEqual method.
        /// </summary>
        [TestMethod]
        public void AreEqual_ImplicitIndirectAttribute_ValueFilterMatchesExpected()
        {
            var target = new ValueFilterSet<DomainAggregateRow>();
            target.AreEqual(row => row.SubContainer.TopContainerId, 7);

            Assert.AreEqual(nameof(SubContainerRow.TopContainerId), target.ValueFilters.ElementAt(0).AttributeLocation.PropertyInfo.Name);
            Assert.AreEqual(FilterType.Equality, target.ValueFilters.ElementAt(0).FilterType);
            Assert.AreEqual(7, target.ValueFilters.ElementAt(0).FilterValues.First());
        }

        /// <summary>
        /// Tests the AreEqual method.
        /// </summary>
        [TestMethod]
        public void AreEqual_ExplicitIndirectAttribute_ValueFilterMatchesExpected()
        {
            var target = new ValueFilterSet<DomainAggregateRow>();
            target.AreEqual<SubContainerRow, int>(row => row.TopContainerId, 7);

            Assert.AreEqual(nameof(SubContainerRow.TopContainerId), target.ValueFilters.ElementAt(0).AttributeLocation.PropertyInfo.Name);
            Assert.AreEqual(FilterType.Equality, target.ValueFilters.ElementAt(0).FilterType);
            Assert.AreEqual(7, target.ValueFilters.ElementAt(0).FilterValues.First());
        }

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
        /// Tests the Include method.
        /// </summary>
        [TestMethod]
        public void Include_DirectAttributesWithMultipleValues_ValueFilterMatchesExpected()
        {
            var target = new ValueFilterSet<DomainAggregateRow>().Include(row => row.SubContainerId, 4, 6, 29, 33);
            Expression<Func<DomainAggregateRow, object>> expression = row => row.SubContainerId;
            var expected = new ValueFilter(expression, FilterType.MatchesSet, 4, 6, 29, 33);
            var actual = target.ValueFilters.First();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests the Include method.
        /// </summary>
        [TestMethod]
        public void Include_ExplicitIndirectAttributesWithMultipleValues_ValueFilterMatchesExpected()
        {
            var target = new ValueFilterSet<DataRow>().Include<DomainAggregateRow, int>(row => row.SubContainerId, 4, 6, 29, 33);
            Expression<Func<DomainAggregateRow, object>> expression = row => row.SubContainerId;
            var expected = new ValueFilter(expression, FilterType.MatchesSet, 4, 6, 29, 33);
            var actual = target.ValueFilters.First();
            Assert.AreEqual(expected, actual);
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
        /// Tests the Between method.
        /// </summary>
        [TestMethod]
        public void Between_ExplicitIndirectAttribute_ValueFilterMatchesExpected()
        {
            Expression<Func<SelectionTestRow, DateTime>> selector = row => row.SomeDate;
            var maxValue = DateTime.Today;
            var minValue = maxValue.AddDays(-1);
            var expected = new ValueFilter(new AttributeLocation(selector), FilterType.Between, minValue, maxValue);
            var target = new ValueFilterSet<DataRow>().Between<SelectionTestRow, DateTime>(row => row.SomeDate, minValue, maxValue);
            var actual = target.ValueFilters.First();

            Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
        }

        /// <summary>
        /// Tests the Between method.
        /// </summary>
        [TestMethod]
        public void Between_MultipleAttributesInBaselineAndBoundaryEntities_ValueFiltersMatchExpected()
        {
            var baseline = new SelectionTestRow
                           {
                               SelectionTestId = 23,
                               SomeDate = DateTime.Today.AddDays(-1),
                               SomeDecimal = 20.332M
                           };

            var boundary = new SelectionTestRow
                           {
                               SelectionTestId = 553,
                               SomeDate = DateTime.Today,
                               SomeDecimal = 40.68M
                           };

            var target = new ValueFilterSet<SelectionTestRow>().Between(
                baseline,
                boundary,
                row => row.SomeDate,
                row => row.SomeDecimal);

            Expression<Func<SelectionTestRow, DateTime>> dateSelector = row => row.SomeDate;
            Expression<Func<SelectionTestRow, decimal>> decimalSelector = row => row.SomeDecimal;

            var expected = new List<ValueFilter>
                           {
                               new ValueFilter(dateSelector, FilterType.Between, baseline.SomeDate, boundary.SomeDate),
                               new ValueFilter(decimalSelector, FilterType.Between, baseline.SomeDecimal, boundary.SomeDecimal)
                           };

            CollectionAssert.AreEqual(expected, target.ValueFilters.ToList());
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
        /// Tests the GreaterThan method.
        /// </summary>
        [TestMethod]
        public void GreaterThan_ExplicitIndirectAttribute_ValueFilterMatchesExpected()
        {
            Expression<Func<DependentRow, int>> valueExpression = row => row.DependentIntegerValue;
            var target = new ValueFilterSet<DataRow>().GreaterThan<DependentRow, int>(row => row.DependentIntegerValue, 35);
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
        /// Tests the GreaterThanOrEqualTo method.
        /// </summary>
        [TestMethod]
        public void GreaterThanOrEqualTo_ExplicitIndirectAttribute_ValueFilterMatchesExpected()
        {
            Expression<Func<DependentRow, int>> valueExpression = row => row.DependentIntegerValue;
            var target = new ValueFilterSet<DataRow>().GreaterThanOrEqualTo<DependentRow, int>(row => row.DependentIntegerValue, 35);
            var expected = new ValueFilter(valueExpression, FilterType.GreaterThanOrEqualTo, 35);
            var actual = target.ValueFilters.First();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests the LessThan method.
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
        /// Tests the LessThan method.
        /// </summary>
        [TestMethod]
        public void LessThan_ExplicitIndirectAttribute_ValueFilterMatchesExpected()
        {
            Expression<Func<DependentRow, int>> valueExpression = row => row.DependentIntegerValue;
            var target = new ValueFilterSet<DataRow>().LessThan<DependentRow, int>(row => row.DependentIntegerValue, 35);
            var expected = new ValueFilter(valueExpression, FilterType.LessThan, 35);
            var actual = target.ValueFilters.First();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests the LessThanOrEqualTo method.
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
        /// Tests the LessThanOrEqualTo method.
        /// </summary>
        [TestMethod]
        public void LessThanOrEqualTo_ExplicitIndirectAttribute_ValueFilterMatchesExpected()
        {
            Expression<Func<DependentRow, int>> valueExpression = row => row.DependentIntegerValue;
            var target = new ValueFilterSet<DataRow>().LessThanOrEqualTo<DependentRow, int>(row => row.DependentIntegerValue, 35);
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
    }
}