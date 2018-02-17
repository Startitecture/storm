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
    using Startitecture.Orm.Schema;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The FieldValue entity POCO.
    /// </summary>
    [Table("FieldValue", Schema = "forms")]
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.34.1.0")]
    public partial class FieldValueRow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldValueRow"/> class.
        /// </summary>
        public FieldValueRow()
        {
        }

        /// <summary>
        /// Gets or sets the FieldValueId (Primary key)
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [System.ComponentModel.DataAnnotations.Schema.Column(@"FieldValueId", Order = 1, TypeName = "bigint")]
        [Index(@"PK_FieldValue", 1, IsUnique = true, IsClustered = true)]
        [Required]
        [Key]
        [Display(Name = "Field value ID")]
        public long FieldValueId { get; set; }

        /// <summary>
        /// Gets or sets the FormSubmissionId
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(@"FormSubmissionId", Order = 2, TypeName = "bigint")]
        [Required]
        [Display(Name = "Form submission ID")]
        public long FormSubmissionId { get; set; }

        /// <summary>
        /// Gets or sets the FieldInstanceId
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(@"FieldInstanceId", Order = 3, TypeName = "int")]
        [Required]
        [Display(Name = "Field instance ID")]
        public int FieldInstanceId { get; set; }

        // Foreign keys

        /// <summary>
        /// Gets or sets the Parent FieldInstance pointed by [FieldValue].([FieldInstanceId]) (FK_FieldValue_FieldInstance)
        /// </summary>
        [ForeignKey("FieldInstanceId")] public FieldInstanceRow FieldInstance { get; set; } // FK_FieldValue_FieldInstance

        /// <summary>
        /// Gets or sets the Parent FormSubmission pointed by [FieldValue].([FormSubmissionId]) (FK_FieldValue_FormSubmission)
        /// </summary>
        [ForeignKey("FormSubmissionId")] public FormSubmissionRow FormSubmission { get; set; } // FK_FieldValue_FormSubmission
    }

}
// </auto-generated>
