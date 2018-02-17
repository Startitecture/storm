// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeSubDataRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
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