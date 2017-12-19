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

namespace Startitecture.Platform.Entities.Workflow
{
    using Startitecture.Orm.Common;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The WorkflowFormSubmission entity POCO.
    /// </summary>
    [Table("WorkflowFormSubmission", Schema = "workflow")]
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.34.1.0")]
    public partial class WorkflowFormSubmissionRow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowFormSubmissionRow"/> class.
        /// </summary>
        public WorkflowFormSubmissionRow()
        {
        }

        /// <summary>
        /// Gets or sets the WorkflowFormSubmissionId (Primary key)
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(@"WorkflowFormSubmissionId", Order = 1, TypeName = "bigint")]
        [Index(@"PK_WorkflowFormSubmission", 1, IsUnique = true, IsClustered = true)]
        [Required]
        [Key]
        [Display(Name = "Workflow form submission ID")]
        public long WorkflowFormSubmissionId { get; set; }

        /// <summary>
        /// Gets or sets the WorkflowPhaseInstanceId
        /// </summary>
        [Column(@"WorkflowPhaseInstanceId", Order = 2, TypeName = "bigint")]
        [Required]
        [Display(Name = "Workflow phase instance ID")]
        public long WorkflowPhaseInstanceId { get; set; }

        /// <summary>
        /// Gets or sets the FormPhaseInstanceId
        /// </summary>
        [Column(@"FormPhaseInstanceId", Order = 3, TypeName = "int")]
        [Required]
        [Display(Name = "Form phase instance ID")]
        public int FormPhaseInstanceId { get; set; }

        // Foreign keys

        /// <summary>
        /// Gets or sets the Parent FormPhaseInstance pointed by [WorkflowFormSubmission].([FormPhaseInstanceId]) (FK_WorkflowFormSubmission_FormPhaseInstance)
        /// </summary>
        [ForeignKey("FormPhaseInstanceId")] public FormPhaseInstanceRow FormPhaseInstance { get; set; } // FK_WorkflowFormSubmission_FormPhaseInstance

        /// <summary>
        /// Gets or sets the Parent WorkflowPhaseInstance pointed by [WorkflowFormSubmission].([WorkflowPhaseInstanceId]) (FK_WorkflowFormSubmission_WorkflowPhaseInstance)
        /// </summary>
        [ForeignKey("WorkflowPhaseInstanceId")] public WorkflowPhaseInstanceRow WorkflowPhaseInstance { get; set; } // FK_WorkflowFormSubmission_WorkflowPhaseInstance
    }

}
// </auto-generated>
