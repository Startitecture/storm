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
    /// The GenericSubmissionValue entity POCO.
    /// </summary>
    [Table("GenericSubmissionValue", Schema = "dbo")]
    [System.CodeDom.Compiler.GeneratedCode("storm.Reverse.POCO.Generator", "1.0.0")]
    public partial class GenericSubmissionValueRow : EntityBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericSubmissionValueRow"/> class.
        /// </summary>
        public GenericSubmissionValueRow()
        {
        }

        /// <summary>
        /// Gets or sets the GenericSubmissionValueId (Primary key)
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(@"GenericSubmissionValueId", Order = 1, TypeName = "bigint")]
        [Index(@"PK_GenericSubmissionValue", 1, IsUnique = true, IsClustered = true)]
        [Required]
        [Key]
        [Display(Name = "Generic submission value ID")]
        [ForeignKey("FieldValue")]
        public long GenericSubmissionValueId { get; set; }

        /// <summary>
        /// Gets or sets the GenericSubmissionId
        /// </summary>
        [Column(@"GenericSubmissionId", Order = 2, TypeName = "int")]
        [Required]
        [Display(Name = "Generic submission ID")]
        public int GenericSubmissionId { get; set; }
    }

}
// </auto-generated>
