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
    /// The FloatingValueItem entity POCO.
    /// </summary>
    [Table("FloatingValueItem", Schema = "forms")]
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.34.1.0")]
    public partial class FloatingValueItemRow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FloatingValueItemRow"/> class.
        /// </summary>
        public FloatingValueItemRow()
        {
        }

        /// <summary>
        /// Gets or sets the FloatingValueItemId (Primary key)
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(@"FloatingValueItemId", Order = 1, TypeName = "bigint")]
        [Index(@"PK_FloatingValueItem", 1, IsUnique = true, IsClustered = true)]
        [Required]
        [Key]
        [Display(Name = "Floating value item ID")]
        [ForeignKey("FieldValueItem")]
        public long FloatingValueItemId { get; set; }

        /// <summary>
        /// Gets or sets the Value
        /// </summary>
        [Column(@"Value", Order = 2, TypeName = "float")]
        [Required]
        [Display(Name = "Value")]
        public double Value { get; set; }

        // Foreign keys

        /// <summary>
        /// Gets or sets the Parent FieldValueItem pointed by [FloatingValueItem].([FloatingValueItemId]) (FK_FloatingValueItem_FieldValueItem)
        /// </summary>
        [ForeignKey("FloatingValueItemId")] public FieldValueItemRow FieldValueItem { get; set; } // FK_FloatingValueItem_FieldValueItem
    }

}
// </auto-generated>
