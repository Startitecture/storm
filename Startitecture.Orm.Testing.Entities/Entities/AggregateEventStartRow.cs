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
    using Startitecture.Orm.Mapper;
    using Startitecture.Orm.Schema;

    /// <summary>
    /// The AggregateEventStart entity POCO.
    /// </summary>
    [Table("AggregateEventStart", Schema = "dbo")]
    [System.CodeDom.Compiler.GeneratedCode("storm.Reverse.POCO.Generator", "1.0.0")]
    public partial class AggregateEventStartRow : EntityBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateEventStartRow"/> class.
        /// </summary>
        public AggregateEventStartRow()
        {
        }

        /// <summary>
        /// Gets or sets the AggregateEventStartId (Primary key)
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(@"AggregateEventStartId", Order = 1, TypeName = "bigint")]
        [Index(@"PK_AggregateEventStart", 1, IsUnique = true, IsClustered = true)]
        [Required]
        [Key]
        [Display(Name = "Aggregate event start ID")]
        public long AggregateEventStartId { get; set; }

        /// <summary>
        /// Gets or sets the DomainAggregateId
        /// </summary>
        [Column(@"DomainAggregateId", Order = 2, TypeName = "int")]
        [Required]
        [Display(Name = "Domain aggregate ID")]
        public int DomainAggregateId { get; set; }

        /// <summary>
        /// Gets or sets the GlobalIdentifier
        /// </summary>
        [Column(@"GlobalIdentifier", Order = 3, TypeName = "uniqueidentifier")]
        [Required]
        [Display(Name = "Global identifier")]
        public System.Guid GlobalIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the DomainIdentityId
        /// </summary>
        [Column(@"DomainIdentityId", Order = 4, TypeName = "int")]
        [Required]
        [Display(Name = "Domain identity ID")]
        public int DomainIdentityId { get; set; }

        /// <summary>
        /// Gets or sets the StartTime
        /// </summary>
        [Column(@"StartTime", Order = 5, TypeName = "datetimeoffset")]
        [Required]
        [Display(Name = "Start time")]
        public System.DateTimeOffset StartTime { get; set; }

        /// <summary>
        /// Gets or sets the EventName (length: 50)
        /// </summary>
        [Column(@"EventName", Order = 6, TypeName = "nvarchar")]
        [Required]
        [MaxLength(50)]
        [StringLength(50)]
        [Display(Name = "Event name")]
        public string EventName { get; set; }

        /// <summary>
        /// Gets or sets the EventDescription
        /// </summary>
        [Column(@"EventDescription", Order = 7, TypeName = "nvarchar(max)")]
        [Required]
        [Display(Name = "Event description")]
        public string EventDescription { get; set; }
    }

}
// </auto-generated>
