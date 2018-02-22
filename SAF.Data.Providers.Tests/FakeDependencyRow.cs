// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeDependencyRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Repository.Tests.Models
{
    using Startitecture.Orm.Schema;

    /// <summary>
    /// The fake dependency row.
    /// </summary>
    [TableName("FakeDependencyEntity")]
    [PrimaryKey("FakeDependencyEntityId")]
    public class FakeDependencyRow : TransactionItemBase
    {
        /// <summary>
        /// Gets or sets the fake dependency entity id.
        /// </summary>
        [Column]
        public long FakeDependencyEntityId { get; set; }

        /// <summary>
        /// Gets or sets the fake complex entity id.
        /// </summary>
        [Column]
        public int FakeComplexEntityId { get; set; }

        /// <summary>
        /// Gets or sets the unique name.
        /// </summary>
        [Column]
        public string UniqueName { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [Column]
        public string Description { get; set; }
    }
}
