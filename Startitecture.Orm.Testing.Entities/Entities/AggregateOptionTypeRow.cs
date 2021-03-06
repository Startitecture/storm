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
    /// The AggregateOptionType entity POCO.
    /// </summary>
    [Table("AggregateOptionType", Schema = "dbo")]
    [System.CodeDom.Compiler.GeneratedCode("storm.Reverse.POCO.Generator", "1.0.0")]
    public partial class AggregateOptionTypeRow : EntityBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateOptionTypeRow"/> class.
        /// </summary>
        public AggregateOptionTypeRow()
        {
        }

        /// <summary>
        /// Gets or sets the AggregateOptionTypeId (Primary key)
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(@"AggregateOptionTypeId", Order = 1, TypeName = "int")]
        [Index(@"PK_AggregateOptionType", 1, IsUnique = true, IsClustered = true)]
        [Required]
        [Key]
        [Display(Name = "Aggregate option type ID")]
        public int AggregateOptionTypeId { get; set; }

        /// <summary>
        /// Gets or sets the Name (length: 50)
        /// </summary>
        [Column(@"Name", Order = 2, TypeName = "nvarchar")]
        [Required]
        [MaxLength(50)]
        [StringLength(50)]
        [Display(Name = "Name")]
        public string Name { get; set; }
    }

}
// </auto-generated>
