// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeRelatedRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Startitecture.Orm.Common;
    using Startitecture.Orm.Model;
    using Startitecture.Orm.Schema;

    /// <summary>
    /// The fake related row.
    /// </summary>
    [Table("Related", Schema = "someschema")]
    public class FakeRelatedRow : ITransactionContext
    {
        /// <summary>
        /// Gets or sets the related id.
        /// </summary>
        [Column]
        [Key]
        public int RelatedId { get; set; }

        /// <summary>
        /// Gets or sets the fake data id.
        /// </summary>
        [Column]
        public int FakeDataId { get; set; }

        /// <summary>
        /// Gets or sets the related property.
        /// </summary>
        [Column]
        public string RelatedProperty { get; set; }

        /// <summary>
        /// Gets the transaction provider.
        /// </summary>
        [Ignore]
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