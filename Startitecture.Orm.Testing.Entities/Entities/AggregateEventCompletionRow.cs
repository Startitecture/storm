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
    /// The AggregateEventCompletion entity POCO.
    /// </summary>
    [Table("AggregateEventCompletion", Schema = "dbo")]
    [System.CodeDom.Compiler.GeneratedCode("storm.Reverse.POCO.Generator", "1.0.0")]
    public partial class AggregateEventCompletionRow : EntityBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateEventCompletionRow"/> class.
        /// </summary>
        public AggregateEventCompletionRow()
        {
        }

        /// <summary>
        /// Gets or sets the AggregateEventCompletionId (Primary key)
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(@"AggregateEventCompletionId", Order = 1, TypeName = "bigint")]
        [Index(@"PK_AggregateEventCompletion", 1, IsUnique = true, IsClustered = true)]
        [Required]
        [Key]
        [Display(Name = "Aggregate event completion ID")]
        [ForeignKey("AggregateEventStart")]
        public long AggregateEventCompletionId { get; set; }

        /// <summary>
        /// Gets or sets the EndTime
        /// </summary>
        [Column(@"EndTime", Order = 2, TypeName = "datetimeoffset")]
        [Required]
        [Display(Name = "End time")]
        public System.DateTimeOffset EndTime { get; set; }

        /// <summary>
        /// Gets or sets the Succeeded
        /// </summary>
        [Column(@"Succeeded", Order = 3, TypeName = "bit")]
        [Required]
        [Display(Name = "Succeeded")]
        public bool Succeeded { get; set; }

        /// <summary>
        /// Gets or sets the Result
        /// </summary>
        [Column(@"Result", Order = 4, TypeName = "nvarchar(max)")]
        [Required]
        [Display(Name = "Result")]
        public string Result { get; set; }
    }

}
// </auto-generated>
