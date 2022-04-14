// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataTableLoaderTests.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.SqlClient.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Orm.Schema;
    using Startitecture.Orm.Testing.Entities;

    /// <summary>
    /// Tests the <see cref="DataTableLoader{T}"/> class.
    /// </summary>
    [TestClass]
    public class DataTableLoaderTests
    {
        /// <summary>
        /// Tests the Load method.
        /// </summary>
        [TestMethod]
        public void Load_ListOfRows_ColumnOrdinalsPreserved()
        {
            var target = new DataTableLoader<DependentRow>(new DataAnnotationsDefinitionProvider());
            var dependentRows = new List<DependentRow>
                                {
                                    new DependentRow
                                    {
                                        FakeDependentEntityId = 454, DependentIntegerValue = 3, DependentTimeValue = DateTimeOffset.MaxValue
                                    },
                                    new DependentRow
                                    {
                                        FakeDependentEntityId = 455, DependentIntegerValue = 2, DependentTimeValue = DateTimeOffset.MinValue
                                    },
                                    new DependentRow
                                    {
                                        FakeDependentEntityId = 456, DependentIntegerValue = 1, DependentTimeValue = DateTimeOffset.Now
                                    },
                                    new DependentRow
                                    {
                                        FakeDependentEntityId = 457, DependentIntegerValue = 1, DependentTimeValue = DateTimeOffset.Now
                                    },
                                    new DependentRow
                                    {
                                        FakeDependentEntityId = 458, DependentIntegerValue = 3, DependentTimeValue = DateTimeOffset.MaxValue
                                    }
                                };

            var actual = target.Load(dependentRows);
            Assert.AreEqual(nameof(DependentRow.FakeDependentEntityId), actual.Columns[0].ColumnName);
            Assert.AreEqual(nameof(DependentRow.DependentIntegerValue), actual.Columns[1].ColumnName);
            Assert.AreEqual(nameof(DependentRow.DependentTimeValue), actual.Columns[2].ColumnName);
        }

        /// <summary>
        /// Tests the Load method.
        /// </summary>
        [TestMethod]
        public void Load_ListOfRows_DataMatchesExpected()
        {
            var target = new DataTableLoader<DependentRow>(new DataAnnotationsDefinitionProvider());
            var dependentRows = new List<DependentRow>
                                {
                                    new DependentRow
                                    {
                                        FakeDependentEntityId = 454, DependentIntegerValue = 3, DependentTimeValue = DateTimeOffset.MaxValue
                                    },
                                    new DependentRow
                                    {
                                        FakeDependentEntityId = 455, DependentIntegerValue = 2, DependentTimeValue = DateTimeOffset.MinValue
                                    },
                                    new DependentRow
                                    {
                                        FakeDependentEntityId = 456, DependentIntegerValue = 1, DependentTimeValue = DateTimeOffset.Now
                                    },
                                    new DependentRow
                                    {
                                        FakeDependentEntityId = 457, DependentIntegerValue = 1, DependentTimeValue = DateTimeOffset.Now
                                    },
                                    new DependentRow
                                    {
                                        FakeDependentEntityId = 458, DependentIntegerValue = 3, DependentTimeValue = DateTimeOffset.MaxValue
                                    }
                                };

            var actual = target.Load(dependentRows);
            Assert.AreEqual(dependentRows.Count, actual.Rows.Count);

            for (int i = 0; i < dependentRows.Count; i++)
            {
                var actualRow = new DependentRow
                {
                    FakeDependentEntityId = (int)actual.Rows[i][nameof(DependentRow.FakeDependentEntityId)],
                    DependentIntegerValue = (int)actual.Rows[i][nameof(DependentRow.DependentIntegerValue)],
                    DependentTimeValue = (DateTimeOffset)actual.Rows[i][nameof(DependentRow.DependentTimeValue)]
                };

                Assert.AreEqual(dependentRows.ElementAt(i), actualRow);
            }
        }
    }
}