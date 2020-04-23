// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericSubmissionRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Testing.Entities
{
    using System.Collections.Generic;

    using Startitecture.Orm.Model;
    using Startitecture.Orm.Schema;

    /// <summary>
    /// The generic submission row.
    /// </summary>
    public partial class GenericSubmissionRow : IEntityAggregate
    {
        /// <summary>
        /// Gets or sets the submitted by.
        /// </summary>
        [Relation]
        public DomainIdentityRow SubmittedBy { get; set; }

        /// <inheritdoc />
        public IEnumerable<IEntityRelation> EntityRelations =>
            new EntityRelationSet<GenericSubmissionRow>().InnerJoin(row => row.SubmittedByDomainIdentifierId, row => row.SubmittedBy.DomainIdentityId)
                .Relations;
    }
}