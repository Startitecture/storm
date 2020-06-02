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
    /// The DiscreteValueItem entity POCO.
    /// </summary>
    [Table("DiscreteValueItem", Schema = "forms")]
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.34.1.0")]
    public partial class DiscreteValueItemRow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiscreteValueItemRow"/> class.
        /// </summary>
        public DiscreteValueItemRow()
        {
        }

        /// <summary>
        /// Gets or sets the DiscreteValueItemId (Primary key)
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [System.ComponentModel.DataAnnotations.Schema.Column(@"DiscreteValueItemId", Order = 1, TypeName = "bigint")]
        [Index(@"PK_DiscreteValueItem", 1, IsUnique = true, IsClustered = true)]
        [Required]
        [Key]
        [Display(Name = "Discrete value item ID")]
        [ForeignKey("FieldValueItem")]
        public long DiscreteValueItemId { get; set; }

        /// <summary>
        /// Gets or sets the Value
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(@"Value", Order = 2, TypeName = "bigint")]
        [Required]
        [Display(Name = "Value")]
        public long Value { get; set; }

        // Foreign keys

        /// <summary>
        /// Gets or sets the Parent FieldValueItem pointed by [DiscreteValueItem].([DiscreteValueItemId]) (FK_DiscreteValueItem_FieldValueItem)
        /// </summary>
        [ForeignKey("DiscreteValueItemId")] public FieldValueItemRow FieldValueItem { get; set; } // FK_DiscreteValueItem_FieldValueItem
    }

}
// </auto-generated>