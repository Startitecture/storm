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
// TargetFrameworkVersion = 4.8
#pragma warning disable 1591    //  Ignore "Missing XML Comment" warning

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Startitecture.Orm.Testing.Entities
{
    using Startitecture.Orm.Mapper;
    using Startitecture.Orm.Schema;

    /// <summary>
    /// The __RefactorLog entity POCO.
    /// </summary>
    /// <summary>
    /// refactoring log
    /// </summary>
    [Table("__RefactorLog", Schema = "dbo")]
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.34.1.0")]
    public partial class RefactorLogRow : TransactionItemBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RefactorLogRow"/> class.
        /// </summary>
        public RefactorLogRow()
        {
        }

        /// <summary>
        /// Gets or sets the OperationKey (Primary key)
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(@"OperationKey", Order = 1, TypeName = "uniqueidentifier")]
        [Index(@"PK____Refact__D3AEFFDBDA866624", 1, IsUnique = true, IsClustered = true)]
        [Required]
        [Key]
        [Display(Name = "Operation key")]
        public System.Guid OperationKey { get; set; }
    }

}
// </auto-generated>
