// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutPageSectionRow.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   The layout page section row.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The layout page section row.
    /// </summary>
    [TableType("[dbo].[LayoutPageSectionTableType]")]
    public partial class LayoutPageSectionRow : ICompositeEntity
    {
        /// <summary>
        /// The layout page section relations.
        /// </summary>
        private static readonly Lazy<IEnumerable<IEntityRelation>> LayoutPageSectionRelations =
            new Lazy<IEnumerable<IEntityRelation>>(
                () =>
                    new TransactSqlFromClause<LayoutPageSectionRow>().InnerJoin(row => row.LayoutPageId, row => row.LayoutPage.LayoutPageId)
                        .InnerJoin(row => row.LayoutSectionId, row => row.LayoutSection.LayoutSectionId)
                        .InnerJoin(row => row.LayoutPage.FormLayoutId, row => row.LayoutPage.FormLayout.FormLayoutId)
                        .Relations);

        /// <summary>
        /// Gets or sets the layout page.
        /// </summary>
        [Relation]
        public LayoutPageRow LayoutPage { get; set; }

        /// <summary>
        /// Gets or sets the layout section.
        /// </summary>
        [Relation]
        public LayoutSectionRow LayoutSection { get; set; }

        /// <summary>
        /// Gets the entity relations.
        /// </summary>
        public IEnumerable<IEntityRelation> EntityRelations => LayoutPageSectionRelations.Value;
    }
}
