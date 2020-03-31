// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeSubDataRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Schema.Tests
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// The fake sub data row.
    /// </summary>
    [Table("FakeSubData", Schema = "dbo")]
    public class FakeSubDataRow : TransactionItemBase
    {
        /// <summary>
        /// Gets or sets the fake sub data id.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FakeSubDataId { get; set; }

        /// <summary>
        /// Gets or sets the parent fake data id.
        /// </summary>
        public int ParentFakeDataId { get; set; }
    }
}