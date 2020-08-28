// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericSubmissionValueTableTypeRow.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Represents a user-defined table type for the GenericSubmissionValueRow entity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Entities.TableTypes
{
    using System.ComponentModel.DataAnnotations.Schema;

    using Startitecture.Orm.Schema;

    /// <summary>
    /// Represents a user-defined table type for the <see cref="GenericSubmissionValueRow"/> entity.
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