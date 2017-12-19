// <auto-generated>
// ReSharper disable ConvertPropertyToExpressionBody
// ReSharper disable DoNotCallOverridableMethodsInConstructor
// ReSharper disable EmptyNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable PartialMethodWithSinglePart
// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable RedundantNameQualifier
// ReSharper disable RedundantOverridenMember
// ReSharper disable UseNameofExpression
// TargetFrameworkVersion = 4.7
#pragma warning disable 1591    //  Ignore "Missing XML Comment" warning

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Startitecture.Platform.Entities.Forms
{
    using Startitecture.Orm.Common;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The FormSubmission entity POCO.
    /// </summary>
    [Table("FormSubmission", Schema = "forms")]
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.34.1.0")]
    public partial class FormSubmissionRow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormSubmissionRow"/> class.
        /// </summary>
        public FormSubmissionRow()
        {
        }

        /// <summary>
        /// Gets or sets the FormSubmissionId (Primary key)
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(@"FormSubmissionId", Order = 1, TypeName = "bigint")]
        [Index(@"PK_FormSubmission", 1, IsUnique = true, IsClustered = true)]
        [Required]
        [Key]
        [Display(Name = "Form submission ID")]
        public long FormSubmissionId { get; set; }

        /// <summary>
        /// Gets or sets the FormVersionId
        /// </summary>
        [Column(@"FormVersionId", Order = 2, TypeName = "int")]
        [Required]
        [Display(Name = "Form version ID")]
        public int FormVersionId { get; set; }

        /// <summary>
        /// Gets or sets the SubmissionTime
        /// </summary>
        [Column(@"SubmissionTime", Order = 3, TypeName = "datetimeoffset")]
        [Required]
        [Display(Name = "Submission time")]
        public System.DateTimeOffset SubmissionTime { get; set; }

        /// <summary>
        /// Gets or sets the SubmittedByPersonId
        /// </summary>
        [Column(@"SubmittedByPersonId", Order = 4, TypeName = "int")]
        [Required]
        [Display(Name = "Submitted by person ID")]
        public int SubmittedByPersonId { get; set; }

        // Foreign keys

        /// <summary>
        /// Gets or sets the Parent FormVersion pointed by [FormSubmission].([FormVersionId]) (FK_FormSubmission_FormVersion)
        /// </summary>
        [ForeignKey("FormVersionId")] public FormVersionRow FormVersion { get; set; } // FK_FormSubmission_FormVersion
    }

}
// </auto-generated>
