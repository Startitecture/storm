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

namespace Startitecture.Platform.Entities
{
    using Startitecture.Orm.Schema;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The ResourceClassification entity POCO.
    /// </summary>
    [Table("ResourceClassification", Schema = "dbo")]
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.34.1.0")]
    public partial class ResourceClassificationRow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceClassificationRow"/> class.
        /// </summary>
        public ResourceClassificationRow()
        {
        }

        /// <summary>
        /// Gets or sets the ResourceClassificationId (Primary key)
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [System.ComponentModel.DataAnnotations.Schema.Column(@"ResourceClassificationId", Order = 1, TypeName = "int")]
        [Index(@"PK_ResourceClassification", 1, IsUnique = true, IsClustered = true)]
        [Required]
        [Key]
        [Display(Name = "Resource classification ID")]
        public int ResourceClassificationId { get; set; }

        /// <summary>
        /// Gets or sets the Name (length: 50)
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(@"Name", Order = 2, TypeName = "nvarchar")]
        [Required]
        [MaxLength(50)]
        [StringLength(50)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Description (length: 250)
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(@"Description", Order = 3, TypeName = "nvarchar")]
        [MaxLength(250)]
        [StringLength(250)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the IsActive
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(@"IsActive", Order = 4, TypeName = "bit")]
        [Required]
        [Display(Name = "Is active")]
        public bool IsActive { get; set; }
    }

}
// </auto-generated>
