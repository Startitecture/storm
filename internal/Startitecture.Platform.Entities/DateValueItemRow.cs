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
    /// The DateValueItem entity POCO.
    /// </summary>
    [Table("DateValueItem", Schema = "forms")]
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.34.1.0")]
    public partial class DateValueItemRow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateValueItemRow"/> class.
        /// </summary>
        public DateValueItemRow()
        {
        }

        /// <summary>
        /// Gets or sets the DateValueItemId (Primary key)
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [System.ComponentModel.DataAnnotations.Schema.Column(@"DateValueItemId", Order = 1, TypeName = "bigint")]
        [Index(@"PK_DateValueItem", 1, IsUnique = true, IsClustered = true)]
        [Required]
        [Key]
        [Display(Name = "Date value item ID")]
        [ForeignKey("FieldValueItem")]
        public long DateValueItemId { get; set; }

        /// <summary>
        /// Gets or sets the Value
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(@"Value", Order = 2, TypeName = "datetimeoffset")]
        [Required]
        [Display(Name = "Value")]
        public System.DateTimeOffset Value { get; set; }

        // Foreign keys

        /// <summary>
        /// Gets or sets the Parent FieldValueItem pointed by [DateValueItem].([DateValueItemId]) (FK_DateValueItem_FieldValueItem)
        /// </summary>
        [ForeignKey("DateValueItemId")] public FieldValueItemRow FieldValueItem { get; set; } // FK_DateValueItem_FieldValueItem
    }

}
// </auto-generated>