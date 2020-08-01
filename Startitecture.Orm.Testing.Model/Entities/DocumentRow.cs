// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Model.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Startitecture.Orm.Common;
    using Startitecture.Orm.Schema;

    /// <summary>
    /// The document row.
    /// </summary>
    [Table("Document", Schema = "dbo")]
    public class DocumentRow : EntityBase
    {
        /// <summary>
        /// Gets or sets the document ID.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(@"DocumentId", Order = 1)]
        [Key]
        public int DocumentId { get; set; }

        /// <summary>
        /// Gets or sets the document version ID.
        /// </summary>
        [Column(@"DocumentVersionId", Order = 2)]
        public long DocumentVersionId { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        [Column(@"DocumentVersionId", Order = 3)]
        public string Identifier { get; set; }

        /// <summary>
        /// Gets or sets the document version.
        /// </summary>
        [Relation]
        public DocumentVersionRow DocumentVersion { get; set; }
    }
}