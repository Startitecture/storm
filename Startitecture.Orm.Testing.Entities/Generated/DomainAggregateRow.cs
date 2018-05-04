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
    /// The DomainAggregate entity POCO.
    /// </summary>
    [Table("DomainAggregate", Schema = "dbo")]
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.34.1.0")]
    public partial class DomainAggregateRow : TransactionItemBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DomainAggregateRow"/> class.
        /// </summary>
        public DomainAggregateRow()
        {
        }

        /// <summary>
        /// Gets or sets the DomainAggregateId (Primary key)
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(@"DomainAggregateId", Order = 1, TypeName = "int")]
        [Index(@"PK_DomainAggregate", 1, IsUnique = true, IsClustered = true)]
        [Required]
        [Key]
        [Display(Name = "Domain aggregate ID")]
        public int DomainAggregateId { get; set; }

        /// <summary>
        /// Gets or sets the SubContainerId
        /// </summary>
        [Column(@"SubContainerId", Order = 2, TypeName = "int")]
        [Required]
        [Display(Name = "Sub container ID")]
        public int SubContainerId { get; set; }

        /// <summary>
        /// Gets or sets the TemplateId
        /// </summary>
        [Column(@"TemplateId", Order = 3, TypeName = "int")]
        [Required]
        [Display(Name = "Template ID")]
        public int TemplateId { get; set; }

        /// <summary>
        /// Gets or sets the CategoryAttributeId
        /// </summary>
        [Column(@"CategoryAttributeId", Order = 4, TypeName = "int")]
        [Required]
        [Display(Name = "Category attribute ID")]
        public int CategoryAttributeId { get; set; }

        /// <summary>
        /// Gets or sets the Name (length: 50)
        /// </summary>
        [Column(@"Name", Order = 5, TypeName = "nvarchar")]
        [Required]
        [MaxLength(50)]
        [StringLength(50)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Description
        /// </summary>
        [Column(@"Description", Order = 6, TypeName = "nvarchar(max)")]
        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the CreatedByDomainIdentityId
        /// </summary>
        [Column(@"CreatedByDomainIdentityId", Order = 7, TypeName = "int")]
        [Required]
        [Display(Name = "Created by domain identity ID")]
        public int CreatedByDomainIdentityId { get; set; }

        /// <summary>
        /// Gets or sets the CreatedTime
        /// </summary>
        [Column(@"CreatedTime", Order = 8, TypeName = "datetimeoffset")]
        [Required]
        [Display(Name = "Created time")]
        public System.DateTimeOffset CreatedTime { get; set; }

        /// <summary>
        /// Gets or sets the LastModifiedByDomainIdentityId
        /// </summary>
        [Column(@"LastModifiedByDomainIdentityId", Order = 9, TypeName = "int")]
        [Required]
        [Display(Name = "Last modified by domain identity ID")]
        public int LastModifiedByDomainIdentityId { get; set; }

        /// <summary>
        /// Gets or sets the LastModifiedTime
        /// </summary>
        [Column(@"LastModifiedTime", Order = 10, TypeName = "datetimeoffset")]
        [Required]
        [Display(Name = "Last modified time")]
        public System.DateTimeOffset LastModifiedTime { get; set; }
    }

}
// </auto-generated>
