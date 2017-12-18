// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeSubDataRow.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Testing.Common
{
    using SAF.Data.Providers;

    /// <summary>
    /// The fake sub data row.
    /// </summary>
    [PrimaryKey("FakeSubDataId", AutoIncrement = false)]
    [TableName("[FakeSubData]")]
    public class FakeSubDataRow : TransactionItemBase
    {
        /// <summary>
        /// Gets or sets the fake sub data id.
        /// </summary>
        public int FakeSubDataId { get; set; }

        /// <summary>
        /// Gets or sets the parent fake data id.
        /// </summary>
        public int ParentFakeDataId { get; set; }
    }
}