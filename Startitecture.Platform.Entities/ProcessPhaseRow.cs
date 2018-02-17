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
    using Startitecture.Orm.Schema;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The ProcessPhase entity POCO.
    /// </summary>
    [Table("ProcessPhase", Schema = "workflow")]
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.34.1.0")]
    public partial class ProcessPhaseRow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessPhaseRow"/> class.
        /// </summary>
        public ProcessPhaseRow()
        {
        }

        /// <summary>
        /// Gets or sets the ProcessPhaseId (Primary key)
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [System.ComponentModel.DataAnnotations.Schema.Column(@"ProcessPhaseId", Order = 1, TypeName = "int")]
        [Index(@"PK_ProcessPhase", 1, IsUnique = true, IsClustered = true)]
        [Required]
        [Key]
        [Display(Name = "Process phase ID")]
        public int ProcessPhaseId { get; set; }

        /// <summary>
        /// Gets or sets the ProcessVersionId
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(@"ProcessVersionId", Order = 2, TypeName = "int")]
        [Required]
        [Display(Name = "Process version ID")]
        public int ProcessVersionId { get; set; }

        /// <summary>
        /// Gets or sets the PhaseTypeId
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(@"PhaseTypeId", Order = 3, TypeName = "int")]
        [Required]
        [Display(Name = "Phase type ID")]
        public int PhaseTypeId { get; set; }

        /// <summary>
        /// Gets or sets the Name (length: 50)
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(@"Name", Order = 4, TypeName = "nvarchar")]
        [Required]
        [MaxLength(50)]
        [StringLength(50)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Order
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(@"Order", Order = 5, TypeName = "smallint")]
        [Required]
        [Display(Name = "Order")]
        public short Order { get; set; }

        // Foreign keys

        /// <summary>
        /// Gets or sets the Parent PhaseType pointed by [ProcessPhase].([PhaseTypeId]) (FK_ProcessPhase_PhaseType)
        /// </summary>
        [ForeignKey("PhaseTypeId")] public PhaseTypeRow PhaseType { get; set; } // FK_ProcessPhase_PhaseType

        /// <summary>
        /// Gets or sets the Parent ProcessVersion pointed by [ProcessPhase].([ProcessVersionId]) (FK_ProcessPhase_ProcessVersion)
        /// </summary>
        [ForeignKey("ProcessVersionId")] public ProcessVersionRow ProcessVersion { get; set; } // FK_ProcessPhase_ProcessVersion
    }

}
// </auto-generated>
