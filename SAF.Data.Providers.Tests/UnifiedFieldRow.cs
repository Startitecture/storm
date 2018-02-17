// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnifiedFieldRow.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests
{
    using System;
    using System.Collections.Generic;

    using Startitecture.Orm.Model;
    using Startitecture.Orm.Query;
    using Startitecture.Orm.Sql;

    /// <summary>
    /// The unified field row.
    /// </summary>
    public partial class UnifiedFieldRow : ICompositeEntity
    {
        /// <summary>
        /// A lazy-loaded set of relations for the current row.
        /// </summary>
        private static readonly Lazy<IEnumerable<IEntityRelation>> LazyRelations =
            new Lazy<IEnumerable<IEntityRelation>>(
                () =>
                    new SqlFromClause<UnifiedFieldRow>().LeftJoin<UnifiedFieldCustomSourceRow>(
                            row => row.UnifiedFieldId,
                            row => row.UnifiedFieldId)
                        .LeftJoin<UnifiedFieldSystemSourceRow>(row => row.UnifiedFieldId, row => row.UnifiedFieldId)
                        .Relations);

        /// <summary>
        /// Gets or sets the custom field id.
        /// </summary>
        [RelatedEntity(typeof(UnifiedFieldCustomSourceRow))]
        public int? CustomFieldId { get; set; }

        /// <summary>
        /// Gets or sets the system field source id.
        /// </summary>
        [RelatedEntity(typeof(UnifiedFieldSystemSourceRow))]
        public int? SystemFieldSourceId { get; set; }

        /// <summary>
        /// Gets the entity relations.
        /// </summary>
        public IEnumerable<IEntityRelation> EntityRelations
        {
            get
            {
                return LazyRelations.Value;
            }
        }
    }
}