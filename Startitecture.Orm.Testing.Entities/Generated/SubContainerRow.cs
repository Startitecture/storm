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
    using Startitecture.Orm.Schema;

    /// <summary>
    /// The SubContainer entity POCO.
    /// </summary>
    [Table("SubContainer", Schema = "dbo")]
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.34.1.0")]
    public partial class SubContainerRow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubContainerRow"/> class.
        /// </summary>
        public SubContainerRow()
        {
        }

        /// <summary>
        /// Gets or sets the SubContainerId (Primary key)
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(@"SubContainerId", Order = 1, TypeName = "int")]
        [Index(@"PK_SubContainer", 1, IsUnique = true, IsClustered = true)]
        [Required]
        [Key]
        [Display(Name = "Sub container ID")]
        public int SubContainerId { get; set; }

        /// <summary>
        /// Gets or sets the TopContainerId
        /// </summary>
        [Column(@"TopContainerId", Order = 2, TypeName = "int")]
        [Required]
        [Display(Name = "Top container ID")]
        public int TopContainerId { get; set; }

        /// <summary>
        /// Gets or sets the Name (length: 50)
        /// </summary>
        [Column(@"Name", Order = 3, TypeName = "nvarchar")]
        [Required]
        [MaxLength(50)]
        [StringLength(50)]
        [Display(Name = "Name")]
        public string Name { get; set; }
    }

}
// </auto-generated>
