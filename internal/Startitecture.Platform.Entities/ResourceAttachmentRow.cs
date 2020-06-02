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
    /// The ResourceAttachment entity POCO.
    /// </summary>
    [Table("ResourceAttachment", Schema = "dbo")]
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.34.1.0")]
    public partial class ResourceAttachmentRow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceAttachmentRow"/> class.
        /// </summary>
        public ResourceAttachmentRow()
        {
        }

        /// <summary>
        /// Gets or sets the ResourceAttahcmentId (Primary key)
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [System.ComponentModel.DataAnnotations.Schema.Column(@"ResourceAttahcmentId", Order = 1, TypeName = "bigint")]
        [Index(@"PK_ResourceAttachment", 1, IsUnique = true, IsClustered = true)]
        [Required]
        [Key]
        [Display(Name = "Resource attahcment ID")]
        public long ResourceAttahcmentId { get; set; }

        /// <summary>
        /// Gets or sets the ExternalResourceId
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.Column(@"ExternalResourceId", Order = 2, TypeName = "int")]
        [Required]
        [Display(Name = "External resource ID")]
        public int ExternalResourceId { get; set; }

        // Foreign keys

        /// <summary>
        /// Gets or sets the Parent ExternalResource pointed by [ResourceAttachment].([ExternalResourceId]) (FK_ResourceAttachment_ExternalResource)
        /// </summary>
        [ForeignKey("ExternalResourceId")] public ExternalResourceRow ExternalResource { get; set; } // FK_ResourceAttachment_ExternalResource
    }

}
// </auto-generated>