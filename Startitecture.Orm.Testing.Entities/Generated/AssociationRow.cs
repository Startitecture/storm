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
    /// The Association entity POCO.
    /// </summary>
    [Table("Association", Schema = "dbo")]
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.34.1.0")]
    public partial class AssociationRow : TransactionItemBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssociationRow"/> class.
        /// </summary>
        public AssociationRow()
        {
        }

        /// <summary>
        /// Gets or sets the OtherAggregateId (Primary key)
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(@"OtherAggregateId", Order = 1, TypeName = "int")]
        [Index(@"PK_Association", 1, IsUnique = true, IsClustered = true)]
        [Index(@"UK_AggregateLink_DomainAggregateId", 1, IsUnique = true, IsClustered = false)]
        [Required]
        [Key]
        [Display(Name = "Other aggregate ID")]
        [ForeignKey("OtherAggregate")]
        public int OtherAggregateId { get; set; }

        /// <summary>
        /// Gets or sets the DomainAggregateId
        /// </summary>
        [Column(@"DomainAggregateId", Order = 2, TypeName = "int")]
        [Required]
        [Display(Name = "Domain aggregate ID")]
        public int DomainAggregateId { get; set; }
    }

}
// </auto-generated>
