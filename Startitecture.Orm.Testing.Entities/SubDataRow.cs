// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubDataRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Startitecture.Orm.Common;
    using Startitecture.Orm.Mapper;

    /// <summary>
    /// The fake sub data row.
    /// </summary>
    [Table("SubData", Schema = "dbo")]
    public class SubDataRow : EntityBase
    {
        /// <summary>
        /// Gets or sets the fake sub data id.
        /// </summary>
        [Key]
        public int FakeSubDataId { get; set; }

        /// <summary>
        /// Gets or sets the parent fake data id.
        /// </summary>
        public int ParentFakeDataId { get; set; }
    }
}