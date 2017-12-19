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
    /// The ProcessVersion entity POCO.
    /// </summary>
    [Table("ProcessVersion", Schema = "workflow")]
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.34.1.0")]
    public partial class ProcessVersionRow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessVersionRow"/> class.
        /// </summary>
        public ProcessVersionRow()
        {
        }

        /// <summary>
        /// Gets or sets the ProcessVersionId (Primary key)
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(@"ProcessVersionId", Order = 1, TypeName = "int")]
        [Index(@"PK_ProcessVersion", 1, IsUnique = true, IsClustered = true)]
        [Required]
        [Key]
        [Display(Name = "Process version ID")]
        public int ProcessVersionId { get; set; }

        /// <summary>
        /// Gets or sets the ProcessId
        /// </summary>
        [Column(@"ProcessId", Order = 2, TypeName = "int")]
        [Required]
        [Display(Name = "Process ID")]
        public int ProcessId { get; set; }

        /// <summary>
        /// Gets or sets the Revision
        /// </summary>
        [Column(@"Revision", Order = 3, TypeName = "int")]
        [Required]
        [Display(Name = "Revision")]
        public int Revision { get; set; }

        /// <summary>
        /// Gets or sets the Name (length: 50)
        /// </summary>
        [Column(@"Name", Order = 4, TypeName = "nvarchar")]
        [Required]
        [MaxLength(50)]
        [StringLength(50)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        // Foreign keys

        /// <summary>
        /// Gets or sets the Parent Process pointed by [ProcessVersion].([ProcessId]) (FK_ProcessVersion_Process)
        /// </summary>
        [ForeignKey("ProcessId")] public ProcessRow Process { get; set; } // FK_ProcessVersion_Process
    }

}
// </auto-generated>
