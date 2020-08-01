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
    /// The Child entity POCO.
    /// </summary>
    [Table("Child", Schema = "dbo")]
    [System.CodeDom.Compiler.GeneratedCode("storm.Reverse.POCO.Generator", "1.0.0")]
    public partial class ChildRow : EntityBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChildRow"/> class.
        /// </summary>
        public ChildRow()
        {
        }

        /// <summary>
        /// Gets or sets the ChildId (Primary key)
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(@"ChildId", Order = 1, TypeName = "int")]
        [Index(@"PK_Child", 1, IsUnique = true, IsClustered = true)]
        [Required]
        [Key]
        [Display(Name = "Child ID")]
        public int ChildId { get; set; }

        /// <summary>
        /// Gets or sets the DomainAggregateId
        /// </summary>
        [Column(@"DomainAggregateId", Order = 2, TypeName = "int")]
        [Required]
        [Display(Name = "Domain aggregate ID")]
        public int DomainAggregateId { get; set; }

        /// <summary>
        /// Gets or sets the Name (length: 50)
        /// </summary>
        [Column(@"Name", Order = 3, TypeName = "nvarchar")]
        [Required]
        [MaxLength(50)]
        [StringLength(50)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Value
        /// </summary>
        [Column(@"Value", Order = 4, TypeName = "money")]
        [Required]
        [Display(Name = "Value")]
        public decimal Value { get; set; }
    }

}
// </auto-generated>
