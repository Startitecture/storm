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
    /// The ReferenceValueItem entity POCO.
    /// </summary>
    [Table("ReferenceValueItem", Schema = "forms")]
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.34.1.0")]
    public partial class ReferenceValueItemRow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceValueItemRow"/> class.
        /// </summary>
        public ReferenceValueItemRow()
        {
        }

        /// <summary>
        /// Gets or sets the ReferenceValueItemId (Primary key)
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(@"ReferenceValueItemId", Order = 1, TypeName = "bigint")]
        [Index(@"PK_ReferenceValueItem", 1, IsUnique = true, IsClustered = true)]
        [Required]
        [Key]
        [Display(Name = "Reference value item ID")]
        [ForeignKey("FieldValueItem")]
        public long ReferenceValueItemId { get; set; }

        /// <summary>
        /// Gets or sets the ItemType (length: 250)
        /// </summary>
        [Column(@"ItemType", Order = 2, TypeName = "nvarchar")]
        [Required]
        [MaxLength(250)]
        [StringLength(250)]
        [Display(Name = "Item type")]
        public string ItemType { get; set; }

        /// <summary>
        /// Gets or sets the ItemId
        /// </summary>
        [Column(@"ItemId", Order = 3, TypeName = "bigint")]
        [Required]
        [Display(Name = "Item ID")]
        public long ItemId { get; set; }

        // Foreign keys

        /// <summary>
        /// Gets or sets the Parent FieldValueItem pointed by [ReferenceValueItem].([ReferenceValueItemId]) (FK_ReferenceValueItem_FieldValueItem)
        /// </summary>
        [ForeignKey("ReferenceValueItemId")] public FieldValueItemRow FieldValueItem { get; set; } // FK_ReferenceValueItem_FieldValueItem
    }

}
// </auto-generated>
