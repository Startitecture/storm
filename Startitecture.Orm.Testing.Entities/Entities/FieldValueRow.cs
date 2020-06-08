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
    /// The FieldValue entity POCO.
    /// </summary>
    [Table("FieldValue", Schema = "dbo")]
    [System.CodeDom.Compiler.GeneratedCode("storm.Reverse.POCO.Generator", "1.0.0")]
    public partial class FieldValueRow : EntityBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldValueRow"/> class.
        /// </summary>
        public FieldValueRow()
        {
        }

        /// <summary>
        /// Gets or sets the FieldValueId (Primary key)
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(@"FieldValueId", Order = 1, TypeName = "bigint")]
        [Index(@"PK_FieldValue", 1, IsUnique = true, IsClustered = true)]
        [Required]
        [Key]
        [Display(Name = "Field value ID")]
        public long FieldValueId { get; set; }

        /// <summary>
        /// Gets or sets the FieldId
        /// </summary>
        [Column(@"FieldId", Order = 2, TypeName = "int")]
        [Required]
        [Display(Name = "Field ID")]
        public int FieldId { get; set; }

        /// <summary>
        /// Gets or sets the LastModifiedByDomainIdentifierId
        /// </summary>
        [Column(@"LastModifiedByDomainIdentifierId", Order = 3, TypeName = "int")]
        [Required]
        [Display(Name = "Last modified by domain identifier ID")]
        public int LastModifiedByDomainIdentifierId { get; set; }

        /// <summary>
        /// Gets or sets the LastModifiedTime
        /// </summary>
        [Column(@"LastModifiedTime", Order = 4, TypeName = "datetimeoffset")]
        [Required]
        [Display(Name = "Last modified time")]
        public System.DateTimeOffset LastModifiedTime { get; set; }
    }

}
// </auto-generated>
