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
    /// The PhaseSignatureOption entity POCO.
    /// </summary>
    [Table("PhaseSignatureOption", Schema = "workflow")]
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.34.1.0")]
    public partial class PhaseSignatureOptionRow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PhaseSignatureOptionRow"/> class.
        /// </summary>
        public PhaseSignatureOptionRow()
        {
        }

        /// <summary>
        /// Gets or sets the PhaseSignatureOptionId (Primary key)
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [System.ComponentModel.DataAnnotations.Schema.Column(@"PhaseSignatureOptionId", Order = 1, TypeName = "int")]
        [Index(@"PK_PhaseSignatureOption", 1, IsUnique = true, IsClustered = true)]
        [Required]
        [Key]
        [Display(Name = "Phase signature option ID")]
        [ForeignKey("ProcessPhase")]
        public int PhaseSignatureOptionId { get; set; }

        /// <summary>
        /// Gets or sets the SignatureTypeId
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(@"SignatureTypeId", Order = 2, TypeName = "int")]
        [Required]
        [Display(Name = "Signature type ID")]
        public int SignatureTypeId { get; set; }

        /// <summary>
        /// Gets or sets the RejectionBehaviorId
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(@"RejectionBehaviorId", Order = 3, TypeName = "int")]
        [Required]
        [Display(Name = "Rejection behavior ID")]
        public int RejectionBehaviorId { get; set; }

        // Foreign keys

        /// <summary>
        /// Gets or sets the Parent ProcessPhase pointed by [PhaseSignatureOption].([PhaseSignatureOptionId]) (FK_PhaseSignatureOption_ProcessPhase)
        /// </summary>
        [ForeignKey("PhaseSignatureOptionId")] public ProcessPhaseRow ProcessPhase { get; set; } // FK_PhaseSignatureOption_ProcessPhase

        /// <summary>
        /// Gets or sets the Parent RejectionBehavior pointed by [PhaseSignatureOption].([RejectionBehaviorId]) (FK_PhaseSignatureOption_RejectionBehavior)
        /// </summary>
        [ForeignKey("RejectionBehaviorId")] public RejectionBehaviorRow RejectionBehavior { get; set; } // FK_PhaseSignatureOption_RejectionBehavior

        /// <summary>
        /// Gets or sets the Parent SignatureType pointed by [PhaseSignatureOption].([SignatureTypeId]) (FK_PhaseSignatureOption_SignatureType)
        /// </summary>
        [ForeignKey("SignatureTypeId")] public SignatureTypeRow SignatureType { get; set; } // FK_PhaseSignatureOption_SignatureType
    }

}
// </auto-generated>