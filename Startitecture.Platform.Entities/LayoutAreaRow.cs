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
    /// The LayoutArea entity POCO.
    /// </summary>
    [Table("LayoutArea", Schema = "forms")]
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.34.1.0")]
    public partial class LayoutAreaRow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutAreaRow"/> class.
        /// </summary>
        public LayoutAreaRow()
        {
        }

        /// <summary>
        /// Gets or sets the LayoutAreaId (Primary key)
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [System.ComponentModel.DataAnnotations.Schema.Column(@"LayoutAreaId", Order = 1, TypeName = "int")]
        [Index(@"PK_LayoutArea", 1, IsUnique = true, IsClustered = true)]
        [Required]
        [Key]
        [Display(Name = "Layout area ID")]
        public int LayoutAreaId { get; set; }

        /// <summary>
        /// Gets or sets the SectionId
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(@"SectionId", Order = 2, TypeName = "int")]
        [Required]
        [Display(Name = "Section ID")]
        public int SectionId { get; set; }

        /// <summary>
        /// Gets or sets the Identifier
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(@"Identifier", Order = 3, TypeName = "uniqueidentifier")]
        [Required]
        [Display(Name = "Identifier")]
        public System.Guid Identifier { get; set; }

        /// <summary>
        /// Gets or sets the Order
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(@"Order", Order = 4, TypeName = "smallint")]
        [Required]
        [Display(Name = "Order")]
        public short Order { get; set; }

        /// <summary>
        /// Gets or sets the Style (length: 50)
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(@"Style", Order = 5, TypeName = "varchar")]
        [Required]
        [MaxLength(50)]
        [StringLength(50)]
        [Display(Name = "Style")]
        public string Style { get; set; }

        // Foreign keys

        /// <summary>
        /// Gets or sets the Parent Section pointed by [LayoutArea].([SectionId]) (FK_LayoutArea_Section)
        /// </summary>
        [ForeignKey("SectionId")] public SectionRow Section { get; set; } // FK_LayoutArea_Section
    }

}
// </auto-generated>
