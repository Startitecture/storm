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
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Schema;

    /// <summary>
    /// The IntegerElement entity POCO.
    /// </summary>
    [Table("IntegerElement", Schema = "dbo")]
    [System.CodeDom.Compiler.GeneratedCode("storm.Reverse.POCO.Generator", "1.0.0")]
    public partial class IntegerElementRow : EntityBase
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
        [Column(@"Value", Order = 2, TypeName = "bigint")]
        [Required]
        [Display(Name = "Value")]
        public long Value { get; set; }
    }

}
// </auto-generated>
