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

namespace Startitecture.Orm.Testing.Entities
{
    using Startitecture.Orm.Mapper;
    using Startitecture.Orm.Schema;

    /// <summary>
    /// The IntegerElement entity POCO.
    /// </summary>
    [Table("IntegerElement", Schema = "dbo")]
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.34.1.0")]
    public partial class IntegerElementRow: TransactionItemBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IntegerElementRow"/> class.
        /// </summary>
        public IntegerElementRow()
        {
        }

        /// <summary>
        /// Gets or sets the IntegerElementId (Primary key)
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(@"IntegerElementId", Order = 1, TypeName = "bigint")]
        [Index(@"PK_IntegerElement", 1, IsUnique = true, IsClustered = true)]
        [Required]
        [Key]
        [Display(Name = "Integer element ID")]
        [ForeignKey("FieldValueElement")]
        public long IntegerElementId { get; set; }

        /// <summary>
        /// Gets or sets the Value
        /// </summary>
        [Column(@"Value", Order = 2, TypeName = "int")]
        [Required]
        [Display(Name = "Value")]
        public int Value { get; set; }
    }

}
// </auto-generated>
