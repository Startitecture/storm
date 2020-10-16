// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateSetTests.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The update set tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model.Tests
{
    using System;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Orm.Schema;
    using Startitecture.Orm.Testing.Entities;

    /// <summary>
    /// The update set tests.
    /// </summary>
    [TestClass]
    public class UpdateSetTests
    {
        /// <summary>
        /// The update statement_ direct data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Set_DirectData_MatchesExpected()
        {
            var match = new DataRow
            {
                NormalColumn = "NormalColumn",
                NullableColumn = "CouldHaveBeenNull",
                ValueColumn = 2,
                NullableValueColumn = null,
                AnotherValueColumn = 12,
                AnotherColumn = "Some Other Value"
            };

            var baseline = new DataRow
            {
                FakeDataId = 10
            };
            var boundary = new DataRow
            {
                FakeDataId = 20
            };

            var updateSet = new UpdateSet<DataRow>().Set(match, new DataAnnotationsDefinitionProvider())
                .From(
                    set => set.InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                        .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                        .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                        .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId))
                .Where(
                    set => set.AreEqual(row => row.ValueColumn, match.ValueColumn)
                        .AreEqual(row => row.NullableColumn, match.NullableColumn)
                        .AreEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                        .Between(baseline, boundary, row => row.FakeDataId));

            var expected = new object[]
                               {
                                   "NormalColumn",
                                   "CouldHaveBeenNull",
                                   2,
                                   12,
                                   "Some Other Value",
                                   2,
                                   "CouldHaveBeenNull",
                                   10,
                                   20
                               };

            var actual = updateSet.PropertyValues.ToArray();

            CollectionAssert.AreEqual(
                expected,
                actual,
                "Expected: {0}{1}Actual: {2}",
                string.Join(",", expected),
                Environment.NewLine,
                string.Join(",", actual));
        }

        /// <summary>
        /// The update statement_ direct data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Set_RaisedDirectData_MatchesExpected()
        {
            var match = new DataRow
            {
                NormalColumn = "NormalColumn",
                NullableColumn = "CouldHaveBeenNull",
                ValueColumn = 2,
                NullableValueColumn = null,
                AnotherValueColumn = 12,
                AnotherColumn = "Some Other Value"
            };

            var baseline = new DataRow
            {
                FakeDataId = 10
            };
            var boundary = new DataRow
            {
                FakeDataId = 20
            };

            var updateSet = new UpdateSet<DataRow>().Set(match, new DataAnnotationsDefinitionProvider())
                .From(
                    set => set.InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                        .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                        .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                        .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId))
                .Where(
                    set => set.AreEqual(row => row.ValueColumn, match.ValueColumn)
                        .AreEqual(row => row.NullableColumn, match.NullableColumn)
                        .AreEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                        .Between(baseline, boundary, row => row.FakeDataId));

            var expected = new object[]
                               {
                                   "NormalColumn",
                                   "CouldHaveBeenNull",
                                   2,
                                   12,
                                   "Some Other Value",
                                   2,
                                   "CouldHaveBeenNull",
                                   10,
                                   20
                               };

            var actual = updateSet.PropertyValues.ToArray();

            CollectionAssert.AreEqual(
                expected,
                actual,
                "Expected: {0}{1}Actual: {2}",
                string.Join(",", expected),
                Environment.NewLine,
                string.Join(",", actual));
        }

        /// <summary>
        /// The update statement_ related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Set_RelatedData_MatchesExpected()
        {
            var match = new DataRow
            {
                NormalColumn = "NormalColumn",
                RelatedAlias = new FakeRelatedRow
                {
                    RelatedProperty = "Related"
                },
                NullableColumn = "CouldHaveBeenNull",
                ValueColumn = 2,
                NullableValueColumn = null,
                AnotherValueColumn = 12,
                AnotherColumn = "Some Other Value"
            };

            var baseline = new DataRow
            {
                FakeDataId = 10
            };
            var boundary = new DataRow
            {
                FakeDataId = 20
            };

            var updateOperation = new UpdateSet<DataRow>().Set(match, new DataAnnotationsDefinitionProvider())
                .From(
                    set => set.InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                        .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                        .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                        .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId))
                .Where(
                    set => set.AreEqual(row => row.ValueColumn, match.ValueColumn)
                        .AreEqual(row => row.NullableColumn, match.NullableColumn)
                        .AreEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                        .AreEqual(row => row.RelatedAlias.RelatedProperty, match.RelatedAlias.RelatedProperty)
                        .Between(baseline, boundary, row => row.FakeDataId));

            var expected = new object[]
                               {
                                   "NormalColumn",
                                   "CouldHaveBeenNull",
                                   2,
                                   12,
                                   "Some Other Value",
                                   2,
                                   "CouldHaveBeenNull",
                                   "Related",
                                   10,
                                   20
                               };
            var actual = updateOperation.PropertyValues.ToArray();

            CollectionAssert.AreEqual(
                expected,
                actual,
                "Expected: {0}{1}Actual: {2}",
                string.Join(",", expected),
                Environment.NewLine,
                string.Join(",", actual));
        }

        /// <summary>
        /// The update statement_ related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Set_RaisedRelatedData_MatchesExpected()
        {
            var match = new DataRow
            {
                NormalColumn = "NormalColumn",
                RelatedAlias = new FakeRelatedRow
                {
                    RelatedProperty = "Related"
                },
                NullableColumn = "CouldHaveBeenNull",
                ValueColumn = 2,
                NullableValueColumn = null,
                AnotherValueColumn = 12,
                AnotherColumn = "Some Other Value"
            };

            var baseline = new DataRow
            {
                FakeDataId = 10
            };
            var boundary = new DataRow
            {
                FakeDataId = 20
            };

            var updateOperation = new UpdateSet<DataRow>().Set(match, new DataAnnotationsDefinitionProvider())
                .From(
                    set => set.InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                        .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                        .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                        .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId))
                .Where(
                    set => set.AreEqual(row => row.ValueColumn, match.ValueColumn)
                        .AreEqual(row => row.NullableColumn, match.NullableColumn)
                        .AreEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                        .AreEqual(row => row.RelatedAlias.RelatedProperty, "Related")
                        .Between(baseline, boundary, row => row.FakeDataId));

            var expected = new object[]
                               {
                                   "NormalColumn",
                                   "CouldHaveBeenNull",
                                   2,
                                   12,
                                   "Some Other Value",
                                   2,
                                   "CouldHaveBeenNull",
                                   "Related",
                                   10,
                                   20
                               };
            var actual = updateOperation.PropertyValues.ToArray();

            CollectionAssert.AreEqual(
                expected,
                actual,
                "Expected: {0}{1}Actual: {2}",
                string.Join(",", expected),
                Environment.NewLine,
                string.Join(",", actual));
        }

        /// <summary>
        /// The update statement_ direct data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Set_DirectDataSpecificSetValuesFromItem_MatchesExpected()
        {
            var match = new DataRow
            {
                NormalColumn = "NormalColumn",
                NullableColumn = "CouldHaveBeenNull",
                ValueColumn = 2,
                NullableValueColumn = null,
            };

            var baseline = new DataRow
            {
                FakeDataId = 10
            };
            var boundary = new DataRow
            {
                FakeDataId = 20
            };

            var target = new DataRow
            {
                NormalColumn = "UpdatedNormalColumn",
                NullableColumn = null
            };
            var updateSet = new UpdateSet<DataRow>().Set(target, row => row.NormalColumn, row => row.NullableColumn)
                .From(
                    set => set.InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                        .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                        .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                        .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId))
                .Where(
                    set => set.AreEqual(row => row.ValueColumn, match.ValueColumn)
                        .AreEqual(row => row.NullableColumn, match.NullableColumn)
                        .AreEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                        .Between(baseline, boundary, row => row.FakeDataId));

            var expected = new object[]
                               {
                                   "UpdatedNormalColumn",
                                   2,
                                   "CouldHaveBeenNull",
                                   10,
                                   20
                               };
            var actual = updateSet.PropertyValues.ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// The update statement_ direct data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Set_RaisedDirectDataSpecificSetValuesFromItem_MatchesExpected()
        {
            var match = new DataRow
            {
                NormalColumn = "NormalColumn",
                NullableColumn = "CouldHaveBeenNull",
                ValueColumn = 2,
                NullableValueColumn = null,
            };

            var baseline = new DataRow
            {
                FakeDataId = 10
            };

            var boundary = new DataRow
            {
                FakeDataId = 20
            };

            var target = new DataRow
            {
                NormalColumn = "UpdatedNormalColumn",
                NullableColumn = null
            };

            var updateSet = new UpdateSet<DataRow>().Set(target, row => row.NormalColumn, row => row.NullableColumn)
                .From(
                    set => set.InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                        .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                        .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                        .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId))
                .Where(
                    set => set.AreEqual(row => row.ValueColumn, match.ValueColumn)
                        .AreEqual(row => row.NullableColumn, match.NullableColumn)
                        .AreEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                        .Between(baseline, boundary, row => row.FakeDataId));

            var expected = new object[]
                               {
                                   "UpdatedNormalColumn",
                                   2,
                                   "CouldHaveBeenNull",
                                   10,
                                   20
                               };

            var actual = updateSet.PropertyValues.ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// The update statement_ related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Set_RelatedDataSpecificSetValuesFromItem_MatchesExpected()
        {
            var match = new DataRow
            {
                NormalColumn = "NormalColumn",
                RelatedAlias = new FakeRelatedRow
                {
                    RelatedProperty = "Related"
                },
                NullableColumn = "CouldHaveBeenNull",
                ValueColumn = 2,
                NullableValueColumn = null,
            };

            var baseline = new DataRow
            {
                FakeDataId = 10
            };

            var boundary = new DataRow
            {
                FakeDataId = 20
            };

            var target = new DataRow
            {
                NormalColumn = "UpdatedNormalColumn",
                NullableColumn = null
            };
            var updateSet = new UpdateSet<DataRow>().Set(target, row => row.NormalColumn, row => row.NullableColumn)
                .From(
                    set => set.InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                        .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                        .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                        .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId))
                .Where(
                    set => set.AreEqual(row => row.ValueColumn, match.ValueColumn)
                        .AreEqual(row => row.NullableColumn, match.NullableColumn)
                        .AreEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                        .AreEqual(row => row.RelatedAlias.RelatedProperty, match.RelatedAlias.RelatedProperty)
                        .Between(baseline, boundary, row => row.FakeDataId));

            var expected = new object[]
                               {
                                   "UpdatedNormalColumn",
                                   2,
                                   "CouldHaveBeenNull",
                                   "Related",
                                   10,
                                   20
                               };

            var actual = updateSet.PropertyValues.ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// The update statement_ related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Set_RaisedRelatedDataSpecificSetValuesFromItem_MatchesExpected()
        {
            var match = new DataRow
            {
                NormalColumn = "NormalColumn",
                RelatedAlias = new FakeRelatedRow
                {
                    RelatedProperty = "Related"
                },
                NullableColumn = "CouldHaveBeenNull",
                ValueColumn = 2,
                NullableValueColumn = null,
            };

            var baseline = new DataRow
            {
                FakeDataId = 10
            };

            var boundary = new DataRow
            {
                FakeDataId = 20
            };

            var target = new DataRow
            {
                NormalColumn = "UpdatedNormalColumn",
                NullableColumn = null
            };

            var updateSet = new UpdateSet<DataRow>().Set(target, row => row.NormalColumn, row => row.NullableColumn)
                .From(
                    set => set.InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                        .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                        .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                        .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId))
                .Where(
                    set => set.AreEqual(row => row.ValueColumn, match.ValueColumn)
                        .AreEqual(row => row.NullableColumn, match.NullableColumn)
                        .AreEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                        .AreEqual(row => row.RelatedAlias.RelatedProperty, "Related")
                        .Between(baseline, boundary, row => row.FakeDataId));

            var expected = new object[]
                               {
                                   "UpdatedNormalColumn",
                                   2,
                                   "CouldHaveBeenNull",
                                   "Related",
                                   10,
                                   20
                               };

            var actual = updateSet.PropertyValues.ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// The update statement_ direct data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Set_DirectDataSpecificSetValuesExplicit_MatchesExpected()
        {
            var match = new DataRow
            {
                NormalColumn = "NormalColumn",
                NullableColumn = "CouldHaveBeenNull",
                ValueColumn = 2,
                NullableValueColumn = null,
            };

            var baseline = new DataRow
            {
                FakeDataId = 10
            };
            var boundary = new DataRow
            {
                FakeDataId = 20
            };

            var updateSet = new UpdateSet<DataRow>().Set(row => row.NormalColumn, "UpdatedNormalColumn")
                .Set(row => row.NullableColumn, null)
                .From(
                    set => set.InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                        .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                        .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                        .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId))
                .Where(
                    set => set.AreEqual(row => row.ValueColumn, match.ValueColumn)
                        .AreEqual(row => row.NullableColumn, match.NullableColumn)
                        .AreEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                        .Between(baseline, boundary, row => row.FakeDataId));

            var expected = new object[]
                               {
                                   "UpdatedNormalColumn",
                                   2,
                                   "CouldHaveBeenNull",
                                   10,
                                   20
                               };
            var actual = updateSet.PropertyValues.ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// The update statement_ direct data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Set_RaisedDirectDataSpecificSetValuesExplicit_MatchesExpected()
        {
            var match = new DataRow
            {
                NormalColumn = "NormalColumn",
                NullableColumn = "CouldHaveBeenNull",
                ValueColumn = 2,
                NullableValueColumn = null,
            };

            var baseline = new DataRow
            {
                FakeDataId = 10
            };

            var boundary = new DataRow
            {
                FakeDataId = 20
            };

            var updateSet = new UpdateSet<DataRow>().Set(row => row.NormalColumn, "UpdatedNormalColumn")
                .Set(row => row.NullableColumn, null)
                .From(
                    set => set.InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                        .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                        .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                        .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId))
                .Where(
                    set => set.AreEqual(row => row.ValueColumn, match.ValueColumn)
                        .AreEqual(row => row.NullableColumn, match.NullableColumn)
                        .AreEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                        .Between(baseline, boundary, row => row.FakeDataId));

            var expected = new object[]
                               {
                                   "UpdatedNormalColumn",
                                   2,
                                   "CouldHaveBeenNull",
                                   10,
                                   20
                               };

            var actual = updateSet.PropertyValues.ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// The update statement_ related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Set_RelatedDataSpecificSetValuesExplicit_MatchesExpected()
        {
            var match = new DataRow
            {
                NormalColumn = "NormalColumn",
                RelatedAlias = new FakeRelatedRow
                {
                    RelatedProperty = "Related"
                },
                NullableColumn = "CouldHaveBeenNull",
                ValueColumn = 2,
                NullableValueColumn = null,
            };

            var baseline = new DataRow
            {
                FakeDataId = 10
            };

            var boundary = new DataRow
            {
                FakeDataId = 20
            };

            var updateSet = new UpdateSet<DataRow>().Set(row => row.NormalColumn, "UpdatedNormalColumn")
                .Set(row => row.NullableColumn, null)
                .From(
                    set => set.InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                        .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                        .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                        .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId))
                .Where(
                    set => set.AreEqual(row => row.ValueColumn, match.ValueColumn)
                        .AreEqual(row => row.NullableColumn, match.NullableColumn)
                        .AreEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                        .AreEqual(row => row.RelatedAlias.RelatedProperty, match.RelatedAlias.RelatedProperty)
                        .Between(baseline, boundary, row => row.FakeDataId));

            var expected = new object[]
                               {
                                   "UpdatedNormalColumn",
                                   2,
                                   "CouldHaveBeenNull",
                                   "Related",
                                   10,
                                   20
                               };

            var actual = updateSet.PropertyValues.ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// The update statement_ related data_ matches expected.
        /// </summary>
        [TestMethod]
        public void Set_RaisedRelatedDataSpecificSetValuesExplicit_MatchesExpected()
        {
            var match = new DataRow
            {
                NormalColumn = "NormalColumn",
                RelatedAlias = new FakeRelatedRow
                {
                    RelatedProperty = "Related"
                },
                NullableColumn = "CouldHaveBeenNull",
                ValueColumn = 2,
                NullableValueColumn = null,
            };

            var baseline = new DataRow
            {
                FakeDataId = 10
            };

            var boundary = new DataRow
            {
                FakeDataId = 20
            };

            var updateSet = new UpdateSet<DataRow>().Set(row => row.NormalColumn, "UpdatedNormalColumn")
                .Set(row => row.NullableColumn, null)
                .From(
                    set => set.InnerJoin(row => row.FakeDataId, row => row.Related.FakeDataId)
                        .InnerJoin(row => row.Related.RelatedId, row => row.DependencyEntity.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.OtherAlias.FakeDataId)
                        .InnerJoin(row => row.OtherAlias.RelatedId, row => row.RelatedDependency.ComplexEntityId)
                        .InnerJoin(row => row.FakeDataId, row => row.RelatedAlias.FakeDataId)
                        .LeftJoin<SubDataRow>(row => row.FakeDataId, row => row.FakeSubDataId))
                .Where(
                    set => set.AreEqual(row => row.ValueColumn, match.ValueColumn)
                        .AreEqual(row => row.NullableColumn, match.NullableColumn)
                        .AreEqual(row => row.NullableValueColumn, match.NullableValueColumn)
                        .AreEqual(row => row.RelatedAlias.RelatedProperty, "Related")
                        .Between(baseline, boundary, row => row.FakeDataId));

            var expected = new object[]
                               {
                                   "UpdatedNormalColumn",
                                   2,
                                   "CouldHaveBeenNull",
                                   "Related",
                                   10,
                                   20
                               };

            var actual = updateSet.PropertyValues.ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}