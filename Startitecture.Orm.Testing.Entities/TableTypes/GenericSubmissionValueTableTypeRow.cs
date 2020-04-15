// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericSubmissionValueTableTypeRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Entities.TableTypes
{
    using System.ComponentModel.DataAnnotations.Schema;

    using Startitecture.Orm.Schema;

    /// <summary>
    /// The generic submission value table type row.
    /// </summary>
    [TableType("GenericSubmissionValueTableType")]
    [Table("GenericSubmissionValue", Schema = "dbo")]
    public class GenericSubmissionValueTableTypeRow
    {
        /// <summary>
        /// Gets or sets the generic submission value id.
        /// </summary>
        [Column(Order = 1)]
        public long GenericSubmissionValueId { get; set; }

        /// <summary>
        /// Gets or sets the generic submission id.
        /// </summary>
        [Column(Order = 2)]
        public int GenericSubmissionId { get; set; }
    }
}