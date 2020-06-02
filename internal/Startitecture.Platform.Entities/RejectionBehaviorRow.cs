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
    /// The RejectionBehavior entity POCO.
    /// </summary>
    [Table("RejectionBehavior", Schema = "workflow")]
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.34.1.0")]
    public partial class RejectionBehaviorRow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RejectionBehaviorRow"/> class.
        /// </summary>
        public RejectionBehaviorRow()
        {
        }

        /// <summary>
        /// Gets or sets the RejectionBehaviorId (Primary key)
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [System.ComponentModel.DataAnnotations.Schema.Column(@"RejectionBehaviorId", Order = 1, TypeName = "int")]
        [Index(@"PK_RejectionBehavior", 1, IsUnique = true, IsClustered = true)]
        [Required]
        [Key]
        [Display(Name = "Rejection behavior ID")]
        public int RejectionBehaviorId { get; set; }

        /// <summary>
        /// Gets or sets the Name (length: 50)
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(@"Name", Order = 2, TypeName = "varchar")]
        [Required]
        [MaxLength(50)]
        [StringLength(50)]
        [Display(Name = "Name")]
        public string Name { get; set; }
    }

}
// </auto-generated>