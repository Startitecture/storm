// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentVersionRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Startitecture.Orm.Common;

    /// <summary>
    /// The document version row.
    /// </summary>
    [Table("DocumentVersion", Schema = "dbo")]
    public class DocumentVersionRow : EntityBase
    {
        /// <summary>
        /// Gets or sets the document version id.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(@"DocumentVersionId", Order = 1)]
        [Key]
        public long DocumentVersionId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Column(@"DocumentVersionId", Order = 2)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the version number.
        /// </summary>
        [Column(@"VersionNumber", Order = 3)]
        public int VersionNumber { get; set; }
    }
}