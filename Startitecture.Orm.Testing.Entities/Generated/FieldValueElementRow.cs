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
// TargetFrameworkVersion = 2.1
#pragma warning disable 1591    //  Ignore "Missing XML Comment" warning

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Startitecture.Orm.Testing.Entities
{
    using Startitecture.Orm.Mapper;
    using Startitecture.Orm.Schema;

    /// <summary>
    /// The FieldValueElement entity POCO.
    /// </summary>
    [Table("FieldValueElement", Schema = "dbo")]
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.34.1.0")]
    public partial class FieldValueElementRow : EntityBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldValueElementRow"/> class.
        /// </summary>
        public FieldValueElementRow()
        {
        }

        /// <summary>
        /// Gets or sets the FieldValueElementId (Primary key)
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(@"FieldValueElementId", Order = 1, TypeName = "bigint")]
        [Index(@"PK_FieldValueElement", 1, IsUnique = true, IsClustered = true)]
        [Required]
        [Key]
        [Display(Name = "Field value element ID")]
        public long FieldValueElementId { get; set; }

        /// <summary>
        /// Gets or sets the FieldValueId
        /// </summary>
        [Column(@"FieldValueId", Order = 2, TypeName = "bigint")]
        [Required]
        [Display(Name = "Field value ID")]
        public long FieldValueId { get; set; }

        /// <summary>
        /// Gets or sets the Order
        /// </summary>
        [Column(@"Order", Order = 3, TypeName = "int")]
        [Required]
        [Display(Name = "Order")]
        public int Order { get; set; }
    }

}
// </auto-generated>
