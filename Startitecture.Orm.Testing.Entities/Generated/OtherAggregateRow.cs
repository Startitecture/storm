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
    /// The OtherAggregate entity POCO.
    /// </summary>
    [Table("OtherAggregate", Schema = "dbo")]
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.34.1.0")]
    public partial class OtherAggregateRow : EntityBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OtherAggregateRow"/> class.
        /// </summary>
        public OtherAggregateRow()
        {
        }

        /// <summary>
        /// Gets or sets the OtherAggregateId (Primary key)
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(@"OtherAggregateId", Order = 1, TypeName = "int")]
        [Index(@"PK_OtherAggregate", 1, IsUnique = true, IsClustered = true)]
        [Required]
        [Key]
        [Display(Name = "Other aggregate ID")]
        public int OtherAggregateId { get; set; }

        /// <summary>
        /// Gets or sets the Name (length: 50)
        /// </summary>
        [Column(@"Name", Order = 2, TypeName = "nvarchar")]
        [Required]
        [MaxLength(50)]
        [StringLength(50)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the AggregateOptionTypeId
        /// </summary>
        [Column(@"AggregateOptionTypeId", Order = 3, TypeName = "int")]
        [Required]
        [Display(Name = "Aggregate option type ID")]
        public int AggregateOptionTypeId { get; set; }
    }

}
// </auto-generated>
