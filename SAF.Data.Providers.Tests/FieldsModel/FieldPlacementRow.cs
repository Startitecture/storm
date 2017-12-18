// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldPlacementRow.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The field placement row.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The field placement row.
    /// </summary>
    public partial class FieldPlacementRow : ICompositeEntity
    {
        /// <summary>
        /// The field placement relations.
        /// </summary>
        private static readonly Lazy<IEnumerable<IEntityRelation>> FieldPlacementRelations =
            new Lazy<IEnumerable<IEntityRelation>>(
                () =>
                    new TransactSqlFromClause<FieldPlacementRow>()
                        .InnerJoin(row => row.LayoutSectionId, row => row.LayoutSection.LayoutSectionId)
                        .InnerJoin(row => row.UnifiedFieldId, row => row.UnifiedField.UnifiedFieldId)
                        .Relations);

        /// <summary>
        /// Gets or sets the layout section.
        /// </summary>
        [Relation]
        public LayoutSectionRow LayoutSection { get; set; }

        /// <summary>
        /// Gets or sets the unified field.
        /// </summary>
        [Relation]
        public UnifiedFieldRow UnifiedField { get; set; }

        /// <summary>
        /// Gets the entity relations.
        /// </summary>
        public IEnumerable<IEntityRelation> EntityRelations
        {
            get
            {
                return FieldPlacementRelations.Value;
            }
        }
    }
}
