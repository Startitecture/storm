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
    using Startitecture.Orm.Mapper;
    using Startitecture.Orm.Schema;

    /// <summary>
    /// The TextElement entity POCO.
    /// </summary>
    [Table("TextElement", Schema = "dbo")]
    [System.CodeDom.Compiler.GeneratedCode("storm.Reverse.POCO.Generator", "1.0.0")]
    public partial class TextElementRow : EntityBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextElementRow"/> class.
        /// </summary>
        public TextElementRow()
        {
        }

        /// <summary>
        /// Gets or sets the TextElementId (Primary key)
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(@"TextElementId", Order = 1, TypeName = "bigint")]
        [Index(@"PK_TextElement", 1, IsUnique = true, IsClustered = true)]
        [Required]
        [Key]
        [Display(Name = "Text element ID")]
        [ForeignKey("FieldValueElement")]
        public long TextElementId { get; set; }

        /// <summary>
        /// Gets or sets the Value
        /// </summary>
        [Column(@"Value", Order = 2, TypeName = "nvarchar(max)")]
        [Required]
        [Display(Name = "Value")]
        public string Value { get; set; }
    }

}
// </auto-generated>
