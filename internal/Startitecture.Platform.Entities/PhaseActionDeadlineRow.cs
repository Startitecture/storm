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
    /// The PhaseActionDeadline entity POCO.
    /// </summary>
    [Table("PhaseActionDeadline", Schema = "workflow")]
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.34.1.0")]
    public partial class PhaseActionDeadlineRow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PhaseActionDeadlineRow"/> class.
        /// </summary>
        public PhaseActionDeadlineRow()
        {
        }

        /// <summary>
        /// Gets or sets the PhaseActionDeadlineId (Primary key)
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [System.ComponentModel.DataAnnotations.Schema.Column(@"PhaseActionDeadlineId", Order = 1, TypeName = "int")]
        [Index(@"PK_PhaseActionDeadline", 1, IsUnique = true, IsClustered = true)]
        [Required]
        [Key]
        [Display(Name = "Phase action deadline ID")]
        [ForeignKey("ProcessPhase")]
        public int PhaseActionDeadlineId { get; set; }

        /// <summary>
        /// Gets or sets the DeadlineTypeId
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(@"DeadlineTypeId", Order = 2, TypeName = "int")]
        [Required]
        [Display(Name = "Deadline type ID")]
        public int DeadlineTypeId { get; set; }

        /// <summary>
        /// Gets or sets the RoutingTypeId
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(@"RoutingTypeId", Order = 3, TypeName = "int")]
        [Required]
        [Display(Name = "Routing type ID")]
        public int RoutingTypeId { get; set; }

        /// <summary>
        /// Gets or sets the DeadlineDays
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(@"DeadlineDays", Order = 4, TypeName = "smallint")]
        [Required]
        [Display(Name = "Deadline days")]
        public short DeadlineDays { get; set; }

        // Foreign keys

        /// <summary>
        /// Gets or sets the Parent DeadlineType pointed by [PhaseActionDeadline].([DeadlineTypeId]) (FK_PhaseActionDeadline_DeadlineType)
        /// </summary>
        [ForeignKey("DeadlineTypeId")] public DeadlineTypeRow DeadlineType { get; set; } // FK_PhaseActionDeadline_DeadlineType

        /// <summary>
        /// Gets or sets the Parent ProcessPhase pointed by [PhaseActionDeadline].([PhaseActionDeadlineId]) (FK_PhaseActionDeadline_ProcessPhase)
        /// </summary>
        [ForeignKey("PhaseActionDeadlineId")] public ProcessPhaseRow ProcessPhase { get; set; } // FK_PhaseActionDeadline_ProcessPhase

        /// <summary>
        /// Gets or sets the Parent RoutingType pointed by [PhaseActionDeadline].([RoutingTypeId]) (FK_PhaseActionDeadline_RoutingType)
        /// </summary>
        [ForeignKey("RoutingTypeId")] public RoutingTypeRow RoutingType { get; set; } // FK_PhaseActionDeadline_RoutingType
    }

}
// </auto-generated>