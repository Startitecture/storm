﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeDataRowBase.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The fake data row base.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Schema;

    /// <summary>
    /// The fake data row base.
    /// </summary>
    [PrimaryKey("FakeRowId", AutoIncrement = true)]
    [TableName("[FakeData]")]
    public class FakeDataRowBase : ITransactionContext
    {
        /// <summary>
        /// Gets or sets the fake data id.
        /// </summary>
        [Column("FakeRowId")]
        public int FakeDataId { get; set; }

        /// <summary>
        /// Gets or sets the normal column.
        /// </summary>
        [Column]
        public string NormalColumn { get; set; }

        /// <summary>
        /// Gets or sets the nullable column.
        /// </summary>
        [Column]
        public string NullableColumn { get; set; }

        /// <summary>
        /// Gets or sets the value column.
        /// </summary>
        [Column]
        public int ValueColumn { get; set; }

        /// <summary>
        /// Gets or sets the another value column.
        /// </summary>
        [Column]
        public int AnotherValueColumn { get; set; }

        /// <summary>
        /// Gets or sets the another column.
        /// </summary>
        [Column]
        public string AnotherColumn { get; set; }

        /// <summary>
        /// Gets or sets the nullable value column.
        /// </summary>
        [Column]
        public int? NullableValueColumn { get; set; }

        /// <summary>
        /// Gets the transaction provider.
        /// </summary>
        public IRepositoryProvider TransactionProvider { get; private set; }

        /// <summary>
        /// The set transaction provider.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider.
        /// </param>
        public void SetTransactionProvider(IRepositoryProvider repositoryProvider)
        {
            this.TransactionProvider = repositoryProvider;
        }
    }
}