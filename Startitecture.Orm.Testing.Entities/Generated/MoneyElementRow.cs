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
    /// The MoneyElement entity POCO.
    /// </summary>
    [Table("MoneyElement", Schema = "dbo")]
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.34.1.0")]
    public partial class MoneyElementRow : TransactionItemBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MoneyElementRow"/> class.
        /// </summary>
        public MoneyElementRow()
        {
        }

        /// <summary>
        /// Gets or sets the MoneyElementId (Primary key)
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(@"MoneyElementId", Order = 1, TypeName = "bigint")]
        [Index(@"PK_MoneyElement", 1, IsUnique = true, IsClustered = true)]
        [Required]
        [Key]
        [Display(Name = "Money element ID")]
        [ForeignKey("FieldValueElement")]
        public long MoneyElementId { get; set; }

        /// <summary>
        /// Gets or sets the Value
        /// </summary>
        [Column(@"Value", Order = 2, TypeName = "money")]
        [Required]
        [Display(Name = "Value")]
        public decimal Value { get; set; }
    }

}
// </auto-generated>
