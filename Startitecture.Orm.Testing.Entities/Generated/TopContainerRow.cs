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
    /// The TopContainer entity POCO.
    /// </summary>
    [Table("TopContainer", Schema = "dbo")]
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.34.1.0")]
    public partial class TopContainerRow: TransactionItemBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TopContainerRow"/> class.
        /// </summary>
        public TopContainerRow()
        {
        }

        /// <summary>
        /// Gets or sets the TopContainerId (Primary key)
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(@"TopContainerId", Order = 1, TypeName = "int")]
        [Index(@"PK_TopContainer", 1, IsUnique = true, IsClustered = true)]
        [Required]
        [Key]
        [Display(Name = "Top container ID")]
        public int TopContainerId { get; set; }

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
