// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The fake raised data row.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Startitecture.Orm.Common;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Schema;

    /// <summary>
    /// The fake raised data row.
    /// </summary>
    [Table("FakeData", Schema = "dbo")]
    public class DataRow : ITransactionContext
    {
        /// <summary>
        /// Gets or sets the fake related row.
        /// </summary>
        [Relation]
        public FakeRelatedRow Related { get; set; }

        /// <summary>
        /// Gets or sets the other alias.
        /// </summary>
        [Relation]
        public FakeRelatedRow OtherAlias { get; set; }

        /// <summary>
        /// Gets or sets the related dependency.
        /// </summary>
        [Relation]
        public DependencyRow RelatedDependency { get; set; }

        /// <summary>
        /// Gets or sets the related alias.
        /// </summary>
        [Relation]
        public FakeRelatedRow RelatedAlias { get; set; }

        /// <summary>
        /// Gets or sets the related dependency.
        /// </summary>
        [Relation]
        public DependencyRow DependencyEntity { get; set; }

        /// <summary>
        /// Gets or sets the fake sub data id.
        /// </summary>
        [RelatedEntity(typeof(SubDataRow))]
        public int? ParentFakeDataId { get; set; }

        /// <summary>
        /// Gets or sets the fake data id.
        /// </summary>
        [Column("FakeRowId")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FakeDataId { get; set; }

        /// <summary>
        /// Gets or sets the normal column.
        /// </summary>
        [Column]
        public string NormalColumn { get; set; }

        /// <summary>
        /// Gets or sets the nullable column.
        /// </summary>
        [Column]
        public string NullableColumn { get; set; }

        /// <summary>
        /// Gets or sets the value column.
        /// </summary>
        [Column]
        public int ValueColumn { get; set; }

        /// <summary>
        /// Gets or sets the another value column.
        /// </summary>
        [Column]
        public int AnotherValueColumn { get; set; }

        /// <summary>
        /// Gets or sets the another column.
        /// </summary>
        [Column]
        public string AnotherColumn { get; set; }

        /// <summary>
        /// Gets or sets the nullable value column.
        /// </summary>
        [Column]
        public int? NullableValueColumn { get; set; }

        /// <summary>
        /// Gets the transaction provider.
        /// </summary>
        public IRepositoryProvider TransactionProvider { get; private set; }

        /// <summary>
        /// The set transaction provider.
        /// </summary>
        /// <param name="repositoryProvider">
        /// The repository provider.
        /// </param>
        public void SetTransactionProvider(IRepositoryProvider repositoryProvider)
        {
            this.TransactionProvider = repositoryProvider;
        }
    }
}
